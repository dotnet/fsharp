# Three-payload F# 11 campaign

[The article](article.md) is the current table-first draft. This directory adds new measurements without replacing the first campaign's observations or its fixed-runtime/DATAS controls.

| Evidence | Coverage |
|---|---|
| [Compilation matrix](compilation-matrix.csv) | Six workloads, three arms, 12 measured runs per arm |
| [IDE matrix](ide-matrix.csv) | Two real source-reference graphs, 12 measured runs per arm |
| [Application matrix](program-matrix.csv) | Eight kernels, six lengths, three arms, three independent launches |
| [Results](results.json) | All 336 scalar records, 432 application launch/cases, quartiles and paired intervals |
| [Raw application reports](programs/) | Nine losslessly compressed original BenchmarkDotNet reports and IL inventories |
| [Profile reports](profiles/) | Three fresh startup-complete Oxpecker allocation traces, all passing the integrity gate |
| [Provenance](provenance.json) | Source/build/R2R identities, runtime and driver fingerprints, CLI equivalence |
| [PR inventory](contributions.csv) | RC1 and RC2 source-ancestry classifications for the original 52-PR survey |

The third arm is **local RC2-source R2R on .NET 11 RC1**. Its source equals the pinned VMR main production tree, but it is not an official RC2 SDK. SDK 10.0.100 stays on .NET 10.0.0; RC1 stays on its exact .NET 11 RC1 runtime. Measured children remove inherited `DOTNET_*` and `COMPlus_*` overrides, then set only host location and telemetry preference. No DATAS, GC-budget, PGO or tiering override is imposed.

## Inputs and preparation

Reuse the original campaign's frozen cases, input inventories, private SDK10 installation, C# collector, C# program driver and trace reader. Their preparation is documented in the [original harness](../../../tests/benchmarks/FSharp11/README.md). The scripts deliberately verify recorded fingerprints and refuse to overwrite observations. A new machine/campaign must record its own deliberately prepared artifacts rather than silently relabel existing data.

In the commands below, `$root` is the persistent artifact directory described in the provenance. Resolve repository paths before changing directories:

```powershell
$harness = (Resolve-Path '.\tests\benchmarks\FSharp11\ThirdWave').Path
$docs = (Resolve-Path '.\docs\fsharp11-performance\third-wave').Path
```

The selected VMR refs are immutable commits, not moving branch names:

```powershell
git init -q "$root\vmr11"
git -C "$root\vmr11" remote add origin https://github.com/dotnet/dotnet.git
git -C "$root\vmr11" config core.longpaths true
git -C "$root\vmr11" fetch --depth=1 --filter=blob:none origin `
    3551975be08744f0418857c5bed8ab1545c5dd47:refs/benchmarks/rc1
git -C "$root\vmr11" fetch --depth=1 --filter=blob:none origin `
    be46bdda4d6599b80dd4ccd89b2de49d96cbf36d:refs/benchmarks/rc2
git -C "$root\vmr11" fetch --depth=1 --filter=blob:none origin `
    0c804ec276a679143cdc4bc3391a1c2d0abfd69b:refs/benchmarks/main
git -C "$root\vmr11" archive --format=zip `
    --output="$root\fsharp-rc2-source.zip" 'refs/benchmarks/rc2:src/fsharp'
Expand-Archive "$root\fsharp-rc2-source.zip" "$root\fsharp-rc2-source"
```

Use a new preparation location for a rebuild; never rebuild a measured payload in place:

```powershell
& "$harness\BuildPayload.ps1" -PerformanceRoot $root
& "$harness\VerifyPayload.ps1" -PerformanceRoot $root
```

The build is serial. Proto uses the installed exact RC1 SDK. Release then uses the new bootstrap, real signatures, optimization and the newly built Core, without `BUILDING_WITH_LKG`. The only source-tree overlay supplies build/output paths; production source is not edited. The repository's in-tree Core reference is retained when compiling FCS; packaging substitutes the freshly built net10.0 Core, as selected for SDK tools by #20555. The packaging project produces non-composite R2R with one crossgen2 worker and no custom F# training profile.

Verification compares all 717 production files with the VMR Git tree, allowing normal Git text line endings, then checks target frameworks, optimization flags, IL/R2R MVID equality and real R2R headers. Logs and binlogs remain in `$root`. The initial packaging attempt detected duplicate Core XML documentation; the corrected build includes only the selected Core variant and passed a clean rebuild.

## Scalar collection

Do not build, profile, or run another experiment while collecting a cohort:

```powershell
foreach ($case in 'oxpecker', 'fsharp-core', 'fsharp-compiler-service',
    'fstoolkit', 'nu', 'fsautocomplete') {
    & "$harness\Measure.ps1" -PerformanceRoot $root -CasePath "$root\case-$case.json"
}
foreach ($case in 'oxpecker-ide', 'fsautocomplete-ide') {
    & "$harness\Measure.ps1" -PerformanceRoot $root -CasePath "$root\case-$case.json" -Operation check
}
```

Rounds -2 and -1 are warmups; rounds 0 through 11 are measured. The six orders ABC/BCA/CAB/CBA/BAC/ACB repeat twice. `-StartRound` and `-EndRound` support explicit cohort continuation at round boundaries; existing outputs are never silently skipped or replaced.

The unchanged collector uses all-thread allocated bytes, per-generation GC deltas, process CPU time and elapsed operation time. Windows process-memory counters retain the lifetime maximum through the final snapshot. Compiler setup is outside the operation allocation/time boundary but inside the memory high-water mark. IDE retained-heap probes occur after checking and are not counted as compiler-operation allocation.

## Application collection

The source is unchanged `Kernels.fs`; each arm uses its matching Core and default F# language version, against the same net10.0 API references. The existing C# driver is reused byte-for-byte. Its historical `FixedRuntime` job label does not select a runtime; the outer host invocation and each launch's recorded runtime do.

```powershell
& "$harness\RunPrograms.ps1" -PerformanceRoot $root -ValidateOnly
& "$harness\RunPrograms.ps1" -PerformanceRoot $root -SkipBuild -Launch 0 -Arms sdk10,sdk11rc1,vmr-rc2-r2r
& "$harness\RunPrograms.ps1" -PerformanceRoot $root -SkipBuild -Launch 1 -Arms vmr-rc2-r2r,sdk10,sdk11rc1
& "$harness\RunPrograms.ps1" -PerformanceRoot $root -SkipBuild -Launch 2 -Arms sdk11rc1,vmr-rc2-r2r,sdk10
```

Each suite covers 48 cases. Every case gets six warmup and fifteen measured iterations, targeting 250 ms per iteration, with outliers retained. The reported ns/op is the median of the three launch-level medians, not a pooled fiction of 45 independent launches. Allocation values and launch ranges are preserved separately.

## CLI equivalence and profiles

Run these after unprofiled collection:

```powershell
& "$harness\ValidateCompiler.ps1" -PerformanceRoot $root
& "$harness\..\CollectProfile.ps1" -PerformanceRoot $root -CasePath "$root\case-oxpecker.json" `
    -Id third-wave-oxpecker-sdk10 -Arm sdk10
& "$harness\..\CollectProfile.ps1" -PerformanceRoot $root -CasePath "$root\case-oxpecker.json" `
    -Id third-wave-oxpecker-sdk11rc1 -Arm sdk11rc1
```

The compiler validator checks all six FCS/CLI DLL pairs and captures the new-payload trace. All profiles use the pinned collector, `dotnet-trace` 10.0.745401, a 512-MiB buffer, startup suspension, no rundown, and the same GC/loader/type/allocation provider. The unchanged TraceEvent reader resolves exact type identities and rejects invalid loss accounting. Weighted function allocation is not an exact closure count.

## Publication and validation

```powershell
node "$harness\Publish.mjs" $root $docs
node "$harness\Validate.mjs" $root $docs
```

The publisher regenerates all numeric tables and CSVs from actual observations. Paired bootstrap intervals use 10,000 resamples and seed 110100; arm medians and paired-ratio medians are distinct statistics. No shared-machine timing or peak-memory observation is discarded. The final command also verifies that the first campaign's measurements, input files and executable harness remain intact.
