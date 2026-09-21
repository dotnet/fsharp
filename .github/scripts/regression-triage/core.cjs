"use strict";

const { createHash } = require("node:crypto");

const POLICY_VERSION = "reported-regression-v1";
const FINGERPRINT_VERSION = 2;
const OVERLAP_MS = 15 * 60 * 1000;
const LIMITS = Object.freeze({
  candidates: 5, issuePages: 10, snapshotReads: 10,
  commentPages: 10, timelinePages: 10, linkedItems: 5, reviewPages: 5,
  modelEntryBytes: 49152, modelInputBytes: 196608, labelRejections: 3,
});
const QUESTIONS = Object.freeze({
  "known-good": "Which earlier version worked with the same source and comparable settings?",
  "affected-component": "Which component changed: the compiler, FSharp.Core, SDK, or runtime?",
  "comparable-configuration": "Were the source, target framework, and build settings the same in the working and failing cases?",
  "producer-consumer": "Which producer and consumer compiler versions worked, and which combination fails?",
});

const object = (value) => value !== null && typeof value === "object" && !Array.isArray(value);
const issueNumber = (value) => Number.isSafeInteger(value) && value > 0;
const timestamp = (value) => typeof value === "string" && Number.isFinite(Date.parse(value));
const compareText = (a, b) => a < b ? -1 : a > b ? 1 : 0;

function validPublication(value) {
  return value == null || object(value)
    && typeof value.operationId === "string" && value.operationId.trim().length > 0 && value.operationId.length <= 64
    && (value.phase === undefined || ["prepared", "sending", "rejected"].includes(value.phase))
    && (value.effect === undefined || ["label", "comment"].includes(value.effect))
    && (value.phase !== "sending" || value.effect !== undefined)
    && (value.phase !== "prepared" || value.effect === undefined)
    && (value.rejections === undefined || issueNumber(value.rejections) && value.rejections <= LIMITS.labelRejections)
    && (value.retryAt === undefined || value.phase === "rejected" && timestamp(value.retryAt))
    && (value.phase !== "rejected" || value.effect === "label" && issueNumber(value.rejections) && timestamp(value.retryAt));
}

function isEligibleIssue(issue) {
  return object(issue) && issueNumber(issue.number) && issue.state === "open"
    && !Object.hasOwn(issue, "pull_request") && !issue.isPullRequest
    && Array.isArray(issue.labels)
    && issue.labels.some((label) => (typeof label === "string" ? label : label?.name) === "Needs-Triage");
}

function eventNumber(event) {
  if ((object(event) && Object.hasOwn(event, "pull_request"))
    || (object(event?.issue) && Object.hasOwn(event.issue, "pull_request"))
    || event?.issue?.isPullRequest) return null;
  const number = event?.issue?.number ?? event?.number;
  return issueNumber(number) ? number : null;
}

// Schema 1: {policyVersion, scan:{updatedThrough,incremental,sweep}, pending,
// issues:{[number]:record}}. Queue entries have number/firstSeenAt and optional
// historical/updatedAt/lastAttemptAt/lastSelectedAt. Records retain issueId, fingerprint, policyVersion,
// classification, evidence, missingFact, lastResult, clarification, humanCorrection,
// humanLabelDecision, pendingPublication and pendingLabelPublication (an older
// label attempt awaiting observation). Only published/noop are terminal.
// readAttempt:{at,updatedAt?} survives queue removal; updatedAt is the last
// complete snapshot's parent timestamp, never proof of unchanged linked input.
// null means confirmed absence, not a failed read. Migration changes the top-level
// policy only: old record policies, human decisions, receipts and intent survive.
function normalizeMemory(raw, { policyVersion = POLICY_VERSION } = {}) {
  if (typeof policyVersion !== "string" || !policyVersion) throw new Error("Invalid policyVersion");
  if (raw === null) {
    return {
      schemaVersion: 1, policyVersion,
      scan: { updatedThrough: null, incremental: null, sweep: null }, pending: [], issues: {},
    };
  }
  const state = typeof raw === "string" ? JSON.parse(raw) : structuredClone(raw);
  if (!object(state) || state.schemaVersion !== 1) throw new Error("Unsupported memory schema");
  if (typeof state.policyVersion !== "string" || !state.policyVersion
    || !object(state.scan) || !object(state.issues) || !Array.isArray(state.pending)) {
    throw new Error("Malformed memory");
  }
  if ((state.clarificationHistoryUnknown !== undefined && typeof state.clarificationHistoryUnknown !== "boolean")
    || (state.clarificationHistoryUnknownThrough !== undefined && !timestamp(state.clarificationHistoryUnknownThrough))) {
    throw new Error("Invalid clarification history boundary");
  }
  const scan = { updatedThrough: null, incremental: null, sweep: null, ...state.scan };
  if (scan.updatedThrough !== null && !timestamp(scan.updatedThrough)) throw new Error("Invalid scan timestamp");
  for (const key of ["incremental", "sweep"]) {
    const cursor = scan[key];
    if (cursor === null) continue;
    if (!object(cursor) || !Number.isSafeInteger(cursor.page) || cursor.page < 0
      || !timestamp(cursor.startedAt) || (cursor.since !== null && !timestamp(cursor.since))
      || !Array.isArray(cursor.boundary)
      || !cursor.boundary.every((entry) => Array.isArray(entry) && issueNumber(entry[0]) && timestamp(entry[1]))) {
      throw new Error(`Invalid ${key} continuation`);
    }
  }
  for (const [number, record] of Object.entries(state.issues)) {
    if (!/^[1-9]\d*$/.test(number) || !issueNumber(Number(number)) || !object(record)
      || (record.issueId !== undefined && !issueNumber(record.issueId))
      || (record.fingerprint !== undefined && typeof record.fingerprint !== "string")
      || (record.policyVersion !== undefined && typeof record.policyVersion !== "string")
      || (record.classification !== undefined && !["regression", "not-regression", "uncertain"].includes(record.classification))
      || (record.evidence !== undefined && !Array.isArray(record.evidence))
      || ["clarification", "humanCorrection", "humanLabelDecision", "pendingPublication"]
        .some((key) => record[key] != null && !object(record[key]))
      || (record.lastResult !== undefined && !object(record.lastResult))
      || (record.readAttempt !== undefined && (!object(record.readAttempt) || !timestamp(record.readAttempt.at)
        || (record.readAttempt.updatedAt !== undefined && !timestamp(record.readAttempt.updatedAt))))) {
      throw new Error(`Invalid issue record: ${number}`);
    }
    const clarification = record.clarification;
    if (!validPublication(record.pendingPublication)
      || !validPublication(record.pendingLabelPublication)
      || (record.pendingLabelPublication != null && record.pendingLabelPublication.effect !== "label")
      || clarification != null && (
        !["pending", "published", "unknown"].includes(clarification.status)
        || (clarification.selector !== undefined
          && (typeof clarification.selector !== "string" || !Object.hasOwn(QUESTIONS, clarification.selector)))
        || (clarification.staged !== undefined && clarification.staged !== true)
        || (clarification.commentId !== undefined && !issueNumber(clarification.commentId))
        || (clarification.url !== undefined
          && clarification.url !== `https://github.com/dotnet/fsharp/issues/${number}#issuecomment-${clarification.commentId}`)
        || (clarification.reason !== undefined && clarification.reason !== "memory-absent")
        || (clarification.status === "published" && !issueNumber(clarification.commentId) && clarification.staged !== true)
        || (clarification.status === "pending" && clarification.selector === undefined)
        || (clarification.status === "unknown" && clarification.selector === undefined && clarification.reason !== "memory-absent")
        || !validPublication(clarification.pendingPublication)
        || (clarification.pendingPublication != null && clarification.pendingPublication.effect !== "comment"))) {
      throw new Error(`Invalid publication issue record: ${number}`);
    }
    // Older publishers terminally suppressed questions when the ledger was lost.
    if (state.clarificationHistoryUnknown === true && record.classification === "uncertain"
      && clarification?.reason === "memory-absent" && record.lastResult?.status === "noop") {
      record.lastResult.status = "unknown";
    }
  }
  const pending = new Map();
  for (const entry of state.pending) {
    if (!object(entry) || !issueNumber(entry.number) || !timestamp(entry.firstSeenAt)
      || (entry.lastAttemptAt !== undefined && !timestamp(entry.lastAttemptAt))
      || (entry.lastSelectedAt !== undefined && !timestamp(entry.lastSelectedAt))
      || (entry.updatedAt !== undefined && !timestamp(entry.updatedAt))) {
      throw new Error("Invalid pending work");
    }
    if (!pending.has(entry.number)) pending.set(entry.number, entry);
    else throw new Error(`Duplicate pending issue: ${entry.number}`);
  }
  return { ...state, policyVersion, scan, pending: [...pending.values()] };
}

const byTimeAndId = (a, b) => compareText(a.createdAt ?? "", b.createdAt ?? "")
  || (a.id ?? 0) - (b.id ?? 0) || compareText(a.sourceId ?? "", b.sourceId ?? "");

function humanInput(snapshot) {
  return {
    number: snapshot.number, issueId: snapshot.issueId ?? null, url: snapshot.url, state: snapshot.state,
    author: [snapshot.authorId ?? null, snapshot.authorType ?? null, snapshot.isBot ?? null],
    title: snapshot.title ?? "", body: snapshot.body ?? "",
    comments: [...snapshot.humanComments].sort(byTimeAndId).map((comment) => [
      comment.sourceId ?? null, comment.id, comment.authorId ?? null,
      comment.createdAt ?? null, comment.updatedAt ?? null, comment.body ?? "",
    ]),
    decisions: [...snapshot.humanDecisions].sort(byTimeAndId).map((decision) => [
      decision.sourceId ?? null, decision.id, decision.actorId ?? null,
      decision.event, decision.label ?? null, decision.createdAt ?? null,
    ]),
    linked: [...snapshot.linked].sort((a, b) => compareText(a.url, b.url)).map(humanInput),
  };
}

// Only serialization/order is normalized. Text (including whitespace and hostile
// instructions) stays verbatim. Partial snapshots must never be classified.
function fingerprintHumanInput(snapshot) {
  return createHash("sha256").update(JSON.stringify({
    version: FINGERPRINT_VERSION, input: humanInput(snapshot),
  })).digest("hex");
}

function isFinishedRecord(record, policyVersion = POLICY_VERSION) {
  return Boolean(record)
    && typeof record.fingerprint === "string" && /^[a-f0-9]{64}$/.test(record.fingerprint)
    && ["regression", "uncertain", "not-regression"].includes(record.classification)
    && record.policyVersion === policyVersion
    && ["published", "noop"].includes(record.lastResult?.status)
    && record.pendingPublication == null;
}

function needsAnalysis(record, snapshot, policyVersion = POLICY_VERSION) {
  return !snapshot.complete || !isFinishedRecord(record, policyVersion)
    || record.fingerprint !== fingerprintHumanInput(snapshot);
}

// discovered entries are {snapshot, historical, firstSeenAt, lastAttemptAt, lastSelectedAt}.
// Return at most limit complete, eligible, changed entries. Reserve the oldest
// waiting slot, rotating after selection even when publication stays unresolved.
// Reading alone must not reset analysis priority. Other slots favor recent input.
function selectCandidates({ event, discovered, memory, limit = LIMITS.candidates, now }) {
  if (!Number.isSafeInteger(limit) || limit < 1) throw new Error("Invalid candidate limit");
  const unique = new Map();
  for (const entry of discovered) {
    const snapshot = entry.snapshot;
    if (isEligibleIssue(snapshot) && snapshot.complete
      && needsAnalysis(memory.issues[snapshot.number], snapshot, memory.policyVersion)) {
      unique.set(snapshot.number, entry);
    }
  }
  const entries = [...unique.values()];
  const historical = entries.filter((entry) => entry.historical
    || !isFinishedRecord(memory.issues[entry.snapshot.number], memory.policyVersion));
  historical.sort((a, b) =>
    compareText(a.lastSelectedAt ?? a.firstSeenAt ?? now, b.lastSelectedAt ?? b.firstSeenAt ?? now)
    || compareText(a.lastAttemptAt ?? "", b.lastAttemptAt ?? "")
    || a.snapshot.number - b.snapshot.number);
  const selected = historical.slice(0, 1);
  const hint = eventNumber(event);
  entries.sort((a, b) =>
    Number(b.snapshot.number === hint) - Number(a.snapshot.number === hint)
    || compareText(b.snapshot.updatedAt ?? "", a.snapshot.updatedAt ?? "")
    || a.snapshot.number - b.snapshot.number);
  for (const entry of entries) {
    if (selected.length === limit) break;
    if (entry !== selected[0]) selected.push(entry);
  }
  return selected;
}

module.exports = {
  POLICY_VERSION, FINGERPRINT_VERSION, OVERLAP_MS, LIMITS, QUESTIONS,
  isEligibleIssue, eventNumber, normalizeMemory, fingerprintHumanInput,
  isFinishedRecord, needsAnalysis, selectCandidates,
};
