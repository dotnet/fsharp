namespace FSharp.Compiler.Benchmarks

open HistoricalBenchmark
open BenchmarkDotNet.Attributes
open Helpers

type SingleFileCompilerWithILCacheClearing(file, options) =
    inherit SingleFileCompiler(file, options)
    
    override this.Cleanup() =
        base.Cleanup()
        FSharp.Compiler.AbstractIL.ILBinaryReader.ClearAllILModuleReaderCache()

[<MemoryDiagnoser>]
type DecentlySizedStandAloneFileBenchmark() =
    inherit SingleFileCompilerBenchmarkBase(
        SingleFileCompiler(
            decentlySizedStandAloneFile,
            OptionsCreationMethod.FromScript
        )
    )

