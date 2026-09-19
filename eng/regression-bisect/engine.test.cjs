const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs/promises");
const path = require("node:path");
const os = require("node:os");
const { execFileSync, spawnSync } = require("node:child_process");
const f = require("./fixtures/contracts.cjs");
const { harness } = require("./fixtures/checkpoint.cjs");
const model = require("./model.cjs");
const state = require("./state.cjs");
const { advanceInvestigation, executeInvestigationWork } = require("./engine.cjs");

const gitExecutable = process.platform === "win32"
    ? execFileSync("where.exe", ["git.exe"], { encoding: "utf8" }).trim().split(/\r?\n/)[0]
    : execFileSync("which", ["git"], { encoding: "utf8" }).trim();

async function fixture(t, outcomes = ["product-failure", "product-failure", "pass"], messages = ["base", "one", "two", "main"]) {
    const root = await fs.mkdtemp(path.join(os.tmpdir(), "engine-test-"));
    t.after(() => fs.rm(root, { recursive: true, force: true }));
    const mirror = path.join(root, "mirror"), scratch = path.join(root, "scratch");
    await fs.mkdir(mirror); await fs.mkdir(scratch);
    const git = (...args) => execFileSync(gitExecutable, ["-c", "core.hooksPath=", "-c", "commit.gpgSign=false", ...args], {
        cwd: mirror, encoding: "utf8", env: { ...process.env, GIT_CONFIG_NOSYSTEM: "1",
            GIT_CONFIG_GLOBAL: path.join(root, "empty"), GIT_AUTHOR_NAME: "Fixture", GIT_AUTHOR_EMAIL: "fixture@example.invalid",
            GIT_COMMITTER_NAME: "Fixture", GIT_COMMITTER_EMAIL: "fixture@example.invalid",
            GIT_AUTHOR_DATE: "2026-09-01T00:00:00Z", GIT_COMMITTER_DATE: "2026-09-01T00:00:00Z" },
    }).trim();
    await fs.writeFile(path.join(root, "empty"), "");
    git("init", "--quiet", "--initial-branch=main");
    const shas = [];
    for (const message of messages) { git("commit", "--quiet", "--allow-empty", "-m", message); shas.push(git("rev-parse", "HEAD")); }
    const h = harness();
    h.context.pinnedMainSha = shas.at(-1);
    h.context.manifest.searchHints.sourceShas = [shas[0]];
    f.seal(h.context.manifest);
    h.context.authorization.manifestDigest = model.fingerprintInput(h.context.manifest);
    h.context.authorization.inputDigest = h.context.manifest.approvedInputDigest;
    const environments = Object.fromEntries(h.context.manifest.matrix.map(c => [c.id,
        { os: c.os, architecture: c.architecture, imageDigest: f.digest("runner image") }]));
    h.context.environmentFingerprint = model.fingerprintInput(environments);
    await h.start();
    const trace = [], calls = [];
    t.after(() => t.diagnostic(JSON.stringify({ bisectCount: calls.length,
        calls: calls.map(args => ({ args: [...args], exitCode: args.exitCode })) })));
    const snapshot = { status: "complete", snapshot: {
        discussionFingerprint: h.context.issueFingerprint,
        input: { discussion: { issue: h.context.issue }, reproduction: { manifest: h.context.manifest } },
        investigations: { status: "complete", active: [] },
    } };
    const deps = {
        transport: h.transport, context: h.context, environments,
        intake: { readIssueSnapshot: async () => { trace.push("snapshot"); return snapshot; } },
        approval: { readSource: async () => Buffer.from('printfn "ok"'),
            authenticateRecord: async () => false, readTrustedCheckpoints: async () => [] },
        platform: { os: "linux", architecture: "x64", rid: "linux-x64" },
        history: { gitExecutable, mirrorDirectory: mirror, scratchDirectory: scratch, mainRef: "refs/heads/main" },
        toolchains: { resolveLatestStableSdk: async () => {
            trace.push("latest-stable");
            return { status: "resolved", toolchain: { sdkVersion: h.context.stableSdk.version,
                metadata: [{ url: h.context.stableSdk.releaseIndexUrl, sha256: h.context.stableSdk.releaseIndexDigest }] } };
        } },
        execute: async (exe, args, settings) => {
            let call;
            if (args.includes("bisect")) {
                const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
                assert.ok(Object.values(cp.state.endpointEvidence).every(Boolean), "bisect preceded durable endpoints");
                call = args.slice(args.indexOf("bisect"));
                calls.push(call);
            }
            const result = spawnSync(exe, args, { ...settings, encoding: "utf8", timeout: settings.timeoutMs, maxBuffer: settings.maxBytes });
            if (result.error) throw result.error;
            if (call) call.exitCode = result.status;
            return { status: "completed", complete: !result.signal, exitCode: result.status, stdout: result.stdout, stderr: result.stderr };
        },
    };
    async function advance() {
        return advanceInvestigation({ now: h.context.now }, deps);
    }
    function observationFor(work, value) {
        const r = work.reservation;
        trace.push(r.request.phase);
        const which = r.request.endpoint;
        const outcome = value ?? outcomes[which === "stable" ? 0 : which === "main" ? 1 : 2];
        const observation = f.observation(r.request.kind, outcome, h.context.manifest, r.request.target.sourceSha);
        observation.buildInstanceId = r.id;
        observation.provenance.runId = r.owner.runId;
        for (const cell of observation.cells) {
            for (const artifact of Object.values(cell.artifacts)) artifact.buildInstanceId = r.id;
            if (observation.kind === "source") cell.buildToolchain = f.buildToolchain(observation.target.sourceSha, cell.environment.os);
        }
        return observation;
    }
    async function finish(work, value) {
        const r = work.reservation;
        const observation = observationFor(work, value);
        await state.commitObservation(h.transport, { ...h.context, ...work.binding }, {
            generation: r.generation, attemptId: r.id, observation, evidence: [],
        });
        return observation;
    }
    return Object.assign(h, { deps, snapshot, trace, calls, shas, git, advance, finish, observationFor, scratch });
}

async function endpoints(h) {
    for (let i = 0; i < 3; i++) {
        const work = await h.advance();
        assert.equal(work.status, "work", JSON.stringify(work));
        await h.finish(work);
    }
}

test("all 27 endpoint tuples deny every actual bisect subprocess except failure/failure/pass", async t => {
    for (const stable of ["pass", "product-failure", "untestable"])
        for (const main of ["pass", "product-failure", "untestable"])
            for (const good of ["pass", "product-failure", "untestable"])
                await t.test(`${stable}/${main}/${good}`, async t => {
                    const h = await fixture(t, [stable, main, good]);
                    let result;
                    for (let i = 0; i < 7; i++) {
                        result = await h.advance();
                        if (result.status !== "work" || result.reservation.request.phase === "bisect") break;
                        await h.finish(result);
                    }
                    const admitted = stable === "product-failure" && main === "product-failure" && good === "pass";
                    assert.equal(h.calls.length > 0, admitted, JSON.stringify(result));
                    if (!admitted) assert.equal(h.calls.length, 0);
                    else assert.equal(result.status, "work", JSON.stringify(result));
                    assert.notEqual(result.status, "complete");
                });
});

test("malformed, stale, unauthenticated and incomplete evidence never spawns bisect", async t => {
    const variants = {
        missing: cp => { cp.state.endpointEvidence.good = f.digest("missing"); },
        fingerprint: cp => { Object.values(cp.observations)[0].data.inputDigest = f.digest("wrong"); },
        authentication: (cp, h) => { h.context.keys.fixture = Buffer.from("wrong-authentication-key"); },
        provenance: cp => { Object.values(cp.observations)[0].data.provenance.policyRevision = f.sha("wrong"); },
        matrix: cp => { Object.values(cp.observations)[0].data.cells = []; },
        artifact: cp => { Object.values(cp.observations)[0].data.cells[0].artifacts.core.sha256 = "wrong"; },
        assertion: cp => { Object.values(cp.observations)[0].data.cells[0].attempts[0].execution.stderr = "different diagnostic"; },
        restore: cp => { Object.values(cp.observations)[0].data.cells[0].attempts[0].checks.restore = false; },
        runtime: cp => { Object.values(cp.observations)[0].data.cells[0].runtime = null; },
        sdk: cp => { Object.values(cp.observations)[0].data.cells[0].sdk = null; },
        buildToolchain: cp => { Object.values(cp.observations).find(o => o.data.kind === "source").data.cells[0].buildToolchain.sourceSha = f.sha("wrong"); },
        approval: (cp, h) => { h.context.authorization.approvalRevision = f.sha("untrusted"); },
        discussion: (cp, h) => { h.snapshot.snapshot.discussionFingerprint = f.digest("new correction"); },
        incompleteSnapshot: (cp, h) => { h.snapshot.status = "blocked"; h.snapshot.reason = "rate limited; incomplete comments"; },
        linkedInvestigation: (cp, h) => { h.snapshot.snapshot.investigations.active.push({ basis: "timeline-cross-reference" }); },
        latestChanged: (cp, h) => { h.deps.toolchains.resolveLatestStableSdk = async () => ({ status: "resolved",
            toolchain: { sdkVersion: "10.0.999", metadata: [] } }); },
        expiredArtifact: (cp, h) => {
            Object.values(cp.observations)[0].evidence = [{ required: true, sha256: f.digest("durable evidence"), url: "https://example.invalid/evidence" }];
            h.transport.readEvidence = async () => null;
        },
    };
    for (const [name, mutate] of Object.entries(variants)) await t.test(name, async t => {
        const h = await fixture(t);
        await endpoints(h);
        const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
        mutate(cp, h);
        if (name !== "authentication") h.checkpoint(cp);
        const result = await h.advance();
        assert.equal(result.status, "blocked", JSON.stringify(result));
        assert.equal(h.calls.length, 0);
    });
});

test("Git skip and two-run flakes retain all possible commits, never a guessed culprit", async t => {
    for (const flaky of [false, true]) await t.test(flaky ? "flaky" : "unbuildable", async t => {
        const h = await fixture(t, undefined, ["base", "middle", "main"]);
        await endpoints(h);
        const work = await h.advance();
        assert.equal(work.status, "work", JSON.stringify(work));
        const o = h.observationFor(work, "untestable");
        for (const c of o.cells) {
            c.attempts = flaky ? [f.attempt("pass"), f.attempt("product-failure")] :
                [f.attempt("pass"), f.attempt("pass")].map(a => ({ ...a, checks: { ...a.checks, build: false } }));
            c.outcome = model.classifyAttempts(c.attempts, h.context.manifest);
        }
        o.outcome = o.cells[0].outcome;
        await state.commitObservation(h.transport, h.context, { generation: work.reservation.generation,
            attemptId: work.reservation.id, observation: o, evidence: [] });
        const result = await h.advance();
        assert.equal(result.status, "blocked", JSON.stringify(result));
        assert.match(result.reason, /all-skipped/);
        assert.deepEqual(new Set(result.interval.remaining), new Set(h.shas.slice(1)));
        assert.equal(result.skips[0].reason, flaky ? "flaky" : "build-failure");
        assert.ok(h.calls.some(args => args[1] === "skip"));
        assert.equal(result.candidate, undefined);
    });
});

test("explicit reverts, reintroductions and cherry-picked backports block before bisect", async t => {
    for (const message of ['Revert "regression"', "Reintroduce changed semantics", "Fix (cherry picked from commit abcdef)"])
        await t.test(message, async t => {
            const h = await fixture(t, undefined, ["base", message, "main"]);
            await endpoints(h);
            const result = await h.advance();
            assert.equal(result.status, "blocked");
            assert.match(result.reason, /history|monotonicity/);
            assert.equal(result.interval.remaining.length, 2);
            assert.equal(h.calls.length, 0);
        });
});

test("merge and side-branch evidence cannot be assigned to a first parent", async t => {
    const h = await fixture(t);
    h.git("checkout", "--quiet", "-b", "side", h.shas[0]);
    h.git("commit", "--quiet", "--allow-empty", "-m", "side change");
    const side = h.git("rev-parse", "HEAD");
    h.git("checkout", "--quiet", "main");
    h.git("merge", "--quiet", "--no-ff", "side", "-m", "merge side");
    h.context.pinnedMainSha = h.git("rev-parse", "HEAD");
    await endpoints(h);
    const result = await h.advance();
    assert.equal(result.status, "blocked");
    assert.match(result.reason, /merge\/side-branch/);
    assert.ok(result.interval.remaining.includes(side));
    assert.equal(h.calls.length, 0);
});

test("cancelled reservations are charged, bounded and resumed through the state lifecycle", async t => {
    const h = await fixture(t);
    const first = await h.advance();
    assert.equal((await h.advance()).status, "blocked");
    h.nextRun();
    const second = await h.advance();
    assert.equal(second.status, "work", JSON.stringify(second));
    assert.equal(second.reservation.attempt, 2);
    assert.notEqual(second.reservation.id, first.reservation.id);
    h.nextRun();
    const result = await h.advance();
    assert.match(result.reason, /attempt budget/);
    const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
    assert.equal(cp.state.budget.consumed.reservedMinutes, 220);
    assert.equal(cp.records[0].status, "cancelled");
    assert.equal(h.calls.length, 0);
});

test("lost selection worktree and resumed process replay only authenticated typed decisions", async t => {
    const h = await fixture(t);
    await endpoints(h);
    const work = await h.advance();
    await h.finish(work, "pass");
    assert.deepEqual(await fs.readdir(h.scratch), []);
    h.nextRun();
    const result = await h.advance();
    assert.equal(result.status, "confirmation", JSON.stringify(result));
    assert.ok(h.calls.filter(args => args[1] === "start").length >= 2);
    assert.ok(h.calls.some(args => args[1] === "good" && args[2] === work.reservation.request.target.sourceSha));
    assert.ok(h.calls.every(args => !["replay", "run", "reset"].includes(args[1])));
});

test("fresh environment and input epochs invalidate evidence without replenishing allowance", async t => {
    for (const change of ["environment", "reproduction", "discussion", "approval", "main", "stable"]) await t.test(change, async t => {
        const h = await fixture(t);
        await h.finish(await h.advance());
        if (change === "environment") h.deps.environments["linux-x64"].imageDigest = f.digest("new image");
        else if (change === "main") {
            h.git("commit", "--quiet", "--allow-empty", "-m", "advance main");
        } else if (change === "stable") {
            h.context.stableSdk.releaseIndexDigest = f.digest("new official metadata");
        } else {
            if (change === "reproduction") h.context.manifest.assertions.pass.stdout = "new\n";
            if (change === "discussion") {
                h.context.manifest.discussionFingerprint = f.digest("corrected discussion");
                h.context.issueFingerprint = h.context.manifest.discussionFingerprint;
                h.snapshot.snapshot.discussionFingerprint = h.context.issueFingerprint;
            }
            if (change === "approval") h.context.policyRevision = f.sha("new approval");
            f.seal(h.context.manifest);
            Object.assign(h.context.authorization, { manifestDigest: model.fingerprintInput(h.context.manifest),
                inputDigest: h.context.manifest.approvedInputDigest, approvalRevision: h.context.policyRevision });
        }
        const work = await h.advance();
        assert.equal(work.status, "work", JSON.stringify(work));
        assert.equal(work.reservation.request.phase, change === "main" ? "main" : "stable");
        const cp = (await state.loadCheckpoint(h.transport, { ...h.context, ...work.binding })).checkpoint;
        assert.equal(cp.state.budget.consumed.workerUnits, 2);
        assert.equal(cp.state.budget.consumed.reservedMinutes, 220);
        assert.equal(cp.history.length, 1);
        assert.equal(h.calls.length, 0);
    });
});

test("expiry, missing state and allowance exhaustion never create a fresh allowance", async t => {
    for (const failure of ["expiry", "missing", "budget"]) await t.test(failure, async t => {
        const h = await fixture(t);
        if (failure === "expiry") h.context.now = "2026-10-01T00:00:00Z";
        if (failure === "missing") h.comment.body = "";
        if (failure === "budget") {
            for (let i = 0; i < 6; i++) {
                const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
                await state.reserveWork(h.transport, h.context, { generation: cp.generation, attemptId: `spent-${i}`,
                    request: { kind: "source", target: { sourceSha: f.sha(`unused-${i}`) }, phase: "good-search", endpoint: null } });
                h.nextRun();
            }
        }
        const result = await h.advance();
        assert.equal(result.status, "blocked", JSON.stringify(result));
        assert.match(result.reason, /deadline|authentication|budget/);
        assert.equal(h.calls.length, 0);
        if (failure === "budget")
            assert.equal((await state.loadCheckpoint(h.transport, h.context)).checkpoint.state.budget.consumed.reservedMinutes, 660);
    });
});

test("linear history saves endpoints, selects Git midpoint and returns fresh confirmation only", async t => {
    const h = await fixture(t);
    for (const phase of ["stable", "main", "good-search"]) {
        const work = await h.advance();
        assert.equal(work.status, "work", JSON.stringify(work));
        assert.equal(work.reservation.request.phase, phase);
        assert.deepEqual(Object.keys(work).sort(), ["binding", "commands", "reservation", "resolved", "status"]);
        assert.equal(h.calls.length, 0);
        await h.finish(work);
    }
    const work = await h.advance();
    assert.equal(work.status, "work", JSON.stringify(work));
    assert.equal(work.reservation.request.target.sourceSha, h.shas[2]);
    await h.finish(work, "pass");
    const result = await h.advance();
    assert.equal(result.status, "confirmation");
    assert.equal(result.candidate, h.shas[3]);
    assert.deepEqual(result.parents, [h.shas[2]]);
    assert.equal(result.requiresFreshConfirmation, true);
    assert.deepEqual(result.interval.remaining, [h.shas[3]]);
    assert.ok(h.calls.length > 0);
    const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
    assert.equal(cp.state.phase, "confirmation");
    assert.deepEqual(cp.state.budget.consumed, { sourceBuilds: 3, workerUnits: 4, reservedMinutes: 440 });
    assert.deepEqual(cp.state.interval.decisions, [{ sha: h.shas[2], outcome: "pass",
        observationDigest: cp.state.testedRevisions.at(-1).observationDigest }]);
});

test("a deterministic midpoint failure narrows bad without turning a build exit into a culprit", async t => {
    const h = await fixture(t, undefined, ["base", "introduces regression", "main"]);
    await endpoints(h);
    const work = await h.advance();
    assert.equal(work.reservation.request.target.sourceSha, h.shas[1]);
    await h.finish(work, "product-failure");
    const result = await h.advance();
    assert.equal(result.status, "confirmation", JSON.stringify(result));
    assert.equal(result.candidate, h.shas[1]);
    assert.deepEqual(result.parents, [h.shas[0]]);
    assert.deepEqual(result.interval.remaining, [h.shas[1]]);
    assert.ok(h.calls.some(args => args[1] === "bad" && args[2] === h.shas[1]));
});

test("observed descendant passes and unrelated branch claims stop before admission", async t => {
    for (const mismatch of [false, true]) await t.test(mismatch ? "ancestry" : "nonmonotonic", async t => {
        const h = await fixture(t);
        await endpoints(h);
        let sha = h.shas[1];
        if (mismatch) {
            h.git("checkout", "--quiet", "--orphan", "unrelated");
            h.git("commit", "--quiet", "--allow-empty", "-m", "other root");
            sha = h.git("rev-parse", "HEAD");
            h.git("checkout", "--quiet", "main");
        }
        for (const [sourceSha, outcome] of [[sha, "product-failure"], ...(!mismatch ? [[h.shas[2], "pass"]] : [])]) {
            const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
            const reservation = await state.reserveWork(h.transport, h.context, {
                generation: cp.generation, attemptId: `additional-${sourceSha}`,
                request: { kind: "source", target: { sourceSha }, endpoint: null, phase: "good-search" },
            });
            await h.finish({ reservation }, outcome);
        }
        const result = await h.advance();
        assert.equal(result.status, "blocked");
        assert.match(result.reason, mismatch ? /ancestry/ : /nonmonotonic/);
        assert.equal(h.calls.length, 0);
    });
});

test("source toolchain transitions remain evidence without changing the frozen sample", async t => {
    const h = await fixture(t, undefined, ["base", "main"]);
    for (const phase of ["stable", "main", "good-search"]) {
        const work = await h.advance();
        const observation = h.observationFor(work);
        if (phase === "good-search") for (const c of observation.cells) {
            const previousSdk = c.buildToolchain.requestedSdk;
            c.buildToolchain.requestedSdk = c.buildToolchain.requestedTools = c.buildToolchain.sdk.version = "9.0.100";
            c.buildToolchain.archive.url = c.buildToolchain.archive.url.replaceAll(previousSdk, "9.0.100");
            c.buildToolchain.archive.redirectUrl = c.buildToolchain.archive.url;
            c.buildToolchain.archive.name = c.buildToolchain.archive.url.split("/").at(-1);
            c.buildToolchain.compilerRuntime = { version: "10.0.0", sha256: f.digest("older compiler runtime"), verified: true };
            c.buildToolchain.requestedCompilerRuntime = "10.0.0";
        }
        await state.commitObservation(h.transport, h.context, { generation: work.reservation.generation,
            attemptId: work.reservation.id, observation, evidence: [] });
    }
    const result = await h.advance();
    assert.equal(result.status, "confirmation", JSON.stringify(result));
    assert.equal(result.toolchainTransitions.length, 1);
    assert.equal(result.toolchainTransitions[0].good.requestedSdk, "9.0.100");
    assert.equal(result.toolchainTransitions[0].main.requestedSdk, "11.0.100-rc.1.26420.103");
    assert.equal(result.toolchainTransitions[0].good.requestedCompilerRuntime, "10.0.0");
    assert.equal(result.toolchainTransitions[0].main.requestedCompilerRuntime, "11.0.0");
});

test("shallow history is untestable rather than a guessed older endpoint", async t => {
    const h = await fixture(t);
    await h.finish(await h.advance());
    await fs.writeFile(path.join(h.deps.history.mirrorDirectory, ".git", "shallow"), `${h.shas.at(-1)}\n`);
    const result = await h.advance();
    assert.equal(result.status, "blocked");
    assert.match(result.reason, /shallow|history/);
    assert.equal(h.calls.length, 0);
});

test("typed decisions and environment epochs have strict schemas", async t => {
    const h = await fixture(t);
    await endpoints(h);
    const work = await h.advance();
    await h.finish(work, "pass");
    await h.advance();
    const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
    for (const mutate of [
        s => { s.interval.decisions[0].replay = "git bisect bad HEAD"; },
        s => { s.interval.decisions[0].sha = "HEAD"; },
        s => { s.interval.decisions[0].outcome = "bad"; },
        s => { s.interval.decisions.push(s.interval.decisions[0]); },
        s => { s.interval.decisions[0].observationDigest = f.digest("untrusted"); },
        s => { s.interval.remaining = []; },
        s => { s.budget.environmentFingerprint = f.digest("other environment"); },
    ]) {
        const s = structuredClone(cp.state);
        mutate(s);
        assert.throws(() => model.validateState(s));
    }
    cp.state.interval.decisions[0].outcome = "product-failure";
    h.checkpoint(cp);
    await assert.rejects(state.loadCheckpoint(h.transport, h.context), /decision.*authenticated/);
});

test("default intake freezes complete corrected discussion and immutable bytes before any work", async t => {
    const h = await fixture(t);
    const issue = { ...h.context.issue, id: 123, title: "regression", body: "reporter claim is not a command" };
    const comment = { id: 22, body: "corrected sample", user: { id: 1 } };
    const transport = { request: async route => ({ data: route.endsWith("/comments") ? [comment] :
        route.endsWith("/timeline") ? [] : issue, headers: { link: "" } }) };
    const { readIssueSnapshot } = require("./intake.cjs");
    const current = await readIssueSnapshot({ repository: "dotnet/fsharp", issueNumber: 123, transport, now: h.context.now });
    assert.equal(current.status, "complete");
    h.context.manifest.discussionFingerprint = current.snapshot.discussionFingerprint;
    f.seal(h.context.manifest);
    Object.assign(h.context.authorization, { manifestDigest: model.fingerprintInput(h.context.manifest),
        inputDigest: h.context.manifest.approvedInputDigest });
    delete h.deps.intake;
    h.deps.intakeTransport = transport;
    h.deps.approval.readTrustedCheckpoints = async () => [];
    h.deps.approval.authenticateRecord = async () => false;
    let reads = 0;
    h.deps.approval.readSource = async source => {
        reads++;
        assert.equal(source.sha256, f.digest('printfn "ok"'));
        return Buffer.from('printfn "ok"');
    };
    const work = await h.advance();
    assert.equal(work.status, "work", JSON.stringify(work));
    assert.equal(work.reservation.request.phase, "stable");
    assert.equal(reads, 1);
    assert.equal(h.calls.length, 0);
});

test("worker seam preserves original FSI/project inputs and charges before invoking existing runners", async t => {
    for (const kind of ["fsi", "project"]) await t.test(kind, async t => {
        const h = await fixture(t);
        if (kind === "project") {
            const m = h.context.manifest;
            m.kind = kind;
            m.sources[0].path = "fixtures/repro.fsproj";
            m.matrix[0].project = m.sources[0].path;
            m.matrix[0].command = ["run-project", m.sources[0].path];
            f.seal(m);
            Object.assign(h.context.authorization, { manifestDigest: model.fingerprintInput(m), inputDigest: m.approvedInputDigest });
        }
        const clock = () => new Date(h.context.now);
        let raw, acquisitionCalls = 0, sourceCalls = 0, reproductionCalls = 0;
        const worker = { sourceDirectory: "immutable-reviewed-bytes", storageDirectory: "isolated-storage",
            reproductionToolchain: { invariant: true }, sampleRuntimes: { "net10.0": { name: "Microsoft.NETCore.App", version: "10.0.0" } } };
        const dependencies = { ...h.deps, worker, clock,
            acquireToolchain: async () => {
                acquisitionCalls++;
                const cp = (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
                assert.equal(cp.state.budget.consumed.workerUnits, 1);
                assert.equal(cp.state.budget.pendingWork.status, "running");
                return { status: "acquired", identity: { requested: {}, archive: { sha256: h.context.stableSdk.archives[0].sha256 } } };
            },
            verifyToolchainIdentity: () => ({ status: "verified" }),
            inventoryReleased: async (identity, id) => ({ kind: "released", buildInstanceId: id }),
            runReproduction: async options => {
                reproductionCalls++;
                assert.deepEqual(options.manifest, h.context.manifest);
                assert.deepEqual(options.manifest.producer, h.context.manifest.producer);
                assert.equal(options.sourceDirectory, worker.sourceDirectory);
                return raw = { outcome: f.observation("released", "product-failure", h.context.manifest).outcome,
                    cells: f.observation("released", "product-failure", h.context.manifest).cells };
            },
            testSourceRevision: async options => {
                sourceCalls++;
                assert.deepEqual(options.manifest.matrix, h.context.manifest.matrix);
                assert.equal(options.sourceDirectory, worker.sourceDirectory);
                assert.equal(options.reproductionToolchain, worker.reproductionToolchain);
                assert.deepEqual(options.sampleRuntimes, worker.sampleRuntimes);
                assert.equal(options.request.sourceSha, h.shas.at(-1));
                assert.equal(options.request.allocation.buildTimeoutMs, 90 * 60000);
                assert.equal(options.request.allocation.workerTimeoutMs, 110 * 60000);
                raw = { status: "tested", actualSourceSha: options.request.sourceSha, buildInstanceId: options.buildInstanceId,
                    outcome: f.observation("source", "product-failure", h.context.manifest).outcome,
                    reproduction: { cells: f.observation("source", "product-failure", h.context.manifest).cells } };
                return raw;
            },
            collectObservation: async ({ reservation, result }) => {
                assert.equal(result, raw);
                return { observation: h.observationFor({ reservation }, "product-failure"), evidence: [] };
            },
        };
        for (const phase of ["stable", "main"]) {
            const work = await h.advance();
            assert.equal(work.status, "work", JSON.stringify(work));
            assert.equal(work.reservation.request.phase, phase);
            await assert.rejects(executeInvestigationWork({ ...work, reservation: { ...work.reservation,
                request: { ...work.reservation.request, target: { sourceSha: f.sha("unreserved target") } } } }, dependencies),
            /no longer current/);
            await executeInvestigationWork(work, dependencies);
            await assert.rejects(executeInvestigationWork(work, dependencies), /no longer current/);
        }
        assert.equal(acquisitionCalls, 1);
        assert.equal(reproductionCalls, 1);
        assert.equal(sourceCalls, 1);
        assert.equal(h.calls.length, 0);
    });
});
