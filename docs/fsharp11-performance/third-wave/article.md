# Performance improvements in the F# 11 compiler

The later F# 11 changes reduce compiler allocation beyond RC1, and remove per-call allocation from several small functional workloads. **Lower allocation does not automatically mean a lower RAM peak or less CPU work.** The tables show both sides.

This comparison uses **SDK 10.0.100 on .NET 10.0.0**, **SDK 11 RC1 on its .NET 11 RC1 runtime**, and an **optimized, self-hosted, ReadyToRun compiler built from the pinned VMR RC2 source**. The last compiler also runs on .NET 11 RC1. It is a local development payload, **not an official RC2 SDK/runtime measurement**. No headline arm forces DATAS, GC budgets, tiering, or JIT tuning.

## The same source, compiled by three compiler payloads

Each workload is a real root project with its dependencies prepared in advance. A fresh process runs `FSharpChecker.Compile` through parsing, checking, optimization and code generation. These are compiler measurements, **not end-to-end restore or `dotnet build` times**.

<!-- table:compilation -->

| Workload | Compiler | Allocated GiB | Peak RAM MiB | Peak private commit MiB | G0 / G1 / G2 | CPU s | Wall s |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: |
| FSharp.Core | 10.0.100 | 7.569 | 659.6 | 765.3 | 85 / 28 / 8.5 | 33.53 | 25.49 |
| FSharp.Core | 11 RC1 | 7.510 | 764.1 | 798.5 | 105.5 / 27 / 7 | 34.10 | 20.73 |
| FSharp.Core | Local RC2 R2R | 5.584 | 770.6 | 803.1 | 51 / 17 / 7 | 32.02 | 19.88 |
| FSharp.Compiler.Service | 10.0.100 | 35.203 | 2326.9 | 2415.1 | 125.5 / 53 / 13 | 128.17 | 76.96 |
| FSharp.Compiler.Service | 11 RC1 | 32.546 | 2972.8 | 3078.5 | 113.5 / 49.5 / 13 | 244.95 | 41.12 |
| FSharp.Compiler.Service | Local RC2 R2R | 27.288 | 3111.0 | 3223.6 | 91 / 42 / 12 | 232.09 | 39.65 |
| FsToolkit.ErrorHandling | 10.0.100 | 2.214 | 526.1 | 599.8 | 36 / 13 / 8 | 13.05 | 9.09 |
| FsToolkit.ErrorHandling | 11 RC1 | 1.955 | 549.0 | 601.1 | 37 / 17 / 10 | 13.84 | 6.73 |
| FsToolkit.ErrorHandling | Local RC2 R2R | 1.720 | 559.7 | 600.7 | 27 / 11 / 6 | 12.62 | 6.44 |
| Oxpecker | 10.0.100 | 1.145 | 392.5 | 351.0 | 29 / 12 / 6 | 6.77 | 5.97 |
| Oxpecker | 11 RC1 | 0.707 | 338.7 | 307.4 | 22.5 / 12 / 6 | 5.49 | 3.73 |
| Oxpecker | Local RC2 R2R | 0.590 | 312.0 | 302.8 | 18 / 8 / 4 | 4.93 | 3.39 |
| Nu | 10.0.100 | 14.764 | 1329.6 | 1481.0 | 77 / 32.5 / 11 | 64.85 | 41.72 |
| Nu | 11 RC1 | 13.850 | 1372.5 | 1535.8 | 81 / 34.5 / 13 | 64.50 | 33.84 |
| Nu | Local RC2 R2R | 11.433 | 1332.7 | 1495.6 | 64 / 27 / 10 | 62.34 | 33.83 |
| FsAutoComplete | 10.0.100 | 5.828 | 1028.4 | 1177.9 | 46 / 23 / 10.5 | 41.11 | 17.12 |
| FsAutoComplete | 11 RC1 | 5.287 | 1025.1 | 1135.4 | 49.5 / 25 / 12 | 38.11 | 12.93 |
| FsAutoComplete | Local RC2 R2R | 4.544 | 941.6 | 1106.5 | 45 / 21 / 11 | 35.97 | 12.50 |

<!-- /table:compilation -->

Every arm has **12 measured runs per workload**, preceded by two warmup processes. Six three-arm orders are repeated twice, balancing both position and predecessor. Nothing runs in parallel with another experiment, and no slow observation is removed. Each cell is the arm median; with an even sample count, median GC counts can end in `.5`.

Allocated GiB means cumulative managed allocation across all threads during compilation. Peak RAM is the Windows working-set high-water mark from process start through compilation completion, including startup; it is not an occasional sampled maximum. Peak private commit is a different memory quantity, not extra physical RAM to add to the working set. GC columns contain overlapping `GC.CollectionCount` deltas, not disjoint collection counts. CPU sums user and kernel time across the process; wall time is elapsed compiler-operation time.

### The improvement, and the limits

The percentages below are medians of **within-round ratios**, with 95% bootstrap intervals over the 12 rounds. They need not equal the ratio of the independently calculated arm medians.

<!-- table:changes -->

| Workload | RC1 vs 10 allocation % [95% CI] | Local vs 10 allocation % [95% CI] | Local vs RC1 allocation % [95% CI] | Local vs 10 peak RAM % [95% CI] |
| --- | --- | ---: | ---: | ---: |
| FSharp.Core | -0.78 [-0.92, -0.60] | -26.23 [-26.40, -26.09] | -25.66 [-25.82, -25.47] | +15.88 [12.02, 21.46] |
| FSharp.Compiler.Service | -7.56 [-7.66, -7.52] | -22.49 [-22.54, -22.47] | -16.16 [-16.18, -16.11] | +31.58 [26.23, 38.29] |
| FsToolkit.ErrorHandling | -11.65 [-11.77, -11.55] | -22.29 [-22.34, -22.23] | -12.06 [-12.14, -11.95] | +3.34 [-0.52, 7.45] |
| Oxpecker | -38.25 [-38.75, -38.16] | -48.51 [-48.87, -48.32] | -16.50 [-16.67, -16.37] | -20.55 [-20.99, -19.68] |
| Nu | -6.14 [-6.45, -5.86] | -22.49 [-22.74, -22.42] | -17.55 [-17.63, -17.41] | +3.19 [-2.89, 7.93] |
| FsAutoComplete | -9.31 [-9.36, -9.23] | -22.04 [-22.08, -22.02] | -14.07 [-14.09, -14.02] | -7.93 [-10.23, -3.54] |

<!-- /table:changes -->

Compared with SDK 10, the local compiler allocates **22%-49% less** across these workloads; compared with RC1, it allocates **12%-26% less**. Oxpecker falls from 1.145 to 0.590 GiB. FCS falls from 35.203 to 27.288 GiB, but its median RAM peak rises from 2326.9 to 3111.0 MiB and its total CPU remains higher. Allocation, RAM and CPU tell different parts of the story.

This is a shared Windows VM exposing 16 logical processors, not an isolated performance lab. Timings are descriptive, and the intervals describe these observed rounds, not other machines. Continuous host-load screening was not collected. Allocation is much steadier than timing and peak RAM, but GC scheduling and adaptive memory budgets still vary.

The [complete results](results.json) include all 336 scalar observations, warmups, arm quartiles and ranges. The [compilation matrix](compilation-matrix.csv) and [IDE matrix](ide-matrix.csv) are derived from those records, not hand-entered estimates.

## Why RC1 was not enough

The comparison follows the **actual F# subtree of dotnet/dotnet**, rather than assuming an SDK label implies a particular upstream compiler.

| Snapshot | Pinned VMR commit | Recorded F# source commit |
|---|---|---|
| Installed SDK 11 RC1 tag | `3551975be08744f0418857c5bed8ab1545c5dd47` | `c251d06dc38db89fa6df30b0b17a233e0ffe403d` |
| RC2 branch | `be46bdda4d6599b80dd4ccd89b2de49d96cbf36d` | `9cd6167a7265ce7264b22719503b7dfa9eb8f83c` |
| main | `0c804ec276a679143cdc4bc3391a1c2d0abfd69b` | `9cd6167a7265ce7264b22719503b7dfa9eb8f83c` |

RC2 and main have **byte-identical `src/fsharp/src` Git trees**, with tree ID `7904104d6778df42799007d3e4d6e19093144325`. Their complete F# subtrees differ in build scripts, tests and documentation. RC2 was selected because it contains the same production code as this pinned VMR main snapshot. This is not a claim that future main commits, or every later upstream dotnet/fsharp change, are included.

RC1 predates broad List/Array inlining, partial-application closure elimination, imported-assembly sharing across projects, and their relevant stabilization changes. The selected later source includes them.

The build matters too. Merely recompiling the old payload in Release would not exercise these changes. The RC1 compiler first built the new bootstrap compiler; that bootstrap then compiled the Release product against the newly built Core, without the `BUILDING_WITH_LKG` define. Packaging selected the new **net10.0 Core** used by SDK tools, followed by non-composite ReadyToRun publication for the exact .NET 11 RC1 runtime. No custom static F# PGO profile was supplied.

The product targets net11.0; its in-repository build uses the source Core reference selected by the repository, and packaging deploys the net10.0 variant. This is an aggregate **source-built payload** comparison, not a claim that compiler source, Core variant, target framework, R2R generation and individual PR effects have been independently isolated.

All 717 production files were checked against the pinned Git tree with normal Windows text normalization. PE metadata confirms optimized IL, the intended target frameworks, matching pre-/post-R2R module identities, and AMD64 ReadyToRun headers in `fsc`, FCS and Core. The new compiler also produced byte-identical DLLs through FCS and the `fsc` CLI for all six workloads. [Build and binary evidence](provenance.json) and [CLI checks](cli-equivalence.json) are included.

## .NET 11 and DATAS: keep the runtime in the comparison

The SDK 10 arm really runs on .NET 10, and the RC1 arm really runs on .NET 11. Inherited runtime tuning variables are removed before each measured child process; compiler server-GC configuration matches the SDK tools. **DATAS is left at its runtime default**, not enabled for only one arm. All compiler workers reported it enabled.

DATAS adapts server GC to application size. It was already enabled by default before .NET 11; it is not itself a new F# 11 feature. .NET 11 RC1 includes a fix keeping `GC.GetTotalAllocatedBytes` monotonic under server GC with DATAS ([dotnet/runtime#131069](https://github.com/dotnet/runtime/pull/131069)). The collector was calibrated against known allocations on both exact runtimes.

The [earlier campaign's fixed-runtime and DATAS controls](../article.md#the-runtime-matters-too-net-11-and-datas) remain separate. They help explain why less cumulative allocation need not imply a proportionately smaller working set. They are not substituted into this campaign's headline table.

Sources: [DATAS overview](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/datas), [GC configuration](https://learn.microsoft.com/en-us/dotnet/core/runtime-config/garbage-collector), [.NET 11 RC1 runtime notes](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/rc1/runtime.md).

## IDE-style work: memory after checking finishes

One checker loads a real source-reference graph and checks its projects serially. The graph and results remain alive through idle and full-GC measurements. These are the Oxpecker server/ViewEngine graph and the FsAutoComplete server/Core/Logging graph: **two and three projects**, not entire solutions including tests, a running language server, or Visual Studio.

<!-- table:ide -->

| Graph | Compiler | Projects | Allocated GiB | Peak RAM MiB | RAM after 10 s idle MiB | Live managed heap after full GC MiB | CPU / wall s |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: |
| FsAutoComplete | 10.0.100 | 3 | 6.027 | 1167.1 | 1185.8 | 544.6 | 39.41 / 17.79 |
| FsAutoComplete | 11 RC1 | 3 | 4.662 | 1062.9 | 1079.3 | 466.1 | 37.17 / 15.85 |
| FsAutoComplete | Local RC2 R2R | 3 | 3.824 | 856.7 | 873.9 | 380.1 | 33.30 / 14.53 |
| Oxpecker | 10.0.100 | 2 | 1.452 | 390.6 | 404.2 | 107.9 | 8.43 / 6.69 |
| Oxpecker | 11 RC1 | 2 | 0.702 | 299.1 | 319.3 | 84.4 | 5.55 / 4.56 |
| Oxpecker | Local RC2 R2R | 2 | 0.583 | 281.9 | 302.2 | 74.9 | 5.30 / 4.32 |

<!-- /table:ide -->

The retained managed heap falls from **544.6 to 380.1 MiB** for FsAutoComplete and from **107.9 to 74.9 MiB** for Oxpecker, comparing SDK 10 with the local payload. These cohorts also have 12 measured runs per arm. Natural RAM after ten seconds of idle and live managed bytes after a forced full GC answer different questions. The peak column ends when checking returns, so later idle RAM can be higher. Retained-heap figures do not describe memory released after closing a workspace.

Checker and graph construction are inside the allocation/time boundary; MSBuild evaluation is outside it. Project-cache capacity is 200; other checker options use the selected API's defaults. Actual source edges, rather than just prebuilt reference DLLs, were established by a positive/negative graph control in the [original evidence](../provenance.json).

Cross-project imported-assembly sharing is present in the local payload but absent from RC1's source snapshot. It is a relevant mechanism, not a measured per-PR attribution. Building FsAutoComplete with a newer SDK would not by itself replace the FCS package that its server loads; this worker explicitly loads the selected FCS.

## The compiler can also make ordinary F# code allocate less

The application experiment compiles the same small functional kernels with each compiler and its matching Core, then calls their prebuilt DLLs from the same C# BenchmarkDotNet driver. **SDK 10 programs run on .NET 10; RC1 and local-RC2 programs run on .NET 11 RC1**, with ordinary application runtime defaults. The compiler language defaults are used, and the source/API reference surface is common.

There are three independent suite processes per arm, in ABC/CAB/BCA order. Each of 48 cases has six warmup and fifteen measured iterations, with no timing outlier removal. Lengths are 0, 1, 4, 16, 64 and 1024. The table shows length four, where a callback object can dominate the allocation.

<!-- table:programs -->

| Kernel, length 4 | 10.0.100 B/op | 11 RC1 B/op | Local RC2 B/op | Median ns/op (10 / RC1 / local) |
| --- | --- | ---: | ---: | ---: |
| Cart-price List.fold | 24 | 24 | 0 | 13.23 / 13.16 / 7.10 |
| Access-rule exists / forall | 48 | 48 | 0 | 14.08 / 14.86 / 7.28 |
| Weighted Array.fold / fold2 | 48 | 48 | 0 | 13.88 / 15.35 / 10.85 |
| Partially applied Option.map | 0 | 0 | 0 | 2.23 / 2.20 / 1.90 |
| Nested group folds | 48 | 48 | 0 | 27.30 / 28.10 / 4.11 |
| Filtering and mapping | 146 | 146 | 146 | 31.98 / 34.30 / 31.34 |
| Escaping callback control | 24 | 24 | 24 | 7.78 / 7.84 / 7.44 |
| Non-capturing callback control | 0 | 0 | 0 | 9.12 / 8.65 / 3.48 |

<!-- /table:programs -->

The cart fold, access-rule checks, weighted array folds and nested folds no longer allocate per call in the local payload at this size. Captured discount, threshold and scale values change during measurement; this is not constant-folded work. Inputs and the outer callable are prepared outside the measured operation.

The controls matter. The escaping callback still allocates. The option and non-capturing examples were already allocation-free. The filter/map pipeline still allocates, including output collections: not every functional pipeline becomes allocation-free. Independent C# checks validate all cases against multiple changing states, including empty inputs and extreme values, at every suite startup.

Generated IL gives a second line of evidence: function-object construction sites fall from **19 in each SDK-generated assembly to 11** in the local-compiler assembly. That is a static site count, not an object count per operation. The [all-size matrix](program-matrix.csv), [raw launch reports](programs/) and [per-launch results](results.json) retain the distinction.

The old same-.NET-11 application measurements are preserved as [historical compiler/Core controls](../article.md#less-allocation-in-the-programs-we-compile). They are not mixed with the matching-runtime results here. The reused driver's legacy job ID is `FixedRuntime`; the recorded process runtime, not that label, determines which runtime actually executed each suite.

## Where did the compiler's closure allocation go?

Fresh, separate Oxpecker allocation profiles were collected for all three payloads. Type IDs were joined to module and metadata-token records and resolved against the exact binaries. A generated name containing `@` is classified as a function object only when its resolved inheritance supports that classification.

<!-- table:profiles -->

| Oxpecker profile | Allocation ticks | Weighted allocation MiB | Weighted generated-function MiB | Lost events |
| --- | --- | ---: | ---: | ---: |
| 10.0.100 | 10999 | 1138.4 | 115.4 | 0 |
| 11 RC1 | 6786 | 710.4 | 97.8 | 0 |
| Local RC2 R2R | 5789 | 602.9 | 45.4 | 0 |

<!-- /table:profiles -->

All three traces pass the integrity gate with zero reported lost events. These are **sampled, weighted allocation estimates**, not precise object counts or the scalar allocation totals. Their detailed reports preserve full names, metadata identities and captured fields. Profiled timings and memory never enter the unprofiled tables.

The earlier large-FCS traces that failed integrity checks remain rejected. This article does not invent full-FCS closure attribution from them.

## The PRs behind the mechanisms

The five-month survey covers **auduchinok** and **T-Gro**, from April 24 through September 24, 2026. Twelve of the 52 selected merge commits are ancestors of RC1's F# source; **49 are ancestors of the selected RC2/main source**. The [full inventory](contributions.csv) records inclusion separately from effect attribution.

| Contributor | Selected PRs | Mechanism and release boundary |
|---|---|---|
| auduchinok | [#20090](https://github.com/dotnet/fsharp/pull/20090), [#20092](https://github.com/dotnet/fsharp/pull/20092) | Avoid eager non-F# assembly traversal and defer imported namespace/type creation; present in RC1 |
| auduchinok | [#20088](https://github.com/dotnet/fsharp/pull/20088), [#20249](https://github.com/dotnet/fsharp/pull/20249), [#20250](https://github.com/dotnet/fsharp/pull/20250) | Remove unnecessary locks/tables and a closure retaining metadata; present in RC1 |
| auduchinok | [#20254](https://github.com/dotnet/fsharp/pull/20254), [#20259](https://github.com/dotnet/fsharp/pull/20259), [#20301](https://github.com/dotnet/fsharp/pull/20301) | Share calling conventions, type references and pickled references; present in RC1 |
| auduchinok | [#20255](https://github.com/dotnet/fsharp/pull/20255), [#20285](https://github.com/dotnet/fsharp/pull/20285), [#20287](https://github.com/dotnet/fsharp/pull/20287), [#20364](https://github.com/dotnet/fsharp/pull/20364) | Smaller imported/typed-tree representations; present in the later source |
| auduchinok | [#20296](https://github.com/dotnet/fsharp/pull/20296), [#20481](https://github.com/dotnet/fsharp/pull/20481) | Cross-project imported-assembly sharing and avoiding repeated background work; after RC1 |
| auduchinok | [#20261](https://github.com/dotnet/fsharp/pull/20261), [#20486](https://github.com/dotnet/fsharp/pull/20486), [#20489](https://github.com/dotnet/fsharp/pull/20489), [#20490](https://github.com/dotnet/fsharp/pull/20490), [#20494](https://github.com/dotnet/fsharp/pull/20494) | Metadata caches, mapping, interning and lazy augmentation; present in the later source |
| T-Gro | [#20244](https://github.com/dotnet/fsharp/pull/20244) | Avoid super-linear guarded shared-or active-pattern compilation; present in RC1 |
| T-Gro | [#20337](https://github.com/dotnet/fsharp/pull/20337), [#20348](https://github.com/dotnet/fsharp/pull/20348), [#20350](https://github.com/dotnet/fsharp/pull/20350), [#20351](https://github.com/dotnet/fsharp/pull/20351), [#20354](https://github.com/dotnet/fsharp/pull/20354) | Hot-path data structures and allocation; after RC1 |
| T-Gro | [#20363](https://github.com/dotnet/fsharp/pull/20363), [#20367](https://github.com/dotnet/fsharp/pull/20367), [#20368](https://github.com/dotnet/fsharp/pull/20368) | Fused optimizer work and constraint-solver/stack-guard closures; after RC1 |
| T-Gro | [#20422](https://github.com/dotnet/fsharp/pull/20422), [#20487](https://github.com/dotnet/fsharp/pull/20487), [#20388](https://github.com/dotnet/fsharp/pull/20388) | Higher-order collection inlining, partial-application closure elimination and shared empty arrays; after RC1 |
| T-Gro | [#20571](https://github.com/dotnet/fsharp/pull/20571), [#20555](https://github.com/dotnet/fsharp/pull/20555) | Stabilize closure optimization for F# 11 and select net10.0 Core for SDK tools; after RC1 |

Three surveyed PRs, [#20424](https://github.com/dotnet/fsharp/pull/20424), [#20425](https://github.com/dotnet/fsharp/pull/20425) and [#20439](https://github.com/dotnet/fsharp/pull/20439), merged into experimental feature branches rather than the selected mainline ancestry. Their merge commits are **not credited as shipped changes here**. Likewise, MSBuild concurrency support in [#20506](https://github.com/dotnet/fsharp/pull/20506) is present in the source, but this serial compiler-only experiment does not measure parallel MSBuild.

PR-local percentages are not additive. These rows identify mechanisms present in the tested payloads; they do not assign a fraction of an aggregate result to an individual PR.

## Reproduction and scope

The [experiment manifest](experiment.json), [reproduction instructions](README.md), [binary/build evidence](provenance.json) and [executable harness](../../../tests/benchmarks/FSharp11/ThirdWave/) accompany this draft. Original frozen workload revisions, source adjustments and dependency hashes are retained in the [shared input manifest](../experiment.json) and [compatibility record](../compatibility.json).

Workload source, dependency references and compiler arguments are identical across compiler arms. This includes the old Core source's symmetric compatibility edits, the common Oxpecker revision, and the repository-specific flags retained for Nu and FsAutoComplete. The compiler's own Core payload is intentionally different; the workloads' prepared dependency binaries are not silently rebuilt between arms.

The story is not "everything got faster." Later F# 11 work substantially reduces cumulative compiler allocation, reduces retained memory in the selected project graphs, and makes several realistic small-collection operations allocation-free. The RAM and CPU columns remain essential qualifications.
