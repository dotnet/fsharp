const assert = require("node:assert/strict");
const f = require("./contracts.cjs");
const model = require("../model.cjs");
const { initializeCheckpoint, serializeWorkflowRecord } = require("../state.cjs");

function harness(snapshot) {
    const a = f.admission();
    const context = {
        repository: "dotnet/fsharp", issueNumber: 123, commentId: 456,
        actor: { id: 77, login: "github-actions[bot]", type: "Bot", appId: 88 },
        workflowPath: ".github/workflows/regression-bisect.yml",
        workflowRef: "refs/heads/main", trustedWorkflowRevisions: [f.sha("workflow")],
        checkpointAnchorSha: f.sha("workflow"), bootstrapDigest: model.fingerprintInput("unused placeholder"),
        keys: { fixture: Buffer.from("synthetic-test-key-not-a-production-secret") }, keyId: "fixture",
        owner: { runId: "12345", attempt: 1 }, now: "2026-09-18T00:00:00Z",
        issue: { number: 123, state: "open", created_at: "2026-09-01T00:00:00Z", labels: ["Regression"] },
        issueFingerprint: a.manifest.discussionFingerprint,
        manifest: a.manifest, authorization: a.budget.authorization, policy: a.budget.policy,
        policyRevision: a.budget.policyRevision, workflowRevision: a.budget.workflowRevision,
        pinnedMainSha: a.ancestry.pinnedMainSha, stableSdk: a.budget.stableSdk,
    };
    if (snapshot) Object.assign(context, snapshot.context, {
        keys: Object.fromEntries(Object.entries(snapshot.context.keys).map(([key, value]) => [key, Buffer.from(value.data)])),
    });
    const runs = new Map(snapshot?.runs);
    const run = owner => ({
        id: Number(owner.runId), run_attempt: owner.attempt, path: context.workflowPath,
        head_sha: context.workflowRevision, head_branch: "main",
        repository: { full_name: context.repository }, head_repository: { full_name: context.repository },
        status: "in_progress", conclusion: null,
    });
    if (!snapshot) runs.set("12345/1", run(context.owner));
    let comment = snapshot?.comment ?? {
        id: 456, user: { id: 77, login: "github-actions[bot]", type: "Bot" },
        performed_via_github_app: { id: 88 },
        issue_url: "https://api.github.com/repos/dotnet/fsharp/issues/123",
        body: "unused placeholder",
    };
    let tail = Promise.resolve();
    const freshness = f.statusTransport(context);
    if (snapshot) freshness.statuses.push(...snapshot.statuses);
    const transport = {
        request: freshness.request,
        getComment: async id => { assert.equal(id, 456); return structuredClone(comment); },
        updateComment: async (id, body) => { assert.equal(id, 456); comment.body = body; return structuredClone(comment); },
        getRun: async (id, attempt) => {
            const value = runs.get(`${id}/${attempt}`);
            if (!value) throw new Error("run unavailable");
            return structuredClone(value);
        },
        readEvidence: async () => Buffer.from("durable evidence"),
    };
    return {
        context, transport, runs, statuses: freshness.statuses,
        get comment() { return comment; },
        set comment(c) { comment = c; },
        checkpoint(cp) {
            cp.previousDigest = f.digest(comment.body);
            cp.generation++;
            comment.body = serializeWorkflowRecord({ kind: "checkpoint", status: "fixture transition", payload: cp }, context);
        },
        lock(action) {
            const next = tail.then(action);
            tail = next.catch(() => {});
            return next;
        },
        async start() {
            return initializeCheckpoint(transport, context, {
                allowanceId: "maintainer-approved-investigation-1", deadline: "2026-10-01T00:00:00Z",
            });
        },
        nextRun() {
            runs.get(`${context.owner.runId}/${context.owner.attempt}`).status = "completed";
            runs.get(`${context.owner.runId}/${context.owner.attempt}`).conclusion = "cancelled";
            context.owner = { runId: String(Number(context.owner.runId) + 1), attempt: 1 };
            runs.set(`${context.owner.runId}/1`, run(context.owner));
        },
    };
}

module.exports = { harness };
