# Performance improvements in the F# 11 compiler

<!-- generated:opening -->
F# 11 puts less pressure on the garbage collector and keeps less compiler data alive after IDE project checks. Across six real-world compilation workloads, the source-built compiler allocates **roughly 20-50% less than the compiler released in SDK 10.0.100**. Our multi-project IDE checks retain **about 30% less managed memory**.

Behind this is a late-summer push: **49 performance and supporting PRs** from auduchinok and T-Gro, merged between **Aug 12 and Sep 21, 2026**. The work tackles unnecessary allocations, duplicated compiler data and memory held after typechecking. **12 changes are already in RC1**; the remaining **37 are in the RC2 source**, headed for .NET 11 and F# 11 GA.

We're excited to share those gains with F# developers: less allocation during compilation, less memory retained by IDE project checking, and fewer short-lived objects in your own code. One change reaches beyond the compiler: [#20422](https://github.com/dotnet/fsharp/pull/20422) inlines higher-order `FSharp.Core` List/Array functions and their lambda arguments, so everyday code using operations such as `List.fold` can stop allocating a closure on every call.
<!-- /generated:opening -->

## Less allocation during compilation

**Old** is the compiler released in **SDK 10.0.100**. **RC1** is SDK 11 RC1, shown as an intermediate point. **New** is our optimized, source-built F# 11 compiler from the RC2/main source snapshot headed for GA.

<!-- generated:compilation -->
| Compilation workload | Old GB | RC1 GB | New GB | RC1 vs old: less allocation | New vs old: less allocation |
| --- | ---: | ---: | ---: | ---: | ---: |
| FSharp.Core | 8.127 | 8.064 | 5.995 | 0.8% | 26.2% |
| FSharp.Compiler.Service | 37.799 | 34.946 | 29.300 | 7.5% | 22.5% |
| FsToolkit.ErrorHandling | 2.378 | 2.100 | 1.847 | 11.7% | 22.3% |
| Oxpecker | 1.230 | 0.759 | 0.634 | 38.3% | 48.5% |
| Nu | 15.853 | 14.871 | 12.276 | 6.2% | 22.6% |
| FsAutoComplete | 6.258 | 5.676 | 4.879 | 9.3% | 22.0% |
<!-- /generated:compilation -->

*Median managed allocation per compilation, over 12 runs per version.*

<!-- generated:headline -->
RC1 already allocates **0.8%-38.3% less than the released compiler**. The source-built F# 11 compiler takes that to **22.0%-48.5% less**, depending on the project.
<!-- /generated:headline -->

The released compiler runs on .NET 10.0.0. RC1 and the self-hosted Release/ReadyToRun source build run on .NET 11 RC1. The new column previews the compiler changes for RC2 and GA; it is **not a measurement of a final GA SDK**. Its exact source is pinned in the [build provenance](../third-wave/provenance.json).

Compilation is measured through `FSharpChecker.Compile`, not restore or a full `dotnet build`.

![Actual compiler allocation in GB for six workloads: the released SDK 10 compiler, RC1, and source-built F# 11. Each project's bar widths use their own zero-based scale.](compiler-allocation.svg)

*Bar widths are scaled separately for each project.*

<!-- generated:sdk-caveat -->
**Lower allocation is not a promise of lower peak RAM or CPU time.** FCS's median peak resident RAM rises from **2439.9 to 3262.1 MB**, and CPU time from 128.17 to 232.09 seconds. Peak RAM is higher in 4 of the six workloads. [All RAM, CPU and wall-time values](../third-wave/compilation-matrix.csv) remain published.
<!-- /generated:sdk-caveat -->

## The allocation gains also reach .NET Framework

The allocation gains also hold on **x64 .NET Framework 4.8.1**.

The old version uses NuGet FSharp.Compiler.Service **43.10.100** and FSharp.Core **10.0.100**, matching the SDK 10.0.100 generation. The new version is built from F# 11 source. Both target netstandard2.0 and use ordinary IL, with no R2R or NGen.

<!-- generated:framework -->
| Compilation workload | Old GB | New GB | New vs old: less allocation |
| --- | ---: | ---: | ---: |
| FSharp.Core | 8.305 | 6.160 | 25.8% |
| FSharp.Compiler.Service | 40.120 | 30.246 | 24.6% |
| FsToolkit.ErrorHandling | 2.414 | 1.889 | 21.7% |
| Oxpecker | 1.240 | 0.636 | 48.7% |
| Nu | 16.663 | 12.858 | 22.8% |
| FsAutoComplete | 6.429 | 5.002 | 22.2% |
<!-- /generated:framework -->

*Median managed allocation per compilation, over 12 runs per version.*

![Actual allocation in GB on .NET Framework, comparing the released compiler generation with source-built F# 11 for all six workloads.](runtime-allocation.svg)

*Bar widths are scaled separately for each project.*

<!-- generated:framework-caveat -->
The source-built compiler allocates **21.7%-48.7% less on .NET Framework**. Peak RAM does not uniformly improve here either: FCS rises from 4290.0 to 5150.0 MB.
<!-- /generated:framework-caveat -->

This is **x64 FCS-hosted compilation**, not a measurement of Visual Studio or a 32-bit compiler. Keep it separate from the first table because the Core target framework and IL/R2R packaging differ. The additional runtime controls remain in the [full dataset](../framework/README.md).

## Less managed heap retained after project checking

We checked the FsAutoComplete server/Core/Logging graph and the Oxpecker/ViewEngine graph, then measured the managed heap with the checker and project results still alive.

<!-- generated:ide -->
| Project graph | Old MB | RC1 MB | New MB | New vs old: less retained memory |
| --- | ---: | ---: | ---: | ---: |
| FsAutoComplete | 571.1 | 488.7 | 398.6 | 30.2% |
| Oxpecker | 113.2 | 88.5 | 78.5 | 30.6% |
<!-- /generated:ide -->

*Managed heap after a full GC; 12 runs per version.*

![Retained managed heap in MB after a full GC: released SDK 10, RC1 and source-built F# 11. Both project graphs retain less memory. All bars share a zero-based MB axis.](ide-retained-heap.svg)

<!-- generated:portable-ide -->
On .NET Framework, retained managed memory also falls by **30.1%-30.6%**. [Detailed IDE measurements](../framework/ide-matrix.csv).
<!-- /generated:portable-ide -->

[Sharing imported assemblies across projects](https://github.com/dotnet/fsharp/pull/20296) avoids keeping separate copies of imported assembly structures for each project. This helps explain the result, though the measurements cover all changes together. An editor or language server must load the newer FCS package to benefit; compiling that tool with a newer SDK alone does not replace its FCS dependency.

## Fewer closure allocations in your F# code

These improvements are not only for the compiler. [Inlining higher-order List/Array functions](https://github.com/dotnet/fsharp/pull/20422) lets the compiler inline a lambda instead of allocating a closure for it. For example, this fold captures `discount`:

```fsharp
let discountedTotal discount prices =
    prices |> List.fold (fun total price -> total + price * (100 - discount) / 100) 0
```

The newer compiler/Core pair can eliminate that per-call closure allocation. [Partial-application closure elimination](https://github.com/dotnet/fsharp/pull/20487) extends the benefit to partially applied functions. Recompile with the newer compiler and FSharp.Core to benefit in your own projects.

We measured operations from cart pricing, access rules, telemetry and nested group folds: **old (SDK 10.0.100) versus new (source-built F# 11)**. The old applications run on .NET 10; new ones run on .NET 11 RC1.

<!-- generated:programs -->
| Operation | Old (SDK 10.0.100) B/op | New (source-built) B/op |
| --- | ---: | ---: |
| Cart-price List.fold | 24 | 0 |
| Access-rule exists / forall | 48 | 0 |
| Weighted Array.fold / fold2 | 48 | 0 |
| Partially applied Option.map | 0 | 0 |
| Nested group folds | 48 | 0 |
| Filtering and mapping | 146 | 146 |
| Escaping closure | 24 | 24 |
| Non-capturing lambda | 0 | 0 |
<!-- /generated:programs -->

*List and array operations use four-element collections. Median bytes allocated per operation across three launches.*

Four operations reach **0 B/op** with the newer compiler/Core pair. The escaping closure and filter/map pipeline still allocate; Option.map and the non-capturing lambda were already allocation-free in this comparison.

See [all tested collection sizes and launch ranges](../third-wave/program-matrix.csv) for the complete results.

<!-- generated:mechanism -->
The [generated IL inventories](../third-wave/programs/) show function-object construction sites falling from **19 to 11** between old and new output. These are static sites, not allocations per call. Separately collected [Oxpecker compiler profiles](../third-wave/profiles/) estimate generated-function allocation falling from **121.0 to 47.6 MB**. Those are weighted sampled bytes, classified by resolved `FSharpFunc` inheritance rather than just `@` in a name; the accepted traces report zero lost events.
<!-- /generated:mechanism -->

## Contributing changes

This is the work behind the compiler and FSharp.Core improvements, in merge order. Results compare complete compiler/Core versions, not individual PRs; the list also includes supporting fixes and build work.

<!-- generated:contributions -->
These **49 PRs** are in the measured source snapshot, ordered by merge time. Each distinct change is listed separately; the cut-line marks RC1's source boundary.

- **Aug 12** - [#20090](https://github.com/dotnet/fsharp/pull/20090): Import: Don't walk non-F# assemblies when labelling trait constraint sources (auduchinok).
- **Aug 12** - [#20088](https://github.com/dotnet/fsharp/pull/20088): Avoid per-instance lock object in InterruptibleLazy and DelayInitArrayMap (auduchinok).
- **Aug 12** - [#20092](https://github.com/dotnet/fsharp/pull/20092): IL: add ILPreNamespace, make ILPreTypeDef creation lazy (auduchinok).
- **Aug 13** - [#20250](https://github.com/dotnet/fsharp/pull/20250): IL: fix leaking binary view (auduchinok).
- **Aug 13** - [#20249](https://github.com/dotnet/fsharp/pull/20249): IL: use empty tables for members when possible (auduchinok).
- **Aug 19** - [#20254](https://github.com/dotnet/fsharp/pull/20254): IL: share ILCallingConv instances (auduchinok).
- **Aug 19** - [#20256](https://github.com/dotnet/fsharp/pull/20256): IL: cache C# extension methods per CCU (auduchinok).
- **Aug 19** - [#20244](https://github.com/dotnet/fsharp/pull/20244): Fix super-linear compilation of guarded shared-or active-pattern matches (T-Gro).
- **Aug 20** - [#20286](https://github.com/dotnet/fsharp/pull/20286): Make Entity's adhoc members list lazy (auduchinok).
- **Aug 24** - [#20301](https://github.com/dotnet/fsharp/pull/20301): IL: share the pickled references (auduchinok).
- **Aug 24** - [#20298](https://github.com/dotnet/fsharp/pull/20298): Name resolution: group C#-style extension members per 'open' and extended type (auduchinok).
- **Aug 24** - [#20259](https://github.com/dotnet/fsharp/pull/20259): IL: cache the ILTypeRef of a type def (auduchinok).

---

**RC1 cut-line: the 12 changes above are in RC1. The 37 below are in the RC2 source, headed for GA.**

- **Aug 26** - [#20285](https://github.com/dotnet/fsharp/pull/20285): Calculate Entity.PublicPath instead of storing (auduchinok).
- **Aug 26** - [#20337](https://github.com/dotnet/fsharp/pull/20337): Replace the stringified pattern-match memo key with a typed one (T-Gro).
- **Aug 27** - [#20255](https://github.com/dotnet/fsharp/pull/20255): IL: reuse the cached ILTypeRef in ILTypeInfo.FromType (auduchinok).
- **Aug 27** - [#20364](https://github.com/dotnet/fsharp/pull/20364): Unpickling: share one EntityRef per non-local reference row (auduchinok).
- **Aug 27** - [#20296](https://github.com/dotnet/fsharp/pull/20296): Import: share assembly CCUs between projects (auduchinok).
- **Aug 28** - [#20348](https://github.com/dotnet/fsharp/pull/20348): Four profiler-guided hot-path wins from self-build tracing (T-Gro).
- **Aug 28** - [#20350](https://github.com/dotnet/fsharp/pull/20350): Avoid Choice allocation in fslib entity/val-ref equality (T-Gro).
- **Aug 28** - [#20351](https://github.com/dotnet/fsharp/pull/20351): Reduce Detuple usage-analysis allocation with a mutable Dictionary (T-Gro).
- **Aug 31** - [#20354](https://github.com/dotnet/fsharp/pull/20354): Avoid redundant FreeVars record allocation for local vals (T-Gro).
- **Aug 31** - [#20363](https://github.com/dotnet/fsharp/pull/20363): Fuse the optimizer inlining copy + type-instantiation passes (T-Gro).
- **Aug 31** - [#20367](https://github.com/dotnet/fsharp/pull/20367): Inline TryD to remove constraint-solver closure allocations (T-Gro).
- **Aug 31** - [#20368](https://github.com/dotnet/fsharp/pull/20368): eliminate per-call closure in StackGuard.Guard via InlineIfLambda (T-Gro).
- **Sep 1** - [#20287](https://github.com/dotnet/fsharp/pull/20287): IL: hold custom attributes in fields rather than a union case (auduchinok).
- **Sep 4** - [#20353](https://github.com/dotnet/fsharp/pull/20353): Cache IL method parameter attributes during overload resolution (T-Gro).
- **Sep 4** - [#20384](https://github.com/dotnet/fsharp/pull/20384): Avoid per-call closure allocation in type-hierarchy traversal (T-Gro).
- **Sep 8** - [#20385](https://github.com/dotnet/fsharp/pull/20385): Inline the free-variable typar foldBacks (T-Gro).
- **Sep 9** - [#20447](https://github.com/dotnet/fsharp/pull/20447): Remove per-call closure allocations in post-inference checks (T-Gro).
- **Sep 9** - [#20423](https://github.com/dotnet/fsharp/pull/20423): Eliminate closure allocations in well-known-attribute queries (T-Gro).
- **Sep 9** - [#20415](https://github.com/dotnet/fsharp/pull/20415): Make List.vMapFold inline to drop the nullness-import closure (T-Gro).
- **Sep 9** - [#20372](https://github.com/dotnet/fsharp/pull/20372): Eliminate closure allocations in List.mapq and List.lengthsEqAndForall2 (T-Gro).
- **Sep 9** - [#20349](https://github.com/dotnet/fsharp/pull/20349): Reverse instead of sort already-ordered branch fixups in IL writer (T-Gro).
- **Sep 10** - [#20374](https://github.com/dotnet/fsharp/pull/20374): Inline the static-abstract interface-constraint predicate (T-Gro).
- **Sep 10** - [#20437](https://github.com/dotnet/fsharp/pull/20437): Drop the per-call closure in generic type-argument codegen (T-Gro).
- **Sep 10** - [#20421](https://github.com/dotnet/fsharp/pull/20421): Drop accessibility and attribute-scan closures via ListInline (T-Gro).
- **Sep 10** - [#20426](https://github.com/dotnet/fsharp/pull/20426): Avoid per-call FSharpFunc closure in remapVal member-info remap (T-Gro).
- **Sep 11** - [#20487](https://github.com/dotnet/fsharp/pull/20487): Eliminate per-call closure for InlineIfLambda partial applications (T-Gro).
- **Sep 16** - [#20422](https://github.com/dotnet/fsharp/pull/20422): Inline List/Array higher-order functions and adapt function arguments (T-Gro).
- **Sep 16** - [#20388](https://github.com/dotnet/fsharp/pull/20388): Share the empty-array singleton for zero-length Array results (T-Gro).
- **Sep 17** - [#20490](https://github.com/dotnet/fsharp/pull/20490): Name resolution: CheckIWSAM only needs the intrinsic methods (auduchinok).
- **Sep 17** - [#20489](https://github.com/dotnet/fsharp/pull/20489): IL: map the short-lived metadata-only PE reader (auduchinok).
- **Sep 17** - [#20486](https://github.com/dotnet/fsharp/pull/20486): IL: intern the attributes and type references read from metadata (auduchinok).
- **Sep 17** - [#20481](https://github.com/dotnet/fsharp/pull/20481): FCS: fix races that made the background builder repeat work (auduchinok).
- **Sep 17** - [#20494](https://github.com/dotnet/fsharp/pull/20494): Typed tree: create a type's augmentation on first use (auduchinok).
- **Sep 17** - [#20261](https://github.com/dotnet/fsharp/pull/20261): IL: fix the per-reader string cache sizing (auduchinok).
- **Sep 18** - [#20571](https://github.com/dotnet/fsharp/pull/20571): Stabilize OptimizeClosureIfNotInlined for F# 11 (T-Gro).
- **Sep 18** - [#20555](https://github.com/dotnet/fsharp/pull/20555): Ship net10.0 FSharp.Core with the SDK tools (T-Gro).
- **Sep 21** - [#20506](https://github.com/dotnet/fsharp/pull/20506): Support multithreaded MSBuild in F# build tasks (T-Gro).

The experimental-branch merges [#20424](https://github.com/dotnet/fsharp/pull/20424), [#20425](https://github.com/dotnet/fsharp/pull/20425), [#20439](https://github.com/dotnet/fsharp/pull/20439) are **absent from the selected mainline ancestry** and are not credited as shipped here. [#20506](https://github.com/dotnet/fsharp/pull/20506) adds MSBuild concurrency support, but these serial compiler-only runs do **not** measure its benefit.

The [full PR inventory](../third-wave/contributions.csv), [VMR audit](../third-wave/vmr-audit.json) and [exact source selection](../third-wave/experiment.json) retain the release boundaries. The later compiler source is `9cd6167a7265ce7264b22719503b7dfa9eb8f83c`; the pinned RC2 and main production tree is `7904104d6778df42799007d3e4d6e19093144325`.
<!-- /generated:contributions -->

## What these measurements do and do not establish

Measurements ran serially on a shared Windows VM. **Timing is descriptive, not a speedup headline:** host load was not continuously monitored.

<details>
<summary>Measurement boundaries, runtime policy and output qualifications</summary>

Compiler assembly loading precedes the allocation/time boundary. Peak RAM includes startup and is the native Windows working-set high-water mark through operation completion, not private virtual reservation or an occasional sample. The IDE graph model is a cold check, not a complete interactive session. Application suites use six warmup and 15 measured iterations per case per launch; launches, not pooled iterations, are the independent repetitions.

No campaign forces DATAS, GC budgets, tiering or PGO. Compiler hosts use server GC; applications use ordinary workstation-GC defaults. **.NET 11's default GC/DATAS policy is context, not an isolated explanation or an F# PR.** DATAS adapts server GC to application size and was already a default before .NET 11. Framework has no DATAS. The [historical fixed-runtime/DATAS controls](../article.md#the-runtime-matters-too-net-11-and-datas) stay separate.

<!-- generated:instrumentation -->
Allocation accounting is also explicit: modern .NET uses `GC.GetTotalAllocatedBytes(true)`; Framework uses `AppDomain.MonitoringTotalAllocatedMemorySize` for the compiler domain, including retired threads. Known-allocation calibration agreed within 2%. Framework monitoring-on/off controls have median paired wall changes from **-1.54% to +0.68%**, but individual peak-RAM changes reach **+37.35%**. This is not proof of zero instrumentation overhead. See [counter calibration and controls](../framework/README.md#boundaries-and-stability).
<!-- /generated:instrumentation -->

The portable experiment also preserves an important output caveat: for each payload, all six workloads emit identical DLLs on .NET 10 and .NET 11, **but Framework outputs are not bitwise identical**. Focused inspection found runtime-dependent compressed resources; new Core additionally has two method-body `tail.`-prefix differences and two observed Framework output hashes. These are not grounds for claiming whole-program semantic equivalence. The [output identities](../framework/output-identities.json) and [focused evidence](../framework/README.md#deployment-and-emitted-output-qualifications) retain those distinctions and the corrected missing-manifest warmup.

</details>

### Full evidence and reproduction

<!-- generated:evidence -->
| Separate campaign | Scalar observations (measured + warmup) | Application launch/cases | Measured application iterations |
| --- | ---: | ---: | ---: |
| [SDK / local R2R](../third-wave/README.md) | 336 (288 + 48) | 432 | 6480 |
| [Portable / Framework](../framework/README.md) | 672 (576 + 96) | 864 | 12960 |

The portable campaign additionally retains 36 instrumentation-control records. [SDK raw results](../third-wave/results.json) and [portable raw results](../framework/results.json) include observations, warmups and all metric distributions.
<!-- /generated:evidence -->

The [original campaign](../README.md#preserved-first-campaign), [frozen inputs](../inputs/), [compatibility record](../compatibility.json), [SDK build provenance](../third-wave/provenance.json) and [portable provenance](../framework/provenance.json) remain unchanged. Original application reports are retained losslessly in each campaign's `programs` directory; large traces and build logs live at the persistent artifact roots recorded in the provenance.

This article, its tables and all three SVGs can be regenerated without a compiler build or benchmark run. See [presentation reproduction](README.md) and [exact chart values with source hashes](chart-data.json).
