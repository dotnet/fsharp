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

    let _logger = Activity.listenToAll ()

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

    let _logger = Activity.listenToAll ()

    let project = SyntheticProject.Create(
        sourceFile "A" [],
        sourceFile "B" ["A"],
        sourceFile "C" ["A"],
        sourceFile "D" ["A"],
        sourceFile "E" ["B"; "C"; "D"])

    ProjectWorkflowBuilder(project, useTransparentCompiler = true) {
        checkFile "E" expectOk
    }
