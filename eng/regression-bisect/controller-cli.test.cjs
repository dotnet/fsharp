const { test } = require("node:test");
const assert = require("node:assert/strict");
const { spawn } = require("node:child_process");
const path = require("node:path");
const fs = require("node:fs/promises");
const { setTimeout: delay } = require("node:timers/promises");
const { fixture, connect } = require("./fixtures/controller-service.cjs");
const { fingerprintInput: fingerprint } = require("./model.cjs");
const contracts = require("./fixtures/contracts.cjs");
const state = require("./state.cjs");
const { createGithubApi } = require("./github.cjs");
const { image } = require("./container.cjs");
const YAML = require("yaml");

const cli = path.join(__dirname, "controller-cli.cjs");
function invoke(args, env = {}, started = () => {}) {
    return new Promise((resolve, reject) => {
        const start = performance.now();
        const child = spawn(process.execPath, [cli, ...args], {
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

async function checkpoint(f) {
    const service = await connect(f.serviceFile);
    const transport = createGithubApi({ repository: "dotnet/fsharp", issueNumber: 123, checkpointCommentId: 456,
        resultCommentId: 457, anchorSha: f.config.anchorSha, token: "offline-controller-token" }, service.send);
    const context = { ...f.context, owner: { runId: f.env.GITHUB_RUN_ID, attempt: 1 } };
    const cp = (await state.authenticateDecodedRecord(transport, context, await transport.getComment(456))).payload;
    Object.assign(context, { stableSdk: cp.state.budget.stableSdk, pinnedMainSha: cp.state.pinnedMainSha,
        environmentFingerprint: cp.state.budget.environmentFingerprint });
    return { context, cp, transport };
}

async function seed(f, endpoint, outcome, target) {
    const { context, cp, transport } = await checkpoint(f);
    const kind = endpoint === "stable" ? "released" : "source";
    const sourceSha = target ?? (endpoint === "good" ? f.read().olderSha : context.pinnedMainSha);
    const reservation = await state.reserveWork(transport, context, { generation: cp.generation,
        attemptId: `seed-${cp.sequence}`, request: { kind, endpoint, phase: endpoint === "good" ? "good-search" : endpoint ?? "bisect",
            target: kind === "released" ? { sdk: context.stableSdk } : { sourceSha } } });
    const observation = contracts.observation(kind, outcome, context.manifest, sourceSha);
    observation.buildInstanceId = reservation.id;
    observation.provenance = { workerId: "synthetic-seed", runId: context.owner.runId,
        workflowRevision: context.workflowRevision, policyRevision: context.policyRevision };
    if (kind === "released") observation.target.sdk = context.stableSdk;
    for (const cell of observation.cells) {
        cell.environment.imageDigest = image.split(":").at(-1);
        cell.sdk.sha256 = context.stableSdk.archives[0].sha256;
        for (const artifact of Object.values(cell.artifacts)) artifact.buildInstanceId = reservation.id;
        if (kind === "source") cell.buildToolchain = contracts.buildToolchain(sourceSha, cell.environment.os);
    }
    return state.commitObservation(transport, context, { generation: reservation.generation,
        attemptId: reservation.id, observation, evidence: [] });
}

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

test("official metadata changes invalidate the evidence epoch, never its spent allowance", async t => {
    for (const interrupted of [false, true]) await t.test(String(interrupted), async t => {
        const f = await fixture(t);
        assert.equal(outcome(await run(f, "bootstrap")).status, "initialized");
        if (interrupted) f.update(db => { db.faults.workerAck = true; });
        assert.equal(outcome(await run(f, "advance")).status, interrupted ? "blocked" : "observed");
        const before = (await checkpoint(f)).cp;
        if (interrupted) f.nextRun();
        f.update(db => { db.faults.newMetadata = true; });
        const result = outcome(await run(f, "advance"));
        assert.equal(result.status, "observed", result.reason);
        const after = (await checkpoint(f)).cp;
        assert.notEqual(after.state.epoch, before.state.epoch);
        assert.equal(after.state.budget.allowanceId, before.state.budget.allowanceId);
        assert.equal(after.state.budget.consumed.workerUnits, 2);
        assert.equal(after.state.budget.consumed.reservedMinutes, 220);
        assert.equal(after.deadline, before.deadline);
        assert.equal(after.records[0].status, interrupted ? "invalidated" : "committed");
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
    const next = outcome(await run(f, "advance"));
    assert.ok(["blocked", "confirmation"].includes(next.status), JSON.stringify(next));
    assert.equal(f.ledger().filter(r => r.kind === "worker").length, 1);
    assert.equal((await checkpoint(f)).cp.state.budget.consumed.workerUnits, 4);
    t.diagnostic(JSON.stringify({ plan, result, next, workers: 1 }));
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
