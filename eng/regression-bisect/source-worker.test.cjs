const { test } = require("node:test");
const assert = require("node:assert/strict");
const { sourceUnit, main } = require("./source-worker.smoke.cjs");
const { manifest: fixtureManifest, seal } = require("./fixtures/contracts.cjs");
const { validateExecutionUnit } = require("./worker.cjs");
const { fingerprintInput } = require("./model.cjs");
const policy = require("../../.github/policies/regression-bisect.json");

test("source smoke binds one fresh physical unit without changing the investigation allowance", () => {
    const manifest = fixtureManifest();
    for (const cell of manifest.matrix) { cell.os = "linux"; cell.architecture = "x64"; }
    seal(manifest);
    const revision = "a".repeat(40), sourceSha = "b".repeat(40), now = new Date("2026-10-20T00:00:00Z");
    const resolved = { sdkVersion: "10.0.402", archive: { sha512: "c".repeat(128) },
        metadata: [{ url: "https://builds.dotnet.microsoft.com/dotnet/release-metadata/releases-index.json", sha256: "d".repeat(64) }] };
    const archive = { status: 200, sha256: "e".repeat(64), sha512: resolved.archive.sha512 };
    const unit = sourceUnit({ revision, sourceSha, manifest, resolved, archive, now });
    assert.equal(validateExecutionUnit(unit), true);
    assert.equal(unit.policyDigest, fingerprintInput(policy));
    assert.equal(unit.workflowRevision, revision);
    assert.equal(unit.request.target.sourceSha, sourceSha);
    assert.equal(unit.stableSdk.archives[0].sha256, archive.sha256);
    assert.equal(Date.parse(unit.deadline) - now.getTime(), 110 * 60000);
    assert.equal(Object.hasOwn(unit, "budget"), false);
    assert.throws(() => sourceUnit({ revision, sourceSha, manifest, resolved,
        archive: { ...archive, sha512: "0".repeat(128) }, now }), /publisher metadata/);
});

test("physical source smoke rejects missing opt-in and relative paths before starting work", async () => {
    for (const args of [[], ["--run", "relative.json", "relative-output"], ["--verify", "x", "y"]])
        await assert.rejects(main(args), /usage:/);
});
