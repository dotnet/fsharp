"use strict";

const {
  POLICY_VERSION, OVERLAP_MS, LIMITS, isEligibleIssue, eventNumber,
  normalizeMemory, fingerprintHumanInput, isFinishedRecord, needsAnalysis, selectCandidates,
} = require("./core.cjs");

const MEMORY_BRANCH = "memory/regression-triage";
const MEMORY_PATH = "state.json";
const compareText = (a, b) => a < b ? -1 : a > b ? 1 : 0;
const chronological = (a, b) => compareText(a.createdAt ?? "", b.createdAt ?? "") || a.id - b.id;
const isBot = (author) => author?.type === "Bot" || /\[bot\]$/i.test(author?.login ?? "");

function readLimits(overrides) {
  const limits = { ...LIMITS, ...overrides };
  for (const [key, value] of Object.entries(limits)) {
    if (!Number.isSafeInteger(value) || value < 1) throw new Error(`Invalid ${key} bound`);
  }
  // Each path needs a boundary re-read AND a forward page to make progress.
  if (limits.issuePages < 4) throw new Error("issuePages must be at least four");
  return limits;
}

function nextPage(response, page) {
  const link = response.headers?.link ?? "";
  const next = link.split(",").find((part) => /;\s*rel="next"/.test(part));
  if (!next) return null;
  const match = next.match(/<([^>]+)>/);
  const number = match && Number(new URL(match[1]).searchParams.get("page"));
  if (!Number.isSafeInteger(number) || number !== page + 1) throw new Error("Invalid pagination Link");
  return number;
}

function apiError(error, context) {
  return { ...context, code: "request-failed", status: error.status ?? null, message: error.message };
}

// A 404 alone is ambiguous (GitHub also hides inaccessible content). Confirm an
// absent file by listing its branch, or an absent branch with default-branch read
// access and a branch lookup. Other failures never become an empty ledger.
async function readMemory(github, repo) {
  let data;
  try {
    ({ data } = await github.rest.repos.getContent({ ...repo, path: MEMORY_PATH, ref: MEMORY_BRANCH }));
  } catch (error) {
    if (error.status !== 404) throw error;
    let root;
    try {
      ({ data: root } = await github.rest.repos.getContent({ ...repo, path: "", ref: MEMORY_BRANCH }));
    } catch (rootError) {
      if (rootError.status !== 404) throw rootError;
      await github.rest.repos.getContent({ ...repo, path: "" });
      try {
        await github.rest.repos.getBranch({ ...repo, branch: MEMORY_BRANCH });
      } catch (branchError) {
        if (branchError.status === 404) return normalizeMemory(null);
        throw branchError;
      }
      throw error;
    }
    if (Array.isArray(root) && !root.some((entry) => entry.name === MEMORY_PATH)) return normalizeMemory(null);
    throw error;
  }
  if (data.type !== "file" || data.encoding !== "base64" || typeof data.content !== "string") {
    throw new Error("Unsupported memory file response");
  }
  return normalizeMemory(Buffer.from(data.content, "base64").toString("utf8"));
}

async function readPages(method, args, bound, stage, errors) {
  const items = [];
  for (let page = 1; page <= bound; page++) {
    try {
      const response = await method({ ...args, page, per_page: 100 });
      if (!Array.isArray(response.data)) throw new Error(`Invalid ${stage} response`);
      items.push(...response.data);
      if (nextPage(response, page) === null) return items;
    } catch (error) {
      errors.push(apiError(error, { stage, number: args.issue_number ?? args.pull_number, page }));
      return items;
    }
  }
  errors.push({ stage, number: args.issue_number ?? args.pull_number, code: `${stage}-page-bound`, bound });
  return items;
}

function discussion(items, prefix, kind) {
  const unique = new Map();
  for (const item of items) {
    unique.set(item.id, {
      id: item.id, sourceId: `${prefix}:${kind}:${item.id}`,
      authorId: item.user?.id ?? null, author: item.user?.login ?? null,
      createdAt: item.created_at ?? item.submitted_at ?? null,
      updatedAt: item.updated_at ?? item.submitted_at ?? null,
      url: item.html_url, body: item.body ?? "", isBot: isBot(item.user),
    });
  }
  return [...unique.values()].sort(chronological);
}

async function readText(github, repo, number, limits, includeReviews = false) {
  const { data: issue } = await github.rest.issues.get({ ...repo, issue_number: number });
  if (issue.number !== number || !Array.isArray(issue.labels)
    || !["open", "closed"].includes(issue.state) || typeof issue.title !== "string") {
    throw new Error("Invalid current issue response");
  }
  const prefix = `${repo.owner.toLowerCase()}/${repo.repo.toLowerCase()}#${number}`;
  const errors = [];
  const comments = discussion(await readPages(github.rest.issues.listComments,
    { ...repo, issue_number: number }, limits.commentPages, "comment", errors), prefix, "comment");
  const timeline = await readPages(github.rest.issues.listEventsForTimeline,
    { ...repo, issue_number: number }, limits.timelinePages, "timeline", errors);
  const isPullRequest = Object.hasOwn(issue, "pull_request");
  if (includeReviews && isPullRequest) {
    for (const [method, kind] of [
      [github.rest.pulls.listReviews, "review"], [github.rest.pulls.listReviewComments, "review-comment"],
    ]) {
      comments.push(...discussion(await readPages(method, { ...repo, pull_number: number },
        limits.reviewPages, kind, errors), prefix, kind));
    }
  }
  const humanDecisions = new Map();
  for (const item of timeline) {
    if (isBot(item.actor)) continue;
    if (!["closed", "reopened"].includes(item.event)
      && !(["labeled", "unlabeled"].includes(item.event)
        && ["Regression", "Needs-Triage"].includes(item.label?.name))) continue;
    humanDecisions.set(item.id, {
      id: item.id, sourceId: `${prefix}:timeline:${item.id}`,
      event: item.event, label: item.label?.name ?? null,
      actorId: item.actor?.id ?? null, actor: item.actor?.login ?? null,
      createdAt: item.created_at, url: item.url,
    });
  }
  return {
    number, url: issue.html_url, state: issue.state, isPullRequest,
    labels: issue.labels.map((label) => typeof label === "string" ? label : label.name),
    title: issue.title, body: issue.body ?? "", titleSourceId: `${prefix}:title`, bodySourceId: `${prefix}:body`,
    authorId: issue.user?.id ?? null, author: issue.user?.login ?? null, updatedAt: issue.updated_at,
    humanComments: comments.filter((item) => !item.isBot).sort(chronological),
    botComments: comments.filter((item) => item.isBot).sort(chronological),
    humanDecisions: [...humanDecisions.values()].sort(chronological),
    linked: [], complete: errors.length === 0, errors,
  };
}

// Only typed GitHub issue/PR references become metadata reads; never fetch a
// reporter-supplied URL. External text cannot choose a method or write target.
function references(snapshot, repo) {
  const found = new Map();
  const add = (owner, name, number) => {
    number = Number(number);
    if (!Number.isSafeInteger(number) || number < 1) return;
    const key = `${owner}/${name}#${number}`.toLowerCase();
    if (key !== `${repo.owner}/${repo.repo}#${snapshot.number}`.toLowerCase()) {
      found.set(key, { owner, repo: name, number });
    }
  };
  for (const text of [snapshot.title, snapshot.body, ...snapshot.humanComments.map((item) => item.body)]) {
    // Consume all URLs so fragments in arbitrary URLs cannot become local #refs.
    const withoutUrls = text.replace(/https?:\/\/[^\s<>"`]+/gi, (raw) => {
      const match = raw.match(/^https:\/\/github\.com\/([a-z\d-]+)\/([a-z\d_.-]+)\/(?:issues|pull)\/([1-9]\d*)(?=$|[/?#).,;!])/i);
      if (match && ![".", ".."].includes(match[2])) add(match[1], match[2], match[3]);
      return "";
    });
    for (const match of withoutUrls.matchAll(/(?:^|[\s(])#([1-9]\d*)\b/g)) add(repo.owner, repo.repo, match[1]);
  }
  return [...found.entries()].sort(([a], [b]) => compareText(a, b)).map(([, value]) => value);
}

/**
 * Snapshot: {number,url,state,isPullRequest,labels,title,body,titleSourceId,
 * bodySourceId,authorId,author,updatedAt,humanComments,humanDecisions,botComments,
 * linked,complete,errors}. Every text source has an API identity and exact text.
 * linked has the same shape with no further traversal (including PR discussion).
 * Bot receipts are available for publication deduplication, never human hashes.
 */
async function readIssueSnapshot(github, { repo, number, limits: overrides }) {
  if (!Number.isSafeInteger(number) || number < 1) throw new Error("Invalid issue number");
  const limits = readLimits(overrides);
  const snapshot = await readText(github, repo, number, limits);
  const links = references(snapshot, repo);
  if (links.length > limits.linkedItems) {
    snapshot.errors.push({ stage: "linked", number, code: "linked-item-bound", bound: limits.linkedItems });
  }
  for (const link of links.slice(0, limits.linkedItems)) {
    try {
      const linked = await readText(github, { owner: link.owner, repo: link.repo }, link.number, limits, true);
      snapshot.linked.push(linked);
      snapshot.errors.push(...linked.errors.map((error) => ({ ...error, repository: `${link.owner}/${link.repo}` })));
    } catch (error) {
      snapshot.errors.push(apiError(error, { stage: "linked", number: link.number, repository: `${link.owner}/${link.repo}` }));
    }
  }
  snapshot.complete = snapshot.errors.length === 0;
  return snapshot;
}

async function scanPath(github, repo, kind, prior, updatedThrough, now, budget) {
  const since = kind === "incremental" && updatedThrough
    ? new Date(Date.parse(updatedThrough) - OVERLAP_MS).toISOString() : null;
  let cursor = prior ?? { page: 0, boundary: [], since, startedAt: now };
  let page = Math.max(1, cursor.page);
  const discovered = [];
  const errors = [];
  let pages = 0;
  for (; pages < budget; pages++) {
    try {
      const response = await github.rest.issues.listForRepo({
        ...repo, state: "open", labels: "Needs-Triage", sort: "updated", direction: "asc",
        ...(cursor.since ? { since: cursor.since } : {}), page, per_page: 100,
      });
      if (!Array.isArray(response.data) || !response.data.every((issue) =>
        Number.isSafeInteger(issue.number) && issue.number > 0
        && Array.isArray(issue.labels) && typeof issue.updated_at === "string"
        && Number.isFinite(Date.parse(issue.updated_at)))) throw new Error("Invalid issue listing");
      discovered.push(...response.data.filter(isEligibleIssue));
      const boundary = response.data.map((issue) => [issue.number, issue.updated_at]);
      // A numeric page is not a snapshot. Re-read the prior boundary; if it has
      // shifted, restart the fixed interval instead of claiming skipped coverage.
      if (prior && page === prior.page && JSON.stringify(boundary) !== JSON.stringify(prior.boundary)) {
        cursor = { ...cursor, page: 0, boundary: [] };
        errors.push({ stage: kind, code: "boundary-changed", page });
        return { discovered, cursor, complete: false, pages: pages + 1, errors };
      }
      const next = nextPage(response, page);
      cursor = { ...cursor, page, boundary };
      if (next === null) return { discovered, cursor: null, complete: true, through: cursor.startedAt, pages: pages + 1, errors };
      page = next;
    } catch (error) {
      errors.push(apiError(error, { stage: kind, page }));
      return { discovered, cursor, complete: false, pages: pages + 1, errors };
    }
  }
  errors.push({ stage: kind, code: "page-budget", page, bound: budget });
  return { discovered, cursor, complete: false, pages, errors };
}

/**
 * Read-only manifest: selected [{number,snapshot,fingerprint,priorRecord}],
 * incomplete [{number,snapshot?}], errors, scan {incremental,sweep}, and
 * stateDelta {scan,pending}. Pending is deduplicated, retains selected work, and
 * records firstSeenAt/lastAttemptAt for fairness. A publisher must atomically
 * commit this WHOLE delta with results/intents, removing only handled work.
 * Never persist scan alone. No ledger or caller objects are mutated here.
 */
async function collectCandidates(github, { repo, event, memory, now, limits: overrides }) {
  const limits = readLimits(overrides);
  if (!Number.isFinite(Date.parse(now))) throw new Error("A valid trusted run time is required");
  now = new Date(now).toISOString();
  memory = normalizeMemory(memory, { policyVersion: memory?.policyVersion ?? POLICY_VERSION });
  const incremental = await scanPath(github, repo, "incremental", memory.scan.incremental,
    memory.scan.updatedThrough, now, Math.ceil(limits.issuePages / 2));
  const sweep = await scanPath(github, repo, "sweep", memory.scan.sweep,
    null, now, Math.floor(limits.issuePages / 2));
  const pending = new Map(memory.pending.map((entry) => [entry.number, {
    ...entry, historical: Boolean(entry.historical || !isFinishedRecord(memory.issues[entry.number], memory.policyVersion)),
  }]));
  function enqueue(number, historical, updatedAt) {
    const previous = pending.get(number);
    pending.set(number, {
      ...previous, number, firstSeenAt: previous?.firstSeenAt ?? now,
      historical: Boolean(previous?.historical || historical),
      ...(updatedAt ? { updatedAt } : {}),
    });
  }
  for (const [path, historical] of [[incremental, false], [sweep, true]]) {
    for (const issue of path.discovered) {
      const record = memory.issues[issue.number];
      enqueue(issue.number, historical || !isFinishedRecord(record, memory.policyVersion), issue.updated_at);
    }
  }
  const hint = eventNumber(event);
  if (hint !== null) enqueue(hint, !isFinishedRecord(memory.issues[hint], memory.policyVersion));
  const queue = [...pending.values()];
  const oldest = (a, b) => compareText(a.lastAttemptAt ?? "", b.lastAttemptAt ?? "")
    || compareText(a.firstSeenAt, b.firstSeenAt) || a.number - b.number;
  const historical = queue.filter((entry) => entry.historical).sort(oldest)[0];
  queue.sort((a, b) => Number(b === historical) - Number(a === historical)
    || Number(b.number === hint) - Number(a.number === hint)
    || compareText(b.updatedAt ?? "", a.updatedAt ?? "") || oldest(a, b));
  const discovered = [];
  const incomplete = [];
  const errors = [...incremental.errors, ...sweep.errors];
  for (const entry of queue.slice(0, limits.snapshotReads)) {
    pending.set(entry.number, { ...entry, lastAttemptAt: now });
    try {
      const snapshot = await readIssueSnapshot(github, { repo, number: entry.number, limits });
      if (!isEligibleIssue(snapshot)) {
        pending.delete(entry.number);
      } else if (!snapshot.complete) {
        incomplete.push({ number: entry.number, snapshot });
        errors.push(...snapshot.errors);
      } else if (!needsAnalysis(memory.issues[entry.number], snapshot, memory.policyVersion)) {
        pending.delete(entry.number);
      } else {
        discovered.push({ ...entry, snapshot });
      }
    } catch (error) {
      incomplete.push({ number: entry.number });
      errors.push(apiError(error, { stage: "snapshot", number: entry.number }));
    }
  }
  const selected = selectCandidates({ event, discovered, memory, limit: limits.candidates, now })
    .map(({ snapshot }) => ({
      number: snapshot.number, snapshot, fingerprint: fingerprintHumanInput(snapshot),
      priorRecord: memory.issues[snapshot.number] ?? null,
    }));
  const summary = ({ complete, pages, errors }) => ({ complete, pages, errors });
  return {
    policyVersion: memory.policyVersion, selected, incomplete, errors,
    scan: { incremental: summary(incremental), sweep: summary(sweep) },
    stateDelta: {
      scan: {
        ...memory.scan,
        updatedThrough: incremental.complete ? incremental.through : memory.scan.updatedThrough,
        incremental: incremental.cursor, sweep: sweep.cursor,
      },
      pending: [...pending.values()],
    },
  };
}

module.exports = {
  MEMORY_BRANCH, MEMORY_PATH, readMemory, collectCandidates, readIssueSnapshot,
};
