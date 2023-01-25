module FSharp.Benchmarks.BackgroundCompilerBenchmarks

open System.IO
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Order
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Text
open FSharp.Compiler.Diagnostics
open FSharp.Test.ProjectGeneration
open FSharp.Compiler.Symbols
open FSharp.Test.ProjectGeneration
open System
open System.IO
open System.Security.Cryptography
open System.Text
open System.Threading
open System.Threading.Tasks
open FSharp.Benchmarks.VsUtils


[<Literal>]
let FSharpCategory = "fsharp"


[<MemoryDiagnoser>]
[<BenchmarkCategory(FSharpCategory)>]
type FindAllReferences () =

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
[<BenchmarkCategory(FSharpCategory)>]
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

    let mutable benchmark : ProjectWorkflowBuilder = Unchecked.defaultof<_>

    [<ParamsAllValues>]
    member val UseGetSource = true with get,set

    [<ParamsAllValues>]
    member val UseChangeNotifications = true with get,set

    [<ParamsAllValues>]
    member val EmptyCache = true with get,set

    [<GlobalSetup>]
    member this.Setup() =
        benchmark <-
            ProjectWorkflowBuilder(
            project,
            useGetSource = this.UseGetSource,
            useChangeNotifications = this.UseChangeNotifications).CreateBenchmarkBuilder()

    [<IterationSetup>]
    member this.EditFirstFile_OnlyInternalChange() =
        if this.EmptyCache then
            benchmark.Checker.InvalidateAll()
            benchmark.Checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    [<Benchmark>]
    member this.ExampleWorkflow() =

        use _ = Activity.start "Benchmark" [
            "UseGetSource", this.UseGetSource.ToString()
            "UseChangeNotifications", this.UseChangeNotifications.ToString()
        ]

        let first = "File001"
        let middle = $"File%03d{size / 2}"
        let last = $"File%03d{size}"

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
        benchmark.DeleteProjectDir()


type Ordering =
    | Ascending = 1
    //| Descending = 2
    | Random = 3


type TaskScheduling =
    | Sequential = 1
    | SemaphoreA = 2
    | SemaphoreB = 3
    | AllAtOnce = 4


[<MemoryDiagnoser>]
[<ThreadingDiagnoser>]
[<Orderer(SummaryOrderPolicy.FastestToSlowest)>]
[<BenchmarkCategory(FSharpCategory)>]
type FindAllReferences_MultiProject () =

    let size = 30

    let somethingToCompile = File.ReadAllText (__SOURCE_DIRECTORY__ ++ "SomethingToCompileSmaller.fs")

    let createProject (name: string) =
        ({ SyntheticProject.Create(name) with
            SourceFiles = [
                sourceFile $"File_{name}_%03d{0}" [] |> addSignatureFile
                for i in 1..size do
                    { sourceFile $"File_{name}_%03d{i}" [$"File_{name}_%03d{i-1}"] with ExtraSource = somethingToCompile }
            ]
        }
        |> updateFile $"File_{name}_%03d{size / 2 - 1}" (addDependency $"File_{name}_000")
        |> updateFile $"File_{name}_%03d{size / 2    }" (addDependency $"File_{name}_000")
        |> updateFile $"File_{name}_%03d{size / 2 + 1}" (addDependency $"File_{name}_000"))

    let firstProject = createProject "First"

    let secondProject =
        { createProject "Second" with DependsOn = [firstProject] }
        |> updateFile $"File_Second_%03d{size / 2}" (addDependency $"File_First_000")

    let thirdProject =
        { createProject "Third" with DependsOn = [firstProject; secondProject] }
        |> updateFile $"File_Third_%03d{size / 2}" (addDependency $"File_First_000")
        |> updateFile $"File_Third_%03d{size / 2}" (addDependency $"File_Second_%03d{size}")

    let fourthProject =
        { createProject "Fourth" with DependsOn = [firstProject; secondProject] }
        |> updateFile $"File_Fourth_%03d{size / 2}" (addDependency $"File_Second_%03d{size / 2}")

    let projects = [firstProject; secondProject; thirdProject; fourthProject]

    let checker = FSharpChecker.Create(
        projectCacheSize = 5000,
        keepAllBackgroundResolutions = false,
        keepAllBackgroundSymbolUses = false,
        enableBackgroundItemKeyStoreAndSemanticClassification = true,
        enablePartialTypeChecking = true,
        enableParallelCheckingWithSignatureFiles = true,
        parallelReferenceResolution = true)

    let getModuleSymbol fileId project =
        async {
            let! results = checkFile fileId project checker
            let typeCheckResult = getTypeCheckResult results
            let moduleName = (project.Find fileId).ModuleName

            let symbolUse =
                typeCheckResult.GetSymbolUseAtLocation(
                    1,
                    moduleName.Length + project.Name.Length + 8,
                    $"module {project.Name}.{moduleName}",
                    [ moduleName ]
                )
                |> Option.defaultWith (fun () -> failwith "no symbol use found")

            return symbolUse.Symbol
        }

    let firstFileSymbol = firstProject |> getModuleSymbol "File_First_000" |> Async.RunSynchronously

    [<ParamsAllValues>]
    member val InterleaveProjects = false with get, set

    [<ParamsAllValues>]
    member val Scheduling = TaskScheduling.SemaphoreA with get, set

    //[<ParamsAllValues>]
    member val EmptyCache = false with get, set

    [<ParamsAllValues>]
    member val FileOrdering = Ordering.Random with get, set

    member this.CreateTasksForProject (symbol: FSharpSymbol) (project: SyntheticProject) =
        let sort =
            match this.FileOrdering with
            | Ordering.Ascending -> Seq.sort
            //| Ordering.Descending -> Seq.sortDescending
            | Ordering.Random -> Seq.sortBy (fun (f: string) -> f |> Encoding.ASCII.GetBytes |> (MD5.Create().ComputeHash) |> Convert.ToHexString)
            | _ -> failwith "invalid ordering"

        let options = project.GetProjectOptions checker
        let files = sort options.SourceFiles

        seq { for file in files -> checker.FindBackgroundReferencesInFile(file, options, symbol, fastCheck = true) }

    [<IterationSetup>]
    member this.IterationSetup() =
        if this.EmptyCache then
            checker.InvalidateAll()
            checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    [<GlobalSetup>]
    member _.Setup() =
        async {
            for project in projects do
                do! saveProject project true checker
                let! results = checker.ParseAndCheckProject(project.GetProjectOptions checker)

                if not (Array.isEmpty results.Diagnostics) then
                    failwith $"Project {project.Name} failed initial check: \n%A{results.Diagnostics}"

        } |> Async.RunSynchronously

    [<Benchmark>]
    member this.FindAllReferences_ToSymbolFromFirstProject() =
        let ct = CancellationToken.None
        backgroundTask {
            let tasks = projects |> Seq.map (this.CreateTasksForProject firstFileSymbol)

            let flattenedTasks =
                if this.InterleaveProjects then
                    interleave tasks
                else
                    Seq.collect id tasks

            let! results =
                match this.Scheduling with
                | TaskScheduling.Sequential ->
                    flattenedTasks
                    |> Async.Sequential
                    |> StartAsyncAsTask ct

                | TaskScheduling.SemaphoreA ->
                    flattenedTasks
                    |> ParallelProcessAsyncsA ct

                | TaskScheduling.SemaphoreB ->
                    flattenedTasks
                    |> ParallelProcessAsyncsB ct

                | TaskScheduling.AllAtOnce ->
                    flattenedTasks
                    |> Seq.map (fun task -> Task.Run<seq<FSharp.Compiler.Text.range>>(fun _ -> task |> StartAsyncAsTask ct))
                    |> Task.WhenAll

                | _ -> failwith "invalid scheduling"

            let results = Seq.collect id results
            let expectedResults = 8
            if Seq.length results <> expectedResults
                then failwith $"Expected {expectedResults} results, got {Seq.length results}\n%A{results}"
        }

    [<GlobalCleanup>]
    member _.Cleanup() =
        projects |> Seq.iter deleteProjectDir
