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

## Sample results

|                                 Method |        Job | UnrollFactor |           Mean |        Error |        StdDev |         Median |       Gen 0 |      Gen 1 |     Gen 2 |  Allocated |
|--------------------------------------- |----------- |------------- |---------------:|-------------:|--------------:|---------------:|------------:|-----------:|----------:|-----------:|
|                          SimplifyNames | DefaultJob |           16 |    17,221.4 us |    378.14 us |   1,097.04 us |    17,164.1 us |   1875.0000 |    31.2500 |         - |  11,654 KB |
|                            UnusedOpens | DefaultJob |           16 |       852.7 us |     16.96 us |      36.87 us |       852.0 us |    120.1172 |    37.1094 |         - |     736 KB |
|                     UnusedDeclarations | DefaultJob |           16 |       208.2 us |      6.65 us |      19.09 us |       202.7 us |     71.5332 |     3.6621 |         - |     438 KB |
|              ParsingCheckExpressionsFs | Job-CXFNSP |            1 |   255,107.0 us | 39,778.24 us | 117,287.03 us |   186,340.7 us |   4000.0000 |  1000.0000 |         - |  30,082 KB |
|                              ILReading | Job-CXFNSP |            1 | 1,256,653.6 us | 24,802.85 us |  48,958.41 us | 1,249,170.3 us | 102000.0000 | 31000.0000 | 2000.0000 | 671,507 KB |
| TypeCheckFileWith100ReferencedProjects | Job-CXFNSP |            1 |     6,541.1 us |    242.62 us |     700.00 us |     6,614.2 us |           - |          - |         - |   3,547 KB |
