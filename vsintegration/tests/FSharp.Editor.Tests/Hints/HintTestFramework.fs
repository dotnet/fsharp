// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests.Hints

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.Hints
open Hints
open FSharp.Editor.Tests.Helpers
open Internal.Utilities.CancellableTasks
open System.Threading

module HintTestFramework =

    // another representation for extra convenience
    type TestHint =
        {
            Content: string
            Location: int * int
            Tooltip: string
        }

    let private convert (hint, tooltip) =
        let content =
            hint.Parts |> Seq.map (fun hintPart -> hintPart.Text) |> String.concat ""

        // that's about different coordinate systems
        // in tests, the most convenient is the one used in editor,
        // hence this conversion
        let location = (hint.Range.StartLine - 1, hint.Range.EndColumn + 1)

        {
            Content = content
            Location = location
            Tooltip = tooltip
        }

    let getFsDocument code =
        // I don't know, without this lib some symbols are just not loaded
        let options =
            { RoslynTestHelpers.DefaultProjectOptions with
                OtherOptions = [| "--targetprofile:netcore" |]
            }

        RoslynTestHelpers.CreateSolution(code, options = options)
        |> RoslynTestHelpers.GetSingleDocument

    let getFsiAndFsDocuments (fsiCode: string) (fsCode: string) =
        let projectId = ProjectId.CreateNewId()
        let projFilePath = "C:\\test.fsproj"

        let fsiDocInfo =
            RoslynTestHelpers.CreateDocumentInfo projectId "C:\\test.fsi" fsiCode

        let fsDocInfo = RoslynTestHelpers.CreateDocumentInfo projectId "C:\\test.fs" fsCode

        let projInfo =
            RoslynTestHelpers.CreateProjectInfo projectId projFilePath [ fsiDocInfo; fsDocInfo ]

        let solution = RoslynTestHelpers.CreateSolution [ projInfo ]
        let project = solution.Projects |> Seq.exactlyOne
        project.Documents

    let getHints (document: Document) hintKinds =
        let task =
            cancellableTask {
                let! ct = CancellableTask.getCurrentCancellationToken ()

                let getTooltip hint =
                    async {
                        let! roslynTexts = hint.GetTooltip document
                        return roslynTexts |> Seq.map (fun roslynText -> roslynText.Text) |> String.concat ""
                    }

                let! sourceText = document.GetTextAsync ct |> Async.AwaitTask
                let! hints = HintService.getHintsForDocument sourceText document hintKinds "test" ct
                let! tooltips = hints |> Seq.map getTooltip |> Async.Parallel
                return tooltips |> Seq.zip hints |> Seq.map convert
            }
            |> CancellableTask.start CancellationToken.None

        task.Result

    let getTypeHints document =
        getHints document (set [ HintKind.TypeHint ])

    let getReturnTypeHints document =
        getHints document (set [ HintKind.ReturnTypeHint ])

    let getParameterNameHints document =
        getHints document (set [ HintKind.ParameterNameHint ])

    let getAllHints document =
        let hintKinds =
            set [ HintKind.TypeHint; HintKind.ParameterNameHint; HintKind.ReturnTypeHint ]

        getHints document hintKinds
