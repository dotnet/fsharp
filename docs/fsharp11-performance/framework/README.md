# The same compiler payload on three runtimes

**The allocation improvement also exists on .NET Framework.** For the six compilation workloads, the new compatible compiler/Core payload allocates 22-49% less than the old one on .NET Framework 4.8.1. The same payload comparison on .NET 10 and .NET 11 shows similar reductions. These are compiler-operation allocations, not peak RAM or a claim that compilation is uniformly faster.

This campaign complements, rather than replaces, the [three SDK/R2R payloads](../third-wave/README.md). Start with the [complete tables](tables.md); every workload and runtime is included.

| Evidence | Coverage |
|---|---|
| [Compilation matrix](compilation-matrix.csv) | Six workloads, six arms, 12 measured runs per cell |
| [IDE matrix](ide-matrix.csv) | Two production source-reference graphs, six arms, 12 measured runs per cell |
| [Application matrix](program-matrix.csv) | Eight kernels, six lengths, six arms, three launches |
| [Results](results.json) | 672 scalar observations, 36 instrumentation controls, 864 application launch/cases |
| [Original application reports](programs/) | 18 losslessly compressed BenchmarkDotNet reports, 12,960 measured iterations |
| [Provenance](provenance.json) | 218 frozen binary/configuration files, calibration, exact source/runtime identities and retained log hashes |
| [Output identities](output-identities.json) | Every emitted DLL hash and its rounds; no blanket cross-runtime equivalence claim |
| [Diagnostic evidence](evidence/) | Calibration, failed warmup, deployment replay and focused IL/resource comparisons |
| [Experiment](experiment.json) | Design, launch order, counts and qualifications |

## What the six arms mean

| Prefix / suffix | Exact identity |
|---|---|
| `old-` | NuGet FSharp.Compiler.Service **43.10.100** and FSharp.Core **10.0.100**, both netstandard2.0; the SDK 10.0.100 source lineage, not 10.0.400 |
| `new-` | Self-hosted **Release**, netstandard2.0 FCS/Core from F# `9cd6167a7265ce7264b22719503b7dfa9eb8f83c`, VMR `be46bdda4d6599b80dd4ccd89b2de49d96cbf36d` |
| `-framework` | x64 .NET Framework **4.8.1**, registry Release 533509, actual CLR **4.8.9345.0** |
| `-net10` | **10.0.0**, hosted by private SDK **10.0.100** |
| `-net11` | **11.0.0-rc.1.26425.128**, hosted by SDK **11.0.100-rc.1.26425.128** |

The new production source equals the previously pinned VMR main source. It is not a moving current-main build or an official RC2 SDK. All six arms use **IL-only compatible payloads**, not R2R or NGen. For each prefix, the FCS/Core DLLs are byte-identical across runtime arms. The C# collector has one source, built for net472 and net10.0; the net10.0 worker is identical across .NET 10 and .NET 11.

This is **x64 FCS-hosted compilation**, not an observation of Visual Studio, its process overhead, or a 32-bit desktop fsc executable. Compiler hosts use server GC and the classic host enables very-large-object support, matching relevant desktop fsc settings. Application hosts use normal workstation-GC defaults. Inherited `DOTNET_*` and `COMPlus_*` settings are cleared. The scripts do not force DATAS, heap budgets, affinity, tiering or PGO. Modern default DATAS is recorded as enabled; Framework has no DATAS. Runtime-to-runtime changes include runtime/JIT/GC and framework-library effects, not an isolated DATAS treatment.

## Boundaries and stability

Compilation uses the exact frozen sources, references, arguments and target frameworks from the earlier campaigns, including all documented compatibility edits. `FSharpChecker.Compile` runs through parse, check, optimization and emit. The IDE collector builds real FCS project references in dependency order, with one checker, cache size 200 and other defaults: Oxpecker/ViewEngine has two projects; FsAutoComplete server/Core/Logging has three. It is a cold graph-check model, not a complete editor session or incremental-edit latency benchmark.

Allocation, elapsed time, CPU user/kernel time and GC-count deltas cover the operation. Compiler assembly loading precedes that boundary. Native Windows counters capture process-lifetime peak working set and peak private commit through operation completion, including startup: there is no polling interval that can miss a brief peak. Peak working set means resident RAM, not reserved address space. IDE retention keeps checker, project options and results rooted, waits ten seconds, records resident RAM, forces full GC, and measures live managed heap. Retained managed heap is not total process RAM.

The modern allocation counter is `GC.GetTotalAllocatedBytes(true)`. The classic counter is `AppDomain.MonitoringTotalAllocatedMemorySize`: all threads in the current compiler domain, including retired threads, with monitoring enabled before FCS loads. It is not a measurement across arbitrary extra AppDomains. Four joined worker threads allocating 40,000 arrays account for the expected 41,920,000 bytes including x64 array headers within 2% on all six arms; counters remain monotonic after full GC. A touched 128-MiB transient buffer exercises memory accounting.

There are two warmups and 12 measured fresh processes per arm and workload. Six-treatment Williams ordering uses base indices `[0,1,5,2,4,3]`, shifted by round modulo six, for two complete measured blocks. Collection is serial. Medians, quartiles and extrema remain available for every metric. Percentage changes distinguish ratios of arm medians from medians of round-paired ratios; no confidence level is claimed for the new factorial. Shared-machine timing and peak-memory observations are retained, not filtered after seeing results.

Separate Framework monitoring-on/off controls use six pairs per payload for Oxpecker and three for FCS, alternating order. Median paired wall-time changes range from -1.54% to +0.68%; this small noisy sample does **not** prove zero instrumentation overhead. Peak-memory control dispersion is substantial (one new-Oxpecker pair reaches +37%). Allocation is therefore the main comparison, not a claim that ARM is free or that RAM is invariant.

Peak RAM actually increases for several compiler workloads. For example, classic FCS rises from **4,091 to 4,911 MiB**, while allocation falls from **37.364 to 28.169 GiB**. New Core also has a higher peak on all three runtimes. The two IDE graphs retain approximately 30-31% less managed heap on every runtime. These different outcomes must not be collapsed into an unqualified "less memory" claim.

## Application interpretation

The unchanged `Kernels.fs` is compiled once with each payload, targeting netstandard2.0, and the identical emitted DLL for that payload runs on all three runtimes. Setup creates collections outside the measured operation. Independent C# semantic checks cover all 48 kernel/length cases and multiple state inputs before each suite and during benchmark setup.

BenchmarkDotNet 0.14.0 uses in-process emit, six warmups, 15 measured iterations targeting 250 ms, and no outlier removal. Three launches per arm rotate runtime pairs, with old/new order AB/BA/AB. This is not full six-position balancing. Timing summaries use the median of three launch medians; iterations are not falsely treated as 45 independent launches.

At length four, Cart, Rules, Telemetry and Nested fall from 24/48 B per operation to **0 B** on all three runtimes. The escaping-callback positive control remains 24 B, filtering/mapping remains 146 B, and the non-capturing control remains zero. **Option is runtime-sensitive:** classic Framework improves from 48 to 24 B, whereas both compiler generations already measure zero on modern .NET. Zero describes the measured kernel operation, not setup or all F# programs.

## Deployment and emitted-output qualifications

The classic hosts require the embedded native `longPathAware` manifest to access unchanged paths of 260 characters or more. AppContext switches alone were insufficient. No frozen source paths were rewritten; the original failed pilot is retained privately.

One initial FsAutoComplete warmup failed because the old NuGet FCS payload lacked `default.win32manifest`. The classic CLR happened to supply a fallback; modern runtimes did not. The SDK10, new payload and Framework fallback copies are byte-identical. Deploying that resource beside the old FCS fixed the same invocation without source, argument or binary changes. The failed warmup is preserved separately and the complete FsAutoComplete cohort was restarted. `manifest_addendum` records this resource correction after the initial binary freeze.

All six workloads emit identical DLLs between .NET 10 and .NET 11 when using the same payload. **Framework outputs are not byte-identical to modern outputs.** Focused Oxpecker inspection finds identical 746 method bodies, decompressed F# signature/optimization resources and external PDB bytes per payload; compressed resources and resulting MVIDs differ with the runtime's deflate implementation.

Focused new-Core inspection finds matching decompressed resources and 9,601 of 9,603 method bodies. Two `ConvMutableToImmutable` callbacks have an additional `tail.` prefix in the captured Framework output. New-Core Framework primary rounds also contain two distinct DLL hashes: rounds 2 and 8 differ from the other 12 observations including warmups. Two controlled replays reproduce the dominant hash. All these records remain included. The experiment establishes successful compilation of identical inputs and independently checked application behavior; it does not establish bitwise reproducibility or semantic equivalence of every generated compiler-workload DLL.

## Reproduce a new campaign

Use a new artifact directory. Never rebuild frozen measured payloads in place. First prepare the pinned sources, references and SDK10 host using the [original harness](../../../tests/benchmarks/FSharp11/README.md), and build the pinned bootstrap/source tree using [third-wave preparation](../third-wave/README.md). Those existing input captures are prerequisites, not network-floating dependencies.

On this machine PowerShell 7, the exact SDK11 RC1 installation, x64 Framework 4.8.1 and `C:\Nuget` are used. Save build logs and binlogs in the new campaign directory. Resolve `$harness` before changing directories:

```powershell
$harness = (Resolve-Path '.\tests\benchmarks\FSharp11\Framework').Path
$docs = (Resolve-Path '.\docs\fsharp11-performance\framework').Path
$env:NUGET_PACKAGES = 'C:\Nuget'

dotnet build "$harness\OldPayload.csproj" -c Release -m:1 -nr:false `
    "-p:BaseIntermediateOutputPath=$root\framework\old-obj\" `
    -o "$root\framework\old-payload"

$s = "$root\fsharp-rc2-source"
Push-Location $s
$env:BUILDING_USING_DOTNET = 'false'
$env:DOTNET_ROOT = 'C:\Program Files\dotnet'
dotnet publish .\src\Compiler\FSharp.Compiler.Service.fsproj `
    -c Release -f netstandard2.0 -m:1 -nr:false `
    -p:BuildInParallel=false -p:DISABLE_ARCADE=true -p:FSHARPCORE_USE_PACKAGE=false `
    -p:IgnoreMibc=true -p:PublishReadyToRun=false `
    "-p:DotnetFscCompilerPath=$s\artifacts\Bootstrap\fsc\fsc.dll" `
    '-p:FscToolPath=C:\Program Files\dotnet' -p:FscToolExe=dotnet.exe `
    -p:SourceRevisionId=9cd6167a7265ce7264b22719503b7dfa9eb8f83c `
    -p:RestoreSources=https://www.nuget.org/api/v2 -p:NuGetAudit=false `
    -o "$root\framework\new-payload"
Pop-Location

Copy-Item "$root\sdk10\sdk\10.0.100\FSharp\default.win32manifest" `
    "$root\framework\old-payload\default.win32manifest"

foreach ($project in 'Collector', 'Programs') {
    $prefix = if ($project -eq 'Collector') { 'collector' } else { 'program' }
    dotnet build "$harness\$project.csproj" -c Release -m:1 -nr:false `
        "-p:BaseIntermediateOutputPath=$root\framework\$prefix-obj\" `
        "-p:BaseOutputPath=$root\framework\$prefix-bin\"
}
& "$harness\RunPrograms.ps1" -PerformanceRoot $root -Prepare -ValidateOnly
& "$harness\Measure.ps1" -PerformanceRoot $root -Case oxpecker `
    -Operation calibrate -StartRound 0 -EndRound 1 -Cohort calibration-frozen
& "$harness\Freeze.ps1" -PerformanceRoot $root
```

Each project has its own package lock: sharing `packages.lock.json` between three colocated projects overwrites dependencies. Restore uses the official NuGet v2 feed because the v3 endpoint failed TLS in this environment. `Programs.config` is separate from the compiler's `App.config`, preventing accidental server-GC application measurements. Do not pass a shared `-o` across target frameworks.

Collect one process at a time, without concurrent builds, profiles, benchmarks or analysis agents:

```powershell
foreach ($case in 'oxpecker', 'fsharp-core', 'fsharp-compiler-service',
    'fstoolkit', 'nu', 'fsautocomplete') {
    & "$harness\Measure.ps1" -PerformanceRoot $root -Case $case
}
foreach ($case in 'oxpecker-ide', 'fsautocomplete-ide') {
    & "$harness\Measure.ps1" -PerformanceRoot $root -Case $case -Operation check
}
& "$harness\RunPrograms.ps1" -PerformanceRoot $root -Launch 0 `
    -Arms old-framework,new-framework,old-net10,new-net10,old-net11,new-net11
& "$harness\RunPrograms.ps1" -PerformanceRoot $root -Launch 1 `
    -Arms new-net10,old-net10,new-net11,old-net11,new-framework,old-framework
& "$harness\RunPrograms.ps1" -PerformanceRoot $root -Launch 2 `
    -Arms old-net11,new-net11,old-framework,new-framework,old-net10,new-net10
& "$harness\MonitoringControls.ps1" -PerformanceRoot $root
```

This run measured old-Framework application launch 0 before the primary scalar campaign and the other suites afterward. The declared relative application order is preserved. Scripts refuse existing observations. Explicit round/subset continuation is available; existing rows must never be silently replaced.

## Regenerate these published results

```powershell
node "$harness\Publish.mjs" $root $docs
node "$harness\Validate.mjs" $root $docs
```

The publisher is for this completed, frozen evidence set, including its deployment addendum and diagnostic records. For a new campaign, record its own identities and diagnostics rather than copying these assertions or relabeling old data. Validation independently recalculates statistics and CSVs, checks recorded order and operation non-overlap, runtime/default-GC settings, workload/payload fingerprints, raw application iterations, allocation controls and emitted-output distributions. It also runs both historical validators, including all 1,727 unique frozen input files. Large binaries, source archives and logs remain at the provenance artifact root; compressed original reports and scalar evidence are committed here.
