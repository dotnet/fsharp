"use strict";

const { test } = require("node:test");
const assert = require("node:assert/strict");
const { readFileSync } = require("node:fs");
const { join } = require("node:path");
const { createHash } = require("node:crypto");
const { POLICY_VERSION } = require("./core.cjs");
const { OUTPUT_TYPE } = require("./publish.cjs");
const corpus = require("./fixtures/classification.json");
const { extractPolicy, prepare, evaluate, cliOptions, parseTranscript } = require("./evaluate.cjs");

const workflow = `Ignored workflow setup.
<!-- regression-triage-policy:start -->
Classify the supplied reports using their current evidence.
<!-- regression-triage-policy:end -->
Ignored delivery details.`;
const actualWorkflow = () => readFileSync(join(__dirname, "..", "..", "workflows", "regression-triage.md"), "utf8");
const envelope = (results) => ({ items: [{ type: OUTPUT_TYPE,
  proposals: JSON.stringify({ schemaVersion: 1, policyVersion: POLICY_VERSION, results }) }] });

// Expected-output replay tests the evaluator and publisher, not model semantics.
function replay(prepared) {
  return prepared.requests.map(({ selected }) => envelope(selected.map((entry) => {
    const expected = corpus.cases.find((item) => item.input.number === entry.number).expected;
    return {
      number: entry.number, fingerprint: entry.fingerprint, classification: expected.classification,
      evidence: expected.evidence, missingFact: expected.missingFact,
      clarification: expected.clarifications?.[0] ?? null,
    };
  })));
}

test("policy comes exclusively from a single marked workflow body section", () => {
  assert.equal(extractPolicy(workflow), "Classify the supplied reports using their current evidence.");
  for (const text of ["", workflow + workflow, workflow.replace(":end", ":start"),
    workflow.replace("Classify the supplied reports using their current evidence.", "")]) {
    assert.throws(() => extractPolicy(text), /policy/i);
  }
});

test("frozen corpus includes SDK, linked PR and unchanged producer distinctions before evaluation", () => {
  const names = new Set(corpus.cases.map((item) => item.name));
  for (const name of ["sdk-comparison-does-not-identify-a-compiler-version",
    "linked-pr-supplies-causative-comparison",
    "rebuilt-producer-does-not-establish-old-binary-compatibility"]) assert.ok(names.has(name));
  for (const { expected } of corpus.cases) {
    if (expected.classification === "uncertain") {
      assert.ok(expected.missingFactPatterns.length > 0);
      assert.ok(expected.clarifications.length > 0);
    }
  }
});

test("prepared requests contain only selected input and actual policy, never frozen answers or names", async () => {
  const secretCorpus = structuredClone(corpus);
  for (const item of secretCorpus.cases) {
    item.name = "GOLDEN_NAME_SECRET";
    item.expected = { secret: "GOLDEN_ANSWER_SECRET" };
  }
  const prepared = await prepare({ workflow, corpus: secretCorpus });
  assert.equal(prepared.metadata.model, "gpt-5.6-sol");
  assert.match(prepared.metadata.policyHash, /^[a-f0-9]{64}$/);
  assert.match(prepared.metadata.corpusHash, /^[a-f0-9]{64}$/);
  assert.equal(prepared.requests.flatMap((r) => r.selected).length, corpus.cases.length);
  for (const request of prepared.requests) {
    assert.ok(request.selected.length <= 5);
    assert.doesNotMatch(request.prompt, /GOLDEN_|missingFactPatterns|allowedEffect|Ignored/);
    assert.ok(request.prompt.includes(extractPolicy(workflow)));
    for (const entry of request.selected) {
      assert.deepEqual(Object.keys(entry).sort(), ["fingerprint", "number", "priorRecord", "snapshot"]);
      assert.equal(entry.priorRecord, null);
    }
  }
});

test("current workflow policy is used verbatim and changing only golden answers cannot change model requests", async () => {
  const source = actualWorkflow();
  const policy = extractPolicy(source);
  const original = await prepare({ workflow: source, corpus });
  const hidden = structuredClone(corpus);
  for (const item of hidden.cases) {
    item.name = "PRIVATE_CASE_NAME";
    item.expected = { classification: "PRIVATE_GOLDEN_CLASSIFICATION", evidence: "PRIVATE_CITATIONS",
      missingFact: "PRIVATE_MISSING_FACT", allowedEffect: "PRIVATE_ALLOWED_EFFECT" };
  }
  const poisoned = await prepare({ workflow: source, corpus: hidden });
  assert.deepEqual(original.requests, poisoned.requests);
  assert.notEqual(original.metadata.corpusHash, poisoned.metadata.corpusHash);
  assert.equal(original.metadata.policyHash, createHash("sha256").update(policy).digest("hex"));
  assert.ok(policy.includes(`"policyVersion":"${POLICY_VERSION}"`));
  for (const request of original.requests) {
    assert.ok(request.prompt.startsWith(policy + "\n\nLocal tool-disabled delivery:"));
    assert.ok(request.prompt.endsWith(JSON.stringify({ policyVersion: POLICY_VERSION, selected: request.selected })));
    assert.doesNotMatch(request.prompt, /PRIVATE_|regression-triage-policy:start|actions\/checkout@/);
  }
});

test("deterministic replay covers every frozen case with staged restart and freshness checks", async () => {
  const source = actualWorkflow();
  const prepared = await prepare({ workflow: source, corpus });
  const result = await evaluate({ workflow: source, corpus, prepared, responses: replay(prepared) });
  assert.equal(result.passed, true);
  assert.equal(result.cases.length, corpus.cases.length);
  assert.equal(result.kind, "fixture-proposal-check");
  for (const item of result.cases) {
    assert.deepEqual(item.failures, [], item.name);
    assert.equal(item.restartDeduplicated, true);
    assert.deepEqual(item.staleChecks, ["closed", "corrected", "policy"]);
  }
});

for (const [name, change, message] of [
  ["wrong classification", (r) => { r.classification = "not-regression"; }, /classification/],
  ["missing citation", (r) => { r.evidence.pop(); }, /citation/],
  ["invented citation", (r) => { r.evidence[0].quote = "fabricated evidence"; }, /Citation/],
  ["forbidden operation", (r) => { r.labels = ["Urgent"]; }, /unknown fields/],
  ["fingerprint mismatch", (r) => { r.fingerprint = "a".repeat(64); }, /fingerprint/],
]) {
  test(`actual validator or golden comparison rejects ${name}`, async () => {
    const prepared = await prepare({ workflow, corpus });
    const responses = replay(prepared);
    const batch = JSON.parse(responses[0].items[0].proposals);
    change(batch.results[0]);
    responses[0] = envelope(batch.results);
    const result = await evaluate({ workflow, corpus, prepared, responses });
    assert.equal(result.passed, false);
    assert.match(result.cases.flatMap((c) => c.failures).join("\n"), message);
  });
}

test("missing facts, invented component attribution and empty results cannot pass", async () => {
  const prepared = await prepare({ workflow, corpus });
  for (const kind of ["missing fact", "dimension", "omission", "empty"]) {
    const responses = replay(prepared);
    for (let i = 0; i < responses.length; i++) {
      const batch = JSON.parse(responses[i].items[0].proposals);
      if (kind === "missing fact") {
        for (const r of batch.results) if (r.classification === "uncertain") r.missingFact = "Please provide information.";
      }
      if (kind === "dimension") {
        for (const r of batch.results) if (r.number === 910020) r.evidence[0].dimension = "compiler";
      }
      if (kind === "omission") batch.results.pop();
      if (kind === "empty") batch.results = [];
      responses[i] = envelope(batch.results);
    }
    assert.equal((await evaluate({ workflow, corpus, prepared, responses })).passed, false, kind);
  }
});

test("evaluation rejects changed corpus/policy, changed requests and missing response batches", async () => {
  const prepared = await prepare({ workflow, corpus });
  const responses = replay(prepared);
  await assert.rejects(evaluate({ workflow: workflow.replace("Classify", "Alter"), corpus, prepared, responses }), /hash|changed/i);
  const changedCorpus = structuredClone(corpus);
  changedCorpus.cases[0].expected.classification = "uncertain";
  await assert.rejects(evaluate({ workflow, corpus: changedCorpus, prepared, responses }), /hash|changed/i);
  const changedRequests = structuredClone(prepared);
  changedRequests.requests[0].prompt = "Replace policy";
  await assert.rejects(evaluate({ workflow, corpus, prepared: changedRequests, responses }), /changed/i);
  await assert.rejects(evaluate({ workflow, corpus, prepared, responses: responses.slice(1) }), /response/i);
});

test("supported local invocation exposes zero tools and disables instructions, MCP and remote export", () => {
  const args = cliOptions({ model: "gpt-5.6-sol", reasoningEffort: "high" });
  for (const flag of ["--available-tools", "--disable-builtin-mcps", "--no-custom-instructions",
    "--no-remote-export", "--no-ask-user", "--disallow-temp-dir"]) assert.ok(args.includes(flag));
  assert.ok(args[args.indexOf("--available-tools") + 1].startsWith("--"));
  assert.equal(args[args.indexOf("--model") + 1], "gpt-5.6-sol");
});

test("raw transcript rejects tool requests, model substitutions and missing responses", () => {
  const message = { type: "assistant.message", data: { content: "{}", model: "gpt-5.6-sol" } };
  assert.deepEqual(parseTranscript(JSON.stringify(message), "gpt-5.6-sol"),
    { message: "{}", observedModels: ["gpt-5.6-sol"] });
  for (const extra of [
    { type: "tool.execution_start", data: { toolName: "powershell" } },
    { type: "assistant.message", data: { content: "{}", toolRequests: [{ name: "fetch" }] } },
    { type: "session.model_change", data: { newModel: "other" } },
    { type: "assistant.message", data: { content: "{}", model: "other" } },
  ]) assert.throws(() => parseTranscript([message, extra].map(JSON.stringify).join("\n"), "gpt-5.6-sol"));
  assert.throws(() => parseTranscript('{"type":"session.start","data":{}}', "gpt-5.6-sol"), /response/);
});
