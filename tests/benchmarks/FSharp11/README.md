# F# 11 performance campaign harness

This Windows harness produced [the article and data](../../../docs/fsharp11-performance/README.md). It is deliberately serial and does not use the compiler built from the current checkout.

## Preparation

Use the exact archive URLs and hashes in [experiment.json](../../../docs/fsharp11-performance/experiment.json). Install SDK 10.0.100 privately at `$root\sdk10`; the measured candidate is installed at `C:\Program Files\dotnet\sdk\11.0.100-rc.1.26425.128`. Do not substitute 10.0.4xx or a newer RC1-like version.

`$root` is a persistent artifact directory, not the repository. Resolve `$harness` to this directory before changing location. Run private-SDK build commands from `$root` to avoid source repositories' differing `global.json` selections.

```powershell
$harness = (Resolve-Path '.\tests\benchmarks\FSharp11').Path
Set-Location $root
& "$root\sdk10\dotnet.exe" build "$harness\Collector.csproj" -c Release `
    -o "$root\collector" "-p:BaseIntermediateOutputPath=$root\collector-obj\"
```

Acquire the pinned repository snapshots using Git archives or GitHub codeload. Apply the exact common-source adjustments in [compatibility.json](../../../docs/fsharp11-performance/compatibility.json). Build the real root project in Release for the selected TFM before capturing anything. This restores dependencies, builds C# and F# project references, and generates compiler inputs.

The F# source snapshot requires `-p:BUILDING_USING_DOTNET=true -p:IgnoreMibc=true`. Core/FCS target netstandard2.0, FsToolkit net9.0, and the other compilation roots net10.0. Use the baseline SDK for preparation. The measured package sources were:

- `https://pkgs.dev.azure.com/dnceng/public/_packaging/dotnet-public/nuget/v3/index.json` for F# and FsToolkit preparation.
- `https://www.nuget.org/api/v2` for packages absent from that mirror and for the remaining workloads.

FsAutoComplete requires Paket 10.3.1. Its lock file keeps the same versions when changing the source URL to the official v2 feed. Pass `-p:PaketExePath=<private-paket.exe>` to its build/capture; use process-local `DOTNET_ROLL_FORWARD=Major` if the tool requires a runtime not installed locally. Do not set that for measured workers; their runtime roll-forward is explicitly disabled.

Example preparation:

```powershell
& "$root\sdk10\dotnet.exe" build $project -c Release -f $tfm `
    -p:NuGetAudit=false -p:TreatWarningsAsErrors=false `
    '-p:RestoreSources=https://www.nuget.org/api/v2' "-bl:$root\prepare.binlog"
```

The source snapshots, all final argv arrays, and content inventories are preserved with the article. Restore and build are outside the timed experiment.

## Compilation

```powershell
& "$harness\Capture.ps1" -PerformanceRoot $root -Project $project -Framework $tfm -Id $id
& "$harness\Measure.ps1" -PerformanceRoot $root -CasePath "$root\case-$id.json" `
    -ResultsDirectory "$root\complete-input-measurements" -Pairs 9 -Warmups 2
```

For Core/FCS capture, include `-Properties @('-p:BUILDING_USING_DOTNET=true','-p:IgnoreMibc=true')`. Capture runs the full Build target while skipping the compiler task, so `BeforeBuild` sources are not lost.

The runner refuses to overwrite an observation. Extend a cohort without repeating warmups:

```powershell
& "$harness\Measure.ps1" -PerformanceRoot $root -CasePath "$root\case-$id.json" `
    -ResultsDirectory "$root\complete-input-measurements" -StartPair 9 -Pairs 15 -Warmups 0
```

`Pairs` is the exclusive final pair index. Every worker uses an exact `--fx-version`, returns its own OS peak while alive, and records precise process-wide allocation deltas. A failed compile stops the campaign and retains logs.

## Controls and graph checks

Use `-Comparison compiler-on-runtime11` to compare compiler payloads on the same RC1 runtime, or `-Comparison datas` to compare RC1 with DATAS off/on. The published FCS controls have 15 pairs each.

```powershell
& "$harness\CaptureGraph.ps1" -PerformanceRoot $root -Project $project -Framework $tfm -Id "$id-ide"
& "$harness\Measure.ps1" -PerformanceRoot $root -CasePath "$root\case-$id-ide.json" `
    -ResultsDirectory "$root\complete-input-measurements" -Operation check
& "$harness\VerifyCli.ps1" -PerformanceRoot $root
```

Graph capture follows evaluated references, including their nearest TFMs and reference-assembly paths. It rejects multiple TFMs for one path rather than silently conflating them. The selected graphs each contain one TFM per project. Verification compares freshly emitted DLL hashes from FCS and the shipping CLI.

## Generated programs

`RunPrograms.ps1 -ValidateOnly` builds the fixed C# driver and both F# DLLs and checks semantic results. The driver uses BenchmarkDotNet 0.14.0 and the exact RC1 runtime. Its in-process toolchain benchmarks the prebuilt F# DLL; it does not rebuild that DLL through BenchmarkDotNet.

```powershell
& "$harness\RunPrograms.ps1" -PerformanceRoot $root -ValidateOnly
for ($launch = 0; $launch -lt 3; $launch++) {
    $order = if ($launch % 2 -eq 0) { @('sdk10', 'sdk11rc1') } else { @('sdk11rc1', 'sdk10') }
    & "$harness\RunPrograms.ps1" -PerformanceRoot $root -SkipBuild -Launch $launch -Arms $order
}
```

Each suite has 48 cases, six warmup iterations and fifteen measured iterations. State varies inside the timed calls. `--validate` checks all kernels/sizes against an independent implementation without benchmarking. `Programs.cs` also resolves generated types and records their IL `newobj` sites.

## Allocation profiles

Restore this directory's private tool manifest from this directory using the candidate SDK, then build `Profile.csproj` using the baseline SDK with an external intermediate directory:

```powershell
Set-Location $harness
dotnet tool restore --add-source 'https://www.nuget.org/api/v2' --ignore-failed-sources
Set-Location $root
& "$root\sdk10\dotnet.exe" build "$harness\Profile.csproj" -c Release `
    -o "$root\profile-reader" "-p:BaseIntermediateOutputPath=$root\profile-reader-obj\"
& "$harness\CollectProfile.ps1" -PerformanceRoot $root -CasePath "$root\case-oxpecker.json" `
    -Id oxpecker-allocation-sdk10 -Arm sdk10
& "$harness\CollectProfile.ps1" -PerformanceRoot $root -CasePath "$root\case-oxpecker.json" `
    -Id oxpecker-allocation-sdk11rc1 -Arm sdk11rc1
```

The trace provider includes sampled-object type logging, needed because allocation ticks expose short names. `Profile.cs` joins type/module events and resolves metadata tokens; it does not guess a closure from `@` alone. Nonzero loss accounting rejects the trace. Raw traces remain in `$root\profiles`; accepted summaries are copied into the article data.

## Regenerate the publication

```powershell
node "$harness\Summarize.mjs" "$root\complete-input-measurements" $publication `
    "$root\programs" "$root\profiles"
& "$harness\PublishEvidence.ps1" -PerformanceRoot $root -Destination $publication
node "$harness\Validate.mjs" $publication
```

`$publication` is the absolute path to `docs\fsharp11-performance`. Summarization preserves warmups separately, validates complete pairs and stable inputs/collector identity, writes raw program reports and matrices, and updates the main article tables. Evidence publication verifies that frozen source/reference bytes and SDK binaries still match their hashes.

This is not a universal benchmark service: the two toolchains and the original campaign's rejected-profile evidence are intentionally pinned. Start a separately identified campaign rather than overwriting these observations when changing versions, sources, flags, or collection policy. See the executed protocol for measurement boundaries and the shared-host timing qualification.
