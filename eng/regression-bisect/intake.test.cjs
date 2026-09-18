const test = require("node:test");
const assert = require("node:assert/strict");
const { manifest, seal, digest, state, sha, statusTransport } = require("./fixtures/contracts.cjs");
const { fingerprintInput, isEligibleIssue } = require("./model.cjs");
const intake = () => require("./intake.cjs");
const repository = "dotnet/fsharp";
const now = "2026-09-18T12:00:00Z";
const issue = (number = 123, patch = {}) => ({
    id: number + 1000, number, title: "Compiler regression", body: "The result should be ok.",
    state: "open", created_at: "2026-09-01T00:00:00Z", updated_at: now,
    labels: [{ name: "Regression" }], locked: false, user: { id: 1, login: "reporter" },
    html_url: `https://github.com/${repository}/issues/${number}`, ...patch,
});
const comment = (id, body = `Comment ${id}`) => ({
    id, node_id: `IC_${id}`, body, created_at: now, updated_at: now,
    user: { id: 1, login: "reporter", type: "User" },
});
const reference = (number, kind = "pull") => ({
    id: number + 2000, event: "cross-referenced", created_at: now,
    source: { issue: { number, html_url: `https://github.com/${repository}/${kind}/${number}` } },
});

function fixture(patch = {}) {
    const data = { issue: issue(), comments: [], timeline: [], linked: {}, runs: {}, ...patch };
    const calls = [];
    const transport = {
        async request(route, args) {
            calls.push({ route, args: structuredClone(args) });
            if (data.fail?.(route, args)) throw new Error("API unavailable");
            if (route.endsWith("/comments") || route.endsWith("/timeline")) {
                const kind = route.endsWith("/comments") ? "comments" : "timeline";
                const pages = data[`${kind}Pages`];
                const rows = pages ? (pages[args.page - 1] ?? [])
                    : data[kind].slice((args.page - 1) * 100, args.page * 100);
                const hasNext = pages ? args.page < pages.length : args.page * 100 < data[kind].length;
                return { data: structuredClone(rows), headers: { link: hasNext ? '<https://api.github.com/next>; rel="next"' : "" } };
            }
            if (route.endsWith("/issues")) {
                const rows = data.list ?? [];
                return {
                    data: structuredClone(rows.slice((args.page - 1) * 100, args.page * 100)),
                    headers: { link: args.page * 100 < rows.length ? '<https://api.github.com/next>; rel="next"' : "" },
                };
            }
            if (route.endsWith("/attempts/{attempt_number}")) {
                const run = data.runs[args.run_id];
                if (!run) throw new Error("Missing run");
                return { data: structuredClone(run) };
            }
            const number = args.issue_number ?? args.pull_number;
            const target = number === 123 && args.repo === "fsharp" ? data.issue : data.linked[number];
            if (!target) throw new Error(`Missing linked issue ${number}`);
            return { data: structuredClone(target) };
        },
    };
    return { data, calls, options: { transport, repository, issueNumber: 123, now } };
}

async function snapshot(f, extra = {}) {
    return intake().readIssueSnapshot({ ...f.options, ...extra });
}

function blocked(result, pattern = /./) {
    assert.equal(result.status, "blocked");
    assert.match(result.reason, pattern);
    assert.equal(result.snapshot, undefined);
    assert.equal(result.inputDigest, undefined);
}

test("reads later corrections and timeline pages before canonicalizing the full snapshot", async () => {
    const f = fixture({
        commentsPages: [[comment(1)], [comment(2, "Correction: expected diagnostic, not a crash.")]],
        timelinePages: [[{ id: 5, event: "labeled", label: { name: "Regression" } }], [{ id: 6, event: "renamed", rename: { from: "Old", to: "New" } }]],
    });
    const result = await snapshot(f);
    assert.equal(result.status, "complete");
    assert.equal(result.snapshot.input.discussion.comments[1].body, f.data.commentsPages[1][0].body);
    assert.equal(result.snapshot.input.discussion.timeline.length, 2);
    assert.equal(result.snapshot.inputDigest, fingerprintInput(result.snapshot.input));
    assert.equal(result.snapshot.inputDigest, digest(result.snapshot.canonicalInput));
    assert.deepEqual(JSON.parse(result.snapshot.canonicalInput), result.snapshot.input);
    assert.equal(result.snapshot.discussionFingerprint, fingerprintInput(result.snapshot.input.discussion));
    assert.equal(f.calls.filter(c => c.args.page === 2).length, 2);
});

for (const [name, mutate] of [
    ["later correction", f => f.data.comments.push(comment(3, "Use preview language version"))],
    ["body edit", f => f.data.comments[0].body = "Corrected reproduction"],
    ["edit timestamp", f => f.data.comments[0].updated_at = "2026-09-19T00:00:00Z"],
    ["comment deletion", f => f.data.comments.splice(0, 1)],
    ["stable identity replacement", f => f.data.comments[0].id = 9],
    ["title edit", f => f.data.issue.title = "Corrected title"],
    ["body edit on issue", f => f.data.issue.body = "New reproduction"],
    ["labels", f => f.data.issue.labels.push({ name: "Compiler" })],
    ["timeline correction", f => f.data.timeline.push({ id: 44, event: "reopened" })],
]) {
    test(`snapshot digest changes after ${name}`, async () => {
        const f = fixture({ comments: [comment(1), comment(2)] });
        const before = await snapshot(f);
        mutate(f);
        const after = await snapshot(f);
        assert.equal(after.status, "complete");
        assert.notEqual(after.snapshot.inputDigest, before.snapshot.inputDigest);
    });
}

test("canonical ordering ignores API object key order and unrelated issue updated_at", async () => {
    const f = fixture({ comments: [comment(1), comment(2)] });
    const before = await snapshot(f);
    f.data.issue.updated_at = "2026-09-19T00:00:00Z";
    f.data.issue.comments = 999;
    f.data.issue = Object.fromEntries(Object.entries(f.data.issue).reverse());
    f.data.comments.reverse();
    const after = await snapshot(f);
    assert.equal(after.snapshot.inputDigest, before.snapshot.inputDigest);
});

test("markers and bot identities are input unless async authentication explicitly succeeds", async () => {
    const bot = { id: 41898282, login: "github-actions[bot]", type: "Bot" };
    const genuine = { ...comment(8, "<!-- checkpoint --> trusted run"), user: bot };
    const authenticatedBody = genuine.body;
    const forged = { ...comment(9, "<!-- checkpoint --> trusted run"), user: bot };
    const f = fixture({ comments: [comment(1), genuine, forged], timeline: [{ ...genuine, event: "commented" }, { ...forged, event: "commented" }] });
    const readTrustedCheckpoints = async () => [];
    const plain = await snapshot(f);
    assert.equal(plain.snapshot.input.discussion.comments.length, 3);
    const authenticated = await snapshot(f, { readTrustedCheckpoints, authenticateRecord: async record => {
        await Promise.resolve();
        return record.id === 8 && record.body === authenticatedBody && record.user.id === bot.id;
    } });
    assert.deepEqual(authenticated.snapshot.input.discussion.comments.map(c => c.id), [1, 9]);
    assert.deepEqual(authenticated.snapshot.input.discussion.timeline.map(c => c.id), [9]);
    const truthy = await snapshot(f, { authenticateRecord: async () => ({ authenticated: true }) });
    assert.equal(truthy.snapshot.inputDigest, plain.snapshot.inputDigest);
    f.data.comments[1].body = "Edited impersonation";
    const edited = await snapshot(f, { readTrustedCheckpoints, authenticateRecord: async c => c.id === 8 && c.body === authenticatedBody });
    assert.equal(edited.snapshot.input.discussion.comments.length, 3);
});

test("authenticated workflow progress does not self-invalidate discussion or input", async () => {
    const f = fixture({ comments: [comment(1)] });
    const authenticateRecord = async c => c.id === 8 && c.user.id === 7 && c.body.startsWith("Authenticated ");
    const readTrustedCheckpoints = async () => [];
    const before = await snapshot(f, { authenticateRecord, readTrustedCheckpoints });
    const managed = { ...comment(8, "Authenticated progress"), user: { id: 7, login: "app" } };
    f.data.comments.push(managed);
    f.data.timeline.push({ ...managed, event: "commented" });
    f.data.issue.updated_at = "2026-09-19T00:00:00Z";
    const after = await snapshot(f, { authenticateRecord, readTrustedCheckpoints });
    assert.equal(after.snapshot.inputDigest, before.snapshot.inputDigest);
});

test("authenticated records require an explicit trusted checkpoint reader before freezing", async () => {
    const f = fixture({ comments: [comment(456, "A controller-authenticated record")] });
    blocked(await snapshot(f, { authenticateRecord: async () => true }), /checkpoint reader required/i);
});

test("real checkpoint authentication excludes only the pinned signed workflow record", async () => {
    const { initializeCheckpoint, authenticateWorkflowRecord, loadCheckpoint, reserveWork } = require("./state.cjs");
    const body = require("node:fs").readFileSync(require.resolve("./fixtures/checkpoint.txt"), "utf8").trimEnd();
    const actor = { id: 77, login: "github-actions[bot]", type: "Bot", appId: 88 };
    const baseline = state();
    const context = {
        repository, issueNumber: 123, commentId: 456, actor,
        workflowPath: ".github/workflows/regression-bisect.yml", workflowRef: "refs/heads/main",
        workflowRevision: sha("workflow"), trustedWorkflowRevisions: [sha("workflow")],
        checkpointAnchorSha: sha("workflow"), bootstrapDigest: fingerprintInput("unused placeholder"),
        keys: { fixture: Buffer.from("synthetic-test-key-not-a-production-secret") }, keyId: "fixture",
        owner: { runId: "12345", attempt: 1 }, now, issue: issue(),
        policy: baseline.budget.policy, policyRevision: baseline.budget.policyRevision,
        manifest: baseline.manifest, issueFingerprint: baseline.issueFingerprint,
        authorization: baseline.budget.authorization, stableSdk: baseline.budget.stableSdk, pinnedMainSha: baseline.pinnedMainSha,
    };
    let runReads = 0;
    const stateTransport = { request: statusTransport(context).request, async getRun(runId, attempt) {
        assert.equal(runId, "12345");
        assert.equal(attempt, 1);
        runReads++;
        return {
            id: 12345, run_attempt: 1, path: context.workflowPath, head_sha: sha("workflow"), head_branch: "main",
            status: "in_progress",
            repository: { full_name: repository }, head_repository: { full_name: repository },
        };
    } };
    const managed = {
        ...comment(456, "unused placeholder"), issue_url: "https://api.github.com/repos/dotnet/fsharp/issues/123",
        user: { id: actor.id, login: actor.login, type: actor.type }, performed_via_github_app: { id: actor.appId },
    };
    const copied = comment(457, body);
    stateTransport.getComment = async id => {
        assert.equal(id, context.commentId);
        return structuredClone(managed);
    };
    stateTransport.updateComment = async (id, updatedBody) => {
        assert.equal(id, context.commentId);
        managed.body = updatedBody;
        return structuredClone(managed);
    };
    await initializeCheckpoint(stateTransport, context, {
        allowanceId: "maintainer-approved-investigation-1", deadline: "2026-10-01T00:00:00Z",
    });
    assert.equal(managed.body, body);
    runReads = 0;
    const f = fixture({
        comments: [comment(1), managed, copied],
        timeline: [{ ...managed, event: "commented" }, { ...copied, event: "commented" }],
        runs: { 12345: { id: 12345, status: "in_progress", run_attempt: 1 } },
    });
    const authenticateRecord = record => authenticateWorkflowRecord(stateTransport, context, record);
    const readerContext = { ...context, owner: { runId: "99999", attempt: 1 } };
    let loaded;
    const readTrustedCheckpoints = async () => {
        loaded = await loadCheckpoint(stateTransport, readerContext);
        assert.deepEqual(loaded.blockers, []);
        return [{
            status: "loaded", state: loaded.checkpoint.state, pending: loaded.checkpoint.pending,
            commentId: loaded.commentId, owner: loaded.owner,
        }];
    };
    blocked(await snapshot(f, { authenticateRecord }), /checkpoint reader required/i);
    const result = await snapshot(f, { authenticateRecord, readTrustedCheckpoints });
    assert.equal(result.status, "complete");
    assert.deepEqual(result.snapshot.input.discussion.comments.map(c => c.id), [1, 457]);
    assert.deepEqual(result.snapshot.input.discussion.timeline.map(c => c.id), [457]);
    assert.equal(result.snapshot.input.discussion.comments[1].body, body);
    assert.equal(runReads, 4);
    assert.equal(loaded.checkpoint.pending, null);
    assert.deepEqual(result.snapshot.investigations.active[0].owner, loaded.owner);
    await reserveWork(stateTransport, context, {
        generation: loaded.checkpoint.generation, attemptId: "intake-integration-main",
        request: { kind: "source", target: { sourceSha: sha("main") }, endpoint: "main", phase: "main" },
    });
    f.data.timeline[0].body = managed.body;
    const pending = await snapshot(f, { authenticateRecord, readTrustedCheckpoints });
    assert.equal(pending.status, "complete");
    assert.equal(pending.snapshot.investigations.active.length, 1);
    assert.deepEqual(pending.snapshot.investigations.active[0].owner, loaded.checkpoint.pending.owner);
    assert.equal(pending.snapshot.inputDigest, result.snapshot.inputDigest);
    const currentBody = managed.body;
    managed.body = body;
    f.data.timeline[0].body = body;
    blocked(await snapshot(f, { authenticateRecord, readTrustedCheckpoints }), /freshness|rollback/i);
    managed.body = currentBody;
    f.data.timeline[0].body = currentBody;
    assert.equal(await authenticateRecord({ ...managed, user: copied.user }), false);
    assert.equal(await authenticateRecord({ ...managed, body: `${body} ` }), false);
    stateTransport.getRun = async () => { throw new Error("workflow run provenance unavailable"); };
    blocked(await snapshot(f, { authenticateRecord, readTrustedCheckpoints }), /workflow run provenance unavailable/);
});

for (const [name, edit, extra] of [
    ["issue API", f => f.data.fail = route => route.endsWith("/issues/{issue_number}"), {}],
    ["later comments API", f => f.data.fail = (route, args) => route.endsWith("/comments") && args.page === 2, {}],
    ["timeline API", f => f.data.fail = route => route.endsWith("/timeline"), {}],
    ["comment page limit", () => {}, { limits: { commentPages: 1 } }],
    ["timeline page limit", f => f.data.timelinePages = [[{ id: 1, event: "labeled" }], [{ id: 2, event: "reopened" }]], { limits: { timelinePages: 1 } }],
    ["byte limit", () => {}, { limits: { inputBytes: 10 } }],
    ["missing comment ID", f => delete f.data.commentsPages[0][0].id, {}],
    ["conflicting duplicate comment", f => f.data.commentsPages[1] = [comment(1, "Changed during pagination")], {}],
    ["authentication API", () => {}, { authenticateRecord: async () => { throw new Error("provenance unavailable"); } }],
    ["checkpoint API", () => {}, { readTrustedCheckpoints: async () => { throw new Error("checkpoint unavailable"); } }],
]) {
    test(`incomplete ${name} blocks without a frozen partial snapshot`, async () => {
        const f = fixture({ commentsPages: [[comment(1)], [comment(2)]] });
        edit(f);
        blocked(await snapshot(f, extra));
    });
}

test("finite safety limits cannot be disabled or increased beyond the hard bounds", async () => {
    for (const limits of [{ commentPages: Infinity }, { timelinePages: 0 }, { inputBytes: -1 }, { linkedTargets: 101 }, { unknown: 1 }]) {
        blocked(await snapshot(fixture(), { limits }), /limit/i);
    }
});

test("manifest freezing binds reviewed raw bytes, exact assertions, and dependency identities", async () => {
    const f = fixture();
    const plain = await snapshot(f);
    const m = manifest();
    m.discussionFingerprint = plain.snapshot.discussionFingerprint;
    seal(m);
    let bytes = Buffer.from('printfn "ok"');
    const readSource = async source => {
        assert.equal(source.path, m.sources[0].path);
        return bytes;
    };
    const first = await snapshot(f, { reviewedManifest: m, readSource });
    assert.equal(first.status, "complete");
    assert.equal(first.snapshot.input.reproduction.sources[0].bytesBase64, bytes.toString("base64"));
    for (const edit of [
        value => value.assertions.pass.stdout = "changed\n",
        value => value.packages[0].sha256 = digest("different package"),
        value => value.producer.sha256 = digest("different producer"),
        value => value.matrix[0].languageVersion = "preview",
    ]) {
        const changed = structuredClone(m);
        edit(changed);
        seal(changed);
        const next = await snapshot(f, { reviewedManifest: changed, readSource });
        assert.equal(next.status, "complete");
        assert.notEqual(next.snapshot.inputDigest, first.snapshot.inputDigest);
    }
    bytes = Buffer.from('printfn "ok"\r\n');
    blocked(await snapshot(f, { reviewedManifest: m, readSource }), /source.*hash/i);
    m.sources[0].sha256 = digest(bytes);
    seal(m);
    const changedBytes = await snapshot(f, { reviewedManifest: m, readSource });
    assert.equal(changedBytes.status, "complete");
    assert.notEqual(changedBytes.snapshot.inputDigest, first.snapshot.inputDigest);
    f.data.issue.body = "Edited after approval";
    blocked(await snapshot(f, { reviewedManifest: m, readSource }), /discussion/i);
});

test("missing, oversized, wrong-issue, or unreadable reviewed sources block", async () => {
    const f = fixture();
    const m = manifest();
    m.discussionFingerprint = (await snapshot(f)).snapshot.discussionFingerprint;
    seal(m);
    blocked(await snapshot(f, { reviewedManifest: m }), /source/i);
    blocked(await snapshot(f, { reviewedManifest: m, readSource: async () => { throw new Error("source unavailable"); } }));
    blocked(await snapshot(f, { reviewedManifest: m, readSource: async () => Buffer.alloc(20), limits: { sourceBytes: 10 } }), /source/i);
    m.issueNumber = 999;
    seal(m);
    blocked(await snapshot(f, { reviewedManifest: m, readSource: async () => Buffer.from('printfn "ok"') }), /issue/i);
});

test("linked investigations use freshly read status and activity rather than old timeline claims", async () => {
    const f = fixture({
        comments: [comment(1, "Already investigating forever!"), comment(2, "See https://github.com/dotnet/fsharp/pull/321")],
        timeline: [reference(321), reference(322), reference(323, "issues")],
        linked: {
            321: issue(321, { state: "closed", merged_at: now }),
            322: issue(322, { state: "open", updated_at: now, merged_at: null }),
            323: issue(323, { state: "open", updated_at: "2026-09-01T00:00:00Z" }),
        },
    });
    const result = await snapshot(f);
    assert.equal(result.status, "complete");
    assert.deepEqual(result.snapshot.investigations.active.map(x => x.number), [322]);
    assert.equal(result.snapshot.investigations.links.length, 3);
    assert.equal(result.snapshot.investigations.links.find(x => x.number === 321).state, "closed");
    assert.equal(f.calls.filter(c => c.args.pull_number === 321).length, 1);
});

test("unlinked prose and unrelated links do not veto; reciprocal current investigation does", async () => {
    const f = fixture({
        comments: [comment(1, "Already investigating! https://github.com/dotnet/fsharp/issues/321")],
        linked: { 321: issue(321, { body: "Unrelated open issue" }) },
    });
    let found = await intake().findActiveInvestigations({ ...f.options, issue: f.data.issue, comments: f.data.comments, timeline: [] });
    assert.equal(found.status, "complete");
    assert.equal(found.active.length, 0);
    f.data.linked[321].body = "Investigating https://github.com/dotnet/fsharp/issues/123";
    found = await intake().findActiveInvestigations({ ...f.options, issue: f.data.issue, comments: f.data.comments, timeline: [] });
    assert.equal(found.active.length, 1);
});

test("required linked reads and linked-target safety limits fail closed", async () => {
    const f = fixture({ timeline: [reference(321)] });
    blocked(await snapshot(f), /linked/i);
    f.data.timeline.push(reference(322));
    blocked(await snapshot(f, { limits: { linkedTargets: 1 } }), /limit/i);
});

test("timeline event and comment identifiers occupy separate namespaces", async () => {
    const f = fixture({ comments: [comment(5)], timeline: [
        { ...comment(5), event: "commented" },
        { id: 5, event: "labeled", label: { name: "Regression" } },
    ] });
    const result = await snapshot(f);
    assert.equal(result.status, "complete");
    assert.equal(result.snapshot.input.discussion.timeline.length, 2);
});

test("a supplied checkpoint reader must return an explicit successful result", async () => {
    for (const value of [undefined, null, {}, [{ status: "loaded" }]]) {
        blocked(await snapshot(fixture(), { readTrustedCheckpoints: async () => value }), /checkpoint/i);
    }
});

test("linked PRs require complete current merged status", async () => {
    const f = fixture({ timeline: [reference(321)], linked: { 321: issue(321) } });
    blocked(await snapshot(f), /linked.*merged/i);
});

test("lost cursor traversal also reaches dense-page overflow candidates", async () => {
    const f = fixture({ list: Array.from({ length: 205 }, (_, i) => issue(i + 1)) });
    const seen = new Set();
    for (let scheduleOrdinal = 0; scheduleOrdinal < 750; scheduleOrdinal++) {
        const start = f.calls.length;
        const found = await intake().discoverEligibleIssues({ ...f.options, issueNumber: undefined, scheduleOrdinal });
        assert.equal(found.status, "complete");
        assert.ok(found.issues.length <= 3);
        assert.ok(f.calls.length - start <= 2);
        found.issues.forEach(value => seen.add(value.number));
    }
    assert.ok(seen.has(4), "lost cursors must not starve the fourth eligible issue");
    assert.ok(seen.has(101), "lost cursors must not starve the second page");
    assert.equal(seen.size, 205);
});

test("trusted checkpoint ownership checks the live run, never a forged marker", async () => {
    const s = state();
    const f = fixture({ comments: [comment(1, "<!-- checkpoint --> active")], runs: {
        44: { id: 44, status: "in_progress", run_attempt: 2 },
    } });
    const loaded = { status: "loaded", state: s, commentId: 77, owner: { runId: "44", attempt: 2 } };
    let result = await snapshot(f, { readTrustedCheckpoints: async () => [loaded] });
    assert.equal(result.status, "complete");
    assert.equal(result.snapshot.investigations.active[0].basis, "trusted-checkpoint-active-run");
    assert.equal(f.calls.at(-1).args.attempt_number, 2);
    f.data.runs[44].status = "completed";
    result = await snapshot(f, { readTrustedCheckpoints: async () => [loaded] });
    assert.equal(result.snapshot.investigations.active.length, 0);
    delete f.data.runs[44];
    blocked(await snapshot(f, { readTrustedCheckpoints: async () => [loaded] }), /run/i);
    blocked(await snapshot(f, { readTrustedCheckpoints: async () => [{ status: "blocked", reason: "mandatory evidence missing" }] }));
    s.issueNumber = 999;
    blocked(await snapshot(f, { readTrustedCheckpoints: async () => [loaded] }), /issue/i);
});

test("pending reservation ownership takes precedence over the checkpoint writer", async () => {
    const f = fixture({ runs: { 44: { id: 44, status: "in_progress", run_attempt: 2 } } });
    const loaded = {
        status: "loaded", state: state(), commentId: 77,
        owner: { runId: "99", attempt: 1 }, pending: { owner: { runId: "44", attempt: 2 } },
    };
    const result = await snapshot(f, { readTrustedCheckpoints: async () => [loaded] });
    assert.equal(result.status, "complete");
    assert.equal(result.snapshot.investigations.active[0].owner.runId, "44");
});

for (const [name, patch] of [
    ["opened with label", {}],
    ["human late label", { created_at: "2026-09-02T00:00:00Z" }],
    ["bot late label", { created_at: "2026-09-03T00:00:00Z", user: { id: 2, login: "bot", type: "Bot" } }],
    ["wrong label case", { labels: ["regression"] }],
    ["before cutoff", { created_at: "2026-08-31T23:59:59Z" }],
    ["closed", { state: "closed" }],
    ["pull request", { pull_request: {} }],
]) {
    test(`manual, event, scheduled and snapshot selection share exact predicate: ${name}`, async () => {
        const value = issue(123, patch);
        const f = fixture({ issue: value, list: [value] });
        const expected = isEligibleIssue(value);
        for (const trigger of ["manual", "event", "schedule"]) {
            const selected = await intake().discoverEligibleIssues({
                ...f.options, issueNumber: trigger === "schedule" ? undefined : 123, trigger, scheduleOrdinal: 0,
            });
            assert.equal(selected.status, "complete");
            assert.equal(selected.issues.length, expected ? 1 : 0);
        }
        assert.equal((await snapshot(f)).status, expected ? "complete" : "ineligible");
    });
}

test("created-order REST discovery has hard 2x100/three-candidate bounds and no overflow skips", async () => {
    const list = Array.from({ length: 205 }, (_, i) => issue(i + 1));
    const f = fixture({ list });
    const seen = new Set();
    let cursor;
    for (let sweep = 0; sweep < 80 && seen.size < list.length; sweep++) {
        const start = f.calls.length;
        const result = await intake().discoverEligibleIssues({ ...f.options, issueNumber: undefined, cursor, scheduleOrdinal: sweep });
        assert.equal(result.status, "complete");
        assert.ok(result.issues.length <= 3);
        assert.ok(f.calls.length - start <= 2);
        result.issues.forEach(item => seen.add(item.number));
        cursor = result.nextCursor;
    }
    assert.equal(seen.size, 205);
    for (const { route, args } of f.calls) {
        assert.equal(route, "GET /repos/{owner}/{repo}/issues");
        assert.equal(args.sort, "created");
        assert.equal(args.direction, "asc");
        assert.equal(args.per_page, 100);
        assert.equal(args.labels, "Regression");
        assert.equal(args.state, "open");
        assert.equal(args.since, undefined);
    }
});

test("discovery rotates over sparse pages and a lost cursor uses the independent schedule ordinal", async () => {
    const list = Array.from({ length: 401 }, (_, i) => issue(i + 1, { labels: i === 400 ? ["Regression"] : [] }));
    const f = fixture({ list });
    let cursor;
    const seen = new Set();
    for (let sweep = 0; sweep < 20; sweep++) {
        const result = await intake().discoverEligibleIssues({ ...f.options, issueNumber: undefined, cursor, scheduleOrdinal: sweep });
        result.issues.forEach(item => seen.add(item.number));
        cursor = result.nextCursor;
    }
    assert.ok(seen.has(401));
    seen.clear();
    for (let sweep = 0; sweep < 120; sweep++) {
        const result = await intake().discoverEligibleIssues({ ...f.options, issueNumber: undefined, scheduleOrdinal: sweep });
        result.issues.forEach(item => seen.add(item.number));
    }
    assert.ok(seen.has(401), "losing the cursor each run must not starve later pages");
    f.data.list[0].labels = ["Regression"];
    let recovered = false;
    for (let sweep = 120; sweep < 750; sweep++) {
        const result = await intake().discoverEligibleIssues({ ...f.options, issueNumber: undefined, scheduleOrdinal: sweep });
        recovered ||= result.issues.some(item => item.number === 1);
    }
    assert.ok(recovered, "old issues labeled by automation are eventually revisited even without cursor storage");
});

test("discovery returns no candidates or advanced cursor after a required API failure", async () => {
    const f = fixture({
        list: Array.from({ length: 201 }, (_, i) => issue(i + 1, { labels: i === 0 ? ["Regression"] : [] })),
        fail: (_, args) => args.page === 2,
    });
    const result = await intake().discoverEligibleIssues({ ...f.options, issueNumber: undefined, scheduleOrdinal: 0 });
    assert.equal(result.status, "blocked");
    assert.deepEqual(result.issues, []);
});
