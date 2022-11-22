module FSharp.Benchmarks.BackgroundCompilerBenchmarks


open System.IO
open BenchmarkDotNet.Attributes
open FSharp.Test.ProjectGeneration
open FSharp.Compiler.CodeAnalysis


[<Literal>]
let FSharpCategory = "fsharp"


[<MemoryDiagnoser>]
[<BenchmarkCategory(FSharpCategory)>]
type BackgroundCompilerBenchmarks () =

    let size = 50

    let somethingToCompile = File.ReadAllText (__SOURCE_DIRECTORY__ ++ "SomethingToCompile.fs")

    [<ParamsAllValues>]
    member val FastFindReferences = true with get,set

    [<ParamsAllValues>]
    member val EmptyCache = true with get,set

    member val Benchmark = Unchecked.defaultof<ProjectBenchmarkBuilder> with get, set

    member this.setup(project) =
        let checker = FSharpChecker.Create(
            enableBackgroundItemKeyStoreAndSemanticClassification = true
        )
        this.Benchmark <- ProjectBenchmarkBuilder.Create(project, checker) |> Async.RunSynchronously

    [<IterationSetup>]
    member this.EditFirstFile_OnlyInternalChange() =
        if this.EmptyCache then
            this.Benchmark.Checker.InvalidateAll()
            this.Benchmark.Checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    /// Only file at the top of the list has the reference
    [<GlobalSetup(Target="FindAllReferences_BestCase")>]
    member this.FindAllReferences_BestCase_Setup() =
        this.setup
            { SyntheticProject.Create() with
                SourceFiles = [
                    sourceFile $"File%03d{0}" [] |> addSignatureFile
                    for i in 1..size do
                        { sourceFile $"File%03d{i}" [$"File%03d{i-1}"] with ExtraSource = somethingToCompile }
                ]
            }

    [<Benchmark>]
    member this.FindAllReferences_BestCase() =
        this.Benchmark {
            findAllReferencesToModuleFromFile "File000" (expectNumberOfResults 3)
        }

    /// Few files in the middle have the reference
    [<GlobalSetup(Target="FindAllReferences_MediumCase")>]
    member this.FindAllReferences_MediumCase_Setup() =
        this.setup(
            { SyntheticProject.Create() with
                SourceFiles = [
                    sourceFile $"File%03d{0}" [] |> addSignatureFile
                    for i in 1..size do
                        { sourceFile $"File%03d{i}" [$"File%03d{i-1}"] with ExtraSource = somethingToCompile }
                ]
            }
            |> updateFile $"File%03d{size / 2 - 1}" (addDependency "File000")
            |> updateFile $"File%03d{size / 2    }" (addDependency "File000")
            |> updateFile $"File%03d{size / 2 + 1}" (addDependency "File000"))

    [<Benchmark>]
    member this.FindAllReferences_MediumCase() =
        this.Benchmark {
            findAllReferencesToModuleFromFile "File000" (expectNumberOfResults 6)
        }

    /// All files have the reference, have to check everything
    [<GlobalSetup(Target="FindAllReferences_WorstCase")>]
    member this.FindAllReferences_WorstCase_Setup() =
        this.setup
            { SyntheticProject.Create() with
                SourceFiles = [
                    sourceFile $"File%03d{0}" [] |> addSignatureFile
                    for i in 1..size do
                        { sourceFile $"File%03d{i}" [$"File000"] with ExtraSource = somethingToCompile }
                ]
            }

    [<Benchmark>]
    member this.FindAllReferences_WorstCase() =
        this.Benchmark {
            findAllReferencesToModuleFromFile "File000" (expectNumberOfResults (size + 2))
        }

    [<GlobalCleanup>]
    member this.Cleanup() =
        this.Benchmark.DeleteProjectDir()
