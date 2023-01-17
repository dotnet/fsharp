// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests.Hints

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.Hints
open Hints
open FSharp.Editor.Tests.Helpers

module HintTestFramework =

    // another representation for extra convenience
    type TestHint =
        { Content: string; Location: int * int }

    let private convert hint =
        let content =
            hint.Parts |> Seq.map (fun hintPart -> hintPart.Text) |> String.concat ""

        // that's about different coordinate systems
        // in tests, the most convenient is the one used in editor,
        // hence this conversion
        let location = (hint.Range.StartLine - 1, hint.Range.EndColumn + 1)

        {
            Content = content
            Location = location
        }

    let getFsDocument (code: string) =
        // I don't know, without this lib some symbols are just not loaded
        let options = { RoslynTestHelpers.DefaultProjectOptions with OtherOptions = [| "--targetprofile:netcore" |] }
        RoslynTestHelpers.CreateSolution(code, options = options)
        |> RoslynTestHelpers.GetSingleDocument

    let getFsiAndFsDocuments (fsiCode: string) (fsCode: string) =
        let solution = RoslynTestHelpers.CreateSolution([|fsiCode; fsCode|])
        let project = solution.Projects |> Seq.exactlyOne
        project.Documents

    let getHints (document: Document) hintKinds =
        async {
            let! ct = Async.CancellationToken
            let! sourceText = document.GetTextAsync ct |> Async.AwaitTask
            let! hints = HintService.getHintsForDocument sourceText document hintKinds "test" ct
            return hints |> Seq.map convert
        }
        |> Async.RunSynchronously

    let getTypeHints document =
        getHints document (Set.empty.Add(HintKind.TypeHint))

    let getParameterNameHints document =
        getHints document (Set.empty.Add(HintKind.ParameterNameHint))

    let getAllHints document =
        let hintKinds = Set.empty.Add(HintKind.TypeHint).Add(HintKind.ParameterNameHint)
        getHints document hintKinds
