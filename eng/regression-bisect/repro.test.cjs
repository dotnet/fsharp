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
    f.product.coreAssets = [{ path: "lib/netstandard2.0/FSharp.Core.dll", sha256: core.sha256 }];
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
]) {
    test(`project rejects ${name}`, async t => {
        const f = await projectFixture(t, { tfms: ["net9.0", "net10.0"] });
        await mutate(f);
        assert.equal((await verifyProjectIdentities(f)).status, "untestable");
    });
}

for (const [name, mutate] of [
    ["false prefix", async f => {
        const pkg = f.evidence[0].core.package;
        Object.assign(pkg, await f.file(path.join("packages-sibling", "fsharp.core", pkg.version, path.basename(pkg.path)),
            await fs.readFile(pkg.path)));
        return pkg.path;
    }],
    ["symlink", async f => {
        const asset = f.evidence[0].core.compile;
        const directory = path.dirname(asset.path);
        const outside = path.join(f.root, "outside");
        await fs.rename(directory, outside);
        await fs.symlink(outside, directory, "junction");
        return asset.path;
    }],
]) {
    test(`project containment rejects ${name}`, async t => {
        const f = await projectFixture(t);
        assert.equal((await verifyProjectIdentities(f)).status, "verified");
        const rejectedPath = await mutate(f);
        const result = await verifyProjectIdentities(f);
        assert.equal(result.status, "untestable");
        assert.equal(result.error, `artifact escapes expected root: ${rejectedPath}`);
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

async function runnerFixture(t, options = {}) {
    const f = await (options.project ? projectFixture : fixture)(t, options);
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

async function projectRunnerFixture(t, { declaredTfms = [], libraryTfm = "net10.0", mutate, buildExitCode = 0,
    undeclared = false, nullable, defines = nullable ? ["NULLABLE"] : [] } = {}) {
    const f = await runnerFixture(t, { project: true });
    const library = await f.file("source/Library.fsproj", "<Project />");
    if (!undeclared) f.manifest.sources.push({ path: "Library.fsproj", sha256: library.sha256 });
    const app = f.manifest.matrix[0];
    for (const tfm of declaredTfms) f.manifest.matrix.push({
        ...app, id: `library-${tfm}`, project: "Library.fsproj", tfm, languageVersion: "9.0",
        command: ["run-project", "Library.fsproj"],
    });
    seal(f.manifest);
    f.product.configuration = app.configuration;
    const roles = Object.fromEntries(f.product.files.map(p => [p.role, p]));
    roles.compiler.runtimeConfig = roles.fsi.runtimeConfig;
    const baseExecute = f.execute;
    f.buildCalls = [];
    f.runtimeCalls = [];
    let structured;
    f.execute = async (executable, args, settings) => {
        const success = { status: "completed", complete: true, exitCode: 0, stdout: "", stderr: "" };
        if (executable === f.helper.executable && args.includes("binlog")) {
            return { ...success, stdout: JSON.stringify(structured) };
        }
        if (args[0] !== "build") {
            if (executable !== f.helper.executable) f.runtimeCalls.push(args);
            return baseExecute(executable, args, settings);
        }
        assert.equal(executable, f.toolchain.hostPath);
        f.buildCalls.push(args);
        const cell = f.manifest.matrix.find(c => path.join(settings.cwd, c.project) === args[1] &&
            args.includes(`-p:TargetFramework=${c.tfm}`));
        assert.ok(cell, "build must select a declared root cell");
        const write = (location, contents) => f.file(path.relative(f.root, location), contents);
        const packageDirectory = path.join(settings.env.NUGET_PACKAGES, "fsharp.core", f.product.corePackageVersion);
        const core = await write(path.join(packageDirectory, "lib", "netstandard2.0", "FSharp.Core.dll"),
            await fs.readFile(roles.core.path));
        await write(path.join(packageDirectory, "fsharp.core.10.0.100.nupkg"), await fs.readFile(roles.corePackage.path));
        f.inspectFiles = [{ ...roles.core, ...core }];
        f.loadedCore = core;
        const builds = [cell];
        if (cell.project === app.project) builds.push(f.manifest.matrix.find(c =>
            c.project === "Library.fsproj" && c.tfm === libraryTfm) ?? { ...cell, project: "Library.fsproj", tfm: libraryTfm });
        const rows = [];
        for (const [index, current] of builds.entries()) {
            const row = structuredClone(f.evidence[0]);
            Object.assign(row, {
                project: path.join(settings.cwd, current.project), targetFramework: current.tfm,
                configuration: current.configuration, expectedLanguageVersion: current.languageVersion,
                expectedOptimize: current.configuration === "Release", expectedDefines: defines, expectedNullable: nullable,
                projectReferences: current.project === app.project ? ["Library.fsproj"] : [],
                assetsFile: path.join(settings.cwd, ".runner", `${index}.assets.json`),
                targetPath: path.join(settings.cwd, "bin", current.tfm, `${path.parse(current.project).name}.dll`),
                fscTasks: row.fsc.map(task => ({
                    ...task, references: [core.path], languageVersion: current.languageVersion, defines, nullable,
                })),
            });
            delete row.fsc;
            delete row.core;
            const identity = `FSharp.Core/${f.product.corePackageVersion}`;
            await write(row.assetsFile, JSON.stringify({
                targets: { [current.tfm]: { [identity]: {
                    compile: { "lib/netstandard2.0/FSharp.Core.dll": {} }, runtime: { "lib/netstandard2.0/FSharp.Core.dll": {} },
                } } },
                libraries: { [identity]: { type: "package", path: `fsharp.core/${f.product.corePackageVersion}` } },
                packageFolders: { [settings.env.NUGET_PACKAGES]: {} },
            }));
            await write(row.targetPath, "synthetic output");
            await write(path.join(settings.cwd, ".runner", `compiler-core-${index}.jsonl`), JSON.stringify({
                ...roles.core, assemblyName: "FSharp.Core", complete: true, entryAssembly: roles.compiler.path,
                runtimeVersion: "10.0.0", runtimePath: path.join(f.toolchain.runtimes[0].path, "10.0.0"), architecture: "x64",
            }));
            rows.push(row);
        }
        structured = { cells: rows, diagnostics: [] };
        if (mutate) await mutate(structured, f);
        await write(args.find(arg => arg.startsWith("-bl:")).slice(4), "synthetic binlog");
        return { ...success, exitCode: buildExitCode };
    };
    return f;
}

for (const [name, options, error] of [
    ["inherited same-TFM library", {}],
    ["declared same-TFM library wins over another declaration", { declaredTfms: ["net9.0", "net10.0"] }],
    ["single declared different-TFM library", { declaredTfms: ["net9.0"], libraryTfm: "net9.0" }],
    ...[false, true].flatMap(nullable => [
        [`nullable ${nullable ? "enabled" : "disabled"}`, { nullable }],
        [`late library nullable ${nullable ? "disable" : "enable"} with explicit NULLABLE define`, {
            nullable, defines: ["NULLABLE"], mutate: s => { s.cells[1].fscTasks[0].nullable = !nullable; },
        }, /late compiler flag override/],
        [`missing library nullable evidence when ${nullable ? "enabled" : "disabled"}`, {
            nullable, mutate: s => { s.cells[1].fscTasks[0].nullable = null; },
        }, /late compiler flag override/],
    ]),
    ["failed referenced-project evidence", {
        buildExitCode: 1, mutate: s => { s.cells[1].complete = false; },
    }, /no unique executed Fsc task: cell-0-dependency-1/],
    ["missing referenced-project evidence", { mutate: s => { s.cells.pop(); } },
        /missing or ambiguous expected inner build: Library\.fsproj net10\.0/],
    ["wrong referenced-project TFM", { libraryTfm: "net9.0" },
        /missing or ambiguous expected inner build: Library\.fsproj net10\.0/],
    ["ambiguous declared TFMs", { declaredTfms: ["net8.0", "net9.0"], libraryTfm: "net9.0" },
        /ambiguous referenced project TFM/],
    ["undeclared reference", { undeclared: true },
        /undeclared project reference \(immutable producers must not be rebuilt\)/],
    ["library without Fsc", { mutate: s => { s.cells[1].fscTasks = []; } },
        /no unique executed Fsc task: cell-0-dependency-1/],
    ["library compiler fallback", { mutate: (s, f) => { s.cells[1].fscTasks[0].compilerPath = f.toolchain.hostPath; } },
        /compilerPath fallback or late override/],
    ["library Core fallback", { mutate: (s, f) => {
        s.cells[1].fscTasks[0].references = [f.product.files.find(p => p.role === "core").path];
    } }, /Fsc did not reference selected Core/],
]) {
    test(`project runner verifies expanded matrix: ${name}`, async t => {
        const f = await projectRunnerFixture(t, options);
        const result = await runReproduction(f, { execute: f.execute });
        if (error) {
            assert.equal(result.outcome.outcome, "untestable", result.error);
            assert.equal(result.outcome.reason, "wrong-identity");
            assert.match(result.error, error);
            assert.equal(f.buildCalls.length, 1);
            assert.equal(f.runtimeCalls.length, 0, "rejected dependency evidence must prevent runtime execution");
        } else {
            assert.equal(result.outcome.outcome, "pass", result.error);
            assert.equal(result.cells.length, f.manifest.matrix.length);
            assert.equal(f.buildCalls.length, result.cells.length * 2);
            assert.equal(f.runtimeCalls.length, f.buildCalls.length);
            for (const [index, cell] of result.cells.entries()) {
                const root = f.manifest.matrix[index];
                const expected = [[root.project, root.tfm, root.configuration, root.languageVersion]];
                if (root.project === "App.fsproj") expected.push([
                    "Library.fsproj", options.libraryTfm ?? root.tfm, root.configuration,
                    options.declaredTfms ? "9.0" : root.languageVersion,
                ]);
                assert.equal(cell.attempts.length, 2);
                for (const record of cell.evidence) {
                    assert.deepEqual(record.projects.map(p => [
                        path.basename(p.project), p.targetFramework, p.configuration, p.languageVersion,
                    ]), expected);
                    assert.ok(record.projects.every(p => p.nullable === options.nullable), "nullable snapshot must survive row projection");
                    assert.equal(record.compilerCoreLoads.length, expected.length);
                    assert.equal(record.commands.at(-1).args.at(-1), record.projects[0].targetPath);
                }
            }
        }
    });
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

for (const mode of ["timeout", "cancelled"]) test(`process ${mode} contains descendants after their parent exits`, async t => {
    const f = await fixture(t);
    const ready = path.join(f.root, "descendant.json");
    const controller = new AbortController();
    const descendant = `
        const fs = require('node:fs');
        fs.writeFileSync(${JSON.stringify(ready + ".new")}, JSON.stringify({ pid: process.pid, parent: process.ppid }));
        fs.renameSync(${JSON.stringify(ready + ".new")}, ${JSON.stringify(ready)});
        process.stdout.write('descendant stdout\\n');
        process.stderr.write('descendant stderr\\n');
        process.send('ready');
        process.disconnect();
        setInterval(() => {}, 1000);
    `;
    const execution = execute(process.execPath, ["-e", `
        const child = require('node:child_process').spawn(process.execPath, ['-e', ${JSON.stringify(descendant)}],
            { stdio: ['ignore', 'inherit', 'inherit', 'ipc'], detached: process.platform === 'win32' });
        child.once('message', () => process.exit(0));
    `], {
        cwd: f.root,
        env: isolatedEnvironment(f.root, { os: process.platform === "win32" ? "windows" : "linux", architecture: process.arch }),
        timeoutMs: mode === "timeout" ? 3000 : 30000, signal: controller.signal,
    });
    let pid;
    let deadline;
    try {
        const limit = Date.now() + 10000;
        let parent;
        while (Date.now() < limit) {
            try {
                ({ pid, parent } = JSON.parse(await fs.readFile(ready, "utf8")));
                try { process.kill(parent, 0); } catch (error) { if (error.code === "ESRCH") break; throw error; }
            } catch (error) { if (error.code !== "ENOENT") throw error; }
            await new Promise(resolve => setTimeout(resolve, 20));
        }
        assert.ok(pid > 0, "descendant must start");
        assert.throws(() => process.kill(parent, 0), { code: "ESRCH" }, "parent exits before termination");
        process.kill(pid, 0);
        if (mode === "cancelled") controller.abort();
        const result = await Promise.race([
            execution,
            new Promise((_, reject) => { deadline = setTimeout(() => reject(new Error("execution did not terminate within 6 seconds")), 6000); }),
        ]);
        assert.equal(result.status, mode, JSON.stringify(result));
        assert.equal(result.complete, false);
        assert.equal(result.error, undefined);
        assert.equal(result.stdout, "descendant stdout\n");
        assert.equal(result.stderr, "descendant stderr\n");
        assert.throws(() => process.kill(pid, 0), { code: "ESRCH" }, "descendant must not survive");
    } finally {
        clearTimeout(deadline);
        controller.abort();
        if (pid) {
            try { process.kill(pid, "SIGKILL"); } catch (error) { if (error.code !== "ESRCH") throw error; }
        }
        await execution;
    }
});

test("process already cancelled does not launch", async t => {
    const f = await fixture(t);
    const marker = path.join(f.root, "launched");
    const result = await execute(process.execPath, ["-e", `require('node:fs').writeFileSync(${JSON.stringify(marker)}, '')`], {
        cwd: f.root,
        env: isolatedEnvironment(f.root, { os: process.platform === "win32" ? "windows" : "linux", architecture: process.arch }),
        timeoutMs: 3000, signal: AbortSignal.abort(),
    });
    assert.deepEqual(result, { status: "cancelled", complete: false, exitCode: null, stdout: "", stderr: "" });
    await assert.rejects(fs.stat(marker), { code: "ENOENT" });
});

test("process preserves arguments, explicit environment, output and exit code", async t => {
    const f = await fixture(t);
    const args = ["", "with spaces", 'a\\"b', 'trailing\\', "\u96ea", "line\nbreak", "& | < > %PATH%"];
    const env = isolatedEnvironment(f.root, { os: process.platform === "win32" ? "windows" : "linux", architecture: process.arch });
    const previous = process.env.REPRO_PROCESS_SECRET;
    process.env.REPRO_PROCESS_SECRET = "must not inherit";
    try {
        const result = await execute(process.execPath, ["-e", `
            process.stdout.write(JSON.stringify({ args: process.argv.slice(1),
                secret: process.env.REPRO_PROCESS_SECRET, allowed: process.env.REPRO_ALLOWED_ENV }));
            process.stderr.write('stderr\\r\\n');
            process.exitCode = 17;
        `, ...args], { cwd: f.root, env: { ...env, REPRO_ALLOWED_ENV: "allowed" }, timeoutMs: 30000 });
        assert.equal(result.status, "completed", result.error);
        assert.equal(result.complete, true);
        assert.equal(result.exitCode, 17);
        assert.deepEqual(JSON.parse(result.stdout), { args, allowed: "allowed" });
        assert.equal(result.stderr, "stderr\n");
    } finally {
        if (previous === undefined) delete process.env.REPRO_PROCESS_SECRET;
        else process.env.REPRO_PROCESS_SECRET = previous;
    }
});

for (const missing of ["executable", "environment"]) test(`process missing ${missing} remains an exception`, async t => {
    const f = await fixture(t);
    const result = await execute(path.join(f.root, "does-not-exist.exe"), [], {
        cwd: f.root,
        env: missing === "environment" ? undefined
            : isolatedEnvironment(f.root, { os: process.platform === "win32" ? "windows" : "linux", architecture: process.arch }),
        timeoutMs: 30000, maxBytes: 0,
    });
    assert.equal(result.status, "exception", JSON.stringify(result));
    assert.equal(result.complete, false);
    assert.ok(result.error);
});

test("process cancellation during launch is bounded and never launches the target", { skip: process.platform !== "win32" }, async t => {
    const f = await fixture(t);
    const marker = path.join(f.root, "launched");
    const controller = new AbortController();
    const execution = execute(process.execPath, ["-e", `require('node:fs').writeFileSync(${JSON.stringify(marker)}, '')`], {
        cwd: f.root,
        env: isolatedEnvironment(f.root, { os: "windows", architecture: process.arch }),
        timeoutMs: 30000, signal: controller.signal,
    });
    controller.abort();
    const result = await execution;
    assert.equal(result.status, "cancelled");
    assert.equal(result.complete, false);
    assert.equal(result.error, undefined);
    await assert.rejects(fs.stat(marker), { code: "ENOENT" });
});

test("process completes and removes detached servers that close their output pipes", { skip: process.platform !== "win32" }, async t => {
    const f = await fixture(t);
    const ready = path.join(f.root, "server.pid");
    const server = `
        require('node:fs').writeFileSync(${JSON.stringify(ready)}, String(process.pid));
        process.send('ready');
        process.disconnect();
        setInterval(() => {}, 1000);
    `;
    try {
        const result = await execute(process.execPath, ["-e", `
            const server = require('node:child_process').spawn(process.execPath, ['-e', ${JSON.stringify(server)}],
                { detached: true, stdio: ['ignore', 'ignore', 'ignore', 'ipc'] });
            server.once('message', () => { process.stdout.write('build complete\\n'); process.exit(17); });
        `], {
            cwd: f.root, env: isolatedEnvironment(f.root, { os: "windows", architecture: process.arch }), timeoutMs: 3000,
        });
        const pid = Number(await fs.readFile(ready, "utf8"));
        assert.equal(result.status, "completed", JSON.stringify(result));
        assert.equal(result.complete, true);
        assert.equal(result.exitCode, 17);
        assert.equal(result.stdout, "build complete\n");
        assert.equal(result.stderr, "");
        assert.throws(() => process.kill(pid, 0), { code: "ESRCH" }, "detached build server must not survive");
    } finally {
        try {
            const pid = Number(await fs.readFile(ready, "utf8"));
            if (pid > 0) process.kill(pid, "SIGKILL");
        } catch (error) { if (!["ENOENT", "ESRCH"].includes(error.code)) throw error; }
    }
});
