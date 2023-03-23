module FSharp.Compiler.ComponentTests.FSharpChecker.CommonWorkflows

open System
open System.IO
open System.Diagnostics

open Xunit

open FSharp.Test.ProjectGeneration
open FSharp.Compiler.Text
open FSharp.Compiler.CodeAnalysis

module FcsDiagnostics = FSharp.Compiler.Diagnostics.Activity

let expectCacheHits n =
    let events = ResizeArray()
    let listener = 
        new ActivityListener(
            ShouldListenTo = (fun s -> s.Name = FcsDiagnostics.FscSourceName),
            Sample = (fun _ -> ActivitySamplingResult.AllData),
            ActivityStopped = (fun a -> events.AddRange a.Events)
        )
    ActivitySource.AddActivityListener listener
    { new IDisposable with 
        member this.Dispose() =
            listener.Dispose()
            Assert.Equal(n, events |> Seq.filter (fun e -> e.Name = FcsDiagnostics.Events.cacheHit) |> Seq.length) }

let makeTestProject () =
    SyntheticProject.Create(
        sourceFile "First" [],
        sourceFile "Second" ["First"],
        sourceFile "Third" ["First"],
        { sourceFile "Last" ["Second"; "Third"] with EntryPoint = true })

[<Fact>]
let ``Edit file, check it, then check dependent file`` () =
    makeTestProject().Workflow {
        updateFile "First" breakDependentFiles
        checkFile "First" expectSignatureChanged
        saveFile "First"
        checkFile "Second" expectErrors
    }

[<Fact>]
let ``Edit file, don't check it, check dependent file`` () =
    makeTestProject().Workflow {
        updateFile "First" breakDependentFiles
        saveFile "First"
        checkFile "Second" expectErrors
    }

[<Fact>]
let ``Check transitive dependency`` () =
    makeTestProject().Workflow {
        updateFile "First" breakDependentFiles
        saveFile "First"
        checkFile "Last" expectSignatureChanged
    }

[<Fact>]
let ``Change multiple files at once`` () =
    makeTestProject().Workflow {
        updateFile "First" (setPublicVersion 2)
        updateFile "Second" (setPublicVersion 2)
        updateFile "Third" (setPublicVersion 2)
        saveAll
        checkFile "Last" (expectSignatureContains "val f: x: 'a -> (ModuleFirst.TFirstV_2<'a> * ModuleSecond.TSecondV_2<'a>) * (ModuleFirst.TFirstV_2<'a> * ModuleThird.TThirdV_2<'a>) * TLastV_1<'a>")
    }

[<Fact>]
let ``Files depend on signature file if present`` () =
    (makeTestProject()
    |> updateFile "First" addSignatureFile
    |> projectWorkflow) {
        updateFile "First" breakDependentFiles
        saveFile "First"
        checkFile "Second" expectNoChanges
    }

[<Fact>]
let ``Adding a file`` () =
    makeTestProject().Workflow {
        addFileAbove "Second" (sourceFile "New" [])
        updateFile "Second" (addDependency "New")
        saveAll
        checkFile "Last" (expectSignatureContains "val f: x: 'a -> (ModuleNew.TNewV_1<'a> * ModuleFirst.TFirstV_1<'a> * ModuleSecond.TSecondV_1<'a>) * (ModuleFirst.TFirstV_1<'a> * ModuleThird.TThirdV_1<'a>) * TLastV_1<'a>")
    }

[<Fact>]
let ``Removing a file`` () =
    makeTestProject().Workflow {
        removeFile "Second"
        saveAll
        checkFile "Last" expectErrors
    }

[<Fact>]
let ``Changes in a referenced project`` () =
    let library = SyntheticProject.Create("library", sourceFile "Library" [])

    let project =
        { makeTestProject() with DependsOn = [library] }
        |> updateFile "First" (addDependency "Library")

    project.Workflow {
        updateFile "Library" updatePublicSurface
        saveFile "Library"
        checkFile "Last" expectSignatureChanged
    }

[<Fact>]
let ``Language service works if the same file is listed twice`` () = 
    let file = sourceFile "First" []
    let project =  SyntheticProject.Create(file)
    project.Workflow {
        checkFile "First" expectOk
        addFileAbove "First" file
        checkFile "First" (expectSingleWarningAndNoErrors "Please verify that it is included only once in the project file.")
    }

[<Fact>]
let ``Using getSource and notifications instead of filesystem`` () =

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

    ProjectWorkflowBuilder(project, useGetSource = true, useChangeNotifications = true) {
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
let ``Using getSource and notifications instead of filesystem with parse caching`` () =

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

    use _ = expectCacheHits 51
    ProjectWorkflowBuilder(project, useGetSource = true, useChangeNotifications = true, useSyntaxTreeCache = true) {
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
let ``Edit file, check it, then check dependent file with parse caching`` () =
    use _ = expectCacheHits 6
    ProjectWorkflowBuilder(makeTestProject(), useSyntaxTreeCache = true) {
        updateFile "First" breakDependentFiles
        checkFile "First" expectSignatureChanged
        saveFile "First"
        checkFile "Second" expectErrors
    }

[<Fact>]
let ``Edit file, don't check it, check dependent file with parse caching `` () =
    use _ = expectCacheHits 6
    ProjectWorkflowBuilder(makeTestProject(), useSyntaxTreeCache = true) {
        updateFile "First" breakDependentFiles
        saveFile "First"
        checkFile "Second" expectErrors
    }

[<Fact>]
let ``Parse cache not used when not enabled`` () =
    use _ = expectCacheHits 0
    ProjectWorkflowBuilder(makeTestProject(), useSyntaxTreeCache = false) {
        updateFile "First" breakDependentFiles
        saveFile "First"
        checkFile "Second" expectErrors
    }