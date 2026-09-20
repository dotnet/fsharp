const { test } = require("node:test");
const assert = require("node:assert/strict");
const { fixture, bootstrapFixture } = require("./fixtures/execution.cjs");
const f = require("./fixtures/contracts.cjs");
const state = require("./state.cjs");
const { runController } = require("./controller.cjs");

const input = { repository: "dotnet/fsharp", issueNumber: 123 };

async function controller(outcome = "pass") {
    const { h, deps } = await fixture();
    let launches = 0, observation, lockDepth = 0;
    deps.withLock = action => h.lock(async () => {
        lockDepth++;
        try { return await action(); } finally { lockDepth--; }
    });
    deps.runIsolated = async unit => {
        assert.equal(lockDepth, 1);
        launches++;
        observation = f.observation("released", outcome, h.context.manifest);
        observation.buildInstanceId = unit.buildInstanceId;
        observation.provenance.runId = h.context.owner.runId;
        for (const cell of observation.cells)
            for (const artifact of Object.values(cell.artifacts)) artifact.buildInstanceId = unit.buildInstanceId;
        return { outcome: observation.outcome, cells: observation.cells };
    };
    deps.collectObservation = async () => {
        assert.equal(lockDepth, 1);
        return { observation, evidence: [] };
    };
    return { h, deps, launches: () => launches };
}

test("one controller invocation commits one unit without selecting or executing a second", async t => {
    for (const outcome of ["pass", "product-failure", "untestable"]) await t.test(outcome, async () => {
        const { h, deps, launches } = await controller(outcome);
        const result = await runController(input, deps);
        assert.equal(result.status, "observed", result.reason);
        assert.equal(result.outcome.outcome, outcome);
        assert.equal(launches(), 1);
        const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
        assert.equal(cp.sequence, 1);
        assert.equal(cp.pending, null);
        assert.equal(cp.state.budget.consumed.reservedMinutes, 110);
        assert.equal(result.generation, cp.generation);
    });
});

test("typed entry rejects alternate destinations, commands, refs and allowance controls before mutation", async t => {
    for (const change of [
        { repository: "attacker/fsharp" }, { issueNumber: "123" }, { issueNumber: 124 }, { issueNumber: -1 },
        { ref: "attacker" }, { command: "echo unsafe" }, { freshAllowance: true }, { epoch: "malformed" },
        { markdown: "publish me" }, { workId: "../work" },
    ]) await t.test(JSON.stringify(change), async () => {
        const { h, deps, launches } = await controller();
        const before = h.comment.body;
        const result = await runController({ ...input, ...change }, deps);
        assert.equal(result.status, "blocked");
        assert.equal(h.comment.body, before);
        assert.equal(launches(), 0);
    });
});

test("a controller without isolation, collector or whole-issue lock cannot reserve", async t => {
    for (const key of ["runIsolated", "collectObservation", "withLock"]) await t.test(key, async () => {
        const { h, deps, launches } = await controller();
        delete deps[key];
        const before = h.comment.body;
        assert.equal((await runController(input, deps)).status, "blocked");
        assert.equal(h.comment.body, before);
        assert.equal(launches(), 0);
    });
});

test("concurrent events serialize; a stable pass stops the second event without a new reservation", async () => {
    const { h, deps, launches } = await controller();
    const results = await Promise.all([runController(input, deps), runController(input, deps)]);
    assert.deepEqual(results.map(r => r.status), ["observed", "stopped"]);
    assert.equal(results[1].reason, "not reproduced on latest stable");
    assert.equal(launches(), 1);
    assert.equal((await state.loadCheckpoint(h.transport, h.context)).checkpoint.sequence, 1);
});

test("a lost result acknowledgement does not repeat a committed worker on the next run", async () => {
    const { h, deps, launches } = await controller();
    const update = h.transport.updateComment;
    h.transport.updateComment = async (...args) => {
        const value = await update(...args);
        if (launches() === 1) throw new Error("result acknowledgement lost");
        return value;
    };
    const first = await runController(input, deps);
    assert.equal(first.status, "blocked");
    assert.match(first.reason, /acknowledgement lost/);
    h.transport.updateComment = update;
    h.nextRun();
    assert.equal((await runController(input, deps)).status, "stopped");
    assert.equal(launches(), 1);
    assert.equal((await state.loadCheckpoint(h.transport, h.context)).checkpoint.sequence, 1);
});

test("missing, rolled back or corrupt state never initializes a replacement allowance", async t => {
    for (const mutation of ["deleted", "rollback", "key", "malformed"]) await t.test(mutation, async () => {
        const { h, deps, launches } = await controller();
        if (mutation === "deleted") h.transport.getComment = async () => { throw new Error("404 missing checkpoint"); };
        if (mutation === "rollback") h.comment.body = "unused placeholder";
        if (mutation === "key") h.context.keys = {};
        if (mutation === "malformed") h.comment.body = "invalid";
        const before = h.comment.body;
        assert.equal((await runController(input, deps)).status, "blocked");
        assert.equal(h.comment.body, before);
        assert.equal(launches(), 0);
    });
});

test("reviewed bootstrap initializes once, then concurrent and fresh-owner events retain the allowance", async t => {
    const { bootstrapController } = require("./controller.cjs");
    const { h, deps, approval, mutations } = await bootstrapFixture();
    const results = await Promise.all([bootstrapController(input, deps), bootstrapController(input, deps)]);
    assert.deepEqual(results.map(r => r.status), ["initialized", "loaded"]);
    assert.equal(mutations.length, 2);
    h.nextRun();
    const result = await bootstrapController(input, deps);
    assert.equal(result.status, "loaded", result.reason);
    const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
    assert.equal(cp.state.budget.allowanceId, approval.bootstrap.allowanceId);
    assert.equal(cp.deadline, approval.bootstrap.deadline);
    assert.equal(cp.generation, 0);
    assert.equal(mutations.length, 2);
    t.diagnostic(JSON.stringify({ mutations, budget: cp.state.budget.consumed }));
});

test("bootstrap refuses missing approval, stale discussion, unsafe registration and incomplete history before mutation", async t => {
    const changes = {
        "dispatch allowance": f => { f.input = { ...input, freshAllowance: true }; },
        "missing reviewed reader": f => { delete f.deps.approval.readApproval; },
        "JSON instead of raw approval bytes": f => { f.deps.approval.readApproval = async () => f.approval; },
        "different manifest": f => { f.approval.manifest.discussionFingerprint = f.digest("different"); },
        "different checkpoint": f => { f.approval.bootstrap.checkpointCommentId++; },
        "different anchor": f => { f.approval.bootstrap.checkpointAnchorSha = "a".repeat(40); },
        "different slot digest": f => { f.approval.bootstrap.checkpointBootstrapDigest = "a".repeat(64); },
        "result equals checkpoint": f => { f.approval.bootstrap.resultCommentId = f.h.context.commentId; },
        "different result body": f => { f.result.body = "copied bot marker"; },
        "missing checkpoint": f => { f.h.transport.getComment = async () => { throw new Error("404 deleted"); }; },
        "deleted result": f => {
            const read = f.h.transport.getComment;
            f.h.transport.getComment = id => id === f.result.id ? Promise.reject(new Error("404 deleted")) : read(id);
        },
        "closed issue": f => { f.data.issue.state = "closed"; },
        "case-sensitive label": f => { f.data.issue.labels = ["regression"]; },
        "later correction": f => { f.data.issue.body += " Correction"; },
        "source digest mismatch": f => { f.deps.approval.readSource = async () => Buffer.from("changed source"); },
        "missing key": f => { f.h.context.keys = {}; },
        "active prior slot owner": f => {
            const prior = f.h.runs.get("12345/1");
            f.h.nextRun();
            prior.status = "in_progress";
        },
        "deadline passes during intake": f => {
            const read = f.deps.approval.readSource;
            f.deps.approval.readSource = async source => {
                const bytes = await read(source);
                f.h.context.now = f.approval.bootstrap.deadline;
                return bytes;
            };
        },
        "expired allowance": f => { f.approval.bootstrap.deadline = f.h.context.now; },
        "incomplete timeline": f => {
            const request = f.h.transport.request;
            f.h.transport.request = (route, args) => route.endsWith("/timeline")
                ? { data: [], headers: { link: '<https://api.github.com/next>; rel="next"' } } : request(route, args);
        },
        "active linked work": f => {
            f.data.timeline.push({ id: 12, event: "cross-referenced",
                source: { issue: { html_url: "https://github.com/dotnet/fsharp/pull/777" } } });
            const request = f.h.transport.request;
            f.h.transport.request = (route, args) => route.endsWith("/pulls/{pull_number}") ? {
                data: { number: 777, state: "open", merged_at: null, updated_at: f.h.context.now },
            } : request(route, args);
        },
        "other issue allowance tombstone": f => {
            f.h.statuses.push({ context: "regression-bisect/123/999", creator: f.h.context.actor, id: 100 });
        },
        "rate limited history": f => {
            f.h.transport.request = async () => { throw new Error("429 rate limited"); };
        },
        "truncated history": f => {
            const read = f.h.transport.request;
            f.h.transport.request = (route, args) => route.includes("/statuses") && route.startsWith("GET")
                ? { data: [], headers: { link: '<https://api.github.com/next>; rel="next"' } } : read(route, args);
        },
    };
    for (const [name, change] of Object.entries(changes)) await t.test(name, async () => {
        const fixture = await bootstrapFixture();
        fixture.digest = f.digest;
        change(fixture);
        const before = fixture.h.comment.body;
        const result = await require("./controller.cjs").bootstrapController(fixture.input ?? input, fixture.deps);
        assert.equal(result.status, "blocked", JSON.stringify(result));
        assert.ok(result.reason);
        assert.equal(fixture.h.comment.body, before);
        assert.deepEqual(fixture.mutations, []);
    });
});

test("bootstrap recovers interruption at each write without replacing its original signed checkpoint", async t => {
    for (const point of ["comment-before", "comment-after", "status-before", "status-after"]) await t.test(point, async () => {
        const { h, deps, mutations } = await bootstrapFixture();
        const { bootstrapController } = require("./controller.cjs");
        const method = point.startsWith("comment") ? "updateComment" : "request";
        const originalMethod = h.transport[method];
        let interrupted = false;
        h.transport[method] = async (...args) => {
            if (interrupted || method === "request" && !args[0].startsWith("POST")) return originalMethod(...args);
            interrupted = true;
            if (point.endsWith("after")) await originalMethod(...args);
            throw new Error(`cancelled at ${point}`);
        };
        const first = await bootstrapController(input, deps);
        assert.equal(first.status, "blocked");
        assert.match(first.reason, /cancelled/);
        const committedBody = h.comment.body;
        h.transport[method] = originalMethod;
        h.nextRun();
        const recovered = await bootstrapController(input, deps);
        assert.ok(["initialized", "loaded"].includes(recovered.status), recovered.reason);
        const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
        assert.equal(cp.sequence, 0);
        assert.equal(h.statuses.length, 1);
        if (point !== "comment-before") assert.equal(h.comment.body, committedBody);
        t.diagnostic(JSON.stringify({ point, mutations, budget: cp.state.budget.consumed }));
    });
});

test("bootstrap will not replenish charged state, replace deleted slots or adopt a rollback", async t => {
    for (const action of ["resume", "delete", "rollback", "new-id"]) await t.test(action, async () => {
        const { h, deps, approval, original, mutations } = await bootstrapFixture();
        const { bootstrapController } = require("./controller.cjs");
        assert.equal((await bootstrapController(input, deps)).status, "initialized");
        await state.reserveWork(h.transport, h.context, {
            generation: 0, attemptId: "charged-before-restart",
            request: { kind: "released", target: { sdk: h.context.stableSdk }, endpoint: "stable", phase: "stable" },
        });
        const charged = h.comment.body;
        if (action === "delete") h.transport.getComment = async () => { throw new Error("404 missing"); };
        if (action === "rollback") h.comment.body = original;
        if (action === "new-id") approval.bootstrap.checkpointCommentId++;
        h.nextRun();
        const count = mutations.length;
        const result = await bootstrapController(input, deps);
        assert.equal(result.status, action === "resume" ? "loaded" : "blocked", result.reason);
        assert.equal(mutations.length, count);
        if (action === "resume") {
            assert.equal(h.comment.body, charged);
            assert.equal(result.budget.reservedMinutes, 110);
        }
    });
});

test("bootstrap fresh-process recovery preserves charged state and repairs only the interrupted receipt", async t => {
    const { spawnSync } = require("node:child_process");
    for (const stage of ["initial-checkpoint", "charged"]) await t.test(stage, async () => {
        const { h, deps } = await bootstrapFixture();
        const { bootstrapController } = require("./controller.cjs");
        assert.equal((await bootstrapController(input, deps)).status, "initialized");
        if (stage === "initial-checkpoint") h.statuses.length = 0;
        else await state.reserveWork(h.transport, h.context, {
            generation: 0, attemptId: "fresh-process-work",
            request: { kind: "released", target: { sdk: h.context.stableSdk }, endpoint: "stable", phase: "stable" },
        });
        const before = h.comment.body;
        const child = spawnSync(process.execPath, ["-e", `
            const fs = require("node:fs");
            (async () => {
                const saved = JSON.parse(fs.readFileSync(0, "utf8"));
                const { h, deps, mutations } = await require("./fixtures/execution.cjs").bootstrapFixture();
                h.comment = saved.comment;
                h.statuses.push(...saved.statuses);
                h.nextRun();
                const result = await require("./controller.cjs").bootstrapController(
                    { repository: "dotnet/fsharp", issueNumber: 123 }, deps);
                console.log(JSON.stringify({ result, body: h.comment.body, mutations, statuses: h.statuses.length }));
            })().catch(error => { console.error(error); process.exitCode = 1; });
        `], { cwd: __dirname, input: JSON.stringify({ comment: h.comment, statuses: h.statuses }),
            encoding: "utf8", timeout: 30000 });
        assert.equal(child.status, 0, child.error?.message ?? child.stderr);
        const after = JSON.parse(child.stdout);
        assert.equal(after.result.status, stage === "charged" ? "loaded" : "initialized", after.result.reason);
        assert.equal(after.body, before);
        assert.equal(after.result.budget.reservedMinutes, stage === "charged" ? 110 : 0);
        assert.deepEqual(after.mutations.map(m => m.operation), stage === "charged" ? [] : ["status"]);
        t.diagnostic(JSON.stringify({ stage, ...after, body: f.digest(after.body) }));
    });
});

test("bootstrap receipt recovery preserves the original checkpoint despite changed current eligibility or expired time", async t => {
    for (const change of ["closed", "deadline", "discussion"]) await t.test(change, async () => {
        const { h, deps, data } = await bootstrapFixture();
        const { bootstrapController } = require("./controller.cjs");
        assert.equal((await bootstrapController(input, deps)).status, "initialized");
        const original = h.comment.body;
        h.statuses.length = 0;
        h.nextRun();
        if (change === "closed") data.issue.state = h.context.issue.state = "closed";
        if (change === "deadline") h.context.now = "2026-11-01T00:00:00Z";
        if (change === "discussion") {
            data.issue.body += " Correction";
            h.context.issueFingerprint = f.digest("changed discussion");
        }
        const result = await bootstrapController(input, deps);
        assert.equal(result.status, "initialized", result.reason);
        assert.equal(h.comment.body, original);
        assert.equal(h.statuses.length, 1);
        assert.equal((await state.loadCheckpoint(h.transport, h.context)).executionAllowed, false);
    });
});
