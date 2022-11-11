module FSharp.Benchmarks.BackgroundCompilerBenchmarks


open System
open System.IO
open BenchmarkDotNet.Attributes
open FSharp.Compiler.Text
open FSharp.Test
open FSharp.Test.ProjectGeneration
open FSharp.Compiler.CodeAnalysis


[<Literal>]
let FSharpCategory = "fsharp"


[<MemoryDiagnoser>]
[<BenchmarkCategory(FSharpCategory)>]
type BackgroundCompilerBenchmarks () =

    let size = 100

    let somethingToCompile = File.ReadAllText (__SOURCE_DIRECTORY__ ++ "SomethingToCompile.fs")

    let project =
        { SyntheticProject.Create() with
            SourceFiles = [
                sourceFile $"File%03d{0}" [] |> addSignatureFile
                for i in 1..size do
                    { sourceFile $"File%03d{i}" [$"File%03d{i-1}"] with ExtraSource = somethingToCompile }
            ]
        }
        |> updateFile "File050" (addDependency "File000")
        //|> updateFile "File075" (addDependency "File000")
        //|> updateFile "File090" (addDependency "File000")

    let checker = FSharpChecker.Create(enableBackgroundItemKeyStoreAndSemanticClassification = true)
    let benchmark = ProjectBenchmarkBuilder.Create(project, checker) |> Async.RunSynchronously

    //[<IterationSetup(Targets = [| "FindAllReferences" |])>]
    //member _.EditFirstFile_OnlyInternalChange() =
    //    checker.InvalidateAll()
    //    checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    [<GlobalCleanup>]
    member _.Cleanup() =
        benchmark.DeleteProjectDir()

    [<Benchmark>]
    member _.FindAllReferences() =
        benchmark {
            findAllReferencesToModuleFromFile "File000" (expectNumberOfResults 4)
        }
