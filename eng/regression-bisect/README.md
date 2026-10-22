# Regression investigation contracts

This directory defines version 1 of the deterministic input, evidence, state,
and admission contracts, complete issue intake, authenticated issue-comment
checkpoints, and isolated exact toolchain acquisition. The endpoint-first engine composes
those contracts with the existing reproduction and clean source-build runners.
Fresh confirmation and deterministic publication guards consume the engine's
intent. The controller transaction library can update an authenticated fixed
result slot through its injected transport. `controller-cli.cjs` supplies the
concrete deployment. The dedicated `regression-bisect.md` agentic workflow and
its compiled lock route intake to that controller; both remain disabled until
explicitly configured by maintainers. Tests use synthetic records and in-memory or
file-backed GitHub services, never live issue mutations.
Records under `fixtures` are not reproduction approvals.
The file-backed controller fixture publishes database updates by atomic
replacement, including operator fault changes and injected clock changes;
concurrent CLI readers see complete snapshots rather than truncated JSON.
Readers reuse snapshot bytes only while file identity, size and timestamps match;
each read still checks the current file and parses an independent value.
The routing fixture serves the real compiled lock as a Git blob directly from
the tested checkout, instead of duplicating it in every mutable API snapshot.
The controller and routing suites each run two independent file-backed cases
concurrently. Each case owns its database, lock directory and subprocesses;
the real-time quota test and the 1,100-sweep stress case do not serialize their
entire suite. The stress case is bounded by the fifteen-minute focused-test
ceiling and gives each sweep
its own production deadline, rather than sharing a deadline across invocations.
Offline CLI fixtures run real, credential-free Git against their synthetic local
history using the existing bounded command runner. This avoids starting a
Windows historical-process supervisor for each trusted Git query; it neither
executes historical code nor substitutes Git results. The adapter is available
only behind the existing explicit offline-fixture gate, forbidden in Actions.
Production execution and its process-tree containment remain unchanged.
The full runner caps file concurrency at four; the host CPU count is not a
safe default for simultaneous durable-file and subprocess stress suites.

Run from the repository root (the production CLI has no npm dependency).
Install the lockfile-pinned YAML test parser only if it is missing:

```powershell
npm ci --prefix eng\regression-bisect --ignore-scripts --no-audit --no-fund
$tests = @(Get-ChildItem 'eng\regression-bisect' -Filter '*.test.cjs' | ForEach-Object FullName)
node --test --test-concurrency=4 @tests
```

CI runs scoped validation for pull requests and pushes to `idea/dm-912`.
An ephemeral Ubuntu runner also executes the production containment smoke with the exact pinned image.
The seven physical modes retain their reports and cleanup receipts as CI artifacts, including on failure.
These checks do not activate production routing or establish source/product acceptance by themselves.
Synthetic trial tests create their signed inputs with a current deadline.
They do not change the production clock or renew an existing investigation allowance.

Push commits with `[source-acceptance]` to `idea/dm-912` to request one contained source-worker smoke.
This opt-in job builds the frozen harmless source fixture and runs its built FSI through the production adapter.
A new trusted process authenticates exported evidence after worker removal.
The job preserves its source, toolchain, build, assertion, and cleanup evidence as artifacts.
It does not establish controller, project-matrix, immutable-producer, or full trial acceptance.
It does not change an existing investigation journal or allowance.
The frozen source graph records the source SDK's compiler runtime from publisher metadata.
The source smoke declares that runtime separately from the stable SDK's bundled runtime.
The existing installer acquires and verifies a supplemental runtime when necessary.
The worker retains its archive receipt and verifies the actual FSI runtime and Core loads.

`[worker-acceptance]` selects the extended smoke instead, on the same authorized
Linux job after all seven containment modes. It runs the controller library
against a durable file-backed GitHub API, first executing the real stable endpoint
and then the clean source endpoint. The source worker reuses `repro.smoke.cjs`
with the production historical execution lane for producer creation and every
consumer. Its matrix includes Linux multi-TFM, exact diagnostics, runtime-only
failures and wrong host/compiler/tasks/targets/Core cases. The frozen producer is
consumed as its exact NuGet package; its DLL and package bytes are retained.
Protected-supervisor and historical-UID probes precede product execution.
Only the exact SDK-hosted evidence-helper command uses the protected lane;
a helper pathname passed as sample data does not select that lane.
After collection, launch scratch is deleted and a new process authenticates the
durable evidence. Replay with `source-worker.smoke.cjs --controller` followed by
the absolute reviewed config, new evidence directory, and containment directory.
These two acceptance units have their own allowance; they neither prepare nor
confirm the separate six-unit investigation.
Released SDKs can ship a bundled FSI Core whose bytes differ from the Core
package's framework assets. The collector binds both to the acquired SDK,
without treating the bundled assembly as a package asset.
The project matrix uses a separate exact released-SDK installation without the
source FSI's supplemental runtime, and the source unit's own acquired build SDK
for local consumers. Each compiler child therefore has one declared runtime;
the original controller FSI assertions retain their frozen reproduction SDK.
Publisher metadata retains every original response losslessly with gzip and its
original SHA-256. `metadataBytes` authenticates and decodes both these records
and legacy uncompressed records, with a 16 MiB decoded limit. The matrix
supplement is retained once in `result.json`, not duplicated in `source.json`.
The existing per-file, launch-transfer and durable-envelope limits are unchanged.
The task-identity negative uses an off-inventory same-byte copy of the tested
tasks and Core, so an older SDK's incompatible task API cannot mask the actual
identity check. It must compile successfully before identity rejection.

Native Linux acceptance on 2026-10-22 passed at `501fe1b4031b15a0a614312133c81b56ad1736ba`
in [owned-fork run 39779256564](https://github.com/T-Gro/fsharp/actions/runs/39779256564).
Both seven-mode containment runs, the real controller stable/source endpoints,
47 product-matrix cases, immutable `FrozenProducer` package `1.0.0`, and the
new-process durable reread passed after worker and launch-scratch removal.
The clean source build/FSI runner took 584,434 ms; the longest reproduction took
28,691 ms. Released matrix consumers used SDK `10.0.402` / runtime `10.0.13`;
local matrix consumers used the source-declared SDK `10.0.301` / runtime `10.0.9`.
The controller's built-FSI assertions used the frozen `10.0.402` reproduction
host with exact runtime `10.0.9`, not host `dotnet fsi`.
The original source-worker artifact ZIP has SHA-256
`1da00583aae62c296b3fafa7bf4196e49e5d132b84d8beac99bca021ad3ff16b`.
The artifact retains exact configuration, commands/exits, source/product/producer
identities, archive bytes, denial/cleanup receipts and the fresh reader PID.
This is worker acceptance, not a completed six-unit trial or production activation.

`[worker-acceptance] [trial-acceptance]` additionally runs the existing one-unit
trial CLI on that same ephemeral Linux host, after current physical acceptance.
It preserves the approved three-commit source graph and assertion, reviews the
complete fixture discussion, and signs the exact configuration before preparing
one new trial. Six explicitly listed route/worker pairs reserve 660/720 minutes
and five source builds; each CLI call is a new controller process. There is no
whole-trial loop, candidate override, allowance reset or live issue write.
The fixture matrix explicitly labels its history and PR URLs as synthetic.
Its PR membership spans two pages and uses actual source commits.
The final fake result write loses its acknowledgement; a new process must adopt
that same authenticated result before read-only verification. Recovery explicitly
acknowledges the stale delivery, routes its current generation, then adopts the
persisted result; these three bounded calls reserve no additional worker. Trial inputs,
process/API/Git traces, journals, archives and evidence are retained with the
source-worker artifact. Generated controller signing keys are not uploaded.
These checks remain opt-in validation, not production registration or activation.

Source reproduction now acquires a publisher-verified supplemental runtime when
the built compiler's measured runtime is absent from the frozen reproduction
SDK. The SDK version and actual archive hash must remain identical; no released
SDK bytes or source declarations are patched. Acquisition failures stay setup
failures, and the collector binds the actual supplemental installation and
retained runtime archive to the reproduced product.
Checkpoint packing interns repeated source/assembly/archive identities as well
as long paths, using the same lossless format as result records. The fixed
48 KiB comment limit and fail-closed overflow behavior are unchanged; legacy
packed records remain readable. Generated fixture signing keys are masked before
Actions exports them to later controller steps.
Signed result payloads reuse the bounded, hash-checked gzip encoding used for
publisher metadata; their full deterministic Markdown remains visible. Decoded
identity tables are capped at 2 MiB, and old uncompressed signed results still
authenticate. This keeps required evidence URLs and all observations inside the
unchanged fixed result slot without dropping report content.

Windows CI checks out the reviewed source history for the controlled trial.
Process-tree tests include Windows supervisor startup before they check descendant termination.
The Windows supervisor receives an explicit OS environment and the built-in module path.
It loads its built-in JSON module by exact path with automatic module discovery disabled.
It does not inherit runner credentials or user module and executable search paths.
Its request reader decodes UTF-8 from the input pipe without changing the console encoding.
The sample receives only its supplied environment after the supervisor joins its Job object.
Both Windows installer phases load their built-in modules explicitly, without automatic discovery.
Installer arguments remain typed data, including switches and literal paths.
Linux installers keep extracted files owned by the extracting account, without restoring archive owner IDs or adding container capabilities.
Worker scratch permits native SDK and product execution, which the physical inspect canary verifies with an owned executable.
Broker scratch, `/tmp`, and `/trusted/artifacts` remain non-executable.
Toolchain identity probes reject incomplete capture and retain process-launch errors.
Historical bootstrap and pack commands permit only same-line runtime patches inside the verified SDK installation.
SDK selection remains exact.
Acquisition, evaluation, and identity commands retain disabled runtime roll-forward.
Source command receipts record the build runtime policy.
Boundary exceptions retain a command receipt and the original error without inventing a process exit code.
Source and helper files are copied as bytes into owned scratch, without restoring input ownership or requiring `CAP_FOWNER`.
The helper uses the acquired SDK environment.
Helper failures preserve acquired archives and command evidence.
After historical descendants stop, the supervisor reclaims only their owned scratch directories.
Ownership changes do not follow SDK links or include protected helper, control, or storage directories.
Historical commands own their isolated NuGet cache root, including newly created HTTP and plugin caches.
The supervisor reclaims these caches after command completion or failure.
After historical processes stop, the supervisor removes their single-link FIFOs and sockets from owned scratch.
Prelaunch IPC nodes, foreign-owned nodes, hard links, and other nonregular nodes still fail validation.
Worker file limits permit the source bootstrap's 6,500 descriptors and remain bounded at that value.
The broker retains its 1,024-descriptor limit.
Docker inspection and the physical inspect canary verify the worker limit.
Docker cleanup recognizes the daemon's exact missing-network response, without treating permission failures as successful removal.
The egress broker accepts Node's default-port Host header only for the same approved CONNECT destination on port 443.
Public Azure DevOps feeds redirect package downloads to the publisher's `*.vsblob.vsassets.io` CDN.
Those CDN tunnels retain public-address validation and the existing header, byte, and time limits.
The broker permits 128 concurrent public tunnels for multi-feed NuGet restores.
Up to 128 additional requests wait in a FIFO queue without starting DNS or an upstream connection.
Under admission pressure, the broker closes tunnels with 30 seconds of inactivity so pooled connections cannot block restores.
The connection, byte, and time budgets remain unchanged.
Historical commands receive a private NuGet configuration that limits concurrent HTTP requests to four per feed.
This configuration changes neither package sources nor credentials and does not replace repository configuration.
The transport receipt retains peak tunnel usage and denial counts before container removal.
The timeout canary keeps its child referenced until the external deadline terminates the container.
The memory canary supervises a bounded allocation child inside the unchanged 256 MiB cgroup.
It requires increases in cgroup `oom` and `oom_kill`, a `SIGKILL` child termination, and a successful supervisor exit.
The child requests at most 512 MiB and cannot bypass the memory or zero-swap limits.
A Docker exit code of 137 or an `OOMKilled` flag alone cannot satisfy the probe.

## Durable intake routing (not activated)

The deployed graph is:

```text
issues(opened/labeled/unlabeled/edited/closed/reopened)
  | workflow_dispatch(issue: number) | hourly schedule (17 * * * *)
  v
regression-bisect.lock.yml / route (trusted, five minutes, discovery lock)
  |-- signed cursor + exact dispatch intent --> controller.yml on main
  |                                              |
  |                                              v
  |                                  per-issue lock + reviewed approval
  |                                  bootstrap / advance / publish
  |                                  at most one charged isolated worker
  |                                  or guarded fixed-slot terminal report
  |
  `-- fresh inert proposal identifiers --> activation --> agent
                                         --> detection --> artifact/noop
```

The fixed controller filename is `regression-bisect-controller.yml`.
There is one production scheduler, in the compiled AW, not in the controller.
The routing job is independent of activation, model credentials, model success,
artifacts and safe outputs. Workflow-level concurrency is run-specific so a
pending/failed model cannot hold later schedules; the separate routing-job
concurrency group and persistent directory lock serialize cursor writers.
Before invoking the routing CLI, the self-hosted job provisions Node 24 through
SHA-pinned `actions/setup-node`, with package-manager caching disabled and no
routing secrets on the setup step. Setup failure stops routing. The runner must
be version 2.327.1 or later for Node 24 actions and have the runtime in its tool
cache or allow the setup action's Node download; neither checkout nor the
controller's separate `node24` JavaScript action puts Node on this job's shell
`PATH`.
An empty sweep or an approved-only sweep skips the model. Schedules reread
bot-added labels missed by GitHub's event-chaining suppression and rotate back
to approved in-progress issues for one more charged unit, without a human gate
before each unit. Removing a label never authorizes work.

`routing-cli.cjs` composes the existing intake, signed state and fixed controller
dispatcher; it does not execute reproductions or grant allowances. Selection
requires an open non-PR issue, the exact case-sensitive `Regression` label and
**creation at or after `2026-09-01T00:00:00Z`, inclusive**. Update time is not a
substitute. Events (including bot labels), manual selection and schedules reread
the current issue, every bounded discussion/timeline page, linked investigations
and reviewed immutable sample bytes. An unapproved issue produces only an inert
`approval-required` proposal. Issue prose cannot choose a command, URL, ref,
workflow, allowance or destination.

```powershell
node eng\regression-bisect\routing-cli.cjs provision --config $TrustedConfig
node eng\regression-bisect\routing-cli.cjs schedule --config $TrustedConfig
node eng\regression-bisect\routing-cli.cjs manual --issue 123 --config $TrustedConfig
node eng\regression-bisect\routing-cli.cjs event --issue 123 --config $TrustedConfig
```

Provisioning requires reviewed version-2 `.github/regression-bisect/discovery.json`
(see `discovery.schema.json`), two distinct existing App-owned comments with exact
bootstrap digests, a fixed anchor, workflow generation and unexpired provisioning
deadline. `commentId` stores the cursor; `freshnessCommentId` stores its signed
generation/body digest and registration digest. Both IDs must differ from the
issue's checkpoint/result slots. Provision issue allowances separately, before discovery if they share
the same issue. Keep the controller-only
`<RB_STATE_DIRECTORY>/dotnet-fsharp-discovery` directory, including signed
`intent.json` and `committed.json`, on the same persistent host used for all
routing entry points. The discovery lock serializes schedule/event/manual
writers without cancelling another run. Never reset these files or the remote
slots/receipts to recover a sweep.
During first provisioning, `committed.json` holds a signed provisioning marker
bound to the reviewed registration until the exact generation-zero intent and
its freshness receipt are acknowledged. A restarted provision or schedule can
finish that transaction, including a lost first freshness acknowledgement,
without replacing the intent. Once complete, the marker becomes the committed
cursor. Missing or forged history is not initialization authority; older
interrupted provisioning without this marker requires operator recovery.

Each sweep reads at most two REST pages of 100 records and selects at most three
issues. The authenticated page/offset cursor preserves dense-page overflow and
wraps to revisit old eligible issues and later labels. Neither acknowledged nor
unacknowledged deliveries hold the scan at one issue. A failed dispatch does not
suppress the other selected dispatches; its error is returned after their
acknowledgements are saved. Retry selection follows the same rotating cursor
(or explicit event/manual selection), not a second pending queue. On revisiting
an unchanged controller generation, routing reconstructs the same dispatch key;
consecutive selections retain the five-minute retry delay.
Selection and exact dispatch intent are signed and persisted locally, then in
the fixed remote discovery slot and freshness receipt, **before** dispatch.
The receipt overwrites only its registered fixed comment, using the same
App/run/workflow/key authentication as the cursor. It does not append commit
statuses or scan shared-anchor status history on each discovery transition:
storage and freshness reads remain constant as sweeps accumulate. Required
local intent/committed records and the two remote slots cross-check generations;
the receipt cannot be replaced by its bootstrap placeholder after provisioning.
Authenticated predecessor-only recovery completes torn writes; missing, forged
or rolled-back required state blocks instead of starting a replacement.
Bounded listing failures preserve the cursor and never reset issue budgets.

The separate `RB_DISPATCH_TOKEN` has only the required Actions dispatch
authority. Its only destination is the controller workflow on `main`, with
typed issue, mode, `routingKey` and `routingGeneration`. The no-argument Actions
parser uses the runner event file; manual selection additionally requires typed
`RB_ACTION_INPUTS={"issue":123}` from `toJSON(inputs)`, not the event's untyped
string as authority. Malformed payloads and override fields fail before
configuration or API access. The compiler adds optional `aw_context`; intake
accepts it only absent or empty, never as a target or instruction.
Offline fixture injection remains forbidden in
Actions. Discovery never receives worker execution authority, and publication
credentials never enter workers.

Delivery can repeat after a lost response. Consumers authenticate current
registration, discussion, samples, owning run, checkpoint generation,
freshness and required evidence under the existing issue lock. Old deliveries
are inert duplicates; future or mismatched generations block. Read-only discovery
recognizes an authenticated single-predecessor interrupted write and dispatches
its receipted generation back to the controller. Only the issue-locked controller
repairs freshness. Original interrupted generation-zero initialization instead
returns through the bootstrap validator, preserving its allowance and slots;
nonzero missing receipts, rollback and malformed predecessors still block.
Keep the controller's signed `dotnet-fsharp-<issue>/bootstrap.json` journal:
the original slot records initialization intent, and a checkpoint records
completion. A missing or forged journal blocks recovery; a completed bootstrap
with a missing receipt requires operator recovery, not reinitialization.
Normal progression
is bootstrap, at most one worker per advance, terminal transition, then guarded
fixed-slot publication. Eligible stopped/blocked outcomes (including stable
passes, main passes, no-green/skip, setup/flake and budget stops) can be reported
without starting another worker. They do not establish a verified culprit.
Edits, stale main, live linked investigations and lost required evidence still
block publication. A freshness rejection persists `requeue`, distinct from
other publication blocks, so the next routed advance reconciles current inputs
under the issue lock before publication can be retried. If the inputs return to
their previous identity, that advance clears the requeue without discarding
valid evidence. Interrupted requeue writes use the same predecessor recovery.
An authenticated completed owner's unclaimed reservation is retired before a
terminal capacity/attempt stop is saved, without refunding its charge. Claimed
work still requires collection; an active or unknown owner cannot be retired.
Terminal reasons are retained in signed state and rendered in the result, including
deadline-capacity and attempt-limit stops that cannot be inferred from aggregate
charges. A resumed search or changed input clears the prior reason.
Counters remain 24 source builds, 40 units, 720 reserved
minutes and two attempts; retries, cancellation and edits do not refund them.

The discovery schema, registration validator, current/retained run authentication
and fixtures all bind `.github/workflows/regression-bisect.lock.yml`.
Deployment `workflowPath` remains the controller path; routing replaces it only
in its discovery record context. Retain both workflows' trusted helper revisions
and signing keys in the reviewed `retainedWorkflowRevisions`/`keyHashes` lists.
Register version-2 cursor/freshness slots and bootstrap hashes together.
The archived nonexistent discovery path and version-1 append-only registration
are rejected, not aliases or permission to reset history. Already-provisioned
records with another identity require reviewed owner migration, not automatic
adoption. No registration, approval or activation configuration is shipped.

Unapproved, stale-approved and unregistered issues produce explicit proposal
intents, not endpoint evidence. Routing records a signed `analysis-attempt` in
`dotnet-fsharp-discovery/proposal-<issue>.json` before exposing identifiers to the
model. This record is bound to the registered discovery issue/comment, trusted
run, selected issue and proposal/discussion key. Unchanged attempts are suppressed
for seven days across rotating pages and restarts; corrections permit a new
attempt. It records **attempted analysis**, not completion. Retain these files
with the cursor journals; malformed records block analysis rather than granting
authority. They own no allowance or budget, and dispatch is committed before
proposal processing. No model output is consumed by routing or the controller.
The model can read issue/repository/linked-PR context and write a local
`proposal.md` artifact (16 KiB maximum, seven-day retention), but cannot post it.
Review that untrusted artifact before manually preparing any approval.

**Historical physical prerequisite inventory:** the read-only inventory on 2026-09-22
found a Windows x64 host with Docker/WSL/SSH clients, but no enrolled native
disposable Linux worker route or owner receipt. No daemon/WSL was started, no
account provisioned and no historical product code executed. Native Linux x64,
Docker >=28, cgroup v2, AppArmor/seccomp, containment and the known-history trial
were then separate, unaccepted gates. The later owned-fork Linux worker
acceptance is recorded above; it does not complete the known-history trial.

Focused acceptance commands (in order, then the full scoped command above):

```powershell
node --test eng\regression-bisect\workflow.test.cjs eng\regression-bisect\routing.test.cjs eng\regression-bisect\controller-cli.test.cjs
if ($LASTEXITCODE -ne 0) { throw 'Compiled workflow integration failed' }
```

Tests use real producer/consumer subprocesses and the file-backed service's
ordered API/worker/Git ledger, including lost acknowledgements and process
cancellation. Routing and controller suites share `invokeCli` in
`fixtures/controller-service.cjs` for process capture; fixture producer, consumer
and controller exits record PID, arguments, duration, exit code, stdout and
stderr alongside the mutations. Set an absolute `RB_TEST_EVIDENCE_DIRECTORY` to
retain these ledgers. Capture each process's stdout, stderr, exit and hashes of the tested
source bytes; historical test totals are not evidence for a changed checkout.

### Source-bound workflow compilation

The fleet compiler is official `github/gh-aw` **v0.76.1**, tag commit
`58d1bedbb7200f59c2d224151339e38fd8687d05`, not the installed shared v0.88.2
extension. Use the isolated release asset `windows-amd64.exe` from
`https://github.com/github/gh-aw/releases/tag/v0.76.1`, verified against release
SHA-256 `fb8789c9ed8fc3c7d36c50659e6171268bb8cc13e95206aed20600fe82942e57`.
Do not replace the shared extension or compile the fleet.
The exact arguments verified with this binary's `compile --help` are
`compile regression-bisect --strict --no-check-update`.
The version-bound official [trigger reference](https://github.com/github/gh-aw/blob/v0.76.1/docs/src/content/docs/reference/triggers.md),
[job compiler](https://github.com/github/gh-aw/blob/v0.76.1/pkg/workflow/compiler_jobs.go),
[pre-activation tests](https://github.com/github/gh-aw/blob/v0.76.1/pkg/workflow/pre_activation_custom_fields_test.go)
and [permission reference](https://github.com/github/gh-aw/blob/v0.76.1/docs/src/content/docs/reference/permissions.md)
define the supported custom-job/output graph and authority split used here.
The inputs are the AW Markdown and `.github/aw/actions-lock.json`.
Structural tests also bind its model settings to `shared/model-defaults.md`.
The lock is compiler output, never hand-edited.
Its scoped Git attributes preserve LF bytes and the compiler's generated
trailing whitespace/blank EOF; handwritten files keep normal whitespace checks.

```powershell
# $Compiler is the absolute path to the verified v0.76.1 executable.
# $Receipts is a private existing directory, outside the helper checkout.
if ((Get-FileHash $Compiler -Algorithm SHA256).Hash.ToLower() -ne
    'fb8789c9ed8fc3c7d36c50659e6171268bb8cc13e95206aed20600fe82942e57') { throw 'Compiler digest mismatch' }
$hashes = foreach ($pass in 1, 2) {
    $p = Start-Process $Compiler -ArgumentList 'compile','regression-bisect','--strict','--no-check-update' `
        -NoNewWindow -PassThru -RedirectStandardOutput "$Receipts\compile-$pass.stdout" `
        -RedirectStandardError "$Receipts\compile-$pass.stderr"
    if (-not $p.WaitForExit(420000)) { Stop-Process -Id $p.Id; throw 'Compile timeout' }
    if ($p.ExitCode -ne 0) { throw 'Compile failed' }
    (Get-FileHash '.github\workflows\regression-bisect.lock.yml' -Algorithm SHA256).Hash
}
if ($hashes[0] -ne $hashes[1]) { throw 'Nondeterministic lock output' }
node --test eng\regression-bisect\workflow.test.cjs
if ($LASTEXITCODE -ne 0) { throw 'Generated DAG failed' }
```

File-bound standard streams avoid an observed Windows compiler startup hang
with piped PowerShell invocation. No strict checks are disabled. v0.76.1 only
allows steps/outputs in its special pre-activation override, not the persistent
runner selection needed here; the supported custom `route` job is therefore
referenced by both the activation condition and prompt. The compiler's built-in
jobs do not support timeout overrides: they retain Actions' finite 360-minute
job default, with explicit 15-minute agent and 20-minute detection step limits.
Routing itself has a five-minute job and CLI deadline; safe outputs have a
15-minute job limit.

Generated authority is checked, not inferred from the Markdown. In particular,
noop-only safe outputs would implicitly enable issue creation in this version.
The explicit inert artifact output plus disabled issue reporting for noop,
missing data/tools, incomplete runs and failures prevents that fallback.
All AW jobs have only read permissions; only `route` receives
`REGRESSION_ROUTING_TOKEN`, `REGRESSION_ROUTING_SIGNING_KEYS` and
`REGRESSION_DISPATCH_TOKEN`. These reviewed additions are for fixed state/dispatch,
not network/tool/model access. Model and detector receive only the Copilot engine
credential and/or read-only job `GITHUB_TOKEN`; explicit tokens avoid the fleet's
potentially wider magic-token fallbacks. Framework checkouts use the triggering
main commit with `persist-credentials: false`; the router checks its immutable
helper SHA, clean tree and exact trusted compiled-lock bytes (bounded at 256 KiB).
Both router and controller reject a missing, non-SHA or mismatched reviewed
revision **before checkout**, not after executing helpers from that revision.
The model never shares the persistent self-hosted runner.
An explicit empty Bash allowlist and disabled MCP CLI mounting prevent the
compiler's default unrestricted shell tools in the analysis job. Both model
invocations explicitly deny shell/delegation tools and disable inherited
custom instructions; they retain the shared model/reasoning defaults. v0.76.1
rejects a local engine override alongside an imported engine, so this workflow
spells out those defaults and tests them against the shared file instead of
changing the fleet's permissions. Inert
local proposal editing is not historical code execution authority.

`workflow.test.cjs` parses source and lock and rejects removal of routing,
model-dependent routing and leaked worker/model write secrets. It also requires
the pinned Node 24 setup before the routing CLI and rejects missing, reordered,
unpinned, wrong-version, conditional or fail-open runtime setup.
`regression-bisect-validation.yml` runs only path-scoped offline validation, not
intake, publication, a production dispatch or physical containment.

## Executable deployment and operator setup

### Opt-in resumable controlled-history trial

`trial.cjs` composes the existing routing and controller transactions. Its API is
always an owned file-backed service for **SYNTHETIC ACCEPTANCE ONLY**, never live
GitHub. Its worker is always the real container/runner/collector. The offline
unit-test worker, injected metadata, clock and runtime callbacks are deliberately
absent from the trial adapter. Actions, `NODE_OPTIONS`, and `RB_LOCAL_TEST*`
injection are rejected. There is no whole-run, reset, refund or target override.

```powershell
node eng\regression-bisect\trial.cjs --help
node eng\regression-bisect\trial.cjs --prepare $ReviewedConfig $NewEvidenceDirectory
node eng\regression-bisect\trial.cjs --advance $ExistingEvidenceDirectory
node eng\regression-bisect\trial.cjs --verify $ExistingEvidenceDirectory
```

These variables must be resolved absolute canonical paths, not literal acceptance
commands. Evidence and the isolated source repository must be outside this
checkout and separate from each other. `--prepare` rejects an existing directory;
it freezes a signed plan, reviewed bytes, helper hashes and a new fake-service
identity without executing historical code. Keep the original reviewed inputs,
source objects, containment receipts and signing keys as well as the evidence
directory: missing or changed required inputs are not cache misses.

The reviewed JSON has exactly these fields:

| Field | Meaning |
| --- | --- |
| `schemaVersion`, `purpose` | `1`, `SYNTHETIC ACCEPTANCE ONLY` |
| `helperRevision`, `deployment` | Full trusted helper revision and the existing exact deployment configuration, including image, App/key and containment identities |
| `api` | `{ path, sha256 }` of a fresh `fixtures/controller-service.cjs` database for fake issue 123; reviewed approval, registration, sample/producer bytes, discovery slots and workflow records, no faults, previous charges or synthetic outcomes |
| `source` | Absolute isolated Git `directory` and full `base`, `good`, `candidate`, `main` SHAs; a single-parent good/introduction/no-op-main graph |
| `stable` | Official `sdkVersion`, publisher-checksummed `archive`, and `{ url, sha256 }` metadata identities |
| `files` | `{ role, path, sha256 }` raw inputs; includes `good-patch`, `introducing-patch`, `source-toolchain`, `stable-toolchain` and `metadata-*`; patches must exactly match Git and every sample/producer blob must remain identical across source revisions |
| `containmentDirectory` | Existing real Linux containment evidence matching `deployment.containment` |
| `runs` | Distinct decimal-string `controller`, `routing`, `supervisor` fake run IDs present in the reviewed database; each advance creates a fresh run from its corresponding template |
| `signature` | HMAC from exported `trial.sign(unsignedConfig, key)` using the independently held reviewed deployment key; supply matching keys through `RB_SIGNING_KEYS`, never the JSON or a worker |

The initial approval must contain one good-source hint and a fixed deadline with
at least 660 minutes remaining. Preparation does not self-approve a proposal.
The source fixture's placeholder discussion fingerprint must be replaced by a
review of the complete selected fake issue/discussion and resealed before signing
the configuration. The deployment JSON embedded in the API must serialize to the
same bytes as the reviewed deployment. No production registration is shipped.

Each advance holds a supervisor ownership lock and invokes **one** existing
transaction: discovery provisioning, an event-to-durable-route transition, or
delivery of the fixed dispatched controller operation. A controller advance can
charge at most one real worker. Routing never depends on model output. The
signed delivery receipt, API database/ledger, controller/routing journals,
reservations, launches, evidence store and process receipts persist between
commands. A lost response is retried through authenticated controller recovery;
it does not authorize replacement evidence or another allowance. Concurrent/live
owners block. A dead owner may resume on the same host; an interrupted handoff
guard or absent required evidence needs investigation, not deletion. Only owned
disposable acquisition/selection scratch is a cache. Never delete `store`,
`launches`, signed journals, `trial.json`, `delivery.json` or API state to recover.
A lost delivery receipt can first return `duplicate`, then route recovery on the
next advance. A claimed observation is recovered without executing again; an
unclaimed lost reservation stays charged even when a later attempt is allowed.
Missing claimed execution evidence blocks rather than launching a replacement.

`--verify` performs no API mutations or network requests. It requires the
authenticated published digest and fixed comment ID, matching complete expected
boundary, six distinct claimed/committed reservations, the exact phase order,
five builds/660 minutes, and hash-verified required physical collector evidence.
It re-reads the complete paginated issue discussion through production intake,
authenticating controller and discovery records before excluding them from the
approved discussion fingerprint. New corrections, spoofed workflow comments,
changed discovery records or active linked investigations block verification
without changing evidence or the persisted report identity.
It fails an incomplete or stopped trial. Optional artifact expiry cannot change
the already authenticated publication identity. JSON always carries the synthetic
acceptance purpose and `physicalAcceptance: false`: a verified local evidence
bundle does not itself sign off the separate product/containment or physical
known-history gates. Exit 1 prints the blocking reason; no failure refunds charges
or extends a deadline.
Verification also authenticates the ordered checkpoint/freshness writes before
every recorded bisect command and requires six distinct committing processes.
A missing restart trace or a mutation outside the selected fixed slots,
freshness anchor and controller dispatch blocks verification. The only mutable
trial fault is `resultAck: true`, which simulates losing the fixed fake result
write's response; worker, runtime and metadata fault injection remain forbidden.

#### Frozen source design (no historical product execution)

`fixtures\trial-source.cjs <absolute-new-directory> <absolute-git-executable>`
constructs the isolated full-source Git graph without changing this branch. It
reuses the actual workspace layout validator and revision declarations, freezes
raw official metadata, and checks Linux SDK/runtime archive availability with
bounded HEAD requests. Archive selection is shared with production acquisition
and matches the product, exact version, RID and archive extension; apphost packs
cannot stand in for the runtime. Missing or duplicate runtime entries block.
The shared clone depends on the original source object
store; retain those objects until the trial is exported or completed.

The reviewed base is `8c0e444de18218ecec91475a6022872e93299e10`: the existing
Linux Arcade product/bootstrap entrypoints and nested FSI/FSC projects pass
`inspectRevisionLayout`. Its own `global.json` specifies SDK/tools **10.0.301**,
and the product TFM is `net10.0`. Official Linux x64 SDK 10.0.301 and runtime
10.0.0 have publisher SHA-512 metadata and returned HTTP 200 on 2026-09-22
for `dotnet-sdk-10.0.301-linux-x64.tar.gz` and
`dotnet-runtime-10.0.0-linux-x64.tar.gz`. These refreshed receipts replace the
earlier apphost-pack HEAD check, which did **not** establish runtime availability;
no measured-provenance exception or Windows approval is reused. Latest stable
metadata at fixture freeze selected **10.0.401**; advancing rejects changed
metadata rather than silently replacing that endpoint.

| Revision | Full SHA |
| --- | --- |
| Good | `05e2f222e08785994d8909707054b411ea677fc4` |
| Introducing fixture change | `a23fa5514d49566559bdcc7479fbe00ceb01b577` |
| No-op bad main | `e648b05ced2bc8f071685a7dbfacbbc15964e2f9` |

Good adds exactly
`fsiConsoleOutput.Out.WriteLine("SYNTHETIC_ACCEPTANCE_SENTINEL")` at entry to
`FsiEvaluationSession.Run` in `src/Compiler/Interactive/fsi.fs`, plus the frozen
`regression-trial/repro.fsx` containing `printfn "TRIAL_SAMPLE"`. The introducing
commit restores the stock FSI blob using a normal source commit, not a Git revert.
The sample is unchanged. Good patch SHA-256:
`156935262500c7cc9f11dae8e3639af4495227f20f49a1e10077a5fe63722aa1`;
introducing patch SHA-256:
`3ca7bc75f1927bb378b3f9f76dcc2805795837659eb13d973c29d9b2f9e93768`.

The **same runtime assertion** expects exit 0, empty stderr, and stdout
`SYNTHETIC_ACCEPTANCE_SENTINEL\nTRIAL_SAMPLE\n` for pass; the expected failure
is exit 0, empty stderr, stdout `TRIAL_SAMPLE\n`. Stock released/main product
bytes are never modified. These are frozen expectations, **not measured product
outcomes**. The real runner must establish stable failure, clean main failure and
good SOURCE pass before starting any bisect command.

The six planned 110-minute units are stable, main, good, Git-selected candidate,
fresh candidate, fresh parent: **660/720 minutes**, five source builds, two source
attempts each for candidate and good. The existing 24-build/40-unit/two-attempt,
two-repetition, 90-minute-build and 300-second-repro limits are unchanged.
Any failed physical experiment remains charged; the fixture cannot be silently
replaced to manufacture a green trial.

The first native controlled trial, at `e66b5ac5a514b26baab77fc276e3f32f88c0f629`
in [owned-fork run 39806658165](https://github.com/T-Gro/fsharp/actions/runs/39806658165),
retains all six reservations and **660 charged minutes**. Its sixth clean parent
passed twice, but checkpoint serialization exceeded 48 KiB before that observation
could be committed. It is not a completed or published trial. The original
9,130,707,644-byte artifact has SHA-256
`384f5877b91abbc465a95f6022234fd3dc165496c78e96c8840936ac1a2293cb`.
Read-only authentication of its original checkpoint and retained parent
reproduced the overflow; the repaired encoding fits the same six-observation
checkpoint and full report without raising limits. A separately reviewed replay
uses the unchanged source graph/assertion and six-unit plan because the repair
changes immutable helper hashes. The original allowance, pending claim and
evidence are retained, never reset or refunded.

`trial.test.cjs` uses explicitly named **SYNTHETIC** product outcomes with real
CLI processes, Git selection, durable routing and signed reporting. It covers
endpoint stops, skipped/ambiguous/toolchain/revert/merge/backport histories,
six-unit/fresh-build accounting, full rendered SHAs and verified PR URLs, paginated
association membership, preparation and authority boundaries. A test-launcher-only
preload runs the actual `trial.cjs` entrypoint against the existing synthetic
worker fixture, exercising signed delivery receipts, live-owner exclusion,
process death at reservation/claim/retained observation/delivery/result write,
six-unit completion, and optional-expiry adoption of the same persisted result.
The normal, unmodified CLI then verifies that synthetic signed evidence bundle
read-only and rejects damaged checkpoint/result/run/evidence/plan/helper inputs
and stale or unauthenticated discussion records. Parameterized stable-identity
tests cover SDK, archive and metadata changes; subprocess bootstrap/advance tests
reject changed official metadata before download, reservation or bisect.
The preload is not a trial option and is never loaded by the acceptance adapter.
Offline source-freezing tests reproduce the exact graph and assertion while
rejecting apphost-only or ambiguous runtime metadata and unavailable archives. Existing
controller, collector, engine, attribution, report, acquisition and reproduction
suites retain cancellation/claim/retained-observation recovery, optional-expiry
lost-ack adoption, required-loss and stale-input rejection, hostile inputs,
runtime/diagnostic distinctions, identity fallback negatives, matrix coverage
and immutable old producer metadata. These cheap tests do not establish either
physical acceptance gate.

Harness validation on 2026-09-22: the stale-discussion CLI regressions first
returned `verified` incorrectly, then passed with read-only production intake.
All 65 focused tests passed in 446 seconds; the full scoped run passed all
2,537 tests in 660 seconds. All three independent on-disk report mutations failed
their authenticated classification, PR-link and skip assertions in under four
seconds each, with unchanged production hashes. Three changed CJS files passed syntax checks;
help and diff checks passed. Scoped evidence-helper Release build/test commands
exited 0 using installed SDK 10.0.112 (the helper has no managed test cases).
The source graph and patch hashes are unchanged; refreshed publisher metadata
and HEAD receipts identify the actual Linux SDK/runtime archives. RED/GREEN logs,
source-bound receipts, raw ledgers, graph/patches and fresh-process read-only
verification proof are retained privately. That September harness run performed
no historical product build, worker containment acceptance or physical
known-history trial.

**Native worker acceptance does not activate production, approve a reproduction,
or complete the separate known-history acceptance trial.**
The controller only supports `workflow_dispatch` of `bootstrap`, `advance`,
or `publish`, from the trusted router or an operator. Automatic intake remains
disabled by default and cannot execute an unapproved issue. There is no model
authority, arbitrary ref/URL/command, or provisioning dispatch input.

```powershell
node eng\regression-bisect\controller-cli.cjs --help
node eng\regression-bisect\controller-cli.cjs provision --issue 123 --config $TrustedConfig
node eng\regression-bisect\controller-cli.cjs bootstrap --issue 123 --config $TrustedConfig
node eng\regression-bisect\controller-cli.cjs advance --issue 123 --config $TrustedConfig
node eng\regression-bisect\controller-cli.cjs publish --issue 123 --config $TrustedConfig
```

`$TrustedConfig` is an absolute path to the reviewed
`.github/regression-bisect/deployment.json`, **not issue input**. Its raw bytes
must equal that path's Git blob at `RB_TRUSTED_REVISION`. The CLI accepts no
other arguments. Repository and workflow destinations are fixed to
`dotnet/fsharp` and `.github/workflows/regression-bisect-controller.yml`.
Exit 0 means the printed JSON is a successful transaction or explicit stopped
intent; exit 1 prints a blocked/requeue reason. An `observed` result means one
unit, not an attributed regression or permission to execute another unit.

### Reviewed files and first use

Use one immutable, reachable main-branch commit as `anchorSha` for all
registrations. **Never change the anchor to bypass missing history.** Review
these deployment fields; unknown fields are rejected:

| Field | Required value |
| --- | --- |
| `schemaVersion` | `1` |
| `workflowPath` | `.github/workflows/regression-bisect-controller.yml` |
| `actor` | `{ id, login, type: "Bot", appId }`: exact publishing GitHub App bot account and app IDs |
| `anchorSha` | Full immutable 40-hex main ancestor; retain permanently |
| `retainedWorkflowRevisions` | Full SHAs of earlier trusted workflow/helper revisions whose records remain live |
| `keyId`, `keyHashes` | Active key ID and map of all retained key IDs to SHA-256 of their raw secret bytes |
| `platform` | `{ "os": "linux", "architecture": "x64", "rid": "linux-x64" }` |
| `gitExecutable` | Absolute trusted host Git executable, normally `/usr/bin/git` |
| `container` | Exact adapter configuration shown in the physical-container section below |
| `containment` | `{ reportSha256, configSha256, receiptSha256 }`: raw report/config hashes and a map from all seven smoke modes to their raw `receipt.json` SHA-256 hashes |

Provisioning is a two-review transaction, not self-approval:

1. Review deployment configuration and
   `.github/regression-bisect/provision/123.json`, exactly
   `{ "schemaVersion": 1, "allowanceId": "<maintainer-selected-unique-id>", "deadline": "<absolute-UTC-deadline>" }`.
   The deadline must leave time for the full reserved worker units; it never
   slides on retries. Use the pinned checkout and a real run/attempt of this
   trusted workflow as provenance. Provisioning alone can use an already
   completed run; executing/writing checkpoints requires the current run to
   be `in_progress`. Supply the separate maintainer credential below.
2. Run `provision`. The CLI positively reads every status page at the fixed
   anchor and every selected-issue comment page before creating anything.
   It persists a signed local journal and remote signed provisioning receipt
   before creation, then records creation intent before each POST. Returned
   real comment IDs are bound into the signed checkpoint/result proposals.
   A lost create acknowledgement is recovered by the exact authenticated
   creation marker; zero or multiple matches block rather than POST again.
   Missing journals, slots, receipts, inaccessible history, forged records or
   another allowance block replacement. Keep the private journal permanently;
   do not manually recreate its contents or remove its creation intent.
3. Review the emitted `registration` object and copy the generated
   `<RB_STATE_DIRECTORY>/dotnet-fsharp-123/registration.json` to
   `.github/regression-bisect/registrations/123.json`. Its contract is
   `registration.schema.json`; the two IDs must differ. Freeze and review the
   complete discussion, corrections, linked investigations, manifest and raw
   source/producer hashes using the existing intake contract. Review
   `.github/regression-bisect/approved/123.json` with exactly
   `{ "schemaVersion": 1, "manifest": <reviewed-manifest>, "bootstrap": <registration> }`.
   The CLI requires identical registration and approval bootstrap objects.
   Fixtures, labels and provisioning output are **not** reproduction approval.
4. Retain the provisioning workflow revision and keys in deployment config,
   pin the reviewed approval revision, and run `bootstrap`. It consumes the
   existing signed slots without creating comments, initializes generation
   zero once, and recovers a lost generation-zero acknowledgement without
   changing allowance, deadline or original slot digests.

Creation intent with no recoverable comment is deliberately blocked: GitHub
comment POST has no cross-process idempotency key. Absence cannot distinguish a
request that never reached GitHub from a deleted just-created comment. An
operator must repair the original records with independent evidence; rerunning
does not create another allowance. Provisioning also refuses surviving signed
slots if both its local registration and remote provisioning receipt are lost.

### Trusted inputs, secrets and workflow

Provision one dedicated Linux x64 controller host/runner label
`regression-container-v1`, with persistent, controller-only state at
`/var/lib/fsharp-regression-controller`. Do not run independent controllers
on different hosts against separate state directories. All workflow modes
share `regression-bisect-dotnet-fsharp-<issue>` Actions concurrency with
`cancel-in-progress: false`; local processes additionally share the directory
lock for that same repository/issue. No job split transfers ownership.

Set protected environments `regression-controller` and `regression-routing`,
restrict them to main, and review deployment/approval policy before activation.
For reviewed unattended continuation, do not configure a per-job required-reviewer
gate: the pinned approvals and immutable allowance govern each unit. Initial
provisioning and reproduction approval remain explicit owner actions.
The controller environment's exact secret names are:

| Secret / explicit input | Purpose |
| --- | --- |
| `REGRESSION_CONTROLLER_TOKEN` / `RB_GITHUB_TOKEN` | Trusted publisher App token: only contents read, actions read, issues write and statuses write |
| `REGRESSION_CONTROLLER_SIGNING_KEYS` / `RB_SIGNING_KEYS` | JSON object mapping reviewed key IDs to base64 secrets of at least 32 random bytes; never put values in tracked files |
| `RB_PROVISION_TOKEN` | Separately authorized operator-only token for the **same reviewed App identity**; never provided by the execution workflow |
| `ACTIONS_RUNTIME_TOKEN`, `ACTIONS_RESULTS_URL` | Runner-supplied artifact identity, available inside the local Node action; never supplied by dispatch or copied to workers |

The routing environment has separate `REGRESSION_ROUTING_TOKEN` (the same
reviewed App identity, contents/actions read and issues write for its two fixed
state slots, **no statuses write**), `REGRESSION_ROUTING_SIGNING_KEYS` (the
reviewed retained key set for state authentication), and
`REGRESSION_DISPATCH_TOKEN` (Actions dispatch only). GitHub token scopes are
repository-wide, so fixed comment IDs and workflow destinations are also
enforced in deterministic code; neither token is exposed to the model.
Signing keys are trusted controller infrastructure, never model inputs.
Only the controller receives selected-issue publication/status authority.
Historical workers receive none of these secrets, the job GitHub token, or
the artifact runtime token; the Docker boundary uses an explicit clean
environment and no credential mounts.

Set `REGRESSION_CONTROLLER_REVISION` to a full reviewed immutable main SHA.
Set the repository-level `REGRESSION_CONTROLLER_ENABLED` variable to `true` only
after the independent physical and known-history gates pass. Set
the repository-level `REGRESSION_ROUTING_ENABLED=true` only after the same gates, reviewed discovery
registration and explicit provisioning. Unset variables disable both paths.
The actual seven-case supported-container attestation remains mandatory in the
controller before any worker, not a label or model assertion. Checkout is
SHA-pinned with `persist-credentials: false`. The pinned local Node action
executes `controller-cli.cjs`, preserving the artifact runtime credentials
without exposing them in shell interpolation or outputs. It passes typed
`toJSON(inputs)` through `RB_ACTION_INPUTS`; only an object containing a positive
safe-integer issue (not a string) and one of the three transaction modes is
accepted. The optional `routingKey`/`routingGeneration` string pair must either
both be empty (an operator transaction) or bind the exact routed transaction.
Extra fields, provisioning and malformed JSON block before
configuration/API access; payload bytes are not echoed in diagnostics.
The no-argument Actions entry always selects the pinned checkout's
`.github/regression-bisect/deployment.json`, never a dispatch-provided path.
There are no PR/contents writes,
push, merge, revert, close or arbitrary dispatch operations.

For explicit operator invocations, set `RB_TRUSTED_REVISION`,
`GITHUB_WORKFLOW_SHA` to the reviewed revision, `GITHUB_REPOSITORY=dotnet/fsharp`,
`GITHUB_REF=refs/heads/main`, and `GITHUB_RUN_ID`/`GITHUB_RUN_ATTEMPT` to the
authenticated workflow run/attempt. Set absolute `RB_STATE_DIRECTORY` and
`RB_CONTAINMENT_DIRECTORY`. Do not invent a run ID. The CLI checks the run's
actual repository, attempt, workflow path, branch, SHA and status. A moved main
workflow SHA requires repinning reviewed helpers/configuration; an issue cannot
select an older workflow. The checked-out helper tree must be clean and both
the revision and anchor must remain reachable from fetched main.

Keep the complete native containment smoke evidence at
`/var/lib/fsharp-regression-containment`; copy the exact original smoke
configuration there as `container.json`. Review the raw report/config and all
seven receipt hashes. The configuration bytes must match the deployment's
container settings.
The CLI requires all seven passed cases, matching current smoke/helper bytes,
four terminated/removed resources per case, and the same daemon ID, version,
kernel, OS, cgroups and security options as current runtime inspection.
Native Linux x64 Docker >=28, cgroup v2, AppArmor, builtin seccomp, resource
controllers and the pinned image remain mandatory. Missing attestation,
different helper bytes/daemon, Windows, rootless Docker, image mismatch or
missing artifact authority blocks before any historical execution.

Rotate keys by adding the new key ID/hash and secret, retaining the old IDs,
then selecting the new `keyId`. Complete provisioning before rotating its
journal key. Retain verification keys and workflow revisions for **at least**
the entire allowance and all evidence/report retention, including recovery.
Removing/swapping a required key blocks; it never initializes replacement state.
Do not print the secret JSON or store it in a manifest, sample or journal.
`RB_LOCAL_TEST_FILE` requires explicit `RB_LOCAL_TEST=explicit-offline-fixture`
and is rejected whenever `GITHUB_ACTIONS` is set. It is a local test facility,
not a production API endpoint or runner override.

### Units, durability, memory and timeouts

`bootstrap`/`advance` reread complete issue/discussion/assets/investigations.
The controller resolves current official released stable metadata and downloads
the exact archive through the existing bounded acquisition HTTP path to verify
SHA-512 and measure SHA-256, **without extracting or executing it on the host**.
An installed/preview SDK cannot substitute. Source selection uses a clean
credential-free Git graph; only the approved public unit, read-only pinned
helpers, approved bytes and a selected-source bundle enter `runContainer`.
The bundle contains only the selected ref and its reachable history, not host
Git configuration or credentials. CLI and adapter share a **1 GiB** bundle
ceiling (inclusive), covering the measured 401,775,766-byte main history.
Both hash it as a stream; the adapter verifies the staged copy again before
creating worker resources and mounts it read-only. Allow up to **2 GiB of host
disk per source launch** for original and staged bundles, in addition to the
history workspace and evidence. Oversized, missing, linked or digest-mismatched
bundles block; a claimed reservation is never refunded or silently relaunched.
Acquisition, existing `eng/build.sh`/`eng/Build.ps1`, local-compiler/Core injection
and reproduction runners execute only inside that adapter. SDK-update CI and
`BuildWithLocalFSharp.fsx` shell input are not bisection execution mechanisms.

`advance` reserves/claims at most one unit, including a fresh confirmation
candidate or parent. Stable pass, main pass, setup failures, flakes and no
source-green interval stop/block before **any** Git bisect operation. Current
input/main/stable/image changes invalidate evidence, never cumulative counters.
On `advance`, the CLI reconciles fetched main with the saved
epoch under the issue lock, including when stable evidence will immediately
stop or block. Unchanged stable evidence is retained; main changes alone do not
charge another stable worker. `publish` still requeues stale main without
rewriting a result: run `advance`, then `publish` for a reportable blocked
investigation. Passing endpoints stop execution but can now publish a guarded
unconfirmed report. Neither path resets the allowance or extends its deadline.
The 24 builds/40 units/720 minutes/two attempts and repetitions/90-minute build/
110-minute worker/300-second reproduction limits are unchanged. Six
reservations cost 660 minutes even if they finish immediately.

Host collection reads bounded inert JSON/archive bytes, verifies raw launch,
identity, acquisition/build/assertion bindings and signs only after verification.
Required bytes must be durably stored before clearing pending work.
The Actions store uses authenticated repository/run/attempt/artifact ownership
and a fixed results authority, with 90-day retention. Keep the allowance
deadline within available retention and retain the host's original launch
receipts and provisioning journal. Required retention loss blocks publication
and recovery, even if an acknowledgement was lost; optional availability is
not recomputed before adopting an authenticated saved result digest.

Controller metadata/evidence streams are bounded (1 GiB per archive; 3 GiB
per launch; 64 MiB small evidence lane). Base64 collection and JSON parsing can
temporarily hold several copies of that lane; provision at least 1 GiB of
host-controller memory **in addition to** configured worker memory/scratch.
Do not load acquired SDK archives into a Node Buffer; storage streams them.
Comment envelopes stay below 48 KiB and human reports below 32,768 UTF-8 bytes.

The finite Actions job timeout is 135 minutes; internal workers retain their
original absolute reservation deadlines. API calls and metadata/transfer
operations have their own bounded deadlines. A timeout is not a lock release:
the CLI awaits the repaired store's outstanding quota-mutation barrier before
handoff. A killed local owner can be reclaimed only after its PID is absent
and its exact old run/attempt has completed. A live PID, different host, unknown
run or interrupted handoff guard blocks for operator investigation; never
delete an ownership marker merely because a timer elapsed.

An unclaimed cancelled reservation can incur a charged retry, without a refund.
A claimed reservation first resumes authenticated retained/remote collection,
not another worker. A completed old owner can be committed by the new active
run only after authenticating the old owner and the original execution claim;
the original deadline, build ID and evidence epoch still apply. Recovery checks
the clock after collection and after required evidence verification, not just
at intake. Before a recovery run uploads, its run/attempt is saved in the
authenticated checkpoint. Subsequent recovery searches every recorded uploader
and rejects conflicting collections; losing per-launch scratch after a durable
upload does not require another worker or charge. Keep the checkpoint's
uploader records and each uploader's artifacts for the same 90-day retention.
Missing or forged collection blocks. `publish` only uses the fixed registered result slot,
rereads current inputs/main/stable and required evidence immediately before
writing, and adopts an authenticated lost acknowledgement without a second
result write. After an evidence epoch refresh it may replace that slot's prior
authenticated result only if the saved allowance history binds its exact
epoch and report digest to the registered slot.

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

All records use `schemaVersion: 1`. Record validators return `true` or throw a
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
| `validateRepositoryPath(path)` | The same bounded portable repository-relative path check used by manifests and worker transfers; rejects traversal, absolute/UNC/device paths, alternate streams and encoded paths. |
| `validateManifest(manifest)` | Frozen repository/issue, discussion fingerprint, reproduction kind, reviewed source hashes, exact positive/failure assertions, matrix, timeout, pinned producer/packages, search hints, and approved-input digest. |
| `fingerprintInput(json)` | SHA-256 of canonical JSON: sorted object keys, preserved array order and string bytes. Rejects non-JSON values. It never removes comments based on markers, authors, or claimed authentication. |
| `classifyAttempts(attempts, manifest)` | Checks both required repetitions against the frozen assertions and all setup/identity checks. Returns a versioned outcome with an explicit reason. |
| `validateOutcome(outcome)` | Only `pass`, `product-failure`, or `untestable`, with a reason appropriate to that outcome. |
| `validateObservation(observation, manifest, options?)` | Validates target, all reproduction fingerprints, matrix evidence, local artifacts, provenance, and recomputed outcomes. |
| `fingerprintEpoch(manifest, budget, pinnedMainSha)` | Validates the manifest and trusted budget context, then binds the manifest, pinned main, resolved stable release, and policy/workflow revisions. |
| `validateState(state)` | Validates the shared persisted-state shape, epoch, authenticated evidence references, cumulative budget, and any charged pending reservation. |
| `admitBisection({ manifest, stable, main, good, ancestry, budget })` | Fail-closed gate for a verified source interval; no execution side effects. |
| `validateBoundaryEndpoints(input)` | The same authenticated endpoint/ancestry checks without capacity for a new worker. Used only to consume confirmation evidence; Git selection still requires `admitBisection`. |

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
`requestedTools`, `requestedCompilerRuntime`, the full SDK `archive` receipt, verified `sdk` and
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

Only `engine.cjs` spawns `git bisect`. It calls the admission gate immediately
before constructing Git state, including every resumed reconstruction. Endpoint
evidence is already durably authenticated at that point; the selected source
worker is durably reserved before execution.

## Endpoint-first controller seam

`advanceInvestigation({ now }, dependencies)` re-reads the complete issue through
`readIssueSnapshot` (including `findActiveInvestigations`), re-resolves official
latest-stable metadata, and returns **at most one** charged `work` reservation or
a `stopped`, `blocked`, or `confirmation` report intent. No engine result is
`complete`, a published report, or an attributed culprit. Controllers must hold
the existing whole-issue concurrency lock across state transitions.

Required trusted dependencies:

* `context` and `transport` use the authenticated state API described below.
  An existing initialized allowance is mandatory. Missing, empty, forged or
  rolled-back state is blocked, never initialized by this engine. Approval and
  policy/workflow revisions come from the pinned trusted default-branch loader.
* `approval` supplies intake's `readSource`, `authenticateRecord` and
  `readTrustedCheckpoints`. These callbacks verify pinned raw reproduction and
  producer assets and authentic workflow publications, not issue-supplied flags.
  `intakeTransport` may supply separate read-only GitHub access.
* `platform` identifies the target worker. `environments` maps **every** matrix
  cell ID to `{ os, architecture, imageDigest }`. Its canonical fingerprint is
  part of the authenticated epoch. Changed images invalidate evidence without
  resetting charges.
* `history` supplies absolute `gitExecutable`, existing `scratchDirectory`,
  trusted full `mirrorDirectory`, and `mainRef: "refs/heads/main"`. The engine
  fetches into its own disposable repository with no checkout, empty hooks,
  isolated configuration, disabled credentials/replacement objects, and local
  transport only. It never runs Git in the caller's checkout or mirror.

The initialized context includes the exact released SDK archive SHA-256
identities already established by trusted acquisition. Every advance resolves
official metadata at `now` and checks its version/index digest against that
context. Changed metadata or a new release blocks stale execution: the controller
must refresh the authenticated stable identity through the existing acquisition
boundary and invalidate the epoch, **not** create another allowance or substitute
an installed SDK. The worker reacquires and verifies the matching archive under
its charged reservation. Main is fetched and pinned only after a verified stable
failure; the saved SHA precedes its build. A fresh process can recover that pin
from authenticated state without remembering a prior return value.

The order is stable failure, pinned-main failure, older-source pass, admission,
then Git selection. Stable pass stops as not reproduced; main pass stops as
fixed/release-specific. Untestable stable/main endpoints block. The deterministic
older search considers approved source-SHA hints that ordinary ancestry queries
verify, then oldest-first candidates in a bounded 4,096-ancestor main window.
SDK/version/URL claims are not converted to source observations or trusted tag
mappings. Every selected source still requires a clean build and repeated sample.
Search work is bounded by that window and the unchanged issue allowance.

Admission requires comparable sample SDK/runtime, environment, producer, matrix
and exact assertions, but not equal revision **build** SDKs. Source observations
used by the engine must retain `buildToolchain`. Different build toolchains are
returned as transition evidence, never erased to make endpoints comparable.

`executeInvestigationWork(work, dependencies)` revalidates the returned permission.
It needs the same current trusted context, intake/approval readers, official
metadata resolver and (for source work) history mirror as selection. It re-reads
complete discussion, immutable assets, eligibility and active investigations,
checks the authenticated epoch and binding, re-resolves latest stable, and
checks current main against the saved pin before claiming execution. The saved
`binding` cannot override current issue data; the returned `resolved` record is
not used instead of execution-time release metadata.

It then calls `beginWork` before invoking any runner, making a lost acknowledgement
or duplicate worker invocation non-executable. Deadline checks surround
asynchronous claiming, acquisition, inventory and collection; the abort signal
uses the reservation's remaining time, not a fresh worker timeout. An expired
claim cannot start a runner even if its checkpoint write completed successfully.
The state ledger keeps the reservation owner, attempt and execution generation.
Rejected, expired and cancelled work is not refunded. A cancelled owning run is
retired only through `reserveWork`, `invalidateStaleEvidence`, or
`cancelCompletedWork({ generation })`; retrying costs
another full reservation and never more than two attempts per item.

Worker dependencies include `worker` options for the existing runners and
`inventoryReleased(identity, buildInstanceId)`. Inventory that requires loading
products belongs inside the isolated worker, along with acquisition, builds and
reproduction. `collectObservation({ result, reservation, manifest, context })`
remains a **trusted collector** boundary outside execution: authenticate captured
worker provenance and required artifacts and return `{ observation, evidence }`
in the existing state format. It must only hash/parse bounded data, never execute
child output or run an inventory probe. A worker setting `verified: true` is not a collector.
The engine checks that successful collected assertions are the original runner's
matrix repetitions and that source SHA/build IDs match the reserved work. Work
results carry only public epoch inputs in `binding`, never controller keys.
Execution takes authentication context from trusted dependencies, not the work
record. Never send those dependencies to an untrusted worker.

Released execution uses `acquireToolchain`, `verifyToolchainIdentity` and
`runReproduction`. Source execution uses `testSourceRevision`, which reads each
revision's declarations, acquires its own build toolchain, and creates fresh
Bootstrap/Proto, bin/obj and package state. `worker.reproductionToolchain` is
required separately for engine source work; optional `sampleRuntimes` freezes
explicit sample runtimes. A source-derived sample SDK retains its acquisition
`requested.sourceSha` as provenance, independently of the tested product SHA.
The workspace/engine bind the product to the reserved source; the reproduction
runner still verifies its local artifacts, host, archive and actual compiler,
sample-runtime and Core loads. Compatible historical projects may therefore
run a net9 compiler with the frozen SDK 10/net10 sample without retargeting.
The workspace runner accepts a reserved `buildInstanceId`; reusing its directory
fails rather than reusing outputs.
Unsupported historical runtime/sample combinations remain untestable. There is
no engine-specific FSI command, compiler runner, SDK installer, retargeting or
`global.json` rewrite.

The persisted interval contains good/bad SHAs, the legacy `gitLog`, and strict
`decisions: [{ sha, outcome, observationDigest }]`. It does not store the
potentially thousands of remaining SHAs in the 48 KiB checkpoint. Report intents
include `interval.remaining: [sha]`, recomputed from Git on each advance.
Reconstruction uses all valid comparable source observations in the interval,
including failures and skips from older-endpoint search, not just bisect workers.
Decisions are saved with observations; the legacy `gitLog` is never executed.
A lost selection worktree is reconstructed using
`bisect start --no-checkout` and typed `good`, `bad`, or `skip` arguments only;
there is no stored replay string, `bisect run`, or interpretation of generic
nonzero build exits as bad. Each selection pass has a five-minute wall-time cap
and each Git subprocess a one-minute cap. All-skipped results retain possible
commits and skip reasons.

Observed descendant passes after failures, branches outside main ancestry,
merges in the asserted interval, and explicit revert/reintroduction/cherry-pick
messages stop automatic attribution with history/interval uncertainty. This
bounded inspection does not prove arbitrary histories monotonic. Merges require
explicit parent analysis rather than first-parent blame.

The policy remains 24 source builds, 40 workers, 720 reserved minutes, two work
attempts and two assertion repetitions, with 90-minute builds, 110-minute
workers and 300-second reproductions. **Six reservations consume 660 minutes**;
the seventh does not fit even when each fixture finishes immediately. Candidate
selection preserves room for candidate-plus-parent confirmation where feasible.
Admission has no next selected item yet, so it must not mistake the ledger's
last completed item for that item after a successful retry. Actual work
reservations still enforce the two-attempt limit. Confirmation checks both
aggregate allowance/deadline and the remaining attempts for each concrete
candidate and parent, including their cancelled reservations under the same
approved input. Otherwise it returns the unconfirmed interval and consumed
budget. A singleton interval returns `confirmation` with `candidate`, all
relevant `parents`, and `requiresFreshConfirmation: true` only when those fresh
reservations fit; a separate consumer must reserve and perform them before any
attribution.

`engine.test.cjs` uses real temporary Git histories and inexpensive injected
revision evaluators, not product acceptance. It records every actual bisect
subprocess and exit code before checking saved endpoint evidence, so an engine
catch cannot hide a forbidden invocation. This includes zero counts for all
denied endpoint tuples and malformed/stale evidence. Process/GitHub/clock
dependencies are injectable at the existing runner boundaries; production
defaults call the original modules.
The scoped suite covers execution claims, cumulative cancellation/retry budgets,
environment/input/main/stable epochs, lost worktrees, skips, toolchain transitions
and uncertain histories. Real 1,000- and 4,096-commit intervals exercise compact
checkpoint round-trips. Boundary cases check concrete confirmation attempts,
the last two worker units, exact deadline capacity, and rejection of worker
outcome/source/build/matrix/repetition mismatches without committing evidence.
Reservation-to-execution edits and clock advances during asynchronous setup
must start no stale or expired worker. Shared project evidence fixtures exercise
the real workspace/reproduction seam with distinct historical compiler and
frozen sample runtimes, for both released and source-derived sample acquisitions.

Fresh Windows x64 product acceptance is retained at
`Q:\rb912-engine-hardening-20260920`. The original workspace runner clean-built
`5a8b9adb034f420a5c2c1f577670c5eeeb454b7e` in build instance
`fbe0059e-2dfd-4ce2-b160-58a65f6092b9` using its approved measured
`11.0.100-rc.1.26420.103` archive, with unchanged `global.json` and two passing
FSI assertions. Released SDK `10.0.401` was reverified against current official
metadata and its retained archive. It and the fresh source products then matched
all 43 existing reproduction-smoke expectations (23 passes, six intended
product failures, 14 identity rejections). The scoped evidence-helper Release build/test
commands exited 0; that helper project contains no managed test cases.
The targeted engine/model/state suite passed 525 tests and the full scoped suite
passed 1,044, including 100 zero-bisect cases and 37 actual bisect subprocesses.
Seven in-memory mutations were detected: bypassed current intake, released SDK
and main freshness, removed asynchronous deadline checks, omitted sample-runtime
forwarding, recoupled acquisition/product source provenance, and a forbidden
pre-admission bisect reset. Repository files were not mutated for those probes.
`final-acceptance-report.json` has SHA-256
`02ef9214a9835070a846ab1cc13678a8ebe0617cdc01e3f717cba60628042528`.
This validates the original product runners, not a real regression's admitted
interval, an activated workflow, or fresh culprit attribution.

## Fresh boundary confirmation and association

`attribution.cjs` exports `confirmBoundary({ intent, now }, dependencies)` and
`resolveCommitAssociations({ repository, sourceSha, pinnedMainSha }, dependencies)`.
All dependencies are trusted controller services, not worker-authored JSON.
The controller must hold the existing whole-issue lock throughout each confirmation
transaction and execution claim.

Confirmation uses the engine's dependencies and its returned `confirmation`
intent, or a `blocked` intent retaining an interval for explicit parent/history
analysis. Generation, phase, interval and authenticated candidate failure must
match the current checkpoint. Endpoints are read from authenticated observations,
and admission is rechecked against actual Git ancestry. Caller-supplied parent
lists are checked against commit objects, never taken as ancestry evidence.
The optional `associationTransport` is the read-only GitHub `request` transport.
Without it, association evidence is `null` (not looked up), not a claim that no
PR exists.

The engine exports its existing `currentInput`, `historyWorkspace`,
`authenticated` and `capacity` helpers for this composition. There is no second
build or assertion evaluator. Each candidate and relevant parent is separately
reserved by `confirmBoundary`, then claimed and run by the caller through
`executeInvestigationWork`, hence
`workspace.testSourceRevision` and `repro.runReproduction`. Whole-parent-set
capacity is checked before the first run. Every run consumes the existing
allowance; rejection, cancellation and changed inputs never refund it.
New build IDs must differ from every search observation and one another.
`validateObservation` receives the new reservation's `finalConfirmation`.
The reviewed producer's raw bytes are reread through `approval.readSource`
using the manifest source with its SHA-256. An absent producer must use the
runner's existing `no-producer`, `0.0.0`, empty-byte digest identity.

`confirmBoundary` never invokes a build. It persists an unconfirmed plan with
the original starting generation, then returns at most one ordinary engine
`{ status: "work", reservation, binding }`. The caller may execute that one unit
and must end the invocation rather than loop over the plan. On resumption,
`advanceInvestigation` returns the saved confirmation intent without any
`git bisect` subprocess; it still rechecks current input, stable identity and main.
Calling `confirmBoundary` consumes independently claimed, committed confirmation
observations newer than the plan, checks capacity for **all remaining** targets,
and reserves only the next missing target. A lost result acknowledgement does
not require another build. Completing a merge at six units/660 minutes needs
no seventh reservation.

Cancellation after reservation or claim is charged even if no build started.
`cancelCompletedWork` requires the exact generation and verifies that the owning
run completed; it cannot cancel an active or unknown owner or refund any charge.
Cancelled attempts still count toward each source's two-attempt limit. Completed
observations and the original plan survive process/owner changes; a cancelled
attempt that exhausts capacity blocks rather than replaying under a new allowance.
Confirmed plans return the same result without another write or build.

Terminal results are `{ status: "confirmed" | "unconfirmed" | "blocked", reason,
confirmation?, generation?, budget? }`. A successful or completed uncertain
analysis is saved through `state.recordConfirmation`; a rejected worker still
owns its charged pending reservation and cannot be silently retried.
`state.confirmation` is optional for older checkpoints, but strict when present:
it binds epoch, manifest digest, starting generation, candidate, candidate/search
observation digests, all parents with comparability/outcome references, remaining
interval, history, optional associations, and `latentRootCause: "unproven"`.
Authenticated checkpoint loading checks each fresh observation's charged
confirmation reservation and execution generation.

This API is not a credential-isolation boundary. Direct engine calls retain
in-process runner defaults for compatibility; never deploy those defaults with
untrusted historical execution beside credentials. The controller transaction
requires a separate `runIsolated` adapter and has no in-process fallback.
A disposable production adapter and real containment acceptance are required
before enabling an unattended controller.

## Controller transaction library

`controller.cjs` exports `bootstrapController(input, dependencies)`,
`runController(input, dependencies)` and `publishController(input, dependencies)`.
All three accept exactly
`{ repository: "dotnet/fsharp", issueNumber: <positive integer> }`.
The issue must match the trusted context. Extra dispatch fields, commands,
refs, destinations, Markdown and fresh-allowance controls are rejected.
These are library entry points; `controller-cli.cjs` provides the separate
concrete deployment.

`runController` requires the engine dependencies above, a whole-issue
`withLock(action)`, `runIsolated(unit, { signal })`, and the trusted
`collectObservation` callback. It loads an existing authenticated checkpoint;
it never initializes or replaces a missing allowance. Inside the lock it
advances the existing engine, consumes/reserves confirmation work if selected,
and executes at most one unit. It returns `observed` with the exact outcome,
work ID, generation and consumed budget, or the engine/confirmation stop,
block or terminal reason. It does not loop into the next worker or publish
automatically. A lost observation-write acknowledgement is recovered by the
existing checkpoint/receipt chain, not another execution.

Immediately after the durable claim, the engine constructs a fresh public
JSON unit with the manifest, request, issue, epoch, build ID, persisted deadline,
platform, resolved stable toolchain and policy/workflow/environment digests.
`worker.validateExecutionUnit` rejects unknown fields and malformed bindings.
Only this data and an abort signal reach `runIsolated`; no state transport,
HMAC key, authorization context, comment destination, collector or executable
callback crosses that seam. Changing the supplied unit does not mutate the
controller's manifest or reservation.

`worker.runExecutionUnit(unit, dependencies, { signal })` is the extracted
physical runner. Inside a disposable boundary it composes the existing
toolchain, workspace and reproduction helpers, verifies its trusted policy
bundle digest and uses the remaining reservation deadline. Local helper paths,
raw reviewed assets, exact reproduction toolchains and inventory callbacks
come from the worker's separately pinned read-only bundle, not issue text.
The extraction does not alter historical `global.json`, install policy,
producer bytes, FSI selection or project identity verification.

**Production activation remains disabled.** Native worker acceptance is recorded
above; it does not provision deployment authority or complete the known-history
trial. The execution
unit is public data, not an authenticated launch receipt or permission to execute
it. The Docker transport and durable collector below do not activate a
controller/bootstrap CLI or scheduled workflow.
The transaction tests use fake transports and runner
callbacks and therefore do not prove confinement. Do not implement
`runIsolated` as an in-process callback, stripped environment or ordinary
credentialed subprocess in production. No workflow or secret names are
configured by this library.

### Bounded public egress transport

`egress.cjs` exports `createEgressBroker({ audit?, limits?, resolve?, connect? })`,
returning `{ server, snapshot(), close() }`. It is an HTTP CONNECT transport
component, **not a container adapter or a network-isolation guarantee**.
It does not listen automatically, install a service, configure a host firewall,
read credentials, launch historical code or grant work permission.
`resolve` and `connect` are trusted test seams; production defaults use Node's
DNS and TCP implementations. `audit` receives bounded-shape connection/denial/
closure records, without request headers or error text that could contain
credentials; the default writes JSON lines to stderr.

CONNECT is accepted only for exact, lowercase `host:443` authorities. Hosts
are the pinned policy's SDK public-feed hosts, plus `api.nuget.org`,
`globalcdn.nuget.org` and `pkgs.dev.azure.com`. Public package redirects also
permit one valid DNS label beneath `vsblob.vsassets.io`.
There are no caller-supplied host additions. Plain HTTP, IP authorities, user information,
alternate ports, credentials/cookies, duplicate headers and unknown headers
are denied. Each tunnel resolves all IPv4 answers once, rejects the entire
answer set if any address is non-public, then dials a validated **numeric**
address without a second lookup. IPv6 and private, loopback, link-local,
metadata, documentation, benchmarking and multicast/reserved IPv4 ranges
are unsupported, not alternate routes.

The hard ceilings are 128 simultaneous tunnels (including pending DNS),
2,048 accepted TCP connections per broker lifetime, 1 MiB uploaded and 1 GiB downloaded
per tunnel, 8 GiB transferred across all tunnels, ten seconds to receive
headers and five minutes absolute lifetime per tunnel, including queueing and DNS/connect.
Up to 128 additional requests can wait for admission without starting DNS or an upstream connection.
The queue preserves FIFO order and the original connection, byte, and lifetime charges.
Before the upstream connection, each request buffers at most 64 KiB.
The existing 1 MiB upload limit still applies across the entire tunnel.
When requests are waiting, the broker reclaims established tunnels after 30 seconds of inactivity.
Actual byte transfers reset that idle interval.
Without waiting requests, the broker does not reclaim idle tunnels before their absolute deadline.
`limits` may only lower those ceilings. Byte limits are checked before
forwarding; backpressure is preserved. Failures and timeout close both owned
socket ends, shutdown is idempotent, and a late DNS answer cannot create a
connection after cancellation. Rejected requests and transferred bytes remain
charged in the broker's counters. The connection ceiling also counts malformed
and non-CONNECT traffic; reaching it stops the listener rather than allowing
unlimited denial logs or resetting counters. These are transport counters, not a new
investigation allowance; the existing worker/build/time budgets are unchanged.
The active-tunnel ceiling fits the broker's unchanged 1,024-descriptor limit.
`snapshot()` reports peak active and queued requests, idle reclamations, and denial counts without URL paths or query strings.
The broker publishes these counters atomically in its private scratch directory.
The adapter reads the bounded counter file before cleanup, including after worker export failures.
It verifies broker ownership and includes the counters in the transport receipt.
If both operations fail, the receipt retains both errors.
Missing or malformed counters block collection but do not skip owned cleanup.

TLS remains end-to-end: the broker does **not** inspect URL paths, HTTP
redirects, checksums or decompressed content. Existing toolchain acquisition
must still enforce exact approved URLs/redirects, publisher checksums or the
narrow measured-archive exception, and bounded archive consumption. A redirect
requiring another host needs another permitted tunnel; unlisted NuGet/CDN
hosts remain blocked rather than expanding the allowlist automatically.

A disposable adapter must make this broker the worker's **only** network
route and keep the broker and its listener away from controller credentials,
host control sockets and untrusted modifications. Setting `HTTPS_PROXY`
alone does not enforce that property. The opt-in Docker adapter below creates
that network topology; it is not wired to an activated controller. Do not
enable unattended execution using this module alone. The socket tests use
owned loopback servers and injected DNS/dialers, never public downloads or
live GitHub writes. Their denials, transfers, quotas and shutdown ledgers
exercise the actual TCP/HTTP implementation but do not prove container,
process, mount, metadata or host-network isolation.

```powershell
node --test eng\regression-bisect\egress.test.cjs eng\regression-bisect\worker-output.test.cjs
if ($LASTEXITCODE -ne 0) { throw 'Egress and transfer tests failed' }
```

### Disposable Docker transport and containment acceptance

`container.cjs` provides `validateContainerConfig`, `inspectRuntime` and
`runContainer(config, options)`. This is a concrete, opt-in transport, **not a
deployed controller**. Neither unit
tests with fake Docker responses nor an unsupported-host receipt establish
containment. The collector authenticates and retains its output as described
below. The concrete controller/workflow still requires reviewed deployment and
the known-history trial before activation; native worker acceptance is recorded
above.

The supported runtime is an already-provisioned disposable Linux x64 worker:
Docker Engine 28 or newer, cgroup v2, built-in seccomp, AppArmor, memory/swap/PID
and CPU-quota support. Windows, Docker Desktop forwarding, rootless Docker,
remote Docker endpoints and historical Windows/macos matrices are rejected,
not retargeted. The adapter never starts or installs Docker, changes daemon
configuration or pulls an image. Provision this exact official Node 24
bookworm **amd64 manifest digest** on the disposable image first:

```json
{
  "schemaVersion": 1,
  "image": "docker.io/library/node@sha256:5a750d3be5e5c80275f8c9a5367c3aed99c2875656590c8d0701c7ee687f5f0a",
  "dockerExecutable": "/usr/bin/docker",
  "limits": {
    "cpus": 2,
    "memoryMiB": 16384,
    "pids": 512,
    "scratchMiB": 12288,
    "timeoutMs": 6600000
  }
}
```

The image was resolved from the official registry's `24-bookworm` index;
tags are not accepted at execution. Runtime inspection rechecks its
RepoDigest, OS and architecture. Limits may only decrease the adapter's
8-CPU/32-GiB-memory/1,024-PID/16-GiB-scratch ceilings and the policy's
110-minute timeout. They do not modify reservation accounting or give a
reservation another 110 minutes: execution also uses its absolute deadline.
These resource ceilings are separate from the unchanged issue allowance.

The transport creates two randomly named, launch-labelled networks and two
containers, with no published ports. The worker has only an internal bridge
using Docker's `gateway_mode_ipv4=isolated`: unlike an ordinary internal bridge,
it has no host bridge address through which to reach host services
([Docker gateway-mode documentation](https://docs.docker.com/engine/network/port-publishing/#gateway-modes)).
Only the trusted broker is connected to that network and an outward bridge.
The broker has IP forwarding disabled and accepts only the bounded
`egress.cjs` CONNECT protocol. Worker DNS is loopback-only; it connects to the
broker's inspected numeric private address. IPv6 is disabled. Direct public,
metadata and host traffic must fail even if a build ignores proxy variables.
The smoke probes this topology; command-line assertions are not acceptance.

The broker and canaries run as UID/GID 1000 with all capabilities dropped.
Physical execution uses a separate UID0 supervisor with only CHOWN, KILL,
SETGID and SETUID, and runs historical/sample commands as UID/GID 1000
without capabilities. The supervisor protects its helpers, reproduction SDK
and control records from that UID. Both modes retain
no-new-privileges, default seccomp, Docker AppArmor, private PID/IPC/cgroup
namespaces, read-only roots, zero swap, bounded tmpfs scratch and temporary
storage, PID/CPU/memory/descriptor limits, and no Docker log-file accumulation.
Arcade gets a separate 16 MiB generated-output tmpfs at `/trusted/artifacts`;
its pinned scripts, logging helper and helper-root `global.json` remain
read-only. That helper-root file is never copied into a historical checkout.
The only bind mounts are fresh, read-only copies of the trusted helper files
and explicit public inputs. No checkout, host home, Git configuration,
credential helper, token, Docker socket or outer environment is mounted.
Docker itself receives an empty owned client-config directory, a fixed local
Unix socket and an explicit credential-free environment; child `env -i`
retains only fixed paths and public proxy settings.

For physical execution, `options` supplies `operation: "execution"`, `input`
(the existing public execution unit), an **existing resolved absolute**
`evidenceDirectory`, reviewed `sourceDirectory`, and for source units a regular
`sourceBundle` with `sourceBundleSha256`. The bundle must be a
credential-free Git bundle; it is read-only inside the worker, where the
existing workspace runner fetches only the selected SHA into fresh scratch.
Only manifest-listed raw sample/producer assets are staged, with hashes
rechecked. Symlinks, hardlinked inputs, unresolved paths and mount delimiters
are rejected. The unit's environment fingerprint must match the pinned image.
`receiptKey` is a host-only authentication key of at least 32 bytes; use the
collector context's selected key. It is never passed to the container.
`signal` can cancel a launch. No executable, shell command, Docker flag,
report destination, key or callback is serialized into the job.

Final receipt publication closes and syncs the replacement before atomically
renaming it over the initial blocked receipt. On Windows only, `EPERM` from a
reader denying delete sharing is retried within one five-second publication
deadline. The authoritative receipt is never deleted first. Exhaustion and
other errors throw, retaining any earlier export/cleanup failure in the error;
collected output is not returned without a published final receipt.

The fixed `container-entry.cjs` constructs physical dependencies **inside**
the worker and invokes `runExecutionUnit`. Exact acquisition, historical
declarations, unmodified `global.json`, source build scripts and reproduction
identity checks remain the existing runners' responsibility. Source work
uses a separately acquired released reproduction SDK, never a host SDK.
The helper is built and inventories products only inside the container.
The win-x64 measured-archive exception is unchanged and does not authorize
Linux downloads. Missing comparable runtimes or permitted historical SDKs
remain untestable.

Physical execution terminates historical/sample descendants before export.
The protected supervisor stays alive until collection because stopping the
container destroys tmpfs. Fixed Docker exec operations transfer fourteen
controller-selected JSON files through the regular-file USTAR reader, followed
by separately bounded immutable acquisition blobs. Sample stdout is captured
as assertion data, never parsed as an envelope or control message. The parent
hashes incoming bytes and retains them outside worker scratch before cleanup.
The small lane remains capped at 64 files, 16 MiB/file, 64 MiB aggregate and
five minutes. Archive blobs are at most 1 GiB each, bounded additionally by the
launch scratch allocation and the durable store's 3 GiB/launch quota.
Blob deadlines cover filesystem opens, writes, syncs, closes and publication,
not just input reads. Cancellation rejects collection immediately; an already
issued filesystem operation is allowed to settle only for cleanup, with any
late destination rolled back. No cancelled transfer supplies a collected receipt.
**There is no writable host output mount, path extraction, or host execution
of worker JS, PowerShell, MSBuild, assemblies or inventory programs.**

The host-signed `receipt.json` retains helper/config/input/image identities, Docker command
exit/digest records, owned resource IDs and cleanup results. Resources are
registered before create calls so a lost create acknowledgement can still be
cleaned. Cleanup verifies the launch label, force-removes owned containers
(including descendants), then removes owned networks and verifies absence.
Cleanup errors block the result. Cancellation is not a refund or permission
to adopt unauthenticated output.

### Durable observation collection

`collector.createObservationCollector({ evidenceDirectory, store })` returns
the engine's `collectObservation({ result, reservation, manifest, context })`.
Place each `runContainer` output in
`<evidenceDirectory>/<reservation.id>` and return its raw `result` from the
trusted `runIsolated` adapter. Collection reloads the controller-selected
directory, authenticates the outer receipt with `context.keys[context.keyId]`,
and checks the exact unit, reservation, owner, epoch, manifest, policy,
workflow, image and acquisition bindings. Worker paths remain provenance,
not instructions to open host files or fetch URLs.

Observations are rebuilt from raw acquisition, inventory, source declaration,
source-build, command, producer and repeated reproduction evidence. The
existing exact assertion comparison and diagnostic normalization are reused.
Source and repetition commands must match distinct command receipts recorded
by the protected supervisor, including executable, arguments and raw results.
Wrong local compiler/tasks/targets/Core paths, host/runtime fallback, incomplete
matrices, missing repetitions and changed bytes cannot supply passing product
evidence. Missing setup identity is conservatively recorded as untestable
incomplete evidence, never promoted to a product result. Essential identities,
both assertion streams/exits and per-observation provenance stay in the signed
checkpoint; external bytes are not a replacement for those fields.
The fixed export includes JSON `null` for unavailable acquisitions. Acquisition
and early setup failures can therefore be collected without inventing archive
identities. Measured restore/build failures and interrupted reproductions remain
untestable even when no runtime-load telemetry was produced.
Compiler and sample runtime selections are checked independently across
repetitions. A project may use different verified compiler and sample runtimes;
changing either selection between repetitions is rejected.
Loaded compiler/FSI Core telemetry must name the selected compiler runtime,
not merely any acquired runtime. Released compiler, FSI and Core paths, hashes
and assembly versions, plus the product runtime, must match acquisition evidence.

Transport authentication proves which bytes the pinned harness collected
under a particular launch. It does **not** prove that historical compiler
code is benign, or turn runtime-hook/binlog telemetry into an independent
proof of arbitrary program semantics. The harness records selected commands,
process outcomes, measured files and helper telemetry separately from sample
stdout. No worker `verified` flag or ready-made observation is authority.

The concrete store is **GitHub Actions v4 artifacts**.
`createActionsEvidenceStore({ directory, repository, runId, attempt, token,
runtimeToken })` uses fixed GitHub/Actions authorities for metadata and upload,
with credential-free access to the exact service-issued Azure blob object.
Workers receive neither credential. Reads authenticate repository, workflow
run/attempt, artifact ID/name, immutable content digest and byte count.
Redirects beyond the approved service handoff, arbitrary URLs and alternative
ZIP members/paths are rejected; the single stored data member is streamed,
not extracted. Uploads request 90 days' retention, subject to repository
retention settings and deletion. A complete collection is at most 64 MiB;
SDK/runtime archives use only the explicit receipt-validated streaming lane.
Publisher checksums and the existing exact win-x64 measured approval remain
distinct; measured bytes never become a publisher checksum.

`createFileEvidenceStore({ directory, repository, runId, attempt })` implements
the same references and validation for offline use. Both stores expose
`writeEvidence`, `readEvidence`, `writeBlob`, `readBlob`, `verifyEvidence` and
`findEvidence`. `readEvidence(url)` returns bounded small-record bytes or
confirmed-absence `null`; archive verification uses `verifyEvidence(url)` to
avoid allocating SDK-sized buffers. Compose both read methods into the state
transport. Authentication, rate-limit, network and integrity failures throw.
No artifact URL support is added to the issue API's `createGithubApi`.
Staging deadlines cover opening, writing, syncing and closing, including error
cleanup. A timeout rejects promptly and destroys the input; uncancellable
filesystem operations finish cleanup when they settle, without publishing late
bytes. File/HTTP reads, metadata requests and downstream sinks are also bounded.
File reads use one deadline from directory validation through open, stat,
streaming and final close, including failed-open cleanup. Actions reads also
use one deadline for run/metadata requests and bodies, redirect validation,
404/410 revalidation, ZIP/hash streaming, sinks and response cleanup.
Each discovery shares its original deadline across every page and candidate
read. An HTTP callback ignoring abort cannot extend the outer wait; a late
response is closed without further requests. Expiry is an error, never verified
absence, a partial discovery result or successful verification.
File writes share one deadline across staging, replay checks, atomic publication,
verification and staging cleanup. Expiry rejects without waiting for stalled
filesystem calls. Once publication has started, complete content-addressed bytes
may survive a timeout, just as they survive process loss. Cleanup removes only
the writer's unique staging file, never a shared object that a retry may already
have acknowledged. Retries independently verify retained bytes before reuse.
Actions writes likewise share one deadline through ZIP hashing (including open,
stat, streaming and close), upload, finalization, verification and staging cleanup.
Late hashing completion cannot start publication or acknowledge an expired write.
Download-time 404/410 responses require authenticated metadata confirmation
before becoming absence; a missing signed blob alone is not that confirmation.
If artifact finalization succeeds but its response is lost, a new store instance
reuses the exact artifact identity without creating or uploading it again.

The collector uploads indispensable bytes and a host-signed collection
descriptor before `commitObservation` can clear pending work. Discovery uses
the owning run/attempt and reservation ID, so a lost upload acknowledgement
does not require its URL or scratch in memory.
`commitCollectedObservation(input, { collectObservation, transport })`
replays a retained collection under the existing controller lock and owning
run; it neither launches a worker nor reserves/refunds work. The state ledger
keeps observation writes and lost acknowledgements idempotent.
The caller must hold the whole-issue lock across collection, quota reservation,
checkpoint commit and acknowledgement, including across processes. A timed-out
write is **not** permission to hand off ownership: await `store.settleQuota()`
before releasing the lock, or confirm the old writer process has terminated.
Quota cancellation is checked after enumeration and while draining queued work;
an already-issued marker creation/sync/close must settle before handoff.
Created markers remain charged even when acknowledgement, sync, close or transfer
fails. Neither cleanup nor retry removes a charged marker.

The collector factory exposes `collectObservation.settleQuota()`. `runController`
drains it inside the actual `withLock` callback. After five seconds of pending
cleanup it reports `blocked` with `evidence ownership cleanup pending`, but the
callback remains outstanding and retains the lock until settlement. The lock
provider must honor callback lifetime, not the bounded controller response or an
expired lease. A host abandoning cleanup must terminate and confirm the writer's
exit before granting a successor ownership. `commitCollectedObservation` also
drains quota work before returning; direct collector/store callers must do the
same inside their lock. The in-process queue is not a distributed lock. These
handoff requirements must be exercised by the later controller deployment gate;
offline tests do not establish a deployed lock provider's behavior.
The file store checks an existing immutable object before publishing with a
same-directory atomic rename. Process loss leaves either unpublished staging
bytes or a complete single-link object that a new process can discover; it
cannot strand a published object as a hardlink to abandoned staging bytes.
Required collection/archive expiry blocks reuse and publication. Optional logs may
expire without changing adoption of an already authenticated pending report.
Signed records remain strictly below 48 KiB and rendered reports below
32,768 UTF-8 bytes; exceeding either bound blocks publication, not evidence
validation or budget accounting.

Offline transport/collector tests perform **zero real launches**. Before any
unattended activation, acceptance must exercise the execution supervisor's UID,
helper/control protection and descendant termination as well as the canary
network/mount/resource tests and actual source-product reproduction. Passing
synthetic Docker or file-store tests is not that acceptance.

Run the real acceptance CLI with absolute resolved paths and a new evidence
directory; the config is operator-reviewed transport configuration, **not an
issue reproduction approval**:

```powershell
node eng\regression-bisect\containment.smoke.cjs --run <absolute-approved-worker-config.json> <absolute-new-evidence-directory>
if ($LASTEXITCODE -ne 0) { throw 'Containment incomplete; do not deploy' }
```

On the supported Linux host, invoke the same file with the platform's path
separator. The CLI has a 15-minute overall budget. It uses the production
adapter with small canary limits, creates only harmless outer canary files and
an environment canary, and attempts outer-file/environment/process/socket
reads, direct metadata/control egress, and prohibited broker authorities.
It checks actual cgroup settings, fills bounded scratch to ENOSPC, exhausts
PID capacity, measures two busy children under a one-CPU quota, requires a
Docker OOM-kill receipt from bounded memory pressure, creates a descendant for forced timeout cleanup, rejects a
malformed/path-bearing output envelope, and requires a bounded official
release-index transfer. Canary files are removed; launch/denial/exit/cleanup
receipts persist. Missing runtime/platform or any missing enforcement evidence
returns exit **2**, records `incomplete`, and is never a mocked pass.

Run transport tests separately:

```powershell
node --test eng\regression-bisect\collector.test.cjs eng\regression-bisect\worker-output.test.cjs eng\regression-bisect\worker.test.cjs eng\regression-bisect\container.test.cjs eng\regression-bisect\evidence-store.test.cjs
if ($LASTEXITCODE -ne 0) { throw 'Container transport tests failed' }
```

### Reviewed first-use transaction and GitHub API

`bootstrapController` takes the same fixed issue input, trusted `context`,
`transport`, `withLock`, and optional `clock` as the other controller operations.
Its `approval` dependency supplies `readApproval({ path, revision })` and
`readSource({ path, sha256 })`, both returning raw `Buffer` bytes. No dispatch
field can provide an allowance, authorization object, command, ref or slot ID.
The approval at the registered `policyRevision` has this shape:

```js
{
  schemaVersion: 1,
  manifest, // The existing frozen, reviewed reproduction contract.
  bootstrap: {
    allowanceId, deadline,
    checkpointCommentId, checkpointAnchorSha, checkpointBootstrapDigest,
    resultCommentId, resultBootstrapDigest
  }
}
```

Provision the two comments in a separate trusted setup operation, retaining their
actual IDs and the immutable reachable anchor in the controller's trusted
registration. This library cannot create comments, anchors or registrations.
The checkpoint slot must be an authenticated `proposal` with exactly:

```js
{
  schemaVersion: 1, purpose: "checkpoint-slot",
  repository: "dotnet/fsharp", issueNumber,
  checkpointAnchorSha, allowanceId, deadline
}
```

The result slot uses the existing `result-slot` proposal below. Sign each at its
own real comment ID through `serializeWorkflowRecord`. Freeze the complete issue
discussion with these authenticated slots excluded, review the raw reproduction
and producer bytes, then commit the approval envelope on the trusted default
branch. Its two bootstrap digests are `fingerprintInput` of the exact original
signed comment bodies, not hashes of whatever is present at execution time.
The controller's registered checkpoint ID, anchor and bootstrap digest must
already exist independently of the loaded approval. Never replace that
registration because a lookup failed.

Under the whole-issue lock, bootstrap authenticates both slots and all their
writer/app/workflow/run bindings. It scans every page of statuses on the
registered anchor for the entire `regression-bisect/<issue>/` prefix. A different
comment's prior allowance, missing slot, incomplete history, API error, failed
signature, or altered registration blocks initialization. Before first use it
also rereads full discussion, linked investigations and reviewed raw assets.
Another still-active slot owner blocks takeover. Only then does the existing
state initializer write generation zero and its independent freshness receipt.
All future reservations use the unchanged 24-build/40-unit/720-minute allowance.

Bootstrap returns `initialized`, `loaded`, or `blocked`; none is permission to
run a worker. Concurrent calls retain the same allowance. A charged checkpoint
is loaded, never reinitialized. An interrupted generation-zero write recovers
its original signed bytes and appends only the missing receipt, even if current
eligibility or time changed; ordinary engine checks still gate execution.
The comment/status mutation ledgers and fresh-Node-process recovery tests are in
`controller.test.cjs`. `state.initializeCheckpoint` retains its low-level legacy
raw-placeholder support; production bootstrap requires the signed slot above.

`github.cjs` supplies the concrete HTTP and pinned-file boundary:

* `createGithubApi({ repository, issueNumber, checkpointCommentId,
  resultCommentId, anchorSha, token }, send = fetch)` returns `request`,
  `getComment`, `updateComment` and `getRun`. It uses only
  `https://api.github.com`, fixed REST route templates, typed parameters, a
  30-second request deadline and a 4 MiB decoded-response limit. Redirects,
  compressed/truncated bodies and invalid UTF-8 fail closed. Mutations are only
  PATCH of the two registered comment IDs and POST of the issue/checkpoint
  freshness context on the registered anchor. Other repositories are readable
  only for linked issue/PR metadata. There is no contents write, issue close,
  PR write, workflow dispatch or arbitrary URL operation.
* `createApprovalReader({ api, revision, issueNumber })` supplies the two raw
  approval callbacks. `revision` must come from trusted immutable default-branch
  registration, never dispatch or issue text. The reader walks that commit's
  nonrecursive Git trees, rejects truncated responses, links and submodules,
  checks the Git blob object ID and byte count, then checks each reviewed asset's
  SHA-256. It never checks out or executes repository bytes. Approval bytes are
  below 48 KiB; each source read is at most 1 MiB, with intake enforcing its
  existing aggregate bound.

Only the controller holds the API token and HMAC key map. The intended token
scope remains `issues: write`, `statuses: write`, `contents: read`,
`actions: read`; key retention/rotation requirements are below. No environment
variables are read automatically and no production secrets are configured by
these APIs. `createGithubApi` deliberately does not fetch artifact downloads or
implement `readEvidence`; the controller must provide its separately restricted,
durable evidence reader. Test with fake HTTP only:

```powershell
node --test eng\regression-bisect\github.test.cjs eng\regression-bisect\controller.test.cjs
```

### Data-only worker output

`worker-output.collectWorkerArchive(stream, files, { signal, timeoutMs })`
reads an **uncompressed regular-file USTAR stream**, without extraction, imports,
inventory probes or execution. `files` is a fixed list of
`{ path, sha256, size }` expectations supplied by the trusted adapter, not an
instruction to read arbitrary worker paths. It returns the same descriptors
with independently hashed `bytes`. Limits are 64 files, 16 MiB per file,
64 MiB total content and five minutes for transfer; `timeoutMs` can only lower
the deadline. The stream byte cap includes headers, block alignment and bounded
tar record padding. Abort, timeout and transport failures destroy the input and
return no partial collection.

Every expected file must occur exactly once, with an exact case-sensitive name,
size and SHA-256. Case aliases, traversal, links, device entries, extended/PAX/GNU
headers, compressed archives, nonzero padding and truncated streams are rejected.
USTAR prefix fields support longer safe relative paths. No archive name is ever
passed to a filesystem API, so a link or reparse point cannot redirect collection
to the credentialed host. A producer must transfer regular files only, not a
directory entry or an archive format that needs path overrides.

`worker.readExecutionOutput(files, unit, launch)` then reads only `result.json`
as bounded, strictly decoded UTF-8 JSON. The trusted adapter retains
`launch = { launchId, imageDigest, unitDigest }`, each a 64-character lowercase
hex identity, where `unitDigest = fingerprintInput(unit)`. The JSON must contain
exactly `{ schemaVersion: 1, ...launch, result }`. The entire public execution
unit is bound, including repository, issue, work/build ID, epoch, source or
released target, matrix, deadline and policy/workflow/environment identities.
Extra envelope fields, including `verified`, observation provenance or a report
destination, are rejected. `result` remains untrusted runner data; its outcome
shape is checked, but its identities and assertions still require the existing
trusted observation collector and engine checks. The 16 MiB raw JSON cap does
not change the signed checkpoint's separate 48 KiB cap.

The adapter must establish the launch/image from the actual runtime, terminate
the child and all descendants, and obtain the complete fixed transfer before
calling these readers. Matching worker-reported hashes or launch IDs is
**binding, not authentication**. Neither reader signs observations, accepts a
child's `verified` flag as authority, proves process termination, persists
required evidence, or grants publication capability. They cannot replace a
containment adapter or its trusted collection provenance.

The focused data-transfer checks, including the platform's real `tar` producer,
run with:

```powershell
node --test eng\regression-bisect\worker-output.test.cjs eng\regression-bisect\worker.test.cjs
if ($LASTEXITCODE -ne 0) { throw 'Worker transfer tests failed' }
```

### Fixed-slot guarded publication

`publishController` requires `context`, `transport`, `withLock`,
`readCurrentContext` (the report API's fresh trusted reader), and a trusted
`resultCommentId`. It never creates a comment, accepts a destination from its
input, or writes worker/model Markdown. Provision and retain that ID outside
dispatch alongside the checkpoint/anchor registration. The slot must already
contain an authenticated `proposal` record, signed at its actual comment ID,
whose payload is exactly:

```js
{
  schemaVersion: 1,
  purpose: "result-slot",
  repository: "dotnet/fsharp",
  issueNumber,
  checkpointCommentId
}
```

This is a publication slot, **not** reproduction approval or first-use allowance
authorization. Its exact writer/app/workflow/run and checkpoint association
must authenticate; an empty comment, copied marker or digest alone is rejected.
The publisher prepares and persists the report digest, authenticates the slot,
refreshes preparation immediately before writing, signs only `report.cjs`'s
validated report, updates the fixed ID, then authenticates completion through
the existing report API. Every mutation remains under the whole-issue lock.

An already-written matching result can be adopted after the original run
completes. Its owner must match the authenticated pending checkpoint writer;
after completion, the persisted canonical comment ID authorizes later reads.
If no result was written, a new run can take over the same pending digest only
after verifying that the previous owning attempt completed. Adoption and
ownership transfer neither execute a worker nor replenish the allowance.
Missing or changed slots, active/unknown owners, stale input, required evidence
loss and failed API writes remain explicit blocks.

Classifications deliberately differ:

| Classification | Required evidence |
| --- | --- |
| `verified-introducing-source-boundary` | Independent deterministic candidate failure and sole comparable parent pass, unchanged sample SDK/runtime/environment and approved assertions. This is not a semantic root-cause claim. |
| `verified-triggering-change` | All comparable merge parents pass and integration fails, or source/build-input changes are coupled. The commit is a tested boundary; latent responsibility remains unknown. |
| `unconfirmed-candidate` | Root/missing/outside-lineage parent, insufficient allowance, skip/flake, identity/setup failure, changed result, ambiguous interval, or contradictory history. Remaining interval, parents, observations, skips and spent allowance remain visible. |

History inspection is bounded to 128 evidence commits and 4,096 interval SHAs.
It retains actual parents, message SHA-256, changed paths, full revert/cherry-pick
references and a normalized patch-similarity SHA-1 (`patchId`, **not** a Git object
identity or proof of provenance). Referenced objects are fetched only from the
trusted local mirror. Reverts/reintroductions, cherry-picks/backports, repeated
patches and observed failure-then-descendant-pass evidence prevent unique
attribution. Missing related objects remain explicit uncertainty. Toolchain
changes retain each observation's requested/resolved build SDK and runtime;
global/build-input changes conservatively couple source and toolchain blame.

Changed paths are Git-quoted display evidence, not executable repository paths.
Raw history output preserves edge whitespace and Git byte escapes. Count and
byte limits bound the retained sample; summaries disclose the total count,
digest and truncation, and the full bounded input is checked for toolchain
changes even beyond the sample. Truncated or malformed history cannot establish
a verified boundary. A digest identifies omitted data but is not a retrievable
copy. Recovery retains authenticated observations, spent allowance and the
saved search boundaries/decisions, and records explicit uncertainty instead of
revalidating the rejected history. Reproduction asset-path validation is unchanged.

Association resolution uses an isolated Git history and GET requests to
`commits/{commit_sha}/pulls`, `pulls/{pull_number}` and
`pulls/{pull_number}/commits`. It verifies exact repository/URL, merged status,
full merge/base/head SHAs, head/commit membership and tested-main ancestry.
Pagination is bounded to ten 100-entry pages and 100 associations; truncation,
rate limiting, duplicate or inconsistent responses return `incomplete`.
Unmerged/closed-only PRs are not introducing associations. A lone verified
main-lineage association is `unique`; multiple or servicing/off-lineage
associations remain `ambiguous`. `none` does not invalidate a verified source
boundary. Search results, titles and patch similarity never authorize a PR link.

`model.validateConfirmation`, `validateConfirmationEvidence(checkpoint)` and
`validateAssociations` enforce these additional evidence shapes. They validate
structure and consistency, not the authenticity of arbitrary supplied data.

## Deterministic reports and publication guards

`report.cjs` exports these controller APIs:

| Export | Contract |
| --- | --- |
| `validateReport(report)` | Strict bounded evidence schema; rejects arbitrary Markdown, extra fields, stale fingerprints, unsupported destinations and unsafe evidence references. |
| `renderReport(report)` | Deterministic escaped Markdown from validated evidence. Does not execute instructions, fetch URLs or publish. |
| `preparePublication({ now, generation }, dependencies)` | Under the issue lock, rechecks fresh approved inputs, complete discussion, eligibility, active investigations, current heads, authenticated checkpoint and result digest. Returns `ready`, `pending`, `published`, `blocked` or `requeue`. |
| `completePublication({ now, generation, digest, commentId }, dependencies)` | Repeats freshness checks and authenticates the actual existing result before recording publication completion. Does not create or edit the result comment. |

Dependencies are `{ context, transport, withLock(action),
readCurrentContext({ now, repository, issueNumber }) }`. `context` is the full
existing state authentication context; it is not agent-supplied authorization.
The mandatory fresh trusted reader returns `manifest`, `authorization`, `policy`,
`policyRevision`, `workflowRevision`, `pinnedMainSha` (fresh main HEAD),
`stableSdk`, `environmentFingerprint`, and `readSource({ path, sha256 })`.
It must read approved assets and policy/workflow/main heads from trusted current
GitHub data, not echo a cached dispatch. The selected issue is fixed by the
authenticated context and manifest. There is no arbitrary destination parameter.

Preparation uses the real `intake.readIssueSnapshot` and
`findActiveInvestigations`, including complete comment/timeline pagination and
raw approved-source checks. Only authenticated checkpoint/result records are
excluded from the discussion fingerprint. Marker copies, altered signed bodies,
different IDs, writers, apps or runs remain ordinary changed input. The current
checkpoint generation and publication digest are checked under the same lock.
Closed, unlabelled or too-old issues, corrections, approval/policy/workflow
changes, moving main, missing required evidence, competing investigations and
stale generations produce no publishable body. Spent allowance is preserved.
Only terminal `complete` or `blocked` checkpoints are reportable; an old
confirmation cannot be published while fresh confirmation is underway.

The report schema is:

```js
{
  schemaVersion: 1,
  kind: "regression-investigation",
  state, // validated state; publication digest/commentId normalized to null
  observations: [{
    digest, data, // authenticated observation, including exact repeated assertions
    evidence: [{ url, sha256, required, available }]
  }],
  deadline, inputDigest, reproductionDigest, policyDigest, checkedHead
}
```

This binds the issue/epoch/result digest without including mutable publication
bookkeeping. Rendering includes approval/input identities, checked versus tested
main, frozen source/producer/package hashes, approved operation descriptors and
matrix settings, expected assertions, exact observations and build/artifact
identities, outcomes/repetitions, evidence references, interval/skips/parents,
verified or ambiguous PR associations, causal uncertainty, allowance use and
resume state. The operation descriptors are the manifest's approved commands,
expanded by the existing runner, not reporter-authored shell strings.
Long display data is bounded and escaped; full evidence remains in authenticated
observation references and the signed structured payload.
Display tables deduplicate product/provenance identities, not acquisition
receipts: timestamps, retained paths and receipt-instance digests do not create
new toolchains. Every original receipt remains in its authenticated observation.
Different requested/resolved products or provenance retain distinct identities.
The rendered UTF-8 body must be strictly below 32,768 bytes, and the complete
signed result must be below 48 KiB. Oversized results block publication while
preserving complete evidence in the authenticated checkpoint; limits are not
raised and observations are not discarded to make a result fit.
Complete remaining-SHA lists and skips are retained in the signed result, not
only optional artifacts. If confirmation recovery had to drop remaining SHAs
to fit a checkpoint, publication is blocked: its count/digest is not a complete
reference. Unknown intervals are labelled as saved boundaries, not exhaustive
remaining lists.

Observation artifacts may additionally retain `assemblyVersion`. Released
cells may retain `releasedToolchain: { sdk, compilerRuntime, archive }` using the
existing exact product-identity and archive-receipt schemas. The released receipt
must match the selected SDK hash, platform and official publisher checksum;
the source-only measured-preview exception cannot authorize a released archive.
Reports render both requested/resolved source toolchains and actual released
receipts. Measured official-HTTPS hashes are explicitly **measured**, never
publisher-provided checksums. Missing historical details remain visibly absent.
A verified boundary/trigger report cannot be published without the selected
stable endpoint's archive/runtime receipt and bundled compiler/Core assembly
versions; legacy incomplete evidence produces a blocked intent, not success.

`state.recordPublication(transport, context, { generation, status, digest,
commentId })` updates the existing authenticated pending/published/blocked fields.
Preparation persists a deterministic pending digest before returning `ready`.
Repeated dispatch returns `pending`, not renewed write authorization. Complete
authenticated discussion scanning recovers a matching result after a successful
write whose response was lost. It never trusts a copied marker or caller Boolean.
Recovery first rechecks current inputs, eligibility, ownership and required
evidence, then authenticates the persisted pending digest and existing result.
Optional artifact expiry does not replace that result's historical availability
or digest. Recovery does not reread optional artifacts; new reports still check
their availability and propagate transport errors. Publication uses
`loadCheckpoint(transport, context, { requiredEvidenceOnly: true })` for this
required-only gate, which never authorizes execution; ordinary checkpoint loads
still check all evidence.
Adoption records the same comment ID without another result write, worker or
allowance reset. Missing intent, conflicting results (even with different
digests), owners that do not match the persisted publication intent,
required-evidence loss and stale inputs fail closed.
`state.authenticateDecodedRecord(transport, context, comment)` returns the
authenticated `{ kind, payload, commentId, owner }` or `null`.

The trusted publisher must re-run preparation as close as possible to
the write, hold the whole-issue lock, and publish only the fixed signed payload:

```js
{
  kind: "result",
  status: "Regression investigation result",
  payload: { schemaVersion: 1, digest, report }
}
```

Use `state.serializeWorkflowRecord` with the **actual final comment ID** in its
authentication context. The returned display `body` alone is not an
authenticated result and must not be published as one. For a lost response,
recover a matching authenticated result or keep the pending intent blocked;
do not blindly create another comment. `controller.publishController` performs
this fixed-slot write; `report.cjs` itself never writes the result.

Evidence transport is restricted to exact repository-bound GitHub artifact URLs:
`https://api.github.com/repos/dotnet/fsharp/actions/artifacts/<id>/zip` or
`https://github.com/dotnet/fsharp/actions/runs/<id>/artifacts/<id>`.
Encoded segments, traversal, credentials, query strings, other origins and local
attachment paths are rejected **before** fetching. The injected
`readEvidence(url, { redirect: "error", maxBytes: 4194304 })` must enforce those
limits without following redirects. Missing required artifacts block; optional
expired artifacts are disclosed. Untrusted log text is display data, never a
new URL to fetch.

### Confirmation/report validation

Run the focused attribution/report, model and state/engine tests, followed by
recursive `node --check` and `git diff --check`. Full-suite acceptance separately
requires all `*.test.cjs` in this directory:

```powershell
node --test eng\regression-bisect\attribution.test.cjs eng\regression-bisect\report.test.cjs
node --test eng\regression-bisect\model.test.cjs
node --test eng\regression-bisect\state.test.cjs eng\regression-bisect\engine.test.cjs
```

Real Git fixtures cover fresh parent sets, merges, reverts/cherry-picks,
toolchain coupling and bounded paginated associations. Report tests authenticate the rendered result,
check both UTF-8 byte limits with distinct receipts, and exercise publication
crash points and semantic mutations removing classifications, PR links or skips.
Product execution is injected in these tests: they do not establish an actual
regression's introducing commit.

Historical runner receipts remain at
`Q:\rb912-s5-i5-20260919\acceptance-audit.json` (SHA-256
`c426ad8a5fbeb65aa0750d00088eaba77051a57c4be112ceb94e47112a9ae8ab`)
and `Q:\rb912-confirmation-20260920\smoke-results.json` (SHA-256
`1e3dec3266eb0e7a3bda888f9724d6ce02008823f664740ddcb33d7cc02d6499`).
They are not fresh acceptance of confirmation/report repairs. Runner/toolchain
identity contract changes require the documented real smoke checks again.

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
attempts }`, not reproduction pass/failure. Missing archives, insufficient
checksum provenance, network failures, unsupported installer flags or declarations,
SDK/tool declaration disagreement, changed source files, and unfulfilled runtime
requirements fail closed. Historical previews are allowed, but never selected
as released stable. `checksum-provenance` means an archive lacks
approved provenance or fails its integrity checks. It also covers rejected
download URLs or redirect chains, even when an availability HEAD probe cannot
establish whether the archive exists. Native HTTP and injected response
validation preserve this reason; they do not report a forbidden redirect as a
missing SDK. Network operations remain outside that validation boundary.
It is distinct from
`unavailable-sdk`/`unavailable-runtime`
(missing archives) and `network-failure`. None is a positive reproduction.
Attempt evidence requires `checks.network`: a failed acquisition request sets it
to `false`, preserving `network-failure` even when checksum, setup and product
identity checks could not complete. It takes precedence over those checks, never
overrides a failure into a pass, and survives observation validation and
authenticated checkpoint reload with the same digest and skip reason. Successful
acquisition sets it to `true`; missing or non-boolean checks are incomplete
evidence, not implicitly successful network requests. Older observations lacking
this required check are rejected rather than silently upgraded.
Exact `global.json.tools.runtimes` arrays (`dotnet`, `aspnetcore`,
`windowsdesktop`, optionally suffixed with the matching architecture) become
separate build-runtime requirements. Unevaluated MSBuild version expressions
and foreign-architecture requirements are explicitly untestable, not guessed.
Acquisition binds each requirement to the archive that supplied it, including
runtimes already bundled in the SDK; missing supplemental receipts cannot be
hidden by an incomplete installation list.

Archive provenance is discriminated, not inferred from the existence of a hash:

* `publisher-checksum` retains the independent SHA-512 in `archive.sha512`.
  Release metadata and official sidecars always take precedence. Released
  stable selection still requires official release metadata and its checksum.
* `approved-official-https-measured` has **no** publisher `sha512` field.
  It requires an exact entry in the pinned policy's `toolchains.measuredArchives`,
  including historical source SHA, prerelease SDK version, product, RID, HTTPS
  origin and URL, permitted redirect URLs, byte/time bounds and approval revision.
  `approvalDigest` is the canonical fingerprint of that entry, not authorization
  supplied by an issue. Adding a public feed does not authorize measured downloads.

The initial exceptions cover only source
`5a8b9adb034f420a5c2c1f577670c5eeeb454b7e`, SDK
`11.0.100-rc.1.26420.103`, product `sdk`, RID `win-x64`, and its exact
`builds.dotnet.microsoft.com` and `ci.dot.net/public` archives. They permit no redirects, at most 1 GiB,
and a five-minute download. Other revisions/platforms need an explicitly reviewed
policy entry or publisher checksum; no version substitution is allowed.
The recorded approval revision is the preserved recovery base
`2ec6de6837b5b367e1f1186e850b8faa2d188b16`; authorization still comes from the
trusted policy checkout, never the existence of that SHA or digest alone.
After both sidecars are checked, bounded availability probes select an approved
exact archive before installation; the installer itself has no public fallback.

Both branches retain attempted metadata/sidecar sources and HTTP statuses,
requested and final archive URLs, every redirect, `bytesReceived`, receipt time,
measured SHA-256/`actualSha512`, and `retained` path/size/digests. The same
discriminator appears in requested/resolved identities, installation receipts,
workspace results and source observation build-toolchain records. Changing the
receipt or claiming a measured digest is a publisher checksum is rejected.
Before reproduction reuse, retained archives must still exist as regular,
non-linked files and are rehashed; missing or changed bytes invalidate reuse.
Read-only file attributes alone are not proof of immutable identity.
The installation list is mandatory, even when all required runtimes are bundled
in the SDK. It cannot replace or omit the SDK receipt; each runtime requirement
must name its supplying archive. Supplemental runtime bytes are rehashed before
reuse, independently of the SDK, and missing receipts fail before any reproduction
process starts. Receipt/schema and trusted-policy
changes invalidate old checkpoint evidence rather than upgrading old results
into measured or publisher-verified claims.

Serialized observation/checkpoint receipts use the worker cell's OS path rules,
not the coordinator's filesystem. Windows receipts therefore round-trip on POSIX
coordinators without rewriting their digested paths. Relative and drive-relative
paths are rejected. Live acquisition and archive rehashing still use the worker's
actual filesystem; accepting a serialized receipt does not prove local byte
availability.

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
feed serves the already downloaded and provenance-verified archive to that
installer. This transport boundary records the **actual public download's**
redirects/digests without reimplementing extraction or trusting a separate HEAD
request. Verification waits for pending server deliveries and file closure,
not just installer exit; cleanup also drains deliveries before removing their
archive. Reads stop at the advertised archive length so a client's completed
download cannot race a redundant EOF read. Late delivery errors still fail
acquisition. The official retained
archive and original download are hashed again and must agree in both SHA-256
and SHA-512. No second public archive download is permitted. Supplemental runtime
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

## Historical revision workspaces

`workspace.cjs` composes acquisition and reproduction without admission, Git
bisection, or publishing. `prepareRevisionWorkspace(options, dependencies?)`
returns an owned, fresh workspace handle; `buildRevision(handle, dependencies?)`
consumes it once. `testSourceRevision(options, dependencies?)` performs both and
runs the unchanged frozen manifest. All four phases (`main`, `older-search`,
`bisect`, `confirmation`) use this implementation. Even repeated confirmations
of the same SHA allocate a new UUID, checkout, SDK, package cache and sample.

Trusted options are `manifest`, its reviewed `sourceDirectory`, an existing
`mirrorDirectory`, separate existing `scratchDirectory` and `evidenceDirectory`,
an absolute `gitExecutable`, optional `signal`, and this request:

```json
{
  "schemaVersion": 1,
  "repository": "dotnet/fsharp",
  "sourceSha": "<full 40-character source SHA>",
  "manifestDigest": "<fingerprintInput(manifest)>",
  "approvalDigest": "<manifest.approvedInputDigest>",
  "phase": "confirmation",
  "platform": { "os": "windows", "architecture": "x64", "rid": "win-x64" },
  "allocation": { "buildTimeoutMs": 5400000, "workerTimeoutMs": 6600000 }
}
```

The controller must associate the trusted mirror with the repository and reserve
the execution allocation before calling. The worker does not fetch issue-supplied
URLs. Git fetches only the pinned object from that mirror into an independent
repository, with empty templates, disabled hooks/credential helpers/submodules,
isolated configuration and Windows long-path support. Tracked links/build output
and unexpected state in a prepared checkout are rejected, not scrubbed or reused.
The environment is constructed from an allowlist, with private homes, caches and
temporary directories; source build scripts cannot print inherited tokens.
This is not an OS sandbox: the credential-free worker/container boundary remains
mandatory before executing historical source or reproductions.

The explicit adapter supports nested `fsiProject`/`fscProject` Arcade layouts
with `.sln` or `.slnx` entrypoints and evaluated
`artifacts/bin/<product>/Release/<tfm>` output. It reads that checkout's
`global.json`, TFM declaration, bootstrap/build scripts and NuGet configuration.
Windows uses its `eng\Build.ps1 -restore -build -c Release -pack -bootstrap
-noVisualStudio -msbuildEngine dotnet`, restricting `Projects` to that revision's
`Microsoft.FSharp.Compiler.fsproj` product graph rather than building the solution
or tests. Bash uses root `build.sh --configuration Release --pack --bootstrap
--product-build` (the root already supplies restore/build). Unsupported
entrypoints, declarations or SDK property-query support are explicitly untestable.
Historical files, TFMs and package versions are never rewritten.

MSBuild-evaluated paths and package identity must agree before/after the build.
After the product graph finishes, the adapter explicitly packs its built Core
with `--no-build --no-restore` into Core's evaluated `PackageOutputPath`.
The graph can place dependency packages under `Dependency/Shipping`; that is not
the standalone Core output, and the adapter does not guess between the two.
Inventory verifies bytes, assembly versions, runtimeconfigs and the Core package,
rejecting missing outputs, SDK-bundled compiler fallback and redirected artifacts.
Only the selected compiler, FSI, tasks, Core and Core package need out-of-process
metadata inspection. Every dependency is still hashed before and after execution;
large localized inventories cannot exceed Windows' process command-line limit.
No timestamp selection or shared same-version Core cache is used. Exact acquisition
uses the trusted checkout's existing Arcade acquisition helper, not a helper
assumed to exist in historical source. The evidence helper likewise comes from
the trusted adapter (or the controller's optional `helper` descriptor). Project
reproduction continues to import the trusted root
`UseLocalCompiler.Directory.Build.props` and `.targets`; their digests and the
adapter's layout/version/digest are recorded.

Results contain the three-way `outcome`, requested/actual identities, commands,
exit codes, durations, build ID and complete reproduction repeats/matrix evidence.
Only `evidence[].path` entries (relative to `evidenceDirectory`, with SHA-256)
are upload candidates, not arbitrary paths appearing in command output.
Build logs/binlogs and observations persist outside scratch. Setup/build errors,
timeouts and wrong/missing identities cannot become product failures even when
their output matches the expected diagnostic. Cancellation terminates process
trees and removes only the UUID-marked, resolved owned scratch child; ordinary
results retain their scratch artifacts for diagnosis. Results expose no mutation
or publisher operation. Controller provenance/admission remains separate.
Acquisition failures retain their original stage, code and message in workspace
evidence: wrong SDK or architecture is `wrong-identity`, not `unavailable-sdk`.
Cancellation also finalizes staged `buildRevision` calls. An interruption during
final source verification or observation persistence invalidates a completed
reproduction, and repeated finalization reuses the same observation.
On Windows, a trusted PowerShell supervisor joins a kill-on-close Job Object
before launching the command. It drains inherited output even after the parent
exits; timeout/cancellation kills the contained descendants. Successful commands
do not wait for detached build servers that have closed their output pipes.
The shared environment supplies private Windows machine-configuration paths so
NuGet can initialize without reading the developer's machine feed configuration.

### Bounded opt-in clean-build smoke

```powershell
node eng\regression-bisect\workspace.smoke.cjs --run <full-current-sha> <trusted-mirror> <existing-storage> <absolute-git-executable>
node --test eng\regression-bisect\model.test.cjs eng\regression-bisect\toolchains.test.cjs eng\regression-bisect\repro.test.cjs eng\regression-bisect\workspace.test.cjs
```

The opt-in smoke freezes a harmless FSI fixture, uses the declared revision SDK,
builds the product graph cleanly and requests two verified local executions.
It prints its retained result/evidence location and exits 2 on an untestable
result. No live issue discovery or full compiler test suite is involved.

Local verification on 2026-09-19: corrective RED cases reproduced the graph's
`Dependency/Shipping` output mismatch and three staged/finalization cancellation
failures. Real subprocess cases reproduced exited-parent hangs and detached-server
timeouts. The final scoped suite passed **523 tests**, including 32 workspace
contracts. In-memory mutations to use Debug and bypass package validation failed
the command-contract case and all four package-identity cases, respectively.
Four additional parameterized cases first failed with the pass-only process
fixture, then passed with per-attempt process results: expected failure,
unexpected diagnostic, reproduction timeout and mixed repetitions. They exercise
the real worker/classifier and verify returned outcomes, complete repeats, and
digested `reproduction.json`/`observation.json` evidence. Forcing either the
worker's returned outcome or only its persisted observation to pass made all
four cases fail. Production outcome propagation required no change.
Changed-script syntax (including the PowerShell helper), formatting, and working/
sprint-range whitespace checks passed.
The real Windows x64 smoke pinned
`5a8b9adb034f420a5c2c1f577670c5eeeb454b7e`, build instance
`2f54fabe-fcb6-424d-a63e-dcd508028681`. Its accurate result is
**untestable / unavailable-sdk**, exit 2, as recorded by the old adapter.
That reason conflated absent checksum provenance with archive availability:
the exact SDK `11.0.100-rc.1.26420.103` archive itself returned HTTP 200.
Release metadata returned 200, but
`Sdk/11.0.100-rc.1.26420.103/dotnet-sdk-11.0.100-rc.1.26420.103-win-x64.zip.sha512`
returned 404 under both `https://builds.dotnet.microsoft.com/dotnet` and
`https://ci.dot.net/public`. Checkout/source-integrity checks passed and
`global.json` retained its hash. Acquisition stopped before the build entrypoint,
so no build binlog or local FSI result exists; this is not a green clean build
or compiler suite. All 18 evidence digests were verified. Retained evidence is
`Q:\fsharp-workspace-smoke-XI1JHA\result.json`, SHA-256
`9edb1e7f3786d4df56ad42989420e2de81a2cc518e96f9db3ebf8e48ffd609a5`.

### Fresh historical acceptance: 2026-09-19

Fresh evidence is retained separately under
`Q:\rb912-s5-20260919-acceptance`. The original failed result above was
rehashed and remains unchanged; no earlier reproduction artifacts were reused.
The supplied archive-200/checksum-404 case is a deterministic RED fixture.
During the fresh live run, the exact build-feed archive instead returned 404;
the existing official CI feed returned 200 for the same version/RID. Its separate
exact policy approval was used without substituting SDKs or weakening stable
release checks.

The clean workspace command completed with exit 0 at source
`5a8b9adb034f420a5c2c1f577670c5eeeb454b7e`, build instance
`6444a4de-52d5-40f6-b065-8969b515357c`. Bootstrap, Release product build, Core
pack, evidence-helper build and two **built FSI** executions succeeded.
Requested/resolved SDK was `11.0.100-rc.1.26420.103`; executing runtime was
`11.0.0-rc.1.26420.103`, Windows x64. Historical declarations stayed unchanged.
The measured CI archive contained 332,886,529 bytes, with SHA-256
`30a36953c458c07ae367c0642d5922973ad84c56be7c7bb8badb86dfa4fd9369`.
Both measured digests, public sources/statuses, zero redirects, retained archive
identity and same-byte installer receipt are preserved in the acquisition evidence.
This is **approved measured HTTPS**, not a claimed publisher checksum.

`verified-products.json` combines that fresh product with a newly acquired
publisher-verified stable SDK `10.0.401` / runtime `10.0.12`.
The full released-and-source reproduction command also exited 0:
**43 scenarios matched their expected outcomes**: 23 positive passes,
6 intended product failures and 14 intentional identity rejections.
The latter are negative controls, not accepted product passes. All four source
compiler/tasks/targets/Core fallback controls compiled successfully before
`wrong-identity` rejection. Both phases exercised real separate project/TFM
builds, Debug/Release and language/nullability settings, FSI, runtime execution,
diagnostics, implicit/explicit/central Core references and immutable older-producer
consumption. The final audit rehashed the older DLL/package, both SDK archives,
all inventoried products and all 45 clean-workspace evidence files.

Principal records, relative to that storage root:

| Record | SHA-256 |
| --- | --- |
| `fsharp-workspace-smoke-b3FroQ\result.json` | `b9c229bdc3d33001fd4462e6630371e9992b5dfdab070d3668c4be79ed1045b8` |
| `repro-both-01\smoke-results.json` | `0b14541fa738841a53f3c9c87dc4e82ae1070bd1240eee63461110c1724215be` |
| `acceptance-audit-final.json` | `ecdd073f5f1856e4d316ebab4a9324c4c040291cdd7e0c87bd53f290426d030d` |

Earlier fresh failures and their logs/receipts remain alongside these records:
archive absence, Windows environment case aliases, restore/build helper-TFM
disagreement and an oversized inventory command line. Their corrections have
deterministic regression coverage; none of those failed runs is relabelled green.
The final scoped suite passed **858 tests**, with no failures or skips.
All 17 changed/new JavaScript files passed syntax checks; repository formatting
changed no files, and whitespace checks passed.

Commands used (the reproduction output directory must be new when repeating):

```powershell
node eng\regression-bisect\workspace.smoke.cjs --run 5a8b9adb034f420a5c2c1f577670c5eeeb454b7e 'Q:\fsharp-worktrees\.bare' 'Q:\rb912-s5-20260919-acceptance' 'C:\Program Files\Git\cmd\git.exe'
node eng\regression-bisect\repro.smoke.cjs 'Q:\rb912-s5-20260919-acceptance\verified-products.json' 'Q:\rb912-s5-20260919-acceptance\repro-both-01'
```

The receipt-completeness, workspace-error and cross-coordinator path repairs were
accepted in a separate fresh run on 2026-09-19, retained under
`Q:\rb912-s5-i2-20260919`. The same pinned source built cleanly in instance
`8c180e7a-a99c-4889-8216-8c8b71d89a27`; its built FSI and the full released/source
project matrix exited 0. All **43 scenarios** matched expectations again:
23 positive passes, 6 intended product failures and 14 identity rejections.
Both SDKs were newly acquired, not copied from the prior run. The audit rehashed
both archives, product inventories, 45 workspace evidence files, 72 project
binlogs (including negative controls), and the frozen released producer DLL/package.
The original failed result and all records in the prior acceptance manifest
remain unchanged.

The scoped suite passed **872 tests**. Removing supplemental-runtime rehashing
made the otherwise-valid missing/changed-runtime cases fail; restoring it passed.
Eight observation/checkpoint cases also passed with POSIX receipt-path semantics
injected on Windows; reverting to coordinator-local paths failed all four
Windows cases. This is cross-host serialization coverage, not a Linux build smoke.
The evidence helper's exact-SDK Release build and `dotnet test -c Release` command
exited 0; that helper has no managed test cases. All 12 changed scripts passed
syntax checks, formatting changed no files, and whitespace checks passed.

| Fresh record (relative to `Q:\rb912-s5-i2-20260919`) | SHA-256 |
| --- | --- |
| `fsharp-workspace-smoke-G26zT9\result.json` | `cf5cd20c162c063123886a6900a1630221c5a098dd17c1e21c772580690c6935` |
| `repro-both\smoke-results.json` | `c86d9388032ca912c9a46181398a0922c8cdf06c8c477008f24be6c65fce1d6d` |
| `acceptance-audit.json` | `54098b840f0c1447010a5469045ee3ce997843dc4840cb14a2b6a94ae0b4be62` |

The network-failure observation repair was verified in another fresh run on
2026-09-19 under `Q:\rb912-s5-i3-20260919`. Build instance
`cd911f1f-8f3e-4584-8a81-98bf1339fdf2` cleanly built the same pinned historical
source and passed both built-FSI repetitions. A separate acquisition again
resolved publisher-verified stable SDK `10.0.401` / runtime `10.0.12`.
The released/source smoke exited 0 with **43 expected outcomes**: 23 passes,
6 intended product failures and 14 identity rejections, including real multi-TFM
execution and frozen released-producer consumption. The audit rehashed both
archives, 209 product files, 45 workspace evidence files, 72 project binlogs and
the producer DLL/package, and verified both prior acceptance manifests and the
original failed result were unchanged.

All **879 helper tests** passed. The network/checksum cases round-trip through
observation validation and authenticated checkpoint commit/reload with absent
product identities, preserving their reason, digest and charged budget.
Relabelling network failures as unavailable SDKs in memory failed both network
regressions. Changed-script syntax and whitespace checks passed. The exact-SDK
Release evidence-helper build/test command exited 0 (no managed test cases).
The initial formatter command lacked the root's SDK; rerunning with the freshly
acquired exact historical SDK succeeded and changed no files.

| Fresh record (relative to `Q:\rb912-s5-i3-20260919`) | SHA-256 |
| --- | --- |
| `fsharp-workspace-smoke-Np6g88\result.json` | `1d4200a191ca02a5ad9561016fde06468c4d3e0af76168e1b77a5019f69cd4f2` |
| `repro-both\smoke-results.json` | `3450faeb7139f429d7d49eb2457f4e43630a27f16a6beae2e29a2940c6314691` |
| `acceptance-audit.json` | `19881e6269b707bd9d06e1e0df031bcf4ab36e434a8c913a307b2fd1ed1b27ce` |

The installer tests were consolidated and revalidated on 2026-09-19: the existing
wrong-SHA256 table row now also requires `checksum-provenance` and the SHA256
diagnostic, retaining its no-process-execution assertion. Removing the duplicate
case reduced the suite to **878 passing tests** without removing a scenario.
In-memory mutations of the error code and message each failed that row.
Fresh acceptance is retained under `Q:\rb912-s5-i4-20260919`: clean build
`03aaca4c-f4cc-4ffd-8da6-90ff66775f9c` and both built-FSI executions passed;
all 43 released/source scenarios matched their expected outcomes (23 passes,
6 intended product failures, 14 identity rejections). The exact-SDK Release
evidence-helper build/test commands, syntax, formatting and whitespace checks
passed. The audit rehashed both archives, 209 product files, 45 workspace evidence
files and 72 project binlogs; all three prior acceptance manifests and the original
failed result remain unchanged. `acceptance-audit.json` has SHA-256
`cae5825442f29472f95a4490f983926e52b23a01cbfe5a0631017f82b50959b4`.

The redirect-provenance repair was revalidated under `Q:\rb912-s5-i5-20260919`.
Before the fix, 32 native/injected HTTP provenance assertions failed while the
three archive-absence/network controls passed; all now pass. The full suite has
**898 passing tests**, with zero failures or skips. Fresh clean build
`a03fc535-eaeb-4389-87cd-e7300655dc8a` and both built-FSI executions passed for the
same pinned historical source. All 43 released/source smoke scenarios matched
expectations: 23 passes, 6 intended product failures and 14 identity rejections.
The audit rehashed both archives, 209 product files, 45 workspace evidence files
and 72 project binlogs, and confirmed all four prior acceptance manifests and
the original failed result remain unchanged. Exact-SDK Release evidence-helper
build/test commands, syntax, formatting and whitespace checks passed.
`acceptance-audit.json` has SHA-256
`c426ad8a5fbeb65aa0750d00088eaba77051a57c4be112ceb94e47112a9ae8ab`.

## Reproduction execution

`repro.cjs` is an execution adapter, not an authorization boundary. Its caller
must first admit the frozen manifest and place execution in a disposable,
credential-free worker. It does not fetch issue code, choose revisions, bisect,
or publish results. A manifest digest alone is not approval.

The exports are `runReproduction`, `selectFsiCommand`,
`verifyProjectIdentities`, and `evaluateAssertion`. Outcomes and the two-run
consistency rule come from `model.cjs`. A successful build never substitutes
for the runtime assertion.

`runReproduction` takes:

* `manifest`, `sourceDirectory` containing exactly the reviewed source assets,
  and `workDirectory` for fresh per-observation/per-attempt workspaces;
* `toolchain`: the verified `identity` returned by `acquireToolchain`;
* `product`: `{ kind, root, sourceSha, buildInstanceId, configuration,
  runtime: { name, version }, corePackageVersion, files }`. `kind` is `source`
  or `released`; local `root` is the clean build checkout, not this adapter's
  checkout. Each inventoried file has `role`, absolute `path`, and `sha256`;
  assemblies also have `assemblyVersion`. Roles are `fsi`, `compiler`,
  `tasks`, `props`, `targets`, `sdkTargets`, `overrides`, `core`, and
  `corePackage`. Managed FSI/compiler records carry
  `runtimeConfig: { path, sha256 }`. There must be exactly one selected
  artifact per role: this adapter does not search for the newest build;
* `helper`: `{ executable, runtimeVersion, dll, hook }`, identifying the
  separately built evidence reader and Core-load startup hook;
* optional `sampleRuntimes`, mapping each TFM to an explicitly acquired
  `{ name, version }`. Omission uses the product's explicit runtime, never
  automatic major-version roll-forward.

The adapter checks inventory bytes and assembly versions, then rechecks frozen
inputs and product bytes after execution. Local FSI uses the selected `fsi.dll`
with an absolute acquired host and `exec --fx-version`; it never invokes
`dotnet fsi`. Windows executable layouts use a separate fixed Core-load probe.
Managed executions capture the actual loaded Core through a startup hook,
including its path, digest, assembly version, runtime version and architecture.
The hook deduplicates assemblies weakly so observation does not keep collectible
load contexts alive.
Unsupported host/runtime/helper combinations are untestable.

Project builds reuse **both** `UseLocalCompiler.Directory.Build` imports.
The local compiler TFM comes from the inventoried runtimeconfig, not this
checkout's current product TFM. Each repetition has a distinct `NUGET_PACKAGES`;
same-version packages from different builds cannot share an extraction cache.
Core package bytes and the selected compile/runtime DLL entries are checked,
not just their version strings. Other packages must match the manifest's
frozen versions and package digests.

MSBuild's compiler child uses an explicit `LatestPatch` policy only when there
is exactly one compatible, acquired runtime in that host. PID-separated startup
hook records must prove that the actual `fsc.dll` process loaded that runtime's
CoreLib bytes and the inventoried compiler Core. This permits a pinned compatible
prerelease runtime without editing the built runtimeconfig or silently selecting
another installed runtime. FSI and application execution use `--fx-version`.

`ReproEvidence.targets` contributes build-context evidence; that evidence alone
is insufficient. The pinned structured binlog reader must find the executed
Fsc task and correlate its assembly, command, references and flags with imports,
assets and the expected project/TFM/configuration cell. Missing inner builds,
skipped Fsc tasks, fallbacks and late flag overrides are untestable.
The pre-compile snapshot includes `Nullable`: the reader accounts for Fsc's
generated `NULLABLE` define and compares the executed `--checknulls` setting,
including when that define is also explicitly declared. Referenced projects must
be reviewed sources, with complete compiler/Core evidence for their selected
TFMs; ambiguous or missing dependency builds cannot be ignored.

Assertions compare complete, bounded streams and exit codes. Line endings are
normalized to LF. Compile-time assertions use a frozen diagnostic of the form
`Program.fs(line,column[,endLine,endColumn]): error FSnnnn: message`, including
the relevant message and span. The reader supplies structured diagnostics with
workspace-relative paths; unrelated diagnostics or restore errors cannot
satisfy the assertion. All other manifests execute the produced application.
Timeouts terminate the process tree; truncated output, unexpected crashes,
missing identity evidence and mixed repetitions are untestable.

An immutable producer is declared in a reviewed `*.producer.json` source asset:
`{ role: "immutable-producer", compiler: { version, sha256 }, flags, tfm,
packages, dll: { path, sha256, resources }, package: { path, id, version,
sha256 } }`. The DLL and optional package are themselves reviewed source
assets; `manifest.producer.sha256` identifies the DLL. Resource records contain
the F# metadata resource names and digests. Consumers must reference those bytes,
not rebuild a producer ProjectReference. An explicit absence of producers is
`{ name: "no-producer", version: "0.0.0", sha256: SHA256(empty bytes) }`.
Producer dependency provenance is retained separately from the candidate's Core;
the adapter never rewrites or repacks a frozen producer.

`result.json` retains outcomes, attempts, command arrays and evidence paths.
`identities.json` retains the exact product/host/SDK/runtime/package identities
and the acquisition digest. Binlogs and loaded-Core evidence remain alongside
each attempt. These are execution records, not authenticated controller
observations or publication-ready issue text.

### Bounded reproduction smoke

The opt-in `repro.smoke.cjs <verified-products.json> <new-output-directory>`
entrypoint generates only small synthetic fixtures. The configuration has
`released` and `source` objects containing `{ product, toolchain, tfm,
sampleRuntimes? }`, plus the shared `helper` above. Supply an acquired released
SDK and products from a separate clean checkout built with:

```powershell
eng\Build.ps1 -restore -build -c Release -pack -bootstrap -noVisualStudio -msbuildEngine dotnet
```

Use an explicit credential-free environment for that build. Do not reuse this
worktree's product artifacts or run competing builds in the temporary checkout.
Windows requires static `OS=Windows_NT` and `PATHEXT=.COM;.EXE;.BAT;.CMD`;
NuGet also needs private `ProgramFiles`/`ProgramFiles(x86)` paths. Do not copy
the user's credentials or machine-wide NuGet configuration to satisfy these.
Build the reader with the explicit
`eng\regression-bisect\evidence\Evidence.csproj` project. Its pinned
`MSBuild.StructuredLogger` dependency reads binlogs, and its dependency-free
`netstandard2.0` hook can run on older Core runtimes independently of the
reader's .NET 8+ host.
The workspace builds it against the acquired runtime with
`-p:EvidenceTargetFramework=net<major>.0`; this property survives restore graph
evaluation, unlike overriding the special `TargetFramework` global property.
The hook retains its own `netstandard2.0` target. Helper restore uses the
repository's public `dotnet-public` mirror, not a machine feed or global cache.

The smoke covers FSI, Debug/Release projects, successful compilation followed by
an intended runtime failure, intended diagnostics, late compiler flag overrides,
explicit/central Core package overrides, two inner TFMs on Windows, and an immutable released
producer. Source-only negative cases successfully compile with SDK compiler,
tasks, targets or Core fallbacks, then require `wrong-identity` rejection.
It also covers nullable settings and late overrides, app/library
references with same/different TFMs, and collectible Core unloading with and
without the observation hook. Its source-evaluation control illustrates how rebuilding a producer
without its old flags would hide the failure; candidate consumers only receive
the frozen DLL/package. Results are written to `smoke-results.json`.
An explicit final `released` or `source` argument selects just that phase while
preparing the other product; a released-only pass is not completion of the
clean-local-product acceptance gate.

Scoped checks (a full compiler test run does not prove injection):

```powershell
node --test eng\regression-bisect\model.test.cjs eng\regression-bisect\toolchains.test.cjs eng\regression-bisect\repro.test.cjs
node eng\regression-bisect\repro.smoke.cjs <verified-products.json> <new-output-directory>
git diff --check
```

Syntax-check changed `.cjs` files with `node --check` as well. Do not report
the real smoke as passing based on mocked process output or Node tests.

Validation recorded on 2026-09-19: all **466** scoped Node tests passed. The
prescribed clean Release/bootstrap/pack build completed with exit 0 at source
`553101960fcb8551d305fc5c0dabad9f2a3e70f3`. All **21** bounded fixture scenarios
matched their expected outcomes, including both deliberate late-override
rejections; normal assertions were repeated twice and both declared inner TFMs
were verified. The reproduction SDK was **10.0.401**, with runtime **10.0.12**
for released FSI/applications and explicitly acquired runtime
**11.0.0-rc.1.26425.128** for the clean local FSI/compiler. Local Core package
**11.0.101** and the released producer's DLL, package and compressed F# metadata
were preserved and checked by digest.

The clean build itself used the repository entrypoint's pinned SDK
`11.0.100-rc.1.26420.103`. Its unavailable archive checksum sidecar was recorded
as a separate acquisition failure, not converted into an acquisition success:
the source reproduction instead used the independently acquired SDK/runtime
above, both of which passed the existing acquisition identity guard.
Complete execution evidence is retained at `Q:\rb912-s4-validated-fixtures`;
`smoke-results.json` SHA-256 is
`b182f1a3c504a9d4da94b0cfda10cde5e5280c0e20deb89338160038e3991ff1`.
These checks do not imply a full compiler-test-suite pass.

Repair validation on 2026-09-19: **483** scoped Node tests and **67** targeted
reproduction tests passed. Before the fixes, the collectible-Core fixture
reported a product failure only with the hook, and a normal nullable-enabled
project was rejected. Both now pass; late nullable changes in either direction
are rejected even with an explicit `NULLABLE` define. Independently replacing
containment with prefix comparison or bypassing realpath resolution made the
corresponding repaired test fail.

All **39** real bounded scenarios matched their expected outcomes under both
released and local products, including app/library TFM selection and hook-free
unloading controls. The unchanged clean local product build above was reused
after rechecking its pristine source checkout, successful build binlog and
artifact digests; the repaired evidence helper was rebuilt in Release with
zero warnings/errors. Evidence and exact identities are retained at
`Q:\rb912-s4-repaired-fixtures`, including `validation-summary.json`.
`smoke-results.json` SHA-256 is
`2b61651b316f343a2be775d203467437ab0104d8b6d14765981d1972808ab47c`.
Syntax checks, `dotnet fantomas .` (no changes), and diff checks passed.

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
  Rotate by adding a new ID/key and selecting it for new signatures, retaining
  all old verification keys and trusted workflow revisions needed by the
  checkpoint, result and append-only anchor history. Removing a required old
  key blocks recovery; never rotate by creating a new anchor or allowance.
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
present. Initialization accepts the exact authenticated `checkpoint-slot`
proposal described above, or recovers its authenticated interrupted
generation-zero checkpoint described below. There is deliberately no
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
| `beginWork(transport, context, { generation, attemptId })` | Durably claims execution for the owning reservation. Returns `{ execute, generation }`; duplicates cannot execute again. Commit using this execution generation. A claim does not add a second charge or refund cancelled work. |
| `commitObservation(transport, context, { generation, attemptId, observation, evidence })` | Checks ownership, request/build/provenance and the shared observation contract, then commits all decisive evidence before clearing pending work. Exact duplicate completion is idempotent. |
| `invalidateStaleEvidence(transport, context, { generation })` | Invalidates changed input/approval/producer/orchestration dependencies without erasing history or charges. Main/stable changes start a new endpoint epoch while retaining exact-input historical source observations. Closed/unlabeled issues stop publication; existing result identities remain auditable. |
| `recordSearchProgress(transport, context, { generation, phase, interval, goodDigest? })` | Persists engine phase/interval and an optional authenticated passing-source endpoint. Typed decisions must match durable observations. Cannot alter a pending reservation, finish attribution, erase usage or refund work. |
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
