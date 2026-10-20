const assert = require("node:assert/strict");
const f = require("./contracts.cjs");
const { harness } = require("./checkpoint.cjs");
const model = require("../model.cjs");
const state = require("../state.cjs");

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

async function bootstrapFixture(options = {}) {
    const { readIssueSnapshot } = require("../intake.cjs");
    const fingerprint = model.fingerprintInput;
    const h = harness();
    const c = h.context;
    c.now = options.now ?? c.now;
    const resultCommentId = 457;
    const allowanceId = "reviewed-first-allowance";
    const deadline = options.deadline ?? "2026-10-01T00:00:00Z";
    const slot = {
        schemaVersion: 1, purpose: "checkpoint-slot", repository: c.repository, issueNumber: c.issueNumber,
        checkpointAnchorSha: c.checkpointAnchorSha, allowanceId, deadline,
    };
    h.comment.body = state.serializeWorkflowRecord({ kind: "proposal", status: "Reserved checkpoint slot", payload: slot }, c);
    const original = h.comment.body;
    const result = { ...structuredClone(h.comment), id: resultCommentId, body: state.serializeWorkflowRecord({
        kind: "proposal", status: "Reserved result slot", payload: {
            schemaVersion: 1, purpose: "result-slot", repository: c.repository,
            issueNumber: c.issueNumber, checkpointCommentId: c.commentId,
        },
    }, { ...c, commentId: resultCommentId }) };
    c.bootstrapDigest = fingerprint(original);
    const issue = { ...c.issue, id: 999, title: "Compiler regression", body: "Reviewed reproduction", user: { id: 1 } };
    const data = { issue, comments: [], timeline: [] };
    const mutations = [], reads = [];
    const { getComment, updateComment, request } = h.transport;
    h.transport.getComment = async id => id === resultCommentId ? structuredClone(result) : getComment(id);
    h.transport.updateComment = async (id, body) => {
        mutations.push({ operation: "comment", id, sha256: f.digest(body) });
        return updateComment(id, body);
    };
    h.transport.request = async (route, args) => {
        reads.push({ route, args: structuredClone(args) });
        if (route.includes("/statuses")) {
            if (route.startsWith("POST")) mutations.push({ operation: "status", sha: args.sha, context: args.context });
            return request(route, args);
        }
        if (route.endsWith("/issues/{issue_number}")) return { data: structuredClone(data.issue) };
        if (route.endsWith("/comments")) return { data: [h.comment, result, ...data.comments], headers: { link: "" } };
        if (route.endsWith("/timeline")) return { data: structuredClone(data.timeline), headers: { link: "" } };
        throw new Error(`Unexpected bootstrap route: ${route}`);
    };
    const authenticateRecord = async comment => {
        if (![c.commentId, resultCommentId].includes(comment.id)) return false;
        return (await state.authenticateDecodedRecord(h.transport, { ...c, commentId: comment.id }, comment)) !== null;
    };
    const snapshot = await readIssueSnapshot({
        transport: h.transport, repository: c.repository, issueNumber: c.issueNumber,
        authenticateRecord, readTrustedCheckpoints: async () => [], now: c.now,
    });
    assert.equal(snapshot.status, "complete", snapshot.reason);
    c.manifest.discussionFingerprint = c.issueFingerprint = snapshot.snapshot.discussionFingerprint;
    f.seal(c.manifest);
    c.authorization.manifestDigest = fingerprint(c.manifest);
    c.authorization.inputDigest = c.manifest.approvedInputDigest;
    const approval = {
        schemaVersion: 1, manifest: structuredClone(c.manifest),
        bootstrap: {
            allowanceId, deadline, checkpointCommentId: c.commentId, checkpointAnchorSha: c.checkpointAnchorSha,
            checkpointBootstrapDigest: c.bootstrapDigest, resultCommentId, resultBootstrapDigest: fingerprint(result.body),
        },
    };
    const deps = {
        context: c, transport: h.transport, withLock: action => h.lock(action), clock: () => new Date(c.now),
        approval: {
            readApproval: async reference => {
                assert.deepEqual(reference, { path: c.authorization.approvalPath, revision: c.policyRevision });
                return Buffer.from(JSON.stringify(approval));
            },
            readSource: async source => {
                assert.deepEqual(source, c.manifest.sources[0]);
                return Buffer.from('printfn "ok"');
            },
        },
    };
    return { h, deps, approval, data, result, original, mutations, reads };
}

module.exports = { fixture, bootstrapFixture };
