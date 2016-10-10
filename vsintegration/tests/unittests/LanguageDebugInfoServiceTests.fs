 // Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open System.Threading

open NUnit.Framework

open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.LanguageService

open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range

[<TestFixture>]
type LanguageDebugInfoServiceTests()  =
    let fileName = "C:\\test.fs"
    let defines = []
    let code = "
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
    static member private testCases: Object[][] = [|
        [| "123456";                    None |] // Numeric literals are not interesting
        [| "is a string";               Some("\"This is a string\"") |]
        [| "objectValue";               Some("objectValue") |]
        [| "exampleMember";             Some("objectValue.exampleMember") |]
        [| "%s";                        Some("\"%d %s %A\"") |]
    |]

    [<TestCaseSource("testCases")>]
    member this.TestDebugInfo(searchToken: string, expectedDataTip: string option) = 
        let searchPosition = code.IndexOf(searchToken)
        Assert.IsTrue(searchPosition >= 0, "SearchToken '{0}' is not found in code", searchToken)

        let sourceText = SourceText.From(code)
        let tokens = FSharpColorizationService.GetColorizationData(sourceText, TextSpan.FromBounds(0, sourceText.Length), Some(fileName), defines, CancellationToken.None)
        let actualDataTipSpanOption = FSharpLanguageDebugInfoService.GetDataTipInformation(sourceText, searchPosition, tokens)
        
        match actualDataTipSpanOption with
        | None -> Assert.IsTrue(expectedDataTip.IsNone, "LanguageDebugInfoService failed to produce a data tip")
        | Some(actualDataTipSpan) ->
            let actualDataTipText = sourceText.GetSubText(actualDataTipSpan).ToString()
            Assert.IsTrue(expectedDataTip.IsSome, "LanguageDebugInfoService produced a data tip while it shouldn't at: {0}", actualDataTipText)
            Assert.AreEqual(expectedDataTip.Value, actualDataTipText, "Expected and actual data tips should match")




    