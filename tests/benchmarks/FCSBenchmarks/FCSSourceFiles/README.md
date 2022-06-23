# FCSSourceFiles

## What is it

* A BDN-based benchmark
* Tests `FSharpChecker.ParseAndCheckFileInProject` on the `FSharp.Core` project.
* Uses locally available source code for both the code being type-checked (`FSharp.Core`) and the code being benchmarked (`FCS`).

## How to run it

1. Run `dotnet run -c release`
2. Output available on the commandline and in `BenchmarkDotNet.Artifacts/`

# Other

You can find this document under 'tests/benchmarks/FCSBenchmarks/FCSSourceFiles/README.md'.