# BenchmarkComparison

## What is it

- A meta-benchmark that compares performance between versions of the FCS codebase for a single file.
- Notebook-based, see `runner.ipynb`.
- Each run performs `FSharpChecker.ParseAndCheckFileInProject` on a single file (see `../decentlySizedStandAloneFile.fs`).

## How to run it

To run a benchmark for a local FCS in the current codebase you can run the benchmark directly:

```dotnet run --project HistoricalBenchmark.fsproj -c Release --filter *```

To run a comparison use the `runner.ipynb` .NET notebook

## How it works
- `runner.ipynb` runs `HistoricalBenchmark.fsproj` for a selection of versions/commits/local codebases.
- Individual runs involve running a BDN benchmark in a separate process (via `HistoricalBenchmark.fsproj`).
- For each version:
  1. For a commit-based run we fetch FCS at a given revision from GitHub into a temporary folder and build it via `build.cmd`
  2. `HistoricalBenchmark.fsproj` is built with custom MSBuild properties that reference FCS in a specific way (project/package or dll reference)
  3. We run `HistoricalBenchmark` which is a BDN benchmark
  4. `runner.ipynb` then parses CSV results from all runs and plots a chart

## `HistoricalBenchmark` and backwards compatibility
Due to the requirement to run the same benchmark on older versions of the codebase and minor changes in the API, code in `HistoricalBenchmark` can be compiled in three different ways by adding one of the following DEFINEs:
- `SERVICE_13_0_0`
- `SERVICE_30_0_0`
- _none_ (default, uses latest API)

As of now the minimum supported version of FCS is 13.0.0 

## Sample results

*TODO*

## Other

You can find this document under 'tests/benchmarks/FCSBenchmarks/BenchmarkComparison/README.md'.