const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs/promises");
const path = require("node:path");
const { fixture } = require("./fixtures/reproduction.cjs");
const { seal, attempt } = require("./fixtures/contracts.cjs");
const { classifyAttempts } = require("./model.cjs");
const { selectFsiCommand, verifyProjectIdentities, evaluateAssertion, runReproduction } = require("./repro.cjs");
const { execute } = require("./repro-process.cjs");
const { isolatedEnvironment, hash } = require("./toolchain-io.cjs");

for (const [name, options] of [
    ["local DLL", {}], ["Windows executable", { executable: true }], ["released SDK", { kind: "released" }],
]) {
    test(`FSI selects the inventoried ${name}`, async t => {
        const f = await fixture(t, options);
        const selected = await selectFsiCommand({ ...f, cell: f.manifest.matrix[0] });
        assert.equal(selected.executable, options.executable
            ? f.product.files.find(x => x.role === "fsi").path : f.toolchain.hostPath);
        assert.ok(!selected.args.includes("fsi"));
        if (!options.executable) {
            assert.ok(selected.args.includes(f.product.files.find(x => x.role === "fsi").path));
            assert.deepEqual(selected.args.slice(0, 3), ["exec", "--fx-version", "10.0.0"]);
        }
    });
}

for (const [name, mutate, options] of [
    ["wrong host", f => { f.toolchain.hostPath = path.join(f.root, "dotnet-other", "dotnet.exe"); }],
    ["host dotnet fsi", f => { f.product.files.find(x => x.role === "fsi").path = "fsi"; }],
    ["missing runtime", f => { f.toolchain.runtimes = []; }],
    ["missing executable runtime", f => { f.toolchain.runtimes = []; }, { executable: true }],
    ["wrong architecture", f => { f.toolchain.architecture = "arm64"; }],
    ["ambiguous layout", f => { f.product.files.push({ ...f.product.files[0] }); }],
    ["wrong artifact bytes", async f => { await fs.appendFile(f.product.files[0].path, "changed"); }],
    ["wrong runtimeconfig", async f => { await fs.writeFile(f.product.files[0].runtimeConfig.path, "{}"); }],
    ["released fallback for local", f => { f.product.root = f.toolchain.sdkBasePath; }],
]) {
    test(`FSI rejects ${name}`, async t => {
        const f = await fixture(t, options);
        await mutate(f);
        await assert.rejects(selectFsiCommand({ ...f, cell: f.manifest.matrix[0] }));
    });
}

test("runtime and diagnostic assertions use the model vocabulary and exact evidence", async t => {
    const f = await fixture(t);
    const execution = { status: "completed", complete: true, ...f.manifest.assertions.pass };
    for (const [name, phase, changes, expected] of [
        ["runtime pass", "runtime", {}, "pass"],
        ["runtime-only failure", "runtime", f.manifest.assertions.failure, "product-failure"],
        ["successful build is not runtime success", "build", {}, "untestable"],
        ["unrelated exception", "runtime", { stderr: "System.NullReferenceException" }, "untestable"],
        ["runtime loader", "runtime", { stderr: "You must install or update .NET to run this application.", exitCode: 1 }, "untestable"],
        ["truncation", "runtime", { complete: false }, "untestable"],
        ["timeout", "runtime", { status: "timeout" }, "untestable"],
        ["crash", "runtime", { status: "exception" }, "untestable"],
    ]) {
        assert.equal(evaluateAssertion(f.manifest, { ...execution, ...changes }, { phase }).outcome, expected, name);
    }
    f.manifest.assertions = {
        pass: { exitCode: 0, stdout: "", stderr: "" },
        failure: { exitCode: 1, stdout: "", stderr: "repro.fs(3,5,3,9): error FS0001: frozen message\n" },
    };
    seal(f.manifest);
    for (const [stderr, outcome] of [
        [f.manifest.assertions.failure.stderr, "product-failure"],
        ["repro.fs(3,5,3,9): error FS0001: different message\n", "untestable"],
        ["repro.fs(3,6,3,9): error FS0001: frozen message\n", "untestable"],
        ["repro.fs(3,5,3,9): error FS0039: frozen message\n", "untestable"],
        ["error NU1301: restore failed\n", "untestable"],
    ]) assert.equal(evaluateAssertion(f.manifest, { ...execution, exitCode: 1, stdout: "", stderr }, { phase: "compile" }).outcome, outcome);
    assert.equal(evaluateAssertion(f.manifest, { ...execution, stdout: "" }, { phase: "compile" }).outcome, "pass");
});

test("mixed repetitions remain untestable", () => {
    const { manifest } = require("./fixtures/contracts.cjs");
    assert.deepEqual(classifyAttempts([attempt("pass"), attempt("product-failure")], manifest()), {
        schemaVersion: 1, outcome: "untestable", reason: "flaky",
    });
});

async function projectFixture(t, options) {
    const f = await fixture(t, options);
    const project = await f.file("source/App.fsproj", "<Project />");
    f.manifest.kind = "project";
    f.manifest.sources = [{ path: "App.fsproj", sha256: project.sha256 }];
    for (const c of f.manifest.matrix) { c.project = "App.fsproj"; c.command = ["run-project", c.project]; }
    seal(f.manifest);
    const roles = Object.fromEntries(f.product.files.map(x => [x.role, x]));
    const sdkImports = [];
    for (const name of ["Sdk.props", "Sdk.targets"]) sdkImports.push((await f.file(
        path.join("dotnet", "sdk", "10.0.100", "Sdks", "Microsoft.NET.Sdk", "Sdk", name))).path);
    const packageRoot = path.join(f.root, "packages");
    const core = await f.file("packages/fsharp.core/10.0.100/lib/netstandard2.0/FSharp.Core.dll", await fs.readFile(roles.core.path));
    const nupkg = await f.file("packages/fsharp.core/10.0.100/fsharp.core.10.0.100.nupkg", await fs.readFile(roles.corePackage.path));
    f.packageRoot = packageRoot;
    f.evidence = f.manifest.matrix.map(c => ({
        project: project.path, targetFramework: c.tfm, configuration: c.configuration, languageVersion: c.languageVersion,
        optimize: c.configuration === "Release", defines: [], sdkVersion: f.toolchain.sdkVersion, hostPath: f.toolchain.hostPath,
        imports: [...["props", "targets", "sdkTargets", "overrides"].map(r => roles[r].path), ...sdkImports],
        projectReferences: [], complete: true,
        fsc: [{ compilerPath: roles.compiler.path, taskAssembly: roles.tasks.path, targetsPath: roles.targets.path, hostPath: f.toolchain.hostPath,
            references: [core.path], languageVersion: c.languageVersion, optimize: c.configuration === "Release",
            defines: [], commandLine: `"${f.toolchain.hostPath}" "${roles.compiler.path}" --langversion:${c.languageVersion}` }],
        core: { compile: core, runtime: core, package: { ...nupkg, id: "FSharp.Core", version: "10.0.100", root: packageRoot } },
    }));
    return f;
}

test("every project/TFM/configuration cell is verified", async t => {
    for (const configuration of ["Debug", "Release"]) {
        const f = await projectFixture(t, { tfms: ["net9.0", "net10.0"], configuration });
        const result = await verifyProjectIdentities(f);
        assert.equal(result.status, "verified", result.error);
    }
});

for (const [name, mutate] of [
    ["compiler fallback", f => { f.evidence[0].fsc[0].compilerPath = f.toolchain.hostPath; }],
    ["task fallback", f => { f.evidence[0].fsc[0].taskAssembly = f.toolchain.hostPath; }],
    ["targets fallback", f => { f.evidence[0].imports.splice(1, 1); }],
    ["SDK fallback", f => { f.evidence[0].imports.pop(); }],
    ["executed target replaced after import", f => { f.evidence[0].fsc[0].targetsPath = f.toolchain.hostPath; }],
    ["Core fallback", f => { f.evidence[0].fsc[0].references = [f.product.files.find(x => x.role === "core").path]; }],
    ["same version wrong Core bytes", async f => { await fs.writeFile(f.evidence[0].core.compile.path, "other Core"); }],
    ["same version wrong package bytes", async f => { await fs.writeFile(f.evidence[0].core.package.path, "other package"); }],
    ["late property override", f => { f.evidence[0].fsc[0].languageVersion = "preview"; }],
    ["late optimization override", f => { f.evidence[0].fsc[0].optimize = false; }],
    ["late define override", f => { f.evidence[0].fsc[0].defines = ["DIFFERENT"]; }],
    ["missing inner TFM", f => { f.evidence.pop(); }],
    ["no Fsc invocation", f => { f.evidence[0].fsc = []; }],
    ["wrong host", f => { f.evidence[0].fsc[0].hostPath = "dotnet"; }],
    ["producer rebuild", f => { f.evidence[0].projectReferences = ["Producer.fsproj"]; }],
    ["false prefix", async f => {
        const other = await f.file("packages-sibling/FSharp.Core.dll", "local/FSharp.Core.dll");
        f.evidence[0].core.compile = other; f.evidence[0].fsc[0].references = [other.path];
    }],
    ["symlink", async f => {
        const other = await f.file("outside/FSharp.Core.dll", "local/FSharp.Core.dll");
        const link = path.join(f.packageRoot, "escape");
        await fs.symlink(path.dirname(other.path), link, "junction");
        f.evidence[0].core.compile = { ...other, path: path.join(link, "FSharp.Core.dll") };
        f.evidence[0].fsc[0].references = [f.evidence[0].core.compile.path];
    }],
]) {
    test(`project rejects ${name}`, async t => {
        const f = await projectFixture(t, { tfms: ["net9.0", "net10.0"] });
        await mutate(f);
        assert.equal((await verifyProjectIdentities(f)).status, "untestable");
    });
}

test("runner rejects unfrozen sources before invoking any process", async t => {
    const f = await fixture(t);
    await fs.appendFile(path.join(f.sourceDirectory, "repro.fsx"), " changed");
    let invoked = false;
    const result = await runReproduction({ ...f, workDirectory: path.join(f.root, "work") }, {
        execute: async () => { invoked = true; throw new Error("must not execute"); },
    });
    assert.equal(result.outcome.outcome, "untestable");
    assert.equal(invoked, false);
});

async function runnerFixture(t) {
    const f = await fixture(t);
    f.workDirectory = path.join(f.root, "work");
    f.helper = { executable: "helper-host", dll: "helper.dll", runtimeVersion: "10.0.0", hook: "CoreHook.dll" };
    let calls = 0;
    f.execute = async (executable, args, settings) => {
        const result = { status: "completed", complete: true, exitCode: 0, stdout: "", stderr: "" };
        if (executable === f.helper.executable) {
            const files = args.slice(args.indexOf("inspect") + 1);
            result.stdout = JSON.stringify(files.map(location => {
                const record = f.product.files.find(p => p.path === location) ?? f.inspectFiles?.find(p => p.path === location);
                if (record.role === "corePackage") return {
                    ...record, id: "FSharp.Core", version: f.product.corePackageVersion,
                    entries: [{ path: "lib/netstandard2.0/FSharp.Core.dll", sha256: f.product.files.find(p => p.role === "core").sha256 }],
                };
                return record;
            }));
        } else {
            const core = f.product.files.find(p => p.role === "core");
            await fs.writeFile(settings.env.REPRO_CORE_EVIDENCE, JSON.stringify({
                ...core, complete: true, assemblyName: "FSharp.Core", runtimeVersion: "10.0.0",
                runtimePath: path.join(f.toolchain.runtimes[0].path, "10.0.0"), architecture: "x64", ...(f.loadedCore ?? {}),
            }));
            Object.assign(result, f.outputs?.[calls++] ?? f.manifest.assertions.pass);
        }
        return result;
    };
    return f;
}

test("runner executes inventoried FSI twice in independent credential-free package caches", async t => {
    const f = await runnerFixture(t);
    const environments = [];
    const result = await runReproduction(f, { execute: (file, args, settings) => {
        environments.push(settings.env);
        return f.execute(file, args, settings);
    } });
    assert.equal(result.outcome.outcome, "pass", result.error);
    assert.equal(result.cells[0].attempts.length, 2);
    const commands = result.cells[0].evidence.map(e => e.commands[0]);
    assert.ok(commands.every(c => c.executable === f.toolchain.hostPath && !c.args.includes("fsi")));
    const caches = new Set(environments.map(e => e.NUGET_PACKAGES));
    assert.equal(caches.size, 3);
    assert.ok(environments.every(e => !Object.keys(e).some(k => /TOKEN|SECRET|PASSWORD/.test(k) && k !== "NUGET_CREDENTIALPROVIDER_SESSIONTOKENCACHE_ENABLED")));
    assert.equal(JSON.parse(await fs.readFile(path.join(result.workDirectory, "result.json"))).outcome.outcome, "pass");
});

for (const [name, change, expected] of [
    ["wrong loaded Core", f => { f.loadedCore = { sha256: "0".repeat(64) }; }, "wrong-identity"],
    ["wrong loaded Core path", f => { f.loadedCore = { path: f.product.files.find(p => p.role === "fsi").path }; }, "wrong-identity"],
    ["wrong loaded Core version", f => { f.loadedCore = { assemblyVersion: "1.0.0.0" }; }, "wrong-identity"],
    ["wrong loaded runtime", f => { f.loadedCore = { runtimeVersion: "9.0.0" }; }, "wrong-identity"],
    ["wrong loaded runtime path", f => { f.loadedCore = { runtimePath: f.root }; }, "wrong-identity"],
    ["wrong loaded architecture", f => { f.loadedCore = { architecture: "arm64" }; }, "wrong-identity"],
    ["flaky actual repetitions", f => { f.outputs = [f.manifest.assertions.pass, f.manifest.assertions.failure]; }, "flaky"],
    ["truncated assertion output", f => { f.outputs = [{ ...f.manifest.assertions.pass, complete: false }]; }, "incomplete-evidence"],
    ["timed out reproduction", f => { f.outputs = [{ ...f.manifest.assertions.pass, status: "timeout", exitCode: null }]; }, "timeout"],
]) {
    test(`runner classifies ${name}`, async t => {
        const f = await runnerFixture(t);
        change(f);
        const result = await runReproduction(f, { execute: f.execute });
        assert.equal(result.outcome.reason, expected, result.error);
    });
}

test("bounded process capture retains no more than the configured bytes", async t => {
    const f = await fixture(t);
    const result = await execute(process.execPath, ["-e", "process.stdout.write('x'.repeat(100000));process.stderr.write('y'.repeat(100000))"], {
        cwd: f.root, env: isolatedEnvironment(f.root, { os: process.platform === "win32" ? "windows" : "linux", architecture: process.arch }),
        timeoutMs: 30000, maxBytes: 128,
    });

    test("a frozen managed exception exit code is an assertion, not automatically a setup crash", async t => {
        const f = await fixture(t);
        f.manifest.assertions.failure = {
            exitCode: process.platform === "win32" ? 3762504530 : 82,
            stdout: "", stderr: "Unhandled exception. System.InvalidOperationException: FROZEN_EXCEPTION\n",
        };
        seal(f.manifest);
        const execution = await execute(process.execPath, ["-e",
            `process.stderr.write(${JSON.stringify(f.manifest.assertions.failure.stderr)});process.exit(-532462766)`], {
            cwd: f.root, env: isolatedEnvironment(f.root, { os: process.platform === "win32" ? "windows" : "linux", architecture: process.arch }),
            timeoutMs: 30000,
        });
        assert.equal(evaluateAssertion(f.manifest, execution).outcome, "product-failure", JSON.stringify(execution));
        assert.equal(evaluateAssertion(f.manifest, {
            status: "completed", complete: true, ...f.manifest.assertions.failure,
            stderr: "Unhandled exception. System.NullReferenceException: unexpected\n",
        }).outcome, "untestable");
    });
    assert.equal(result.exitCode, 0);
    assert.equal(result.complete, false);
    assert.equal(Buffer.byteLength(result.stdout), 128);
    assert.equal(Buffer.byteLength(result.stderr), 128);
});

test("timeout kills a real child process tree", async t => {
    const f = await fixture(t);
    const result = await execute(process.execPath, ["-e",
        "const c=require('node:child_process').spawn(process.execPath,['-e','setInterval(()=>{},1000)'],{stdio:'ignore'});console.log(c.pid);setInterval(()=>{},1000)"], {
        cwd: f.root, env: isolatedEnvironment(f.root, { os: process.platform === "win32" ? "windows" : "linux", architecture: process.arch }),
        timeoutMs: 3000,
    });
    assert.equal(result.status, "timeout");
    const pid = Number(result.stdout.trim());
    assert.ok(Number.isSafeInteger(pid) && pid > 0, result.error);
    assert.throws(() => process.kill(pid, 0), { code: "ESRCH" });
});

test("same-version local Core packages never share a cache across observations", async t => {
    const f = await runnerFixture(t);
    const first = await runReproduction(f, { execute: f.execute });
    for (const role of ["core", "corePackage"]) {
        const record = f.product.files.find(p => p.role === role);
        const bytes = `second-build-${role}`;
        await fs.writeFile(record.path, bytes);
        record.sha256 = hash(bytes);
    }
    const second = await runReproduction(f, { execute: f.execute });
    assert.equal(first.outcome.outcome, "pass", first.error);
    assert.equal(second.outcome.outcome, "pass", second.error);
    assert.notEqual(first.productFingerprint, second.productFingerprint);
    assert.notEqual(first.workDirectory, second.workDirectory);
});

for (const [tamper, compressed] of [[false, false], [true, false], [false, true]]) test(
    `frozen producer metadata: tamper=${tamper}, compressed=${compressed}`, async t => {
    const f = await runnerFixture(t);
    const dll = await f.file("source/producer/Old.dll", "old inline metadata");
    const pkg = await f.file("source/producer/Old.1.0.0.nupkg", "old package bytes");
    const resources = [{ name: `FSharpSignature${compressed ? "Compressed" : ""}Data.Old`, sha256: hash("old inline signature") }];
    f.inspectFiles = [{ ...dll, assemblyVersion: "1.0.0.0", resources }];
    const descriptor = {
        role: "immutable-producer", compiler: { version: "8.0.100", sha256: hash("released compiler") },
        flags: ["--optimize+", "--define:OLD_PRODUCER"], tfm: "net8.0", packages: [],
        dll: { path: "producer/Old.dll", sha256: dll.sha256, resources },
        package: { path: "producer/Old.1.0.0.nupkg", sha256: pkg.sha256, id: "Old", version: "1.0.0" },
    };
    const provenance = await f.file("source/old.producer.json", JSON.stringify(descriptor));
    f.manifest.sources.push(
        { path: descriptor.dll.path, sha256: dll.sha256 },
        { path: descriptor.package.path, sha256: pkg.sha256 },
        { path: "old.producer.json", sha256: provenance.sha256 });
    f.manifest.producer = { name: "Old", version: "1.0.0", sha256: dll.sha256 };
    f.manifest.packages = [{ id: "Old", version: "1.0.0", sha256: pkg.sha256 }];
    seal(f.manifest);
    const result = await runReproduction(f, { execute: async (file, args, settings) => {
        const result = await f.execute(file, args, settings);
        if (tamper && file !== f.helper.executable) await fs.writeFile(path.join(settings.cwd, "producer", "Old.dll"), "rebuilt metadata hides failure");
        return result;
    } });
    assert.equal(result.outcome.outcome, tamper ? "untestable" : "pass", result.error);
    if (tamper) assert.match(result.error, /artifact bytes changed/);
    else {
        assert.deepEqual(result.cells[0].evidence[0].producers[0], descriptor);
        assert.equal(hash(await fs.readFile(dll.path)), dll.sha256);
        assert.equal(hash(await fs.readFile(pkg.path)), pkg.sha256);
    }
});
