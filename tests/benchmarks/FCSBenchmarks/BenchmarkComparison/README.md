# BenchmarkComparison

## What is it

* A meta-benchmark that compares performance between versions of the FCS codebase for a single-file codebase.
* Notebook-based, see `runner.ipynb`.
* Specified versions of FCS are downloaded from GitHub, built and benchmarked.
* Individual runs involve running a BDN benchmark in a separate process.
* Each run tests `FSharpChecker.ParseAndCheckFileInProject` on a single file (see `../decentlySizedStandAloneFile.fs`).

## How to run it

To run a benchmark for a local FCS in the current codebase , run:

```dotnet run --project HistoricalBenchmark.fsproj -c Release --filter *```

To run a comparison use the `runner.ipynb` .NET notebook

## How it works
- `runner.ipynb` runs `HistoricalBenchmark.fsproj` for a selection of versions/commits/local codebases
- For each version:
- - For a commit-based run the codebase at a given revision is fetched from GitHub and built
- - `HistoricalBenchmark.fsproj` is built with custom MSBuild properties that reference FCS in a specific way
- - a BDN benchmark is run and outputs saved
- `runner.ipynb` then parses CSV results from all runs and plots a chart

## `HistoricalBenchmark` and backwards compatibility
Due to the requirement to run the same benchmark on older versions of the codebase and minor changes in the API, code in `HistoricalBenchmark` can be compiled in three different ways by adding one of the following DEFINEs:
- `SERVICE_13_0_0`
- `SERVICE_30_0_0`
- _none_ (default, uses latest API)

## Sample results

*TODO*

## Other

You can find this document under 'tests/benchmarks/FCSBenchmarks/BenchmarkComparison/README.md'.