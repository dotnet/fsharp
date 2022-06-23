# FCSBenchmarks

## What can be found here

Benchmarks that exercise performance of `FSharp.Compiler.Service`

## Testing performance of FSharp.Compiler.Service 
Performance of the compiler service is crucial for having good developer experience.
This includes compilation, type checking and any other parts of the API used by IDEs.

When making changes to the FCS source code, consider running some of these to assess impact of the changes on performance.

## Benchmark list
* `BenchmarkComparison/` - a Notebook-based benchmark that analyses performance of `FSharpChecker.ParseAndCheckFileInProject` on a single-file F# project. Supports comparing different revisions of the FCS codebase and fetching the code automatically.
* `CompilerServiceBenchmarks/` - 
* `FCSSourceFiles/` - analyses performance of `FSharpChecker.ParseAndCheckFileInProject` for the `FSharp.Core` project. Uses locally available source code.

All the above benchmarks use BenchmarkDotNet.

# Other

You can find this document under 'tests/benchmarks/FCSBenchmarks/README.md'.