// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open System.Threading
open Xunit
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.Text

type LanguageDebugInfoServiceTests() =
    let fileName = "C:\\test.fs"
    let defines = []

    let code =
        "
// This is a comment

[<EntryPoint>]
let main argv = 
    let integerValue = 123456
    let stringValue = \"This is a string\"
    let objectValue = exampleType(789)

    printfn \"%d %s %A\" integerValue stringValue objectValue

    let booleanValue = true
    match booleanValue with
    | true -> printfn \"correct\"
    | false -> printfn \"%d\" objectValue.exampleMember

    0 // return an integer exit code
    "

    static member testCases: Object[][] =
        [|
            [| "123456"; None |] // Numeric literals are not interesting
            [| "is a string"; Some("\"This is a string\"") |]
            [| "objectValue"; Some("objectValue") |]
            [| "exampleMember"; Some("objectValue.exampleMember") |]
            [| "%s"; Some("\"%d %s %A\"") |]
        |]

    [<Theory>]
    [<MemberData(nameof (LanguageDebugInfoServiceTests.testCases))>]
    member this.TestDebugInfo(searchToken: string, expectedDataTip: string option) =
        let searchPosition = code.IndexOf(searchToken)
        Assert.True(searchPosition >= 0, $"SearchToken '{searchToken}' is not found in code")

        let sourceText = SourceText.From(code)
        let documentId = DocumentId.CreateNewId(ProjectId.CreateNewId())

        let classifiedSpans = ResizeArray<_>()

        Tokenizer.classifySpans (
            documentId,
            sourceText,
            TextSpan.FromBounds(0, sourceText.Length),
            Some(fileName),
            defines,
            None,
            None,
            classifiedSpans,
            CancellationToken.None
        )

        let actualDataTipSpanOption =
            FSharpLanguageDebugInfoService.GetDataTipInformation(sourceText, searchPosition, classifiedSpans)

        match actualDataTipSpanOption with
        | None -> Assert.True(expectedDataTip.IsNone, "LanguageDebugInfoService failed to produce a data tip")
        | Some (actualDataTipSpan) ->
            let actualDataTipText = sourceText.GetSubText(actualDataTipSpan).ToString()

            Assert.True(expectedDataTip.IsSome, $"LanguageDebugInfoService produced a data tip while it shouldn't at: {actualDataTipText}")

            Assert.Equal(expectedDataTip.Value, actualDataTipText)
