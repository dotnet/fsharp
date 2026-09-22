const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs/promises");
const path = require("node:path");
const { routingFixture, checkpoint, connect, invokeCli, writeDatabase } = require("./fixtures/controller-service.cjs");
const contracts = require("./fixtures/contracts.cjs");
const state = require("./state.cjs");
const { sign, adapter, helpers } = require("./trial.cjs");
const { freeze, base, sourcePath, samplePath, sample, sentinel } = require("./fixtures/trial-source.cjs");

const invoke = (args, env = {}, started, synthetic = false) => invokeCli(args, env, started, {
    entry: "trial.cjs", kind: "trial-exit", storageFaults: false,
    preload: synthetic ? [path.join(__dirname, "fixtures", "trial-driver.cjs")] : [],
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

test("trial helper identity prunes disposable directories before reading them", async t => {
    const expected = await helpers(), readdir = fs.readdir;
    t.mock.method(fs, "readdir", async (directory, options) => {
        assert.equal(options.recursive, undefined, "recursive enumeration enters transient Git and build directories");
        const entries = await readdir(directory, options);
        return directory === __dirname ? [...entries, ...[".attribution-test-gone", "obj", "bin", "node_modules"]
            .map(name => ({ name, isDirectory: () => true }))] : entries;
    });
    assert.deepEqual(await helpers(), expected);
    assert.ok(expected.some(e => e.path.endsWith("fixtures/trial-driver.cjs")));
    assert.ok(expected.some(e => e.path.endsWith("trial.cjs")));
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

test("SYNTHETIC trial stable guard accepts only the frozen official identity", async t => {
    const r = await reviewed(t);
    assert.equal((await invoke(["--prepare", r.file, r.evidence], r.env)).code, 0);
    const service = await adapter({ ...r.env, RB_TRIAL_DIRECTORY: r.evidence });
    for (const [name, change] of [
        ["unchanged", () => {}],
        ["resolver details", value => {
            value.resolvedAt = new Date().toISOString();
            value.metadata.forEach(record => { record.bytesBase64 = "ignored resolved bytes"; });
        }],
        ["sdk version", value => { value.sdkVersion = "10.0.200"; }],
        ["archive URL", value => { value.archive.url += "?changed"; }],
        ["archive checksum", value => { value.archive.sha512 = "0".repeat(128); }],
        ["archive provenance", value => { value.archive.provenance.kind = "changed"; }],
        ["metadata URL", value => { value.metadata[0].url += "?changed"; }],
        ["metadata digest", value => { value.metadata[0].sha256 = "0".repeat(64); }],
        ["metadata order", value => { value.metadata.reverse(); }],
        ["missing metadata", value => { value.metadata.pop(); }],
        ["extra metadata", value => { value.metadata.push(value.metadata[0]); }],
    ]) await t.test(name, () => {
        const value = structuredClone(r.config.stable);
        change(value);
        if (["unchanged", "resolver details"].includes(name)) assert.doesNotThrow(() => service.validateStable(value));
        else assert.throws(() => service.validateStable(value), /frozen official stable metadata changed/);
    });
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

test("SYNTHETIC source freeze selects publisher runtime, never apphost, and preserves the reviewed graph", async t => {
    const f = await history(t), feed = "https://builds.dotnet.microsoft.com/dotnet";
    const archive = (product, version, rid = "linux-x64") => ({ name: `${product}-${rid}.tar.gz`, rid,
        url: `${feed}/${product === "dotnet-sdk" ? "Sdk" : "Runtime"}/${version}/${product}-${version}-${rid}.tar.gz`,
        hash: "a".repeat(128) });
    const sdk = { version: "10.0.301", files: [archive("dotnet-sdk", "10.0.301")] };
    const runtime = { version: "10.0.0", files: [archive("dotnet-apphost-pack", "10.0.0"),
        archive("dotnet-runtime", "10.0.0", "linux-arm64"), archive("dotnet-runtime", "10.0.0")] };
    const indexUrl = `${feed}/release-metadata/releases-index.json`, channelUrl = `${feed}/release-metadata/10.0/releases.json`;
    for (const mode of ["valid", "apphost-only", "duplicate-runtime", "unavailable"]) await t.test(mode, async () => {
        const calls = [], directory = path.join(f.directory, mode);
        const record = structuredClone(runtime);
        if (mode === "apphost-only") record.files = record.files.slice(0, 1);
        if (mode === "duplicate-runtime") record.files.push(record.files.at(-1));
        const http = async (url, options = {}) => {
            calls.push({ url, method: options.method ?? "GET" });
            assert.ok([indexUrl, channelUrl, sdk.files[0].url, ...record.files.map(f => f.url)].includes(url), url);
            return { status: mode === "unavailable" && options.method === "HEAD" ? 404 : 200, url, redirects: [],
                bytes: Buffer.from(JSON.stringify(url === indexUrl
                    ? { "releases-index": [{ "channel-version": "10.0", "releases.json": channelUrl }] }
                    : { "channel-version": "10.0", releases: [{ "release-date": "2026-06-01", sdk, runtime: record }] })) };
        };
        const pending = freeze(directory, f.config.gitExecutable, { http, clock: () => new Date("2026-09-22") });
        if (mode !== "valid") {
            await assert.rejects(pending, mode === "unavailable" ? /unavailable/ : /missing or ambiguous/);
            await assert.rejects(fs.stat(path.join(directory, "source-graph.json")), { code: "ENOENT" });
        } else {
            const result = await pending;
            const frozen = JSON.parse(await fs.readFile(path.join(directory, "source-toolchain.json")));
            assert.deepEqual(frozen.archives.map(a => a.archive.url), [sdk.files[0].url, runtime.files.at(-1).url]);
            assert.deepEqual(calls.filter(c => c.method === "HEAD").map(c => c.url), frozen.archives.map(a => a.archive.url));
            assert.equal(result.source.base, base);
            assert.deepEqual([result.source.good, result.source.candidate, result.source.main], [
                "05e2f222e08785994d8909707054b411ea677fc4", "a23fa5514d49566559bdcc7479fbe00ceb01b577",
                "e648b05ced2bc8f071685a7dbfacbbc15964e2f9"]);
            const git = args => f.git(["--git-dir", result.source.directory, ...args]);
            assert.equal(git(["rev-list", "--parents", "-n", "1", result.source.candidate]),
                `${result.source.candidate} ${result.source.good}`);
            assert.equal(git(["show", `${result.source.candidate}:${sourcePath}`]), git(["show", `${base}:${sourcePath}`]));
            assert.ok(git(["show", `${result.source.good}:${sourcePath}`]).includes(sentinel.trim()));
            assert.equal(git(["show", `${result.source.main}:${samplePath}`]), sample.trim());
            for (const file of result.files) assert.equal(contracts.digest(await fs.readFile(file.path)), file.sha256);
            const manifest = JSON.parse(await fs.readFile(path.join(directory, "frozen-manifest.json")));
            assert.deepEqual(manifest.assertions, {
                pass: { exitCode: 0, stdout: "SYNTHETIC_ACCEPTANCE_SENTINEL\nTRIAL_SAMPLE\n", stderr: "" },
                failure: { exitCode: 0, stdout: "TRIAL_SAMPLE\n", stderr: "" } });
            assert.deepEqual(result.plan, { sourceBuilds: 5, workerUnits: 6, reservedMinutes: 660, totalMinutes: 720,
                attempts: { [result.source.good]: 2, [result.source.candidate]: 2, [result.source.main]: 1 }, actualProductRuns: 0 });
            await assert.rejects(freeze(directory, f.config.gitExecutable, { http }), { code: "EEXIST" });
        }
        assert.equal(calls.filter(c => c.method === "HEAD" && c.url.includes("apphost")).length, 0);
    });
});

async function preparedDriver(t) {
    const r = await reviewed(t);
    assert.equal((await invoke(["--prepare", r.file, r.evidence], r.env)).code, 0);
    const api = path.join(r.evidence, "api.json");
    const read = async () => JSON.parse(await fs.readFile(api));
    const update = async action => { const db = await read(); action(db); writeDatabase(api, db); };
    const step = async (fault, started) => invoke(["--advance", r.evidence],
        { ...r.env, RB_SYNTHETIC_TRIAL_FAULT: fault ?? "" }, started, true);
    const ledger = async () => (await fs.readFile(`${api}.ledger`, "utf8")).trim().split("\n").map(JSON.parse);
    const cp = () => checkpoint({ ...r.f, serviceFile: api });
    const receipt = async () => {
        const { signature, ...body } = JSON.parse(await fs.readFile(path.join(r.evidence, "delivery.json")));
        assert.equal(signature, sign(body, r.f.context.keys.fixture));
        assert.equal(body.trialId, (await read()).trialId);
        return body;
    };
    const stop = async fault => {
        let child;
        const barrier = path.join(r.evidence, "synthetic-barrier.json");
        await fs.rm(barrier, { force: true });
        const running = step(fault, value => { child = value; });
        t.after(() => { if (child.exitCode === null) child.kill("SIGKILL"); });
        let marker;
        for (let i = 0; i < 400; i++) {
            try { marker = JSON.parse(await fs.readFile(barrier)); break; }
            catch (error) { if (error.code !== "ENOENT") throw error; }
            await new Promise(resolve => setTimeout(resolve, 25));
        }
        assert.equal(marker?.point, fault, `barrier not reached; child exit ${child.exitCode}`);
        assert.equal(marker.pid, child.pid);
        const concurrent = await step();
        assert.equal(concurrent.code, 1, concurrent.stdout);
        assert.match(JSON.parse(concurrent.stdout).reason, /live process/);
        child.kill("SIGKILL");
        await running;
        return child.pid;
    };
    return { ...r, api, read, update, step, ledger, cp, receipt, stop };
}

test("SYNTHETIC trial controller blocks changed stable metadata before download or charge", async t => {
    for (const [mode, transitions] of [["bootstrap", 2], ["advance", 4]]) await t.test(mode, async t => {
        const r = await preparedDriver(t);
        for (let i = 0; i < transitions; i++) assert.equal((await r.step()).code, 0);
        const before = await r.read(), start = (await r.ledger()).length;
        assert.equal(before.dispatches.at(-1).inputs.mode, mode);
        const denied = await r.step("stable-metadata");
        assert.equal(denied.code, 1, denied.stdout);
        assert.match(JSON.parse(denied.stdout).reason, /frozen official stable metadata changed/);
        const trace = (await r.ledger()).slice(start), after = await r.read();
        assert.ok(trace.some(e => e.kind === "metadata"), "controller must resolve current official metadata");
        assert.equal(trace.filter(e => ["download", "worker"].includes(e.kind) ||
            e.kind === "git" && e.args.includes("bisect") || e.kind === "api" && e.method !== "GET").length, 0);
        assert.deepEqual(after.comments, before.comments);
        assert.deepEqual(after.statuses, before.statuses);
        const resumed = await r.step();
        assert.equal(resumed.code, 0, resumed.stdout);
        assert.equal(JSON.parse(resumed.stdout).status, mode === "bootstrap" ? "initialized" : "observed");
        const { cp } = await r.cp();
        assert.equal(cp.state.budget.consumed.workerUnits, mode === "bootstrap" ? 0 : 1);
        assert.equal(cp.deadline, r.f.approval.bootstrap.deadline);
        assert.equal(cp.state.budget.allowanceId, r.f.approval.bootstrap.allowanceId);
    });
});

test("SYNTHETIC trial driver recovers delivery ownership and verifies a complete six-unit result read-only", async t => {
    const r = await preparedDriver(t);
    for (const expected of ["provisioned", "routed"]) {
        const result = await r.step();
        assert.equal(JSON.parse(result.stdout).status, expected, result.stdout);
    }
    await r.stop("delivery-before");
    const bootstrap = (await r.cp()).cp;
    const duplicate = await r.step();
    assert.equal(JSON.parse(duplicate.stdout).status, "duplicate", duplicate.stdout);
    const bootstrapReceipt = await r.receipt();
    assert.equal(bootstrapReceipt.routingKey, (await r.read()).dispatches.at(-1).inputs.routingKey);
    assert.deepEqual((await r.cp()).cp.state.budget, bootstrap.state.budget);
    assert.equal((await r.ledger()).filter(e => e.kind === "worker").length, 0);
    const deliveryFile = path.join(r.evidence, "delivery.json"), deliveryBytes = await fs.readFile(deliveryFile);
    for (const foreign of [false, true]) {
        const receipt = { ...bootstrapReceipt, ...(foreign ? { trialId: "another-trial" } : {}) };
        await fs.writeFile(deliveryFile, JSON.stringify({ ...receipt,
            signature: foreign ? sign(receipt, r.f.context.keys.fixture) : "0".repeat(64) }));
        const api = await fs.readFile(r.api);
        const denied = await r.step();
        assert.equal(denied.code, 1, denied.stdout);
        assert.match(JSON.parse(denied.stdout).reason, /delivery receipt authentication/);
        assert.deepEqual(await fs.readFile(r.api), api, "invalid delivery cannot allocate a run");
    }
    await fs.writeFile(deliveryFile, deliveryBytes);
    for (let unit = 0; unit < 6; unit++) {
        assert.equal((await r.step()).code, 0);
        const before = (await r.cp()).cp;
        if (unit === 0) {
            await r.stop("retained-observation");
            const claimed = (await r.cp()).cp;
            assert.ok(claimed.pending.executionGeneration !== undefined);
            const duplicate = await r.step();
            assert.equal(JSON.parse(duplicate.stdout).status, "duplicate", duplicate.stdout);
            assert.equal((await r.cp()).cp.pending.id, claimed.pending.id);
            assert.equal((await r.step()).code, 0);
            const result = await r.step();
            assert.equal(JSON.parse(result.stdout).recovered, true, result.stdout);
        } else if (unit === 1) {
            await r.stop("delivery-before");
            const work = (await r.cp()).cp.records.at(-1);
            const cache = path.join(r.evidence, "state", "dotnet-fsharp-123", "launches", work.id, "bundle-check");
            assert.equal(await fs.realpath(cache), cache);
            await fs.rm(cache, { recursive: true });
            const result = await r.step();
            assert.equal(JSON.parse(result.stdout).status, "duplicate", result.stdout);
        } else if (unit === 2) {
            await r.stop("delivery-after");
            assert.equal((await r.receipt()).routingKey, (await r.read()).dispatches.at(-1).inputs.routingKey);
        } else {
            const result = await r.step();
            assert.equal(result.code, 0, result.stdout);
        }
        const after = (await r.cp()).cp;
        assert.equal(after.state.budget.consumed.workerUnits, unit + 1);
        assert.equal(after.deadline, bootstrap.deadline);
        assert.equal(after.state.budget.allowanceId, bootstrap.state.budget.allowanceId);
        assert.equal(after.state.budget.consumed.reservedMinutes - before.state.budget.consumed.reservedMinutes, 110);
    }
    let result;
    for (let i = 0; i < 6; i++) {
        result = await r.step();
        assert.equal(result.code, 0, result.stdout);
        if ((await r.read()).dispatches.at(-1).inputs.mode === "publish") break;
    }
    await r.stop("result-write");
    const pending = (await r.cp()).cp;
    const persisted = (await r.read()).comments.find(c => c.id === 457).body;
    for (const e of Object.values(pending.observations).flatMap(o => o.evidence).filter(e => !e.required)) {
        await fs.unlink(path.join(r.evidence, "state", "dotnet-fsharp-123", "store",
            `${new URL(e.url).searchParams.get("name")}.bin`));
    }
    for (let i = 0; i < 4; i++) {
        result = await r.step();
        assert.equal(result.code, 0, result.stdout);
        if (JSON.parse(result.stdout).status === "published") break;
    }
    assert.equal(JSON.parse(result.stdout).status, "published", result.stdout);
    assert.equal((await r.read()).comments.find(c => c.id === 457).body, persisted);
    assert.equal((await r.ledger()).filter(e => e.kind === "api" && e.method === "PATCH" && e.route === "issues/comments/457").length, 1);
    const complete = (await r.cp()).cp;
    assert.deepEqual(complete.state.budget.consumed, { sourceBuilds: 5, workerUnits: 6, reservedMinutes: 660 });
    assert.equal((await r.ledger()).filter(e => e.kind === "worker").length, 6);
    const snapshot = async () => {
        const names = (await fs.readdir(r.evidence, { recursive: true })).sort(), entries = [];
        for (const name of names) {
            const file = path.join(r.evidence, name), stat = await fs.stat(file);
            entries.push([name, stat.mtimeMs, stat.isFile() ? contracts.digest(await fs.readFile(file)) : null]);
        }
        return entries;
    };
    const frozen = await snapshot();
    const verified = await invoke(["--verify", r.evidence], r.env);
    assert.equal(verified.code, 0, verified.stdout);
    assert.deepEqual(JSON.parse(verified.stdout), { status: "verified", purpose: "SYNTHETIC ACCEPTANCE ONLY",
        trialId: (await r.read()).trialId, digest: complete.state.publication.digest, commentId: 457,
        budget: complete.state.budget.consumed, physicalAcceptance: false });
    assert.deepEqual(await snapshot(), frozen, "verify cannot write a ledger, touch evidence or create ownership files");
    const originalApi = await fs.readFile(r.api);
    for (const [target, reason] of [
        ["checkpoint", /checkpoint authentication/], ["result", /authenticated persisted publication/],
        ["run", /workflow run provenance authentication/], ["required-evidence", /required evidence/],
        ["plan", /six-unit allowance identity/], ["helper", /helper drift/],
        ["discussion", /reviewed discussion fingerprint is stale/],
        ["spoofed workflow comment", /reviewed discussion fingerprint is stale/],
        ["discovery comment", /discovery record authentication/],
    ]) await t.test(target, async () => {
        const saved = path.join(r.evidence, "trial.json"), bytes = await fs.readFile(saved);
        let lost;
        if (["checkpoint", "result"].includes(target)) await r.update(db => {
            db.comments.find(c => c.id === (target === "checkpoint" ? 456 : 457)).body += "forged";
        });
        if (target === "run") await r.update(db => {
            for (const run of Object.values(db.runs)) run.head_sha = "0".repeat(40);
        });
        if (target === "discussion") await r.update(db => {
            db.comments.push({ id: 9000, body: "Correction: the approved reproduction is no longer valid", user: { id: 42 } });
        });
        if (target === "spoofed workflow comment") await r.update(db => {
            db.comments.push({ ...db.comments.find(c => c.id === 457), id: 9000 });
        });
        if (target === "discovery comment") await r.update(db => {
            db.comments.find(c => c.id === 458).body += "forged";
        });
        if (target === "required-evidence") {
            const e = Object.values(complete.observations)[0].evidence.find(e => e.required);
            const file = path.join(r.evidence, "state", "dotnet-fsharp-123", "store", `${new URL(e.url).searchParams.get("name")}.bin`);
            lost = { file, bytes: await fs.readFile(file) };
            await fs.unlink(file);
        }
        if (["plan", "helper"].includes(target)) {
            const { signature, ...savedPlan } = JSON.parse(bytes);
            if (target === "plan") savedPlan.deadline = "2026-10-02T00:00:00Z";
            else savedPlan.helpers[0].sha256 = "0".repeat(64);
            await fs.writeFile(saved, JSON.stringify({ ...savedPlan, signature: sign(savedPlan, r.f.context.keys.fixture) }));
        }
        try {
            const before = await snapshot();
            const denied = await invoke(["--verify", r.evidence], r.env);
            assert.equal(denied.code, 1, denied.stdout);
            assert.match(JSON.parse(denied.stdout).reason, reason);
            assert.deepEqual(await snapshot(), before);
        } finally {
            await fs.writeFile(saved, bytes);
            await fs.writeFile(r.api, originalApi);
            if (lost) await fs.writeFile(lost.file, lost.bytes);
        }
    });
    const artifacts = process.env.RB_TEST_EVIDENCE_DIRECTORY;
    if (artifacts) {
        const target = path.join(artifacts, path.basename(r.f.directory) + "-SYNTHETIC-trial");
        await fs.cp(r.evidence, target, { recursive: true });
        await fs.writeFile(path.join(target, "verify-receipt.json"), JSON.stringify({ ...verified, snapshot: frozen }));
    }
});

test("SYNTHETIC trial driver preserves charges after reservation or claim cancellation", async t => {
    for (const fault of ["reservation", "claim"]) await t.test(fault, async t => {
        const r = await preparedDriver(t);
        for (let i = 0; i < 4; i++) {
            const result = await r.step();
            assert.equal(result.code, 0, result.stdout);
        }
        await r.stop(fault);
        const before = (await r.cp()).cp;
        assert.equal(before.state.budget.consumed.workerUnits, 1);
        assert.equal((await r.ledger()).filter(e => e.kind === "worker").length, 0);
        assert.equal(JSON.parse((await r.step()).stdout).status, "duplicate");
        assert.equal((await r.step()).code, 0);
        const resumed = await r.step(), after = (await r.cp()).cp;
        assert.equal(after.deadline, before.deadline);
        assert.equal(after.state.budget.allowanceId, before.state.budget.allowanceId);
        if (fault === "claim") {
            assert.equal(resumed.code, 1, resumed.stdout);
            assert.match(JSON.parse(resumed.stdout).reason, /ENOENT|missing/);
            assert.deepEqual(after.state.budget.consumed, before.state.budget.consumed);
            assert.equal(after.pending.id, before.pending.id);
            assert.equal((await r.ledger()).filter(e => e.kind === "worker").length, 0);
        } else {
            assert.equal(resumed.code, 0, resumed.stdout);
            assert.equal(after.state.budget.consumed.workerUnits, 2, "unclaimed lost unit stays charged");
            assert.equal(after.state.budget.consumed.reservedMinutes, 220);
            assert.equal((await r.ledger()).filter(e => e.kind === "worker").length, 1);
        }
    });
});
