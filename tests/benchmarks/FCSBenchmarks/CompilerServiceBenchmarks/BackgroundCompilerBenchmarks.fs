﻿module FSharp.Benchmarks.BackgroundCompilerBenchmarks

open System.IO
open BenchmarkDotNet.Attributes
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
open FSharp.Compiler.Diagnostics
open FSharp.Test.ProjectGeneration
open BenchmarkDotNet.Engines
open FSharp.Benchmarks.Common.Categories

[<MemoryDiagnoser>]
[<BenchmarkCategory(LongCategory)>]
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
            enableBackgroundItemKeyStoreAndSemanticClassification = true,
            captureIdentifiersWhenParsing = true
        )
        this.Benchmark <- ProjectWorkflowBuilder(project, checker=checker).CreateBenchmarkBuilder()

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
[<BenchmarkCategory(ShortCategory)>]
type ParsingBenchmark() =

    let mutable checker: FSharpChecker = Unchecked.defaultof<_>
    let mutable parsingOptions: FSharpParsingOptions = Unchecked.defaultof<_>

    let filePath = __SOURCE_DIRECTORY__ ++ ".." ++ ".." ++ ".." ++ ".." ++ "src" ++ "Compiler" ++ "Checking" ++ "CheckExpressions.fs"
    let source = File.ReadAllText filePath |> SourceText.ofString

    [<ParamsAllValues>]
    member val IdentCapture = true with get, set

    [<GlobalSetup>]
    member this.Setup() =
        checker <- FSharpChecker.Create(captureIdentifiersWhenParsing = this.IdentCapture)
        parsingOptions <- { (checker.GetParsingOptionsFromCommandLineArgs([]) |> fst) with SourceFiles = [| filePath |] }

    [<IterationSetup>]
    member _.IterationSetup() =
        checker.InvalidateAll()
        checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    [<Benchmark>]
    member _.ParseBigFile() =
        let result = checker.ParseFile(filePath, source, parsingOptions) |> Async.RunSynchronously

        if result.ParseHadErrors then
            failwith "ParseHadErrors"

[<MemoryDiagnoser>]
[<BenchmarkCategory(LongCategory)>]
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

    let mutable benchmark : ProjectWorkflowBuilder = Unchecked.defaultof<_>

    [<ParamsAllValues>]
    member val UseInMemoryDocuments = true with get,set

    [<ParamsAllValues>]
    member val EmptyCache = true with get,set

    [<GlobalSetup>]
    member this.Setup() =
        benchmark <-
            ProjectWorkflowBuilder(
            project,
            useGetSource = this.UseInMemoryDocuments,
            useChangeNotifications = this.UseInMemoryDocuments).CreateBenchmarkBuilder()

    [<IterationSetup>]
    member this.EditFirstFile_OnlyInternalChange() =
        if this.EmptyCache then
            benchmark.Checker.InvalidateAll()
            benchmark.Checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    [<Benchmark>]
    member this.ExampleWorkflow() =

        use _ = Activity.start "Benchmark" [
            "UseInMemoryDocuments", this.UseInMemoryDocuments.ToString()
        ]

        let first = "File001"
        let middle = $"File%03d{size / 2}"
        let last = $"File%03d{size}"

        if this.UseInMemoryDocuments then

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
        benchmark.DeleteProjectDir()



type TestProjectType =
    | DependencyChain = 1
    | DependentGroups = 2
    | ParallelGroups = 3


[<MemoryDiagnoser>]
[<ThreadingDiagnoser>]
[<SimpleJob(warmupCount=1,iterationCount=4)>]
[<BenchmarkCategory(LongCategory)>]
type TransparentCompilerBenchmark() =

    let size = 30

    let groups = 6
    let filesPerGroup = size / groups
    let somethingToCompile = File.ReadAllText (__SOURCE_DIRECTORY__ ++ "SomethingToCompileSmaller.fs")

    let projects = Map [

        TestProjectType.DependencyChain,
            SyntheticProject.Create("SingleDependencyChain", [|
                sourceFile $"File%03d{0}" []
                for i in 1..size do
                    { sourceFile $"File%03d{i}" [$"File%03d{i-1}"] with ExtraSource = somethingToCompile }
            |])

        TestProjectType.DependentGroups,
            SyntheticProject.Create("GroupDependenciesProject", [|
                for group in 1..groups do
                    for i in 1..filesPerGroup do
                        { sourceFile $"G{group}_F%03d{i}" [
                            if group > 1 then $"G1_F%03d{1}"
                            if i > 1 then $"G{group}_F%03d{i - 1}" ]
                            with ExtraSource = somethingToCompile }
            |])

        TestProjectType.ParallelGroups,
            SyntheticProject.Create("ParallelGroupsProject", [|
                for group in 1..groups do
                    for i in 1..filesPerGroup do
                        { sourceFile $"G{group}_F%03d{i}" [
                            if group > 1 then
                                for i in 1..filesPerGroup do
                                    $"G{group-1}_F%03d{i}" ]
                            with ExtraSource = somethingToCompile }
            |])
    ]

    let mutable benchmark : ProjectWorkflowBuilder = Unchecked.defaultof<_>

    member val UseGetSource = true with get,set

    member val UseChangeNotifications = true with get,set

    //[<ParamsAllValues>]
    member val EmptyCache = false with get,set

    [<ParamsAllValues>]
    member val UseTransparentCompiler = true with get,set

    [<ParamsAllValues>]
    member val ProjectType = TestProjectType.ParallelGroups with get,set

    member this.Project = projects[this.ProjectType]

    [<GlobalSetup>]
    member this.Setup() =
        benchmark <-
            ProjectWorkflowBuilder(
            this.Project,
            useGetSource = this.UseGetSource,
            useChangeNotifications = this.UseChangeNotifications,
            useTransparentCompiler = this.UseTransparentCompiler,
            runTimeout = 15_000).CreateBenchmarkBuilder()

    [<IterationSetup>]
    member this.EditFirstFile_OnlyInternalChange() =
        if this.EmptyCache then
            benchmark.Checker.InvalidateAll()
            benchmark.Checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    [<Benchmark>]
    member this.ExampleWorkflow() =

        use _ = Activity.start "Benchmark" [
            "UseTransparentCompiler", this.UseTransparentCompiler.ToString()
        ]

        let first = this.Project.SourceFiles[0].Id
        let middle = this.Project.SourceFiles[size / 2].Id
        let last = this.Project.SourceFiles |> List.last |> fun f -> f.Id

        benchmark {
            updateFile first updatePublicSurface
            checkFile first expectSignatureChanged
            checkFile last expectSignatureChanged
            updateFile middle updatePublicSurface
            checkFile last expectOk
            addFileAbove middle (sourceFile "addedFile" [first])
            updateFile middle (addDependency "addedFile")
            checkFile middle expectSignatureChanged
            checkFile last expectOk
        }

    [<GlobalCleanup>]
    member this.Cleanup() =
        benchmark.DeleteProjectDir()


// needs Giraffe repo somewhere nearby, hence benchmarks disabled
[<MemoryDiagnoser>]
[<ThreadingDiagnoser>]
[<SimpleJob(warmupCount=1,iterationCount=8)>]
type TransparentCompilerGiraffeBenchmark() =

    let mutable benchmark : ProjectWorkflowBuilder = Unchecked.defaultof<_>

    let rng = System.Random()

    let addComment s = $"{s}\n// {rng.NextDouble().ToString()}"
    let prependSlash s = $"/{s}\n// {rng.NextDouble()}"

    let modify (sourceFile: SyntheticSourceFile) =
        { sourceFile with Source = addComment sourceFile.Source }

    let break' (sourceFile: SyntheticSourceFile) =
        { sourceFile with Source = prependSlash sourceFile.Source }

    let fix (sourceFile: SyntheticSourceFile) =
        { sourceFile with Source = sourceFile.Source.Substring 1 }

    [<ParamsAllValues>]
    member val UseTransparentCompiler = true with get,set

    [<ParamsAllValues>]
    member val SignatureFiles = true with get,set

    member this.Project =
        let projectDir = if this.SignatureFiles then "Giraffe-signatures" else "Giraffe"

        let project = SyntheticProject.CreateFromRealProject (__SOURCE_DIRECTORY__ ++ ".." ++ ".." ++ ".." ++ ".." ++ ".." ++ projectDir ++ "src/Giraffe")
        { project with OtherOptions = "--nowarn:FS3520"::project.OtherOptions }

    [<GlobalSetup>]
    member this.Setup() =
        benchmark <-
            ProjectWorkflowBuilder(
            this.Project,
            useGetSource = true,
            useChangeNotifications = true,
            useTransparentCompiler = this.UseTransparentCompiler,
            runTimeout = 15_000).CreateBenchmarkBuilder()

    //[<Benchmark>]
    member this.ChangeFirstCheckLast() =

        use _ = Activity.start "Benchmark" [
            "UseTransparentCompiler", this.UseTransparentCompiler.ToString()
        ]

        benchmark {
            updateFile this.Project.SourceFiles.Head.Id modify
            checkFile (this.Project.SourceFiles |> List.last).Id expectOk
        }

    //[<Benchmark>]
    member this.ChangeSecondCheckLast() =

        use _ = Activity.start "Benchmark" [
            "UseTransparentCompiler", this.UseTransparentCompiler.ToString()
        ]

        benchmark {
            updateFile this.Project.SourceFiles[1].Id modify
            checkFile (this.Project.SourceFiles |> List.last).Id expectOk
        }

    // [<Benchmark>]
    member this.SomeWorkflow() =

        use _ = Activity.start "Benchmark" [
            "UseTransparentCompiler", this.UseTransparentCompiler.ToString()
        ]

        benchmark {
            updateFile "Json" modify
            checkFile "Json" expectOk
            checkFile "ModelValidation" expectOk
            updateFile "ModelValidation" modify
            checkFile "ModelValidation" expectOk
            updateFile "Xml" modify
            checkFile "Xml" expectOk
            updateFile "ModelValidation" modify
            checkFile "ModelValidation" expectOk

            updateFile "Core" break'
            checkFile "Core" expectErrors
            checkFile "Routing" (if this.SignatureFiles then expectOk else expectErrors)
            updateFile "Routing" modify
            checkFile "Streaming" (if this.SignatureFiles then expectOk else expectErrors)
            checkFile "EndpointRouting" (if this.SignatureFiles then expectOk else expectErrors)

            updateFile "Core" fix
            checkFile "Core" expectOk
            checkFile "Routing" expectOk
            checkFile "Streaming" expectOk
            checkFile "EndpointRouting" expectOk
        }