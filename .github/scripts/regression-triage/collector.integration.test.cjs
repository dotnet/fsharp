"use strict";

const { test } = require("node:test");
const assert = require("node:assert/strict");
const { POLICY_VERSION, LIMITS, normalizeMemory, fingerprintHumanInput } = require("./core.cjs");
const { collectCandidates, readIssueSnapshot } = require("./github.cjs");
const { OUTPUT_TYPE, publishBatch } = require("./publish.cjs");
const { repo, now, before, clone, report, comment, fake, emptyMemory } = require("./test-support.cjs");

// Deterministic proposals test collector/publication mechanics, not model judgment.
function stagedCollector({ memory = emptyMemory(), ...options }) {
  const api = fake({ pageSize: 100, ...options });
  let state = memory;
  let headOid = "b".repeat(40);
  let run = 0;
  const mutations = [];
  const deny = async () => { mutations.push("write"); throw new Error("Staged write leaked"); };
  const store = {
    read: async () => ({ state: clone(state), headOid, missing: null }),
    commit: deny,
  };
  api.github.rest.issues.addLabels = deny;
  api.github.rest.issues.createComment = deny;
  api.github.rest.git = { createRef: deny };
  api.github.graphql = deny;
  const collect = async () => {
    const time = new Date(Date.parse(now) + run++ * 60000).toISOString();
    const binding = { repository: "dotnet/fsharp", runId: String(run), runAttempt: 1,
      policyVersion: POLICY_VERSION, collectorRevision: "a".repeat(40), memoryHead: headOid };
    const manifest = { ...await collectCandidates(api.github, { repo, memory: state, now: time }), binding };
    const output = { items: [{ type: OUTPUT_TYPE, proposals: JSON.stringify({
      schemaVersion: 1, policyVersion: POLICY_VERSION, results: manifest.selected.map((item) => ({
        number: item.number, fingerprint: item.fingerprint, classification: "regression",
        evidence: [{ sourceId: item.snapshot.bodySourceId, url: item.snapshot.url, quote: item.snapshot.body }],
        missingFact: null, clarification: null,
      })),
    }) }] };
    return { github: api.github, store, repo, manifest, output, context: binding,
      bot: { id: 99, login: "regression-triage[bot]" }, now: time, staged: true, env: {} };
  };
  return { api, mutations, collect, restart: (result) => {
    state = normalizeMemory(JSON.stringify(result.state));
    headOid = run.toString(16).padStart(40, "0");
  } };
}

for (const number of [1, 87]) for (const timing of ["before collection", "before publication"]) {
  test(`collector/publisher/restart: root transfer to Other/Repo#${number} ${timing} cannot block valid reports`, async () => {
    const memory = emptyMemory();
    memory.pending = [{ number: 1, firstSeenAt: before, historical: true }];
    memory.issues[1] = { clarification: { status: "published", commentId: 7,
      url: "https://github.com/dotnet/fsharp/issues/1#issuecomment-7" } };
    const s = stagedCollector({ memory, issues: [report(2)] });
    let transferred = timing === "before collection";
    const get = s.api.github.rest.issues.get;
    s.api.github.rest.issues.get = async (args) => {
      if (args.issue_number !== 1) return get(args);
      s.api.calls.push({ name: "get", ...clone(args) });
      return { data: transferred ? report(number, { html_url: `https://github.com/Other/Repo/issues/${number}` })
        : report(1) };
    };
    for (let run = 0; run < 3; run++) {
      if (run === 1) s.api.issues.push(report(3));
      const args = await s.collect();
      transferred = true;
      const result = await publishBatch(args);
      assert.deepEqual(result.receipts.filter((receipt) => receipt.type === "would-add-label")
        .map((receipt) => receipt.number), run < 2 ? [run + 2] : []);
      assert.ok(result.receipts.some((receipt) => receipt.type === "would-save-memory"));
      assert.ok(result.state.pending.some((entry) => entry.number === 1));
      assert.equal(result.state.issues[1].readAttempt.at, args.now);
      assert.deepEqual(result.state.issues[1].clarification, memory.issues[1].clarification);
      if (run === 0 && timing === "before publication") {
        assert.equal(result.outcomes.find((item) => item.number === 1).status, "retryable");
      } else {
        assert.ok(!args.manifest.selected.some((item) => item.number === 1));
        assert.deepEqual(args.manifest.incomplete.map((item) => item.number), [1]);
        assert.ok(args.manifest.errors.some((error) => error.stage === "snapshot" && error.number === 1));
      }
      s.restart(result);
    }
    assert.ok(s.api.calls.every((call) => call.owner === repo.owner && call.repo === repo.repo),
      "out-of-scope roots must not initiate foreign discussion reads");
    assert.deepEqual(s.mutations, []);
  });
}

for (const historicalCount of [6, 11]) for (const outcome of ["unknown", "stale", "omitted"]) {
  test(`collector/publisher/restart: ${outcome} historical work cannot starve later reports (${historicalCount} unresolved)`, async () => {
    const memory = emptyMemory();
    memory.clarificationHistoryUnknownThrough = now;
    const historical = Array.from({ length: historicalCount }, (_, i) => i + 1);
    const later = [1, 2, 3].map((offset) => historicalCount + offset);
    const s = stagedCollector({ memory, issues: historical.map((number) => report(number)) });
    const seen = [];
    const retried = new Set();
    for (let run = 0; run < 2 * (historicalCount + later.length); run++) {
      if (run === 2) s.api.issues.push(...later.map((number) => report(number)));
      s.api.calls.length = 0;
      const args = await s.collect();
      assert.deepEqual(args.manifest.errors, []);
      assert.ok(args.manifest.selected.length <= LIMITS.candidates);
      assert.ok(s.api.calls.filter((call) => call.name === "get").length <= 2 * LIMITS.snapshotReads);
      const batch = JSON.parse(args.output.items[0].proposals);
      for (const proposal of batch.results.filter((item) => item.number <= historicalCount)) {
        if (outcome === "unknown") Object.assign(proposal, {
          classification: "uncertain", evidence: [], missingFact: "Which earlier version worked?",
          clarification: "known-good",
        });
        if (outcome === "stale") s.api.issues[proposal.number - 1].body += " Correction.";
        if (run > historicalCount + 1) retried.add(proposal.number);
      }
      if (outcome === "omitted") batch.results = batch.results.filter((item) => item.number > historicalCount);
      args.output.items[0].proposals = JSON.stringify(batch);
      const result = await publishBatch(args);
      for (const item of result.outcomes.filter((item) => item.number <= historicalCount)) assert.equal(item.status, outcome);
      seen.push(...result.receipts.filter((receipt) => receipt.type === "would-add-label").map((receipt) => receipt.number));
      assert.ok(!result.receipts.some((receipt) => receipt.type === "would-comment"));
      assert.ok(historical.every((number) => result.state.pending.some((item) => item.number === number)));
      s.restart(result);
    }
    assert.deepEqual([...seen].sort((a, b) => a - b), later);
    assert.deepEqual([...retried].sort((a, b) => a - b), historical);
    assert.deepEqual(s.mutations, []);
  });
}

for (const hot of [false, true]) {
  test(`collector/publisher/restart: ${hot ? "hot parent and linked changes cannot starve unprocessed reports" : "eleven stable reports drain"}`, async () => {
    const reports = Array.from({ length: hot ? 12 : 11 }, (_, i) => report(i + 1, {
      body: `${report().body}${hot && i >= 9 ? " See #99." : ""}`,
      updated_at: hot && i < 9 ? now : before,
    }));
    const s = stagedCollector({ issues: [...reports, ...(hot ? [report(99, { labels: [] })] : [])] });
    const seen = new Set();
    for (let run = 0; run < (hot ? 15 : 5); run++) {
      if (hot && run > 0) {
        for (const issue of reports.slice(0, LIMITS.candidates - 1)) {
          issue.body += " More evidence.";
          issue.updated_at = new Date(Date.parse(now) + run * 60000).toISOString();
        }
        s.api.issues.at(-1).body += " More linked evidence.";
      }
      s.api.calls.length = 0;
      const args = await s.collect();
      assert.deepEqual(args.manifest.errors, []);
      assert.ok(args.manifest.selected.length <= LIMITS.candidates);
      assert.ok(s.api.calls.filter((call) => call.name === "get" && call.issue_number !== 99)
        .length <= 2 * LIMITS.snapshotReads);
      const result = await publishBatch(args);
      assert.ok(result.receipts.some((receipt) => receipt.type === "would-save-memory"));
      for (const receipt of result.receipts.filter((receipt) => receipt.type === "would-add-label")) {
        if (!hot || receipt.number >= 5 && receipt.number <= 9) assert.ok(!seen.has(receipt.number), "duplicate effect");
        seen.add(receipt.number);
      }
      s.restart(result);
      if (!hot && run >= 2) assert.equal(seen.size, 11);
      if (!hot && run >= 3) assert.deepEqual(args.manifest.selected, []);
    }
    assert.deepEqual([...seen].sort((a, b) => a - b), reports.map((issue) => issue.number));
    assert.deepEqual(s.mutations, []);
  });
}

for (const changedNumber of [1, 2]) {
  test(`collector/publisher/restart: timestamp-only churn on ${changedNumber === 1 ? "target" : "linked issue"} does not loop`, async () => {
    const s = stagedCollector({ issues: [
      report(1, { body: `${report().body} See #2.` }), report(2, { labels: [] }),
    ] });
    const initial = await s.collect();
    const fingerprint = initial.manifest.selected[0].fingerprint;
    const published = await publishBatch(initial);
    assert.equal(published.receipts.filter((receipt) => receipt.type === "would-add-label").length, 1);
    s.restart(published);
    for (let run = 1; run <= 3; run++) {
      s.api.issues[changedNumber - 1].updated_at = new Date(Date.parse(now) + run * 60000).toISOString();
      s.api.calls.length = 0;
      const args = await s.collect();
      assert.deepEqual(args.manifest.errors, []);
      assert.deepEqual(args.manifest.selected, []);
      assert.deepEqual(args.manifest.stateDelta.pending, []);
      assert.ok(s.api.calls.some((call) => call.name === "get" && call.issue_number === changedNumber));
      const snapshot = await readIssueSnapshot(s.api.github, { repo, number: 1 });
      assert.equal(snapshot.complete, true);
      assert.equal(fingerprintHumanInput(snapshot), fingerprint);
      const result = await publishBatch(args);
      assert.ok(result.receipts.every((receipt) => receipt.type === "would-save-memory"));
      assert.deepEqual(result.state.pending, []);
      assert.equal(result.state.issues[1].fingerprint, fingerprint);
      s.restart(result);
    }
    assert.deepEqual(s.mutations, []);
  });
}

for (const reference of ["dotnet/fsharp#2", "https://www.github.com/dotnet/fsharp/pull/2"]) {
  test(`collector/publisher/restart: linked-only correction rejects stale proposal through ${reference}`, async () => {
    const s = stagedCollector({
      issues: [report(1, { body: `${report().body} See ${reference}.` }),
        report(2, { labels: [], pull_request: {}, url: "https://github.com/dotnet/fsharp/pull/2",
          html_url: "https://github.com/dotnet/fsharp/pull/2" })],
      comments: { 2: [comment(1, { html_url: "https://github.com/dotnet/fsharp/pull/2#issuecomment-1" })] },
    });
    const args = await s.collect();
    const originalParentTime = s.api.issues[0].updated_at;
    s.api.comments[2][0].body = "Correction: the earlier compiler also failed.";
    const rejected = await publishBatch(args);
    assert.equal(rejected.state.issues[1].lastResult.status, "stale");
    assert.ok(rejected.state.pending.some((entry) => entry.number === 1));
    assert.ok(rejected.receipts.every((receipt) => receipt.type === "would-save-memory"));
    s.restart(rejected);
    const retry = await s.collect();
    assert.notEqual(retry.manifest.selected[0].fingerprint, args.manifest.selected[0].fingerprint);
    const result = await publishBatch(retry);
    assert.deepEqual(result.receipts.filter((receipt) => receipt.type === "would-add-label")
      .map((receipt) => receipt.number), [1]);
    s.restart(result);
    assert.deepEqual((await s.collect()).manifest.selected, []);
    assert.equal(s.api.issues[0].updated_at, originalParentTime);
    assert.deepEqual(s.mutations, []);
  });
}
