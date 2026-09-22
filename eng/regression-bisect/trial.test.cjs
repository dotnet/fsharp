const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs/promises");
const path = require("node:path");
const { routingFixture, checkpoint, connect, invokeCli } = require("./fixtures/controller-service.cjs");
const contracts = require("./fixtures/contracts.cjs");
const state = require("./state.cjs");
const { sign, adapter } = require("./trial.cjs");

const invoke = (args, env = {}) => invokeCli(args, env, undefined, {
    entry: "trial.cjs", kind: "trial-exit", storageFaults: false,
});

async function history(t, scenario = "linear", endpoints = {}) {
    const f = await routingFixture(t), shas = {};
    const commit = async (name, outcome, file = "source.txt") => {
        await fs.writeFile(path.join(f.read().mirrorDirectory, file), outcome);
        f.git(["add", file]); f.git(["commit", "--quiet", "--allow-empty", "-m", name]);
        return f.git(["rev-parse", "HEAD"]);
    };
    shas.base = f.git(["rev-parse", "HEAD"]);
    for (const source of f.context.manifest.sources) {
        const file = path.join(f.read().mirrorDirectory, source.path);
        await fs.mkdir(path.dirname(file), { recursive: true });
        await fs.writeFile(file, Buffer.from(f.read().files[source.path], "base64"));
        f.git(["add", source.path]);
    }
    shas.good = await commit("SYNTHETIC good", endpoints.good ?? "pass");
    if (["skips", "all-skipped"].includes(scenario)) shas.skip = await commit("SYNTHETIC unbuildable", "untestable");
    if (scenario === "skips") await commit("SYNTHETIC still unbuildable", "untestable");
    if (scenario === "toolchain") {
        await fs.writeFile(path.join(f.read().mirrorDirectory, "global.json"), '{"sdk":{"version":"synthetic"}}');
        f.git(["add", "global.json"]);
    }
    shas.candidate = await commit("SYNTHETIC introduction", scenario === "all-skipped" ? "untestable" : "product-failure");
    if (scenario === "reintroduction") {
        f.git(["revert", "--no-edit", shas.candidate]);
        await commit("SYNTHETIC reintroduction", "product-failure");
    }
    if (scenario === "merge") {
        f.git(["checkout", "--quiet", "-b", "side", shas.good]);
        await commit("SYNTHETIC side change", "side", "side.txt");
        f.git(["checkout", "--quiet", "main"]);
        f.git(["merge", "--quiet", "--no-ff", "side", "-m", "SYNTHETIC integration"]);
    }
    if (scenario === "backport") {
        f.git(["branch", "servicing", shas.candidate]);
        f.git(["checkout", "--quiet", "-B", "main", shas.good]);
        f.git(["cherry-pick", "-x", shas.candidate]);
        shas.candidate = f.git(["rev-parse", "HEAD"]);
    }
    const mainOutcome = endpoints.main ?? "product-failure";
    const current = await fs.readFile(path.join(f.read().mirrorDirectory, "source.txt"), "utf8");
    if (current !== mainOutcome) shas.main = await commit("SYNTHETIC bad main", mainOutcome);
    else { f.git(["commit", "--quiet", "--allow-empty", "-m", "SYNTHETIC no-op main"]); shas.main = f.git(["rev-parse", "HEAD"]); }
    f.context.pinnedMainSha = shas.main;
    f.context.manifest.searchHints.sourceShas = [shas.good];
    contracts.seal(f.context.manifest);
    f.approval.manifest = structuredClone(f.context.manifest);
    f.update(db => {
        db.SYNTHETIC = { stable: endpoints.stable ?? "product-failure" };
        db.olderSha = shas.good;
        db.files[".github/regression-bisect/approved/123.json"] = Buffer.from(JSON.stringify(f.approval)).toString("base64");
    });
    return Object.assign(f, { shas });
}

async function cli(f, mode, routing = false) {
    const args = [mode, ...(routing && mode === "provision" ? [] : ["--issue", "123"]), "--config", f.configPath];
    const result = await invokeCli(args, { ...f.env, GITHUB_RUN_ID: routing ? "22345" : "12345" }, undefined,
        { entry: routing ? "routing-cli.cjs" : "controller-cli.cjs", storageFaults: false });
    return { ...result, result: JSON.parse(result.stdout) };
}

async function routedStep(f) {
    f.update(db => { db.runs["12345/1"].status = "completed"; db.runs["12345/1"].conclusion = "success"; });
    const route = await cli(f, "event", true);
    assert.equal(route.code, 0, route.stdout);
    const inputs = f.read().dispatches.at(-1).inputs;
    f.update(db => { db.runs["12345/1"].status = "in_progress"; db.runs["12345/1"].conclusion = null; });
    const r = await invokeCli([inputs.mode, "--issue", String(inputs.issue), "--config", f.configPath,
        "--routing-key", inputs.routingKey, "--routing-generation", inputs.routingGeneration],
    { ...f.env, GITHUB_RUN_ID: "12345" }, undefined, { storageFaults: false });
    return { ...r, result: JSON.parse(r.stdout) };
}

test("SYNTHETIC event-to-report linear trial reserves exactly six units, including independent confirmations", async t => {
    const f = await history(t);
    const candidate = f.shas.candidate;
    f.update(db => {
        db.associations = { [candidate]: [{ number: 700 }] };
        db.pulls = { 700: { details: { number: 700, html_url: "https://github.com/dotnet/fsharp/pull/700",
            state: "closed", merged: true, merged_at: db.now, updated_at: db.now, merge_commit_sha: candidate,
            base: { ref: "main", sha: f.shas.good, repo: { full_name: "dotnet/fsharp" } },
            head: { sha: candidate, repo: { full_name: "dotnet/fsharp" } } },
        commits: [{ sha: candidate }] } };
    });
    assert.equal((await cli(f, "provision", true)).code, 0);
    const results = [];
    for (let i = 0; i < 12; i++) {
        const r = await routedStep(f);
        assert.equal(r.code, 0, r.stdout);
        results.push(r.result.status);
        if (r.result.status === "published") break;
    }
    assert.equal(results.at(-1), "published", JSON.stringify(results));
    const { cp, context, transport } = await checkpoint(f);
    assert.deepEqual(cp.state.budget.consumed, { sourceBuilds: 5, workerUnits: 6, reservedMinutes: 660 });
    assert.deepEqual(cp.records.map(r => r.request.phase),
        ["stable", "main", "good-search", "bisect", "confirmation", "confirmation"]);
    assert.equal(new Set(cp.records.map(r => r.id)).size, 6);
    assert.deepEqual(cp.records.filter(r => r.request.phase === "confirmation").map(r => r.request.target.sourceSha),
        [candidate, f.shas.good]);
    assert.equal(cp.state.confirmation.classification, "verified-introducing-source-boundary");
    assert.notEqual(cp.state.confirmation.searchObservationDigest, cp.state.confirmation.candidateObservationDigest);
    const comment = await transport.getComment(457);
    const authenticated = await state.authenticateDecodedRecord(transport, { ...context, commentId: 457 }, comment);
    assert.equal(authenticated.kind, "result");
    const display = comment.body.split("\n\nRegression investigation result\n\n```json\n")[0];
    for (const sha of [candidate, f.shas.good, f.shas.main]) assert.ok(display.includes(sha), display);
    assert.match(display, /Classification: verified introducing source boundary\./);
    assert.match(display, /https:\/\/github.com\/dotnet\/fsharp\/pull\/700/);
    assert.match(display, /unproven/);
    const writes = f.ledger().filter(e => e.kind === "api" && e.method !== "GET");
    assert.ok(writes.every(e => e.method === "PATCH" && /^issues\/comments\/(?:456|457|458|459)$/.test(e.route) ||
        e.method === "POST" && (e.route === `statuses/${f.config.anchorSha}` ||
            e.route === "actions/workflows/regression-bisect-controller.yml/dispatches")), JSON.stringify(writes));
    assert.equal(writes.filter(e => e.route === "issues/comments/457").length, 1);
    assert.equal(f.ledger().filter(e => e.kind === "worker").length, 6);
    assert.ok(f.ledger().some(e => e.kind === "git" && e.args.includes("bisect")));
});

test("SYNTHETIC actual CLI denies endpoint combinations before every bisect subprocess", async t => {
    for (const [endpoints, classification] of [
        [{ stable: "pass" }, /not reproduced on latest stable/],
        [{ main: "pass" }, /release-specific or fixed on main/],
        [{ stable: "untestable" }, /stable untestable/],
        [{ main: "untestable" }, /main untestable/],
        [{ good: "product-failure" }, /no verified starting interval|budget|attempt/],
        [{ good: "untestable" }, /no verified starting interval|budget|attempt/],
    ]) await t.test(JSON.stringify(endpoints), async t => {
        const f = await history(t, "linear", endpoints);
        assert.equal((await cli(f, "provision", true)).code, 0);
        assert.equal((await cli(f, "bootstrap")).code, 0);
        let last;
        for (let i = 0; i < 8; i++) {
            last = await cli(f, "advance");
            if (last.result.status !== "observed") break;
        }
        assert.match(last.result.reason, classification, last.stdout);
        assert.equal(f.ledger().filter(e => e.kind === "git" && e.args.includes("bisect")).length, 0);
    });
});

test("SYNTHETIC real histories retain uncertainty rather than unique blame", async t => {
    for (const [scenario, reason] of [
        ["skips", /unconfirmed|skip|parent/i], ["all-skipped", /skip|unconfirmed/i],
        ["toolchain", /toolchain.*coupled/i], ["reintroduction", /revert|reintroduction/i],
        ["merge", /unconfirmed interval/i], ["backport", /cherry|backport/i],
    ]) await t.test(scenario, async t => {
        const f = await history(t, scenario);
        assert.equal((await cli(f, "provision", true)).code, 0);
        assert.equal((await cli(f, "bootstrap")).code, 0);
        let last;
        for (let i = 0; i < 10; i++) {
            last = await cli(f, "advance");
            if (last.result.status !== "observed") break;
        }
        const { cp } = await checkpoint(f);
        assert.notEqual(cp.state.confirmation?.classification, "verified-introducing-source-boundary");
        assert.match(last.result.reason, reason, last.stdout);
        if (scenario === "toolchain") assert.equal(cp.state.confirmation.classification, "verified-triggering-change");
        if (scenario === "merge") assert.ok(cp.state.confirmation.history.commits.some(c => c.parents.length > 1));
        if (["skips", "all-skipped"].includes(scenario)) assert.ok(cp.state.skips.length > 0);
    });
});

test("SYNTHETIC file API associations consume later pages before claiming a unique PR", async t => {
    for (const count of [0, 1, 2]) await t.test(`${count} verified associations`, async t => {
        const f = await history(t), candidate = f.shas.candidate;
        f.update(db => {
            db.associationPageSize = 1;
            db.associations = { [candidate]: [{ number: 699 }, ...Array.from({ length: count }, (_, i) => ({ number: 700 + i }))] };
            db.pulls = { 699: { details: { number: 699, html_url: "https://github.com/dotnet/fsharp/pull/699",
                base: { repo: { full_name: "dotnet/fsharp" } }, state: "open", merged: false, merged_at: null } } };
            for (let i = 0; i < count; i++) db.pulls[700 + i] = {
                details: { number: 700 + i, html_url: `https://github.com/dotnet/fsharp/pull/${700 + i}`,
                    state: "closed", merged: true, merged_at: db.now, updated_at: db.now, merge_commit_sha: f.shas.main,
                    base: { ref: "main", sha: f.shas.good, repo: { full_name: "dotnet/fsharp" } },
                    head: { sha: candidate, repo: { full_name: "dotnet/fsharp" } } },
                commits: [...Array.from({ length: 100 }, (_, j) => ({ sha: contracts.sha(`SYNTHETIC member ${j}`) })), { sha: candidate }],
            };
        });
        const service = await connect(f.serviceFile);
        const transport = require("./github.cjs").createGithubApi({ repository: "dotnet/fsharp", issueNumber: 123,
            checkpointCommentId: 456, resultCommentId: 457, anchorSha: f.config.anchorSha, token: "offline" }, service.send);
        const associations = await require("./attribution.cjs").resolveCommitAssociations({
            repository: "dotnet/fsharp", sourceSha: candidate, pinnedMainSha: f.shas.main,
        }, { transport, history: { gitExecutable: f.config.gitExecutable, scratchDirectory: f.directory,
            mirrorDirectory: f.read().mirrorDirectory, mainRef: "refs/heads/main" }, execute: service.executeGit });
        assert.equal(associations.status, ["none", "unique", "ambiguous"][count], associations.reason);
        assert.equal(associations.pulls.length, count);
        if (count) {
            assert.ok(f.ledger().some(e => e.route === `commits/${candidate}/pulls` && e.query.page === "2"));
            assert.ok(f.ledger().some(e => e.route === "pulls/700/commits" && e.query.page === "2"));
            assert.ok(associations.pulls.every(p => p.relation === "contained-commit" && p.onLineage));
        }
    });
});

test("trial CLI exposes only prepare, one-step advance, read-only verify and help", async () => {
    const result = await invoke(["--help"]);
    assert.equal(result.code, 0, result.stderr);
    for (const name of ["--prepare", "--advance", "--verify"]) assert.ok(result.stdout.includes(name));
    for (const args of [[], ["--reset"], ["--advance", "relative"], ["--verify", "relative"],
        ["--prepare", "relative", "relative"], ["--help", "--synthetic"]]) {
        const denied = await invoke(args);
        assert.equal(denied.code, 1);
        assert.equal(JSON.parse(denied.stdout).status, "blocked");
    }
});

async function reviewed(t) {
    const f = await history(t);
    const files = [];
    const service = await connect(f.serviceFile);
    const { toolchain } = await require("./toolchains.cjs").resolveLatestStableSdk({
        policy: contracts.policy(), platform: f.config.platform,
    }, service.toolchainDependencies);
    for (const role of ["good-patch", "introducing-patch", "source-toolchain", "stable-toolchain"]) {
        const file = path.join(f.directory, `${role}.json`);
        const raw = role.endsWith("-patch") ? f.git(["diff", "--no-ext-diff", "--no-textconv", "--binary",
            ...(role === "good-patch" ? [f.shas.base, f.shas.good] : [f.shas.good, f.shas.candidate])]) + "\n" :
            JSON.stringify({ SYNTHETIC: role });
        await fs.writeFile(file, raw);
        files.push({ role, path: file, sha256: contracts.digest(raw) });
    }
    for (const [i, m] of toolchain.metadata.entries()) {
        const file = path.join(f.directory, `metadata-${i}.json`);
        await fs.writeFile(file, Buffer.from(m.bytesBase64, "base64"));
        files.push({ role: `metadata-${i}`, path: file, sha256: m.sha256 });
    }
    f.update(db => {
        delete db.SYNTHETIC;
        db.runs["32345/1"] = { ...db.runs["12345/1"], id: 32345 };
    });
    const config = { schemaVersion: 1, purpose: "SYNTHETIC ACCEPTANCE ONLY",
        helperRevision: f.context.policyRevision, deployment: f.config,
        api: { path: f.serviceFile, sha256: contracts.digest(await fs.readFile(f.serviceFile)) },
        source: { directory: f.read().mirrorDirectory, ...f.shas },
        stable: { sdkVersion: toolchain.sdkVersion, archive: toolchain.archive,
            metadata: toolchain.metadata.map(({ url, sha256 }) => ({ url, sha256 })) },
        files, containmentDirectory: path.join(f.directory, "containment"),
        runs: { controller: "12345", routing: "22345", supervisor: "32345" } };
    const file = path.join(f.directory, "reviewed.json"), evidence = path.join(f.directory, "evidence");
    const save = async () => fs.writeFile(file, JSON.stringify({ ...config, signature: sign(config, f.context.keys.fixture) }));
    await save();
    return { f, config, file, evidence, save, env: { RB_SIGNING_KEYS: f.env.RB_SIGNING_KEYS,
        RB_LOCAL_TEST: "", RB_LOCAL_TEST_FILE: "", RB_TRIAL_DIRECTORY: "", GITHUB_ACTIONS: "", NODE_OPTIONS: "" } };
}

test("SYNTHETIC preparation freezes a signed plan without worker execution; verify is read-only and incomplete", async t => {
    const r = await reviewed(t);
    const prepared = await invoke(["--prepare", r.file, r.evidence], r.env);
    assert.equal(prepared.code, 0, prepared.stdout);
    assert.deepEqual(JSON.parse(prepared.stdout).workerUnits, 6);
    const before = await fs.readdir(r.evidence, { recursive: true });
    const raw = await fs.readFile(path.join(r.evidence, "api.json"));
    const incomplete = await invoke(["--verify", r.evidence], r.env);
    assert.equal(incomplete.code, 1);
    assert.deepEqual(await fs.readdir(r.evidence, { recursive: true }), before);
    assert.deepEqual(await fs.readFile(path.join(r.evidence, "api.json")), raw);
    assert.equal((await invoke(["--prepare", r.file, r.evidence], r.env)).code, 1);
    const service = await adapter({ ...r.env, RB_TRIAL_DIRECTORY: r.evidence });
    for (const key of ["runContainer", "collectObservation", "inspectRuntime", "toolchainDependencies", "download"])
        assert.equal(service[key], undefined, `${key} must be inaccessible to the acceptance CLI`);
    assert.ok(Math.abs(service.clock().getTime() - Date.now()) < 1000, "no injected deadline clock");
    const provision = await invoke(["--advance", r.evidence], r.env);
    assert.equal(provision.code, 0, provision.stdout);
    const routed = await invoke(["--advance", r.evidence], r.env);
    assert.equal(routed.code, 0, routed.stdout);
    const db = JSON.parse(await fs.readFile(path.join(r.evidence, "api.json")));
    assert.equal(db.dispatches.length, 1);
    assert.equal(db.dispatches[0].inputs.mode, "bootstrap");
    assert.equal(db.dispatches[0].inputs.issue, 123);
    if (process.platform !== "linux") {
        const blocked = await invoke(["--advance", r.evidence], r.env);
        assert.equal(blocked.code, 1);
        assert.match(JSON.parse(blocked.stdout).reason, /native Linux/);
    }
});

test("SYNTHETIC reviewed config conflicts, changed immutable inputs and authority widening fail closed", async t => {
    for (const change of ["unknown", "signature", "source", "file", "api", "synthetic", "actions", "target"]) await t.test(change, async t => {
        const r = await reviewed(t);
        if (change === "unknown") { r.config.worker = "attacker.cjs"; await r.save(); }
        if (change === "signature") await fs.writeFile(r.file, JSON.stringify({ ...r.config, signature: "0".repeat(64) }));
        if (change === "source") { r.config.source.candidate = r.config.source.good; await r.save(); }
        if (change === "target") { r.config.repository = "attacker/repo"; await r.save(); }
        const env = { ...r.env, ...(change === "synthetic" ? r.f.env : {}),
            ...(change === "actions" ? { GITHUB_ACTIONS: "true" } : {}) };
        const first = await invoke(["--prepare", r.file, r.evidence], env);
        if (["file", "api"].includes(change)) {
            assert.equal(first.code, 0, first.stdout);
            const file = change === "file" ? r.config.files[0].path : path.join(r.evidence, "api.json");
            if (change === "file") await fs.appendFile(file, "changed");
            else {
                const db = JSON.parse(await fs.readFile(file));
                db.issue.body += "\nchanged";
                await fs.writeFile(file, JSON.stringify(db));
            }
            const denied = await invoke(["--advance", r.evidence], env);
            assert.equal(denied.code, 1, denied.stdout);
            assert.match(JSON.parse(denied.stdout).reason, /mismatch|changed/);
        } else assert.equal(first.code, 1, first.stdout);
        assert.equal(r.f.ledger().filter(e => e.kind === "worker").length, 0);
    });
});

test("SYNTHETIC prepare rejects a linked evidence parent before creating any destination", async t => {
    const r = await reviewed(t);
    const link = path.join(r.f.directory, "redirect");
    await fs.symlink(r.f.directory, link, process.platform === "win32" ? "junction" : "dir");
    const denied = await invoke(["--prepare", r.file, path.join(link, "evidence")], r.env);
    assert.equal(denied.code, 1, denied.stdout);
    assert.match(JSON.parse(denied.stdout).reason, /parent.*links/);
    await assert.rejects(fs.stat(r.evidence), { code: "ENOENT" });
    await fs.unlink(link);
});
