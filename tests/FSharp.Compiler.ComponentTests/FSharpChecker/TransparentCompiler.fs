module FSharp.Compiler.ComponentTests.FSharpChecker.TransparentCompiler

open System
open System.IO
open System.Diagnostics

open Xunit

open FSharp.Test.ProjectGeneration
open FSharp.Compiler.Text
open FSharp.Compiler.CodeAnalysis

module Activity =
    let listen (filter: string) =
        let indent (activity: Activity) =
            let rec loop (activity: Activity) n =
                if activity.Parent <> null then
                    loop (activity.Parent) (n + 1)
                else
                    n

            String.replicate (loop activity 0) "    "

        let collectTags (activity: Activity) =
            [ for tag in activity.Tags -> $"{tag.Key}: %A{tag.Value}" ]
            |> String.concat ", "

        let listener =
            new ActivityListener(
                ShouldListenTo = (fun source -> source.Name = FSharp.Compiler.Diagnostics.ActivityNames.FscSourceName),
                Sample =
                    (fun context ->
                        if context.Name.Contains(filter) then
                            ActivitySamplingResult.AllDataAndRecorded
                        else
                            ActivitySamplingResult.None),
                ActivityStarted = (fun a -> Trace.TraceInformation $"{indent a}{a.OperationName}     {collectTags a}")
            )

        ActivitySource.AddActivityListener(listener)

    let listenToAll () = listen ""


[<Fact>]
let ``Use Transparent Compiler`` () =

    Activity.listenToAll ()

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

    Activity.listenToAll ()

    let project = SyntheticProject.Create(
        sourceFile "A" [],
        sourceFile "B" ["A"],
        sourceFile "C" ["A"],
        sourceFile "D" ["A"],
        sourceFile "E" ["B"; "C"; "D"])

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        checkFile "E" expectOk
    }


[<Fact>]
let ``Parallel processing with signatures`` () =

    Activity.listenToAll ()

    let project = SyntheticProject.Create(
        sourceFile "A" [] |> addSignatureFile,
        sourceFile "B" ["A"] |> addSignatureFile,
        sourceFile "C" ["A"] |> addSignatureFile,
        sourceFile "D" ["A"] |> addSignatureFile,
        sourceFile "E" ["B"; "C"; "D"] |> addSignatureFile)

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        checkFile "C" expectOk
        //updateFile "A" updatePublicSurface
        //checkFile "E" expectSignatureChanged
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
let ``Adding a file`` () =
    testWorkflow() {
        addFileAbove "Second" (sourceFile "New" [])
        updateFile "Second" (addDependency "New")
        checkFile "Last" (expectSignatureContains "val f: x: 'a -> (ModuleNew.TNewV_1<'a> * ModuleFirst.TFirstV_1<'a> * ModuleSecond.TSecondV_1<'a>) * (ModuleFirst.TFirstV_1<'a> * ModuleThird.TThirdV_1<'a>) * TLastV_1<'a>")
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

    project.Workflow {
        updateFile "Library" updatePublicSurface
        saveFile "Library"
        checkFile "Last" expectSignatureChanged
    }