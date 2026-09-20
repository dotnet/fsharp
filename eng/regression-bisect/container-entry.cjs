const fs = require("node:fs/promises");
const path = require("node:path");
const net = require("node:net");
const { spawn } = require("node:child_process");
const { createEgressBroker } = require("./egress.cjs");
const { runExecutionUnit, validateExecutionUnit } = require("./worker.cjs");
const { acquireToolchain } = require("./toolchains.cjs");
const { execute } = require("./repro-process.cjs");
const { hash, fileIdentity, isolatedEnvironment, requireValue } = require("./toolchain-io.cjs");
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
        spawn(process.execPath, ["-e", "setInterval(() => {}, 1000)"], { detached: true, stdio: "ignore" }).unref();
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
    let readonly = false;
    try { await fs.writeFile("/trusted/probe", "must fail"); }
    catch (error) { requireValue(["EROFS", "EACCES"].includes(error.code), error.message); readonly = true; }
    requireValue(readonly, "trusted bundle is writable");
    return { mode: input.mode, denials, direct, proxied, cgroups, uid: process.getuid(),
        statusSha256: hash(status), legitimateTransfer: { bytes: bytes.length, sha256: hash(bytes) } };
}

async function physicalRunner(unit) {
    validateExecutionUnit(unit);
    const signal = AbortSignal.timeout(Math.max(1, Date.parse(unit.deadline) - Date.now()));
    const withProxy = (executable, args, settings) => execute(executable, args, {
        ...settings, signal,
        env: { ...settings.env, ...Object.fromEntries(["HTTP_PROXY", "HTTPS_PROXY", "http_proxy", "https_proxy"].map(k => [k, process.env[k]])) },
    });
    const storage = "/scratch/storage";
    for (const name of [storage, "/scratch/work", "/scratch/evidence", "/scratch/executions"]) await fs.mkdir(name);
    const acquired = await acquireToolchain({ policy, toolchain: unit.resolved, storageDirectory: storage, signal }, { run: withProxy });
    if (acquired.status !== "acquired") return acquired;
    const toolchain = acquired.identity;
    const helperDirectory = "/scratch/helper";
    await fs.cp("/trusted/eng/regression-bisect/evidence", helperDirectory, { recursive: true });
    const env = isolatedEnvironment("/scratch", unit.platform);
    for (const key of ["HTTP_PROXY", "HTTPS_PROXY", "http_proxy", "https_proxy"]) env[key] = process.env[key];
    for (const dir of [env.HOME, env.TEMP, env.APPDATA, env.LOCALAPPDATA]) await fs.mkdir(dir, { recursive: true });
    const runtime = toolchain.compilerRuntime.version;
    const helperOutput = path.join(helperDirectory, "output");
    const built = await withProxy(toolchain.hostPath, ["build", path.join(helperDirectory, "Evidence.csproj"),
        "-c", "Release", "-o", helperOutput, `-p:EvidenceTargetFramework=net${runtime.split(".")[0]}.0`, "-nr:false", "--nologo"],
    { cwd: helperDirectory, env, timeoutMs: 300000, maxBytes: 1024 * 1024 });
    requireValue(built.complete && built.exitCode === 0, `isolated evidence helper build failed: ${built.stderr}`);
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
    return runExecutionUnit(unit, {
        policy, inventoryReleased, acquireToolchain: async () => acquired,
        workerExecute: withProxy, workspaceDependencies: { execute: withProxy },
        worker: { storageDirectory: storage, scratchDirectory: "/scratch/work", evidenceDirectory: "/scratch/evidence",
            workDirectory: "/scratch/executions", sourceDirectory: "/input/samples", mirrorDirectory: "/input/source.bundle",
            gitExecutable: "/usr/bin/git", reproductionToolchain: toolchain, helper },
    }, { signal });
}

async function main() {
    const [operation] = process.argv.slice(2);
    requireValue(process.argv.length === 3, "fixed container entry required");
    if (operation === "broker") {
        const broker = createEgressBroker();
        broker.server.listen(3128, "0.0.0.0");
        for (const signal of ["SIGTERM", "SIGINT"]) process.once(signal, () => {
            broker.close().catch(error => { console.error(error.message); process.exitCode = 1; });
        });
        return;
    }
    if (operation === "ready") {
        requireValue(await connect("127.0.0.1", 3128) === "connected", "broker is not responsive");
        return;
    }
    requireValue(operation === "worker" && process.argv.length === 3, "fixed container entry required");
    const job = JSON.parse(await fs.readFile("/input/input.json", "utf8"));
    requireValue(job.schemaVersion === 1 && ["execution", "canary"].includes(job.operation), "invalid public container input");
    const result = job.operation === "canary" ? await canary(job) : await physicalRunner(job.input);
    if (job.operation === "canary" && job.input.mode === "malformed") return;
    process.stdout.write(JSON.stringify({ schemaVersion: 1, ...job.launch, result }));
}

if (require.main === module) main().catch(error => { console.error(error.message); process.exitCode = 1; });
