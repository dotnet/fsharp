const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs/promises");
const path = require("node:path");
const { fixture } = require("./fixtures/reproduction.cjs");
const { seal } = require("./fixtures/contracts.cjs");
const { fingerprintInput } = require("./model.cjs");
const { hash, isolatedEnvironment } = require("./toolchain-io.cjs");
const { execute } = require("./repro-process.cjs");
const { prepareRevisionWorkspace, buildRevision, testSourceRevision } = require("./workspace.cjs");

const git = process.platform === "win32" ? "C:\\Program Files\\Git\\cmd\\git.exe" : "/usr/bin/git";
const platform = { os: "windows", architecture: "x64", rid: "win-x64" };
const ok = stdout => ({ status: "completed", complete: true, exitCode: 0, stdout, stderr: "" });

async function controlled(t) {
    const f = await fixture(t);
    const mirror = path.join(f.root, "mirror");
    await fs.mkdir(mirror);
    await fs.writeFile(path.join(f.root, "gitconfig"), "");
    const command = async args => {
        const result = await execute(git, ["-c", "core.hooksPath=", ...args], {
            cwd: mirror, env: { ...isolatedEnvironment(f.root, platform),
                GIT_CONFIG_NOSYSTEM: "1", GIT_CONFIG_GLOBAL: path.join(f.root, "gitconfig") }, timeoutMs: 30000,
        });
        assert.equal(result.exitCode, 0, result.stderr);
        return result.stdout.trim();
    };
    await command(["init"]);
    const shas = [];
    for (const [sdk, tfm, solution] of [["10.0.100", "net10.0", "FSharp.slnx"], ["9.0.100", "net9.0", "FSharp.sln"]]) {
        for (const [name, content] of Object.entries({
            "global.json": JSON.stringify({ sdk: { version: sdk }, tools: { dotnet: sdk } }),
            "eng/TargetFrameworks.props": `<Project><PropertyGroup><FSharpNetCoreProductTargetFramework>${tfm}</FSharpNetCoreProductTargetFramework></PropertyGroup></Project>`,
            "eng/Build.ps1": `param($restore,$build,$configuration,$pack,$bootstrap,$noVisualStudio,$msbuildEngine,$properties)\n${solution}\n/p:Projects=$projects\n@properties`,
            "eng/build.sh": `--bootstrap --pack --product-build\nproto.proj\nConfiguration=Proto\n${solution}`,
            "build.sh": '"$scriptroot/eng/build.sh" --restore --build "$@"',
            "Build.cmd": "eng\\Build.ps1 -restore -build",
            "eng/build-utils.ps1": 'function Make-BootstrapBuild { "proto.proj"; "Proto"; "Bootstrap" }',
            "proto.proj": '<Project><MSBuild Projects="src/fsi/fsiProject/fsi.fsproj" Properties="Configuration=$(Configuration)" /></Project>',
            "NuGet.config": "<configuration><packageSources><clear /></packageSources></configuration>",
            "src/fsi/fsiProject/fsi.fsproj": "<Project />",
            "src/fsc/fscProject/fsc.fsproj": "<Project />",
            "src/FSharp.Core/FSharp.Core.fsproj": "<Project />",
            "src/Microsoft.FSharp.Compiler/Microsoft.FSharp.Compiler.fsproj": "<Project />",
        })) await f.file(path.join("mirror", name), content);
        await command(["add", "."]);
        await command(["-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid", "commit", "-qm", sdk]);
        shas.push(await command(["rev-parse", "HEAD"]));
    }
    f.manifest.matrix[0].languageVersion = "preview";
    seal(f.manifest);
    const options = {
        request: {
            schemaVersion: 1, repository: "dotnet/fsharp", sourceSha: shas[0], phase: "main",
            manifestDigest: fingerprintInput(f.manifest), approvalDigest: f.manifest.approvedInputDigest,
            platform, allocation: { buildTimeoutMs: 60000, workerTimeoutMs: 120000 },
        },
        manifest: f.manifest, sourceDirectory: f.sourceDirectory, mirrorDirectory: mirror,
        scratchDirectory: path.join(f.root, "scratch"), evidenceDirectory: path.join(f.root, "evidence"),
        gitExecutable: git,
        helper: { executable: "helper-host", dll: "helper.dll", hook: "hook.dll", runtimeVersion: "10.0.0" },
    };
    for (const dir of [options.scratchDirectory, options.evidenceDirectory]) await fs.mkdir(dir);
    const builds = [];
    let activeSdk;
    const toolchains = {
        http: async url => {
            if (url.endsWith("releases-index.json")) return { status: 200, url, bytes: Buffer.from('{"releases-index":[]}') };
            return { status: 200, url, bytes: Buffer.from("a".repeat(128)) };
        },
        installer: async request => {
            activeSdk = request.version;
            const runtime = `${activeSdk.split(".")[0]}.0.0`;
            for (const [name, text] of Object.entries({
                "dotnet.exe": "host",
                [`shared/Microsoft.NETCore.App/${runtime}/System.Private.CoreLib.dll`]: "runtime",
                [`sdk/${activeSdk}/FSharp/fsi.runtimeconfig.json`]: JSON.stringify({ runtimeOptions: { framework: { name: "Microsoft.NETCore.App", version: runtime } } }),
                [`sdk/${activeSdk}/FSharp/fsi.deps.json`]: '{"libraries":{}}',
                [`sdk/${activeSdk}/FSharp/fsi.dll`]: "sdk-fsi",
                [`sdk/${activeSdk}/FSharp/fsc.dll`]: "sdk-fsc",
                [`sdk/${activeSdk}/FSharp/FSharp.Core.dll`]: "sdk-core",
            })) {
                const file = path.join(request.root, name);
                await fs.mkdir(path.dirname(file), { recursive: true });
                await fs.writeFile(file, text);
            }
            return { attempts: [], archive: { ...request.archive, redirectUrl: request.archive.url,
                actualSha512: request.archive.sha512, sha256: hash("archive") } };
        },
        run: async (exe, args) => {
            const root = path.dirname(exe), runtime = `${activeSdk.split(".")[0]}.0.0`;
            if (args[0] === "--info") return ok(`Base Path: ${path.join(root, "sdk", activeSdk)}\nRID: win-x64\n`);
            if (args[0] === "--version") return ok(activeSdk);
            if (args[0] === "--list-sdks") return ok(`${activeSdk} [${path.join(root, "sdk")}]`);
            if (args[0] === "--list-runtimes") return ok(`Microsoft.NETCore.App ${runtime} [${path.join(root, "shared", "Microsoft.NETCore.App")}]`);
            return ok(`ARCH|x64\nRUNTIME|${runtime}\n` +
                ["fsc", "fsi", "FSharp.Core"].map(n => `FILE|${n}.dll|${n}|1.0.0.0|1.0.0`).join("\n"));
        },
    };
    const properties = async (cwd, project) => {
        const global = JSON.parse(await fs.readFile(path.join(cwd, "global.json")));
        const tfm = `net${global.sdk.version.split(".")[0]}.0`;
        const name = path.basename(project, ".fsproj");
        return {
            TargetFramework: tfm, TargetFrameworks: tfm, TargetPath: path.join(cwd, "artifacts", "bin", name, "Release", tfm, `${name}.dll`),
            PackageId: "FSharp.Core", PackageVersion: "1.0.0", PackageOutputPath: path.join(cwd, "artifacts", "packages", "Release", "Shipping"),
        };
    };
    const deps = { toolchains, execute: async (exe, args, settings) => {
        if (exe === git) return execute(exe, args, settings);
        for (const key of ["GITHUB_TOKEN", "GH_TOKEN", "NUGET_AUTH_TOKEN", "SSH_AUTH_SOCK", "MSBuildSDKsPath"]) {
            assert.equal(settings.env[key], undefined);
        }
        if (args.includes("inspect")) {
            const result = [];
            for (const file of args.slice(args.indexOf("inspect") + 1)) {
                const bytes = await fs.readFile(file);
                result.push({ path: file, sha256: hash(bytes), assemblyVersion: "1.0.0.0", fileVersion: "1.0.0",
                    assemblyName: path.basename(file, ".dll"),
                    ...(file.endsWith(".nupkg") ? { id: "FSharp.Core", version: "1.0.0", entries: [
                        { path: "lib/netstandard2.0/FSharp.Core.dll", sha256: hash(`core-${builds.at(-1).sha}`) },
                    ] } : {}) });
            }
            return ok(JSON.stringify(result));
        }
        if (args[0] === "msbuild") return ok(JSON.stringify({ Properties: await properties(settings.cwd, args[1]) }));
        if (args.includes("--exec")) {
            const fsi = args[3];
            const core = path.join(path.dirname(fsi), "FSharp.Core.dll");
            const runtime = args[2];
            await fs.writeFile(settings.env.REPRO_CORE_EVIDENCE, JSON.stringify({
                path: core, sha256: hash(await fs.readFile(core)), assemblyVersion: "1.0.0.0",
                assemblyName: "FSharp.Core", complete: true, architecture: "x64", runtimeVersion: runtime,
                runtimePath: path.join(path.dirname(exe), "shared", "Microsoft.NETCore.App", runtime),
            }));
            return ok(f.manifest.assertions.pass.stdout);
        }
        const source = settings.cwd;
        const sha = (await execute(git, ["rev-parse", "HEAD"], { ...settings, timeoutMs: 30000 })).stdout.trim();
        builds.push({ sha, source, env: settings.env, args });
        if (f.fail) {
            const log = path.join(source, "artifacts", "log", "Release", "bootstrap.binlog");
            await fs.mkdir(path.dirname(log), { recursive: true });
            await fs.writeFile(log, "failed bootstrap");
            return { ...ok(""), exitCode: 1, stderr: f.manifest.assertions.failure.stderr };
        }
        if (f.timeout) return { ...ok(""), status: "timeout", complete: false, exitCode: null };
        if (f.cancel) { f.cancel.abort(); return { ...ok(""), status: "cancelled", exitCode: null }; }
        for (const name of ["fsi", "fsc"]) {
            const p = await properties(source, `${name}.fsproj`);
            const dir = path.dirname(p.TargetPath);
            await fs.mkdir(dir, { recursive: true });
            await fs.writeFile(p.TargetPath, f.stale === name ? `sdk-${name}` : `${name}-${sha}`);
            await fs.writeFile(path.join(dir, `${name}.runtimeconfig.json`), JSON.stringify({
                runtimeOptions: { tfm: p.TargetFramework, framework: { name: "Microsoft.NETCore.App", version: `${activeSdk.split(".")[0]}.0.0` } },
            }));
            for (const item of ["FSharp.Core.dll", "FSharp.Compiler.Service.dll", "FSharp.Build.dll", "Microsoft.FSharp.NetSdk.props",
                "Microsoft.FSharp.Targets", "Microsoft.FSharp.NetSdk.targets", "Microsoft.FSharp.Overrides.NetSdk.targets"]) {
                await fs.writeFile(path.join(dir, item), item === "FSharp.Core.dll" ? `core-${sha}` : item);
            }
        }
        const p = await properties(source, "FSharp.Core.fsproj");
        await fs.mkdir(p.PackageOutputPath, { recursive: true });
        if (!f.missingPackage) await fs.writeFile(path.join(p.PackageOutputPath, "FSharp.Core.1.0.0.nupkg"), `package-${sha}`);
        await f.afterBuild?.(source);
        return ok("");
    } };
    return Object.assign(f, { options, deps, shas, builds, command });
}

test("fresh revisions use their exact SHA, SDK, TFM and independent same-version Core bytes", async t => {
    const f = await controlled(t);
    await f.file("mirror/artifacts/Bootstrap/fsi/fsi.dll", "poison");
    await f.file("mirror/artifacts/bin/fsi/Proto/fsi.dll", "poison");
    await f.file("mirror/bin/fsi.dll", "poison");
    await f.file("mirror/obj/project.assets.json", "poison");
    const hookMarker = path.join(f.root, "hook-ran");
    await f.file("mirror/.git/hooks/post-checkout", `#!/bin/sh\ntouch "${hookMarker.replace(/\\/g, "/")}"\n`);
    await f.file("nuget/packages/fsharp.core/1.0.0/FSharp.Core.dll", "host");
    const results = [];
    for (const [i, sha] of f.shas.entries()) {
        const result = await testSourceRevision({ ...f.options, request: { ...f.options.request, sourceSha: sha } }, f.deps);
        assert.equal(result.outcome.outcome, "pass", JSON.stringify(result));
        assert.equal(result.actualSourceSha, sha);
        assert.equal(result.toolchain.sdkVersion, i ? "9.0.100" : "10.0.100");
        assert.equal(result.product.targetFramework, i ? "net9.0" : "net10.0");
        assert.equal(result.adapter.layout, i ? "arcade-sln-products" : "arcade-slnx-products");
        assert.equal(result.globalJson.before, result.globalJson.after);
        assert.equal(result.reproduction.cells[0].attempts.length, 2);
        assert.equal(result.product.files.filter(x => x.path.endsWith("FSharp.Compiler.Service.dll")).length, 2);
        results.push(result);
    }
    assert.notEqual(results[0].product.files.find(x => x.role === "corePackage").sha256,
        results[1].product.files.find(x => x.role === "corePackage").sha256);
    assert.notEqual(f.builds[0].env.NUGET_PACKAGES, f.builds[1].env.NUGET_PACKAGES);
    assert.equal(await fs.readFile(path.join(f.root, "nuget/packages/fsharp.core/1.0.0/FSharp.Core.dll"), "utf8"), "host");
    for (const b of f.builds) await assert.rejects(fs.access(path.join(b.source, "bin", "fsi.dll")));
    await assert.rejects(fs.access(hookMarker));
});

for (const [name, change, reason] of [
    ["failed source build", f => { f.fail = true; }, "build-failure"],
    ["timed out source build", f => { f.timeout = true; }, "timeout"],
    ["stale fsi", f => { f.stale = "fsi"; }, "wrong-identity"],
    ["stale fsc", f => { f.stale = "fsc"; }, "wrong-identity"],
    ["missing package", f => { f.missingPackage = true; }, "wrong-identity"],
    ["missing FSI", f => { f.afterBuild = source => fs.rm(path.join(source, "artifacts/bin/fsi/Release/net10.0/fsi.dll")); }, "wrong-identity"],
    ["changed global.json", f => { f.afterBuild = source => fs.appendFile(path.join(source, "global.json"), " "); }, "wrong-identity"],
    ["unavailable SDK", f => { f.deps.toolchains.http = async () => { throw new Error("SDK unavailable"); }; }, "unavailable-sdk"],
]) test(`${name} is untestable, never the reported product failure`, async t => {
    const f = await controlled(t);
    change(f);
    const result = await testSourceRevision(f.options, f.deps);
    assert.equal(result.outcome.outcome, "untestable");
    assert.equal(result.outcome.reason, reason, JSON.stringify(result));
    assert.ok(result.error);
    if (f.fail) assert.ok(result.evidence.some(e => e.path.endsWith(".binlog")));
    assert.ok(result.evidence.every(e => !path.isAbsolute(e.path) && !e.path.split("/").includes("..") && /^[a-f0-9]{64}$/.test(e.sha256)));
});

test("confirmation and all phases always allocate independent build instances", async t => {
    const f = await controlled(t);
    const ids = new Set();
    for (const phase of ["main", "older-search", "bisect", "confirmation", "confirmation"]) {
        const result = await testSourceRevision({ ...f.options, request: { ...f.options.request, phase } }, f.deps);
        assert.equal(result.outcome.outcome, "pass", JSON.stringify(result));
        ids.add(result.buildInstanceId);
    }
    assert.equal(ids.size, 5);
});

test("cancellation removes only its scratch tree and retains failure evidence", async t => {
    const f = await controlled(t);
    f.cancel = new AbortController();
    const sibling = await prepareRevisionWorkspace(f.options, f.deps);
    assert.equal(sibling.status, "prepared");
    const result = await testSourceRevision({ ...f.options, signal: f.cancel.signal }, f.deps);
    assert.equal(result.outcome.outcome, "untestable");
    assert.equal(result.error.code, "cancelled");
    await fs.access(sibling.root);
    assert.deepEqual(await fs.readdir(f.options.scratchDirectory), [path.basename(sibling.root)]);
    await fs.access(f.options.mirrorDirectory);
});

test("concurrent workspace preparation cannot collide and prepared poison is rejected", async t => {
    const f = await controlled(t);
    const workspaces = await Promise.all([prepareRevisionWorkspace(f.options, f.deps), prepareRevisionWorkspace(f.options, f.deps)]);
    assert.ok(workspaces.every(w => w.status === "prepared"));
    assert.notEqual(workspaces[0].root, workspaces[1].root);
    for (const name of ["artifacts/Bootstrap/fsc/fsc.dll", "artifacts/bin/fsi/Proto/fsi.dll", "bin/fsi.dll", "obj/project.assets.json"]) {
        const location = path.join(workspaces[0].sourceDirectory, name);
        await fs.mkdir(path.dirname(location), { recursive: true });
        await fs.writeFile(location, "poison");
    }
    const result = await buildRevision(workspaces[0], f.deps);
    assert.equal(result.outcome.outcome, "untestable");
    assert.match(result.error.message, /unexpected state/);
    assert.equal(f.builds.length, 0);
    const built = await buildRevision(workspaces[1], f.deps);
    assert.equal(built.status, "built", JSON.stringify(built));
    assert.equal((await buildRevision(workspaces[1], f.deps)).outcome.outcome, "untestable");
});

test("process cancellation is bounded and does not run an already cancelled command", async t => {
    const f = await controlled(t);
    const settings = { cwd: f.root, env: isolatedEnvironment(f.root, platform), timeoutMs: 30000 };
    const cancelled = AbortSignal.abort();
    assert.equal((await execute(process.execPath, ["-e", "throw Error('must not run')"],
        { ...settings, signal: cancelled })).status, "cancelled");
    const controller = new AbortController();
    const running = execute(process.execPath, ["-e", "setInterval(()=>{}, 1000)"],
        { ...settings, signal: controller.signal });
    setTimeout(() => controller.abort(), 100);
    const result = await running;
    assert.equal(result.status, "cancelled");
    assert.equal(result.complete, false);
});

test("unsupported layout and invalid digest fail closed before a build", async t => {
    const f = await controlled(t);
    const invalid = await testSourceRevision({ ...f.options, request: { ...f.options.request, manifestDigest: "a".repeat(64) } }, f.deps);
    assert.equal(invalid.outcome.outcome, "untestable");
    await fs.writeFile(path.join(f.options.mirrorDirectory, "eng", "Build.ps1"), "unsupported");
    await f.command(["add", "."]);
    await f.command(["-c", "user.name=Fixture", "-c", "user.email=fixture@example.invalid", "commit", "-qm", "unsupported layout"]);
    const sourceSha = await f.command(["rev-parse", "HEAD"]);
    const result = await testSourceRevision({ ...f.options, request: { ...f.options.request, sourceSha } }, f.deps);
    assert.equal(result.outcome.outcome, "untestable");
    assert.equal(result.error.stage, "layout");
    assert.match(result.error.message, /unsupported historical build entrypoint/);
    assert.equal(f.builds.length, 0);
});

test("evaluated output wins over a newer decoy and host credentials never reach scripts", async t => {
    const f = await controlled(t);
    const old = process.env.GITHUB_TOKEN;
    process.env.GITHUB_TOKEN = "must-not-leak";
    t.after(() => { if (old === undefined) delete process.env.GITHUB_TOKEN; else process.env.GITHUB_TOKEN = old; });
    f.afterBuild = async source => {
        const decoy = path.join(source, "artifacts", "bin", "fsi", "Release", "net99.0", "fsi.dll");
        await fs.mkdir(path.dirname(decoy), { recursive: true });
        await fs.writeFile(decoy, "wrong");
        await fs.utimes(decoy, new Date("2099-01-01"), new Date("2099-01-01"));
    };
    const result = await testSourceRevision(f.options, f.deps);
    assert.equal(result.outcome.outcome, "pass", JSON.stringify(result));
    assert.ok(!JSON.stringify(result).includes("must-not-leak"));
    assert.ok(result.product.files.find(x => x.role === "fsi").path.includes("net10.0"));
});
