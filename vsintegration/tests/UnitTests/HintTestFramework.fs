// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace VisualFSharp.UnitTests.Editor.Hints

open System.Threading
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.Hints
open Microsoft.VisualStudio.FSharp.Editor.Hints.HintService
open VisualFSharp.UnitTests.Editor
open Microsoft.CodeAnalysis.Text

module HintTestFramework =

// another representation for extra convenience
type TestHint = {
    Content: string
    Location: int * int
}

let private convert hint = 
    let content = 
        hint.Parts
        |> Seq.map (fun hintPart -> hintPart.Text)
        |> String.concat ""

    // that's about different coordinate systems
    // in tests, the most convenient is the one used in editor,
    // hence this conversion
    let location = (hint.Range.StartLine - 1, hint.Range.EndColumn + 1)

    { Content = content 
      Location = location }

let getFsDocument code = 
    use project = SingleFileProject code
    let fileName = fst project.Files.Head
    let document, _ = RoslynTestHelpers.CreateSingleDocumentSolution(fileName, code)
    document

let getFsiAndFsDocuments (fsiCode: string) (fsCode: string) = 
    RoslynTestHelpers.CreateTwoDocumentSolution(
        "test.fsi",
        SourceText.From fsiCode,
        "test.fs",
        SourceText.From fsCode)

let getHints document = 
    task {
        let! hints = HintService.getHintsForDocument document "test" CancellationToken.None
        return hints |> Seq.map convert
    }
    |> Async.AwaitTask
    |> Async.RunSynchronously
