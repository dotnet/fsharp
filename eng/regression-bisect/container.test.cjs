const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs/promises");
const os = require("node:os");
const path = require("node:path");
const { spawnSync } = require("node:child_process");
const { validateContainerConfig, inspectRuntime, runContainer, image } = require("./container.cjs");

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
    const root = await fs.realpath(await fs.mkdtemp(path.join(os.tmpdir(), "container-boundary-")));
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
    const root = await fs.realpath(await fs.mkdtemp(path.join(os.tmpdir(), "container-ledger-")));
    t.after(() => fs.rm(root, { recursive: true, force: true }));
    const resources = new Map(), calls = [];
    let inputDirectory, failed = false;
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
                assert.equal(args[args.indexOf("--user") + 1], "1000:1000");
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
                    Id: name, Image: "sha256:" + "c".repeat(64), Config: { User: "1000:1000", Labels: labels },
                    AppArmorProfile: "docker-default",
                    HostConfig: { ReadonlyRootfs: true, Privileged: false, PidMode: "", NetworkMode: network, CapDrop: ["ALL"],
                        IpcMode: "private", CgroupnsMode: "private", SecurityOpt: ["no-new-privileges=true"],
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
            return value ? out([value]) : { exitCode: 1, stdout: Buffer.alloc(0), stderr: Buffer.from("No such object: " + args[2]) };
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
            return out(null);
        }
        if (args[1] === "start") {
            if (failure === "timeout") throw new Error("worker timeout");
            const job = JSON.parse(await fs.readFile(path.join(inputDirectory, "input.json")));
            assert.deepEqual(Object.keys(job).sort(), ["hostAddresses", "input", "launch", "limits", "operation", "proxy", "schemaVersion"]);
            if (failure === "binding") job.launch.launchId = "f".repeat(64);
            if (failure === "oom") resources.get(args.at(-1)).State.OOMKilled = true;
            return out({ schemaVersion: 1, ...job.launch, result: { mode: "inspect" } });
        }
        if (args[1] === "rm") {
            resources.delete(args.at(-1));
            return out(null);
        }
        assert.fail(`unexpected Docker command: ${args}`);
    };
    return { root, calls, resources, deps: { platform: "linux", command: run } };
}

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
