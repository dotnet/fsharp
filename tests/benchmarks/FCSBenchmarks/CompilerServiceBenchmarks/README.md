# CompilerServiceBenchmarks

## What is it

* A selection of BDN benchmarks analyzing FCS performance
* Uses BDN's command line API

## How to run it

Running all benchmarks:
```dotnet run -c Release --filter *```

Running a specific benchmark:
```dotnet run -c Release --filter *ParsingCheckExpressionsFs*```

## Namespace-import benchmarks (lazy ILPreNamespace)

`NamespaceImportBenchmarks.fs` measures the effect of lazy namespace reading in the IL reader: a project
that references large **non-F#** assemblies (the BCL) but opens only a few namespaces should read and
retain less. F# assemblies are unpickled and do not exercise this path.

Startup time + transient allocation (BDN, `MemoryDiagnoser`):
```dotnet run -c Release --filter *NamespaceImportStartup*```
`NarrowImport` opens one namespace (where laziness should pay off); `WideImport` opens many (control —
should stay flat). Compare the `Mean` and `Allocated` columns across `main` and this branch.

Retained (live-heap) memory — MemoryDiagnoser only sees allocation *during* an op, not what survives, so
this is a separate standalone probe:
```dotnet run -c Release -- retained-memory```
It prints retained KB for Narrow and Wide. The win is un-opened namespaces never being *retained*, so
compare `Narrow` retained across `main` and this branch (and Narrow-vs-Wide within a branch).

To compare branches: build + run on `main`, note the numbers, `git checkout il-pre-namespace`, rebuild
and re-run, diff. (The `BenchmarkComparison/` project automates historical before/after runs if preferred.)

## DeclarationChecker benchmarks

`DeclarationCheckerBenchmarks.fs` measures single-file incremental checking and per-file live heap
retention for `DeclarationChecker` versus `FSharpChecker`:

* **`DeclarationCheckerBenchmarks`**: Uses a 60-declaration implementation module (`Bench.Declarations`)
  with downstream callers (`compute31`..`compute35` calling `compute30`). Compares warm single-file
  `FSharpChecker.ParseAndCheckFileInProject` (`BodyEdit_WholeFile_FSharpChecker`, baseline) against:
  * `BodyEdit_WholeFile_DeclarationChecker` — whole-file `DeclarationChecker.CheckFile` without
    materializing `TcResultsSink` (background signature checking).
  * `BodyEdit_Incremental_Cutoff` — `Parse`, `Split`, single-declaration `CheckDeclaration` with
    `SignatureFingerprint` cutoff (0 downstream declarations re-checked), and `FinishFile`.
  * `BodyEdit_Incremental_WithCheckResults` — same incremental check plus forcing `fileRes.CheckResults`
    to materialize full `FSharpCheckFileResults` from merged sinks.
* **`declaration-memory` probe (`DeclarationMemoryProbe`)**: Deterministic `GC.GetTotalMemory(true)`
  measurement comparing closed-file `FileSignature`, open-file `FileStart + DeclarationResult[]`, and
  `FSharpCheckFileResults` on the 60-declaration module after `TcImports` warmup.

Build and run with `BUILDING_USING_DOTNET=true`:
```powershell
$env:BUILDING_USING_DOTNET = "true"
dotnet build-server shutdown
dotnet build ..\..\..\..\src\FSharp.Build\FSharp.Build.fsproj -c Release --disable-build-servers
dotnet build ..\..\..\..\src\Compiler\FSharp.Compiler.Service.fsproj -c Release --disable-build-servers
dotnet build ..\..\..\FSharp.Test.Utilities\FSharp.Test.Utilities.fsproj -c Release --disable-build-servers
dotnet build ..\..\FSharp.Benchmarks.Common\FSharp.Benchmarks.Common.fsproj -c Release --disable-build-servers
dotnet build FSharp.Compiler.Benchmarks.fsproj -c Release --no-dependencies --disable-build-servers
dotnet run -c Release --no-build -- --filter *DeclarationCheckerBenchmarks* --warmupCount 3 --iterationCount 10
dotnet run -c Release --no-build -- declaration-memory
```

## Sample results

|                                 Method |        Job | UnrollFactor |           Mean |        Error |        StdDev |         Median |       Gen 0 |      Gen 1 |     Gen 2 |  Allocated |
|--------------------------------------- |----------- |------------- |---------------:|-------------:|--------------:|---------------:|------------:|-----------:|----------:|-----------:|
|                          SimplifyNames | DefaultJob |           16 |    17,221.4 us |    378.14 us |   1,097.04 us |    17,164.1 us |   1875.0000 |    31.2500 |         - |  11,654 KB |
|                            UnusedOpens | DefaultJob |           16 |       852.7 us |     16.96 us |      36.87 us |       852.0 us |    120.1172 |    37.1094 |         - |     736 KB |
|                     UnusedDeclarations | DefaultJob |           16 |       208.2 us |      6.65 us |      19.09 us |       202.7 us |     71.5332 |     3.6621 |         - |     438 KB |
|              ParsingCheckExpressionsFs | Job-CXFNSP |            1 |   255,107.0 us | 39,778.24 us | 117,287.03 us |   186,340.7 us |   4000.0000 |  1000.0000 |         - |  30,082 KB |
|                              ILReading | Job-CXFNSP |            1 | 1,256,653.6 us | 24,802.85 us |  48,958.41 us | 1,249,170.3 us | 102000.0000 | 31000.0000 | 2000.0000 | 671,507 KB |
| TypeCheckFileWith100ReferencedProjects | Job-CXFNSP |            1 |     6,541.1 us |    242.62 us |     700.00 us |     6,614.2 us |           - |          - |         - |   3,547 KB |
