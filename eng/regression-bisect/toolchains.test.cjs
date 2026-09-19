const test = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs/promises");
const os = require("node:os");
const path = require("node:path");
const { createHash } = require("node:crypto");
const { resolveLatestStableSdk, readRevisionToolchain, acquireToolchain, verifyToolchainIdentity } = require("./toolchains.cjs");
const { policy, admission, observation, seal, digest } = require("./fixtures/contracts.cjs");
const { admitBisection, fingerprintInput } = require("./model.cjs");
const { capture, publicUrl, isolatedEnvironment } = require("./toolchain-io.cjs");

const indexUrl = "https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json";
const feed = "https://builds.dotnet.microsoft.com/dotnet";
const platform = { os: "windows", rid: "win-x64", architecture: "x64" };
const hash512 = bytes => createHash("sha512").update(bytes).digest("hex");
const archiveBytes = Buffer.from("verified SDK archive");
const archive = (version, rid = "win-x64") => ({
    name: `dotnet-sdk-${rid}${rid.startsWith("win") ? ".zip" : ".tar.gz"}`, rid,
    url: `${feed}/Sdk/${version}/dotnet-sdk-${version}-${rid}${rid.startsWith("win") ? ".zip" : ".tar.gz"}`,
    hash: hash512(archiveBytes),
});
const sdk = (version, rids = ["win-x64", "win-arm64", "linux-x64", "linux-musl-x64", "osx-arm64"]) => ({
    version, "runtime-version": `${version.split(".")[0]}.0.0`, files: rids.map(rid => archive(version, rid)),
});
const release = (version, date = "2026-09-08", others = []) => ({
    "release-date": date, sdk: sdk(version), sdks: others.map(v => sdk(v)),
    runtime: { version: `${version.split(".")[0]}.0.0`, files: [] },
});

function metadata(channels = [
    ["11.0", "sts", [release("11.0.100-rc.1.26425.128")]],
    ["10.0", "lts", [release("10.0.401", "2026-09-08", ["10.0.108", "10.0.401"]) ]],
    ["9.0", "sts", [release("9.0.999")]],
]) {
    const docs = new Map();
    docs.set(indexUrl, {
        "releases-index": channels.map(([channel, type, releases]) => {
            const url = `${feed}/release-metadata/${channel}/releases.json`;
            docs.set(url, { "channel-version": channel, releases });
            return { "channel-version": channel, "release-type": type, "releases.json": url };
        }),
    });
    return {
        docs,
        clock: () => new Date("2026-09-19T00:00:00Z"),
        async http(url) {
            assert.ok(docs.has(url), `unexpected HTTP ${url}`);
            return { status: 200, url, redirects: [], bytes: Buffer.from(JSON.stringify(docs.get(url))) };
        },
    };
}
const options = (p = platform) => ({ policy: policy(), platform: p });

for (const [name, channels, expected] of [
    ["preview first and multiple SDKs", undefined, "10.0.401"],
    ["stable STS newer than LTS", [["10.0", "lts", [release("10.0.401")]], ["11.0", "sts", [release("11.0.100")]]], "11.0.100"],
    ["numeric not lexical", [["9.0", "sts", [release("9.0.999")]], ["10.0", "lts", [release("10.0.99"), release("10.0.100")]]], "10.0.100"],
    ["sdks exceeds singular SDK", [["10.0", "lts", [release("10.0.100", "2026-09-08", ["10.0.401"])]]], "10.0.401"],
    ["future release excluded", [["10.0", "lts", [release("10.0.402", "2026-09-20"), release("10.0.401")]]], "10.0.401"],
    ["reordered metadata", [["9.0", "sts", [release("9.0.999")]], ["10.0", "lts", [release("10.0.401")]], ["11.0", "sts", [release("11.0.100-rc.1")]]], "10.0.401"],
]) {
    test(`stable resolution: ${name}`, async () => {
        const deps = metadata(channels);
        const result = await resolveLatestStableSdk(options(), deps);
        assert.equal(result.status, "resolved", JSON.stringify(result));
        assert.equal(result.toolchain.sdkVersion, expected);
        assert.equal(result.toolchain.archive.sha512, hash512(archiveBytes));
        for (const record of result.toolchain.metadata) {
            assert.equal(digest(Buffer.from(record.bytesBase64, "base64")), record.sha256);
        }
        assert.equal(result.toolchain.frozenAt, "2026-09-19T00:00:00.000Z");
    });
}

for (const [p, suffix] of [
    [platform, "win-x64.zip"],
    [{ os: "windows", rid: "win-arm64", architecture: "arm64" }, "win-arm64.zip"],
    [{ os: "linux", rid: "linux-x64", architecture: "x64" }, "linux-x64.tar.gz"],
    [{ os: "linux", rid: "linux-musl-x64", architecture: "x64" }, "linux-musl-x64.tar.gz"],
    [{ os: "macos", rid: "osx-arm64", architecture: "arm64" }, "osx-arm64.tar.gz"],
]) {
    test(`exact archive: ${p.rid}`, async () => {
        const deps = metadata();
        const result = await resolveLatestStableSdk(options(p), deps);
        assert.equal(result.status, "resolved", JSON.stringify(result));
        assert.ok(result.toolchain.archive.url.endsWith(suffix));
    });
}

for (const [name, mutate] of [
    ["missing archive", d => d.docs.get(`${feed}/release-metadata/10.0/releases.json`).releases[0].sdks = []],
    ["malformed checksum", d => {
        const r = d.docs.get(`${feed}/release-metadata/10.0/releases.json`).releases[0];
        r.sdk.files[0].hash = "not a hash"; r.sdks = [];
    }],
    ["unapproved archive host", d => {
        const r = d.docs.get(`${feed}/release-metadata/10.0/releases.json`).releases[0];
        r.sdk.files[0].url = "https://unapproved.example/sdk.zip"; r.sdks = [];
    }],
    ["outage", d => d.http = async () => { throw new Error("HTTP 503"); }],
    ["redirect violation", d => d.http = async url => ({
        status: 200, url: "https://unapproved.example/index", redirects: [], bytes: Buffer.from(JSON.stringify(d.docs.get(url))),
    })],
]) {
    test(`resolution fails closed: ${name}`, async () => {
        const deps = metadata();
        mutate(deps);
        if (name === "missing archive") deps.docs.get(`${feed}/release-metadata/10.0/releases.json`).releases[0].sdk.files = [
            { ...archive("10.0.401"), name: "dotnet-sdk-win-x64.exe", url: `${feed}/Sdk/10.0.401/installer.exe` },
        ];
        const result = await resolveLatestStableSdk(options(), deps);
        assert.equal(result.status, "untestable");
        assert.equal(result.outcome.outcome, "untestable");
        assert.ok(result.error.message);
    });
}

test("architecture and RID disagreements are never substituted", async () => {
    for (const p of [{ ...platform, architecture: "x86" }, { ...platform, rid: "win-arm64" }, { ...platform, os: "linux" }]) {
        assert.equal((await resolveLatestStableSdk(options(p), metadata())).status, "untestable");
    }
});

test("duplicate SDK entries compare product identity, not Visual Studio descriptions or file ordering", async () => {
    const deps = metadata([["10.0", "lts", [release("10.0.401", "2026-09-08", ["10.0.401"])]]]);
    const r = deps.docs.get(`${feed}/release-metadata/10.0/releases.json`).releases[0];
    r.sdk["vs-version"] = "10.0.0";
    r.sdks[0]["vs-version"] = "18.0.0";
    r.sdks[0].files.reverse();
    assert.equal((await resolveLatestStableSdk(options(), deps)).status, "resolved");
    r.sdks[0].files.find(f => f.rid === "win-x64").hash = "0".repeat(128);
    assert.equal((await resolveLatestStableSdk(options(), deps)).status, "untestable");
});

test("obsolete repeated SDK entries cannot make a newer stable release undiscoverable", async () => {
    const older = release("1.1.14", "2018-04-10", ["1.1.14"]);
    older.sdks[0]["runtime-version"] = "1.0.16";
    const result = await resolveLatestStableSdk(options(),
        metadata([["10.0", "lts", [release("10.0.401")]], ["1.0", "lts", [older]]]));
    assert.equal(result.status, "resolved", JSON.stringify(result));
    assert.equal(result.toolchain.sdkVersion, "10.0.401");
});

async function directory(t) {
    const root = await fs.mkdtemp(path.join(os.tmpdir(), "fsharp-toolchain-test-"));
    t.after(() => fs.rm(root, { recursive: true, force: true }));
    return fs.realpath(root);
}
async function source(t, version = "10.0.401", tools = version) {
    const root = await directory(t);
    const bytes = Buffer.from(JSON.stringify({
        sdk: { version, rollForward: "latestMinor", allowPrerelease: true, paths: [".dotnet", "$host$"] },
        tools: { dotnet: tools },
    }, null, 2));
    await fs.writeFile(path.join(root, "global.json"), bytes);
    const result = await readRevisionToolchain({
        ...options(), sourceDirectory: root, sourceSha: "a".repeat(40), runtimeConfigs: [],
    });
    return { root, bytes, result };
}

test("historical declarations need no modern TFM file and remain byte-for-byte intact", async t => {
    const { root, bytes, result } = await source(t, "8.0.100-preview.1.23115.2");
    assert.equal(result.status, "resolved", JSON.stringify(result));
    assert.equal(result.toolchain.sdkVersion, "8.0.100-preview.1.23115.2");
    assert.equal(result.toolchain.toolsVersion, result.toolchain.sdkVersion);
    assert.equal(result.toolchain.declarations.targetFrameworks, null);
    assert.deepEqual(await fs.readFile(path.join(root, "global.json")), bytes);
});

test("different sdk and tools declarations are preserved, not silently reconciled", async t => {
    const { result } = await source(t, "9.0.100", "8.0.100");
    assert.equal(result.status, "untestable");
    assert.match(result.error.message, /9\.0\.100.*8\.0\.100/);
});

test("runtimeconfig requirements retain exact identities and roles", async t => {
    const { root } = await source(t);
    const config = path.join(root, "fsc.runtimeconfig.json");
    await fs.writeFile(config, JSON.stringify({ runtimeOptions: { framework: { name: "Microsoft.NETCore.App", version: "9.0.8" } } }));
    const result = await readRevisionToolchain({
        ...options(), sourceDirectory: root, sourceSha: "b".repeat(40), runtimeConfigs: [{ path: config, role: "compiler" }],
    });
    assert.equal(result.status, "resolved", JSON.stringify(result));
    assert.deepEqual(result.toolchain.runtimeRequirements.map(({ name, version, role }) => ({ name, version, role })),
        [{ name: "Microsoft.NETCore.App", version: "9.0.8", role: "compiler" }]);
});

for (const [declarations, expected] of [
    [{ dotnet: ["9.0.8"], "aspnetcore/x64": ["9.0.8"] }, "resolved"],
    [{ "dotnet/arm64": ["9.0.8"] }, "untestable"],
    [{ dotnet: ["$(RuntimeVersion)"] }, "untestable"],
]) {
    test(`historical tools.runtimes: ${JSON.stringify(declarations)}`, async t => {
        const { root } = await source(t);
        const file = path.join(root, "global.json");
        const content = JSON.parse(await fs.readFile(file));
        content.tools.runtimes = declarations;
        await fs.writeFile(file, JSON.stringify(content));
        const result = await readRevisionToolchain({ ...options(), sourceDirectory: root, sourceSha: "a".repeat(40) });
        assert.equal(result.status, expected, JSON.stringify(result));
        if (expected === "resolved") {
            assert.deepEqual(result.toolchain.runtimeRequirements.map(r => [r.name, r.version, r.role]),
                [["Microsoft.NETCore.App", "9.0.8", "build"], ["Microsoft.AspNetCore.App", "9.0.8", "build"]]);
        }
    });
}

function captured(toolchain, root) {
    return {
        sdkVersion: toolchain.sdkVersion, architecture: platform.architecture, rid: platform.rid,
        hostPath: path.join(root, "dotnet.exe"), root, sdkBasePath: path.join(root, "sdk", toolchain.sdkVersion),
        sdks: [{ version: toolchain.sdkVersion, path: path.join(root, "sdk") }],
        runtimes: [{ name: "Microsoft.NETCore.App", version: "10.0.0", path: path.join(root, "shared", "Microsoft.NETCore.App"), sha256: digest("runtime") }],
        compilerRuntime: { name: "Microsoft.NETCore.App", version: "10.0.0" },
        runtimeExecutions: [],
        files: Object.fromEntries(Object.entries({ compiler: "fsc", fsi: "fsi", core: "FSharp.Core" }).map(([name, assemblyName]) => [name, {
            path: path.join(root, "sdk", toolchain.sdkVersion, "FSharp", `${assemblyName}.dll`),
            sha256: digest(name), assemblyName, assemblyVersion: "10.0.0.0", fileVersion: "10.0.0.0",
        }])),
        archive: { ...toolchain.archive, sha256: digest(archiveBytes), actualSha512: hash512(archiveBytes), redirectUrl: toolchain.archive.url },
    };
}

for (const [name, edit] of [
    ["wrong architecture", i => i.architecture = "arm64"],
    ["wrong resolved SDK", i => i.sdkVersion = "11.0.100-rc.1"],
    ["host SDK leakage", i => i.sdks.push({ version: "11.0.100", path: path.resolve("host-sdk") })],
    ["wrong host", i => i.hostPath = path.resolve("machine", "dotnet.exe")],
    ["self-asserted host root", i => { i.root = path.resolve("machine"); i.hostPath = path.join(i.root, "dotnet.exe"); }],
    ["same-version SDK outside root", i => i.sdkBasePath = path.resolve("machine", "sdk", "10.0.401")],
    ["archive mismatch", i => i.archive.actualSha512 = "0".repeat(128)],
    ["wrong archive URL", i => i.archive.url = `${feed}/Sdk/wrong.zip`],
    ["unapproved redirect destination", i => i.archive.redirectUrl = "https://unapproved.example/sdk.zip"],
    ["missing runtime", i => i.runtimes = []],
    ["wrong compiler runtime", i => i.compilerRuntime.version = "9.0.0"],
    ["outside Core path", i => i.files.core.path = path.resolve("host-core.dll")],
    ["wrong F# assembly", i => i.files.core.assemblyName = "Other.Assembly"],
]) {
    test(`identity fails closed: ${name}`, async () => {
        const { toolchain } = await resolveLatestStableSdk(options(), metadata());
        const identity = captured(toolchain, path.resolve("isolated"));
        toolchain.installationRoot = identity.root;
        edit(identity);
        const result = verifyToolchainIdentity({ toolchain, identity, policy: policy() });
        assert.equal(result.status, "untestable", JSON.stringify(result));
        assert.equal(result.outcome.reason, "wrong-identity");
    });
}

test("exact acquisition sanitizes state and never calls a PATH-selected SDK", async t => {
    const root = await directory(t);
    await fs.writeFile(path.join(root, "global.json"), JSON.stringify({ sdk: { version: "11.0.100-rc.1.26425.128" } }));
    for (const key of ["runtime_source_feed_key", "GITHUB_TOKEN", "NUGET_AUTH_TOKEN", "MSBuildSDKsPath"]) {
        const previous = process.env[key];
        process.env[key] = "must-not-inherit";
        t.after(() => { if (previous === undefined) delete process.env[key]; else process.env[key] = previous; });
    }
    const { toolchain } = await resolveLatestStableSdk(options(), metadata());
    const calls = [];
    const result = await acquireToolchain({
        policy: policy(), toolchain, storageDirectory: root, helperRoot: path.resolve("."),
    }, {
        ...metadata(),
        installer: async request => {
            assert.equal(request.version, "10.0.401");
            for (const key of ["runtime_source_feed_key", "GITHUB_TOKEN", "NUGET_AUTH_TOKEN", "MSBuildSDKsPath"]) {
                assert.equal(request.env[key], undefined);
            }
            assert.ok(request.env.DOTNET_ROOT.startsWith(root));
            assert.ok(request.env.NUGET_PACKAGES.startsWith(root));
            assert.ok(request.env.DOTNET_CLI_HOME.startsWith(root));
            assert.equal(request.env.DOTNET_MULTILEVEL_LOOKUP, "0");
            calls.push(request);
            return { archive: captured(toolchain, root).archive, installer: { sha256: digest("script") }, attempts: [] };
        },
        capture: async request => {
            assert.equal(request.executable, path.join(request.root, "dotnet.exe"));
            const global = JSON.parse(await fs.readFile(path.join(request.cwd, "global.json")));
            assert.deepEqual(global.sdk, { version: "10.0.401", rollForward: "disable", allowPrerelease: false });
            return captured(toolchain, request.root);
        },
    });
    assert.equal(result.status, "acquired", JSON.stringify(result));
    assert.equal(calls.length, 1);
    assert.equal(result.identity.sdkVersion, "10.0.401");
});

test("two acquisitions with the same versions never share CLI or NuGet state", async t => {
    const root = await directory(t);
    const { toolchain } = await resolveLatestStableSdk(options(), metadata());
    const environments = [];
    const deps = {
        installer: async request => {
            environments.push(request.env);
            return { archive: captured(toolchain, root).archive, installer: {}, attempts: [] };
        },
        capture: async request => captured(toolchain, request.root),
    };
    for (let i = 0; i < 2; i++) {
        assert.equal((await acquireToolchain({ policy: policy(), toolchain, storageDirectory: root }, deps)).status, "acquired");
    }
    for (const key of ["DOTNET_ROOT", "NUGET_PACKAGES", "DOTNET_CLI_HOME", "HOME"]) {
        assert.notEqual(environments[0][key], environments[1][key]);
    }
});

test("unavailable historical SDK records public fallback attempts without running a host SDK", async t => {
    const { result: { toolchain } } = await source(t, "8.0.100-preview.1.12345.6");
    const urls = [];
    const result = await acquireToolchain({ policy: policy(), toolchain, storageDirectory: await directory(t) }, {
        ...metadata(),
        http: async url => {
            urls.push(url);
            if (url.endsWith(".sha512")) return { status: 404, url, redirects: [], bytes: Buffer.from("unavailable") };
            return metadata().http(url);
        },
        installer: async () => { assert.fail("unverified archive must never reach installer"); },
        capture: async () => { assert.fail("no host SDK fallback"); },
    });
    assert.equal(result.status, "untestable");
    assert.ok(urls.some(url => url.startsWith("https://ci.dot.net/public/")));
    assert.ok(result.attempts.some(a => a.status === 404));
});

for (const location of [
    "https://ci.dot.net/public/../../private/sdk.zip",
    "https://ci.dot.net/public/%2e%2e/private/sdk.zip",
    "https://builds.dotnet.microsoft.com/dotnet/sdk.zip?token=private",
    "https://builds.dotnet.microsoft.com.evil.test/dotnet/sdk.zip",
    "http://builds.dotnet.microsoft.com/dotnet/sdk.zip",
]) {
    test(`public feed rejects ${location}`, () => assert.throws(() => publicUrl(location, policy()), /unapproved/));
}

test("bad archive aborts acquisition before any acquired executable runs", async t => {
    const { toolchain } = await resolveLatestStableSdk(options(), metadata());
    const result = await acquireToolchain({ policy: policy(), toolchain, storageDirectory: await directory(t) }, {
        installer: async () => ({ archive: { ...toolchain.archive, actualSha512: "0".repeat(128) }, attempts: [] }),
        capture: () => assert.fail("acquired executable must not run before archive validation"),
    });
    assert.equal(result.status, "untestable");
    assert.match(result.error.message, /checksum/);
});

test("existing historical SDK search directories cannot leak a same-version machine SDK", async t => {
    const { root, result: { toolchain }, bytes } = await source(t);
    await fs.mkdir(path.join(root, ".dotnet"));
    const result = await acquireToolchain({ policy: policy(), toolchain, storageDirectory: await directory(t) }, {
        installer: () => assert.fail("source search path must be checked before installation"),
    });
    assert.equal(result.status, "untestable");
    assert.match(result.error.message, /not isolated/);
    assert.deepEqual(await fs.readFile(path.join(root, "global.json")), bytes);
});

for (const available of [true, false]) {
    test(`explicit supplemental runtime ${available ? "acquired and separated" : "unavailable is untestable"}`, async t => {
        const { root, bytes, result: initial } = await source(t);
        const sampleConfig = path.join(root, "sample.runtimeconfig.json");
        await fs.writeFile(sampleConfig, JSON.stringify({
            runtimeOptions: { framework: { name: "Microsoft.NETCore.App", version: "9.0.8" } },
        }));
        const { toolchain } = await readRevisionToolchain({
            ...options(), sourceDirectory: root, sourceSha: initial.toolchain.sourceSha, runtimeConfigs: [{ path: sampleConfig, role: "sample" }],
        });
        const deps = metadata();
        const runtimeRelease = deps.docs.get(`${feed}/release-metadata/9.0/releases.json`).releases[0];
        runtimeRelease.runtime = {
            version: "9.0.8", files: [{ ...archive("9.0.8"), name: "dotnet-runtime-win-x64.zip",
                url: `${feed}/Runtime/9.0.8/dotnet-runtime-9.0.8-win-x64.zip` }],
        };
        const products = [];
        const result = await acquireToolchain({ policy: policy(), toolchain, storageDirectory: await directory(t) }, {
            ...deps,
            installer: async request => {
                products.push(request.product);
                if (request.product === "dotnet" && !available) throw new Error("runtime archive HTTP 404");
                return {
                    archive: { ...request.archive, actualSha512: hash512(archiveBytes), sha256: digest(archiveBytes), redirectUrl: request.archive.url },
                    installer: {}, attempts: [{ url: request.archive.url, version: request.version, status: 200 }],
                };
            },
            capture: async request => {
                const identity = captured(request.toolchain, request.root);
                identity.runtimes.push({
                    name: "Microsoft.NETCore.App", version: "9.0.8",
                    path: path.join(request.root, "shared", "Microsoft.NETCore.App"), sha256: digest("9.0.8"),
                });
                return identity;
            },
        });
        assert.deepEqual(products, ["sdk", "dotnet"]);
        assert.deepEqual(await fs.readFile(path.join(root, "global.json")), bytes);
        if (available) {
            assert.equal(result.status, "acquired", JSON.stringify(result));
            assert.equal(result.identity.compilerRuntime.version, "10.0.0");
            assert.equal(result.identity.requested.runtimeRequirements[0].version, "9.0.8");
            assert.equal(result.identity.buildToolchain, undefined, "SDK-only acquisition cannot attest a not-yet-built local compiler");
        } else {
            assert.equal(result.status, "untestable");
            assert.equal(result.outcome.reason, "unavailable-runtime");
        }
    });
}

test("historical public preview fallback preserves checksum provenance and exact requested version", async t => {
    const { result: { toolchain } } = await source(t, "8.0.100-preview.1.12345.6");
    const calls = [];
    const result = await acquireToolchain({ policy: policy(), toolchain, storageDirectory: await directory(t) }, {
        ...metadata(),
        http: async url => {
            if (!url.endsWith(".sha512")) return metadata().http(url);
            return { status: url.startsWith("https://ci.dot.net/") ? 200 : 404, url, redirects: [], bytes: Buffer.from(hash512(archiveBytes)) };
        },
        installer: async request => {
            calls.push(request);
            const fsharp = path.join(request.root, "sdk", request.version, "FSharp");
            await fs.mkdir(fsharp, { recursive: true });
            await fs.writeFile(path.join(fsharp, "fsi.runtimeconfig.json"), JSON.stringify({
                runtimeOptions: { framework: { name: "Microsoft.NETCore.App", version: "8.0.0-preview.1.1" } },
            }));
            await fs.mkdir(path.join(request.root, "shared", "Microsoft.NETCore.App", "8.0.0-preview.1.1"), { recursive: true });
            return {
                archive: { ...request.archive, actualSha512: hash512(archiveBytes), sha256: digest(archiveBytes), redirectUrl: request.archive.url },
                installer: {}, attempts: [],
            };
        },
        capture: async request => {
            const identity = captured(request.toolchain, request.root);
            identity.compilerRuntime.version = identity.runtimes[0].version = "8.0.0-preview.1.1";
            return identity;
        },
    });
    assert.equal(result.status, "acquired", JSON.stringify(result));
    assert.equal(calls[0].version, toolchain.sdkVersion);
    assert.ok(calls[0].archive.url.startsWith("https://ci.dot.net/public/"));
    assert.deepEqual(result.attempts.filter(a => a.url.endsWith(".sha512")).map(a => a.status), [404, 200]);
});

async function captureFixture(request, { packaged = true, rid = request.toolchain.platform.rid } = {}) {
    const { root, executable, toolchain } = request;
    const version = toolchain.sdkVersion;
    const runtimeVersion = toolchain.bundledRuntime.version;
    const fsharp = path.join(root, "sdk", version, "FSharp");
    await fs.mkdir(fsharp, { recursive: true });
    for (const name of ["fsc.dll", "fsi.dll", "FSharp.Core.dll"]) await fs.writeFile(path.join(fsharp, name), name);
    await fs.writeFile(path.join(fsharp, "fsi.deps.json"), JSON.stringify({ libraries: { "fsi/1.0.0": { type: "project" } } }));
    await fs.writeFile(path.join(fsharp, "fsi.runtimeconfig.json"), JSON.stringify({
        runtimeOptions: { framework: { name: "Microsoft.NETCore.App", version: runtimeVersion } },
    }));
    if (packaged) {
        await fs.mkdir(path.join(fsharp, "library-packs"));
        await fs.writeFile(path.join(fsharp, "library-packs", "FSharp.Core.10.1.401.nupkg"), "package");
    }
    const runtime = path.join(root, "shared", "Microsoft.NETCore.App");
    const runtimeVersions = [...new Set([runtimeVersion, ...toolchain.runtimeRequirements.map(r => r.version)])];
    for (const version of runtimeVersions) {
        await fs.mkdir(path.join(runtime, version), { recursive: true });
        await fs.writeFile(path.join(runtime, version, "System.Private.CoreLib.dll"), `runtime-${version}`);
    }
    await fs.writeFile(executable, "host");
    const graph = { runtimes: {
        "win10-x64": { "#import": ["win10", "win7-x64"] }, "win7-x64": { "#import": ["win-x64"] },
        "ubuntu.22.04-x64": { "#import": ["ubuntu-x64"] }, "ubuntu-x64": { "#import": ["linux-x64"] },
        "osx.12-arm64": { "#import": ["osx-arm64"] },
        "alpine.3.18-x64": { "#import": ["linux-musl-x64"] }, "linux-musl-x64": { "#import": ["linux-x64"] },
    } };
    await fs.writeFile(path.join(root, "sdk", version, "RuntimeIdentifierGraph.json"), JSON.stringify(graph));
    return {
        fs,
        run: async (file, args, settings) => {
            assert.equal(file, executable);
            if (args.at(-1) === "--help") {
                assert.equal(settings.env.DOTNET_ROLL_FORWARD, "Disable");
                await fs.appendFile(settings.env.DOTNET_HOST_TRACEFILE,
                    `Probing ${args[3]}\nChose FX version [${path.join(runtime, args[2])}]\n`);
                return { exitCode: 0, stderr: "", stdout: "F# compiler help" };
            }
            const outputs = {
                "--info": `Base Path: ${path.join(root, "sdk", version)}\nRID: ${rid}`,
                "--version": version,
                "--list-sdks": `${version} [${path.join(root, "sdk")}]`,
                "--list-runtimes": runtimeVersions.map(v => `Microsoft.NETCore.App ${v} [${runtime}]`).join("\n"),
                exec: `ARCH|${toolchain.platform.architecture}\nRUNTIME|${runtimeVersion}\nFILE|fsc.dll|fsc|15.2.401.0|15.204.126.42413\n` +
                    "FILE|fsi.dll|fsi|15.2.401.0|15.204.126.42413\nFILE|FSharp.Core.dll|FSharp.Core|10.1.0.0|10.104.126.42413\n" +
                    (packaged ? "PACKAGE|FSharp.Core.10.1.401.nupkg|FSharp.Core|10.1.401" : ""),
            };
            return { exitCode: 0, stderr: "", stdout: outputs[args[0]] };
        },
    };
}

for (const packaged of [true, false]) {
    test(`captured identity with bundled package: ${packaged}`, async t => {
        const root = await directory(t);
        const env = isolatedEnvironment(root, platform);
        await fs.mkdir(env.HOME);
        const request = {
            root, env, executable: path.join(root, "dotnet.exe"), cwd: root,
            toolchain: { platform, sdkVersion: "10.0.401", bundledRuntime: { version: "10.0.12" }, runtimeRequirements: [] },
        };
        const identity = await capture(request, await captureFixture(request, { packaged }));
        assert.equal(identity.files.core.packageIdentity, packaged ? "FSharp.Core/10.1.401" : null);
        assert.equal(identity.files.core.assemblyVersion, "10.1.0.0");
        assert.equal(identity.compilerRuntime.version, "10.0.12");
        assert.deepEqual(identity.runtimeExecutions, []);
    });
}

async function acquireSource(t, version, sourceSha, p, { rid = p.rid, edit = () => {} } = {}) {
    const { root, bytes } = await source(t, version);
    const runtimeVersion = `${version.split(".")[0]}.0.0`;
    const runtimeConfigs = [];
    // Equal config bytes and repeated roles must still retain independent traces.
    for (const [name, role] of [["fsc", "compiler"], ["fsi", "fsi"], ["other-fsc", "compiler"]]) {
        const file = path.join(root, `${name}.runtimeconfig.json`);
        await fs.writeFile(file, JSON.stringify({ runtimeOptions: { framework: { name: "Microsoft.NETCore.App", version: runtimeVersion } } }));
        await fs.writeFile(path.join(root, `${name}.dll`), name);
        runtimeConfigs.push({ path: file, role });
    }
    const { toolchain } = await readRevisionToolchain({ ...options(p), sourceDirectory: root, sourceSha, runtimeConfigs });
    const result = await acquireToolchain({ policy: policy(), toolchain, storageDirectory: await directory(t) }, {
        ...metadata([[version.split(".").slice(0, 2).join("."), "lts", [release(version)]]]),
        installer: async request => {
            await fs.mkdir(path.join(request.root, "shared", "Microsoft.NETCore.App", runtimeVersion), { recursive: true });
            return {
                archive: { ...request.archive, actualSha512: hash512(archiveBytes), sha256: digest(archiveBytes), redirectUrl: request.archive.url },
                installer: {}, attempts: [],
            };
        },
        capture: async request => {
            assert.equal(request.cwd, root);
            const identity = await capture(request, await captureFixture(request, { rid }));
            edit(identity);
            return identity;
        },
    });
    assert.deepEqual(await fs.readFile(path.join(root, "global.json")), bytes);
    return result;
}

for (const [p, hostRid] of [
    [platform, "win10-x64"],
    [{ os: "linux", rid: "linux-x64", architecture: "x64" }, "ubuntu.22.04-x64"],
    [{ os: "macos", rid: "osx-arm64", architecture: "arm64" }, "osx.12-arm64"],
    [{ os: "linux", rid: "linux-musl-x64", architecture: "x64" }, "alpine.3.18-x64"],
    [platform, "win-x64"],
]) {
    test(`acquisition to admission preserves ${hostRid} source toolchains and execution evidence`, async t => {
        const a = admission();
        a.manifest.matrix[0].os = p.os;
        a.manifest.matrix[0].architecture = p.architecture;
        seal(a.manifest);
        a.budget.authorization.manifestDigest = fingerprintInput(a.manifest);
        a.budget.authorization.inputDigest = a.manifest.approvedInputDigest;
        a.stable = observation("released", "product-failure", a.manifest);
        a.budget.stableSdk = a.stable.target.sdk;
        for (const [key, version, outcome, rid] of [
            ["good", "6.0.428", "pass", hostRid], ["main", "11.0.100-rc.1.26425.128", "product-failure", p.rid],
        ]) {
            const sourceSha = a[key].target.sourceSha;
            a[key] = observation("source", outcome, a.manifest, sourceSha);
            const result = await acquireSource(t, version, sourceSha, p, { rid });
            assert.equal(result.status, "acquired", JSON.stringify(result));
            const { buildToolchain, ...evidence } = result.identity;
            assert.equal(buildToolchain.evidenceDigest, fingerprintInput(evidence));
            assert.equal(buildToolchain.sourceSha, sourceSha);
            assert.equal(buildToolchain.globalJsonDigest, evidence.requested.globalJson.sha256);
            assert.equal(buildToolchain.sdk.version, version);
            assert.equal(buildToolchain.compilerRuntime.version, `${version.split(".")[0]}.0.0`);
            assert.equal(evidence.rid, rid);
            const traces = evidence.runtimeExecutions.map(r => r.hostTrace.path);
            assert.equal(new Set(traces).size, 3, "each execution owns its trace");
            for (const execution of evidence.runtimeExecutions) {
                assert.equal(digest(await fs.readFile(execution.hostTrace.path)), execution.hostTrace.sha256);
            }
            assert.deepEqual(JSON.parse(await fs.readFile(result.evidencePath)).identity, result.identity);
            a[key].cells[0].buildToolchain = buildToolchain;
        }
        a.budget.authenticatedObservationDigests = [a.stable, a.main, a.good].map(fingerprintInput);
        const admitted = admitBisection(a);
        assert.equal(admitted.decision, "admit", JSON.stringify(admitted));
        assert.deepEqual(admitted.toolchainTransitions, [{
            cellId: a.main.cells[0].id, good: a.good.cells[0].buildToolchain, main: a.main.cells[0].buildToolchain,
        }]);
        for (const edit of [
            c => c.buildToolchain.requestedSdk = "8.0.100",
            c => c.buildToolchain.sourceSha = "0".repeat(40),
            c => c.buildToolchain.verified = false,
            c => c.buildToolchain.supported = false,
            c => c.buildToolchain.compilerRuntime.version = "7.0.0",
            c => c.runtime.version = "7.0.0",
        ]) {
            const invalid = structuredClone(a);
            edit(invalid.good.cells[0]);
            invalid.budget.authenticatedObservationDigests = [invalid.stable, invalid.main, invalid.good].map(fingerprintInput);
            assert.equal(admitBisection(invalid).decision, "blocked");
        }
    });
}

for (const [name, edit, message, rid = platform.rid, p = platform] of [
    ["missing compiler execution", i => i.runtimeExecutions = i.runtimeExecutions.filter(r => r.role !== "compiler"), /unverified compiler/],
    ["missing FSI execution", i => i.runtimeExecutions = i.runtimeExecutions.filter(r => r.role !== "fsi"), /unverified fsi/],
    ["missing same-role same-digest config execution", i => i.runtimeExecutions.pop(), /unverified compiler/],
    ["wrong config digest", i => i.runtimeExecutions.forEach(r => r.configDigest = digest("other")), /unverified compiler/],
    ["wrong local runtime", i => i.runtimeExecutions.forEach(r => r.version = "7.0.0"), /unverified compiler/],
    ["wrong local architecture", i => i.runtimeExecutions.forEach(r => r.architecture = "arm64"), /unverified compiler/],
    ["wrong host OS", i => i.rid = "linux-x64", /RID/],
    ["wrong host architecture", i => i.rid = "win-arm64", /RID/],
    ["unproven historical RID", i => delete i.runtimeGraph, /RID/, "win10-x64"],
    ["changed RID graph", i => { if (i.runtimeGraph) i.runtimeGraph.sha256 = digest("other"); }, /RID/, "win10-x64"],
    ["foreign RID graph", i => { if (i.runtimeGraph) i.runtimeGraph.path = path.resolve("machine", "RuntimeIdentifierGraph.json"); }, /RID/, "win10-x64"],
    ["unknown historical RID", () => {}, /RID/, "unknown-x64"],
    ["musl host for glibc archive", () => {}, /RID/, "alpine.3.18-x64", { os: "linux", rid: "linux-x64", architecture: "x64" }],
    ["glibc host for musl archive", () => {}, /RID/, "ubuntu.22.04-x64", { os: "linux", rid: "linux-musl-x64", architecture: "x64" }],
]) {
    test(`acquisition emits no admission record for ${name}`, async t => {
        const result = await acquireSource(t, "6.0.428", "a".repeat(40), p, { edit, rid });
        assert.equal(result.status, "untestable", JSON.stringify(result));
        assert.equal(result.outcome.reason, "wrong-identity");
        assert.match(result.error.message, message);
        assert.equal(result.identity, undefined);
    });
}
