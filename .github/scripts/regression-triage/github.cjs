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
const validLabels = (labels) => Array.isArray(labels)
  && labels.every((label) => typeof label === "string" || typeof label?.name === "string");

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
async function readMemory(github, repo, { versioned = false } = {}) {
  let headOid = null;
  if (versioned) {
    try {
      headOid = (await github.rest.repos.getBranch({ ...repo, branch: MEMORY_BRANCH })).data.commit.sha;
    } catch (error) {
      if (error.status !== 404) throw error;
      await github.rest.repos.getContent({ ...repo, path: "" });
      try {
        headOid = (await github.rest.repos.getBranch({ ...repo, branch: MEMORY_BRANCH })).data.commit.sha;
      } catch (confirmation) {
        if (confirmation.status !== 404) throw confirmation;
        return { headOid: null, state: normalizeMemory(null), missing: "branch" };
      }
    }
    if (typeof headOid !== "string" || !/^[a-f0-9]{40}$/.test(headOid)) throw new Error("Invalid memory head");
  }
  const ref = headOid ?? MEMORY_BRANCH;
  const result = (state, missing = null) => versioned ? { headOid, state, missing } : state;
  let data;
  try {
    ({ data } = await github.rest.repos.getContent({ ...repo, path: MEMORY_PATH, ref }));
  } catch (error) {
    if (error.status !== 404) throw error;
    let root;
    try {
      ({ data: root } = await github.rest.repos.getContent({ ...repo, path: "", ref }));
    } catch (rootError) {
      if (rootError.status !== 404 || versioned) throw rootError;
      await github.rest.repos.getContent({ ...repo, path: "" });
      try {
        await github.rest.repos.getBranch({ ...repo, branch: MEMORY_BRANCH });
      } catch (branchError) {
        if (branchError.status === 404) return normalizeMemory(null);
        throw branchError;
      }
      throw error;
    }
    if (Array.isArray(root) && !root.some((entry) => entry.name === MEMORY_PATH)) return result(normalizeMemory(null), "file");
    throw error;
  }
  if (data.type !== "file" || data.encoding !== "base64" || typeof data.content !== "string") {
    throw new Error("Unsupported memory file response");
  }
  return result(normalizeMemory(Buffer.from(data.content, "base64").toString("utf8")));
}

async function readPages(method, args, bound, stage, errors) {
  let items = [];
  let previous;
  let unstable = false;
  let page = 1;
  const context = { stage, number: args.issue_number ?? args.pull_number, retryable: true };
  // Two matching ordered enumerations detect shifts and edits, not atomicity.
  // Every request, including consistency rereads, consumes the same page budget.
  for (let calls = 0; calls < bound; calls++) {
    try {
      const response = await method({ ...args, page, per_page: 100 });
      if (!Array.isArray(response.data) || !response.data.every((item) => item && typeof item === "object"
        && (stage !== "timeline" || typeof item.event === "string")
        && (stage === "timeline" && !["labeled", "unlabeled", "closed", "reopened"].includes(item.event)
          || Number.isSafeInteger(item.id) && item.id > 0)
        && (item.body == null || typeof item.body === "string"))) throw new Error(`Invalid ${stage} response`);
      items.push(...response.data);
      const next = nextPage(response, page);
      if (next !== null) {
        page = next;
        continue;
      }
      const ids = items.filter((item) => item.id != null)
        .map((item) => stage === "timeline" ? `${item.event}:${item.id}` : item.id);
      if (new Set(ids).size !== ids.length) throw new Error(`Duplicate ${stage} identities`);
      const stamp = JSON.stringify(items.map((item) => [
        item.id, item.user?.id, item.user?.login, item.user?.type, item.actor?.id, item.actor?.login, item.actor?.type,
        item.event, item.label?.name, item.body, item.created_at, item.updated_at, item.submitted_at, item.html_url, item.url,
      ]));
      if (stamp === previous) return items;
      unstable ||= previous !== undefined;
      previous = stamp;
      if (calls + 1 < bound) items = [];
      page = 1;
    } catch (error) {
      errors.push(apiError(error, { ...context, page }));
      return items;
    }
  }
  errors.push({ ...context, code: `${stage}-${unstable ? "unstable" : "page-bound"}`, bound });
  return items;
}

function discussion(items, prefix, kind) {
  const unique = new Map();
  for (const item of items) {
    unique.set(item.id, {
      id: item.id, sourceId: `${prefix}:${kind}:${item.id}`,
      authorId: item.user?.id ?? null, author: item.user?.login ?? null, authorType: item.user?.type ?? null,
      createdAt: item.created_at ?? item.submitted_at ?? null,
      updatedAt: item.updated_at ?? item.submitted_at ?? null,
      url: item.html_url, body: item.body ?? "", isBot: isBot(item.user),
    });
  }
  return [...unique.values()].sort(chronological);
}

async function readText(github, repo, number, limits, isLinked = false) {
  const requested = { ...repo, issue_number: number };
  const { data: issue } = await github.rest.issues.get(requested);
  const location = issue.html_url?.match(/^https:\/\/github\.com\/([a-z\d-]+)\/([a-z\d_.-]+)\/(issues|pull)\/([1-9]\d*)$/i);
  const isPullRequest = Object.hasOwn(issue, "pull_request");
  // Root publication/ledger keys cannot migrate; linked evidence may follow transfers.
  if (!Number.isSafeInteger(issue.number) || issue.number < 1 || (!isLinked && issue.number !== number)
    || !location || [".", ".."].includes(location[2]) || Number(location[4]) !== issue.number
    || !validLabels(issue.labels)
    || !["open", "closed"].includes(issue.state) || typeof issue.title !== "string"
    || (issue.body != null && typeof issue.body !== "string")
    || typeof issue.updated_at !== "string" || !Number.isFinite(Date.parse(issue.updated_at))
    || (issue.created_at != null && (typeof issue.created_at !== "string" || !Number.isFinite(Date.parse(issue.created_at))))
    || (issue.comments !== undefined && (!Number.isSafeInteger(issue.comments) || issue.comments < 0))) {
    throw new Error("Invalid current issue response");
  }
  if (!isLinked && `${location[1]}/${location[2]}`.toLowerCase() !== "dotnet/fsharp") {
    throw new Error("Current issue is outside the publication repository");
  }
  repo = { owner: location[1], repo: location[2] };
  number = issue.number;
  const prefix = `${repo.owner.toLowerCase()}/${repo.repo.toLowerCase()}#${number}`;
  const errors = [];
  const comments = discussion(await readPages(github.rest.issues.listComments,
    { ...repo, issue_number: number }, limits.commentPages, "comment", errors), prefix, "comment");
  const commentCount = comments.length;
  const timeline = await readPages(github.rest.issues.listEventsForTimeline,
    { ...repo, issue_number: number }, limits.timelinePages, "timeline", errors);
  if (isLinked && isPullRequest) {
    for (const [method, kind] of [
      [github.rest.pulls.listReviews, "review"], [github.rest.pulls.listReviewComments, "review-comment"],
    ]) {
      comments.push(...discussion(await readPages(method, { ...repo, pull_number: number },
        limits.reviewPages, kind, errors), prefix, kind));
    }
  }
  const { data: current } = await github.rest.issues.get(requested);
  const metadata = (value) => JSON.stringify([
    value.number, value.title, value.body, value.state, value.user?.id, value.html_url,
    Object.hasOwn(value, "pull_request"), value.comments, value.updated_at, value.created_at,
    value.labels?.map((label) => typeof label === "string" ? label : label.name).sort(),
  ]);
  if (metadata(issue) !== metadata(current)) {
    errors.push({ stage: "issue", number, code: "issue-changed", retryable: true });
  }
  if (current.comments !== undefined && current.comments !== commentCount) {
    errors.push({ stage: "comment", number, code: "comment-count-mismatch", retryable: true });
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
    authorId: issue.user?.id ?? null, author: issue.user?.login ?? null,
    createdAt: issue.created_at ?? null, updatedAt: issue.updated_at,
    humanComments: comments.filter((item) => !item.isBot).sort(chronological),
    botComments: comments.filter((item) => item.isBot).sort(chronological),
    humanDecisions: [...humanDecisions.values()].sort(chronological),
    linked: [], complete: errors.length === 0, errors,
  };
}

// Only typed GitHub issue/PR references become metadata reads; never fetch a
// reporter-supplied URL. External text cannot choose a method or write target.
function references(snapshot) {
  const [owner, name] = snapshot.bodySourceId.split("#")[0].split("/");
  const repo = { owner, repo: name };
  const found = new Map();
  const add = (owner, name, number) => {
    number = Number(number);
    if (!Number.isSafeInteger(number) || number < 1 || [".", ".."].includes(name)) return;
    const key = `${owner}/${name}#${number}`.toLowerCase();
    if (key !== `${repo.owner}/${repo.repo}#${snapshot.number}`.toLowerCase()) {
      found.set(key, { owner, repo: name, number });
    }
  };
  for (const text of [snapshot.title, snapshot.body, ...snapshot.humanComments.map((item) => item.body)]) {
    // Consume all URLs so fragments in arbitrary URLs cannot become local #refs.
    const urls = /(?:[a-z][a-z\d+.-]*:)?\/\//gi;
    let withoutUrls = "";
    let end = 0;
    for (let url; (url = urls.exec(text)) !== null;) {
      const opening = text[url.index - 1];
      const closing = opening === "(" ? ")" : opening === "[" ? "]" : null;
      // An enclosing Markdown delimiter ends the URL, not the following link.
      // Balanced delimiters inside the URL still belong to its path/fragment.
      let depth = 0;
      let stop = urls.lastIndex;
      for (; stop < text.length; stop++) {
        const char = text[stop];
        if (/[\s<>"`]/.test(char)) break;
        if (closing && char === opening) depth++;
        else if (char === closing && depth-- === 0) break;
      }
      urls.lastIndex = stop;
      const raw = text.slice(url.index, stop);
      const match = raw.match(/^https?:\/\/(?:www\.)?github\.com\/([a-z\d-]+)\/([a-z\d_.-]+)\/(?:issues|pull)\/([1-9]\d*)(?=$|[/?#]|[)\].,;!:*_~]+$)/i);
      if (match) add(match[1], match[2], match[3]);
      withoutUrls += `${text.slice(end, url.index)} `;
      end = urls.lastIndex;
    }
    withoutUrls += text.slice(end);
    withoutUrls = withoutUrls.replace(/(?:^|[^\p{L}\p{N}_/#])_*([a-z\d-]+)\/([a-z\d_.-]+)#([1-9]\d*)(?=_*(?:$|[^\p{L}\p{N}_]))/giu,
      (_, owner, name, number) => { add(owner, name, number); return " "; });
    for (const match of withoutUrls.matchAll(/(?:^|[^\p{L}\p{N}_/#])_*#([1-9]\d*)(?=_*(?:$|[^\p{L}\p{N}_]))/gu)) {
      add(repo.owner, repo.repo, match[1]);
    }
  }
  return [...found.entries()].sort(([a], [b]) => compareText(a, b)).map(([, value]) => value);
}

/**
 * Snapshot: {number,url,state,isPullRequest,labels,title,body,titleSourceId,
 * bodySourceId,authorId,author,createdAt,updatedAt,humanComments,humanDecisions,botComments,
 * linked,complete,errors}. Every text source has an API identity and exact text.
 * linked has the same shape with no further traversal (including PR discussion).
 * Source identities use canonical API locations, including transferred linked
 * numbers. The root number remains bound to the requested publication target.
 * Bot receipts are available for publication deduplication, never human hashes.
 * Page limits count actual calls across stability passes per endpoint/item.
 * Each item also costs two issue metadata reads; unresolved changes are retryable
 * and incomplete. REST cannot promise an atomic snapshot across these endpoints.
 * Publication uses recheckTarget for a second bounded target-only pass after
 * linked reads, so dependency latency cannot bypass the target freshness guard.
 */
async function readIssueSnapshot(github, { repo, number, limits: overrides, recheckTarget = false }) {
  if (!Number.isSafeInteger(number) || number < 1) throw new Error("Invalid issue number");
  const limits = readLimits(overrides);
  const snapshot = await readText(github, repo, number, limits);
  const targetFingerprint = recheckTarget ? fingerprintHumanInput(snapshot) : null;
  const links = references(snapshot);
  if (links.length > limits.linkedItems) {
    snapshot.errors.push({ stage: "linked", number, code: "linked-item-bound", bound: limits.linkedItems });
  }
  const seen = new Map([[snapshot.bodySourceId, snapshot]]);
  for (const link of links.slice(0, limits.linkedItems)) {
    if (seen.has(`${link.owner}/${link.repo}#${link.number}:body`.toLowerCase())) continue;
    try {
      const linked = await readText(github, { owner: link.owner, repo: link.repo }, link.number, limits, true);
      snapshot.errors.push(...linked.errors.map((error) => ({ ...error, repository: `${link.owner}/${link.repo}` })));
      const previous = seen.get(linked.bodySourceId);
      if (!previous) {
        snapshot.linked.push(linked);
        seen.set(linked.bodySourceId, linked);
      } else if (fingerprintHumanInput({ ...previous, linked: [] }) !== fingerprintHumanInput(linked)) {
        snapshot.errors.push({ stage: "linked", number: linked.number, code: "issue-changed", retryable: true });
      }
    } catch (error) {
      snapshot.errors.push(apiError(error, { stage: "linked", number: link.number, repository: `${link.owner}/${link.repo}` }));
    }
  }
  if (recheckTarget && links.length > 0) {
    const current = await readText(github, repo, number, limits);
    snapshot.errors.push(...current.errors);
    if (fingerprintHumanInput(current) !== targetFingerprint
      || JSON.stringify([...snapshot.labels].sort()) !== JSON.stringify([...current.labels].sort())
      || snapshot.updatedAt !== current.updatedAt || snapshot.createdAt !== current.createdAt) {
      snapshot.errors.push({ stage: "issue", number, code: "issue-changed", retryable: true });
    }
    snapshot.botComments = current.botComments;
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
      // Validate before eligibility filtering: malformed entries are failed
      // coverage, not evidence that an issue lacks Needs-Triage.
      if (!Array.isArray(response.data) || !response.data.every((issue) =>
        issue && !Array.isArray(issue) && Number.isSafeInteger(issue.number) && issue.number > 0
        && ["open", "closed"].includes(issue.state) && validLabels(issue.labels) && typeof issue.updated_at === "string"
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
 * stateDelta {scan,pending,issues}. Issues retains all prior records plus trusted
 * readAttempt stamps, including unsuccessful attempts. Pending is deduplicated
 * and retains selected work. A publisher must preserve stamps when merging
 * completed records so queue removal/requeue cannot reset read age, and atomically
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
  const lastAttempt = (entry) => memory.issues[entry.number]?.readAttempt?.at ?? entry.lastAttemptAt ?? "";
  const oldest = (a, b) => compareText(lastAttempt(a), lastAttempt(b))
    || compareText(a.firstSeenAt, b.firstSeenAt) || a.number - b.number;
  const previouslyPending = new Set(memory.pending.map((entry) => entry.number));
  const outstanding = (entry) => !isFinishedRecord(memory.issues[entry.number], memory.policyVersion)
    || previouslyPending.has(entry.number)
    || entry.updatedAt !== memory.issues[entry.number]?.readAttempt?.updatedAt;
  const historical = queue.filter((entry) => entry.historical).sort(oldest)[0];
  queue.sort((a, b) => Number(b === historical) - Number(a === historical)
    || Number(b.number === hint) - Number(a.number === hint)
    || Number(outstanding(b)) - Number(outstanding(a))
    || compareText(b.updatedAt ?? "", a.updatedAt ?? "") || oldest(a, b));
  const issues = { ...memory.issues };
  const discovered = [];
  const incomplete = [];
  const errors = [...incremental.errors, ...sweep.errors];
  for (const entry of queue.slice(0, limits.snapshotReads)) {
    pending.set(entry.number, { ...entry, lastAttemptAt: now });
    const prior = memory.issues[entry.number];
    issues[entry.number] = { ...prior, readAttempt: { ...prior?.readAttempt, at: now } };
    try {
      const snapshot = await readIssueSnapshot(github, { repo, number: entry.number, limits });
      if (snapshot.complete) issues[entry.number].readAttempt.updatedAt = snapshot.updatedAt;
      if (!snapshot.complete) {
        incomplete.push({ number: entry.number, snapshot });
        errors.push(...snapshot.errors);
      } else if (!isEligibleIssue(snapshot)) {
        pending.delete(entry.number);
      } else if (!needsAnalysis(memory.issues[entry.number], snapshot, memory.policyVersion)) {
        pending.delete(entry.number);
      } else {
        discovered.push({ ...entry, lastAttemptAt: lastAttempt(entry), snapshot });
      }
    } catch (error) {
      incomplete.push({ number: entry.number });
      errors.push(apiError(error, { stage: "snapshot", number: entry.number }));
    }
  }
  const contentBound = (number) => {
    incomplete.push({ number });
    errors.push({ stage: "model-input", number, code: "content-bound", retryable: true });
  };
  // Reject oversized evidence before reserving the oldest admissible report.
  const admissible = discovered.filter((entry) => {
    const { snapshot } = entry;
    entry.candidate = {
      number: snapshot.number, snapshot, fingerprint: fingerprintHumanInput(snapshot),
      priorRecord: memory.issues[snapshot.number] ?? null,
    };
    entry.bytes = Buffer.byteLength(JSON.stringify(entry.candidate));
    if (entry.bytes <= Math.min(limits.modelEntryBytes, limits.modelInputBytes)) return true;
    contentBound(snapshot.number);
    return false;
  });
  const selected = [];
  let bytes = 0;
  // Order every already-read candidate so a batch-size rejection can be refilled.
  for (const entry of selectCandidates({ event, discovered: admissible, memory, limit: limits.snapshotReads, now })) {
    if (selected.length === limits.candidates) break;
    if (bytes + entry.bytes > limits.modelInputBytes) {
      contentBound(entry.snapshot.number);
      continue;
    }
    bytes += entry.bytes;
    selected.push(entry.candidate);
    pending.set(entry.snapshot.number, { ...pending.get(entry.snapshot.number), lastSelectedAt: now });
  }
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
      issues,
    },
  };
}

module.exports = {
  MEMORY_BRANCH, MEMORY_PATH, readMemory, collectCandidates, readIssueSnapshot,
};
