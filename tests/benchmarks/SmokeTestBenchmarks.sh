# Smoke test for checking that some (faster) benchmarks work.
# The test is successful if all the benchmarks run and produce results.
# The actual numbers produced aren't accurate.

run() {
    local path=$1
    dotnet run \
        --project $path \
        -c Release \
        --no-build \
        --job Dry \
        --allCategories short \
        --stopOnFirstError
}

run "FCSBenchmarks/BenchmarkComparison/HistoricalBenchmark.fsproj"
run "FCSBenchmarks/CompilerServiceBenchmarks/FSharp.Compiler.Benchmarks.fsproj"
run "FCSBenchmarks/FCSSourceFiles/FCSSourceFiles.fsproj"
