const { test } = require("node:test");
const assert = require("node:assert/strict");
const { fixture } = require("./fixtures/execution.cjs");
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
