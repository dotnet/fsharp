module FSharp.Benchmarks.ComputationExpressionBenchmarks

open System.IO
open BenchmarkDotNet.Attributes
open FSharp.Compiler.CodeAnalysis
open FSharp.Test.ProjectGeneration
open FSharp.Benchmarks.Common.Categories

[<MemoryDiagnoser>]
[<BenchmarkCategory(LongCategory)>]
type ComputationExpressionBenchmarks() =

    let mutable sourceFileName = ""
    [<Params("CE100xnest1.fs",
             "CE100xnest5.fs",
             "CE1xnest15.fs",
             // "CE100xnest10.fs" // enable if you have the spare time
             "CE200xnest5.fs",
             "CEwCO500xnest1.fs",
             "CEwCO100xnest5.fs")>]
    member public this.Source
        with get () = File.ReadAllText(__SOURCE_DIRECTORY__ ++ "ce" ++ sourceFileName)
        and set f = sourceFileName <- f

    [<ParamsAllValues>]
    member val EmptyCache = true with get,set

    member val Benchmark = Unchecked.defaultof<_> with get, set

    member this.setup(project) =
        let checker = FSharpChecker.Create()
        this.Benchmark <- ProjectWorkflowBuilder(project, checker = checker).CreateBenchmarkBuilder()
        saveProject project false checker |> Async.RunSynchronously

    [<IterationSetup(Targets = [| "CheckCE"; "CompileCE" |])>]
    member this.StartIteration() =
        if this.EmptyCache then
            this.Benchmark.Checker.InvalidateAll()
            this.Benchmark.Checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    [<GlobalSetup(Targets = [| "CheckCE"; "CompileCE" |])>]
    member this.SetupWithSource() =
        this.setup
            { SyntheticProject.Create() with
                SourceFiles =
                    [

                        { sourceFile "File" [] with
                            ExtraSource = this.Source
                        }
                    ]
                OtherOptions = []
            }

    [<Benchmark>]
    member this.CheckCE() =
        this.Benchmark { checkFile "File" expectOk }

    [<Benchmark>]
    member this.CompileCE() = this.Benchmark { compileWithFSC }
