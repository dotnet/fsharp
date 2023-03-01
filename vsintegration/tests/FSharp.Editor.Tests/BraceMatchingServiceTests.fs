// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open Xunit
open Microsoft.CodeAnalysis.Text
open FSharp.Compiler.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Editor.Tests.Helpers

type BraceMatchingServiceTests() =
    let checker = FSharpChecker.Create()

    let fileName = "C:\\test.fs"

    member private this.VerifyNoBraceMatch(fileContents: string, marker: string) =
        let sourceText = SourceText.From(fileContents)
        let position = fileContents.IndexOf(marker)
        Assert.True(position >= 0, $"Cannot find marker '{marker}' in file contents")

        let parsingOptions, _ =
            checker.GetParsingOptionsFromProjectOptions RoslynTestHelpers.DefaultProjectOptions

        match
            FSharpBraceMatchingService.GetBraceMatchingResult(checker, sourceText, fileName, parsingOptions, position, "UnitTest")
            |> Async.RunImmediateExceptOnUI
        with
        | None -> ()
        | Some (left, right) -> failwith $"Found match for brace '{marker}'"

    member private this.VerifyBraceMatch(fileContents: string, startMarker: string, endMarker: string) =
        let sourceText = SourceText.From(fileContents)
        let startMarkerPosition = fileContents.IndexOf(startMarker)
        let endMarkerPosition = fileContents.IndexOf(endMarker)

        Assert.True(startMarkerPosition >= 0, $"Cannot find start marker '{startMarkerPosition}' in file contents")
        Assert.True(endMarkerPosition >= 0, $"Cannot find end marker '{endMarkerPosition}' in file contents")

        let parsingOptions, _ =
            checker.GetParsingOptionsFromProjectOptions RoslynTestHelpers.DefaultProjectOptions

        match
            FSharpBraceMatchingService.GetBraceMatchingResult(
                checker,
                sourceText,
                fileName,
                parsingOptions,
                startMarkerPosition,
                "UnitTest"
            )
            |> Async.RunImmediateExceptOnUI
        with
        | None -> failwith $"Didn't find a match for start brace at position '{startMarkerPosition}"
        | Some (left, right) ->
            let endPositionInRange (range) =
                let span = RoslynHelpers.FSharpRangeToTextSpan(sourceText, range)
                span.Start <= endMarkerPosition && endMarkerPosition <= span.End

            Assert.True(endPositionInRange (left) || endPositionInRange (right), "Found end match at incorrect position")

    [<Theory>]
    // Starting Brace
    [<InlineData("(marker1", ")marker1")>]
    [<InlineData("{marker2", "}marker2")>]
    [<InlineData("(marker3", ")marker3")>]
    [<InlineData("[marker4", "]marker4")>]
    // Ending Brace
    [<InlineData(")marker1", "(marker1")>]
    [<InlineData("}marker2", "{marker2")>]
    [<InlineData(")marker3", "(marker3")>]
    [<InlineData("]marker4", "[marker4")>]
    member this.NestedBrackets(startMarker: string, endMarker: string) =
        let code =
            "
            (marker1
            {marker2
            (marker3
            [marker4
            ]marker4
            )marker3
            }marker2
            )marker1"

        this.VerifyBraceMatch(code, startMarker, endMarker)

    [<Fact>]
    member this.BracketInExpression() =
        this.VerifyBraceMatch("let x = (3*5)-1", "(3*", ")-1")

    [<Fact>]
    member this.BraceInInterpolatedStringSimple() =
        this.VerifyBraceMatch("let x = $\"abc{1}def\"", "{1", "}def")

    [<Fact>]
    member this.BraceInInterpolatedStringWith3Dollars() =
        this.VerifyBraceMatch("let x = $$$\"\"\"abc{{{1}}}def\"\"\"", "{{{", "}}}")

    [<Theory>]
    [<InlineData("{{not")>]
    [<InlineData("}}match")>]
    [<InlineData("f{")>]
    [<InlineData("6}")>]
    member this.BraceNoMatchInNestedInterpolatedStrings(marker) =
        let source =
            "let x = $$$\"\"\"{{not a }}match
e{{{4$\"f{56}g\"}}}h
\"\"\""

        this.VerifyNoBraceMatch(source, marker)

    [<Theory>]
    [<InlineData("{{{23", "}}}d")>]
    [<InlineData("{{{4$", "}}}h")>]
    [<InlineData("{56", "}g")>]
    member this.BraceMatchInNestedInterpolatedStrings(startMark, endMark) =
        let source =
            "let x = $$$\"\"\"a{{{01}}}b --- c{{{23}}}d
e{{{4$\"f{56}g\"}}}h
\"\"\""

        this.VerifyBraceMatch(source, startMark, endMark)

    [<Fact>]
    member this.BraceInInterpolatedStringTwoHoles() =
        this.VerifyBraceMatch("let x = $\"abc{1}def{2+3}hij\"", "{2", "}hij")

    [<Fact>]
    member this.BraceInInterpolatedStringNestedRecord() =
        this.VerifyBraceMatch("let x = $\"abc{ id{contents=3}.contents }\"", "{contents", "}.contents")
        this.VerifyBraceMatch("let x = $\"abc{ id{contents=3}.contents }\"", "{ id", "}\"")

    [<Theory>]
    [<InlineData("[start")>]
    [<InlineData("]end")>]
    member this.BraceInMultiLineCommentShouldNotBeMatched(startMarker: string) =
        let code =
            "
            let x = 3
            (* This [start
            is a multiline
            comment ]end
            *)
            printf \"%d\" x"

        this.VerifyNoBraceMatch(code, startMarker)

    [<Fact>]
    member this.BraceInAttributesMatch() =
        let code =
            "
            [<Attribute>]
            module internal name"

        this.VerifyBraceMatch(code, "[<", ">]")

    [<Fact>]
    member this.BraceEncapsulatingACommentShouldBeMatched() =
        let code =
            "
            let x = 3 + (start
            (* this  is a comment *)
            )end"

        this.VerifyBraceMatch(code, "(start", ")end")

    [<Theory>]
    [<InlineData("(endsInComment")>]
    [<InlineData(")endsInComment")>]
    [<InlineData("<startsInComment")>]
    [<InlineData(">startsInComment")>]
    member this.BraceStartingOrEndingInCommentShouldNotBeMatched(startMarker: string) =
        let code =
            "
            let x = 123 + (endsInComment
            (* )endsInComment <startsInComment *)
            >startsInComment"

        this.VerifyNoBraceMatch(code, startMarker)

    [<Theory>]
    [<InlineData("(endsInDisabledCode")>]
    [<InlineData(")endsInDisabledCode")>]
    [<InlineData("<startsInDisabledCode")>]
    [<InlineData(">startsInDisabledCode")>]
    member this.BraceStartingOrEndingInDisabledCodeShouldNotBeMatched(startMarker: string) =
        let code =
            "
            let x = 123 + (endsInDisabledCode
            #if UNDEFINED
            )endsInDisabledCode <startsInDisabledCode
            #endif
            >startsInDisabledCode"

        this.VerifyNoBraceMatch(code, startMarker)

    [<Theory>]
    [<InlineData("(endsInString")>]
    [<InlineData(")endsInString")>]
    [<InlineData("<startsInString")>]
    [<InlineData(">startsInString")>]
    member this.BraceStartingOrEndingInStringShouldNotBeMatched(startMarker: string) =
        let code =
            "
            let x = \"stringValue\" + (endsInString +
            \" )endsInString <startsInString \" +
            + >startsInString"

        this.VerifyNoBraceMatch(code, startMarker)

    [<Fact>]
    member this.BraceMatchingAtEndOfLine_Bug1597() =
        // https://github.com/dotnet/fsharp/issues/1597
        let code =
            """
[<EntryPoint>]
let main argv = 
    let arg1 = ""
    let arg2 = ""
    let arg3 = ""
    (printfn "%A '%A' '%A'" (arg1) (arg2) (arg3))endBrace
    0 // return an integer exit code"""

        this.VerifyBraceMatch(code, "(printfn", ")endBrace")

    [<Theory>]
    [<InlineData("let a1 = [ 0 .. 100 ]", 9, 20)>]
    [<InlineData("let a2 = [| 0 .. 100 |]", 9, 10, 22)>]
    [<InlineData("let a3 = <@ 0 @>", 9, 10, 15)>]
    [<InlineData("let a4 = <@@ 0 @@>", 9, 10, 11, 15, 16)>]
    [<InlineData("let a6 = (  ()  )", 9, 16)>]
    [<InlineData("[<ReflectedDefinition>]\nlet a7 = 70", 0, 1, 22)>]
    [<InlineData("let a8 = seq { yield() }", 13, 23)>]
    member this.DoNotMatchOnInnerSide(fileContents: string, [<ParamArray>] matchingPositions: int[]) =
        let sourceText = SourceText.From(fileContents)

        let parsingOptions, _ =
            checker.GetParsingOptionsFromProjectOptions RoslynTestHelpers.DefaultProjectOptions

        for position in matchingPositions do
            match
                FSharpBraceMatchingService.GetBraceMatchingResult(checker, sourceText, fileName, parsingOptions, position, "UnitTest")
                |> Async.RunSynchronously
            with
            | Some _ -> ()
            | None ->
                match position with
                | 0 -> ""
                | _ -> fileContents.[position - 1] |> sprintf " (previous character '%c')"
                |> sprintf "Didn't find a matching brace at position '%d' %s" position
                |> raise (exn ())
