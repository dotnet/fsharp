# Compiler-source measurements

`measurement.py` is a Python 3.11+ standard-library CLI and importable module.
It prepares and replays **FSharp.Compiler.Service compiler sources**, not the
FSharp.Core benchmark or generated-program runtime benchmarks. A decision is
measurement evidence, **not permission to publish** an optimization.

Historical validation records below describe earlier local runs. Their ignored
interpreters, compiler binaries, logs and traces were not recovered with the
Git checkpoint and are **unavailable**, not present-day evidence. Recollect
missing artifacts; verify any recovered file's existence and hash before reuse.
New captures against current main require new workload fingerprints.

`memory.py` preserves versioned attempts, negative decisions, retry dependencies
and outstanding publication intents in the local Git journal. Its receipts
explicitly report `remote_acknowledged: false`; local CAS is not remote
durability. The current `CORRECTNESS_COMMAND` is the ambiguous root
`dotnet test -c Release`, not a repository-supported product test receipt.
Passing memory fixtures does not certify production acceptance or publication.

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
Manifests record `published: false`; no encryption/publishing infrastructure is
implied. Public summaries still require human review for paths or sensitive
content.

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
