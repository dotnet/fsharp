const { test } = require("node:test");
const assert = require("node:assert/strict");
const { sourceUnit, main } = require("./source-worker.smoke.cjs");
const { manifest: fixtureManifest, seal } = require("./fixtures/contracts.cjs");
const { validateExecutionUnit } = require("./worker.cjs");
const { fingerprintInput } = require("./model.cjs");
const policy = require("../../.github/policies/regression-bisect.json");
const fs = require("node:fs/promises");
const path = require("node:path");
const os = require("node:os");
const { createFixture } = require("./fixtures/smoke.cjs");
const { hash } = require("./toolchain-io.cjs");

test("task identity negative uses a verified compatible off-inventory copy rather than an incompatible SDK task", async t => {
    const root = await fs.mkdtemp(path.join(os.tmpdir(), "rb-task-negative-"));
    t.after(() => fs.rm(root, { recursive: true, force: true }));
    const files = [];
    for (const name of ["FSharp.Build.dll", "FSharp.Core.dll"]) {
        const file = path.join(root, name), bytes = Buffer.from(`synthetic same-byte ${name}; no product execution`);
        await fs.writeFile(file, bytes);
        files.push({ path: file, sha256: hash(bytes) });
    }
    const directory = path.join(root, "sample");
    const manifest = await createFixture(directory, { kind: "project", tfm: "net10.0", scenario: "fallback-tasks",
        taskAssembly: files[0], taskCore: files[1] });
    for (const record of files) {
        const relative = `wrong/${path.basename(record.path)}`;
        assert.equal(manifest.sources.find(f => f.path === relative)?.sha256, record.sha256);
        assert.equal(hash(await fs.readFile(path.join(directory, relative))), record.sha256);
    }
    assert.match(await fs.readFile(path.join(directory, "Repro.fsproj"), "utf8"),
        /<FSharpBuildAssemblyFile>\$\(MSBuildProjectDirectory\)\/wrong\/FSharp.Build.dll/);
});

test("matrix acquisition separates released runtime from source build SDK without retargeting the worker", async t => {
    const { prepareMatrix } = require("./repro.smoke.cjs");
    assert.equal(typeof prepareMatrix, "function");
    const root = await fs.mkdtemp(path.join(os.tmpdir(), "rb-matrix-tools-"));
    t.after(() => fs.rm(root, { recursive: true, force: true }));
    const build = { sdkVersion: "10.0.301", compilerRuntime: { version: "10.0.9" } };
    const buildFile = path.join(root, "acquisition.json");
    await fs.writeFile(buildFile, JSON.stringify({ status: "acquired", identity: build }));
    const unit = { resolved: { sdkVersion: "10.0.402", runtimeRequirements: [{ version: "10.0.9" }] }, buildInstanceId: "source" };
    const primary = { archive: { sha256: "a".repeat(64) } };
    const released = { ...primary, sdkVersion: "10.0.402", compilerRuntime: { version: "10.0.13" } };
    const product = { kind: "source" };
    const config = await prepareMatrix({ unit, product, primary, buildFile, storageDirectory: root, helper: {},
        inventoryReleased: async identity => ({ kind: "released", root: identity.sdkVersion }) }, {
        acquireToolchain: async ({ toolchain }) => {
            assert.deepEqual(toolchain.runtimeRequirements, []);
            assert.equal(toolchain.sdkVersion, "10.0.402");
            return { status: "acquired", identity: released };
        },
    });
    assert.equal(config.released.toolchain, released);
    assert.deepEqual(config.source.toolchain, build);
    assert.equal(config.source.product, product);
    assert.deepEqual(unit.resolved.runtimeRequirements, [{ version: "10.0.9" }]);
});

test("producer preparation uses the same historical execution lane as consumers", async t => {
    const root = await fs.mkdtemp(path.join(os.tmpdir(), "rb-producer-lane-"));
    t.after(() => fs.rm(root, { recursive: true, force: true }));
    const source = await fs.readFile(path.join(__dirname, "repro.smoke.cjs"), "utf8");
    const dependencies = { execute: () => assert.fail("no product execution in this focused fixture") };
    const context = { module: {}, process, console, require: name => name === "./repro.cjs" ? {
        runReproduction: async (_, actual) => {
            assert.equal(actual, dependencies);
            throw new Error("historical lane selected");
        },
    } : require(name) };
    require("node:vm").runInNewContext(source, context);
    await assert.rejects(context.module.exports.runMatrix({
        released: { product: { kind: "released" }, tfm: "net10.0" }, helper: {},
    }, path.join(root, "matrix"), "released", dependencies), /historical lane selected/);
});

test("the reusable matrix includes Linux multi-TFM and consumes the frozen producer package", async t => {
    assert.equal(typeof require("./repro.smoke.cjs").runMatrix, "function");
    const root = await fs.mkdtemp(path.join(os.tmpdir(), "rb-matrix-"));
    t.after(() => fs.rm(root, { recursive: true, force: true }));
    const manifest = await createFixture(root, { kind: "project", tfm: "net10.0", scenario: "multi-tfm" });
    assert.equal(new Set(manifest.matrix.map(c => c.tfm)).size, 2);
    const source = await fs.readFile(path.join(__dirname, "fixtures", "smoke.cjs"), "utf8");
    assert.match(source, /PackageReference Include="FrozenProducer"/);
    assert.doesNotMatch(source, /<Reference Include="FrozenProducer"/);
});

test("source smoke binds one fresh physical unit without changing the investigation allowance", () => {
    const manifest = fixtureManifest();
    for (const cell of manifest.matrix) { cell.os = "linux"; cell.architecture = "x64"; }
    seal(manifest);
    const revision = "a".repeat(40), sourceSha = "b".repeat(40), now = new Date("2026-10-20T00:00:00Z");
    const resolved = { sdkVersion: "10.0.402", bundledRuntime: { version: "10.0.13" }, runtimeRequirements: [],
        archive: { sha512: "c".repeat(128) },
        metadata: [{ url: "https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json", sha256: "d".repeat(64) }] };
    const archive = { status: 200, sha256: "e".repeat(64), sha512: resolved.archive.sha512 };
    const compilerRuntime = { name: "Microsoft.NETCore.App", version: "10.0.9" };
    const frozen = fingerprintInput(resolved);
    const unit = sourceUnit({ revision, sourceSha, manifest, resolved, archive, compilerRuntime, now });
    assert.equal(validateExecutionUnit(unit), true);
    assert.equal(unit.policyDigest, fingerprintInput(policy));
    assert.equal(unit.workflowRevision, revision);
    assert.equal(unit.request.target.sourceSha, sourceSha);
    assert.equal(unit.stableSdk.archives[0].sha256, archive.sha256);
    assert.deepEqual(unit.resolved.runtimeRequirements, [{ ...compilerRuntime, role: "sample" }]);
    assert.equal(unit.resolved.bundledRuntime.version, "10.0.13");
    assert.equal(fingerprintInput(resolved), frozen);
    assert.equal(Date.parse(unit.deadline) - now.getTime(), 110 * 60000);
    assert.equal(Object.hasOwn(unit, "budget"), false);
    assert.throws(() => sourceUnit({ revision, sourceSha, manifest, resolved, compilerRuntime,
        archive: { ...archive, sha512: "0".repeat(128) }, now }), /publisher metadata/);
    for (const compilerRuntime of [undefined, { name: "Other.Runtime", version: "10.0.9" },
        { name: "Microsoft.NETCore.App", version: "latest" }]) {
        assert.throws(() => sourceUnit({ revision, sourceSha, manifest, resolved, archive, compilerRuntime, now }),
            /source compiler runtime/);
    }
});

test("physical source smoke rejects missing opt-in and relative paths before starting work", async () => {
    for (const args of [[], ["--run", "relative.json", "relative-output"], ["--verify", "x", "y"]])
        await assert.rejects(main(args), /usage:/);
});
