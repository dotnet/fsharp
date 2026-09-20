const { test } = require("node:test");
const assert = require("node:assert/strict");
const { readFileSync } = require("node:fs");
const { join } = require("node:path");
const f = require("./fixtures/contracts.cjs");
const model = require("./model.cjs");
const {
    initializeCheckpoint, serializeWorkflowRecord, authenticateWorkflowRecord,
    loadCheckpoint, reserveWork, beginWork, commitObservation, invalidateStaleEvidence,
} = require("./state.cjs");

const clone = structuredClone;
const { harness } = require("./fixtures/checkpoint.cjs");

test("execution claims survive lost acknowledgements and cannot execute a reservation twice", async () => {
    const h = harness(); await h.start();
    const pending = await reserve(h);
    const first = await beginWork(h.transport, h.context, {
        generation: pending.generation, attemptId: pending.id,
    });
    const duplicate = await beginWork(h.transport, h.context, {
        generation: pending.generation, attemptId: pending.id,
    });
    assert.equal(first.execute, true);
    assert.equal(duplicate.execute, false);
    assert.equal(first.generation, duplicate.generation);
    const done = { ...completion(h, pending), generation: first.generation };
    await commitObservation(h.transport, h.context, done);
    await commitObservation(h.transport, h.context, done);
    const cp = (await loadCheckpoint(h.transport, h.context)).checkpoint;
    assert.equal(cp.state.budget.consumed.workerUnits, 1);
    assert.equal(cp.records[0].executionGeneration, first.generation);
});

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
            status: outcome === "untestable" ? "timeout" : a.execution.status,
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
            h.statuses.length = 0;
            await assert.rejects(h.start(), /authentic|provenance/i);
            assert.equal(h.statuses.length, 0);
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

test("streaming evidence verification preserves required-loss and transport-error semantics", async t => {
    for (const fault of ["absent", "changed", "authentication", "oversized"]) await t.test(fault, async () => {
        const h = harness(); await h.start();
        const pending = await reserve(h);
        const done = completion(h, pending);
        done.evidence = [{ required: true, sha256: f.digest("durable evidence"),
            url: "https://github.com/dotnet/fsharp/actions/runs/12345/artifacts/1" }];
        const measured = { bytes: 16, sha256: done.evidence[0].sha256 };
        h.transport.readEvidence = () => assert.fail("streaming transport must not buffer blobs");
        h.transport.verifyEvidence = async () => measured;
        await commitObservation(h.transport, h.context, done);
        h.transport.verifyEvidence = async () => {
            if (fault === "authentication") throw new Error("artifact authentication failed");
            return fault === "absent" ? null : fault === "changed" ? { ...measured, sha256: f.digest("changed") } :
                { ...measured, bytes: 1073741825 };
        };
        if (fault === "authentication" || fault === "oversized")
            await assert.rejects(loadCheckpoint(h.transport, h.context), /authentication|streaming/);
        else {
            const loaded = await loadCheckpoint(h.transport, h.context);
            assert.equal(loaded.executionAllowed, false);
            assert.equal(loaded.publicationAllowed, false);
            assert.equal(loaded.checkpoint.state.budget.consumed.sourceBuilds, 1);
        }
    });
});

for (const [os, kind] of [
    ["windows", "publisher-checksum"], ["windows", "approved-official-https-measured"],
    ["linux", "publisher-checksum"], ["macos", "publisher-checksum"],
]) test(`checkpoint roundtrip preserves ${os} ${kind} worker receipt paths`, async () => {
    const h = harness();
    const { context } = h;
    context.manifest.matrix[0].os = os;
    f.seal(context.manifest);
    context.authorization.manifestDigest = model.fingerprintInput(context.manifest);
    context.authorization.inputDigest = context.manifest.approvedInputDigest;
    context.stableSdk.archives[0].os = os;
    context.pinnedMainSha = f.policy().toolchains.measuredArchives[0].sourceSha;
    await h.start();
    const pending = await reserve(h, 0, "receipt-roundtrip", request(context.pinnedMainSha));
    const done = completion(h, pending);
    done.observation.cells[0].buildToolchain = f.buildToolchain(context.pinnedMainSha, os, kind);
    await commitObservation(h.transport, context, done);
    const { checkpoint: loaded } = await loadCheckpoint(h.transport, context);
    assert.deepEqual(loaded.observations[model.fingerprintInput(done.observation)].data, done.observation);
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

test("restoring an authentic old body to the same comment cannot roll back evidence or budgets", async () => {
    const h = harness(); await h.start();
    const initialBody = h.comment.body;
    const first = await reserve(h);
    await commitObservation(h.transport, h.context, completion(h, first));
    for (let i = 0; i < 5; i++) {
        const cp = (await loadCheckpoint(h.transport, h.context)).checkpoint;
        await reserve(h, cp.generation, `charged-${i}`, request(f.sha(`charged-${i}`), null));
        h.nextRun();
    }
    const latestBody = h.comment.body;
    const latest = (await loadCheckpoint(h.transport, h.context)).checkpoint;
    assert.equal(latest.state.budget.consumed.reservedMinutes, 660);
    assert.equal(Object.keys(latest.observations).length, 1);
    await assert.rejects(reserve(h, latest.generation, "exhausted", request(f.sha("last"), null)), /budget/);
    h.comment.body = initialBody;
    await assert.rejects(loadCheckpoint(h.transport, h.context), /freshness|rollback/i);
    await assert.rejects(reserve(h, 0, "rollback"), /freshness|rollback/i);
    await assert.rejects(invalidateStaleEvidence(h.transport, h.context, { generation: 0 }), /freshness|rollback/i);
    h.comment.body = "unused placeholder";
    await assert.rejects(h.start(), /freshness|initializ/i);
    h.comment.body = latestBody;
    assert.deepEqual((await loadCheckpoint(h.transport, h.context)).checkpoint, latest);
});

test("freshness receipts cannot be forged, omitted, truncated or replaced by a newer replay", async t => {
    for (const [name, change] of [
        ["missing", h => { h.statuses.length = 0; }],
        ["writer", h => { h.statuses[0].creator.id++; }],
        ["signature", h => { h.statuses[0].target_url += "0"; }],
        ["generation", h => { h.statuses[0].description = h.statuses[0].description.replace("checkpoint:0:", "checkpoint:1:"); }],
        ["status", h => { h.statuses[0].state = "pending"; }],
        ["wrong issue", h => { h.statuses[0].target_url = h.statuses[0].target_url.replace("/123#", "/124#"); }],
        ["wrong comment", h => { h.statuses[0].context = "regression-bisect/123/789"; }],
        ["wrong anchor", h => { h.context.checkpointAnchorSha = f.sha("another anchor"); }],
        ["API failure", h => { h.transport.request = async () => { throw new Error("freshness unavailable"); }; }],
        ["HTTP error", h => { h.transport.request = async () => ({ status: 403, data: [] }); }],
        ["incomplete pagination", h => { h.transport.request = async () => ({ data: [], headers: { link: '<https://api.github.com/next>; rel="next"' } }); }],
        ["same-generation replacement", async h => {
            const cp = (await loadCheckpoint(h.transport, h.context)).checkpoint;
            h.comment.body = serializeWorkflowRecord({ kind: "checkpoint", status: "different authentic body",
                payload: cp }, h.context);
        }],
    ]) {
        await t.test(name, async () => {
            const h = harness(); await h.start();
            await change(h);
            await assert.rejects(loadCheckpoint(h.transport, h.context), /freshness/i);
        });
    }
    const h = harness(); await h.start();
    const initialBody = h.comment.body, oldReceipt = clone(h.statuses[0]);
    await reserve(h);
    for (let i = 0; i < 101; i++) h.statuses.push({ ...clone(oldReceipt), id: i + 10 });
    const receiptCount = h.statuses.length;
    assert.equal((await loadCheckpoint(h.transport, h.context)).checkpoint.state.budget.consumed.workerUnits, 1);
    assert.equal(h.statuses.length, receiptCount);
    h.comment.body = initialBody;
    await assert.rejects(loadCheckpoint(h.transport, h.context), /freshness|rollback/i);
});

test("freshness write cancellation recovers only the signed immediate successor without refund or execution", async t => {
    for (const stage of ["initialization", "reservation", "observation", "receipt-acknowledgement"]) {
        await t.test(stage, async () => {
            const h = harness();
            let pending;
            if (stage !== "initialization") await h.start();
            if (stage === "observation") pending = await reserve(h);
            const send = h.transport.request;
            h.transport.request = async (route, args) => {
                if (route.startsWith("POST")) {
                    if (stage === "receipt-acknowledgement") await send(route, args);
                    throw new Error("cancelled before freshness acknowledgement");
                }
                return send(route, args);
            };
            await assert.rejects(stage === "initialization" ? h.start() : stage === "observation"
                ? commitObservation(h.transport, h.context, completion(h, pending)) : reserve(h), /cancelled/);
            h.transport.request = send;
            h.nextRun();
            if (stage === "initialization") await h.start();
            const cp = (await loadCheckpoint(h.transport, h.context)).checkpoint;
            assert.equal(cp.state.budget.consumed.workerUnits, stage === "initialization" ? 0 : 1);
            assert.equal(Object.keys(cp.observations).length, stage === "observation" ? 1 : 0);
            if (cp.pending) {
                const retry = await reserve(h, cp.generation, "retry");
                assert.equal(retry.attempt, 2);
                assert.equal((await loadCheckpoint(h.transport, h.context)).checkpoint.state.budget.consumed.workerUnits, 2);
            }
        });
    }
    for (const gap of [false, true]) {
        const h = harness(); const cp = await h.start();
        h.checkpoint(cp);
        if (gap) cp.generation++;
        else cp.previousDigest = f.digest("unrelated checkpoint");
        h.comment.body = serializeWorkflowRecord({ kind: "checkpoint", status: "invalid predecessor", payload: cp }, h.context);
        await assert.rejects(loadCheckpoint(h.transport, h.context), /freshness|predecessor/i);
    }
});

test("interrupted initialization recovers original state before checking changed current inputs", async t => {
    const changes = {
        main: h => { h.context.pinnedMainSha = f.sha("new main"); },
        stable: h => { h.context.stableSdk.releaseIndexDigest = f.digest("new release metadata"); },
        discussion: h => { h.context.issueFingerprint = f.digest("unapproved correction"); },
        reproduction: h => {
            h.context.manifest.sources[0].sha256 = f.digest("corrected reproduction");
            f.seal(h.context.manifest);
            h.context.authorization.manifestDigest = model.fingerprintInput(h.context.manifest);
            h.context.authorization.inputDigest = h.context.manifest.approvedInputDigest;
        },
        approval: h => { h.context.authorization = null; },
        manifest: h => { h.context.manifest = null; },
        policy: h => { h.context.policyRevision = f.sha("new approval"); h.context.authorization.approvalRevision = h.context.policyRevision; },
        workflow: h => {
            h.context.workflowRevision = f.sha("new workflow");
            h.context.trustedWorkflowRevisions.push(h.context.workflowRevision);
            h.runs.get(`${h.context.owner.runId}/1`).head_sha = h.context.workflowRevision;
        },
        label: h => { h.context.issue.labels = []; },
        closed: h => { h.context.issue.state = "closed"; },
        deadline: h => { h.context.now = "2026-10-02T00:00:00Z"; },
    };
    for (const point of ["comment-acknowledgement", "receipt"]) {
        for (const [name, change] of Object.entries(changes)) {
            await t.test(`${point}: ${name}`, async () => {
                const h = harness();
                const original = await harness().start();
                const { updateComment, request: send } = h.transport;
                if (point === "comment-acknowledgement") {
                    h.transport.updateComment = async (...args) => {
                        await updateComment(...args);
                        throw new Error("initialization interrupted");
                    };
                } else {
                    h.transport.request = async (route, args) => {
                        if (route.startsWith("POST")) throw new Error("initialization interrupted");
                        return send(route, args);
                    };
                }
                await assert.rejects(h.start(), /initialization interrupted/);
                Object.assign(h.transport, { updateComment, request: send });
                const originalBody = h.comment.body;
                assert.equal(h.statuses.length, 0);
                h.nextRun();
                change(h);
                assert.deepEqual(await h.lock(() => h.start()), original);
                assert.equal(h.comment.body, originalBody);
                assert.equal(h.statuses.length, 1);
                const loaded = await loadCheckpoint(h.transport, h.context);
                assert.deepEqual(loaded.checkpoint, original);
                assert.deepEqual(loaded.owner, { runId: "12345", attempt: 1 });
                assert.equal(loaded.executionAllowed, false);
                assert.equal(loaded.publicationAllowed, false);
                await assert.rejects(reserve(h), /blocked/);
                const cp = await invalidateStaleEvidence(h.transport, h.context, { generation: 0 });
                assert.deepEqual(cp.state.budget.consumed, original.state.budget.consumed);
                assert.equal(cp.state.budget.allowanceId, original.state.budget.allowanceId);
                assert.equal(cp.deadline, original.deadline);
                assert.equal((await loadCheckpoint(h.transport, h.context)).executionAllowed,
                    ["main", "stable", "reproduction", "policy", "workflow"].includes(name));
            });
        }
    }
});

test("initialization recovery rejects malformed, noninitial or changed-allowance records without writing", async t => {
    const changes = {
        schema: cp => { cp.schemaVersion++; },
        issue: cp => { cp.state.issueNumber++; },
        generation: cp => { cp.generation++; },
        bootstrap: cp => { cp.previousDigest = f.digest("different placeholder"); },
        history: cp => { cp.history.push({ reason: "not initial state" }); },
        phase: cp => { cp.state.phase = "stopped"; },
        allowance: cp => { cp.state.budget.allowanceId = "different allowance"; },
        deadline: cp => { cp.deadline = "2026-11-01T00:00:00Z"; },
    };
    for (const [name, change] of Object.entries(changes)) {
        await t.test(name, async () => {
            const h = harness();
            const cp = await h.start();
            change(cp);
            h.comment.body = serializeWorkflowRecord({ kind: "checkpoint", status: "initialization", payload: cp }, h.context);
            h.statuses.length = 0;
            h.nextRun();
            const body = h.comment.body;
            await assert.rejects(h.lock(() => h.start()));
            assert.equal(h.comment.body, body);
            assert.equal(h.statuses.length, 0);
        });
    }
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
    h.checkpoint(published);
    h.context.issue.state = "closed";
    let cp = await invalidateStaleEvidence(h.transport, h.context, { generation: published.generation });
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

for (const reason of ["timeout", "network-failure", "checksum-provenance"])
test(`${reason} observations retain tested and skipped reasons through checkpoint reload`, async () => {
    const h = harness(); await h.start();
    const p = await reserve(h);
    const done = completion(h, p, "untestable");
    if (reason !== "timeout") {
        const cell = done.observation.cells[0];
        cell.sdk = cell.runtime = null;
        cell.artifacts = Object.fromEntries(Object.keys(cell.artifacts).map(name => [name, null]));
        for (const attempt of cell.attempts) {
            attempt.checks = Object.fromEntries(Object.keys(attempt.checks).map(name => [name, false]));
            attempt.checks.network = reason !== "network-failure";
            attempt.execution = { status: "exception", complete: false, exitCode: null, stdout: "", stderr: reason };
        }
        cell.outcome = done.observation.outcome = { schemaVersion: 1, outcome: "untestable", reason };
    }
    const cp = await commitObservation(h.transport, h.context, done);
    const digest = model.fingerprintInput(done.observation);
    assert.equal(cp.state.skips[0].reason, reason);
    assert.equal(cp.state.skips[0].observationDigest, digest);
    assert.equal(cp.state.testedRevisions[0].observationDigest, digest);
    const loaded = (await loadCheckpoint(h.transport, h.context)).checkpoint;
    assert.deepEqual(loaded, cp);
    assert.deepEqual(loaded.observations[digest].data, done.observation);
    assert.equal(loaded.pending, null);
    assert.deepEqual(loaded.state.budget.consumed, { sourceBuilds: 1, workerUnits: 1, reservedMinutes: 110 });
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

test("moving main or stable clears only that endpoint and the interval, retaining historical evidence", async t => {
    for (const changed of ["main", "stable"]) {
        await t.test(changed, async () => {
            const h = harness(); let old = await h.start();
            for (const endpoint of ["stable", "main", "good", null]) {
                const work = endpoint === "stable"
                    ? { kind: "released", target: { sdk: h.context.stableSdk }, endpoint, phase: "stable" }
                    : request(f.sha(endpoint ?? "skipped"), endpoint);
                const p = await reserve(h, old.generation, endpoint ?? "skipped", work);
                old = await commitObservation(h.transport, h.context,
                    completion(h, p, endpoint === "good" ? "pass" : endpoint === null ? "untestable" : "product-failure"));
            }
            old.state.interval = { goodSha: f.sha("good"), badSha: f.sha("main"), gitLog: ["verified ancestry"] };
            old.state.publication = { digest: f.digest("result"), commentId: 789 };
            old.publicationStatus = "published";
            h.checkpoint(old);
            if (changed === "main") h.context.pinnedMainSha = f.sha("new main");
            else h.context.stableSdk.releaseIndexDigest = f.digest("new release metadata");
            const cp = await invalidateStaleEvidence(h.transport, h.context, { generation: old.generation });
            assert.notEqual(cp.state.epoch, old.state.epoch);
            assert.deepEqual(cp.state.endpointEvidence, { ...old.state.endpointEvidence, [changed]: null });
            assert.equal(cp.state.interval, null);
            assert.deepEqual(cp.state.publication, { digest: null, commentId: null });
            assert.equal(cp.publicationStatus, "unpublished");
            for (const field of ["observations", "records", "sequence"]) assert.deepEqual(cp[field], old[field]);
            for (const field of ["testedRevisions", "skips", "budget"]) assert.deepEqual(cp.state[field],
                field === "budget" ? { ...old.state.budget, stableSdk: h.context.stableSdk } : old.state[field]);
            assert.equal(cp.history.length, 1);
            assert.deepEqual(cp.history[0].endpointEvidence, old.state.endpointEvidence);
            assert.deepEqual(cp.history[0].interval, old.state.interval);
            assert.deepEqual(cp.history[0].publication, old.state.publication);
        });
    }
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
            const publication = await loadCheckpoint(h.transport, h.context, { requiredEvidenceOnly: true });
            assert.equal(publication.executionAllowed, false, "publication-only evidence checks cannot authorize execution");
            assert.equal(publication.publicationAllowed, !required);
            assert.equal(publication.warnings.length, required ? 1 : 0);
            assert.deepEqual(publication.checkpoint, loaded.checkpoint);
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
    await h.start();
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
