const { test } = require("node:test");
const assert = require("node:assert/strict");
const { readFileSync } = require("node:fs");
const { join } = require("node:path");
const f = require("./fixtures/contracts.cjs");
const model = require("./model.cjs");
const {
    initializeCheckpoint, serializeWorkflowRecord, authenticateWorkflowRecord,
    loadCheckpoint, reserveWork, commitObservation, invalidateStaleEvidence,
} = require("./state.cjs");

const clone = structuredClone;
function harness() {
    const a = f.admission();
    const context = {
        repository: "dotnet/fsharp", issueNumber: 123, commentId: 456,
        actor: { id: 77, login: "github-actions[bot]", type: "Bot", appId: 88 },
        workflowPath: ".github/workflows/regression-bisect.yml",
        workflowRef: "refs/heads/main", trustedWorkflowRevisions: [f.sha("workflow")],
        keys: { fixture: Buffer.from("synthetic-test-key-not-a-production-secret") }, keyId: "fixture",
        owner: { runId: "12345", attempt: 1 }, now: "2026-09-18T00:00:00Z",
        issue: { number: 123, state: "open", created_at: "2026-09-01T00:00:00Z", labels: ["Regression"] },
        issueFingerprint: a.manifest.discussionFingerprint,
        manifest: a.manifest, authorization: a.budget.authorization, policy: a.budget.policy,
        policyRevision: a.budget.policyRevision, workflowRevision: a.budget.workflowRevision,
        pinnedMainSha: a.ancestry.pinnedMainSha, stableSdk: a.budget.stableSdk,
    };
    const runs = new Map();
    const run = owner => ({
        id: Number(owner.runId), run_attempt: owner.attempt, path: context.workflowPath,
        head_sha: context.workflowRevision, head_branch: "main",
        repository: { full_name: context.repository }, head_repository: { full_name: context.repository },
        status: "in_progress", conclusion: null,
    });
    runs.set("12345/1", run(context.owner));
    let comment = {
        id: 456, user: { id: 77, login: "github-actions[bot]", type: "Bot" },
        performed_via_github_app: { id: 88 },
        issue_url: "https://api.github.com/repos/dotnet/fsharp/issues/123",
        body: "unused placeholder",
    };
    let tail = Promise.resolve();
    const transport = {
        getComment: async id => { assert.equal(id, 456); return clone(comment); },
        updateComment: async (id, body) => { assert.equal(id, 456); comment.body = body; return clone(comment); },
        getRun: async (id, attempt) => {
            const value = runs.get(`${id}/${attempt}`);
            if (!value) throw new Error("run unavailable");
            return clone(value);
        },
        readEvidence: async () => Buffer.from("durable evidence"),
    };
    return {
        context, transport, runs,
        get comment() { return comment; },
        set comment(c) { comment = c; },
        // This stands for the controller's whole issue concurrency group, not comment CAS.
        lock(action) {
            const next = tail.then(action);
            tail = next.catch(() => {});
            return next;
        },
        async start() {
            context.bootstrapDigest = model.fingerprintInput(comment.body);
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

const request = (sourceSha = f.sha("main"), endpoint = "main") => ({
    kind: "source", target: { sourceSha }, endpoint, phase: "main",
});
const reserve = (h, generation = 0, attemptId = "attempt-1", work = request()) =>
    reserveWork(h.transport, h.context, { generation, attemptId, request: work });
function completion(h, pending, outcome = "product-failure") {
    const data = f.observation(pending.request.kind, outcome, h.context.manifest, pending.request.target.sourceSha);
    data.buildInstanceId = pending.id;
    data.provenance.runId = pending.owner.runId;
    for (const cell of data.cells) {
        cell.attempts = cell.attempts.map(a => ({ ...a, execution: {
            ...a.execution, ...h.context.manifest.assertions[outcome === "pass" ? "pass" : "failure"],
        } }));
        cell.outcome = model.classifyAttempts(cell.attempts, h.context.manifest);
        for (const artifact of Object.values(cell.artifacts)) artifact.buildInstanceId = pending.id;
    }
    data.outcome = data.cells[0].outcome;
    return { generation: pending.generation, attemptId: pending.id, observation: data, evidence: [] };
}

test("checkpoint authenticates exact author, app, comment, issue and live run provenance", async t => {
    const mutations = {
        author: h => { h.comment.user.id++; },
        app: h => { h.comment.performed_via_github_app.id++; },
        displayName: h => { h.comment.user.login = "impostor"; },
        comment: h => { h.comment.id++; },
        issue: h => { h.comment.issue_url += "4"; },
        body: h => { h.comment.body = h.comment.body.replace("discovery", "complete"); },
        runRepository: h => { h.runs.get("12345/1").repository.full_name = "attacker/fsharp"; },
        runHeadRepository: h => { h.runs.get("12345/1").head_repository.full_name = "attacker/fsharp"; },
        runRef: h => { h.runs.get("12345/1").head_branch = "untrusted"; },
        runRevision: h => { h.runs.get("12345/1").head_sha = f.sha("attacker"); },
        runPath: h => { h.runs.get("12345/1").path = ".github/workflows/other.yml"; },
        runAttempt: h => { h.runs.get("12345/1").run_attempt = 2; },
        key: h => { h.context.keys.fixture = Buffer.from("wrong-key"); },
    };
    for (const [name, mutate] of Object.entries(mutations)) {
        await t.test(name, async () => {
            const h = harness(); await h.start(); mutate(h);
            await assert.rejects(loadCheckpoint(h.transport, h.context), /authentic|provenance/i);
        });
    }
    const h = harness(); await h.start();
    const loaded = await loadCheckpoint(h.transport, h.context);
    assert.equal(loaded.executionAllowed, true);
    assert.equal(loaded.commentId, 456);
    assert.deepEqual(loaded.owner, { runId: "12345", attempt: 1 });
    assert.equal(loaded.checkpoint.state.budget.consumed.workerUnits, 0);
    await assert.rejects(h.start(), /bootstrap|initializ/i);
});

test("only authenticated workflow records may be excluded from issue input", async () => {
    const h = harness(); await h.start();
    assert.equal(await authenticateWorkflowRecord(h.transport, h.context, h.comment), true);
    h.comment.user.id++;
    assert.equal(await authenticateWorkflowRecord(h.transport, h.context, h.comment), false);
    h.comment.user.id--;
    h.comment.body = "<!-- regression-bisect -->\n```json\n{}\n```";
    assert.equal(await authenticateWorkflowRecord(h.transport, h.context, h.comment), false);
});

test("malformed, unknown-schema and oversized checkpoints fail closed without replacement", async t => {
    for (const body of ["", "```json\n{\n```", "x".repeat(48 * 1024 + 1)]) {
        await t.test(`length ${body.length}`, async () => {
            const h = harness(); await h.start(); h.comment.body = body;
            await assert.rejects(loadCheckpoint(h.transport, h.context));
            assert.equal(h.comment.body, body);
        });
    }
    const h = harness(); const checkpoint = await h.start();
    checkpoint.schemaVersion = 99;
    h.comment.body = serializeWorkflowRecord({ kind: "checkpoint", status: "discovery", payload: checkpoint }, h.context);
    await assert.rejects(loadCheckpoint(h.transport, h.context), /schema/i);
    h.transport.getComment = async () => { throw new Error("missing authoritative comment"); };
    await assert.rejects(loadCheckpoint(h.transport, h.context), /missing authoritative/);
});

test("signed malformed ledgers cannot lose reservations or manufacture authenticated evidence", async t => {
    for (const mutate of [
        cp => { cp.extra = true; },
        cp => { cp.state.budget.consumed.workerUnits = 0; },
        cp => { cp.records = []; cp.sequence = 0; },
        cp => { cp.pending = null; cp.state.budget.pendingWork = null; },
        cp => { cp.state.budget.authenticatedObservationDigests = [f.digest("fabricated")]; },
    ]) {
        await t.test("invalid ledger", async () => {
            const h = harness(); await h.start(); await reserve(h);
            const cp = (await loadCheckpoint(h.transport, h.context)).checkpoint;
            mutate(cp);
            h.comment.body = serializeWorkflowRecord({ kind: "checkpoint", status: "tampered", payload: cp }, h.context);
            await assert.rejects(loadCheckpoint(h.transport, h.context));
        });
    }
});

test("state is not approval and manual invocation cannot bypass current eligibility", async t => {
    for (const change of [
        h => { h.context.issue.labels = ["regression"]; },
        h => { h.context.issue.state = "closed"; },
        h => { h.context.issue.pull_request = {}; },
        h => { h.context.issue.created_at = "2026-08-31T23:59:59Z"; },
        h => { h.context.authorization = null; },
        h => { h.context.authorization.approvalRevision = f.sha("other"); },
        h => { h.context.issueFingerprint = f.digest("correction"); },
    ]) {
        await t.test("current context", async () => {
            const h = harness(); await h.start(); change(h);
            const loaded = await loadCheckpoint(h.transport, h.context);
            assert.equal(loaded.executionAllowed, false);
            assert.equal(loaded.publicationAllowed, false);
            await assert.rejects(reserve(h), /blocked|eligible|approval|stale/i);
        });
    }
});

test("reservation is durable before execution and duplicate dispatch cannot charge twice", async () => {
    const h = harness(); await h.start();
    const first = await reserve(h);
    const duplicate = await reserve(h);
    assert.equal(first.execute, true);
    assert.equal(duplicate.execute, false);
    assert.deepEqual({ ...duplicate, execute: true }, first);
    const loaded = (await loadCheckpoint(h.transport, h.context)).checkpoint;
    assert.equal(loaded.generation, 1);
    assert.deepEqual(loaded.state.budget.consumed, { sourceBuilds: 1, workerUnits: 1, reservedMinutes: 110 });
    assert.equal(loaded.pending.id, "attempt-1");
    assert.deepEqual(loaded.pending.request, request());
    assert.equal(loaded.pending.deadline, "2026-09-18T01:50:00.000Z");
    await assert.rejects(reserve(h, 1, "attempt-2"), /active|pending/i);
    await assert.rejects(reserve(h, 1, "attempt-1", request(f.sha("other"))), /identity|reuse|conflict/i);
});

test("lost write acknowledgement cannot cause duplicate execution under the same reservation", async () => {
    const h = harness(); await h.start();
    const update = h.transport.updateComment;
    h.transport.updateComment = async (...args) => { await update(...args); throw new Error("response lost"); };
    await assert.rejects(reserve(h), /response lost/);
    h.transport.updateComment = update;
    const duplicate = await reserve(h);
    assert.equal(duplicate.execute, false);
    assert.equal((await loadCheckpoint(h.transport, h.context)).checkpoint.state.budget.consumed.workerUnits, 1);
});

test("expired reservations cannot issue another execution permission", async () => {
    const h = harness(); await h.start(); await reserve(h);
    h.context.now = "2026-09-18T02:00:00Z";
    const duplicate = await reserve(h);
    assert.equal(duplicate.execute, false);
    assert.equal((await loadCheckpoint(h.transport, h.context)).checkpoint.state.budget.attempts[0].count, 1);
});

test("transport failure never returns executable work or partially alters caller input", async () => {
    const h = harness(); await h.start();
    const before = h.comment.body;
    h.transport.updateComment = async () => { throw new Error("checkpoint write failed"); };
    await assert.rejects(reserve(h), /checkpoint write failed/);
    assert.equal(h.comment.body, before);
});

test("controller lock serializes concurrent requests; stale generation and owner are rejected", async () => {
    const h = harness(); await h.start();
    const results = await Promise.allSettled([
        h.lock(() => reserve(h, 0, "one")),
        h.lock(() => reserve(h, 0, "two")),
    ]);
    assert.deepEqual(results.map(r => r.status), ["fulfilled", "rejected"]);
    assert.match(results[1].reason.message, /generation/);
    const pending = results[0].value;
    const done = completion(h, pending);
    h.context.owner = { runId: "54321", attempt: 1 };
    await assert.rejects(commitObservation(h.transport, h.context, done), /owner/);
    h.context.owner = pending.owner;
    done.generation--;
    await assert.rejects(commitObservation(h.transport, h.context, done), /generation/);
});

test("commit binds exact request, build identity, owner and observation; duplicate completion is idempotent", async () => {
    const h = harness(); await h.start();
    const pending = await reserve(h);
    const done = completion(h, pending);
    const wrongTarget = clone(done);
    wrongTarget.observation = f.observation("source", "pass", h.context.manifest, f.sha("other"));
    await assert.rejects(commitObservation(h.transport, h.context, wrongTarget), /request|identity|build/);
    const checkpoint = await commitObservation(h.transport, h.context, done);
    assert.equal(checkpoint.pending, null);
    assert.equal(checkpoint.state.endpointEvidence.main, model.fingerprintInput(done.observation));
    assert.equal(checkpoint.state.testedRevisions.length, 1);
    assert.equal(checkpoint.records[0].status, "committed");
    assert.deepEqual(await commitObservation(h.transport, h.context, done), checkpoint);
    const changed = completion(h, pending, "pass");
    await assert.rejects(commitObservation(h.transport, h.context, changed), /identity|conflict/);
    await assert.rejects(commitObservation(h.transport, h.context, { ...done, generation: 999 }), /generation/);
    await assert.rejects(reserve(h, 1, "next"), /generation/);
});

test("a cancelled owner cannot commit a late worker result", async () => {
    const h = harness(); await h.start();
    const pending = await reserve(h);
    const done = completion(h, pending);
    h.runs.get("12345/1").status = "completed";
    h.runs.get("12345/1").conclusion = "cancelled";
    await assert.rejects(commitObservation(h.transport, h.context, done), /active|owner|completed/);
    assert.equal((await loadCheckpoint(h.transport, h.context)).checkpoint.pending.id, pending.id);
});

test("all three cancellation points preserve charges and replay only uncommitted work", async t => {
    for (const point of ["before-observation", "before-commit", "after-commit"]) {
        await t.test(point, async () => {
            const h = harness(); await h.start();
            const pending = await reserve(h);
            const done = completion(h, pending);
            if (point === "after-commit") await commitObservation(h.transport, h.context, done);
            h.nextRun();
            const loaded = (await loadCheckpoint(h.transport, h.context)).checkpoint;
            if (point === "after-commit") {
                assert.equal(loaded.pending, null);
                assert.equal(Object.keys(loaded.observations).length, 1);
                const next = await reserve(h, loaded.generation, "next", request(f.sha("good"), "good"));
                assert.equal(next.sequence, 2);
            } else {
                const retry = await reserve(h, loaded.generation, "retry");
                assert.equal(retry.sequence, 2);
                assert.equal(retry.attempt, 2);
                assert.equal((await loadCheckpoint(h.transport, h.context)).checkpoint.records[0].status, "cancelled");
                await assert.rejects(commitObservation(h.transport, h.context, done), /owner|generation|cancelled/);
            }
            assert.deepEqual((await loadCheckpoint(h.transport, h.context)).checkpoint.state.budget.consumed,
                { sourceBuilds: 2, workerUnits: 2, reservedMinutes: 220 });
        });
    }
});

test("active, unknown or unreadable owning run cannot be reclaimed even after deadline", async t => {
    for (const status of ["in_progress", "queued", "waiting", "requested", "pending", "unknown", "unreadable"]) {
        await t.test(status, async () => {
            const h = harness(); await h.start(); await reserve(h);
            h.context.now = "2026-09-19T00:00:00Z";
            h.runs.get("12345/1").status = status;
            if (status === "unreadable") h.transport.getRun = async () => { throw new Error("run unavailable"); };
            await assert.rejects(reserve(h, 1, "retry"), /active|run|pending/);
            assert.equal(h.comment.body.includes("retry"), false);
        });
    }
});

test("two attempts per item, cumulative reservation limit and absolute deadline survive redispatch", async () => {
    const h = harness(); await h.start(); await reserve(h); h.nextRun();
    await reserve(h, 1, "retry"); h.nextRun();
    await assert.rejects(reserve(h, 2, "third"), /attempt/);
    let generation = 2;
    for (let i = 0; i < 4; i++) {
        const p = await reserve(h, generation, `different-${i}`, request(f.sha(`revision-${i}`), null));
        generation = p.generation;
        h.nextRun();
    }
    await assert.rejects(reserve(h, generation, "exhausted", request(f.sha("last"), null)), /budget/);
    const totals = (await loadCheckpoint(h.transport, h.context)).checkpoint.state.budget.consumed;
    assert.equal(totals.reservedMinutes, 660);
    h.context.now = "2026-10-02T00:00:00Z";
    await assert.rejects(reserve(h, generation, "late", request(f.sha("last"), null)), /deadline/);
});

test("input changes and label toggles invalidate evidence but never replenish an allowance", async () => {
    const h = harness(); await h.start();
    const p = await reserve(h);
    await commitObservation(h.transport, h.context, completion(h, p));
    const before = (await loadCheckpoint(h.transport, h.context)).checkpoint.state.budget.consumed;
    h.context.issue.labels = [];
    let cp = await invalidateStaleEvidence(h.transport, h.context, { generation: 2 });
    assert.equal(cp.state.phase, "stopped");
    assert.equal(Object.keys(cp.observations).length, 1);
    h.context.issue.labels = ["Regression"];
    h.context.manifest.discussionFingerprint = f.digest("later correction");
    h.context.issueFingerprint = h.context.manifest.discussionFingerprint;
    f.seal(h.context.manifest);
    h.context.authorization.manifestDigest = model.fingerprintInput(h.context.manifest);
    h.context.authorization.inputDigest = h.context.manifest.approvedInputDigest;
    cp = await invalidateStaleEvidence(h.transport, h.context, { generation: cp.generation });
    assert.deepEqual(cp.state.budget.consumed, before);
    assert.equal(cp.state.budget.allowanceId, "maintainer-approved-investigation-1");
    assert.deepEqual(cp.state.endpointEvidence, { stable: null, main: null, good: null });
    assert.equal(Object.values(cp.observations)[0].valid, false);
    assert.equal(cp.state.publication.commentId, null);
    assert.equal(cp.publicationStatus, "unpublished");
    assert.equal((await loadCheckpoint(h.transport, h.context)).executionAllowed, true);
});

test("invalidating an active run cannot release its reservation; stopped publication retains its audit trail", async () => {
    const h = harness(); await h.start(); await reserve(h);
    h.context.issue.labels = [];
    await assert.rejects(invalidateStaleEvidence(h.transport, h.context, { generation: 1 }), /active/);
    const active = await loadCheckpoint(h.transport, h.context);
    assert.equal(active.executionAllowed, false);
    assert.notEqual(active.checkpoint.pending, null);
    h.nextRun();
    const cp = await invalidateStaleEvidence(h.transport, h.context, { generation: 1 });
    assert.equal(cp.pending, null);
    assert.equal(cp.records[0].status, "invalidated");
    assert.equal(cp.state.budget.consumed.workerUnits, 1);
});

test("withdrawn approval invalidates evidence and label removal retains published result identity", async () => {
    const h = harness(); await h.start();
    const p = await reserve(h);
    const published = await commitObservation(h.transport, h.context, completion(h, p));
    published.state.publication = { digest: f.digest("result"), commentId: 789 };
    published.publicationStatus = "published";
    h.comment.body = serializeWorkflowRecord({ kind: "checkpoint", status: "published", payload: published }, h.context);
    h.context.issue.state = "closed";
    let cp = await invalidateStaleEvidence(h.transport, h.context, { generation: 2 });
    assert.deepEqual(cp.state.publication, published.state.publication);
    assert.equal(cp.publicationStatus, "blocked");
    h.context.issue.state = "open";
    h.context.authorization = null;
    cp = await invalidateStaleEvidence(h.transport, h.context, { generation: cp.generation });
    assert.equal(cp.state.phase, "blocked");
    assert.equal(Object.values(cp.observations)[0].valid, false);
    assert.deepEqual(cp.state.budget.authenticatedObservationDigests, []);
    assert.deepEqual(cp.state.endpointEvidence, { stable: null, main: null, good: null });
    assert.ok(cp.history.some(entry => entry.publication.commentId === 789));
});

test("observation size overflow leaves the charged reservation durable and blocks completion", async () => {
    const h = harness(); await h.start();
    const p = await reserve(h); const done = completion(h, p);
    done.evidence = Array.from({ length: 16 }, (_, i) => ({
        required: false, sha256: f.digest("durable evidence"),
        url: `https://example.com/${i}/${String(i).repeat(4000)}`,
    }));
    await assert.rejects(commitObservation(h.transport, h.context, done), /size|48/);
    const cp = (await loadCheckpoint(h.transport, h.context)).checkpoint;
    assert.equal(cp.pending.id, p.id);
    assert.equal(cp.state.budget.consumed.workerUnits, 1);
    assert.deepEqual(cp.observations, {});
});

test("untestable observations retain ordered tested and skipped reasons and full evidence", async () => {
    const h = harness(); await h.start();
    const p = await reserve(h);
    const done = completion(h, p);
    for (const cell of done.observation.cells) {
        cell.attempts.forEach(a => { a.execution.status = "timeout"; });
        cell.outcome = model.classifyAttempts(cell.attempts, h.context.manifest);
    }
    done.observation.outcome = done.observation.cells[0].outcome;
    const cp = await commitObservation(h.transport, h.context, done);
    assert.equal(cp.state.skips[0].reason, "timeout");
    assert.equal(cp.state.skips[0].observationDigest, cp.state.testedRevisions[0].observationDigest);
});

test("changed approval, reproduction, producer and orchestration invalidate dependent pending work", async t => {
    for (const change of [
        h => { h.context.manifest.sources[0].sha256 = f.digest("corrected source"); },
        h => { h.context.manifest.assertions.failure.stderr = "corrected assertion"; },
        h => { h.context.manifest.producer.sha256 = f.digest("new producer"); },
        h => { h.context.policyRevision = f.sha("new approval"); h.context.authorization.approvalRevision = h.context.policyRevision; },
        h => { h.context.workflowRevision = f.sha("new workflow"); h.context.trustedWorkflowRevisions.push(h.context.workflowRevision); },
    ]) {
        await t.test("changed dependency", async () => {
            const h = harness(); await h.start(); const p = await reserve(h); const done = completion(h, p);
            h.nextRun(); change(h);
            h.runs.get(`${h.context.owner.runId}/1`).head_sha = h.context.workflowRevision;
            f.seal(h.context.manifest);
            h.context.authorization.manifestDigest = model.fingerprintInput(h.context.manifest);
            h.context.authorization.inputDigest = h.context.manifest.approvedInputDigest;
            assert.equal((await loadCheckpoint(h.transport, h.context)).executionAllowed, false);
            const cp = await invalidateStaleEvidence(h.transport, h.context, { generation: 1 });
            assert.equal(cp.pending, null);
            assert.equal(cp.records[0].status, "invalidated");
            assert.equal(cp.state.budget.consumed.workerUnits, 1);
            await assert.rejects(commitObservation(h.transport, h.context, done));
        });
    }
});

test("moving main and stable start endpoint epochs while retaining exact historical source evidence", async () => {
    const h = harness(); await h.start();
    const p = await reserve(h, 0, "good", request(f.sha("good"), "good"));
    const done = completion(h, p, "pass");
    await commitObservation(h.transport, h.context, done);
    const old = (await loadCheckpoint(h.transport, h.context)).checkpoint;
    h.context.pinnedMainSha = f.sha("new main");
    let cp = await invalidateStaleEvidence(h.transport, h.context, { generation: old.generation });
    assert.notEqual(cp.state.epoch, old.state.epoch);
    assert.equal(cp.state.endpointEvidence.good, model.fingerprintInput(done.observation));
    assert.equal(cp.observations[model.fingerprintInput(done.observation)].valid, true);
    assert.equal(cp.state.testedRevisions.length, 1);
    assert.equal(cp.state.interval, null);
    h.context.stableSdk.releaseIndexDigest = f.digest("new release metadata");
    cp = await invalidateStaleEvidence(h.transport, h.context, { generation: cp.generation });
    assert.equal(cp.state.endpointEvidence.good, model.fingerprintInput(done.observation));
    assert.equal(cp.history.length, 2);
});

test("mandatory external evidence loss blocks reuse; optional expired logs are disclosed", async t => {
    for (const required of [true, false]) {
        await t.test(required ? "mandatory" : "optional", async () => {
            const h = harness(); await h.start();
            const p = await reserve(h);
            const done = completion(h, p);
            done.evidence = [{ required, sha256: f.digest("durable evidence"), url: "https://github.com/dotnet/fsharp/actions/runs/12345/artifacts/1" }];
            await commitObservation(h.transport, h.context, done);
            h.transport.readEvidence = async () => null;
            const loaded = await loadCheckpoint(h.transport, h.context);
            assert.equal(loaded.executionAllowed, !required);
            assert.equal(loaded.publicationAllowed, !required);
            assert.match(loaded.warnings.join(), /missing.*evidence|expired/i);
            assert.equal(loaded.checkpoint.state.budget.consumed.workerUnits, 1);
        });
    }
});

test("external evidence hashes and malformed observations cannot be accepted", async () => {
    const h = harness(); await h.start();
    const p = await reserve(h); const done = completion(h, p);
    done.evidence = [{ required: true, sha256: f.digest("not the data"), url: "https://example.com/evidence" }];
    await assert.rejects(commitObservation(h.transport, h.context, done), /evidence/);
    done.evidence = []; done.observation.cells[0].attempts = [];
    await assert.rejects(commitObservation(h.transport, h.context, done), /attempt|evidence/);
    assert.notEqual((await loadCheckpoint(h.transport, h.context)).checkpoint.pending, null);
});

test("repeated identity/evidence tables compact below 48 KiB without dropping observations", async () => {
    const h = harness();
    h.context.manifest.assertions.failure.stderr = "x".repeat(12000);
    f.seal(h.context.manifest);
    h.context.authorization.manifestDigest = model.fingerprintInput(h.context.manifest);
    h.context.authorization.inputDigest = h.context.manifest.approvedInputDigest;
    await h.start();
    const p = await reserve(h);
    const done = completion(h, p);
    await commitObservation(h.transport, h.context, done);
    assert.ok(Buffer.byteLength(h.comment.body) < 48 * 1024);
    const cp = (await loadCheckpoint(h.transport, h.context)).checkpoint;
    assert.deepEqual(cp.observations[model.fingerprintInput(done.observation)].data, done.observation);
    cp.history.push({ reason: "x".repeat(48 * 1024) });
    assert.throws(() => serializeWorkflowRecord({ kind: "checkpoint", status: "blocked", payload: cp }, h.context), /size|48/);
});

test("serialized checkpoint fixture replays reservation, observation, cancellation and resume", async () => {
    const h = harness();
    h.comment.body = readFileSync(join(__dirname, "fixtures", "checkpoint.txt"), "utf8").trimEnd();
    const initial = await loadCheckpoint(h.transport, h.context);
    assert.equal(initial.checkpoint.generation, 0);
    const first = await reserve(h);
    await commitObservation(h.transport, h.context, completion(h, first));
    const uncommitted = await reserve(h, 2, "cancelled", request(f.sha("good"), "good"));
    completion(h, uncommitted, "pass");
    h.nextRun();
    const resumed = await reserve(h, 3, "resumed", request(f.sha("good"), "good"));
    const cp = await commitObservation(h.transport, h.context, completion(h, resumed, "pass"));
    assert.equal(Object.keys(cp.observations).length, 2);
    assert.equal(cp.records[1].status, "cancelled");
    assert.deepEqual(cp.state.budget.consumed, { sourceBuilds: 3, workerUnits: 3, reservedMinutes: 330 });
    assert.equal(cp.generation, 5);
});

module.exports = { harness };
