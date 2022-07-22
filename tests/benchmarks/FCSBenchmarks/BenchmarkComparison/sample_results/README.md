# Sample results
This folder contains a selection of results obtained by running the notebook located in `../runner.ipynb`

## Timings are not accurate
The results were gathered on a busy machine without much care taken to provide a reliable performance environment.
While this means the timing metrics are not very useful, the results can still be useful for two reasons:
* allocation data is quite accurate as it doesn't tend to depend much on the environment
* they work as examples that can make using the benchmarks easier

## Structure
Each directory contains 3 files output by `HistoricalBenchmark.Runner.runAll` function for a given selection of versions.

The three different version sets are:
- `sample_versions` - an arbitrary selection featuring all three types of versions supported
- `between_2_nuget_versions` - all commits between two NuGet versions of FCS
- `10_latest_nuget_versions` - 10 FCS NuGet versions between `v41.0.2` and ``v41.0.5-preview.22327.2`

## Observations
One thing that can be observed by looking at the results in `between_2_nuget_versions` is the noticeable increase of allocations in https://github.com/dotnet/fsharp/pull/11517

While this isn't necessarily something worth addressing, partly because later revisions show reduced allocations, it shows how running a historical benchmark can be potentially useful.

## Notes
- The metrics gathered here are very limited - much more data can be gathered from each benchmark.
- Such historical benchmarks run locally might be mostly deprecated once CI setup exists for performance tests that will provide the necessary historical information