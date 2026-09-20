const path = require("node:path");
const fs = require("node:fs/promises");
const { createHmac, timingSafeEqual } = require("node:crypto");
const { fingerprintInput: fingerprint, validateObservation, classifyAttempts } = require("./model.cjs");
const { validateExecutionUnit, readExecutionOutput } = require("./worker.cjs");
const { verifyToolchainIdentity } = require("./toolchains.cjs");
const { compileAssertion, diagnosticsExecution } = require("./repro.cjs");
const { hash, requireValue } = require("./toolchain-io.cjs");
const { loadContainerEvidence, verifyTransportReceipt } = require("./container.cjs");

const equal = (a, b) => fingerprint(a) === fingerprint(b);
const untestable = reason => ({ schemaVersion: 1, outcome: "untestable", reason });

// These records describe what the pinned harness measured, not whether historical code is trustworthy.
function formObservation({ unit, launch, files }, reservation, context) {
    validateExecutionUnit(unit);
    const manifest = context.manifest;
    requireValue(unit.repository === context.repository && unit.issueNumber === context.issueNumber &&
        unit.buildInstanceId === reservation.id && unit.epoch === reservation.epoch &&
        unit.deadline === reservation.deadline && equal(unit.request, reservation.request) &&
        equal(unit.manifest, manifest) && unit.policyDigest === fingerprint(context.policy) &&
        unit.policyRevision === context.policyRevision && unit.workflowRevision === context.workflowRevision &&
        unit.environmentFingerprint === context.environmentFingerprint &&
        equal(unit.stableSdk, context.stableSdk) &&
        reservation.owner.runId === context.owner.runId && reservation.owner.attempt === context.owner.attempt,
    "collected execution does not match trusted reservation/context");
    const environment = Object.fromEntries(manifest.matrix.map(c => [c.id,
        { os: c.os, architecture: c.architecture, imageDigest: launch.imageDigest }]));
    requireValue(fingerprint(environment) === unit.environmentFingerprint, "collected image/matrix mismatch");
    const result = readExecutionOutput(files, unit, launch);
    const json = name => {
        const matches = files.filter(f => f.path === name);
        requireValue(matches.length === 1, `missing or duplicate ${name}`);
        const file = matches[0];
        requireValue(Buffer.isBuffer(file.bytes) && file.size === file.bytes.length && file.size <= 16 * 1024 * 1024 &&
            hash(file.bytes) === file.sha256, `changed or oversized ${name}`);
        const value = JSON.parse(new TextDecoder("utf-8", { fatal: true }).decode(file.bytes));
        fingerprint(value);
        return value;
    };
    const source = unit.request.kind === "source";
    if (source) requireValue((result.buildInstanceId === undefined || result.buildInstanceId === reservation.id) &&
        (result.actualSourceSha === undefined || result.actualSourceSha === unit.request.target.sourceSha),
    "source result does not match reserved build");
    const reproduction = source ? result.reproduction : result;
    const filePath = unit.platform.os === "windows" ? path.win32 : path.posix;
    const within = (root, value) => {
        requireValue(typeof root === "string" && typeof value === "string" &&
            filePath.isAbsolute(root) && filePath.isAbsolute(value) && !/[\0\r\n]/.test(value),
        "invalid measured absolute path");
        const relative = filePath.relative(root, value);
        return relative !== "" && relative !== ".." && !relative.startsWith(`..${filePath.sep}`) &&
            !filePath.isAbsolute(relative);
    };
    const identity = name => {
        const record = json(name);
        const value = record.identity ?? record;
        const checked = verifyToolchainIdentity({ policy: context.policy, toolchain: value.requested, identity: value }, filePath);
        requireValue(checked.status === "verified", `invalid ${name}: ${checked.error?.message}`);
        requireValue(equal(value.requested.platform, unit.platform), "acquired toolchain platform mismatch");
        return value;
    };
    let cells;
    if (!reproduction?.identities) {
        requireValue(result.outcome.outcome === "untestable" && (result.error || reproduction?.error),
            "missing indispensable reproduction evidence");
        const reasons = {
            "network-failure": "network", "checksum-provenance": "checksumProvenance",
            "restore-failure": "restore", "build-failure": "build", "setup-failure": "setup",
            "wrong-identity": "identity", "unavailable-sdk": "sdk", "unavailable-runtime": "runtime",
        };
        const reason = result.outcome.reason;
        requireValue(reasons[reason] || ["timeout", "incomplete-evidence"].includes(reason), "unsubstantiated setup outcome");
        cells = manifest.matrix.map(c => {
            const checks = Object.fromEntries(Object.values(reasons).map(k => [k, true]));
            checks.identity = checks.sdk = checks.runtime = false;
            if (reasons[reason]) checks[reasons[reason]] = false;
            const attempt = { schemaVersion: 1, checks,
                execution: { status: reason === "timeout" ? "timeout" : "exception",
                    complete: false, exitCode: null, stdout: "", stderr: "" } };
            const attempts = [attempt];
            return { id: c.id, command: c.command, environment: environment[c.id],
                sdk: null, runtime: null, artifacts: { compiler: null, tasks: null, targets: null, core: null },
                producer: manifest.producer, attempts, outcome: classifyAttempts(attempts, manifest) };
        });
    } else {
        requireValue(reproduction.manifestDigest === fingerprint(manifest) &&
            reproduction.cells.length === manifest.matrix.length &&
            new Set(reproduction.cells.map(c => c.id)).size === manifest.matrix.length, "incomplete or changed reproduction matrix");
        const identities = json("identities.json");
        const identityFile = files.find(f => f.path === "identities.json");
        requireValue(identityFile.sha256 === reproduction.identities.sha256, "reproduction identities digest mismatch");
        const commands = json("commands.json");
        requireValue(Array.isArray(commands.source) && Array.isArray(commands.execution) &&
            equal(commands.source.map(({ result, ...command }) => command), source ? result.commands : []) &&
            commands.source.every(command => command.result.exitCode === command.exitCode &&
                (command.result.status ?? "completed") === command.status && command.result.complete !== false),
        "source command receipts differ from build result");
        const observed = [...commands.execution];
        for (const command of [...commands.source, ...reproduction.cells.flatMap(cell =>
            cell.evidence.flatMap(record => record.commands))]) {
            const index = observed.findIndex(actual => ["trusted", "historical"].includes(actual.lane) &&
                actual.executable === command.executable && equal(actual.args, command.args) &&
                (command.cwd === undefined || actual.cwd === command.cwd) && equal(actual.result, command.result));
            requireValue(index >= 0, "command differs from protected supervisor evidence");
            observed.splice(index, 1);
        }
        const acquisition = identity("acquisition.json");
        requireValue(identities.acquisitionDigest === fingerprint(acquisition) &&
            acquisition.sdkVersion === unit.stableSdk.version &&
            acquisition.archive.sha256 === unit.stableSdk.archives.find(a =>
                a.os === unit.platform.os && a.architecture === unit.platform.architecture)?.sha256 &&
            identities.host.path === acquisition.hostPath && identities.host.sha256 === acquisition.hostSha256 &&
            identities.architecture === unit.platform.architecture && equal(identities.runtimes, acquisition.runtimes),
        "reproduction acquisition/host/runtime mismatch");
        const product = identities.product;
        requireValue(product.kind === unit.request.kind && product.buildInstanceId === reservation.id &&
            reproduction.productFingerprint === fingerprint(product), "product inventory binding mismatch");
        if (source) requireValue(result.status === "tested" && result.actualSourceSha === unit.request.target.sourceSha &&
            result.buildInstanceId === reservation.id && product.sourceSha === unit.request.target.sourceSha &&
            result.globalJson.before === result.globalJson.after &&
            equal(result.product.files, product.files) && result.product.root === product.root,
        "source/build inventory binding mismatch");
        else requireValue(product.root === filePath.join(acquisition.sdkBasePath, "FSharp"), "released product path fallback");
        const roles = Object.fromEntries(["fsi", "compiler", "tasks", "targets", "core", "props", "sdkTargets", "overrides", "corePackage"]
            .map(role => {
                const found = product.files.filter(f => f.role === role);
                requireValue(found.length === 1 && within(product.root, found[0].path) &&
                    /^[a-f0-9]{64}$/.test(found[0].sha256) && (!source || !within(acquisition.root, found[0].path)),
                `missing, ambiguous or fallback ${role} inventory`);
                return [role, found[0]];
            }));
        requireValue(roles.corePackage.id === "FSharp.Core" && roles.corePackage.version === product.corePackageVersion &&
            product.coreAssets?.some(a => a.sha256 === roles.core.sha256),
        "Core package inventory mismatch");
        requireValue(Array.isArray(identities.producers) && identities.producers.length <= 1, "invalid producer inventory");
        if (manifest.producer.name === "no-producer") requireValue(identities.producers.length === 0 &&
            manifest.producer.version === "0.0.0" && manifest.producer.sha256 === hash(""), "producer mismatch");
        else {
            const producer = identities.producers[0];
            requireValue(producer?.role === "immutable-producer" && producer.dll.sha256 === manifest.producer.sha256 &&
                manifest.sources.some(s => s.path === producer.dll.path && s.sha256 === producer.dll.sha256) &&
                producer.dll.resources?.some(r => /^FSharp(Signature|Optimization)(Compressed)?DataB?\./.test(r.name)) &&
                producer.compiler?.sha256 && producer.compiler.version && producer.tfm && Array.isArray(producer.flags) &&
                Array.isArray(producer.packages) && (!producer.package || manifest.packages.some(p =>
                    p.id === producer.package.id && p.version === producer.package.version && p.sha256 === producer.package.sha256)),
            "immutable producer provenance mismatch");
        }
        let buildToolchain;
        if (source) {
            const build = identity("build-acquisition.json");
            const requested = build.requested;
            const declaration = json("global.json");
            requireValue(requested.kind === "source" && requested.sourceSha === unit.request.target.sourceSha &&
                result.toolchain.acquisitionDigest === fingerprint(build) &&
                result.globalJson.before === requested.globalJson.sha256 &&
                files.find(f => f.path === "global.json").sha256 === requested.globalJson.sha256 &&
                declaration.sdk?.version === requested.sdkVersion && declaration.tools?.dotnet === requested.toolsVersion &&
                result.commands.filter(c => c.stage === "build").length >= 2 &&
                result.commands.every(c => c.status === "completed" && c.exitCode === 0),
            "source acquisition/build execution mismatch");
            const runtime = product.runtime;
            const runtimeFile = build.runtimes.find(r => r.name === runtime?.name && r.version === runtime.version);
            requireValue(runtimeFile, "missing source compiler runtime evidence");
            for (const role of ["fsi", "compiler"]) {
                const name = `${role}.runtimeconfig.json`;
                const config = json(name).runtimeOptions?.framework;
                requireValue(files.find(f => f.path === name).sha256 === roles[role].runtimeConfig.sha256 &&
                    config?.name === runtime.name &&
                    config.version.split(".").slice(0, 2).join(".") === runtime.version.split(".").slice(0, 2).join(".") &&
                    Number(config.version.split(".")[2].split("-")[0]) <= Number(runtime.version.split(".")[2].split("-")[0]),
                "source compiler runtimeconfig mismatch");
            }
            const { buildToolchain: ignored, ...measured } = build;
            buildToolchain = {
                sourceSha: unit.request.target.sourceSha, globalJsonDigest: requested.globalJson.sha256,
                evidenceDigest: fingerprint(measured), requestedSdk: requested.sdkVersion, requestedTools: requested.toolsVersion,
                requestedCompilerRuntime: runtime.version,
                sdk: { version: build.sdkVersion, sha256: build.archive.sha256, verified: true },
                compilerRuntime: { version: runtime.version, sha256: runtimeFile.sha256, verified: true },
                archive: build.archive, verified: true, supported: true,
            };
        }
        cells = manifest.matrix.map(c => {
            const measured = reproduction.cells.find(row => row.id === c.id);
            requireValue(measured && measured.attempts.length === context.policy.limits.assertionRepetitions &&
                measured.evidence.length === measured.attempts.length, "missing repeated assertion evidence");
            let selectedRuntime;
            const attempts = measured.attempts.map((attempt, index) => {
                const record = measured.evidence[index];
                requireValue(record.productFingerprint === fingerprint(product) && equal(record.producers, identities.producers) &&
                    Array.isArray(record.commands) && record.commands.length > 0, "assertion inventory/producers mismatch");
                const runtime = manifest.kind === "fsi" ? record.selection?.runtime : record.compilerRuntimeSelection?.runtime;
                requireValue(runtime && acquisition.runtimes.some(r => equal(r, runtime)) &&
                    (!selectedRuntime || equal(selectedRuntime, runtime)), "runtime selection mismatch");
                selectedRuntime = runtime;
                const last = record.commands.at(-1);
                const isCompile = compileAssertion(manifest);
                const directory = filePath.join(reproduction.workDirectory, `cell-${manifest.matrix.indexOf(c)}-${index}`);
                const interruptedBuild = record.build && (record.build.status !== "completed" || !record.build.complete);
                const restoreFailure = record.diagnostics?.some(d => d.severity === "error" &&
                    (/^NU\d+$/.test(d.code) || filePath.basename(d.file ?? "").toLowerCase() === "nuget.targets"));
                if (manifest.kind === "project" && (interruptedBuild || restoreFailure ||
                    !isCompile && record.build?.exitCode !== 0)) {
                    requireValue(record.commands.length === 1 && last.executable === acquisition.hostPath &&
                        last.args[0] === "build" && last.args[1] === filePath.join(directory, c.project) &&
                        last.args.includes(`-p:TargetFramework=${c.tfm}`) &&
                        last.args.includes(`-p:Configuration=${c.configuration}`) &&
                        last.args.includes(`-p:LangVersion=${c.languageVersion}`) && equal(last.result, record.build),
                    "failed project command/matrix mismatch");
                    const checks = Object.fromEntries(Object.keys(attempt.checks).map(k => [k, true]));
                    if (restoreFailure) checks.restore = false;
                    else if (!interruptedBuild) checks.build = false;
                    requireValue(equal(attempt.checks, checks) && equal(attempt.execution, Object.fromEntries(
                        ["status", "complete", "exitCode", "stdout", "stderr"].map(k => [k, last.result[k]]))) &&
                        classifyAttempts([attempt, attempt], manifest).outcome === "untestable",
                    "failed project assertion differs from measured command");
                    return structuredClone(attempt);
                }
                requireValue(last.executable === acquisition.hostPath &&
                    (manifest.kind === "project" && isCompile ? last.args[0] === "build" :
                        last.args[0] === "exec" && last.args[1] === "--fx-version"), "assertion host fallback");
                let execution = last.result;
                if (manifest.kind === "fsi") {
                    requireValue(record.selection.fsi.path === roles.fsi.path && record.selection.fsi.sha256 === roles.fsi.sha256 &&
                        record.selection.core.path === roles.core.path && record.selection.core.sha256 === roles.core.sha256 &&
                        record.selection.host?.path === acquisition.hostPath &&
                        record.selection.host.sha256 === acquisition.hostSha256 &&
                        equal(last.args.slice(0, -1), ["exec", "--fx-version", runtime.version, roles.fsi.path,
                            "--nologo", "--noninteractive", `--langversion:${c.languageVersion}`,
                            `--optimize${c.configuration === "Release" ? "+" : "-"}`, "--exec"]) &&
                        last.args.at(-1) === filePath.join(directory, c.command[1]),
                    "FSI command, flags or local inventory mismatch");
                    if (isCompile) execution = { ...execution,
                        stderr: execution.stderr.split(`${directory}${filePath.sep}`).join("").replace(/^\n+/, "") };
                } else {
                    const rows = record.projects;
                    requireValue(Array.isArray(rows) && rows.length > 0 && rows.some(row =>
                        row.project === filePath.join(directory, c.project) && row.targetFramework === c.tfm &&
                        row.configuration === c.configuration), "missing exact project matrix cell");
                    for (const row of rows) {
                        requireValue(row.complete && row.fsc.length === 1 && within(directory, row.project) &&
                            manifest.sources.some(s => filePath.join(directory, s.path) === row.project) &&
                            row.sdkVersion === acquisition.sdkVersion && row.hostPath === acquisition.hostPath,
                        "project/SDK identity mismatch");
                        const compiler = row.fsc[0];
                        const declared = manifest.matrix.filter(m => m.project && filePath.join(directory, m.project) === row.project &&
                            m.configuration === c.configuration);
                        const expected = declared.find(m => m.tfm === row.targetFramework) ??
                            (declared.length === 0 ? c : null);
                        requireValue(expected && row.targetFramework === expected.tfm &&
                            row.configuration === expected.configuration && row.languageVersion === expected.languageVersion &&
                            row.projectReferences.every(p => rows.some(dependency =>
                                dependency.project === filePath.resolve(filePath.dirname(row.project), p))),
                        "referenced project matrix/flags mismatch");
                        requireValue(compiler.compilerPath === roles.compiler.path && compiler.taskAssembly === roles.tasks.path &&
                            compiler.targetsPath === roles.targets.path && compiler.hostPath === acquisition.hostPath &&
                            compiler.commandLine && compiler.languageVersion === row.languageVersion &&
                            compiler.optimize === row.optimize && compiler.nullable === row.nullable &&
                            equal(compiler.defines.toSorted(), row.defines.toSorted()), "compiler/task/flags fallback");
                        for (const imported of ["props", "targets", "sdkTargets", "overrides"])
                            requireValue(row.imports.includes(roles[imported].path), "local targets import fallback");
                        for (const name of ["Sdk.props", "Sdk.targets"])
                            requireValue(row.imports.includes(filePath.join(acquisition.sdkBasePath, "Sdks", "Microsoft.NET.Sdk", "Sdk", name)),
                                "SDK import fallback");
                        requireValue(row.core.package.id === "FSharp.Core" && row.core.package.version === product.corePackageVersion &&
                            row.core.package.sha256 === roles.corePackage.sha256 &&
                            row.core.package.root === filePath.join(directory, ".runner", "nuget", "packages") &&
                            compiler.references.filter(p => filePath.basename(p).toLowerCase() === "fsharp.core.dll").length === 1 &&
                            compiler.references.includes(row.core.compile.path), "Core package/compiler reference fallback");
                        const packageDirectory = filePath.join(directory, ".runner", "nuget", "packages",
                            "fsharp.core", product.corePackageVersion.toLowerCase());
                        for (const asset of [row.core.compile, row.core.runtime])
                            requireValue(product.coreAssets.some(a =>
                                filePath.join(packageDirectory, a.path) === asset.path && a.sha256 === asset.sha256),
                            "Core asset/cache fallback");
                        for (const producer of identities.producers) {
                            requireValue(!row.targetPath || filePath.basename(row.targetPath) !== filePath.basename(producer.dll.path),
                                "immutable producer was rebuilt");
                        }
                    }
                    for (const producer of identities.producers)
                        requireValue(rows.some(row => row.fsc.some(compiler => compiler.references.some(reference =>
                            filePath.basename(reference) === filePath.basename(producer.dll.path) && within(directory, reference)))),
                        "immutable producer reference missing");
                    const row = rows.find(r => r.project === filePath.join(directory, c.project) && r.targetFramework === c.tfm);
                    requireValue(row.languageVersion === c.languageVersion && row.configuration === c.configuration &&
                        record.build?.complete && record.build.status === "completed" &&
                        record.compilerCoreLoads?.length === rows.length, "project execution/setup mismatch");
                    const buildCommand = record.commands[0];
                    requireValue(buildCommand.executable === acquisition.hostPath && buildCommand.args[0] === "build" &&
                        buildCommand.args[1] === row.project && buildCommand.args.includes(`-p:TargetFramework=${c.tfm}`) &&
                        buildCommand.args.includes(`-p:Configuration=${c.configuration}`) &&
                        buildCommand.args.includes(`-p:LangVersion=${c.languageVersion}`) &&
                        equal(buildCommand.result, record.build), "project command/matrix mismatch");
                    if (isCompile) {
                        requireValue(Array.isArray(record.diagnostics), "missing raw compiler diagnostics");
                        execution = diagnosticsExecution(last.result, record.diagnostics, directory, filePath);
                    } else {
                        requireValue(record.build.exitCode === 0 && last.args.at(-1) === row.targetPath &&
                            within(directory, row.targetPath), "project build/output fallback");
                        selectedRuntime = acquisition.runtimes.find(r => r.version === last.args[2] &&
                            r.name === "Microsoft.NETCore.App");
                        requireValue(selectedRuntime && (last.result.status !== "completed" ||
                            record.loadedCore?.runtimeVersion === selectedRuntime.version),
                            "sample runtime mismatch");
                        requireValue((record.runtimeProducers?.length ?? 0) === identities.producers.length &&
                            identities.producers.every(p => record.runtimeProducers.some(r =>
                                r.path === filePath.join(filePath.dirname(row.targetPath), filePath.basename(p.dll.path)) &&
                                r.sha256 === p.dll.sha256)), "runtime producer bytes mismatch");
                    }
                }
                const loads = [...(record.compilerCoreLoads ?? []), ...(record.loadedCore ? [record.loadedCore] : [])];
                const projectRow = record.projects?.find(row =>
                    row.project === filePath.join(directory, c.project) && row.targetFramework === c.tfm);
                requireValue(execution.status !== "completed" || !execution.complete ||
                    loads.length > 0 && loads.every(load => load.complete && load.assemblyName === "FSharp.Core" &&
                    (load.sha256 === roles.core.sha256 || load === record.loadedCore && manifest.kind === "project" &&
                        product.coreAssets.some(a => a.sha256 === load.sha256)) &&
                    load.assemblyVersion === roles.core.assemblyVersion &&
                    load.architecture.toLowerCase() === c.architecture &&
                    acquisition.runtimes.some(r => r.version === load.runtimeVersion &&
                        filePath.join(r.path, r.version) === load.runtimePath) &&
                    (manifest.kind === "fsi" ? load.path === roles.core.path :
                        load === record.loadedCore
                            ? [projectRow.core.runtime.path, filePath.join(filePath.dirname(projectRow.targetPath), "FSharp.Core.dll")].includes(load.path)
                            : load.path === filePath.join(filePath.dirname(roles.compiler.path), "FSharp.Core.dll"))),
                "loaded Core/runtime identity mismatch");
                requireValue(equal(attempt.execution, Object.fromEntries(
                    ["status", "complete", "exitCode", "stdout", "stderr"].map(k => [k, execution[k]]))),
                "assertion differs from measured command output");
                requireValue(Object.values(attempt.checks).every(v => v === true), "failed setup cannot supply product evidence");
                return structuredClone(attempt);
            });
            const sdk = { version: acquisition.sdkVersion, sha256: acquisition.archive.sha256, verified: true };
            return { id: c.id, command: c.command, environment: environment[c.id], sdk,
                runtime: { version: selectedRuntime.version, sha256: selectedRuntime.sha256, verified: true },
                artifacts: Object.fromEntries(["compiler", "tasks", "targets", "core"].map(role => [role, {
                    sha256: roles[role].sha256, origin: source ? "local-build" : "released-sdk",
                    sourceSha: source ? unit.request.target.sourceSha : null, buildInstanceId: reservation.id,
                    ...(roles[role].assemblyVersion ? { assemblyVersion: roles[role].assemblyVersion } : {}),
                }])), producer: manifest.producer, attempts, outcome: classifyAttempts(attempts, manifest),
                ...(source ? { buildToolchain } : { releasedToolchain: { sdk,
                    compilerRuntime: { version: acquisition.compilerRuntime.version,
                        sha256: acquisition.runtimes.find(r => r.name === acquisition.compilerRuntime.name &&
                            r.version === acquisition.compilerRuntime.version).sha256, verified: true }, archive: acquisition.archive } }),
            };
        });
    }
    const outcome = cells.find(c => c.outcome.outcome === "untestable")?.outcome ??
        (cells.every(c => equal(c.outcome, cells[0].outcome)) ? cells[0].outcome : untestable("flaky"));
    const observation = {
        schemaVersion: 1, kind: unit.request.kind, target: unit.request.target,
        manifestDigest: fingerprint(manifest), inputDigest: manifest.approvedInputDigest,
        discussionFingerprint: manifest.discussionFingerprint, matrixFingerprint: fingerprint(manifest.matrix),
        producerFingerprint: fingerprint(manifest.producer), packageFingerprint: fingerprint(manifest.packages),
        buildInstanceId: reservation.id, cached: false, cells, outcome,
        provenance: { workerId: launch.launchId, runId: reservation.owner.runId,
            workflowRevision: context.workflowRevision, policyRevision: context.policyRevision },
    };
    validateObservation(observation, manifest);
    return observation;
}

function createObservationCollector({ evidenceDirectory, store }) {
    requireValue(path.isAbsolute(evidenceDirectory) &&
        ["writeEvidence", "readEvidence", "writeBlob", "verifyEvidence", "findEvidence"].every(k => typeof store?.[k] === "function"),
    "owned launch directory and controlled durable evidence store required");
    const signature = (body, key) => {
        requireValue(Buffer.isBuffer(key) && key.length >= 32, "collector authentication key required");
        return createHmac("sha256", key).update("regression-collection-v1\0").update(fingerprint(body)).digest();
    };
    return async function collectObservation({ result, reservation, manifest, context }) {
        requireValue(equal(manifest, context.manifest) && /^[\w-]{1,128}$/.test(reservation.id),
            "collector manifest or work identity mismatch");
        const references = await store.findEvidence({ runId: reservation.owner.runId,
            attempt: reservation.owner.attempt, buildInstanceId: reservation.id });
        requireValue(references.length <= 1, "conflicting durable collections for reservation");
        let collection, reference;
        if (references.length) {
            reference = references[0];
            const bytes = await store.readEvidence(reference.url);
            requireValue(Buffer.isBuffer(bytes) && bytes.length <= 64 * 1024 * 1024 &&
                hash(bytes) === reference.sha256, "required durable collection unavailable or changed");
            const envelope = JSON.parse(new TextDecoder("utf-8", { fatal: true }).decode(bytes));
            const { signature: actual, ...body } = envelope;
            requireValue(typeof actual === "string" && /^[a-f0-9]{64}$/.test(actual) &&
                timingSafeEqual(Buffer.from(actual, "hex"), signature(body, context.keys[body.keyId])),
            "durable collection authentication failed");
            requireValue(body.schemaVersion === 1 && Object.keys(body).sort().join(" ") ===
                "blobs files keyId receipt schemaVersion unit", "invalid durable collection schema");
            const files = body.files.map(file => {
                requireValue(Object.keys(file).sort().join(" ") === "bytesBase64 path sha256 size" &&
                    typeof file.bytesBase64 === "string", "invalid durable file schema");
                const bytes = Buffer.from(file.bytesBase64, "base64");
                requireValue(bytes.toString("base64") === file.bytesBase64, "invalid durable file encoding");
                const { bytesBase64, ...descriptor } = file;
                return { ...descriptor, bytes };
            });
            collection = { ...body, files };
        } else {
            const directory = path.join(evidenceDirectory, reservation.id);
            const unitFile = path.join(directory, "unit.json");
            const handle = await fs.open(unitFile, "r");
            let unit;
            try {
                const stat = await handle.stat();
                requireValue(stat.isFile() && stat.nlink === 1 && stat.size <= 16 * 1024 * 1024 &&
                    await fs.realpath(unitFile) === unitFile, "invalid retained execution unit");
                unit = JSON.parse(new TextDecoder("utf-8", { fatal: true }).decode(await handle.readFile()));
            } finally { await handle.close(); }
            validateExecutionUnit(unit);
            const local = await loadContainerEvidence(directory, unit, context.keys[context.keyId]);
            formObservation({ unit, launch: local.receipt.launch, files: local.files }, reservation, context);
            const binding = { workId: reservation.id, ...local.receipt.launch };
            const blobs = [];
            for (const blob of local.blobs) {
                const handle = await fs.open(blob.path, "r");
                try {
                    const stat = await handle.stat();
                    requireValue(stat.isFile() && stat.nlink === 1 && await fs.realpath(blob.path) === blob.path,
                        "retained archive is not a regular unlinked file");
                    const reference = await store.writeBlob(handle.createReadStream({ autoClose: false }), {
                        binding, request: { policy: context.policy, kind: blob.kind, sourceSha: blob.sourceSha,
                            product: blob.product, version: blob.version, platform: blob.platform, archive: blob.archive },
                        receipt: blob.archive,
                    });
                    blobs.push({ slot: blob.slot, reference });
                } finally { await handle.close(); }
            }
            const body = { schemaVersion: 1, keyId: context.keyId, unit, receipt: local.receipt,
                files: local.files.map(({ bytes, ...file }) => ({ ...file, bytesBase64: bytes.toString("base64") })), blobs };
            const bytes = Buffer.from(JSON.stringify({ ...body, signature: signature(body, context.keys[context.keyId]).toString("hex") }));
            requireValue(bytes.length <= 64 * 1024 * 1024, "durable collection exceeds byte limit");
            reference = await store.writeEvidence(bytes, binding);
            collection = { ...body, files: local.files };
        }
        const { unit, receipt, files, blobs } = collection;
        verifyTransportReceipt(receipt, unit, context.keys[collection.keyId]);
        requireValue(equal(files.map(({ path, size, sha256 }) => ({ path, size, sha256 })), receipt.files) &&
            files.every(file => Buffer.isBuffer(file.bytes) && file.bytes.length === file.size && hash(file.bytes) === file.sha256),
        "durable raw evidence differs from authenticated launch");
        const raw = readExecutionOutput(files, unit, receipt.launch);
        requireValue(result === undefined || equal(result, raw), "caller result differs from collected execution");
        const expectedArchives = [];
        for (const [scope, name] of [["reproduction", "acquisition.json"], ["build", "build-acquisition.json"]]) {
            const acquisition = JSON.parse(files.find(f => f.path === name).bytes);
            const identity = acquisition?.identity ?? acquisition;
            if (identity?.installations) for (const installation of identity.installations)
                expectedArchives.push({ scope, kind: identity.requested.kind,
                    sourceSha: identity.requested.sourceSha ?? null, product: installation.product,
                    version: installation.version, platform: unit.platform, archive: installation.archive });
        }
        requireValue(equal(expectedArchives, receipt.blobs.map(blob => ({
            scope: blob.scope, kind: blob.kind, sourceSha: blob.sourceSha, product: blob.product,
            version: blob.version, platform: blob.platform, archive: blob.archive,
        }))), "acquisition archives missing from authenticated transfer");
        requireValue(blobs.length === receipt.blobs.length && new Set(blobs.map(b => b.slot)).size === blobs.length,
            "missing or duplicate durable archives");
        for (const blob of blobs) {
            const expected = receipt.blobs.find(b => b.slot === blob.slot);
            const actual = await store.verifyEvidence(blob.reference.url);
            requireValue(expected && blob.reference.required === true && blob.reference.sha256 === expected.sha256 &&
                actual !== null && actual.sha256 === expected.sha256 && actual.sha512 === expected.sha512 &&
                actual.bytes === expected.size, "required archive unavailable or changed");
        }
        const evidence = [reference, ...new Map(blobs.map(b => [b.reference.url, b.reference])).values()];
        requireValue(evidence.length <= 16, "required durable evidence exceeds checkpoint limit");
        return { observation: formObservation({ unit, launch: receipt.launch, files }, reservation, context), evidence };
    };
}

async function commitCollectedObservation(input, { collectObservation, transport }) {
    const { loadCheckpoint, commitObservation } = require("./state.cjs");
    const loaded = await loadCheckpoint(transport, input.context);
    const record = loaded.checkpoint.records.find(r => r.id === input.reservation.id);
    requireValue(record && record.executionGeneration !== undefined &&
        equal(record.owner, input.reservation.owner) && record.requestDigest === input.reservation.requestDigest &&
        record.epoch === input.reservation.epoch, "missing or changed durable execution claim");
    const collected = await collectObservation(input);
    return commitObservation(transport, input.context, { generation: record.executionGeneration,
        attemptId: record.id, observation: collected.observation, evidence: collected.evidence });
}

module.exports = { formObservation, createObservationCollector, commitCollectedObservation };
