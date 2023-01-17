// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open Xunit
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open Microsoft.CodeAnalysis.Formatting
open FSharp.Editor.Tests.Helpers

type EditorFormattingServiceTests() =
    let filePath = "C:\\test0.fs"

    let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())
    let indentStyle = FormattingOptions.IndentStyle.Smart

    let indentTemplate =
        """
let foo = [
    15
    ]marker1

async {
    return 10
    }marker2

let abc =
    [|
        10
        |]marker3

let def =
    (
        "hi"
        )marker4
"""

    let pasteTemplate =
        """

let foo =
    printfn "Something here"
    marker1

marker2

            marker3

marker4"""

    [<Theory>]
    [<InlineData("marker1", "]")>]
    [<InlineData("marker2", "}")>]
    [<InlineData("marker3", "    |]")>]
    [<InlineData("marker4", "    )")>]
    member this.TestIndentation(marker: string, expectedLine: string) =
        let checker = FSharpChecker.Create()
        let position = indentTemplate.IndexOf(marker)
        Assert.True(position >= 0, "Precondition failed: unable to find marker in template")

        let sourceText = SourceText.From(indentTemplate)

        let lineNumber =
            sourceText.Lines |> Seq.findIndex (fun line -> line.Span.Contains position)

        let parsingOptions, _ = checker.GetParsingOptionsFromProjectOptions RoslynTestHelpers.DefaultProjectOptions

        let changesOpt =
            FSharpEditorFormattingService.GetFormattingChanges(
                documentId,
                sourceText,
                filePath,
                checker,
                indentStyle,
                parsingOptions,
                position
            )
            |> Async.RunSynchronously

        match changesOpt with
        | None -> failwith "Expected a text change, but got None"
        | Some changes ->
            let changedText = sourceText.WithChanges(changes)
            let lineText = changedText.Lines.[lineNumber].ToString()
            Assert.True(lineText.StartsWith(expectedLine), "Changed line does not start with expected text")

    [<Theory>]
    [<InlineData(true, "")>]
    [<InlineData(true, "        ")>]
    [<InlineData(false, "")>]
    member this.TestPasteChanges_PastingOntoIndentedLine(enabled: bool, prefix: string) =
        let checker = FSharpChecker.Create()
        let parsingOptions, _ = checker.GetParsingOptionsFromProjectOptions RoslynTestHelpers.DefaultProjectOptions

        let clipboard =
            prefix
            + """[<Class>]
        type SomeNameHere () =
            member _.Test ()"""

        let start =
            """

let foo =
    printfn "Something here"
    $

somethingElseHere
"""

        let expected =
            """

let foo =
    printfn "Something here"
    [<Class>]
    type SomeNameHere () =
        member _.Test ()

somethingElseHere
"""

        let sourceText = SourceText.From(start.Replace("$", clipboard))
        let span = TextSpan(start.IndexOf '$', clipboard.Length)

        let formattingOptions = { FormatOnPaste = enabled }

        let changesOpt =
            FSharpEditorFormattingService.GetPasteChanges(
                documentId,
                sourceText,
                filePath,
                formattingOptions,
                4,
                parsingOptions,
                clipboard,
                span
            )
            |> Async.RunSynchronously
            |> Option.map List.ofSeq

        if enabled then
            match changesOpt with
            | Some changes ->
                let changedText = sourceText.WithChanges(changes).ToString()
                Assert.Equal(expected, changedText)
            | _ -> failwithf "Expected text changes, but got %+A" changesOpt
        else
            changesOpt |> Assert.shouldBeEqualWith None "Expected no changes as FormatOnPaste is disabled"

    [<Theory>]
    [<InlineData "">]
    [<InlineData "        ">]
    member this.TestPasteChanges_PastingOntoEmptyLine(prefix: string) =
        let checker = FSharpChecker.Create()
        let parsingOptions, _ = checker.GetParsingOptionsFromProjectOptions RoslynTestHelpers.DefaultProjectOptions

        let clipboard =
            prefix
            + """[<Class>]
        type SomeNameHere () =
            member _.Test ()"""

        let start =
            """
$

let foo =
    printfn "Something here"

somethingElseHere
"""

        let expected =
            """
[<Class>]
type SomeNameHere () =
    member _.Test ()

let foo =
    printfn "Something here"

somethingElseHere
"""

        let sourceText = SourceText.From(start.Replace("$", clipboard))
        let span = TextSpan(start.IndexOf '$', clipboard.Length)

        let formattingOptions = { FormatOnPaste = true }

        let changesOpt =
            FSharpEditorFormattingService.GetPasteChanges(
                documentId,
                sourceText,
                filePath,
                formattingOptions,
                4,
                parsingOptions,
                clipboard,
                span
            )
            |> Async.RunSynchronously
            |> Option.map List.ofSeq

        match changesOpt with
        | Some changes ->
            let changedText = sourceText.WithChanges(changes).ToString()
            Assert.Equal(expected, changedText)
        | _ -> failwithf "Expected a changes, but got %+A" changesOpt

    [<Fact>]
    member this.TestPasteChanges_PastingWithAutoIndentationInPasteSpan() =
        let checker = FSharpChecker.Create()
        let parsingOptions, _ = checker.GetParsingOptionsFromProjectOptions RoslynTestHelpers.DefaultProjectOptions

        let clipboard =
            """[<Class>]
        type SomeNameHere () =
            member _.Test ()"""

        let start =
            """

let foo =
    printfn "Something here"
    $

somethingElseHere
"""

        let expected =
            """

let foo =
    printfn "Something here"
    [<Class>]
    type SomeNameHere () =
        member _.Test ()

somethingElseHere
"""

        let sourceText = SourceText.From(start.Replace("$", clipboard))

        // If we're pasting on an empty line which has been automatically indented,
        // then the pasted span includes this automatic indentation. Check that we
        // still format as expected
        let span = TextSpan(start.IndexOf '$' - 4, clipboard.Length + 4)

        let formattingOptions = { FormatOnPaste = true }

        let changesOpt =
            FSharpEditorFormattingService.GetPasteChanges(
                documentId,
                sourceText,
                filePath,
                formattingOptions,
                4,
                parsingOptions,
                clipboard,
                span
            )
            |> Async.RunSynchronously
            |> Option.map List.ofSeq

        match changesOpt with
        | Some changes ->
            let changedText = sourceText.WithChanges(changes).ToString()
            Assert.Equal(expected, changedText)
        | _ -> failwithf "Expected a changes, but got %+A" changesOpt
