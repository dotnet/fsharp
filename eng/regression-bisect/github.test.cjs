const { test } = require("node:test");
const assert = require("node:assert/strict");
const { createHash } = require("node:crypto");
const f = require("./fixtures/contracts.cjs");

const config = {
    repository: "dotnet/fsharp", issueNumber: 123, checkpointCommentId: 456,
    resultCommentId: 457, anchorSha: f.sha("anchor"), token: "harmless-fake-api-token",
};
const options = { owner: "dotnet", repo: "fsharp" };
const response = (value, init = {}) => new Response(JSON.stringify(value), { status: 200, ...init });

test("routing dispatcher admits only typed fixed-main controller delivery", async t => {
    const { createDispatcher } = require("./github.cjs");
    const calls = [];
    const dispatch = createDispatcher({ token: "offline-dispatch" }, async (url, options) => {
        calls.push({ url, ...options });
        return new Response(null, { status: 204 });
    });
    const valid = { issue: 123, mode: "advance", routingKey: "a".repeat(64), routingGeneration: "0" };
    for (const change of [{ issue: "123" }, { issue: 0 }, { mode: "provision" }, { routingGeneration: 0 },
        { routingGeneration: "-1" }, { routingGeneration: "01" }, { routingKey: "invalid" },
        ...["ref", "command", "url", "workflow", "repository", "allowanceId"].map(key => ({ [key]: "override" }))])
        await t.test(JSON.stringify(change), async () => { await assert.rejects(dispatch({ ...valid, ...change })); });
    assert.equal(calls.length, 0);
    await dispatch(valid);
    assert.equal(calls[0].url, "https://api.github.com/repos/dotnet/fsharp/actions/workflows/regression-bisect-controller.yml/dispatches");
    assert.equal(calls[0].redirect, "error");
    assert.deepEqual(JSON.parse(calls[0].body), { ref: "main", inputs: valid });
    for (const status of [200, 302, 429, 503]) await assert.rejects(createDispatcher({ token: "offline" },
        async () => new Response(null, { status }))(valid), /dispatch failed/);
});

test("GitHub API uses fixed authority and bounded manual-redirect requests without shell interpretation", async () => {
    const calls = [];
    const api = require("./github.cjs").createGithubApi(config, async (url, settings) => {
        calls.push({ url, ...settings });
        return response({ id: 456 }, { headers: { link: "", "content-type": "application/json" } });
    });
    assert.deepEqual(await api.getComment(456), { id: 456 });
    await api.getRun("12345", 2);
    await api.request("GET /repos/{owner}/{repo}/issues", {
        ...options, state: "open", labels: "Regression", sort: "created", direction: "asc", page: 1, per_page: 100,
    });
    for (const call of calls) {
        assert.equal(new URL(call.url).origin, "https://api.github.com");
        assert.equal(call.redirect, "error");
        assert.equal(call.headers.Authorization, `Bearer ${config.token}`);
        assert.equal(call.url.includes(config.token), false);
    }
    assert.match(calls[1].url, /\/actions\/runs\/12345\/attempts\/2$/);
    assert.match(calls[2].url, /labels=Regression/);
});

test("GitHub API rejects alternate authority, destinations, mutation capabilities and extra parameters before I/O", async t => {
    const cases = [
        ["PUT /repos/{owner}/{repo}/contents/{path}", { ...options, path: "pwn" }],
        ["POST /repos/{owner}/{repo}/issues/{issue_number}/comments", { ...options, issue_number: 123, body: "arbitrary" }],
        ["PATCH /repos/{owner}/{repo}/issues/{issue_number}", { ...options, issue_number: 123, state: "closed" }],
        ["GET https://attacker.invalid", {}],
        ["GET /repos/{owner}/{repo}/issues/{issue_number}", { ...options, issue_number: "../secret" }],
        ["GET /repos/{owner}/{repo}/issues/{issue_number}", { ...options, issue_number: 123, headers: { authorization: "override" } }],
        ["GET /repos/{owner}/{repo}/git/commits/{commit_sha}", { ...options, commit_sha: "main" }],
        ["PATCH /repos/{owner}/{repo}/issues/comments/{comment_id}", { ...options, comment_id: 999, body: "arbitrary" }],
        ["PATCH /repos/{owner}/{repo}/issues/comments/{comment_id}", { owner: "other", repo: "repo", comment_id: 456, body: "arbitrary" }],
        ["POST /repos/{owner}/{repo}/statuses/{sha}", { ...options, sha: f.sha("other"), state: "success" }],
        ["GET /repos/{owner}/{repo}/issues/{issue_number}", { owner: "..", repo: "fsharp", issue_number: 123 }],
        ["GET /repos/{owner}/{repo}/issues", { ...options, page: 0, per_page: 100 }],
        ["GET /repos/{owner}/{repo}/issues", { ...options, page: 1, per_page: 1000 }],
    ];
    for (const [route, args] of cases) await t.test(`${route}: ${JSON.stringify(args)}`, async () => {
        const api = require("./github.cjs").createGithubApi(config, async () => assert.fail("denied request reached network"));
        await assert.rejects(api.request(route, args), /invalid|forbidden|unsupported|unexpected/);
    });
    for (const change of [{ repository: "other/repo" }, { token: "" }, { resultCommentId: 456 }, { anchorSha: "main" }])
        assert.throws(() => require("./github.cjs").createGithubApi({ ...config, ...change }), /invalid/);
});

test("GitHub API blocks rate limiting, redirects, oversized, compressed, truncated and invalid UTF-8 responses", async t => {
    const cases = {
        "rate limit": () => response({}, { status: 429 }),
        redirect: () => response({}, { status: 302, headers: { location: "https://attacker.invalid" } }),
        "content length": () => response({}, { headers: { "content-length": "999" } }),
        oversized: () => new Response(Buffer.alloc(4 * 1024 * 1024 + 1)),
        compressed: () => response({}, { headers: { "content-encoding": "gzip" } }),
        malformed: () => new Response("{"),
        "invalid UTF-8": () => new Response(Buffer.from([0xff])),
    };
    for (const [name, reply] of Object.entries(cases)) await t.test(name, async () => {
        const api = require("./github.cjs").createGithubApi(config, async () => reply());
        await assert.rejects(api.getComment(456));
    });
});

test("GitHub writes are restricted to registered comments and the issue checkpoint anchor", async t => {
    const calls = [];
    const api = require("./github.cjs").createGithubApi(config, async (url, settings) => {
        calls.push({ url, method: settings.method, body: JSON.parse(settings.body) });
        return response({ id: 1 }, { status: settings.method === "POST" ? 201 : 200 });
    });
    for (const id of [config.checkpointCommentId, config.resultCommentId]) await api.updateComment(id, "bounded signed record");
    const args = {
        ...options, sha: config.anchorSha, state: "success", context: "regression-bisect/123/456",
        description: `checkpoint:0:${"a".repeat(64)}`,
        target_url: `https://github.com/dotnet/fsharp/issues/123#checkpoint-fixture:${"b".repeat(64)}`,
    };
    await api.request("POST /repos/{owner}/{repo}/statuses/{sha}", args);
    assert.deepEqual(calls.map(c => c.method), ["PATCH", "PATCH", "POST"]);
    for (const change of [
        { context: "regression-bisect/124/456" }, { context: "regression-bisect/123/789" },
        { target_url: "https://attacker.invalid" }, { description: "forged" }, { state: "pending" },
    ]) await assert.rejects(api.request("POST /repos/{owner}/{repo}/statuses/{sha}", { ...args, ...change }));
    await assert.rejects(api.updateComment(456, "x".repeat(48 * 1024)));
    assert.equal(calls.length, 3);
    t.diagnostic(JSON.stringify({ mutations: calls }));
});

test("pinned approval reader reads raw regular Git blobs, verifies object identity and never follows paths or refs", async t => {
    const revision = f.sha("trusted default branch commit");
    const files = new Map([
        [".github/regression-bisect/approved/123.json", Buffer.from('{"schemaVersion":1}')],
        ["samples/producer.dll", Buffer.from([0, 128, 255, 1])],
    ]);
    const blobs = new Map(), trees = new Map();
    const objectId = (kind, bytes) => createHash("sha1").update(`${kind} ${bytes.length}\0`).update(bytes).digest("hex");
    const tree = prefix => {
        const entries = new Map();
        for (const [name, bytes] of files) {
            if (!name.startsWith(prefix)) continue;
            const parts = name.slice(prefix.length).split("/");
            if (parts.length > 1) entries.set(parts[0], { path: parts[0], mode: "040000", type: "tree", sha: tree(`${prefix}${parts[0]}/`) });
            else {
                const sha = objectId("blob", bytes);
                blobs.set(sha, { sha, encoding: "base64", size: bytes.length, content: bytes.toString("base64") });
                entries.set(parts[0], { path: parts[0], mode: "100644", type: "blob", sha, size: bytes.length });
            }
        }
        const sha = f.sha(prefix);
        trees.set(sha, { sha, truncated: false, tree: [...entries.values()] });
        return sha;
    };
    const root = tree("");
    const reads = [];
    const api = { request: async (route, args) => {
        reads.push({ route, args });
        if (args.commit_sha) return { data: { sha: revision, tree: { sha: root } } };
        if (args.tree_sha) return { data: structuredClone(trees.get(args.tree_sha)) };
        if (args.file_sha) return { data: structuredClone(blobs.get(args.file_sha)) };
        assert.fail(route);
    } };
    const reader = () => require("./github.cjs").createApprovalReader({ api, revision, issueNumber: 123 });
    const r = reader();
    assert.deepEqual(await r.readApproval({ path: ".github/regression-bisect/approved/123.json", revision }), files.values().next().value);
    const bytes = files.get("samples/producer.dll");
    assert.deepEqual(await r.readSource({ path: "samples/producer.dll", sha256: f.digest(bytes) }), bytes);
    await assert.rejects(r.readApproval({ path: ".github/regression-bisect/approved/124.json", revision }), /approval/);
    await assert.rejects(r.readApproval({ path: ".github/regression-bisect/approved/123.json", revision: "main" }), /approval/);
    for (const badPath of ["../outside", "C:/outer", "//server/share", "samples\\producer.dll"])
        await assert.rejects(r.readSource({ path: badPath, sha256: f.digest(bytes) }));
    const entry = trees.get(f.sha("samples/")).tree[0];
    for (const mode of ["120000", "160000"]) await t.test(mode, async () => {
        entry.mode = mode;
        await assert.rejects(reader().readSource({ path: "samples/producer.dll", sha256: f.digest(bytes) }), /regular/);
    });
    entry.mode = "100644";
    trees.get(root).truncated = true;
    await assert.rejects(reader().readSource({ path: "samples/producer.dll", sha256: f.digest(bytes) }), /truncated/);
    trees.get(root).truncated = false;
    blobs.get(entry.sha).content = Buffer.from("substituted").toString("base64");
    await assert.rejects(reader().readSource({ path: "samples/producer.dll", sha256: f.digest(bytes) }), /identity|size/);
    t.diagnostic(JSON.stringify({ pinnedRevision: revision, reads: reads.length }));
});

test("bootstrap composes the concrete HTTP boundary with authenticated state using only fake GitHub", async t => {
    const { h, deps, mutations } = await require("./fixtures/execution.cjs").bootstrapFixture();
    const requests = [];
    const own = { owner: "dotnet", repo: "fsharp" };
    const send = async (address, settings) => {
        const url = new URL(address);
        const suffix = url.pathname.slice("/repos/dotnet/fsharp/".length);
        requests.push({ method: settings.method, path: url.pathname });
        let data;
        const comment = /^issues\/comments\/(\d+)$/.exec(suffix);
        const run = /^actions\/runs\/(\d+)\/attempts\/(\d+)$/.exec(suffix);
        if (comment) data = settings.method === "GET" ? await h.transport.getComment(Number(comment[1])) :
            await h.transport.updateComment(Number(comment[1]), JSON.parse(settings.body).body);
        else if (run) data = await h.transport.getRun(run[1], Number(run[2]));
        else {
            const page = { per_page: Number(url.searchParams.get("per_page")), page: Number(url.searchParams.get("page")) };
            let route, args;
            if (suffix.startsWith("commits/")) {
                route = "GET /repos/{owner}/{repo}/commits/{ref}/statuses";
                args = { ...own, ref: h.context.checkpointAnchorSha, ...page };
            } else if (suffix.startsWith("statuses/")) {
                route = "POST /repos/{owner}/{repo}/statuses/{sha}";
                args = { ...own, sha: h.context.checkpointAnchorSha, ...JSON.parse(settings.body) };
            } else {
                assert.match(suffix, /^issues\/123(?:\/comments|\/timeline)?$/);
                route = `GET /repos/{owner}/{repo}/${suffix.replace("123", "{issue_number}")}`;
                args = { ...own, issue_number: 123, ...page };
            }
            data = (await h.transport.request(route, args)).data;
        }
        return response(data, { status: settings.method === "POST" ? 201 : 200, headers: { link: "" } });
    };
    deps.transport = require("./github.cjs").createGithubApi({ ...config, anchorSha: h.context.checkpointAnchorSha }, send);
    const bootstrap = require("./controller.cjs").bootstrapController;
    const input = { repository: "dotnet/fsharp", issueNumber: 123 };
    const first = await bootstrap(input, deps);
    assert.equal(first.status, "initialized", first.reason);
    assert.equal((await bootstrap(input, deps)).status, "loaded");
    assert.deepEqual(requests.filter(r => r.method !== "GET").map(r => r.method), ["PATCH", "POST"]);
    assert.equal(mutations.length, 2);
    t.diagnostic(JSON.stringify({ requests, mutations }));
});
