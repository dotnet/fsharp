"use strict";

const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const os = require("node:os");
const { POLICY_VERSION, LIMITS, normalizeMemory } = require("./core.cjs");
const { OUTPUT_TYPE, ACKNOWLEDGEMENT } = require("./publish.cjs");
const { fake, report, comment, now, clone } = require("./test-support.cjs");
const { eventOptions, collectWorkflow, publishWorkflow, artifactPrefix, verifyArtifact,
  collectAction, resolveArtifact, publishAction } = require("./workflow.cjs");

const env = {
  GITHUB_REPOSITORY: "dotnet/fsharp", GITHUB_RUN_ID: "123", GITHUB_RUN_ATTEMPT: "1",
  GITHUB_SHA: "a".repeat(40), GITHUB_WORKFLOW_SHA: "a".repeat(40),
  GITHUB_REF: "refs/heads/main",
  GITHUB_WORKFLOW_REF: "dotnet/fsharp/.github/workflows/regression-triage.lock.yml@refs/heads/main",
  GITHUB_EVENT_NAME: "schedule",
};
const event = (fields = {}) => ({
  repository: { full_name: "dotnet/fsharp", default_branch: "main" },
  sender: { type: "User", login: "new-contributor" }, ...fields,
});
const envelope = (manifest) => ({ items: [{ type: OUTPUT_TYPE, proposals: JSON.stringify({
  schemaVersion: 1, policyVersion: POLICY_VERSION, results: manifest.selected.map((item) => ({
    number: item.number, fingerprint: item.fingerprint, classification: "regression",
    evidence: [{ sourceId: item.snapshot.bodySourceId, url: item.snapshot.url, quote: item.snapshot.body }],
    missingFact: null, clarification: null,
  })),
}) }] });

function setup(issues = [report()], memory = normalizeMemory(null)) {
  const api = fake({ issues, pageSize: 100 });
  let version = { headOid: "b".repeat(40), state: memory, missing: null };
  const mutations = [];
  const deny = async () => { mutations.push("unexpected write"); throw new Error("Staged write leaked"); };
  const store = { read: async () => clone(version), commit: deny };
  api.github.rest.issues.addLabels = deny;
  api.github.rest.issues.createComment = deny;
  api.github.rest.git = { createRef: deny };
  api.github.graphql = deny;
  const restart = (result) => {
    version = { headOid: "c".repeat(40), state: clone(result.state), missing: null };
  };
  const collect = (fields = {}) => collectWorkflow({
    github: api.github, store, env, event: event(), now, ...fields,
  });
  const publish = (collected, fields = {}) => publishWorkflow({
    github: api.github, store, env: { ...env, GH_AW_SAFE_OUTPUTS_STAGED: "true" }, event: event(),
    now, manifestText: collected.manifestText, artifactName: collected.artifactName,
    output: envelope(collected.manifest), ...fields,
  });
  return { api, store, mutations, restart, collect, publish };
}

for (const [eventName, actions] of [
  ["issues", ["opened", "edited", "reopened", "labeled", "transferred"]],
  ["issue_comment", ["created", "edited", "deleted"]],
]) {
  for (const action of actions) for (const association of ["NONE", "FIRST_TIMER"]) {
    test(`actual ${eventName}/${action} envelope reads ${association}`, async () => {
      const payload = event({ action, issue: report(), label: { name: "Needs-Triage" },
        comment: comment(1, { author_association: association }) });
      const options = eventOptions({ ...env, GITHUB_EVENT_NAME: eventName }, payload);
      assert.equal(options.active, true);
      assert.equal(options.hint, 42);
      const s = setup();
      assert.equal((await s.collect({ env: { ...env, GITHUB_EVENT_NAME: eventName }, event: payload }))
        .manifest.selected[0].number, 42);
    });
  }
}

for (const [name, eventName, fields] of [
  ["PR issue", "issues", { issue: { ...report(), pull_request: {} }, action: "opened" }],
  ["PR comment", "issue_comment", { issue: { ...report(), pull_request: {} }, action: "created", comment: comment(1) }],
  ["unrelated label", "issues", { issue: report(), action: "labeled", label: { name: "Bug" } }],
  ["own label", "issues", { issue: report(), action: "labeled", label: { name: "Regression" }, sender: { type: "Bot" } }],
  ["own question", "issue_comment", { issue: report(), action: "created", comment: comment(1, { user: { type: "Bot" } }) }],
  ["bot edit", "issues", { issue: report(), action: "edited", sender: { type: "Bot" } }],
]) {
  test(`${name} is rejected before collection/model activation`, async () => {
    const s = setup();
    const result = await s.collect({ env: { ...env, GITHUB_EVENT_NAME: eventName }, event: event(fields) });
    assert.equal(result.active, false);
    assert.deepEqual(s.api.calls, []);
  });
}

test("manual controls accept only a positive safe integer and boolean; code is default-branch only", () => {
  const manual = { ...env, GITHUB_EVENT_NAME: "workflow_dispatch" };
  assert.deepEqual(eventOptions(manual, event({ inputs: { issue: "42", staged: "true" } })),
    { active: true, hint: 42, staged: true });
  assert.equal(eventOptions(manual, event()).staged, true);
  assert.equal(eventOptions(manual, event({ inputs: { aw_context: "" } })).staged, true);
  assert.equal(eventOptions(manual, event({ inputs: { staged: "false" } })).staged, false);
  assert.equal(eventOptions({ ...manual, GH_AW_SAFE_OUTPUTS_STAGED: "true" },
    event({ inputs: { staged: "false" } })).staged, true);
  for (const issue of ["0", "-1", "1.5", "9007199254740992", "42; rm", "1e2"]) {
    assert.throws(() => eventOptions(manual, event({ inputs: { issue } })));
  }
  for (const inputs of [{ staged: "no" }, { instructions: "label everything" }, { aw_context: "label everything" },
    { issue: "1", repository: "elsewhere" }]) {
    assert.throws(() => eventOptions(manual, event({ inputs })));
  }
  for (const fields of [{ GITHUB_REF: "refs/heads/reporter" }, { GITHUB_WORKFLOW_SHA: "d".repeat(40) },
    { GITHUB_REPOSITORY: "fork/fsharp" }, { GITHUB_WORKFLOW_REF: "dotnet/fsharp/.github/workflows/other.yml@refs/heads/main" }]) {
    assert.throws(() => eventOptions({ ...manual, ...fields }, event()));
  }
});

test("opened before bot labeling, no label event, then schedules drain eleven reports across restarts", async () => {
  const issues = Array.from({ length: 11 }, (_, n) => report(n + 1, { labels: [] }));
  const s = setup(issues);
  let run = await s.collect({ env: { ...env, GITHUB_EVENT_NAME: "issues" },
    event: event({ action: "opened", issue: issues[0] }) });
  assert.equal(run.manifest.selected.length, 0);
  s.restart(await s.publish(run));
  for (const issue of issues) issue.labels.push("Needs-Triage");
  const labeled = new Set();
  for (let n = 0; n < 5; n++) {
    const runtime = { ...env, GITHUB_RUN_ID: String(124 + n), GH_AW_SAFE_OUTPUTS_STAGED: "true" };
    run = await s.collect({ env: runtime, now: new Date(Date.parse(now) + (n + 1) * 3600000).toISOString() });
    assert.ok(run.manifest.selected.length <= 5);
    const result = await s.publish(run, { env: runtime });
    for (const receipt of result.receipts.filter((r) => r.type === "would-add-label")) {
      assert.ok(!labeled.has(receipt.number));
      assert.deepEqual(receipt.labels, ["Regression"]);
      labeled.add(receipt.number);
    }
    s.restart(result);
  }
  assert.equal(labeled.size, 11);
  assert.deepEqual(s.mutations, []);
  assert.equal((await s.collect()).manifest.selected.length, 0);
});

test("empty batch persists discovery; missing output does not advance memory", async () => {
  const s = setup([]);
  const run = await s.collect();
  assert.equal(run.active, true);
  const before = await s.store.read();
  await assert.rejects(s.publish(run, { output: undefined }));
  assert.deepEqual(await s.store.read(), before);
  const result = await s.publish(run);
  assert.equal(result.state.scan.updatedThrough, now);
  assert.ok(result.receipts.some((r) => r.type === "would-save-memory"));
});

test("omitting selected results fails without advancing discovery", async () => {
  const s = setup();
  const run = await s.collect();
  const before = await s.store.read();
  await assert.rejects(s.publish(run, { output: envelope({ selected: [] }) }), /Incomplete proposal batch/);
  assert.deepEqual(await s.store.read(), before);
  assert.deepEqual(s.mutations, []);
});

test("linked evidence cannot replace a citation to the selected report", async () => {
  const s = setup([report(42, { body: "Compiler B now fails on my source; see #43." }), report(43, { labels: [] })]);
  const run = await s.collect();
  const output = envelope(run.manifest);
  const batch = JSON.parse(output.items[0].proposals);
  const linked = run.manifest.selected[0].snapshot.linked[0];
  batch.results[0].evidence = [{ sourceId: linked.bodySourceId, url: linked.url, quote: linked.body }];
  output.items[0].proposals = JSON.stringify(batch);
  const before = await s.store.read();
  await assert.rejects(s.publish(run, { output }), /Missing selected-report citation/);
  assert.deepEqual(await s.store.read(), before);
  assert.deepEqual(s.mutations, []);
});

test("reconciliation reads bot-created reports, but current closed/unlabeled reports are never selected", async () => {
  const s = setup([report(1, { user: { id: 41898282, login: "github-actions[bot]", type: "Bot" } }),
    report(2, { state: "closed" }), report(3, { labels: [] })]);
  const run = await s.collect();
  assert.deepEqual(run.manifest.selected.map((item) => item.number), [1]);
  assert.equal((await s.publish(run)).receipts.filter((r) => r.type === "would-add-label").length, 1);
});

test("trusted input errors remain visible; oversized evidence is not partially classified", async () => {
  const s = setup([report(1, { body: "text ".repeat(20000) })]);
  const run = await s.collect();
  assert.equal(run.manifest.selected.length, 0);
  assert.ok(run.manifest.incomplete.some((item) => item.number === 1));
  assert.match(run.summary, /incomplete|bound/);
  const result = await s.publish(run);
  assert.equal(result.incomplete, true);
  assert.ok(result.state.pending.some((item) => item.number === 1));
  const failed = setup();
  failed.api.github.rest.issues.listForRepo = async () => { throw new Error("API unavailable"); };
  assert.match((await failed.collect()).summary, /request-failed/);
  const partial = setup();
  partial.api.github.rest.issues.listComments = async () => { throw new Error("Incomplete discussion"); };
  const incomplete = await partial.collect();
  assert.deepEqual(incomplete.manifest.incomplete, [{ number: 42 }]);
  assert.equal(incomplete.manifest.selected.length, 0);
  assert.ok(incomplete.manifest.errors.some((error) => error.stage === "comment"));
});

for (const [oversized, hot] of [[5, false], [10, false], [5, true]]) {
  test(`${oversized} oversized reports cannot starve eleven complete reports (${hot ? "changing" : "stable"}) across staged restarts`, async () => {
    const reports = Array.from({ length: oversized + 11 }, (_, i) =>
      report(i + 1, i < oversized ? { body: "text ".repeat(10000) } : {}));
    const s = setup(reports);
    const seen = new Set();
    for (let run = 0; run < 18; run++) {
      const time = new Date(Date.parse(now) + run * 3600000).toISOString();
      if (hot) for (const issue of reports.slice(-LIMITS.candidates)) {
        issue.body += " More evidence.";
        issue.updated_at = time;
      }
      s.api.calls.length = 0;
      const collected = await s.collect({ now: time });
      assert.ok(s.api.calls.filter((call) => call.name === "get").length <= 2 * LIMITS.snapshotReads);
      assert.ok(collected.manifest.selected.length <= LIMITS.candidates);
      assert.ok(collected.manifest.selected.every((entry) => entry.number > oversized));
      if (oversized === 5 && !hot && run === 0) {
        assert.deepEqual(collected.manifest.selected.map((entry) => entry.number), [6, 7, 8, 9, 10]);
      }
      const result = await s.publish(collected, { now: time });
      assert.equal(result.incomplete, collected.manifest.incomplete.length > 0);
      assert.ok(result.receipts.some((receipt) => receipt.type === "would-save-memory"));
      for (const receipt of result.receipts.filter((receipt) => receipt.type === "would-add-label")) {
        if (!hot || receipt.number <= reports.length - LIMITS.candidates) {
          assert.ok(!seen.has(receipt.number), "duplicate staged effect");
        }
        seen.add(receipt.number);
      }
      for (let number = 1; number <= oversized; number++) {
        assert.ok(result.state.pending.some((entry) => entry.number === number));
        assert.equal(result.state.issues[number]?.classification, undefined);
      }
      s.restart(result);
    }
    assert.deepEqual([...seen].sort((a, b) => a - b), reports.slice(oversized).map((issue) => issue.number));
    reports[0].body = report().body;
    const time = new Date(Date.parse(now) + 18 * 3600000).toISOString();
    const recovered = await s.collect({ now: time });
    assert.ok(recovered.manifest.selected.some((entry) => entry.number === 1));
    assert.ok((await s.publish(recovered, { now: time })).receipts.some((receipt) =>
      receipt.type === "would-add-label" && receipt.number === 1));
    assert.deepEqual(s.mutations, []);
  });
}

for (const bytes of [49151, 49152, 49153]) {
  test(`model entry content bound uses exact UTF-8 bytes: ${bytes}`, async () => {
    const s = setup();
    const original = (await s.collect()).manifest.selected[0];
    s.api.issues[0].body += " ".repeat(bytes - Buffer.byteLength(JSON.stringify(original)));
    const collected = await s.collect();
    assert.equal(collected.manifest.stateDelta.pending[0].lastSelectedAt, bytes <= 49152 ? now : undefined);
    if (bytes <= 49152) {
      assert.equal(Buffer.byteLength(JSON.stringify(collected.manifest.selected[0])), bytes);
      assert.equal(collected.manifest.selected[0].snapshot.body, s.api.issues[0].body);
      assert.deepEqual(collected.manifest.incomplete, []);
    } else {
      assert.deepEqual(collected.manifest.selected, []);
      assert.deepEqual(collected.manifest.incomplete, [{ number: 42 }]);
    }
  });
}

test("batch byte limits refill from complete snapshots and retry deferred evidence without truncation", async () => {
  const s = setup(Array.from({ length: 6 }, (_, i) => report(i + 1)));
  for (let number = 1; number <= 5; number++) {
    s.api.comments[number] = [comment(number, { body: "\u96ea".repeat(15000),
      html_url: `${report(number).url}#issuecomment-${number}` })];
  }
  const collected = await s.collect();
  assert.deepEqual(collected.manifest.selected.map((entry) => entry.number), [1, 2, 3, 4, 6]);
  assert.deepEqual(collected.manifest.incomplete, [{ number: 5 }]);
  for (const entry of collected.manifest.stateDelta.pending) {
    assert.equal(entry.lastSelectedAt, entry.number === 5 ? undefined : now);
  }
  assert.ok(collected.manifest.errors.some((error) => error.code === "content-bound" && error.number === 5));
  assert.ok(collected.manifest.selected.reduce((bytes, entry) => {
    const size = Buffer.byteLength(JSON.stringify(entry));
    assert.ok(size <= 49152);
    if (entry.number !== 6) assert.equal(entry.snapshot.humanComments[0].body, s.api.comments[entry.number][0].body);
    return bytes + size;
  }, 0) <= 196608);
  const first = await s.publish(collected);
  assert.equal(first.incomplete, true);
  s.restart(first);
  const retry = await s.collect();
  assert.deepEqual(retry.manifest.selected.map((entry) => entry.number), [5]);
  assert.equal(retry.manifest.selected[0].snapshot.humanComments[0].body, s.api.comments[5][0].body);
  const second = await s.publish(retry);
  assert.equal(second.incomplete, false);
  s.restart(second);
  assert.deepEqual((await s.collect()).manifest.selected, []);
  assert.deepEqual(s.mutations, []);
});

test("the Actions entry points bind immutable artifact metadata, dispatch staging and the event file", async () => {
  const temp = fs.mkdtempSync(path.join(os.tmpdir(), "regression-triage-"));
  const prior = { ...process.env };
  const outputs = {};
  const summaries = [];
  const failures = [];
  const core = {
    setOutput: (name, value) => { outputs[name] = value; },
    setFailed: (message) => { failures.push(message); },
    warning: (message) => summaries.push(message),
    summary: { addRaw(text) { summaries.push(text); return this; },
      addCodeBlock(text) { summaries.push(text); return this; }, async write() {} },
  };
  try {
    Object.assign(process.env, env, { RUNNER_TEMP: temp,
      GITHUB_EVENT_NAME: "workflow_dispatch", GITHUB_EVENT_PATH: path.join(temp, "event.json"),
      GH_AW_AGENT_OUTPUT: path.join(temp, "output.json") });
    delete process.env.GH_AW_SAFE_OUTPUTS_STAGED;
    fs.writeFileSync(process.env.GITHUB_EVENT_PATH, JSON.stringify(event({ inputs: { staged: "true" } })));
    const s = setup();
    await collectAction({ github: s.api.github, core });
    assert.equal(outputs.active, "true");
    const manifestText = fs.readFileSync(path.join(temp, "regression-triage-manifest", "manifest.json"), "utf8");
    const manifest = JSON.parse(manifestText);
    assert.equal(manifest.binding.memoryHead, null); // Confirmed missing branch via the real store.
    assert.equal(manifest.binding.collectorRevision, env.GITHUB_WORKFLOW_SHA);
    const view = JSON.parse(fs.readFileSync(path.join(temp, "regression-triage-input", "input.json"), "utf8"));
    assert.equal(view.selected[0].snapshot.body, report().body);
    assert.equal(view.stateDelta, undefined);
    assert.equal(view.binding, undefined);
    const artifact = { id: 123, name: outputs["manifest-name"], expired: false,
      workflow_run: { id: 123, head_sha: env.GITHUB_WORKFLOW_SHA } };
    s.api.github.rest.actions = { listWorkflowRunArtifacts: async (args) => {
      assert.equal(args.run_id, env.GITHUB_RUN_ID);
      return { data: { total_count: 1, artifacts: [artifact] } };
    } };
    await resolveArtifact({ github: s.api.github, core });
    assert.equal(outputs["artifact-id"], "123");
    for (const mutate of [
      (a) => { a.expired = true; }, (a) => { a.workflow_run.id = 456; },
      (a) => { a.workflow_run.head_sha = "d".repeat(40); }, (a) => { a.name = "agent"; },
    ]) {
      const changed = clone(artifact);
      mutate(changed);
      s.api.github.rest.actions.listWorkflowRunArtifacts = async () => ({
        data: { total_count: 1, artifacts: [changed] },
      });
      await assert.rejects(resolveArtifact({ github: s.api.github, core }));
    }
    for (const artifacts of [[], [artifact, { ...artifact, id: 124 }]]) {
      s.api.github.rest.actions.listWorkflowRunArtifacts = async () => ({ data: { total_count: artifacts.length, artifacts } });
      await assert.rejects(resolveArtifact({ github: s.api.github, core }));
    }
    fs.mkdirSync(path.join(temp, "regression-triage-trusted"));
    fs.writeFileSync(path.join(temp, "regression-triage-trusted", "manifest.json"), manifestText);
    fs.writeFileSync(process.env.GH_AW_AGENT_OUTPUT, JSON.stringify({ ...envelope(manifest), errors: [] }));
    process.env.TRIAGE_ARTIFACT_NAME = artifact.name;
    await publishAction({ github: s.api.github, core });
    assert.deepEqual(s.mutations, []);
    assert.deepEqual(failures, []);
    assert.match(summaries.join("\n"), /would-add-label/);
    assert.match(summaries.join("\n"), /would-save-memory/);
    s.api.github.rest.issues.listForRepo = async () => { throw new Error("Unavailable"); };
    await collectAction({ github: s.api.github, core });
    const partial = fs.readFileSync(path.join(temp, "regression-triage-manifest", "manifest.json"), "utf8");
    fs.writeFileSync(path.join(temp, "regression-triage-trusted", "manifest.json"), partial);
    fs.writeFileSync(process.env.GH_AW_AGENT_OUTPUT, JSON.stringify(envelope(JSON.parse(partial))));
    process.env.TRIAGE_ARTIFACT_NAME = outputs["manifest-name"];
    await publishAction({ github: s.api.github, core });
    assert.match(failures[0], /Incomplete regression triage/);
    assert.match(summaries.join("\n"), /request-failed/);
    assert.deepEqual(s.mutations, []);
    fs.rmSync(process.env.GH_AW_AGENT_OUTPUT);
    await assert.rejects(publishAction({ github: s.api.github, core }), /ENOENT/);
  } finally {
    for (const key of Object.keys(process.env)) if (!Object.hasOwn(prior, key)) delete process.env[key];
    Object.assign(process.env, prior);
    fs.rmSync(temp, { recursive: true });
  }
});

test("artifact absence, wrong binding, substitution and content tampering fail closed", async () => {
  const s = setup();
  const run = await s.collect();
  assert.match(run.artifactName, new RegExp(`^${artifactPrefix(env)}`));
  for (const fields of [
    { artifactName: "" }, { manifestText: "" },
    { env: { ...env, GITHUB_RUN_ATTEMPT: "2" } },
    { manifestText: run.manifestText.replace("Compiler behavior changed", "Replaced evidence") },
    { manifestText: run.manifestText.replace('"pending":', '"injected":') },
  ]) await assert.rejects(s.publish(run, fields));
  assert.throws(() => verifyArtifact(run.manifestText, run.artifactName, { ...env, GITHUB_RUN_ID: "456" }));
  assert.deepEqual(s.mutations, []);
});

test("trusted Action deadlines fail the process instead of continuing after timeout", async (t) => {
  let callback;
  let milliseconds;
  const exits = [];
  const failures = [];
  t.mock.method(globalThis, "setTimeout", (action, delay) => { callback = action; milliseconds = delay; return 1; });
  t.mock.method(globalThis, "clearTimeout", () => {});
  t.mock.method(process, "exit", (code) => exits.push(code));
  t.mock.method(fs, "readFileSync", () => { throw new Error("fixture stopped at runtime input"); });
  for (const [action, minutes] of [[collectAction, 10], [resolveArtifact, 2], [publishAction, 15]]) {
    const pending = action({ github: fake().github, core: { setFailed: (message) => failures.push(message) } });
    assert.equal(milliseconds, minutes * 60000);
    callback();
    await assert.rejects(pending);
  }
  assert.deepEqual(exits, [1, 1, 1]);
  assert.ok(failures.every((message) => message.includes("deadline")));
});

for (const [name, controls] of [
  ["dispatch", { event: event({ inputs: { staged: "true" } }), env: { ...env, GITHUB_EVENT_NAME: "workflow_dispatch" } }],
  ["environment", { env: { ...env, GH_AW_SAFE_OUTPUTS_STAGED: "true" } }],
]) test(`${name} staging suppresses all writes`, async () => {
  const s = setup();
  const run = await s.collect(controls);
  const result = await s.publish(run, controls);
  assert.ok(result.receipts.some((r) => r.type === "would-add-label"));
  assert.deepEqual(s.mutations, []);
  const output = envelope(run.manifest);
  output.items[0].staged = false;
  await assert.rejects(s.publish(run, { ...controls, output }));
});

for (const change of [
  (issue) => { issue.state = "closed"; },
  (issue) => { issue.labels = []; },
  (issue) => { issue.body = "Correction: this never worked."; },
]) test("publication rechecks current eligibility/corrections", async () => {
  const s = setup();
  const run = await s.collect();
  change(s.api.issues[0]);
  const result = await s.publish(run);
  assert.equal(result.outcomes[0].status, "stale");
  assert.ok(!result.receipts.some((r) => r.type === "would-add-label"));
});

test("compiled completion guard rejects missing output and incomplete publication, not gated skips", async (t) => {
  const root = path.resolve(__dirname, "..", "..", "workflows");
  const lock = fs.readFileSync(path.join(root, "regression-triage.lock.yml"), "utf8");
  const job = (text, name) => text.match(new RegExp(`^  ${name}:\\r?\\n[\\s\\S]*?(?=^  [\\w-]+:|^\\S|$(?![\\s\\S]))`, "m"))?.[0];
  const guard = job(lock, "regression_triage_completion");
  assert.ok(guard, "missing trusted completion job: a successful agent can omit required output");
  const condition = (text) => text.match(/^    if: (?:>\r?\n)?([\s\S]*?)(?=^    \S)/m)[1].trim();
  const evaluate = (expression, needs) => require("node:vm").runInNewContext(expression, {
    needs, always: () => true, cancelled: () => false, contains: (value, item) => value.includes(item),
  });
  const completed = guard.match(/TRIAGE_COMPLETED: \$\{\{ ([\s\S]*?) \}\}/)[1];
  const script = guard.match(/          script: \|\r?\n([\s\S]*)/)[1].replace(/^            /gm, "");
  assert.match(guard, /if: always\(\) && needs\.pre_activation\.outputs\.active == 'true'/);
  assert.match(guard, /permissions:\s+\{\}/);
  assert.doesNotMatch(guard, /checkout@|continue-on-error|: write/);
  for (const name of ["pre_activation", "activation", "agent", "detection", "publish_regression_triage"]) {
    assert.match(guard, new RegExp(`^      - ${name}$`, "m"));
  }
  const source = job(fs.readFileSync(path.join(root, "regression-triage.md"), "utf8"), "regression_triage_completion");
  assert.equal(condition(source), condition(guard));
  assert.equal(source.match(/TRIAGE_COMPLETED: \$\{\{ ([\s\S]*?) \}\}/)[1], completed);
  assert.equal(source.match(/          script: \|\r?\n([\s\S]*)/)[1].replace(/\r/g, "").replace(/^            /gm, "").trim(), script.trim());
  const success = {
    pre_activation: { result: "success", outputs: { active: "true" } },
    activation: { result: "success" },
    agent: { result: "success", outputs: { output_types: OUTPUT_TYPE, has_patch: "false" } },
    detection: { result: "success", outputs: { detection_success: "true", detection_conclusion: "success" } },
    publish_regression_triage: { result: "success" },
  };
  const cases = [
    ["valid empty batch (staged)", {}, false],
    ["published batch", {}, false],
    ["no safe-output call", { agent: { result: "success", outputs: { output_types: "", has_patch: "false" } } }, true],
    ["ingestion rejected every item", { agent: { result: "success", outputs: { output_types: "", has_patch: "false" } } }, true],
    ["ingestion errors alongside valid output", { publish_regression_triage: { result: "failure" } }, true],
    ["detection rejected output", { detection: { result: "success", outputs: { detection_success: "false", detection_conclusion: "failure" } } }, true],
    ["missing detection verdict", { detection: { result: "success", outputs: {} } }, true],
  ];
  for (const name of Object.keys(success)) for (const result of ["failure", "cancelled", "skipped"]) {
    cases.push([`${name} ${result}`, { [name]: { ...success[name], result } }, true]);
  }
  for (const active of ["false", ""]) {
    cases.push([active ? "collector rejected event" : "trusted checkout gated out", {
      pre_activation: { result: "success", outputs: { active } },
      ...Object.fromEntries(["activation", "agent", "detection", "publish_regression_triage"]
        .map((name) => [name, { ...success[name], result: "skipped" }])),
    }, false]);
  }
  for (const [name, changes, fails] of cases) await t.test(name, async () => {
    if (name === "valid empty batch (staged)") {
      const s = setup([]);
      const published = await s.publish(await s.collect());
      assert.equal(published.incomplete, false);
      assert.ok(published.receipts.some((receipt) => receipt.type === "would-save-memory"));
      assert.deepEqual(s.mutations, []);
    }
    const needs = { ...clone(success), ...clone(changes) };
    for (const name of ["detection", "publish_regression_triage"]) {
      if (!evaluate(condition(job(lock, name)), needs)) needs[name].result = "skipped";
    }
    const failures = [];
    if (evaluate(condition(guard), needs)) require("node:vm").runInNewContext(script, {
      process: { env: { TRIAGE_COMPLETED: String(evaluate(completed, needs)) } },
      core: { setFailed: (message) => failures.push(message) },
    });
    assert.equal(failures.length, fails ? 1 : 0);
    if (fails) assert.match(failures[0], /required proposal.*publication/i);
  });
});

test("source and pinned generated workflow enforce independent triggers and one output route", () => {
  const root = path.resolve(__dirname, "..", "..", "workflows");
  const source = fs.readFileSync(path.join(root, "regression-triage.md"), "utf8");
  const lock = fs.readFileSync(path.join(root, "regression-triage.lock.yml"), "utf8");
  assert.deepEqual(require("./output-validation.json"), {
    [OUTPUT_TYPE]: { defaultMax: 1, fields: { proposals: { required: true, type: "string", sanitize: false } } },
  });
  for (const text of [source, lock]) {
    for (const type of ["opened", "edited", "reopened", "labeled", "transferred", "created", "deleted"]) {
      assert.match(text, new RegExp(`\\b${type}\\b`));
    }
    assert.match(text, /issue_comment:/);
    assert.match(text, /workflow_dispatch:/);
    assert.match(text, /cancel-in-progress: false/);
    assert.match(text, /regression-triage/);
    assert.doesNotMatch(text, /actions: write|pull-requests: write/);
  }
  assert.match(source, /roles: all/);
  assert.match(source, /bash: \[\]/);
  assert.match(source, /edit: false/);
  assert.match(source, /min-integrity: none/);
  assert.match(source, /shared\/model-defaults.md/);
  assert.doesNotMatch(source, /repo-memory:|add-labels:|add-comment:|create-issue:|reaction:|gh-proxy/);
  assert.match(lock, /v0\.76\.1/);
  assert.match(lock, /publish_regression_triage:/);
  assert.match(lock, /GH_AW_SAFE_OUTPUTS_STAGED/);
  assert.match(lock, /needs\.detection\.outputs/);
  assert.match(lock, /GH_AW_DETECTION_CONTINUE_ON_ERROR: "false"/);
  const viewName = source.match(/name: (regression-triage-dotnet-fsharp-[^\r\n]+-input)/)[1]
    .replace("${{ github.run_id }}", env.GITHUB_RUN_ID)
    .replace("${{ github.run_attempt }}", env.GITHUB_RUN_ATTEMPT)
    .replace("${{ github.workflow_sha }}", env.GITHUB_WORKFLOW_SHA);
  assert.equal(viewName, artifactPrefix(env) + "input");
  const collector = lock.slice(lock.indexOf("\n  pre_activation:"), lock.indexOf("\n  publish_regression_triage:"));
  assert.doesNotMatch(collector, /: write/);
  assert.match(collector, /ref: \$\{\{ github\.workflow_sha \}\}/);
  assert.match(collector, /github\.sha == github\.workflow_sha && !github\.event\.issue\.pull_request/);
  assert.match(collector, /if: steps\.trusted_helpers\.outcome == 'success'/);
  const agent = lock.slice(lock.indexOf("\n  agent:"), lock.indexOf("\n  conclusion:"));
  for (const text of [source, lock]) {
    assert.match(text, /GH_AW_VALIDATION_CONFIG_PATH: \$\{\{ github\.workspace \}\}\/\.github\/scripts\/regression-triage\/output-validation\.json/);
  }
  assert.match(agent, /name: Load immutable proposal validation/);
  assert.match(agent, /ref: \$\{\{ github\.workflow_sha \}\}/);
  assert.doesNotMatch(agent, /--allow-all-tools|--allow-tool shell|needs\.pre_activation|shell\(gh/);
  assert.match(agent, /exec \/tmp\/gh-aw\/copilot-original --deny-tool=write --deny-tool=shell --deny-tool=url --excluded-tools=task/);
  assert.doesNotMatch(agent, /--available-tools/);
  assert.match(agent, /--no-custom-instructions/);
  const restrict = agent.indexOf("- name: Enforce read-only classifier CLI");
  assert.ok(restrict > agent.indexOf("- name: Install GitHub Copilot CLI"));
  assert.ok(restrict < agent.indexOf("- name: Execute GitHub Copilot CLI"));
  assert.match(agent, /copilot_harness\.cjs \/usr\/local\/bin\/copilot /);
  assert.match(agent, /"GITHUB_READ_ONLY": "1"/);
  assert.match(agent, /github\(issue_read\)/);
  assert.match(agent, /github\(pull_request_read\)/);
  assert.doesNotMatch(agent, /add_labels|add_comment|create_issue|push_repo_memory|report_incomplete/);
  const safeConfig = JSON.parse(agent.match(/^\s*(\{"publish-regression-triage":.*\})\r?$/m)[1]);
  assert.deepEqual(Object.keys(safeConfig), ["publish-regression-triage"]);
  assert.deepEqual(Object.keys(safeConfig["publish-regression-triage"].inputs), ["proposals"]);
  assert.equal(safeConfig["publish-regression-triage"].output, ACKNOWLEDGEMENT);
  assert.match(safeConfig["publish-regression-triage"].inputs.proposals.description, /at most 64000 bytes/);
  assert.match(source, /A batch is at most 64000 UTF-8 bytes/);
  assert.doesNotMatch(source + lock, /65536/);
  const publisher = lock.slice(lock.indexOf("\n  publish_regression_triage:"), lock.indexOf("\n  regression_triage_completion:"));
  assert.match(publisher, /needs\.agent\.result == 'success'/);
  assert.match(publisher, /needs\.detection\.result == 'success'/);
  assert.match(publisher, /needs\.detection\.outputs\.detection_success == 'true'/);
  assert.match(publisher, /needs\.detection\.outputs\.detection_conclusion == 'success'/);
  assert.doesNotMatch(publisher, /needs\.pre_activation|needs\.activation/);
  assert.match(publisher, /ref: \$\{\{ github\.workflow_sha \}\}/);
  assert.match(publisher, /artifact-ids: \$\{\{ steps\.manifest\.outputs\.artifact-id \}\}/);
  const conclusion = lock.slice(lock.indexOf("\n  conclusion:"), lock.indexOf("\n  detection:"));
  assert.match(conclusion, /permissions: \{\}/);
  assert.doesNotMatch(conclusion, /github-token:.*GH_AW_GITHUB_TOKEN/);
  assert.match(conclusion, /GH_AW_FAILURE_REPORT_AS_ISSUE: "false"/);
  const detector = lock.slice(lock.indexOf("\n  detection:"), lock.indexOf("\n  pre_activation:"));
  assert.match(detector, /--deny-tool=write --deny-tool=shell --deny-tool=url --excluded-tools=task --no-custom-instructions/);
  assert.match(detector, /COPILOT_MODEL: \$\{\{ vars\.GH_AW_MODEL_DETECTION_COPILOT \|\| needs\.activation\.outputs\.model \}\}/);
  assert.doesNotMatch(detector, /--available-tools/);
});
