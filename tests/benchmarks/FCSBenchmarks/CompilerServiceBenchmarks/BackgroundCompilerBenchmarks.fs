module FSharp.Benchmarks.BackgroundCompilerBenchmarks


open System.IO
open BenchmarkDotNet.Attributes
open FSharp.Test.ProjectGeneration
open FSharp.Compiler.CodeAnalysis
open FSharp.Test.ProjectGeneration.Internal
open FSharp.Compiler.Text


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

    member val Benchmark = Unchecked.defaultof<_> with get, set

    member this.setup(project) =
        let checker = FSharpChecker.Create(
            enableBackgroundItemKeyStoreAndSemanticClassification = true
        )
        this.Benchmark <- ProjectWorkflowBuilder(project, checker=checker).Benchmark()

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
            findAllReferencesToModuleFromFile "File000" this.FastFindReferences (expectNumberOfResults 3)
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
            findAllReferencesToModuleFromFile "File000" this.FastFindReferences  (expectNumberOfResults 6)
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
            findAllReferencesToModuleFromFile "File000" this.FastFindReferences (expectNumberOfResults (size + 2))
        }

    [<GlobalCleanup>]
    member this.Cleanup() =
        this.Benchmark.DeleteProjectDir()


[<MemoryDiagnoser>]
[<BenchmarkCategory(FSharpCategory)>]
type NoFileSystemCheckerBenchmark() =

    let size = 30

    let somethingToCompile = File.ReadAllText (__SOURCE_DIRECTORY__ ++ "SomethingToCompileSmaller.fs")

    let project =
        { SyntheticProject.Create() with
            SourceFiles = [
                sourceFile $"File%03d{0}" []
                for i in 1..size do
                    { sourceFile $"File%03d{i}" [$"File%03d{i-1}"] with ExtraSource = somethingToCompile }
            ]
        }

    [<ParamsAllValues>]
    member val UseGetSource = true with get,set

    [<ParamsAllValues>]
    member val UseChangeNotifications = true with get,set

    member this.Benchmark =
        ProjectWorkflowBuilder(
        project,
        useGetSource = this.UseGetSource,
        useChangeNotifications = this.UseChangeNotifications).Benchmark()

    [<Benchmark>]
    member this.ExampleWorkflow() =

        let first = "File001"
        let middle = $"File%03d{size / 2}"
        let last = $"File%03d{size}"

        let benchmark = this.Benchmark

        if this.UseGetSource && this.UseChangeNotifications then

            benchmark {
                updateFile first updatePublicSurface
                checkFile first expectSignatureChanged
                checkFile last expectSignatureChanged
                updateFile middle updatePublicSurface
                checkFile last expectSignatureChanged
                addFileAbove middle (sourceFile "addedFile" [first])
                updateFile middle (addDependency "addedFile")
                checkFile middle expectSignatureChanged
                checkFile last expectSignatureChanged
            }

        else

            benchmark {
                updateFile first updatePublicSurface
                saveFile first
                checkFile first expectSignatureChanged
                checkFile last expectSignatureChanged
                updateFile middle updatePublicSurface
                saveFile middle
                checkFile last expectSignatureChanged
                addFileAbove middle (sourceFile "addedFile" [first])
                saveFile "addedFile"
                updateFile middle (addDependency "addedFile")
                saveFile middle
                checkFile middle expectSignatureChanged
                checkFile last expectSignatureChanged
            }

    [<GlobalCleanup>]
    member this.Cleanup() =
        this.Benchmark.DeleteProjectDir()