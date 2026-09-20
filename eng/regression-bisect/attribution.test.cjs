const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs/promises");
const path = require("node:path");
const { execFileSync, spawnSync } = require("node:child_process");
const f = require("./fixtures/contracts.cjs");
const { harness } = require("./fixtures/checkpoint.cjs");
const model = require("./model.cjs");
const state = require("./state.cjs");
const { advanceInvestigation, executeInvestigationWork, historyWorkspace } = require("./engine.cjs");
const { confirmBoundary, resolveCommitAssociations } = require("./attribution.cjs");

const gitExecutable = execFileSync(process.platform === "win32" ? "where.exe" : "which",
    [process.platform === "win32" ? "git.exe" : "git"], { encoding: "utf8" }).trim().split(/\r?\n/)[0];

async function fixture(t, kind = "linear", changedPaths) {
    const root = await fs.mkdtemp(path.join(__dirname, ".attribution-test-"));
    t.after(() => fs.rm(root, { recursive: true, force: true }));
    const mirror = path.join(root, "mirror"), scratch = path.join(root, "scratch");
    await fs.mkdir(mirror); await fs.mkdir(scratch);
    await fs.writeFile(path.join(root, "config"), "");
    const gitInput = (args, input, raw = false) => {
        const output = execFileSync(gitExecutable,
        ["-c", "core.hooksPath=", "-c", "commit.gpgSign=false", ...args], {
            cwd: mirror, input, encoding: "utf8", env: { ...process.env, GIT_CONFIG_NOSYSTEM: "1",
                GIT_CONFIG_GLOBAL: path.join(root, "config"), GIT_AUTHOR_NAME: "Fixture",
                GIT_AUTHOR_EMAIL: "fixture@example.invalid", GIT_COMMITTER_NAME: "Fixture",
                GIT_COMMITTER_EMAIL: "fixture@example.invalid" },
        });
        return raw ? output : output.trim();
    };
    const git = (...args) => gitInput(args);
    const gitRaw = (...args) => gitInput(args, undefined, true);
    git("init", "--quiet", "--initial-branch=main");
    async function commit(name, file = "source.fs", content = name) {
        await fs.writeFile(path.join(mirror, file), content);
        git("add", "--", file); git("commit", "--quiet", "-m", name);
        return git("rev-parse", "HEAD");
    }
    const good = await commit("base");
    let related;
    if (changedPaths) {
        const blob = git("hash-object", "-w", "source.fs");
        const tree = gitInput(["mktree", "-z"], Buffer.concat(["source.fs", ...changedPaths].map(file =>
            Buffer.concat([Buffer.from(`100644 blob ${blob}\t`), Buffer.from(file), Buffer.from([0])]))));
        git("update-ref", "HEAD", git("commit-tree", tree, "-p", good, "-m", "regression"));
    } else if (kind === "merge" || kind === "foreign-parent") {
        git("checkout", "--quiet", "-b", "side");
        related = await commit("side", "side.fs");
        git("checkout", "--quiet", "main");
        await commit("main-side", "main.fs");
        git("merge", "--quiet", "--no-ff", "side", "-m", "merge integration");
    } else if (kind === "backport") {
        git("checkout", "--quiet", "-b", "servicing");
        related = await commit("upstream change");
        git("checkout", "--quiet", "main");
        await commit("main-side", "main.fs");
        git("cherry-pick", "--quiet", "-x", related);
    } else if (kind === "revert") {
        related = await commit("change");
        git("revert", "--no-edit", related);
        git("revert", "--no-edit", git("rev-parse", "HEAD"));
    } else if (kind !== "root") {
        await commit("regression", kind === "toolchain" ? "global.json" : "source.fs");
    }
    const candidate = git("rev-parse", "HEAD");
    const parents = git("rev-list", "--parents", "-n", "1", candidate).split(" ").slice(1);
    const h = harness();
    h.context.pinnedMainSha = candidate;
    h.context.manifest.searchHints.sourceShas = [good];
    h.context.manifest.sources.push({ path: "fixtures/producer.dll", sha256: f.digest("producer") });
    f.seal(h.context.manifest);
    Object.assign(h.context.authorization, { manifestDigest: model.fingerprintInput(h.context.manifest),
        inputDigest: h.context.manifest.approvedInputDigest });
    const environments = Object.fromEntries(h.context.manifest.matrix.map(c => [c.id,
        { os: c.os, architecture: c.architecture, imageDigest: f.digest("runner image") }]));
    h.context.environmentFingerprint = model.fingerprintInput(environments);
    await h.start();
    const read = async () => (await state.loadCheckpoint(h.transport, h.context)).checkpoint;
    function observation(sourceSha, id, outcome = "product-failure", kind = "source") {
        const o = f.observation(kind, outcome, h.context.manifest, sourceSha);
        o.buildInstanceId = id;
        o.provenance.runId = h.context.owner.runId;
        for (const c of o.cells) {
            for (const a of Object.values(c.artifacts)) a.buildInstanceId = id;
            if (kind === "source") c.buildToolchain = f.buildToolchain(sourceSha, c.environment.os);
        }
        return o;
    }
    for (const [endpoint, outcome, sha] of [["stable", "product-failure", null],
        ["main", "product-failure", candidate], ["good", "pass", good]]) {
        const kind = endpoint === "stable" ? "released" : "source";
        const r = await state.reserveWork(h.transport, h.context, { generation: (await read()).generation,
            attemptId: `search-${endpoint}`, request: { kind,
                target: kind === "source" ? { sourceSha: sha } : { sdk: h.context.stableSdk },
                endpoint, phase: endpoint === "good" ? "good-search" : endpoint } });
        await state.commitObservation(h.transport, h.context, { generation: r.generation,
            attemptId: r.id, observation: observation(sha, r.id, outcome, kind), evidence: [] });
    }
    const calls = [], gitCalls = [];
    const deps = {
        transport: h.transport, context: h.context, environments,
        execute: async (exe, args, options) => {
            gitCalls.push(args);
            const r = spawnSync(exe, args, { ...options, encoding: "utf8", timeout: options.timeoutMs, maxBuffer: options.maxBytes });
            if (r.error) throw r.error;
            return { status: "completed", complete: !r.signal, exitCode: r.status, stdout: r.stdout, stderr: r.stderr };
        },
        history: { gitExecutable, mirrorDirectory: mirror, scratchDirectory: scratch, mainRef: "refs/heads/main" },
        intake: { readIssueSnapshot: async () => ({ status: "complete", snapshot: {
            discussionFingerprint: h.context.issueFingerprint,
            input: { discussion: { issue: h.context.issue }, reproduction: { manifest: h.context.manifest } },
            investigations: { status: "complete", active: [] },
        } }) },
        approval: { readSource: async source => Buffer.from(source.path.endsWith(".dll") ? "producer" : 'printfn "ok"'),
            authenticateRecord: async () => false, readTrustedCheckpoints: async () => [] },
        toolchains: { resolveLatestStableSdk: async () => ({ status: "resolved", toolchain: {
            sdkVersion: h.context.stableSdk.version,
            metadata: [{ url: h.context.stableSdk.releaseIndexUrl, sha256: h.context.stableSdk.releaseIndexDigest }],
        } }) },
        platform: { os: "linux", architecture: "x64", rid: "linux-x64" },
        clock: () => new Date(h.context.now), worker: { reproductionToolchain: { frozen: true } },
        testSourceRevision: async options => {
            calls.push(options);
            const cp = await read();
            assert.equal(cp.pending.id, options.buildInstanceId);
            assert.ok(cp.pending.executionGeneration);
            const o = observation(options.request.sourceSha, options.buildInstanceId,
                options.request.sourceSha === candidate ? "product-failure" : "pass");
            return { status: "tested", actualSourceSha: options.request.sourceSha, buildInstanceId: options.buildInstanceId,
                outcome: o.outcome, reproduction: { cells: o.cells }, observation: o };
        },
        collectObservation: async ({ result }) => ({ observation: result.observation, evidence: [] }),
    };
    let intent = await advanceInvestigation({ now: h.context.now }, deps);
    if (kind === "root") {
        const cp = await state.recordSearchProgress(h.transport, h.context, { generation: (await read()).generation,
            phase: "blocked", interval: { goodSha: good, badSha: candidate, gitLog: [] } });
        intent = { status: "blocked", requiresFreshConfirmation: true, generation: cp.generation,
            interval: { ...cp.state.interval, remaining: [candidate] } };
    }
    return Object.assign(h, { deps, intent, calls, gitCalls, candidate, good, parents, related, read, git, gitRaw, observation,
        confirm: async () => {
            let next = intent;
            for (let i = 0; i < 18; i++) {
                const result = await confirmBoundary({ intent: next, now: h.context.now }, deps);
                if (result.status !== "work") return result;
                try { await executeInvestigationWork(result, deps); }
                catch (error) {
                    const cp = await read();
                    return { status: "unconfirmed", reason: error.message, confirmation: cp.state.confirmation };
                }
                const cp = await read();
                next = { ...next, status: cp.state.phase, generation: cp.generation };
            }
            assert.fail("confirmation did not terminate");
        } });
}

test("fresh independent candidate and parent, not search evidence, prove a linear boundary", async t => {
    const h = await fixture(t);
    assert.equal(h.intent.status, "confirmation");
    const before = await h.read();
    const result = await h.confirm();
    assert.equal(result.status, "confirmed", JSON.stringify(result));
    assert.equal(result.confirmation.classification, "verified-introducing-source-boundary");
    assert.deepEqual(h.calls.map(c => c.request.sourceSha), [h.candidate, h.good]);
    assert.equal(new Set(h.calls.map(c => c.buildInstanceId)).size, 2);
    assert.ok(h.calls.every(c => c.request.phase === "confirmation" &&
        model.fingerprintInput(c.manifest) === model.fingerprintInput(h.context.manifest)));
    const cp = await h.read();
    assert.deepEqual(cp.state.confirmation, result.confirmation);
    assert.equal(cp.state.budget.consumed.workerUnits, before.state.budget.consumed.workerUnits + 2);
    for (const d of [result.confirmation.candidateObservationDigest, ...result.confirmation.parents.map(p => p.observationDigest)]) {
        assert.ok(!before.observations[d]);
        const o = cp.observations[d].data;
        assert.equal(o.cached, false);
        assert.ok(o.cells.every(c => c.attempts.length === 2 && c.buildToolchain.sourceSha === o.target.sourceSha));
    }
    assert.equal(result.confirmation.latentRootCause, "unproven");
    assert.deepEqual(await fs.readdir(h.deps.history.scratchDirectory), []);
    for (const mutate of [
        c => { c.markdown = "arbitrary claim"; },
        c => { c.latentRootCause = h.candidate; },
        c => { c.candidateObservationDigest = c.searchObservationDigest; },
        c => { c.parents = []; },
        c => { c.epoch = f.digest("old epoch"); },
        c => { c.startGeneration = cp.generation; },
    ]) {
        const forged = structuredClone(cp);
        mutate(forged.state.confirmation);
        assert.throws(() => model.validateConfirmationEvidence(forged));
    }
});

test("durable confirmation selects one fresh unit and resumes without replaying accepted builds", async t => {
    for (const kind of ["linear", "merge"]) await t.test(kind, async t => {
        const h = await fixture(t, kind), before = await h.read();
        let result = await confirmBoundary({ intent: h.intent, now: h.context.now }, h.deps);
        for (const [index, sourceSha] of [h.candidate, ...h.parents].entries()) {
            assert.equal(result.status, "work", JSON.stringify(result));
            assert.equal(result.reservation.request.target.sourceSha, sourceSha);
            assert.equal(h.calls.length, index);
            assert.equal((await h.read()).state.budget.consumed.workerUnits, before.sequence + index + 1);
            assert.equal((await h.read()).state.phase, "confirmation");
            assert.equal((await h.read()).state.confirmation.classification, "unconfirmed-candidate");
            await executeInvestigationWork(result, h.deps);
            h.nextRun();
            h.gitCalls.length = 0;
            const next = await advanceInvestigation({ now: h.context.now }, h.deps);
            assert.equal(next.status, "confirmation", JSON.stringify(next));
            result = await confirmBoundary({ intent: next, now: h.context.now }, h.deps);
            assert.equal(h.gitCalls.filter(args => args.includes("bisect")).length, 0);
        }
        assert.equal(result.status, "confirmed", JSON.stringify(result));
        assert.equal(h.calls.length, h.parents.length + 1);
        const cp = await h.read();
        assert.equal(cp.state.budget.consumed.reservedMinutes, (before.sequence + h.calls.length) * 110);
        assert.ok(Buffer.byteLength(h.comment.body) < 48 * 1024);
        assert.equal(model.validateConfirmationEvidence(cp), true);
        t.diagnostic(JSON.stringify({ kind, generation: cp.generation, consumed: cp.state.budget.consumed,
            reservations: cp.records.map(r => ({ id: r.id, source: r.request.target.sourceSha, status: r.status,
                owner: r.owner, generation: r.generation, executionGeneration: r.executionGeneration })) }));
    });
});

test("confirmation transaction cancellation retains charges and committed evidence", async t => {
    for (const stage of ["plan", "reservation", "claim", "observation"]) await t.test(stage, async t => {
        const h = await fixture(t), before = await h.read();
        const update = h.transport.updateComment;
        let failed = false;
        h.transport.updateComment = async (id, body) => {
            const written = await update(id, body);
            const decoded = await state.authenticateDecodedRecord(h.transport, h.context, written);
            const cp = decoded.payload;
            if (!failed && (stage === "plan" ? cp.state.confirmation && !cp.pending :
                stage === "reservation" ? cp.pending && !cp.pending.executionGeneration :
                stage === "claim" ? cp.pending?.executionGeneration :
                cp.records.some(r => r.request.phase === "confirmation" && r.status === "committed"))) {
                failed = true;
                throw new Error(`lost ${stage} acknowledgement`);
            }
            return written;
        };
        const work = await confirmBoundary({ intent: h.intent, now: h.context.now }, h.deps);
        if (["claim", "observation"].includes(stage))
            await assert.rejects(executeInvestigationWork(work, h.deps), new RegExp(`lost ${stage}`));
        else assert.notEqual(work.status, "work");
        assert.equal(failed, true);
        h.transport.updateComment = update;
        const interrupted = await h.read();
        h.nextRun();
        const next = await advanceInvestigation({ now: h.context.now }, h.deps);
        const resumed = await confirmBoundary({ intent: next, now: h.context.now }, h.deps);
        const cp = await h.read();
        if (["reservation", "claim"].includes(stage)) {
            assert.equal(resumed.status, "unconfirmed", JSON.stringify(resumed));
            assert.match(resumed.reason, /budget/);
            assert.equal(cp.pending, null);
            assert.equal(cp.records.at(-1).status, "cancelled");
            assert.deepEqual(cp.state.budget.consumed, interrupted.state.budget.consumed);
            assert.equal(h.calls.length, 0);
        } else {
            assert.equal(resumed.status, "work", JSON.stringify(resumed));
            assert.equal(resumed.reservation.request.target.sourceSha, stage === "plan" ? h.candidate : h.good);
            assert.equal(cp.sequence, before.sequence + (stage === "plan" ? 1 : 2));
            assert.equal(h.calls.length, stage === "plan" ? 0 : 1);
            if (stage === "observation") assert.ok(cp.state.confirmation.candidateObservationDigest);
        }
        assert.equal(cp.state.budget.consumed.reservedMinutes, cp.sequence * 110);
        assert.equal(model.validateConfirmationEvidence(cp), true);
    });
});

test("concurrent confirmation selection and claims have one durable winner under the issue lock", async t => {
    const h = await fixture(t);
    const results = await Promise.all([
        h.lock(() => confirmBoundary({ intent: h.intent, now: h.context.now }, h.deps)),
        h.lock(() => confirmBoundary({ intent: h.intent, now: h.context.now }, h.deps)),
    ]);
    assert.equal(results.filter(r => r.status === "work").length, 1);
    const work = results.find(r => r.status === "work");
    const claims = await Promise.all([1, 2].map(() => h.lock(() => state.beginWork(h.transport, h.context,
        { generation: work.reservation.generation, attemptId: work.reservation.id }))));
    assert.equal(claims.filter(r => r.execute).length, 1);
    assert.equal((await h.read()).sequence, 4);
    assert.equal(h.calls.length, 0);
    const active = await advanceInvestigation({ now: h.context.now }, h.deps);
    assert.equal(active.status, "blocked");
    assert.match(active.reason, /active owner/);
});

test("resumed confirmation rechecks moving inputs without refunding accepted work", async t => {
    for (const change of ["main", "stable", "discussion", "required-evidence"]) await t.test(change, async t => {
        const h = await fixture(t);
        const candidate = await confirmBoundary({ intent: h.intent, now: h.context.now }, h.deps);
        await executeInvestigationWork(candidate, h.deps);
        const before = await h.read();
        if (change === "main") h.git("commit", "--quiet", "--allow-empty", "-m", "new main");
        if (change === "stable") h.deps.toolchains.resolveLatestStableSdk = async () => ({
            status: "resolved", toolchain: { sdkVersion: "10.0.999", metadata: [] },
        });
        if (change === "discussion") h.context.issueFingerprint = f.digest("edited discussion");
        if (change === "required-evidence") {
            const cp = await h.read();
            cp.observations[cp.records.at(-1).observationDigest].evidence = [
                { required: true, sha256: f.digest("durable evidence"), url: "https://example.invalid/required" },
            ];
            h.checkpoint(cp);
            h.transport.readEvidence = async () => null;
        }
        h.gitCalls.length = 0;
        const next = await advanceInvestigation({ now: h.context.now }, h.deps);
        assert.equal(h.gitCalls.filter(args => args.includes("bisect")).length, 0);
        const after = (await state.authenticateDecodedRecord(h.transport, h.context, h.comment)).payload;
        assert.equal(after.state.budget.allowanceId, before.state.budget.allowanceId);
        if (change === "main") {
            assert.equal(next.status, "work", JSON.stringify(next));
            assert.equal(next.reservation.request.endpoint, "main");
            assert.equal(after.state.confirmation, undefined);
            assert.equal(after.sequence, before.sequence + 1);
        } else {
            assert.equal(next.status, "blocked", JSON.stringify(next));
            assert.deepEqual(after.state.budget.consumed, before.state.budget.consumed);
        }
        assert.equal(h.calls.length, 1);
        assert.deepEqual(await fs.readdir(h.deps.history.scratchDirectory), []);
    });
});

test("confirmation capacity uses the clock after history inspection, not invocation time", async t => {
    const h = await fixture(t), before = await h.read();
    before.deadline = new Date(Date.parse(h.context.now) + 220 * 60000).toISOString();
    h.checkpoint(before);
    h.intent.generation = before.generation;
    h.deps.clock = () => new Date(Date.parse(h.context.now) + 1);
    const result = await confirmBoundary({ intent: h.intent, now: h.context.now }, h.deps);
    assert.equal(result.status, "unconfirmed");
    assert.match(result.reason, /budget/);
    assert.deepEqual((await h.read()).state.budget.consumed, before.state.budget.consumed);
    assert.equal(h.calls.length, 0);
});

test("a fresh process consumes the saved candidate and selects only its unbuilt parent", async t => {
    const h = await fixture(t);
    const candidate = await confirmBoundary({ intent: h.intent, now: h.context.now }, h.deps);
    await executeInvestigationWork(candidate, h.deps);
    h.nextRun();
    const child = spawnSync(process.execPath, ["-e", `
        const fs = require("node:fs");
        const { harness } = require("./fixtures/checkpoint.cjs");
        const engine = require("./engine.cjs");
        const { confirmBoundary } = require("./attribution.cjs");
        (async () => {
            const input = JSON.parse(fs.readFileSync(0, "utf8")), h = harness();
            const keys = h.context.keys;
            Object.assign(h.context, input.context, { keys });
            h.comment = input.comment;
            h.statuses.splice(0, h.statuses.length, ...input.statuses);
            h.runs.clear();
            for (const [id, run] of input.runs) h.runs.set(id, run);
            const deps = {
                context: h.context, transport: h.transport, history: input.history,
                environments: input.environments, platform: input.platform,
                clock: () => new Date(h.context.now),
                intake: { readIssueSnapshot: async () => ({ status: "complete", snapshot: {
                    discussionFingerprint: h.context.issueFingerprint,
                    input: { discussion: { issue: h.context.issue }, reproduction: { manifest: h.context.manifest } },
                    investigations: { status: "complete", active: [] },
                } }) },
                approval: { readSource: async () => Buffer.from("producer"),
                    authenticateRecord: async () => false, readTrustedCheckpoints: async () => [] },
                toolchains: { resolveLatestStableSdk: async () => ({ status: "resolved", toolchain: {
                    sdkVersion: h.context.stableSdk.version, metadata: [{
                        url: h.context.stableSdk.releaseIndexUrl, sha256: h.context.stableSdk.releaseIndexDigest,
                    }],
                } }) },
            };
            const intent = await engine.advanceInvestigation({ now: h.context.now }, deps);
            const result = await confirmBoundary({ intent, now: h.context.now }, deps);
            process.stdout.write(JSON.stringify({ result, comment: h.comment, statuses: h.statuses,
                bisectCalls: intent.commands.filter(c => c.args.includes("bisect")).length }));
        })().catch(error => { console.error(error); process.exitCode = 1; });
    `], { cwd: __dirname, encoding: "utf8", timeout: 60000, maxBuffer: 1024 * 1024,
        input: JSON.stringify({ context: { ...h.context, keys: undefined }, comment: h.comment, statuses: h.statuses,
            runs: [...h.runs], history: h.deps.history, environments: h.deps.environments, platform: h.deps.platform }) });
    assert.equal(child.status, 0, child.stderr);
    const restarted = JSON.parse(child.stdout);
    assert.equal(restarted.result.status, "work", JSON.stringify(restarted.result));
    assert.equal(restarted.result.reservation.request.target.sourceSha, h.good);
    assert.equal(restarted.bisectCalls, 0);
    h.comment = restarted.comment;
    h.statuses.splice(0, h.statuses.length, ...restarted.statuses);
    const cp = await h.read();
    assert.equal(cp.sequence, 5);
    assert.equal(cp.state.confirmation.candidateObservationDigest, cp.records.find(r => r.id === candidate.reservation.id).observationDigest);
    await executeInvestigationWork(restarted.result, h.deps);
    const intent = await advanceInvestigation({ now: h.context.now }, h.deps);
    const result = await confirmBoundary({ intent, now: h.context.now }, h.deps);
    assert.equal(result.status, "confirmed", JSON.stringify(result));
    assert.deepEqual(h.calls.map(c => c.request.sourceSha), [h.candidate, h.good]);
    const repeated = await confirmBoundary({
        intent: await advanceInvestigation({ now: h.context.now }, h.deps), now: h.context.now,
    }, h.deps);
    assert.deepEqual(repeated, result);
    assert.equal((await h.read()).sequence, 5);
});

test("fresh confirmation retains bounded Git display paths without treating them as executable assets", async t => {
    for (const [name, paths, toolchain] of [
        ["space", ["source with space.fs"], false],
        ["unicode", ["λ-日本語.fs"], false],
        ["controls and Markdown", ["tab\tline\n[link](x)`\u0001.fs"], false],
        ["non-UTF8", [Buffer.from([0x66, 0x80, 0xff, 0x2e, 0x66, 0x73])], false],
        ["258 files", Array.from({ length: 258 }, (_, i) => `file-${String(i).padStart(3, "0")}.fs`), false],
        ["byte-limited sample", Array.from({ length: 24 }, (_, i) => `${i}-${"λ".repeat(60)}.fs`), false],
        ["toolchain after truncation", [...Array.from({ length: 257 }, (_, i) => `a-${i}.fs`), "z.props"], true],
        ["quoted toolchain", ["λ\nbuild.fsproj"], true],
        ["edge spaces", [" leading.fs", "trailing.fs "], false],
    ]) await t.test(name, async t => {
        const h = await fixture(t, "linear", paths);
        const before = await h.read();
        const args = ["-c", "core.quotePath=true", "diff-tree", "--root", "-m",
            "--no-commit-id", "--name-only", "-r", h.candidate];
        const output = h.gitRaw(...args);
        const history = await historyWorkspace(h.deps.history, h.deps.execute);
        try {
            assert.equal(await history.git(args, [0], { raw: true }), output);
            assert.equal(await history.git(["rev-parse", "HEAD"]), h.candidate);
        } finally { await history.dispose(); }
        const r = await h.confirm();
        const truncated = paths.length > 256 || name === "byte-limited sample";
        assert.equal(r.status, truncated ? "unconfirmed" : "confirmed", JSON.stringify(r));
        assert.equal(r.confirmation.classification, truncated ? "unconfirmed-candidate" :
            toolchain ? "verified-triggering-change" : "verified-introducing-source-boundary");
        const commit = r.confirmation.history.commits.find(c => c.sha === h.candidate);
        assert.equal(commit.changedPathsSummary.encoding, "git-quoted");
        assert.equal(commit.changedPathsSummary.totalCount, paths.length);
        assert.equal(commit.changedPathsSummary.toolchainCoupled, toolchain);
        assert.equal(commit.changedPathsSummary.truncated, truncated);
        assert.equal(commit.changedPathsSummary.sha256, model.fingerprintInput(output.slice(0, -1).split("\n").sort()));
        assert.ok(commit.changedPaths.length <= 256);
        assert.ok(Buffer.byteLength(JSON.stringify(commit.changedPaths)) <= 4096);
        assert.ok(commit.changedPaths.every(p => !/[\u0000-\u001f\u007f]/.test(p)));
        if (name === "edge spaces") assert.deepEqual(commit.changedPaths, [" leading.fs", "trailing.fs "]);
        if (name === "non-UTF8") assert.deepEqual(commit.changedPaths, ['"f\\200\\377.fs"']);
        if (name === "unicode") assert.ok(commit.changedPaths[0].includes("\\"));
        if (name === "toolchain after truncation") assert.ok(!commit.changedPaths.includes("z.props"));
        assert.deepEqual((await h.read()).state.confirmation, r.confirmation);
        assert.equal((await h.read()).state.budget.consumed.workerUnits, before.state.budget.consumed.workerUnits + 2);
        assert.ok(Buffer.byteLength(h.comment.body) < 48 * 1024);
        if (truncated) assert.match(r.reason, /incomplete.*path/i);
    });
});

test("malformed or oversized history persists bounded uncertainty rather than revalidating rejected data", async t => {
    for (const variant of ["bad-parent", "too-many-parents", "malformed-path", "path-bytes",
        "129-commits", "4096-remaining", "4097-remaining", "bad-remaining"])
        await t.test(variant, async t => {
            const h = await fixture(t);
            const before = await h.read();
            const remaining = variant === "bad-remaining" ? [h.candidate, "not-a-sha"] :
                [h.candidate, ...Array.from({ length: Number.parseInt(variant) - 1 || 0 }, (_, i) => f.sha(`revision-${i}`))];
            if (variant.includes("remaining") || variant === "129-commits") h.intent.interval.remaining = remaining;
            const run = h.deps.execute;
            h.deps.execute = async (exe, args, options) => {
                const result = await run(exe, args, options);
                if (args.includes("rev-list") && args.includes("--parents") && args.at(-1) === h.candidate) {
                    if (variant === "bad-parent") result.stdout = `${h.candidate} invalid\n`;
                    if (variant === "too-many-parents") result.stdout = `${h.candidate} ${Array.from({ length: 17 }, (_, i) => f.sha(`parent-${i}`)).join(" ")}\n`;
                }
                if (args.includes("diff-tree") && args.at(-1) === h.candidate) {
                    if (variant === "malformed-path") result.stdout = '"unterminated\n';
                    if (variant === "path-bytes") result.stdout = `${"x".repeat(1024 * 1024 + 1)}\n`;
                }
                if (args.includes("--max-count=4097")) result.stdout = remaining.join("\n") + "\n";
                return result;
            };
            const r = await h.confirm();
            assert.equal(r.status, "unconfirmed", JSON.stringify(r));
            assert.equal(model.validateConfirmation(r.confirmation), true);
            assert.equal(r.confirmation.interval.goodSha, h.good);
            assert.equal(r.confirmation.interval.badSha, h.candidate);
            if (["bad-parent", "too-many-parents", "malformed-path", "path-bytes"].includes(variant)) {
                assert.equal(r.confirmation.interval.remaining, null);
                assert.equal(r.confirmation.interval.remainingSummary, undefined);
            } else {
                assert.ok(r.confirmation.interval.remaining.includes(h.candidate));
                assert.ok(r.confirmation.interval.remaining.length <= 4096);
            }
            assert.ok(r.confirmation.history.commits.length <= 128);
            const cp = await h.read();
            assert.deepEqual(cp.state.confirmation, r.confirmation);
            assert.equal(cp.state.phase, "blocked");
            assert.deepEqual(cp.state.interval, before.state.interval);
            assert.deepEqual(cp.observations, before.observations);
            assert.deepEqual(cp.state.budget.consumed, before.state.budget.consumed);
            assert.ok(Buffer.byteLength(h.comment.body) < 48 * 1024);
            assert.equal(h.calls.length, 0);
            if (variant.startsWith("409")) {
                assert.equal(r.confirmation.interval.remainingSummary.totalCount, remaining.length);
                assert.equal(r.confirmation.interval.remainingSummary.truncated, true);
                assert.equal(r.confirmation.interval.remainingSummary.sha256, model.fingerprintInput(remaining));
            }
        });
});

test("post-build confirmation rejection preserves charged independent observations in a valid checkpoint", async t => {
    for (const variant of ["malformed-history", "oversized-associations"]) await t.test(variant, async t => {
        const h = await fixture(t), before = await h.read();
        if (variant === "malformed-history") {
            const save = state.recordConfirmation;
            let first = true;
            t.after(() => { state.recordConfirmation = save; });
            state.recordConfirmation = async (transport, context, request) => {
                if (first && request.phase !== "confirmation") {
                    first = false;
                    request.confirmation.history.commits[0].changedPaths = ["invalid\npath"];
                }
                return save(transport, context, request);
            };
        } else {
            const transport = associationTransport(h, "one"), request = transport.request;
            transport.request = async (route, args) => {
                if (route.endsWith("/commits/{commit_sha}/pulls"))
                    return { data: Array.from({ length: 100 }, (_, i) => ({ number: i + 1 })), headers: { link: "" } };
                const response = await request(route, args);
                if (!route.endsWith("/commits"))
                    response.data.base.ref = Array.from({ length: 4 }, (_, i) => f.digest(`ref-${args.pull_number}-${i}`)).join("");
                return response;
            };
            h.deps.associationTransport = transport;
        }
        const r = await h.confirm(), cp = await h.read();
        assert.equal(r.status, "unconfirmed", JSON.stringify(r));
        assert.match(r.reason, variant === "malformed-history" ? /display paths/ : /48 KiB/);
        assert.equal(model.validateConfirmationEvidence(cp), true);
        assert.deepEqual(cp.state.confirmation, r.confirmation);
        assert.equal(cp.state.budget.consumed.workerUnits, before.state.budget.consumed.workerUnits + 2);
        assert.equal(h.calls.length, 2);
        assert.ok(r.confirmation.candidateObservationDigest);
        assert.ok(r.confirmation.parents.every(p => p.observationDigest));
        assert.deepEqual(cp.state.interval, before.state.interval);
        for (const [id, observation] of Object.entries(before.observations)) assert.deepEqual(cp.observations[id], observation);
        if (variant === "oversized-associations") assert.notEqual(r.confirmation.associations?.status, "unique");
        assert.ok(Buffer.byteLength(h.comment.body) < 48 * 1024);
    });
});

test("fresh failures and identity contradictions never yield unique blame", async t => {
    const variants = ["candidate-pass", "parent-fail", "flake", "skip", "cached", "reused-build",
        "wrong-sha", "changed-matrix", "changed-producer", "changed-environment", "changed-sdk",
        "missing-toolchain", "original-attempts"];
    for (const variant of variants) await t.test(variant, async t => {
        const h = await fixture(t);
        const run = h.deps.testSourceRevision;
        h.deps.testSourceRevision = async options => {
            const result = await run(options), o = result.observation;
            if (variant === "candidate-pass" || variant === "parent-fail" || variant === "flake" || variant === "skip") {
                const attempts = variant === "flake" ? [f.attempt("pass"), f.attempt("product-failure")] :
                    [f.attempt(variant === "candidate-pass" ? "pass" : variant === "skip" ? "untestable" : "product-failure"),
                        f.attempt(variant === "candidate-pass" ? "pass" : variant === "skip" ? "untestable" : "product-failure")];
                for (const c of o.cells) { c.attempts = attempts; c.outcome = model.classifyAttempts(attempts, h.context.manifest); }
                result.outcome = o.outcome = o.cells[0].outcome;
            }
            if (variant === "cached") o.cached = true;
            if (variant === "reused-build") result.buildInstanceId = o.buildInstanceId = "search-main";
            if (variant === "wrong-sha") result.actualSourceSha = o.target.sourceSha = h.good;
            if (variant === "changed-matrix") o.matrixFingerprint = f.digest("other matrix");
            if (variant === "changed-producer") o.cells[0].producer.sha256 = f.digest("other bytes");
            if (variant === "changed-environment") o.cells[0].environment.imageDigest = f.digest("other image");
            if (variant === "changed-sdk") o.cells[0].sdk.sha256 = f.digest("other SDK");
            if (variant === "missing-toolchain") delete o.cells[0].buildToolchain;
            if (variant === "original-attempts") result.reproduction = { cells: [{ id: o.cells[0].id, attempts: [f.attempt("pass")] }] };
            return result;
        };
        const result = await h.confirm();
        assert.notEqual(result.status, "confirmed", JSON.stringify(result));
        assert.notEqual(result.confirmation?.classification, "verified-introducing-source-boundary");
        assert.ok((await h.read()).state.budget.consumed.workerUnits > 3);
    });
});

test("untrusted intent, generation, assets and insufficient allowance cannot execute", async t => {
    for (const variant of ["generation", "candidate", "parents", "producer", "budget", "deadline", "spent", "approval"])
        await t.test(variant, async t => {
            const h = await fixture(t);
            if (variant === "generation") h.intent.generation--;
            if (variant === "candidate") h.intent.candidate = h.good;
            if (variant === "parents") h.intent.parents = [];
            if (variant === "producer") h.deps.approval.readSource = async () => Buffer.from("different producer");
            if (variant === "approval") h.context.authorization.inputDigest = f.digest("forged");
            if (variant === "spent") {
                for (const sourceSha of [h.candidate, h.good]) {
                    const r = await state.reserveWork(h.transport, h.context, { generation: (await h.read()).generation,
                        attemptId: `spent-${sourceSha}`, request: { kind: "source", target: { sourceSha },
                            endpoint: null, phase: "confirmation" } });
                    await state.commitObservation(h.transport, h.context, { generation: r.generation, attemptId: r.id,
                        observation: h.observation(sourceSha, r.id, sourceSha === h.good ? "pass" : "product-failure"), evidence: [] });
                }
                h.intent.generation = (await h.read()).generation;
            }
            if (variant === "budget" || variant === "deadline") {
                const cp = await h.read();
                cp.deadline = new Date(Date.parse(h.context.now) + (variant === "budget" ? 110 : 220) * 60000 - 1).toISOString();
                h.checkpoint(cp);
                h.intent.generation = cp.generation;
            }
            const result = await h.confirm();
            assert.notEqual(result.status, "confirmed", JSON.stringify(result));
            assert.equal(h.calls.length, 0);
        });
});

test("real merge, root, backport, revert and toolchain boundaries retain uncertainty", async t => {
    for (const kind of ["merge", "root", "backport", "revert", "toolchain"])
        await t.test(kind, async t => {
            const h = await fixture(t, kind);
            const before = await h.read();
            const result = await h.confirm();
            assert.notEqual(result.confirmation?.classification, "verified-introducing-source-boundary", JSON.stringify(result));
            if (kind === "merge" || kind === "toolchain") {
                assert.equal(result.status, "confirmed", JSON.stringify(result));
                assert.equal(result.confirmation.classification, "verified-triggering-change");
                assert.deepEqual(h.calls.map(c => c.request.sourceSha), [h.candidate, ...h.parents]);
                const cp = await h.read(), builds = h.calls.map(c => c.buildInstanceId);
                assert.equal(new Set(builds).size, h.parents.length + 1);
                assert.equal(cp.state.budget.consumed.workerUnits - before.state.budget.consumed.workerUnits, builds.length);
                assert.deepEqual([result.confirmation.candidateObservationDigest,
                    ...result.confirmation.parents.map(p => p.observationDigest)].map(d => cp.observations[d].data.buildInstanceId), builds);
                assert.ok(builds.every(id => cp.records.some(r => r.id === id && r.status === "committed" &&
                    r.executionGeneration && r.request.phase === "confirmation")));
                if (kind === "toolchain") assert.equal(result.confirmation.history.toolchainCoupled, true);
            } else {
                assert.notEqual(result.status, "confirmed");
                assert.ok(result.reason);
                if (kind !== "root") {
                    const commits = result.confirmation.history.commits;
                    assert.ok(commits.some(c => c.sha === h.related));
                    assert.equal(commits.find(c => c.sha === h.related).patchId,
                        commits.find(c => c.sha === h.candidate).patchId);
                }
            }
        });
});

test("cached or reused parent-only builds cannot confirm linear or merge boundaries", async t => {
    for (const [kind, parentIndex] of [["linear", 0], ["merge", 0], ["merge", 1]])
        for (const variant of ["cached", "reused-build"]) await t.test(`${kind} parent ${parentIndex} ${variant}`, async t => {
            const h = await fixture(t, kind), before = await h.read();
            const run = h.deps.testSourceRevision;
            h.deps.testSourceRevision = async options => {
                const r = await run(options);
                if (options.request.sourceSha === h.parents[parentIndex]) {
                    if (variant === "cached") r.observation.cached = true;
                    else r.buildInstanceId = r.observation.buildInstanceId = h.calls[0].buildInstanceId;
                }
                return r;
            };
            const r = await h.confirm(), cp = await h.read();
            assert.equal(r.status, "unconfirmed", JSON.stringify(r));
            assert.equal(r.confirmation.classification, "unconfirmed-candidate");
            assert.ok(r.confirmation.candidateObservationDigest);
            assert.equal(cp.observations[r.confirmation.candidateObservationDigest].data.cached, false);
            assert.deepEqual(h.calls.map(c => c.request.sourceSha), [h.candidate, ...h.parents.slice(0, parentIndex + 1)]);
            assert.equal(cp.state.budget.consumed.workerUnits - before.state.budget.consumed.workerUnits, parentIndex + 2);
            assert.equal(new Set(h.calls.map(c => c.buildInstanceId)).size, parentIndex + 2);
            assert.ok(cp.records.filter(r => !before.records.some(old => old.id === r.id))
                .every(r => r.request.phase === "confirmation" && r.executionGeneration));
            assert.equal(model.validateConfirmationEvidence(cp), true);
        });
});

test("confirmation rejects authenticated nonmonotonic observations without fresh workers", async t => {
    const h = await fixture(t);
    const reservation = await state.reserveWork(h.transport, h.context, { generation: (await h.read()).generation,
        attemptId: "contradictory-ancestor", request: { kind: "source", target: { sourceSha: h.good },
            endpoint: null, phase: "good-search" } });
    await state.commitObservation(h.transport, h.context, { generation: reservation.generation,
        attemptId: reservation.id, observation: h.observation(h.good, reservation.id), evidence: [] });
    const cp = await state.recordSearchProgress(h.transport, h.context, { generation: (await h.read()).generation,
        phase: "confirmation", interval: (await h.read()).state.interval });
    h.intent.generation = cp.generation;
    const r = await h.confirm(), after = await h.read();
    assert.equal(r.status, "unconfirmed", JSON.stringify(r));
    assert.equal(r.confirmation.history.monotonicity, "contradicted");
    assert.match(r.reason, /nonmonotonic/);
    assert.equal(h.calls.length, 0);
    assert.deepEqual(after.state.budget.consumed, cp.state.budget.consumed);
    assert.deepEqual(after.observations, cp.observations);
    assert.deepEqual(after.state.confirmation, r.confirmation);
});

test("merge parent failure and parent confirmation budget preserve the entire parent set", async t => {
    for (const variant of ["failure", "budget", "outside"]) await t.test(variant, async t => {
        const h = await fixture(t, "merge");
        const before = await h.read();
        if (variant === "failure") {
            const run = h.deps.testSourceRevision;
            h.deps.testSourceRevision = async options => {
                const r = await run(options);
                for (const c of r.observation.cells) {
                    c.attempts = [f.attempt("product-failure"), f.attempt("product-failure")];
                    c.outcome = model.classifyAttempts(c.attempts, h.context.manifest);
                }
                r.outcome = r.observation.outcome = r.observation.cells[0].outcome;
                return r;
            };
        } else {
            const cp = await h.read();
            if (variant === "budget") cp.deadline = new Date(Date.parse(h.context.now) + 220 * 60000).toISOString();
            else cp.state.interval.goodSha = h.parents[0];
            h.checkpoint(cp);
            h.intent.generation = cp.generation;
            h.intent.interval.goodSha = cp.state.interval.goodSha;
        }
        const r = await h.confirm();
        assert.notEqual(r.status, "confirmed", JSON.stringify(r));
        assert.equal(r.confirmation.parents.length, 2);
        if (variant !== "failure") assert.equal(h.calls.length, 0);
        else {
            const cp = await h.read();
            assert.deepEqual(h.calls.map(c => c.request.sourceSha), [h.candidate, ...h.parents]);
            assert.equal(new Set(h.calls.map(c => c.buildInstanceId)).size, 3);
            assert.equal(cp.state.budget.consumed.workerUnits - before.state.budget.consumed.workerUnits, 3);
            assert.ok(r.confirmation.parents.every(p => cp.observations[p.observationDigest].data.target.sourceSha === p.sourceSha));
        }
    });
});

function associationTransport(h, variant) {
    const pull = n => ({ number: n, html_url: `https://github.com/dotnet/fsharp/pull/${n}`,
        state: "closed", merged: true, merged_at: "2026-09-01T00:00:00Z", merge_commit_sha: h.candidate,
        base: { sha: h.good, ref: "main", repo: { full_name: "dotnet/fsharp" } },
        head: { sha: h.candidate, repo: { full_name: "dotnet/fsharp" } } });
    const requests = [];
    return { requests, async request(route, args) {
        requests.push([route, args]);
        assert.equal(args.owner, "dotnet"); assert.equal(args.repo, "fsharp");
        if (variant === "rate-limited") return { status: 403, data: {} };
        if (route.endsWith("/commits/{commit_sha}/pulls")) {
            assert.equal(args.commit_sha, h.candidate);
            return { data: variant === "none" ? [] : variant === "several" ? [pull(1), pull(2)] : [pull(1)],
                headers: { link: variant === "incomplete" ? '<https://api.github.com/next>; rel="next"' : "" } };
        }
        if (route.endsWith("/commits")) return { data: [{ sha: h.candidate }], headers: { link: "" } };
        const p = pull(args.pull_number);
        if (variant === "closed") { p.merged = false; p.merged_at = null; }
        if (variant === "foreign") p.base.repo.full_name = "other/repo";
        if (variant === "empty-base") p.base.ref = "";
        if (variant === "mismatch") p.merge_commit_sha = f.sha("missing");
        if (variant === "servicing") p.base.ref = "release/dev17.0";
        if (variant === "unsafe-url") p.html_url = "https://evil.invalid/pull/1";
        return { data: p };
    } };
}

test("commit association mapping never guesses a PR from incomplete or foreign evidence", async t => {
    for (const [variant, expected] of [["none", "none"], ["one", "unique"], ["several", "ambiguous"],
        ["closed", "none"], ["rate-limited", "incomplete"], ["incomplete", "incomplete"],
        ["foreign", "incomplete"], ["empty-base", "incomplete"], ["mismatch", "incomplete"],
        ["servicing", "ambiguous"], ["unsafe-url", "incomplete"]])
        await t.test(variant, async t => {
            const h = await fixture(t);
            const transport = associationTransport(h, variant);
            const r = await resolveCommitAssociations({ repository: "dotnet/fsharp", sourceSha: h.candidate,
                pinnedMainSha: h.candidate }, { transport, history: h.deps.history, execute: h.deps.execute });
            assert.equal(r.status, expected, JSON.stringify(r));
            assert.ok(r.reason);
            assert.equal(r.sourceSha, h.candidate);
            model.validateAssociations(r);
            if (variant === "one" || variant === "several") assert.deepEqual(r.pulls,
                (variant === "one" ? [1] : [1, 2]).map(number => ({ number,
                    url: `https://github.com/dotnet/fsharp/pull/${number}`, mergeSha: h.candidate,
                    baseSha: h.good, headSha: h.candidate, baseRef: "main", relation: "merge-commit", onLineage: true })));
            assert.ok(transport.requests.every(([r]) => r.startsWith("GET ")));
        });

        await t.test("contained-commit associations require complete bounded association and membership pagination", async t => {
            const h = await fixture(t);
            h.git("commit", "--quiet", "--allow-empty", "-m", "later integration");
            const main = h.git("rev-parse", "HEAD");
            for (const [variant, expected] of [
                ["later-pages", "unique"], ["tenth-page", "unique"], ["empty-terminal", "unique"],
                ["all-closed-empty-terminal", "none"],
                ["1000-members-complete", "unique"], ["1000-members-incomplete", "incomplete"],
                ["duplicate-association", "incomplete"], ["duplicate-membership", "incomplete"],
                ["101-associations", "incomplete"], ["association-page-limit", "incomplete"],
                ["later-rate-limit-403", "incomplete"], ["later-rate-limit-429", "incomplete"],
                ["later-foreign", "incomplete"], ["later-sha-mismatch", "incomplete"], ["later-head-mismatch", "incomplete"],
            ]) await t.test(variant, async () => {
                const transport = associationTransport(h, "one"), request = transport.request;
                const pages = { associations: [], membership: [] };
                const lastAssociation = variant === "tenth-page" ? 10 : variant.endsWith("empty-terminal") ? 100 : 2;
                transport.request = async (route, args) => {
                    if (route.endsWith("/commits/{commit_sha}/pulls")) {
                        pages.associations.push(args.page);
                        assert.equal(args.per_page, 100);
                        if (variant.endsWith("empty-terminal")) return { data: args.page === 1 ?
                            Array.from({ length: 100 }, (_, i) => ({ number: i + 1 })) : [] };
                        if (variant === "101-associations") return { data: args.page === 1 ?
                            Array.from({ length: 100 }, (_, i) => ({ number: i + 1 })) : [{ number: 101 }] };
                        const number = variant === "duplicate-association" ? 1 : args.page;
                        return { data: [{ number }], headers: { link: args.page < lastAssociation ||
                            variant === "association-page-limit" ? '<https://api.github.com/next>; rel="next"' : "" } };
                    }
                    if (route.endsWith("/commits")) {
                        pages.membership.push(args.page);
                        assert.equal(args.per_page, 100);
                        if (args.page === 2 && variant.startsWith("later-rate-limit"))
                            return { status: Number(variant.slice(-3)), data: {} };
                        const end = variant === "tenth-page" || variant.startsWith("1000-") ? 10 : 2;
                        const sha = variant === "later-sha-mismatch" ? f.sha("unrelated") : h.candidate;
                        if (variant === "empty-terminal") return { data: args.page === 1 ?
                            [...Array.from({ length: 99 }, (_, i) => ({ sha: f.sha(`member-${i}`) })), { sha }] : [] };
                        const data = variant.startsWith("1000-") ?
                            Array.from({ length: 100 }, (_, i) => ({ sha: f.sha(`member-${args.page}-${i}`) })) :
                            [{ sha: f.sha(`member-${args.page}`) }];
                        if (args.page === end || variant === "duplicate-membership") data[data.length - 1] = { sha };
                        return { data, ...(variant === "1000-members-incomplete" ? {} :
                            { headers: { link: args.page < end ? '<https://api.github.com/next>; rel="next"' : "" } }) };
                    }
                    const response = await request(route, args);
                    response.data.merge_commit_sha = main;
                    if (args.pull_number !== lastAssociation || variant === "all-closed-empty-terminal") {
                        response.data.merged = false; response.data.merged_at = null;
                    }
                    if (variant === "later-foreign" && args.pull_number === lastAssociation)
                        response.data.base.repo.full_name = "other/repo";
                    if (variant === "later-sha-mismatch" || variant === "later-head-mismatch")
                        response.data.head.sha = f.sha("unrelated");
                    return response;
                };
                const r = await resolveCommitAssociations({ repository: "dotnet/fsharp", sourceSha: h.candidate, pinnedMainSha: main },
                    { transport, history: h.deps.history, execute: h.deps.execute });
                assert.equal(r.status, expected, JSON.stringify(r));
                assert.ok(pages.associations.length <= 10 && pages.membership.length <= 10);
                assert.equal(model.validateAssociations(r), true);
                if (variant === "later-sha-mismatch") assert.match(r.reason, /no exact source commit/);
                if (variant === "later-head-mismatch") assert.match(r.reason, /head\/commit evidence/);
                if (expected === "none") assert.deepEqual([pages.associations, pages.membership, r.pulls], [[1, 2], [], []]);
                if (expected === "unique") {
                    assert.deepEqual(r.pulls.map(p => [p.number, p.url, p.relation, p.onLineage]),
                        [[lastAssociation, `https://github.com/dotnet/fsharp/pull/${lastAssociation}`, "contained-commit", true]]);
                    assert.equal(pages.associations.at(-1), variant === "tenth-page" ? 10 : 2);
                    assert.equal(pages.membership.at(-1), variant === "tenth-page" || variant.startsWith("1000-") ? 10 : 2);
                }
            });
        });
});
