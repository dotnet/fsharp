# Assumes all the benchmark projects have already been built to save time

dotnet run -c release --project CompilerServiceBenchmarks/FSharp.Compiler.Benchmarks.fsproj -- --warmupCount 0 --maxIterationCount 1 --runOncePerIteration

dotnet run -c release --project FCSSourceFiles/FCSSourceFiles.fsproj -- --warmupCount 0 --maxIterationCount 1 --runOncePerIteration

dotnet run -c release --project BenchmarkComparison/run_current.fsproj -- --warmupCount 0 --maxIterationCount 1 --runOncePerIteration