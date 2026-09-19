"use strict";

const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const { POLICY_VERSION, normalizeMemory } = require("./core.cjs");
const { OUTPUT_TYPE } = require("./publish.cjs");
const { collectWorkflow, publishWorkflow } = require("./workflow.cjs");
const { fake, report, now, clone, repo } = require("./test-support.cjs");

// Run against the official v0.76.1 runtime, never a local sanitizer imitation.
test("pinned MCP -> ingestion -> guarded staged publication preserves exact proposal text", {
  skip: !process.env.GH_AW_RUNTIME && "Set GH_AW_RUNTIME to the official v0.76.1 actions/setup/js directory",
}, async (t) => {
  const runtime = process.env.GH_AW_RUNTIME;
  const { registerDynamicTools } = require(path.join(runtime, "safe_outputs_tools_loader.cjs"));
  const { main: ingest } = require(path.join(runtime, "collect_ndjson_output.cjs"));
  const source = fs.readFileSync(path.join(__dirname, "..", "..", "workflows", "regression-triage.md"), "utf8");
  const lock = fs.readFileSync(path.join(__dirname, "..", "..", "workflows", "regression-triage.lock.yml"), "utf8");
  const config = JSON.parse(lock.match(/^\s*(\{"publish-regression-triage":.*\})\r?$/m)[1]);
  const validationPath = path.join(__dirname, "output-validation.json");
  assert.match(source, /GH_AW_VALIDATION_CONFIG_PATH: \$\{\{ github\.workspace \}\}\/\.github\/scripts\/regression-triage\/output-validation\.json/);
  const validation = fs.readFileSync(validationPath, "utf8");
  assert.deepEqual(JSON.parse(validation), {
    [OUTPUT_TYPE]: { defaultMax: 1, fields: { proposals: { required: true, type: "string", sanitize: false } } },
  });
  const env = {
    GITHUB_REPOSITORY: "dotnet/fsharp", GITHUB_RUN_ID: "123", GITHUB_RUN_ATTEMPT: "1",
    GITHUB_SHA: "a".repeat(40), GITHUB_WORKFLOW_SHA: "a".repeat(40), GITHUB_REF: "refs/heads/main",
    GITHUB_WORKFLOW_REF: "dotnet/fsharp/.github/workflows/regression-triage.lock.yml@refs/heads/main",
    GITHUB_EVENT_NAME: "schedule", GH_AW_SAFE_OUTPUTS_STAGED: "true",
  };
  const event = { repository: { full_name: "dotnet/fsharp", default_branch: "main" } };
  const files = new Map([["validation.json", validation], ["config.json", JSON.stringify(config)]]);
  const outputs = {};
  const failures = [];
  for (const key of ["core", "context", "github"]) {
    assert.equal(Object.hasOwn(globalThis, key), false);
    t.after(() => { delete globalThis[key]; });
  }
  globalThis.core = {
    info() {}, debug() {}, warning() {}, error(message) { failures.push(message); },
    setFailed(message) { failures.push(message); }, exportVariable() {},
    setOutput(name, value) { outputs[name] = value; },
  };
  globalThis.context = { repo, eventName: "schedule", payload: {} };
  globalThis.github = {};
  t.mock.property(process, "env", { ...process.env, ...env,
    GH_AW_SAFE_OUTPUTS: "proposals.jsonl", GH_AW_SAFE_OUTPUTS_CONFIG_PATH: "config.json",
    GH_AW_VALIDATION_CONFIG_PATH: "validation.json", GH_AW_ALLOWED_DOMAINS: "github.com",
  });
  // Only the pinned ingestion file I/O is virtualized; parsing/sanitizing/schema code is real.
  const read = fs.readFileSync;
  t.mock.method(fs, "readFileSync", (file, ...args) => files.has(file) ? files.get(file) : read(file, ...args));
  t.mock.method(fs, "existsSync", (file) => files.has(file));
  t.mock.method(fs, "mkdirSync", () => {});
  t.mock.method(fs, "writeFileSync", (file, content) => files.set(file, content));
  t.mock.method(fs, "appendFileSync", (file, content) => files.set(file, (files.get(file) ?? "") + content));
  const server = { tools: {} };
  registerDynamicTools(server, [], config, "proposals.jsonl",
    (s, tool) => { s.tools[tool.name] = tool; }, (name) => name.replace(/-/g, "_"));
  for (const body of [
    "Compiler A worked; compiler B fails with List<T> and <summary>text</summary>.",
    "Compiler A worked; B fails at https://example.invalid/repro and http://example.invalid/old.",
    "Compiler A worked; B fails. @contributor says <!-- precise correction -->.",
    'Compiler A worked; B fails for "quotes", C:\\src\\test.fs, `code`, {{template}} and %253A.\nNext line.',
    "Compiler A worked; B fails with Unicode \u00e9 and \u{1f600}.",
  ]) {
    files.set("proposals.jsonl", "");
    const api = fake({ issues: [report(42, { body })], pageSize: 100 });
    const mutations = [];
    const deny = async () => { mutations.push("write"); throw new Error("Staged write leaked"); };
    api.github.rest.issues.addLabels = api.github.rest.issues.createComment = deny;
    api.github.rest.git = { createRef: deny };
    api.github.graphql = deny;
    let version = { headOid: "b".repeat(40), state: normalizeMemory(null), missing: null };
    const store = { read: async () => clone(version), commit: deny };
    const collected = await collectWorkflow({ github: api.github, store, env, event, now });
    const entry = collected.manifest.selected[0];
    const batch = { schemaVersion: 1, policyVersion: POLICY_VERSION, results: [{
      number: 42, fingerprint: entry.fingerprint, classification: "regression",
      evidence: [{ sourceId: entry.snapshot.bodySourceId, url: entry.snapshot.url, quote: body }],
      missingFact: null, clarification: null,
    }] };
    const proposals = JSON.stringify(batch);
    server.tools[OUTPUT_TYPE].handler({ proposals });
    await ingest();
    assert.deepEqual(failures, []);
    const output = JSON.parse(outputs.output);
    assert.deepEqual(output.errors, []);
    assert.equal(output.items[0].proposals, proposals);
    const published = await publishWorkflow({ github: api.github, store, env, event, now,
      manifestText: collected.manifestText, artifactName: collected.artifactName, output });
    assert.equal(published.state.issues[42].evidence[0].quote, body);
    assert.equal(published.receipts.filter((r) => r.type === "would-add-label").length, 1);
    version = { ...version, state: published.state };
    assert.equal((await collectWorkflow({ github: api.github, store, env, event, now })).manifest.selected.length, 0);
    assert.deepEqual(mutations, []);
    files.set("proposals.jsonl", files.get("proposals.jsonl") + '{"type":"add_labels"}\n');
    await ingest();
    await assert.rejects(publishWorkflow({ github: api.github, store, env, event, now,
      manifestText: collected.manifestText, artifactName: collected.artifactName, output: outputs.output }));
  }
});
