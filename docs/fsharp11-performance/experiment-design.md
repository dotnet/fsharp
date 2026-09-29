# Executed experiment protocol

This document describes the measured campaign, replacing the initial design-only proposal. Unexecuted proposal elements are identified explicitly below; they are not implied by the data.

## Questions and independent scopes

1. How much allocation, CPU, elapsed time, and peak process memory does full compilation require on fixed inputs?
2. What does a single FCS checker retain after checking a real multi-project dependency graph?
3. Do ordinary small-collection F# programs allocate less when compiled with the selected compiler/Core payloads?

Compilation, IDE checking, generated-program benchmarks, profiles, and diagnostic controls have separate result sets. No profile-derived timings are substituted into scalar tables.

## Toolchains and host

| Arm | Compiler payload | Executing runtime |
|---|---|---|
| `sdk10` | SDK 10.0.100 | 10.0.0 |
| `sdk11rc1` | SDK 11.0.100-rc.1.26425.128 | 11.0.0-rc.1.26425.128 |
| `sdk10-on-runtime11` | Exactly the baseline compiler and its dependencies | Exactly the RC1 runtime |

Every worker launch uses an absolute host path, `dotnet exec --fx-version <exact> --roll-forward Disable`, and a fresh process. The worker loads FCS and FSharp.Core from the selected SDK directory and records their actual informational versions. The old-compiler/new-runtime control is exercised, not assumed compatible.

The host is Windows 11 Enterprise 10.0.26200 under Hyper-V. Its EPYC model name says "64-Core", but the visible topology is **8 cores / 16 logical processors**, with 67,056,476 KiB total visible memory. It is shared with other work.

The baseline ZIP was SHA-512 verified. Exact releases, component-source commits and binary hashes are pinned in [experiment.json](experiment.json). SDK 10.0.401 is installed on the machine but is not an experiment compiler. The historical baseline is for trusted local benchmarking, not a deployment recommendation.

The source/build copies and baseline SDK are private to the campaign. Package restore uses the existing `C:\Nuget` cache; the initially proposed private package-cache isolation was not used. Dependencies are not restored inside measurements. Immutable reference files are inventoried by content hash.

## Workloads and frozen inputs

| Workload | Root / TFM | Pairs | Language mode |
|---|---|---:|---|
| FSharp.Core | `src\FSharp.Core\FSharp.Core.fsproj`, netstandard2.0 | 15 | 10.0 |
| FSharp.Compiler.Service | `src\Compiler\FSharp.Compiler.Service.fsproj`, netstandard2.0 | 15 | 10.0 |
| FsToolkit.ErrorHandling | Main library, net9.0 | 9 | 9.0 |
| Oxpecker | Main server library, net10.0 | 9 | 10.0 |
| Nu | `Nu\Nu\Nu.fsproj`, net10.0 | 15 | 10.0 |
| FsAutoComplete | Server project, net10.0 | 9 | 10.0 |

Source revisions are fixed before either arm is accepted. Core and FCS use the 10.0.100 release source, not a compiler bootstrapped during measurement. Core has three symmetric legacy-`or` compatibility replacements. The originally proposed Oxpecker revision failed baseline compilation in optional-value-option forwarding; a preceding compatible revision was chosen before measurements. FsAutoComplete's Paket source endpoint was changed from official NuGet v3 to official NuGet v2 without changing locked versions. [compatibility.json](compatibility.json) records these decisions.

Restore, parser/resource generation, assembly attributes, and prerequisite builds happen outside the interval. FCS references fixed FSharp.Core 9.0.300 as a workload dependency; Nu references fixed FSharp.Core 10.1.204. These are distinct from the Core assembly executing inside the compiler. Oxpecker's ViewEngine, Nu's C# math project, and FsAutoComplete's Core/Logging binaries are prepared first.

Capture executes the **Build** target with `SkipCompilerExecution=true` and `ProvideCommandLineArgs=true`, not merely the Compile target. The latter omitted `BeforeBuild`-generated `buildproperties.fs` in the initial attempt. That first Core cohort is retained as superseded and excluded from every published statistic.

Compiler argument boundaries, ordered sources, reference bytes, generated files, defines, optimizer settings, signing inputs, and resources are preserved. Missing language-version defaults become explicit `10.0`; an explicit repository version is retained. All warning-as-error promotions and diagnostic `--times` are removed symmetrically. Other project flags remain: Nu disables tail calls, and FsAutoComplete enables graph-based checking and graph dumping.

The [input directory](inputs) contains each final case and its file inventory. Absolute paths are historical provenance: a reproduction on another machine must reevaluate the pinned projects rather than blindly replace path substrings.

## Primary scalar worker

The same C# collector DLL runs on both runtimes. It has no FCS package reference; dependency resolution uses the chosen SDK's FSharp directory. Before the start snapshot it loads the assemblies, reads the case, and warms the scalar snapshot path. The timed region creates a checker and awaits one `FSharpChecker.Compile`.

| Metric | Source and boundary |
|---|---|
| Allocated bytes | `GC.GetTotalAllocatedBytes(true)` end minus start, across all worker threads |
| G0 / G1 / G2 | `GC.CollectionCount(g)` deltas; API cumulative-generation semantics |
| Wall nanoseconds | Monotonic `Stopwatch` timestamps |
| User/kernel CPU | Process CPU-time deltas, including JIT, GC, and all compiler threads |
| Peak working set | Windows `GetProcessMemoryInfo`, lifetime high-water mark read while the worker is alive, through operation completion |
| Peak private commit | `PROCESS_MEMORY_COUNTERS_EX.PeakPagefileUsage`, not physical residency or pagefile I/O |
| End working set / commit | Current OS counters in the completion snapshot |

Small reflection and diagnostic-enumeration costs are inside the operation. JSON serialization is outside it. The result remains rooted through the end snapshot. Compilation exceptions and error diagnostics reject the observation; no zero-valued success fallback is used.

The lifetime peak includes process startup and cannot be reset. It is not a polling maximum, and two peaks are never subtracted to invent an operation peak. The controller also stores launch-to-exit time and lifetime CPU, but these are not the headline operation values.

### Calibration

Each exact runtime allocated 4000 1024-byte arrays across four threads, then allocated and touched a 128-MiB buffer. The known payload minimum was 138,313,728 bytes. Observed totals were 138,443,008 and 138,445,464 bytes, including object/harness overhead. The peak remained observable after collection, while the retained heap dropped below 1 MiB. Calibration is the only deliberately concurrent harness activity; workload observations never overlap.

This exercises all-thread coverage and transient-peak retention. It is not an exact per-type CLR allocation census or a proof that no runtime counter bug can exist.

### Repetition and statistics

Two sacrificial fresh processes per arm prime filesystem caches. Measured processes still have cold managed compiler caches. Nine measured pairs are collected in alternating AB/BA order, starting AB; the ordering is deterministic, not randomized by a seed.

The extension thresholds are IQR/median above 5% for wall time, 1% for allocation, or 5% for peak working set. Affected cohorts were extended directly from 9 to the cap of 15 pairs, rather than through a 12-pair intermediate checkpoint. Remaining spread is reported, not removed.

All successful scalar observations enter the summary. The proposed 20-ms host/memory timeline, low-memory gate, and continuous contamination-window filter were **not implemented**. The stored run-average `host_other_cpu_fraction` field is unvalidated and was not used to filter data. Consequently there is no "clean-window" subset or clean-machine timing claim.

Each arm reports a median and linearly interpolated quartiles. Each paired contrast reports the median candidate/baseline ratio and a percentile bootstrap interval over complete pairs: 10,000 resamples, deterministic generator seed 110100. A zero baseline has no ratio. These intervals condition on the collected pairs; adaptive repetition, shared-host effects, and generalization to other machines are not eliminated by bootstrap.

The runtime-control and DATAS cohorts each have 15 pairs plus warmups. DATAS changes only the child-process `DOTNET_GCDynamicAdaptationMode` setting, with effective modes 0 and 1 checked in the results. Both shipping arms report server GC and DATAS enabled.

## Shipping entry-point verification

For every workload and compiler, rerun full compilation through the FCS worker and through that compiler's `fsc.dll` with identical arguments and working directory. All twelve pairs emitted byte-identical DLLs; hashes are in [cli-equivalence.json](cli-equivalence.json).

Long Windows command lines use F# response files, which preserve one compiler option per line. No splitting on spaces or generic shell quoting is applied to these records. The CLI check establishes output equivalence, not equal startup overhead.

Whole-MSBuild process-tree benchmarking was dropped from this campaign: it measures restore/build orchestration and concurrent process accounting, a different question from the requested compiler matrix. No sum of per-process peaks is presented as simultaneous solution RAM.

## IDE-style graph checking

The selected graphs are production-root dependency closures, not all projects or fixtures in a solution:

| Graph | Nodes / TFMs | Pairs |
|---|---|---:|
| Oxpecker | ViewEngine and server, both net10.0 | 15 |
| FsAutoComplete | Logging net8.0, Core net10.0, server net10.0 | 9 |

MSBuild supplies evaluated project references and nearest target frameworks. Each project/TFM has one shared `FSharpProjectOptions` node; references use the exact reference-assembly paths present in the compiler arguments. One checker, cache size 200, and otherwise shipping API defaults check nodes topologically. Checker creation and in-memory option-graph construction are inside the measured region; MSBuild evaluation is outside.

The graph/results stay rooted. After checking, record the operation counters and peak, wait ten seconds without forced GC, record current RAM/commit, then perform full blocking gen2 GC, wait for finalizers, collect again, and read `GC.GetTotalMemory(false)`. Those diagnostic GCs are excluded from checking counts. The peak ends at check completion; later idle RAM can exceed it.

The graph model was validated on both SDKs with Oxpecker's reference DLL temporarily absent. Checks succeeded with the source edge and failed without it. This distinguishes real FCS project references from prebuilt-DLL imports.

Closed-workspace reclamation, dropping client results, edit invalidation, whole-solution fixtures, and an actual LSP/VS host are not measured. They answer different lifecycle/integration questions; the data is specifically about a rooted cold-check workspace. Post-RC1 imported-assembly sharing is not available in these SDK payloads.

## Generated-program benchmarks

`Kernels.fs` contains cart pricing, access predicates, weighted array folds, an optional configuration, nested group folds, filter/map, an escaping-function control, and a non-capturing control. Collections are prepared outside measurement. Captured values change at runtime; each invocation returns a consumed integer. An independent C# implementation checks nine states for every size/kernel on each arm before benchmarking.

The F# DLLs use the two exact compilers and their respective SDK Core references, with common .NET 10 reference assemblies and F# 10 language mode. This is a compiler-plus-Core contrast, not a compiler-only contrast. The same prebuilt C# driver executes both DLLs on exactly the RC1 runtime; BenchmarkDotNet never recompiles them.

BenchmarkDotNet 0.14.0 uses its in-process emit toolchain inside each independently launched suite worker. Three suite launches per arm run AB/BA/AB. Every size/kernel has six warmup iterations and fifteen measured iterations per launch, targeting 250 ms per iteration, with `OutlierMode.DontRemove`. Cases within a suite share its process; they are not separate launches. There are 288 launch/case records and 4320 measured workload iterations.

Lengths are 0, 1, 4, 16, 64, and 1024. Article timings are medians of the three launch-level medians; B/op is the median of the three MemoryDiagnoser results. Full reports preserve actual operation counts and GC statistics. These GC statistics normalize differently from the compilation table. The initial shorter suites are pilot data, not the published program cohort.

Generated-type inheritance, captured fields, and `newobj` sites are inventoried from emitted IL. No JIT-disassembly claim is made. Unchanged allocation is a valid result: the requested RC1 predates the broad collection-inlining and partial-application changes in the contributor inventory.

## Explanatory allocation profiles

Pinned tools are `dotnet-trace` 10.0.745401 and TraceEvent 3.2.6. Startup-suspended launches begin tracing before the worker runs. Known-payload traces on both runtimes had named allocation ticks and zero reported loss.

The accepted Oxpecker pair uses runtime provider `0x1280009` at verbose level: GC, Loader, Type, sampled object allocation, and heap/type names. The buffer is 512 MiB; rundown is disabled. Typed object sampling supplies metadata, while byte attribution uses weighted `GCAllocationTick` amounts. Profiles run separately from all scalar observations.

Short generated names are insufficient. Join tick TypeID to bulk-type ModuleID and TypeNameID, join ModuleID to its loaded binary path, resolve valid non-nil TypeDef tokens against the exact SDK assembly, then check `@` and `FSharpFunc`/optimized-function inheritance. Synthetic/unresolved types stay unclassified. Reports retain tokens, full resolved names, bases, and captured fields.

Both accepted traces report zero lost events. Larger FCS traces did not pass: the reader reported invalid negative or huge loss values, including after provider/buffer/rundown/runtime changes. Their statistics are rejected, not patched into plausible zero-loss results. Exact counts and trace paths are preserved in the evidence.

Weighted samples are not exact allocated-object counts, and profiler overhead is not a scalar result. Actual CPU consumption comes from OS process counters; no managed stack-sample count is relabeled CPU usage.

## Evidence and exclusions

The publication retains accepted scalar runs including warmups, superseded early Core data with its reason, final input inventories, all final BenchmarkDotNet iterations, calibration results, binary/tool fingerprints, CLI-equivalence hashes, and accepted profile summaries. Large preparation logs, binlogs and raw traces stay in the recorded persistent artifact directory.

No later compiler, per-PR benchmark percentage, presumed zero, failed run, or incomplete profile fills a result cell. The contributor survey distinguishes recorded source ancestry from direct proof about every possible VMR-only edit or cherry-pick.
