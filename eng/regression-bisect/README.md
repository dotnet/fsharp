# Regression investigation contracts

This directory defines version 1 of the deterministic input, evidence, state,
and admission contracts, complete issue intake, authenticated issue-comment
checkpoints, and isolated exact toolchain acquisition. Acquisition runs the
official installer and fixed product-identity probes, not reproductions, source
builds, or bisection. It does not activate a workflow.
GitHub access is injected by the trusted controller; tests use
synthetic records and an in-memory transport, never live issue mutations.
Records under `fixtures` are not reproduction approvals.

Run the dependency-free suite from the repository root:

```powershell
node --test eng\regression-bisect\model.test.cjs eng\regression-bisect\intake.test.cjs eng\regression-bisect\state.test.cjs eng\regression-bisect\toolchains.test.cjs eng\regression-bisect\toolchain-installer.test.cjs
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
An admitted interval with different verified source build toolchains also
returns `toolchainTransitions` for coupled compiler/toolchain attribution.

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
across all endpoints and **reproduction** SDK/runtime identities across the two
source endpoints. A source cell can additionally carry `buildToolchain`:
`sourceSha`, `globalJsonDigest`, `evidenceDigest`, `requestedSdk`,
`requestedTools`, `requestedCompilerRuntime`, verified `sdk` and
`compilerRuntime` product identities, and `verified: true`, `supported: true`.
Requested/resolved versions and source SHA must agree. Both source endpoints
must supply this field or both omit it; legacy observations retain the original
SDK/runtime equality checks. Different valid build toolchains are not evidence
that the compiler source change alone caused the regression: admission retains
both records in each coupled `toolchainTransitions` entry.
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
subprocess behavior; no bisection engine is implemented here.

## Exact toolchain acquisition

`toolchains.cjs` exports four operations. All options are trusted-controller
inputs, never instructions extracted from an issue. Each operation requires the
validated maintainer `policy`. Resolution and acquisition also require
`platform: { os, rid, architecture }`, either directly or in the frozen toolchain.
Supported architectures are `x64` and `arm64`; RIDs are `win-*`, `linux-*`,
`linux-musl-*`, or `osx-*`, matching that OS and architecture.
These are archive RIDs. A historical source SDK can report a versioned host RID
such as `win10-x64` or `ubuntu.22.04-x64`. Its acquired
`RuntimeIdentifierGraph.json` must prove compatibility with the requested
portable RID; the raw graph bytes/hash and actual host RID remain in the
evidence. Traversal stops at portable OS/ABI boundaries, so musl and glibc are
never interchangeable. Missing or inconsistent graph evidence is untestable.

| Operation | Result |
| --- | --- |
| `resolveLatestStableSdk({ policy, platform, signal? }, dependencies?)` | `{ status: "resolved", toolchain, attempts }`; freezes the run clock and raw official index/channel metadata bytes and SHA-256 hashes. Inspects both `sdk` and `sdks`, orders stable versions numerically across LTS/STS channels, excludes future releases, and selects exactly one matching ZIP/tar archive. |
| `readRevisionToolchain({ policy, platform, sourceDirectory, sourceSha, runtimeConfigs? }, dependencies?)` | Preserves raw `global.json`, both requested SDK declarations, original SDK-selection settings, optional historical build/TFM files, and exact runtimeconfig requirements. Each config is `{ path, role: "compiler" \| "fsi" \| "sample" }`. No source file is rewritten. |
| `acquireToolchain({ policy, toolchain, storageDirectory, helperRoot?, signal? }, dependencies?)` | `{ status: "acquired", identity, evidencePath, launch, attempts }`; uses a unique child of existing storage, persists complete identity/metadata evidence, and retains verified archives. `helperRoot` is a repository root, defaulting to the historical source for source acquisition and this trusted checkout for released acquisition. |
| `verifyToolchainIdentity({ policy, toolchain, identity })` | `{ status: "verified" }` only for matching requested/resolved SDK, host/SDK paths, RID/architecture, archive hashes, bundled F# identities, and all exact runtime requirements. The acquisition request binds `toolchain.installationRoot`; evidence cannot nominate a different root. This validates captured evidence, not the authenticity of arbitrary JSON. |

Failures return `{ status: "untestable", outcome, error: { stage, code, message },
attempts }`, not reproduction pass/failure. Missing historical metadata,
unavailable/checksum-less archives, unsupported installer flags or declarations,
SDK/tool declaration disagreement, changed source files, and unfulfilled runtime
requirements fail closed. Historical previews are allowed, but never selected
as released stable. Public preview fallback uses an official archive SHA-512
sidecar; missing checksum provenance cannot become success through a host SDK.
Exact `global.json.tools.runtimes` arrays (`dotnet`, `aspnetcore`,
`windowsdesktop`, optionally suffixed with the matching architecture) become
separate build-runtime requirements. Unevaluated MSBuild version expressions
and foreign-architecture requirements are explicitly untestable, not guessed.

The shared boundaries are injectable: `fs` (`node:fs/promises`), `clock`
(`() => Date`), `http(url, { destination?, method? })`, `run(executable, args,
{ cwd, env, timeoutMs })`, `installer`, and `capture`. HTTP returns status,
final URL, redirect hops and raw bytes, or streams a large download to
`destination`. Every production redirect is checked against the policy's
credential-free public-feed allowlist; downloads, processes and smoke runs
are bounded. Redirects without a nonempty `Location` fail before another
request. Process execution receives a newly constructed environment, not
the caller's secrets, private feeds, credential providers or user profile.

`toolchain-installer.cjs` calls the existing Arcade `GetDotNetInstallScript`
helper with a freshly verified official-script cache. It verifies actual
script parameter support, then invokes the official installer with exact
version/architecture and archive-retention flags. Linux-musl archive requests
use the official `--os linux-musl` option rather than dropping the ABI from the
RID. A loopback-only, bounded
feed serves the already downloaded and SHA-512-verified archive to that
installer. This transport boundary records the **actual public download's**
redirects/digests without reimplementing extraction or trusting a separate HEAD
request. Verification waits for pending server deliveries and file closure,
not just installer exit; cleanup also drains deliveries before removing their
archive. Late delivery errors still fail acquisition. The official retained
archive is hashed again. Supplemental runtime
installation uses the official skip-non-versioned-files flag to preserve the
SDK host. Arcade-owned files are neither edited nor copied.

`launch` contains the explicit executable, historical build working directory,
isolated environment, and a separate sample directory with exact SDK selection
and roll-forward disabled. Source search paths that could select a different
installation are rejected. CLI home and all NuGet caches are unique per
acquisition. Source `global.json` remains unchanged; no TFM is overridden.
Callers must retain these launch settings and choose reviewed public NuGet
sources explicitly when they eventually execute an approved reproduction.

Identity captures `--info`, `--list-sdks`, `--list-runtimes`, resolved SDK base
path, real host/file paths and hashes, actual process architecture, F# compiler/
FSI/Core assembly and file versions, dependency-manifest identity, and bundled
Core nuspec/package identity when present. A fixed FSI probe runs only after
archive verification. SDK-bundled runtime, supplemental sample requirements,
and local compiler execution runtimes are separate records. Supplied local
compiler/FSI runtimeconfigs are verified using the corresponding built DLL's
`--help` and host trace with the exact runtime; sample code is never executed.
Each execution owns a unique trace file and binds both the runtimeconfig's
resolved path and digest, including when several configs have identical bytes
or the same compiler role.
Only acquisition with both local compiler and FSI execution evidence emits the
`buildToolchain` admission record. An SDK acquired before a source build does
not attest the identity of a compiler that has not been built yet.

### Bounded opt-in acquisition smoke

```powershell
node eng\regression-bisect\toolchains.smoke.cjs --run --storage "$env:TEMP"
```

The smoke resolves current official metadata, acquires one stable SDK, verifies
the real archive and running identities, prints a JSON result, and removes only
its uniquely created child directory. It has a ten-minute cancellation deadline.
Exit 0 means actual acquisition/identity success; exit 2 records the exact
block, never mocked download success. This is not a source build or live issue run.

On 2026-09-19 at 01:13:39 UTC, that command resolved metadata and completed
successfully (exit 0) on Windows x64:
requested/resolved SDK `10.0.401`, RID `win-x64`, bundled/executing runtime
`10.0.12`, compiler and FSI assembly `15.2.401.0`, Core assembly `10.1.0.0`,
and bundled package `FSharp.Core/10.1.401`. The archive's final public URL was
`https://builds.dotnet.microsoft.com/dotnet/Sdk/10.0.401/dotnet-sdk-10.0.401-win-x64.zip`
(no redirects). Official and downloaded SHA-512 were both
`24b670ad3d923bfcf47df6c3b034152398b42f6dbc388e10d783aee1cfb5e5817d399fc0ae2a12cfa822a55e61d34830ccb15c50ef6efee437ab874bb7c79430`;
archive SHA-256 was
`c1b96dea223e1ab2af8a51187e0cc96338d14c68834be1656136be18953cea09`.
The official PowerShell install script SHA-256 was
`e8b873e18a81e5c4cd8ab69d84dac8fead291d50b3c44633cd7fddad709a13d6`.
Temporary acquisition storage was cleaned. These are recorded smoke results,
not production version constants.

Validation also completed `node --check` for all twelve top-level `.cjs`
files and the required
`node --test eng\regression-bisect\model.test.cjs eng\regression-bisect\toolchains.test.cjs`
command (416 passed), plus
`node --test eng\regression-bisect\toolchains.test.cjs` (166 passed).
The broader suite above passed all 642 tests without skips in three consecutive
runs, including intake, authenticated state replay and installer-boundary coverage.
`node --test eng\regression-bisect\toolchain-installer.test.cjs` passed all 47
tests. Controlled Windows/Linux delivery ordering reproduced six failures before
synchronization was added. The regression cases now verify pending delivery,
stream/close errors, cleanup after installer failure, and rejection of success
without an archive GET (including HEAD-only requests).
Earlier repair cases reproduced
seven failures for musl installer arguments, historical host RIDs, and colliding
trace files. An additional failing case caught missing same-role execution
evidence hidden by identical config digests. The passing acquisition-to-admission
cases use captured evidence, preserve differing historical/preview build
toolchains and their coupled transitions, and reject invalid execution evidence
and incomparable reproduction runtimes. Cross-platform installer/identity cases
use injected boundaries; the actual download smoke above ran on Windows only.
The additional failure-path cases supply malformed trace bytes to production
capture for compiler, FSI and repeated compiler roles, and exercise production
HTTP redirects with only `fetch` mocked. Removing trace validation caused 18
failures; removing redirect allowlist validation caused 45 failures. Both guards
were restored. Fifteen missing/empty `Location` cases failed before immediate
rejection was added; permitted relative/public redirects still preserve
GET/HEAD behavior, provenance and hashes.
`dotnet fantomas .` completed without changes or errors; working-tree and sprint
commit-range `git diff --check` checks passed. The signed checkpoint fixture
was regenerated through its existing harness for the new pinned public-feed
policy; an old-policy checkpoint is not silently accepted as a new-policy one.
Root compiler Release builds/tests are not this Node adapter's acceptance gate.

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
The result records links, status, and the basis for suppression. Every timeline
cross-reference must identify a resolvable issue/PR target; missing/redacted
source identities and unreadable required targets block intake rather than
silently disappearing.

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
example, the scheduled UTC slot number from a fixed epoch). Scheduled calls must
continue with bounded gaps between ordinals; consecutive slots are not required.
Lost or corrupt cursors use expanding rounds: round `r` visits pages `1..r+1`,
holding each page for `r+1` ordinal slots, at offset `3 * (r % 34)`. Round `r`
starts at `r * (r+1) * (2*r+1) / 6`. Once the dwell time exceeds the schedule gap,
every page/offset in subsequent rounds is visited, including all three-item
slots on dense pages. This prevents fixed cadences such as `2 + 60 * sweep`
from aliasing the rotation and permanently skipping eligible issues.
Fallback recovery deliberately repeats candidates and is slower than retaining
the cursor; sustained unbounded scheduling outages cannot guarantee progress.
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
* `checkpointAnchorSha` is the immutable trusted commit SHA from the initial
  checkpoint creation receipt. Retain that same anchor across main advances,
  policy changes, redispatch, and key rotation; never derive it from issue text,
  the current comment, or a disposable cache.
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
| `request(route, params)` | Octokit-style REST `{ data, headers, status? }` for listing **all** commit statuses on the pinned anchor and appending a status. Pass through pagination headers and errors; never use the combined/latest-status endpoint or a cached status list. |

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
present. Initialization refuses an existing envelope except its authenticated,
interrupted generation-zero checkpoint described below. There is deliberately no
automatic reset/new-allowance operation; ordinary issue edits, label toggles,
redispatches, and cancellations cannot replenish any counter.

### Independent freshness

A valid HMAC authenticates an old body too. Every checkpoint write therefore
appends a signed freshness receipt to GitHub's commit-status history on
`checkpointAnchorSha`, under `regression-bisect/<issueNumber>/<commentId>`.
Receipts bind the repository, issue, comment, anchor, generation, exact body
digest, and key identity. Reads verify the numeric writer identity and signature
and select the **highest authenticated generation across every page**, not the
most recently posted status. Reposting an old receipt cannot lower that mark.
The status is only an anti-rollback witness; the single issue comment still
contains the authoritative observations, reservation ledger, and publication
state. No memory branch or product-content writer is introduced.

The controller needs `statuses: read` for normal loads and `statuses: write` for
transitions/recovery, in addition to its issue-comment permissions. The
[commit-status REST API](https://docs.github.com/en/rest/commits/statuses)
appends statuses rather than editing/deleting them and does not depend on
Actions log/artifact retention. Keep the anchor commit reachable and retain
verification keys for existing receipts. Unavailable history, invalid receipts,
conflicting digests, or more than ten pages of 100 statuses block work.
Exhausting GitHub's per-context status limit likewise blocks, never resets the
allowance. This context must not be configured as a required product-build check.

The signed checkpoint includes `previousDigest`, the exact predecessor body
hash (the trusted bootstrap digest for generation zero). A transition writes
the comment, then its receipt, and grants execution only after both succeed.
Under the issue lock, `loadCheckpoint` can finish the receipt write for exactly
one authenticated successor whose predecessor matches the independent mark.
This preserves observations when cancellation interrupts the two writes; it
does not grant a second execution permission. An older body, a changed body at
the same generation, a generation gap, or a wrong predecessor fails closed.
An interrupted initial write can only be completed by `initializeCheckpoint`
with the original bootstrap receipt and the identical initial allowance ID and
deadline. Recovery authenticates and validates the original generation-zero
checkpoint and appends its missing receipt without rewriting the comment or
reconstructing state from today's inputs. It preserves the original allowance
even when the discussion, approval, endpoints, or eligibility changed, or its
deadline expired. The controller then uses `loadCheckpoint` and
`invalidateStaleEvidence` to apply current context; recovering a receipt is not
execution or publication permission. Missing receipts are never synthesized
from arbitrary loaded state.

### Controller transaction

Hold one issue-scoped controller lock across **read, reserve, worker, commit,
and report**. GitHub comment updates are not compare-and-swap. The generation
and owner checks below add stale-request protection; they do not replace that
lock. All state-transition calls reread the authoritative comment.

| Export | Behavior |
| --- | --- |
| `loadCheckpoint(transport, context)` | Returns `{ checkpoint, bodyDigest, commentId, owner, executionAllowed, publicationAllowed, blockers, warnings }`. Authenticates freshness and can finish an interrupted receipt write under the issue lock. Invalid authentication/schema/freshness throws; stale approvals/inputs or required evidence loss explicitly block reuse. |
| `reserveWork(transport, context, { generation, attemptId, request })` | Durably charges and saves the exact request before returning a reservation with `execute: true`. Duplicate requests return `execute: false`; they **must not launch another worker**. Conflicting attempt IDs, stale generations, active pending owners, exhausted counters, and deadlines fail closed. |
| `commitObservation(transport, context, { generation, attemptId, observation, evidence })` | Checks ownership, request/build/provenance and the shared observation contract, then commits all decisive evidence before clearing pending work. Exact duplicate completion is idempotent. |
| `invalidateStaleEvidence(transport, context, { generation })` | Invalidates changed input/approval/producer/orchestration dependencies without erasing history or charges. Main/stable changes start a new endpoint epoch while retaining exact-input historical source observations. Closed/unlabeled issues stop publication; existing result identities remain auditable. |
| `serializeWorkflowRecord({ kind, status, payload }, context)` | Signs a `checkpoint`, `proposal`, or `result` envelope. Low-level serialization is not approval or a storage transition. |
| `authenticateWorkflowRecord(transport, context, comment)` | Returns true only for a matching signed record and fetched run provenance, not its freshness. Forged records return false; required API failures throw. Bind the expected record ID before using this as intake's exclusion callback, and use `loadCheckpoint` in the mandatory trusted-checkpoint reader to detect rollback before freezing. |

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
