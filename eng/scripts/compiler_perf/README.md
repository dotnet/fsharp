# Compiler-source measurements

`measurement.py` is a Python 3.11+ standard-library CLI and importable module.
It prepares and replays **FSharp.Compiler.Service compiler sources**, not the
FSharp.Core benchmark or generated-program runtime benchmarks. A decision is
measurement evidence, **not permission to publish** an optimization.

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
