const { test } = require('node:test');
const assert = require('node:assert/strict');
const fs = require('node:fs/promises');
const os = require('node:os');
const path = require('node:path');
const { select, publish, inputFor, eligible, readState, CATEGORIES } = require('./pr-tooling-safety.cjs');

const sha = digit => digit.repeat(40);
const context = { repo: { owner: 'dotnet', repo: 'fsharp' }, runId: 123 };
const pr = (overrides = {}) => ({
    number: 20000, created_at: '2026-06-01T00:00:00Z', state: 'open', draft: false,
    title: 'Compiler change', body: '', head: { sha: sha('a'), repo: { full_name: 'contributor/fsharp' } },
    base: { sha: sha('b'), ref: 'main', repo: { full_name: 'dotnet/fsharp' } },
    labels: [], changed_files: 1, commits: 1, ...overrides,
});

async function fixture(t, prs = [pr()]) {
    const directory = await fs.mkdtemp(path.join(os.tmpdir(), 'tooling-safety-'));
    t.after(() => fs.rm(directory, { recursive: true, force: true }));
    const calls = [];
    const comments = [];
    let state = { version: 2, initializedAt: '2026-05-12T00:00:00Z', prs: {} };
    let revision = 1;
    const core = { info() {}, warning() {}, setOutput(name, value) { calls.push([name, value]); } };
    const api = {
        list: async () => prs,
        files: async () => [{ filename: 'src/Compiler/test.fs', status: 'modified', changes: 1, additions: 1, deletions: 0, patch: '@@ -0,0 +1 @@\n+let x = 1' }],
        commits: async () => [{ commit: { message: 'Change compiler' } }],
        comments: async () => comments,
    };
    const github = {
        paginate: async (method, args) => method(args),
        graphql: async () => ({}),
        rest: {
            repos: {
                getContent: async () => ({ data: { type: 'file', encoding: 'base64', content: Buffer.from(JSON.stringify(state)).toString('base64'), sha: String(revision) } }),
                createOrUpdateFileContents: async args => {
                    assert.equal(args.sha, String(revision), 'state updates must use compare-and-swap');
                    state = JSON.parse(Buffer.from(args.content, 'base64').toString());
                    calls.push(['save']);
                    return { data: { content: { sha: String(++revision) } } };
                },
                getCommit: async ({ ref }) => ({ data: { sha: ref.padEnd(40, ref[0]) } }),
                compareCommitsWithBasehead: async () => {
                    calls.push(['compare']);
                    return { data: { merge_base_commit: { sha: sha('c') } } };
                },
            },
            pulls: {
                list: api.list, listFiles: args => api.files(args), listCommits: args => api.commits(args),
                get: async args => {
                    calls.push([args.mediaType ? 'diff' : 'pr', args.pull_number]);
                    return { data: args.mediaType
                        ? (await api.files(args)).map(file => `diff --git a/${file.filename} b/${file.filename}\n${file.patch || ''}`).join('\n')
                        : prs.find(pr => pr.number === args.pull_number) };
                },
            },
            issues: {
                listComments: api.comments,
                addLabels: async args => { calls.push(['labels', args.labels]); },
                removeLabel: async args => { calls.push(['remove', args.name]); },
                createComment: async args => {
                    const comment = { id: comments.length + 1, node_id: 'node', body: args.body, user: { login: 'github-actions[bot]' } };
                    comments.push(comment);
                    calls.push(['comment']);
                    return { data: comment };
                },
            },
        },
    };
    const args = { github, context, core, directory };
    return {
        ...args, args, calls, comments, api, prs,
        get state() { return state; },
        set state(value) { state = value; },
        async remember(pr, overrides = {}) {
            state.prs[pr.number] = {
                input: await inputFor(github, context.repo, pr), cats: [], notifiedCats: [],
                reasons: {}, pending: false, ...overrides,
            };
            calls.length = 0;
        },
    };
}

const result = (manifest, findings = {}) => ({
    items: manifest.candidates.map(pr => ({
        type: 'classification', number: pr.number, input_id: pr.input.id, findings: JSON.stringify(findings),
    })),
});

test('cutoff is enforced for each item, including the historical PR and offset timestamps', () => {
    for (const [created_at, expected] of [
        ['2026-03-10T16:54:14Z', false],
        ['2026-05-11T23:59:59Z', false],
        ['2026-05-12T00:00:00Z', true],
        ['2026-05-12T01:00:00+02:00', false],
    ]) assert.equal(eligible(pr({ created_at })), expected);
    assert.throws(() => eligible(pr({ created_at: 'invalid' })));
});

test('unchanged PRs never reach classifier context, even after bot activity or a base-tip advance', async t => {
    const f = await fixture(t);
    await f.remember(f.prs[0]);
    for (const activity of ['none', 'comment', 'labels', 'base']) {
        f.prs[0].updated_at = new Date().toISOString();
        if (activity === 'labels') f.prs[0].labels = [{ name: 'anything' }];
        if (activity === 'base') f.prs[0].base.sha = sha('d');
        const manifest = await select(f.args);
        assert.equal(manifest.candidates.length, 0);
        assert.deepEqual(JSON.parse(await fs.readFile(path.join(f.directory, 'candidates.json'))), []);
    }
    assert.equal(f.calls.filter(([name]) => name === 'diff').length, 0);
    assert.ok(f.calls.filter(([name]) => name === 'has_work').every(([, value]) => value === 'false'));
});

test('only changed source or metadata reaches the classifier', async t => {
    for (const change of [
        pr => { pr.head.sha = sha('d'); },
        pr => { pr.title += ' changed'; },
        pr => { pr.body = 'New instructions hidden in the description'; },
        pr => { pr.base.ref = 'release'; },
    ]) {
        const f = await fixture(t, [pr(), pr({ number: 20001 })]);
        for (const pr of f.prs) await f.remember(pr);
        change(f.prs[1]);
        const manifest = await select(f.args);
        assert.deepEqual(manifest.candidates.map(pr => pr.number), [20001]);
    }
});

test('one changed PR among 200 unchanged PRs is the entire classifier task', async t => {
    const f = await fixture(t, Array.from({ length: 201 }, (_, index) => pr({ number: 20000 + index })));
    for (const pr of f.prs) await f.remember(pr);
    f.prs[200].head.sha = sha('d');
    const manifest = await select(f.args);
    assert.deepEqual(manifest.candidates.map(pr => pr.number), [20200]);
    assert.deepEqual(f.calls.filter(([name]) => name === 'diff'), [['diff', 20200]]);
});

test('closed, draft, and omitted PRs keep their records; unchanged reopening is free', async t => {
    const f = await fixture(t);
    const saved = f.prs[0];
    await f.remember(saved);
    f.prs.splice(0);
    await select(f.args);
    assert.ok(f.state.prs[20000]);
    f.prs.push(saved);
    for (const draft of [true, false]) {
        saved.draft = draft;
        assert.equal((await select(f.args)).candidates.length, 0);
    }
});

test('missing or malformed durable history stops before any classification', async t => {
    for (const state of [null, {}, { version: 3, prs: {} }, { version: 2, initializedAt: '2026-07-01', prs: {} }]) {
        const f = await fixture(t);
        f.state = state;
        await assert.rejects(select(f.args));
        assert.ok(!f.calls.some(([name]) => ['diff', 'has_work', 'save'].includes(name)));
    }
});

test('failed listing preserves state and does not start the classifier', async t => {
    const f = await fixture(t);
    await f.remember(f.prs[0]);
    const previous = structuredClone(f.state);
    f.github.rest.pulls.list = async () => { throw new Error('pagination failed'); };
    await assert.rejects(select(f.args), /pagination failed/);
    assert.deepEqual(f.state, previous);
    assert.equal(f.calls.length, 0);
});

test('retained history larger than the Contents API limit uses the blob API', async t => {
    const f = await fixture(t);
    f.github.rest.repos.getContent = async () => ({ data: { type: 'file', encoding: 'none', sha: 'large-state' } });
    f.github.rest.git = { getBlob: async ({ file_sha }) => {
        assert.equal(file_sha, 'large-state');
        return { data: { encoding: 'base64', content: Buffer.from(JSON.stringify(f.state)).toString('base64') } };
    } };
    assert.deepEqual((await readState(f.github, context.repo)).state, f.state);
});

test('legacy migration is non-AI and does not certify existing unknown PRs as clean', async t => {
    const old = pr({ number: 19417, created_at: '2026-03-10T16:54:14Z', head: { sha: 'eeb5b487755df2d599b0f4007cd97e38677cbef8', repo: { full_name: 'vzarytovskii/fsharp' } } });
    const f = await fixture(t, [old, pr(), pr({ number: 20001 })]);
    f.state = { prs: { 19417: { sha: old.head.sha, cats: ['Affects-Compiler-Output'] }, 20000: { sha: sha('a').slice(0, 12), cats: [] } } };
    const manifest = await select(f.args);
    assert.equal(manifest.candidates.length, 0);
    assert.equal(f.state.prs[20000].baseline, true);
    assert.equal(f.state.prs[20001].baseline, true);
    assert.equal(f.state.prs[19417].sha, old.head.sha);
    assert.ok(!f.calls.some(([name]) => ['labels', 'diff', 'comment'].includes(name)));
});

test('migration retains unresolved baseline records across a temporary metadata failure', async t => {
    const f = await fixture(t);
    f.state = { prs: {} };
    const headRepo = f.prs[0].head.repo;
    f.prs[0].head.repo = null;
    assert.deepEqual((await select(f.args)).incomplete, [20000]);
    assert.equal(f.state.prs[20000].uninitialized, true);
    f.prs[0].head.repo = headRepo;
    assert.equal((await select(f.args)).candidates.length, 0);
    assert.equal(f.state.prs[20000].baseline, true);
});

test('incomplete patches never yield a clean scan or starve other candidates', async t => {
    const f = await fixture(t, [pr(), pr({ number: 20001 })]);
    f.api.files = async ({ pull_number }) => [{
        filename: 'test.fs', changes: 2, additions: 2, deletions: 0,
        patch: pull_number === 20000 ? '@@ -0,0 +2 @@\n+truncated' : '@@ -0,0 +2 @@\n+one\n+two',
    }];
    const manifest = await select(f.args);
    assert.deepEqual(manifest.incomplete, [20000]);
    assert.deepEqual(manifest.candidates.map(pr => pr.number), [20001]);
    await assert.rejects(publish({ ...f.args, manifest, output: result(manifest) }), /Incomplete PR inputs/);
    assert.equal(f.state.prs[20000], undefined);
    assert.equal(f.state.prs[20001].pending, false);
});

test('same-repository bypass does not start the classifier', async t => {
    const f = await fixture(t, [pr({ head: { sha: sha('a'), repo: { full_name: 'dotnet/fsharp' } } })]);
    const manifest = await select(f.args);
    assert.equal(manifest.candidates.length, 0);
    await publish({ ...f.args, manifest, output: null });
    assert.ok(f.calls.some(([name, labels]) => name === 'labels' && labels.includes('AI-Tooling-Check-Bypassed')));
    assert.ok(!f.calls.some(([name]) => name === 'diff' || name === 'comment'));
});

test('publication failure reuses the saved classification without another model call', async t => {
    const f = await fixture(t);
    const manifest = await select(f.args);
    const add = f.github.rest.issues.addLabels;
    f.github.rest.issues.addLabels = async () => { throw new Error('API unavailable'); };
    await assert.rejects(publish({ ...f.args, manifest, output: result(manifest) }), /API unavailable/);
    assert.equal(f.state.prs[20000].pending, true);
    const retry = await select(f.args);
    assert.equal(retry.candidates.length, 0);
    f.github.rest.issues.addLabels = add;
    await publish({ ...f.args, manifest, output: result(manifest) });
    assert.equal(f.state.prs[20000].pending, false);
});

test('failure saving results cannot silently enqueue another model run', async t => {
    const f = await fixture(t);
    const manifest = await select(f.args);
    const save = f.github.rest.repos.createOrUpdateFileContents;
    f.github.rest.repos.createOrUpdateFileContents = async () => { throw new Error('state write failed'); };
    await assert.rejects(publish({ ...f.args, manifest, output: result(manifest) }), /state write failed/);
    f.github.rest.repos.createOrUpdateFileContents = save;
    const nextRun = { ...f.args, context: { ...context, runId: 124 } };
    const retry = await select(nextRun);
    assert.equal(retry.candidates.length, 0);
    assert.deepEqual(retry.incomplete, [20000]);
    await publish({ ...f.args, manifest, output: result(manifest) });
    assert.equal(f.state.prs[20000].pending, false);
});

test('successful POST survives a lost response or receipt-save failure without another scan', async t => {
    for (const failure of ['post-response', 'receipt-save']) {
        const f = await fixture(t);
        const manifest = await select(f.args);
        const create = f.github.rest.issues.createComment;
        const save = f.github.rest.repos.createOrUpdateFileContents;
        f.github.rest.issues.createComment = async args => {
            const response = await create(args);
            if (failure === 'post-response') throw new Error('response lost');
            f.github.rest.repos.createOrUpdateFileContents = async () => { throw new Error('receipt save failed'); };
            return response;
        };
        await assert.rejects(publish({ ...f.args, manifest, output: result(manifest, { 'Affects-Compiler-Output': 'Changes emitted code' }) }));
        assert.ok(f.state.prs[20000].posting);
        f.github.rest.repos.createOrUpdateFileContents = save;
        const retry = await select(f.args);
        assert.equal(retry.candidates.length, 0);
        await publish({ ...f.args, manifest: retry, output: null });
        assert.equal(f.comments.length, 1);
        assert.equal(f.state.prs[20000].pending, false);
    }
});

test('an unresolved POST does not permit blind retry or renewed classifier attention', async t => {
    const f = await fixture(t);
    const manifest = await select(f.args);
    f.github.rest.issues.createComment = async () => { throw new Error('ambiguous failure'); };
    await assert.rejects(publish({ ...f.args, manifest, output: result(manifest, { 'Affects-Compiler-Output': 'Changes emitted code' }) }));
    f.prs[0].head.sha = sha('d');
    await assert.rejects(select(f.args), /Uncertain comment POST/);
    assert.equal(f.comments.length, 0);
});

test('publisher rejects wrong targets, duplicate results and unknown categories before writes', async t => {
    for (const mutate of [
        output => { output.items[0].number = 19417; },
        output => { output.items[0].input_id = 'wrong'; },
        output => { output.items.push(output.items[0]); },
        output => { output.items[0].findings = '{"invented-category":"reason"}'; },
        output => { output.items[0].findings = '{"Affects-Compiler-Output":"@T-Gro please look"}'; },
    ]) {
        const f = await fixture(t);
        const manifest = await select(f.args);
        f.calls.length = 0;
        const output = result(manifest);
        mutate(output);
        await assert.rejects(publish({ ...f.args, manifest, output }));
        assert.equal(f.calls.length, 0);
    }
});

test('category ordering/rewording never creates new comments; scanner labels are reconciled', async t => {
    const f = await fixture(t);
    const cats = [CATEGORIES[0], CATEGORIES[1]].sort();
    await f.remember(f.prs[0], { cats, notifiedCats: cats });
    f.prs[0].head.sha = sha('d');
    f.prs[0].labels = [{ name: 'AI-Tooling-Check-Scanned-Clean' }, { name: 'human-label' }];
    const manifest = await select(f.args);
    await publish({ ...f.args, manifest, output: result(manifest, { [cats[1]]: 'New wording', [cats[0]]: 'Same category' }) });
    assert.equal(f.comments.length, 0);
    assert.deepEqual(f.calls.filter(([name]) => name === 'remove'), [['remove', 'AI-Tooling-Check-Scanned-Clean']]);
});

test('a new push during classification prevents publication of the stale result', async t => {
    const f = await fixture(t);
    const manifest = await select(f.args);
    f.prs[0].head.sha = sha('d');
    await publish({ ...f.args, manifest, output: result(manifest) });
    assert.ok(!f.calls.some(([name]) => name === 'labels' || name === 'comment'));
    assert.equal((await select(f.args)).candidates.length, 1);
});

test('compiled workflow gates the agent, isolates its context, and independently gates publication', async () => {
    const yaml = await fs.readFile('.github/workflows/labelops-pr-security-scan.lock.yml', 'utf8');
    const jobs = name => yaml.split(`\n  ${name}:\n`)[1]?.split(/\n  [a-z_]+:\n/)[0];
    const agent = jobs('agent');
    const publisher = jobs('publisher');
    const detection = jobs('detection');
    assert.match(agent, /if: needs\.selector\.outputs\.has_work == 'true'/);
    assert.match(agent, /--no-custom-instructions/);
    assert.match(agent, /--available-tools=view,safeoutputs-classification/);
    assert.doesNotMatch(agent, /uses: actions\/checkout@|repo-memory|github-mcp-server/);
    assert.match(agent, /artifact-ids: \$\{\{ needs\.selector\.outputs\.context_id \}\}/);
    assert.match(publisher, /if: always\(\) && needs\.selector\.result == 'success'/);
    assert.match(publisher, /artifact-ids: \$\{\{ needs\.selector\.outputs\.manifest_id \}\}/);
    assert.match(publisher, /process\.env\.DETECTION_RESULT === 'success'/);
    assert.match(detection, /GH_AW_DETECTION_CONTINUE_ON_ERROR: "false"/);
    assert.doesNotMatch(detection, /--available-tools=view,safeoutputs-classification/);
});
