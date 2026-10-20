const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs/promises");
const path = require("node:path");
const { spawn, spawnSync } = require("node:child_process");
const { once } = require("node:events");
const { createHash } = require("node:crypto");
const { validateContainerConfig, inspectRuntime, runContainer, image, loadContainerEvidence,
    verifyTransportReceipt, verifyContainerEvidence, readCollectedExecution } = require("./container.cjs");
const { Readable } = require("node:stream");
const { executionEvidence } = require("./worker.cjs");
const { workerArchive } = require("./worker-output.cjs");

const config = () => ({
    schemaVersion: 1, image, dockerExecutable: "/usr/bin/docker",
    limits: { cpus: 2, memoryMiB: 4096, pids: 128, scratchMiB: 2048, timeoutMs: 60000 },
});

test("container configuration pins the image and only permits reductions of finite limits", async t => {
    assert.equal(validateContainerConfig(config()), true);
    for (const [name, change] of Object.entries({
        tag: c => { c.image = "node:24-bookworm"; },
        otherImage: c => { c.image = "docker.io/library/node@sha256:" + "a".repeat(64); },
        command: c => { c.command = "arbitrary"; },
        socket: c => { c.dockerHost = "tcp://remote:2375"; },
        relative: c => { c.dockerExecutable = "docker"; },
        extraLimit: c => { c.limits.swap = 0; },
        cpu: c => { c.limits.cpus = 9; },
        memory: c => { c.limits.memoryMiB = 32769; },
        pids: c => { c.limits.pids = 1025; },
        disk: c => { c.limits.scratchMiB = 16385; },
        timeout: c => { c.limits.timeoutMs = 110 * 60000 + 1; },
        zero: c => { c.limits.timeoutMs = 0; },
    })) await t.test(name, () => {
        const value = config(); change(value);
        assert.throws(() => validateContainerConfig(value), /config|image|limit|executable/);
    });
});

test("runtime inspection fails closed before creating resources on unsupported hosts", async t => {
    for (const platform of ["win32", "darwin"]) await t.test(platform, async () => {
        await assert.rejects(inspectRuntime(config(), { platform, command: () => assert.fail("no Docker launch") }), /Linux/);
    });
    const info = {
        OSType: "linux", Architecture: "x86_64", ServerVersion: "29.0.0", CgroupVersion: "2",
        SecurityOptions: ["name=seccomp,profile=builtin", "name=apparmor"],
        MemoryLimit: true, SwapLimit: true, PidsLimit: true, CpuCfsQuota: true,
    };
    for (const [name, change] of Object.entries({
        oldDocker: i => { i.ServerVersion = "27.5.0"; },
        cgroup: i => { i.CgroupVersion = "1"; },
        rootless: i => { i.SecurityOptions.push("name=rootless"); },
        seccomp: i => { i.SecurityOptions = ["name=apparmor"]; },
        apparmor: i => { i.SecurityOptions = ["name=seccomp,profile=builtin"]; },
        memory: i => { i.MemoryLimit = false; },
        swap: i => { i.SwapLimit = false; },
        pids: i => { i.PidsLimit = false; },
        cpu: i => { i.CpuCfsQuota = false; },
    })) await t.test(name, async () => {
        const value = structuredClone(info); change(value);
        await assert.rejects(inspectRuntime(config(), {
            platform: "linux", command: async () => ({ stdout: Buffer.from(JSON.stringify(value)), exitCode: 0 }),
        }), /runtime/);
    });
    assert.deepEqual(await inspectRuntime(config(), {
        platform: "linux", command: async () => ({ stdout: Buffer.from(JSON.stringify(info)), exitCode: 0 }),
    }), info);
});

test("unsupported production launches retain an explicit blocked receipt and create no resources", async t => {
    const root = await fs.realpath(await fs.mkdtemp(path.join(__dirname, ".container-boundary-")));
    t.after(() => fs.rm(root, { recursive: true, force: true }));
    const result = await runContainer(config(), {
        operation: "canary", input: { mode: "inspect" }, evidenceDirectory: root,
    }, { platform: "win32", command: () => assert.fail("unsupported platform must not launch") });
    assert.equal(result.status, "blocked");
    assert.match(result.reason, /Linux/);
    const receipt = JSON.parse(await fs.readFile(path.join(root, "receipt.json")));
    assert.equal(receipt.status, "blocked");
    assert.deepEqual(receipt.resources, []);
    assert.equal(receipt.image, image);
    assert.equal(receipt.workerStarted, false);
});

async function dockerFixture(t, failure) {
    const root = await fs.realpath(await fs.mkdtemp(path.join(__dirname, ".container-ledger-")));
    t.after(() => fs.rm(root, { recursive: true, force: true }));
    const resources = new Map(), calls = [];
    let inputDirectory, failed = false, execution, exported, acquisition;
    const run = async (executable, original, settings) => {
        assert.deepEqual(Object.keys(settings.env).sort(), original.includes("info")
            ? ["DOCKER_CONFIG", "HOME", "PATH"] : ["HOME", "PATH"]);
        assert.equal(original[0], "--host");
        assert.equal(original[1], "unix:///var/run/docker.sock");
        const args = original.slice(original[2] === "--config" ? 4 : 2);
        calls.push(args);
        const out = value => ({ exitCode: 0, stdout: Buffer.from(JSON.stringify(value)), stderr: Buffer.alloc(0) });
        if (args[0] === "info") return out({
            OSType: "linux", Architecture: "x86_64", ServerVersion: "29.0.0", CgroupVersion: "2",
            SecurityOptions: ["name=seccomp,profile=builtin", "name=apparmor"],
            MemoryLimit: true, SwapLimit: true, PidsLimit: true, CpuCfsQuota: true,
        });
        if (args[0] === "image") return out([{
            Os: "linux", Architecture: "amd64", Id: "sha256:" + "c".repeat(64), RepoDigests: [image],
        }]);
        const label = "org.dotnet.fsharp.regression-launch";
        if (args[1] === "create") {
            const name = args[0] === "network" ? args.at(-1) : args[args.indexOf("--name") + 1];
            const labels = { [label]: args[args.indexOf("--label") + 1].split("=")[1] };
            if (args[0] === "network") resources.set(name, {
                Driver: "bridge", Internal: args.includes("--internal"), EnableIPv6: false, Labels: labels,
                Options: { "com.docker.network.bridge.gateway_mode_ipv4": "isolated" },
            });
            else {
                const network = args[args.indexOf("--network") + 1];
                assert.equal(args.includes("--privileged"), false);
                const user = args[args.indexOf("--user") + 1];
                assert.ok(["1000:1000", "0:0"].includes(user));
                assert.equal(args[args.indexOf("--cap-drop") + 1], "ALL");
                assert.ok(args.includes("no-new-privileges=true"));
                assert.ok(args.includes("--read-only"));
                const mounts = args.filter(a => a.startsWith("type=bind,"));
                assert.ok(mounts.every(m => m.endsWith(",readonly")));
                assert.ok(mounts.every(m => !/docker.sock|target=\/home/.test(m)));
                if (name.endsWith("-worker")) {
                    inputDirectory = mounts.find(m => m.includes("target=/input")).split(",")[1].slice("source=".length);
                    assert.ok(args.includes("--dns"));
                    assert.equal(args.includes("RB_OUTER_CANARY"), false);
                }
                resources.set(name, {
                    Id: name, Image: "sha256:" + "c".repeat(64), Config: { User: user, Labels: labels },
                    AppArmorProfile: "docker-default",
                    HostConfig: { ReadonlyRootfs: true, Privileged: false, PidMode: "", NetworkMode: network, CapDrop: ["ALL"],
                        IpcMode: "private", CgroupnsMode: "private", SecurityOpt: ["no-new-privileges=true"],
                        CapAdd: args.flatMap((v, i) => v === "--cap-add" ? [args[i + 1]] : []),
                        Memory: Number(args[args.indexOf("--memory") + 1].slice(0, -1)) * 1024 * 1024,
                        MemorySwap: Number(args[args.indexOf("--memory-swap") + 1].slice(0, -1)) * 1024 * 1024,
                        PidsLimit: Number(args[args.indexOf("--pids-limit") + 1]),
                        NanoCpus: Number(args[args.indexOf("--cpus") + 1]) * 1e9, Dns: ["127.0.0.1"] },
                    Mounts: mounts.map(m => ({ Type: "bind", RW: false,
                        Source: m.split(",")[1].slice("source=".length), Destination: m.split(",")[2].slice("target=".length) })),
                    NetworkSettings: { Networks: { [network]: { IPAddress: "172.30.0.2" } } },
                    State: { Running: false, ExitCode: 0, OOMKilled: false },
                });
            }
            if (failure === `create-${name.split("-").at(-1)}` && !failed) {
                failed = true;
                throw new Error("lost create acknowledgement");
            }
            return out(name);
        }
        if (args[1] === "inspect") {
            const value = resources.get(args[2]);
            if (failure === "ownership" && args[2].endsWith("-worker") && value)
                value.Config.Labels[label] = "not-this-launch";
            return value ? out([value]) : { exitCode: 1, stdout: Buffer.alloc(0), stderr: Buffer.from(args[0] === "network"
                ? `Error response from daemon: network ${args[2]} not found\n` : "No such object: " + args[2]) };
        }
        if (args[1] === "connect") {
            resources.get(args[3]).NetworkSettings.Networks[args[2]] = { IPAddress: "172.30.0.2" };
            if (failure === "connect" && !failed) { failed = true; throw new Error("lost connect acknowledgement"); }
            return out(null);
        }
        if (args[1] === "start" && !args.includes("--attach")) {
            resources.get(args[2]).State.Running = true;
            if (failure === "start-broker" && !failed) { failed = true; throw new Error("lost broker start acknowledgement"); }
            return out(null);
        }
        if (args[1] === "exec") {
            if (failure === "ready") throw new Error("broker not responsive");
            const operation = args.at(-2) === "blob" ? "blob" : args.at(-1);
            if (["collect", "archive", "blob"].includes(operation)) {
                assert.deepEqual(args.slice(2, 4), ["--user", "0:0"]);
                const worker = resources.get(args[4]);
                assert.equal(worker.State.Running, true, "STOP would have destroyed tmpfs before export");
                const job = JSON.parse(await fs.readFile(path.join(inputDirectory, "input.json")));
                if (operation === "collect") {
                    exported = await executionEvidence(job.input, job.launch, execution, {
                        evidenceDirectory: path.join(root, "worker-evidence"), scratchDirectory: root, acquisition,
                    });
                    return out({ schemaVersion: 1, ...job.launch, historicalTerminated: true,
                        files: exported.files.map(({ path, size, sha256 }) => ({ path, size, sha256 })), blobs: exported.blobs });
                }
                if (operation === "blob") {
                    const blob = exported.blobs.find(b => b.slot === args.at(-1));
                    return { ...out(null), collected: await settings.consume(Readable.from([
                        await fs.readFile(blob.archive.retained.path),
                    ])) };
                }
                return { ...out(null), collected: await settings.consume(Readable.from(workerArchive(exported.files))) };
            }
            return out(null);
        }
        if (args[1] === "start") {
            if (failure === "timeout") throw new Error("worker timeout");
            const job = JSON.parse(await fs.readFile(path.join(inputDirectory, "input.json")));
            assert.deepEqual(Object.keys(job).sort(), ["hostAddresses", "input", "launch", "limits", "operation", "proxy", "schemaVersion"]);
            if (failure === "binding") job.launch.launchId = "f".repeat(64);
            if (failure === "oom") resources.get(args.at(-1)).State.OOMKilled = true;
            return out({ schemaVersion: 1, ...job.launch, result: execution ?? { mode: "inspect" } });
        }
        if (args[1] === "rm") {
            resources.delete(args.at(-1));
            return out(null);
        }
        assert.fail(`unexpected Docker command: ${args}`);
    };
    return { root, calls, resources, setResult: value => { execution = value; },
        setAcquisition: value => { acquisition = value; }, deps: { platform: "linux", command: run } };
}

function executionUnit() {
    const contracts = require("./fixtures/contracts.cjs");
    const { fingerprintInput: fingerprint } = require("./model.cjs");
    const manifest = contracts.manifest();
    return {
        schemaVersion: 1, repository: manifest.repository, issueNumber: manifest.issueNumber,
        epoch: contracts.digest("epoch"), buildInstanceId: "physical-source", deadline: new Date(Date.now() + 60000).toISOString(),
        request: { kind: "source", target: { sourceSha: contracts.sha("main") }, endpoint: "main", phase: "main" },
        manifest, platform: { os: "linux", architecture: "x64", rid: "linux-x64" },
        stableSdk: contracts.observation("released", "pass").target.sdk, resolved: {}, policyDigest: fingerprint(contracts.policy()),
        policyRevision: contracts.sha("policy"), workflowRevision: contracts.sha("workflow"),
        environmentFingerprint: fingerprint(Object.fromEntries(manifest.matrix.map(c =>
            [c.id, { os: c.os, architecture: c.architecture, imageDigest: image.split(":").at(-1) }]))),
    };
}

test("source bundle handoff is bounded, digest-bound and staged before worker creation", async t => {
    for (const scenario of ["representative", "ceiling", "oversize", "empty", "missing-path", "missing-digest",
        "missing-both", "wrong-digest", "hardlink", "symlink", "copy-change", "copy-oversize", "aborted"]) await t.test(scenario, async t => {
        const f = await dockerFixture(t);
        const sourceDirectory = path.join(f.root, "samples");
        const sourceBundle = path.join(f.root, "source.bundle");
        const size = scenario === "representative" ? 401775766 : scenario === "ceiling" ? 1024 ** 3
            : scenario === "oversize" ? 1024 ** 3 + 1 : scenario === "empty" ? 0 : 16;
        const file = await fs.open(sourceBundle, "wx");
        try { await file.truncate(size); } finally { await file.close(); }
        const measure = async name => {
            const file = await fs.open(name, "r"), digest = createHash("sha256");
            try {
                for await (const chunk of file.createReadStream({ autoClose: false, highWaterMark: 1024 * 1024 })) digest.update(chunk);
                return digest.digest("hex");
            } finally { await file.close(); }
        };
        const sourceBundleSha256 = scenario === "oversize" ? "a".repeat(64) : await measure(sourceBundle);
        const unit = executionUnit();
        await fs.mkdir(path.join(sourceDirectory, "fixtures"), { recursive: true });
        await fs.writeFile(path.join(sourceDirectory, unit.manifest.sources[0].path), 'printfn "ok"');
        const options = { operation: "execution", input: unit, evidenceDirectory: f.root, sourceDirectory,
            sourceBundle, sourceBundleSha256, receiptKey: Buffer.alloc(32, 7) };
        if (scenario === "missing-path" || scenario === "missing-both") delete options.sourceBundle;
        if (scenario === "missing-digest" || scenario === "missing-both") delete options.sourceBundleSha256;
        if (scenario === "wrong-digest") options.sourceBundleSha256 = "b".repeat(64);
        if (scenario === "aborted") options.signal = AbortSignal.abort(new Error("cancelled handoff"));
        if (scenario === "hardlink") await fs.link(sourceBundle, path.join(f.root, "alias.bundle"));
        if (scenario === "symlink") {
            const alias = path.join(f.root, "alias");
            await fs.symlink(f.root, alias, "junction");
            options.sourceBundle = path.join(alias, "source.bundle");
        }
        f.setResult({ status: "untestable", outcome: { schemaVersion: 1, outcome: "untestable", reason: "setup-failure" },
            error: { message: "synthetic bundle handoff only" } });
        const readFile = fs.readFile, copyFile = fs.copyFile;
        let copies = 0;
        t.mock.method(fs, "readFile", (file, ...args) => {
            assert.equal(String(file).endsWith(".bundle"), false, "bundles must not be buffered in controller memory");
            return readFile(file, ...args);
        });
        t.mock.method(fs, "copyFile", async (...args) => {
            copies++;
            await copyFile(...args);
            if (scenario === "copy-change") await fs.writeFile(args[1], Buffer.alloc(size, 1));
            if (scenario === "copy-oversize") await fs.truncate(args[1], 1024 ** 3 + 1);
        });
        const command = f.deps.command;
        let staged = false;
        f.deps.command = async (exe, args, settings) => {
            if (args.includes("image")) {
                const directory = (await fs.readdir(f.root)).find(n => n.startsWith("staging-"));
                const copy = path.join(f.root, directory, "input", "source.bundle");
                assert.equal((await fs.stat(copy)).size, size);
                assert.equal(await measure(copy), options.sourceBundleSha256);
                staged = true;
            }
            return command(exe, args, settings);
        };
        const result = await runContainer(config(), options, f.deps);
        const accepted = ["representative", "ceiling"].includes(scenario);
        assert.equal(result.status, accepted ? "collected" : "blocked", result.reason);
        assert.equal(staged, accepted);
        assert.equal(result.receipt.workerStarted, accepted);
        if (scenario === "oversize") {
            assert.match(result.reason, /bounded|byte limit/);
            assert.equal(copies, 0, "oversized input must be rejected before copying bytes");
        }
        if (accepted) assert.equal(result.receipt.sourceBundleSha256, options.sourceBundleSha256);
        else {
            assert.ok(result.reason);
            assert.deepEqual(result.receipt.resources, []);
            assert.equal(f.calls.some(args => args[1] === "create" || args[1] === "start"), false);
        }
        t.diagnostic(JSON.stringify({ scenario, size, status: result.status, workerStarted: result.receipt.workerStarted,
            commands: f.calls.map(args => args.slice(0, 2)) }));
    });
});

test("physical source evidence survives destroyed worker tmpfs, not merely a result containing paths", async t => {
    const f = await dockerFixture(t);
    const contracts = require("./fixtures/contracts.cjs");
    const { fingerprintInput: fingerprint } = require("./model.cjs");
    const unit = executionUnit(), manifest = unit.manifest;
    const scratch = path.join(f.root, "worker-tmpfs");
    await fs.mkdir(scratch);
    const archiveBytes = Buffer.from("SDK archive linux-x64");
    const archivePath = path.join(scratch, "sdk.tar.gz");
    await fs.writeFile(archivePath, archiveBytes);
    const sdkArchive = contracts.archiveReceipt({
        name: "dotnet-sdk-10.0.100-linux-x64.tar.gz",
        url: "https://builds.dotnet.microsoft.com/dotnet/Sdk/10.0.100/dotnet-sdk-10.0.100-linux-x64.tar.gz",
        rid: "linux-x64", architecture: "x64", provenance: { kind: "publisher-checksum" },
    }, "/scratch/sdk.tar.gz", archiveBytes);
    sdkArchive.sha512 = sdkArchive.actualSha512;
    // The offline worker filesystem is Windows; only its raw transfer stream crosses this seam.
    f.setAcquisition({ status: "acquired", identity: {
        requested: { kind: "released", sdkVersion: "10.0.100", platform: unit.platform },
        installations: [{ product: "sdk", version: "10.0.100", archive: sdkArchive }],
    } });
    const invoke = f.deps.command;
    f.deps.command = async (exe, args, settings) => args.at(-2) === "blob"
        ? { exitCode: 0, stdout: Buffer.alloc(0), stderr: Buffer.alloc(0),
            collected: await settings.consume(Readable.from([await fs.readFile(archivePath)])) }
        : invoke(exe, args, settings);
    const identities = Buffer.from(JSON.stringify({ product: { kind: "source", sourceSha: unit.request.target.sourceSha,
        buildInstanceId: unit.buildInstanceId, files: [] }, producers: [] }));
    const identityPath = path.join(scratch, "identities.json");
    await fs.writeFile(identityPath, identities);
    const outcome = { schemaVersion: 1, outcome: "pass", reason: "positive-assertion" };
    const reproduction = { outcome, cells: [{ id: manifest.matrix[0].id,
        attempts: [contracts.attempt("pass"), contracts.attempt("pass")], evidence: [] }],
        workDirectory: scratch, identities: { path: identityPath, sha256: contracts.digest(identities) },
        manifestDigest: fingerprint(manifest), productFingerprint: fingerprint(JSON.parse(identities).product) };
    const sourceResult = { status: "tested", outcome, reproduction, actualSourceSha: unit.request.target.sourceSha,
        buildInstanceId: unit.buildInstanceId, requested: unit.request, product: JSON.parse(identities).product,
        commands: [], evidence: [], globalJson: { before: contracts.digest("global"), after: contracts.digest("global") } };
    f.setResult(sourceResult);
    const exportSettings = { evidenceDirectory: path.join(f.root, "worker-evidence"), scratchDirectory: f.root };
    const linked = path.join(scratch, "linked.json");
    await fs.link(identityPath, linked);
    await assert.rejects(executionEvidence(unit, {}, sourceResult, exportSettings), /unlinked/);
    await fs.unlink(linked);
    const alias = path.join(f.root, "scratch-link");
    await fs.symlink(scratch, alias, "junction");
    const redirected = structuredClone(sourceResult);
    redirected.reproduction.workDirectory = alias;
    redirected.reproduction.identities.path = path.join(alias, "identities.json");
    await assert.rejects(executionEvidence(unit, {}, redirected, exportSettings), /links/);
    await fs.unlink(alias);
    const samples = path.join(f.root, "samples");
    await fs.mkdir(path.join(samples, "fixtures"), { recursive: true });
    await fs.writeFile(path.join(samples, manifest.sources[0].path), 'printfn "ok"');
    const bundle = path.join(f.root, "source.bundle");
    await fs.writeFile(bundle, "source");
    const result = await runContainer(config(), {
        operation: "execution", input: unit, evidenceDirectory: f.root, sourceDirectory: samples,
        sourceBundle: bundle, sourceBundleSha256: contracts.digest("source"), receiptKey: Buffer.alloc(32, 7),
    }, f.deps);
    await fs.rm(scratch, { recursive: true });
    assert.equal(result.status, "collected", result.reason);
    assert.deepEqual(result.files?.find(file => file.path === "identities.json")?.bytes, identities,
        "required raw identities must remain available after tmpfs destruction");
    assert.equal(verifyTransportReceipt(result.receipt, unit, Buffer.alloc(32, 7)), true);
    assert.equal(verifyContainerEvidence(result, unit, Buffer.alloc(32, 7)), true);
    const substituted = { ...result, files: result.files.map(f => ({ ...f })) };
    substituted.files[0].bytes = Buffer.from(substituted.files[0].bytes.toString().replace('"pass"', '"product-failure"'));
    substituted.files[0].size = substituted.files[0].bytes.length;
    substituted.files[0].sha256 = contracts.digest(substituted.files[0].bytes);
    assert.throws(() => verifyContainerEvidence(substituted, unit, Buffer.alloc(32, 7)), /authenticated transfer/);
    assert.throws(() => verifyContainerEvidence({ ...result, blobs: result.blobs.map(b => ({ ...b, path: "outside.blob" })) },
        unit, Buffer.alloc(32, 7)), /authenticated storage/);
    const reloaded = await loadContainerEvidence(f.root, unit, Buffer.alloc(32, 7));
    assert.deepEqual(reloaded.result, result.result);
    assert.deepEqual(reloaded.files, result.files);
    assert.deepEqual(await fs.readFile(reloaded.blobs[0].path), archiveBytes);
    const collected = await readCollectedExecution(f.root, { keys: { fixture: Buffer.alloc(32, 7) }, keyId: "fixture" });
    assert.deepEqual(collected.unit, unit);
    assert.deepEqual(collected.launch, result.receipt.launch);
    assert.deepEqual(collected.result, result.result);
    await assert.rejects(readCollectedExecution(f.root, { keys: { fixture: Buffer.alloc(32, 8) }, keyId: "fixture" }), /authentication/);
    for (const [name, mutate] of Object.entries({
        image: r => { r.launch.imageDigest = "a".repeat(64); },
        unit: r => { r.launch.unitDigest = "a".repeat(64); },
        helper: r => { r.helperDigest = "a".repeat(64); },
        hash: r => { r.files[0].sha256 = "a".repeat(64); },
        termination: r => { r.historicalTerminated = false; },
    })) {
        const changed = structuredClone(result.receipt); mutate(changed);
        assert.throws(() => verifyTransportReceipt(changed, unit, Buffer.alloc(32, 7)), /authentication/, name);
    }
    assert.throws(() => verifyTransportReceipt(result.receipt, unit, Buffer.alloc(32, 8)), /authentication/);
    const child = spawnSync(process.execPath, ["-e", `
        require(${JSON.stringify(path.join(__dirname, "container.cjs"))}).loadContainerEvidence(
            ${JSON.stringify(f.root)}, ${JSON.stringify(unit)}, Buffer.alloc(32, 7))
            .then(x => process.stdout.write(x.files.find(f => f.path === "identities.json").sha256))
            .catch(e => { console.error(e.message); process.exitCode = 1; });
    `], { encoding: "utf8", timeout: 10000 });
    assert.equal(child.status, 0, child.stderr);
    assert.equal(child.stdout, contracts.digest(identities));
    assert.equal(f.calls.some(args => args.includes("--attach")), false, "execution control is not sample stdout");
    assert.equal(f.calls.some(args => args[1] === "cp" || args[1] === "stop"), false);
    await fs.appendFile(path.join(f.root, "identities.json"), " ");
    await assert.rejects(loadContainerEvidence(f.root, unit, Buffer.alloc(32, 7)), /digest|size/);
    t.diagnostic(JSON.stringify({ transferredBytes: result.receipt.transferredBytes,
        files: result.receipt.files, blobs: result.receipt.blobs.map(({ slot, size, sha256 }) => ({ slot, size, sha256 })) }));
    t.diagnostic("offline Docker transport fixture; zero real launches");
});

test("execution export failures block authentication and remove owned resources", async t => {
    const roots = new Set();
    for (const fault of ["collect", "binding", "archive", "truncated", "blob", "blob-digest", "cleanup",
        ...(process.platform === "win32" ? ["receipt-sharing", "receipt-sharing-exhausted"] : []), "receipt-error"])
        await t.test(fault, async t => {
            const f = await dockerFixture(t), unit = executionUnit();
            assert.equal(roots.has(f.root), false, "receipt fixtures must not share paths");
            roots.add(f.root);
            const handles = new Set(), open = fs.open, rename = fs.rename;
            let renameAttempts = 0, holder, holderExit, expiry;
            t.mock.method(fs, "open", async (...args) => {
                const handle = await open(...args), close = handle.close.bind(handle);
                handles.add(handle);
                t.mock.method(handle, "close", async () => { await close(); handles.delete(handle); });
                return handle;
            });
            if (fault === "receipt-sharing-exhausted") {
                const timeout = AbortSignal.timeout;
                t.mock.method(AbortSignal, "timeout", ms => {
                    if (ms !== 5000) return timeout(ms);
                    expiry = new AbortController();
                    return expiry.signal;
                });
            }
            t.mock.method(fs, "rename", async (from, to) => {
                assert.equal(from, path.join(f.root, "receipt.new"));
                assert.equal(to, path.join(f.root, "receipt.json"));
                assert.equal(handles.size, 0, "all owned handles must close before receipt replacement");
                renameAttempts++;
                if (fault.startsWith("receipt-sharing") && !holder) {
                    holder = spawn("powershell.exe", ["-NoProfile", "-NonInteractive", "-Command",
                        "$h=[IO.File]::Open($env:RECEIPT_PATH,[IO.FileMode]::Open,[IO.FileAccess]::Read,[IO.FileShare]::ReadWrite); " +
                        "[Console]::WriteLine('locked'); [Console]::In.ReadLine() > $null; $h.Dispose()"],
                    { env: { ...process.env, RECEIPT_PATH: to }, stdio: ["pipe", "pipe", "pipe"] });
                    holderExit = once(holder, "exit");
                    assert.match((await once(holder.stdout, "data"))[0].toString(), /locked/);
                }
                if (fault === "receipt-error") throw Object.assign(new Error("injected receipt EIO"), { code: "EIO" });
                try { return await rename(from, to); }
                catch (error) {
                    t.diagnostic(JSON.stringify({ fault, renameAttempts, code: error.code, ownedHandles: handles.size, from, to }));
                    assert.equal(error.code, "EPERM", "controlled Windows delete-sharing violation");
                    const initial = JSON.parse(await fs.readFile(to));
                    assert.equal(initial.status, "blocked", "initial receipt remains authoritative");
                    assert.equal(initial.signature, undefined);
                    if (fault === "receipt-sharing") {
                        holder.stdin.end("\n");
                        await holderExit;
                    } else if (fault === "receipt-sharing-exhausted") {
                        expiry?.abort(new DOMException("receipt deadline", "TimeoutError"));
                    }
                    throw error;
                }
            });
            const contracts = require("./fixtures/contracts.cjs");
            const bytes = Buffer.from("approved archive");
            const archive = contracts.archiveReceipt({
                name: "dotnet-sdk-10.0.100-linux-x64.tar.gz", rid: "linux-x64", architecture: "x64",
                url: "https://builds.dotnet.microsoft.com/dotnet/Sdk/10.0.100/dotnet-sdk-10.0.100-linux-x64.tar.gz",
                provenance: { kind: "publisher-checksum" },
            }, "/scratch/sdk.tar.gz", bytes);
            archive.sha512 = archive.actualSha512;
            unit.stableSdk.archives[0].sha256 = archive.sha256;
            f.setAcquisition({ status: "acquired", identity: {
                requested: { kind: "released", sdkVersion: unit.stableSdk.version },
                installations: [{ product: "sdk", version: unit.stableSdk.version, archive }],
            } });
            f.setResult({ outcome: { schemaVersion: 1, outcome: "untestable", reason: "setup-failure" }, error: "setup" });
            const source = path.join(f.root, "samples");
            await fs.mkdir(path.join(source, "fixtures"), { recursive: true });
            await fs.writeFile(path.join(source, unit.manifest.sources[0].path), 'printfn "ok"');
            const bundle = path.join(f.root, "source.bundle");
            await fs.writeFile(bundle, "source");
            const invoke = f.deps.command;
            let blobReached = false;
            f.deps.command = async (exe, args, settings) => {
                const operation = args.at(-2) === "blob" ? "blob" : args.at(-1);
                if (operation === "blob") blobReached = true;
                if (fault === operation || fault === "cleanup" && args.includes("rm") && args.at(-1).endsWith("-worker"))
                    throw new Error(`injected ${fault} failure`);
                if (operation === "blob") return { exitCode: 0, stdout: Buffer.alloc(0), stderr: Buffer.alloc(0),
                    collected: await settings.consume(Readable.from([
                        fault === "blob-digest" || fault.startsWith("receipt-") ? Buffer.alloc(bytes.length) : bytes])) };
                if (operation === "archive" && fault === "truncated")
                    return { collected: await settings.consume(Readable.from([Buffer.alloc(512)])) };
                const result = await invoke(exe, args, settings);
                if (operation === "collect" && fault === "binding") {
                    const descriptor = JSON.parse(result.stdout);
                    descriptor.launchId = "f".repeat(64);
                    result.stdout = Buffer.from(JSON.stringify(descriptor));
                }
                return result;
            };
            const key = Buffer.alloc(32, 7);
            let result, publicationError;
            try { result = await runContainer(config(), { operation: "execution", input: unit,
                evidenceDirectory: f.root, sourceDirectory: source, sourceBundle: bundle,
                sourceBundleSha256: contracts.digest("source"), receiptKey: key }, f.deps); }
            catch (error) { publicationError = error; }
            finally {
                if (holder) { holder.stdin.end("\n"); await holderExit; }
            }
            if (["receipt-sharing-exhausted", "receipt-error"].includes(fault)) {
                assert.match(publicationError?.message ?? "", /receipt.*(deadline|EIO)/);
                assert.match(publicationError.message, /archive blob digest or size mismatch/, "publication must not mask the export failure");
                assert.equal(result, undefined);
                assert.equal(f.resources.size, 0);
                await assert.rejects(loadContainerEvidence(f.root, unit, key), /authentication|termination/);
                assert.equal(renameAttempts, 1, "no retries after expiry or for non-sharing errors");
                return;
            }
            assert.equal(publicationError, undefined, publicationError?.stack);
            assert.equal(result.status, "blocked");
            assert.match(result.reason, {
                collect: /injected collect failure/, binding: /protected export launch binding mismatch/,
                archive: /injected archive failure/, truncated: /truncated worker archive/,
                blob: /injected blob failure/, "blob-digest": /archive blob digest or size mismatch/,
                "receipt-sharing": /archive blob digest or size mismatch/,
                cleanup: /injected cleanup failure/,
            }[fault]);
            assert.equal(blobReached, ["blob", "blob-digest", "cleanup", "receipt-sharing"].includes(fault));
            assert.ok(renameAttempts >= (fault === "receipt-sharing" ? 2 : 1));
            assert.throws(() => verifyTransportReceipt(result.receipt, unit, key), /termination mismatch/);
            await assert.rejects(loadContainerEvidence(f.root, unit, key), /termination mismatch/);
            assert.equal(f.resources.size, fault === "cleanup" ? 1 : 0);
            assert.equal(f.calls.some(args => args.includes("--attach") || args[1] === "stop" || args[1] === "cp"), false);
            t.diagnostic(JSON.stringify({ fault, realLaunches: 0, reason: result.reason, cleanup: result.receipt.cleanup }));
        });
});

test("Docker transactions collect only stopped launch-bound output and clean every owned resource", async t => {
    for (const failure of [null, "create-private", "create-public", "create-broker", "create-worker",
        "connect", "start-broker", "ready", "timeout", "binding", "oom"])
        await t.test(failure ?? "completed", async t => {
            const f = await dockerFixture(t, failure);
            const result = await runContainer(config(), {
                operation: "canary", input: { mode: "inspect" }, evidenceDirectory: f.root,
            }, f.deps);
            assert.equal(result.status, failure ? "blocked" : "collected", result.reason);
            assert.ok(result.receipt.helpers.some(f => f.path === "global.json"));
            assert.ok(result.receipt.helpers.some(f => f.path === "eng/common/pipeline-logging-functions.sh"));
            for (const args of f.calls.filter(a => a[0] === "container" && a[1] === "create"))
                assert.ok(args.includes("/trusted/artifacts:rw,nosuid,nodev,noexec,size=16m,uid=1000,gid=1000,mode=0700"));
            assert.equal(f.resources.size, 0);
            assert.ok(result.receipt.cleanup.every(r => r.removed));
            if (!failure) assert.deepEqual(result.result, { mode: "inspect" });
            else await assert.rejects(fs.access(path.join(f.root, "result.json")), { code: "ENOENT" });
            assert.equal(f.calls.filter(a => a.includes("--attach")).length,
                failure?.startsWith("create-") || ["connect", "start-broker", "ready"].includes(failure) ? 0 : 1);
            t.diagnostic(JSON.stringify({ failure, commands: f.calls, cleanup: result.receipt.cleanup }));
        });
});

test("cleanup refuses a substituted ownership label instead of deleting a foreign resource", async t => {
    const f = await dockerFixture(t, "ownership");
    const result = await runContainer(config(), {
        operation: "canary", input: { mode: "inspect" }, evidenceDirectory: f.root,
    }, f.deps);
    assert.equal(result.status, "blocked");
    assert.match(result.reason, /not owned/);
    assert.equal(f.resources.size, 1);
    assert.equal(f.calls.some(args => args.includes("--attach")), false);
    assert.equal(f.calls.some(args => args[1] === "rm" && args.at(-1).endsWith("-worker")), false);
    assert.equal(result.receipt.cleanup.find(r => r.name.endsWith("-worker")).removed, false);
});

test("network cleanup rejects permission errors and missing responses for another resource", async t => {
    for (const failure of ["permission denied", "Error response from daemon: network foreign not found",
        "No such object: foreign", "Error: No such network: foreign"]) {
        const f = await dockerFixture(t);
        const command = f.deps.command;
        f.deps.command = async (executable, args, settings) => {
            const operation = args.slice(args[2] === "--config" ? 4 : 2);
            if (operation[0] === "network" && operation[1] === "inspect" && !f.resources.has(operation[2]))
                return { exitCode: 1, stdout: Buffer.alloc(0), stderr: Buffer.from(failure) };
            return command(executable, args, settings);
        };
        const result = await runContainer(config(), {
            operation: "canary", input: { mode: "inspect" }, evidenceDirectory: f.root,
        }, f.deps);
        assert.equal(result.status, "blocked");
        assert.ok(result.reason.includes(failure));
        assert.ok(result.receipt.cleanup.some(r => r.kind === "network" && !r.removed));
    }
});

test("fixed child entry and opt-in smoke reject command-like arguments in fresh processes", async t => {
    for (const [file, args, code] of [
        ["container-entry.cjs", ["eval"], 1],
        ["container-entry.cjs", ["worker", "arbitrary.js"], 1],
        ["container-entry.cjs", ["broker", "extra"], 1],
        ["container-entry.cjs", ["ready", "extra"], 1],
        ["containment.smoke.cjs", [], 2],
        ["containment.smoke.cjs", ["--run", "relative.json", "relative-output"], 2],
    ]) await t.test(`${file} ${args.join(" ")}`, () => {
        const child = spawnSync(process.execPath, [path.join(__dirname, file), ...args], {
            encoding: "utf8", timeout: 10000, maxBuffer: 65536,
        });
        assert.equal(child.status, code, child.stderr);
        assert.match(child.stderr, /fixed container entry|usage:/);
        assert.equal(child.stdout, "");
    });
});
