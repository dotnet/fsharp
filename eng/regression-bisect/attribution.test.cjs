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
const { advanceInvestigation } = require("./engine.cjs");
const { confirmBoundary, resolveCommitAssociations } = require("./attribution.cjs");

const gitExecutable = execFileSync(process.platform === "win32" ? "where.exe" : "which",
    [process.platform === "win32" ? "git.exe" : "git"], { encoding: "utf8" }).trim().split(/\r?\n/)[0];

async function fixture(t, kind = "linear") {
    const root = await fs.mkdtemp(path.join(os.tmpdir(), "attribution-test-"));
    t.after(() => fs.rm(root, { recursive: true, force: true }));
    const mirror = path.join(root, "mirror"), scratch = path.join(root, "scratch");
    await fs.mkdir(mirror); await fs.mkdir(scratch);
    await fs.writeFile(path.join(root, "config"), "");
    const git = (...args) => execFileSync(gitExecutable,
        ["-c", "core.hooksPath=", "-c", "commit.gpgSign=false", ...args], {
            cwd: mirror, encoding: "utf8", env: { ...process.env, GIT_CONFIG_NOSYSTEM: "1",
                GIT_CONFIG_GLOBAL: path.join(root, "config"), GIT_AUTHOR_NAME: "Fixture",
                GIT_AUTHOR_EMAIL: "fixture@example.invalid", GIT_COMMITTER_NAME: "Fixture",
                GIT_COMMITTER_EMAIL: "fixture@example.invalid" },
        }).trim();
    git("init", "--quiet", "--initial-branch=main");
    async function commit(name, file = "source.fs", content = name) {
        await fs.writeFile(path.join(mirror, file), content);
        git("add", "--", file); git("commit", "--quiet", "-m", name);
        return git("rev-parse", "HEAD");
    }
    const good = await commit("base");
    let related;
    if (kind === "merge" || kind === "foreign-parent") {
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
    const calls = [];
    const deps = {
        transport: h.transport, context: h.context, environments,
        execute: async (exe, args, options) => {
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
    return Object.assign(h, { deps, intent, calls, candidate, good, parents, related, read, git, observation,
        confirm: () => confirmBoundary({ intent, now: h.context.now }, deps) });
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
            const result = await h.confirm();
            assert.notEqual(result.confirmation?.classification, "verified-introducing-source-boundary", JSON.stringify(result));
            if (kind === "merge" || kind === "toolchain") {
                assert.equal(result.status, "confirmed", JSON.stringify(result));
                assert.equal(result.confirmation.classification, "verified-triggering-change");
                assert.deepEqual(h.calls.map(c => c.request.sourceSha), [h.candidate, ...h.parents]);
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

test("merge parent failure and parent confirmation budget preserve the entire parent set", async t => {
    for (const variant of ["failure", "budget", "outside"]) await t.test(variant, async t => {
        const h = await fixture(t, "merge");
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
        if (variant === "mismatch") p.merge_commit_sha = f.sha("missing");
        if (variant === "servicing") p.base.ref = "release/dev17.0";
        if (variant === "unsafe-url") p.html_url = "https://evil.invalid/pull/1";
        return { data: p };
    } };
}

test("commit association mapping never guesses a PR from incomplete or foreign evidence", async t => {
    for (const [variant, expected] of [["none", "none"], ["one", "unique"], ["several", "ambiguous"],
        ["closed", "none"], ["rate-limited", "incomplete"], ["incomplete", "incomplete"],
        ["foreign", "incomplete"], ["mismatch", "incomplete"], ["servicing", "ambiguous"], ["unsafe-url", "incomplete"]])
        await t.test(variant, async t => {
            const h = await fixture(t);
            const transport = associationTransport(h, variant);
            const r = await resolveCommitAssociations({ repository: "dotnet/fsharp", sourceSha: h.candidate,
                pinnedMainSha: h.candidate }, { transport, history: h.deps.history, execute: h.deps.execute });
            assert.equal(r.status, expected, JSON.stringify(r));
            assert.ok(r.reason);
            assert.equal(r.sourceSha, h.candidate);
            model.validateAssociations(r);
            if (variant === "several") assert.equal(r.pulls.length, 2);
            assert.ok(transport.requests.every(([r]) => r.startsWith("GET ")));
        });
});
