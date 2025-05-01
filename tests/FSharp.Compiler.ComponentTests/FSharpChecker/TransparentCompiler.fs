[<Xunit.Collection(nameof FSharp.Test.NotThreadSafeResourceCollection)>]
module FSharpChecker.TransparentCompiler

open System.Collections.Concurrent
open System.Diagnostics
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.IO
open FSharp.Compiler.Text
open Internal.Utilities.Collections
open FSharp.Compiler.CodeAnalysis.TransparentCompiler
open Internal.Utilities.Library.Extras
open FSharp.Compiler.GraphChecking.GraphProcessing
open FSharp.Compiler.Diagnostics

open Xunit

open FSharp.Test
open FSharp.Test.ProjectGeneration
open FSharp.Test.ProjectGeneration.Helpers
open System.IO
open Microsoft.CodeAnalysis
open System
open System.Threading.Tasks
open System.Threading
open TypeChecks

open OpenTelemetry
open OpenTelemetry.Resources
open OpenTelemetry.Trace

let fileName fileId = $"File%s{fileId}.fs"

let internal recordAllEvents groupBy =
    let mutable cache : AsyncMemoize<_,_,_> option = None
    let events = ConcurrentQueue()

    let observe (getCache: CompilerCaches -> AsyncMemoize<_,_,_>) (checker: FSharpChecker) =
        cache <- Some (getCache checker.Caches)
        cache.Value.Event
        |> Event.map (fun (e, k) -> groupBy k, e)
        |> Event.add events.Enqueue

    let getEvents () =
        events |> List.ofSeq

    observe, getEvents

let getFileNameKey (_l, (f: string, _p), _) = Path.GetFileName f

 // TODO: currently the label for DependecyGraph cache is $"%d{fileSnapshots.Length} files ending with {lastFile}"
let getDependecyGraphKey (_l, _, _) = failwith "not implemented"

let internal recordEvents groupBy =
    let observe, getEvents = recordAllEvents groupBy

    let check key expected =
        let events = getEvents()
        let actual = events |> Seq.filter (fun e -> fst e = key) |> Seq.map snd |> Seq.toList
        printfn $"{key}: %A{actual}"
        Assert.Equal<JobEvent>(expected, actual)

    observe, check

#nowarn "57"

[<Fact>]
let ``Use Transparent Compiler`` () =

    let size = 20

    let project =
        { SyntheticProject.Create() with
            SourceFiles = [
                sourceFile $"File%03d{0}" []
                for i in 1..size do
                    sourceFile $"File%03d{i}" [$"File%03d{i-1}"]
            ]
        }

    let first = "File001"
    let middle = $"File%03d{size / 2}"
    let last = $"File%03d{size}"

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
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

[<Fact>]
let ``Parallel processing`` () =

    let project = SyntheticProject.Create(
        sourceFile "A" [],
        sourceFile "B" ["A"],
        sourceFile "C" ["A"],
        sourceFile "D" ["A"],
        sourceFile "E" ["B"; "C"; "D"])

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        checkFile "E" expectOk
        updateFile "A" updatePublicSurface
        saveFile "A"

        checkFile "E" expectSignatureChanged
    }

[<Fact>]
let ``Parallel processing with signatures`` () =

    let project = SyntheticProject.Create(
        sourceFile "A" [] |> addSignatureFile,
        sourceFile "B" ["A"] |> addSignatureFile,
        sourceFile "C" ["A"] |> addSignatureFile,
        sourceFile "D" ["A"] |> addSignatureFile,
        sourceFile "E" ["B"; "C"; "D"] |> addSignatureFile)

    //let cacheEvents = ConcurrentBag<_>()

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        //withChecker (fun checker -> checker.CacheEvent.Add cacheEvents.Add)
        checkFile "E" expectOk
        updateFile "A" updatePublicSurface
        checkFile "E" expectNoChanges
        regenerateSignature "A"
        regenerateSignature "B"
        regenerateSignature "C"
        regenerateSignature "D"
        regenerateSignature "E"
        checkFile "E" expectSignatureChanged
    }

let makeTestProject () =
    SyntheticProject.Create(
        sourceFile "First" [],
        sourceFile "Second" ["First"],
        sourceFile "Third" ["First"],
        { sourceFile "Last" ["Second"; "Third"] with EntryPoint = true })

let testWorkflow () =
    ProjectWorkflowBuilder(makeTestProject(), useTransparentCompiler = true)

[<Fact>]
let ``Edit file, check it, then check dependent file`` () =
    testWorkflow() {
        updateFile "First" breakDependentFiles
        checkFile "First" expectSignatureChanged
        checkFile "Second" expectErrors
    }

[<Fact>]
let ``Edit file, don't check it, check dependent file`` () =
    testWorkflow() {
        updateFile "First" breakDependentFiles
        checkFile "Second" expectErrors
    }

[<Fact>]
let ``Check transitive dependency`` () =
    testWorkflow() {
        updateFile "First" breakDependentFiles
        checkFile "Last" expectSignatureChanged
    }

[<Fact>]
let ``Change multiple files at once`` () =
    testWorkflow() {
        updateFile "First" (setPublicVersion 2)
        updateFile "Second" (setPublicVersion 2)
        updateFile "Third" (setPublicVersion 2)
        checkFile "Last" (expectSignatureContains "val f: x: 'a -> (ModuleFirst.TFirstV_2<'a> * ModuleSecond.TSecondV_2<'a>) * (ModuleFirst.TFirstV_2<'a> * ModuleThird.TThirdV_2<'a>) * TLastV_1<'a>")
    }

[<Fact>]
let ``Files depend on signature file if present`` () =
    let project = makeTestProject() |> updateFile "First" addSignatureFile

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        updateFile "First" breakDependentFiles
        saveFile "First"
        checkFile "Second" expectNoChanges
    }

[<Fact>]
let ``Project with signatures`` () =

    let project = SyntheticProject.Create(
        { sourceFile "First" [] with
            Source = "let f (x: int) = x"
            SignatureFile = AutoGenerated },
        { sourceFile "Second" ["First"] with
            Source = "let a x = ModuleFirst.f x"
            SignatureFile = AutoGenerated })

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        checkFile "Second" expectOk
    }

[<Fact>]
let ``Signature update`` () =

    let project = SyntheticProject.Create(
        { sourceFile "First" [] with
            Source = "let f (x: int) = x"
            SignatureFile = Custom "val f: x: int -> int" },
        { sourceFile "Second" ["First"] with
            Source = "let a x = ModuleFirst.f x" })

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        checkFile "Second" expectOk
        updateFile "First" (fun f -> { f with SignatureFile = Custom "val f: x: string -> string" })
        checkFile "Second" expectSignatureChanged
    }

[<Fact>]
let ``Adding a file`` () =
    testWorkflow() {
        addFileAbove "Second" (sourceFile "New" [])
        updateFile "Second" (addDependency "New")
        checkFile "Last" (expectSignatureContains "val f: x: 'a -> (ModuleFirst.TFirstV_1<'a> * ModuleNew.TNewV_1<'a> * ModuleSecond.TSecondV_1<'a>) * (ModuleFirst.TFirstV_1<'a> * ModuleThird.TThirdV_1<'a>) * TLastV_1<'a>")
    }

[<Fact>]
let ``Removing a file`` () =
    testWorkflow() {
        removeFile "Second"
        checkFile "Last" expectErrors
    }

[<Fact>]
let ``Changes in a referenced project`` () =
    let library = SyntheticProject.Create("library", sourceFile "Library" [])

    let project =
        { makeTestProject() with DependsOn = [library] }
        |> updateFile "First" (addDependency "Library")

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {

        updateFile "First" updatePublicSurface
        checkFile "Last" expectOk

        updateFile "Library" updatePublicSurface
        saveFile "Library"
        checkFile "Last" expectSignatureChanged

    }

[<Fact>]
let ``File is not checked twice`` () =

    let observe, check = recordEvents getFileNameKey

    testWorkflow() {
        withChecker (observe _.TcIntermediate)
        updateFile "First" updatePublicSurface
        checkFile "Third" expectOk
    } |> ignore

    check (fileName "First") [Weakened; Requested; Started; Finished]
    check (fileName "Third") [Weakened; Requested; Started; Finished]


[<Fact>]
let ``If a file is checked as a dependency it's not re-checked later`` () =
    let observe, check = recordEvents getFileNameKey

    testWorkflow() {
        withChecker (observe _.TcIntermediate)
        updateFile "First" updatePublicSurface
        checkFile "Last" expectOk
        checkFile "Third" expectOk
    } |> ignore

    check (fileName "Third") [Weakened; Requested; Started; Finished; Requested]

[<Fact>] // TODO: differentiate complete and minimal checking requests
let ``We don't check files that are not depended on`` () =
    let project = SyntheticProject.Create(
        sourceFile "First" [],
        sourceFile "Second" ["First"],
        sourceFile "Third" ["First"],
        sourceFile "Last" ["Third"])

    let observe, check = recordEvents getFileNameKey

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        withChecker (observe _.TcIntermediate)
        updateFile "First" updatePublicSurface
        checkFile "Last" expectOk
    } |> ignore

    check "FileFirst.fs" [Weakened; Requested; Started; Finished]
    check "FileThird.fs" [Weakened; Requested; Started; Finished]
    // check "FileSecond.fs" [] // TODO: assert does not hold.

[<Fact(Skip="needs fixing")>] // TODO: differentiate complete and minimal checking requests
let ``Files that are not depended on don't invalidate cache`` () =
    let project = SyntheticProject.Create(
        sourceFile "First" [],
        sourceFile "Second" ["First"],
        sourceFile "Third" ["First"],
        sourceFile "Last" ["Third"])

    let observeTcIntermediateEvents, _getTcIntermediateEvents = recordAllEvents getFileNameKey
    // let observeGraphConstructionEvents, checkGraphConstructionEvents = record getDependecyGraphKey

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        updateFile "First" updatePublicSurface
        checkFile "Last" expectOk
        withChecker (observeTcIntermediateEvents _.TcIntermediate)
        // withChecker (observeGraphConstructionEvents _.DependencyGraph)
        updateFile "Second" updatePublicSurface
        checkFile "Last" expectOk
    } |> ignore

    // Assert.Empty(getTcIntermediateEvents()) TODO: assert does not hold
    // checkGraphConstructionEvents "FileLast.fs" [Started; Finished]

[<Fact(Skip="needs fixing")>] // TODO: differentiate complete and minimal checking requests
let ``Files that are not depended on don't invalidate cache part 2`` () =
    let project = SyntheticProject.Create(
        sourceFile "A" [],
        sourceFile "B" ["A"],
        sourceFile "C" ["A"],
        sourceFile "D" ["B"; "C"],
        sourceFile "E" ["C"])

    let observeTcIntermediateEvents, checkTcIntermediateEvents = recordEvents getFileNameKey
    // let observeGraphConstructionEvents, checkGraphConstructionEvents = record getDependecyGraphKey

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        updateFile "A" updatePublicSurface
        checkFile "D" expectOk
        withChecker (observeTcIntermediateEvents _.TcIntermediate)
        // withChecker (observeGraphConstructionEvents _.DependencyGraph)
        updateFile "B" updatePublicSurface
        checkFile "E" expectOk
    } |> ignore

    checkTcIntermediateEvents "FileE.fs" [Weakened; Requested; Started; Finished]
    // checkGraphConstructionEvents "FileE.fs" [Weakened; Requested; Started; Finished]

[<Fact>]
let ``Changing impl files doesn't invalidate cache when they have signatures`` () =
    let project = SyntheticProject.Create(
        { sourceFile "A" [] with SignatureFile = AutoGenerated },
        { sourceFile "B" ["A"] with SignatureFile = AutoGenerated },
        { sourceFile "C" ["B"] with SignatureFile = AutoGenerated })

    let observe, getEvents = recordAllEvents getFileNameKey

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        updateFile "A" updatePublicSurface
        checkFile "C" expectOk
        withChecker ( observe _.TcIntermediate)
        updateFile "A" updateInternal
        checkFile "C" expectOk
    } |> ignore

    Assert.Empty(getEvents())

[<Fact>]
let ``Changing impl file doesn't invalidate an in-memory referenced project`` () =
    let library = SyntheticProject.Create("library", { sourceFile "A" [] with SignatureFile = AutoGenerated })

    let project = {
        SyntheticProject.Create("project", sourceFile "B" ["A"] )
        with DependsOn = [library] }

    let mutable count = 0

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        checkFile "B" expectOk
        withChecker (fun checker ->
            async {
                checker.Caches.TcIntermediate.OnEvent (fun _ -> Interlocked.Increment &count |> ignore)
            })
        updateFile "A" updateInternal
        checkFile "B" expectOk
    } |> ignore

    Assert.Equal(0, count)

[<Theory>]
[<InlineData true>]
[<InlineData false>]
let ``Multi-project`` signatureFiles =

    let sigFile = if signatureFiles then AutoGenerated else No

    let library = SyntheticProject.Create("library",
        { sourceFile "LibA" []
            with
                Source = "let f (x: int) = x"
                SignatureFile = sigFile },
        { sourceFile "LibB" ["LibA"] with SignatureFile = sigFile },
        { sourceFile "LibC" ["LibA"] with SignatureFile = sigFile },
        { sourceFile "LibD" ["LibB"; "LibC"] with SignatureFile = sigFile }
        )

    let project =
        { SyntheticProject.Create("app",
            sourceFile "A" ["LibB"],
            sourceFile "B" ["A"; "LibB"],
            sourceFile "C" ["A"; "LibC"],
            sourceFile "D" ["A"; "LibD"]
            )
          with DependsOn = [library] }

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        updateFile "LibA" updatePublicSurface
        checkFile "D" expectOk
    }



type ProjectAction = Get | Modify of (SyntheticProject -> SyntheticProject)
type ProjectModification = Update of int | Add | Remove
type ProjectRequest = ProjectAction * AsyncReplyChannel<SyntheticProject>

type FuzzingEvent = StartedChecking | FinishedChecking of bool | AbortedChecking of string | ModifiedImplFile | ModifiedSigFile

type SignatureFiles = Yes = 1 | No = 2 | Some = 3

let fuzzingTest seed (project: SyntheticProject) = task {
    let rng = System.Random seed

    let checkingThreads = 10
    let maxModificationDelayMs = 50
    let maxCheckingDelayMs = 20
    //let runTimeMs = 30000
    let signatureFileModificationProbability = 0.25
    let modificationLoopIterations = 50
    let checkingLoopIterations = 5

    let minCheckingTimeoutMs = 0
    let maxCheckingTimeoutMs = 300

    let builder = ProjectWorkflowBuilder(project, useTransparentCompiler = true, autoStart = false)
    let checker = builder.Checker

    // Force creation and caching of options
    do! SaveAndCheckProject project checker false |> Async.Ignore

    let projectAgent = MailboxProcessor.Start(fun (inbox: MailboxProcessor<ProjectRequest>) ->
        let rec loop project =
            async {
                let! action, reply = inbox.Receive()
                let! project =
                    match action with
                    | Modify f -> async {
                        let p = f project
                        do! saveProject p false checker
                        return p }
                    | Get -> async.Return project
                reply.Reply project
                return! loop project
            }
        loop project)

    let getProject () =
        projectAgent.PostAndAsyncReply(pair Get)

    let modifyProject f =
        projectAgent.PostAndAsyncReply(pair(Modify f)) |> Async.Ignore

    let modificationProbabilities = [
        Update 1, 80
        Update 2, 5
        Update 10, 5
        //Add, 2
        //Remove, 1
    ]

    let modificationPicker = [|
        for op, prob in modificationProbabilities do
            for _ in 1 .. prob do
                op
    |]

    let addComment s = $"{s}\n\n// {rng.NextDouble()}"
    let modifyImplFile f = { f with ExtraSource = f.ExtraSource |> addComment }
    let modifySigFile f = { f with SignatureFile = Custom (f.SignatureFile.CustomText |> addComment) }

    let getRandomItem (xs: 'x array) = xs[rng.Next(0, xs.Length)]

    let getRandomModification () = modificationPicker |> getRandomItem

    let getRandomFile (project: SyntheticProject) = project.GetAllFiles() |> List.toArray |> getRandomItem

    let log = new ThreadLocal<_>((fun () -> ResizeArray<_>()), true)

    let exceptions = ConcurrentBag()

    let modificationLoop _ = task {
        for _ in 1 .. modificationLoopIterations do
            do! Task.Delay (rng.Next maxModificationDelayMs)
            let modify project =
                match getRandomModification() with
                | Update n ->

                    use _ = Activity.start "Update" [||]
                    let files = Set [ for _ in 1..n -> getRandomFile project |> snd ]
                    (project, files)
                    ||> Seq.fold (fun p file ->
                        let fileId = file.Id
                        let project, file = project.FindInAllProjects fileId
                        let opName, f =
                            if file.HasSignatureFile && rng.NextDouble() < signatureFileModificationProbability
                            then ModifiedSigFile, modifySigFile
                            else ModifiedImplFile, modifyImplFile
                        log.Value.Add (DateTime.Now.Ticks, opName, $"{project.Name} / {fileId}")
                        p |> updateFileInAnyProject fileId f)
                | Add
                | Remove ->
                    // TODO:
                    project
            do! modifyProject modify
    }

    let checkingLoop n _ = task {
        for _ in 1 .. checkingLoopIterations do
            let! project = getProject()
            let p, file = project |> getRandomFile

            let timeout = rng.Next(minCheckingTimeoutMs, maxCheckingTimeoutMs)

            log.Value.Add (DateTime.Now.Ticks, StartedChecking, $"Loop #{n} {file.Id} ({timeout} ms timeout)")
            let ct = new CancellationTokenSource()
            ct.CancelAfter(timeout)
            let job = Async.StartAsTask(checker |> checkFile file.Id p, cancellationToken = ct.Token)
            try
                use _ = Activity.start "Check" [||]

                let! parseResult, checkResult = job
                log.Value.Add (DateTime.Now.Ticks, FinishedChecking (match checkResult with FSharpCheckFileAnswer.Succeeded _ -> true | _ -> false),  $"Loop #{n} {file.Id}")
                expectOk (parseResult, checkResult) ()
            with ex ->
                let message =
                    match ex with
                    | :? AggregateException as e ->
                        match e.InnerException with
                        | :? GraphProcessingException as e -> $"GPE: {e.InnerException.Message}"
                        | _ -> e.Message
                    | _ -> ex.Message
                log.Value.Add (DateTime.Now.Ticks, AbortedChecking (message), $"Loop #{n} {file.Id} %A{ex}")
                if ex.Message <> "A task was canceled." then exceptions.Add ex

            do! Task.Delay (rng.Next maxCheckingDelayMs)
    }

    use _ = Activity.start $"Fuzzing {project.Name}" [ Activity.Tags.project, project.Name; "seed", seed.ToString() ]

    do! task {
        let threads =
            seq {
                modificationLoop CancellationToken.None
                // ignore modificationLoop
                for n in 1..checkingThreads do
                    checkingLoop n CancellationToken.None
            }

        try
            let! _x = threads |> Task.WhenAll
            ()
        with
            | e ->
                let _log = log.Values |> Seq.collect id |> Seq.sortBy p13 |> Seq.toArray
                failwith $"Seed: {seed}\nException: %A{e}"
    }
    let log = log.Values |> Seq.collect id |> Seq.sortBy p13 |> Seq.toArray

    let _stats = log |> Array.groupBy (p23) |> Array.map (fun (op, xs) -> op, xs.Length) |> Map

    let _errors = _stats |> Map.toSeq |> Seq.filter (fst >> function AbortedChecking ex when ex <> "A task was canceled." -> true | _ -> false) |> Seq.toArray

    let _exceptions = exceptions

    Assert.Equal<array<_>>([||], _errors)

    //Assert.Equal<Map<_,_>>(Map.empty, _stats)

    builder.DeleteProjectDir()
}

[<Theory(Skip="TODO: this sometimes fails, needs investigation")>]
[<InlineData(SignatureFiles.Yes)>]
[<InlineData(SignatureFiles.No)>]
[<InlineData(SignatureFiles.Some)>]
let Fuzzing signatureFiles =

    let seed = System.Random().Next()
    let rng = System.Random(int seed)

    let fileCount = 30
    let maxDepsPerFile = 3

    let fileName i = sprintf $"F%03d{i}"

    //let extraCode = __SOURCE_DIRECTORY__ ++ ".." ++ ".." ++ ".." ++ "src" ++ "Compiler" ++ "Utilities" ++ "EditDistance.fs" |> File.ReadAllLines |> Seq.skip 5 |> String.concat "\n"
    let extraCode = ""

    let files =
        [| for i in 1 .. fileCount do
            let name = fileName i
            let deps = [
                for _ in 1 .. maxDepsPerFile do
                    if i > 1 then
                      fileName <| rng.Next(1, i) ]
            let signature =
                match signatureFiles with
                | SignatureFiles.Yes -> AutoGenerated
                | SignatureFiles.Some when rng.NextDouble() < 0.5 -> AutoGenerated
                | _ -> No

            { sourceFile name deps
                with
                    SignatureFile = signature
                    ExtraSource = extraCode }
        |]

    let initialProject = SyntheticProject.Create(files)

    let builder = ProjectWorkflowBuilder(initialProject, useTransparentCompiler = true, autoStart = false)
    let checker = builder.Checker

    let initialProject = initialProject |> absorbAutoGeneratedSignatures checker |> Async.RunSynchronously

    fuzzingTest seed initialProject


let reposDir = __SOURCE_DIRECTORY__ ++ ".." ++ ".." ++ ".." ++ ".."
let giraffeDir = reposDir ++ "Giraffe" ++ "src" ++ "Giraffe" |> Path.GetFullPath
let giraffeTestsDir = reposDir ++ "Giraffe" ++ "tests" ++ "Giraffe.Tests" |> Path.GetFullPath
let giraffeSignaturesDir = reposDir ++ "giraffe-signatures" ++ "src" ++ "Giraffe" |> Path.GetFullPath
let giraffeSignaturesTestsDir = reposDir ++ "giraffe-signatures" ++ "tests" ++ "Giraffe.Tests" |> Path.GetFullPath


type GiraffeTheoryAttribute() =
    inherit Xunit.TheoryAttribute()
        do
            if not (Directory.Exists giraffeDir) then
                do base.Skip <- $"Giraffe not found ({giraffeDir}). You can get it here: https://github.com/giraffe-fsharp/Giraffe"
            if not (Directory.Exists giraffeSignaturesDir) then
                do base.Skip <- $"Giraffe (with signatures) not found ({giraffeSignaturesDir}). You can get it here: https://github.com/nojaf/Giraffe/tree/signatures"

[<GiraffeTheory>]
[<InlineData true>]
[<InlineData false>]
let GiraffeFuzzing signatureFiles =
    let seed = System.Random().Next()

    let giraffeDir = if signatureFiles then giraffeSignaturesDir else giraffeDir
    let giraffeTestsDir = if signatureFiles then giraffeSignaturesTestsDir else giraffeTestsDir

    let giraffeProject = SyntheticProject.CreateFromRealProject giraffeDir
    let giraffeProject = { giraffeProject with OtherOptions = "--nowarn:FS3520"::giraffeProject.OtherOptions }

    let testsProject = SyntheticProject.CreateFromRealProject giraffeTestsDir
    let testsProject =
        { testsProject
            with
                OtherOptions = "--nowarn:FS3520"::testsProject.OtherOptions
                DependsOn = [ giraffeProject ]
                NugetReferences = giraffeProject.NugetReferences @ testsProject.NugetReferences
                }

    fuzzingTest seed testsProject



[<GiraffeTheory>]
[<InlineData true>]
[<InlineData false>]
let ``File moving test`` signatureFiles =
    let giraffeDir = if signatureFiles then giraffeSignaturesDir else giraffeDir
    let giraffeProject = SyntheticProject.CreateFromRealProject giraffeDir
    let giraffeProject = { giraffeProject with OtherOptions = "--nowarn:FS3520"::giraffeProject.OtherOptions }

    giraffeProject.Workflow {
        // clearCache -- for better tracing
        checkFile "Json" expectOk
        moveFile "Json" 1 Down
        checkFile "Json" expectOk
    }


[<GiraffeTheory>]
[<InlineData true>]
let ``What happens if bootstrapInfoStatic needs to be recomputed`` _ =

    let giraffeProject = SyntheticProject.CreateFromRealProject giraffeSignaturesDir
    let giraffeProject = { giraffeProject with OtherOptions = "--nowarn:FS3520"::giraffeProject.OtherOptions }

    giraffeProject.Workflow {
        updateFile "Helpers" (fun f -> { f with SignatureFile = Custom (f.SignatureFile.CustomText + "\n") })
        checkFile "EndpointRouting" expectOk
        withChecker (fun checker ->
            async {
                checker.Caches.BootstrapInfoStatic.Clear()
                checker.Caches.BootstrapInfo.Clear()
                checker.Caches.FrameworkImports.Clear()
                ignore checker
                return ()
            })
        updateFile "Core" (fun f -> { f with SignatureFile = Custom (f.SignatureFile.CustomText + "\n") })
        checkFile "EndpointRouting" expectOk
    }


module ParsedInputHashing =

    let source = """

type T = { A: int; B: string }

module Stuff =

    // Some comment
    let f x = x + 75
"""

    let getParseResult source =
        let fileName, snapshot, checker = singleFileChecker source
        checker.ParseFile(fileName, snapshot) |> Async.RunSynchronously

    //[<Fact>]
    let ``Hash stays the same when whitespace changes`` () =

        //let parseResult = getParseResult source

        //let hash = parseResult.ParseTree |> parsedInputHash |> BitConverter.ToString

        //let parseResult2 = getParseResult (source + "\n \n")

        //let hash2 = parseResult2.ParseTree |> parsedInputHash |> BitConverter.ToString

        //Assert.Equal<string>(hash, hash2)

        ()

/// Update these paths to a local response file with compiler arguments of existing F# projects.
/// References projects are expected to have been built.
let localResponseFiles =
    [|
        @"C:\Projects\fantomas\src\Fantomas.Core.Tests\Fantomas.Core.Tests.rsp"
    |]
    |> Array.collect (fun f ->
        [|
            [| true :> obj; f:> obj |]
            [| false :> obj; f :> obj|]
        |]
    )

// Uncomment this attribute if you want run this test against local response files.
// [<Theory>]
[<MemberData(nameof(localResponseFiles))>]
let ``TypeCheck last file in project with transparent compiler`` useTransparentCompiler responseFile =
    let responseFile = FileInfo responseFile
    let syntheticProject = mkSyntheticProjectForResponseFile responseFile

    let workflow =
        ProjectWorkflowBuilder(
            syntheticProject,
            isExistingProject = true,
            useTransparentCompiler = useTransparentCompiler
        )

    let lastFile =
        syntheticProject.SourceFiles
        |> List.tryLast
        |> Option.map (fun sf -> sf.Id)

    match lastFile with
    | None -> failwithf "Last file of project could not be found"
    | Some lastFile ->

    workflow {
        clearCache
        checkFile lastFile expectOk
    }

[<Fact>]
let ``LoadClosure for script is computed once`` () =
        let project = SyntheticProject.CreateForScript(
            sourceFile "First" [])

        let observe, getEvents = recordAllEvents getFileNameKey

        ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
            withChecker (observe _.ScriptClosure)
            checkFile "First" expectOk
        }
        |> ignore

        Assert.Empty(getEvents())

[<Fact>]
let ``LoadClosure for script is recomputed after changes`` () =

    let project = SyntheticProject.CreateForScript(
        sourceFile "First" [])

    let observe, check = recordEvents getFileNameKey

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        withChecker (observe _.ScriptClosure)
        checkFile "First" expectOk
        updateFile "First" updateInternal
        checkFile "First" expectOk
        updateFile "First" updatePublicSurface
        checkFile "First" expectOk
    } |> ignore

    check (fileName "First") [Weakened; Requested; Started; Finished; Weakened; Requested; Started; Finished]

[<Fact>]
let ``TryGetRecentCheckResultsForFile returns None before first call to ParseAndCheckFileInProject`` () =
    let project = SyntheticProject.Create(
        sourceFile "First" [])

    ProjectWorkflowBuilder(project) {
        clearCache
        tryGetRecentCheckResults "First" expectNone
    } |> ignore

[<Fact>]
let ``TryGetRecentCheckResultsForFile returns result after first call to ParseAndCheckFileInProject`` () =
    let project = SyntheticProject.Create(
        sourceFile "First" [] )

    ProjectWorkflowBuilder(project) {
        tryGetRecentCheckResults "First" expectSome
    } |> ignore

[<Fact>]
let ``TryGetRecentCheckResultsForFile returns no result after edit`` () =
    let project = SyntheticProject.Create(
        sourceFile "First" [])

    ProjectWorkflowBuilder(project) {
        tryGetRecentCheckResults "First" expectSome
        updateFile "First" updatePublicSurface
        tryGetRecentCheckResults "First" expectNone
        checkFile "First" expectOk
        tryGetRecentCheckResults "First" expectSome
    } |> ignore

[<Fact>]
let ``TryGetRecentCheckResultsForFile returns result after edit of other file`` () =
    let project = SyntheticProject.Create(
        sourceFile "First" [],
        sourceFile "Second" ["First"])

    ProjectWorkflowBuilder(project) {
        tryGetRecentCheckResults "First" expectSome
        tryGetRecentCheckResults "Second" expectSome
        updateFile "First" updatePublicSurface
        tryGetRecentCheckResults "First"  expectNone
        tryGetRecentCheckResults "Second" expectSome // file didn't change so we still want to get the recent result
    } |> ignore

[<Fact(Skip="TransparentCompiler assumeDotNetFramework differs from default checker")>]
let ``Background compiler and Transparent compiler return the same options`` () =
    task {
        let backgroundChecker = FSharpChecker.Create(useTransparentCompiler = false)
        let transparentChecker = FSharpChecker.Create(useTransparentCompiler = true)
        let scriptName = Path.Combine(__SOURCE_DIRECTORY__, "script.fsx")
        let content = SourceTextNew.ofString ""

        let! backgroundSnapshot, backgroundDiags = backgroundChecker.GetProjectSnapshotFromScript(scriptName, content)
        let! transparentSnapshot, transparentDiags = transparentChecker.GetProjectSnapshotFromScript(scriptName, content)
        Assert.Empty(backgroundDiags)
        Assert.Empty(transparentDiags)
        Assert.Equal<string list>(backgroundSnapshot.OtherOptions, transparentSnapshot.OtherOptions)
        Assert.Equal<ProjectSnapshot.ReferenceOnDisk list>(backgroundSnapshot.ReferencesOnDisk, transparentSnapshot.ReferencesOnDisk)
    }

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``Unused warning should still produce after parse warning`` useTransparentCompiler =

    // There should be parse warning because of the space in the file name:
    //   warning FS0221: The declarations in this file will be placed in an implicit module 'As 01' based on the file name 'As 01.fs'.
    //   However this is not a valid F# identifier, so the contents will not be accessible from other files. Consider renaming the file or adding a 'module' or 'namespace' declaration at the top of the file.

    let project =
        { SyntheticProject.Create(
            { sourceFile "As 01" [] with
                Source = """
do
    let _ as b = ()
    ()

// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"
"""
                SignatureFile = No

            }) with
            AutoAddModules = false
            OtherOptions = [
                "--target:exe"
                "--warn:3"
                "--warnon:1182"
                "--warnaserror:3239"
                "--noframework"
            ]
        }

    let expectTwoWarnings (_parseResult:FSharpParseFileResults, checkAnswer: FSharpCheckFileAnswer) (_, _) =
        match checkAnswer with
        | FSharpCheckFileAnswer.Aborted -> failwith "Should not have aborted"
        | FSharpCheckFileAnswer.Succeeded checkResults ->
            let hasParseWarning =
                checkResults.Diagnostics
                |> Array.exists (fun diag -> diag.Severity = FSharpDiagnosticSeverity.Warning && diag.ErrorNumber = 221)
            Assert.True(hasParseWarning, "Expected parse warning FS0221")

            let hasCheckWarning =
                checkResults.Diagnostics
                |> Array.exists (fun diag -> diag.Severity = FSharpDiagnosticSeverity.Warning && diag.ErrorNumber = 1182)
            Assert.True(hasCheckWarning, "Expected post inference warning FS1182")

    ProjectWorkflowBuilder(project, useTransparentCompiler = useTransparentCompiler) {
        checkFile "As 01" expectTwoWarnings
    }

[<Fact>]
let ``Transparent Compiler ScriptClosure cache is populated after GetProjectOptionsFromScript`` () =
    task {
        let transparentChecker = FSharpChecker.Create(useTransparentCompiler = true)
        let scriptName = Path.Combine(__SOURCE_DIRECTORY__, "script.fsx")
        let content = SourceTextNew.ofString ""
        let! _ = transparentChecker.GetProjectOptionsFromScript(scriptName, content)
        Assert.Equal(1, transparentChecker.Caches.ScriptClosure.Count)
    }

type private LoadClosureTestShim(currentFileSystem: IFileSystem) =
    inherit DefaultFileSystem()
    let mutable bDidUpdate = false
    let asStream (v:string) = new MemoryStream(System.Text.Encoding.UTF8.GetBytes v)
    let knownFiles = set [ "a.fsx"; "b.fsx"; "c.fsx" ]

    member val aFsx = "#load \"b.fsx\""
    member val  bFsxInitial = ""
    member val  bFsxUpdate = "#load \"c.fsx\""
    member val  cFsx = ""

    member x.DocumentSource (fileName: string) =
        async {
            if not (knownFiles.Contains fileName) then
                return None
            else
                match fileName with
                | "a.fsx" -> return Some (SourceText.ofString x.aFsx)
                | "b.fsx" ->  return Some (SourceText.ofString (if bDidUpdate then x.bFsxUpdate else x.bFsxInitial))
                | "c.fsx" -> return Some (SourceText.ofString x.cFsx)
                | _ -> return  None
        }

    member x.UpdateB () = bDidUpdate <- true

    override _.FileExistsShim(path) =
        if knownFiles.Contains path then true else currentFileSystem.FileExistsShim(path)
    override _.GetFullPathShim(fileName) =
        if knownFiles.Contains fileName then fileName else currentFileSystem.GetFullPathShim(fileName)
    override x.OpenFileForReadShim(fileName, ?useMemoryMappedFile: bool, ?shouldShadowCopy: bool) =
        match fileName with
        | "a.fsx" -> asStream x.aFsx
        | "b.fsx" ->  asStream (if bDidUpdate then x.bFsxUpdate else x.bFsxInitial)
        | "c.fsx" -> asStream x.cFsx
        | _ ->
            currentFileSystem.OpenFileForReadShim(
                fileName,
                ?useMemoryMappedFile = useMemoryMappedFile,
                ?shouldShadowCopy = shouldShadowCopy
            )

// Because it is mutating FileSystem!
[<Collection(nameof NotThreadSafeResourceCollection)>]
module TestsMutatingFileSystem =

    [<Theory>]
    [<InlineData(false)>]
    [<InlineData(true)>]
    let ``The script load closure should always be evaluated`` useTransparentCompiler =
        async {
            // The LoadScriptClosure uses the file system shim so we need to reset that.
            let currentFileSystem = FileSystemAutoOpens.FileSystem
            let assumeDotNetFramework =
                // The old BackgroundCompiler uses assumeDotNetFramework = true
                // This is not always correctly loading when this test runs on non-Windows.
                if System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework") then
                    None
                else
                    Some false

            try
                let checker = FSharpChecker.Create(useTransparentCompiler = useTransparentCompiler)
                let fileSystemShim = LoadClosureTestShim(currentFileSystem)
                // Override the file system shim for loading b.fsx
                FileSystem <- fileSystemShim

                let! initialSnapshot, _ =
                    checker.GetProjectSnapshotFromScript(
                        "a.fsx",
                        SourceTextNew.ofString fileSystemShim.aFsx,
                        documentSource = DocumentSource.Custom fileSystemShim.DocumentSource,
                        ?assumeDotNetFramework = assumeDotNetFramework
                    )

                // File b.fsx should also be included in the snapshot.
                Assert.Equal(2, initialSnapshot.SourceFiles.Length)

                let! checkResults = checker.ParseAndCheckFileInProject("a.fsx", initialSnapshot)

                match snd checkResults with
                | FSharpCheckFileAnswer.Aborted -> failwith "Did not expected FSharpCheckFileAnswer.Aborted"
                | FSharpCheckFileAnswer.Succeeded checkFileResults -> Assert.Equal(0, checkFileResults.Diagnostics.Length)

                // Update b.fsx, it should now load c.fsx
                fileSystemShim.UpdateB()

                // The constructed key for the load closure will the exactly the same as the first GetProjectSnapshotFromScript call.
                // However, a none cached version will be computed first in GetProjectSnapshotFromScript and update the cache afterwards.
                let! secondSnapshot, _ =
                    checker.GetProjectSnapshotFromScript(
                        "a.fsx",
                        SourceTextNew.ofString fileSystemShim.aFsx,
                        documentSource = DocumentSource.Custom fileSystemShim.DocumentSource,
                        ?assumeDotNetFramework = assumeDotNetFramework
                    )

                Assert.Equal(3, secondSnapshot.SourceFiles.Length)

                let! checkResults = checker.ParseAndCheckFileInProject("a.fsx", secondSnapshot)

                match snd checkResults with
                | FSharpCheckFileAnswer.Aborted -> failwith "Did not expected FSharpCheckFileAnswer.Aborted"
                | FSharpCheckFileAnswer.Succeeded checkFileResults -> Assert.Equal(0, checkFileResults.Diagnostics.Length)
            finally
                FileSystemAutoOpens.FileSystem <- currentFileSystem
        }

[<Fact>]
let ``Parsing without cache and without project snapshot`` () =
    async {
        let checker = FSharpChecker.Create(useTransparentCompiler = true)
        let fileName = "Temp.fs"
        let sourceText = "let a = 0" |> SourceText.ofString
        let parsingOptions = { FSharpParsingOptions.Default with SourceFiles = [| fileName |]; IsExe = true }
        let! parseResult = checker.ParseFile(fileName, sourceText, parsingOptions, cache = false)
        Assert.False(parseResult.ParseHadErrors)
        Assert.True(Array.isEmpty parseResult.Diagnostics)
        Assert.Equal(0, checker.Caches.ParseFile.Count)
        Assert.Equal(0, checker.Caches.ParseFileWithoutProject.Count)
    }

// In this scenario, the user is typing something in file B.fs.
// The idea is that the IDE will introduce an additional (fake) identifier in order to have a potential complete syntax tree.
// The user never wrote this code so we need to ensure nothing is added to checker.Caches.ParseFile
[<Fact>]
let ``Parsing with cache and without project snapshot`` () =
    async {
        let checker = FSharpChecker.Create(useTransparentCompiler = true)
        let fileName = "B.fs"
        let parsingOptions = { FSharpParsingOptions.Default with SourceFiles = [| "A.fs"; fileName; "C.fs" |] }
        let sourceText =
            SourceText.ofString """
module B

let b : int = ExtraIdentUserNeverWroteRulezzz
"""
        let! parseResult = checker.ParseFile(fileName, sourceText, parsingOptions, cache = true)
        Assert.False(parseResult.ParseHadErrors)
        Assert.True(Array.isEmpty parseResult.Diagnostics)

        let! parseAgainResult = checker.ParseFile(fileName, sourceText, parsingOptions, cache = true)
        Assert.False(parseAgainResult.ParseHadErrors)
        Assert.True(Array.isEmpty parseAgainResult.Diagnostics)

        Assert.Equal(0, checker.Caches.ParseFile.Count)
        Assert.Equal(1, checker.Caches.ParseFileWithoutProject.Count)
    }
