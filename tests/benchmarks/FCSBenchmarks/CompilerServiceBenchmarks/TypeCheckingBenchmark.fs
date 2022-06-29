namespace FSharp.Compiler.Benchmarks

open Benchmark
open BenchmarkDotNet.Attributes
open BenchmarkHelpers

[<MemoryDiagnoser>]
type TypeCheckingBenchmark() =
    inherit SingleFileCompilerBenchmarkBase(
        SingleFileCompiler(
            decentlySizedStandAloneFile,
            OptionsCreationMethod.FromScript
        )
    )
