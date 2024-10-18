module FSharp.Compiler.Benchmarks.ReuseTypecheckingResultsBenchmarks

open System.IO
open BenchmarkDotNet.Attributes
open FSharp.Test.ProjectGeneration
open FSharp.Benchmarks.Common.Categories
open FSharp.Compiler.DiagnosticsLogger

[<MemoryDiagnoser>]
[<BenchmarkCategory(ShortCategory)>]
type ReuseTypecheckingResultsBenchmarks () =

    let somethingToCompile = 
        File.ReadAllText (__SOURCE_DIRECTORY__ ++ "SomethingToCompile.fs")

    member val Benchmark = Unchecked.defaultof<_> with get, set

    [<ParamsAllValues>]
    member val ReuseTypecheckingResults = true with get, set

    [<GlobalSetup>]
    member this.Setup() =
        let project = 
            { SyntheticProject.Create() with
                SourceFiles =
                    [
                        { sourceFile "Blah1" [] with ExtraSource = somethingToCompile }
                        { sourceFile "Blah2" [ "Blah1" ] with ExtraSource = somethingToCompile }
                    ]
                OtherOptions =
                    [
                        if this.ReuseTypecheckingResults then
                            "--reusetypecheckingresults"
                    ]
            }

        this.Benchmark <- ProjectWorkflowBuilder(project).CreateBenchmarkBuilder()

    [<Benchmark>]
    member this.Compile() = 
        try
            let _ = this.Benchmark {
                compileWithFSC 
                compileWithFSC
            }
            true
        with 
            | :? StopProcessingExn -> false
