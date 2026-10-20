const fs = require("node:fs/promises");
const { writeFileSync, renameSync } = require("node:fs");
const path = require("node:path");
const net = require("node:net");
const { spawn } = require("node:child_process");
const { createEgressBroker } = require("./egress.cjs");
const { runExecutionUnit, validateExecutionUnit, executionEvidence } = require("./worker.cjs");
const { workerArchive } = require("./worker-output.cjs");
const { once } = require("node:events");
const { acquireToolchain } = require("./toolchains.cjs");
const { prepareEnvironment } = require("./repro.cjs");
const { execute } = require("./repro-process.cjs");
const { hash, fileIdentity, copyOwnedFile, requireValue } = require("./toolchain-io.cjs");
const policy = require("../../.github/policies/regression-bisect.json");

function connect(host, port, request) {
    return new Promise(resolve => {
        const socket = net.connect({ host, port });
        let result = "";
        const timer = setTimeout(() => socket.destroy(), 2000);
        socket.on("connect", () => {
            result = "connected";
            if (request) socket.write(request);
            else socket.destroy();
        });
        socket.on("data", bytes => { result = bytes.toString("ascii").split("\r\n")[0]; socket.destroy(); });
        socket.on("error", () => { result = "denied"; });
        socket.on("close", () => { clearTimeout(timer); resolve(result || "timeout"); });
    });
}

async function canary(job) {
    const { input, limits, proxy } = job;
    requireValue(["inspect", "timeout", "disk", "pids", "memory", "cpu", "malformed"].includes(input.mode), "unknown canary mode");
    if (input.mode === "memory") {
        const blocks = [];
        for (;;) blocks.push(Buffer.alloc(8 * 1024 * 1024, 1));
    }
    if (input.mode === "cpu") {
        const started = Date.now();
        const measurements = await Promise.all(Array.from({ length: 2 }, () => new Promise((resolve, reject) => {
            const child = spawn(process.execPath, ["-e",
                "const start=process.cpuUsage(),end=Date.now()+2000; while(Date.now()<end){}; const cpu=process.cpuUsage(start); console.log(cpu.user+cpu.system);"],
            { stdio: ["ignore", "pipe", "ignore"] });
            let text = "";
            child.stdout.on("data", bytes => { text += bytes.toString("ascii"); });
            child.once("error", reject);
            child.once("close", code => code === 0 && /^\d+\n$/.test(text) ? resolve(Number(text) / 1e6) :
                reject(new Error("CPU canary child failed")));
        })));
        const wallSeconds = (Date.now() - started) / 1000, cpuSeconds = measurements.reduce((a, b) => a + b, 0);
        requireValue(limits.cpus === 1 && cpuSeconds <= wallSeconds * 1.35, "CPU quota was not enforced");
        return { mode: input.mode, wallSeconds, cpuSeconds };
    }
    if (input.mode === "timeout") {
        spawn(process.execPath, ["-e", "setInterval(() => {}, 1000)"], { detached: true, stdio: "ignore" });
        await new Promise(() => {});
    }
    if (input.mode === "malformed") {
        process.stdout.write('{"result":{"path":"../../outside","verified":true}}');
        return null;
    }
    if (input.mode === "disk") {
        const file = await fs.open("/scratch/fill", "wx");
        let bytes = 0, error;
        try {
            for (let n = 0; n <= limits.scratchMiB; n++) { await file.writeFile(Buffer.alloc(1024 * 1024)); bytes += 1024 * 1024; }
        } catch (e) { error = e.code; }
        finally { await file.close(); }
        requireValue(error === "ENOSPC" && bytes <= limits.scratchMiB * 1024 * 1024, "scratch disk limit not enforced");
        return { mode: input.mode, bytes, denied: error };
    }
    if (input.mode === "pids") {
        const children = [];
        let denied = false;
        try {
            for (let n = 0; n < limits.pids + 1; n++) {
                const child = spawn("/bin/sleep", ["60"], { stdio: "ignore" });
                const started = await new Promise((resolve, reject) => {
                    child.once("spawn", () => resolve(true));
                    child.once("error", error => error.code === "EAGAIN" ? resolve(false) : reject(error));
                });
                if (!started) { denied = true; break; }
                children.push(child);
            }
        } finally {
            await Promise.all(children.map(child => new Promise(resolve => {
                child.once("close", resolve);
                child.kill("SIGKILL");
            })));
        }
        requireValue(denied, "PID limit not enforced");
        return { mode: input.mode, spawned: children.length, denied };
    }
    const denials = [];
    for (const file of [...input.outerPaths, "/var/run/docker.sock", "/run/containerd/containerd.sock",
        "/root/.docker/config.json", "/root/.gitconfig", "/root/.config/gh/hosts.yml"]) {
        try {
            const bytes = await fs.readFile(file);
            requireValue(hash(bytes) !== input.canaryDigest, `outer canary readable: ${file}`);
            throw new Error(`unexpected readable outer path: ${file}`);
        } catch (error) {
            requireValue(["ENOENT", "EACCES", "ENOTDIR"].includes(error.code), error.message);
            denials.push({ file, code: error.code });
        }
    }
    const environment = await fs.readFile("/proc/1/environ");
    requireValue(!environment.toString("utf8").split("\0").some(v => hash(v.slice(v.indexOf("=") + 1)) === input.canaryDigest) &&
        !Object.values(process.env).some(v => hash(v) === input.canaryDigest) &&
        Object.keys(process.env).every(k => ["PATH", "HOME", "TMPDIR", "HTTPS_PROXY", "HTTP_PROXY",
            "https_proxy", "http_proxy", "NO_PROXY", "NODE_USE_ENV_PROXY"].includes(k)), "outer environment leaked");
    const status = await fs.readFile("/proc/self/status", "utf8");
    requireValue(process.getuid() === 1000 && /^CapEff:\s+0+$/m.test(status) &&
        /^NoNewPrivs:\s+1$/m.test(status) && /^Seccomp:\s+2$/m.test(status), "worker privilege controls absent");
    const openFiles = /^Max open files\s+(\d+)\s+(\d+)/m.exec(await fs.readFile("/proc/self/limits", "utf8"));
    requireValue(openFiles?.[1] === "6500" && openFiles[2] === "6500", "source bootstrap file limit differs from launch");
    const cgroups = {};
    for (const name of ["memory.max", "memory.swap.max", "pids.max", "cpu.max"])
        cgroups[name] = (await fs.readFile(`/sys/fs/cgroup/${name}`, "utf8")).trim();
    requireValue(cgroups["memory.max"] === String(limits.memoryMiB * 1024 * 1024) &&
        cgroups["memory.swap.max"] === "0" && cgroups["pids.max"] === String(limits.pids) &&
        cgroups["cpu.max"].split(" ").map(Number)[0] / cgroups["cpu.max"].split(" ").map(Number)[1] === limits.cpus,
    "cgroup limits differ from launch");
    const direct = [];
    for (const host of ["169.254.169.254", "1.1.1.1", ...new Set([...input.hostAddresses, ...job.hostAddresses])]) {
        const result = await connect(host, 443);
        requireValue(result !== "connected", `direct network route escaped: ${host}`);
        direct.push({ host, result });
    }
    const broker = new URL(proxy);
    const proxied = [];
    for (const host of ["api.github.com", "169.254.169.254", "host.docker.internal", "results-receiver.actions.githubusercontent.com"]) {
        const result = await connect(broker.hostname, 3128, `CONNECT ${host}:443 HTTP/1.1\r\nHost: ${host}:443\r\n\r\n`);
        requireValue(result === "HTTP/1.1 403 Forbidden", `control endpoint not denied: ${host}`);
        proxied.push({ host, result });
    }
    const response = await fetch("https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json",
        { redirect: "error", signal: AbortSignal.timeout(30000) });
    requireValue(response.status === 200, "public egress canary failed");
    const chunks = [];
    let total = 0;
    for await (const bytes of response.body) {
        total += bytes.length;
        requireValue(total <= 1024 * 1024, "canary download exceeded limit");
        chunks.push(Buffer.from(bytes));
    }
    const bytes = Buffer.concat(chunks);
    requireValue(Array.isArray(JSON.parse(bytes)["releases-index"]), "public egress returned unexpected bytes");
    const nativeProbe = "/scratch/native-execution-probe";
    const nativeBytes = await fs.readFile("/usr/bin/true");
    await fs.writeFile(nativeProbe, nativeBytes, { flag: "wx", mode: 0o700 });
    let nativeExecution;
    try {
        nativeExecution = await execute(nativeProbe, [], {
            cwd: "/scratch", env: { PATH: "/usr/bin:/bin" }, timeoutMs: 30000, maxBytes: 1024,
        });
    } finally { await fs.unlink(nativeProbe); }
    requireValue(nativeExecution.complete && nativeExecution.exitCode === 0,
        `worker scratch cannot execute acquired native files: ${nativeExecution.error ?? nativeExecution.stderr}`);
    let readonly = false;
    try { await fs.writeFile("/trusted/probe", "must fail"); }
    catch (error) { requireValue(["EROFS", "EACCES"].includes(error.code), error.message); readonly = true; }
    requireValue(readonly, "trusted bundle is writable");
    return { mode: input.mode, denials, direct, proxied, cgroups, uid: process.getuid(),
        openFiles: { soft: Number(openFiles[1]), hard: Number(openFiles[2]) },
        statusSha256: hash(status), legitimateTransfer: { bytes: bytes.length, sha256: hash(bytes) },
        scratchExecution: { complete: nativeExecution.complete, exitCode: nativeExecution.exitCode, sha256: hash(nativeBytes) } };
}

async function terminateHistoricalProcesses() {
    const deadline = Date.now() + 5000;
    for (;;) {
        const live = [];
        for (const name of await fs.readdir("/proc")) {
            if (!/^\d+$/.test(name)) continue;
            try {
                const status = await fs.readFile(`/proc/${name}/status`, "utf8");
                if (/^Uid:\s+1000\s/m.test(status) && !/^State:\s+Z/m.test(status)) live.push(Number(name));
            } catch (error) { if (error.code !== "ENOENT" && error.code !== "ESRCH") throw error; }
        }
        if (!live.length) return;
        requireValue(Date.now() < deadline, "historical descendants did not terminate");
        for (const pid of live) {
            try { process.kill(pid, "SIGKILL"); } catch (error) { if (error.code !== "ESRCH") throw error; }
        }
        await new Promise(resolve => setTimeout(resolve, 10));
    }
}

async function historicalDirectory(directory, uid = 1000) {
    requireValue(path.resolve(directory) === directory && await fs.realpath(directory) === directory &&
        directory.startsWith("/scratch/") && !directory.startsWith("/scratch/helper") &&
        !directory.startsWith("/scratch/control") && !directory.startsWith("/scratch/storage") &&
        [0, 1000].includes(uid),
    "historical writable directory is not owned scratch");
    const visit = async file => {
        const stat = await fs.lstat(file);
        if (stat.isSymbolicLink()) return;
        requireValue((stat.isDirectory() || stat.isFile() && stat.nlink === 1) && [0, 1000].includes(stat.uid),
            "historical scratch contains nonregular or foreign-owned files");
        if (stat.uid !== 0) await fs.chown(file, 0, 0);
        const access = stat.isDirectory() ? 0o700 : 0o400;
        if ((stat.mode & access) !== access) await fs.chmod(file, (stat.mode & 0o777) | access);
        if (stat.isDirectory()) for (const name of await fs.readdir(file)) await visit(path.join(file, name));
        if (uid !== 0) await fs.chown(file, uid, uid);
    };
    await visit(directory);
}

async function historicalAccess(file) {
    for (let parent = path.dirname(file); parent.startsWith("/scratch/"); parent = path.dirname(parent)) {
        requireValue(!parent.startsWith("/scratch/control"), "control data must not reach historical processes");
        const stat = await fs.lstat(parent);
        requireValue(stat.isDirectory() && !stat.isSymbolicLink(), "historical access parent must be a regular directory");
        if (stat.uid === 0) await fs.chmod(parent, 0o755);
    }
}

async function physicalRunner(unit, launch) {
    validateExecutionUnit(unit);
    requireValue(process.getuid() === 0, "execution requires the protected supervisor UID");
    const signal = AbortSignal.timeout(Math.max(1, Date.parse(unit.deadline) - Date.now()));
    const commands = [], nugetHomes = new Set();
    const withProxy = async (executable, args, settings, historical = false) => {
        const env = { ...settings.env,
            ...Object.fromEntries(["HTTP_PROXY", "HTTPS_PROXY", "http_proxy", "https_proxy"].map(k => [k, process.env[k]])) };
        // NuGet creates HTTP and plugin caches beside its packages directory.
        const cacheRoots = ["NUGET_PACKAGES", "NUGET_HTTP_CACHE_PATH", "NUGET_PLUGINS_CACHE_PATH"]
            .flatMap(key => env[key] ? [path.dirname(env[key])] : []);
        const writable = [...new Set([settings.cwd, env.HOME, env.TEMP, ...cacheRoots].filter(Boolean))];
        const directories = writable.filter(directory => !writable.some(parent =>
            parent !== directory && directory.startsWith(`${parent}/`)));
        if (historical) {
            await terminateHistoricalProcesses();
            if (env.HOME && !nugetHomes.has(env.HOME)) {
                const directory = path.join(env.HOME, ".nuget", "NuGet");
                await fs.mkdir(directory, { recursive: true });
                await fs.writeFile(path.join(directory, "NuGet.Config"),
                    '<configuration><config><add key="maxHttpRequestsPerSource" value="4" /></config></configuration>',
                    { flag: "wx", mode: 0o644 });
                nugetHomes.add(env.HOME);
            }
            await historicalAccess(executable);
            await historicalAccess(settings.cwd + "/entry");
            for (const directory of directories) await historicalDirectory(directory);
        }
        const entry = { executable, args, cwd: settings.cwd, lane: historical ? "historical" : "trusted" };
        commands.push(entry);
        try {
            entry.result = await execute(historical ? "/usr/bin/setpriv" : executable,
                historical ? ["--reuid=1000", "--regid=1000", "--clear-groups", "--no-new-privs", "--", executable, ...args] : args,
                { ...settings, signal, env });
            return entry.result;
        } finally {
            if (historical) {
                await terminateHistoricalProcesses();
                for (const directory of directories) await historicalDirectory(directory, 0);
            }
        }
    };
    const storage = "/scratch/storage";
    for (const name of [storage, "/scratch/work", "/scratch/evidence", "/scratch/executions"]) await fs.mkdir(name);
    const acquired = await acquireToolchain({ policy, toolchain: unit.resolved, storageDirectory: storage, signal }, { run: withProxy });
    if (acquired.status !== "acquired") return executionEvidence(unit, launch, acquired, {
        acquisition: acquired, commands, evidenceDirectory: "/scratch/evidence", scratchDirectory: "/scratch",
    });
    let result;
    try {
        const toolchain = acquired.identity;
        const helperDirectory = "/scratch/helper", helperSources = "/trusted/eng/regression-bisect/evidence";
        await fs.mkdir(helperDirectory);
        for (const relative of await fs.readdir(helperSources, { recursive: true })) {
            const source = path.join(helperSources, relative), target = path.join(helperDirectory, relative);
            const stat = await fs.lstat(source);
            requireValue(!stat.isSymbolicLink() && (stat.isDirectory() || stat.isFile() && stat.nlink === 1),
                "evidence helper source must be a regular file or directory");
            await fs.mkdir(stat.isDirectory() ? target : path.dirname(target), { recursive: true });
            if (stat.isFile()) await copyOwnedFile(source, target);
        }
        const env = await prepareEnvironment("/scratch", toolchain, unit.platform);
        for (const key of ["HTTP_PROXY", "HTTPS_PROXY", "http_proxy", "https_proxy"]) env[key] = process.env[key];
        const runtime = toolchain.compilerRuntime.version;
        const helperOutput = path.join(helperDirectory, "output");
        const built = await withProxy(toolchain.hostPath, ["build", path.join(helperDirectory, "Evidence.csproj"),
            "-c", "Release", "-o", helperOutput, `-p:EvidenceTargetFramework=net${runtime.split(".")[0]}.0`, "-nr:false", "--nologo"],
        { cwd: helperDirectory, env, timeoutMs: 300000, maxBytes: 1024 * 1024 });
        requireValue(built.complete && built.exitCode === 0,
            `isolated evidence helper build failed (${built.status}, exit ${built.exitCode}): ${built.error ?? (built.stderr || built.stdout)}`);
        const helper = { executable: toolchain.hostPath, dll: path.join(helperOutput, "Evidence.dll"),
            hook: path.join(helperOutput, "Repro.StartupHook.dll"), runtimeVersion: runtime };
        const inventoryReleased = async (identity, buildInstanceId) => {
            const root = path.join(identity.sdkBasePath, "FSharp"), files = [];
            for (const [role, name] of Object.entries({
                fsi: "fsi.dll", compiler: "fsc.dll", core: "FSharp.Core.dll", tasks: "FSharp.Build.dll",
                props: "Microsoft.FSharp.NetSdk.props", targets: "Microsoft.FSharp.Targets",
                sdkTargets: "Microsoft.FSharp.NetSdk.targets", overrides: "Microsoft.FSharp.Overrides.NetSdk.targets",
            })) {
                const file = { role, ...await fileIdentity(path.join(root, name), root) };
                if (["fsi", "compiler"].includes(role))
                    file.runtimeConfig = await fileIdentity(file.path.replace(/\.dll$/, ".runtimeconfig.json"), root);
                files.push(file);
            }
            const pkg = identity.files.core.package;
            requireValue(pkg?.id === "FSharp.Core", "released Core package missing");
            files.push({ role: "corePackage", ...pkg });
            const inspected = await withProxy(helper.executable, ["exec", "--fx-version", runtime, helper.dll, "inspect",
                ...files.filter(f => f.path.endsWith(".dll") || f.role === "corePackage").map(f => f.path)],
            { cwd: helperDirectory, env, timeoutMs: 120000, maxBytes: 1024 * 1024 });
            requireValue(inspected.complete && inspected.exitCode === 0, "released product inspection failed");
            for (const value of JSON.parse(inspected.stdout)) {
                const file = files.find(f => f.path === value.path);
                requireValue(file && file.sha256 === value.sha256, "released inventory digest mismatch");
                Object.assign(file, value);
            }
            return { kind: "released", root, buildInstanceId, files, runtime: identity.compilerRuntime, corePackageVersion: pkg.version };
        };
        const historical = (executable, args, settings) => withProxy(executable, args, settings, !args.includes(helper.dll));
        result = await runExecutionUnit(unit, {
            policy, inventoryReleased, acquireToolchain: async () => acquired,
            workerExecute: historical, workspaceDependencies: { execute: historical, toolchains: { run: withProxy } },
            worker: { storageDirectory: storage, scratchDirectory: "/scratch/work", evidenceDirectory: "/scratch/evidence",
                workDirectory: "/scratch/executions", sourceDirectory: "/input/samples", mirrorDirectory: "/input/source.bundle",
                gitExecutable: "/usr/bin/git", reproductionToolchain: toolchain, helper },
        }, { signal });
    } catch (error) {
        result = { status: "untestable", outcome: { schemaVersion: 1, outcome: "untestable",
            reason: signal.aborted ? "timeout" : "setup-failure" },
            error: { stage: "worker", code: error.code ?? error.name, message: error.message } };
    }
    await terminateHistoricalProcesses();
    return executionEvidence(unit, launch, result, {
        acquisition: acquired, commands, evidenceDirectory: "/scratch/evidence", scratchDirectory: "/scratch",
    });
}

async function main() {
    const [operation] = process.argv.slice(2);
    requireValue(process.argv.length === (operation === "blob" ? 4 : 3), "fixed container entry required");
    if (operation === "broker") {
        let broker;
        const persist = () => {
            writeFileSync("/scratch/egress.new", JSON.stringify(broker.snapshot()), { mode: 0o600 });
            renameSync("/scratch/egress.new", "/scratch/egress.json");
        };
        broker = createEgressBroker({ audit: event => {
            process.stderr.write(`${JSON.stringify(event)}\n`);
            persist();
        } });
        persist();
        broker.server.listen(3128, "0.0.0.0");
        for (const signal of ["SIGTERM", "SIGINT"]) process.once(signal, () => {
            broker.close().catch(error => { console.error(error.message); process.exitCode = 1; });
        });
        return;
    }
    if (operation === "egress") {
        const file = "/scratch/egress.json", stat = await fs.lstat(file);
        requireValue(stat.isFile() && !stat.isSymbolicLink() && stat.nlink === 1 && stat.size <= 4096,
            "broker counters must be a bounded regular file");
        process.stdout.write(await fs.readFile(file));
        return;
    }
    if (operation === "ready") {
        requireValue(await connect("127.0.0.1", 3128) === "connected", "broker is not responsive");
        return;
    }
    if (["collect", "archive", "blob"].includes(operation)) {
        requireValue(process.getuid() === 0, "protected export UID required");
        const deadline = Date.now() + 110 * 60000;
        let descriptor;
        for (;;) {
            try { descriptor = JSON.parse(await fs.readFile("/scratch/control/ready.json", "utf8")); break; }
            catch (error) { if (error.code !== "ENOENT") throw error; }
            requireValue(Date.now() < deadline, "protected execution result unavailable");
            await new Promise(resolve => setTimeout(resolve, 100));
        }
        requireValue(!descriptor.error, descriptor.error);
        if (operation === "collect") { process.stdout.write(JSON.stringify(descriptor)); return; }
        const write = async bytes => { if (!process.stdout.write(bytes)) await once(process.stdout, "drain"); };
        if (operation === "archive") {
            const files = await Promise.all(descriptor.files.map(async file =>
                ({ ...file, bytes: await fs.readFile(path.join("/scratch/control", file.path)) })));
            for await (const bytes of workerArchive(files)) await write(bytes);
        } else {
            const slot = process.argv[3];
            requireValue(/^archive-(0|[1-9]\d*)$/.test(slot), "fixed archive slot required");
            const blob = descriptor.blobs.find(b => b.slot === slot);
            requireValue(blob, "unknown archive slot");
            const retained = blob.archive.retained;
            const stat = await fs.lstat(retained.path);
            requireValue(retained.path.startsWith("/scratch/") && await fs.realpath(retained.path) === retained.path &&
                stat.isFile() && !stat.isSymbolicLink() && stat.nlink === 1 && stat.uid === 0 &&
                stat.size === retained.size && stat.size <= 1073741824, "archive export requires protected regular unlinked bytes");
            const file = await fs.open(retained.path, "r");
            try { for await (const bytes of file.createReadStream({ autoClose: false })) await write(bytes); }
            finally { await file.close(); }
        }
        return;
    }
    requireValue(operation === "worker" && process.argv.length === 3, "fixed container entry required");
    const job = JSON.parse(await fs.readFile("/input/input.json", "utf8"));
    requireValue(job.schemaVersion === 1 && ["execution", "canary"].includes(job.operation), "invalid public container input");
    if (job.operation === "execution") {
        await fs.mkdir("/scratch/control", { mode: 0o700 });
        let descriptor;
        try {
            const evidence = await physicalRunner(job.input, job.launch);
            for (const file of evidence.files)
                await fs.writeFile(path.join("/scratch/control", file.path), file.bytes, { flag: "wx", mode: 0o400 });
            descriptor = { schemaVersion: 1, ...job.launch, historicalTerminated: true,
                files: evidence.files.map(({ path, size, sha256 }) => ({ path, size, sha256 })), blobs: evidence.blobs };
        } catch (error) {
            await terminateHistoricalProcesses();
            descriptor = { error: error.message };
        }
        await fs.writeFile("/scratch/control/ready.new", JSON.stringify(descriptor), { flag: "wx", mode: 0o400 });
        await fs.rename("/scratch/control/ready.new", "/scratch/control/ready.json");
        // Docker removes tmpfs on STOP. Only the supervisor remains until the outer transfer/cleanup finishes.
        setInterval(() => {}, 1000);
        return;
    }
    const result = await canary(job);
    if (job.operation === "canary" && job.input.mode === "malformed") return;
    process.stdout.write(JSON.stringify({ schemaVersion: 1, ...job.launch, result }));
}

if (require.main === module) main().catch(error => {
    console.error([error.message, error.cause?.code, error.cause?.message].filter(Boolean).join(": "));
    process.exitCode = 1;
});
