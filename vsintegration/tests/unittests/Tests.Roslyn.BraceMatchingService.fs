// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
namespace Microsoft.VisualStudio.FSharp.Editor.Tests.Roslyn

open System
open System.Threading

open NUnit.Framework

open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor

[<TestFixture>]
type BraceMatchingServiceTests()  =

    member private this.VerifyNoBraceMatch(fileContents: string, marker: string) =
        let markerPosition = fileContents.IndexOf(marker)
        Assert.IsTrue(markerPosition >= 0, "Cannot find marker '{0}' in file contents", marker)

        match FSharpBraceMatchingService.FindMatchingBrace(SourceText.From(fileContents), Some("test.fs"), [], markerPosition, CancellationToken.None) with
        | None -> ()
        | Some(foundMatch) -> Assert.Fail("Found match for brace at position '{0}'", foundMatch)

    member private this.VerifyBraceMatch(fileContents: string, startMarker: string, endMarker: string) =
        let startMarkerPosition = fileContents.IndexOf(startMarker)
        Assert.IsTrue(startMarkerPosition >= 0, "Cannot find start marker '{0}' in file contents", startMarkerPosition)
        let endMarkerPosition = fileContents.IndexOf(endMarker)
        Assert.IsTrue(endMarkerPosition >= 0, "Cannot find end marker '{0}' in file contents", endMarkerPosition)

        match FSharpBraceMatchingService.FindMatchingBrace(SourceText.From(fileContents), Some("test.fs"), [], startMarkerPosition, CancellationToken.None) with
        | None -> Assert.Fail("Didn't find a match for brace at position '{0}", startMarkerPosition)
        | Some(foundMatch) -> Assert.AreEqual(endMarkerPosition, foundMatch, "Found match at incorrect position")
        

    // Starting Brace
    [<TestCase("(marker1", ")marker1")>]
    [<TestCase("{marker2", "}marker2")>]
    [<TestCase("(marker3", ")marker3")>]
    [<TestCase("[marker4", "]marker4")>]
    [<TestCase("<marker5", ">marker5")>]
    // Ending Brace
    [<TestCase(")marker1", "(marker1")>]
    [<TestCase("}marker2", "{marker2")>]
    [<TestCase(")marker3", "(marker3")>]
    [<TestCase("]marker4", "[marker4")>]
    [<TestCase(">marker5", "<marker5")>]
    member this.NestedBrackets(startMarker: string, endMarker: string) = 
        let code = "
            (marker1
            {marker2
            (marker3
            [marker4
            <marker5
            >marker5
            ]marker4
            )marker3
            }marker2
            )marker1"
        this.VerifyBraceMatch(code, startMarker, endMarker)

    [<Test>]
    member this.BracketInExpression() = 
        this.VerifyBraceMatch("let x = (3*5)-1", "(3*", ")-1")

    [<TestCase("[start")>]
    [<TestCase("]end")>]
    member this.BraceInMultiLineCommentShouldNotBeMatched(startMarker: string) = 
        let code = "
            let x = 3
            (* This [start
            is a multiline
            comment ]end
            *)
            printf \"%d\" x"
        this.VerifyNoBraceMatch(code, startMarker)
        
    [<Test>]
    member this.BraceEncapsulatingACommentShouldBeMatched() = 
        let code = "
            let x = 3 + (start
            (* this  is a comment *)
            )end"
        this.VerifyBraceMatch(code, "(start", ")end")
        
    [<TestCase("(endsInComment")>]
    [<TestCase(")endsInComment")>]
    [<TestCase("<startsInComment")>]
    [<TestCase("<startsInComment")>]
    member this.BraceStartingOrEndingInCommentShouldNotBeMatched(startMarker: string) = 
        let code = "
            let x = 123 + (endsInComment
            (* )endsInComment <startsInComment *)
            >startsInComment"
        this.VerifyNoBraceMatch(code, startMarker)
        
    [<TestCase("(endsInDisabledCode")>]
    [<TestCase(")endsInDisabledCode")>]
    [<TestCase("<startsInDisabledCode")>]
    [<TestCase("<startsInDisabledCode")>]
    member this.BraceStartingOrEndingInDisabledCodeShouldNotBeMatched(startMarker: string) = 
        let code = "
            let x = 123 + (endsInDisabledCode
            #if UNDEFINED
            )endsInDisabledCode <startsInDisabledCode
            #endif
            <startsInDisabledCode"
        this.VerifyNoBraceMatch(code, startMarker)
        
    [<TestCase("(endsInString")>]
    [<TestCase(")endsInString")>]
    [<TestCase("<startsInString")>]
    [<TestCase(">startsInString")>]
    member this.BraceStartingOrEndingInStringShouldNotBeMatched(startMarker: string) = 
        let code = "
            let x = \"stringValue\" + (endsInString +
            \" )endsInString <startsInString \" +
            + >startsInString"
        this.VerifyNoBraceMatch(code, startMarker)