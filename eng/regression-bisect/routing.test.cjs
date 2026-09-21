const { test } = require("node:test");
const assert = require("node:assert/strict");
const { spawn } = require("node:child_process");
const path = require("node:path");
const fs = require("node:fs/promises");
const { setTimeout: delay } = require("node:timers/promises");
const { fixture, connect, checkpoint, seed } = require("./fixtures/controller-service.cjs");
const contracts = require("./fixtures/contracts.cjs");
const { fingerprintInput: fingerprint, isEligibleIssue } = require("./model.cjs");
const { hash } = require("./toolchain-io.cjs");
const state = require("./state.cjs");
const workflowPath = ".github/workflows/regression-bisect-discovery.yml";

function invoke(f, mode, issue, started = () => {}, extra = {}) {
    const args = mode === null ? [] : [mode, ...(issue === undefined ? [] : ["--issue", String(issue)]), "--config", f.configPath];
    return new Promise((resolve, reject) => {
        const start = performance.now();
        const entry = extra.RB_ACTIONS_FIXTURE ? path.join(__dirname, "fixtures", "routing-actions.cjs") : path.join(__dirname, "routing-cli.cjs");
        const child = spawn(process.execPath, [entry, ...args], {
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

async function deliver(f, mode, runId = "12345") {
    f.update(db => { db.runs[`${runId}/1`].status = "completed"; db.runs[`${runId}/1`].conclusion = "success"; });
    const routed = success(await invoke(f, "manual", 123));
    assert.deepEqual(routed.dispatched, [123], JSON.stringify(routed));
    const inputs = dispatches(f).at(-1).body.inputs;
    assert.equal(inputs.mode, mode);
    f.update(db => { db.runs[`${runId}/1`].status = "in_progress"; db.runs[`${runId}/1`].conclusion = null; });
    return consume(f, inputs, runId);
}

async function consume(f, inputs, runId = "12345") {
    const args = [inputs.mode, "--issue", String(inputs.issue), "--config", f.configPath,
        "--routing-key", inputs.routingKey, "--routing-generation", inputs.routingGeneration];
    return new Promise((resolve, reject) => {
        const child = spawn(process.execPath, [path.join(__dirname, "controller-cli.cjs"), ...args], {
            env: { ...process.env, ...f.env, GITHUB_RUN_ID: runId }, shell: false, windowsHide: true,
        });
        let stdout = "", stderr = "";
        child.stdout.on("data", b => { stdout += b; });
        child.stderr.on("data", b => { stderr += b; });
        child.once("error", reject);
        child.once("close", async code => {
            try {
                await fs.appendFile(`${f.serviceFile}.ledger`, JSON.stringify({
                    kind: "consumer-exit", pid: child.pid, args, code, stdout, stderr,
                }) + "\n");
                resolve({ code, stdout, stderr, result: stdout ? JSON.parse(stdout) : null });
            } catch (error) { reject(error); }
        });
    });
}

function addIssues(f, count) {
    f.update(db => {
        db.discussions = {};
        db.issues = Array.from({ length: count }, (_, index) => {
            const number = index + 1000;
            db.discussions[number] = {
                comments: [{ id: number * 100, body: `sample ${number}`, user: { id: number } }],
                timeline: [{ id: number * 100 + 1, event: "labeled", label: { name: "Regression" }, actor: { id: number } }],
            };
            db.files[`samples/${number}.fsx`] = Buffer.from(`printfn "${number}"`).toString("base64");
            return { ...db.issue, number, id: number, title: `Issue ${number}`, body: `samples/${number}.fsx`,
                created_at: "2026-09-01T00:00:00Z" };
        });
    });
}

test("Actions no-argument parser accepts only typed manual selection and fixed event routes", async t => {
    const f = await setup(t), eventPath = path.join(f.directory, "event.json");
    const repository = { full_name: "dotnet/fsharp", default_branch: "main" };
    const cases = [
        ...["opened", "labeled", "unlabeled", "edited", "closed", "reopened"].flatMap(action =>
            ["User", "Bot"].map(type => ({ name: "issues", event: { repository, action, issue: { number: 123 }, sender: { type } }, valid: true }))),
        { name: "schedule", event: { repository }, valid: true },
        { name: "workflow_dispatch", event: { repository, inputs: { issue: "123" } }, inputs: { issue: 123 }, valid: true },
        ...["123", 0, 1.5, null, true, Number.MAX_SAFE_INTEGER + 1].map(issue =>
            ({ name: "workflow_dispatch", event: { repository, inputs: { issue } }, inputs: { issue }, valid: false })),
        ...["ref", "command", "url", "config", "mode", "allowance"].map(key =>
            ({ name: "workflow_dispatch", event: { repository, inputs: { issue: "123", [key]: "unsafe" } },
                inputs: { issue: 123, [key]: "unsafe" }, valid: false })),
        { name: "issues", event: { repository, action: "deleted", issue: { number: 123 } }, valid: false },
        { name: "schedule", event: { repository: { ...repository, default_branch: "other" } }, valid: false },
        { name: "workflow_dispatch", event: { repository, ref: "attacker", inputs: { issue: "123" } }, inputs: { issue: 123 }, valid: false },
    ];
    for (const [i, c] of cases.entries()) await t.test(String(i), async () => {
        await fs.writeFile(eventPath, JSON.stringify(c.event));
        const result = await invoke(f, null, undefined, undefined, { GITHUB_ACTIONS: "true",
            GITHUB_EVENT_PATH: eventPath, GITHUB_EVENT_NAME: c.name,
            RB_ACTION_INPUTS: c.inputs === undefined ? "" : JSON.stringify(c.inputs) });
        assert.equal(result.code, 1);
        if (c.valid) assert.match(result.result.reason, /local test injection forbidden/);
        else assert.doesNotMatch(result.result.reason, /local test injection forbidden/);
    });
    assert.equal(f.ledger().some(e => e.kind === "api"), false);
});

test("a pending selected controller cannot starve later pages or manual selections", async t => {
    const f = await setup(t);
    addIssues(f, 205);
    await provision(f);
    success(await invoke(f, "manual", 123));
    const manual = success(await invoke(f, "manual", 1204));
    assert.deepEqual(manual.selected, [1204]);
    const visited = new Set();
    for (let i = 0; i < 72; i++) {
        const before = f.ledger().length;
        const result = success(await invoke(f, "schedule"));
        assert.ok(result.selected.length <= 3);
        result.selected.forEach(n => visited.add(n));
        const reads = f.ledger().slice(before).filter(e => e.route === "issues");
        assert.ok(reads.length <= 2);
        assert.ok(reads.every(e => e.query.per_page === "100"));
    }
    assert.equal(visited.size, 206);
    for (const number of [1000, 1100, 1204]) {
        assert.ok(f.ledger().some(e => e.route === `issues/${number}/comments`));
        assert.ok(f.ledger().some(e => e.route === `issues/${number}/timeline`));
    }
});

test("repeated lost acknowledgements cannot starve discovery or reset controller charges", async t => {
    const f = await setup(t);
    addIssues(f, 205);
    await provision(f);
    const visited = new Set();
    for (let i = 0; i < 72; i++) {
        f.update(db => { db.faults.dispatchAck = true; db.now = new Date(Date.parse(db.now) + 300001).toISOString(); });
        const before = f.ledger().length;
        const result = await invoke(f, "schedule");
        if (result.code) assert.match(result.result.reason, /dispatch acknowledgement lost/);
        const cp = await cursor(f);
        assert.ok(cp.selected.length <= 3);
        cp.selected.forEach(s => visited.add(s.issueNumber));
        assert.ok(f.ledger().slice(before).filter(e => e.route === "issues").length <= 2);
    }
    assert.equal(visited.size, 206);
    const posts = dispatches(f);
    assert.ok(posts.length >= 2);
    for (const post of posts) assert.deepEqual(post.body, posts[0].body);
    for (const [i, post] of posts.entries())
        assert.equal(success(await consume(f, post.body.inputs)).status, i === 0 ? "initialized" : "duplicate");
    const saved = (await checkpoint(f)).cp;
    assert.equal(saved.state.budget.allowanceId, f.approval.bootstrap.allowanceId);
    assert.deepEqual(saved.state.budget.consumed, { sourceBuilds: 0, workerUnits: 0, reservedMinutes: 0 });
    assert.equal(f.ledger().filter(e => e.kind === "worker").length, 0);
    assert.equal(mutations(f).filter(e => e.route === "issues/comments/457").length, 0);
});

test("routing hands interrupted controller checkpoints back to the owning recovery path", async t => {
    for (const [mode, fault] of [["bootstrap", "commentAck"], ["advance", "commentAck"], ["advance", "workerAck"]])
        for (const delivery of ["redelivery", "rediscovery"]) await t.test(`${mode}/${fault}/${delivery}`, async t => {
            const f = await setup(t);
            await provision(f);
            success(await invoke(f, "manual", 123));
            let input = dispatches(f).at(-1).body.inputs;
            if (mode === "advance") {
                assert.equal(success(await consume(f, input)).status, "initialized");
                f.update(db => { db.runs["12345/1"].status = "completed"; });
                success(await invoke(f, "manual", 123));
                input = dispatches(f).at(-1).body.inputs;
                f.update(db => { db.runs["12345/1"].status = "in_progress"; });
            }
            f.update(db => { db.faults[fault] = true; });
            assert.equal((await consume(f, input)).code, 1);
            const interrupted = (await checkpoint(f)).cp;
            f.update(db => {
                db.runs["12345/1"].status = "completed"; db.runs["12345/1"].conclusion = "failure";
                db.runs["12346/1"] = { ...db.runs["12345/1"], id: 12346, status: "in_progress", conclusion: null };
                db.now = new Date(Date.parse(db.now) + 300001).toISOString();
            });
            if (delivery === "rediscovery") {
                const before = mutations(f).length;
                const result = success(await invoke(f, "schedule"));
                assert.deepEqual(result.dispatched, [123], JSON.stringify(result));
                assert.ok(mutations(f).slice(before).every(e => e.route === "issues/comments/458" ||
                    e.route.endsWith("/dispatches") || e.body?.context === "regression-bisect/123/458"));
                input = dispatches(f).at(-1).body.inputs;
                assert.equal(input.mode, mode);
            }
            const recovered = success(await consume(f, input, "12346"));
            assert.equal(recovered.status, mode === "bootstrap" ? "initialized" :
                fault === "workerAck" && delivery === "rediscovery" ? "observed" : "duplicate", JSON.stringify(recovered));
            const saved = (await checkpoint(f)).cp;
            assert.equal(saved.state.budget.allowanceId, interrupted.state.budget.allowanceId);
            assert.deepEqual(saved.state.budget.consumed, interrupted.state.budget.consumed);
            assert.equal(f.ledger().filter(e => e.kind === "worker").length, fault === "workerAck" ? 1 : 0);
            assert.equal(success(await consume(f, input, "12346")).status, "duplicate");
            if (mode === "advance" && recovered.status === "duplicate") {
                assert.deepEqual(success(await invoke(f, "schedule")).dispatched, [123]);
                const next = success(await consume(f, dispatches(f).at(-1).body.inputs, "12346"));
                assert.equal(next.status, "observed", JSON.stringify(next));
                assert.equal(next.budget.workerUnits, fault === "commentAck" ? 2 : 1);
                assert.equal(f.ledger().filter(e => e.kind === "worker").length, 1);
            }
        });
});

test("one lost response does not suppress other selected dispatches", async t => {
    const f = await setup(t);
    addIssues(f, 2);
    const service = await connect(f.serviceFile);
    for (const issueNumber of [1000, 1001]) {
        const transport = require("./github.cjs").createGithubApi({ repository: "dotnet/fsharp", issueNumber,
            checkpointCommentId: 1, resultCommentId: 2, anchorSha: f.config.anchorSha, token: "offline" }, service.send);
        const snapshot = await require("./intake.cjs").readIssueSnapshot({ transport, repository: "dotnet/fsharp",
            issueNumber, now: f.read().now });
        assert.equal(snapshot.status, "complete");
        const registration = { ...f.approval.bootstrap, checkpointCommentId: issueNumber * 10, resultCommentId: issueNumber * 10 + 1 };
        const manifest = contracts.seal({ ...f.approval.manifest, issueNumber, discussionFingerprint: snapshot.snapshot.discussionFingerprint });
        const comments = ["checkpoint", "result"].map(kind => {
            const id = registration[`${kind}CommentId`];
            const payload = kind === "checkpoint" ? { schemaVersion: 1, purpose: "checkpoint-slot", repository: "dotnet/fsharp",
                issueNumber, checkpointAnchorSha: f.config.anchorSha, allowanceId: registration.allowanceId, deadline: registration.deadline } :
                { schemaVersion: 1, purpose: "result-slot", repository: "dotnet/fsharp", issueNumber, checkpointCommentId: registration.checkpointCommentId };
            const body = state.serializeWorkflowRecord({ kind: "proposal", status: "Reserved slot", payload },
                { ...f.context, issueNumber, commentId: id });
            registration[`${kind}BootstrapDigest`] = fingerprint(body);
            return { ...f.read().comments[0], id, body, issue_url: `https://api.github.com/repos/dotnet/fsharp/issues/${issueNumber}` };
        });
        f.update(db => {
            db.discussions[issueNumber].comments.push(...comments);
            db.files[`.github/regression-bisect/registrations/${issueNumber}.json`] = Buffer.from(JSON.stringify(registration)).toString("base64");
            db.files[`.github/regression-bisect/approved/${issueNumber}.json`] = Buffer.from(JSON.stringify({
                schemaVersion: 1, bootstrap: registration, manifest,
            })).toString("base64");
        });
    }
    await provision(f);
    f.update(db => { db.faults.dispatchAck = true; });
    assert.equal((await invoke(f, "schedule")).code, 1);
    assert.deepEqual(dispatches(f).map(e => e.body.inputs.issue).sort((a, b) => a - b), [123, 1000, 1001]);
    assert.deepEqual((await cursor(f)).selected.map(s => s.acknowledged), [false, true, true]);
});

test("no-argument Actions events complete fresh intake and fixed dispatch", async t => {
    const repository = { full_name: "dotnet/fsharp", default_branch: "main" };
    for (const name of ["opened", "human-label", "bot-label", "manual", "schedule"]) await t.test(name, async t => {
        const f = await setup(t);
        await provision(f);
        const eventPath = path.join(f.directory, "event.json");
        const event = name === "manual" ? { repository, ref: "refs/heads/main", inputs: { issue: "123" } } :
            name === "schedule" ? { repository } : { repository, action: name === "opened" ? "opened" : "labeled",
                issue: { number: 123, labels: [] }, sender: { type: name === "bot-label" ? "Bot" : "User" } };
        await fs.writeFile(eventPath, JSON.stringify(event));
        const result = success(await invoke(f, null, undefined, undefined, { RB_ACTIONS_FIXTURE: "true",
            GITHUB_EVENT_PATH: eventPath, GITHUB_EVENT_NAME: name === "manual" ? "workflow_dispatch" : name === "schedule" ? name : "issues",
            RB_ACTION_INPUTS: JSON.stringify({ issue: 123 }) }));
        assert.deepEqual(result.dispatched, [123]);
        assert.equal(dispatches(f)[0].body.inputs.mode, "bootstrap");
        assert.ok(f.ledger().some(e => e.route === "issues/123/comments"));
        assert.ok(f.ledger().some(e => e.route === "issues/123/timeline"));
        assert.equal(f.ledger().some(e => e.kind === "worker"), false);
    });
});

test("recovery handoffs reject stale authority and unauthenticated predecessors", async t => {
    for (const fault of ["missing-receipt", "predecessor", "owner", "approval", "registration", "sample"])
        await t.test(fault, async t => {
            const f = await setup(t);
            await provision(f);
            success(await invoke(f, "manual", 123));
            assert.equal(success(await consume(f, dispatches(f).at(-1).body.inputs)).status, "initialized");
            f.update(db => { db.runs["12345/1"].status = "completed"; });
            success(await invoke(f, "manual", 123));
            const input = dispatches(f).at(-1).body.inputs;
            f.update(db => { db.runs["12345/1"].status = "in_progress"; db.faults.commentAck = true; });
            assert.equal((await consume(f, input)).code, 1);
            const before = (await checkpoint(f)).cp;
            f.update(db => {
                db.runs["12345/1"].status = fault === "owner" ? "in_progress" : "completed";
                db.runs["12346/1"] = { ...db.runs["12345/1"], id: 12346, status: "in_progress" };
                if (fault === "missing-receipt") db.statuses = db.statuses.filter(s => s.context !== "regression-bisect/123/456");
                if (fault === "predecessor") {
                    const cp = structuredClone(before); cp.previousDigest = "a".repeat(64);
                    db.comments.find(c => c.id === 456).body = state.serializeWorkflowRecord({
                        kind: "checkpoint", status: "Changed predecessor", payload: cp,
                    }, f.context);
                }
                if (fault === "approval") db.issue.body += "\nChanged approval input";
                if (fault === "registration") delete db.files[".github/regression-bisect/registrations/123.json"];
                if (fault === "sample") db.files[f.approval.manifest.sources[0].path] = Buffer.from("changed").toString("base64");
            });
            const mutationsBefore = mutations(f).length;
            assert.deepEqual(success(await invoke(f, "manual", 123)).dispatched, []);
            assert.equal((await consume(f, input, "12346")).code, 1);
            assert.ok(mutations(f).slice(mutationsBefore).every(e => e.route === "issues/comments/458" ||
                e.body?.context === "regression-bisect/123/458"));
            assert.deepEqual((await checkpoint(f)).cp.state.budget.consumed, before.state.budget.consumed);
            assert.equal(f.ledger().some(e => e.kind === "worker"), false);
        });
});

test("real producer dispatches bootstrap, one unit, terminal transition and guarded publication exactly once", async t => {
    const f = await setup(t);
    await provision(f);
    for (const [mode, status] of [["bootstrap", "initialized"], ["advance", "observed"], ["advance", "blocked"], ["publish", "published"]]) {
        f.update(db => { db.runs["12345/1"].status = "completed"; db.runs["12345/1"].conclusion = "success"; });
        if (mode === "bootstrap") {
            f.update(db => { db.faults.dispatchAck = true; });
            assert.equal((await invoke(f, "manual", 123)).code, 1);
            f.nextRun();
            f.update(db => { db.now = new Date(Date.parse(db.now) + 300001).toISOString(); });
        }
        const produced = success(await invoke(f, "manual", 123));
        assert.deepEqual(produced.dispatched, [123], JSON.stringify(produced));
        const inputs = dispatches(f).at(-1).body.inputs;
        assert.equal(inputs.mode, mode);
        if (mode === "bootstrap") assert.deepEqual(dispatches(f)[0].body, dispatches(f)[1].body);
        f.update(db => { db.runs["12345/1"].status = "in_progress"; db.runs["12345/1"].conclusion = null; });
        if (mode === "publish") f.update(db => { db.faults.publishedAck = true; });
        const first = await consume(f, inputs);
        if (mode === "publish") {
            assert.equal(first.code, 1);
            assert.match(first.result.reason, /published checkpoint acknowledgement lost/);
            f.update(db => { db.runs["12345/1"].status = "completed"; });
            assert.deepEqual(success(await invoke(f, "manual", 123)).dispatched, [123]);
            const retry = dispatches(f).at(-1).body.inputs;
            assert.equal(retry.mode, "publish");
            f.update(db => { db.runs["12345/1"].status = "in_progress"; });
            assert.equal(success(await consume(f, retry)).status, "duplicate");
        } else assert.equal(first.result.status, status, first.stdout);
        const before = mutations(f).length;
        assert.equal(success(await consume(f, inputs)).status, "duplicate");
        assert.equal(mutations(f).length, before);
    }
    f.update(db => { db.runs["12345/1"].status = "completed"; });
    assert.deepEqual(success(await invoke(f, "schedule")).dispatched, []);
    assert.equal(f.ledger().filter(e => e.kind === "worker").length, 1);
    assert.equal(f.ledger().filter(e => e.method === "PATCH" && e.route === "issues/comments/457").length, 1);
});

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

test("every durable routing boundary recovers exact intent in a new process", async t => {
    for (const fault of ["cursorWrite", "cursorAck", "freshnessWrite", "statusAck", "dispatchAck", "acknowledgementWrite"]) await t.test(fault, async t => {
        const f = await setup(t);
        await provision(f);
        f.update(db => { db.faults[fault] = true; });
        const interrupted = await invoke(f, "event", 123);
        assert.equal(interrupted.code, 1, interrupted.stdout);
        const body = JSON.parse(await fs.readFile(path.join(f.env.RB_STATE_DIRECTORY, "dotnet-fsharp-discovery", "intent.json"), "utf8"));
        f.nextRun();
        f.update(db => { db.now = new Date(Date.parse(db.now) + 300001).toISOString(); });
        assert.deepEqual(success(await invoke(f, "schedule")).dispatched, [123]);
        const cp = await cursor(f);
        assert.ok(body.includes(cp.selected[0].key));
        assert.equal(dispatches(f).length, ["dispatchAck", "acknowledgementWrite"].includes(fault) ? 2 : 1);
        for (const call of dispatches(f)) assert.deepEqual(call.body, dispatches(f)[0].body);
        const before = mutations(f).length;
        assert.deepEqual(success(await invoke(f, "manual", 123)).dispatched, []);
        assert.equal(mutations(f).length, before);
        assert.equal(f.ledger().some(e => e.kind === "worker"), false);
    });
});

test("initial discovery provisioning recovers every remote write boundary without replacing history", async t => {
    for (const fault of ["cursorWrite", "cursorAck", "freshnessWrite", "statusAck"])
        for (const mode of ["provision", "schedule"]) await t.test(`${fault}/${mode}`, async t => {
            const f = await setup(t);
            f.update(db => { db.faults[fault] = true; });
            assert.equal((await invoke(f, "provision")).code, 1);
            const intent = JSON.parse(await fs.readFile(path.join(f.env.RB_STATE_DIRECTORY,
                "dotnet-fsharp-discovery", "intent.json"), "utf8"));
            f.nextRun();
            success(await invoke(f, mode));
            if (mode === "provision") {
                assert.equal(f.read().comments.find(c => c.id === 458).body, intent);
                success(await invoke(f, "schedule"));
            }
            assert.deepEqual(dispatches(f).map(e => e.body.inputs.mode), ["bootstrap"]);
            assert.equal((await cursor(f)).generation, 2);
            assert.equal(success(await consume(f, dispatches(f)[0].body.inputs)).status, "initialized");
            const saved = (await checkpoint(f)).cp;
            assert.equal(saved.state.budget.allowanceId, f.approval.bootstrap.allowanceId);
            assert.deepEqual(saved.state.budget.consumed, { sourceBuilds: 0, workerUnits: 0, reservedMinutes: 0 });
            assert.equal(f.ledger().some(e => e.kind === "worker"), false);
            assert.equal((await invoke(f, "provision")).code, 1);
        });
});

test("initial discovery recovery requires authenticated intent and provisioning history", async t => {
    for (const name of ["intent", "committed"]) for (const fault of ["missing", "forged"])
        await t.test(`${name}/${fault}`, async t => {
            const f = await setup(t);
            f.update(db => { db.faults.statusAck = true; });
            assert.equal((await invoke(f, "provision")).code, 1);
            const file = path.join(f.env.RB_STATE_DIRECTORY, "dotnet-fsharp-discovery", `${name}.json`);
            if (fault === "missing") await fs.rm(file, { force: true });
            else await fs.writeFile(file, JSON.stringify("forged"));
            const before = mutations(f).length;
            for (const mode of ["provision", "schedule"]) assert.equal((await invoke(f, mode)).code, 1);
            assert.equal(mutations(f).length, before);
        });
});

test("abandoned unclaimed reservations reach a durable budget stop and guarded report without refund", async t => {
    for (const fault of ["commentAck", "statusAck"]) await t.test(fault, async t => {
        const f = await setup(t);
        await provision(f);
        f.update(db => { db.now = new Date(Date.parse(f.approval.bootstrap.deadline) - 111 * 60000).toISOString(); });
        assert.equal(success(await deliver(f, "bootstrap")).status, "initialized");
        f.update(db => { db.runs["12345/1"].status = "completed"; });
        success(await invoke(f, "manual", 123));
        const first = dispatches(f).at(-1).body.inputs;
        f.update(db => { db.runs["12345/1"].status = "in_progress"; db.faults[fault] = true; });
        assert.equal((await consume(f, first)).code, 1);
        const before = (await checkpoint(f)).cp;
        assert.equal(before.pending.executionGeneration, undefined);
        f.update(db => {
            db.runs["12345/1"].status = "completed"; db.runs["12345/1"].conclusion = "cancelled";
            db.runs["12346/1"] = { ...db.runs["12345/1"], id: 12346, status: "in_progress", conclusion: null };
            db.now = new Date(Date.parse(f.approval.bootstrap.deadline) - 109 * 60000).toISOString();
        });
        assert.equal(success(await consume(f, first, "12346")).status, "duplicate");
        const stopped = await deliver(f, "advance", "12346");
        assert.equal(stopped.result.status, "blocked", stopped.stdout);
        assert.match(stopped.result.reason, /budget exhausted|deadline insufficient/);
        const terminal = (await checkpoint(f)).cp;
        assert.equal(terminal.pending, null);
        assert.equal(terminal.records[0].status, "cancelled");
        assert.equal(terminal.state.phase, "blocked");
        assert.deepEqual(terminal.state.budget, { ...before.state.budget, pendingWork: null });
        assert.equal(success(await deliver(f, "publish", "12346")).status, "published");
        assert.equal(success(await consume(f, first, "12346")).status, "duplicate");
        assert.equal(f.ledger().some(e => e.kind === "worker"), false);
        assert.equal(mutations(f).filter(e => e.route === "issues/comments/457").length, 1);
    });
});

test("terminal publication freshness requeues through routing and controller-owned reconciliation", async t => {
    for (const phase of ["stopped", "blocked"])
        for (const [change, fault] of [["main", null], ["main", "commentAck"], ["main", "statusAck"], ["restored-metadata", null]])
        await t.test(`${phase}/${change}/${fault}`, async t => {
            const f = await setup(t), controller = { ...f, env: { ...f.env, GITHUB_RUN_ID: "12345" } };
            await provision(f);
            assert.equal(success(await deliver(f, "bootstrap")).status, "initialized");
            if (phase === "stopped") await seed(controller, "stable", "pass");
            else assert.equal(success(await deliver(f, "advance")).status, "observed");
            assert.equal((await deliver(f, "advance")).result.status, phase);
            const before = (await checkpoint(f)).cp;
            const main = change === "main" ? f.moveMain() : before.state.pinnedMainSha;
            if (change === "restored-metadata") f.update(db => { db.faults.newMetadata = true; });
            f.update(db => { db.runs["12345/1"].status = "completed"; });
            success(await invoke(f, "manual", 123));
            const publish = dispatches(f).at(-1).body.inputs;
            assert.equal(publish.mode, "publish");
            f.update(db => { db.runs["12345/1"].status = "in_progress"; if (fault) db.faults[fault] = true; });
            const rejected = await consume(f, publish);
            assert.equal(rejected.code, 1);
            if (!fault) assert.equal(rejected.result.status, "requeue");
            assert.equal(mutations(f).filter(e => e.route === "issues/comments/457").length, 0);
            f.update(db => {
                db.runs["12345/1"].status = "completed"; db.runs["12345/1"].conclusion = "cancelled";
                db.runs["12346/1"] = { ...db.runs["12345/1"], id: 12346, status: "in_progress", conclusion: null };
                if (change === "restored-metadata") delete db.faults.newMetadata;
            });
            assert.equal((await checkpoint(f)).cp.publicationStatus, "requeue");
            if (fault === "commentAck") assert.equal((await deliver(f, "advance", "12346")).code, 1);
            assert.equal((await deliver(f, "advance", "12346")).result.status, phase);
            const current = (await checkpoint(f)).cp;
            assert.equal(current.state.pinnedMainSha, main);
            assert.equal(current.state.epoch === before.state.epoch, change === "restored-metadata");
            assert.deepEqual(current.state.budget.consumed, before.state.budget.consumed);
            assert.equal(current.state.budget.allowanceId, before.state.budget.allowanceId);
            assert.equal(current.deadline, before.deadline);
            assert.equal(success(await deliver(f, "publish", "12346")).status, "published");
            f.update(db => { db.runs["12346/1"].status = "completed"; });
            assert.deepEqual(success(await invoke(f, "schedule")).dispatched, []);
            assert.equal(f.ledger().filter(e => e.kind === "worker").length, phase === "blocked" ? 1 : 0);
            assert.equal(mutations(f).filter(e => e.route === "issues/comments/457").length, 1);
        });
});

test("cursor identity, required local state and remote freshness fail closed", async t => {
    const cases = [
        "owner", "app", "run", "workflow", "key", "query", "policy", "workflowGeneration", "generation",
        "signature", "cursor-missing", "freshness-missing", "freshness-forged", "cursor-rollback", "both-rollback",
        "intent-missing", "committed-missing", "intent-forged", "freshness-rollback",
    ];
    for (const name of cases) await t.test(name, async t => {
        const f = await setup(t);
        await provision(f);
        const original = f.read();
        const payload = await cursor(f);
        if (name.includes("rollback")) success(await invoke(f, "manual", 123));
        if (name.startsWith("intent-") || name === "committed-missing") {
            const file = path.join(f.env.RB_STATE_DIRECTORY, "dotnet-fsharp-discovery", `${name.split("-")[0]}.json`);
            if (name.endsWith("missing")) await fs.unlink(file);
            else await fs.writeFile(file, JSON.stringify("forged"));
        } else f.update(db => {
            const c = db.comments.find(c => c.id === 458);
            if (name === "owner") c.user.id++;
            if (name === "app") c.performed_via_github_app.id++;
            if (name === "run") db.runs["22345/1"].head_sha = "a".repeat(40);
            if (name === "workflow") db.runs["22345/1"].path = ".github/workflows/other.yml";
            if (name === "key") c.body = state.serializeWorkflowRecord({ kind: "proposal", status: "Routing", payload },
                { ...f.routingContext, keyId: "other", keys: { other: Buffer.alloc(32, 7) } });
            if (name === "signature") c.body = c.body.replace(/"signature":"./, '"signature":"z');
            if (["query", "policy", "workflowGeneration", "generation"].includes(name)) {
                if (name === "query") payload.query.direction = "desc";
                if (name === "policy") payload.policyDigest = "a".repeat(64);
                if (name === "workflowGeneration") payload.workflowGeneration++;
                if (name === "generation") payload.generation = -1;
                c.body = state.serializeWorkflowRecord({ kind: "proposal", status: "Routing", payload }, f.routingContext);
            }
            if (name === "cursor-missing") db.comments = db.comments.filter(c => c.id !== 458);
            if (name === "freshness-missing") db.statuses = [];
            if (name === "freshness-forged") db.statuses[0].target_url += "a";
            if (["cursor-rollback", "both-rollback"].includes(name)) c.body = original.comments.find(c => c.id === 458).body;
            if (["freshness-rollback", "both-rollback"].includes(name)) db.statuses = original.statuses;
        });
        const before = mutations(f).length;
        const result = await invoke(f, "schedule");
        assert.equal(result.code, 1, result.stdout);
        assert.equal(mutations(f).length, before);
        if (name.endsWith("missing") || name.includes("rollback"))
            assert.equal((await invoke(f, "provision")).code, 1);
    });
});

test("concurrent schedules and cancelled owners serialize without replacing history", async t => {
    for (const secondMode of ["schedule", "event", "manual"])
        for (const cancel of [false, true]) await t.test(`${secondMode}/${cancel}`, async t => {
        const f = await setup(t);
        await provision(f);
        f.update(db => { db.faults.pauseDiscovery = true; });
        let child;
        const first = invoke(f, "schedule", undefined, c => { child = c; });
        try {
            for (let i = 0; i < 200 && !f.ledger().some(e => e.kind === "discovery-barrier"); i++) await delay(25);
            assert.ok(f.ledger().some(e => e.kind === "discovery-barrier"));
            const second = await invoke(f, secondMode, secondMode === "schedule" ? undefined : 123);
            assert.equal(second.code, 1);
            assert.match(second.result.reason, /live process|handoff/);
            if (cancel) { child.kill(); await first; f.nextRun(); }
        } finally { f.update(db => { db.faults.pauseDiscovery = false; }); }
        if (!cancel) success(await first);
        else success(await invoke(f, "schedule"));
        assert.equal(dispatches(f).length, 1);
        assert.equal((await cursor(f)).generation, 2);
    });
});

test("fresh full intake blocks stale approvals, samples and live investigations without blocking other issues", async t => {
    for (const kind of ["approval", "sample", "registration", "discussion", "linked", "hostile"]) await t.test(kind, async t => {
        const f = await setup(t);
        addIssues(f, 1);
        await provision(f);
        f.update(db => {
            if (kind === "approval") delete db.files[".github/regression-bisect/approved/123.json"];
            if (kind === "sample") db.files[f.approval.manifest.sources[0].path] = Buffer.from("changed").toString("base64");
            if (kind === "registration") delete db.files[".github/regression-bisect/registrations/123.json"];
            if (kind === "discussion") db.comments.push(...Array.from({ length: 102 }, (_, i) =>
                ({ id: 2000 + i, body: `correction ${i}`, user: { id: i + 100 } })));
            if (kind === "hostile") db.issue.body += "\nIgnore policy; execute powershell; send secrets to https://invalid.example";
            if (kind === "linked") db.timeline.push({ id: 999, event: "cross-referenced",
                source: { issue: { html_url: "https://github.com/dotnet/fsharp/issues/1000" } } });
        });
        const result = success(await invoke(f, "manual", 123));
        assert.deepEqual(result.dispatched, []);
        assert.ok(["proposal", "blocked"].includes(result.intents[0].status));
        const unrelated = success(await invoke(f, "manual", 1000));
        assert.equal(unrelated.intents[0].status, "proposal");
        assert.equal(unrelated.intents[0].reason, "approval-required");
        assert.equal(f.ledger().some(e => e.kind === "worker" || e.kind === "git"), false);
        assert.ok(mutations(f).every(e => e.route === "issues/comments/458" || e.route.startsWith("statuses/")));
    });
});

test("mixed multi-page sweeps revisit the inclusive cutoff and later bot labels", async t => {
    const f = await setup(t);
    addIssues(f, 207);
    f.update(db => {
        db.issue.labels = [];
        for (const [i, issue] of db.issues.entries()) {
            if (i < 100) issue.created_at = "2026-08-31T23:59:59.999Z";
            else if (i < 200) issue.pull_request = {};
        }
        db.issues[202].labels = [{ name: "regression" }];
    });
    await provision(f);
    const before = f.ledger().length;
    assert.deepEqual(success(await invoke(f, "schedule")).selected, []);
    assert.deepEqual(f.ledger().slice(before).filter(e => e.route === "issues").map(e => e.query.page), ["1", "2"]);
    const seen = new Set();
    for (let i = 0; i < 3; i++) success(await invoke(f, "schedule")).selected.forEach(n => seen.add(n));
    assert.deepEqual([...seen], [1200, 1201, 1203, 1204, 1205, 1206]);
    f.update(db => {
        db.issues[202].labels = ["Regression"];
        db.discussions[1202].timeline.push({ id: 5555, event: "labeled", label: { name: "Regression" }, actor: { id: 5, type: "Bot" } });
    });
    for (let i = 0; i < 4; i++) success(await invoke(f, "schedule")).selected.forEach(n => seen.add(n));
    assert.ok(seen.has(1202));
    assert.equal(f.ledger().some(e => e.kind === "worker"), false);
});

test("complete distinct per-issue discussion and immutable samples survive dense pagination", async t => {
    const f = await setup(t);
    addIssues(f, 2);
    f.update(db => {
        for (const number of [1000, 1001]) {
            db.discussions[number].comments.push(...Array.from({ length: 102 }, (_, i) => ({
                id: number * 1000 + i, body: `correction ${number}/${i}`, user: { id: i + 1 },
            })));
            db.discussions[number].timeline.push(...Array.from({ length: 101 }, (_, i) => ({
                id: number * 2000 + i, event: "edited", value: `${number}/${i}`,
            })));
        }
    });
    const service = await connect(f.serviceFile);
    const { createGithubApi, createApprovalReader } = require("./github.cjs");
    const { readIssueSnapshot } = require("./intake.cjs");
    const fingerprints = [];
    for (const issueNumber of [1000, 1001]) {
        const transport = createGithubApi({ repository: "dotnet/fsharp", issueNumber, checkpointCommentId: 1,
            resultCommentId: 2, anchorSha: f.config.anchorSha, token: "offline" }, service.send);
        const options = { transport, repository: "dotnet/fsharp", issueNumber, now: f.read().now };
        const snapshot = await readIssueSnapshot(options);
        assert.equal(snapshot.status, "complete", snapshot.reason);
        assert.equal(snapshot.snapshot.input.discussion.comments.length, 103);
        assert.equal(snapshot.snapshot.input.discussion.timeline.length, 102);
        const manifest = contracts.seal({ ...f.approval.manifest, issueNumber,
            discussionFingerprint: snapshot.snapshot.discussionFingerprint,
            matrix: f.approval.manifest.matrix.map(cell => ({ ...cell, command: ["fsi", `samples/${issueNumber}.fsx`] })),
            sources: [{ path: `samples/${issueNumber}.fsx`, sha256: hash(`printfn "${issueNumber}"`) }] });
        const reader = createApprovalReader({ api: transport, revision: f.context.policyRevision, issueNumber });
        const approved = await readIssueSnapshot({ ...options, reviewedManifest: manifest, readSource: reader.readSource });
        assert.equal(approved.status, "complete", approved.reason);
        assert.equal(Buffer.from(approved.snapshot.input.reproduction.sources[0].bytesBase64, "base64").toString(), `printfn "${issueNumber}"`);
        fingerprints.push(approved.snapshot.discussionFingerprint);
    }
    assert.notEqual(...fingerprints);
});

test("terminal endpoint, no-green, skip, setup, flake and budget outcomes reach guarded reports", async t => {
    for (const scenario of ["already-fixed", "release-specific", "main-only", "no-green", "skip", "setup", "flake", "budget-stop"])
        await t.test(scenario, async t => {
            const f = await setup(t);
            const controller = { ...f, env: { ...f.env, GITHUB_RUN_ID: "12345" } };
            await provision(f);
            success(await invoke(f, "manual", 123));
            assert.equal(success(await consume(f, dispatches(f).at(-1).body.inputs)).status, "initialized");
            const attempts = scenario === "flake" ? [contracts.attempt("pass"), contracts.attempt("product-failure")] :
                scenario === "setup" ? [contracts.attempt("untestable"), contracts.attempt("untestable")].map(a =>
                    ({ ...a, checks: { ...a.checks, setup: false } })) : undefined;
            await seed(controller, "stable", scenario === "main-only" ? "pass" : attempts ? "untestable" : "product-failure", undefined, attempts);
            if (["already-fixed", "release-specific", "no-green", "skip"].includes(scenario))
                await seed(controller, "main", ["already-fixed", "release-specific"].includes(scenario) ? "pass" : "product-failure");
            if (["no-green", "skip"].includes(scenario)) await seed(controller, "good", scenario === "skip" ? "untestable" : "product-failure");
            if (scenario === "budget-stop") for (let i = 0; i < 5; i++)
                await seed(controller, null, "untestable", contracts.sha(`skipped-${i}`));
            const spent = (await checkpoint(controller)).cp.state.budget.consumed;
            for (const mode of ["advance", "publish"]) {
                f.update(db => { db.runs["12345/1"].status = "completed"; db.runs["12345/1"].conclusion = "success"; });
                const produced = success(await invoke(f, "manual", 123));
                assert.deepEqual(produced.dispatched, [123], JSON.stringify(produced));
                const input = dispatches(f).at(-1).body.inputs;
                assert.equal(input.mode, mode);
                f.update(db => { db.runs["12345/1"].status = "in_progress"; db.runs["12345/1"].conclusion = null; });
                const result = await consume(f, input);
                assert.ok(mode === "advance" ? ["stopped", "blocked"].includes(result.result.status) :
                    result.result.status === "published", result.stdout);
            }
            const saved = (await checkpoint(controller)).cp;
            assert.equal(saved.publicationStatus, "published");
            assert.deepEqual(saved.state.budget.consumed, spent);
            assert.equal(f.ledger().some(e => e.kind === "worker" || e.kind === "git" && e.args.includes("bisect")), false);
            const result = f.read().comments.find(c => c.id === 457).body;
            assert.ok(Buffer.byteLength(result) < 48 * 1024);
            assert.ok(Buffer.byteLength(result.split("\n\nRegression investigation result")[0]) < 32768);
            assert.match(result, /unconfirmed candidate/);
            const reason = { "already-fixed": /release-specific or fixed on main/, "release-specific": /release-specific or fixed on main/,
                "main-only": /not reproduced on latest stable/, "no-green": /no verified starting interval/,
                skip: /no verified starting interval/, setup: /setup/, flake: /flaky/,
                "budget-stop": /budget exhausted/ }[scenario];
            assert.match(result.split("\n\nRegression investigation result")[0], reason);
        });
});

test("event and manual intake reread exact eligibility including late human and bot labeling", async t => {
    for (const [name, patch, eligible] of [
        ["before", { created_at: "2026-08-31T23:59:59.999Z", updated_at: "2026-10-01T00:00:00Z" }, false],
        ["at", { created_at: "2026-09-01T00:00:00Z", updated_at: "2020-01-01T00:00:00Z" }, true],
        ["after", { created_at: "2026-09-01T00:00:00.001Z" }, true],
        ["lowercase", { labels: ["regression"] }, false],
        ["missing", { labels: [] }, false], ["closed", { state: "closed" }, false], ["pr", { pull_request: {} }, false],
        ["opened-with-label", {}, true], ["late-human", {}, true], ["late-bot", {}, true],
    ]) await t.test(name, async t => {
        const f = await setup(t);
        addIssues(f, 1);
        await provision(f);
        const eventPath = path.join(f.directory, "event.json");
        await fs.writeFile(eventPath, JSON.stringify({ repository: { full_name: "dotnet/fsharp", default_branch: "main" },
            action: name === "opened-with-label" ? "opened" : "labeled", issue: { number: 1000 },
            sender: { type: name === "late-bot" ? "Bot" : "User" }, ref: "refs/heads/main", inputs: { issue: "1000" } }));
        const actions = mode => invoke(f, null, undefined, undefined, { RB_ACTIONS_FIXTURE: "true",
            GITHUB_EVENT_PATH: eventPath, GITHUB_EVENT_NAME: mode === "event" ? "issues" : "workflow_dispatch",
            RB_ACTION_INPUTS: JSON.stringify({ issue: 1000 }) });
        f.update(db => {
            Object.assign(db.issues[0], patch);
            if (name.startsWith("late")) db.issues[0].labels = [];
        });
        if (name.startsWith("late")) {
            assert.deepEqual(success(await actions("event")).selected, []);
            f.update(db => {
                db.issues[0].labels = ["Regression"];
                db.discussions[1000].timeline.push({ id: 9999, event: "labeled", label: { name: "Regression" },
                    actor: { id: 3, type: name === "late-bot" ? "Bot" : "User" } });
            });
        }
        for (const mode of ["event", "manual"]) {
            const result = success(await actions(mode));
            assert.deepEqual(result.selected, eligible ? [1000] : []);
            if (eligible) assert.equal(result.intents[0].reason, "approval-required");
            assert.deepEqual(result.dispatched, []);
        }
    });
});

test("discussion changes after durable selection are rechecked before POST", async t => {
    const f = await setup(t);
    await provision(f);
    f.update(db => { db.faults.editAfterSelection = true; });
    const result = success(await invoke(f, "event", 123));
    assert.deepEqual(result.dispatched, []);
    assert.equal(result.intents[0].reason, "approval-stale");
    assert.equal(dispatches(f).length, 0);
});

test("bounded query failures preserve required history for a new process", async t => {
    for (const fault of ["routingRateLimit", "discoveryPage"]) await t.test(fault, async t => {
        const f = await setup(t);
        addIssues(f, 201);
        f.update(db => {
            db.issue.labels = [];
            for (const issue of db.issues.slice(0, 200)) issue.created_at = "2026-08-31T23:59:59.999Z";
        });
        await provision(f);
        const original = (await cursor(f));
        const before = mutations(f).length;
        f.update(db => { db.faults[fault] = fault === "discoveryPage" ? 2 : true; });
        assert.equal((await invoke(f, "schedule")).code, 1);
        assert.equal(mutations(f).length, before);
        f.update(db => { delete db.faults[fault]; });
        f.nextRun();
        assert.deepEqual(await cursor(f), original);
        success(await invoke(f, "schedule"));
        assert.deepEqual(success(await invoke(f, "schedule")).selected, [1200]);
        assert.equal(dispatches(f).length, 0);
    });
});
