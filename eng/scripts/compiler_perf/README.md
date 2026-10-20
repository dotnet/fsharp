# Compiler-source measurements

`measurement.py` is a Python 3.11+ standard-library CLI and importable module.
It prepares and replays **FSharp.Compiler.Service compiler sources**, not the
FSharp.Core benchmark or generated-program runtime benchmarks. A decision is
measurement evidence, **not permission to publish** an optimization.

The restored-checkpoint validation records below describe earlier local runs. Their ignored
interpreters, compiler binaries, logs and traces were not recovered with the
Git checkpoint and are **unavailable**, not present-day evidence. Recollect
missing artifacts; verify any recovered file's existence and hash before reuse.
New captures against current main require new workload fingerprints.

`memory.py` preserves versioned attempts, negative decisions, retry dependencies
and outstanding publication intents in the local Git journal. Its receipts
explicitly report `remote_acknowledged: false`; local CAS is not remote
durability. Historical root `dotnet test -c Release` receipts remain readable,
but cannot authorize new publication. Correctness schema 5 binds explicit plans,
candidate identities and runner-reported results to retained invocation, discovery,
XML and outer-runner captures. Passing memory fixtures does not certify production acceptance.

## Trusted lifecycle

`workflow.py` connects the helpers from a protected controller checkout. Its
`preflight`, `investigate`, `checkpoint`, `validate-output` and `finalize`
subcommands require `--repository`, an owned local Git cache `--journal`, and
an owner-only `--root` **outside the source checkout**. `preflight --input`
accepts the discovery request below (version 1 experiment requests remain
readable for retained controllers); it does not accept commands, remotes,
branches, credentials, policy changes or deadline overrides. All JSON is bounded,
versioned data. Exit 0 means the operation completed (including a rejected or
no-finding experiment), 2 means blocked setup/collection/publication, and 130
means a gracefully cancelled investigation.

Preflight verifies the default-branch revision and every page of structured PR
metadata, including author and publication identity. API denial is not an empty
list. Planned Release compiler-source, helper, SDK, architecture and policy
identities are frozen before a reservation; actual bootstrap/workload/compiler
identities are added by preparation and measurement. Only recorded relevant
compiler paths or SDK changes invalidate a rejected hypothesis; a README commit
does not. Active owners and incomplete publication intents block duplicate work.

`transport.py` binds production to `https://github.com/dotnet/fsharp.git` and
`memory/compiler-self-profile-state`. It checks exact-ref absence, fetches and
validates state, performs an ancestry-checked expected-head update, and verifies
remote readback. Conflicts preserve other decisions and unmerged local candidates.
The transport returns its acknowledgment directly, with `scope`, `acknowledged`,
`commit`, `branch`, `remote` and the unchanged GitJournal receipt in `local`.
That nested receipt still says `remote_acknowledged: false`. The controller checks
both identities; a `local_fixture` acknowledgment is only local-Git test evidence,
never GitHub durability. The native
`memory/compiler-self-profile` branch remains untrusted notes, not authority.

After a lost acknowledgment, read the fixed ref before retrying. A reservation
that reached the remote keeps its original owner even if the local session was
not saved. Missing authenticated session custody blocks execution, rather than
inventing a second owner from the unsealed checkpoint. Retained candidate refs
are not discarded by reconciliation.

Investigation refuses missing Linux/bubblewrap, pinned SDK or collector
capabilities before bootstrap or profiling. `investigate` prepares the controller;
it does not consume a seeded proposal or conclude that an empty inbox is a finding.
The adapter starts profiling when the agent is ready, delivers CPU stacks,
sampled allocation estimates, GC/heap/RSS distinctions and binlog conclusions,
then waits for an explicit decision. Discovery ownership uses journal schema 2,
without manufacturing a hypothesis or a rejected attempt. Version 1 journals
upgrade on their first discovery reservation.

One aggregate 3,600-second setup deadline includes prerequisite probes and
preparation. Readiness must fit the separate 600-second startup allowance; a late
agent is blocked rather than receiving a shortened investigation. The authenticated
14,400-second active deadline covers profiles, agent think time, revisions, worker
waits and validation. Monotonic elapsed time bounds a live process; sealed absolute
deadlines and last-observed wall time bound restart. Reentry cannot renew either
clock, and backwards time cancels incomplete work. Finalization has a separate
2,400-second bound. Cancellation preserves the last acknowledged journal timestamp
if wall time moves backwards. A worker entering finalization is monitored against
that existing bound, not interrupted merely because the active clock expires.
`runtime.py` connects these clocks to the pinned lifecycle below. Its local
checks do **not** prove a compiled workflow graph or live Linux containment.

Candidate execution uses the existing empty Linux namespace, never a host fallback.
Only bounded patch bytes enter the controller inbox through the data endpoint.
Controller code, frozen inputs, receipts, custody key, journal and output must be
outside **every** agent mount, including default workspace, home and temporary
mounts. A hard runner cancellation may prevent the last checkpoint or finalizer;
earlier acknowledged state remains authoritative, not proof of completion.

`artifacts.py` retains checked summaries and encrypted diagnostics, with a
512 MiB run budget (including ciphertext overhead and manifests), 20 MiB compact
budget and 14-day retention. Public Actions artifacts are not private storage:
raw traces/dumps must never enter upload roots or Git. No trace is truncated to fit.

Encryption requires the official **age v1.2.1** executable
([pinned release](https://github.com/FiloSottile/age/releases/tag/v1.2.1)).
The owner provisions and verifies the release before configuring the protected
controller's `retention-config.json` with exactly `recipient` (one native
`age1...` X25519 public recipient) and `tool` (`path`, `sha256` of that executable).
No private key, passphrase, SSH recipient or plugin is accepted. Recipient
configuration is never taken from agent messages, issues or PRs. Encryption uses
the pinned `--encrypt --recipient ... --output ...` contract, credential-free
tool environments, checked input handles and complete binary output; the
executable identity/version and output framing/length are verified.
On Windows, read handles deny writes and replacement while hashing/encrypting;
read-only access-time updates cannot masquerade as changed diagnostic bytes.

Missing/malformed configuration, tool/version/exit failures, incomplete output,
read/write denial and budget overflow block retention explicitly, without a
plaintext fallback. Compact conclusions remain eligible even when a binary
cannot be retained. Evidence records keep the original kind, plaintext hash and
byte count separately from ciphertext hash/count, recipient identity, tool hash,
run/workload fingerprint and expiry. Ciphertext plus summaries and manifests
share the per-run budget; repeated staging cannot renew retention or reset it.
Accounting includes atomically moved manifests whose session save was interrupted,
as well as recorded uploads; expired binaries do not replenish the run budget.
Missing or changed recorded manifests block further staging rather than resetting
that budget.

`Controller.stage_evidence()` returns the exact allowlisted upload files and
hashes, bounds, expiry and pending/expired/blocked status. Never upload a
directory recursively. Staging is atomic, and a sealed write-ahead job permits
restart after the file move but before acknowledgment. Journal schema 3 adds
discovery evidence without inventing an experiment or rewriting old decisions;
artifact schema 2 distinguishes local pending transport from remote availability.
Version 1/2 journals remain readable. Old local-summary records are not treated
as remote receipts. Expiry removes availability, not conclusions or hashes.
The controller seals each manifest before its atomic staging move. Restart
recovers compact records from that authenticated manifest even after ciphertext
expiry or deletion, or loss of its source dump workspace, without encrypting or
uploading expired evidence again. Missing or altered manifests cannot supply
recovered records.
Analyzed dumps surviving interruption are removed only through their sealed
workspace and file-identity receipts, including after expiry. A replacement
file or denied deletion blocks retention and is never silently discarded; the
verified encrypted upload inventory and compact records remain intact.

Only `Controller.acknowledge_evidence(sink)`, called by the protected runtime,
can advance pending records. The sink receives the checked inventory,
`manifest_sha256`, `retention_days: 14` and the evidence deadline `expires_at`.
Its successful receipt must bind the manifest hash and 14-day policy,
`scope: actions`, `artifact_id` and the matching run's Actions artifact `location`,
plus the actual transport `created_at`/`expires_at`. Those timestamps must prove
14-day retention at the upload step. A delayed upload records its real storage
expiry but never extends the original evidence-availability deadline. Uploads
finishing after that deadline cannot be acknowledged. A `local_fixture` receipt never establishes
remote availability. Receipts saved in authenticated controller custody can be
checkpointed after process restart without uploading again; partial-batch retries
preserve completed receipts and send only manifests with unacknowledged evidence.
Receipts lost before that sealed save remain pending and require trusted sink
reconciliation; agent-supplied receipts cannot mint approval.
No production uploader or additional storage service is installed here.

`validate-output` freshly reconciles sealed controller captures with the pinned
runner selection, discovery and XML, recomputes measurements using immutable
policy, and reapplies the proposal in an independent disposable Git tree. Custody
authenticates retained invocation evidence, **not arbitrary test-body honesty**.
Unsigned, copied or altered receipt booleans are not approval. The controller
reserves one deterministic intent before emitting `output/safe-output.json` and
`output/proposal.patch`. The approval binds the entire `items` payload, repository,
base, run, attempt, candidate tree, exact am patch bytes and evidence. GH AW
v0.76.1 must be configured with `patch-format: am`, one draft, no fallback issue,
no alternate transport and no bundle. Its `branch`, `base`, `base_commit` and
`patch_path` fields are fixed by the controller, not agent output.

`finalize` reconciles exact branch/head/author/base/draft/intent metadata and saves
the actual PR number and URL. It has no CLI push, merge, ready-for-review, issue
or comment writer. Retrying `validate-output` after interruption freshly validates
the candidate and reuses only the exact expected patch and payload, without
rewriting either file. Changed or extra output stays blocked and preserved.
The publisher transport is exercised only through a fake
effect sink in portable tests. A future workflow must consume only the sealed,
rechecked output in a credential-separated publisher job; these helpers do not
install a schedule or grant permissions. Windows/local-Git fixtures do not prove
Linux isolation or a real compiler-source improvement.

Portable lifecycle checks (Python 3.11+, Git, and the pinned SDK for the existing
offline SDK-resolver test): install `requirements-agent.txt` in an owned Python
environment for HTTP MCP coverage, and use that interpreter below. Put the exact
SDK on `PATH` and verify `dotnet --version` matches `global.json`; roll-forward
or a skipped SDK test is not pinned-SDK validation. Set
`COMPILER_PERF_TEST_AGE` to the absolute pinned executable path, with its
`age-keygen` sibling. Encryption tests generate disposable recipients outside
Git, decrypt actual ciphertext and remove their private keys afterward. Never
point a fixture at production key material.

```powershell
python -m unittest discover -s tests\CompilerPerfWorkflow -p "test_workflow*.py" -v
python -m unittest discover -s tests\CompilerPerfWorkflow -p test_memory.py -v
python -m unittest discover -s tests\CompilerPerfWorkflow -p "test_*.py" -v
```

These use owned local bare repositories and fake publication effects. Optional
Linux isolation and live runner tests have separate prerequisites and are not
certified by portable results.
On 2026-09-20, the prescribed suites completed on Windows with Python 3.13.7:

| Suite pattern | Tested commit | Tests run | Skips | Duration |
| --- | --- | --- | --- | --- |
| `test_workflow*.py` | `31c4e4c1ffa` | 59 | 0 | 1984.891 s |
| `test_memory.py` | `31c4e4c1ffa` | 38 | 0 | 44.533 s |
| `test_*.py` | `e60d1f04819` | 203 | 7 | 3290.137 s |

All three recorded runs exited zero with no failures or errors. The complete
helper run required repairing fixture Git initialization to reuse the existing
allowlisted environment; its earlier 28 setup errors remain failed evidence.
All six independent publication crash rows and seven tampering rows completed,
along with lost-acknowledgment, authority-renewal, CAS and exact-PR-identity checks.
The seven skips comprise two Linux/bubblewrap methods, two opt-in outer-runner
methods, two opt-in trace/dump methods, and one Windows symlink-privilege subtest
(`change='patch-link'`), not seven skipped methods. The offline SDK resolver ran.
These historical results precede the subsequent main merge and do not certify
the newer agent interface. No Linux, live, hosted or GitHub-durability coverage
is implied.

### Agent integration validation (2026-09-20)

Complete helper discovery passed on Windows with Python **3.13.7**, MCP **1.30.0**
and SDK **11.0.100-rc.1.26420.103**: **231 tests, zero failures/errors, seven skips**,
in **3551.924 seconds**. The skips remain six opt-in/Linux methods and the
Windows symlink-privilege subtest listed above. The actual HTTP SDK client,
synthetic accepted/rejected agent interactions, local bare-Git CAS, immutable
deadline/restart tests and offline SDK resolver all ran.

| Validation | Tests | Result | Seconds |
| --- | --- | --- | --- |
| Initial agent RED | 11 | Two new tests errored because discovery schema 2 was unsupported | 104.378 |
| Intermediate agent run | 12 | One HTTP response-shape error and one early-worker-exit failure | 641.678 |
| Agent matrix GREEN | 14 | Passed, no skips | 319.074 |
| Final affected memory, setup and checkpoint selectors | 2 + 2 + 4 | Passed, no skips | 0.028 + 3.589 + 8.715 |
| Frozen complete discovery | 231 | Passed, seven skips | 3551.924 |

The initial RED command also discovered nine imported existing tests; the shared
fixture import was corrected without removing those tests from their original
module. The final aggregate-preflight-deadline test was added after the 14-test
matrix and passed both its focused invocation and complete discovery.

Frozen tested tree: `43bd4bb58f3d4157c390fc19fc51e3032b198c03`.
Full-run recorded executable/fixture input manifest SHA-256:
`44ed3fd02bfebc67836c37fee212ca1f9104e5348d01f8c758a3f27d56c0896a`.
The full command exited zero in 3553.591 seconds including runner overhead,
below its immutable 3600-second bound. The single portable batch, including
failed attempts and targeted checks, spent 5020.950 seconds of 7200.
Executable/test/SDK inputs were compared after completion and were unchanged;
this evidence-only README addition does not require another full replay.
Raw PID/start/deadline/exit records and input-file hashes remain private session
evidence, not repository contents.

These are portable helper results, not a compiler-source speedup or Linux
containment proof. No schedule, hosted run, remote write or real PR was created.
Compiler build/test and live Linux execution were not part of this portable
validation.

### Agent repair validation (2026-09-20)

Executable source blobs now export as nonexecutable files; symlinks and Git links
remain rejected. Clock-anomaly cancellation persists through canonical Git, and
output-validation cancellation cannot leave discovery ownership active. The
watchdog preserves an already-started finalizer. Normalized legacy histories
retain every independent branch's latest rejection and require relevant
invalidation against all of them before selecting a deterministic retry link.

The repair RED run executed **7 tests** with **37 subtest failures and one error**
in **49.755 seconds**, reproducing the reported defects. An intermediate command
hit its recorded 125-second cap; only its owned process tree was stopped.
Another run exposed a test-clock mistake: real elapsed preparation time had
already consumed the asserted one-second margin. The boundary test now injects
both clocks rather than assuming fixture setup is instantaneous.

Final focused GREEN: **13 tests, zero failures/errors/skips**, in **142.144 seconds**
(143.861 seconds including command overhead), using Python 3.13.7 and MCP 1.30.0.
Coverage includes canonical legacy retries and backwards-clock cancellation,
immutable deadlines/restart, output-authorization cancellation, finalizer races,
successful and expired preparation, executable dump operations with deletion,
regular-file export modes, unsafe mounts, actual HTTP authentication/origin/host
checks, and early worker exit. The frozen executable/test tree is
`942a37671b1bb4f45b37bb851b71d399a5d4d58e`; input manifest SHA-256 is
`529126d0fae8328e64fba28ab97b52231846888153da24b68f11e198f5370a1b`.
Inputs were unchanged after the run; this README is evidence-only.

### Retained-input revalidation (2026-09-22)

The retained `b915376d635` helpers, with the Windows artifact timestamp repair,
passed the prescribed serial suites on Python **3.12.10**, MCP **1.30.0** and
the exact SDK **11.0.100-rc.1.26420.103**:

| Suite pattern | Tests run | Skips | Duration |
| --- | --- | --- | --- |
| `test_workflow*.py` | 83 | 0 | 2335.652 s |
| `test_memory.py` | 51 | 0 | 87.132 s |
| `test_*.py` | 239 | 7 | 3322.574 s |

All exited zero. Complete discovery includes the scripted rejection/synthetic
improvement interaction, actual HTTP SDK client, offline SDK resolver, repaired
clocks/export/retries, direct receipt contract and publication/restart fixtures.
The seven skips remain the two Linux methods, two opt-in runner methods, two
opt-in trace/dump methods and Windows symlink-privilege subtest described above.
Tested input manifest SHA-256:
`755c097e548a22d77a4298b94b57438e894d626b91a89568008797107940e72a`.
Source and tool inputs were unchanged throughout; this README update is
documentation-only. Per-command bounds, process identities, hashes, RED/GREEN
results and logs remain private evidence.

Windows `stat`/`fstat` expose different ctime semantics in Python 3.12, and handle
ChangeTime can advance on read-only access. Artifact reads retain birth-time,
hash, identity, size, link and modification-time checks, with Windows handles
denying writes and replacement until both hashing and path verification finish.
POSIX retains cross-API change-time checks. Timestamp-preserving rewrite tests
require write denial on Windows or changed-byte rejection on POSIX; neither
platform may return a stale hash or delete a replacement dump. The historical
results above do not certify these guards; exact-input evidence remains private.

No task-owned Linux executor was available: the only WSL distribution was stopped
and both Docker engine probes failed. Live namespace, attach/dump and network/model
proxy acceptance remain unverified. Temporary-recipient encryption coverage is
local-only, not production-recipient validation or live artifact durability. The owner
handoff requests separate fixed-revision deployment and Linux enrollment
(`owner-launch-enrolled`), not ancestor hot-reload or a portable-validation gate.
Retained launch pins are GH AW **v0.76.1**, Copilot **1.0.52** and AWF **v0.25.55**;
installed GH AW **v0.88.2** and Copilot **1.0.87** are not substitutes. Diagnostic
tools **8.0.547301** were verified. No hosted or remote-write effects occurred.

## Agent interaction

The protected owner supplies the current default-branch SHA and run ID, not a
hotspot, hypothesis or regression selector:

```json
{"schema_version":2,"run_id":"12345","base_sha":"aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa","primary_metric":"elapsed_ms"}
```

For standalone helper use, run `workflow.py preflight --input discovery.json`,
then `workflow.py investigate`, with the same protected `--repository`, `--root`
and `--journal` arguments. Actions must use the runtime entry points below.
Preflight checks the default branch, **all** PR pages, live owners and outstanding
publication before canonical CAS reservation. Setup uses the pinned SDK and local
Release FSharp.Compiler.Service compiler-source workload. Do not remove `global.json`.
Dependency preparation belongs before agent execution, not in its active budget.

`agent_bridge.py` exposes one MCP tool, `investigate(message)`. Its argument is
strict JSON with exactly `schema_version: 1`, a bounded unique `id`, `operation`,
and `data`. Unknown fields, duplicate keys, nonfinite values, invalid UTF-8 and
oversized messages are rejected. All messages, diagnostics and source/PR text
are data, never shell instructions. Reusing an ID with different bytes is rejected;
retrying an acknowledged mutation returns its saved acknowledgment.

| Operation | `data` | Effect |
| --- | --- | --- |
| `status` | `{}` | Current phase, diagnostics, previous hypotheses, revision results and clocks |
| `start_profile` | `{}` | Starts the owned profiling worker; returns without waiting for collection |
| `select_hypothesis` | `compiler_paths`, `regressions`, `hypothesis`, `complexity`, `hot_operation` | Queues selection of a displayed stack index and compiler-source scope; `status.selection_result` reports reservation, skip or blocking reason |
| `submit_patch` | `patch` | Retains bounded UTF-8 Git patch bytes; cannot replace an already submitted revision |
| `start_validation` | `{}` | Starts real candidate preparation and isolated correctness/unprofiled measurement |
| `request_dump` | `reason`, `hypothesis` | Reason is `retention`, `heap-growth` or `unexplained-rss`; controller launches and verifies its own diagnostic compiler process and destination |
| `finish_no_finding` | `reason` | Queues trusted finalization; only untested, completed valid diagnostics can end as healthy no-finding |
| `finish_rejected` | `reason` | Queues finalization with the retained negative experiment outcome, not a speedup or healthy no-finding |
| `cancel` | `reason` | Cancels owned work and preserves incomplete evidence |

`regressions` uses the existing `{path, method}` selector schema. Paths select only
the existing bounded compiler/test scope; no operation accepts arbitrary filesystem
paths, command lines, URLs, environment, credentials, timeouts, policy, passed-test
lists or acceptance decisions. Git derives the actual allowed diff after applying
the patch. Rejected patches and reasons remain retained after rollback. A different
eligible observed operation may be investigated in the same session, but changing
an ID or prose cannot bypass the recorded relevant-dependency retry policy.
The existing path-scoped hypothesis identity is preserved, including version-1
negative history. Same-scope retries require a recorded compiler-path/toolchain
change; unrelated README changes or selecting another stack for the same scope
do not qualify.

Individual start/status calls do not own the four-hour task. The bridge retains
worker handles and sealed launch/checkpoint records across MCP reconnects. Restart
must reconcile an existing worker rather than launch a duplicate or trust a reused
PID. No per-worker one-hour cap is introduced. Missing tools, failed collection,
failed correctness, noisy measurements and cancellation remain distinct failures.
An accepted result still requires the protected `validate-output` boundary, fresh
correctness/measurement reconciliation and one canonical draft intent; the agent
cannot invoke publication or self-report a speedup.

### HTTP transport and containment

GH AW **v0.76.1**, commit
[`58d1bedbb7200f59c2d224151339e38fd8687d05`](https://github.com/github/gh-aw/tree/58d1bedbb7200f59c2d224151339e38fd8687d05),
supports host HTTP MCP through its gateway. Its
[custom-server validator](https://github.com/github/gh-aw/blob/58d1bedbb7200f59c2d224151339e38fd8687d05/pkg/workflow/mcp_config_custom.go)
rejects arbitrary non-Docker Python stdio. Use the isolated protocol dependency in
`requirements-agent.txt`; ordinary helper imports do not require the MCP SDK.
Install it in a controller-only Python 3.11+ environment:

```powershell
python -m venv protected-agent-env
protected-agent-env\Scripts\python -m pip install -r eng\scripts\compiler_perf\requirements-agent.txt
protected-agent-env\Scripts\python eng\scripts\compiler_perf\runtime.py configuration
```

Start the protected HTTP server before gateway initialization. It creates or
reuses the owner-only `<root>\endpoint-key`; only the trusted gateway receives
that scoped bearer capability. The supported
configuration fragment is `mcp-servers.compiler-perf` with `type: http`,
`url: http://127.0.0.1:8765/mcp`, `allowed: [investigate]`, and an `Authorization`
header carrying **only** the endpoint capability. Never use a write-capable Git
token. HTTP entries cannot specify `command`, `args`, `env` or `mounts`.
Use `engine.mcp.tool-timeout: 5m` (below the supported 600-second maximum).
Omit `engine.mcp.session-timeout`: GH AW v0.76.1 renders it as `sessionTimeout`,
which the pinned gateway v0.3.19 schema rejects. The gateway's
[session default is six hours](https://github.com/github/gh-aw-mcpg/blob/a74a1c1525e1e3871370e22801ef4476de00cdfa/internal/config/config_env.go#L78-L82);
do not shorten it through `MCP_GATEWAY_SESSION_TIMEOUT`. This is not an active-deadline
override. The server uses supported Streamable HTTP request/response, not
server-initiated notification delivery.

The pinned gateway uses Linux host networking and rewrites loopback backend URLs
to `host.docker.internal`, mapped to `127.0.0.1`. The endpoint explicitly permits
that Host header without disabling DNS-rebinding checks. Only the trusted gateway
may reach the backend; the agent gets the gateway route, not direct host access to
8765. Authentication, body bounds and strict operation validation also apply to
direct backend requests.

**Explicit mounts are not an allowlist in stock GH AW.** Its
[mount generation](https://github.com/github/gh-aw/blob/58d1bedbb7200f59c2d224151339e38fd8687d05/pkg/workflow/awf_helpers.go)
adds mounts to defaults; home/workspace/temp may be writable, and `/opt` readable.
A controller placed under mounted temp is not protected; chmod on same-user files
does not fix this. Use a disposable public-source export without Git metadata,
no host/Docker socket or credential files, and protected roots disjoint from
**all actual mounts**. Stock AWF mounts host temp; it is not a private agent tmpfs.
The runtime inspects Docker's effective mounts and environment before readiness,
rather than treating the bridge's structural checks as live proof. The gateway is trusted infrastructure;
its Docker socket must never be reachable by agent code.
Export accepts regular Git modes `100644` and `100755`, materializing both with
mode `0644`; it never follows a source symlink or exports a submodule.

Portable tests exercise actual SDK HTTP clients, synthetic diagnostics and real
local bare-Git CAS. They do not establish the actual Linux mount/network boundary,
a compiled 360-minute graph, hosted execution, or a compiler-source speedup. Those
require their separately authorized fixed-revision Linux execution.

### Pinned Actions runtime contract

`runtime.py` is the workflow adapter; the former unprovisioned
`agent_command` capsule has been removed. No separate image, model proxy or
container network is built here. Verified upstream inputs are GH AW **v0.76.1**
(`58d1bedbb7200f59c2d224151339e38fd8687d05`), Copilot **1.0.52**, AWF **v0.25.55**
(`39b8a59ed302636889c7f86bd015a31633b80415`) and MCP gateway **v0.3.19**.
The retained locks use setup action
`github/gh-aw-actions/setup@46d564922b082d0db93244972e8005ea6904ee5f`.
Installed newer CLIs are not substitutes.

The pinned [main-job generator](https://github.com/github/gh-aw/blob/58d1bedbb7200f59c2d224151339e38fd8687d05/pkg/workflow/compiler_yaml_main_job.go)
places top-level `pre-steps` before checkout/install, `pre-agent-steps` after
engine installation but before MCP gateway startup, and `post-steps` after
built-in collection. Use those exact hooks, not `jobs.agent.post-steps` (ignored
for built-in jobs). The pinned [Copilot engine](https://github.com/github/gh-aw/blob/58d1bedbb7200f59c2d224151339e38fd8687d05/pkg/workflow/copilot_engine_execution.go)
supports `engine.harness`, HTTP MCP, credential-steered AWF execution and
step-level `timeout-minutes`; the agent job retains Actions' 360-minute default.
`runtime.py configuration` prints the exact engine/MCP fragments and budgets,
not a compiled workflow. Merge its engine additions with the existing shared
model defaults; do not replace those defaults or hand-edit a lock.

The next graph must disable the default checkout and agent safe-output writers.
Keep controller-only MCP permissions; do not enable wildcard bash, write-capable
GitHub MCP or a separately timed delegated optimizer.
Trusted setup provisions a fixed-revision checkout, Python environment, journal
cache, encryption configuration and publisher handoff **outside** workspace,
home/tool-cache mounts, runner temp, `/tmp`, `/opt` and system mounts. An owned
directory under `/var/lib/compiler-perf` is suitable for stock hosted Linux.
The default workspace must start empty: `setup` exports only bounded regular
public Git blobs into its `source` child. No controller checkout, Git metadata,
key, journal, raw diagnostic, receipt or publisher output is mounted there.
`start-service` rejects changed helper/policy/SDK inputs and wrong installed
Copilot/AWF versions before preparation.

Use the same protected arguments for each entry point; the executable and
inputs named by these variables must come from trusted setup, never issue/PR text:

```sh
python "$PERF_RUNTIME" setup --root "$PERF_ROOT" --repository "$PERF_SOURCE" --journal "$PERF_JOURNAL" --workspace "$GITHUB_WORKSPACE" --input "$PERF_INPUT" --job-started-at "$PERF_JOB_STARTED_AT"
python "$PERF_RUNTIME" start-service --root "$PERF_ROOT" --repository "$PERF_SOURCE" --journal "$PERF_JOURNAL" --workspace "$GITHUB_WORKSPACE"
python "$PERF_RUNTIME" handoff --root "$PERF_ROOT" --repository "$PERF_SOURCE" --journal "$PERF_JOURNAL" --workspace "$GITHUB_WORKSPACE" --destination "$PERF_PUBLISHER_OUTPUT" --engine-outcome "$PERF_ENGINE_OUTCOME"
python "$PERF_RUNTIME" finalize --root "$PERF_ROOT" --repository "$PERF_SOURCE" --journal "$PERF_JOURNAL" --workspace "$GITHUB_WORKSPACE"
```

Run `setup` in `pre-steps`. `PERF_JOB_STARTED_AT` must be the actual enclosing
job-attempt start from trusted Actions metadata, including time already spent
checking out the private helper and setting up tools. It cannot be a fresh
timestamp after installation. Preflight verifies the frozen main SHA, every PR
page and versioned owners before bootstrap; API denial blocks. Stock installs
then run within the same aggregate 3,600-second clock. Run `start-service` as
the `pre-agent-steps` step with **id `controller`**. It prepares frozen inputs,
installs only `compiler_perf_harness.cjs` alongside the unchanged stock harness,
starts the owned service, checks a real authenticated HTTP initialize response,
and emits masked `token` plus public `execution_deadline_ms` step outputs.
Its foreground `serve` child is supervised through sealed stop/exit records;
reentry cannot adopt a saved PID or duplicate a launch.
Version checks accept the pinned Copilot banner's sentence-ending period but
reject longer and prerelease version tokens. Launch, readiness, cancellation
and step-output failures latch an unhealthy runtime and stop only the original
child handle, after releasing the controller lock. Cleanup shares the existing
finalization deadline; an exited child permits an incomplete canonical conclusion,
while unconfirmed cleanup preserves outstanding work and reports `cleanup_failed`.

Configure the printed HTTP server fragment, `engine.harness:
compiler_perf_harness.cjs`, its deadline environment variable,
`engine.mcp.tool-timeout: 5m` (with the gateway's six-hour session default), and
workflow `timeout-minutes: 250` (the engine step). The wrapper launches the
**unchanged stock Copilot harness**, preserving model proxy, arguments and
retry behavior, while bounding the entire harness rather than each retry.
The first authenticated agent tool call triggers actual Docker inspection of
`awf-agent`, its complete mounts/environment/namespaces, the running pinned
`awf-api-proxy` and token-steering configuration. Failed inspection cannot start
active work. Credential inspection accepts only the pinned AWF Copilot dummy
(`ghu_` followed by 36 `a` characters), including `COPILOT_TOKEN`; any real or
different token is rejected. The separate bubblewrap capability probe checks empty credentials,
zero effective capabilities, inaccessible custody/receipt paths and denied
network; denial never falls back to host execution.

Budget accounting is **559 seconds readiness + 14,400 active + 1 second
scheduling margin + 40 seconds shutdown = 15,000 seconds**. Readiness begins
when the service is armed, so gateway/proxy/credential-cleaning overhead consumes
that same slack, not another ten minutes. The wrapper sends owned termination
at the end of the active envelope, permits 20 seconds for its child group,
and reserves 20 seconds for AWF shutdown. A late agent is blocked, not granted
less than 240 minutes. Absolute execution and active deadlines survive reentry;
MCP dispatch timeouts acknowledge pending work without stopping or duplicating
workers. Output authorization runs in the existing trusted worker process,
not a service thread with invalid main-thread signal timers. Once authorization
has begun, its independent finalization deadline applies even if active time
expires during revalidation; it cannot start late or renew either clock.

Run `handoff` in an **always-running `post-steps`** step, with
`PERF_ENGINE_OUTCOME` bound to `steps.agentic_execution.outcome`, not agent
output. It requests owned cancellation, waits against the existing finalization
bound, retains incomplete evidence, and returns the exact encrypted/compact
artifact inventory. A failed/cancelled/skipped engine or interrupted/unhealthy service
blocks publication. The host monitor observes the inspected container's exact
ID through `docker ps --all --no-trunc --filter id=... --format '{{json .}}'`.
When it stops or disappears, a sealed exit boundary conservatively uses the last
host observation of it running, including polling latency and AWF teardown.
Denied/malformed inventory is an explicit unhealthy runtime, not an empty result.
The monitor checkpoints and shuts down without waiting for `post-steps`.
Built-in collection, handoff, artifact transport and
publication share **one** 2,400-second finalization clock; `finalize` cannot
renew it. The sealed boundary can only shorten an existing clock and survives
restart, so stock collection before `handoff` is charged too.
Setup <=60 + execution <=250 + combined finalization <=40 fits 350
minutes, leaving ten minutes of the 360-minute enclosing job for runner/action
teardown, not another agent allowance. Hard termination can prevent the last
write: only the last acknowledged journal is durable.

`handoff` independently reapplies and revalidates sealed output, exports only
`safe-output.json` and `proposal.patch` to the protected publisher destination,
and binds repository/base/run/attempt/candidate/patch/evidence. Its sealed export
intent permits recovery of missing copies but never overwrites changed files.
The publisher must consume **only this inventory**, not the agent-writable
`GH_AW_SAFE_OUTPUTS` path; configure the existing one-draft `am` transport.
`finalize` is read/reconciliation only on the CLI. The Python-only fake-publisher
seam exercises partial branch/PR publication and lost acknowledgments locally.
It also refuses publication after an unhealthy handoff; read-only reconciliation
remains available without granting new publication authority.
The protected artifact uploader calls `Controller.acknowledge_evidence(sink)`;
local encrypted-sink receipts stay `local_fixture`, never remote availability.

Required owner settings are the protected paths above, the actual job start,
step-scoped controller `GH_TOKEN` (canonical branch write plus PR/actions read),
stock engine `COPILOT_GITHUB_TOKEN`, and the prior encrypted-retention
`retention-config.json`/verified age tool. Do not put either token in job-wide
environment, workspace, home or temp files. Publisher credentials belong only
to the trusted publisher boundary, never the agent step. Existing
`GH_AW_MODEL_AGENT_COPILOT` and `GH_AW_REASONING_EFFORT` overrides retain defaults
**gpt-5.6-sol** and **high**.

Native repo-memory is suitable only for optional notes on
`memory/compiler-self-profile`: it clones editable files into `/tmp/gh-aw`,
uploads an agent-produced patch and pushes with merge-on-retry. Its size/glob
limits do not authenticate journal transitions or preserve this controller's
CAS/authority rules. Therefore `memory/compiler-self-profile-state` remains
controller-only; no native memory push may target it.

Portable validation uses local bare Git, disposable age recipients, fake
publication, actual HTTP SDK calls and a real Node wrapper with a fixture stock
harness. Mocked Docker mounts and capability responses are contract tests,
**not** Linux execution, a generated graph, hosted durability or live model
access. Those remain a separate explicit Linux gate.

Runtime adapter validation on 2026-10-20 used Python **3.12.10**, MCP **1.30.0**,
Node **25.8.0** and age **v1.2.1**, with no failures, errors or skips:

| Command | Tests | Exit | Seconds |
| --- | --- | --- | --- |
| `python -m unittest discover -s tests\CompilerPerfWorkflow -p "test_workflow*.py" -v` | 123 | 0 | 3328.406 |
| `python -m unittest discover -s tests\CompilerPerfWorkflow -p test_memory.py -v` | 52 | 0 | 81.822 |
| `python -m unittest discover -s tests\CompilerPerfWorkflow -p test_workflow_runtime.py -v` | 19 | 0 | 190.567 |

The broad batch began before the final runtime-only mount-alias/blocker guards;
the 19-test replay covers those final guards. Three focused mount/layout/setup
selectors also passed after replacing duplicate overlap logic with the existing
bridge helper (exit 0, 2.049 seconds). `node --check` on the wrapper, Python
`py_compile` on changed modules and `git diff --check` exited zero. Initial RED
runs exposed the missing adapter/wrapper and fixture/health-probe errors; exact
commands, failed results and final logs remain in the private session evidence.
No compiler build, hosted run, model call, remote write or Linux proof is implied.

The repaired runtime replay on 2026-10-20 used the same tool versions and passed
with zero failures, errors or skips:

| Command | Tests | Exit | Seconds |
| --- | --- | --- | --- |
| `python -m unittest discover -s tests\CompilerPerfWorkflow -p "test_workflow*.py" -v` | 130 | 0 | 3829.990 |
| `python -m unittest discover -s tests\CompilerPerfWorkflow -p test_memory.py -v` | 52 | 0 | 83.659 |

The workflow suite includes all **26 runtime tests**, including direct startup
command/output checks, frozen SDK/helper/policy denial, the pinned gateway schema,
and automatic authorization through a real HTTP SDK session. Its synthetic
accepted candidate crosses active expiry during revalidation, survives observed
engine shutdown, exports sealed output and reconciles a lost publisher
acknowledgment without a second draft. Engine-exit/restart tests charge earlier
stock collection to the same finalization deadline.
Disabling automatic authorization as a negative control fails specifically at
the required sealed-approval assertion; the control command exited zero.
The schema fixture's validation keywords were compared with the exact pinned
gateway source; that comparison, Python `py_compile`, `node --check` and
`git diff --check` exited zero. Initial RED logs and exact selector commands are
retained in private session evidence. The checksum-verified pinned Windows CLI's
`--version` probe exceeded 30 seconds; hooks/defaults were verified from pinned
source, not a successful CLI or compiled workflow run. Linux containment and live
workflow/model/publication proof remain unclaimed.

The subsequent lifecycle repair passed on the same Windows tool versions:

| Command | Tests | Exit | Seconds |
| --- | --- | --- | --- |
| `python -m unittest discover -s tests\CompilerPerfWorkflow -p "test_workflow*.py" -v` | 133 | 0 | 4336.349 |
| `python -m unittest discover -s tests\CompilerPerfWorkflow -p test_memory.py -v` | 52 | 0 | 91.936 |
| `python -m unittest discover -s tests\CompilerPerfWorkflow -p test_workflow_runtime.py -k start_service_entry -k startup_cleanup -v` | 2 | 0 | 23.398 |

All passed without skips. The workflow suite includes **29 runtime tests**,
with real startup SIGTERM delivery/handler restoration, failed launch/readiness/
output writes, same-owner reentry, bounded cleanup and unacknowledged-worker
preservation. Accepted sealed output cannot publish after a failed/cancelled/
skipped handoff; read-only reconciliation and the one-draft lost-ack path remain
covered. The incomplete HTTP handoff now reuses the existing service fixture.
The checksum-verified Copilot v1.0.52 Windows release's `--version` exited zero
with `GitHub Copilot CLI 1.0.52.` and its update-hint line; near-version and
prerelease tokens remain rejected. Initial RED, corrected fixture failures and
an interrupted earlier replay remain private evidence, not passing results.
Final input hashes, Python `py_compile`, `node --check` and `git diff --check`
were checked. No compiler build, remote write, hosted run or Linux proof is implied.

## Disposable experiment controller

Run `experiment.py` from the **trusted checkout**, never the proposed candidate.
`prepare` consumes a workload from `measurement.prepare_workload` (the existing
`measurement.py prepare` CLI), an exact base SHA and a bounded UTF-8 Git patch.
It does not accept build commands, acceptance decisions or passed-test arrays.

```powershell
python eng\scripts\compiler_perf\experiment.py prepare --repository . --request request.json --patch proposal.patch --manifest frozen-workload.json --owned-root disposable-attempt
python eng\scripts\compiler_perf\experiment.py validate --owned-root disposable-attempt --profile private-profile\evidence-manifest.json
python eng\scripts\compiler_perf\experiment.py discard --owned-root disposable-attempt
```

Request schema (replace the illustrative revision, paths and method):

```json
{
  "schema_version": 1,
  "attempt_id": "checking-1",
  "run_id": "12345",
  "base_sha": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
  "compiler_paths": ["src/Compiler/Checking/Expressions/CheckExpressions.fs"],
  "regressions": [{
    "path": "tests/FSharp.Compiler.ComponentTests/Conformance/Regression.fs",
    "method": "Avoid duplicate checking"
  }],
  "hypothesis": "Avoid the repeated operation identified by the retained profile",
  "primary_metric": "elapsed_ms",
  "complexity": "One local expression change; no additional mechanism"
}
```

An experiment permits at most eight compiler files in one area, eight related
regression selectors and a 1 MiB patch. Git applies the patch in a new independent
clone and the validator checks the actual staged paths, modes and source tree.
Only compiler `.fs`/`.fsi`, related test sources/baselines, narrowly additive
`Compile` registrations and `docs/release-notes/.FSharp.Compiler.Service/11.0.100.md`
are allowed. Binary changes, deletions, renames, links, executable modes, helpers,
policy, harnesses, SDK/build configuration and unrelated areas are rejected.
The named regression must occur in added/changed lines. Preserve the hotspot's
RED semantic/operation-count evidence before optimizing; a semantics-preserving
optimization can instead have explicit failing paired-performance evidence and
unchanged semantic guards. Do not add wall-clock unit-test thresholds.

**Live validation requires Linux and `/usr/bin/bwrap`.** Windows can prepare and
inspect candidates but cannot fall back to unsandboxed candidate execution.
Prepare the frozen workload on the same Linux host, with the pinned SDK and the
compatibility SDK (9.0.300 or later) in the recorded dotnet installation. Provision
the complete Release/test dependency cache during trusted setup, before freezing.
Fresh preparation records all installed SDK files, deployed/bootstrap files and
package-cache identities; older manifests lacking these records block, rather
than acquiring invented provenance. Missing dependencies/SDKs require a new
preparation, not edits to `global.json` or network access by candidate code.

The child boundary has separate mount, PID and network namespaces, no
capabilities, and an allowlisted environment. System tools and frozen inputs are
read-only; only owned artifacts, compatibility fixture outputs, HOME/temp and
the exact replay output files are writable. The replay compiler installation is
also read-only. Host homes, Git credentials, agent tokens, Actions output files,
canonical memory and validator receipts are not mounted. Synthetic account files
support the runner's user lookup without exposing host account records. The parent records
exits, launch times, owned PIDs, command arrays, hashes and results. Owned process
groups/jobs are reaped on failure/cancellation; no process-name killing occurs.
Locks serialize candidate operations and the existing measurement lock serializes
frozen-workload outputs. An existing lock is an unproven owner, never reclaimed.

Validation reuses the recorded Release pack recipe and bootstrap, then checks
explicit Release compiler/component projects with the pack's
`BUILDING_USING_DOTNET=false` and `BuildNoRealsig=true` properties. Correctness uses
an owned copy of the frozen dependency cache; existing package hashes are checked
after each command, and generated compatibility packages never enter the frozen cache.
Changed-area plans require the
new regression selectors, the component siblings, runtime execution and all six
compatibility scenarios. Codegen/optimizer/IL areas add emitted-IL and optimizer
checks; service/typed-tree/API areas add service tests, and `.fsi` changes add
surface-area tests. Unknown/general compiler areas receive conservative checks.
Fresh attestation regenerates the plan from the changed paths; coherently removing
an area-specific check from the plan, its digest, required IDs and receipts still
blocks approval.
The built pinned MTP runner's `--help` must expose the required selectors and
xUnit reporter; tests use explicit `--project`, `--no-build` and
`--minimum-expected-tests 1`. Empty, failing, missing or stale reports block
approval. Required regressions use an exact method suffix (`*.Method`, never
`*Method*`) and `--fail-skips on`; a passing decoy cannot excuse a skipped regression.
They must match the declared method and source file in the XML, and the selected
discovery inventory and outer runner's per-test completions must agree.
Broad suites first run serially in a separate checkout of the frozen base, including
its own Release pack and test builds. Only exact test-name/reason skips observed
there can be excluded from candidate checks; new skips and skipped regressions
block approval. Individual XML results must agree with assembly totals; claimed
totals alone are not evidence. Approval also requires successful observed completion
of the fixed SDK's **outer `dotnet test --project` runner**. On the pinned SDK,
missing startup handshake and missing test-session end fail that outer invocation,
even if the child exits zero and writes matching passing XML. Direct assembly
execution is not an equivalent completion check. Totals, discovery, XML, a success
banner, a signature or a test-body sentinel alone cannot authorize approval.

**Runner-reported consistency:** SDK `11.0.100-rc.1.26420.103`, MTP 2.0.2 and
xUnit 3.2.2 are fixed. Outer `--output Detailed --no-ansi --no-progress` and
`--list-tests json` precede `--`; xUnit `--culture en-US`, selectors and execution
report flags follow it. `DOTNET_CLI_UI_LANGUAGE=en-US` pins outer messages.
Discovery uses the same project/framework/selection but omits reports and
`--minimum-expected-tests` (incompatible with discovery). It supplies an inventory,
not completion. The pinned outer help exposes no JSON execution export; xUnit's
CTRF and other exports are additional in-process reports, not outer-runner captures.

`runner_results.py` accepts one complete module, unique discovered identities,
matching XML leaves and exact per-test completion frames followed by matching
module and overall totals. XML class/method selectors and module/framework identity
are checked, including enumerated parameterized tests. Missing, duplicate,
conflicting, truncated, unknown or ambiguous framing fails closed; child log text
is not scanned for passing names. Broad-category skips must match fresh baseline
runner/XML evidence exactly; regressions cannot skip. Non-enumerable or ambiguous
test identities block approval rather than falling back to totals.
The concrete `decoy-completion` attack still completes with outer/child exit zero,
but its `passing` completion disagrees with the requested/discovered `regression`
and forged XML, so collection and fresh authorization reject it.

Protected parent-side invocation files and bounded stdout/stderr captures (with the
launcher's existing secret redaction) are hashed outside candidate write access.
Fresh authorization rederives the category plan, checks the frozen selection and
SDK probe, re-reads discovery/execution/XML evidence and recomputes the normalized
runner-reported identity/outcome digest. Cached pass arrays or re-signed receipts
cannot excuse inconsistent evidence. The optimizing agent cannot change the
authoritative helpers, command selection, SDK or measurement policy, mint receipts
or acquire publication credentials.

The pinned SDK receives test identities and outcomes from the in-process MTP
engine; its [result handler](https://github.com/dotnet/sdk/blob/6bb52e256cd35f45066ee209812c099f2195122a/src/Cli/dotnet/Commands/Test/MTP/TestApplicationHandler.cs#L296-L361)
checks protocol/session consistency, not execution of the requested method.
The evidence contract is conventional CI: test-host code is trusted to report
assertions honestly to its framework. It is **not cryptographic proof of
test-body execution**, nor proof that arbitrary code sharing the framework process
cannot lie to that framework.

Reports stream through a fresh write-only FIFO in
a sealed directory to a parent-owned evidence file, rather than a candidate-writable
XML file. Late writes append to the stream and invalidate it; they cannot truncate
or replace earlier failures/skips. **The FIFO does not authenticate its first
writer.** The parent binds the captured report, command,
candidate, nonce and exit to a process-local execution receipt, separately from
final acceptance, only after checking the outer invocation and matched results.
The signature prevents later receipt substitution; it cannot replace runner-result
reconciliation or authenticate assertions. Skips never count as matched passes, and categories
with no applicable passing tests block approval. Test
outputs must contain the selected candidate service binary.
The controller drives the existing compatibility projects through six explicit
pack/build/runtime scenarios, pinning each SDK with `rollForward: disable` in the
owned fixture directory. The generated pin preserves non-SDK sections of the frozen
root `global.json`, including `msbuild-sdks`, so versionless Arcade/Helix imports
keep their frozen package versions. It checks both SDK probes and the application's
reported library/application SDK and compiler paths. A success banner or the presence of
.NET 9 in `--list-sdks` cannot substitute for historical compiler execution.
On the pinned SDK, probe help before the `--` separator, and supply report
basenames with a separate `--results-directory`. Selectors, per-application
minimum test count and report flags go after `--`.

Only after correctness and artifact-backed diagnostic analysis does the parent
run the existing unprofiled A/A and interleaved A/B collector on the **same frozen
compiler-source response file**, using `compare_measurements` and unchanged
`POLICY`. Profiling is never part of these timings. Noise remains `inconclusive`;
identical timings are not an improvement. `validate --no-finding` can withdraw
an unmeasured hypothesis after complete diagnostic analysis, but cannot relabel
an already attempted measurement as a healthy no-finding.

`candidate.json`, the original patch, profile references, command logs,
correctness receipts, raw samples and local `attempt.json` retain the hypothesis,
source/binary/patch/validator identities, outcome, reason and next action. An unsuccessful
validation discards only its verified owned `tree`; accepted candidates are
retained. Cancellation returns 130 and normally discards source. Explicit
`--checkpoint-on-cancel` preserves an interrupted candidate only after verifying
its fingerprints. An atomic `checkpoint.json` retains the cancelled attempt;
another `validate` checks its recorded digest and uses `memory.resume_reason`
before collecting fresh correctness/sample receipts. The checkpoint binds the
request/run, owned tree, retained patch, compiler/workload/policy identities and
partial sample inventory. Missing, corrupt, redirected or changed retained inputs
block resume, never initialize a fresh attempt or authorize a draft. The original
checkpoint and patch remain failure evidence. Older digest-only checkpoints require
a new disposable attempt. An interruption after checkpoint replacement but before
its candidate-state binding also requires a new attempt; the unbound checkpoint is
preserved, not trusted as fresh state. This also blocks `validate --no-finding`
with `blocked_setup` (exit code 2), even if no samples were collected.
Both entry points reject redirected checkpoint entries, including dangling links
or junctions. Cancellation before resume authentication completes refuses checkpoint
replacement (`Resume authentication incomplete`), preserves the original checkpoint
and patch, and discards only the owned candidate tree; cancellation still returns 130.

The source checkout may contain staged, unstaged and untracked user work: the
controller clones the requested committed base without resetting or cleaning that
checkout. Discard verifies the disposable tree's identity, not just its parent's;
substituted trees and redirected ancestors are refused. It does not clean other
worktrees, outstanding proposals or canonical memory.

Individual pack/check commands have bounds (60/30 minutes); there
is no overall experiment or investigation timeout. Full correctness input
inventories remain in separately hashed JSON evidence beside the receipts; the
bounded journal contains only their file identities. Fresh validation checks both
the inventory hash and every referenced input, without increasing the journal limits.
Historical reads need not have those artifacts available.

Approval uses a process-local attestation, not a portable JSON permission token.
`acceptance_ready` and `publication_intent` reject forged or reloaded attestations
without fresh trusted validation. Historical journal schema 1 is preserved;
correctness schema 5 is nested within it. Older root-command and schema-2 through schema-4 receipts
remain readable history but cannot authorize new publication. Historical command and regression
paths retain the producer's POSIX or Windows semantics when read on another platform; no
artifact availability or fresh approval is inferred from loading them. `draft_eligible`
records a validation decision, not permission for a later publisher to skip revalidation. Human draft
review must still assess relevance and complexity. The controller only creates
local receipts/commit objects; it never pushes, creates a PR or dispatches work.

The controller fixtures use temporary Git repositories and real isolated
children, including malicious write attempts, RED/GREEN checks, failed checks,
cancellation with a surviving sentinel, checkpoint resume and rollback. Their
100-to-90 samples and toy toolchain are **synthetic**, not a live F# speedup.
The lifecycle matrix also exercises unavailable SDKs, failed packs, late report
forgery and realistic dependency inventories on both acceptance and rejection paths.
For the real runner matrix, set `COMPILER_PERF_RUNNER_OUTPUT` to an existing
owned output parent, then run:

```powershell
.tools\compiler-perf-python\python.exe -m unittest discover -s tests\CompilerPerfWorkflow -p test_experiment.py -k PinnedRunnerTests -v
```

The fixture creates a new retained directory, resolves `.dotnet/dotnet` (or
`dotnet.exe`, with optional `COMPILER_PERF_DOTNET` override), preserves root
`global.json`, checks the SDK/xUnit pins, builds the explicit fixture project in
Release and saves actual runner help/package identities. Restore occurs only
after missing assets are observed, into owned storage. The shared fixture varies
only attack phase: pass, fail, zero matches, skipped required method with a passing
decoy, module-initializer exit, in-test exit and late `ProcessExit` overwrite.
The decoy-completion phase also combines a forged first report with a genuine
passing session for a different method, retaining that child's arguments and
execution sentinel. It is a rejection assertion, not an expected failure or skip.
All production test launches come from `correctness_plan` through `run_correctness`;
fresh validation and memory approval remain active. Only unrelated compiler-build,
compatibility and measurement inputs are synthetic. No live compiler improvement
is claimed. Temporarily bypassing the logical invocation/completion gate must make
the first-report attacks violate the rejection assertion; restoring them must
reject even the resulting signed receipts.

Windows exercises the outer runner and approval checks with ordinary report files,
not production isolation or FIFO capture. Linux uses bubblewrap and the real FIFO;
the Linux boundary gate must explicitly execute this matrix and the full-controller
fixture before overall acceptance. Missing opted-in infrastructure fails, rather
than becoming a passing skip. Direct assembly runs are retained separately as
diagnostic controls and never feed approval. Each run retains argv/cwd, outer and
available child exits, report bytes/hashes, required source/method, body sentinel
and approval/mutation outcomes. Rejection assertions and retained verdicts cover
`run_correctness`, fresh `validate_correctness`, `_attest_correctness` and
`acceptance_ready` independently, even when an earlier gate incorrectly approves.
Sentinels only diagnose which phase executed.

## Process profiling (diagnostic evidence only)

`collection.py` consumes the same version-1 frozen workload/compiler manifests.
It performs one **separate, instrumented** replay, never an acceptance sample.
Use a new output directory under an existing parent:

```powershell
python eng\scripts\compiler_perf\collection.py collect --manifest workload.json --compiler compiler.json --output-dir private-profile
python eng\scripts\compiler_perf\collection.py analyze --input private-profile\evidence-manifest.json --output profile-summary.json
```

The target is the manifest's absolute `dotnet` executable, local Release
`fsc.dll`, and exact hashed response file, running in the frozen compiler-source
directory. Inputs, compiler installation, runtime files and arguments are checked
before and after replay. A shared `measurement.lock` excludes simultaneous
acceptance collection. The subprocess gets an allowlisted environment with
private HOME/temp/cache directories, no inherited tokens, startup hooks or
profiler settings. It starts suspended using
`DOTNET_DefaultDiagnosticPortSuspend=1`. The pinned `dotnet-trace` attaches
without resuming; `dotnet-counters` attaches and resumes the runtime. On Linux,
both use the target's TMPDIR for the diagnostics socket. There is no PID-by-name
selection, `sudo`, container privilege escalation, or cross-namespace fallback.

Process identity includes PID, creation time, full argument array, executable
hash, parent, session, owner and PID namespace. Windows uses CIM plus the owned
process handle, with explicit UTF-8 encoding on both sides of the CIM JSON
exchange so non-ASCII paths retain their exact identity; Linux uses `/proc`
plus `wait4`. The profiler PIDs remain distinct
from the target PID. The end identity is tied to the owned process handle/wait,
not a lookup of a potentially reused PID. Preflight checks tool pins/help,
architecture, memory/disk budget and diagnostics ownership. Actual EventPipe
startup and required trace events must also succeed; a help command alone is
not a compatibility test.

Tools are restored from `.config/dotnet-tools.json` only when missing, never
globally upgraded. The checked v8 redirected-output startup marker is
`Output File`, emitted after `StartEventPipeSession`; its interactive
`Recording trace` status is not emitted when stdout is redirected.
Original `.nettrace`, pinned TraceEvent reductions, System.Runtime CSV,
sanitized command/stdout/stderr records, source hashes, provider keywords,
intervals and completeness status stay together. `analyze` verifies artifact
hashes, reduction identity and exact counter/CSV agreement. Timing evidence
requires successful build/extraction records, a hashed binlog and extractor,
and hashed output containing the recorded project-tagged timing rows.
Requested dumps must retain their verified identity, successful tool records
and hashed private artifact. Artifact hashes are verified independently of
metadata; a recorded `size_bytes` must also match the file's actual byte length.
Neither a status flag nor an exporter exit code alone is evidence.
Persisted command arguments, stdout/stderr and failure reasons redact credential
assignments, including quoted JSON values, escaped quotes and truncated strings.
Raw tool streams are deleted after bounded sanitization.

The summary keeps these quantities separate:

| Field | Interpretation |
| --- | --- |
| `elapsed_ms` | Monotonic launch-to-exit time, including diagnostics startup suspension; not acceptance timing |
| `process_cpu_ms` | Target user+kernel CPU accounting; not stack duration |
| `cpu_sample_weight`, `cpu_sample_unit` | SampleProfiler stack samples, not exclusive CPU milliseconds |
| `allocated_bytes_estimate` | Sampled GCAllocationTick byte estimate, not exact allocation accounting |
| `allocation_counter_observed_bytes` | Sum of observed System.Runtime allocation increments, including the first; not exact whole-process allocation |
| `gc_pause_ms`, `gc_counts` | Union of stop-the-world intervals; collection counts by reported generation |
| `managed_heap_bytes` | Last System.Runtime heap gauge observation; exporter MB means decimal 1,000,000 bytes |
| `peak_rss_bytes` | OS peak working set/RSS; Linux KiB is converted to bytes |
| `retained_heap_bytes` | Null with a reason unless live/post-GC retention was actually measured |

The pinned `dotnet-counters` 8.0.547301 EventCounters exporter labels allocation
rows `Rate` / `B / 1 sec`, but exports the payload's `Increment` unchanged
([payload conversion](https://github.com/dotnet/diagnostics/blob/358ff9556edceba1e8689fbfd1fc67588d8ab6cb/src/Microsoft.Diagnostics.Monitoring.EventPipe/Counters/TraceEventExtensions.cs),
[CSV exporter](https://github.com/dotnet/diagnostics/blob/358ff9556edceba1e8689fbfd1fc67588d8ab6cb/src/Tools/dotnet-counters/Exporters/CSVExporter.cs)).
Each increment is counted once, including zero values and distinct rows with
equal rounded timestamps; timestamps only establish ordering and coverage,
not allocation weights. This observed-window subtotal can miss startup/tail
allocation and need not match the sampled trace estimate.
Old summaries containing `allocation_rate_integral_estimate_bytes` used the
incorrect timestamp-weighted reducer, identified by their capture manifest's
collector hash. They are not corrected results; old `rate`-semantics manifests
are rejected, not silently relabeled.

Cumulative counters are rejected. Stack weights are inclusive sampled stacks.
Counter timestamps are exporter wall-clock observations, not process elapsed
time. Tool globalization is invariant; the adapter also recognizes the pinned
Windows en-US timestamp spelling. Counter coverage is checked against the
observed trace interval, allowing one counter interval at each edge, rather
than charging diagnostic-tool startup time to the target. Every observation
must lie within its time domain (CSV timestamps allow one second of exporter
rounding); trace allocation buckets may begin before the first event only
within that event's 100 ms bucket. Numeric inputs and derived sums
and unit conversions must remain finite. Allocation events
are summed into bounded 100 ms buckets with the original event count retained;
the bucket reduction does not change the allocation-byte sum.
Unsupported schemas, timestamps, providers,
units, runtime/PID mismatches, absent mandatory evidence, incomplete intervals,
dropped events, tool failures and inadequate coverage return
`blocked_collection` with a nonzero CLI exit.
The trace reducer rejects matching full input/output paths before PID validation
or reduction. This error is reported only on stderr, without writing failure
JSON over the original trace; failures with a separate output still emit JSON.

After replay, a separate diagnostic Release rebuild with `--times` produces a
binlog. The frozen local-compiler props append `--times`; the collector does not
replace `OtherFlags` with a global MSBuild property, which would suppress project
flags such as `--realsig+` and `--extraoptimizationloops:1`.
The existing `ExtractTimingsFromBinlog.fsx` output must contain a real,
project-tagged Fsc numeric timing row. Its log/hash and binlog are separate from
unprofiled measurements; “No Fsc tasks” with exit zero is a blocker.

### Conditional private dumps

Record a specific retention, growth or unexplained-RSS hypothesis, then use a
dedicated profiling replay:

```powershell
python eng\scripts\compiler_perf\collection.py collect --manifest workload.json --compiler compiler.json --output-dir private-retention --dump-reason heap-growth --hypothesis "Managed heap continues growing across collections"
python eng\scripts\compiler_perf\collection.py dump --process private-retention\verified-process.json --reason heap-growth --output-dir another-private-dump
```

The standalone `dump` command is for an **already running** verified replay;
it fails if the process has exited. Its input records `identity`,
`profiling_replay: true`, `hypothesis`, and the target diagnostics environment.
Collection rechecks full identity immediately before attachment and after
capture. `Heap` is the least supported dump type containing the managed heap;
Mini/Triage cannot answer these hypotheses. The dump must also load through
`dotnet-dump analyze` and produce actual heap statistics. A heap snapshot alone
does not establish live retained bytes. Early exit, permissions/ptrace,
timeouts, empty/corrupt dumps and tool failures retain phase/exit/stderr
evidence and block the requested collection.

**Raw dumps are private local evidence, never standard public-repository Actions
artifacts.** Output directories are owner-only (POSIX mode or a protected
Windows DACL granting only the current user access).
Collection manifests record `published: false`. The controller analyzes the
exact owned dump, encrypts it for the configured recipient, then unlinks only
the identity-checked plaintext file. Windows keeps its write/delete-denying handle
through final byte/identity validation and marks that same object for deletion.
POSIX isolates the pathname in a private cleanup directory and verifies the moved
object before unlinking; a replacement is preserved and blocks retention.
The ciphertext is pending until a trusted
sink acknowledges it. Failed retention/deletion is explicit; nothing falls back
to uploading plaintext. Removal is a filesystem unlink, not a claim of forensic
erasure. Public summaries still require human review for sensitive content.

Limits are fixed: one replay per invocation, 180 seconds for the target,
512 MiB per trace, 1 GiB per other artifact, 2 GiB total, 1 MiB per tool stream,
and 16 MiB per JSON reduction. Dump capture/validation each allow 120 seconds;
the diagnostic rebuild allows 600 seconds. Only owned children are terminated.
Windows children start suspended, join a private kill-on-close job, then resume;
job assignment failure blocks launch without executing the child. Jobs and POSIX
process groups are terminated even after their wrapper exits, before log cleanup.
An exited wrapper does not hide a descendant-termination failure.
Cancellation terminates owned children and persists blocked command/collection
evidence before propagating; it never starts subsequent diagnostic tools.
The CLI returns exit code 130 for cancellation.
Target duration and artifact budgets remain monitored during a requested dump,
including its tool probes and validation. Persisted manifests and summaries
count toward the disk budget.
Owned incomplete binaries are hashed and recorded before removal; completed
traces remain available even if another requested collection fails.
No binary trace is loaded into Python memory.

Portable fixtures use synthetic numbers, not performance measurements. The
separately opt-in `COMPILER_PERF_LIVE=1` trace-adapter tests use a tiny public
.NET child to exercise the real diagnostics tools/reducer and negative controls;
that smoke child is **not** compiler self-compilation. Investigation conclusions
should cite the frozen-source trial, record tested hypotheses and sampled hot
paths, and distinguish an infrastructure blocker from a negative finding.

### Process-profiling validation record

On 2026-09-19, collection fixtures first failed because `collection.py` did not
exist (`artifacts/compiler-profile-validation/red.log`). A subsequent RED
coverage fixture caught the distinction between tool startup and runtime
coverage. The final full helper run passes **all 53 tests** with
`COMPILER_PERF_LIVE=1` (`suite-with-live-final.log`), including a real public .NET child trace, negative
trace controls and a successfully captured/analyzed private .NET 8 Heap dump.
The pinned dump analyzer cannot interpret .NET 11's debugging interface
version 10; that requested dump analysis is an explicit compatibility blocker,
not evidence of a corrupt managed heap or a successful retention measurement.

The actual compiler-source trial uses the retained Release build and frozen
revision `cdb9dc5e52e74e5c260e0489e81c0dfef60a3ad9`: **407 tracked sources,
21 generated inputs, 178 references**. Version-1 provenance was reconstructed
from the retained successful build/capture records and all 935 materialized
input/runtime/compiler file records were reverified. Commands and provenance
are under `artifacts/compiler-profile-validation`; `prepare-retained.py`
records that migration rather than inventing a new bootstrap build.

The final successful collection is `compiler-verified/evidence-manifest.json`.
Verified target **PID 39920**, runtime VM `11.0.26.42103`, exited zero, as did
trace, counters and the real TraceEvent reducer. The retained 279,668,746-byte
trace has SHA-256
`90870c8b30f9de5710b648be8d1b558d142b5fcc477318e2a5fbc3398a0bf80a`
and zero lost events. The independent `analyze` invocation reproduces the
collected metrics:

| Evidence | Recorded value |
| --- | --- |
| Monotonic process duration | 99,101.538 ms, profiled and including startup suspension |
| Target process CPU | 152,437.5 ms |
| SampleProfiler stack samples | 288,049 |
| Allocation-tick estimate | 33,975,719,240 bytes from 314,455 events |
| GC pause union | 4,490.073 ms |
| GC counts, generations 0 / 1 / 2 | 62 / 37 / 13 |
| Last managed heap gauge | 2,223,804,992 bytes |
| Peak process RSS | 3,093,159,936 bytes |

The trace resolves named F# compiler stacks, including parsing and checking;
sampled waiting-thread stacks are not exclusive CPU hot spots. This trial
establishes collection fidelity, **not a performance improvement or retention
claim**. Original failed trials remain marked blocked, including the
redirected-output handshake and bounded allocation-row regressions.

The separate diagnostic Release rebuild succeeded with zero warnings/errors;
the existing binlog extractor produced numeric, project-tagged Fsc phase rows.
`dotnet fantomas .` reported zero errors and made no unrelated changes.
The literal root `dotnet build -c Release` and `dotnet test -c Release` were
also attempted: both require an explicit solution because this repository has
multiple solutions. The explicit frozen compiler project Release build and
the helper/live diagnostics suites above are the successful scoped validation.
No source-compiler optimization, cloud dispatch, artifact upload or publication
was performed.

The fail-closed corrections were validated on 2026-09-19. Regression fixtures
first failed for counter/file disagreement, missing or invalid timing/dump
evidence, numeric overflow, out-of-window observations, missing trace completion,
collection-count and aggregate/metadata budgets. The Windows permission assertion
also exposed explicit grants left behind by inheritance-only ACL removal.
The final helper run passes **all 62 tests**, including both real diagnostic
smoke tests (`revision-suite-final.log`). Artifact-backed analysis and a complete
mocked collection lifecycle are explicitly synthetic unit fixtures, not live
performance evidence. Four isolated guard-removal mutations (artifact validation,
collection count, aggregate budget and owner-only permissions) were all rejected
by those tests (`revision-mutations.log`).

A fresh frozen-source replay, `revision-compiler/evidence-manifest.json`, verified
target PID **35716**, creation time **639253760992038550**, and the exact frozen
dotnet/compiler/response-file command. The target, trace, counters, reducer,
diagnostic Release build and existing binlog extractor all exited zero.
The trace has zero lost events and SHA-256
`ef3299afb3b7a6145a0b9536fd66d30a7816476ae5916a5cc1fa73af6e7add2e`.
It contains 313,583 stack samples, an allocation-tick estimate of
33,964,057,056 bytes and a 4,740.2302 ms GC pause union. Independent analysis
with the corrected collector succeeded (`revision-reanalysis.json`).
This remains diagnostic evidence, not an acceptance sample or improvement claim.

Validation commands used the retained Python executable under
`artifacts\compiler-perf-validation\python` because the host's `python` alias
points to the Microsoft Store:

```powershell
$env:COMPILER_PERF_LIVE = '1'
artifacts\compiler-perf-validation\python\python.exe -m unittest discover -s tests\CompilerPerfWorkflow -p "test_*.py" -v
artifacts\compiler-perf-validation\python\python.exe eng\scripts\compiler_perf\collection.py collect --manifest artifacts\compiler-profile-validation\workload.json --compiler artifacts\compiler-profile-validation\compiler.json --output-dir artifacts\compiler-profile-validation\revision-compiler
```

The literal root build/test chain still requires a solution selection. Explicit
Release builds of the compiler and required test fixtures passed, followed by
`dotnet test --project tests\FSharp.Compiler.Service.Tests\FSharp.Compiler.Service.Tests.fsproj -c Release --filter-class "*SurfaceAreaTest*"`
(1 passed, `revision-dotnet-test.log`). Missing build-task restore assets and
test fixture binaries were restored/built before those successful runs.
All revision logs and raw evidence remain under the same local validation
directory, with owner-only collection subdirectories; nothing was pushed,
dispatched or published.

Dump integration and Windows exit-race corrections were also validated on
2026-09-19. `iteration3-red.log` records three failures rejecting valid
`collect_dump` artifacts and the deterministic termination-race error.
The dump bytes and hash were unchanged: the extra size metadata caused an
exact-record comparison to fail. The correction verifies path/hash and size
independently. Cleanup tolerates a concurrent child exit, not a still-live
child's termination error. The parameterized lifecycle now covers requested
dump success, failure and early exit. Boundary tests use materialized frozen
inputs and a local Git index, and exercise the diagnostic build/extractor
instead of replacing those boundaries with successful status flags.

All **65 helper tests** pass with live diagnostics enabled
(`iteration3-suite-final.log`), including the real public-child trace and
private .NET 8 dump smoke tests. Five isolated mutations removing frozen-input
validation, binlog collection, requested dumps, size verification or live-child
termination-error propagation were caught (`iteration3-mutations.log`).
Explicit Release compiler and test-project builds and the surface-area smoke
test pass. Root build/test commands still require project selection; the test
project was built with `--disable-build-servers -p:BuildProjectReferences=false`
after dependency-output locks interrupted its first build. Validation logs
retain that failure and the successful build/test commands' results.

The fresh frozen-source trial is `iteration3-compiler/evidence-manifest.json`,
using the same `collect` command above with that new output directory.
Target PID **17912**, creation time **639253787882168440**, and exact frozen
command/binary identity were verified. The target, trace, counters, real
reducer, diagnostic Release build and binlog extractor succeeded.
The 279,440,926-byte trace has zero lost events and SHA-256
`afdcc4c201902eb573238632cede4bcac4299c16a68aa7367a9be38cbebdec91`.
Independent analysis (`iteration3-reanalysis.json`) reproduces 289,156 stack
samples, a 33,936,879,776-byte allocation estimate and 6,302.7316 ms GC pause
union. This is diagnostic evidence, not an acceptance sample, optimization
claim or retention measurement. All evidence remains local and private.

Trace-input preservation was validated on 2026-09-19. The three collision
regressions first reproduced overwritten trace bytes (`iteration4-red.log`),
then passed with unchanged bytes and SHA-256 (`iteration4-green.log`).
The two analyze-CLI failure scenarios now share one parameterized test.
All **65 helper tests** pass with live diagnostics enabled
(`iteration4-suite-live.log`); explicit Release compiler/test-project builds,
the surface-area smoke test and `dotnet fantomas .` also pass. Root build/test
commands still require project selection.

The fresh `iteration4-compiler/evidence-manifest.json` records verified compiler
PID **13812**, zero lost events and successful diagnostic build/binlog extraction.
Independent analysis succeeds (`iteration4-reanalysis.json`). All 935 frozen
file records were reverified, and attempting same-path reduction of the actual
275,831,334-byte trace exited nonzero without changing its SHA-256,
`3fddefce02f394b1fb945e27a7213b3f479504d8cd73f5316e2005b746ef53fe`
(`iteration4-verification.json`). These are local diagnostic results, not
acceptance samples.

Descendant ownership and credential redaction were validated on 2026-09-19.
`iteration5-red.log` reproduced seven quoted-value leaks and four surviving
descendant cases after successful/failed wrapper exits, with both inherited and
detached streams. The corrected full suite passes **68 tests**, including live
public-child trace/dump smoke tests (`iteration5-suite-live.log`). Three isolated
mutations removing redaction, exited-wrapper cleanup or startup suspension are
all caught (`iteration5-mutations.log`). Explicit Release compiler/test builds,
the surface-area smoke test and formatting pass; root build/test commands still
require project selection.

The fresh `iteration5-compiler/evidence-manifest.json` verifies compiler PID
**50588**, start time **639253839402767020**, exact frozen command/binary identity,
zero lost events and successful diagnostic build/binlog extraction. Independent
analysis and all **935** frozen file checks pass (`iteration5-verification.json`).
Its trace SHA-256 is
`0277b80fceed3778dd2b91af96f794e4da2e59a4cabacecf85d17975b774bd25`;
the reduction records 286,562 stack samples, 33,939,106,936 estimated allocated
bytes and a 5,066.8541 ms GC pause union. Commands and DoD evidence are recorded
in `iteration5-validation.json`. These remain private local diagnostics, not
acceptance samples or a retention/improvement claim.

On 2026-09-19, Unicode process-identity, evaluated compiler-flag and cancellation
regressions reproduced six failures and one error (`iteration6-red.log`).
All **71 helper tests** now pass with live diagnostics enabled
(`iteration6-suite-live-final.log`); three isolated guard-removal mutations are
caught. Explicit Release compiler/test builds, the service smoke test and
formatting pass. Root build/test commands still require project selection.
The fresh `iteration6-compiler/evidence-manifest.json` verifies PID **28868**,
start time **639253864805637210**, zero lost events and successful Fsc timing
extraction. Independent analysis verifies all **935** frozen files; the
diagnostic build's **688 arguments** exactly match the frozen replay after
removing `--times` (`iteration6-verification.json`). Its trace SHA-256 is
`c5486c1c38f567cd9e753d892185b026d6a62c391d34fa5aa71c34fc560bd3fa`.
Commands and DoD evidence are in `iteration6-validation.json`; evidence remains
private, local and excluded from acceptance samples.

## Preparation

Fetch `origin/main` locally first. Pass a full SHA from that branch's history.
The SDK version comes from the frozen `global.json`; the product target framework
comes from MSBuild evaluation of `eng/TargetFrameworks.props`.

```sh
python eng/scripts/compiler_perf/measurement.py prepare \
  --source-sha <full-sha> --work-dir <new-owned-directory> --output <manifest.json>
```

The working directory must not exist. Two detached local clones share only Git
objects with the invoking repository: a Release bootstrap/compiler build and an
unchanged source workload. Keep the original object store available throughout
the experiment. There is no push, dispatch, publication, shared-cache eviction,
or removal of `global.json`.

Preparation uses the repository Release pack build, an owned `NUGET_PACKAGES`,
and the existing `UseLocalCompiler.Directory.Build.props/targets` selection
mechanism. It sets these properties directly rather than converting an argument
array into the local helper's shell-command string. Like that helper, it evicts
the selected Core version only from its owned package cache before using the
local pack. Restore and generated-source
creation happen before sampling. `Capture.targets` imports the existing local
Core override and captures evaluated `Fsc.CommandLineArgs` after a real
`CoreCompile`, with a binlog and explicit compiler/project identity.
The compiler command prefix is MSBuild-escaped with literal quotes, matching the
Fsc task's command-line convention even when the installation path contains spaces.

Reference assembly generation is disabled for this workload: the current Fsc
capture API omits the trailing `--refout`/`--refonly` arguments. Sources, references
and flags otherwise retain their captured order. Preparation preserves the raw
`compiler.rsp` and writes a separate `replay.rsp`, removing the bare `--times`
injected by the local-compiler props. The capture build may be instrumented;
smoke replay and acceptance collection are not. Diagnostic instrumentation
(including statistics, typed-tree output, progress, and all `--test:` switches)
fails validation, even if a sample claims `profiler_attached: false`.
Acceptance requires the canonical captured `--optimize+` and parallel switch;
legacy optimization-disabling aliases and pass-disabling overrides are rejected.
Option checks cover dash/slash spellings without interpreting source paths or
reference/resource values as switches. The response file is one
unquoted argument per line, as in Fsc's own response-file generation; it is not
shell-tokenized. Generated sources, resources, signing and SourceLink inputs are
materialized and hashed. Unknown additional compiler tools and nested response
files fail closed. Output files retain their captured locations.

Preparation finishes with a fresh-process smoke replay using the selected local
Release compiler. Windows supports this smoke replay, but only Linux supports
acceptance collection. Per-process CPU and RSS on other systems are `null` with
explicit unavailability reasons.

## Compiler identity and collection

`prepare` writes `baseline-compiler.json` in the owned directory. A later
candidate builder must use the **same bootstrap and toolchain**, without
rebuilding or editing the workload. Create its compiler manifest using:

```python
import measurement

workload = measurement.read_json("manifest.json")
identity = measurement.compiler_manifest(
    candidate_installation,
    candidate_source_revision,  # full Git SHA
    patch_sha256,              # SHA-256 of the exact patch bytes
    workload["invariants"]["bootstrap"],
    workload["invariants"]["toolchain"],
)
measurement.write_json("candidate-compiler.json", identity)
```

The installation includes fsc, FCS, runtimeconfig, deps, Core, and all adjacent
files. Every relative manifest entry must name that file under the installation
containing `fsc.dll`; companion hashes from another installation are rejected.
The builder is responsible for supplying truthful source/patch/build
provenance; a manifest is not an attestation or a correctness gate. The runtime
configuration must match the baseline. Candidate compiler/Core binaries can
differ, but the **Core reference used to compile the frozen workload cannot**.

```sh
python eng/scripts/compiler_perf/measurement.py measure \
  --manifest <manifest.json> --compiler <candidate-compiler.json> --output <samples.json>
python eng/scripts/compiler_perf/measurement.py compare \
  --baseline <samples.baseline.json> --candidate <samples.json> --output <comparison.json>
```

One `measure` invocation produces both arms, with a fresh collection ID. It runs
two unmeasured warm-ups per installation, five A/A pairs using the baseline
installation in both arms, then five baseline/candidate pairs. Arm order within
each pair is shuffled with seed 910. Each sample is a fresh process; no restore
or MSBuild runs occur between samples. Only explicitly owned output files are
reset. Inputs and binaries are verified before and after collection. The executing
measurement module's path/hash and Python version must also match the recorded
toolchain at both checks; verifying an alternate collector file is insufficient.
Offline comparison does not require the collector's Python installation.
Failed or
cancelled collection retains incomplete evidence and cannot qualify. Both arm
files must have `collection_complete: true`, set only after final verification;
a zero exit without an output assembly is also a failed sample.

Each launched command has an isolated process group. Interrupted waits terminate
the owned POSIX group before reaping the wrapper and propagating the exception.
Windows commands start suspended, join the same private kill-on-close job used
by diagnostic commands, then resume. Job cleanup also runs after a wrapper exits,
so cancellation never depends on rediscovering descendants from a live wrapper
PID. Assignment failures prevent execution. Cleanup failures surface explicitly;
unrelated processes are never targeted.

Dependency/filesystem caches are warmed, not flushed. Build-server reuse and
.NET diagnostic attachment are disabled. Collection uses Linux `wait4` on the
specific compiler child, not cumulative child-process counters or the Python
process's RSS. CPU is user plus system milliseconds; Linux peak RSS is converted
from KiB to bytes. This collector adds no managed profiler.

## Policy and contracts

All documents use `schema_version: 1` and a `kind` discriminator. Unsupported
versions, duplicate JSON keys, nonfinite/negative metrics, malformed input and
missing metrics without reasons fail closed. Hashes are SHA-256; JSON
fingerprints sort object keys but never arrays. Raw sample IDs are preserved.
Workload and sample documents share workload validation: complete ordered input
inventories, compiler-source target/paths, Core identity, output ownership and
Release/parallel arguments are mandatory even when both arms agree on invalid
evidence. Comparison also checks each compiler's actual recorded runtimeconfig
hash against the shared toolchain, independently of declared fingerprints.
Matching fingerprints do not excuse missing provenance: bootstrap commands and
binaries, SDK/runtime/helper identities, host/runner metadata, controlled
environment variables and experiment declarations are validated in both arms.
Unavailable host APIs and runner fields are explicitly `null`, with named
`environment.unavailable_reasons`; missing fields or empty placeholder identities
are not accepted.

The workload manifest separates six invariant sections:

| Section | Identity |
| --- | --- |
| `workload` | Frozen revision, target/TFM, Release/parallel flags, ordered source/generated/reference/resource hashes, Core, arguments/response, working directory and owned outputs |
| `bootstrap` | Source tree/revision, bootstrap binary hashes, actual commands and exit statuses |
| `toolchain` | Resolved dotnet binary, SDK/runtime identities and runtime files, compiler runtime configuration, Git/Python/helper versions and hashes |
| `environment` | OS/architecture, CPU topology/affinity, hostname, Actions runner/image/run/job IDs, allowlisted variables, cache policy |
| `policy` | Checked-in thresholds, counts, seed and bootstrap method parameters |
| `experiment` | Unique experiment ID, predeclared primary metric and collector |

Compiler-under-test manifests and fingerprints are separate from these
invariants. Samples also include collection ID, arm, pair/ordinal, warm-up or
measured role, compiler/invariant fingerprints, profiler status, elapsed/CPU/RSS,
exit status, and completeness.

The default primary metric is `elapsed_ms`. Policy is not CLI-configurable:
five complete pairs, median **paired relative improvement** at least 3%, and
the lower endpoint of a paired 95% bootstrap interval strictly above zero.
The bootstrap samples the five paired relative effects with replacement 10,000
times, seed 910, using the median statistic and conservative nearest-rank
2.5%/97.5% endpoints. Policy arithmetic uses exact rational values derived from
the recorded decimal numbers, without epsilon tolerances. Summaries are converted
to JSON numbers only after the decision. No outliers are removed.

A/A absolute difference of arm medians must be at most 1% of the first arm's
median. Each A/A and A/B timing series must have relative median absolute
deviation at most 2%. Excess noise, changed invariants, mismatched collection IDs
or incomplete schedules are `inconclusive`; complete evidence below the effect
or uncertainty threshold is `rejected`. Profiled samples are `rejected`.

For a memory-retention hypothesis, declare
`--primary-metric process_peak_rss_bytes` **during preparation**. The same
3%/interval criterion and 1% A/A control limit apply to that exact metric, and the upper paired 95%
elapsed-time regression bound must be **strictly below 1%**. CPU and RSS evidence
remain independent of timing acceptance; the improving column is never chosen
after measurement. The supported retention collector is the same uninstrumented
OS collector in both arms. Instrumented allocation/retention collectors are not
accepted by this version; in particular, profiled memory runs cannot be
laundered into unprofiled timing evidence.

`--parallel` creates a separate parallel-on cell; the default is parallel-off.
Debug is not an acceptance configuration. Comparison output includes the
decision, primary metric, raw IDs, medians, relative MAD, effect and interval,
independent metric evidence, invariant mismatches and reasons.

Run the portable helper fixtures (all fixture timings are synthetic):

```sh
python -m unittest discover -s tests/CompilerPerfWorkflow -p "test_*.py" -v
```

Controlled preparation fixtures exercise separate checkouts, Release packing,
SDK selection, local Core/capture, path quoting, replay and failure handling.
Replay fixtures launch distinguishable baseline/candidate child processes and
check the selected installation, response file, working directory and output
reset for every A/A, warm-up and A/B event.
Independent schedule assertions require adjacent baseline/candidate pairs;
CLI dispatch tests cover preparation/collection arguments, outputs and failures.
Nonuniform timing fixtures distinguish the memory hypothesis's uncertainty
guard from its median, and boundary fixtures include adjacent representable
values at multiple scales.
Portable collector fixtures check per-child user plus system CPU accounting,
millisecond/byte conversions, exit/signal status, cancellation cleanup and explicit
unavailable-metric reasons without presenting synthetic readings as live evidence.
Real wrapper/child/grandchild fixtures interrupt collection only after both
descendants are connected, require their connections to close, and verify that
an unrelated process remains responsive, with unchanged sentinel and
launcher/executor PID and creation-time identities. They cover cancellation,
wait errors and already-exited wrappers; portable unit fixtures also check
POSIX group-exit races and Windows suspended job assignment.
Valid workload-pair fixtures refresh all fingerprints and require an explicit
workload mismatch for changed revisions, flags, ordered inputs, hashes and paths;
a changed compiler alone remains admissible. Capture guards have SDK-independent
structural assertions plus isolated MSBuild fixtures for Release/execution,
argument capture, reference-assembly exclusions and target scope. The latter
explicitly skip if the pinned SDK is unavailable; they do not compile F# sources.

## Local validation record

On 2026-09-18, the initial helper fixtures failed with `FileNotFoundError` for
the not-yet-created `measurement.py` (RED), then passed with the implementation.
The original assertions remain in the suite.

Windows validation provisioned Python 3.13.7 and the pinned
`11.0.100-rc.1.26420.103` SDK using their supported installers. Upstream `main`
was rechecked and the workload frozen at
`cdb9dc5e52e74e5c260e0489e81c0dfef60a3ad9`. The disposable repository
`build.cmd -c Release -pack -noVisualStudio` succeeded. After correcting
Shipping-package selection, capture and a direct local-Release-compiler replay
passed against the separate unchanged checkout: **407 source files, 21 generated
inputs, 178 references**, targeting the evaluated `net11.0`.

Commands, exit statuses, the workload binlog and hashed smoke evidence were
retained under the owned `artifacts/compiler-perf-smoke` directory
(`validation-commands.json`, `workload.binlog`, `smoke-replay.json`), not Git.
This was a smoke replay, not a performance comparison. Linux `wait4` acceptance
collection is explicitly unsupported on this Windows host; no real A/B
improvement is claimed. The literal repository-root
`dotnet build -c Release && dotnet test -c Release` stops at MSB1011 because the
root contains multiple solutions; the explicit repository Release build and
the Python helper suite are the applicable validation for these changes.

The verifier regressions were reproduced before corrective implementation in
`artifacts/compiler-perf-validation/red.log`. At that point, all 20 helper tests passed,
including actual comparison CLI failures and controlled preparation success and
failure cases. In-memory mutations bypassing preparation or selecting only the
baseline compiler are detected by the new assertions.

A second real Release workload build, using the same pack copied to a compiler
installation path containing spaces, completed with zero warnings/errors.
Capture and fresh-process replay succeeded with the same ordered input hashes
and counts above; the captured workload also passes the shared contract
validator. All 104 copied compiler files match the local Release pack.
The commands, exit statuses, binlog and hashes are retained in
`artifacts/compiler-perf-validation/live-replay.json` and `workload.binlog`.
The standalone root `dotnet test -c Release` also requires an explicit solution;
no full compiler suite was run for these Python-only changes.

The remaining provenance, instrumentation and arithmetic regressions were
recorded before implementation in `red-identity.log`. All **26** helper tests
pass (`green-identity.log`); eight in-memory mutations covering the reported
validation and test gaps are detected (`mutations-identity.log`). Current
`origin/main` was rechecked at the same frozen revision above.

A fresh explicit Release workload build and **uninstrumented** compiler-source
replay passed with unchanged source/generated/reference hashes and the same
407/21/178 input counts. Raw capture contains the inherited `--times`; the replay
response does not. Commands, exit statuses, compiler/input hashes, environment
and binlog are retained in `artifacts/compiler-perf-validation/identity-live`.
This Windows smoke result is not an A/A or A/B acceptance measurement. The
repository-root build/test commands still require an explicit solution; the
explicit Release build succeeded with zero warnings/errors.

Additional diagnostic/optimization-alias and executing-toolchain regressions
failed before implementation (`red-gates.log`: 122 subtest failures). All **29**
helper tests now pass (`green-gates.log`), and all 15 targeted in-memory mutations
are detected (`mutations-gates.log`), including negative CPU/RSS values, incorrect
units/status, and bypassed collector/Python checks. Linux-style absolute source
paths remain valid even when a directory name resembles a diagnostic option.
The real Release build and frozen-source replay still pass with the same
407/21/178 input counts; this remains Windows smoke evidence, not an acceptance
comparison. Logs and hashed evidence are under `artifacts/compiler-perf-validation`.
`gates-final-live/final-replay.json` records the final helper's identity and replay,
and links the successful Release build/capture evidence and binlog.

The final coverage check reproduced four surviving mutations against the original
29-test suite: accepting different workloads and removing each capture guard
(`surviving-coverage-mutations.log`). New assertions detect all four
(`red-coverage-mutations.log`, `mutations-coverage.log`); all **31** tests pass
(`green-coverage.log`), including 27 valid workload-pair cases and nine evaluated
capture cases. No production behavior changed. The explicit Release build and
frozen-source replay passed again with 407/21/178 inputs and 738 unchanged file
records (`coverage-live/evidence.json`). Current upstream `main` still matches
the frozen revision. Root build/test commands still require an explicit solution,
and Linux acceptance collection remains unsupported on this Windows host.

The descendant-cancellation regression failed before the launcher fix
(`red-cancellation.log`): the wrapper was reaped but its child and grandchild
remained connected. Isolated process-group/tree cleanup now passes all **32**
helper tests (`green-cancellation.log`). Both wrapper-only cleanup and omission
of Windows `/T` are detected by the real-process assertions
(`mutations-cancellation.log`). The explicit Release build and uninstrumented
frozen-source replay passed with the same 407/21/178 inputs; 738 file records
were verified, including the updated helper (`cancellation-live/evidence.json`).
Upstream `main` still matches the frozen revision. Bare root build/test commands
still require an explicit solution; this Windows replay is not Linux acceptance
timing evidence. No F# sources or shipped build targets changed.
