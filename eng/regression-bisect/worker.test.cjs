const { test } = require("node:test");
const assert = require("node:assert/strict");
const f = require("./fixtures/contracts.cjs");
const state = require("./state.cjs");
const engine = require("./engine.cjs");
const { fixture } = require("./fixtures/execution.cjs");
const { validateExecutionUnit, runExecutionUnit, readExecutionOutput } = require("./worker.cjs");

test("isolated execution receives only public data after the authenticated durable claim", async () => {
    const { h, deps } = await fixture();
    const work = await engine.advanceInvestigation({ now: h.context.now }, deps);
    assert.equal(work.status, "work", work.reason);
    let launches = 0;
    let observation;
    deps.runIsolated = async (unit, { signal }) => {
        launches++;
        const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
        assert.equal(cp.state.budget.pendingWork.status, "running");
        assert.equal(cp.state.budget.consumed.reservedMinutes, 110);
        assert.equal(unit.buildInstanceId, work.reservation.id);
        assert.equal(unit.epoch, cp.state.epoch);
        assert.equal(unit.deadline, cp.pending.deadline);
        assert.deepEqual(unit.manifest, h.context.manifest);
        assert.equal(signal.aborted, false);
        const text = JSON.stringify(unit);
        for (const forbidden of ["keys", "authorization", "transport", "collectObservation", "commentId",
            h.context.keys.fixture.toString(), h.context.keys.fixture.toString("hex")])
            assert.equal(text.includes(forbidden), false, forbidden);
        assert.deepEqual(JSON.parse(text), unit);
        observation = f.observation("released", "product-failure", h.context.manifest);
        observation.buildInstanceId = unit.buildInstanceId;
        for (const cell of observation.cells)
            for (const artifact of Object.values(cell.artifacts)) artifact.buildInstanceId = unit.buildInstanceId;
        return { outcome: observation.outcome, cells: observation.cells };
    };
    deps.collectObservation = async ({ context }) => {
        assert.equal(context.keys, h.context.keys);
        return { observation, evidence: [] };
    };
    const cp = await engine.executeInvestigationWork(work, deps);
    assert.equal(cp.pending, null);
    assert.equal(launches, 1);
    await assert.rejects(engine.executeInvestigationWork(work, deps), /no longer current/);
    assert.equal(launches, 1);
});

test("isolated launch errors, cancellation and deadline exhaustion keep the charged claim", async t => {
    for (const stage of ["launch", "cancel", "deadline"]) await t.test(stage, async () => {
        const { h, deps } = await fixture();
        const work = await engine.advanceInvestigation({ now: h.context.now }, deps);
        const abort = new AbortController();
        deps.signal = abort.signal;
        deps.runIsolated = async () => {
            if (stage === "launch") throw new Error("container launch failed");
            if (stage === "cancel") abort.abort(new Error("controller cancelled"));
            return {};
        };
        let now = h.context.now;
        const clock = () => new Date(now);
        deps.clock = clock;
        if (stage === "deadline") deps.runIsolated = async () => { now = work.reservation.deadline; return {}; };
        deps.collectObservation = async () => assert.fail("failed launch must not collect");
        await assert.rejects(engine.executeInvestigationWork(work, deps), /launch failed|cancelled|deadline/);
        const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
        assert.equal(cp.state.budget.consumed.reservedMinutes, 110);
        assert.equal(cp.state.budget.pendingWork.status, "running");
        assert.deepEqual(cp.observations, {});
    });
});

test("the worker rejects malformed public units and changed helper policy before any physical work", async t => {
    const { h, deps } = await fixture();
    const work = await engine.advanceInvestigation({ now: h.context.now }, deps);
    let unit;
    deps.runIsolated = async value => { unit = value; throw new Error("capture public input"); };
    deps.collectObservation = async () => assert.fail("no worker result");
    await assert.rejects(engine.executeInvestigationWork(work, deps), /capture public input/);
    assert.equal(validateExecutionUnit(unit), true);
    const cases = {
        repository: u => { u.repository = "foreign/repo"; },
        issue: u => { u.issueNumber++; },
        key: u => { u.keys = { attacker: "key" }; },
        command: u => { u.command = "arbitrary command"; },
        path: u => { u.buildInstanceId = "../outside"; },
        request: u => { u.request.target.sdk = {}; },
        epoch: u => { u.epoch = "marker"; },
        policy: u => { u.policyDigest = f.digest("different policy"); },
        deadline: u => { u.deadline = h.context.now; },
        platform: u => { u.platform.rid = "win-x64"; },
        manifest: u => { u.manifest.matrix[0].command = ["bash", "evil"]; },
    };
    for (const [name, mutate] of Object.entries(cases)) await t.test(name, async () => {
        const changed = structuredClone(unit);
        mutate(changed);
        await assert.rejects(runExecutionUnit(changed, { ...deps, policy: h.context.policy,
            worker: { storageDirectory: "unused" } }), /invalid|mismatch|deadline|manifest|unit|command/);
    });
});

test("data-only collection binds stopped-worker output to the trusted launch and exact execution unit", async t => {
    const { h, deps } = await fixture();
    const work = await engine.advanceInvestigation({ now: h.context.now }, deps);
    let unit;
    deps.runIsolated = async value => { unit = value; throw new Error("capture"); };
    deps.collectObservation = async () => assert.fail("no worker result");
    await assert.rejects(engine.executeInvestigationWork(work, deps), /capture/);
    const { fingerprintInput } = require("./model.cjs");
    const { createHash } = require("node:crypto");
    const launch = { launchId: "a".repeat(64), imageDigest: f.digest("runner image"), unitDigest: fingerprintInput(unit) };
    const payload = {
        schemaVersion: 1, ...launch,
        result: { outcome: { schemaVersion: 1, outcome: "untestable", reason: "setup-failure" }, cells: [] },
    };
    const file = value => {
        const bytes = Buffer.from(JSON.stringify(value));
        return { path: "result.json", bytes, size: bytes.length, sha256: createHash("sha256").update(bytes).digest("hex") };
    };
    assert.deepEqual(readExecutionOutput([file(payload)], unit, launch), payload.result);
    const changes = {
        "worker-selected launch": p => { p.launchId = "b".repeat(64); },
        "forged image": p => { p.imageDigest = "c".repeat(64); },
        "another work, issue or epoch": p => { p.unitDigest = "d".repeat(64); },
        "child verified flag": p => { p.verified = true; },
        "child observation": p => { p.observation = f.observation("released", "pass"); },
        "report target": p => { p.issueNumber = 999; },
        "invalid outcome": p => { p.result.outcome.outcome = "green"; },
        "null result": p => { p.result = null; },
    };
    for (const [name, mutate] of Object.entries(changes)) await t.test(name, () => {
        const changed = structuredClone(payload);
        mutate(changed);
        assert.throws(() => readExecutionOutput([file(changed)], unit, launch), /launch|output|outcome|schema/);
    });
    for (const key of ["issueNumber", "epoch", "buildInstanceId", "environmentFingerprint"]) await t.test(key, () => {
        const changed = structuredClone(unit);
        changed[key] = key === "issueNumber" ? unit.issueNumber + 1 : "e".repeat(64);
        assert.throws(() => readExecutionOutput([file(payload)], changed, launch), /launch|identity/);
    });
    const valid = file(payload);
    const large = structuredClone(payload);
    large.result.cells = [{ stdout: "a".repeat(65536) }];
    assert.deepEqual(readExecutionOutput([file(large)], unit, launch), large.result);
    assert.throws(() => readExecutionOutput([valid, valid], unit, launch), /result file/);
    assert.throws(() => readExecutionOutput([], unit, launch), /result file/);
    assert.throws(() => readExecutionOutput([{ ...valid, sha256: "f".repeat(64) }], unit, launch), /digest/);
    assert.throws(() => readExecutionOutput([{ ...valid, bytes: Buffer.alloc(16 * 1024 * 1024 + 1) }], unit, launch), /size/);
    assert.throws(() => readExecutionOutput([file(payload)], unit, { ...launch, verified: true }), /launch/);
    assert.throws(() => readExecutionOutput([file(payload)], unit, { ...launch, imageDigest: "g".repeat(64) }), /launch/);
});
