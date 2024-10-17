# Smoke test for checking that some (faster) benchmarks work.
# The test is successful if all the benchmarks run and produce results.
# The actual numbers produced aren't accurate.

function Run {
    param (
        [string[]]$path
    )

    dotnet run `
        --project $path `
        -c Release `
        --no-build `
        --job Dry `
        --allCategories short `
        --stopOnFirstError
}

Run "FCSBenchmarks/BenchmarkComparison/HistoricalBenchmark.fsproj"
Run "FCSBenchmarks/CompilerServiceBenchmarks/FSharp.Compiler.Benchmarks.fsproj"
Run "FCSBenchmarks/FCSSourceFiles/FCSSourceFiles.fsproj"