namespace FSharp.Compiler.Benchmarks

open System.IO
open HistoricalBenchmark
open BenchmarkDotNet.Attributes

type SingleFileCompilerWithILCacheClearing(file, options) =
    inherit SingleFileCompiler(file, options)
    
    override this.Cleanup() =
        base.Cleanup()
        FSharp.Compiler.AbstractIL.ILBinaryReader.ClearAllILModuleReaderCache()

[<MemoryDiagnoser>]
type DecentlySizedStandAloneFileBenchmark() =
    inherit SingleFileCompilerBenchmarkBase(
        SingleFileCompiler(
            Path.Combine(__SOURCE_DIRECTORY__, "../decentlySizedStandAloneFile.fs"),
            OptionsCreationMethod.FromScript
        )
    )

