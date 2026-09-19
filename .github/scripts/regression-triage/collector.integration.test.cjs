"use strict";

const { test } = require("node:test");
const assert = require("node:assert/strict");
const { POLICY_VERSION, LIMITS, normalizeMemory } = require("./core.cjs");
const { collectCandidates } = require("./github.cjs");
const { OUTPUT_TYPE, publishBatch } = require("./publish.cjs");
const { repo, now, before, clone, report, comment, fake, emptyMemory } = require("./test-support.cjs");

// Deterministic proposals test collector/publication mechanics, not model judgment.
function stagedCollector(options) {
  const api = fake({ pageSize: 100, ...options });
  let state = emptyMemory();
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
