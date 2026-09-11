import assert from 'node:assert/strict';
import { spawnSync } from 'node:child_process';
import { existsSync, mkdtempSync, readFileSync, rmSync, writeFileSync } from 'node:fs';
import { tmpdir } from 'node:os';
import { dirname, join, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';
import test from 'node:test';

const root = resolve(dirname(fileURLToPath(import.meta.url)), '../..');
const releaseNotes = readFileSync(join(root, '.github/workflows/check_release_notes.yml'), 'utf8');
const head = 'a'.repeat(40);
const base = 'b'.repeat(40);
const marker = '<!-- DO_NOT_REMOVE: release_notes_check -->';
const notePath = 'docs/release-notes/.FSharp.Compiler.Service/11.0.100.md';

// Execute the actual inline workflow scripts, with only GitHub's API replaced.
function block(source, header) {
    const lines = source.split('\n');
    const start = lines.findIndex(line => line.trim() === header);
    assert.notEqual(start, -1, `Missing workflow block: ${header}`);
    const indent = lines[start].search(/\S/) + 2;
    const end = lines.findIndex((line, index) => index > start && line.trim() && line.search(/\S/) < indent);
    return lines.slice(start + 1, end < 0 ? undefined : end).map(line => line.slice(indent)).join('\n');
}

function check({ labels = [], currentLabels = labels, finalLabels = currentLabels, files = ['src/Compiler/Checking/CheckExpressions.fs'],
    note = '', firstHead = head, finalHead = head, apiError = false, finalApiError = false } = {}) {
    const directory = mkdtempSync(join(tmpdir(), 'fsharp-workflow-test-'));
    try {
        writeFileSync(join(directory, 'gh'), readFileSync(join(root, 'eng/tests/fixtures/release-notes-gh.cjs')), { mode: 0o755 });
        writeFileSync(join(directory, 'fixture.json'), JSON.stringify({
            prs: [firstHead, finalHead].map((sha, index) => ({
                head: { sha },
                labels: (index === 0 ? currentLabels : finalLabels).map(name => ({ name }))
            })),
            files: files.map(filename => ({
                filename, status: 'modified',
                contents_url: `https://api.github.com/repos/dotnet/fsharp/contents/${filename}?ref=${head}`
            })),
            note, apiError, finalApiError
        }));
        const output = join(directory, 'output');
        writeFileSync(output, '');
        const result = spawnSync('bash', ['-c', block(releaseNotes, 'run: |')], {
            encoding: 'utf8',
            env: {
                ...process.env, PATH: `${directory}:${process.env.PATH}`, FIXTURE_DIR: directory,
                GITHUB_OUTPUT: output, GITHUB_REPOSITORY: 'dotnet/fsharp',
                PR_NUMBER: '123', PR_AUTHOR: 'contributor', PR_BASE_SHA: base, PR_HEAD_SHA: head,
                VNEXT: '11.0.100'
            }
        });
        assert.ifError(result.error);
        return { ...result, output: readFileSync(output, 'utf8') };
    } finally {
        rmSync(directory, { recursive: true, force: true });
    }
}

for (const [name, options, status, heading] of [
    ['missing notes fail', {}, 1, 'Release notes required'],
    ['exemption added before execution', { currentLabels: ['NO_RELEASE_NOTES'] }, 0, 'Release-note check exempted'],
    ['exemption added during validation', { finalLabels: ['NO_RELEASE_NOTES'] }, 0, 'Release-note check exempted'],
    ['exemption removed during validation', { labels: ['NO_RELEASE_NOTES'], finalLabels: [] }, 1, 'Release notes required'],
    ['valid notes have a success heading', {
        files: ['src/Compiler/Checking/CheckExpressions.fs', notePath],
        note: '* Fix the bug. (https://github.com/dotnet/fsharp/pull/123)'
    }, 0, 'Release notes checked'],
    ['untracked paths need no notes', { files: ['README.md'] }, 0, 'No release notes required']
]) {
    test(name, () => {
        const result = check(options);
        assert.equal(result.status, status, result.stdout + result.stderr);
        assert.ok(result.output.includes(heading), result.output);
        assert.ok(!result.output.includes('@dotnet/fsharp-team-msft'), result.output);
        assert.ok(result.output.includes(`release-notes-required=${status === 1}`), result.output);
        const labels = options.finalLabels ?? options.currentLabels ?? options.labels ?? [];
        assert.ok(result.output.includes(`release-notes-exempt=${labels.includes('NO_RELEASE_NOTES')}`), result.output);
    });
}

for (const phase of ['firstHead', 'finalHead']) {
    test(`discard stale head at ${phase}`, () => {
        const result = check({ [phase]: 'c'.repeat(40) });
        assert.equal(result.status, 0, result.stderr);
        assert.equal(result.output, '');
    });
}

for (const phase of ['apiError', 'finalApiError']) {
    test(`GitHub ${phase} fails instead of exempting the PR`, () => {
        const result = check({ [phase]: true, labels: ['NO_RELEASE_NOTES'] });
        assert.notEqual(result.status, 0);
        assert.equal(result.output, '');
    });
}

async function publish({ existing, required = true, body = `${marker}\nResult`, error,
    exempt = false, currentExempt = exempt, currentHead = head } = {}) {
    const calls = [];
    const warnings = [];
    const environment = {
        COMMENT_BODY: body, RELEASE_NOTES_REQUIRED: String(required),
        RELEASE_NOTES_EXEMPT: String(exempt), PR_HEAD_SHA: head
    };
    const previous = Object.fromEntries(Object.keys(environment).map(key => [key, process.env[key]]));
    Object.assign(process.env, environment);
    try {
        const github = {
            paginate: async () => existing ? [existing] : [],
            rest: { pulls: {
                get: async () => ({ data: {
                    head: { sha: currentHead }, labels: currentExempt ? [{ name: 'NO_RELEASE_NOTES' }] : []
                } })
            }, issues: {
                listComments: () => {},
                createComment: async options => {
                    if (error) throw error;
                    calls.push({ method: 'create', ...options });
                    return { data: { id: 42 } };
                },
                updateComment: async options => {
                    if (error) throw error;
                    calls.push({ method: 'update', ...options });
                    return { data: { id: options.comment_id } };
                }
            } }
        };
        const AsyncFunction = Object.getPrototypeOf(async function () {}).constructor;
        await new AsyncFunction('github', 'context', 'core', block(releaseNotes, 'script: |'))(
            github, { repo: { owner: 'dotnet', repo: 'fsharp' }, issue: { number: 123 } },
            { warning: message => warnings.push(message), info: () => {} }
        );
        return { calls, warnings };
    } finally {
        for (const [key, value] of Object.entries(previous)) {
            if (value === undefined) delete process.env[key];
            else process.env[key] = value;
        }
    }
}

test('unchanged bot comments are not updated', async () => {
    const result = await publish({ existing: { id: 10, user: { login: 'github-actions[bot]' }, body: `${marker}\nResult` } });
    assert.deepEqual(result.calls, []);
});

test('successful checks do not create a new comment', async () => {
    assert.deepEqual((await publish({ required: false })).calls, []);
});

test('successful checks resolve an existing failure comment', async () => {
    const result = await publish({
        required: false, body: `${marker}\nResolved`,
        existing: { id: 10, user: { login: 'github-actions[bot]' }, body: `${marker}\nMissing notes` }
    });
    assert.equal(result.calls.length, 1);
    assert.equal(result.calls[0].method, 'update');
    assert.equal(result.calls[0].comment_id, 10);
});

test('missing notes create one bot comment without editing contributor comments', async () => {
    const result = await publish({ existing: { id: 10, user: { login: 'contributor' }, body: marker } });
    assert.equal(result.calls.length, 1);
    assert.equal(result.calls[0].method, 'create');
});

test('comment API failures remain visible warnings', async () => {
    const result = await publish({ error: new Error('Resource not accessible by integration') });
    assert.equal(result.warnings.length, 1);
    assert.match(result.warnings[0], /Resource not accessible by integration/);
});

for (const options of [
    { currentHead: 'c'.repeat(40) },
    { currentExempt: true },
    { exempt: true, currentExempt: false }
]) {
    test(`discard outdated comment: ${JSON.stringify(options)}`, async () => {
        assert.deepEqual((await publish(options)).calls, []);
    });
}

test('label events cannot skip the validation job or bypass its verdict', () => {
    const job = releaseNotes.slice(releaseNotes.indexOf('  check_release_notes:'));
    assert.doesNotMatch(job.split('    steps:')[0], /^    (if|name):/m);
    assert.doesNotMatch(block(releaseNotes, 'run: |'), /github\.event\.(action|label)/);
});

test('comment environment carries the validated outputs', () => {
    for (const [variable, output] of [
        ['COMMENT_BODY', 'release-notes-check-message'],
        ['RELEASE_NOTES_REQUIRED', 'release-notes-required'],
        ['RELEASE_NOTES_EXEMPT', 'release-notes-exempt']
    ]) {
        assert.ok(releaseNotes.includes(`${variable}: \${{ steps.release_notes_changes.outputs.${output} }}`));
    }
});

test('maintenance pushes preserve merge commits', () => {
    const workflow = readFileSync(join(root, '.github/workflows/labelops-pr-maintenance.lock.yml'), 'utf8');
    const config = JSON.parse(JSON.parse(workflow.match(/GH_AW_SAFE_OUTPUTS_HANDLER_CONFIG: (.+)/)[1]));
    assert.equal(config.push_to_pull_request_branch.signed_commits, false);
});

test('maintenance disables automatic comments but retains explicit explanations', () => {
    const workflow = readFileSync(join(root, '.github/workflows/labelops-pr-maintenance.lock.yml'), 'utf8');
    const messages = block(workflow, 'safe_outputs:').match(/^  GH_AW_SAFE_OUTPUT_MESSAGES: (.+)$/m);
    assert.ok(messages, 'Missing safe_outputs job message configuration');
    assert.equal(JSON.parse(JSON.parse(messages[1])).activationComments, 'false');
    const config = JSON.parse(JSON.parse(workflow.match(/GH_AW_SAFE_OUTPUTS_HANDLER_CONFIG: (.+)/)[1]));
    assert.equal(config.add_comment.max, 5);
});

test('Repo Assist uses a new scheduled-only definition without losing its memory', () => {
    const workflows = join(root, '.github/workflows');
    const path = join(workflows, 'repo-assist-scheduled.lock.yml');
    assert.ok(existsSync(path), 'The scheduled assistant needs a separate workflow registration');
    assert.ok(!existsSync(join(workflows, 'repo-assist.md')));
    assert.ok(!existsSync(join(workflows, 'repo-assist.lock.yml')));
    const workflow = readFileSync(path, 'utf8');
    const triggers = block(workflow, 'on:').split('\n').filter(line => /^[a-z_]+:$/.test(line));
    assert.deepEqual(triggers, ['schedule:', 'workflow_dispatch:']);
    assert.match(block(workflow, 'on:'), /cron: "30 \*\/12 \* \* \*"/);
    assert.match(workflow, /name: "Repo Assist"/);
    assert.match(workflow, /memory\/repo-assist/);
    assert.doesNotMatch(workflow, /memory\/repo-assist-scheduled/);
    assert.match(workflow, /runtime-import \.github\/workflows\/repo-assist-scheduled\.md/);
    assert.doesNotMatch(workflow, /\.github\/workflows\/repo-assist(?:\.md|\.lock\.yml)/);
});
