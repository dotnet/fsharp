const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs/promises");
const path = require("node:path");
const os = require("node:os");
const { createHmac } = require("node:crypto");
const { spawnSync } = require("node:child_process");
const { fixture: reproductionFixture, projectBuild } = require("./fixtures/reproduction.cjs");
const { harness } = require("./fixtures/checkpoint.cjs");
const f = require("./fixtures/contracts.cjs");
const { runReproduction } = require("./repro.cjs");
const { fingerprintInput: fingerprint, classifyAttempts } = require("./model.cjs");
const state = require("./state.cjs");
const { formObservation, createObservationCollector, commitCollectedObservation } = require("./collector.cjs");
const { createFileEvidenceStore } = require("./evidence-store.cjs");
const { executionFileNames } = require("./worker-output.cjs");
const { executionEvidence, readExecutionOutput } = require("./worker.cjs");
const { image } = require("./container.cjs");

const jsonFile = (name, value) => {
    const bytes = Buffer.from(JSON.stringify(value));
    return { path: name, bytes, size: bytes.length, sha256: f.digest(bytes) };
};

async function fixture(t, { kind = "source", outputs, tfms = ["net10.0"], project = false, compile = false,
    setup, buildSdk = "10.0.100", producer, sampleRuntime = "10.0.0", packageCoreSha } = {}) {
    const sample = await reproductionFixture(t, { kind, tfms });
    if (kind === "released") {
        const pkg = sample.product.files.find(file => file.role === "corePackage");
        sample.toolchain.files.core.package = { path: pkg.path, sha256: pkg.sha256,
            id: "FSharp.Core", version: sample.product.corePackageVersion };
    }
    if (sampleRuntime !== "10.0.0") {
        const runtime = await sample.file(`dotnet/shared/Microsoft.NETCore.App/${sampleRuntime}/System.Private.CoreLib.dll`);
        sample.toolchain.runtimes.push({ name: "Microsoft.NETCore.App", version: sampleRuntime,
            path: path.dirname(path.dirname(runtime.path)), sha256: runtime.sha256 });
        const archive = f.archiveReceipt({ url: `https://builds.dotnet.microsoft.com/dotnet/Runtime/${sampleRuntime}/dotnet-runtime-${sampleRuntime}-win-x64.zip`,
            name: "runtime.zip", rid: "win-x64", architecture: "x64", provenance: { kind: "publisher-checksum" } },
        (await sample.file("runtime.zip", "runtime archive")).path, "runtime archive");
        archive.sha512 = archive.actualSha512;
        sample.toolchain.installations.push({ product: "dotnet", version: sampleRuntime, archive });
        const config = Buffer.from(JSON.stringify({ runtimeOptions: {
            framework: { name: "Microsoft.NETCore.App", version: sampleRuntime },
        } }));
        sample.toolchain.requested.runtimeRequirements.push({ name: "Microsoft.NETCore.App", version: sampleRuntime,
            role: "sample", archiveUrl: archive.url, config: {
                ...await sample.file("sample.runtimeconfig.json", config), bytesBase64: config.toString("base64"),
            } });
    }
    sample.sampleRuntimes = Object.fromEntries(tfms.map(tfm => [tfm, { name: "Microsoft.NETCore.App", version: sampleRuntime }]));
    sample.product.files.find(f => f.role === "compiler").runtimeConfig =
        sample.product.files.find(f => f.role === "fsi").runtimeConfig;
    if (project) {
        const file = await sample.file("source/App.fsproj", "<Project />");
        sample.manifest.kind = "project";
        sample.manifest.sources = [{ path: "App.fsproj", sha256: file.sha256 }];
        sample.product.configuration = "Release";
        const roles = Object.fromEntries(sample.product.files.map(file => [file.role, file]));
        roles.compiler.runtimeConfig = roles.fsi.runtimeConfig;
        for (const cell of sample.manifest.matrix) {
            cell.project = "App.fsproj";
            cell.command = ["run-project", cell.project];
        }
    }
    if (compile) sample.manifest.assertions = {
        pass: { exitCode: 0, stdout: "", stderr: "" },
        failure: { exitCode: 1, stdout: "", stderr: `${project ? "repro.fs" : "repro.fsx"}(3,5,3,9): error FS0001: frozen message\n` },
    };
    let producerRecord;
    if (producer) {
        const dll = await sample.file("source/producer/Old.dll", "frozen metadata");
        producerRecord = { role: "immutable-producer", compiler: { version: "8.0.100", sha256: f.digest("old compiler") },
            tfm: "netstandard2.0", flags: [], packages: [], dll: { path: "producer/Old.dll", sha256: dll.sha256,
                resources: [{ name: `FSharpSignature${producer === "package" ? "Compressed" : ""}Data.Old`, sha256: f.digest("metadata") }] } };
        if (producer === "package") {
            const pkg = await sample.file("source/producer/Old.1.0.0.nupkg", "frozen package");
            producerRecord.package = { path: "producer/Old.1.0.0.nupkg", sha256: pkg.sha256, id: "Old", version: "1.0.0" };
            sample.manifest.packages.push({ id: "Old", version: "1.0.0", sha256: pkg.sha256 });
        }
        const declaration = await sample.file("source/old.producer.json", JSON.stringify(producerRecord));
        sample.manifest.sources.push({ path: "old.producer.json", sha256: declaration.sha256 },
            ...[producerRecord.dll, ...(producerRecord.package ? [producerRecord.package] : [])]
                .map(({ path, sha256 }) => ({ path, sha256 })));
        sample.manifest.producer = { name: "Old", version: "1.0.0", sha256: dll.sha256 };
    }
    f.seal(sample.manifest);
    const h = harness(), c = h.context;
    c.manifest = sample.manifest;
    c.authorization.manifestDigest = fingerprint(c.manifest);
    c.authorization.inputDigest = c.manifest.approvedInputDigest;
    c.stableSdk = { ...c.stableSdk, archives: [{
        os: "windows", architecture: "x64", sha256: sample.toolchain.archive.sha256,
    }] };
    c.pinnedMainSha = sample.product.sourceSha;
    const imageDigest = image.split(":").at(-1);
    c.environmentFingerprint = fingerprint(Object.fromEntries(c.manifest.matrix.map(cell =>
        [cell.id, { os: cell.os, architecture: cell.architecture, imageDigest }])));
    await h.start();
    const reservation = await state.reserveWork(h.transport, c, {
        generation: 0, attemptId: "source-evidence-1",
        request: { kind, target: kind === "source" ? { sourceSha: c.pinnedMainSha } : { sdk: c.stableSdk },
            phase: kind === "source" ? "main" : "stable", endpoint: kind === "source" ? "main" : "stable" },
    });
    const started = await state.beginWork(h.transport, c, { generation: reservation.generation, attemptId: reservation.id });
    sample.product.buildInstanceId = reservation.id;
    if (kind === "released") delete sample.product.sourceSha;
    sample.workDirectory = path.join(sample.root, "executions");
    sample.helper = { executable: "fixture-helper", dll: "Evidence.dll", hook: "StartupHook.dll", runtimeVersion: "10.0.0" };
    let repetition = 0, projectEvidence;
    const reproduction = await runReproduction(sample, { execute: async (exe, args, settings) => {
        const ok = { status: "completed", complete: true, exitCode: 0, stdout: "", stderr: "" };
        if (exe === sample.helper.executable && args.includes("binlog"))
            return { ...ok, stdout: JSON.stringify(projectEvidence.structured) };
        if (exe === sample.helper.executable) return { ...ok, stdout: JSON.stringify(
            args.slice(args.indexOf("inspect") + 1).map(location => {
                if (producerRecord && location === path.join(sample.sourceDirectory, producerRecord.dll.path))
                    return { ...producerRecord.dll, path: location };
                const record = sample.product.files.find(p => p.path === location) ??
                    { ...sample.product.files.find(p => p.role === "core"), ...projectEvidence.core };
                return record.role === "corePackage" ? { ...record, id: "FSharp.Core",
                    version: sample.product.corePackageVersion, entries: [{
                        path: "lib/netstandard2.0/FSharp.Core.dll",
                        sha256: packageCoreSha ?? sample.product.files.find(p => p.role === "core").sha256,
                    }] } : record;
            })) };
        if (args[0] === "build") {
            if (setup === "restore") {
                projectEvidence = { structured: { cells: [], diagnostics: [
                    { severity: "error", code: "NU1301", message: "Restore unavailable", file: "NuGet.targets" },
                ] } };
                await fs.writeFile(args.find(a => a.startsWith("-bl:")).slice(4), "fixture restore failure");
                return { ...ok, exitCode: 1, stderr: "error NU1301: Restore unavailable" };
            }
            const cell = sample.manifest.matrix.find(c => args.includes(`-p:TargetFramework=${c.tfm}`));
            projectEvidence = await projectBuild({ product: sample.product, toolchain: sample.toolchain, cells: [cell], settings });
            if (producerRecord) for (const row of projectEvidence.structured.cells) {
                const dll = path.join(settings.cwd, producerRecord.dll.path);
                row.fscTasks[0].references.push(dll);
                await fs.copyFile(dll, path.join(path.dirname(row.targetPath), "Old.dll"));
            }
            await fs.writeFile(args.find(a => a.startsWith("-bl:")).slice(4), "fixture binlog");
            if (compile) {
                projectEvidence.structured.diagnostics = [{ severity: "error", code: "FS0001", message: "frozen message",
                    file: path.join(settings.cwd, "repro.fs"), line: 3, column: 5, endLine: 3, endColumn: 9 }];
                return { ...ok, exitCode: 1, stderr: "MSBuild diagnostic output" };
            }
            return ok;
        }
        const core = sample.product.files.find(p => p.role === "core");
        await fs.writeFile(settings.env.REPRO_CORE_EVIDENCE, JSON.stringify({
            ...core, ...(project ? projectEvidence.core : {}), complete: true, assemblyName: "FSharp.Core", runtimeVersion: args[2],
            runtimePath: path.join(sample.toolchain.runtimes[0].path, args[2]), architecture: "x64",
        }));
        return { ...ok, ...(setup === "timeout" ? { status: "timeout", complete: false, exitCode: null } :
            outputs?.[repetition++] ?? (compile ? sample.manifest.assertions.failure : sample.manifest.assertions.pass)) };
    } });
    assert.ok(reproduction.identities, JSON.stringify(reproduction));
    const identities = JSON.parse(await fs.readFile(reproduction.identities.path));
    // The real helper enriches package identities before reproduction; the fixture helper supplies the same record.
    const pkg = sample.product.files.find(p => p.role === "corePackage");
    Object.assign(pkg, { id: "FSharp.Core", version: sample.product.corePackageVersion });
    Object.assign(identities.product.files.find(p => p.role === "corePackage"), pkg);
    reproduction.productFingerprint = fingerprint(identities.product);
    for (const cell of reproduction.cells) for (const record of cell.evidence)
        record.productFingerprint = reproduction.productFingerprint;
    const acquisition = structuredClone(sample.toolchain);
    const build = structuredClone(acquisition);
    build.installations = build.installations.filter(i => i.product === "sdk");
    build.runtimes = build.runtimes.filter(r => r.version === "10.0.0");
    build.root = build.requested.installationRoot = path.join(sample.root, "build-dotnet");
    build.hostPath = path.join(build.root, "dotnet.exe");
    build.sdkVersion = build.requested.sdkVersion = build.requested.toolsVersion = build.sdks[0].version = buildSdk;
    build.sdks[0].path = path.join(build.root, "sdk");
    build.sdkBasePath = path.join(build.sdks[0].path, buildSdk);
    for (const file of Object.values(build.files))
        file.path = path.join(build.sdkBasePath, "FSharp", path.basename(file.path));
    for (const runtime of build.runtimes) runtime.path = path.join(build.root, "shared", runtime.name);
    build.archive.url = build.archive.url.replaceAll("10.0.100", buildSdk);
    build.archive = f.archiveReceipt(build.archive, (await sample.file("build-sdk.zip", `archive-${buildSdk}`)).path, `archive-${buildSdk}`);
    build.archive.sha512 = build.archive.actualSha512;
    build.installations[0].archive = build.archive;
    build.installations[0].version = buildSdk;
    build.requested.archive = build.archive;
    const global = { sdk: { version: build.sdkVersion }, tools: { dotnet: build.sdkVersion } };
    const globalBytes = Buffer.from(JSON.stringify(global));
    Object.assign(build.requested, { kind: "source", sourceSha: c.pinnedMainSha,
        globalJson: { path: path.join(sample.product.root, "global.json"), sha256: f.digest(globalBytes) },
        runtimeRequirements: [],
    });
    build.runtimeExecutions = [];
    const source = { status: "tested", outcome: reproduction.outcome, reproduction,
        actualSourceSha: c.pinnedMainSha, buildInstanceId: reservation.id,
        globalJson: { before: f.digest(globalBytes), after: f.digest(globalBytes) },
        product: sample.product, toolchain: { acquisitionDigest: fingerprint(build), requested: build.requested, archive: build.archive },
        commands: ["product-build", "pack"].map(operation => ({ stage: "build", status: "completed",
            exitCode: 0, executable: build.hostPath, args: [operation] })),
        evidence: [{ path: `${reservation.id}/acquisition.json`, sha256: f.digest(JSON.stringify(build)) }],
    };
    const identitiesFile = jsonFile("identities.json", identities);
    reproduction.identities.sha256 = identitiesFile.sha256;
    const unit = {
        schemaVersion: 1, repository: c.repository, issueNumber: c.issueNumber,
        buildInstanceId: reservation.id, epoch: reservation.epoch, deadline: reservation.deadline,
        request: reservation.request, manifest: c.manifest, platform: sample.toolchain.requested.platform,
        stableSdk: c.stableSdk, resolved: sample.toolchain.requested,
        policyDigest: fingerprint(c.policy), policyRevision: c.policyRevision, workflowRevision: c.workflowRevision,
        environmentFingerprint: c.environmentFingerprint,
    };
    const launch = { launchId: f.digest("outer launch"), unitDigest: fingerprint(unit), imageDigest };
    const result = kind === "source" ? source : reproduction;
    const runtimeConfig = await fs.readFile(sample.product.files.find(f => f.role === "fsi").runtimeConfig.path);
    const sourceCommands = kind === "source" ? source.commands.map(command => ({ ...command,
        result: { status: "completed", complete: true, exitCode: 0, stdout: "", stderr: "" },
    })) : [];
    const executionCommands = [...sourceCommands, ...reproduction.cells.flatMap(cell =>
        cell.evidence.flatMap(record => record.commands))].map(({ executable, args, result }) =>
        ({ lane: "historical", executable, args, result }));
    const files = [
        jsonFile("result.json", { schemaVersion: 1, ...launch, result }),
        identitiesFile, jsonFile("acquisition.json", acquisition),
        jsonFile("commands.json", { source: sourceCommands, execution: executionCommands }),
        ...(kind === "source" ? [jsonFile("build-acquisition.json", build), {
            path: "global.json", bytes: globalBytes, size: globalBytes.length, sha256: f.digest(globalBytes),
        }, ...["fsi", "compiler"].map(role => ({ path: `${role}.runtimeconfig.json`,
            bytes: runtimeConfig, size: runtimeConfig.length, sha256: f.digest(runtimeConfig) }))] : []),
    ];
    return { h, context: c, reservation, started, sample, result, unit, launch, files, acquisition, build, setup };
}

for (const kind of ["released", "source"]) test(`collector distinguishes bundled and packaged Core for ${kind}`, async t => {
    const data = await fixture(t, { kind, packageCoreSha: f.digest("publisher package asset distinct from bundled FSI Core") });
    if (kind === "released") assert.equal(formObservation(data, data.reservation, data.context).outcome.outcome, "pass");
    else assert.throws(() => formObservation(data, data.reservation, data.context), /Core package inventory/);
});

async function retainedFixture(t, options = {}) {
    const data = await fixture(t, options);
    const root = await fs.realpath(await fs.mkdtemp(path.join(os.tmpdir(), "collected-evidence-")));
    t.after(() => fs.rm(root, { recursive: true, force: true }));
    const evidenceDirectory = path.join(root, "launches");
    const directory = path.join(evidenceDirectory, data.reservation.id);
    await fs.mkdir(directory, { recursive: true });
    const workerEvidence = path.join(data.sample.root, "evidence");
    const buildDirectory = path.join(workerEvidence, data.reservation.id);
    await fs.mkdir(buildDirectory, { recursive: true });
    const reproduction = data.unit.request.kind === "source" ? data.result.reproduction : data.result;
    await fs.writeFile(reproduction.identities.path, data.files.find(f => f.path === "identities.json").bytes);
    let acquisition = { status: "acquired", identity: data.acquisition };
    if (["acquisition", "build-acquisition", "missing-acquisitions", "reproduction-setup"].includes(options.setup)) {
        const failure = { status: "untestable", outcome: {
            schemaVersion: 1, outcome: "untestable", reason: "unavailable-sdk",
        }, error: { stage: "sdk", code: "archive-unavailable", message: "Exact SDK unavailable" } };
        if (options.setup === "reproduction-setup") {
            const failed = await runReproduction({ ...data.sample, sourceDirectory: path.join(data.sample.root, "absent") });
            assert.equal(failed.outcome.outcome, "untestable");
            data.result = data.unit.request.kind === "source"
                ? { ...data.result, reproduction: failed, outcome: failed.outcome } : failed;
        } else {
            data.result = failure;
            if (options.setup !== "build-acquisition") acquisition = options.setup === "acquisition" ? failure : null;
            if (options.setup === "build-acquisition")
                await fs.writeFile(path.join(buildDirectory, "acquisition.json"), JSON.stringify(failure));
        }
    }
    if (data.unit.request.kind === "source" && data.result.reproduction) {
        await fs.writeFile(path.join(buildDirectory, "acquisition.json"), JSON.stringify({ status: "acquired", identity: data.build }));
        await fs.writeFile(data.build.requested.globalJson.path, data.files.find(f => f.path === "global.json").bytes);
        await fs.writeFile(path.join(buildDirectory, "declarations.json"), JSON.stringify({ status: "resolved", toolchain: data.build.requested }));
        for (const [i, command] of data.result.commands.entries())
            await fs.writeFile(path.join(buildDirectory, `command-${i + 1}.json`), JSON.stringify({
                ...command, result: { status: "completed", complete: true, exitCode: 0, stdout: "", stderr: "" },
            }));
    }
    const exported = await executionEvidence(data.unit, data.launch, data.result, {
        acquisition, evidenceDirectory: workerEvidence, scratchDirectory: data.sample.root,
        commands: JSON.parse(data.files.find(f => f.path === "commands.json").bytes).execution,
    });
    data.files = exported.files;
    data.result = readExecutionOutput(data.files, data.unit, data.launch);
    assert.deepEqual(data.files.map(f => f.path), executionFileNames);
    for (const file of data.files) await fs.writeFile(path.join(directory, file.path), file.bytes);
    let blobs = [];
    for (const blob of exported.blobs) {
        await fs.copyFile(blob.archive.retained.path, path.join(directory, `${blob.slot}.blob`));
        blobs.push({ ...blob, size: blob.archive.bytesReceived, sha256: blob.archive.sha256, sha512: blob.archive.actualSha512 });
    }
    if (options.workerOs === "linux") {
        // Project native offline exports onto Linux paths; this does not execute a Linux worker.
        const converted = JSON.parse(JSON.stringify({
            values: Object.fromEntries(data.files.map(file => [file.path, JSON.parse(file.bytes)])), blobs,
        }).replaceAll(JSON.stringify(data.sample.root).slice(1, -1), "/scratch")
            .replaceAll("\\\\", "/").replaceAll("dotnet.exe", "dotnet")
            .replaceAll("win-x64", "linux-x64").replaceAll('"windows"', '"linux"'));
        const v = converted.values, unit = v["unit.json"], manifest = f.seal(unit.manifest);
        const h = harness(), context = h.context;
        Object.assign(context, data.context, { manifest, stableSdk: unit.stableSdk,
            authorization: { ...data.context.authorization, manifestDigest: fingerprint(manifest),
                inputDigest: manifest.approvedInputDigest },
            environmentFingerprint: fingerprint(Object.fromEntries(manifest.matrix.map(cell =>
                [cell.id, { os: cell.os, architecture: cell.architecture, imageDigest: data.launch.imageDigest }]))),
        });
        await h.start();
        const reservation = await state.reserveWork(h.transport, context, {
            generation: 0, attemptId: data.reservation.id, request: unit.request,
        });
        const started = await state.beginWork(h.transport, context, { generation: reservation.generation, attemptId: reservation.id });
        Object.assign(unit, { epoch: reservation.epoch, environmentFingerprint: context.environmentFingerprint });
        const launch = { ...data.launch, unitDigest: fingerprint(unit) };
        const result = v["result.json"].result;
        const reproduction = unit.request.kind === "source" ? result.reproduction : result;
        const identities = v["identities.json"];
        if (identities) {
            identities.acquisitionDigest = fingerprint(v["acquisition.json"].identity);
            reproduction.identities.sha256 = jsonFile("identities.json", identities).sha256;
            reproduction.manifestDigest = fingerprint(manifest);
            reproduction.productFingerprint = fingerprint(identities.product);
            for (const cell of reproduction.cells) for (const record of cell.evidence)
                record.productFingerprint = reproduction.productFingerprint;
            if (unit.request.kind === "source") result.product = identities.product;
        }
        if (result.toolchain && v["build-acquisition.json"]?.identity)
            result.toolchain.acquisitionDigest = fingerprint(v["build-acquisition.json"].identity);
        Object.assign(v, { "result.json": { schemaVersion: 1, ...launch, result },
            "source.json": unit.request.kind === "source" ? result : null, "reproduction.json": reproduction ?? null });
        Object.assign(data, { h, context, reservation, started, unit, launch,
            files: Object.entries(v).map(([name, value]) => jsonFile(name, value)) });
        data.result = readExecutionOutput(data.files, unit, launch);
        blobs = converted.blobs;
        for (const file of data.files) await fs.writeFile(path.join(directory, file.path), file.bytes);
    }
    const receipt = { schemaVersion: 1, status: "collected", image, evidenceDirectory: directory,
        launch: data.launch, historicalTerminated: true, cleanup: [{ removed: true }],
        files: data.files.map(({ path, size, sha256 }) => ({ path, size, sha256 })), blobs };
    receipt.signature = createHmac("sha256", data.context.keys.fixture).update(fingerprint(receipt)).digest("hex");
    await fs.writeFile(path.join(directory, "receipt.json"), JSON.stringify(receipt));
    const storeConfig = { directory: path.join(root, "store"), repository: data.context.repository,
        runId: data.context.owner.runId, attempt: data.context.owner.attempt };
    const store = createFileEvidenceStore(storeConfig);
    Object.assign(data.h.transport, { readEvidence: store.readEvidence, verifyEvidence: store.verifyEvidence });
    const collector = createObservationCollector({ evidenceDirectory, store });
    const input = { result: data.result, reservation: data.reservation, manifest: data.context.manifest, context: data.context };
    return { ...data, root, directory, evidenceDirectory, storeConfig, store, collector, input };
}

test("factory consumes physical exports for every matrix and setup shape (zero real launches)", async t => {
    for (const workerOs of ["windows", "linux"]) for (const kind of ["released", "source"]) for (const project of [false, true])
        for (const setup of [undefined, "acquisition", "missing-acquisitions", "reproduction-setup",
            ...(kind === "source" ? ["build-acquisition"] : []), ...(project ? ["restore"] : ["timeout"])])
            await t.test(`${workerOs}/${kind}/${project ? "project" : "fsi"}/${setup ?? "tested"}`, async t => {
                const data = await retainedFixture(t, { workerOs, kind, project, setup, buildSdk: "10.0.101", tfms: ["net9.0", "net10.0"] });
                await fs.rm(data.sample.root, { recursive: true });
                const collected = await data.collector(data.input);
                assert.equal(collected.observation.outcome.outcome, setup ? "untestable" : "pass");
                assert.equal(collected.observation.cells.length, 2);
                if (!setup && kind === "source") for (const cell of collected.observation.cells) {
                    assert.equal(cell.sdk.version, "10.0.100");
                    assert.equal(cell.buildToolchain.sdk.version, "10.0.101");
                    assert.notEqual(cell.sdk.sha256, cell.buildToolchain.sdk.sha256);
                }
                const checkpoint = await commitCollectedObservation(data.input, {
                    collectObservation: data.collector, transport: data.h.transport,
                });

                assert.equal(checkpoint.pending, null);
                assert.equal(Object.keys(checkpoint.observations).length, 1);
                assert.deepEqual(checkpoint.state.budget.consumed, {
                    sourceBuilds: kind === "source" ? 1 : 0, workerUnits: 1, reservedMinutes: 110,
                });
                t.diagnostic(JSON.stringify({ realLaunches: 0, outcome: collected.observation.outcome,
                    transferred: data.files.map(({ path, size, sha256 }) => ({ path, size, sha256 })) }));
            });
});

test("acceptance reader authenticates durable collections in a new process after launch deletion", async t => {
    const data = await retainedFixture(t);
    await data.collector(data.input);
    const apiFile = path.join(data.root, "api.json"), readerFile = path.join(data.root, "reader.json");
    await fs.writeFile(apiFile, "{}");
    await fs.writeFile(`${apiFile}.ledger`, JSON.stringify({ kind: "api", method: "GET" }));
    await fs.writeFile(readerFile, JSON.stringify({ context: { ...data.context, keys: undefined },
        storeConfig: data.storeConfig, reservations: [data.reservation], launches: data.evidenceDirectory, apiFile }));
    await fs.rm(data.evidenceDirectory, { recursive: true });
    const before = await fs.readdir(data.storeConfig.directory);
    const result = spawnSync(process.execPath, [path.join(__dirname, "controller.smoke.cjs"), "--read", readerFile], {
        env: { ...process.env, RB_SMOKE_KEY: data.context.keys.fixture.toString("hex") }, encoding: "utf8", timeout: 30000,
    });
    assert.equal(result.status, 0, result.stderr);
    assert.equal(JSON.parse(result.stdout).launchScratchDeleted, true);
    assert.deepEqual(await fs.readdir(data.storeConfig.directory), before);
});

test("acceptance exports retain indispensable matrix bytes once instead of duplicating the supplement", async t => {
    const data = await retainedFixture(t);
    const acceptance = { status: "passed", indispensableLog: "fixture assertion bytes".repeat(65536) };
    const exported = await executionEvidence(data.unit, data.launch, { ...data.result, acceptance }, {
        acquisition: { status: "acquired", identity: data.acquisition },
        evidenceDirectory: path.join(data.sample.root, "evidence"), scratchDirectory: data.sample.root,
    });
    const value = name => JSON.parse(exported.files.find(file => file.path === name).bytes);
    assert.deepEqual(value("result.json").result.acceptance, acceptance);
    assert.ok(!Object.hasOwn(value("source.json"), "acceptance"));
    assert.equal(value("source.json").actualSourceSha, data.result.actualSourceSha);
});

test("source evidence forms an observation after all worker scratch is destroyed (zero real launches)", async t => {
    const data = await fixture(t);
    await fs.rm(data.sample.root, { recursive: true });
    const observation = formObservation(data, data.reservation, data.context);
    assert.equal(observation.outcome.outcome, "pass");
    assert.equal(observation.buildInstanceId, data.reservation.id);
    assert.equal(observation.provenance.workerId, data.launch.launchId);
    assert.equal(observation.cells[0].buildToolchain.sourceSha, data.unit.request.target.sourceSha);
    assert.deepEqual(observation.cells[0].attempts, data.result.reproduction.cells[0].attempts);
    t.diagnostic(JSON.stringify({ realLaunches: 0, transferred: data.files.map(({ path, size, sha256 }) => ({ path, size, sha256 })) }));
});

test("released and source observations recompute exact repetitions and full matrices", async t => {
    for (const kind of ["source", "released"]) for (const outcome of ["pass", "product-failure", "flaky"])
        await t.test(`${kind}/${outcome}`, async t => {
            const m = f.manifest();
            const outputs = outcome === "pass" ? [m.assertions.pass, m.assertions.pass] :
                outcome === "product-failure" ? [m.assertions.failure, m.assertions.failure] :
                    [m.assertions.pass, m.assertions.failure];
            const data = await fixture(t, { kind, outputs: [...outputs, ...outputs], tfms: ["net9.0", "net10.0"] });
            const observation = formObservation(data, data.reservation, data.context);
            assert.equal(observation.cells.length, 2);
            assert.deepEqual(observation.cells.map(c => c.outcome),
                [classifyAttempts(observation.cells[0].attempts, data.context.manifest),
                    classifyAttempts(observation.cells[1].attempts, data.context.manifest)]);
            assert.equal(observation.outcome.outcome, outcome === "flaky" ? "untestable" : outcome);
        });

});

test("collector preserves project and compiler-diagnostic assertion semantics", async t => {
    for (const project of [false, true]) for (const compile of [false, true])
        await t.test(`${project ? "project" : "fsi"}/${compile ? "compile" : "runtime"}`, async t => {
            const data = await retainedFixture(t, { project, compile, tfms: ["net9.0", "net10.0"] });
            const { observation } = await data.collector(data.input);
            assert.equal(observation.outcome.outcome, compile ? "product-failure" : "pass");
            for (const cell of observation.cells)
                assert.deepEqual(cell.attempts, data.result.reproduction.cells.find(c => c.id === cell.id).attempts);
        });
});

test("factory retains frozen legacy and package producer provenance", async t => {
    for (const producer of ["legacy", "package"]) await t.test(producer, async t => {
        const data = await retainedFixture(t, { project: true, producer });
        await fs.rm(data.sample.root, { recursive: true });
        const { observation } = await data.collector(data.input);
        assert.equal(observation.outcome.outcome, "pass");
        assert.deepEqual(observation.cells[0].producer, data.context.manifest.producer);
        const identities = JSON.parse(data.files.find(f => f.path === "identities.json").bytes);
        assert.equal(identities.producers[0].compiler.version, "8.0.100");
        assert.equal(Boolean(identities.producers[0].package), producer === "package");
    });

});

test("factory keeps compiler and sample runtimes independent across repetitions", async t => {
    for (const workerOs of ["windows", "linux"]) for (const kind of ["released", "source"]) for (const sampleRuntime of ["10.0.0", "9.0.0"])
        await t.test(`${workerOs}/${kind}/${sampleRuntime}`, async t => {
            const data = await retainedFixture(t, { workerOs, kind, project: true, sampleRuntime, tfms: ["net9.0", "net10.0"] });
            await fs.rm(data.sample.root, { recursive: true });
            const cp = await commitCollectedObservation(data.input, { collectObservation: data.collector, transport: data.h.transport });
            assert.equal(cp.pending, null);
            const observation = Object.values(cp.observations)[0].data;
            assert.equal(observation.outcome.outcome, "pass");
            for (const cell of observation.cells) {
                assert.equal(cell.runtime.version, sampleRuntime);
                assert.equal((cell.buildToolchain ?? cell.releasedToolchain).compilerRuntime.version, "10.0.0");
                assert.equal(cell.attempts.length, 2);
            }
            if (sampleRuntime === "9.0.0") for (const changedRuntime of ["compiler", "sample"]) {
                const payload = JSON.parse(data.files.find(f => f.path === "result.json").bytes);
                const reproduction = kind === "source" ? payload.result.reproduction : payload.result;
                const record = reproduction.cells[0].evidence[1];
                const runtimes = JSON.parse(data.files.find(f => f.path === "acquisition.json").bytes).identity.runtimes;
                if (changedRuntime === "compiler") record.compilerRuntimeSelection.runtime = runtimes[1];
                else {
                    record.commands.at(-1).args[2] = runtimes[0].version;
                    record.loadedCore.runtimeVersion = runtimes[0].version;
                    record.loadedCore.runtimePath = path.posix.join(runtimes[0].path, runtimes[0].version);
                }
                const commands = JSON.parse(data.files.find(f => f.path === "commands.json").bytes);
                commands.execution = [...commands.source, ...reproduction.cells.flatMap(cell =>
                    cell.evidence.flatMap(record => record.commands))].map(command => ({ ...command, lane: "historical" }));
                const files = data.files.map(file => file.path === "result.json" ? jsonFile(file.path, payload) :
                    file.path === "commands.json" ? jsonFile(file.path, commands) : file);
                assert.throws(() => formObservation({ ...data, files }, data.reservation, data.context),
                    changedRuntime === "compiler" ? /runtime selection mismatch/ : /sample runtime mismatch/);
            }
        });
});

test("missing setup identity is untestable, never a worker-authored green observation", async t => {
    const data = await fixture(t);
    const payload = JSON.parse(data.files[0].bytes);
    payload.result = { outcome: { schemaVersion: 1, outcome: "untestable", reason: "unavailable-sdk" },
        error: { stage: "sdk", code: "archive-unavailable", message: "Exact SDK unavailable" } };
    data.files = [jsonFile("result.json", payload)];
    const observation = formObservation(data, data.reservation, data.context);
    assert.deepEqual(observation.outcome, { schemaVersion: 1, outcome: "untestable", reason: "incomplete-evidence" });
    assert.ok(observation.cells.every(c => c.sdk === null && c.runtime === null &&
        c.attempts.every(a => !a.checks.identity && !a.checks.sdk && !a.checks.runtime)));
    payload.result.outcome = { schemaVersion: 1, outcome: "pass", reason: "positive-assertion" };
    data.files = [jsonFile("result.json", payload)];
    assert.throws(() => formObservation(data, data.reservation, data.context), /missing indispensable/);
});

test("factory rejects product and runtime contradictions in authenticated exports", async t => {
    const cases = [
        ...["compiler", "fsi", "core"].map(role => ({ kind: "released", fault: `${role}-hash`, role })),
        { kind: "released", fault: "compiler-version", role: "compiler" },
        { kind: "released", fault: "product-runtime", project: true },
        ...["source", "released"].flatMap(kind => [
            { kind, fault: "compiler-load", project: true },
            { kind, fault: "fsi-load" },
            { kind, fault: "compiler-selection", project: true },
        ]),
    ];
    for (const workerOs of ["windows", "linux"]) for (const scenario of cases)
        await t.test(`${workerOs}/${scenario.kind}/${scenario.fault}`, async t => {
            const data = await retainedFixture(t, { workerOs, ...scenario, sampleRuntime: "9.0.0", tfms: ["net9.0", "net10.0"] });
            const values = Object.fromEntries(data.files.map(file => [file.path, JSON.parse(file.bytes)]));
            const identities = values["identities.json"], product = identities.product;
            const result = values["result.json"].result;
            const reproduction = scenario.kind === "source" ? result.reproduction : result;
            if (scenario.role) {
                const file = product.files.find(file => file.role === scenario.role);
                if (scenario.fault.endsWith("-hash")) file.sha256 = f.digest("substituted product");
                else file.assemblyVersion = "99.0.0.0";
                if (scenario.role === "core") for (const asset of product.coreAssets) asset.sha256 = file.sha256;
                for (const cell of reproduction.cells) for (const record of cell.evidence) {
                    if (record.selection[scenario.role]) record.selection[scenario.role].sha256 = file.sha256;
                    if (scenario.role === "core") record.loadedCore.sha256 = file.sha256;
                }
            } else {
                const runtime = values["acquisition.json"].identity.runtimes.find(r => r.version === "9.0.0");
                if (scenario.fault === "product-runtime") product.runtime = { name: runtime.name, version: runtime.version };
                for (const cell of reproduction.cells) for (const record of cell.evidence) {
                    if (["compiler-selection", "product-runtime"].includes(scenario.fault)) record.compilerRuntimeSelection.runtime = runtime;
                    for (const load of scenario.project ? record.compilerCoreLoads : [record.loadedCore]) {
                        load.runtimeVersion = runtime.version;
                        load.runtimePath = (workerOs === "linux" ? path.posix : path.win32).join(runtime.path, runtime.version);
                    }
                }
            }
            reproduction.identities.sha256 = jsonFile("identities.json", identities).sha256;
            reproduction.productFingerprint = fingerprint(product);
            for (const cell of reproduction.cells) for (const record of cell.evidence)
                record.productFingerprint = reproduction.productFingerprint;
            values["product.json"] = product;
            values["reproduction.json"] = reproduction;
            if (scenario.kind === "source") values["source.json"] = result;
            const files = Object.entries(values).map(([name, value]) => jsonFile(name, value));
            const receipt = JSON.parse(await fs.readFile(path.join(data.directory, "receipt.json")));
            delete receipt.signature;
            receipt.files = files.map(({ path, size, sha256 }) => ({ path, size, sha256 }));
            receipt.signature = createHmac("sha256", data.context.keys.fixture).update(fingerprint(receipt)).digest("hex");
            for (const file of files) await fs.writeFile(path.join(data.directory, file.path), file.bytes);
            await fs.writeFile(path.join(data.directory, "receipt.json"), JSON.stringify(receipt));
            await fs.rm(data.sample.root, { recursive: true });
            await assert.rejects(data.collector({ ...data.input, result }),
                scenario.role ? /released product.*acquisition/ : /runtime.*mismatch/);
            assert.deepEqual(await data.store.findEvidence({
                runId: data.context.owner.runId, attempt: data.context.owner.attempt, buildInstanceId: data.reservation.id,
            }), []);
        });
});

test("durable collection and acknowledgement loss resume in a new process without worker scratch", async t => {
    for (const [outcome, options] of [
        ["pass", { workerOs: "linux", project: true, sampleRuntime: "9.0.0" }],
        ["restore-failure", { workerOs: "linux", project: true, setup: "restore" }],
        ["flaky", { workerOs: "linux", outputs: [f.manifest().assertions.pass, f.manifest().assertions.failure] }],
    ]) for (const fault of ["collection", "durable-write", "observation-write", "acknowledgement"])
        await t.test(`${outcome}/${fault}`, async t => {
            const data = await retainedFixture(t, options);
            const original = data.store.writeEvidence;
            const transport = data.h.transport;
            const update = transport.updateComment;
            if (fault === "collection" || fault === "durable-write") data.store.writeEvidence = async (...args) => {
                if (fault === "durable-write") await original(...args);
                throw new Error(`fault-${fault}`);
            };
            else transport.updateComment = async (...args) => {
                if (fault === "acknowledgement") await update(...args);
                throw new Error(`fault-${fault}`);
            };
            const collector = createObservationCollector({ evidenceDirectory: data.evidenceDirectory, store: data.store });
            await assert.rejects(commitCollectedObservation(data.input, { collectObservation: collector, transport }), new RegExp(`fault-${fault}`));
            data.store.writeEvidence = original;
            transport.updateComment = update;
            await fs.rm(data.sample.root, { recursive: true });
            if (fault !== "collection") await fs.rm(data.directory, { recursive: true });
            const snapshotFile = path.join(data.root, "checkpoint-transport.json");
            await fs.writeFile(snapshotFile, JSON.stringify({
                context: data.context, comment: data.h.comment, statuses: data.h.statuses, runs: [...data.h.runs],
                evidenceDirectory: data.evidenceDirectory, storeConfig: data.storeConfig,
            }));
            const script = `
                const fs = require("node:fs");
                const state = require(${JSON.stringify(require.resolve("./state.cjs"))});
                const { createFileEvidenceStore } = require(${JSON.stringify(require.resolve("./evidence-store.cjs"))});
                const { createObservationCollector, commitCollectedObservation } = require(${JSON.stringify(require.resolve("./collector.cjs"))});
                (async () => {
                    const file = process.argv[1], snapshot = JSON.parse(fs.readFileSync(file));
                    const h = require(${JSON.stringify(require.resolve("./fixtures/checkpoint.cjs"))}).harness(snapshot);
                    const context = h.context;
                    const save = () => fs.writeFileSync(file, JSON.stringify({...snapshot,
                        comment:h.comment, statuses:h.statuses}));
                    const store = createFileEvidenceStore(snapshot.storeConfig);
                    const transport = {
                        ...h.transport, ...store,
                        updateComment: async (id,body) => { const value=await h.transport.updateComment(id,body); save(); return value; },
                        request: async (route,args) => {
                            const result=await h.transport.request(route,args); save(); return result;
                        }
                    };
                    const before = await state.loadCheckpoint(transport,context);
                    const reservation = before.checkpoint.records[0];
                    const collectObservation = createObservationCollector({evidenceDirectory:snapshot.evidenceDirectory,store});
                    const input={reservation,manifest:context.manifest,context};
                    const first=await commitCollectedObservation(input,{collectObservation,transport});
                    const second=await commitCollectedObservation(input,{collectObservation,transport});
                    if(JSON.stringify(first)!==JSON.stringify(second)) throw Error("non-idempotent acknowledgement");
                    console.log(JSON.stringify({pending:second.pending,consumed:second.state.budget.consumed,
                        observations:Object.keys(second.observations).length, records:second.records.length,
                        buildId:Object.values(second.observations)[0].data.buildInstanceId,
                        outcome:Object.values(second.observations)[0].data.outcome,
                        signedBytes:Buffer.byteLength(h.comment.body),realLaunches:0}));
                })().catch(e=>{console.error(e.stack);process.exitCode=1;});`;
            const child = spawnSync(process.execPath, ["-e", script, snapshotFile], { encoding: "utf8", timeout: 30000, maxBuffer: 65536 });
            assert.equal(child.status, 0, child.stderr);
            const replay = JSON.parse(child.stdout);
            assert.equal(replay.pending, null);
            assert.deepEqual(replay.consumed, { sourceBuilds: 1, workerUnits: 1, reservedMinutes: 110 });
            assert.equal(replay.observations, 1);
            assert.equal(replay.records, 1);
            assert.equal(replay.buildId, data.reservation.id);
            assert.equal(replay.outcome.outcome, outcome === "pass" ? "pass" : "untestable");
            if (outcome !== "pass") assert.equal(replay.outcome.reason, outcome);
            assert.ok(replay.signedBytes < 48 * 1024);
            assert.equal(replay.realLaunches, 0);
            t.diagnostic(JSON.stringify({ fault, ...replay }));
        });
});

test("forged launch receipts and required loss block collection/reuse without refund", async t => {
    for (const fault of ["unsigned", "signature", "raw-bytes", "forged-collection", "required-archive", "required-collection"])
        await t.test(fault, async t => {
            const data = await retainedFixture(t);
            if (["unsigned", "signature"].includes(fault)) {
                const file = path.join(data.directory, "receipt.json");
                const receipt = JSON.parse(await fs.readFile(file));
                if (fault === "unsigned") delete receipt.signature;
                else receipt.signature = "a".repeat(64);
                await fs.writeFile(file, JSON.stringify(receipt));
                await assert.rejects(data.collector(data.input), /authentication/);
            } else if (fault === "raw-bytes") {
                await fs.appendFile(path.join(data.directory, "identities.json"), " ");
                await assert.rejects(data.collector(data.input), /digest|size/);
            } else {
                const collected = await data.collector(data.input);
                if (fault === "forged-collection") {
                    const reference = collected.evidence[0];
                    const envelope = JSON.parse(await data.store.readEvidence(reference.url));
                    envelope.receipt.launch.imageDigest = f.digest("forged image");
                    const bytes = Buffer.from(JSON.stringify(envelope));
                    const collector = createObservationCollector({ evidenceDirectory: data.evidenceDirectory, store: {
                        ...data.store, findEvidence: async () => [{ ...reference, sha256: f.digest(bytes) }],
                        readEvidence: async () => bytes,
                    } });
                    await assert.rejects(collector(data.input), /authentication/);
                    const cp = (await state.loadCheckpoint(data.h.transport, data.context)).checkpoint;
                    assert.equal(cp.pending.id, data.reservation.id);
                    assert.equal(cp.state.budget.consumed.sourceBuilds, 1);
                    return;
                }
                await state.commitObservation(data.h.transport, data.context, {
                    generation: data.started.generation, attemptId: data.reservation.id, ...collected,
                });
                const reference = collected.evidence[fault === "required-collection" ? 0 : 1];
                const name = new URL(reference.url).searchParams.get("name");
                await fs.rm(path.join(data.storeConfig.directory, `${name}.bin`));
                const loaded = await state.loadCheckpoint(data.h.transport, data.context);
                assert.equal(loaded.executionAllowed, false);
                assert.equal(loaded.publicationAllowed, false);
                assert.ok(loaded.warnings.some(w => w.startsWith("required")));
                assert.equal(loaded.checkpoint.state.budget.consumed.sourceBuilds, 1);
            }
        });
});

test("collector rejects altered reservations, raw identities and incomplete assertions", async t => {
    const original = await fixture(t);
    const changes = {
        work: d => { d.reservation.id = "another"; },
        source: d => { d.unit.request.target.sourceSha = f.sha("another"); },
        epoch: d => { d.reservation.epoch = f.digest("another"); },
        workflow: d => { d.context.workflowRevision = f.sha("another"); },
        policy: d => { d.context.policyRevision = f.sha("another"); },
        owner: d => { d.context.owner.attempt++; },
        image: d => { d.launch.imageDigest = f.digest("another"); },
        missing: d => { d.files = d.files.filter(f => f.path !== "identities.json"); },
        duplicate: d => { d.files.push(d.files.find(f => f.path === "identities.json")); },
        truncated: d => { d.files[1].bytes = d.files[1].bytes.subarray(0, -1); },
        hash: d => { d.files[1].sha256 = f.digest("changed"); },
        sourceDeclaration: d => {
            const index = d.files.findIndex(f => f.path === "global.json");
            d.files[index] = jsonFile("global.json", { sdk: { version: "99.0.100" }, tools: { dotnet: "99.0.100" } });
        },
        runtimeDeclaration: d => {
            const index = d.files.findIndex(f => f.path === "compiler.runtimeconfig.json");
            d.files[index] = jsonFile("compiler.runtimeconfig.json", { runtimeOptions: {
                framework: { name: "Microsoft.NETCore.App", version: "99.0.0" },
            } });
        },
    };
    for (const [name, mutate] of Object.entries(changes)) await t.test(name, () => {
        const data = { ...original, unit: structuredClone(original.unit), launch: { ...original.launch },
            reservation: structuredClone(original.reservation),
            context: { ...original.context, owner: { ...original.context.owner } },
            files: original.files.map(file => ({ ...file, bytes: Buffer.from(file.bytes) })) };
        mutate(data);
        assert.throws(() => formObservation(data, data.reservation, data.context));
    });
    for (const [name, mutate] of Object.entries({
        host: r => { r.reproduction.cells[0].evidence[0].commands[0].executable = "dotnet"; },
        command: r => { r.reproduction.cells[0].evidence[0].commands[0].args[3] = "fsi"; },
        output: r => { r.reproduction.cells[0].attempts[0].execution.stdout = "forged"; },
        missingRepetition: r => { r.reproduction.cells[0].evidence.pop(); },
        core: r => { r.reproduction.cells[0].evidence[0].loadedCore.sha256 = f.digest("wrong Core"); },
        runtime: r => { r.reproduction.cells[0].evidence[0].loadedCore.runtimePath = "host-runtime"; },
        loadedPath: r => { r.reproduction.cells[0].evidence[0].loadedCore.path = "host-core.dll"; },
        setup: r => { r.reproduction.cells[0].attempts[0].checks.restore = false; },
        build: r => { r.commands[0].exitCode = 1; },
        buildId: r => { r.buildInstanceId = "other-build"; },
        source: r => { r.actualSourceSha = f.sha("other"); },
        matrix: r => { r.reproduction.cells[0].id = "missing"; },
    })) await t.test(name, () => {
        const payload = JSON.parse(original.files[0].bytes);
        mutate(payload.result);
        const data = { ...original, files: [jsonFile("result.json", payload), ...original.files.slice(1)] };
        assert.throws(() => formObservation(data, data.reservation, data.context));
    });
    for (const [name, mutate] of Object.entries({
        missingCommand: c => { c.execution.pop(); },
        commandResult: c => { c.execution.at(-1).result.stdout = "forged"; },
        commandHost: c => { c.execution.at(-1).executable = "dotnet"; },
        sourceCommand: c => { c.source[0].result.exitCode = 1; },
    })) await t.test(name, () => {
        const commands = JSON.parse(original.files.find(f => f.path === "commands.json").bytes);
        mutate(commands);
        const data = { ...original, files: original.files.map(file =>
            file.path === "commands.json" ? jsonFile(file.path, commands) : file) };
        assert.throws(() => formObservation(data, data.reservation, data.context), /command/);
    });
});
