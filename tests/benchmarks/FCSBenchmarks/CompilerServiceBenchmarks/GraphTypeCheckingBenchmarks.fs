module FSharp.Benchmarks.GraphTypeCheckingBenchmarks

open System.IO
open BenchmarkDotNet.Attributes
open FSharp.Compiler.CodeAnalysis
open FSharp.Test.ProjectGeneration

[<Literal>]
let FSharpCategory = "fsharp"

[<MemoryDiagnoser>]
[<BenchmarkCategory(FSharpCategory)>]
type GraphTypeCheckingBenchmarks() =

    let size = 250

    let somethingToCompile =
        File.ReadAllText(__SOURCE_DIRECTORY__ ++ "SomethingToCompile.fs")

    member val Benchmark = Unchecked.defaultof<_> with get, set

    [<ParamsAllValues>]
    member val GraphTypeChecking = true with get, set

    member this.setup(project) =
        let checker = FSharpChecker.Create()
        this.Benchmark <- ProjectWorkflowBuilder(project, checker = checker).CreateBenchmarkBuilder()
        saveProject project false checker |> Async.RunSynchronously

    // Each file depends on the previous one
    [<GlobalSetup(Target = "SingleDependentChain")>]
    member this.SingleDependentChain_Setup() =
        this.setup
            { SyntheticProject.Create() with
                SourceFiles =
                    [
                        for i in 0..size do
                            { sourceFile
                                  $"File%04d{i}"
                                  [
                                      if i > 0 then
                                          $"File%04d{i - 1}"
                                  ] with
                                ExtraSource = somethingToCompile
                            }
                    ]
                OtherOptions =
                    [
                        if this.GraphTypeChecking then
                            "--test:GraphBasedChecking"
                    ]
            }

    [<Benchmark>]
    member this.SingleDependentChain() = this.Benchmark { compileWithFSC }

    // No file has any dependency
    [<GlobalSetup(Target = "NoDependencies")>]
    member this.NoDependencies_Setup() =
        this.setup
            { SyntheticProject.Create() with
                SourceFiles =
                    [
                        for i in 0..size do
                            { sourceFile $"File%04d{i}" [] with
                                ExtraSource = somethingToCompile
                            }
                    ]
                OtherOptions =
                    [
                        if this.GraphTypeChecking then
                            "--test:GraphBasedChecking"
                    ]
            }

    [<Benchmark>]
    member this.NoDependencies() = this.Benchmark { compileWithFSC }
