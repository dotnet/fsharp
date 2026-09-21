const { test } = require("node:test");
const assert = require("node:assert/strict");
const { spawn } = require("node:child_process");
const path = require("node:path");
const fs = require("node:fs/promises");
const { setTimeout: delay } = require("node:timers/promises");
const { fixture, connect } = require("./fixtures/controller-service.cjs");
const { fingerprintInput: fingerprint, isEligibleIssue } = require("./model.cjs");
const { hash } = require("./toolchain-io.cjs");
const state = require("./state.cjs");
const workflowPath = ".github/workflows/regression-bisect-discovery.yml";

function invoke(f, mode, issue, started = () => {}, extra = {}) {
    const args = [mode, ...(issue === undefined ? [] : ["--issue", String(issue)]), "--config", f.configPath];
    return new Promise((resolve, reject) => {
        const start = performance.now();
        const child = spawn(process.execPath, [path.join(__dirname, "routing-cli.cjs"), ...args], {
            env: { ...process.env, ...f.env, ...extra }, shell: false, windowsHide: true,
        });
        let stdout = "", stderr = "";
        child.stdout.on("data", b => { stdout += b; });
        child.stderr.on("data", b => { stderr += b; });
        child.once("error", reject);
        child.once("close", async code => {
            try {
                await fs.appendFile(`${f.serviceFile}.ledger`, JSON.stringify({
                    kind: "routing-exit", pid: child.pid, args, code, durationMs: performance.now() - start, stdout, stderr,
                }) + "\n");
                resolve({ code, stdout, stderr, result: stdout ? JSON.parse(stdout) : null });
            } catch (error) { reject(error); }
        });
        started(child);
    });
}

async function setup(t) {
    const f = await fixture(t);
    const registration = {
        schemaVersion: 1, repository: "dotnet/fsharp", issueNumber: 123, commentId: 458,
        checkpointAnchorSha: f.config.anchorSha, bootstrapDigest: hash("Reviewed discovery slot"),
        provisionDeadline: "2026-10-01T00:00:00Z", workflowPath, workflowGeneration: 1,
    };
    f.env.RB_DISPATCH_TOKEN = "offline-separate-dispatch-token";
    f.update(db => {
        db.runs["22345/1"] = { ...db.runs["12345/1"], id: 22345, path: workflowPath };
        db.issues = [db.issue];
        db.comments.push({ ...db.comments[0], id: 458, body: "Reviewed discovery slot" });
        db.files[".github/regression-bisect/discovery.json"] = Buffer.from(JSON.stringify(registration)).toString("base64");
        db.files[workflowPath] = Buffer.from("fixture discovery workflow").toString("base64");
    });
    f.env.GITHUB_RUN_ID = "22345";
    f.registration = registration;
    f.routingContext = { ...f.context, workflowPath, commentId: 458, owner: { runId: "22345", attempt: 1 } };
    return f;
}

async function cursor(f) {
    const api = await connect(f.serviceFile);
    const transport = require("./github.cjs").createGithubApi({
        repository: "dotnet/fsharp", issueNumber: 123, checkpointCommentId: 458,
        resultCommentId: 457, anchorSha: f.config.anchorSha, token: "offline",
    }, api.send);
    const record = await state.authenticateDecodedRecord(transport, f.routingContext, await transport.getComment(458));
    assert.ok(record);
    return record.payload;
}

function success(result) {
    assert.equal(result.code, 0, result.stderr || result.stdout);
    return result.result;
}
async function provision(f) { return success(await invoke(f, "provision")); }
const dispatches = f => f.ledger().filter(e => e.kind === "api" && e.route.endsWith("/dispatches"));
const mutations = f => f.ledger().filter(e => e.kind === "api" && e.method !== "GET");

test("eligibility uses the inclusive creation cutoff, never update time", async t => {
    for (const [created_at, eligible] of [
        ["2026-08-31T23:59:59.999Z", false], ["2026-09-01T00:00:00Z", true],
        ["2026-09-01T00:00:00.001Z", true], ["2026-09-03T12:00:00+02:00", true],
        ["2026-09-01T00:00:00", false], ["2026-09-31T00:00:00Z", false], ["invalid", false],
    ]) for (const updated_at of ["2020-01-01T00:00:00Z", "2026-10-01T00:00:00Z", "invalid"])
        await t.test(`${created_at}/${updated_at}`, () => {
            const issue = { state: "open", labels: ["Regression"], created_at, updated_at };
            assert.equal(isEligibleIssue(issue), eligible);
            for (const patch of [{ pull_request: {} }, { state: "closed" }, { labels: [] },
                { labels: ["regression"] }, { labels: ["Regression "] }, { labels: ["NotRegression"] }])
                assert.equal(isEligibleIssue({ ...issue, ...patch }), false);
        });
});

test("fresh-process provisioning and all routes commit the same fixed bootstrap dispatch", async t => {
    for (const mode of ["event", "manual", "schedule"]) await t.test(mode, async t => {
        const f = await setup(t);
        await provision(f);
        const result = success(await invoke(f, mode, mode === "schedule" ? undefined : 123));
        assert.deepEqual(result.selected, [123]);
        assert.deepEqual(result.dispatched, [123]);
        const cp = await cursor(f);
        assert.equal(cp.generation, 2);
        assert.equal(cp.selected[0].issueNumber, 123);
        assert.equal(cp.selected[0].status, "pending");
        const calls = dispatches(f);
        assert.equal(calls.length, 1);
        assert.equal(calls[0].route, "actions/workflows/regression-bisect-controller.yml/dispatches");
        assert.deepEqual(calls[0].body, { ref: "main", inputs: {
            issue: 123, mode: "bootstrap", routingKey: cp.selected[0].key, routingGeneration: "-1",
        } });
        const writes = mutations(f);
        assert.deepEqual(writes.map(e => e.method), ["PATCH", "POST", "PATCH", "POST", "POST", "PATCH", "POST"]);
        assert.equal(f.ledger().some(e => e.kind === "worker"), false);
    });
});

test("unlabeled, closed and PR fresh reads cannot dispatch or publish", async t => {
    for (const patch of [{ labels: [] }, { state: "closed" }, { pull_request: {} }]) await t.test(JSON.stringify(patch), async t => {
        const f = await setup(t);
        await provision(f);
        f.update(db => { Object.assign(db.issue, patch); });
        const before = mutations(f).length;
        const result = success(await invoke(f, "event", 123));
        assert.deepEqual(result.selected, []);
        assert.equal(dispatches(f).length, 0);
        assert.equal(mutations(f).length, before);
    });
});

test("lost dispatch response retains committed selection across new processes and retries the identical key", async t => {
    const f = await setup(t);
    await provision(f);
    f.update(db => { db.faults.dispatchAck = true; });
    const failed = await invoke(f, "event", 123);
    assert.equal(failed.code, 1, failed.stdout);
    const first = await cursor(f);
    assert.equal(first.generation, 1);
    assert.equal(first.selected[0].status, "pending");
    f.nextRun();
    f.update(db => { db.now = new Date(Date.parse(db.now) + 300001).toISOString(); });
    success(await invoke(f, "schedule"));
    assert.equal(dispatches(f).length, 2);
    assert.deepEqual(dispatches(f)[0].body, dispatches(f)[1].body);
    assert.equal((await cursor(f)).selected[0].key, first.selected[0].key);
});

test("a missing cursor is not a cache miss and cannot reset discovery", async t => {
    const f = await setup(t);
    await provision(f);
    f.update(db => { db.comments = db.comments.filter(c => c.id !== 458); });
    for (const mode of ["schedule", "provision"]) {
        const result = await invoke(f, mode);
        assert.equal(result.code, 1, result.stdout);
    }
    assert.equal(dispatches(f).length, 0);
});
