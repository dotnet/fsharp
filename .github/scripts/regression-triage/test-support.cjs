"use strict";

const { POLICY_VERSION, normalizeMemory, fingerprintHumanInput } = require("./core.cjs");
const { collectCandidates } = require("./github.cjs");

const repo = { owner: "dotnet", repo: "fsharp" };
const now = "2026-09-18T18:00:00.000Z";
const before = "2026-09-01T00:00:00.000Z";
const limits = { issuePages: 4, commentPages: 4, timelinePages: 4, linkedItems: 2, reviewPages: 4, snapshotReads: 10 };
const clone = (value) => structuredClone(value);
const failure = (status) => Object.assign(new Error(`HTTP ${status}`), { status });

function report(number = 42, fields = {}) {
  const url = `https://github.com/dotnet/fsharp/issues/${number}`;
  return {
    number, url, html_url: url, state: "open", isPullRequest: false,
    labels: ["Needs-Triage"], title: "Compiler behavior changed",
    body: "Compiler A accepted this program; compiler B rejects it.",
    user: { id: 10, login: "reporter", type: "User" },
    created_at: "2010-01-01T00:00:00Z", updated_at: before,
    humanComments: [], humanDecisions: [], linked: [], complete: true,
    ...fields,
  };
}

function comment(id, fields = {}) {
  return {
    id, user: { id: 20, login: "contributor", type: "User" }, author_association: "NONE",
    body: "An independent version comparison.", created_at: before, updated_at: before,
    html_url: `${report().url}#issuecomment-${id}`, ...fields,
  };
}

function fake({ issues = [], comments = {}, timeline = {}, reviews = {}, reviewComments = {},
  pageSize = 2, onList, memory = null, memoryError } = {}) {
  const calls = [];
  const paged = (items, args) => {
    const start = (args.page - 1) * pageSize;
    return {
      data: clone(items.slice(start, start + pageSize)),
      headers: start + pageSize < items.length
        ? { link: `<https://api.github.com/repos/dotnet/fsharp/issues?page=${args.page + 1}>; rel="next"` }
        : {},
    };
  };
  const wrap = (name, fn) => async (args) => {
    calls.push({ name, ...clone(args) });
    return fn(args);
  };
  const github = { rest: {
    issues: {
      listForRepo: wrap("list", (args) => {
        onList?.(args);
        return paged(issues.filter((item) => item.state === "open"
          && item.labels.some((label) => (label.name ?? label) === "Needs-Triage")
          && (!args.since || Date.parse(item.updated_at) >= Date.parse(args.since)))
          .sort((a, b) => a.updated_at.localeCompare(b.updated_at) || a.number - b.number), args);
      }),
      get: wrap("get", (args) => {
        const item = issues.find((item) => item.number === args.issue_number);
        if (!item) throw failure(404);
        return { data: clone(item) };
      }),
      listComments: wrap("comments", (args) => paged(comments[args.issue_number] ?? [], args)),
      listEventsForTimeline: wrap("timeline", (args) => paged(timeline[args.issue_number] ?? [], args)),
    },
    pulls: {
      listReviews: wrap("reviews", (args) => paged(reviews[args.pull_number] ?? [], args)),
      listReviewComments: wrap("reviewComments", (args) => paged(reviewComments[args.pull_number] ?? [], args)),
    },
    repos: {
      getContent: wrap("content", (args) => {
        if (memoryError) throw memoryError;
        if (args.path === "") return { data: memory === null ? [] : [{ name: "state.json" }] };
        if (memory === null) throw failure(404);
        return { data: { type: "file", encoding: "base64",
          content: Buffer.from(typeof memory === "string" ? memory : JSON.stringify(memory)).toString("base64") } };
      }),
      getBranch: wrap("branch", () => { throw failure(404); }),
    },
  } };
  return { github, calls, issues, comments, timeline };
}

const emptyMemory = () => normalizeMemory(null, { policyVersion: POLICY_VERSION });
const completed = (snapshot, fields = {}) => ({
  fingerprint: fingerprintHumanInput(snapshot), policyVersion: POLICY_VERSION,
  classification: "regression", lastResult: { status: "published", operationId: "op-42" },
  ...fields,
});
const collect = (api, memory = emptyMemory(), fields = {}) =>
  collectCandidates(api.github, { repo, memory, now, limits, ...fields });

async function publishRun(api, memory, fields = {}, publish) {
  const result = await collect(api, memory, fields);
  if (publish) memory = await publish(result, memory);
  else {
    memory = { ...memory, ...result.stateDelta };
    for (const item of result.selected) {
      memory.issues[item.number] = { ...memory.issues[item.number], ...completed(item.snapshot) };
    }
    const handled = new Set(result.selected.map((item) => item.number));
    memory.pending = memory.pending.filter((entry) => !handled.has(entry.number));
  }
  return { result, memory: normalizeMemory(JSON.stringify(memory)) };
}

module.exports = { repo, now, before, limits, clone, failure, report, comment, fake, emptyMemory, completed, collect, publishRun };
