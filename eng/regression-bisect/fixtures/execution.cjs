const assert = require("node:assert/strict");
const f = require("./contracts.cjs");
const { harness } = require("./checkpoint.cjs");
const model = require("../model.cjs");

async function fixture() {
    const h = harness();
    const environments = Object.fromEntries(h.context.manifest.matrix.map(c =>
        [c.id, { os: c.os, architecture: c.architecture, imageDigest: f.digest("runner image") }]));
    h.context.environmentFingerprint = model.fingerprintInput(environments);
    await h.start();
    const deps = {
        context: h.context, transport: h.transport, environments,
        clock: () => new Date(h.context.now),
        platform: { os: "linux", architecture: "x64", rid: "linux-x64" },
        intake: { readIssueSnapshot: async () => ({ status: "complete", snapshot: {
            discussionFingerprint: h.context.issueFingerprint,
            input: { discussion: { issue: h.context.issue }, reproduction: { manifest: h.context.manifest } },
            investigations: { status: "complete", active: [] },
        } }) },
        approval: { readSource: async () => Buffer.from('printfn "ok"'),
            authenticateRecord: async () => false, readTrustedCheckpoints: async () => [] },
        toolchains: { resolveLatestStableSdk: async () => ({ status: "resolved", toolchain: {
            sdkVersion: h.context.stableSdk.version,
            metadata: [{ url: h.context.stableSdk.releaseIndexUrl, sha256: h.context.stableSdk.releaseIndexDigest }],
        } }) },
        acquireToolchain: async () => assert.fail("acquisition must not run in the controller"),
        inventoryReleased: async () => assert.fail("inventory must not run in the controller"),
        runReproduction: async () => assert.fail("reproduction must not run in the controller"),
    };
    return { h, deps };
}

module.exports = { fixture };
