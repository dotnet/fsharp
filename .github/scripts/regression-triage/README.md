# Reported regression publication

These dependency-free helpers classify **reported** old-working/new-broken behavior.
They do not execute reports, reproduce failures, bisect, or establish that a claim is
true. Unknown contributors' discussion is evidence, not instructions. The collector
is read-only; `publish.cjs` is the only issue/ledger writer.

The independent [workflow operation guide](../../docs/regression-triage.md)
documents triggers, trusted artifact transport and local staged/semantic commands.
`workflow.cjs` wires these contracts to the Actions entry points; `evaluate.cjs`
evaluates the actual workflow policy against hidden frozen expectations.

## Trusted adapter

Exports from `publish.cjs`:

- `createGitHubStore(github, repo)` supplies `read()` and
  `commit({state, expectedHeadOid})`. Both return
  `{state, headOid, missing: null | "branch" | "file"}`.
- `validateProposals(output, manifest)` returns the validated result array or throws.
- `publishBatch({github, store, repo, manifest, output, context, bot, now,
  staged = false, env = process.env, limits})` returns
  `{state, headOid, outcomes, receipts}`. `repo` is
  `{owner: "dotnet", repo: "fsharp"}`. `bot` is the trusted publishing app's
  numeric user `id` and exact `login`, not a reporter or model-supplied identity.
  `now` is a trusted ISO timestamp. `limits` uses the collector's existing bounds.
- `OUTPUT_TYPE`, `ACKNOWLEDGEMENT`, `DIMENSIONS`, `QUESTIONS`, and
  `receiptMarker(repo, number)` expose the fixed protocol vocabulary.

`github.cjs` also exports the backward-compatible
`readMemory(github, repo, {versioned: true})` form used by the store. Omitting the
third argument still returns only normalized state. Issue snapshots also expose
the API `created_at` as `createdAt` (or `null` if unavailable); it scopes ledger-loss
recovery, not the human-evidence fingerprint.

Before collection, call `store.read()`. Pass its state to `collectCandidates` and
attach this binding to the resulting manifest:

```json
{
  "repository": "dotnet/fsharp",
  "runId": "123456789",
  "runAttempt": 1,
  "policyVersion": "reported-regression-v1",
  "collectorRevision": "<40 lowercase hex characters: trusted collector checkout>",
  "memoryHead": "<40 lowercase hex characters from store.read(), or null>"
}
```

The trusted publisher receives the manifest separately from agent output. Its
`context` must equal `manifest.binding`: compare the repository, run ID/attempt,
policy and actual collector checkout revision with trusted runtime values, retaining
the collector's original memory head. Transport the manifest in a separately named,
immutable, same-run artifact uploaded by the collector. Neither the artifact name
nor its contents may come from the model or an agent-writable file.

## One model-visible proposal route

For repository-pinned **GH AW v0.76.1**, use one custom job named
`publish-regression-triage`, with one **required string** input `proposals`.
Its acknowledgement must be:

> Proposal received for validation; publication is not confirmed.

Pass the entire raw JSON text from `GH_AW_AGENT_OUTPUT` as `output` to retain
duplicate-key detection. A parsed object is also accepted for trusted callers/tests.
Its only allowed shape is:

```json
{
  "items": [{
    "type": "publish_regression_triage",
    "proposals": "{\"schemaVersion\":1,\"policyVersion\":\"reported-regression-v1\",\"results\":[]}"
  }]
}
```

The `proposals` string contains a batch with exactly `schemaVersion`, `policyVersion`
and `results`. A result has this shape:

```json
{
  "number": 42,
  "fingerprint": "<64 lowercase hex characters from the selected manifest entry>",
  "classification": "regression",
  "evidence": [{
    "sourceId": "dotnet/fsharp#42:body",
    "url": "https://github.com/dotnet/fsharp/issues/42",
    "quote": "Compiler A accepted this program; compiler B rejects it.",
    "dimension": "compiler"
  }],
  "missingFact": null,
  "clarification": null
}
```

All shown result fields are required. The only optional result field is
`correction: {sourceId, url, quote}`, identifying a human rejecting correction;
it is not allowed with `regression`. It uses the same exact-source validation and
is preserved as a human veto until a subsequent human Regression application.

| Field | Contract |
| --- | --- |
| `results` | 0 to 5 unique selected issue numbers; omitted selected work stays pending |
| `number` | Positive safe integer identifying a selected, complete, eligible snapshot |
| `classification` | Exactly `regression`, `not-regression`, or `uncertain` |
| `evidence` | Up to 12 citations; at least one for `regression` |
| Citation | Exact source ID, exact canonical API-provided GitHub URL, nonblank exact substring |
| Citation bounds | `sourceId` <= 200, `url` <= 500, `quote` <= 1000 characters |
| `dimension` | Optional: `compiler`, `sdk`, `fsharpCore`, `runtime`, `targetFramework`, `configuration`, `producer`, `consumer` |
| `missingFact` | Nonblank string <= 1000 characters for `uncertain`; otherwise `null` |
| `clarification` | `null`, or for uncertainty only: `known-good`, `affected-component`, `comparable-configuration`, `producer-consumer` |
| JSON bounds | Envelope <= 128 KiB, batch <= 64 KiB, nesting <= 16 |

Unknown fields, duplicate envelopes/results/JSON keys, unsupported policies, bot
citations, invented sources, altered fingerprints and incomplete snapshots fail
before writes. Citations can address current title/body, human comments, linked
issue/PR text, reviews and review comments. Deterministic provenance checks are
**not semantic proof**: the classifier must consider human corrections, intended
changes, unsupported setups and automation failures, not the word "regression".

The model cannot choose labels, repository, branch/path, permissions, operations,
comment bodies or state deltas. Do not enable independent built-in label/comment
safe outputs. Missing output is an error and does not commit discovery progress.
A valid empty batch commits the complete trusted discovery delta.

## Durable state and recovery

Only `memory/regression-triage:state.json` is written. Reads pin the file to the
authoritative branch head; absence is confirmed separately from forbidden,
corrupt or failed reads. Initialization creates that literal branch at the
repository API's default-branch head. State commits use GraphQL
`createCommitOnBranch`, `expectedHeadOid`, one literal file addition and a fixed
message. There is no shell push, tree upload, unsigned fallback, or agent-writable
automatic repo-memory ledger.

The publisher first CAS-saves the **whole** trusted scan/queue/read-age delta and
prepared publication intents. Operation IDs bind repository, issue, policy and
human fingerprint. A bounded `discoveryReceipt` binds the most recent manifest
and accepted batch; retries cannot change the accepted decisions. A second CAS
claims each external attempt before a fresh complete snapshot immediately adjacent
to the issue write. The publisher calls `readIssueSnapshot` with
`recheckTarget: true`: when linked evidence is present, it rereads the target's
metadata, discussion and timeline after those dependencies, using another bounded
target-only pass. A change or incomplete read prevents the mutation. The collector's
default read budgets and exported call remain unchanged.
Only the literal `Regression` label can be added, never removed.
`Needs-Triage`, human label applications/removals and stored human corrections
are preserved. Negative/uncertain classifications never remove labels.

Each record retains the analyzed fingerprint, classification, compact reported
citations, policy, missing fact, latest actual outcome, latest human label decision,
correction excerpt, clarification status/receipt and any unfinished intent.
Compatible unknown fields and fair-read age survive schema-1 policy migration.
Unsupported schemas and malformed known fields fail instead of resetting history.
Clarification status, selector, receipt identity/URL and publication intent fields
are validated on both read and write; older receipts may omit their URL.
The serialized store is bounded to 1 MiB and fails explicitly when
full; it never silently evicts human history or receipts.

`published` means the effect was observed after the request; `noop` means no issue
mutation was needed. `stale`, `retryable` and `unknown` keep work pending and must be
surfaced by the caller, as must thrown errors. A post-write correction/closure
records a partial stale outcome without undoing the effect. Request errors are
unknown outcomes until a real label/receipt is observed, not success-shaped
fallbacks. Prepared intents can resume; claimed attempts without an observable
result are retained without blind retransmission. A known-unsent failed recheck
returns its claim to prepared state.

Clarifications are fixed, short, AI-disclosed questions with an issue-level marker
independent of policy/fingerprint. Recovery accepts a live marker only from the
configured **ID + login + API Bot type**, never a human copying it. Durable receipts
also prevent repeats after comment deletion. If the ledger is confirmed missing,
the publisher persists the trusted recovery time as
`clarificationHistoryUnknownThrough`. Issues created at or before that boundary,
or with no creation timestamp, have ambiguous history: absence cannot prove a
question was never posted. A requested question remains `unknown` and pending for
receipt reconciliation, not terminal `noop`. Issues created strictly afterward
can ask their first question, even if discovered much later. Later runs and policy
changes do not advance this boundary. A previously missing creation timestamp can
resolve this uncertainty on recheck, but cannot clear a real unresolved comment
attempt. Regression additions remain independent.

The legacy repository-wide `clarificationHistoryUnknown: true` flag migrates to a
boundary at the migration run's trusted time, because its original recovery time
was not recorded. Legacy memory-absent uncertainty `noop` records become unfinished
again without discarding receipts, intents or human decisions. Restoring the
trusted ledger restores its history; no missing-ledger path blindly reposts a
question.

On reanalysis, an unresolved comment attempt moves into
`clarification.pendingPublication`, retaining its original operation ID and sending
phase. The current analysis gets its own publication intent: an old question's
receipt cannot complete a new Regression addition, and an unknown question outcome
cannot block it. Uncertainty requesting another question stays pending until the
old receipt is observed; no second question is sent. Known-unsent label rechecks
also preserve that independent clarification history.

An older label attempt likewise moves into `pendingLabelPublication` when its
operation ID differs or the current decision is no longer positive. Its original
intent is retained until a complete read observes Regression; it is never replayed
on behalf of the newer decision. An old label receipt cannot complete a new
clarification or negative decision, and an unknown label outcome cannot block one.
Retries of the same positive operation still retain their unresolved sending claim.

On CAS mismatch the adapter reloads after a failed mutation and throws retryable
`CAS_CONFLICT`; the publisher never replays a stale whole-manifest queue or cursor.
There are no automatic CAS retry loops. Recollect from the latest state on a later
run. GitHub has no transaction spanning issue state, labels, comments and memory:
adjacent rechecks and durable claims reduce races, not provide absolute exactly-once
guarantees. Do not configure transport-level retries for comment creation.

## Staged execution and workflow wiring

Trusted `staged: true` **or** `GH_AW_SAFE_OUTPUTS_STAGED=true` forces a local sink.
The same validation, complete rechecks, record construction and recovery execute,
but neither `store.commit`, branch creation nor issue mutations run. Receipts are
`would-add-label`, `would-comment`, and `would-save-memory`. Returned state can seed
an in-memory store for staged restart tests; it must never be committed as live
publication history.

The independently triggered workflow is not implemented here. When wiring it:
import unchanged `shared/model-defaults.md`, compile with v0.76.1, serialize all
triggers in one concurrency group with `cancel-in-progress: false`, and give only
the trusted custom publication job issue/content write permissions. In that
version custom safe jobs cannot depend directly on `pre_activation`/`activation`;
use the trusted immutable artifact transport instead. Preserve the existing
project-labeling and Repo Assist workflows and their separate memory.

## Local verification

```powershell
node --test .github\scripts\regression-triage\core.test.cjs .github\scripts\regression-triage\publish.test.cjs
node --test (Get-ChildItem .github\scripts\regression-triage -Recurse -Filter *.test.cjs).FullName
node --check .github\scripts\regression-triage\core.cjs
node --check .github\scripts\regression-triage\github.cjs
node --check .github\scripts\regression-triage\publish.cjs
git --no-pager diff --check
```

Tests use the frozen classification corpus, shared collector simulation, fake
REST/GraphQL and an interleaved CAS store. No live write API is used.
