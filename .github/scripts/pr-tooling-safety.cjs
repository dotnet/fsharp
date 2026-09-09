const { createHash } = require('node:crypto');
const fs = require('node:fs/promises');
const path = require('node:path');

const BRANCH = 'safety/scanned-PRs';
const CUTOFF = Date.parse('2026-05-12T00:00:00Z');
const WORKFLOW = '<!-- gh-aw-workflow-call-id: dotnet/fsharp/labelops-pr-security-scan -->';
const CATEGORIES = [
    'Affects-Build-Infra', 'Affects-Compiler-Output', 'Affects-Bootstrap',
    'Affects-Restore', 'Affects-Design-Time', 'Affects-Test-Tooling',
    'Affects-Agent-Config', 'Suspicious-Prompting', 'Scope-Review-Needed',
];
const CLEAN = 'AI-Tooling-Check-Scanned-Clean';
const BYPASSED = 'AI-Tooling-Check-Bypassed';
const WARNING = '\u26a0\ufe0f ';
const MANAGED_LABELS = [CLEAN, BYPASSED, ...CATEGORIES.map(category => WARNING + category)];
const hash = value => createHash('sha256').update(JSON.stringify(value)).digest('hex');
const equal = (left, right) => JSON.stringify(left) === JSON.stringify(right);
class ScanInputError extends Error {}
const requireValue = (condition, message) => {
    if (!condition) throw new ScanInputError(message);
};
const fullSha = value => typeof value === 'string' && /^[a-f0-9]{40}$/.test(value);
const object = value => value !== null && typeof value === 'object' && !Array.isArray(value);
const categories = value => {
    requireValue(Array.isArray(value) && value.every(category => CATEGORIES.includes(category)),
        'Invalid scan categories');
    return [...new Set(value)].sort();
};

function metadata(pr) {
    requireValue(Number.isSafeInteger(pr.number) && fullSha(pr.head?.sha) && fullSha(pr.base?.sha)
        && pr.head.repo?.full_name && pr.base.repo?.full_name && typeof pr.title === 'string'
        && (pr.body === null || typeof pr.body === 'string')
        && Number.isFinite(Date.parse(pr.created_at)), `Incomplete PR metadata: ${pr.number}`);
    return hash([pr.head.sha, pr.head.repo.full_name, pr.base.repo.full_name, pr.base.ref, pr.title, pr.body || '']);
}

function eligible(pr) {
    requireValue(Number.isFinite(Date.parse(pr.created_at)), `Invalid PR creation date: ${pr.number}`);
    return Date.parse(pr.created_at) >= CUTOFF;
}

async function readState(github, repo) {
    const file = (await github.rest.repos.getContent({ ...repo, path: 'state.json', ref: BRANCH })).data;
    requireValue(file.type === 'file', 'Scan state is not a readable file');
    const blob = file.encoding === 'none'
        ? (await github.rest.git.getBlob({ ...repo, file_sha: file.sha })).data
        : file;
    requireValue(blob.encoding === 'base64', 'Scan state is not a readable blob');
    const state = JSON.parse(Buffer.from(blob.content, 'base64').toString('utf8'));
    requireValue(object(state) && object(state.prs), 'Invalid scan state; refusing to start the classifier');
    const legacy = state.version === undefined;
    requireValue(legacy || (state.version === 2 && Number.isFinite(Date.parse(state.initializedAt))),
        'Unsupported scan state; refusing to start the classifier');
    for (const [number, entry] of Object.entries(state.prs)) {
        requireValue(/^[1-9][0-9]*$/.test(number) && object(entry), 'Invalid scan state entry');
        categories(entry.cats);
        if (!legacy && entry.uninitialized === true) continue;
        if (legacy || entry.legacy) {
            requireValue(typeof entry.sha === 'string' && /^[a-f0-9]{7,40}$/.test(entry.sha), 'Invalid legacy SHA');
        } else {
            const input = entry.input || entry.inflight?.input;
            requireValue(object(input) && fullSha(input.head)
                && fullSha(input.base) && fullSha(input.mergeBase)
                && /^[a-f0-9]{64}$/.test(input.metadata)
                && /^[a-f0-9]{64}$/.test(input.id), `Invalid scan identity: ${number}`);
            requireValue(entry.inflight || entry.baseline === true || (typeof entry.pending === 'boolean' && object(entry.reasons)),
                `Missing classification state: ${number}`);
            if (entry.notifiedCats !== null) categories(entry.notifiedCats);
        }
    }
    let sha = file.sha;
    let previous = JSON.stringify(state);
    return {
        state,
        legacy,
        async save() {
            const content = JSON.stringify(state);
            if (content === previous) return;
            const result = await github.rest.repos.createOrUpdateFileContents({
                ...repo, path: 'state.json', branch: BRANCH, sha,
                message: 'Update tooling safety scan state',
                content: Buffer.from(`${JSON.stringify(state, null, 2)}\n`).toString('base64'),
            });
            sha = result.data.content.sha;
            previous = content;
        },
    };
}

async function inputFor(github, repo, pr, entry) {
    const metadataId = metadata(pr);
    let mergeBase = pr.head.sha;
    if (pr.head.repo.full_name !== `${repo.owner}/${repo.repo}`) {
        if (entry?.input?.metadata === metadataId && entry.input.base === pr.base.sha) {
            return { ...entry.input };
        }
        mergeBase = (await github.rest.repos.compareCommitsWithBasehead({
            ...repo, basehead: `${pr.base.sha}...${pr.head.sha}`, per_page: 1,
        })).data.merge_base_commit?.sha;
        requireValue(fullSha(mergeBase), `Missing merge base: ${pr.number}`);
    }
    return { id: hash([metadataId, mergeBase]), metadata: metadataId, mergeBase, base: pr.base.sha, head: pr.head.sha };
}

async function snapshot(github, repo, pr, input) {
    const [files, commits] = await Promise.all([
        github.paginate(github.rest.pulls.listFiles, { ...repo, pull_number: pr.number, per_page: 100 }),
        github.paginate(github.rest.pulls.listCommits, { ...repo, pull_number: pr.number, per_page: 100 }),
    ]);
    requireValue(files.length === pr.changed_files && commits.length === pr.commits,
        `Incomplete file or commit list: ${pr.number}`);
    requireValue(files.every(file => file.changes === 0 || typeof file.patch === 'string'),
        `Missing diff text (binary or truncated file): ${pr.number}`);
    requireValue(files.every(file => !file.patch
        || (file.patch.split('\n').filter(line => line.startsWith('+')).length === file.additions
            && file.patch.split('\n').filter(line => line.startsWith('-')).length === file.deletions)),
        `Truncated file patch: ${pr.number}`);
    const diff = (await github.rest.pulls.get({
        ...repo, pull_number: pr.number, mediaType: { format: 'diff' },
    })).data;
    requireValue(typeof diff === 'string' && (files.length === 0 || diff.startsWith('diff --git ')),
        `Invalid diff: ${pr.number}`);
    requireValue(diff.split('\n').filter(line => line.startsWith('diff --git ')).length === files.length
        && files.every(file => !file.patch || diff.includes(file.patch)), `Incomplete diff: ${pr.number}`);
    const after = (await github.rest.pulls.get({ ...repo, pull_number: pr.number })).data;
    requireValue(after.state === 'open' && !after.draft && metadata(after) === input.metadata
        && after.base.sha === input.base, `PR changed while collecting its diff: ${pr.number}`);
    return {
        number: pr.number, input, title: pr.title, body: pr.body || '',
        files: files.map(file => ({ path: file.filename, previousPath: file.previous_filename, status: file.status })),
        commits: commits.map(commit => commit.commit.message), diff,
    };
}

async function select({ github, context, core, directory }) {
    const repo = context.repo;
    const store = await readState(github, repo);
    const { state } = store;
    // Absence from this inventory is never evidence that a saved PR is closed.
    const prs = (await github.paginate(github.rest.pulls.list, {
        ...repo, state: 'open', sort: 'created', direction: 'asc', per_page: 100,
    })).filter(eligible);
    if (store.legacy) {
        state.version = 2;
        state.initializedAt = new Date().toISOString();
        for (const entry of Object.values(state.prs)) entry.legacy = true;
        for (const pr of prs) state.prs[pr.number] ||= { uninitialized: true, cats: [], notifiedCats: null };
    }
    const manifest = { candidates: [], policy: null, incomplete: [] };
    for (const [number, entry] of Object.entries(state.prs)) {
        if (entry.posting) await recoverPosting(github, repo, number, entry);
    }
    for (const listed of prs) {
        const number = String(listed.number);
        const entry = state.prs[number];
        requireValue(entry || store.legacy || Date.parse(listed.created_at) > Date.parse(state.initializedAt),
            `Missing history for existing PR ${number}; restore state instead of rescanning`);
        try {
            if (listed.draft && !store.legacy) continue;
            const input = await inputFor(github, repo, listed, entry);
            if (entry?.inflight?.input.id === input.id && entry.inflight.run !== context.runId) {
                manifest.incomplete.push(listed.number);
                core.warning(`PR ${number}: rerun failed jobs in scan run ${entry.inflight.run}; not repeating classification`);
                continue;
            }
            if (store.legacy || entry?.legacy || entry?.uninitialized) {
                let unchanged = entry.uninitialized === true;
                if (!unchanged) {
                    const commit = (await github.rest.repos.getCommit({ ...repo, ref: entry.sha })).data;
                    requireValue(fullSha(commit.sha) && commit.sha.startsWith(entry.sha), `Unresolved legacy SHA: ${number}`);
                    unchanged = commit.sha === input.head;
                }
                if (unchanged) {
                    state.prs[number] = {
                        input, cats: categories(entry.cats), notifiedCats: entry.uninitialized ? null : categories(entry.cats),
                        baseline: true,
                    };
                    core.info(`PR ${number}: preserving legacy baseline without classification`);
                    continue;
                }
            }
            if (entry?.input?.id === input.id) {
                entry.input = input;
                continue;
            }
            if (listed.draft) continue;
            if (listed.head.repo.full_name === `${repo.owner}/${repo.repo}`) {
                state.prs[number] = { input, cats: [], reasons: {}, bypass: true, pending: true, notifiedCats: entry?.notifiedCats ?? null };
                continue;
            }
            if (manifest.candidates.length >= 25) continue;
            const pr = (await github.rest.pulls.get({ ...repo, pull_number: listed.number })).data;
            requireValue(metadata(pr) === input.metadata && pr.base.sha === input.base && !pr.draft && pr.state === 'open',
                `PR changed during selection: ${number}`);
            manifest.candidates.push(await snapshot(github, repo, pr, input));
            state.prs[number] ||= { cats: [], notifiedCats: null };
            state.prs[number].inflight = { input, run: context.runId };
        } catch (error) {
            if (!(error instanceof ScanInputError) && ![404, 409, 422, 429, 500, 502, 503, 504].includes(error.status)) throw error;
            manifest.incomplete.push(listed.number);
            core.warning(`PR ${number}: ${error.message}`);
        }
    }
    const workflow = await fs.readFile('.github/workflows/labelops-pr-security-scan.md', 'utf8');
    const rules = await fs.readFile('.github/tooling-check-repo-rules.md', 'utf8');
    requireValue(workflow.includes('<categories>'), 'Missing classifier rules');
    manifest.policy = hash([workflow.slice(workflow.indexOf('\n# PR Tooling Safety Check')), rules]);
    await store.save();
    await fs.mkdir(directory, { recursive: true });
    await fs.writeFile(path.join(directory, 'manifest.json'), JSON.stringify(manifest));
    // Line-oriented text remains readable with the classifier's read-only, paginated tool.
    const candidates = manifest.candidates.map(({ body, commits, diff, ...candidate }) => ({
        ...candidate, body: body.split('\n'), commits: commits.map(message => message.split('\n')), diff: diff.split('\n'),
    }));
    await fs.writeFile(path.join(directory, 'candidates.json'), JSON.stringify(candidates, null, 2));
    await fs.writeFile(path.join(directory, 'rules.md'), rules);
    core.setOutput('has_work', manifest.candidates.length > 0 ? 'true' : 'false');
    core.info(`Selected ${manifest.candidates.length} changed PRs; ${prs.length - manifest.candidates.length} require no classifier attention`);
    return manifest;
}

function resultsFor(manifest, output) {
    requireValue(object(output) && Array.isArray(output.items), 'Missing classifier output');
    const results = new Map();
    for (const item of output.items) {
        if (['noop', 'report_incomplete', 'missing_data', 'missing_tool'].includes(item.type)) continue;
        requireValue(item.type === 'classification', `Unexpected classifier output: ${item.type}`);
        const candidate = manifest.candidates.find(pr => pr.number === item.number && pr.input.id === item.input_id);
        requireValue(candidate && !results.has(item.number), 'Unrequested, stale, or duplicate classification');
        const findings = JSON.parse(item.findings);
        requireValue(object(findings), 'Findings must be a category-to-reason object');
        const cats = categories(Object.keys(findings));
        requireValue(cats.every(category => typeof findings[category] === 'string'
            && findings[category].trim().length > 0 && findings[category].length <= 160
            && !/[\r\n@<>`]/.test(findings[category])
            && findings[category].trim().split(/\s+/).length <= 10), 'Invalid classification reason');
        results.set(item.number, { candidate, cats, reasons: findings });
    }
    return results;
}

function scannerComments(comments) {
    return comments.filter(comment => comment.user?.login === 'github-actions[bot]'
        && comment.body?.includes(WORKFLOW)).sort((a, b) => b.id - a.id);
}

async function recoverPosting(github, repo, number, entry) {
    const comments = scannerComments(await github.paginate(github.rest.issues.listComments, {
        ...repo, issue_number: Number(number), per_page: 100,
    }));
    const comment = comments.find(comment => comment.body.includes(`<!-- tooling-safety:v2 ${entry.posting.key} -->`));
    requireValue(comment, `Uncertain comment POST for PR ${number} in run ${entry.posting.run}; reconcile it before retrying`);
    entry.commentId = comment.id;
    entry.notifiedKey = entry.posting.key;
    entry.notifiedCats = entry.cats;
    delete entry.posting;
}

async function publish({ github, context, core, manifest, output }) {
    const repo = context.repo;
    const store = await readState(github, repo);
    requireValue(!store.legacy, 'Selector must migrate state before publishing');
    const { state } = store;
    const results = output === null ? new Map() : resultsFor(manifest, output);
    requireValue(output !== null || manifest.candidates.length === 0, 'Classification was skipped despite selected PRs');
    for (const { candidate, cats, reasons } of results.values()) {
        const old = state.prs[candidate.number];
        if (!old?.inflight && old?.input?.id === candidate.input.id && old.policy === manifest.policy) continue;
        requireValue(old?.inflight?.input.id === candidate.input.id && old.inflight.run === context.runId,
            'Classification does not belong to the pending scan');
        requireValue(!old?.posting, `Unresolved comment publication for PR ${candidate.number}`);
        state.prs[candidate.number] = {
            input: candidate.input, cats, reasons, policy: manifest.policy, pending: true,
            notifiedCats: old.notifiedCats ?? (old.legacy ? categories(old.cats) : null), notifiedKey: old.notifiedKey ?? null,
        };
    }
    // Persist completed classifications before any fallible label/comment writes.
    await store.save();
    let labelsLeft = 50;
    let commentsLeft = 25;
    for (const [number, entry] of Object.entries(state.prs)) {
        if (!entry.pending) continue;
        const pr = (await github.rest.pulls.get({ ...repo, pull_number: Number(number) })).data;
        if (!eligible(pr) || pr.state !== 'open' || pr.draft) continue;
        const input = await inputFor(github, repo, pr, entry);
        if (input.id !== entry.input.id) {
            core.info(`PR ${number}: not publishing a superseded classification`);
            continue;
        }
        const desired = entry.bypass ? [BYPASSED] : entry.cats.length ? entry.cats.map(cat => WARNING + cat) : [CLEAN];
        const existing = pr.labels.map(label => label.name);
        const add = desired.filter(label => !existing.includes(label));
        const remove = existing.filter(label => MANAGED_LABELS.includes(label) && !desired.includes(label));
        if (add.length + remove.length > labelsLeft || commentsLeft === 0) {
            core.info(`PR ${number}: publication deferred by output limit`);
            continue;
        }
        labelsLeft -= add.length + remove.length;
        for (const name of remove) await github.rest.issues.removeLabel({ ...repo, issue_number: Number(number), name });
        if (add.length) await github.rest.issues.addLabels({ ...repo, issue_number: Number(number), labels: add });
        if (!entry.bypass && entry.cats.length && !equal(entry.cats, entry.notifiedCats)) {
            const comments = scannerComments(await github.paginate(github.rest.issues.listComments, {
                ...repo, issue_number: Number(number), per_page: 100,
            }));
            const key = entry.posting?.key || hash([number, entry.input.id, entry.cats, entry.notifiedKey]);
            const marker = `<!-- tooling-safety:v2 ${key} -->`;
            let comment = comments.find(comment => comment.body.includes(marker));
            if (!comment) {
                requireValue(!entry.posting,
                    `Uncertain comment POST for PR ${number} in run ${entry.posting?.run}; reconcile it before retrying`);
                entry.posting = { key, run: context.runId };
                await store.save();
                comment = (await github.rest.issues.createComment({
                    ...repo, issue_number: Number(number),
                    body: [
                        `\u{1f50d} Tooling Safety Check \u2014 ${entry.cats.join(', ')}`,
                        ...entry.cats.map(cat => `${cat}: ${entry.reasons[cat]}`),
                        '', `Scan: \`${entry.input.head}\` \u00b7 [workflow run](https://github.com/${repo.owner}/${repo.repo}/actions/runs/${context.runId})`,
                        '', marker, WORKFLOW,
                    ].join('\n'),
                })).data;
                commentsLeft--;
            }
            entry.commentId = comment.id;
            entry.notifiedKey = key;
            entry.notifiedCats = entry.cats;
            delete entry.posting;
            await store.save();
            for (const older of comments.filter(older => older.id !== comment.id)) {
                await github.graphql(
                    'mutation($id: ID!) { minimizeComment(input: {subjectId: $id, classifier: OUTDATED}) { minimizedComment { isMinimized } } }',
                    { id: older.node_id },
                );
            }
        }
        entry.notifiedCats = entry.cats;
        entry.pending = false;
        await store.save();
    }
    requireValue(results.size === manifest.candidates.length, 'Incomplete classifications; completed results were saved for reuse');
    requireValue(!manifest.incomplete?.length, `Incomplete PR inputs: ${manifest.incomplete?.join(', ')}`);
}

module.exports = { select, publish, inputFor, eligible, readState, CATEGORIES };
