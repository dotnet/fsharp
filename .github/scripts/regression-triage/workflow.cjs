"use strict";

const fs = require("node:fs");
const path = require("node:path");
const { createHash } = require("node:crypto");
const { POLICY_VERSION } = require("./core.cjs");
const { collectCandidates } = require("./github.cjs");
const { createGitHubStore, validateProposals, publishBatch } = require("./publish.cjs");

const repo = Object.freeze({ owner: "dotnet", repo: "fsharp" });
const bot = Object.freeze({ id: 41898282, login: "github-actions[bot]" });
const digest = (text) => createHash("sha256").update(text).digest("hex");
const positive = (number) => Number.isSafeInteger(number) && number > 0;
const isBot = (user) => user?.type === "Bot" || /\[bot\]$/i.test(user?.login ?? "");
const requireThat = (condition, message) => { if (!condition) throw new Error(message); };

function artifactPrefix(env) {
  requireThat(env.GITHUB_REPOSITORY === "dotnet/fsharp" && /^[1-9]\d{0,19}$/.test(env.GITHUB_RUN_ID)
    && /^[1-9]\d*$/.test(env.GITHUB_RUN_ATTEMPT) && positive(Number(env.GITHUB_RUN_ATTEMPT))
    && /^[a-f0-9]{40}$/.test(env.GITHUB_WORKFLOW_SHA), "Invalid trusted run identity");
  return `regression-triage-${env.GITHUB_REPOSITORY.replace("/", "-")}-${env.GITHUB_RUN_ID}-${env.GITHUB_RUN_ATTEMPT}-${POLICY_VERSION}-${env.GITHUB_WORKFLOW_SHA}-`;
}

function eventOptions(env, event) {
  artifactPrefix(env);
  const branch = event.repository?.default_branch;
  requireThat(event.repository?.full_name === env.GITHUB_REPOSITORY && typeof branch === "string" && branch.length > 0
    && env.GITHUB_REF === `refs/heads/${branch}` && env.GITHUB_SHA === env.GITHUB_WORKFLOW_SHA
    && env.GITHUB_WORKFLOW_REF === `dotnet/fsharp/.github/workflows/regression-triage.lock.yml@refs/heads/${branch}`,
  "Only the immutable default-branch workflow revision is trusted");
  let hint = null;
  let staged = env.GH_AW_SAFE_OUTPUTS_STAGED === "true";
  if (env.GITHUB_EVENT_NAME === "workflow_dispatch") {
    const inputs = event.inputs ?? {};
    requireThat(Object.keys(inputs).every((key) => ["issue", "staged", "aw_context"].includes(key))
      && (inputs.aw_context === undefined || inputs.aw_context === ""), "Unsupported dispatch input");
    if (inputs.issue !== undefined && inputs.issue !== "") {
      requireThat(typeof inputs.issue === "string" && /^[1-9]\d*$/.test(inputs.issue)
        && positive(Number(inputs.issue)), "Issue hint must be a positive safe integer");
      hint = Number(inputs.issue);
    }
    requireThat(inputs.staged === undefined || [true, false, "true", "false"].includes(inputs.staged),
      "Staged input must be boolean");
    staged ||= inputs.staged === undefined || inputs.staged === true || inputs.staged === "true";
  } else if (env.GITHUB_EVENT_NAME !== "schedule") {
    const actions = env.GITHUB_EVENT_NAME === "issues" ? ["opened", "edited", "reopened", "labeled", "transferred"]
      : env.GITHUB_EVENT_NAME === "issue_comment" ? ["created", "edited", "deleted"] : [];
    if (!actions.includes(event.action) || !event.issue || Object.hasOwn(event, "pull_request")
      || Object.hasOwn(event.issue, "pull_request") || event.issue.isPullRequest
      || isBot(event.sender) || env.GITHUB_EVENT_NAME === "issue_comment" && isBot(event.comment?.user)
      || event.action === "labeled" && event.label?.name !== "Needs-Triage") return { active: false, hint, staged };
    requireThat(positive(event.issue.number), "Invalid event issue number");
    hint = event.issue.number;
  }
  return { active: true, hint, staged };
}

function binding(env, memoryHead) {
  artifactPrefix(env);
  return { repository: env.GITHUB_REPOSITORY, runId: env.GITHUB_RUN_ID,
    runAttempt: Number(env.GITHUB_RUN_ATTEMPT), policyVersion: POLICY_VERSION,
    collectorRevision: env.GITHUB_WORKFLOW_SHA, memoryHead };
}

function status(manifest) {
  const codes = [...new Set(manifest.errors.map((error) => error.code))];
  return `Selected ${manifest.selected.length}; incomplete ${manifest.incomplete.length}; `
    + `incremental complete=${manifest.scan.incremental.complete}; sweep complete=${manifest.scan.sweep.complete}`
    + (codes.length ? `; ${codes.join(", ")}` : "");
}

async function collectWorkflow({ github, store = createGitHubStore(github, repo), env, event, now }) {
  const options = eventOptions(env, event);
  if (!options.active) return options;
  const memory = await store.read();
  const manifest = await collectCandidates(github, {
    repo, event: options.hint === null ? {} : { number: options.hint }, memory: memory.state, now,
  });
  manifest.binding = binding(env, memory.headOid);
  manifest.incomplete = manifest.incomplete.map(({ number }) => ({ number }));
  // Never give the model a silently shortened discussion. Leave oversized work
  // pending, just like a failed bounded API read.
  let bytes = 0;
  manifest.selected = manifest.selected.filter((entry) => {
    const size = Buffer.byteLength(JSON.stringify(entry));
    if (size <= 49152 && bytes + size <= 196608) { bytes += size; return true; }
    manifest.incomplete.push({ number: entry.number });
    manifest.errors.push({ stage: "model-input", number: entry.number, code: "content-bound", retryable: true });
    return false;
  });
  const manifestText = JSON.stringify(manifest);
  requireThat(Buffer.byteLength(manifestText) <= 4194304, "Trusted manifest exceeds 4 MiB; no progress saved");
  const selected = manifest.selected.map(({ number, fingerprint, snapshot, priorRecord }) => ({
    number, fingerprint, snapshot,
    priorRecord: priorRecord && {
      classification: priorRecord.classification, clarification: priorRecord.clarification,
      humanCorrection: priorRecord.humanCorrection, humanLabelDecision: priorRecord.humanLabelDecision,
    },
  }));
  return { ...options, manifest, manifestText,
    artifactName: artifactPrefix(env) + digest(manifestText),
    viewName: artifactPrefix(env) + "input",
    viewText: JSON.stringify({ schemaVersion: 1, policyVersion: POLICY_VERSION, selected }),
    summary: status(manifest) };
}

function verifyArtifact(manifestText, artifactName, env) {
  const prefix = artifactPrefix(env);
  requireThat(typeof manifestText === "string" && Buffer.byteLength(manifestText) <= 4194304
    && artifactName === prefix + digest(manifestText), "Absent or tampered collector artifact");
  const manifest = JSON.parse(manifestText);
  const expected = binding(env, manifest.binding?.memoryHead);
  requireThat(JSON.stringify(manifest.binding) === JSON.stringify(expected), "Collector artifact run/revision mismatch");
  return manifest;
}

async function publishWorkflow({ github, store = createGitHubStore(github, repo), env, event, now,
  manifestText, artifactName, output }) {
  const options = eventOptions(env, event);
  requireThat(options.active, "Inactive event cannot publish");
  const manifest = verifyArtifact(manifestText, artifactName, env);
  const results = validateProposals(output, manifest);
  requireThat(results.length === manifest.selected.length, "Incomplete proposal batch");
  requireThat(results.every((result) => result.evidence.some((citation) =>
    citation.sourceId.startsWith(`${env.GITHUB_REPOSITORY}#${result.number}:`))), "Missing selected-report citation");
  const result = await publishBatch({ github, store, repo, manifest, output,
    context: binding(env, manifest.binding.memoryHead), bot, now, staged: options.staged, env });
  return { ...result, incomplete: manifest.errors.length > 0 || manifest.incomplete.length > 0
    || !manifest.scan.incremental.complete || !manifest.scan.sweep.complete
    || result.outcomes.some((outcome) => !["published", "noop"].includes(outcome.status)),
  summary: status(manifest) };
}

const readEvent = () => JSON.parse(fs.readFileSync(process.env.GITHUB_EVENT_PATH, "utf8"));
const directory = (name) => path.join(process.env.RUNNER_TEMP, name);

async function collectAction({ github, core }) {
  const result = await collectWorkflow({ github, env: process.env, event: readEvent(), now: new Date().toISOString() });
  core.setOutput("active", String(result.active));
  if (!result.active) return;
  for (const [name, file, text] of [
    ["regression-triage-manifest", "manifest.json", result.manifestText],
    ["regression-triage-input", "input.json", result.viewText],
  ]) {
    fs.mkdirSync(directory(name), { recursive: true });
    fs.writeFileSync(path.join(directory(name), file), text);
  }
  core.setOutput("manifest-name", result.artifactName);
  core.setOutput("view-name", result.viewName);
  await core.summary.addRaw(result.summary).write();
  if (result.manifest.errors.length) core.warning(result.summary);
}

async function resolveArtifact({ github, core }) {
  eventOptions(process.env, readEvent());
  const prefix = artifactPrefix(process.env);
  // A run has a small fixed number of framework artifacts. Fail closed rather
  // than search unbounded history or fall back to an agent-uploaded file.
  const response = await github.rest.actions.listWorkflowRunArtifacts({
    ...repo, run_id: process.env.GITHUB_RUN_ID, per_page: 100,
  });
  requireThat(response.data.total_count <= 100, "Too many run artifacts");
  const matches = response.data.artifacts.filter((item) => item.name.startsWith(prefix)
    && /^[a-f0-9]{64}$/.test(item.name.slice(prefix.length)));
  requireThat(matches.length === 1, "Exactly one immutable collector artifact is required");
  const [artifact] = matches;
  requireThat(positive(artifact.id) && !artifact.expired
    && String(artifact.workflow_run?.id) === process.env.GITHUB_RUN_ID
    && artifact.workflow_run?.head_sha === process.env.GITHUB_WORKFLOW_SHA,
  "Collector artifact metadata mismatch");
  core.setOutput("artifact-id", String(artifact.id));
  core.setOutput("artifact-name", artifact.name);
}

async function publishAction({ github, core }) {
  const result = await publishWorkflow({ github, env: process.env, event: readEvent(), now: new Date().toISOString(),
    manifestText: fs.readFileSync(path.join(directory("regression-triage-trusted"), "manifest.json"), "utf8"),
    artifactName: process.env.TRIAGE_ARTIFACT_NAME,
    output: fs.readFileSync(process.env.GH_AW_AGENT_OUTPUT, "utf8") });
  await core.summary.addRaw(result.summary).addCodeBlock(JSON.stringify({
    outcomes: result.outcomes, receipts: result.receipts,
  }, null, 2), "json").write();
  if (result.incomplete) core.setFailed("Incomplete regression triage; pending work retained for reconciliation");
}

// v0.76.1 drops custom step timeout-minutes; enforce deadlines in the trusted
// process. Abrupt publication termination leaves its CAS intent for recovery.
function boundedAction(action, minutes) {
  return async (args) => {
    const timer = setTimeout(() => {
      args.core.setFailed(`Regression triage exceeded its ${minutes}-minute deadline; retry from durable memory`);
      process.exit(1);
    }, minutes * 60000);
    try { return await action(args); } finally { clearTimeout(timer); }
  };
}

module.exports = { eventOptions, artifactPrefix, collectWorkflow, verifyArtifact, publishWorkflow,
  collectAction: boundedAction(collectAction, 10), resolveArtifact: boundedAction(resolveArtifact, 2),
  publishAction: boundedAction(publishAction, 15) };
