﻿module FSharpChecker.TransparentCompiler

open System.Collections.Concurrent
open System.Diagnostics
open FSharp.Compiler.CodeAnalysis
open Internal.Utilities.Collections
open FSharp.Compiler.CodeAnalysis.TransparentCompiler
open Internal.Utilities.Library.Extras
open FSharp.Compiler.GraphChecking.GraphProcessing
open FSharp.Compiler.Diagnostics

open Xunit

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

    let cacheEvents = ConcurrentQueue()

    testWorkflow() {
        withChecker (fun checker ->
            async {
                do! Async.Sleep 50 // wait for events from initial project check
                checker.Caches.TcIntermediate.OnEvent cacheEvents.Enqueue
            })
        updateFile "First" updatePublicSurface
        checkFile "Third" expectOk
    } |> ignore

    let intermediateTypeChecks =
        cacheEvents
        |> Seq.groupBy (fun (_e, (_l, (f, _p), _)) -> f |> Path.GetFileName)
        |> Seq.map (fun (k, g) -> k, g |> Seq.map fst |> Seq.toList)
        |> Map

    Assert.Equal<JobEvent list>([Weakened; Requested; Started; Finished], intermediateTypeChecks["FileFirst.fs"])
    Assert.Equal<JobEvent list>([Weakened; Requested; Started; Finished], intermediateTypeChecks["FileThird.fs"])

[<Fact>]
let ``If a file is checked as a dependency it's not re-checked later`` () =
    let cacheEvents = ConcurrentQueue()

    testWorkflow() {
        withChecker (fun checker ->
            async {
                do! Async.Sleep 50 // wait for events from initial project check
                checker.Caches.TcIntermediate.OnEvent cacheEvents.Enqueue
            })
        updateFile "First" updatePublicSurface
        checkFile "Last" expectOk
        checkFile "Third" expectOk
    } |> ignore

    let intermediateTypeChecks =
        cacheEvents
        |> Seq.groupBy (fun (_e, (_l, (f, _p), _)) -> f |> Path.GetFileName)
        |> Seq.map (fun (k, g) -> k, g |> Seq.map fst |> Seq.toList)
        |> Map

    Assert.Equal<JobEvent list>([Weakened; Requested; Started; Finished; Requested], intermediateTypeChecks["FileThird.fs"])


// [<Fact>] TODO: differentiate complete and minimal checking requests
let ``We don't check files that are not depended on`` () =
    let project = SyntheticProject.Create(
        sourceFile "First" [],
        sourceFile "Second" ["First"],
        sourceFile "Third" ["First"],
        sourceFile "Last" ["Third"])

    let cacheEvents = ConcurrentQueue()

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        withChecker (fun checker ->
            async {
                do! Async.Sleep 50 // wait for events from initial project check
                checker.Caches.TcIntermediate.OnEvent cacheEvents.Enqueue
            })
        updateFile "First" updatePublicSurface
        checkFile "Last" expectOk
    } |> ignore

    let intermediateTypeChecks =
        cacheEvents
        |> Seq.groupBy (fun (_e, (_l, (f, _p), _)) -> Path.GetFileName f)
        |> Seq.map (fun (k, g) -> k, g |> Seq.map fst |> Seq.toList)
        |> Map

    Assert.Equal<JobEvent list>([Started; Finished], intermediateTypeChecks["FileFirst.fs"])
    Assert.Equal<JobEvent list>([Started; Finished], intermediateTypeChecks["FileThird.fs"])
    Assert.False (intermediateTypeChecks.ContainsKey "FileSecond.fs")

// [<Fact>] TODO: differentiate complete and minimal checking requests
let ``Files that are not depended on don't invalidate cache`` () =
    let project = SyntheticProject.Create(
        sourceFile "First" [],
        sourceFile "Second" ["First"],
        sourceFile "Third" ["First"],
        sourceFile "Last" ["Third"])

    let cacheTcIntermediateEvents = ConcurrentQueue()
    let cacheGraphConstructionEvents = ConcurrentQueue()

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        updateFile "First" updatePublicSurface
        checkFile "Last" expectOk
        withChecker (fun checker ->
            async {
                do! Async.Sleep 50 // wait for events from initial project check
                checker.Caches.TcIntermediate.OnEvent cacheTcIntermediateEvents.Enqueue
                checker.Caches.DependencyGraph.OnEvent cacheGraphConstructionEvents.Enqueue

            })
        updateFile "Second" updatePublicSurface
        checkFile "Last" expectOk
    } |> ignore

    let intermediateTypeChecks =
        cacheTcIntermediateEvents
        |> Seq.groupBy (fun (_e, (l, _k, _)) -> l)
        |> Seq.map (fun (k, g) -> k, g |> Seq.map fst |> Seq.toList)
        |> Map

    let graphConstructions =
        cacheGraphConstructionEvents
        |> Seq.groupBy (fun (_e, (l, _k, _)) -> l)
        |> Seq.map (fun (k, g) -> k, g |> Seq.map fst |> Seq.toList)
        |> Map

    Assert.Equal<JobEvent list>([Started; Finished], graphConstructions["FileLast.fs"])

    Assert.Equal<string * JobEvent list>([], intermediateTypeChecks |> Map.toList)

// [<Fact>] TODO: differentiate complete and minimal checking requests
let ``Files that are not depended on don't invalidate cache part 2`` () =
    let project = SyntheticProject.Create(
        sourceFile "A" [],
        sourceFile "B" ["A"],
        sourceFile "C" ["A"],
        sourceFile "D" ["B"; "C"],
        sourceFile "E" ["C"])

    let cacheTcIntermediateEvents = ConcurrentQueue()
    let cacheGraphConstructionEvents = ConcurrentQueue()

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        updateFile "A" updatePublicSurface
        checkFile "D" expectOk
        withChecker (fun checker ->
            async {
                do! Async.Sleep 50 // wait for events from initial project check
                checker.Caches.TcIntermediate.OnEvent cacheTcIntermediateEvents.Enqueue
                checker.Caches.DependencyGraph.OnEvent cacheGraphConstructionEvents.Enqueue
            })
        updateFile "B" updatePublicSurface
        checkFile "E" expectOk
    } |> ignore

    let intermediateTypeChecks =
        cacheTcIntermediateEvents
        |> Seq.groupBy (fun (_e, (l, _k, _)) -> l)
        |> Seq.map (fun (k, g) -> k, g |> Seq.map fst |> Seq.toList)
        |> Seq.toList

    let graphConstructions =
        cacheGraphConstructionEvents
        |> Seq.groupBy (fun (_e, (l, _k, _)) -> l)
        |> Seq.map (fun (k, g) -> k, g |> Seq.map fst |> Seq.toList)
        |> Seq.toList

    Assert.Equal<string * JobEvent list>(["FileE.fs", [Started; Finished]], graphConstructions)
    Assert.Equal<string * JobEvent list>(["FileE.fs", [Started; Finished]], intermediateTypeChecks)

[<Fact>]
let ``Changing impl files doesn't invalidate cache when they have signatures`` () =
    let project = SyntheticProject.Create(
        { sourceFile "A" [] with SignatureFile = AutoGenerated },
        { sourceFile "B" ["A"] with SignatureFile = AutoGenerated },
        { sourceFile "C" ["B"] with SignatureFile = AutoGenerated })

    let cacheEvents = ConcurrentQueue()

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        updateFile "A" updatePublicSurface
        checkFile "C" expectOk
        withChecker (fun checker ->
            async {
                do! Async.Sleep 50 // wait for events from initial project check
                checker.Caches.TcIntermediate.OnEvent cacheEvents.Enqueue
            })
        updateFile "A" updateInternal
        checkFile "C" expectOk
    } |> ignore

    let intermediateTypeChecks =
        cacheEvents
        |> Seq.groupBy (fun (_e, (l, _k, _)) -> l)
        |> Seq.map (fun (k, g) -> k, g |> Seq.map fst |> Seq.toList)
        |> Seq.toList

    Assert.Equal<string * JobEvent list>([], intermediateTypeChecks)

[<Fact>]
let ``Changing impl file doesn't invalidate an in-memory referenced project`` () =
    let library = SyntheticProject.Create("library", { sourceFile "A" [] with SignatureFile = AutoGenerated })

    let project = {
        SyntheticProject.Create("project", sourceFile "B" ["A"] )
        with DependsOn = [library] }

    let cacheEvents = ConcurrentQueue()

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        checkFile "B" expectOk
        withChecker (fun checker ->
            async {
                do! Async.Sleep 50 // wait for events from initial project check
                checker.Caches.TcIntermediate.OnEvent cacheEvents.Enqueue
            })
        updateFile "A" updateInternal
        checkFile "B" expectOk
    } |> ignore

    let intermediateTypeChecks =
        cacheEvents
        |> Seq.groupBy (fun (_e, (l, _k, _)) -> l)
        |> Seq.map (fun (k, g) -> k, g |> Seq.map fst |> Seq.toList)
        |> Seq.toList

    Assert.Equal<string * JobEvent list>([], intermediateTypeChecks)


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
type ProjectModificaiton = Update of int | Add | Remove
type ProjectRequest = ProjectAction * AsyncReplyChannel<SyntheticProject>

type FuzzingEvent = StartedChecking | FinishedChecking of bool | AbortedChecking of string | ModifiedImplFile | ModifiedSigFile

[<RequireQualifiedAccess>]
type SignatureFiles = Yes = 1 | No = 2 | Some = 3

let fuzzingTest seed (project: SyntheticProject) = task {
    let rng = System.Random seed

    let checkingThreads = 3
    let maxModificationDelayMs = 10
    let maxCheckingDelayMs = 20
    //let runTimeMs = 30000
    let signatureFileModificationProbability = 0.25
    let modificationLoopIterations = 10
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

    use _tracerProvider =
        Sdk.CreateTracerProviderBuilder()
            .AddSource("fsc")
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName="F# Fuzzing", serviceVersion = "1"))
            .AddJaegerExporter()
            .Build()

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
            let! _x = threads |> Seq.skip 1 |> Task.WhenAll
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


[<Theory>]
[<InlineData(SignatureFiles.Yes)>]
[<InlineData(SignatureFiles.No)>]
[<InlineData(SignatureFiles.Some)>]
let Fuzzing signatureFiles =

    let seed = 1106087513
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
    //let seed = 1044159179

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
let ``What happens if bootrstapInfoStatic needs to be recomputed`` _ =

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
        |> List.last
        |> fun sf -> sf.Id

    workflow {
        clearCache 
        checkFile lastFile expectOk
    }
