"use strict";

const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const http = require("node:http");
const { POLICY_VERSION, normalizeMemory } = require("./core.cjs");
const { OUTPUT_TYPE } = require("./publish.cjs");
const { collectWorkflow, publishWorkflow } = require("./workflow.cjs");
const { fake, report, now, clone, repo } = require("./test-support.cjs");

// Run the production HTTP path; the stdio dynamic handler skips large-field offloading.
test("pinned HTTP MCP -> ingestion -> guarded staged publication preserves exact proposal text", {
  skip: !process.env.GH_AW_RUNTIME && "Set GH_AW_RUNTIME to the official v0.76.1 actions/setup/js directory",
}, async (t) => {
  const runtime = process.env.GH_AW_RUNTIME;
  const { main: ingest } = require(path.join(runtime, "collect_ndjson_output.cjs"));
  const { main: generateTools } = require(path.join(runtime, "generate_safe_outputs_tools.cjs"));
  const source = fs.readFileSync(path.join(__dirname, "..", "..", "workflows", "regression-triage.md"), "utf8");
  const lock = fs.readFileSync(path.join(__dirname, "..", "..", "workflows", "regression-triage.lock.yml"), "utf8");
  const config = JSON.parse(lock.match(/^\s*(\{"publish-regression-triage":.*\})\r?$/m)[1]);
  const toolsMeta = JSON.parse(lock.match(/GH_AW_TOOLS_META_JSON: \|\r?\n([\s\S]*?)\s+GH_AW_VALIDATION_JSON:/)[1]);
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
  const files = new Map([["trusted-validation.json", validation], ["config.json", JSON.stringify(config)]]);
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
    GH_AW_VALIDATION_CONFIG_PATH: "trusted-validation.json", GH_AW_ALLOWED_DOMAINS: "github.com",
    GH_AW_SAFE_OUTPUTS_TOOLS_PATH: "tools.json", GH_AW_TOOLS_META_JSON: JSON.stringify(toolsMeta),
    GH_AW_SAFE_OUTPUTS_TOOLS_SOURCE_PATH: path.join(runtime, "safe_outputs_tools.json"),
    GH_AW_VALIDATION_JSON: "{}",
  });
  // Only file I/O is virtualized, including the large-content sink. Transport and ingestion are real.
  const read = fs.readFileSync;
  const exists = fs.existsSync;
  t.mock.method(fs, "readFileSync", (file, ...args) => files.has(file) ? files.get(file) : read(file, ...args));
  t.mock.method(fs, "existsSync", (file) => files.has(file) || exists(file));
  t.mock.method(fs, "mkdirSync", () => {});
  t.mock.method(fs, "writeFileSync", (file, content) => files.set(file, content));
  t.mock.method(fs, "appendFileSync", (file, content) => files.set(file, (files.get(file) ?? "") + content));
  await generateTools();
  const { createMCPServer } = require(path.join(runtime, "safe_outputs_mcp_server_http.cjs"));
  const { MCPHTTPTransport } = require(path.join(runtime, "mcp_http_transport.cjs"));
  const { server } = createMCPServer();
  assert.deepEqual([...server.tools.keys()], [OUTPUT_TYPE]);
  const transport = new MCPHTTPTransport();
  await server.connect(transport);
  const listener = http.createServer((req, res) => transport.handleRequest(req, res));
  await new Promise((resolve) => listener.listen(0, "127.0.0.1", resolve));
  t.after(() => new Promise((resolve) => { listener.close(resolve); listener.closeAllConnections(); }));
  const call = async (method, params) => {
    const response = await fetch(`http://127.0.0.1:${listener.address().port}`, {
      method: "POST", headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ jsonrpc: "2.0", id: 1, method, params }),
    });
    assert.equal(response.status, 200);
    const message = await response.json();
    assert.equal(message.error, undefined);
    return message.result;
  };
  assert.deepEqual((await call("tools/list", {})).tools.map((tool) => tool.name), [OUTPUT_TYPE]);
  const examples = [
    "Compiler A worked; compiler B fails with List<T> and <summary>text</summary>.",
    "Compiler A worked; B fails at https://example.invalid/repro and http://example.invalid/old.",
    "Compiler A worked; B fails. @contributor says <!-- precise correction -->.",
    'Compiler A worked; B fails for "quotes", C:\\src\\test.fs, `code`, {{template}} and %253A.\nNext line.',
    "Compiler A worked; B fails with Unicode \u00e9 and \u{1f600}.",
  ].map((body) => ({ name: body, body, count: 1 }));
  examples.push({ name: "valid empty batch preserves discovery", count: 0 });
  for (const missing of ["absent", "empty", "invalid"]) {
    examples.push({ name: `${missing} output leaves no publishable output type`, count: 0, missing });
  }
  for (const [name, character] of [["ASCII", "x"], ["Unicode", "\u00e9\u{1f600}"], ["escaped", '"\\\n']]) {
    for (const bytes of [63999, 64000, 64001, 65536]) {
      examples.push({ name: `${name} five-result batch at ${bytes} bytes`,
        body: "Compiler A worked; compiler B fails. " + character.repeat(150), count: 5, bytes });
    }
  }
  for (const { name, body, count, bytes, missing } of examples) await t.test(name, async () => {
    files.set("proposals.jsonl", "");
    const api = fake({ issues: Array.from({ length: count }, (_, i) => report(42 + i, { body })), pageSize: 100 });
    const mutations = [];
    const deny = async () => { mutations.push("write"); throw new Error("Staged write leaked"); };
    api.github.rest.issues.addLabels = api.github.rest.issues.createComment = deny;
    api.github.rest.git = { createRef: deny };
    api.github.graphql = deny;
    let version = { headOid: "b".repeat(40), state: normalizeMemory(null), missing: null };
    const store = { read: async () => clone(version), commit: deny };
    const collected = await collectWorkflow({ github: api.github, store, env, event, now });
    assert.equal(collected.manifest.selected.length, count);
    const batch = { schemaVersion: 1, policyVersion: POLICY_VERSION, results: collected.manifest.selected.map((entry) => ({
      number: entry.number, fingerprint: entry.fingerprint, classification: "regression",
      evidence: Array.from({ length: bytes ? 10 : 1 }, () => ({
        sourceId: entry.snapshot.bodySourceId, url: entry.snapshot.url, quote: body,
      })),
      missingFact: null, clarification: null,
    })) };
    let proposals = JSON.stringify(batch);
    if (bytes) {
      proposals += " ".repeat(bytes - Buffer.byteLength(proposals));
      assert.equal(Buffer.byteLength(proposals), bytes);
    }
    const output = { items: [{ type: OUTPUT_TYPE, proposals }], errors: [] };
    const publish = (output) => publishWorkflow({ github: api.github, store, env, event, now,
      manifestText: collected.manifestText, artifactName: collected.artifactName, output });
    if (missing) {
      if (missing === "absent") files.delete("proposals.jsonl");
      if (missing === "invalid") files.set("proposals.jsonl", '{"type":"add_labels"}\n');
      await ingest();
      assert.deepEqual(failures, []);
      assert.equal(outputs.output_types, "");
      await assert.rejects(publish(outputs.output));
      assert.deepEqual(mutations, []);
      return;
    }
    const rejected = bytes > 64000;
    await call("tools/call", { name: OUTPUT_TYPE, arguments: { proposals } });
    await ingest();
    assert.deepEqual(failures, []);
    const ingested = JSON.parse(outputs.output);
    assert.deepEqual(ingested.errors, []);
    if (rejected) {
      if (proposals.length > 64000) assert.match(ingested.items[0].proposals, /^\[Content too large, saved to file: /);
      else assert.equal(ingested.items[0].proposals, proposals);
      await assert.rejects(publish(output), /JSON size/);
      await assert.rejects(publish(ingested));
      assert.deepEqual(mutations, []);
      return;
    }
    assert.equal((await publish(output)).receipts.filter((r) => r.type === "would-add-label").length, count);
    assert.equal(ingested.items[0].proposals, proposals);
    const published = await publish(ingested);
    assert.equal(published.incomplete, false);
    assert.ok(published.receipts.some((receipt) => receipt.type === "would-save-memory"));
    for (const entry of collected.manifest.selected) assert.equal(published.state.issues[entry.number].evidence[0].quote, body);
    assert.equal(published.receipts.filter((r) => r.type === "would-add-label").length, count);
    version = { ...version, state: published.state };
    assert.equal((await collectWorkflow({ github: api.github, store, env, event, now })).manifest.selected.length, 0);
    assert.deepEqual(mutations, []);
    files.set("proposals.jsonl", files.get("proposals.jsonl") + '{"type":"add_labels"}\n');
    await ingest();
    await assert.rejects(publishWorkflow({ github: api.github, store, env, event, now,
      manifestText: collected.manifestText, artifactName: collected.artifactName, output: outputs.output }));
  });
});
