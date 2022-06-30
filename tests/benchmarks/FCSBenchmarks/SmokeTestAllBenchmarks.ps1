# Smoke test for checking that all the benchmarks work
# The test is successful if all the benchmarks run and produce results.
# The actual numbers produced aren't accurate.

function Run {
    param (
        [string[]]$path
    )
    # Build the benchmark project itself but no dependencies - still fast, but reduces risk of not accounting for code changes
    dotnet build -c release --no-dependencies $path
    # Run the minimum the CLI API allows - ideally we would use ColdStart but that's not part of the CLI API
    # For API details see https://benchmarkdotnet.org/articles/guides/console-args.html
    dotnet run -c release --project $path --no-build -- --warmupCount 0 --iterationCount 1 --runOncePerIteration --filter *
}

Run "BenchmarkComparison/HistoricalBenchmark.fsproj"
Run "CompilerServiceBenchmarks/FSharp.Compiler.Benchmarks.fsproj"
Run "FCSSourceFiles/FCSSourceFiles.fsproj"