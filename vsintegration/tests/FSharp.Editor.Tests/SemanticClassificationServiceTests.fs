// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open Xunit
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Classification
open FSharp.Editor.Tests.Helpers
open FSharp.Test
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks

type SemanticClassificationServiceTests() =
    let getRanges (source: string) : SemanticClassificationItem list =
        asyncMaybe {
            let! ct = Async.CancellationToken |> liftAsync

            let document =
                RoslynTestHelpers.CreateSolution(source) |> RoslynTestHelpers.GetSingleDocument

            let! _, checkFileResults =
                document.GetFSharpParseAndCheckResultsAsync("SemanticClassificationServiceTests")
                |> CancellableTask.start ct

            return checkFileResults.GetSemanticClassification(None)
        }
        |> Async.RunSynchronously
        |> Option.toList
        |> List.collect Array.toList

    let verifyClassificationAtEndOfMarker (fileContents: string, marker: string, classificationType: string) =
        let text = SourceText.From(fileContents)
        let ranges = getRanges fileContents

        let line =
            text.Lines.GetLinePosition(fileContents.IndexOf(marker) + marker.Length - 1)

        let markerPos =
            Position.mkPos (Line.fromZ line.Line) (line.Character + marker.Length - 1)

        match ranges |> List.tryFind (fun item -> Range.rangeContainsPos item.Range markerPos) with
        | None -> failwith "Cannot find colorization data for end of marker"
        | Some item ->
            let actual = FSharpClassificationTypes.getClassificationTypeName item.Type

            actual
            |> Assert.shouldBeEqualWith
                classificationType
                $"Classification data doesn't match for end of marker: {classificationType} ≠ {actual} ({item.Type})"

    let verifyNoClassificationDataAtEndOfMarker (fileContents: string, marker: string, classificationType: string) =
        let text = SourceText.From(fileContents)
        let ranges = getRanges fileContents

        let line =
            text.Lines.GetLinePosition(fileContents.IndexOf(marker) + marker.Length - 1)

        let markerPos =
            Position.mkPos (Line.fromZ line.Line) (line.Character + marker.Length - 1)

        let anyData =
            ranges
            |> List.exists (fun item ->
                Range.rangeContainsPos item.Range markerPos
                && ((FSharpClassificationTypes.getClassificationTypeName item.Type) = classificationType))

        Assert.False(anyData, "Classification data was found when it wasn't expected.")

    [<Theory>]
    [<InlineData("(*1*)", ClassificationTypeNames.StructName)>]
    [<InlineData("(*2*)", ClassificationTypeNames.ClassName)>]
    [<InlineData("(*3*)", ClassificationTypeNames.StructName)>]
    [<InlineData("(*4*)", ClassificationTypeNames.ClassName)>]
    [<InlineData("(*5*)", ClassificationTypeNames.StructName)>]
    [<InlineData("(*6*)", ClassificationTypeNames.StructName)>]
    [<InlineData("(*7*)", ClassificationTypeNames.ClassName)>]
    member _.Measured_Types(marker: string, classificationType: string) =
        verifyClassificationAtEndOfMarker (
            """
                open System
                
                [<MeasureAnnotatedAbbreviation>] type (*1*)Guid<[<Measure>] 'm> = Guid
                [<MeasureAnnotatedAbbreviation>] type (*2*)string<[<Measure>] 'm> = string
                
                let inline cast<'a, 'b> (a : 'a) : 'b = (# "" a : 'b #)
                
                type Uom =
                    static member inline tag<[<Measure>]'m> (x : Guid) : (*3*)Guid<'m> = cast x
                    static member inline tag<[<Measure>]'m> (x : string) : (*4*)string<'m> = cast x
                
                type [<Measure>] Ms
                
                let i: (*5*)int<Ms> = 1<Ms>
                let g: (*6*)Guid<Ms> = Uom.tag Guid.Empty
                let s: (*7*)string<Ms> = Uom.tag "foo" """,
            marker,
            classificationType
        )

    [<Theory>]
    [<InlineData("(*1*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*2*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*3*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*4*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*5*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*6*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*7*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*8*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*9*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*10*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*11*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*12*)", FSharpClassificationTypes.MutableVar)>]
    member _.MutableValues(marker: string, classificationType: string) =
        let sourceText =
            """
type R1 = { mutable (*1*)Doop: int}
let r1 = { (*2*)Doop = 12 }
r1.Doop

let mutable (*3*)first = 12

printfn "%d" (*4*)first

let g ((*5*)xRef: outref<int>) = (*6*)xRef <- 12

let f() =
    let (*7*)second = &first
    let (*8*)third: outref<int> = &first
    printfn "%d%d" (*9*)second (*10*)third

type R = { (*11*)MutableField: int ref }
let r = { (*12*)MutableField = ref 12 }
r.MutableField
r.MutableField := 3
"""

        verifyClassificationAtEndOfMarker (sourceText, marker, classificationType)

    [<Theory>]
    [<InlineData("(*1*)", FSharpClassificationTypes.DisposableType)>]
    [<InlineData("(*2*)", FSharpClassificationTypes.DisposableTopLevelValue)>]
    [<InlineData("(*3*)", FSharpClassificationTypes.DisposableType)>]
    [<InlineData("(*4*)", FSharpClassificationTypes.DisposableTopLevelValue)>]
    [<InlineData("(*5*)", FSharpClassificationTypes.DisposableLocalValue)>]
    [<InlineData("(*6*)", FSharpClassificationTypes.DisposableType)>]
    [<InlineData("(*7*)", FSharpClassificationTypes.DisposableLocalValue)>]
    member _.Disposables(marker: string, classificationType: string) =
        let sourceText =
            """
open System

type (*1*)Disposable() =
  interface IDisposable with
    member _.Dispose() = ()

let (*2*)topLevel1 = new (*3*)Disposable()
let (*4*)topLevel2 = { new IDisposable with member _.Dispose() = () }

let f() =
  let (*5*)local1 = new (*6*)Disposable()
  let (*7*)local2 = { new IDisposable with member _.Dispose() = () }
  ()
"""

        verifyClassificationAtEndOfMarker (sourceText, marker, classificationType)

    [<Theory>]
    [<InlineData("(*1*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*2*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*3*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*4*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*5*)", FSharpClassificationTypes.MutableVar)>]
    [<InlineData("(*6*)", FSharpClassificationTypes.MutableVar)>]
    member _.NoInrefsExpected(marker: string, classificationType: string) =
        let sourceText =
            """
let f (item: (*1*)inref<int>) = printfn "%d" (*2*)item
let g() =
    let x = 1
    let y = 2
    let (*3*)xRef = &x
    let (*4*)yRef: inref<int> = &y
    f (*5*)&xRef
    f (*6*)&yRef
"""

        verifyNoClassificationDataAtEndOfMarker (sourceText, marker, classificationType)

    [<Theory>]
    [<InlineData("(*1*)", ClassificationTypeNames.Keyword)>]
    [<InlineData("(*2*)", ClassificationTypeNames.Keyword)>]
    [<InlineData("(*3*)", ClassificationTypeNames.Keyword)>]
    [<InlineData("(*4*)", ClassificationTypeNames.LocalName)>]
    [<InlineData("(*5*)", ClassificationTypeNames.LocalName)>]
    [<InlineData("(*6*)", ClassificationTypeNames.LocalName)>]
    [<InlineData("(*7*)", ClassificationTypeNames.Identifier)>]
    [<InlineData("(*8*)", ClassificationTypeNames.Identifier)>]
    [<InlineData("(*9*)", ClassificationTypeNames.ClassName)>]
    [<InlineData("(*10*)", ClassificationTypeNames.ClassName)>]
    [<InlineData("(*11*)", ClassificationTypeNames.ClassName)>]
    [<InlineData("(*12*)", ClassificationTypeNames.ClassName)>]
    [<InlineData("(*13*)", ClassificationTypeNames.ClassName)>]
    [<InlineData("(*14*)", ClassificationTypeNames.TypeParameterName)>]
    [<InlineData("(*15*)", ClassificationTypeNames.TypeParameterName)>]
    [<InlineData("(*16*)", ClassificationTypeNames.Keyword)>]
    [<InlineData("(*17*)", ClassificationTypeNames.Keyword)>]
    [<InlineData("(*18*)", ClassificationTypeNames.Keyword)>]
    member _.``nameof ident, nameof<'T>, match … with nameof ident``(marker: string, classificationType: string) =
        let sourceText =
            """
module ``Normal usage of nameof should show up as a keyword`` =
    let f x = (*1*)nameof x
    let g (x : 'T) = (*2*)nameof<'T>
    let h x y = match x with (*3*)nameof y -> () | _ -> ()

module ``Redefined nameof should shadow the intrinsic one`` =
    let a x = match x with (*4*)nameof -> ()
    let b (*5*)nameof = (*6*)nameof
    let (*7*)nameof = "redefined"
    let _ = (*8*)nameof

    type (*9*)nameof () = class end
    let _ = (*10*)nameof ()
    let _ = new (*11*)nameof ()

    module (*12*)nameof =
        let f x = x

    let _ = (*13*)nameof.f 3

    let c (x : '(*14*)nameof) = x
    let d (x : (*15*)'nameof) = x

module ``It should still show up as a keyword even if the type parameter is invalid`` =
    let _ = (*16*)nameof<>
    let a (x : 'a) (y : 'b) = (*17*)nameof<'c> // FS0039: The type parameter 'c is not defined.
    let _ = (*18*)nameof<int> // FS3250: Expression does not have a name.
"""

        verifyClassificationAtEndOfMarker (sourceText, marker, classificationType)

    [<Fact>]
    member _.``Optional parameters should be classified correctly``() =
        let sourceText =
            """
type TestType() =
    member _.memb(?optional:string) = optional
"""

        let ranges = getRanges sourceText

        // The issue was that QuickParse returning None for '?' caused misclassification
        // This test verifies that we get semantic classification data and nothing is
        // incorrectly classified as a type or namespace due to the ? prefix

        // Look for any identifier "optional" in the classifications
        let text = SourceText.From(sourceText)

        let optionalRanges =
            ranges
            |> List.filter (fun item ->
                try
                    // Get the actual text from the source using SourceText
                    let span = RoslynHelpers.TryFSharpRangeToTextSpan(text, item.Range)

                    match span with
                    | ValueSome textSpan ->
                        let actualText = text.GetSubText(textSpan).ToString()
                        actualText = "optional"
                    | ValueNone -> false
                with _ ->
                    false)

        // Provide detailed diagnostics if test fails
        let allClassifications =
            ranges
            |> List.map (fun item ->
                try
                    let span = RoslynHelpers.TryFSharpRangeToTextSpan(text, item.Range)

                    let textStr =
                        match span with
                        | ValueSome ts -> text.GetSubText(ts).ToString()
                        | ValueNone -> "[no span]"

                    sprintf "Range %A: '%s' (%A)" item.Range textStr item.Type
                with ex ->
                    sprintf "Range %A: [error: %s] (%A)" item.Range ex.Message item.Type)
            |> String.concat "\n"

        let errorMessage =
            sprintf
                "Should have classification data for 'optional' identifier.\nFound %d ranges total.\nAll classifications:\n%s"
                ranges.Length
                allClassifications

        Assert.True(optionalRanges.Length > 0, errorMessage)

        // Verify that none of the "optional" occurrences are classified as type/namespace
        // (which would indicate the bug is present)
        for optionalRange in optionalRanges do
            let classificationType =
                FSharpClassificationTypes.getClassificationTypeName optionalRange.Type

            Assert.NotEqual<string>(ClassificationTypeNames.ClassName, classificationType)
            Assert.NotEqual<string>(ClassificationTypeNames.NamespaceName, classificationType)

    [<Fact>]
    member _.``Copy-and-update field should not be classified as type name``() =
        let sourceText =
            """
type MyRecord = { ValidationErrors: string list; Name: string }
let x = { ValidationErrors = []; Name = "" }
let updated = { x with (*1*)ValidationErrors = [] }

[<Struct>]
type StructRecord = { Count: int; Label: string }
let sr = { Count = 0; Label = "" }
let sr2 = { sr with (*2*)Count = 1 }
"""

        let text = SourceText.From(sourceText)
        let ranges = getRanges sourceText

        // DEBUG: Print all classifications around (*1*)
        let line1 = text.Lines.GetLinePosition(sourceText.IndexOf("(*1*)") + 5)
        let markerPos1 = Position.mkPos (Line.fromZ line1.Line) (line1.Character + 1)

        let overlappingRanges1 =
            ranges |> List.filter (fun item -> Range.rangeContainsPos item.Range markerPos1)

        printfn "=== Classifications overlapping with (*1*) at position %A ===" markerPos1

        for item in overlappingRanges1 do
            let classificationType =
                FSharpClassificationTypes.getClassificationTypeName item.Type

            printfn "  Range: %A, Type: %s (%A)" item.Range classificationType item.Type

        if List.isEmpty overlappingRanges1 then
            printfn "  (No classifications found)"

        // The field should be classified as PropertyName (RecordField), not as a type name.
        // Before the fix, Item.Types was registered with mWholeExpr and ItemOccurrence.Use,
        // causing the entire copy-and-update range to get a type classification that
        // overshadowed the correct RecordField classification at the field position.
        verifyClassificationAtEndOfMarker (sourceText, "(*1*)", ClassificationTypeNames.PropertyName)
        verifyNoClassificationDataAtEndOfMarker (sourceText, "(*1*)", ClassificationTypeNames.ClassName)
        // Also verify struct record copy-and-update
        verifyClassificationAtEndOfMarker (sourceText, "(*2*)", ClassificationTypeNames.PropertyName)
        verifyNoClassificationDataAtEndOfMarker (sourceText, "(*2*)", ClassificationTypeNames.StructName)

    [<Fact>]
    member _.``Union case tester property range should not include dot``() =
        let sourceText =
            """
type Shape = Circle | Square | HyperbolicCaseWithLongName
let s = Circle
let result = s.(*1*)IsCircle
let result2 = s.(*2*)IsHyperbolicCaseWithLongName
"""

        let ranges = getRanges sourceText
        let text = SourceText.From(sourceText)

        // Find the dot position in "s.IsCircle"
        let dotIdx = sourceText.IndexOf("s.(*1*)IsCircle") + 1
        let dotLine = text.Lines.GetLinePosition(dotIdx)
        let dotPos = Position.mkPos (Line.fromZ dotLine.Line) dotLine.Character

        // There should be a UnionCase (EnumName) classification covering IsCircle
        let isCirclePos =
            let idx = sourceText.IndexOf("(*1*)IsCircle") + "(*1*)".Length
            let linePos = text.Lines.GetLinePosition(idx)
            Position.mkPos (Line.fromZ linePos.Line) linePos.Character

        let unionCaseAtIdentifier =
            ranges
            |> List.filter (fun item ->
                FSharpClassificationTypes.getClassificationTypeName item.Type = ClassificationTypeNames.EnumName
                && Range.rangeContainsPos item.Range isCirclePos)

        Assert.True(unionCaseAtIdentifier.Length > 0, "Expected a UnionCase classification covering 'IsCircle'")

        // No UnionCase classification should include the dot position.
        // Before the fix, the identifier range was computed by shifting m.Start by +1,
        // producing ".IsCircle" — the dot at index 0 survived fixupSpan and got UnionCase color.
        let unionCaseAtDot =
            ranges
            |> List.filter (fun item ->
                FSharpClassificationTypes.getClassificationTypeName item.Type = ClassificationTypeNames.EnumName
                && Range.rangeContainsPos item.Range dotPos)

        Assert.True(
            unionCaseAtDot.IsEmpty,
            sprintf
                "UnionCase classification should not include the dot, but found items with ranges: %A"
                (unionCaseAtDot |> List.map (fun i -> i.Range))
        )

        // Also verify the long case name has a UnionCase (EnumName) classification.
        // Use explicit filter instead of verifyClassificationAtEndOfMarker, because both
        // Property and UnionCase classifications overlap at the same position.
        let longCasePos =
            let idx = sourceText.IndexOf("(*2*)IsHyperbolicCaseWithLongName") + "(*2*)".Length
            let linePos = text.Lines.GetLinePosition(idx)
            Position.mkPos (Line.fromZ linePos.Line) linePos.Character

        let longCaseUnionItems =
            ranges
            |> List.filter (fun item ->
                FSharpClassificationTypes.getClassificationTypeName item.Type = ClassificationTypeNames.EnumName
                && Range.rangeContainsPos item.Range longCasePos)

        Assert.True(longCaseUnionItems.Length > 0, "Expected a UnionCase classification covering 'IsHyperbolicCaseWithLongName'")
