# Performance improvements in the F# 11 compiler

> **Preserved first campaign.** The [latest three-compiler article](third-wave/article.md) adds the self-hosted VMR RC2-source Release/R2R compiler and matching-runtime application measurements. The data and tables below remain the original RC1 comparison.

> Article draft with measured data. The comparison is .NET SDK **10.0.100** versus **11.0.100-rc.1.26425.128**, not SDK 10.0.4xx or a newer development compiler. All runs were serial.

## The same code, compiled by two generations of F#

RC1 allocated less while compiling every project in this sample. That is not the same as using less RAM: several workloads reached a higher memory peak. FSharp.Compiler.Service is the clearest tradeoff, with lower allocation and elapsed time but substantially higher peak RAM and CPU consumption.

### Compilation results

One root project and one target framework per workload; dependencies are prepared before measurement. Each observation uses a fresh process hosting the selected SDK's `FSharpChecker.Compile`, including optimization and code generation. This isolates compiler work from restore and MSBuild. It is **not** a table of end-to-end `dotnet build` times.

| Workload | SDK compiler | Allocated GiB | Peak RAM MiB | Peak private commit MiB | G0 / G1 / G2 | CPU s | Wall s |
|---|---|---:|---:|---:|---|---:|---:|
| FSharp.Core | 10.0.100 | 7.572 | 663.6 | 695.9 | 82 / 29 / 8 | 33.47 | 25.30 |
| FSharp.Core | 11 RC1 | 7.501 | 765.9 | 799.2 | 101 / 25 / 7 | 33.66 | 20.55 |
| FSharp.Compiler.Service | 10.0.100 | 35.205 | 2354.8 | 2437.4 | 126 / 53 / 13 | 122.73 | 74.54 |
| FSharp.Compiler.Service | 11 RC1 | 32.552 | 3133.4 | 3255.7 | 113 / 49 / 12 | 233.67 | 39.20 |
| FsToolkit.ErrorHandling | 10.0.100 | 2.215 | 532.5 | 589.4 | 36 / 13 / 8 | 13.09 | 9.13 |
| FsToolkit.ErrorHandling | 11 RC1 | 1.955 | 529.7 | 564.4 | 37 / 17 / 10 | 12.84 | 6.68 |
| Oxpecker | 10.0.100 | 1.149 | 395.0 | 352.6 | 29 / 12 / 6 | 6.86 | 6.03 |
| Oxpecker | 11 RC1 | 0.707 | 339.0 | 309.0 | 22 / 12 / 6 | 4.97 | 3.58 |
| Nu | 10.0.100 | 14.804 | 1256.6 | 1469.8 | 78 / 34 / 10 | 66.13 | 42.36 |
| Nu | 11 RC1 | 13.842 | 1404.1 | 1532.8 | 81 / 35 / 13 | 66.47 | 35.41 |
| FsAutoComplete | 10.0.100 | 5.829 | 1005.6 | 1157.1 | 51 / 23 / 11 | 41.53 | 17.11 |
| FsAutoComplete | 11 RC1 | 5.287 | 1025.9 | 1141.1 | 49 / 25 / 12 | 37.92 | 12.68 |

Allocated bytes are cumulative managed allocation across all threads during the compiler operation, not the surviving heap. Peak RAM is the OS working-set high-water mark from process start through compilation completion, including startup. Private commit is a different quantity, not additional RAM to add to it. GC counts are `GC.CollectionCount(0/1/2)` deltas; they are not disjoint collections or BenchmarkDotNet's normalized counts.

Core, FCS, and Nu have **15 measured pairs** each; FsToolkit, Oxpecker, and FsAutoComplete have **nine** each. Each cohort follows two warmup processes per compiler, with AB/BA order alternating. The table gives each arm's median.

Oxpecker shows the largest allocation reduction, from 1.149 to 0.707 GiB, accompanied by a lower memory peak. FsToolkit allocates less with an essentially unchanged peak. FCS allocates about 2.65 GiB less, yet its median peak grows from 2355 to 3133 MiB. Its lower wall time should not be described as less total CPU work: the recorded CPU time increases from 122.73 to 233.67 seconds.

### How repeatable were the differences?

The following changes are **median per-pair ratios**, with a 95% bootstrap interval over pairs. They are a different statistic from dividing the two arm medians above, so the percentages need not coincide.

| Workload | Allocation change, % [95% interval] | Peak RAM change, % [95% interval] |
|---|---:|---:|
| FSharp.Core | -1.04 [-1.17, -0.87] | +14.91 [+12.61, +17.84] |
| FSharp.Compiler.Service | -7.55 [-7.57, -7.52] | +28.88 [+26.96, +31.59] |
| FsToolkit.ErrorHandling | -11.72 [-11.80, -11.59] | -0.29 [-4.75, +2.29] |
| Oxpecker | -38.61 [-38.72, -38.09] | -13.95 [-19.11, -7.89] |
| Nu | -6.48 [-6.64, -6.31] | +8.82 [+2.21, +14.01] |
| FsAutoComplete | -9.32 [-9.38, -9.23] | +1.08 [-3.54, +5.34] |

Allocation was much more stable than peak RAM. GC counts and memory peaks are not deterministic: scheduling, background GC, DATAS, and memory pressure matter.

This is a shared Windows VM exposing 16 logical processors. The proposed continuous host-load screening was not collected, so **all timings are descriptive shared-machine observations**, not clean-machine speedup estimates. No slow scalar run was removed. The intervals describe the observed pairs, not uncertainty across other machines. Raw observations, arm IQRs, and paired intervals are in [results.json](results.json).

The frozen inputs and compiler options are identical between arms. Core's old source needs three symmetric compatibility edits for the legacy `or` operator. Oxpecker uses a common revision preceding its multipart API, which failed baseline compilation at the initially selected revision. FsAutoComplete retains its repository's graph-checking and graph-dump flags; Nu retains `--tailcalls-`. [The input manifest](experiment.json) and [compatibility record](compatibility.json) make these choices explicit.

As an entry-point check, all **12 workload/compiler combinations produced byte-identical DLLs** through the FCS worker and the shipping `fsc.dll` with the same arguments. [The hashes](cli-equivalence.json) establish compiler-output equivalence; they do not turn the table into whole-build timings.

## The runtime matters too: .NET 11 and DATAS

The headline comparison changes the runtime as well as the compiler payload. To separate those effects, we also compiled FCS with both payloads on exactly the RC1 runtime, then varied DATAS with the RC1 compiler held fixed. Each control has 15 measured pairs.

| FCS compilation control | Allocated GiB | Peak RAM MiB | G0 / G1 / G2 | CPU s | Wall s |
|---|---:|---:|---|---:|---:|
| Old compiler, .NET 11 RC1 runtime | 35.250 | 2498.6 | 127 / 53 / 13 | 121.09 | 74.54 |
| RC1 compiler, same runtime | 32.544 | 3054.2 | 114 / 50 / 13 | 238.36 | 39.53 |
| RC1 compiler/runtime, DATAS off | 32.583 | 5333.1 | 14 / 7 / 4 | 244.05 | 40.50 |
| RC1 compiler/runtime, DATAS on | 32.541 | 2980.5 | 113 / 49 / 13 | 244.94 | 40.74 |

Moving only the old compiler to .NET 11 does not reproduce the RC1 compiler's allocation or wall-time result. The fixed-runtime comparison still shows lower allocation and wall time with higher CPU and peak RAM. This is a **compiler-payload** contrast, including the FSharp.Core used inside the compiler, not a decomposition into individual PR effects.

DATAS makes a substantial difference to the memory budget in this workload: the median peak is 5333 MiB with it off, versus 2980 MiB with it on. Allocation volume and elapsed time are much closer, while collection counts change considerably. These are separate diagnostic cohorts, not replacements for the shipping-default rows.

DATAS adapts server GC to application size. It was already enabled by default in .NET 9; **it is not a new F# 11 feature**. Both shipping workers reported it enabled. .NET 11 RC1 also includes a fix keeping `GC.GetTotalAllocatedBytes` monotonic under server GC with DATAS ([dotnet/runtime#131069](https://github.com/dotnet/runtime/pull/131069)). We calibrated allocation accounting on both exact runtimes using known allocations on multiple threads and a touched 128-MiB transient buffer.

Sources: [DATAS overview](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/datas), [GC configuration](https://learn.microsoft.com/en-us/dotnet/core/runtime-config/garbage-collector), [.NET 11 RC1 runtime notes](https://github.com/dotnet/core/blob/main/release-notes/11.0/preview/rc1/runtime.md).

## Loading a project graph, not just compiling one project

An IDE keeps project state after checking finishes. We used one FCS checker, an MSBuild-evaluated graph, shared project-reference nodes, and serial `ParseAndCheckProject` calls. Results and the graph remain rooted during the idle and retained-heap measurements. This measures **cold production dependency graphs**, not full solutions with tests, an LSP server, or Visual Studio.

| Production graph | SDK FCS | Projects checked | Allocated GiB | Peak RAM MiB | RAM after 10 s idle MiB | Live managed heap after full GC MiB | CPU / wall s |
|---|---|---:|---:|---:|---:|---:|---|
| FsAutoComplete | 10.0.100 | 3 | 6.028 | 1165.5 | 1180.7 | 544.7 | 40.17 / 17.78 |
| FsAutoComplete | 11 RC1 | 3 | 4.662 | 1065.5 | 1081.5 | 466.2 | 39.09 / 16.00 |
| Oxpecker | 10.0.100 | 2 | 1.451 | 383.8 | 395.2 | 108.0 | 8.48 / 6.63 |
| Oxpecker | 11 RC1 | 2 | 0.701 | 305.1 | 323.6 | 84.3 | 5.64 / 4.56 |

FsAutoComplete's graph contains its server, Core, and Logging projects; the evaluated target mapping is net10.0/net10.0/net8.0. Oxpecker contains the server and ViewEngine, both net10.0. They have nine and fifteen measured pairs respectively.

Managed memory retained after a full GC falls from 544.7 to 466.2 MiB for FsAutoComplete, and from 108.0 to 84.3 MiB for Oxpecker. These are **results-alive** measurements, not a claim about memory returned after closing a workspace. The peak column ends when checking returns; later idle RAM can be higher. Natural idle RAM and forced-GC live bytes are intentionally separate. The time/allocation boundary includes checker and in-memory graph construction; MSBuild evaluation is outside it.

The source edges were checked by temporarily removing Oxpecker's prebuilt reference DLL: both SDKs still checked successfully, while removing the FCS project edge made the negative control fail. This guards against accidentally measuring a graph of binary references.

**Release boundary:** cross-project imported-assembly sharing in [#20296](https://github.com/dotnet/fsharp/pull/20296) merged after RC1's F# source snapshot. It cannot explain these results. Also, changing the SDK used to build FsAutoComplete does not replace the FCS package its server loads; our worker explicitly loads the selected SDK's FCS.

## What about closure allocations?

We collected separate startup-complete Oxpecker allocation profiles with pinned `dotnet-trace` and TraceEvent versions. Allocation type IDs were joined to module and metadata-token records, then resolved against the exact SDK binaries. A generated name containing `@` counts here only when its resolved type inherits from `FSharpFunc` or its optimized-function bases; captured fields are retained in the report.

| Oxpecker profile | Allocation ticks | Weighted allocation MiB | Weighted generated-function MiB | Reported lost events |
|---|---:|---:|---:|---:|
| 10.0.100 | 11006 | 1139.5 | 116.3 | 0 |
| 11 RC1 | 6827 | 717.7 | 93.4 | 0 |

These are **sampled, weighted estimates**, not exact closure counts or the precise scalar totals above. They show that generated function objects remain part of compiler allocation; they do not establish that closure removal explains the entire reduction. Full names, metadata tokens, inheritance, and captured fields are in the [baseline](profiles/oxpecker-allocation-sdk10-summary.json) and [RC1](profiles/oxpecker-allocation-sdk11rc1-summary.json) reports.

The larger FCS traces failed the reader's integrity gate, including invalid loss counts, and were rejected rather than interpreted. Profiled timing and memory are never mixed into the scalar tables.

## Less allocation in the programs we compile?

That is a different experiment. We compiled realistic small-collection kernels with each SDK's compiler and FSharp.Core, then called the prebuilt DLLs from one fixed C# BenchmarkDotNet driver on **the same .NET 11 RC1 runtime**. Both variants passed independent C# result checks for empty and nonempty inputs and varying captured values.

Each arm has three independent suite processes in AB/BA/AB order, six warmup iterations and fifteen measured iterations per case, with no timing outlier removal. Lengths are 0, 1, 4, 16, 64, and 1024. The table shows the median across the three launch-level results for length four.

| Workload, length 4 | 10.0.100 B/op | 11 RC1 B/op | Median ns/op (old / new) |
|---|---:|---:|---|
| Cart-price List.fold | 24 | 24 | 14.02 / 13.72 |
| Access-rule List.exists / List.forall | 48 | 48 | 15.96 / 15.60 |
| Weighted Array.fold / Array.fold2 | 48 | 48 | 16.00 / 15.69 |
| Partially applied Option.map | 0 | 0 | 2.30 / 2.52 |
| Nested group folds | 48 | 48 | 28.57 / 28.88 |
| Filtering and mapping a batch | 146 | 146 | 35.59 / 33.73 |
| Escaping callback control | 24 | 24 | 8.23 / 8.14 |
| Non-capturing callback control | 0 | 0 | 9.78 / 8.88 |

There is **no allocation improvement in these length-four cases**: each B/op value repeated across all three launches. The cart still allocates its captured callback, the escaping callback still allocates, and the already-optimized option/non-capturing cases allocate nothing. Filter/map includes its output collections; its bytes per operation average over changing thresholds. The ns/op values are elapsed microbenchmark timings, not OS CPU time.

This is consistent with the release boundary: broad List/Array inlining in [#20422](https://github.com/dotnet/fsharp/pull/20422) and partial-application closure elimination in [#20487](https://github.com/dotnet/fsharp/pull/20487) are **not in this RC1 snapshot**. Inlining can improve ordinary F# programs, not just the compiler, but these particular RC1 measurements cannot demonstrate changes that arrived later.

All sizes, raw iterations, allocation counts, and generated IL allocation-site inventories are included in [the program data](generated-program-matrix.csv) and [results.json](results.json).

## PRs contributing to the compiler story

The survey covers performance and supporting work by **auduchinok** and **T-Gro**, merged from April 24 through September 24, 2026. The [52-PR inventory](contributions.csv) includes titles, dates, merge commits, and ancestry evidence. Twelve entries are in RC1's recorded F# source ancestry; forty are not.

These mechanisms are present in that ancestry. The table does not assign a fraction of our measured result to any one PR.

| Contributor | PRs present in RC1's recorded source ancestry | Mechanism |
|---|---|---|
| auduchinok | [#20090](https://github.com/dotnet/fsharp/pull/20090), [#20092](https://github.com/dotnet/fsharp/pull/20092) | Avoid eager non-F# assembly traversal; defer namespace and type-definition creation |
| auduchinok | [#20088](https://github.com/dotnet/fsharp/pull/20088), [#20249](https://github.com/dotnet/fsharp/pull/20249) | Avoid per-instance lock objects and unnecessary member tables |
| auduchinok | [#20250](https://github.com/dotnet/fsharp/pull/20250) | Stop retaining a binary metadata view through a closure |
| auduchinok | [#20254](https://github.com/dotnet/fsharp/pull/20254), [#20259](https://github.com/dotnet/fsharp/pull/20259), [#20301](https://github.com/dotnet/fsharp/pull/20301) | Share calling conventions, type references, and pickled references |
| auduchinok | [#20256](https://github.com/dotnet/fsharp/pull/20256), [#20298](https://github.com/dotnet/fsharp/pull/20298) | Cache and group C# extension-member information |
| auduchinok | [#20286](https://github.com/dotnet/fsharp/pull/20286) | Allocate the ad-hoc member list only when needed |
| T-Gro | [#20244](https://github.com/dotnet/fsharp/pull/20244) | Avoid super-linear compilation of guarded shared-or active-pattern matches |

### More work after RC1's source snapshot

These entries belong to the same five-month survey, but not to the measured RC1 attribution:

| Contributor | PRs | Area |
|---|---|---|
| auduchinok | [#20255](https://github.com/dotnet/fsharp/pull/20255), [#20285](https://github.com/dotnet/fsharp/pull/20285), [#20287](https://github.com/dotnet/fsharp/pull/20287), [#20364](https://github.com/dotnet/fsharp/pull/20364) | Smaller imported and typed-tree representations |
| auduchinok | [#20296](https://github.com/dotnet/fsharp/pull/20296), [#20481](https://github.com/dotnet/fsharp/pull/20481) | Cross-project sharing and avoiding repeated background work |
| auduchinok | [#20261](https://github.com/dotnet/fsharp/pull/20261), [#20486](https://github.com/dotnet/fsharp/pull/20486), [#20489](https://github.com/dotnet/fsharp/pull/20489), [#20490](https://github.com/dotnet/fsharp/pull/20490), [#20494](https://github.com/dotnet/fsharp/pull/20494) | Metadata caches, mapping, interning, and lazy augmentation |
| T-Gro | [#20337](https://github.com/dotnet/fsharp/pull/20337), [#20348](https://github.com/dotnet/fsharp/pull/20348), [#20350](https://github.com/dotnet/fsharp/pull/20350), [#20351](https://github.com/dotnet/fsharp/pull/20351), [#20354](https://github.com/dotnet/fsharp/pull/20354) | Hot-path data structures and allocation |
| T-Gro | [#20363](https://github.com/dotnet/fsharp/pull/20363), [#20367](https://github.com/dotnet/fsharp/pull/20367), [#20368](https://github.com/dotnet/fsharp/pull/20368) | Fused optimizer passes and constraint-solver/stack-guard closures |
| T-Gro | [#20422](https://github.com/dotnet/fsharp/pull/20422), [#20487](https://github.com/dotnet/fsharp/pull/20487), [#20388](https://github.com/dotnet/fsharp/pull/20388) | Collection inlining, partial applications, and shared empty arrays |
| T-Gro | [#20571](https://github.com/dotnet/fsharp/pull/20571), [#20555](https://github.com/dotnet/fsharp/pull/20555), [#20506](https://github.com/dotnet/fsharp/pull/20506) | Supporting stabilization, SDK Core payload, and MSBuild concurrency |

The full inventory also lists the individual compiler closure-elimination PRs. PR-local percentages are not additive, and their source projects, runtime settings, and baselines are not necessarily ours.

## Reproduction

[The protocol](experiment-design.md), [SDK/source manifest](experiment.json), [input and tool evidence](provenance.json), and [executable harness](../../tests/benchmarks/FSharp11/README.md) accompany the data. SDK provenance follows the binary informational version to its VMR commit and F# source manifest, rather than assuming the SDK release date is the compiler's source cutoff.

The central result is narrower and more useful than "everything got faster": allocation decreased across these compiler workloads; RAM and CPU did not always decrease with it. The IDE-style graphs retained less managed memory, while the selected application kernels showed unchanged allocation under the specifically requested RC1 compiler.
