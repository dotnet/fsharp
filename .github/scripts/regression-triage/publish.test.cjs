"use strict";

const { test } = require("node:test");
const assert = require("node:assert/strict");
const { createHash } = require("node:crypto");
const { POLICY_VERSION, fingerprintHumanInput, normalizeMemory } = require("./core.cjs");
const { readIssueSnapshot, MEMORY_BRANCH, MEMORY_PATH } = require("./github.cjs");
const {
  validateProposals, publishBatch, createGitHubStore, OUTPUT_TYPE, ACKNOWLEDGEMENT, receiptMarker,
} = require("./publish.cjs");
const {
  repo, now, before, clone, failure, report, comment, fake, emptyMemory, collect, publishRun,
} = require("./test-support.cjs");

const oid = (n) => n.toString(16).padStart(40, "0");
const bot = { id: 99, login: "regression-triage[bot]" };
const context = (head = oid(1), runId = "123") => ({
  repository: "dotnet/fsharp", runId, runAttempt: 1, policyVersion: POLICY_VERSION,
  collectorRevision: oid(100), memoryHead: head,
});
const envelope = (results) => ({ items: [{ type: OUTPUT_TYPE,
  proposals: JSON.stringify({ schemaVersion: 1, policyVersion: POLICY_VERSION, results }) }] });
const proposal = (item, fields = {}) => ({
  number: item.number, fingerprint: item.fingerprint, classification: "regression",
  evidence: [{ sourceId: item.snapshot.bodySourceId, url: item.snapshot.url,
    quote: item.snapshot.body, dimension: "compiler" }],
  missingFact: null, clarification: null, ...fields,
});
const uncertain = { classification: "uncertain", missingFact: "An earlier working compiler.", clarification: "known-good" };

function casStore(state = emptyMemory(), head = oid(1), missing = null) {
  const store = {
    value: { state: clone(state), headOid: head, missing }, writes: [],
    async read() { return clone(store.value); },
    async commit({ expectedHeadOid, state }) {
      await store.beforeCommit?.(state);
      if (expectedHeadOid !== store.value.headOid) {
        throw Object.assign(new Error("Memory changed; recollect"), { code: "CAS_CONFLICT", retryable: true });
      }
      store.writes.push(clone(state));
      store.value = { state: clone(state), headOid: oid(store.writes.length + 1), missing: null };
      await store.afterCommit?.(state);
      return clone(store.value);
    },
  };
  return store;
}

function writableApi(options = {}) {
  const api = fake({ issues: [report()], pageSize: 100, ...options });
  api.github.rest.issues.addLabels = async (args) => {
    api.calls.push({ name: "addLabels", ...clone(args) });
    const issue = api.issues.find((item) => item.number === args.issue_number);
    issue.labels = [...new Set([...issue.labels, ...args.labels])];
    return { data: issue.labels.map((name) => ({ name })) };
  };
  api.github.rest.issues.createComment = async (args) => {
    api.calls.push({ name: "createComment", ...clone(args) });
    const comments = api.comments[args.issue_number] ??= [];
    const item = comment(1000 + comments.length, { body: args.body, user: { ...bot, type: "Bot" },
      html_url: `${report(args.issue_number).url}#issuecomment-${1000 + comments.length}` });
    comments.push(item);
    return { data: clone(item) };
  };
  return api;
}

async function setup(options = {}, state = emptyMemory()) {
  const api = writableApi(options);
  const store = casStore(state);
  const manifest = { ...await collect(api, state, { limits: undefined }), binding: context() };
  const output = envelope(manifest.selected.map((item) => proposal(item)));
  const args = { github: api.github, store, repo, manifest, output, context: context(), bot, now, env: {} };
  return { api, store, manifest, output, args };
}

const writes = (api, name) => api.calls.filter((call) => call.name === name);

test("one GH AW route acknowledges validation, not publication", () => {
  assert.equal(OUTPUT_TYPE, "publish_regression_triage");
  assert.equal(ACKNOWLEDGEMENT, "Proposal received for validation; publication is not confirmed.");
});

for (const issues of [[], [report()]]) test(`pinned GH AW envelope accepts empty ingestion errors (${issues.length} results)`, async () => {
  const { args, store } = await setup({ issues });
  const output = { ...args.output, errors: [] };
  assert.deepEqual(validateProposals(output, args.manifest), JSON.parse(output.items[0].proposals).results);
  for (const errors of [["Line 2: Unexpected output type"], null, {}, false, ""]) {
    await assert.rejects(publishBatch({ ...args, output: { ...output, errors } }));
  }
  await assert.rejects(publishBatch({ ...args, output: { ...output, stateDelta: {} } }));
  assert.equal(store.writes.length, 0);
  const staged = await publishBatch({ ...args, output, staged: true });
  assert.equal(staged.outcomes.length, issues.length);
  assert.ok(staged.outcomes.every((outcome) => outcome.status === "published"));
  assert.ok(staged.receipts.some((receipt) => receipt.type === "would-save-memory"));
});

test("positive reports without a keyword add exactly Regression once, then deduplicate", async () => {
  const { api, store, args } = await setup();
  const first = await publishBatch(args);
  assert.deepEqual(writes(api, "addLabels"), [{ name: "addLabels", ...repo, issue_number: 42, labels: ["Regression"] }]);
  assert.deepEqual(api.issues[0].labels, ["Needs-Triage", "Regression"]);
  assert.deepEqual(writes(api, "createComment"), []);
  const record = first.state.issues[42];
  assert.equal(record.classification, "regression");
  assert.equal(record.lastResult.status, "published");
  assert.equal(record.pendingPublication, null);
  assert.equal(record.evidence[0].quote, report().body);
  assert.doesNotMatch(JSON.stringify(record), /reproduced|verified/i);
  const commits = store.writes.length;
  await publishBatch(args);
  assert.equal(store.writes.length, commits);
  assert.equal(writes(api, "addLabels").length, 1);
  assert.ok(store.writes[0].issues[42].pendingPublication);
  assert.ok(store.writes[0].pending.some((entry) => entry.number === 42));
  assert.ok(first.state.issues[42].readAttempt.at);
  assert.deepEqual(first.state.pending, []);
});

for (const { name, input, expected } of require("./fixtures/classification.json").cases) {
  test(`frozen classification provenance and effects: ${name}`, async () => {
    const issues = [input, ...input.linked].map((item) => report(item.number, {
      ...item, html_url: item.url, ...(item.isPullRequest ? { pull_request: {} } : {}),
    }));
    const comments = Object.fromEntries([input, ...input.linked].map((item) => [item.number,
      item.humanComments.map((c) => comment(c.id, { body: c.body, html_url: c.url,
        user: { id: c.authorId, login: c.author, type: "User" },
        created_at: c.createdAt, updated_at: c.updatedAt }))]));
    const timeline = { [input.number]: input.humanDecisions.map((d) => ({
      id: d.id, event: d.event, label: { name: d.label }, actor: { id: d.actorId, login: d.actor, type: "User" },
      created_at: d.createdAt, url: d.url,
    })) };
    const { api, args } = await setup({ issues, comments, timeline });
    const selected = args.manifest.selected.find((item) => item.number === input.number);
    const result = proposal(selected, { classification: expected.classification,
      evidence: expected.evidence, missingFact: expected.missingFact });
    args.output = envelope([result]);
    assert.deepEqual(validateProposals(args.output, args.manifest), [result]);
    await publishBatch(args);
    assert.deepEqual(writes(api, "addLabels").flatMap((call) => call.labels), expected.allowedEffect.addLabels);
    assert.ok(api.issues[0].labels.includes("Needs-Triage"));
    if (input.labels.includes("Regression")) assert.ok(api.issues[0].labels.includes("Regression"));
  });
}

for (const [name, change] of [
  ...["labels", "comment", "close", "edit", "code", "secret", "dispatch", "repository", "branch", "path",
    "operations", "stateDelta", "permissions", "reproduced", "staged"].map((key) => [key, (r) => { r[key] = "hostile"; }]),
  ["unsafe number", (r) => { r.number = 9007199254740992; }],
  ["unselected", (r) => { r.number = 55; }],
  ["hash", (r) => { r.fingerprint = "0".repeat(64); }],
  ["classification", (r) => { r.classification = "verified"; }],
  ["no evidence", (r) => { r.evidence = []; }],
  ["missing fact", (r) => { r.classification = "uncertain"; }],
  ["arbitrary clarification", (r) => { Object.assign(r, uncertain, { clarification: "Run this script" }); }],
  ["non-string clarification", (r) => { Object.assign(r, uncertain, { clarification: ["known-good"] }); }],
  ["oversized quote", (r) => { r.evidence[0].quote = "x".repeat(1001); }],
  ["invented quote", (r) => { r.evidence[0].quote = "This was independently verified."; }],
  ["invented source", (r) => { r.evidence[0].sourceId = "dotnet/fsharp#42:comment:404"; }],
  ["invented URL", (r) => { r.evidence[0].url = "https://evil.invalid"; }],
  ["unsupported dimension", (r) => { r.evidence[0].dimension = "verified"; }],
  ["arbitrary correction", (r) => { r.correction = { sourceId: "fake", url: "fake", quote: "reject" }; }],
]) {
  test(`strict proposal rejects ${name} before any writes`, async () => {
    const { api, store, args } = await setup();
    const result = proposal(args.manifest.selected[0]);
    change(result);
    args.output = envelope([result]);
    await assert.rejects(publishBatch(args));
    assert.equal(store.writes.length, 0);
    assert.equal(writes(api, "addLabels").length + writes(api, "createComment").length, 0);
  });
}

for (const [name, output] of [
  ["missing", undefined], ["malformed", "{"], ["oversized", " ".repeat(131073)],
  ["unknown envelope field", { items: [], operations: [] }],
  ["no envelope", { items: [] }], ["wrong output", { items: [{ type: "add_labels", proposals: "{}" }] }],
  ["duplicate envelopes", { items: [{ type: OUTPUT_TYPE, proposals: "{}" }, { type: OUTPUT_TYPE, proposals: "{}" }] }],
  ["duplicate JSON keys", '{"items":[],"items":[]}'],
]) {
  test(`strict output rejects ${name}`, async () => {
    const { store, args } = await setup();
    await assert.rejects(publishBatch({ ...args, output }));
    assert.equal(store.writes.length, 0);
  });
}

test("duplicate results, wrong batch policy/schema and unknown batch fields fail", async () => {
  const { args } = await setup();
  const result = proposal(args.manifest.selected[0]);
  for (const batch of [
    { schemaVersion: 1, policyVersion: POLICY_VERSION, results: [result, result] },
    { schemaVersion: 2, policyVersion: POLICY_VERSION, results: [result] },
    { schemaVersion: 1, policyVersion: "other", results: [result] },
    { schemaVersion: 1, policyVersion: POLICY_VERSION, results: [result], runId: "hostile" },
  ]) {
    assert.throws(() => validateProposals({ items: [{ type: OUTPUT_TYPE, proposals: JSON.stringify(batch) }] }, args.manifest));
  }
});

for (const [name, suffix] of [["ASCII", "x"], ["Unicode", "\u00e9\u{1f600}"], ["escaped", '"\\\n']]) {
  test(`proposal UTF-8 byte limit matches HTTP transport (${name})`, async () => {
    const { args, store, api } = await setup({ issues: [report(42, { body: report().body + suffix })] });
    const output = envelope(args.manifest.selected.map((item) => proposal(item)));
    const item = output.items[0];
    item.proposals += " ".repeat(64000 - Buffer.byteLength(item.proposals));
    assert.equal(Buffer.byteLength(item.proposals), 64000);
    assert.equal(validateProposals(output, args.manifest).length, 1);
    item.proposals += " ";
    await assert.rejects(publishBatch({ ...args, output }), /JSON size/);
    assert.equal(store.writes.length, 0);
    assert.equal(writes(api, "addLabels").length + writes(api, "createComment").length, 0);
  });
}

for (const field of ["repository", "runId", "runAttempt", "policyVersion", "collectorRevision", "memoryHead"]) {
  test(`trusted artifact must match independent runtime ${field}`, async () => {
    const { store, args } = await setup();
    args.manifest.binding[field] = field === "runAttempt" ? 2 : "wrong";
    await assert.rejects(publishBatch(args));
    assert.equal(store.writes.length, 0);
  });
}

test("incomplete or altered trusted snapshots cannot authorize proposals", async () => {
  for (const change of [(s) => { s.complete = false; }, (s) => { s.body += "changed"; }]) {
    const { store, args } = await setup();
    change(args.manifest.selected[0].snapshot);
    await assert.rejects(publishBatch(args));
    assert.equal(store.writes.length, 0);
  }
});

const changes = {
  closed: (api) => { api.issues[0].state = "closed"; },
  "Needs-Triage removed": (api) => { api.issues[0].labels = []; },
  title: (api) => { api.issues[0].title += " corrected"; },
  body: (api) => { api.issues[0].body += " corrected"; },
  "issue identity": (api) => { api.issues[0].id = 123456; },
  "author provenance": (api) => { api.issues[0].user.type = "Bot"; },
  "comment corrected": (api) => { api.comments[42][0].body += " corrected"; },
  "comment deleted": (api) => { api.comments[42] = []; },
  "linked claim": (api) => { api.issues[1].body += " corrected"; },
  "human label applied": (api) => {
    api.issues[0].labels.push("Regression");
    api.timeline[42] = [{ id: 3, event: "labeled", label: { name: "Regression" },
      actor: { id: 20, type: "User" }, created_at: now }];
  },
  "human label removed": (api) => {
    api.timeline[42] = [{ id: 3, event: "unlabeled", label: { name: "Regression" },
      actor: { id: 20, type: "User" }, created_at: now }];
  },
};

for (const kind of ["label", "clarification"]) {
  for (const timing of ["before snapshot", "during linked reads"]) {
    for (const [name, change] of Object.entries(changes)) {
      test(`adjacent ${kind} recheck prevents stale ${name} ${timing}`, async () => {
        const { api, store, args } = await setup({
          issues: [report(42, { body: `${report().body} #43` }), report(43, { labels: [] })],
          comments: { 42: [comment(1)] },
        });
        if (kind === "clarification") args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
        let claimed = false;
        let changed = false;
        store.afterCommit = (state) => {
          claimed ||= state.issues[42].pendingPublication?.phase === "sending";
          if (timing === "before snapshot" && !changed && claimed) {
            changed = true;
            change(api);
          }
        };
        const get = api.github.rest.issues.get;
        api.github.rest.issues.get = (a) => {
          if (timing === "during linked reads" && !changed && claimed && a.issue_number === 43) {
            changed = true;
            change(api);
          }
          return get(a);
        };
        const result = await publishBatch(args);
        assert.equal(changed, true, "test reaches the durable claim just before final recheck");
        assert.equal(writes(api, "addLabels").length + writes(api, "createComment").length, 0);
        assert.ok(["stale", "retryable"].includes(result.state.issues[42].lastResult.status));
        assert.ok(result.state.pending.some((entry) => entry.number === 42));
      });
    }
  }
}

test("one templated AI-disclosed clarification over fingerprints and policies", async () => {
  const { api, store, args } = await setup();
  args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
  await publishBatch(args);
  const posted = writes(api, "createComment");
  assert.equal(posted.length, 1);
  assert.match(posted[0].body, /AI/);
  assert.ok(posted[0].body.includes(receiptMarker(repo, 42)));
  assert.ok(!posted[0].body.includes(uncertain.missingFact));
  api.comments[42] = [];
  api.issues[0].body += " A new detail.";
  store.value.state.policyVersion = "old-policy";
  store.value.state.issues[42].policyVersion = "old-policy";
  const binding = context(store.value.headOid, "124");
  args.context = binding;
  args.manifest = { ...await collect(api, normalizeMemory(store.value.state)), binding };
  args.output = envelope([proposal(args.manifest.selected[0], { ...uncertain, clarification: "affected-component" })]);
  const result = await publishBatch(args);
  assert.equal(writes(api, "createComment").length, 1);
  assert.equal(result.state.issues[42].clarification.status, "published");
  assert.equal(result.state.issues[42].missingFact, uncertain.missingFact);
});

test("clarification history cannot suppress an unrelated stable issue identity", async () => {
  const state = emptyMemory();
  state.issues[41] = { issueId: 123456, clarification: { status: "published", commentId: 9 } };
  const { api, args } = await setup({ issues: [report(42, { id: 654321 })] }, state);
  args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
  const result = await publishBatch(args);
  assert.equal(writes(api, "createComment").length, 1);
  assert.equal(result.state.issues[42].issueId, 654321);
  assert.deepEqual(result.state.issues[41].clarification, state.issues[41].clarification);
});

for (const history of ["receipt", "deleted receipt", "legacy receipt", "unresolved attempt"]) {
  test(`collector/publisher/restart: clarification survives transfer out and back with ${history}`, async () => {
    const { api, store, args } = await setup({ issues: [report(42, { id: 123456 })] });
    if (history === "unresolved attempt") api.github.rest.issues.createComment = async (request) => {
      api.calls.push({ name: "createComment", ...clone(request) });
      throw failure(503);
    };
    args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
    await publishBatch(args);
    if (history === "legacy receipt") delete store.value.state.issues[42].issueId;
    const original = clone(store.value.state.issues[42]);
    const get = api.github.rest.issues.get;
    api.github.rest.issues.get = async (request) => request.issue_number === 42
      ? { data: report(87, { id: 123456, html_url: "https://github.com/Other/Repo/issues/87" }) }
      : get(request);
    api.issues.length = 0;
    let binding = context(store.value.headOid, "130");
    args.manifest = { ...await collect(api, store.value.state), binding };
    args.context = binding;
    args.output = envelope([]);
    await publishBatch(args);
    api.issues.push(report(87, { id: 123456 }));
    api.comments[87] = history === "deleted receipt" ? [] : (api.comments[42] ?? []).map((item) => ({
      ...item, html_url: `https://github.com/dotnet/fsharp/issues/87#issuecomment-${item.id}`,
    }));
    for (let run = 0; run < 3; run++) {
      store.value.state = normalizeMemory(JSON.stringify(store.value.state));
      api.issues[0].body += " More detail.";
      binding = context(store.value.headOid, String(131 + run));
      args.manifest = { ...await collect(api, store.value.state), binding };
      args.context = binding;
      args.output = envelope([proposal(args.manifest.selected.find((item) => item.number === 87), uncertain)]);
      const result = await publishBatch(args);
      assert.equal(writes(api, "createComment").length, 1);
      assert.equal(writes(api, "addLabels").length, 0);
      assert.equal(result.state.issues[87].clarification.status, history === "unresolved attempt" ? "unknown" : "published");
      assert.equal(result.state.issues[87].issueId, 123456);
      assert.deepEqual(result.state.issues[42].clarification, original.clarification);
      if (history === "unresolved attempt") {
        assert.deepEqual(result.state.issues[87].clarification.pendingPublication, original.pendingPublication);
      }
      api.comments[87] = [];
    }
  });
}

for (const markerNumber of [42, 41, 40]) for (const [identity, user, authenticated] of [
  ["human forgery", { ...bot, login: "reporter", type: "User" }, false],
  ["other bot", { id: 777, login: "other[bot]", type: "Bot" }, false],
  ["right login wrong id", { ...bot, id: 777, type: "Bot" }, false],
  ["right id wrong login", { ...bot, login: "other[bot]", type: "Bot" }, false],
  ["right id and login wrong type", { ...bot, type: "User" }, false],
  ["authenticated bot", { ...bot, type: "Bot" }, true],
]) {
  test(`clarification receipts authenticate ${identity} with marker #${markerNumber}`, async () => {
    const state = emptyMemory();
    if (markerNumber !== 42) state.issues[41] = { clarification: { status: "published", commentId: 9 } };
    const { api, args } = await setup({ issues: [report(), report(markerNumber, { labels: [] })],
      comments: { 42: [comment(9, {
      body: receiptMarker(repo, markerNumber), user,
    })] } }, state);
    args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
    const result = await publishBatch(args);
    assert.equal(writes(api, "createComment").length, authenticated && markerNumber !== 40 ? 0 : 1);
    assert.equal(result.state.issues[42].clarification.status, "published");
  });
}

test("complete current human decisions veto re-add with no record, but later human application survives", async () => {
  const { api, store, args } = await setup({ timeline: { 42: [{
    id: 1, event: "unlabeled", label: { name: "Regression" }, actor: { id: 20, type: "User" }, created_at: before,
  }] } });
  await publishBatch(args);
  assert.equal(writes(api, "addLabels").length, 0);
  assert.equal(store.value.state.issues[42].humanLabelDecision.event, "unlabeled");
  api.issues[0].labels.push("Regression");
  api.timeline[42].push({ id: 2, event: "labeled", label: { name: "Regression" },
    actor: { id: 21, type: "User" }, created_at: now });
  const binding = context(store.value.headOid, "125");
  args.manifest = { ...await collect(api, store.value.state), binding };
  args.context = binding;
  args.output = envelope([proposal(args.manifest.selected[0], { ...uncertain, clarification: null })]);
  await publishBatch(args);
  assert.deepEqual(api.issues[0].labels, ["Needs-Triage", "Regression"]);
  assert.equal(store.value.state.issues[42].humanLabelDecision.event, "labeled");
});

test("empty batch commits whole discovery delta, missing output commits nothing", async () => {
  const { store, args } = await setup({ issues: Array.from({ length: 11 }, (_, i) => report(i + 1)) });
  await assert.rejects(publishBatch({ ...args, output: undefined }));
  assert.equal(store.writes.length, 0);
  const result = await publishBatch({ ...args, output: envelope([]) });
  assert.deepEqual(result.state.scan, args.manifest.stateDelta.scan);
  assert.deepEqual(result.state.pending, args.manifest.stateDelta.pending);
  assert.deepEqual(result.state.issues, args.manifest.stateDelta.issues);
  assert.equal(result.state.pending.length, 11);
});

test("real repeated collector-publication drains eleven default-budget reports and restart", async () => {
  const api = writableApi({ issues: Array.from({ length: 11 }, (_, i) => report(i + 1)) });
  const store = casStore();
  let memory = emptyMemory();
  for (let run = 0; run < 5; run++) {
    const cycle = await publishRun(api, memory, { limits: undefined,
      now: new Date(Date.parse(now) + run * 60000).toISOString() }, async (manifest) => {
      const binding = context(store.value.headOid, String(123 + run));
      const result = await publishBatch({ github: api.github, store, repo, context: binding, bot, now, env: {},
        manifest: { ...manifest, binding }, output: envelope(manifest.selected.map((item) => proposal(item))) });
      return result.state;
    });
    memory = cycle.memory;
    if (run === 2) assert.equal(writes(api, "addLabels").length, 11);
    if (run > 2) assert.deepEqual(cycle.result.selected, []);
  }
  assert.equal(writes(api, "addLabels").length, 11);
});

test("transient per-issue read failure keeps its queue and other successful outcomes", async () => {
  const { api, args } = await setup({ issues: [report(42), report(43)] });
  const get = api.github.rest.issues.get;
  api.github.rest.issues.get = (a) => a.issue_number === 42 ? Promise.reject(failure(503)) : get(a);
  const result = await publishBatch(args);
  assert.equal(result.state.issues[42].lastResult.status, "retryable");
  assert.equal(result.state.issues[43].lastResult.status, "published");
  assert.deepEqual(result.state.pending.map((entry) => entry.number), [42]);
  assert.ok(result.outcomes.some((item) => item.number === 42 && item.status === "retryable"));
});

for (const kind of ["label", "clarification"]) {
  for (const stage of ["comments", "timeline", "linked"]) {
    test(`known-unsent ${kind} claim recovers from incomplete ${stage} evidence`, async () => {
      const { api, store, args } = await setup({
        issues: [report(42, { body: `${report().body} #43` }), report(43, { labels: [] })],
      });
      if (kind === "clarification") args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
      const method = stage === "comments" ? "listComments" : stage === "timeline" ? "listEventsForTimeline" : "get";
      const read = api.github.rest.issues[method];
      let fail = false;
      store.afterCommit = (state) => { fail ||= state.issues[42].pendingPublication?.phase === "sending"; };
      api.github.rest.issues[method] = (a) =>
        fail && a.issue_number === (stage === "linked" ? 43 : 42) ? Promise.reject(failure(503)) : read(a);
      const retryable = await publishBatch(args);
      assert.equal(retryable.state.issues[42].lastResult.status, "retryable");
      assert.equal(retryable.state.issues[42].lastResult.detail.code, "snapshot-incomplete");
      assert.equal(retryable.state.issues[42].pendingPublication.phase, "prepared");
      assert.equal(retryable.state.issues[42].clarification, null);
      assert.ok(retryable.state.pending.some((entry) => entry.number === 42));
      assert.equal(writes(api, "addLabels").length + writes(api, "createComment").length, 0);
      api.github.rest.issues[method] = read;
      store.afterCommit = undefined;
      const recovered = await publishBatch(args);
      assert.equal(recovered.state.issues[42].lastResult.status, "published");
      assert.equal(recovered.state.issues[42].pendingPublication, null);
      assert.deepEqual(recovered.state.pending, []);
      await publishBatch(args);
      assert.equal(writes(api, kind === "label" ? "addLabels" : "createComment").length, 1);
    });
  }
  for (const point of ["prepared", "sending", "effect", "timeout applied", "timeout absent"]) {
    test(`${kind} crash/unknown recovery at ${point}`, async () => {
      const { api, store, args } = await setup();
      if (kind === "clarification") args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
      let crashed = false;
      const method = kind === "label" ? "addLabels" : "createComment";
      const mutate = api.github.rest.issues[method];
      if (point.startsWith("timeout")) {
        api.github.rest.issues[method] = async (a) => {
          if (point === "timeout applied") await mutate(a);
          else api.calls.push({ name: method, ...a });
          throw Object.assign(new Error("Unknown outcome"), { code: "ETIMEDOUT" });
        };
        await publishBatch(args);
      } else {
        store.afterCommit = (state) => {
          if (!crashed && state.issues[42].pendingPublication?.phase === point) {
            crashed = true;
            throw new Error("process crash");
          }
        };
        store.beforeCommit = (state) => {
          if (!crashed && point === "effect" && state.issues[42].lastResult?.status === "published") {
            crashed = true;
            throw new Error("process crash");
          }
        };
        await assert.rejects(publishBatch(args), /process crash/);
      }
      store.afterCommit = store.beforeCommit = undefined;
      const result = await publishBatch(args);
      assert.ok(writes(api, method).length <= 1);
      if (point === "sending" || point === "timeout absent") {
        assert.equal(result.state.issues[42].lastResult.status, "unknown");
        assert.ok(result.state.issues[42].pendingPublication);
      } else assert.equal(result.state.issues[42].lastResult.status, "published");
    });
  }
}

for (const observed of [false, true]) {
  for (const next of ["regression", "uncertain", "not-regression"]) {
    test(`old clarification (${observed ? "observed" : "unknown"}) cannot complete or block newer ${next}`, async () => {
      const { api, store, args } = await setup();
      args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
      if (observed) {
        store.beforeCommit = (state) => {
          if (state.issues[42].lastResult.status === "published") throw new Error("crash after comment");
        };
      } else {
        store.afterCommit = (state) => {
          if (state.issues[42].pendingPublication?.phase === "sending") throw new Error("crash before comment");
        };
      }
      await assert.rejects(publishBatch(args), /crash/);
      store.beforeCommit = store.afterCommit = undefined;
      const oldIntent = clone(store.value.state.issues[42].pendingPublication);
      api.issues[0].body += " The earlier compiler accepted the same source and settings.";
      store.value.state.issues[42].policyVersion = "old-policy";
      const binding = context(store.value.headOid, "131");
      args.context = binding;
      args.manifest = { ...await collect(api, store.value.state), binding };
      args.output = envelope([proposal(args.manifest.selected[0], next === "uncertain"
        ? { ...uncertain, clarification: "producer-consumer" } : { classification: next })]);
      if (next === "regression") {
        const get = api.github.rest.issues.get;
        store.afterCommit = (state) => {
          if (state.issues[42].pendingPublication?.phase === "sending") {
            api.github.rest.issues.get = async () => { throw failure(503); };
          }
        };
        const retryable = await publishBatch(args);
        assert.equal(retryable.state.issues[42].lastResult.status, "retryable");
        assert.equal(retryable.state.issues[42].clarification.status, observed ? "published" : "pending");
        if (!observed) assert.deepEqual(retryable.state.issues[42].clarification.pendingPublication, oldIntent);
        assert.equal(writes(api, "addLabels").length, 0);
        api.github.rest.issues.get = get;
        store.afterCommit = undefined;
      }
      const result = await publishBatch(args);
      const record = result.state.issues[42];
      assert.equal(record.lastResult.status, next === "regression" ? "published"
        : next === "uncertain" && !observed ? "unknown" : "noop");
      assert.notEqual(record.lastResult.operationId, oldIntent.operationId);
      assert.equal(record.fingerprint, args.manifest.selected[0].fingerprint);
      assert.equal(writes(api, "addLabels").length, next === "regression" ? 1 : 0);
      assert.equal(record.clarification.status, observed ? "published" : "pending");
      if (!observed) assert.deepEqual(record.clarification.pendingPublication, oldIntent);
      await publishBatch(args);
      assert.equal(writes(api, "createComment").length, observed ? 1 : 0);
      assert.equal(writes(api, "addLabels").length, next === "regression" ? 1 : 0);
    });
  }
}

for (const point of ["sending", "effect", "complete"]) {
  for (const change of ["fingerprint", "policy"]) {
    for (const next of ["regression", "uncertain", "not-regression"]) {
      test(`old label (${point}, changed ${change}) cannot complete or block newer ${next}`, async () => {
        const { api, store, args } = await setup();
        store.afterCommit = (state) => {
          if (point === "sending" && state.issues[42].pendingPublication?.phase === "sending") throw new Error("crash before label");
        };
        store.beforeCommit = (state) => {
          if (point === "effect" && state.issues[42].lastResult.status === "published") throw new Error("crash after label");
        };
        if (point === "complete") await publishBatch(args);
        else await assert.rejects(publishBatch(args), /crash/);
        store.beforeCommit = store.afterCommit = undefined;
        // A previous-policy operation was derived from that policy, not today's.
        if (change === "policy" && point !== "complete") {
          store.value.state.issues[42].pendingPublication.operationId = createHash("sha256")
            .update(JSON.stringify([args.context.repository, 42, "old-policy", store.value.state.issues[42].fingerprint])).digest("hex");
        }
        const oldIntent = clone(store.value.state.issues[42].pendingPublication);
        const oldFingerprint = store.value.state.issues[42].fingerprint;
        if (change === "fingerprint") api.issues[0].body += " The affected component is still unclear.";
        else store.value.state.issues[42].policyVersion = "old-policy";
        const binding = context(store.value.headOid, "132");
        args.context = binding;
        args.manifest = { ...await collect(api, store.value.state), binding };
        args.output = envelope([proposal(args.manifest.selected[0], next === "uncertain" ? uncertain : { classification: next })]);
        const result = await publishBatch(args);
        const record = result.state.issues[42];
        assert.equal(record.fingerprint === oldFingerprint, change === "policy");
        assert.equal(record.lastResult.status, next === "uncertain" || next === "regression" && point === "sending" ? "published" : "noop");
        if (oldIntent) assert.notEqual(record.lastResult.operationId, oldIntent.operationId);
        assert.equal(record.pendingPublication, null);
        assert.deepEqual(record.pendingLabelPublication ?? null,
          point === "sending" && next !== "regression" ? oldIntent : null);
        assert.equal(record.clarification?.status ?? null, next === "uncertain" ? "published" : null);
        assert.deepEqual(result.state.pending, []);
        await publishBatch(args);
        assert.equal(writes(api, "createComment").length, next === "uncertain" ? 1 : 0);
        assert.equal(writes(api, "addLabels").length, point !== "sending" || next === "regression" ? 1 : 0);
        assert.ok(api.issues[0].labels.includes("Needs-Triage"));
      });
    }
  }
}

for (const next of ["regression", "uncertain", "not-regression"]) {
  test(`a new manifest for the same input reconciles an unknown label independently of ${next}`, async () => {
    const { api, store, args } = await setup();
    store.afterCommit = (state) => {
      if (state.issues[42].pendingPublication?.phase === "sending") throw new Error("crash");
    };
    await assert.rejects(publishBatch(args), /crash/);
    store.afterCommit = undefined;
    const intent = clone(store.value.state.issues[42].pendingPublication);
    const binding = context(store.value.headOid, "133");
    args.context = binding;
    args.manifest = { ...await collect(api, store.value.state), binding };
    args.output = envelope([proposal(args.manifest.selected[0], next === "uncertain" ? uncertain : { classification: next })]);
    const result = await publishBatch(args);
    const record = result.state.issues[42];
    assert.deepEqual(next === "regression" ? record.pendingPublication : record.pendingLabelPublication, intent);
    assert.equal(record.lastResult.status, next === "regression" ? "unknown" : next === "uncertain" ? "published" : "noop");
    assert.deepEqual(result.state.pending.map((entry) => entry.number), next === "regression" ? [42] : []);
    assert.equal(writes(api, "createComment").length, next === "uncertain" ? 1 : 0);
    assert.equal(writes(api, "addLabels").length, 0);
    if (next === "regression") return;
    api.issues[0].labels.push("Regression");
    api.issues[0].body += " The affected component is still unclear.";
    const latest = context(store.value.headOid, "134");
    const manifest = { ...await collect(api, store.value.state), binding: latest };
    const recovered = await publishBatch({ ...args, context: latest, manifest,
      output: envelope([proposal(manifest.selected[0], next === "uncertain" ? uncertain : { classification: next })]) });
    assert.equal(recovered.state.issues[42].pendingLabelPublication, null);
    assert.equal(recovered.state.issues[42].lastResult.status, "noop");
    assert.equal(writes(api, "createComment").length, next === "uncertain" ? 1 : 0);
    assert.equal(writes(api, "addLabels").length, 0);
  });
}

test("the final target read hands a newly visible clarification receipt to the publisher", async () => {
  const { api, store, args } = await setup({
    issues: [report(42, { body: `${report().body} #43` }), report(43, { labels: [] })],
  });
  args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
  let claimed = false;
  let appeared = false;
  store.afterCommit = (state) => { claimed ||= state.issues[42].pendingPublication?.phase === "sending"; };
  const get = api.github.rest.issues.get;
  api.github.rest.issues.get = (a) => {
    if (claimed && !appeared && a.issue_number === 43) {
      appeared = true;
      api.comments[42] = [comment(9, { body: receiptMarker(repo, 42), user: { ...bot, type: "Bot" } })];
    }
    return get(a);
  };
  const result = await publishBatch(args);
  assert.equal(appeared, true);
  assert.deepEqual(result.state.issues[42].clarification,
    { status: "published", commentId: 9, url: `${report().url}#issuecomment-9` });
  assert.equal(result.state.issues[42].lastResult.status, "published");
  assert.equal(result.state.issues[42].lastResult.detail.code, "receipt-observed");
  assert.equal(result.state.issues[42].pendingPublication, null);
  assert.deepEqual(result.state.pending, []);
  await publishBatch(args);
  assert.equal(writes(api, "createComment").length + writes(api, "addLabels").length, 0);
});

for (const kind of ["label", "clarification"]) {
  for (const phase of ["prepared", "sending"]) {
    test(`two ${kind} publishers collide at ${phase}: only one mutation survives restart`, { timeout: 10000 }, async () => {
      const { api, store, args } = await setup();
      if (kind === "clarification") args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
      let unblock;
      let arrived;
      const waiting = new Promise((resolve) => { arrived = resolve; });
      const gate = new Promise((resolve) => { unblock = resolve; });
      let first = true;
      store.beforeCommit = async (state) => {
        if (first && state.issues[42].pendingPublication?.phase === phase) {
          first = false;
          arrived();
          await gate;
        }
      };
      const slower = publishBatch(args);
      await waiting;
      let faster;
      try { faster = await publishBatch(args); }
      finally { unblock(); }
      await assert.rejects(slower, { code: "CAS_CONFLICT", retryable: true });
      assert.deepEqual(store.value.state, faster.state);
      assert.equal(faster.state.issues[42].lastResult.status, "published");
      assert.equal(faster.state.issues[42].pendingPublication, null);
      assert.deepEqual(faster.state.pending, []);
      if (kind === "clarification") {
        assert.deepEqual(faster.state.issues[42].clarification,
          { status: "published", commentId: 1000, url: `${report().url}#issuecomment-1000` });
      }
      const restarted = casStore(normalizeMemory(JSON.stringify(store.value.state)), store.value.headOid);
      await publishBatch({ ...args, store: restarted });
      assert.equal(restarted.writes.length, 0);
      assert.equal(writes(api, "addLabels").length, kind === "label" ? 1 : 0);
      assert.equal(writes(api, "createComment").length, kind === "clarification" ? 1 : 0);
    });
  }
}

test("newer memory rejects an old whole-manifest delta rather than regressing cursor or queue", async () => {
  const { store, args } = await setup();
  store.value.headOid = oid(90);
  store.value.state.scan.updatedThrough = "2026-09-19T00:00:00Z";
  store.value.state.pending = [{ number: 88, firstSeenAt: now }];
  await assert.rejects(publishBatch(args), { code: "CAS_CONFLICT", retryable: true });
  assert.equal(store.writes.length, 0);
  assert.equal(store.value.state.pending[0].number, 88);
});

test("a resumed manifest cannot replace its accepted decisions", async () => {
  const { api, store, args } = await setup();
  store.afterCommit = () => { throw new Error("crash after intent"); };
  await assert.rejects(publishBatch(args), /crash/);
  store.afterCommit = undefined;
  args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
  await assert.rejects(publishBatch(args), /accepted|conflict/i);
  assert.equal(writes(api, "createComment").length, 0);
  assert.equal(writes(api, "addLabels").length, 0);
});

test("a known-unsent stale clarification does not strand the next current analysis", async () => {
  const { api, store, args } = await setup();
  args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
  let changed = false;
  store.afterCommit = (state) => {
    if (!changed && state.issues[42].pendingPublication?.phase === "sending") {
      changed = true;
      api.issues[0].body += " More detail.";
    }
  };
  const stale = await publishBatch(args);
  assert.equal(stale.state.issues[42].lastResult.status, "stale");
  const binding = context(store.value.headOid, "126");
  args.manifest = { ...await collect(api, store.value.state), binding };
  args.context = binding;
  args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
  const result = await publishBatch(args);
  assert.equal(result.state.issues[42].lastResult.status, "published");
  assert.equal(writes(api, "createComment").length, 1);
});

for (const kind of ["label", "clarification"]) {
  test(`closure after ${kind} records a partial stale outcome, never undoes the effect`, async () => {
    const { api, store, args } = await setup();
    if (kind === "clarification") args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
    const method = kind === "label" ? "addLabels" : "createComment";
    const original = api.github.rest.issues[method];
    api.github.rest.issues[method] = async (a) => {
      const response = await original(a);
      api.issues[0].state = "closed";
      return response;
    };
    const result = await publishBatch(args);
    assert.equal(result.state.issues[42].lastResult.status, "stale");
    assert.equal(result.state.issues[42].lastResult.detail.effectObserved, true);
    assert.equal(writes(api, method).length, 1);
    if (kind === "label") assert.ok(api.issues[0].labels.includes("Regression"));
    const binding = context(store.value.headOid, "127");
    api.issues[0].state = "open";
    args.manifest = { ...await collect(api, store.value.state), binding };
    args.context = binding;
    args.output = envelope([proposal(args.manifest.selected[0], kind === "label" ? {} : uncertain)]);
    await publishBatch(args);
    assert.equal(writes(api, method).length, 1);
  });
}

test("human rejecting correction and fair-read metadata survive migration and a new positive proposal", async () => {
  const correction = comment(1, { body: "Correction: the earlier compiler also failed." });
  const { api, store, args } = await setup({ comments: { 42: [correction] } });
  const source = args.manifest.selected[0].snapshot.humanComments[0];
  args.output = envelope([proposal(args.manifest.selected[0], { ...uncertain, clarification: null,
    correction: { sourceId: source.sourceId, url: source.url, quote: source.body } })]);
  await publishBatch(args);
  const prior = clone(store.value.state.issues[42]);
  store.value.state.issues[42].policyVersion = "old-policy";
  api.issues[0].body += " Another detail.";
  const binding = context(store.value.headOid, "128");
  args.manifest = { ...await collect(api, store.value.state), binding };
  args.context = binding;
  args.output = envelope([proposal(args.manifest.selected[0])]);
  await publishBatch(args);
  assert.equal(writes(api, "addLabels").length, 0);
  assert.deepEqual(store.value.state.issues[42].humanCorrection, prior.humanCorrection);
  assert.deepEqual(store.value.state.issues[42].readAttempt, prior.readAttempt);
  assert.equal(store.value.state.issues[42].lastResult.detail.code, "human-veto");
});

for (const origin of ["title", "body", "linked title", "linked body"]) {
  for (const [author, human] of [
    [{ id: 10, login: "reporter", type: "User" }, true],
    [{ id: 10, login: "triage[bot]", type: "Bot" }, false],
    [{ id: 10, login: "triage[bot]", type: "User" }, false],
    [{ id: 10, login: "unknown" }, false],
    [null, false],
  ]) test(`durable ${origin} corrections require human provenance: ${JSON.stringify(author)}`, async () => {
    const { api, store, args } = await setup({ issues: [
      report(42, { body: "Correction: see #43.", ...(origin.startsWith("linked") ? {} : { user: author }) }),
      report(43, { labels: [], user: author }),
    ] });
    const entry = args.manifest.selected[0];
    const snapshot = origin.startsWith("linked") ? entry.snapshot.linked[0] : entry.snapshot;
    const field = origin.endsWith("title") ? "title" : "body";
    const citation = { sourceId: snapshot[`${field}SourceId`], url: snapshot.url, quote: snapshot[field] };
    const decision = proposal(entry, { classification: "not-regression", evidence: [citation] });
    assert.deepEqual(validateProposals(envelope([decision]), args.manifest), [decision],
      "bot-authored reports remain readable evidence, not durable human vetoes");
    args.output = envelope([{ ...decision, correction: citation }]);
    if (human) {
      const result = await publishBatch(args);
      assert.equal(result.state.issues[42].humanCorrection.sourceId, citation.sourceId);
    } else {
      await assert.rejects(publishBatch(args), /human/i);
      assert.equal(store.writes.length, 0);
    }
    assert.equal(writes(api, "addLabels").length + writes(api, "createComment").length, 0);
  });
}

for (const loss of ["branch", "file", "stale state"]) {
  test(`lost memory (${loss}) reconciles authenticated receipts without another question`, async () => {
    const { api, args } = await setup();
    args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
    await publishBatch(args);
    const head = loss === "branch" ? null : oid(90);
    const store = casStore(emptyMemory(), head, loss === "stale state" ? null : loss);
    const binding = context(head, "129");
    args.store = store;
    args.manifest = { ...await collect(api), binding };
    args.context = binding;
    args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
    const result = await publishBatch(args);
    assert.equal(writes(api, "createComment").length, 1);
    assert.equal(result.state.issues[42].clarification.status, "published");
  });
}

for (const history of ["existing", "branch", "file", "legacy"]) {
  for (const createdAt of [before, now, "2026-09-18T18:00:01.000Z", null]) {
    test(`missing-ledger history is bounded: ${history}, created=${createdAt}`, async () => {
      const { api, store, args } = await setup({ issues: [] });
      if (history === "branch") store.value.headOid = null;
      if (["branch", "file"].includes(history)) store.value.missing = history;
      if (history === "legacy") store.value.state.clarificationHistoryUnknown = true;
      args.context = args.manifest.binding = context(store.value.headOid);
      await publishBatch(args);

      // Discovery after initialization does not imply creation after memory loss.
      api.issues.push(report(42, { created_at: createdAt }));
      const mayAsk = history === "existing" || Date.parse(createdAt) > Date.parse(now);
      for (let run = 0; run < 2; run++) {
        args.now = `2026-09-18T18:0${run + 1}:00.000Z`;
        args.context = context(store.value.headOid, String(130 + run));
        args.manifest = { ...await collect(api, store.value.state, { now: args.now }), binding: args.context };
        assert.equal(args.manifest.selected.length, run === 0 || !mayAsk ? 1 : 0);
        args.output = envelope(args.manifest.selected.map((item) => proposal(item, uncertain)));
        const result = await publishBatch(args);
        const record = result.state.issues[42];
        assert.equal(writes(api, "createComment").length, mayAsk ? 1 : 0);
        assert.equal(record.lastResult.status, mayAsk ? "published" : "unknown");
        assert.equal(record.clarification.status, mayAsk ? "published" : "unknown");
        assert.equal(record.missingFact, uncertain.missingFact);
        assert.equal(result.state.pending.some((entry) => entry.number === 42), !mayAsk);
        assert.deepEqual(api.issues[0].labels, ["Needs-Triage"]);
        if (history !== "existing") {
          assert.equal(result.state.clarificationHistoryUnknownThrough, now);
          assert.equal(result.state.clarificationHistoryUnknown, undefined);
        }
      }
    });
  }
}

test("legacy memory-absent noop rechecks unchanged input and recovers a live receipt", async () => {
  const { api, store, args } = await setup();
  store.value.state.clarificationHistoryUnknown = true;
  const item = args.manifest.selected[0];
  store.value.state.issues[42] = {
    fingerprint: item.fingerprint, policyVersion: POLICY_VERSION, classification: "uncertain",
    clarification: { status: "unknown", reason: "memory-absent" },
    lastResult: { status: "noop", operationId: "legacy", detail: { code: "no-mutation-needed" } },
  };
  for (const recovered of [false, true]) {
    if (recovered) api.comments[42] = [comment(90, {
      body: receiptMarker(repo, 42), user: { ...bot, type: "Bot" },
    })];
    args.context = context(store.value.headOid, recovered ? "133" : "132");
    args.manifest = { ...await collect(api, store.value.state), binding: args.context };
    assert.equal(args.manifest.selected.length, 1);
    args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
    const result = await publishBatch(args);
    assert.equal(result.state.issues[42].lastResult.status, recovered ? "noop" : "unknown");
    assert.equal(result.state.issues[42].clarification.status, recovered ? "published" : "unknown");
    assert.equal(result.state.pending.some((entry) => entry.number === 42), !recovered);
  }
  assert.equal(writes(api, "createComment").length, 0);
});

test("a recovered creation timestamp resolves missing-ledger uncertainty without changing human input", async () => {
  const state = { ...emptyMemory(), clarificationHistoryUnknownThrough: before };
  const { api, store, args } = await setup({ issues: [report(42, { created_at: null })] }, state);
  args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
  await publishBatch(args);
  assert.equal(writes(api, "createComment").length, 0);
  api.issues[0].created_at = now;
  args.context = context(store.value.headOid, "134");
  args.manifest = { ...await collect(api, store.value.state), binding: args.context };
  args.output = envelope([proposal(args.manifest.selected[0], uncertain)]);
  const result = await publishBatch(args);
  assert.equal(result.state.issues[42].lastResult.status, "published");
  assert.equal(writes(api, "createComment").length, 1);
});

for (const [field, value] of [
  ["clarificationHistoryUnknown", "true"],
  ["clarificationHistoryUnknownThrough", null],
  ["clarificationHistoryUnknownThrough", "invalid"],
]) {
  test(`invalid history boundary fails before writes: ${field}=${value}`, async () => {
    const { store, args } = await setup();
    store.value.state[field] = value;
    await assert.rejects(publishBatch(args), /history/i);
    assert.equal(store.writes.length, 0);
  });
}

test("bot receipt text cannot serve as human classification evidence", async () => {
  const { args, store } = await setup({ comments: { 42: [comment(9, {
    body: report().body, user: { ...bot, type: "Bot" },
  })] } });
  args.output = envelope([proposal(args.manifest.selected[0], {
    evidence: [{ sourceId: "dotnet/fsharp#42:comment:9", url: `${report().url}#issuecomment-9`, quote: report().body }],
  })]);
  await assert.rejects(publishBatch(args), /trusted evidence/);
  assert.equal(store.writes.length, 0);
});

for (const forcedBy of ["dispatch", "environment"]) {
  test(`staged ${forcedBy} uses the real adapter but never invokes a remote write`, async () => {
    const { api, args } = await setup({ issues: [report(42), report(43)] });
    const forbidden = async () => { assert.fail("staged remote mutation"); };
    api.github.rest.issues.addLabels = api.github.rest.issues.createComment = forbidden;
    api.github.rest.git = { createRef: forbidden };
    api.github.graphql = forbidden;
    api.github.rest.repos.getBranch = async () => ({ data: { commit: { sha: oid(1) } } });
    api.github.rest.repos.getContent = async () => ({ data: {
      type: "file", encoding: "base64", content: Buffer.from(JSON.stringify(emptyMemory())).toString("base64"),
    } });
    args.store = createGitHubStore(api.github, repo);
    args.output = envelope(args.manifest.selected.map((item) => proposal(item, item.number === 43 ? uncertain : {})));
    args.staged = forcedBy === "dispatch";
    args.env = forcedBy === "environment" ? { GH_AW_SAFE_OUTPUTS_STAGED: "true" } : {};
    const result = await publishBatch(args);
    for (const type of ["would-add-label", "would-comment", "would-save-memory"]) {
      assert.ok(result.receipts.some((receipt) => receipt.type === type));
    }
    assert.equal(result.state.issues[42].lastResult.status, "published");
    assert.equal(result.state.issues[43].clarification.status, "published");
  });
}

function memoryApi({ head = oid(1), state = emptyMemory(), missingFile = false } = {}) {
  const calls = [];
  const github = { rest: { repos: {
    async getBranch(a) {
      calls.push({ name: "getBranch", ...a });
      if (a.branch === MEMORY_BRANCH && head === null) throw failure(404);
      return { data: { commit: { sha: a.branch === MEMORY_BRANCH ? head : oid(100) } } };
    },
    async getContent(a) {
      calls.push({ name: "getContent", ...a });
      if (a.path === "") return { data: missingFile || head === null ? [] : [{ name: MEMORY_PATH }] };
      if (head === null || missingFile) throw failure(404);
      return { data: { type: "file", encoding: "base64",
        content: Buffer.from(typeof state === "string" ? state : JSON.stringify(state)).toString("base64") } };
    },
    async get(a) { calls.push({ name: "getRepo", ...a }); return { data: { default_branch: "main" } }; },
  }, git: {
    async createRef(a) { calls.push({ name: "createRef", ...a }); head = a.sha; return { data: {} }; },
  } },
  async graphql(query, { input }) {
    calls.push({ name: "graphql", query, input });
    assert.equal(input.expectedHeadOid, head);
    state = JSON.parse(Buffer.from(input.fileChanges.additions[0].contents, "base64").toString("utf8"));
    head = oid(2);
    return { createCommitOnBranch: { commit: { oid: head } } };
  } };
  return { github, calls };
}

for (const absence of ["none", "branch", "file"]) {
  test(`real CAS adapter reads immutable head and writes only literal state; absence=${absence}`, async () => {
    const api = memoryApi({ head: absence === "branch" ? null : oid(1), missingFile: absence === "file" });
    const store = createGitHubStore(api.github, repo);
    const read = await store.read();
    assert.equal(read.headOid, absence === "branch" ? null : oid(1));
    assert.equal(read.missing, absence === "none" ? null : absence);
    const saved = await store.commit({ expectedHeadOid: read.headOid, state: emptyMemory() });
    assert.equal(saved.headOid, oid(2));
    const call = api.calls.find((c) => c.name === "graphql");
    assert.match(call.query, /createCommitOnBranch/);
    assert.deepEqual(call.input.branch, { repositoryNameWithOwner: "dotnet/fsharp", branchName: MEMORY_BRANCH });
    assert.equal(call.input.expectedHeadOid, absence === "branch" ? oid(100) : oid(1));
    assert.deepEqual(Object.keys(call.input.fileChanges), ["additions"]);
    assert.deepEqual(call.input.fileChanges.additions.map((a) => a.path), [MEMORY_PATH]);
    assert.deepEqual(call.input.message, { headline: "Persist regression triage state" });
    if (absence === "branch") assert.deepEqual(api.calls.find((c) => c.name === "createRef"),
      { name: "createRef", ...repo, ref: `refs/heads/${MEMORY_BRANCH}`, sha: oid(100) });
    else assert.ok(api.calls.some((c) => c.name === "getContent" && c.ref === oid(1)));
  });
}

for (const kind of ["corrupt", "forbidden", "ambiguous missing file", "unsupported schema", "timeout"]) {
  test(`real store fails visibly on ${kind}`, async () => {
    const api = memoryApi({ state: kind === "corrupt" ? "{" : kind === "unsupported schema" ? { schemaVersion: 9 } : emptyMemory() });
    if (["forbidden", "ambiguous missing file", "timeout"].includes(kind)) {
      api.github.rest.repos.getContent = async () => { throw failure(kind === "forbidden" ? 403 : kind === "timeout" ? 503 : 404); };
    }

    await assert.rejects(createGitHubStore(api.github, repo).read());
    assert.equal(api.calls.filter((c) => c.name === "graphql" || c.name === "createRef").length, 0);
  });
}

test("adapter branch initialization collision rereads and returns a bounded retryable conflict", async () => {
  const api = memoryApi({ head: null });
  const create = api.github.rest.git.createRef;
  api.github.rest.git.createRef = async (a) => { await create(a); throw failure(422); };
  const store = createGitHubStore(api.github, repo);
  await assert.rejects(store.commit({ expectedHeadOid: null, state: emptyMemory() }), { code: "CAS_CONFLICT", retryable: true });
  assert.equal(api.calls.filter((c) => c.name === "graphql").length, 0);
  assert.equal(api.calls.filter((c) => c.name === "createRef").length, 1);
});

for (const changed of [false, true]) {
  test(`adapter GraphQL failure reloads without blind retransmission (changed=${changed})`, async () => {
    const api = memoryApi();
    const commit = api.github.graphql;
    let attempts = 0;
    api.github.graphql = async (...a) => {
      attempts++;
      if (changed) await commit(...a);
      throw failure(503);
    };
    const store = createGitHubStore(api.github, repo);
    await assert.rejects(store.commit({ expectedHeadOid: oid(1), state: emptyMemory() }),
      changed ? { code: "CAS_CONFLICT" } : { status: 503 });
    assert.ok(api.calls.some((c) => c.name === "getBranch"));
    assert.equal(attempts, 1);
  });
}

for (const [key, value] of [
  ["issueId", null], ["issueId", -1], ["issueId", "123456"], ["issueId", 9007199254740992],
  ["pendingPublication", "invalid"], ["clarification", []], ["humanCorrection", false],
  ["humanLabelDecision", "removed"], ["evidence", {}], ["classification", "verified"],
  ["clarification", {}], ["clarification", { status: "invented" }],
  ["clarification", { status: "published" }], ["clarification", { status: "published", commentId: -1 }],
  ["clarification", { status: "published", commentId: 9007199254740992 }],
  ["clarification", { status: "published", commentId: 9, url: "https://evil.invalid" }],
  ["clarification", { status: "pending" }],
  ["clarification", { status: "pending", selector: "arbitrary question" }],
  ["clarification", { status: "unknown" }],
  ["clarification", { status: "unknown", reason: false }],
  ["clarification", { status: "unknown", selector: "known-good", pendingPublication: {} }],
  ["pendingPublication", {}], ["pendingPublication", { operationId: 42, phase: "prepared" }],
  ["pendingPublication", { operationId: "a".repeat(64), phase: "finished" }],
  ["pendingPublication", { operationId: "a".repeat(64), phase: "sending" }],
  ["pendingPublication", { operationId: "a".repeat(64), phase: "sending", effect: "close" }],
  ["pendingLabelPublication", {}],
  ["pendingLabelPublication", { operationId: "a".repeat(64), phase: "prepared" }],
  ["pendingLabelPublication", { operationId: "a".repeat(64), phase: "sending", effect: "comment" }],
  ["pendingLabelPublication", { operationId: "", phase: "sending", effect: "label" }],
]) {
  test(`malformed persisted ${key}=${JSON.stringify(value)} is not reset or treated as successful`, async () => {
    const state = emptyMemory();
    state.issues[42] = { [key]: value };
    const api = memoryApi({ state });
    await assert.rejects(createGitHubStore(api.github, repo).read(), /record/);
    const run = await setup();
    run.store.value.state = state;
    await assert.rejects(publishBatch(run.args), /record/);
    assert.equal(run.store.writes.length, 0);
    assert.equal(writes(run.api, "addLabels").length + writes(run.api, "createComment").length, 0);
  });
}

test("equal issue and comment IDs across repositories retain distinct fingerprints and citations", async () => {
  const repositories = ["dotnet/fsharp", "dotnet/runtime", "example/fsharp"];
  const apis = new Map(repositories.map((repository) => {
    const url = `https://github.com/${repository}/issues/2`;
    return [repository, fake({ pageSize: 100, issues: [report(2, {
      labels: [], url, html_url: url, body: `Reported comparison from ${repository}.`,
    })], comments: { 2: [comment(7, {
      body: `Human comparison from ${repository}.`, html_url: `${url}#issuecomment-7`,
    })] } })];
  }));
  const api = apis.get("dotnet/fsharp");
  api.issues.push(report(1, { body: [
    "#2 DotNet/FSharp#2 dotnet/runtime#2 example/fsharp#2",
    "https://github.com/DotNet/Runtime/issues/2 https://github.com/Example/FSharp/issues/2",
  ].join(" ") }));
  const github = { rest: { issues: Object.fromEntries(
    Object.keys(api.github.rest.issues).map((method) => [method, (args) =>
      apis.get(`${args.owner}/${args.repo}`.toLowerCase()).github.rest.issues[method](args)]),
  ) } };
  let memory = emptyMemory();
  for (const [repository, linkedApi] of apis) {
    if (memory.issues[1]) linkedApi.comments[2][0].body += " Correction: the earlier compiler also failed.";
    const manifest = await collect({ github }, memory, { limits: undefined });
    assert.deepEqual(manifest.errors, []);
    assert.deepEqual(manifest.selected.map((item) => item.number), [1]);
    const item = manifest.selected[0];
    assert.notEqual(item.fingerprint, memory.issues[1]?.fingerprint, repository);
    const links = item.snapshot.linked;
    assert.equal(links.length, repositories.length);
    const evidence = [];
    for (const repository of repositories) {
      const linked = links.find((link) => link.bodySourceId === `${repository}#2:body`);
      assert.ok(linked);
      assert.equal(linked.url, `https://github.com/${repository}/issues/2`);
      assert.equal(linked.body, apis.get(repository).issues[0].body);
      assert.equal(linked.humanComments.length, 1);
      const source = linked.humanComments[0];
      assert.equal(source.sourceId, `${repository}#2:comment:7`);
      assert.equal(source.url, `${linked.url}#issuecomment-7`);
      assert.equal(source.body, apis.get(repository).comments[2][0].body);
      evidence.push({ sourceId: linked.bodySourceId, url: linked.url, quote: linked.body },
        { sourceId: source.sourceId, url: source.url, quote: source.body });
    }
    const result = proposal(item, { evidence });
    assert.deepEqual(validateProposals(envelope([result]), manifest), [result]);
    for (const field of ["url", "quote"]) {
      const swapped = clone(result);
      swapped.evidence[1][field] = evidence[3][field];
      assert.throws(() => validateProposals(envelope([swapped]), manifest));
    }
    memory = (await publishRun({ github }, memory, { limits: undefined })).memory;
    assert.deepEqual((await collect({ github }, memory, { limits: undefined })).selected, []);
  }
});

for (const kind of ["title", "comment", "linked body", "review", "review-comment"]) {
  test(`current ${kind} evidence is validated by API identity, URL and exact text`, async () => {
    const apiOptions = {
      issues: [report(42, { body: "Comparison in #43" }), report(43, { labels: [], pull_request: {},
        html_url: "https://github.com/dotnet/fsharp/pull/43" })],
      comments: { 42: [comment(1)] },
      reviews: { 43: [comment(2, { html_url: "https://github.com/dotnet/fsharp/pull/43#pullrequestreview-2" })] },
      reviewComments: { 43: [comment(3, { html_url: "https://github.com/dotnet/fsharp/pull/43#discussion_r3" })] },
    };
    const { api, args } = await setup(apiOptions);
    const { snapshot } = args.manifest.selected[0];
    const linked = snapshot.linked[0];
    const source = kind === "title" ? { sourceId: snapshot.titleSourceId, url: snapshot.url, body: snapshot.title }
      : kind === "linked body" ? { sourceId: linked.bodySourceId, url: linked.url, body: linked.body }
      : kind === "comment" ? snapshot.humanComments[0]
      : linked.humanComments.find((c) => c.sourceId.includes(`:${kind}:`));
    args.output = envelope([proposal(args.manifest.selected[0], {
      evidence: [{ sourceId: source.sourceId, url: source.url, quote: source.body }],
    })]);
    await publishBatch(args);
    assert.equal(writes(api, "addLabels").length, 1);
  });
}

test("staged absent-branch initialization is local only, including restart", async () => {
  const { api, args } = await setup();
  const memory = memoryApi({ head: null });
  const forbidden = async () => { assert.fail("staged write"); };
  memory.github.rest.git.createRef = memory.github.graphql = forbidden;
  api.github.rest.issues.addLabels = api.github.rest.issues.createComment = forbidden;
  args.store = createGitHubStore(memory.github, repo);
  args.context = context(null);
  args.manifest.binding = args.context;
  args.staged = true;
  const result = await publishBatch(args);
  assert.ok(result.receipts.some((r) => r.type === "would-add-label"));
  const local = casStore(result.state, result.headOid);
  local.commit = forbidden;
  const restarted = await publishBatch({ ...args, store: local });
  assert.deepEqual(restarted.receipts, []);
  assert.deepEqual(restarted.state, result.state);
});

test("actual GH AW staged environment cannot be disabled through trusted option defaults", async () => {
  const { api, store, args } = await setup();
  const previous = process.env.GH_AW_SAFE_OUTPUTS_STAGED;
  process.env.GH_AW_SAFE_OUTPUTS_STAGED = "true";
  try {
    const result = await publishBatch({ ...args, staged: false, env: {} });
    assert.ok(result.receipts.some((r) => r.type === "would-add-label"));
    assert.equal(writes(api, "addLabels").length, 0);
    assert.equal(store.writes.length, 0);
  } finally {
    if (previous === undefined) delete process.env.GH_AW_SAFE_OUTPUTS_STAGED;
    else process.env.GH_AW_SAFE_OUTPUTS_STAGED = previous;
  }
});

for (const kind of ["missing acknowledgement", "forbidden initialization", "422 without a branch"]) {
  test(`real store reports ${kind} without proceeding`, async () => {
    const api = memoryApi({ head: kind === "missing acknowledgement" ? oid(1) : null });
    if (kind === "missing acknowledgement") api.github.graphql = async () => ({});
    else api.github.rest.git.createRef = async () => { throw failure(kind === "forbidden initialization" ? 403 : 422); };
    await assert.rejects(createGitHubStore(api.github, repo).commit({
      expectedHeadOid: kind === "missing acknowledgement" ? oid(1) : null, state: emptyMemory(),
    }));
  });
}

for (const [name, rootAlias, canonical, number, pull] of [
  ["root alias", true, "DotNet/FSharp", 43, true],
  ["linked rename", false, "Example/Renamed-Compiler", 43, true],
  ["linked transfer", false, "Example/Transferred-Compiler", 87, false],
]) test(`repository redirect: ${name} retains canonical provenance through staged reanalysis`, async () => {
  const alias = "Legacy/Compiler";
  const readRepo = rootAlias ? { owner: "Legacy", repo: "Compiler" } : repo;
  const url = `https://github.com/${canonical}/${pull ? "pull" : "issues"}/${number}`;
  const reviews = { [number]: [comment(2, { html_url: `${url}#pullrequestreview-2` })] };
  const reviewComments = { [number]: [comment(3, { html_url: `${url}#discussion_r3` })] };
  const api = writableApi({ issues: [
    report(42, { body: `${report().body} ` + (rootAlias
      ? `#43 ${alias}#42 DotNet/FSharp#42` : `${alias}#43 ${canonical}#${number}`) }),
    report(number, { labels: [], html_url: url, ...(pull ? { pull_request: {} } : {}) }),
  ], comments: { 42: [comment(1)], [number]: [comment(1, { html_url: `${url}#issuecomment-1` })] },
  reviews, reviewComments });
  for (const area of ["issues", "pulls"]) {
    for (const [method, read] of Object.entries(api.github.rest[area])) {
      api.github.rest[area][method] = (args) => {
        const repository = `${args.owner}/${args.repo}`.toLowerCase();
        const requested = args.issue_number ?? args.pull_number;
        if (method === "listForRepo") assert.deepEqual([args.owner, args.repo], [readRepo.owner, readRepo.repo]);
        else {
          const root = requested === 42 && (repository === "dotnet/fsharp" || rootAlias && repository === alias.toLowerCase());
          const linked = repository === canonical.toLowerCase() && requested === number
            || repository === alias.toLowerCase() && requested === 43;
          assert.ok(root || linked, `Unexpected read: ${repository}#${requested}`);
          if (rootAlias && linked) assert.equal(repository, "dotnet/fsharp", "local references use the canonical root");
          if (method !== "get") assert.equal(repository, root ? "dotnet/fsharp" : canonical.toLowerCase());
          if (method === "get" && linked) args = { ...args, issue_number: number };
        }
        return read(args);
      };
    }
  }
  let memory = emptyMemory();
  let fingerprint;
  for (const corrected of [false, true]) {
    if (corrected) (pull ? reviewComments[number][0] : api.comments[number][0]).body += " Correction: A also failed.";
    const manifest = { ...await collect(api, memory, { repo: readRepo, limits: undefined }), binding: context() };
    assert.deepEqual(manifest.errors, []);
    assert.equal(manifest.selected.length, 1);
    const item = manifest.selected[0];
    assert.notEqual(item.fingerprint, fingerprint);
    fingerprint = item.fingerprint;
    assert.equal(item.snapshot.linked.length, 1, "alias and canonical references identify one source");
    const linked = item.snapshot.linked[0];
    assert.equal(linked.number, number);
    assert.equal(linked.bodySourceId, `${canonical.toLowerCase()}#${number}:body`);
    assert.equal(linked.url, url);
    assert.equal(linked.humanComments.length, pull ? 3 : 1);
    const evidence = [item.snapshot, linked].flatMap((source) => [
      { sourceId: source.titleSourceId, url: source.url, quote: source.title },
      { sourceId: source.bodySourceId, url: source.url, quote: source.body },
      ...source.humanComments.map((c) => ({ sourceId: c.sourceId, url: c.url, quote: c.body })),
    ]);
    const result = proposal(item, { evidence });
    const output = envelope([result]);
    assert.deepEqual(validateProposals(output, manifest), [result]);
    for (const field of ["sourceId", "url"]) {
      const forged = clone(result);
      forged.evidence[0][field] = field === "sourceId" ? `${alias.toLowerCase()}#42:title`
        : "https://github.com/Unrelated/Compiler/issues/42";
      assert.throws(() => validateProposals(envelope([forged]), manifest));
    }
    const store = casStore(memory);
    const published = await publishBatch({ github: api.github, store, repo, manifest, output,
      context: context(), bot, now, env: {}, staged: true });
    assert.ok(published.receipts.some((receipt) => receipt.type === "would-add-label"));
    assert.equal(store.writes.length, 0);
    assert.equal(writes(api, "addLabels").length + writes(api, "createComment").length, 0);
    memory = normalizeMemory(JSON.stringify(published.state));
    assert.deepEqual((await collect(api, memory, { repo: readRepo, limits: undefined })).selected, []);
  }
});

for (const [name, change] of [
  ["conflicting evidence", (issue) => { issue.body += " Correction: A also failed."; }],
  ["changed label", (issue) => { issue.labels = []; }],
]) test(`repository redirect self-alias recheck rejects ${name}`, async () => {
  const { api, args, store } = await setup({ issues: [report(42, { body: `${report().body} Legacy/Compiler#42` })] });
  const get = api.github.rest.issues.get;
  let changed = false;
  api.github.rest.issues.get = (params) => {
    if (!changed && params.owner === "Legacy") { changed = true; change(api.issues[0]); }
    return get(params);
  };
  const result = await publishBatch({ ...args, staged: true });
  assert.equal(result.outcomes[0].status, "retryable");
  assert.ok(result.receipts.every((receipt) => receipt.type === "would-save-memory"));
  assert.equal(store.writes.length, 0);
  assert.equal(writes(api, "addLabels").length + writes(api, "createComment").length, 0);
});

for (const [name, fields] of [
  ["out-of-repository root", { html_url: "https://github.com/Unrelated/Compiler/issues/42" }],
  ["renumbered root", { number: 87, html_url: "https://github.com/dotnet/fsharp/issues/87" }],
  ["unrelated host", { html_url: "https://evil.invalid/dotnet/fsharp/issues/42" }],
  ["inconsistent API number", { html_url: "https://github.com/dotnet/fsharp/issues/87" }],
]) test(`repository redirect rejects ${name} without publication`, async () => {
  const api = writableApi();
  const get = api.github.rest.issues.get;
  api.github.rest.issues.get = async (args) => ({ data: { ...(await get(args)).data, ...fields } });
  const manifest = { ...await collect(api), binding: context() };
  assert.equal(manifest.selected.length, 0);
  const store = casStore();
  const args = { github: api.github, store, repo, manifest, context: context(), bot, now, env: {}, staged: true };
  assert.equal(manifest.incomplete.length, 1);
  const result = await publishBatch({ ...args, output: envelope([]) });
  assert.ok(result.receipts.every((receipt) => receipt.type === "would-save-memory"));
  assert.equal(store.writes.length, 0);
  assert.equal(writes(api, "addLabels").length + writes(api, "createComment").length, 0);
});

test("publisher independently rejects an out-of-repository selected root before saving progress", async () => {
  const { api, args, store } = await setup();
  const item = args.manifest.selected[0];
  Object.assign(item.snapshot, {
    url: "https://github.com/other/repo/issues/42",
    titleSourceId: "other/repo#42:title", bodySourceId: "other/repo#42:body",
  });
  item.fingerprint = fingerprintHumanInput(item.snapshot);
  args.output = envelope([proposal(item)]);
  await assert.rejects(publishBatch(args), /Wrong selected repository/);
  assert.equal(store.writes.length, 0);
  assert.equal(writes(api, "addLabels").length + writes(api, "createComment").length, 0);
});

test("linked canonical API URLs retain repository casing while source IDs are normalized", async () => {
  const url = "https://github.com/fsharp/FSharp.Compiler.Tools/issues/43";
  const { api, args } = await setup({ issues: [
    report(42, { body: `Version comparison: ${url}` }),
    report(43, { labels: [], html_url: url }),
  ] });
  const item = args.manifest.selected[0];
  const source = item.snapshot.linked[0];
  args.output = envelope([proposal(item, {
    evidence: [{ sourceId: source.bodySourceId, url: source.url, quote: source.body }],
  })]);
  await publishBatch(args);
  assert.equal(writes(api, "addLabels").length, 1);
});

for (const classification of ["regression", "not-regression", "uncertain"]) {
  test(`${classification} preserves existing human Regression and Needs-Triage`, async () => {
    const state = emptyMemory();
    state.futureField = { retained: true };
    state.issues[42] = { futureField: { retained: true } };
    const { api, args } = await setup({ issues: [report(42, { labels: ["Needs-Triage", "Regression"] })] }, state);
    args.output = envelope([proposal(args.manifest.selected[0], {
      classification, missingFact: classification === "uncertain" ? "A known-good version." : null,
    })]);
    const result = await publishBatch(args);
    assert.deepEqual(api.issues[0].labels, ["Needs-Triage", "Regression"]);
    assert.equal(writes(api, "addLabels").length + writes(api, "createComment").length, 0);
    assert.deepEqual(result.state.futureField, state.futureField);
    assert.deepEqual(result.state.issues[42].futureField, state.issues[42].futureField);
  });
}

for (const size of [1048576, 1048577]) {
  test(`durable file bound is exactly one MiB, not a rounded base64 estimate (${size})`, async () => {
    const state = { ...emptyMemory(), futureField: "" };
    state.futureField = "x".repeat(size - Buffer.byteLength(JSON.stringify(state) + "\n"));
    const api = memoryApi();
    const commit = createGitHubStore(api.github, repo).commit({ expectedHeadOid: oid(1), state });
    if (size === 1048576) await commit;
    else {
      await assert.rejects(commit, /bound/);
      assert.equal(api.calls.length, 0);
    }
  });
}
