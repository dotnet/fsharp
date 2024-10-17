# FCSSourceFiles

## What is it

* A BDN-based benchmark
* Tests `FSharpChecker.ParseAndCheckFileInProject` on the `FSharp.Core` project.
* Uses locally available source code for both the code being type-checked (`FSharp.Core`) and the code being benchmarked (`FCS`).

## How to run it

1. Run `dotnet run -c release`
2. Output available on the command line and in `BenchmarkDotNet.Artifacts/`

## Sample results

```
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22621
11th Gen Intel Core i7-1185G7 3.00GHz, 1 CPU, 8 logical and 4 physical cores
.NET SDK=6.0.320
  [Host]     : .NET 6.0.25 (6.0.2523.51912), X64 RyuJIT DEBUG
  DefaultJob : .NET 6.0.25 (6.0.2523.51912), X64 RyuJIT
```

|                     Method |    Mean |   Error |  StdDev |        Gen 0 |       Gen 1 |     Gen 2 | Allocated |
|--------------------------- |--------:|--------:|--------:|-------------:|------------:|----------:|----------:|
| ParseAndCheckFileInProject | 22.14 s | 0.543 s | 1.522 s | 1645000.0000 | 307000.0000 | 6000.0000 |     10 GB |
