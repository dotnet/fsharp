# Regression investigation contracts

This directory defines version 1 of the deterministic input, evidence, state,
and admission contracts, complete issue intake, and authenticated issue-comment
checkpoints. It does not activate a workflow, execute reproductions, or spawn
subprocesses. GitHub access is injected by the trusted controller; tests use
synthetic records and an in-memory transport, never live issue mutations.
Records under `fixtures` are not reproduction approvals.

Run the dependency-free suite from the repository root:

```powershell
node --test eng\regression-bisect\model.test.cjs eng\regression-bisect\intake.test.cjs eng\regression-bisect\state.test.cjs
```

## Trust boundary

The exact `Regression` label selects an **open issue, not a pull request**, created
at or after `2026-09-01T00:00:00Z`. Updates and label toggles do not change that
cutoff or authorize execution.

The host must load this module and `.github/policies/regression-bisect.json` from
a pinned, trusted default-branch revision, not from the source revision under
test. `validatePolicy` accepts only that policy, including its finite limits; an
issue, dispatch input, or agent cannot override it.
The module reads its own policy snapshot from that checkout, bypassing Node's
shared JSON module cache so caller mutations cannot change validation limits,
authorization, or selection, even before this module is loaded.

The host must construct `budget.authorization` from the reviewed approval at
`.github/regression-bisect/approved/<issue-number>.json` at `budget.policyRevision`.
Its `manifestDigest` covers the entire manifest; `inputDigest` must equal the
manifest's `approvedInputDigest`. Computing or supplying a digest is **not**
approval. The approval path, repository, issue number, and trusted revision must
also match.

Likewise, `budget.authenticatedObservationDigests` comes exclusively from the
workflow's authenticated worker records. It is not an agent-supplied allowlist.
`verified`, provenance strings, and hashes in arbitrary JSON do not authenticate
anything. These pure validators check structure and consistency; the host is
responsible for authenticating the records before supplying this trusted
context. Source checkout and untrusted reproduction execution must not be able
to replace that context.

## JSON contracts and exports

All records use `schemaVersion: 1`. Validators return `true` or throw a
path-qualified error. Unknown or missing fields, unsupported versions, and
malformed identities are rejected. `classifyAttempts` instead maps incomplete
attempt evidence to an explicit `untestable` outcome; `admitBisection` returns
`{ decision, reason }`, with `decision` exactly `admit`, `stop`, or `blocked`.

`fixtures/contracts.cjs` provides executable examples of every record shape.

| Export | Contract |
| --- | --- |
| `isEligibleIssue(issue)` | Boolean selection using the issues API's `state`, `created_at`, `labels`, and absence of `pull_request`. Dates must have an explicit timezone and valid calendar components. |
| `validatePolicy(policy)` | Exact agreement with the trusted policy, including accounting rules and finite positive integer limits. |
| `validateManifest(manifest)` | Frozen repository/issue, discussion fingerprint, reproduction kind, reviewed source hashes, exact positive/failure assertions, matrix, timeout, pinned producer/packages, search hints, and approved-input digest. |
| `fingerprintInput(json)` | SHA-256 of canonical JSON: sorted object keys, preserved array order and string bytes. Rejects non-JSON values. It never removes comments based on markers, authors, or claimed authentication. |
| `classifyAttempts(attempts, manifest)` | Checks both required repetitions against the frozen assertions and all setup/identity checks. Returns a versioned outcome with an explicit reason. |
| `validateOutcome(outcome)` | Only `pass`, `product-failure`, or `untestable`, with a reason appropriate to that outcome. |
| `validateObservation(observation, manifest, options?)` | Validates target, all reproduction fingerprints, matrix evidence, local artifacts, provenance, and recomputed outcomes. |
| `fingerprintEpoch(manifest, budget, pinnedMainSha)` | Validates the manifest and trusted budget context, then binds the manifest, pinned main, resolved stable release, and policy/workflow revisions. |
| `validateState(state)` | Validates the shared persisted-state shape, epoch, authenticated evidence references, cumulative budget, and any charged pending reservation. |
| `admitBisection({ manifest, stable, main, good, ancestry, budget })` | Fail-closed gate for a verified source interval; no execution side effects. |

### Frozen reproduction

Source entries are `{ path, sha256 }`, hashing the **raw source bytes**. Serialized
repository paths use `/` regardless of worker OS. Absolute paths, traversal,
encoded paths, Windows device names/streams, and URLs are forbidden. The asset
loader must also verify tracked text blobs and hashes and reject symlinks or
resolved paths outside its isolated checkout before execution.

Each matrix cell has an ID, OS (`linux`, `windows`, `macos`), architecture (`x64`,
`arm64`), project or `null`, TFM, configuration, language version, and command.
Only `["fsi", "<reviewed .fsx path>"]` and
`["run-project", "<reviewed .fsproj path>"]` are allowed, matching the manifest
kind. These are operation descriptors, not shell command lines. The engine
expands them using trusted executable paths and the cell's frozen settings,
never by shell interpolation. HTTPS search-hint URLs are discovery hints only,
not executable assets or approval sources.

Assertions are exact `{ exitCode, stdout, stderr }` triples. Both streams are
bounded by the policy and compared without trimming or diagnostic normalization.
Exit codes preserve signed 32-bit statuses and unsigned Windows DWORD statuses
(`-2147483648` through `4294967295`); signed and unsigned representations are not
normalized into equivalent assertions.
A nonzero exit, unrelated exception, truncated output, or missing evidence is
not proof of the frozen product failure. Required restore/build/setup success is
separate from the reproduction assertion: an expected compiler rejection can
satisfy the failure assertion only after the compiler itself was built and
verified successfully.

`approvedInputDigest` hashes the manifest with only that field omitted. It does
not include the moving main SHA. Epochs require the same validated approval and
pinned policy/workflow identities as admission; charging more work does not
change the epoch. The discussion fingerprint must cover the
complete issue and all reporter/maintainer comments, corrections, edits, and
edit timestamps. The collector must supply the complete snapshot; hashing
cannot detect omitted input. This module excludes **no** discussion records.
If the collector excludes workflow publications, it must authenticate record
IDs and content against its own publication provenance; a lookalike marker or
bot display name is insufficient.

### Evidence and reuse

A source observation targets `{ sourceSha }`; a released observation targets
`{ sdk }`. The stable SDK identity binds a non-preview version, verified latest
release metadata at an official release-index URL, the metadata digest, and
per-OS/architecture archive hashes. An installed SDK version alone is not a
stable release resolution. Historical source builds may use a pinned prerelease
build SDK without treating it as a stable release.

Every matrix cell records its command, OS/architecture and runner-image digest,
verified SDK/runtime versions and hashes, producer identity, compiler/tasks/
targets/Core artifact identities, both attempts, and derived outcome. Local
artifacts must name the tested source SHA and observation's build-instance ID.
Released artifacts cannot satisfy a source identity. An unavailable identity
can be `null` only in untestable evidence with the corresponding checks failed.

The observation binds the complete manifest, approved input, discussion, matrix,
producer, and package fingerprints. Admission also compares runner environments
across all endpoints and SDK/runtime identities across the two source endpoints.
Cached source observations are reusable only with these same inputs and trusted
provenance. Different source revisions naturally have different artifact hashes.

Final confirmation calls must supply
`{ finalConfirmation: { runId, buildInstanceId } }` from the new confirmation
reservation. Cached evidence, a different run, or a different build instance is
rejected even when all reproduction fingerprints match. Omit options for
ordinary endpoint validation.

### State, budgets, and admission

State contains the issue/approval fingerprints, manifest, pinned main, epoch,
phase, endpoint evidence digests, tested source revisions, skips, current
good/bad interval and Git log, and publication digest/comment ID. Its `budget`
contains policy/workflow identities, the resolved stable release, authorization,
allowance ID, authenticated observation digests, cumulative consumed counters,
per-work-item attempt counts, next work-item ID, and nullable `pendingWork`.

The host must durably and atomically charge a reservation **before execution**.
Counters include pending and completed work, failed attempts, and cancelled
work; clearing a pending reservation is not a refund. Retries increment the
attempt count and cost another reservation. Cancellation, redispatch, new runs,
input edits, main advances, and label toggles must retain the same issue
allowance and counters. Only a deliberate maintainer-authorized new
investigation can create a new allowance. Shape validation is not storage,
locking, transition authorization, or recovery.

Admission accepts an already charged reservation for the same allowance/item/
attempt, or checks space for the next source build, worker unit, and full
110-minute worker reservation. This conservative reservation includes the
90-minute build timeout and two reproductions capped at five minutes each.
An already charged last reservation is usable even at the cumulative limit;
an exhausted allowance cannot start another item.

The only admitted endpoint combination is stable `product-failure`, pinned-main
`product-failure`, and older source `pass`, consistently across every repetition
and matrix cell. `ancestry` must bind `goodSha`, `badSha`, `pinnedMainSha`, and
verified `isAncestor: true`, with distinct good/bad SHAs. The host must obtain
this relationship from the pinned source graph, never issue claims or timestamps.
Stable pass stops investigation; main pass stops as release-specific/already
fixed; untestable evidence blocks it.

Only the orchestration engine may eventually spawn `git bisect`. It must call
this gate immediately before the first operation and again after any
evidence-invalidating change, using the current authenticated context and
charged reservation. This contract suite does not claim to prove an engine's
subprocess behavior; there is no engine or subprocess execution here.

## Complete intake and bounded discovery

`intake.cjs` exports `readIssueSnapshot(options)`,
`discoverEligibleIssues(options)`, and `findActiveInvestigations(options)`.
They use an injected Octokit-style `transport.request(route, params)` returning
`{ data, headers }`. Repository/issue identities and all callbacks come from
the trusted controller, not issue content. The helpers only read GitHub.

`readIssueSnapshot({ transport, repository, issueNumber, ... })` returns
`{ status: "complete", snapshot }`, or an explicit `ineligible`/`blocked`
result without a partial snapshot. Opened-with-label, late human/bot labeling,
scheduled recovery, and manual requests use the same shared exact eligibility
predicate; no trigger authorizes a reproduction.

The snapshot reads metadata, **all** comments, and timeline/cross-references.
It preserves stable comment identities, current text, edits, corrections,
labels, and relevant issue state. Its `input`, `canonicalInput`, `inputDigest`,
and `discussionFingerprint` describe the complete frozen input; issue
`updated_at` alone is not an input identity. State's `issueFingerprint` uses
`snapshot.discussionFingerprint`. Snapshot `inputDigest` is a collection
fingerprint, distinct from the manifest's `approvedInputDigest`.

Optional `reviewedManifest` must match that discussion fingerprint.
`readSource({ path, sha256 })` must return the raw reviewed bytes from the pinned
trusted checkout; intake verifies their hashes and freezes the reproduction,
exact pass/failure assertions, and dependency identities. A snapshot without a
reviewed reproduction is suitable for a proposal, not execution. The state
helpers separately require today's matching default-branch approval.

Optional `authenticateRecord(comment)` must delegate to authenticated state/
publication provenance. Only exact `true` excludes a record. No callback means
**nothing is excluded**. Markers and copied JSON remain untrusted input.
The integration test uses the actual signed checkpoint fixture to verify that
workflow records are excluded while reporter copies remain hashed.
Production dispatch must supply both authentication and trusted-checkpoint
callbacks. Once a record authenticates, omitting `readTrustedCheckpoints` blocks
the freeze. Omitting both callbacks is only untrusted-input collection, not
duplicate-safe dispatch.

`findActiveInvestigations` examines linked issues/PRs, their current status and
activity, and trusted checkpoints. Suppression requires an open/unmerged target
active within 14 days and a timeline cross-reference or verified backlink; an
arbitrary "already investigating" comment is not a permanent veto.
The controller must inspect `snapshot.investigations.active` before dispatch.
The result records links, status, and the basis for suppression; unreadable
required targets block intake rather than silently disappearing.

To supply trusted checkpoints, adapt `loadCheckpoint()` into
`readTrustedCheckpoints` records
`{ status: "loaded", state, pending, commentId, owner: { runId, attempt } }`,
using `result.checkpoint.state` and `result.checkpoint.pending`. Required read/
evidence failures must propagate as blocked, not as an absent checkpoint.
Use `result.commentId` and `result.owner`, which are recovered from authenticated
provenance, rather than substituting the new controller's run identity.
Returning `[]` asserts that a complete trusted-checkpoint lookup found none.
Pending ownership takes precedence; the specific owning Actions run attempt is
inspected before suppressing or reclaiming work.

Default safety bounds are 20 comment pages, 20 timeline pages, 100 linked
targets, 4 MiB of canonical input, and 1 MiB of aggregate source bytes.
Overrides can only lower these bounds. Exceeding a bound, incomplete pagination,
or an API failure blocks the freeze; first-page truncation is never accepted.

### Discovery cursor lifetime

`discoverEligibleIssues({ transport, repository, cursor, scheduleOrdinal })`
uses created-ordered REST issue listing, not Search's 1,000-result-limited
index. It returns `{ status, issues, nextCursor }`, with at most two pages of
100 records and three candidates. API label filtering only narrows the query;
every candidate still passes `isEligibleIssue`.

Persist the returned `{ version, page, offset }` cursor after consuming its
candidates. The offset retains overflow candidates on a page rather than
skipping them when three have been selected. Traversal resets and revisits old
eligible creation dates so later bot labeling, including issues created exactly
at the cutoff, is eventually discovered.

Supply a monotonic schedule ordinal independently of the cursor cache (for
example, the scheduled UTC slot number from a fixed epoch). Lost or corrupt
cursors use expanding triangular page rounds and rotating three-item offsets;
repeated cache loss must not always return page one's first three issues.
The cursor is only a discovery optimization. Its loss never initializes a new
allowance, removes a checkpoint, or changes consumed budgets. Event/manual
paths and discovery candidates still enter the same serialized issue
transaction before any dispatch.

## Authenticated comment checkpoints

`state.cjs` wraps the shared model state rather than introducing a second model
or changing admission. It keeps the full observations and their manifests,
ordered reservation/completion/cancellation records, cumulative charges,
per-item attempts, pending owner/request/deadline, endpoint epoch history,
intervals, skips, and publication digest/comment ID. Identity tables intern
repeated objects and long assertion streams without dropping observations.
The entire signed UTF-8 comment must be **less than 48 KiB**. A write that cannot
fit blocks further work; it never discards evidence or refunds a reservation.
`fixtures/checkpoint.txt` is a concrete signed checkpoint using a **synthetic
test key**, replayed through reservation, commit, cancellation, and resume.
Its checkout line endings are pinned to LF so Windows Git settings cannot
change the authenticated bytes.

The trusted controller supplies `context`:

* `repository`, `issueNumber`, and the exact authoritative `commentId`;
  `actor: { id, login, type, appId }` are pinned numeric GitHub author/app
  identities, not just the shared Actions bot display name.
* `workflowPath`, `workflowRef` (for example `refs/heads/main`),
  `workflowRevision`, and `trustedWorkflowRevisions`. Old orchestration
  revisions may be retained only after independent maintainer review.
* `keys` maps key IDs to secret `Buffer`s of at least 32 bytes; `keyId` chooses
  the current signing key. Production keys belong only to the trusted
  controller's secret store, never this repository, issue input, source builds,
  reproduction processes, worker output, or caches.
* `owner: { runId, attempt }`, with a decimal-string run ID; UTC `now`;
  the freshly read `issue` and complete `issueFingerprint`; today's reviewed
  `manifest`, `authorization`, `policy`, `policyRevision`, `pinnedMainSha`, and
  `stableSdk`. These have the shared model shapes above. Missing or mismatched
  current approval blocks execution even if the old checkpoint is authentic.

The transport implements these asynchronous methods:

| Method | Required result |
| --- | --- |
| `getComment(commentId)` | Full REST issue-comment object: `id`, `body`, `issue_url`, `user`, and `performed_via_github_app`. Missing comments throw, never return empty state. |
| `updateComment(commentId, body)` | The full updated comment. The helper verifies the returned body and provenance before granting execution. |
| `getRun(runId, attempt)` | The **specific run attempt**, with REST `id`, `run_attempt`, `path`, `head_sha`, `head_branch`, `repository.full_name`, `head_repository.full_name`, `status`, and `conclusion`. Do not silently substitute the latest rerun. |
| `readEvidence(url)` | Verified downloaded bytes as a `Buffer`, or `null` for confirmed missing/expired content. Authentication, rate-limit, and network errors throw. The controller restricts downloads to its evidence stores. |

The versioned envelope has a human-readable authenticated status, a compact
payload, and HMAC-SHA256 over its canonical contents. Its signed binding includes
the exact comment, issue, repository, workflow path/ref/revision, and run attempt.
Reading also checks GitHub author/app identities and independently fetches the
bound run. Copying the marker, envelope, or bot name to another comment does not
establish authority. Untrusted workers must not receive the signing key or
construct the controller context. `commitObservation` accepts observations
collected and authenticated by that controller, not arbitrary reproduction
stdout presented as a worker result.

The controller must retain or rediscover the **same authenticated comment ID**,
requiring a unique valid checkpoint for the issue; a missing or ambiguous record
blocks resumption. Never adopt the newest marker or bootstrap because a cache
missed. `initializeCheckpoint(transport, context, { allowanceId, deadline })`
is only for the initial maintainer-authorized allowance, under the issue lock.
`context.bootstrapDigest` must come from the trusted creation receipt for the
exact initial placeholder, not from hashing whichever comment is currently
present. Initialization refuses an existing envelope. There is deliberately no
automatic reset/new-allowance operation; ordinary issue edits, label toggles,
redispatches, and cancellations cannot replenish any counter.

### Controller transaction

Hold one issue-scoped controller lock across **read, reserve, worker, commit,
and report**. GitHub comment updates are not compare-and-swap. The generation
and owner checks below add stale-request protection; they do not replace that
lock. All state-transition calls reread the authoritative comment.

| Export | Behavior |
| --- | --- |
| `loadCheckpoint(transport, context)` | Returns `{ checkpoint, commentId, owner, executionAllowed, publicationAllowed, blockers, warnings }`. Invalid authentication/schema throws; stale approvals/inputs or required evidence loss explicitly block reuse. |
| `reserveWork(transport, context, { generation, attemptId, request })` | Durably charges and saves the exact request before returning a reservation with `execute: true`. Duplicate requests return `execute: false`; they **must not launch another worker**. Conflicting attempt IDs, stale generations, active pending owners, exhausted counters, and deadlines fail closed. |
| `commitObservation(transport, context, { generation, attemptId, observation, evidence })` | Checks ownership, request/build/provenance and the shared observation contract, then commits all decisive evidence before clearing pending work. Exact duplicate completion is idempotent. |
| `invalidateStaleEvidence(transport, context, { generation })` | Invalidates changed input/approval/producer/orchestration dependencies without erasing history or charges. Main/stable changes start a new endpoint epoch while retaining exact-input historical source observations. Closed/unlabeled issues stop publication; existing result identities remain auditable. |
| `serializeWorkflowRecord({ kind, status, payload }, context)` | Signs a `checkpoint`, `proposal`, or `result` envelope. Low-level serialization is not approval or a storage transition. |
| `authenticateWorkflowRecord(transport, context, comment)` | Returns true only for a matching signed record and fetched run provenance. Forged records return false; required API failures throw. Bind the expected record ID before using this as intake's exclusion callback. |

A work request is
`{ kind, target, endpoint, phase }`: `kind`/`target` use the observation shape;
`endpoint` is `stable`, `main`, `good`, or `null`; `phase` is `stable`, `main`,
`good-search`, `admission`, `bisect`, or `confirmation`. Work-item identity
binds kind, target, and approved input digest, so renaming a request or changing
its role cannot buy extra retries. Every reservation charges one worker unit,
one source build for source work, and the complete policy worker timeout
(110 minutes). The policy limits remain 24 builds, 40 worker units,
720 cumulative reserved minutes, and two attempts per item; the minutes limit
can bind before the other limits.

The worker uses the returned reservation ID as its `buildInstanceId` and returns
the exact pinned request's observation. `evidence` contains
`{ required, sha256, url }` entries for external evidence/logs. Complete assertion
evidence and product/environment identities are retained in the comment itself.
Missing required external evidence blocks execution/publication; optional
expired logs produce an explicit warning. Neither artifacts nor Actions caches
are authoritative memory.

On restart, an outstanding reservation is reclaimed only after fetching its
owning run attempt and establishing `status: completed` with a conclusion.
Queued, active, unknown, or unreadable owners remain blocking even after their
deadline. No cancellation finalizer is required. The old reservation stays
charged and a retry consumes another attempt/unit/time reservation; at most the
one uncommitted observation repeats. If a write succeeded but its acknowledgement
was lost, retrying that attempt does not grant another execution permission.
