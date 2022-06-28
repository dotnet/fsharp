module BenchmarkComparison

open System.IO
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open BenchmarkHelpers

[<MemoryDiagnoser>]
type TypeCheckingBenchmarkBase(compiler : BenchmarkSingleFileCompiler) =
    [<GlobalSetup>]
    member _.Setup() =
        compiler.Setup()

    [<Benchmark>]
    member _.Run() =
        compiler.Run()

    [<IterationCleanup>]
    member _.Cleanup() =
        compiler.Cleanup()

[<MemoryDiagnoser>]
type DecentlySizedStandAloneFileBenchmark() =
    inherit TypeCheckingBenchmarkBase(BenchmarkSingleFileCompiler(Path.Combine(__SOURCE_DIRECTORY__, "../decentlySizedStandAloneFile.fs")))

[<EntryPoint>]
let main args =
    BenchmarkSwitcher.FromAssembly(typeof<DecentlySizedStandAloneFileBenchmark>.Assembly).Run(args) |> ignore
    0