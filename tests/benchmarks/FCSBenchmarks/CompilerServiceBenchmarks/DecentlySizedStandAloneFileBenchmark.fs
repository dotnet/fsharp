namespace FSharp.Compiler.Benchmarks

open HistoricalBenchmark
open BenchmarkDotNet.Attributes
open Helpers

[<MemoryDiagnoser>]
type DecentlySizedStandAloneFileBenchmark() =
    inherit SingleFileCompilerBenchmarkBase(
        SingleFileCompiler(
            decentlySizedStandAloneFile,
            OptionsCreationMethod.FromScript
        )
    )
