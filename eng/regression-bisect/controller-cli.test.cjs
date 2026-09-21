const { test, describe } = require("node:test");
const assert = require("node:assert/strict");
const { spawn } = require("node:child_process");
const path = require("node:path");
const fs = require("node:fs/promises");
const { setTimeout: delay } = require("node:timers/promises");
const { fixture, connect, checkpoint, seed } = require("./fixtures/controller-service.cjs");
const { fingerprintInput: fingerprint } = require("./model.cjs");
const contracts = require("./fixtures/contracts.cjs");
const state = require("./state.cjs");
const { createGithubApi } = require("./github.cjs");
const { image } = require("./container.cjs");
const { argumentsFor } = require("./controller-cli.cjs");
const { dispatchKey } = require("./routing.cjs");
const YAML = require("yaml");

const cli = path.join(__dirname, "controller-cli.cjs");
function invoke(args, env = {}, started = () => {}) {
    return new Promise((resolve, reject) => {
        const start = performance.now();
        const preload = env.RB_LOCAL_TEST_FILE ? ["--require", path.join(__dirname, "fixtures", "controller-storage-faults.cjs")] : [];
        const child = spawn(process.execPath, [...preload, cli, ...args], {
            env: { ...process.env, ...env }, shell: false, windowsHide: true,
        });
        let stdout = "", stderr = "";
        child.stdout.on("data", bytes => { stdout += bytes; });
        child.stderr.on("data", bytes => { stderr += bytes; });
        child.once("error", reject);
        child.once("close", async code => {
            try {
                if (env.RB_LOCAL_TEST_FILE) await fs.appendFile(`${env.RB_LOCAL_TEST_FILE}.ledger`, JSON.stringify({
                    kind: "cli-exit", pid: child.pid, args, code, durationMs: performance.now() - start, stdout, stderr,
                }) + "\n");
                resolve({ code, stdout, stderr });
            } catch (error) { reject(error); }
        });
        started(child);
    });
}

describe("file-backed controller CLI", { concurrency: 2 }, () => {
test("the executable advertises only the four deterministic modes", async () => {
    const result = await invoke(["--help"]);
    assert.equal(result.code, 0, result.stderr);
    assert.match(result.stdout, /bootstrap.*advance.*publish.*provision/);
});

test("invalid CLI inputs block before configuration, API, worker or Git access", async t => {
    for (const args of [
        [], ["advance", "--issue", "0", "--config", "missing.json"],
        ["advance", "--issue", "123;echo unsafe", "--config", "missing.json"],
        ["advance", "--issue", "1.5", "--config", "missing.json"],
        ["advance", "--issue", "123", "--config", "missing.json", "--ref", "attacker"],
        ["advance", "--issue", "123", "--config", "missing.json", "--repository", "attacker/repo"],
        ["advance", "--issue", "123", "--config", "missing.json", "--worker", "unsafe.cjs"],
        ["advance", "--issue", "123", "--config", "missing.json", "--issue", "124"],
    ]) await t.test(JSON.stringify(args), async () => {
        const result = await invoke(args);
        assert.equal(result.code, 1);
        const output = JSON.parse(result.stdout);
        assert.equal(output.status, "blocked");
        assert.match(output.reason, /usage|issue|argument/i);
    });
});

const run = (f, mode, env = {}) => invoke([mode, "--issue", "123", "--config", f.configPath], { ...f.env, ...env });
const outcome = result => { assert.equal(result.stderr, ""); return JSON.parse(result.stdout); };
function noExecution(f) {
    const ledger = f.ledger();
    assert.equal(ledger.filter(r => r.kind === "worker").length, 0);
    assert.equal(ledger.filter(r => r.kind === "git" && r.args.includes("bisect")).length, 0);
}

test("typed Actions inputs normalize only to fixed deployment arguments", async t => {
    const f = await fixture(t);
    for (const mode of ["bootstrap", "advance", "publish"]) for (const routing of [
        {}, { routingKey: "", routingGeneration: "" },
    ]) await t.test(`${mode} ${JSON.stringify(routing)}`, async () => {
        const env = { ...f.env, GITHUB_ACTIONS: "true", RB_ACTION_INPUTS: JSON.stringify({ issue: 123, mode, ...routing }) };
        assert.deepEqual(argumentsFor([], env), { mode, issueNumber: 123,
            configPath: path.resolve(__dirname, "..", "..", ".github", "regression-bisect", "deployment.json") });
        const result = await invoke([], env);
        assert.equal(result.code, 1);
        assert.match(outcome(result).reason, /local test injection forbidden/);
    });
    assert.deepEqual(f.ledger().filter(e => e.kind !== "cli-exit"), []);
});

test("routing Actions inputs reject partial, malformed and mode-inconsistent identities before service access", async t => {
    const f = await fixture(t);
    const routingKey = contracts.digest("dispatch");
    for (const fields of [
        { routingKey }, { routingGeneration: "0" }, { routingKey: "", routingGeneration: "0" },
        ...[null, 0, "", "-1", "-0", "01", "1.5", "1e2", "9007199254740992", "0;unsafe"].map(
            routingGeneration => ({ routingKey, routingGeneration })),
        { routingKey: routingKey.toUpperCase(), routingGeneration: "0" },
        { routingKey: `${routingKey}0`, routingGeneration: "0" },
        { routingKey, routingGeneration: "0", ref: "attacker" },
    ]) await t.test(JSON.stringify(fields), async () => {
        const result = await invoke([], { ...f.env, GITHUB_ACTIONS: "true",
            RB_ACTION_INPUTS: JSON.stringify({ issue: 123, mode: "advance", ...fields }) });
        assert.equal(result.code, 1);
        assert.match(outcome(result).reason, /invalid typed/);
    });
    for (const mode of ["bootstrap", "advance", "publish"]) {
        const routingGeneration = mode === "bootstrap" ? "-1" : "0";
        const parsed = argumentsFor([], { GITHUB_ACTIONS: "true",
            RB_ACTION_INPUTS: JSON.stringify({ issue: 123, mode, routingKey, routingGeneration }) });
        assert.equal(parsed.routingKey, routingKey);
        assert.equal(parsed.routingGeneration, Number(routingGeneration));
    }
    assert.deepEqual(f.ledger().filter(e => e.kind !== "cli-exit"), []);
});

test("invalid Actions payloads block before configuration or service access", async t => {
    const f = await fixture(t);
    const valid = { issue: 123, mode: "advance" };
    const cases = [
        ...[null, [], true, 123, "advance", {}, { issue: 123 }, { mode: "advance" },
            ...["123", 0, -1, 1.5, Number.MAX_SAFE_INTEGER + 1, "123;echo unsafe"].map(issue => ({ ...valid, issue })),
            ...["provision", "ADVANCE", "advance;echo unsafe", null].map(mode => ({ ...valid, mode })),
            ...["repository", "ref", "config", "worker", "url", "allowance", "slot"].map(key => ({ ...valid, [key]: "unsafe" })),
        ].map(value => JSON.stringify(value)),
        '{"issue":123,"mode":"advance"', "SECRET-CANARY-NOT-JSON",
    ];
    for (const payload of cases) await t.test(payload, async () => {
        const result = await invoke([], { ...f.env, GITHUB_ACTIONS: "true", RB_ACTION_INPUTS: payload });
        assert.equal(result.code, 1);
        assert.deepEqual(outcome(result), { status: "blocked", reason: "invalid typed Actions input" });
    });
    for (const [actions, payload] of [["true", ""], ["", JSON.stringify(valid)], ["false", JSON.stringify(valid)]]) {
        const result = await invoke([], { ...f.env, GITHUB_ACTIONS: actions, RB_ACTION_INPUTS: payload });
        assert.equal(result.code, 1);
        assert.match(outcome(result).reason, /Usage:/);
    }
    assert.deepEqual(f.ledger().filter(e => e.kind !== "cli-exit"), []);
});

test("fixture writers publish complete snapshots without modifying previous file identities", async t => {
    for (const writer of ["operator", "api"]) await t.test(writer, async t => {
        const f = await fixture(t);
        const before = await fs.readFile(f.serviceFile, "utf8");
        const snapshot = path.join(f.directory, "previous.json");
        await fs.link(f.serviceFile, snapshot);
        if (writer === "operator") f.update(db => { db.faults.pauseRuntime = true; });
        else assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
        assert.notEqual(await fs.readFile(f.serviceFile, "utf8"), before);
        assert.equal(contracts.digest(await fs.readFile(snapshot)), contracts.digest(before),
            "readers of the previous file identity must retain the complete snapshot");
        assert.deepEqual((await fs.readdir(f.directory)).filter(name => name.endsWith(".tmp")), []);
        noExecution(f);
    });
});

test("a transient Windows fixture reader does not fail an actual CLI write", { skip: process.platform !== "win32" }, async t => {
    const f = await fixture(t);
    const reader = await fs.open(f.serviceFile, "r");
    const first = run(f, "bootstrap");
    try {
        for (let i = 0; i < 400 && !f.ledger().some(e => e.kind === "api" && e.method === "PATCH"); i++) await delay(25);
        assert.ok(f.ledger().some(e => e.kind === "api" && e.method === "PATCH"));
        await delay(50);
        assert.equal(JSON.parse(await reader.readFile("utf8")).comments.length, 2);
    } finally { await reader.close(); }
    const result = outcome(await first);
    assert.equal(result.status, "initialized", result.reason);
    noExecution(f);
});

function routed(f, mode, generation, identity = {}) {
    const key = dispatchKey({ issueNumber: 123, allowanceId: f.approval.bootstrap.allowanceId,
        generation, mode, discussionFingerprint: f.context.manifest.discussionFingerprint,
        policyRevision: f.context.policyRevision, ...identity });
    return invoke([mode, "--issue", "123", "--config", f.configPath,
        "--routing-key", key, "--routing-generation", String(generation)], f.env);
}

const mutations = f => f.ledger().filter(r => r.kind === "worker" ||
    r.kind === "api" && r.method !== "GET" || r.kind === "git" && r.args.includes("bisect"));

test("routing duplicate subprocess deliveries preserve bootstrap, advance and publish mutations and budgets", async t => {
    const f = await fixture(t);
    for (const [mode, status] of [["bootstrap", "initialized"], ["advance", "observed"], ["publish", "published"]])
        await t.test(mode, async () => {
            if (mode === "publish") assert.equal(outcome(await run(f, "advance")).status, "blocked");
            const generation = mode === "bootstrap" ? -1 : (await checkpoint(f)).cp.generation;
            const first = outcome(await routed(f, mode, generation));
            assert.equal(first.status, status, first.reason);
            const before = (await checkpoint(f)).cp, writes = mutations(f);
            f.nextRun();
            const repeated = outcome(await routed(f, mode, generation));
            assert.equal(repeated.status, "duplicate", repeated.reason);
            assert.deepEqual((await checkpoint(f)).cp, before);
            assert.deepEqual(mutations(f), writes, "duplicate must not write a slot, charge or launch");
        });
    assert.equal(f.ledger().filter(r => r.kind === "worker").length, 1);
    assert.equal((await checkpoint(f)).cp.state.budget.consumed.workerUnits, 1);
    assert.equal(mutations(f).filter(r => r.route === "issues/comments/457").length, 1);
});

test("routing future generations fail before freshness repair; old generations can repair only the receipt", async t => {
    for (const mode of ["advance", "publish"]) for (const interrupted of [false, true])
        await t.test(`${mode} interrupted=${interrupted}`, async t => {
            const f = await fixture(t);
            assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
            if (interrupted) {
                const { context, cp, transport } = await checkpoint(f);
                f.update(db => { db.faults.commentAck = true; });
                await assert.rejects(state.reserveWork(transport, context, { generation: cp.generation,
                    attemptId: "torn-routing", request: { kind: "released", endpoint: "stable", phase: "stable",
                        target: { sdk: context.stableSdk } } }), /acknowledgement lost/);
            }
            const before = (await checkpoint(f)).cp, writes = mutations(f), statuses = f.read().statuses;
            const future = await routed(f, mode, before.generation + 1);
            assert.equal(outcome(future).status, "blocked");
            assert.match(outcome(future).reason, /routing.*generation/);
            assert.deepEqual(mutations(f), writes);
            assert.deepEqual(f.read().statuses, statuses);
            if (interrupted) {
                f.nextRun();
                assert.equal(outcome(await routed(f, mode, before.generation - 1)).status, "duplicate");
                assert.deepEqual((await checkpoint(f)).cp, before);
                assert.deepEqual(mutations(f).slice(writes.length).map(r => [r.method, r.route]),
                    [["POST", `statuses/${f.config.anchorSha}`]]);
            }
            noExecution(f);
        });
});

test("routing tampered bindings and stale duplicates cannot mutate durable state", async t => {
    for (const mode of ["bootstrap", "advance", "publish"]) for (const change of [
        "allowance", "discussion", "policy", "issue", "mode", "closed", "current-discussion", "sample", "freshness", "owner", "evidence",
    ]) await t.test(`${mode} ${change}`, async t => {
        const f = await fixture(t);
        const identity = change === "allowance" ? { allowanceId: "replacement" } :
            change === "discussion" ? { discussionFingerprint: contracts.digest("stale") } :
            change === "policy" ? { policyRevision: contracts.sha("stale") } :
            change === "issue" ? { issueNumber: 124 } :
            change === "mode" ? { mode: mode === "advance" ? "publish" : "advance" } : {};
        const tampered = Object.keys(identity).length > 0;
        if (mode !== "bootstrap" || !tampered) assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
        const generation = mode === "bootstrap" ? -1 : (await checkpoint(f)).cp.generation;
        if (!tampered && (mode !== "bootstrap" || change === "evidence"))
            assert.equal(outcome(await run(f, "advance")).status, "observed");
        if (change === "owner") f.nextRun();
        f.update(db => {
            if (change === "closed") db.issue.state = "closed";
            if (change === "current-discussion") db.issue.body += " correction";
            if (change === "sample") db.files[f.context.manifest.sources[0].path] = Buffer.from("changed").toString("base64");
            if (change === "freshness") db.statuses = [];
            if (change === "owner") { db.runs["12345/1"].status = "in_progress"; db.runs["12345/1"].conclusion = null; }
        });
        if (change === "evidence") {
            const store = path.join(f.env.RB_STATE_DIRECTORY, "dotnet-fsharp-123", "store");
            const files = (await fs.readdir(store)).filter(name => name.endsWith(".bin"));
            assert.ok(files.length > 0);
            await Promise.all(files.map(file => fs.unlink(path.join(store, file))));
        }
        const writes = mutations(f), before = f.read();
        const result = outcome(await routed(f, mode, generation, identity));
        assert.equal(result.status, "blocked", JSON.stringify(result));
        assert.deepEqual(mutations(f), writes);
        assert.deepEqual(f.read().comments, before.comments);
        assert.deepEqual(f.read().statuses, before.statuses);
    });
});

test("actual CLI bootstraps once and advances one charged contained unit with durable collection", async t => {
    const f = await fixture(t);
    assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
    assert.equal(outcome(await run(f, "bootstrap")).status, "loaded");
    const observed = outcome(await run(f, "advance"));
    assert.equal(observed.status, "observed", observed.reason);
    assert.deepEqual(observed.budget, { sourceBuilds: 0, workerUnits: 1, reservedMinutes: 110 });
    assert.equal(observed.outcome.outcome, "untestable");
    const stopped = outcome(await run(f, "advance"));
    assert.equal(stopped.status, "blocked", stopped.reason);
    assert.match(stopped.reason, /stable untestable/);
    assert.equal(f.ledger().filter(r => r.kind === "worker").length, 1);
    assert.equal(f.ledger().find(r => r.kind === "worker").exitCode, 0);
    assert.deepEqual(f.ledger().find(r => r.kind === "worker").credentialNames, []);
    assert.equal(f.ledger().filter(r => r.kind === "git" && r.args.includes("bisect")).length, 0);
    const published = outcome(await run(f, "publish"));
    assert.equal(published.status, "published", published.reason);
    const again = outcome(await run(f, "publish"));
    assert.equal(again.status, "published", again.reason);
    assert.equal(f.ledger().filter(r => r.kind === "api" && r.method === "PATCH" && r.route === "issues/comments/457").length, 1);
    t.diagnostic(JSON.stringify({ observed, requests: f.ledger().filter(r => r.kind === "api").length,
        workers: 1, bisect: 0 }));
});

test("actual CLI rejects oversized source bundles without refunds or relaunches", async t => {
    const f = await fixture(t);
    assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
    await seed(f, "stable", "product-failure");
    f.update(db => { db.faults.oversizeBundle = true; });
    const result = outcome(await run(f, "advance"));
    assert.equal(result.status, "blocked", result.reason);
    assert.match(result.reason, /bounded|byte limit/);
    const { cp } = await checkpoint(f);
    assert.deepEqual(cp.state.budget.consumed, { sourceBuilds: 1, workerUnits: 2, reservedMinutes: 220 });
    assert.notEqual(cp.pending.executionGeneration, undefined);
    assert.equal(outcome(await run(f, "advance")).status, "blocked");
    assert.deepEqual((await checkpoint(f)).cp.state.budget.consumed, cp.state.budget.consumed);
    noExecution(f);
});

test("provisioning binds returned real IDs, survives lost creation acknowledgements, and never executes code", async t => {
    const f = await fixture(t);
    f.update(db => { db.comments = []; db.faults.creationAck = true; });
    const first = outcome(await run(f, "provision"));
    assert.equal(first.status, "blocked");
    assert.match(first.reason, /acknowledgement lost/);
    const recovered = outcome(await run(f, "provision"));
    assert.equal(recovered.status, "provisioned", recovered.reason);
    assert.equal(recovered.approvalRequired, true);
    assert.equal(f.read().comments.length, 2);
    assert.deepEqual(outcome(await run(f, "provision")).registration, recovered.registration);
    const approval = { ...f.approval, bootstrap: recovered.registration };
    f.update(db => {
        db.files[".github/regression-bisect/registrations/123.json"] = Buffer.from(JSON.stringify(recovered.registration)).toString("base64");
        db.files[".github/regression-bisect/approved/123.json"] = Buffer.from(JSON.stringify(approval)).toString("base64");
    });
    assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
    noExecution(f);
});

test("missing/deleted registration, slot or incomplete allowance history cannot authorize replacement", async t => {
    for (const change of ["history", "pagination", "deleted-slot", "deleted-journal", "forged-receipt", "rollback"]) await t.test(change, async t => {
        const f = await fixture(t);
        f.update(db => { db.comments = []; });
        const provisioned = outcome(await run(f, "provision"));
        assert.equal(provisioned.status, "provisioned", provisioned.reason);
        if (change === "deleted-journal")
            await fs.unlink(path.join(f.env.RB_STATE_DIRECTORY, "dotnet-fsharp-123", "provision.json"));
        else f.update(db => {
            if (["history", "pagination"].includes(change)) db.faults[change] = true;
            if (change === "deleted-slot") db.comments.shift();
            if (change === "forged-receipt") db.statuses[0].target_url += "a";
            if (change === "rollback") db.statuses = [];
        });
        const count = f.read().comments.length;
        const blocked = outcome(await run(f, "provision"));
        assert.equal(blocked.status, "blocked", JSON.stringify(blocked));
        assert.equal(f.read().comments.length, count);
        noExecution(f);
    });
});

test("real subprocess exclusion covers intake through transition", async t => {
    const f = await fixture(t);
    f.update(db => { db.faults.pauseRuntime = true; });
    const first = run(f, "bootstrap");
    for (let i = 0; i < 400 && !f.ledger().some(e => e.kind === "barrier"); i++) await delay(25);
    assert.ok(f.ledger().some(e => e.kind === "barrier"), "first process acquired lock");
    const denied = outcome(await run(f, "bootstrap"));
    assert.equal(denied.status, "blocked");
    assert.match(denied.reason, /lock/);
    f.update(db => { delete db.faults.pauseRuntime; });
    assert.equal(outcome(await first).status, "initialized");
    assert.equal(outcome(await run(f, "bootstrap")).status, "loaded");
    noExecution(f);
});

test("a killed process cannot hand off until its exact previous run has completed", async t => {
    const f = await fixture(t);
    f.update(db => { db.faults.pauseRuntime = true; });
    let child;
    const first = invoke(["bootstrap", "--issue", "123", "--config", f.configPath], f.env, value => { child = value; });
    for (let i = 0; i < 400 && !f.ledger().some(e => e.kind === "barrier"); i++) await delay(25);
    assert.ok(f.ledger().some(e => e.kind === "barrier"));
    child.kill("SIGKILL");
    await first;
    f.update(db => { delete db.faults.pauseRuntime; });
    const blocked = outcome(await run(f, "bootstrap"));
    assert.equal(blocked.status, "blocked");
    assert.match(blocked.reason, /abandoned issue owner/);
    f.nextRun();
    const recovered = outcome(await run(f, "bootstrap"));
    assert.equal(recovered.status, "initialized", recovered.reason);
    noExecution(f);
});

test("missing/swapped keys, identity/runtime failures and malicious inputs block with zero bisect operations", async t => {
    const cases = {
        "missing key": { RB_SIGNING_KEYS: "{}" },
        "malformed key JSON": { RB_SIGNING_KEYS: "SECRET-CANARY-NOT-JSON" },
        "swapped key": { RB_SIGNING_KEYS: JSON.stringify({ fixture: Buffer.alloc(32).toString("base64") }) },
        "workflow revision": { GITHUB_WORKFLOW_SHA: "a".repeat(40) },
        "repository": { GITHUB_REPOSITORY: "attacker/fsharp" },
        "run": { GITHUB_RUN_ID: "999" },
        "attempt": { GITHUB_RUN_ATTEMPT: "2" },
        "credentials": { RB_GITHUB_TOKEN: "" },
        "production injection": { GITHUB_ACTIONS: "true" },
    };
    for (const [name, env] of Object.entries(cases)) await t.test(name, async t => {
        const f = await fixture(t);
        const result = await run(f, "advance", env);
        assert.equal(outcome(result).status, "blocked");
        assert.ok(!result.stdout.includes("SECRET-CANARY-NOT-JSON"));
        noExecution(f);
    });
    for (const name of ["runtime", "anchor", "policy", "closed", "label", "created", "pull", "correction", "asset"]) await t.test(name, async t => {
        const f = await fixture(t);
        f.update(db => {
            if (name === "runtime") db.faults.runtime = true;
            if (name === "anchor") db.anchorSha = "a".repeat(40);
            if (name === "policy") db.files[".github/policies/regression-bisect.json"] = Buffer.from("{}").toString("base64");
            if (name === "closed") db.issue.state = "closed";
            if (name === "label") db.issue.labels = ["regression"];
            if (name === "created") db.issue.created_at = "2026-08-31T23:59:59Z";
            if (name === "pull") db.issue.pull_request = {};
            if (name === "correction") db.issue.body += " correction";
            if (name === "asset") db.files[f.context.manifest.sources[0].path] = Buffer.from("changed").toString("base64");
        });
        assert.equal(outcome(await run(f, "bootstrap")).status, "blocked");
        noExecution(f);
    });
});

test("generation-zero write acknowledgement loss loads the same allowance", async t => {
    const f = await fixture(t);
    f.update(db => { db.faults.commentAck = true; });
    assert.equal(outcome(await run(f, "bootstrap")).status, "blocked");
    const result = outcome(await run(f, "bootstrap"));
    assert.equal(result.status, "initialized", result.reason);
    assert.deepEqual(result.budget, { sourceBuilds: 0, workerUnits: 0, reservedMinutes: 0 });
    noExecution(f);
});

test("bootstrap recovery requires its original authenticated local journal", async t => {
    for (const interrupted of [false, true]) for (const fault of ["missing", "forged"])
        await t.test(`${interrupted}/${fault}`, async t => {
            const f = await fixture(t);
            if (interrupted) f.update(db => { db.faults.commentAck = true; });
            assert.equal(outcome(await run(f, "bootstrap")).status, interrupted ? "blocked" : "initialized");
            const file = path.join(f.env.RB_STATE_DIRECTORY, "dotnet-fsharp-123", "bootstrap.json");
            if (fault === "missing") await fs.unlink(file);
            else await fs.writeFile(file, JSON.stringify("forged bootstrap"));
            const writes = mutations(f), before = (await checkpoint(f)).cp;
            const result = outcome(await routed(f, "bootstrap", -1));
            assert.equal(result.status, "blocked");
            assert.match(result.reason, /bootstrap journal/);
            assert.deepEqual(mutations(f), writes);
            assert.deepEqual((await checkpoint(f)).cp, before);
            noExecution(f);
        });
});

test("claimed execution loss resumes authenticated collection without a second launch or charge", async t => {
    const f = await fixture(t);
    assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
    f.update(db => { db.faults.workerAck = true; });
    assert.equal(outcome(await run(f, "advance")).status, "blocked");
    f.nextRun();
    const result = outcome(await run(f, "advance"));
    assert.equal(result.status, "observed", result.reason);
    assert.equal(result.recovered, true);
    assert.equal(result.budget.workerUnits, 1);
    assert.equal(f.ledger().filter(r => r.kind === "worker").length, 1);
});

test("claimed recovery preserves the absolute deadline and refuses another active owner", async t => {
    for (const expired of [false, true]) await t.test(String(expired), async t => {
        const f = await fixture(t);
        assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
        f.update(db => { db.faults.workerAck = true; });
        assert.equal(outcome(await run(f, "advance")).status, "blocked");
        f.nextRun();
        f.update(db => {
            if (expired) db.now = new Date(Date.parse(db.now) + 111 * 60000).toISOString();
            else { db.runs["12345/1"].status = "in_progress"; db.runs["12345/1"].conclusion = null; }
        });
        const result = outcome(await run(f, "advance"));
        assert.equal(result.status, "blocked");
        assert.match(result.reason, expired ? /deadline/ : /active linked investigation/);
        assert.equal((await checkpoint(f)).cp.state.budget.consumed.reservedMinutes, 110);
        assert.equal(f.ledger().filter(r => r.kind === "worker").length, 1);
    });
});

test("claimed recovery samples its deadline after collection and evidence verification", async t => {
    for (const point of ["unit", "verify"]) await t.test(point, async t => {
        const f = await fixture(t);
        assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
        f.update(db => { db.faults.workerAck = true; });
        assert.equal(outcome(await run(f, "advance")).status, "blocked");
        f.nextRun();
        const before = (await checkpoint(f)).cp;
        f.update(db => {
            db.now = new Date(Date.parse(before.pending.deadline) - 1000).toISOString();
            db.faults.clockAt = point;
            db.faults.clockAfter = new Date(Date.parse(before.pending.deadline) + 1000).toISOString();
        });
        const result = outcome(await run(f, "advance"));
        assert.equal(f.ledger().filter(r => r.kind === "clock").length, 1);
        assert.equal(result.status, "blocked", JSON.stringify(result));
        assert.match(result.reason, /deadline/);
        const after = (await checkpoint(f)).cp;
        assert.equal(after.pending.id, before.pending.id);
        assert.equal(after.pending.deadline, before.pending.deadline);
        assert.deepEqual(after.state.budget.consumed, before.state.budget.consumed);
        assert.deepEqual(after.observations, {});
        assert.equal(f.ledger().filter(r => r.kind === "worker").length, 1);
        assert.equal(f.ledger().filter(r => r.kind === "git" && r.args.includes("bisect")).length, 0);
    });
});

test("a third CLI recovers the recovery uploader's durable bytes without launch scratch", async t => {
    const f = await fixture(t);
    assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
    f.update(db => { db.faults.workerAck = true; });
    assert.equal(outcome(await run(f, "advance")).status, "blocked");
    f.nextRun();
    f.update(db => { db.faults.durableWriteAck = true; });
    assert.match(outcome(await run(f, "advance")).reason, /durable upload acknowledgement lost/);
    assert.equal(f.ledger().filter(r => r.kind === "durable-write").length, 1);
    const before = (await checkpoint(f)).cp;
    const launch = path.join(f.env.RB_STATE_DIRECTORY, "dotnet-fsharp-123", "launches", before.pending.id);
    await fs.rm(launch, { recursive: true });
    f.update(db => { delete db.faults.durableWriteAck; });
    f.nextRun();
    const result = outcome(await run(f, "advance"));
    assert.equal(result.status, "observed", result.reason);
    assert.equal(result.recovered, true);
    const after = (await checkpoint(f)).cp;
    assert.equal(after.pending, null);
    assert.deepEqual(after.state.budget.consumed, before.state.budget.consumed);
    assert.match(Object.values(after.observations)[0].evidence[0].url, /runs\/12346\/attempts\/1/);
    assert.equal(f.ledger().filter(r => r.kind === "worker").length, 1);
    assert.equal(f.ledger().filter(r => r.kind === "git" && r.args.includes("bisect")).length, 0);
});

test("actual CLI retains issue ownership past a quota timeout until outstanding I/O settles", { timeout: 380000 }, async t => {
    const f = await fixture(t);
    assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
    f.update(db => { db.faults.pauseQuota = true; });
    let exited = false;
    const first = run(f, "advance").then(result => { exited = true; return result; });
    try {
        for (let i = 0; i < 1400 && !f.ledger().some(e => e.kind === "quota-timeout"); i++) await delay(250);
        assert.ok(f.ledger().some(e => e.kind === "quota-entered"));
        assert.ok(f.ledger().some(e => e.kind === "quota-timeout" && e.error === "TimeoutError"));
        // Let the controller's real five-second cleanup deadline elapse as well.
        await delay(5500);
        assert.equal(exited, false, "CLI must not exit and release its lock");
        const denied = outcome(await run(f, "advance"));
        assert.equal(denied.status, "blocked");
        assert.match(denied.reason, /lock.*live process/);
        const cp = (await checkpoint(f)).cp;
        assert.ok(cp.pending);
        assert.equal(cp.state.budget.consumed.reservedMinutes, 110);
    } finally {
        f.update(db => { delete db.faults.pauseQuota; });
        assert.equal(outcome(await first).status, "blocked");
    }
    assert.ok(f.ledger().some(e => e.kind === "quota-released"));
    f.nextRun();
    const result = outcome(await run(f, "advance"));
    assert.equal(result.status, "observed", result.reason);
    assert.equal(result.budget.workerUnits, 1);
    assert.equal(f.ledger().filter(r => r.kind === "worker").length, 1);
    assert.equal(f.ledger().filter(r => r.kind === "git" && r.args.includes("bisect")).length, 0);
});

test("current endpoint changes invalidate the evidence epoch, never its spent allowance", async t => {
    for (const changed of ["metadata", "main"]) for (const interrupted of [false, true]) await t.test(`${changed}/${interrupted}`, async t => {
        const f = await fixture(t);
        assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
        if (interrupted) f.update(db => { db.faults.workerAck = true; });
        assert.equal(outcome(await run(f, "advance")).status, interrupted ? "blocked" : "observed");
        const before = (await checkpoint(f)).cp;
        if (interrupted) f.nextRun();
        if (changed === "main") f.moveMain();
        else f.update(db => { db.faults.newMetadata = true; });
        const launches = changed === "metadata" || interrupted ? 2 : 1;
        const result = outcome(await run(f, "advance"));
        assert.equal(result.status, launches === 2 ? "observed" : "blocked", result.reason);
        const after = (await checkpoint(f)).cp;
        assert.notEqual(after.state.epoch, before.state.epoch);
        assert.equal(after.state.budget.allowanceId, before.state.budget.allowanceId);
        assert.equal(after.state.budget.consumed.workerUnits, launches);
        assert.equal(after.state.budget.consumed.reservedMinutes, launches * 110);
        assert.equal(after.deadline, before.deadline);
        assert.equal(after.records[0].status, interrupted ? "invalidated" : "committed");
        assert.equal(f.ledger().filter(r => r.kind === "worker").length, launches);
        assert.equal(f.ledger().filter(r => r.kind === "git" && r.args.includes("bisect")).length, 0);
    });
});

test("main refresh reconciles terminal endpoints before returning without another worker", async t => {
    for (const scenario of ["blocked", "published", "stopped"]) await t.test(scenario, async t => {
        const f = await fixture(t);
        assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
        if (scenario === "stopped") await seed(f, "stable", "pass");
        else assert.equal(outcome(await run(f, "advance")).status, "observed");
        const status = scenario === "stopped" ? "stopped" : "blocked";
        assert.equal(outcome(await run(f, "advance")).status, status);
        if (scenario === "published") assert.equal(outcome(await run(f, "publish")).status, "published");
        const before = (await checkpoint(f)).cp;
        const main = f.moveMain();
        const stale = outcome(await run(f, "publish"));
        assert.equal(stale.status, "requeue");
        assert.match(stale.reason, /stale main head/);
        for (let i = 0; i < 2; i++) {
            const result = outcome(await run(f, "advance"));
            assert.equal(result.status, status, result.reason);
            const after = (await checkpoint(f)).cp;
            assert.equal(after.state.pinnedMainSha, main);
            assert.notEqual(after.state.epoch, before.state.epoch);
            assert.equal(after.state.endpointEvidence.stable, before.state.endpointEvidence.stable);
            assert.deepEqual(after.state.budget.consumed, before.state.budget.consumed);
            assert.equal(after.state.budget.allowanceId, before.state.budget.allowanceId);
            assert.equal(after.deadline, before.deadline);
            assert.equal(after.history.length, before.history.length + 1);
            const published = outcome(await run(f, "publish"));
            assert.equal(published.status, "published", published.reason);
        }
        assert.equal(f.ledger().filter(r => r.kind === "worker").length, scenario === "stopped" ? 0 : 1);
        assert.equal(f.ledger().filter(r => r.kind === "git" && r.args.includes("bisect")).length, 0);
        assert.equal(f.ledger().filter(r => r.kind === "api" && r.method === "PATCH" && r.route === "issues/comments/457").length,
            scenario === "published" ? 2 : 1);
    });
});

test("malicious collected payload cannot clear a reservation or start an uncharged replacement", async t => {
    const f = await fixture(t);
    assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
    f.update(db => { db.faults.maliciousWorker = true; });
    const failed = outcome(await run(f, "advance"));
    assert.equal(failed.status, "blocked");
    assert.match(failed.reason, /receipt authentication/);
    f.nextRun();
    assert.equal(outcome(await run(f, "advance")).status, "blocked");
    assert.equal(f.ledger().filter(r => r.kind === "worker").length, 1);
});

test("all 26 denied endpoint combinations perform zero actual git bisect operations", async t => {
    const outcomes = ["pass", "product-failure", "untestable"];
    for (const stable of outcomes) for (const main of outcomes) for (const good of outcomes) {
        if (stable === "product-failure" && main === "product-failure" && good === "pass") continue;
        await t.test(`${stable}/${main}/${good}`, async t => {
            const f = await fixture(t);
            assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
            await seed(f, "stable", stable); await seed(f, "main", main); await seed(f, "good", good);
            const result = outcome(await run(f, "advance"));
            assert.ok(["stopped", "blocked"].includes(result.status), JSON.stringify(result));
            noExecution(f);
            t.diagnostic(JSON.stringify({ endpoints: [stable, main, good], result, workers: 0, bisect: 0 }));
        });
    }
});

test("six reservations cost 660 minutes and cannot authorize a seventh worker", async t => {
    const f = await fixture(t);
    assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
    for (let i = 0; i < 6; i++) await seed(f, null, "untestable", contracts.sha(String(i)));
    const result = outcome(await run(f, "advance"));
    assert.equal(result.status, "blocked", result.reason);
    assert.match(result.reason, /budget exhausted/);
    assert.equal((await checkpoint(f)).cp.state.budget.consumed.reservedMinutes, 660);
    noExecution(f);
});

test("fresh candidate confirmation consumes just one unit and keeps the existing confirmation IDs", async t => {
    const f = await fixture(t);
    assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
    await seed(f, "stable", "product-failure"); await seed(f, "main", "product-failure"); await seed(f, "good", "pass");
    const result = outcome(await run(f, "advance"));
    assert.equal(result.status, "observed", result.reason);
    assert.equal(result.budget.workerUnits, 4);
    assert.equal(result.budget.reservedMinutes, 440);
    const cp = (await checkpoint(f)).cp;
    assert.ok(cp.state.confirmation);
    const plan = fingerprint(cp.state.confirmation);
    assert.equal(f.ledger().filter(r => r.kind === "worker").length, 1);
    const bundles = f.ledger().filter(r => r.kind === "source-bundle");
    assert.equal(bundles.length, 1);
    const worker = f.ledger().find(r => r.kind === "worker");
    assert.equal(bundles[0].sourceSha, worker.unit.request.target.sourceSha);
    assert.equal(bundles[0].heads, `${bundles[0].sourceSha} refs/heads/selected`);
    assert.equal(cp.pending, null);
    assert.deepEqual(worker.credentialNames, []);
    const next = outcome(await run(f, "advance"));
    assert.ok(["blocked", "confirmation"].includes(next.status), JSON.stringify(next));
    assert.equal(f.ledger().filter(r => r.kind === "worker").length, 1);
    assert.equal((await checkpoint(f)).cp.state.budget.consumed.workerUnits, 4);
    t.diagnostic(JSON.stringify({ plan, result, next, bundles, workers: 1 }));
});

test("production cannot substitute missing, forged or stale containment receipts", async t => {
    const f = await fixture(t);
    const { containment } = require("./controller-cli.cjs");
    await assert.rejects(containment(f.config, undefined, {}), /containment evidence directory/);
    await assert.rejects(containment(f.config, f.directory, {}), /ENOENT/);
    await fs.writeFile(path.join(f.directory, "containment.json"), '{"status":"passed"}');
    await assert.rejects(containment(f.config, f.directory, {}), /digest mismatch/);
    f.config.containment.reportSha256 = contracts.digest('{"status":"passed"}');
    await assert.rejects(containment(f.config, f.directory, {}), /incomplete or stale/);
    noExecution(f);
});

test("synthetic file-backed containment attestation checks every receipt and current binding", async t => {
    const f = await fixture(t);
    const { containment } = require("./controller-cli.cjs");
    const modes = ["inspect", "disk", "pids", "cpu", "memory", "malformed", "timeout"];
    const runtime = { ID: "synthetic-daemon", ServerVersion: "28.0.1", KernelVersion: "synthetic-kernel",
        OperatingSystem: "synthetic-linux", CgroupVersion: "2",
        SecurityOptions: ["name=seccomp,profile=builtin", "name=apparmor"] };
    const helperPath = "eng/regression-bisect/container-entry.cjs";
    const helper = { path: helperPath, sha256: contracts.digest(await fs.readFile(path.join(__dirname, "container-entry.cjs"))) };
    const stale = /incomplete or stale/, receiptMismatch = /runtime or cleanup attestation mismatch/;
    const cases = [
        ["accepted", () => {}, null],
        ["report status", s => { s.report.status = "incomplete"; }, stale],
        ["report image", s => { s.report.image = "untrusted"; }, stale],
        ["report source", s => { s.report.sourceSha256 = contracts.digest("old smoke"); }, stale],
        ["report config", s => { s.report.configSha256 = contracts.digest("old config"); }, stale],
        ["missing case", s => { s.report.cases.pop(); }, stale],
        ["duplicate case", s => { s.report.cases[1] = s.report.cases[0]; }, stale],
        ["reordered cases", s => { s.report.cases.reverse(); }, stale],
        ["failed case", s => { s.report.cases[0].passed = false; }, stale],
        ["changed configuration", s => { s.container.limits.cpus++; }, /configuration has changed/],
        ["worker not started", s => { s.receipt.workerStarted = false; }, receiptMismatch],
        ["receipt image", s => { s.receipt.image = "untrusted"; }, receiptMismatch],
        ["missing resource", s => { s.receipt.resources.pop(); }, receiptMismatch],
        ["missing cleanup", s => { s.receipt.cleanup.pop(); }, receiptMismatch],
        ["incomplete cleanup", s => { s.receipt.cleanup[0].removed = false; }, receiptMismatch],
        ["helper inventory digest", s => { s.receipt.helperDigest = contracts.digest("wrong"); }, receiptMismatch],
        ["changed helper bytes", s => { s.receipt.helpers[0].sha256 = contracts.digest("old helper");
            s.receipt.helperDigest = fingerprint(s.receipt.helpers); }, /helper bytes have changed/],
        ["unsafe helper path", s => { s.receipt.helpers[0].path = "../outside";
            s.receipt.helperDigest = fingerprint(s.receipt.helpers); }, /path/],
        ...Object.keys(runtime).flatMap(key => [
            [`changed runtime ${key}`, s => { s.receipt.runtime[key] = "changed"; }, receiptMismatch],
            [`missing current runtime ${key}`, s => { delete s.runtime[key]; }, receiptMismatch],
        ]),
        ...modes.map(mode => [`invalid ${mode} receipt`, s => { s.mode = mode; s.receipt.workerStarted = false; }, receiptMismatch]),
    ];
    for (const [name, change, expected] of cases) await t.test(name, async () => {
        const config = structuredClone(f.config);
        const container = structuredClone(config.container);
        const report = { status: "passed", image, configSha256: contracts.digest(JSON.stringify(container)),
            sourceSha256: contracts.digest(await fs.readFile(path.join(__dirname, "containment.smoke.cjs"))),
            cases: modes.map(mode => ({ mode, passed: true })) };
        const receipt = { workerStarted: true, image, runtime, helpers: [helper], helperDigest: fingerprint([helper]),
            resources: modes.slice(0, 4).map(name => ({ name })), cleanup: modes.slice(0, 4).map(name => ({ name, removed: true })) };
        const s = structuredClone({ report, container, receipt, runtime, mode: "inspect" });
        change(s);
        const directory = path.join(f.directory, "containment");
        await fs.mkdir(directory, { recursive: true });
        const write = async (file, value) => {
            const raw = JSON.stringify(value);
            await fs.writeFile(path.join(directory, file), raw);
            return contracts.digest(raw);
        };
        config.containment.reportSha256 = await write("containment.json", s.report);
        config.containment.configSha256 = await write("container.json", s.container);
        for (const mode of modes) {
            await fs.mkdir(path.join(directory, mode), { recursive: true });
            config.containment.receiptSha256[mode] = await write(path.join(mode, "receipt.json"), mode === s.mode ? s.receipt : receipt);
        }
        // Keep the report's config binding current so changed settings reach the config comparison.
        if (name === "changed configuration") {
            s.report.configSha256 = config.containment.configSha256;
            config.containment.reportSha256 = await write("containment.json", s.report);
        }
        if (expected) await assert.rejects(containment(config, directory, s.runtime), expected);
        else {
            await containment(config, directory, s.runtime);
            await fs.appendFile(path.join(directory, "container.json"), " ");
            await assert.rejects(containment(config, directory, s.runtime), /configuration has changed/);
            await write("container.json", s.container);
            await fs.appendFile(path.join(directory, "timeout", "receipt.json"), " ");
            await assert.rejects(containment(config, directory, s.runtime), /receipt digest mismatch/);
            await fs.unlink(path.join(directory, "timeout", "receipt.json"));
            await assert.rejects(containment(config, directory, s.runtime), /ENOENT/);
        }
    });
    noExecution(f);
});

test("publication adopts a lost fixed-slot acknowledgement and required retention loss blocks", async t => {
    for (const retentionLost of [false, true]) await t.test(String(retentionLost), async t => {
        const f = await fixture(t);
        assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
        assert.equal(outcome(await run(f, "advance")).status, "observed");
        await run(f, "advance");
        f.update(db => { db.faults.resultAck = true; });
        assert.equal(outcome(await run(f, "publish")).status, "blocked");
        if (retentionLost) {
            const store = path.join(f.env.RB_STATE_DIRECTORY, "dotnet-fsharp-123", "store");
            const file = (await fs.readdir(store)).find(name => name.endsWith(".bin"));
            assert.ok(file);
            await fs.unlink(path.join(store, file));
        }
        const result = outcome(await run(f, "publish"));
        assert.equal(result.status, retentionLost ? "blocked" : "published", result.reason);
        assert.equal(f.ledger().filter(r => r.kind === "api" && r.method === "PATCH" && r.route === "issues/comments/457").length, 1);
        assert.equal((await checkpoint(f)).cp.state.budget.consumed.workerUnits, 1);
    });
});

test("publication replaces only an authenticated prior epoch in its registered slot", async t => {
    for (const wrongHistory of [false, true]) await t.test(String(wrongHistory), async t => {
        const f = await fixture(t);
        assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
        assert.equal(outcome(await run(f, "advance")).status, "observed");
        assert.equal(outcome(await run(f, "advance")).status, "blocked");
        assert.equal(outcome(await run(f, "publish")).status, "published");
        f.update(db => { db.faults.newMetadata = true; });
        assert.equal(outcome(await run(f, "advance")).status, "observed");
        assert.equal(outcome(await run(f, "advance")).status, "blocked");
        const { context, transport } = await checkpoint(f);
        if (wrongHistory) {
            const old = await state.authenticateDecodedRecord(transport, { ...context, commentId: 457 },
                f.read().comments.find(c => c.id === 457));
            old.payload.report.deadline = "2026-12-01T00:00:00Z";
            old.payload.digest = fingerprint(old.payload.report);
            f.update(db => {
                db.comments.find(c => c.id === 457).body = state.serializeWorkflowRecord({
                    kind: "result", status: "Regression investigation result", payload: old.payload }, { ...context, commentId: 457 });
            });
        }
        const before = f.read().comments.find(c => c.id === 457).body;
        const result = outcome(await run(f, "publish"));
        assert.equal(result.status, wrongHistory ? "blocked" : "published", result.reason);
        assert.equal(f.ledger().filter(r => r.kind === "api" && r.method === "PATCH" && r.route === "issues/comments/457").length,
            wrongHistory ? 1 : 2);
        assert.equal(f.read().comments.find(c => c.id === 457).body === before, wrongHistory);
        if (!wrongHistory) assert.notEqual(before, f.read().comments.find(c => c.id === 457).body);
        assert.equal((await checkpoint(f)).cp.state.budget.consumed.reservedMinutes, 220);
        assert.equal(f.ledger().filter(r => r.kind === "git" && r.args.includes("bisect")).length, 0);
    });
});

test("publication rejects an authenticated current result in an alternate slot", async t => {
    const f = await fixture(t);
    assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
    assert.equal(outcome(await run(f, "advance")).status, "observed");
    assert.equal(outcome(await run(f, "advance")).status, "blocked");
    f.update(db => { db.faults.resultAck = true; });
    assert.equal(outcome(await run(f, "publish")).status, "blocked");
    const { context, transport } = await checkpoint(f);
    const original = f.read().comments.find(c => c.id === 457);
    const result = await state.authenticateDecodedRecord(transport, { ...context, commentId: 457 }, original);
    f.update(db => { db.comments.push({ ...original, id: 458, body: state.serializeWorkflowRecord({
        kind: "result", status: "Regression investigation result", payload: result.payload,
    }, { ...context, commentId: 458 }) }); });
    const blocked = outcome(await run(f, "publish"));
    assert.equal(blocked.status, "blocked");
    assert.match(blocked.reason, /outside the registered result slot/);
    assert.equal(f.ledger().filter(r => r.kind === "api" && r.method === "PATCH" && r.route !== "issues/comments/456").length, 1);
    assert.equal((await checkpoint(f)).cp.state.budget.consumed.reservedMinutes, 110);
    assert.equal(f.ledger().filter(r => r.kind === "git" && r.args.includes("bisect")).length, 0);
});

test("pending active owners block; completed unclaimed owners retry with a second charge", async t => {
    const f = await fixture(t);
    assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
    const { context, transport, cp } = await checkpoint(f);
    await state.reserveWork(transport, context, { generation: cp.generation, attemptId: "abandoned",
        request: { kind: "released", endpoint: "stable", phase: "stable", target: { sdk: context.stableSdk } } });
    const blocked = outcome(await run(f, "advance"));
    assert.equal(blocked.status, "blocked");
    assert.match(blocked.reason, /active owner/);
    noExecution(f);
    f.nextRun();
    const retried = outcome(await run(f, "advance"));
    assert.equal(retried.status, "observed", retried.reason);
    assert.equal(retried.budget.workerUnits, 2);
    assert.equal(retried.budget.reservedMinutes, 220);
    assert.equal(f.ledger().filter(r => r.kind === "worker").length, 1);
});

test("workflow structure preserves trust, permissions, issue lock, bounds and the actual executable", async () => {
    const document = YAML.parseDocument(await fs.readFile(path.join(__dirname, "..", "..", ".github", "workflows",
        "regression-bisect-controller.yml"), "utf8"), { strict: true, uniqueKeys: true });
    assert.deepEqual(document.errors, []);
    const workflow = document.toJS();
    assert.deepEqual(Object.keys(workflow.on), ["workflow_dispatch"]);
    assert.equal(workflow.on.workflow_dispatch.inputs.issue.type, "number");
    assert.deepEqual(workflow.on.workflow_dispatch.inputs.mode.options, ["bootstrap", "advance", "publish"]);
    assert.deepEqual(workflow.permissions, { contents: "read", actions: "read", issues: "write", statuses: "write" });
    assert.equal(workflow.concurrency["cancel-in-progress"], false);
    assert.equal(workflow.concurrency.group, "regression-bisect-dotnet-fsharp-${{ inputs.issue }}");
    assert.deepEqual(Object.keys(workflow.jobs), ["controller"]);
    const job = workflow.jobs.controller;
    assert.match(job.if, /github\.repository == 'dotnet\/fsharp'.*github\.ref == 'refs\/heads\/main'.*ENABLED == 'true'/);
    assert.equal(job.needs, undefined);
    assert.equal(job.environment, "regression-controller");
    assert.equal(job["timeout-minutes"], 135);
    assert.deepEqual(job["runs-on"], ["self-hosted", "linux", "x64", "regression-container-v1"]);
    const [checkout, controller] = job.steps;
    assert.match(checkout.uses, /^actions\/checkout@[a-f0-9]{40}$/);
    assert.equal(checkout.with.repository, "dotnet/fsharp");
    assert.equal(checkout.with.ref, "${{ vars.REGRESSION_CONTROLLER_REVISION }}");
    assert.equal(checkout.with["persist-credentials"], false);
    assert.equal(controller.uses, "./eng/regression-bisect");
    assert.equal(controller.env.RB_ACTION_INPUTS, "${{ toJSON(inputs) }}");
    assert.ok(job.steps.every(step => step.run === undefined), "no dispatch interpolation into a shell");
    const action = YAML.parse(await fs.readFile(path.join(__dirname, "action.yml"), "utf8"));
    assert.deepEqual(action.runs, { using: "node24", main: "controller-cli.cjs" });
});
});
