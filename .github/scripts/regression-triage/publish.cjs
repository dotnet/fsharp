"use strict";

const { createHash } = require("node:crypto");
const { isDeepStrictEqual } = require("node:util");
const {
  POLICY_VERSION, LIMITS, QUESTIONS, normalizeMemory, fingerprintHumanInput, isEligibleIssue, isFinishedRecord,
} = require("./core.cjs");
const { MEMORY_BRANCH, MEMORY_PATH, readMemory, readIssueSnapshot } = require("./github.cjs");

const OUTPUT_TYPE = "publish_regression_triage";
const ACKNOWLEDGEMENT = "Proposal received for validation; publication is not confirmed.";
const DIMENSIONS = Object.freeze([
  "compiler", "sdk", "fsharpCore", "runtime", "targetFramework", "configuration", "producer", "consumer",
]);
const object = (value) => value !== null && typeof value === "object" && !Array.isArray(value);
const positive = (value) => Number.isSafeInteger(value) && value > 0;
const oid = (value) => typeof value === "string" && /^[a-f0-9]{40}$/.test(value);
const hash = (value) => createHash("sha256").update(JSON.stringify(value)).digest("hex");
const text = (value, max) => typeof value === "string" && value.trim().length > 0 && value.length <= max;
const conflict = () => Object.assign(new Error("Memory head changed; recollect before retrying"), {
  code: "CAS_CONFLICT", retryable: true,
});

function requireThat(condition, message) {
  if (!condition) throw new Error(message);
}

function keys(value, required, optional = []) {
  requireThat(object(value) && required.every((key) => Object.hasOwn(value, key))
    && Object.keys(value).every((key) => required.includes(key) || optional.includes(key)), "Invalid or unknown fields");
}

function parseJson(value, maxBytes) {
  requireThat(typeof value === "string" && Buffer.byteLength(value) <= maxBytes, "Invalid JSON size/type");
  const parsed = JSON.parse(value);
  // JSON.parse otherwise silently accepts duplicate keys. Tokenize only after
  // syntax validation; quoted braces/commas cannot alter the container stack.
  const stack = [];
  for (const [token] of value.matchAll(/"(?:\\.|[^"\\])*"|[{}\[\]:,]/g)) {
    const top = stack.at(-1);
    if (token === "{" || token === "[") {
      requireThat(stack.length < 16, "JSON nesting limit");
      stack.push({ keys: token === "{" ? new Set() : null, key: true });
    } else if (token === "}" || token === "]") stack.pop();
    else if (token === ",") { if (top) top.key = true; }
    else if (token === ":") top.key = false;
    else if (top?.keys && top.key) {
      const name = JSON.parse(token);
      requireThat(!top.keys.has(name), "Duplicate JSON field");
      top.keys.add(name);
      top.key = false;
    }
  }
  return parsed;
}

function sources(snapshot) {
  const found = new Map();
  for (const item of [snapshot, ...snapshot.linked]) {
    requireThat(item.complete === true, "Incomplete evidence snapshot");
    const prefix = item.bodySourceId?.match(/^([a-z\d-]+\/[a-z\d_.-]+)#([1-9]\d*):body$/);
    requireThat(prefix && Number(prefix[2]) === item.number, "Invalid source identity");
    const url = item.url;
    const location = url?.match(/^https:\/\/github\.com\/([a-z\d-]+\/[a-z\d_.-]+)\/(issues|pull)\/([1-9]\d*)$/i);
    requireThat(location && location[1].toLowerCase() === prefix[1]
      && location[2] === (item.isPullRequest ? "pull" : "issues") && Number(location[3]) === item.number
      && item.titleSourceId === `${prefix[1]}#${item.number}:title`, "Invalid canonical source URL");
    const add = (sourceId, source) => {
      requireThat(!found.has(sourceId), "Duplicate source identity");
      found.set(sourceId, source);
    };
    add(item.titleSourceId, { url, body: item.title, createdAt: item.updatedAt });
    add(item.bodySourceId, { url, body: item.body, createdAt: item.updatedAt });
    for (const comment of item.humanComments) {
      const match = comment.sourceId?.match(/:(comment|review|review-comment):([1-9]\d*)$/);
      requireThat(match && positive(comment.id) && Number(match[2]) === comment.id && !comment.isBot
        && comment.sourceId === `${prefix[1]}#${item.number}:${match[1]}:${comment.id}`, "Invalid human source");
      const base = `https://github.com/${location[1]}`;
      const canonical = match[1] === "comment" ? `${url}#issuecomment-${comment.id}`
        : match[1] === "review" ? `${base}/pull/${item.number}#pullrequestreview-${comment.id}` : null;
      const reviewUrls = [`${base}/pull/${item.number}#discussion_r${comment.id}`,
        `${base}/pull/${item.number}/files#r${comment.id}`, `${base}/pull/${item.number}/files#discussion_r${comment.id}`];
      requireThat(canonical ? comment.url === canonical : reviewUrls.includes(comment.url),
        "Invalid canonical comment URL");
      add(comment.sourceId, { url: comment.url, body: comment.body, createdAt: comment.updatedAt ?? comment.createdAt });
    }
  }
  return found;
}

function validateCitation(citation, evidence, correction = false) {
  keys(citation, ["sourceId", "url", "quote"], correction ? [] : ["dimension"]);
  requireThat(text(citation.sourceId, 200) && text(citation.url, 500) && text(citation.quote, 1000), "Invalid citation bounds");
  requireThat(citation.dimension === undefined || DIMENSIONS.includes(citation.dimension), "Unsupported evidence dimension");
  const source = evidence.get(citation.sourceId);
  requireThat(source && source.url === citation.url && source.body.includes(citation.quote), "Citation does not match trusted evidence");
  return source;
}

/**
 * Only {items:[{type:OUTPUT_TYPE,proposals:JSON.stringify({
 * schemaVersion:1,policyVersion:POLICY_VERSION,results:[{
 * number,fingerprint,classification,evidence:[{sourceId,url,quote,dimension?}],
 * missingFact,clarification,correction?:{sourceId,url,quote}
 * }]})}]} is accepted. Bounds and nullable fields are described in README.md.
 * Provenance validation checks reported text, NOT the semantic truth of a claim.
 */
function validateProposals(output, manifest) {
  output = typeof output === "string" ? parseJson(output, 131072) : parseJson(JSON.stringify(output), 131072);
  keys(output, ["items"]);
  requireThat(Array.isArray(output.items) && output.items.length === 1, "Exactly one proposal envelope is required");
  const [item] = output.items;
  keys(item, ["type", "proposals"]);
  requireThat(item.type === OUTPUT_TYPE, "Unsupported output route");
  const batch = parseJson(item.proposals, 65536);
  keys(batch, ["schemaVersion", "policyVersion", "results"]);
  requireThat(batch.schemaVersion === 1 && batch.policyVersion === POLICY_VERSION
    && manifest.policyVersion === POLICY_VERSION, "Unsupported schema/policy");
  requireThat(Array.isArray(batch.results) && batch.results.length <= LIMITS.candidates, "Invalid result count");
  const selected = new Map();
  for (const entry of manifest.selected) {
    requireThat(!selected.has(entry.number) && positive(entry.number) && entry.number === entry.snapshot.number
      && isEligibleIssue(entry.snapshot) && entry.snapshot.complete
      && entry.fingerprint === fingerprintHumanInput(entry.snapshot), "Invalid selected snapshot");
    selected.set(entry.number, entry);
  }
  const seen = new Set();
  for (const result of batch.results) {
    keys(result, ["number", "fingerprint", "classification", "evidence", "missingFact", "clarification"], ["correction"]);
    const entry = selected.get(result.number);
    requireThat(positive(result.number) && entry && !seen.has(result.number), "Unselected/duplicate issue");
    seen.add(result.number);
    requireThat(/^[a-f0-9]{64}$/.test(result.fingerprint) && result.fingerprint === entry.fingerprint, "Incorrect fingerprint");
    requireThat(["regression", "not-regression", "uncertain"].includes(result.classification), "Unsupported classification");
    const uncertain = result.classification === "uncertain";
    requireThat(uncertain ? text(result.missingFact, 1000) : result.missingFact === null, "Invalid missing fact");
    requireThat(result.clarification === null || uncertain && typeof result.clarification === "string"
      && Object.hasOwn(QUESTIONS, result.clarification), "Invalid clarification selector");
    requireThat(Array.isArray(result.evidence) && result.evidence.length <= 12
      && (result.classification !== "regression" || result.evidence.length > 0), "Invalid evidence count");
    const evidence = sources(entry.snapshot);
    for (const citation of result.evidence) validateCitation(citation, evidence);
    if (result.correction !== undefined) {
      requireThat(result.classification !== "regression", "Rejecting correction cannot authorize Regression");
      validateCitation(result.correction, evidence, true);
    }
  }
  return batch.results;
}

function validateBinding(manifest, context, repo) {
  keys(context, ["repository", "runId", "runAttempt", "policyVersion", "collectorRevision", "memoryHead"]);
  requireThat(context.repository === `${repo.owner}/${repo.repo}` && context.repository === "dotnet/fsharp"
    && typeof context.runId === "string" && /^[1-9]\d{0,19}$/.test(context.runId)
    && positive(context.runAttempt) && context.policyVersion === POLICY_VERSION
    && oid(context.collectorRevision) && (context.memoryHead === null || oid(context.memoryHead))
    && isDeepStrictEqual(manifest.binding, context), "Trusted manifest binding mismatch");
  for (const entry of manifest.selected) {
    requireThat(entry.snapshot.bodySourceId === `${context.repository}#${entry.number}:body`, "Wrong selected repository");
  }
}

/** read() -> {state,headOid,missing:null|"branch"|"file"}; commit({state,expectedHeadOid}) -> same.
 * A conflict is retryable but never automatically replays a stale state object.
 */
function createGitHubStore(github, repo) {
  requireThat(repo.owner === "dotnet" && repo.repo === "fsharp", "Unsupported repository");
  const read = () => readMemory(github, repo, { versioned: true });
  return {
    read,
    async commit({ state, expectedHeadOid }) {
      requireThat(expectedHeadOid === null || oid(expectedHeadOid), "Invalid expected head");
      state = normalizeMemory(state);
      const serialized = JSON.stringify(state) + "\n";
      requireThat(Buffer.byteLength(serialized) <= 1048576, "Memory exceeds durable store bound");
      const contents = Buffer.from(serialized).toString("base64");
      if (expectedHeadOid === null) {
        if ((await read()).headOid !== null) throw conflict();
        const { data: repository } = await github.rest.repos.get(repo);
        requireThat(text(repository.default_branch, 250), "Missing trusted default branch");
        expectedHeadOid = (await github.rest.repos.getBranch({ ...repo, branch: repository.default_branch })).data.commit.sha;
        requireThat(oid(expectedHeadOid), "Invalid trusted base");
        try {
          await github.rest.git.createRef({ ...repo, ref: `refs/heads/${MEMORY_BRANCH}`, sha: expectedHeadOid });
        } catch (error) {
          if (error.status === 422 && (await read()).headOid !== null) throw conflict();
          throw error;
        }
      }
      let response;
      try {
        response = await github.graphql(`mutation($input: CreateCommitOnBranchInput!) {
          createCommitOnBranch(input: $input) { commit { oid } }
        }`, { input: {
          branch: { repositoryNameWithOwner: `${repo.owner}/${repo.repo}`, branchName: MEMORY_BRANCH },
          expectedHeadOid, message: { headline: "Persist regression triage state" },
          fileChanges: { additions: [{ path: MEMORY_PATH, contents }] },
        } });
      } catch (error) {
        // Even a timeout may have committed. Stop this publisher; the next read
        // reconciles its persisted intent instead of assuming either outcome.
        const latest = await read();
        if (latest.headOid !== expectedHeadOid) throw conflict();
        throw error;
      }
      const headOid = response?.createCommitOnBranch?.commit?.oid;
      requireThat(oid(headOid), "Memory write acknowledgement missing; reload before retry");
      return { headOid, state, missing: null };
    },
  };
}

const receiptMarker = (repo, number) => `<!-- regression-triage:clarification:${repo.owner}/${repo.repo}#${number} -->`;

function observedReceipt(snapshot, repo, bot) {
  const marker = receiptMarker(repo, snapshot.number);
  const comment = snapshot.botComments.find((item) =>
    item.authorType === "Bot" && item.authorId === bot.id && item.author === bot.login && item.body.includes(marker));
  return comment ? { status: "published", commentId: comment.id, url: comment.url } : null;
}

function humanState(record, snapshot, proposal) {
  const current = snapshot.humanDecisions.filter((item) => item.label === "Regression").at(-1);
  if (current) record.humanLabelDecision = current;
  if (proposal.correction) {
    const source = sources(snapshot).get(proposal.correction.sourceId);
    record.humanCorrection = { ...proposal.correction, createdAt: source.createdAt ?? null };
  }
  const decision = record.humanLabelDecision;
  const correction = record.humanCorrection;
  const reversed = decision?.event === "labeled" && correction?.createdAt
    && Date.parse(decision.createdAt) > Date.parse(correction.createdAt);
  return decision?.event === "unlabeled" || Boolean(correction && !reversed);
}

/**
 * The manifest and context come from trusted same-run artifact/runtime inputs,
 * NEVER the agent workspace. Only output is model-controlled. Returns
 * {state,headOid,outcomes,receipts}; retryable/stale/unknown outcomes keep work
 * pending. Store/CAS failures throw and stop effects. Callers must surface both.
 */
async function publishBatch({ github, store, repo, manifest, output, context, bot, now,
  staged = false, env = process.env, limits }) {
  validateBinding(manifest, context, repo);
  const results = validateProposals(output, manifest);
  requireThat(positive(bot?.id) && text(bot?.login, 100), "A trusted publication bot identity is required");
  requireThat(typeof staged === "boolean" && typeof now === "string" && Number.isFinite(Date.parse(now)), "Invalid trusted run options");
  staged ||= process.env.GH_AW_SAFE_OUTPUTS_STAGED === "true" || env.GH_AW_SAFE_OUTPUTS_STAGED === "true";
  const receipts = [];
  const outcomes = [];
  let version = await store.read();
  let state = normalizeMemory(version.state);
  const manifestId = hash(manifest);
  const proposalHash = hash(results);
  const resumed = state.discoveryReceipt?.manifestId === manifestId;
  if (version.headOid !== context.memoryHead && !resumed) throw conflict();
  requireThat(!resumed || state.discoveryReceipt.proposalHash === proposalHash, "Conflicting accepted proposals");
  const save = async () => {
    if (staged) {
      receipts.push({ type: "would-save-memory", branch: MEMORY_BRANCH, path: MEMORY_PATH });
      version = { headOid: version.headOid, state: structuredClone(state), missing: null };
    } else version = await store.commit({ expectedHeadOid: version.headOid, state });
  };
  if (!resumed) {
    // Exact-base application only: this delta contains the WHOLE queue and
    // read-age observations, not a patch safe to replay over another writer.
    state = normalizeMemory({ ...state, ...manifest.stateDelta });
    if (version.missing !== null || state.clarificationHistoryUnknown === true) {
      state.clarificationHistoryUnknownThrough ??= now;
    }
    delete state.clarificationHistoryUnknown;
    state.discoveryReceipt = { manifestId, proposalHash };
    for (const result of results) {
      const prior = state.issues[result.number] ?? {};
      if (isFinishedRecord(prior) && prior.fingerprint === result.fingerprint) continue;
      const operationId = hash([context.repository, result.number, POLICY_VERSION, result.fingerprint]);
      const unresolved = prior.pendingPublication && prior.pendingPublication.phase !== "prepared";
      const priorComment = unresolved && prior.pendingPublication.effect === "comment";
      const priorLabel = unresolved && prior.pendingPublication.effect === "label"
        && (prior.pendingPublication.operationId !== operationId || result.classification !== "regression");
      state.issues[result.number] = {
        ...prior, fingerprint: result.fingerprint, policyVersion: POLICY_VERSION,
        classification: result.classification, evidence: result.evidence, missingFact: result.missingFact,
        clarification: priorComment
          ? { ...prior.clarification, pendingPublication: prior.pendingPublication } : prior.clarification ?? null,
        humanCorrection: prior.humanCorrection ?? null,
        humanLabelDecision: prior.humanLabelDecision ?? null,
        pendingLabelPublication: priorLabel ? prior.pendingPublication : prior.pendingLabelPublication ?? null,
        pendingPublication: unresolved && !priorComment && !priorLabel ? prior.pendingPublication : { operationId, phase: "prepared" },
        lastResult: { status: "pending", operationId },
      };
    }
    await save();
  }
  for (const result of results) {
    const record = state.issues[result.number];
    if (isFinishedRecord(record) && record.fingerprint === result.fingerprint) {
      outcomes.push({ number: result.number, ...record.lastResult });
      continue;
    }
    requireThat(record?.pendingPublication, "Missing durable publication intent");
    const intent = record.pendingPublication;
    const operationId = intent.operationId;
    let claimedHere = false;
    let attempted = false;
    const finish = async (status, detail) => {
      record.lastResult = { status, operationId, detail };
      if (["published", "noop"].includes(status)) {
        record.pendingPublication = null;
        state.pending = state.pending.filter((item) => item.number !== result.number);
      } else if (!state.pending.some((item) => item.number === result.number)) {
        state.pending.push({ number: result.number, firstSeenAt: now });
      }
      outcomes.push({ number: result.number, ...record.lastResult });
      await save();
    };
    const recheck = async () => {
      const unsent = () => {
        if (claimedHere && !attempted) {
          if (intent.effect === "comment" && record.clarification?.status === "pending") record.clarification = null;
          intent.phase = "prepared";
          delete intent.effect;
        }
      };
      let snapshot;
      try {
        snapshot = await readIssueSnapshot(github, { repo, number: result.number, limits, recheckTarget: true });
      } catch (error) {
        unsent();
        await finish("retryable", { code: "snapshot-read-failed", status: error.status ?? null });
        return null;
      }
      if (!snapshot.complete) {
        unsent();
        await finish("retryable", { code: "snapshot-incomplete", errors: snapshot.errors.slice(0, 8)
          .map(({ stage, code, status, number }) => ({ stage, code, status, number })) });
        return null;
      }
      const receipt = observedReceipt(snapshot, repo, bot);
      if (receipt) record.clarification = receipt;
      if (snapshot.labels.includes("Regression")) record.pendingLabelPublication = null;
      humanState(record, snapshot, {});
      if (!isEligibleIssue(snapshot) || fingerprintHumanInput(snapshot) !== result.fingerprint) {
        const effect = intent.effect;
        const effectObserved = effect === "label" ? snapshot.labels.includes("Regression")
          : effect === "comment" ? record.clarification?.status === "published" : false;
        unsent();
        await finish("stale", { code: "input-changed", ...(effect ? { effect, effectObserved } : {}) });
        return null;
      }
      // Repeat citation checks against current API evidence, not only the artifact.
      const evidence = sources(snapshot);
      for (const citation of result.evidence) validateCitation(citation, evidence);
      if (result.correction) validateCitation(result.correction, evidence, true);
      return snapshot;
    };
    let snapshot = await recheck();
    if (!snapshot) continue;
    const veto = humanState(record, snapshot, result);
    let effect = null;
    if (result.classification === "regression" && !veto && !snapshot.labels.includes("Regression")) effect = "label";
    const afterHistoryBoundary = state.clarificationHistoryUnknownThrough
      && Date.parse(snapshot.createdAt) > Date.parse(state.clarificationHistoryUnknownThrough);
    if (result.clarification !== null && record.clarification?.reason === "memory-absent"
      && afterHistoryBoundary && record.clarification.pendingPublication == null) record.clarification = null;
    if (result.clarification !== null && record.clarification === null) {
      if (state.clarificationHistoryUnknownThrough && !afterHistoryBoundary) {
        record.clarification = { status: "unknown", reason: "memory-absent" };
      } else effect = "comment";
    }
    const alreadyObserved = intent.effect === "label" && snapshot.labels.includes("Regression")
      || intent.effect === "comment" && record.clarification?.status === "published";
    if (alreadyObserved) { await finish("published", { code: "effect-observed" }); continue; }
    if (intent.phase !== "prepared") {
      await finish("unknown", { code: "prior-attempt-unresolved" });
      continue;
    }
    if (result.clarification !== null && ["pending", "unknown"].includes(record.clarification?.status)) {
      await finish("unknown", { code: "prior-clarification-unresolved" });
      continue;
    }
    if (effect === null) {
      await finish("noop", { code: veto ? "human-veto" : "no-mutation-needed" });
      continue;
    }
    intent.phase = "sending";
    intent.effect = effect;
    if (effect === "comment") record.clarification = { status: "pending", selector: result.clarification };
    await save();
    claimedHere = true;
    // No ledger/model/network work between this complete recheck and mutation.
    snapshot = await recheck();
    if (!snapshot) continue;
    if (effect === "label" && (humanState(record, snapshot, result) || snapshot.labels.includes("Regression"))) {
      await finish("noop", { code: "no-mutation-needed" });
      continue;
    }
    if (effect === "comment" && record.clarification?.status === "published") {
      await finish("published", { code: "receipt-observed" });
      continue;
    }
    const body = effect === "comment"
      ? `AI-assisted triage question: ${QUESTIONS[result.clarification]}\n\n${receiptMarker(repo, result.number)}` : null;
    if (staged) {
      receipts.push(effect === "label" ? { type: "would-add-label", number: result.number, labels: ["Regression"] }
        : { type: "would-comment", number: result.number, body });
      if (effect === "comment") record.clarification = { status: "published", staged: true };
      await finish("published", { code: "staged" });
      continue;
    }
    let requestError = null;
    attempted = true;
    try {
      if (effect === "label") await github.rest.issues.addLabels({ ...repo, issue_number: result.number, labels: ["Regression"] });
      else await github.rest.issues.createComment({ ...repo, issue_number: result.number, body });
    } catch (error) {
      // Request errors are unknown outcomes, never a license to repeat a comment.
      requestError = { status: error.status ?? null, code: text(error.code, 80) ? error.code : "request-failed" };
    }
    snapshot = await recheck();
    if (!snapshot) continue;
    const observed = effect === "label" ? snapshot.labels.includes("Regression") : record.clarification?.status === "published";
    if (!observed && effect === "comment") record.clarification.status = "unknown";
    await finish(observed ? "published" : "unknown", {
      code: observed ? "effect-observed" : requestError ? "request-outcome-unknown" : "effect-not-observed",
      ...(requestError ? { requestError } : {}),
    });
  }
  return { state, headOid: version.headOid, outcomes, receipts };
}

module.exports = {
  OUTPUT_TYPE, ACKNOWLEDGEMENT, DIMENSIONS, QUESTIONS,
  validateProposals, createGitHubStore, receiptMarker, publishBatch,
};
