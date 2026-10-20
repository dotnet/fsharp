const fs = require("node:fs/promises");
const path = require("node:path");
const { spawn } = require("node:child_process");
const { setTimeout: delay } = require("node:timers/promises");
const { randomBytes, createHmac, timingSafeEqual } = require("node:crypto");
const { validateRepositoryPath, fingerprintInput: fingerprint } = require("./model.cjs");
const { validateExecutionUnit, readExecutionOutput } = require("./worker.cjs");
const { hash, requireValue, boundedWait } = require("./toolchain-io.cjs");
const { collectWorkerArchive, collectWorkerBlob, validateExecutionFiles } = require("./worker-output.cjs");
const { validateArchiveReceipt } = require("./archive-provenance.cjs");
const { hashFile } = require("./toolchain-installer.cjs");
const policy = require("../../.github/policies/regression-bisect.json");

const image = "docker.io/library/node@sha256:5a750d3be5e5c80275f8c9a5367c3aed99c2875656590c8d0701c7ee687f5f0a";
const sourceBundleBytes = 1024 * 1024 * 1024;
const ceilings = { cpus: 8, memoryMiB: 32768, pids: 1024, scratchMiB: 16384, timeoutMs: 110 * 60000 };
const trustedRoot = path.resolve(__dirname, "..", "..");

function validateContainerConfig(c) {
    requireValue(c && Object.keys(c).sort().join(" ") === "dockerExecutable image limits schemaVersion" &&
        c.schemaVersion === 1 && c.image === image, "invalid pinned container config or image");
    requireValue(typeof c.dockerExecutable === "string" && c.dockerExecutable.startsWith("/") &&
        path.posix.normalize(c.dockerExecutable) === c.dockerExecutable && !/[\s,\\]/.test(c.dockerExecutable),
    "absolute Linux Docker executable required");
    requireValue(c.limits && Object.keys(c.limits).sort().join(" ") === Object.keys(ceilings).sort().join(" ") &&
        Object.entries(ceilings).every(([k, max]) => Number.isSafeInteger(c.limits[k]) && c.limits[k] > 0 && c.limits[k] <= max),
    "invalid container limit");
    return true;
}

function command(executable, args, { env, signal, timeoutMs = 30000, maxBytes = 1024 * 1024, consume } = {}) {
    return new Promise((resolve, reject) => {
        signal = AbortSignal.any([AbortSignal.timeout(timeoutMs), ...(signal ? [signal] : [])]);
        signal.throwIfAborted();
        const child = spawn(executable, args, { env, shell: false, stdio: ["ignore", "pipe", "pipe"] });
        const chunks = { stdout: [], stderr: [] }, sizes = { stdout: 0, stderr: 0 };
        let failure;
        const stop = error => { failure ??= error; child.kill("SIGKILL"); };
        const abort = () => stop(signal.reason);
        signal.addEventListener("abort", abort, { once: true });
        const consumed = consume ? Promise.resolve().then(() => consume(child.stdout)).catch(error => { stop(error); }) : null;
        for (const name of Object.keys(chunks).filter(n => !consume || n !== "stdout")) child[name].on("data", bytes => {
            sizes[name] += bytes.length;
            if (sizes[name] > maxBytes) stop(new Error("container command output limit exceeded"));
            else chunks[name].push(bytes);
        });
        child.once("error", error => { failure = error; });
        child.once("close", async exitCode => {
            const collected = await consumed;
            signal.removeEventListener("abort", abort);
            if (failure) reject(failure);
            else resolve({ exitCode, stdout: Buffer.concat(chunks.stdout), stderr: Buffer.concat(chunks.stderr),
                ...(consume ? { collected } : {}) });
        });
    });
}

async function inspectRuntime(config, deps = {}) {
    validateContainerConfig(config);
    requireValue((deps.platform ?? process.platform) === "linux", "container adapter requires a disposable Linux x64 host; no retargeting");
    const result = await (deps.command ?? command)(config.dockerExecutable,
        ["--host", "unix:///var/run/docker.sock", "info", "--format", "{{json .}}"],
        { env: { PATH: "/usr/bin:/bin", HOME: "/nonexistent", DOCKER_CONFIG: "/nonexistent" } });
    requireValue(result.exitCode === 0, "Docker runtime unavailable");
    const info = JSON.parse(result.stdout.toString("utf8"));
    requireValue(info.OSType === "linux" && info.Architecture === "x86_64" && /^\d+\./.test(info.ServerVersion) &&
        Number(info.ServerVersion.split(".")[0]) >= 28 && info.CgroupVersion === "2" &&
        info.SecurityOptions?.includes("name=seccomp,profile=builtin") && info.SecurityOptions.includes("name=apparmor") &&
        !info.SecurityOptions.includes("name=rootless") &&
        ["MemoryLimit", "SwapLimit", "PidsLimit", "CpuCfsQuota"].every(k => info[k] === true),
    "unsupported Docker runtime: require Engine >=28, cgroup v2, seccomp, AppArmor and resource controllers");
    return info;
}

async function regularFile(file, maxBytes) {
    requireValue(typeof file === "string" && path.isAbsolute(file), "absolute input file required");
    const stat = await fs.lstat(file);
    requireValue(stat.isFile() && !stat.isSymbolicLink() && stat.nlink === 1 && stat.size <= maxBytes &&
        await fs.realpath(file) === file, "input is not a bounded, unlinked regular file");
    return stat;
}

async function regularBytes(file, maxBytes) {
    const stat = await regularFile(file, maxBytes);
    const bytes = await fs.readFile(file);
    requireValue(bytes.length === stat.size, "input file changed during read");
    return bytes;
}

async function sourceBundleIdentity(file, signal) {
    await regularFile(file, sourceBundleBytes);
    const identity = await hashFile(fs, file, sourceBundleBytes, signal);
    requireValue(identity.size > 0, "source bundle is empty");
    return identity;
}

async function stageHelpers(root) {
    const files = [
        ".github/policies/regression-bisect.json", "global.json", "UseLocalCompiler.Directory.Build.props",
        "UseLocalCompiler.Directory.Build.targets", "eng/TargetFrameworks.props",
        "eng/common/tools.sh", "eng/common/pipeline-logging-functions.sh",
        ...(await fs.readdir(__dirname)).filter(n => n.endsWith(".cjs") && !/\.(test|smoke)\.cjs$/.test(n))
            .map(n => `eng/regression-bisect/${n}`),
        "eng/regression-bisect/ReproEvidence.targets",
        ...(await fs.readdir(path.join(__dirname, "evidence"), { recursive: true }))
            .filter(n => /\.(cs|csproj)$/.test(n) && !n.split(/[\\/]/).some(p => ["bin", "obj"].includes(p)))
            .map(n => `eng/regression-bisect/evidence/${n.split(path.sep).join("/")}`),
    ];
    const digests = [];
    for (const name of files.sort()) {
        validateRepositoryPath(name);
        const bytes = await regularBytes(path.join(trustedRoot, name), 1024 * 1024);
        const target = path.join(root, name);
        await fs.mkdir(path.dirname(target), { recursive: true });
        await fs.writeFile(target, bytes, { flag: "wx", mode: 0o444 });
        digests.push({ path: name, sha256: hash(bytes) });
    }
    return digests;
}

async function readableTree(root) {
    for (const relative of ["", ...await fs.readdir(root, { recursive: true })]) {
        const file = path.join(root, relative), stat = await fs.lstat(file);
        requireValue(!stat.isSymbolicLink() && (stat.isDirectory() || stat.isFile() && stat.nlink === 1),
            "staged input must contain only directories and regular unlinked files");
        await fs.chmod(file, stat.isDirectory() ? 0o755 : 0o444);
    }
}

function verifyTransportReceipt(receipt, unit, key) {
    requireValue(Buffer.isBuffer(key) && key.length >= 32, "outer receipt authentication key required");
    const { signature, ...signed } = receipt ?? {};
    requireValue(typeof signature === "string" && /^[a-f0-9]{64}$/.test(signature) &&
        timingSafeEqual(Buffer.from(signature, "hex"), createHmac("sha256", key).update(fingerprint(signed)).digest()),
    "transport receipt authentication failed");
    requireValue(receipt.status === "collected" && receipt.launch?.unitDigest === fingerprint(unit) &&
        receipt.launch.imageDigest === image.split(":").at(-1) && receipt.image === image &&
        receipt.historicalTerminated === true && receipt.cleanup.every(r => r.removed),
    "transport receipt launch or termination mismatch");
    validateExecutionFiles(receipt.files);
    return true;
}

function verifyContainerEvidence(output, unit, key) {
    requireValue(output?.status === "collected", "physical execution was not collected");
    verifyTransportReceipt(output.receipt, unit, key);
    requireValue(Array.isArray(output.files) &&
        fingerprint(output.files.map(({ path, size, sha256 }) => ({ path, size, sha256 }))) === fingerprint(output.receipt.files),
    "physical files differ from authenticated transfer");
    for (const file of output.files)
        requireValue(Buffer.isBuffer(file.bytes) && file.bytes.length === file.size && hash(file.bytes) === file.sha256,
            "physical evidence bytes differ from authenticated transfer");
    const result = readExecutionOutput(output.files, unit, output.receipt.launch);
    requireValue(fingerprint(result) === fingerprint(output.result), "physical result differs from authenticated transfer");
    requireValue(Array.isArray(output.blobs) && fingerprint(output.blobs.map(({ path, ...blob }) => blob)) === fingerprint(output.receipt.blobs),
        "physical archives differ from authenticated transfer");
    requireValue(output.blobs.every(blob => blob.path === path.join(output.receipt.evidenceDirectory, blob.slot + ".blob")),
        "physical archive path differs from authenticated storage");
    return true;
}

async function loadContainerEvidence(evidenceDirectory, unit, key) {
    const root = await fs.realpath(evidenceDirectory);
    requireValue(root === path.resolve(evidenceDirectory), "resolved owned evidence directory required");
    const receipt = JSON.parse(await regularBytes(path.join(root, "receipt.json"), 1024 * 1024));
    verifyTransportReceipt(receipt, unit, key);
    requireValue(receipt.evidenceDirectory === root, "retained evidence directory binding mismatch");
    const files = [];
    for (const file of receipt.files) {
        const bytes = await regularBytes(path.join(root, file.path), 16 * 1024 * 1024);
        requireValue(bytes.length === file.size && hash(bytes) === file.sha256, "retained evidence digest or size mismatch");
        files.push({ ...file, bytes });
    }
    const blobs = [];
    for (const blob of receipt.blobs) {
        requireValue(/^archive-(0|[1-9]\d*)$/.test(blob.slot), "invalid retained archive slot");
        const location = path.join(root, blob.slot + ".blob");
        // Re-read and hash through the bounded streaming lane without loading archives into memory.
        const file = await fs.open(location, "r");
        try {
            const stat = await file.stat();
            requireValue(stat.isFile() && stat.nlink === 1 && stat.size === blob.size &&
                await fs.realpath(location) === location, "invalid retained archive file");
            const { createHash } = require("node:crypto");
            const digest = createHash("sha256"), sha512 = createHash("sha512");
            let size = 0;
            requireValue(blob.size > 0 && blob.size <= 1073741824, "invalid retained archive size");
            for await (const chunk of file.createReadStream({ autoClose: false })) {
                size += chunk.length;
                requireValue(size <= blob.size, "retained archive exceeds byte limit");
                digest.update(chunk); sha512.update(chunk);
            }
            requireValue(size === blob.size && digest.digest("hex") === blob.sha256 &&
                sha512.digest("hex") === blob.sha512, "retained archive digest mismatch");
        } finally { await file.close(); }
        blobs.push({ ...blob, path: location });
    }
    return { status: "collected", result: readExecutionOutput(files, unit, receipt.launch), receipt, files, blobs };
}

async function readCollectedExecution(evidenceDirectory, { keys, keyId }) {
    const root = await fs.realpath(evidenceDirectory);
    requireValue(root === path.resolve(evidenceDirectory), "resolved owned evidence directory required");
    const unit = JSON.parse(new TextDecoder("utf-8", { fatal: true }).decode(
        await regularBytes(path.join(root, "unit.json"), 16 * 1024 * 1024)));
    validateExecutionUnit(unit);
    const output = await loadContainerEvidence(root, unit, keys?.[keyId]);
    return { ...output, unit, launch: output.receipt.launch };
}

async function runContainer(config, options, deps = {}) {
    const receipt = { schemaVersion: 1, image: config?.image, configDigest: fingerprint(config),
        launchId: randomBytes(32).toString("hex"), status: "blocked", workerStarted: false,
        resources: [], commands: [], cleanup: [], startedAt: new Date().toISOString() };
    const root = await fs.realpath(options.evidenceDirectory);
    requireValue(root === path.resolve(options.evidenceDirectory) && !(await fs.lstat(root)).isSymbolicLink(),
        "resolved owned evidence directory required");
    requireValue(!/[,\r\n]/.test(root), "evidence directory cannot contain Docker mount delimiters");
    receipt.evidenceDirectory = root;
    const receiptFile = path.join(root, "receipt.json");
    await fs.writeFile(receiptFile, JSON.stringify(receipt, null, 2), { flag: "wx" });
    let staging, answer, failure, cleaning = false, files = [], blobs = [];
    const invoke = deps.command ?? command;
    const docker = async (args, settings = {}, allowMissing = false) => {
        const result = await invoke(config.dockerExecutable,
            ["--host", "unix:///var/run/docker.sock", "--config", path.join(staging, "docker"), ...args],
            { env: { PATH: "/usr/bin:/bin", HOME: staging },
                ...(cleaning ? { timeoutMs: 5000 } : { signal: options.signal }), ...settings });
        receipt.commands.push({ args, exitCode: result.exitCode, stdoutSha256: hash(result.stdout),
            stderrSha256: hash(result.stderr ?? Buffer.alloc(0)) });
        const error = (result.stderr?.toString("utf8") ?? "").trim();
        const missing = /^(?:(?:Error response from daemon|Error): )?No such (object|container|network): (.+)$/i.exec(error);
        if (allowMissing && result.exitCode !== 0 && args[1] === "inspect" &&
            (missing && missing[2] === args[2] && ["object", args[0]].includes(missing[1].toLowerCase()) ||
             args[0] === "network" && args[1] === "inspect" &&
             error === `Error response from daemon: network ${args[2]} not found`)) return null;
        requireValue(result.exitCode === 0, `Docker ${args.slice(0, 2).join(" ")} failed: ${result.stderr?.toString("utf8").slice(0, 1024)}`);
        return result.collected ?? result.stdout;
    };
    const inspect = async (kind, name, allowMissing = false) => {
        const bytes = await docker([kind, "inspect", name], {}, allowMissing);
        if (bytes === null) return null;
        const values = JSON.parse(bytes.toString("utf8"));
        requireValue(Array.isArray(values) && values.length === 1, "ambiguous Docker inspect");
        return values[0];
    };
    const own = (kind, name) => receipt.resources.push({ kind, name });
    const label = "org.dotnet.fsharp.regression-launch";
    try {
        options.signal?.throwIfAborted();
        receipt.runtime = await inspectRuntime(config, deps);
        options.signal?.throwIfAborted();
        requireValue(["execution", "canary"].includes(options.operation), "invalid container operation");
        if (options.operation === "execution") {
            requireValue(Buffer.isBuffer(options.receiptKey) && options.receiptKey.length >= 32, "outer receipt authentication key required");
            validateExecutionUnit(options.input);
            requireValue(options.input.platform.os === "linux" && options.input.platform.architecture === "x64" &&
                options.input.platform.rid === "linux-x64", "container cannot retarget reviewed execution platform");
            requireValue(options.input.environmentFingerprint === fingerprint(Object.fromEntries(options.input.manifest.matrix.map(c =>
                [c.id, { os: c.os, architecture: c.architecture, imageDigest: image.split(":").at(-1) }]))),
            "execution environment is not bound to the pinned container image");
        }
        staging = await fs.mkdtemp(path.join(root, "staging-"));
        await fs.chmod(staging, 0o755);
        const bundle = path.join(staging, "bundle"), input = path.join(staging, "input");
        await fs.mkdir(bundle); await fs.mkdir(input); await fs.mkdir(path.join(staging, "docker"));
        receipt.helpers = await stageHelpers(bundle);
        await fs.mkdir(path.join(bundle, "artifacts"));
        await readableTree(bundle);
        receipt.helperDigest = fingerprint(receipt.helpers);
        const launch = { launchId: receipt.launchId, imageDigest: image.split(":").at(-1), unitDigest: fingerprint(options.input) };
        receipt.launch = launch;
        if (options.operation === "execution") {
            for (const source of options.input.manifest.sources) {
                const sourceRoot = await fs.realpath(options.sourceDirectory);
                const bytes = await regularBytes(path.join(sourceRoot, source.path), 1024 * 1024);
                requireValue(hash(bytes) === source.sha256, "reviewed sample bytes changed");
                const target = path.join(input, "samples", source.path);
                await fs.mkdir(path.dirname(target), { recursive: true });
                await fs.writeFile(target, bytes, { flag: "wx", mode: 0o444 });
            }
            if (options.input.request.kind === "source") {
                await regularFile(options.sourceBundle, sourceBundleBytes);
                const target = path.join(input, "source.bundle");
                await fs.copyFile(options.sourceBundle, target, fs.constants.COPYFILE_EXCL);
                const identity = await sourceBundleIdentity(target, options.signal);
                requireValue(identity.sha256 === options.sourceBundleSha256, "source bundle digest mismatch");
                receipt.sourceBundleSha256 = identity.sha256;
            }
        }
        const actualImage = await inspect("image", image);
        requireValue(actualImage.Os === "linux" && actualImage.Architecture === "amd64" &&
            actualImage.RepoDigests?.some(d => d.endsWith(`@sha256:${launch.imageDigest}`)), "local image digest/platform mismatch");
        receipt.imageId = actualImage.Id;
        const name = `rb-${receipt.launchId.slice(0, 24)}`, internal = `${name}-private`, external = `${name}-public`;
        for (const [network, internalOnly] of [[internal, true], [external, false]]) {
            own("network", network);
            await docker(["network", "create", "--driver", "bridge", "--label", `${label}=${receipt.launchId}`,
                ...(internalOnly ? ["--internal", "--opt", "com.docker.network.bridge.gateway_mode_ipv4=isolated",
                    "--opt", "com.docker.network.bridge.enable_ip_masquerade=false"] : []), network]);
            const inspected = await inspect("network", network);
            requireValue(inspected.Driver === "bridge" && inspected.Internal === internalOnly &&
                inspected.EnableIPv6 === false && inspected.Labels?.[label] === receipt.launchId &&
                (!internalOnly || inspected.Options?.["com.docker.network.bridge.gateway_mode_ipv4"] === "isolated"),
            "Docker did not enforce the isolated network configuration");
            receipt[internalOnly ? "privateNetwork" : "publicNetwork"] = inspected;
        }
        const common = (containerName, network, limits, supervisor = false, executableScratch = false) => [
            "container", "create", "--name", containerName, "--label", `${label}=${receipt.launchId}`,
            "--network", network, "--user", supervisor ? "0:0" : "1000:1000", "--cap-drop", "ALL",
            ...(supervisor ? ["--cap-add", "SETUID", "--cap-add", "SETGID", "--cap-add", "KILL", "--cap-add", "CHOWN"] : []),
            "--security-opt", "no-new-privileges=true", "--security-opt", "apparmor=docker-default",
            "--read-only", "--init", "--cgroupns", "private", "--ipc", "private", "--log-driver", "none", "--restart", "no",
            "--cpus", String(limits.cpus), "--memory", `${limits.memoryMiB}m`, "--memory-swap", `${limits.memoryMiB}m`,
            "--pids-limit", String(limits.pids), "--ulimit", "nofile=1024:1024", "--ulimit", "core=0:0",
            "--sysctl", "net.ipv4.ip_forward=0", "--sysctl", "net.ipv6.conf.all.disable_ipv6=1",
            "--tmpfs", `/scratch:rw,${executableScratch ? "exec" : "noexec"},nosuid,nodev,size=${limits.scratchMiB}m,uid=${supervisor ? 0 : 1000},gid=${supervisor ? 0 : 1000},mode=${supervisor ? "0755" : "0700"}`,
            "--tmpfs", "/tmp:rw,nosuid,nodev,noexec,size=16m,uid=1000,gid=1000,mode=0700",
            "--tmpfs", `/trusted/artifacts:rw,nosuid,nodev,noexec,size=16m,uid=${supervisor ? 0 : 1000},gid=${supervisor ? 0 : 1000},mode=0700`,
            "--mount", `type=bind,source=${bundle},target=/trusted,readonly`,
            "--workdir", "/scratch", "--entrypoint", "/usr/bin/env",
        ];
        const broker = `${name}-broker`, worker = `${name}-worker`;
        own("container", broker);
        await docker([...common(broker, external, { cpus: 1, memoryMiB: 256, pids: 32, scratchMiB: 16 }),
            image, "-i", "PATH=/usr/local/bin:/usr/bin:/bin", "HOME=/scratch", "node",
            "/trusted/eng/regression-bisect/container-entry.cjs", "broker"]);
        await docker(["network", "connect", internal, broker]);
        await docker(["container", "start", broker]);
        const brokerState = await inspect("container", broker);
        requireValue(brokerState.State?.Running, "egress broker failed to start");
        const brokerIp = brokerState.NetworkSettings?.Networks?.[internal]?.IPAddress;
        requireValue(typeof brokerIp === "string" && /^(?:\d{1,3}\.){3}\d{1,3}$/.test(brokerIp), "missing private broker address");
        const proxy = `http://${brokerIp}:3128`;
        await fs.writeFile(path.join(input, "input.json"), JSON.stringify({
            schemaVersion: 1, operation: options.operation, input: options.input, launch, proxy,
            hostAddresses: [receipt.privateNetwork, receipt.publicNetwork]
                .flatMap(n => n.IPAM?.Config?.map(c => c.Gateway).filter(Boolean) ?? []),
            limits: config.limits,
        }), { flag: "wx", mode: 0o444 });
        await readableTree(input);
        own("container", worker);
        const supervisor = options.operation === "execution";
        await docker([...common(worker, internal, config.limits, supervisor, true), "--dns", "127.0.0.1",
            "--mount", `type=bind,source=${input},target=/input,readonly`,
            image, "-i", "PATH=/usr/local/bin:/usr/bin:/bin", "HOME=/scratch", "TMPDIR=/scratch",
            `HTTPS_PROXY=${proxy}`, `HTTP_PROXY=${proxy}`, `https_proxy=${proxy}`, `http_proxy=${proxy}`,
            "NO_PROXY=", "NODE_USE_ENV_PROXY=1", "node", "--use-env-proxy",
            "/trusted/eng/regression-bisect/container-entry.cjs", "worker"]);
        const created = await inspect("container", worker);
        requireValue(created.Image === actualImage.Id && created.Config?.User === (supervisor ? "0:0" : "1000:1000") &&
            created.Config.Labels?.[label] === receipt.launchId && created.AppArmorProfile === "docker-default" &&
            created.HostConfig?.ReadonlyRootfs && created.HostConfig.Privileged === false &&
            created.HostConfig.PidMode === "" && created.HostConfig.NetworkMode === internal &&
            created.HostConfig.IpcMode === "private" && created.HostConfig.CgroupnsMode === "private" &&
            created.HostConfig.CapDrop?.join() === "ALL" && (supervisor
                ? created.HostConfig.CapAdd?.map(c => c.replace(/^CAP_/, "")).sort().join() === "CHOWN,KILL,SETGID,SETUID"
                : !created.HostConfig.CapAdd?.length) &&
            created.HostConfig.SecurityOpt?.includes("no-new-privileges=true") &&
            created.HostConfig.Memory === config.limits.memoryMiB * 1024 * 1024 &&
            created.HostConfig.MemorySwap === created.HostConfig.Memory &&
            created.HostConfig.PidsLimit === config.limits.pids && created.HostConfig.NanoCpus === config.limits.cpus * 1e9 &&
            created.HostConfig.Dns?.join() === "127.0.0.1" && !Object.keys(created.HostConfig.PortBindings ?? {}).length &&
            created.Mounts?.filter(m => m.Type === "bind").length === 2 && created.Mounts.every(m =>
                m.Type === "tmpfs" && ["/scratch", "/tmp", "/trusted/artifacts"].includes(m.Destination) ||
                m.Type === "bind" && m.RW === false &&
                    (m.Destination === "/trusted" && m.Source === bundle || m.Destination === "/input" && m.Source === input)) &&
            Object.keys(created.NetworkSettings.Networks).join() === internal,
        "worker launch isolation mismatch");
        receipt.containerId = created.Id;
        receipt.workerConfig = created.HostConfig;
        await docker(["container", "exec", broker, "/usr/local/bin/node",
            "/trusted/eng/regression-bisect/container-entry.cjs", "ready"]);
        const remaining = options.operation === "execution" ? Date.parse(options.input.deadline) - Date.now() : config.limits.timeoutMs;
        requireValue(remaining > 0, "worker reservation deadline exhausted");
        receipt.workerStarted = true;
        if (supervisor) {
            await docker(["container", "start", worker]);
            const exportCommand = operation => ["container", "exec", "--user", "0:0", worker, "/usr/local/bin/node",
                "/trusted/eng/regression-bisect/container-entry.cjs", operation];
            const descriptor = JSON.parse(await docker(exportCommand("collect"), {
                timeoutMs: Math.min(remaining, config.limits.timeoutMs), maxBytes: 1024 * 1024,
            }));
            requireValue(descriptor && Object.keys(descriptor).sort().join(" ") ===
                "blobs files historicalTerminated imageDigest launchId schemaVersion unitDigest" &&
                descriptor.schemaVersion === 1 && descriptor.historicalTerminated === true &&
                Object.keys(launch).every(k => descriptor[k] === launch[k]), "protected export launch binding mismatch");
            const live = await inspect("container", worker);
            requireValue(live.State?.Running === true && live.State.OOMKilled === false, "trusted export supervisor is not alive");
            const total = validateExecutionFiles(descriptor.files);
            requireValue(total <= config.limits.scratchMiB * 1024 * 1024, "evidence transfer exceeds launch quota");
            const transferSignal = AbortSignal.any([AbortSignal.timeout(300000), ...(options.signal ? [options.signal] : [])]);
            files = await docker(exportCommand("archive"), { signal: transferSignal, timeoutMs: 300000,
                consume: stream => collectWorkerArchive(stream, descriptor.files, { signal: transferSignal }) });
            answer = readExecutionOutput(files, options.input, launch);
            requireValue(fingerprint(JSON.parse(files.find(f => f.path === "unit.json").bytes)) === fingerprint(options.input),
                "exported execution unit binding mismatch");
            requireValue(Array.isArray(descriptor.blobs) && descriptor.blobs.length <= 16, "invalid archive identity list");
            requireValue(descriptor.blobs.every(b => Number.isSafeInteger(b?.archive?.retained?.size) &&
                b.archive.retained.size > 0 && b.archive.retained.size <= 1073741824) &&
                descriptor.blobs.reduce((sum, b) => sum + b.archive.retained.size, 0) <= 3 * 1024 * 1024 * 1024,
            "archive identities exceed the 3 GiB launch quota");
            let transferred = total;
            const archiveIdentities = new Set();
            for (const [index, blob] of descriptor.blobs.entries()) {
                requireValue(blob && Object.keys(blob).sort().join(" ") ===
                    "archive kind platform product scope slot sourceSha version" && blob.slot === `archive-${index}` &&
                    ["reproduction", "build"].includes(blob.scope) &&
                    fingerprint(blob.platform) === fingerprint(options.input.platform), "invalid fixed archive slot");
                const archiveIdentity = fingerprint({ scope: blob.scope, product: blob.product, version: blob.version, archive: blob.archive });
                requireValue(!archiveIdentities.has(archiveIdentity), "duplicate archive identity");
                archiveIdentities.add(archiveIdentity);
                const acquired = JSON.parse(files.find(f => f.path ===
                    (blob.scope === "build" ? "build-acquisition.json" : "acquisition.json")).bytes);
                requireValue(acquired?.status === "acquired" && blob.kind === acquired.identity.requested.kind &&
                    blob.sourceSha === (acquired.identity.requested.sourceSha ?? null) &&
                    (blob.scope !== "build" || options.input.request.kind === "source" &&
                        blob.sourceSha === options.input.request.target.sourceSha) &&
                    acquired.identity.installations.some(i => i.product === blob.product && i.version === blob.version &&
                        fingerprint(i.archive) === fingerprint(blob.archive)), "archive acquisition binding mismatch");
                if (blob.scope === "reproduction") requireValue(
                    acquired.identity.requested.sdkVersion === options.input.stableSdk.version &&
                    acquired.identity.installations.some(i => i.product === "sdk" && i.version === options.input.stableSdk.version &&
                        i.archive.sha256 === options.input.stableSdk.archives.find(a =>
                            a.os === options.input.platform.os && a.architecture === options.input.platform.architecture)?.sha256),
                "reproduction archive differs from the approved stable SDK");
                validateArchiveReceipt({ policy, ...blob }, blob.archive, path.posix);
                requireValue(blob.archive.retained.size <= 1073741824, "archive exceeds approved byte limit");
                transferred += blob.archive.retained.size;
                requireValue(transferred <= config.limits.scratchMiB * 1024 * 1024, "archive transfer exceeds launch quota");
                const retained = await docker([...exportCommand("blob"), blob.slot], { signal: transferSignal, timeoutMs: 300000,
                    consume: stream => collectWorkerBlob(stream, blob.archive, path.join(root, blob.slot + ".blob"),
                        { signal: transferSignal }) });
                blobs.push({ ...blob, ...retained });
            }
            const expectedBlobs = ["acquisition.json", "build-acquisition.json"].flatMap(name => {
                const value = JSON.parse(files.find(f => f.path === name).bytes);
                return value?.status === "acquired" ? value.identity.installations : [];
            });
            requireValue(expectedBlobs.length === blobs.length, "required archive transfer missing");
            for (const file of files) {
                const pending = path.join(root, file.path + ".partial");
                const handle = await fs.open(pending, "wx", 0o600);
                try { await handle.writeFile(file.bytes); await handle.sync(); } finally { await handle.close(); }
                await fs.link(pending, path.join(root, file.path)); await fs.unlink(pending);
            }
            receipt.files = files.map(({ path, size, sha256 }) => ({ path, size, sha256 }));
            receipt.blobs = blobs.map(({ path, ...blob }) => blob);
            receipt.historicalTerminated = true;
            receipt.transferredBytes = transferred;
        } else {
            const output = await docker(["container", "start", "--attach", worker], {
                signal: options.signal, timeoutMs: Math.min(remaining, config.limits.timeoutMs), maxBytes: 16 * 1024 * 1024,
            });
            const stopped = await inspect("container", worker);
            receipt.exit = stopped.State;
            requireValue(stopped.State?.Running === false && stopped.State.ExitCode === 0 && stopped.State.OOMKilled === false,
                "worker did not terminate successfully");
            const payload = JSON.parse(new TextDecoder("utf-8", { fatal: true }).decode(output));
            requireValue(payload && Object.keys(payload).sort().join(" ") === "imageDigest launchId result schemaVersion unitDigest" &&
                payload.schemaVersion === 1 && Object.keys(launch).every(k => payload[k] === launch[k]), "canary launch binding mismatch");
            answer = payload.result;
            receipt.resultSha256 = hash(output);
            await fs.writeFile(path.join(root, "result.json"), output, { flag: "wx" });
        }
        receipt.status = "collected";
    } catch (error) {
        failure = error;
        receipt.status = "blocked";
        receipt.reason = error.message;
    } finally {
        cleaning = true;
        for (const resource of receipt.resources.toReversed()) {
            try {
                const value = await inspect(resource.kind, resource.name, true);
                if (value !== null) {
                    requireValue((resource.kind === "container" ? value.Config?.Labels : value.Labels)?.[label] === receipt.launchId,
                        "refusing cleanup of a resource not owned by this launch");
                    await docker([resource.kind, "rm", ...(resource.kind === "container" ? ["--force"] : []), resource.name]);
                    requireValue(await inspect(resource.kind, resource.name, true) === null, "owned resource survived removal");
                }
                receipt.cleanup.push({ ...resource, removed: true,
                    ...(resource.kind === "container" && value ? { state: value.State } : {}) });
            } catch (error) {
                failure ??= error;
                receipt.status = "blocked";
                receipt.reason = `${receipt.reason ?? "cleanup failed"}; ${error.message}`;
                receipt.cleanup.push({ ...resource, removed: false, reason: error.message });
            }
        }
        if (staging) await fs.rm(staging, { recursive: true });
        receipt.completedAt = new Date().toISOString();
        if (options.operation === "execution" && Buffer.isBuffer(options.receiptKey))
            receipt.signature = createHmac("sha256", options.receiptKey).update(fingerprint(receipt)).digest("hex");
        const replacement = path.join(root, "receipt.new");
        const handle = await fs.open(replacement, "wx", 0o600);
        try { await handle.writeFile(JSON.stringify(receipt, null, 2)); await handle.sync(); } finally { await handle.close(); }
        const signal = AbortSignal.timeout(5000);
        try {
            await boundedWait((async () => {
                while (true) {
                    signal.throwIfAborted();
                    try { await fs.rename(replacement, receiptFile); break; }
                    catch (error) {
                        // Windows readers without delete sharing temporarily block atomic replacement.
                        if (process.platform !== "win32" || error.code !== "EPERM") throw error;
                        await delay(10, undefined, { signal });
                    }
                }
                signal.throwIfAborted();
            })(), signal);
        } catch (error) {
            throw new Error(`receipt publication failed: ${signal.aborted ? signal.reason.message : error.message}` +
                (failure ? `; ${receipt.reason}` : ""), { cause: error });
        }
    }
    return failure ? { status: "blocked", reason: receipt.reason, receipt } :
        { status: "collected", result: answer, receipt, ...(options.operation === "execution" ? { files, blobs } : {}) };
}

module.exports = { image, validateContainerConfig, inspectRuntime, runContainer, sourceBundleIdentity,
    verifyTransportReceipt, verifyContainerEvidence, loadContainerEvidence, readCollectedExecution };
