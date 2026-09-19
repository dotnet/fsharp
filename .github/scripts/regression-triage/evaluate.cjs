"use strict";

const assert = require("node:assert/strict");
const { createHash } = require("node:crypto");
const fs = require("node:fs");
const path = require("node:path");
const { spawnSync } = require("node:child_process");
const { POLICY_VERSION, LIMITS, normalizeMemory } = require("./core.cjs");
const { collectCandidates } = require("./github.cjs");
const { OUTPUT_TYPE, validateProposals, publishBatch } = require("./publish.cjs");
const { repo, now, report, comment, fake, emptyMemory } = require("./test-support.cjs");

const hash = (text) => createHash("sha256").update(text).digest("hex");
const oid = (n) => n.toString(16).padStart(40, "0");
const envelope = (results) => ({ items: [{ type: OUTPUT_TYPE,
  proposals: JSON.stringify({ schemaVersion: 1, policyVersion: POLICY_VERSION, results }) }] });
const effects = (result) => result.receipts.filter((r) => r.type !== "would-save-memory");

function extractPolicy(workflow) {
  const start = "<!-- regression-triage-policy:start -->";
  const end = "<!-- regression-triage-policy:end -->";
  assert.equal(workflow.split(start).length, 2, "Exactly one policy start marker required");
  assert.equal(workflow.split(end).length, 2, "Exactly one policy end marker required");
  assert.ok(workflow.indexOf(end) > workflow.indexOf(start), "Invalid policy marker order");
  const policy = workflow.slice(workflow.indexOf(start) + start.length, workflow.indexOf(end)).trim();
  assert.ok(policy, "Empty policy");
  return policy;
}

async function scenario(input) {
  const items = [input, ...input.linked];
  const api = fake({
    pageSize: 100,
    issues: items.map((s) => report(s.number, { ...s, html_url: s.url,
      ...(s.isPullRequest ? { pull_request: {} } : {}) })),
    comments: Object.fromEntries(items.map((s) => [s.number, s.humanComments.map((c) =>
      comment(c.id, { body: c.body, html_url: c.url, created_at: c.createdAt, updated_at: c.updatedAt,
        user: { id: c.authorId, login: c.author, type: "User" } }))])),
    timeline: Object.fromEntries(items.map((s) => [s.number, s.humanDecisions.map((d) => ({
      id: d.id, event: d.event, label: { name: d.label }, created_at: d.createdAt, url: d.url,
      actor: { id: d.actorId, login: d.actor, type: "User" },
    }))])),
  });
  const mutations = [];
  const forbidden = async (...args) => {
    mutations.push(args);
    throw new Error("Staged evaluation leaked a remote mutation");
  };
  api.github.rest.issues.addLabels = api.github.rest.issues.createComment = forbidden;
  api.github.rest.git = { createRef: forbidden };
  api.github.graphql = forbidden;
  let version = { state: emptyMemory(), headOid: oid(1), missing: null };
  const store = { read: async () => structuredClone(version), commit: forbidden };
  const collect = async () => {
    const manifest = await collectCandidates(api.github, { repo, now, memory: normalizeMemory(version.state) });
    assert.deepEqual(manifest.errors, []);
    assert.deepEqual(manifest.incomplete, []);
    // Linked reports are evidence, not additional fixture subjects.
    manifest.selected = manifest.selected.filter((entry) => entry.number === input.number);
    manifest.binding = { repository: "dotnet/fsharp", runId: "123", runAttempt: 1,
      policyVersion: POLICY_VERSION, collectorRevision: oid(100), memoryHead: version.headOid };
    return manifest;
  };
  const manifest = await collect();
  assert.equal(manifest.selected.length, 1);
  const args = { github: api.github, store, repo, manifest, context: manifest.binding,
    bot: { id: 99, login: "regression-triage[bot]" }, now, staged: true, env: {} };
  return { api, args, collect, mutations,
    restart(state) { version = { state: JSON.parse(JSON.stringify(state)), headOid: oid(2), missing: null }; },
  };
}

async function prepare({ workflow, corpus, model = "gpt-5.6-sol", reasoningEffort = "high", modelSource = "shared defaults" }) {
  assert.equal(corpus.schemaVersion, 1);
  assert.ok(corpus.cases.length > 0);
  assert.equal(new Set(corpus.cases.map((c) => c.input.number)).size, corpus.cases.length, "Duplicate fixture number");
  const policy = extractPolicy(workflow);
  const selected = [];
  for (const { input } of corpus.cases) selected.push((await scenario(input)).args.manifest.selected[0]);
  const requests = [];
  for (let i = 0; i < selected.length; i += LIMITS.candidates) {
    const entries = selected.slice(i, i + LIMITS.candidates);
    requests.push({ selected: entries, prompt: `${policy}\n\n`
      + "Local tool-disabled delivery: return only the JSON safe-output envelope "
      + '{"items":[{"type":"publish_regression_triage","proposals":"<JSON batch string>"}]}. '
      + "There are no tools. Apply the policy to every selected entry. Supplied input:\n"
      + JSON.stringify({ policyVersion: POLICY_VERSION, selected: entries }) });
  }
  return { metadata: { model, reasoningEffort, modelSource, policyVersion: POLICY_VERSION,
    policyHash: hash(policy), corpusHash: hash(JSON.stringify(corpus)) }, requests };
}

async function stagedChecks(input, proposal) {
  const simulation = await scenario(input);
  const output = envelope([proposal]);
  const first = await publishBatch({ ...simulation.args, output });
  assert.ok(first.receipts.some((r) => r.type === "would-save-memory"));
  assert.ok(["published", "noop"].includes(first.state.issues[input.number].lastResult.status));
  simulation.restart(first.state);
  const restartedManifest = await simulation.collect();
  assert.equal(restartedManifest.selected.length, 0, "Restart reselected unchanged completed work");
  const restarted = await publishBatch({ ...simulation.args, manifest: restartedManifest,
    context: restartedManifest.binding, output: envelope([]) });
  assert.deepEqual(effects(restarted), [], "Restart duplicated an effect");
  const staleChecks = [];
  for (const change of ["closed", "corrected", "policy"]) {
    const stale = await scenario(input);
    if (change === "closed") stale.api.issues[0].state = "closed";
    if (change === "corrected") stale.api.issues[0].body += "\nCorrection: the original comparison was invalid.";
    if (change === "policy") {
      const wrong = structuredClone(output);
      const batch = JSON.parse(wrong.items[0].proposals);
      batch.policyVersion = "obsolete-policy";
      wrong.items[0].proposals = JSON.stringify(batch);
      await assert.rejects(publishBatch({ ...stale.args, output: wrong }), /policy/);
      const old = structuredClone(first.state);
      old.issues[input.number].policyVersion = "obsolete-policy";
      stale.restart(old);
      const retryManifest = await stale.collect();
      assert.equal(retryManifest.selected.length, 1, "Old policy was not retried");
      const retry = await publishBatch({ ...stale.args, manifest: retryManifest,
        context: retryManifest.binding, output });
      assert.ok(["published", "noop"].includes(retry.state.issues[input.number].lastResult.status));
      assert.equal(effects(retry).filter((r) => r.type === "would-comment").length, 0, "Policy retry repeated clarification");
    } else {
      const rejected = await publishBatch({ ...stale.args, output });
      assert.equal(rejected.outcomes[0].status, "stale");
      assert.deepEqual(effects(rejected), []);
    }
    assert.deepEqual(stale.mutations, []);
    staleChecks.push(change);
  }
  assert.deepEqual(simulation.mutations, []);
  return { effects: effects(first), restartDeduplicated: true, staleChecks };
}

async function evaluate({ workflow, corpus, prepared, responses }) {
  const current = await prepare({ workflow, corpus, ...prepared.metadata });
  assert.deepEqual(prepared, current, "Policy/corpus hash or prepared requests changed");
  assert.equal(responses.length, prepared.requests.length, "Missing/extra response batches");
  const records = new Map();
  for (let i = 0; i < responses.length; i++) {
    const { selected } = prepared.requests[i];
    try {
      const proposals = validateProposals(responses[i], { policyVersion: POLICY_VERSION, selected });
      for (const entry of selected) records.set(entry.number, {
        proposal: proposals.find((p) => p.number === entry.number),
      });
    } catch (error) {
      for (const entry of selected) records.set(entry.number, { error: error.message });
    }
  }
  const cases = [];
  for (const { name, input, expected } of corpus.cases) {
    const { proposal, error } = records.get(input.number);
    const failures = [];
    const check = (condition, message) => { if (!condition) failures.push(message); };
    const result = { name, number: input.number, failures };
    if (error || !proposal) failures.push(error ?? "Missing proposal for selected fixture");
    else {
      check(proposal.classification === expected.classification, "Wrong classification");
      for (const citation of expected.evidence) {
        check(proposal.evidence.some((actual) => actual.sourceId === citation.sourceId
          && actual.url === citation.url && actual.quote.includes(citation.quote)
          && (!citation.dimension || actual.dimension === citation.dimension)),
        `Missing citation fact: ${citation.sourceId} (${citation.dimension ?? "comparison"})`);
      }
      for (const dimension of expected.forbiddenEvidenceDimensions ?? []) {
        check(!proposal.evidence.some((c) => c.dimension === dimension), `Invented evidence dimension: ${dimension}`);
      }
      if (expected.classification === "uncertain") {
        for (const pattern of expected.missingFactPatterns) {
          check(new RegExp(pattern, "i").test(proposal.missingFact ?? ""), `Missing fact does not address ${pattern}`);
        }
        check(proposal.clarification === null || expected.clarifications.includes(proposal.clarification),
          "Inappropriate clarification selector");
      } else check(proposal.missingFact === null && proposal.clarification === null, "Unexpected missing fact/clarification");
      try {
        Object.assign(result, await stagedChecks(input, proposal));
        const labels = result.effects.flatMap((e) => e.labels ?? []);
        check(JSON.stringify(labels) === JSON.stringify(expected.allowedEffect.addLabels), "Wrong allowed label effects");
        const comments = result.effects.filter((e) => e.type === "would-comment");
        check(comments.length === (proposal.clarification === null ? 0 : 1), "Wrong clarification effects");
        check(result.effects.every((e) => ["would-add-label", "would-comment"].includes(e.type)), "Unexpected effect");
      } catch (failure) { failures.push(failure.message); }
    }
    cases.push(result);
  }
  return { kind: "fixture-proposal-check", metadata: prepared.metadata,
    passed: cases.every((c) => c.failures.length === 0), cases };
}

function cliOptions({ model, reasoningEffort }) {
  return ["--model", model, "--reasoning-effort", reasoningEffort, "--available-tools",
    "--disable-builtin-mcps", "--no-custom-instructions", "--no-remote-export",
    "--no-ask-user", "--disallow-temp-dir", "--no-auto-update", "--no-color",
    "--stream", "off", "--silent", "--output-format", "json"];
}

function parseTranscript(raw, model) {
  const events = raw.trim().split(/\r?\n/).map((line) => JSON.parse(line));
  assert.ok(!events.some((e) => e.type?.startsWith("tool.") || e.data?.toolRequests?.length),
    "Tool activity in a tool-disabled evaluation");
  const observedModels = [...new Set(events.flatMap((e) =>
    [e.data?.model, e.type === "session.model_change" ? e.data?.newModel : null].filter(Boolean)))];
  assert.ok(observedModels.every((m) => [model, `copilot/${model}`, `openai/${model}`].includes(m)),
    "Unexpected model substitution");
  const message = events.filter((e) => e.type === "assistant.message").at(-1)?.data?.content;
  assert.equal(typeof message, "string", "Missing raw model response");
  return { message, observedModels };
}

async function main() {
  const [command, directory, cliArgument] = process.argv.slice(2);
  const cli = cliArgument && path.resolve(cliArgument);
  assert.ok(["prepare", "check", "run"].includes(command) && directory,
    "Usage: node evaluate.cjs prepare|check|run <private-artifact-directory> [copilot.exe|npm-loader.js]");
  const root = path.resolve(__dirname, "..", "..", "..");
  const artifacts = fs.realpathSync(directory);
  const relative = path.relative(root, artifacts);
  assert.ok(relative.startsWith(`..${path.sep}`) || path.isAbsolute(relative), "Artifacts must be outside the repository");
  const workflow = fs.readFileSync(path.join(root, ".github", "workflows", "regression-triage.md"), "utf8");
  const corpus = JSON.parse(fs.readFileSync(path.join(__dirname, "fixtures", "classification.json"), "utf8"));
  const saved = (name) => path.join(artifacts, name);
  const write = (name, data) => fs.writeFileSync(saved(name), JSON.stringify(data, null, 2) + "\n");
  const read = (name) => JSON.parse(fs.readFileSync(saved(name), "utf8"));
  if (command === "prepare" || command === "run") {
    const defaults = fs.readFileSync(path.join(root, ".github", "workflows", "shared", "model-defaults.md"), "utf8");
    const variable = (name) => {
      const response = spawnSync("gh", ["variable", "get", name, "--repo", "dotnet/fsharp"], { encoding: "utf8" });
      if (response.status === 0) return response.stdout.trim();
      assert.match(response.stderr ?? "", /was not found/, `Cannot establish repository variable ${name}`);
      return null;
    };
    const model = variable("GH_AW_MODEL_AGENT_COPILOT") ?? defaults.match(/vars\.GH_AW_MODEL_AGENT_COPILOT \|\| '([^']+)'/)?.[1];
    const reasoningEffort = variable("GH_AW_REASONING_EFFORT") ?? defaults.match(/vars\.GH_AW_REASONING_EFFORT \|\| '([^']+)'/)?.[1];
    assert.ok(model && reasoningEffort, "Cannot resolve shared model defaults");
    write("prepared.json", await prepare({ workflow, corpus, model, reasoningEffort,
      modelSource: "repository variables with shared/model-defaults.md fallback" }));
  }
  if (command === "prepare") return;
  const prepared = read("prepared.json");
  if (command === "run") {
    assert.ok(cli && fs.existsSync(cli), "Supply the supported Copilot executable or npm-loader.js path");
    const tests = fs.readdirSync(__dirname).filter((f) => f.endsWith(".test.cjs")).map((f) => path.join(__dirname, f));
    const suite = spawnSync(process.execPath, ["--test", ...tests], { cwd: root, encoding: "utf8", maxBuffer: 16 * 1024 * 1024 });
    fs.writeFileSync(saved("deterministic-suite.log"), (suite.stdout ?? "") + (suite.stderr ?? ""));
    assert.equal(suite.status, 0, "Complete deterministic suite must pass before model invocation");
    const home = saved("isolated-copilot-home");
    const cwd = saved("model-input-only");
    fs.mkdirSync(home);
    fs.mkdirSync(cwd);
    const auth = spawnSync("gh", ["auth", "token"], { encoding: "utf8" });
    assert.equal(auth.status, 0, "Local Copilot authentication unavailable");
    const env = { ...process.env, COPILOT_HOME: home, COPILOT_GITHUB_TOKEN: auth.stdout.trim(),
      COPILOT_ALLOW_ALL: "false", COPILOT_CUSTOM_INSTRUCTIONS_DIRS: "", USE_TGREP: "false" };
    for (const key of Object.keys(env)) if (key.startsWith("COPILOT_PROVIDER_")) delete env[key];
    const responses = [];
    const observedModels = new Set();
    const js = cli.endsWith(".js");
    const version = spawnSync(js ? process.execPath : cli, [...(js ? [cli] : []), "--no-auto-update", "--version"],
      { cwd, env, encoding: "utf8" });
    assert.equal(version.status, 0, "Cannot establish local Copilot version");
    const revision = spawnSync("git", ["rev-parse", "HEAD"], { cwd: root, encoding: "utf8" });
    assert.equal(revision.status, 0, "Cannot establish repository revision");
    for (let i = 0; i < prepared.requests.length; i++) {
      const args = [...cliOptions(prepared.metadata), "--log-dir", home, "--prompt", prepared.requests[i].prompt];
      const output = spawnSync(js ? process.execPath : cli, js ? [cli, ...args] : args,
        { cwd, env, encoding: "utf8", timeout: 600000, maxBuffer: 16 * 1024 * 1024 });
      fs.writeFileSync(saved(`raw-${i}.jsonl`), output.stdout ?? "");
      fs.writeFileSync(saved(`stderr-${i}.log`), output.stderr ?? "");
      assert.equal(output.status, 0, `Model invocation ${i} failed: ${output.error?.message ?? output.stderr}`);
      const { message, observedModels: reported } = parseTranscript(output.stdout, prepared.metadata.model);
      for (const model of reported) observedModels.add(model);
      responses.push(message);
      write("responses.json", responses);
    }
    write("invocation.json", { model: prepared.metadata.model, args: cliOptions(prepared.metadata),
      observedModels: [...observedModels], cliVersion: version.stdout.trim(), revision: revision.stdout.trim(),
      policyHash: prepared.metadata.policyHash, corpusHash: prepared.metadata.corpusHash,
      responseHashes: responses.map(hash), toolCalls: 0, deterministicSuiteExit: suite.status });
  }
  const result = await evaluate({ workflow, corpus, prepared, responses: read("responses.json") });
  write("result.json", result);
  console.log(JSON.stringify(result, null, 2));
  if (!result.passed) process.exitCode = 1;
}

module.exports = { extractPolicy, prepare, evaluate, cliOptions, parseTranscript };
if (require.main === module) main().catch((error) => { console.error(error.message); process.exitCode = 1; });
