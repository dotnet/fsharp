// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open FSharp.Editor.Tests.Helpers

module DocumentHighlightsServiceTests =

    open Xunit
    open Microsoft.CodeAnalysis
    open Microsoft.CodeAnalysis.Text
    open Microsoft.VisualStudio.FSharp.Editor
    open FSharp.Compiler.Text

    let filePath = "C:\\test0.fs"

    let private getSpans (fileContents: string) (caretPosition: int) =
        let document = RoslynTestHelpers.CreateSingleDocumentSolution(filePath, fileContents)

        FSharpDocumentHighlightsService.GetDocumentHighlights(document, caretPosition)
        |> Async.RunSynchronously
        |> Option.defaultValue [||]

    let private span sourceText isDefinition (startLine, startCol) (endLine, endCol) =
        let range =
            Range.mkRange filePath (Position.mkPos startLine startCol) (Position.mkPos endLine endCol)

        {
            IsDefinition = isDefinition
            TextSpan = RoslynHelpers.FSharpRangeToTextSpan(sourceText, range)
        }

    [<Fact>]
    let ShouldHighlightAllSimpleLocalSymbolReferences () =
        let fileContents =
            """
    let foo x = 
        x + x
    let y = foo 2
    """

        let sourceText = SourceText.From(fileContents)
        let caretPosition = fileContents.IndexOf("foo") + 1
        let spans = getSpans fileContents caretPosition

        let expected =
            [| span sourceText true (2, 8) (2, 11); span sourceText false (4, 12) (4, 15) |]

        Assert.Equal<FSharpHighlightSpan array>(expected, spans)

    [<Fact>]
    let ShouldHighlightAllQualifiedSymbolReferences () =
        let fileContents =
            """
    let x = System.DateTime.Now
    let y = System.DateTime.MaxValue
    """

        let sourceText = SourceText.From(fileContents)
        let caretPosition = fileContents.IndexOf("DateTime") + 1
        let _ = DocumentId.CreateNewId(ProjectId.CreateNewId())

        let spans = getSpans fileContents caretPosition

        let expected =
            [|
                span sourceText false (2, 19) (2, 27)
                span sourceText false (3, 19) (3, 27)
            |]

        Assert.Equal<FSharpHighlightSpan array>(expected, spans)

        let caretPosition = fileContents.IndexOf("Now") + 1
        let _ = DocumentId.CreateNewId(ProjectId.CreateNewId())
        let spans = getSpans fileContents caretPosition
        let expected = [| span sourceText false (2, 28) (2, 31) |]

        Assert.Equal<FSharpHighlightSpan array>(expected, spans)
