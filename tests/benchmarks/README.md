# Benchmarks

## What can be found here

This folder contains code and scripts used for running a selection of performance benchmarks.

### How the tests are used

The existing benchmarks are designed for on-demand local runs, to guide developers in performance improvement efforts and provide limited information during code change discussions.
Each of them assesses a slightly different use case and is run in a different way.

Since there is currently no dedicated hardware setup for running benchmarks in a highly accurate fashion, the results obtained by running them locally have to be treated carefully.
Specifically results obtained on different hardware or in different environments should be treated differently.

Note that there are plans to update the performance testing infrastructure. More information can be found at the following links:
* https://github.com/dotnet/fsharp/discussions/12526
* https://github.com/dotnet/performance/issues/2457

### Jupyter notebooks

Some benchmarks are written using F# Notebooks that use the .NET Interactive kernel.
Those can be identified by the `.ipynb` extension.
For instruction on how to run them see https://fsharp.org/use/notebooks/.

### BenchmarkDotNet

Most of the benchmarks use [BenchmarkDotNet](https://benchmarkdotnet.org/) (BDN), a popular benchmarking library for .NET.
It helps avoid common benchmarking pitfalls and provide highly-accurate, repeatable results.

A BDN benchmark is an executable. To run it, simply run `dotnet run %BenchmarkProject.fsproj%` in the benchmark's directory.

### Writing a new benchmark

When adding a benchmark, consider:
* choosing an appropriate subdirectory
* adding a README with the following details:
* * what is being measured
* * how to run the benchmark, including any environment requirements
* * an example of the results it produces

For instructions on how to write a BDN benchmark see [DEVGUIDE](https://github.com/dotnet/fsharp/blob/main/DEVGUIDE.md).

## Other

You can find this document under 'tests/benchmarks/README.md'.
