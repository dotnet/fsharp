const { createHash } = require("node:crypto");
const { isEligibleIssue, validateManifest, fingerprintInput } = require("./model.cjs");

const hardLimits = Object.freeze({
    commentPages: 20, timelinePages: 20, linkedTargets: 100,
    inputBytes: 4 * 1024 * 1024, sourceBytes: 1024 * 1024,
});
const requireValue = (condition, message) => { if (!condition) throw new Error(message); };
const blocked = error => ({ status: "blocked", reason: error.message });
const positive = value => Number.isSafeInteger(value) && value > 0;
const recordId = value => positive(value) || (typeof value === "string" && /^[1-9]\d*$/.test(value));
const repoParts = repository => {
    requireValue(typeof repository === "string" && /^[\w.-]+\/[\w.-]+$/.test(repository), "invalid repository");
    const [owner, repo] = repository.split("/");
    return { owner, repo };
};

function limitsFor(options) {
    const limits = { ...hardLimits };
    for (const [name, value] of Object.entries(options.limits ?? {})) {
        requireValue(Object.hasOwn(hardLimits, name) && positive(value) && value <= hardLimits[name], `invalid ${name} limit`);
        limits[name] = value;
    }
    return limits;
}

async function request(options, route, params) {
    const response = await options.transport.request(`GET /repos/{owner}/{repo}/${route}`, params);
    requireValue(response && (!response.status || response.status < 400) && response.data !== undefined, `incomplete API response: ${route}`);
    return response;
}

async function page(options, route, params, number) {
    const response = await request(options, route, { ...params, per_page: 100, page: number });
    requireValue(Array.isArray(response.data) && response.data.length <= 100, `invalid API page: ${route}`);
    return {
        rows: response.data,
        next: response.headers?.link === undefined ? response.data.length === 100
            : /;\s*rel="?next"?/.test(response.headers.link),
    };
}

async function discussionPages(options, kind, params, limits) {
    const rows = [];
    let size = 0;
    for (let number = 1; number <= limits[`${kind === "comments" ? "comment" : "timeline"}Pages`]; number++) {
        const result = await page(options, `issues/{issue_number}/${kind}`, params, number);
        size += Buffer.byteLength(JSON.stringify(result.rows));
        requireValue(size <= limits.inputBytes, `${kind} input byte limit exceeded`);
        rows.push(...result.rows);
        if (!result.next) return rows;
    }
    throw new Error(`${kind} page limit exceeded; discussion incomplete`);
}

function normalizeComment(record) {
    requireValue(recordId(record.id), "comment missing stable ID");
    requireValue(typeof record.body === "string", "comment missing body");
    requireValue(record.user && recordId(record.user.id), "comment missing writer identity");
    return Object.fromEntries(["id", "node_id", "body", "created_at", "updated_at", "user", "performed_via_github_app", "author_association"]
        .map(key => [key, record[key] ?? null]));
}

function orderedRecords(records) {
    const byId = new Map();
    for (const record of records) {
        const id = `${record.event ?? "comment"}:${record.id ?? record.node_id ?? record.sha ?? fingerprintInput(record)}`;
        const prior = byId.get(id);
        requireValue(!prior || fingerprintInput(prior) === fingerprintInput(record), "conflicting duplicate record during pagination");
        byId.set(id, record);
    }
    return [...byId].sort(([a], [b]) => a.localeCompare(b, "en", { numeric: true })).map(([, value]) => value);
}

function issueInput(issue) {
    requireValue(positive(issue.number) && recordId(issue.id) && typeof issue.title === "string" &&
        (issue.body === null || typeof issue.body === "string"), "incomplete issue metadata");
    // updated_at and comment count also change for authenticated workflow progress.
    return {
        id: issue.id, number: issue.number, title: issue.title, body: issue.body,
        state: issue.state, state_reason: issue.state_reason ?? null, created_at: issue.created_at,
        closed_at: issue.closed_at ?? null, locked: issue.locked ?? false, user: issue.user ?? null,
        labels: issue.labels.map(label => typeof label === "string" ? label : label.name).sort(),
    };
}

/**
 * All APIs take {transport: {request(route, params)}, repository, issueNumber}.
 * request is Octokit's read-only request interface and returns {data, headers?}.
 * Limits may only lower the finite defaults above; every failure returns blocked,
 * never partial frozen input. No callback means no workflow record is trusted.
 *
 * authenticateRecord(comment) must asynchronously return exactly true only after
 * authenticating the exact ID, body, writer/app and trusted workflow/run provenance.
 * It is also called for timeline "commented" records; marker/author tests are not
 * authentication. Any authenticated record requires readTrustedCheckpoints before
 * freezing. Production controllers must supply both callbacks; omitting them is
 * only suitable for untrusted read-only collection, never duplicate-safe dispatch.
 * readTrustedCheckpoints is documented by findActiveInvestigations.
 *
 * reviewedManifest is supplied only by a trusted default-branch approval loader.
 * readSource({path, sha256}) reads raw Buffer/Uint8Array bytes at that same pinned
 * revision, enforcing tracked-blob/path/symlink policy. Intake checks every hash.
 * First read without a manifest to obtain discussionFingerprint; a supplied
 * manifest must match it. A complete snapshot by itself never authorizes work.
 * canonicalInput is UTF-8 JSON; inputDigest hashes those bytes using model rules,
 * not the manifest's distinct approvedInputDigest. Investigation status is kept
 * outside input so authenticated checkpoint progress cannot invalidate itself.
 */
async function readIssueSnapshot(options) {
    try {
        const limits = limitsFor(options);
        const params = { ...repoParts(options.repository), issue_number: options.issueNumber };
        requireValue(positive(options.issueNumber), "invalid issue number");
        const { data: issue } = await request(options, "issues/{issue_number}", params);
        requireValue(issue?.number === options.issueNumber, "issue identity mismatch");
        if (!isEligibleIssue(issue)) return { status: "ineligible", reason: "issue does not meet exact eligibility" };
        const rawComments = await discussionPages(options, "comments", params, limits);
        const rawTimeline = await discussionPages(options, "timeline", params, limits);
        const comments = [];
        const timeline = [];
        const authenticate = async record => {
            const authenticated = await options.authenticateRecord?.(structuredClone(record)) === true;
            requireValue(!authenticated || typeof options.readTrustedCheckpoints === "function",
                "trusted checkpoint reader required for authenticated records");
            return authenticated;
        };
        for (const record of rawComments) {
            const normalized = normalizeComment(record);
            if (!(await authenticate(record))) comments.push(normalized);
        }
        for (const record of rawTimeline) {
            requireValue(record && typeof record.event === "string", "incomplete timeline event");
            if (record.event === "commented") {
                const normalized = normalizeComment(record);
                if (!(await authenticate(record))) timeline.push({ ...normalized, event: record.event });
            } else {
                timeline.push(record);
            }
        }
        const discussion = { issue: issueInput(issue), comments: orderedRecords(comments), timeline: orderedRecords(timeline) };
        const discussionFingerprint = fingerprintInput(discussion);
        let reproduction = null;
        if (options.reviewedManifest !== undefined) {
            const manifest = structuredClone(options.reviewedManifest);
            validateManifest(manifest);
            requireValue(manifest.repository === options.repository && manifest.issueNumber === options.issueNumber, "manifest issue mismatch");
            requireValue(manifest.discussionFingerprint === discussionFingerprint, "reviewed discussion fingerprint is stale");
            requireValue(typeof options.readSource === "function", "reviewed source reader required");
            const sources = [];
            let size = 0;
            for (const source of manifest.sources) {
                const bytes = await options.readSource({ ...source });
                requireValue(bytes instanceof Uint8Array, `source ${source.path}: raw bytes required`);
                size += bytes.byteLength;
                requireValue(size <= limits.sourceBytes, "source byte limit exceeded");
                const sha256 = createHash("sha256").update(bytes).digest("hex");
                requireValue(sha256 === source.sha256, `source ${source.path}: hash mismatch`);
                sources.push({ ...source, bytesBase64: Buffer.from(bytes).toString("base64") });
            }
            reproduction = { manifest, sources };
        }
        const investigations = await findActiveInvestigations({ ...options, issue, comments: rawComments, timeline: rawTimeline });
        if (investigations.status !== "complete") return investigations;
        const input = { schemaVersion: 1, repository: options.repository, discussion, reproduction };
        const inputDigest = fingerprintInput(input);
        const canonicalInput = JSON.stringify(input, (_, value) => value && typeof value === "object" && !Array.isArray(value)
            ? Object.fromEntries(Object.keys(value).sort().map(key => [key, value[key]])) : value);
        requireValue(Buffer.byteLength(canonicalInput) <= limits.inputBytes, "canonical input byte limit exceeded");
        return { status: "complete", snapshot: { input, canonicalInput, inputDigest, discussionFingerprint, investigations } };
    } catch (error) {
        return blocked(error);
    }
}

function issueLink(value) {
    if (typeof value !== "string") return null;
    const match = /^https:\/\/github\.com\/([\w.-]+\/[\w.-]+)\/(issues|pull)\/([1-9]\d*)(?:[/?#]|$)/.exec(value);
    if (!match || !positive(Number(match[3]))) return null;
    return { repository: match[1], kind: match[2] === "pull" ? "pull" : "issue", number: Number(match[3]) };
}

/**
 * Supply complete current issue/comments/timeline; normally called by snapshot.
 * GitHub cross-references establish a relationship; prose URLs require a backlink
 * in the freshly read target. Open, unmerged targets updated within 14 days count.
 * Arbitrary claims of ongoing work have no authority. Required linked reads block.
 *
 * readTrustedCheckpoints({repository, issueNumber, comments}) is an injected,
 * authenticated reader returning an array of {status:"loaded", state, commentId,
 * owner:{runId,attempt}, pending?:{owner}} or {status:"absent"} load results.
 * A blocked result/throw blocks intake. An empty array means a complete lookup
 * found no trusted checkpoints, not permission to ignore a known checkpoint.
 * Owner is the authenticated checkpoint writer, not the new triggering controller.
 * Pending ownership takes priority. Every live owning
 * run is re-read; only nonterminal state with that exact active attempt suppresses
 * duplication. Omission recognizes no trusted checkpoints (never trusts markers).
 * Adapt state.loadCheckpoint's successful result by taking state and pending from
 * result.checkpoint, and its authenticated result.commentId and result.owner.
 * Never substitute the new controller's owner or trust an envelope's claims.
 * Propagate read errors or unavailable required evidence as a throw or
 * status:"blocked"; do not
 * turn them into an empty array. This reader does not grant execution approval.
 * now may be a timestamp string or epoch milliseconds for deterministic tests.
 */
async function findActiveInvestigations(options) {
    try {
        const limits = limitsFor(options);
        const targets = new Map();
        const add = (link, basis) => {
            if (!link || (link.repository === options.repository && link.number === options.issueNumber)) return;
            const key = `${link.repository}#${link.number}`;
            const existing = targets.get(key);
            targets.set(key, { ...link, basis: existing?.basis === "timeline-cross-reference" ? existing.basis : basis });
            requireValue(targets.size <= limits.linkedTargets, "linked target limit exceeded");
        };
        for (const event of options.timeline) {
            if (event.event === "cross-referenced" && event.source?.issue) {
                const link = issueLink(event.source.issue.html_url);
                requireValue(link, "required linked timeline target is unreadable");
                add(link, "timeline-cross-reference");
            }
        }
        for (const record of [options.issue, ...options.comments]) {
            for (const match of (record.body ?? "").matchAll(/https:\/\/github\.com\/[\w.-]+\/[\w.-]+\/(?:issues|pull)\/\d+\b/g)) {
                add(issueLink(match[0]), "reciprocal-reference");
            }
        }
        const now = options.now === undefined ? Date.now() : typeof options.now === "number" ? options.now : Date.parse(options.now);
        requireValue(Number.isFinite(now), "invalid activity timestamp");
        const links = [];
        const active = [];
        for (const target of targets.values()) {
            let value;
            const params = repoParts(target.repository);
            try {
                if (target.kind === "pull") {
                    value = (await request(options, "pulls/{pull_number}", { ...params, pull_number: target.number })).data;
                } else {
                    value = (await request(options, "issues/{issue_number}", { ...params, issue_number: target.number })).data;
                    if (Object.hasOwn(value, "pull_request")) {
                        target.kind = "pull";
                        value = (await request(options, "pulls/{pull_number}", { ...params, pull_number: target.number })).data;
                    }
                }
            } catch (error) {
                throw new Error(`required linked target ${target.repository}#${target.number}: ${error.message}`);
            }
            requireValue(value?.number === target.number && ["open", "closed"].includes(value.state), "incomplete linked target status");
            requireValue(target.kind !== "pull" || value.merged_at === null || typeof value.merged_at === "string",
                "incomplete linked PR merged status");
            const updated = Date.parse(value.updated_at);
            requireValue(Number.isFinite(updated) && updated <= now, "invalid linked target activity");
            const text = `${value.title ?? ""}\n${value.body ?? ""}`;
            const reciprocal = [...text.matchAll(/https:\/\/github\.com\/[\w.-]+\/[\w.-]+\/issues\/\d+\b/g)]
                .some(match => {
                    const link = issueLink(match[0]);
                    return link?.repository === options.repository && link.number === options.issueNumber;
                }) || (target.repository === options.repository && new RegExp(`(?:^|\\s)#${options.issueNumber}(?!\\d)`).test(text));
            const current = {
                ...target, state: value.state, merged: !!value.merged_at, updatedAt: value.updated_at,
                active: value.state === "open" && !value.merged_at && now - updated <= 14 * 86400000 &&
                    (target.basis === "timeline-cross-reference" || reciprocal),
            };
            links.push(current);
            if (current.active) active.push(current);
        }
        const checkpoints = options.readTrustedCheckpoints === undefined ? [] : await options.readTrustedCheckpoints({
            repository: options.repository, issueNumber: options.issueNumber, comments: structuredClone(options.comments),
        });
        requireValue(Array.isArray(checkpoints) && checkpoints.length <= limits.linkedTargets, "invalid trusted checkpoint results");
        for (const loaded of checkpoints) {
            if (loaded?.status === "absent") continue;
            requireValue(loaded?.status === "loaded", `trusted checkpoint blocked: ${loaded?.reason ?? "load failed"}`);
            requireValue(loaded.state?.repository === options.repository && loaded.state?.issueNumber === options.issueNumber,
                "trusted checkpoint issue mismatch");
            const owner = loaded.pending?.owner ?? loaded.owner;
            if (!owner) {
                requireValue(!loaded.state.budget?.pendingWork, "trusted checkpoint pending owner missing");
                continue;
            }
            requireValue(recordId(owner.runId) && positive(owner.attempt), "trusted checkpoint run ownership missing");
            let run;
            try {
                run = (await request(options, "actions/runs/{run_id}/attempts/{attempt_number}", {
                    ...repoParts(options.repository), run_id: owner.runId, attempt_number: owner.attempt,
                })).data;
            } catch (error) {
                throw new Error(`required checkpoint run: ${error.message}`);
            }
            requireValue(String(run.id) === String(owner.runId) && run.run_attempt === owner.attempt, "checkpoint run ownership mismatch");
            requireValue(["completed", "queued", "in_progress", "waiting", "pending", "requested"].includes(run.status), "unknown checkpoint run status");
            if (run.status !== "completed" && !["complete", "blocked", "stopped", "published"].includes(loaded.state.phase)) {
                active.push({ repository: options.repository, number: options.issueNumber, commentId: loaded.commentId,
                    basis: "trusted-checkpoint-active-run", owner: { ...owner }, status: run.status });
            }
        }
        return { status: "complete", links, active };
    } catch (error) {
        return blocked(error);
    }
}

function restartCursor(ordinal) {
    requireValue(Number.isSafeInteger(ordinal) && ordinal >= 0, "scheduleOrdinal must be a nonnegative safe integer");
    // Triangular rounds revisit pages 1; 1,2; 1,2,3; ... . Each round rotates
    // the three-item starting slot, so repeated cursor loss starves neither a
    // later page nor the remaining eligible candidates on a densely eligible page.
    const round = Math.floor((Math.sqrt(8 * ordinal + 1) - 1) / 2);
    return { version: 1, page: ordinal - round * (round + 1) / 2 + 1, offset: (round % 34) * 3 };
}

/**
 * Manual/event paths supply issueNumber and re-read it with the shared predicate.
 * Scheduled paths omit it: at most two REST created-ascending 100-item pages and
 * three identities per sweep. Persist nextCursor only after consuming candidates;
 * its page/offset resumes before any overflow candidate, then wraps after the end.
 * The cursor is discovery optimization, never issue state or deduplication.
 * Missing/corrupt cursor restarts using scheduleOrdinal, a monotonic independent
 * schedule slot (not a counter stored with the cursor). Ordinal defaults to zero
 * for a one-off restart; scheduled callers MUST supply it to guarantee recovery.
 * A shortened page restarts its offset; failures return no candidates and do not
 * advance the supplied cursor. Candidate eligibility must be rechecked at intake.
 */
async function discoverEligibleIssues(options) {
    try {
        const params = repoParts(options.repository);
        if (options.issueNumber !== undefined) {
            requireValue(positive(options.issueNumber), "invalid issue number");
            const { data } = await request(options, "issues/{issue_number}", { ...params, issue_number: options.issueNumber });
            requireValue(data?.number === options.issueNumber, "issue identity mismatch");
            return { status: "complete", issues: isEligibleIssue(data) ? [{ repository: options.repository, number: data.number }] : [], nextCursor: null };
        }
        const saved = options.cursor;
        let cursor = saved?.version === 1 && positive(saved.page) && Number.isInteger(saved.offset) && saved.offset >= 0 && saved.offset < 100
            ? { ...saved } : restartCursor(options.scheduleOrdinal ?? 0);
        const issues = [];
        const seen = new Set();
        for (let reads = 0; reads < 2; reads++) {
            const result = await page(options, "issues", { ...params, state: "open", labels: "Regression", sort: "created", direction: "asc" }, cursor.page);
            const start = cursor.offset < result.rows.length ? cursor.offset : 0;
            for (let index = start; index < result.rows.length; index++) {
                const value = result.rows[index];
                requireValue(positive(value?.number), "issue listing missing identity");
                if (isEligibleIssue(value) && !seen.has(value.number)) {
                    seen.add(value.number);
                    issues.push({ repository: options.repository, number: value.number });
                }
                if (issues.length === 3) {
                    cursor = index + 1 < result.rows.length ? { ...cursor, offset: index + 1 }
                        : { version: 1, page: result.next ? cursor.page + 1 : 1, offset: 0 };
                    return { status: "complete", issues, nextCursor: cursor };
                }
            }
            cursor = { version: 1, page: result.next ? cursor.page + 1 : 1, offset: 0 };
            if (!result.next) break;
        }
        return { status: "complete", issues, nextCursor: cursor };
    } catch (error) {
        return { ...blocked(error), issues: [], nextCursor: options.cursor ?? null };
    }
}

module.exports = { readIssueSnapshot, discoverEligibleIssues, findActiveInvestigations };
