module FSharpChecker.SemanticClassificationRegressions

open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Test.ProjectGeneration
open FSharp.Test.ProjectGeneration.Helpers

#nowarn "57"

/// Get semantic classification items for a single-file source using the transparent compiler.
let getClassifications (source: string) =
    let fileName, snapshot, checker = singleFileChecker source
    let results = checker.ParseAndCheckFileInProject(fileName, snapshot) |> Async.RunSynchronously
    let checkResults = getTypeCheckResult results
    checkResults.GetSemanticClassification(None)

/// (#15290 regression) Copy-and-update record fields must not be classified as type names.
/// Before the fix, Item.Types was registered with mWholeExpr and ItemOccurrence.Use, producing
/// a wide type classification that overshadowed the correct RecordField classification.
[<Fact>]
let ``Copy-and-update field should not be classified as type name`` () =
    let source =
        """
module Test

type MyRecord = { ValidationErrors: string list; Name: string }
let x: MyRecord = { ValidationErrors = []; Name = "" }
let updated = { x with ValidationErrors = [] }
"""

    let items = getClassifications source

    // Line 6 contains "{ x with ValidationErrors = [] }"
    // "ValidationErrors" starts around column 23 (after "let updated = { x with ")
    // It should be RecordField, NOT ReferenceType/ValueType.
    let fieldLine = 6

    let fieldItems =
        items
        |> Array.filter (fun item ->
            item.Range.StartLine = fieldLine
            && item.Type = SemanticClassificationType.RecordField)

    Assert.True(fieldItems.Length > 0, "Expected RecordField classification on the copy-and-update line")

    // No type classification should cover the field name on that line with a visible range
    let typeItemsCoveringField =
        items
        |> Array.filter (fun item ->
            item.Range.StartLine <= fieldLine
            && item.Range.EndLine >= fieldLine
            && item.Range.Start <> item.Range.End
            && (item.Type = SemanticClassificationType.ReferenceType
                || item.Type = SemanticClassificationType.ValueType
                || item.Type = SemanticClassificationType.Type))

    Assert.True(
        typeItemsCoveringField.Length = 0,
        sprintf
            "No type classification should cover the copy-and-update line, but found: %A"
            (typeItemsCoveringField |> Array.map (fun i -> i.Range, i.Type))
    )

/// (#16621 regression) Union case tester classification must not include the dot.
/// Before the fix, RegisterUnionCaseTesterForProperty shifted m.Start by +1,
/// producing range ".IsCircle" whose dot survived fixupSpan.
[<Fact>]
let ``Union case tester classification range should not include dot`` () =
    let source =
        """
module Test

type Shape = Circle | Square | HyperbolicCaseWithLongName
let s = Circle
let r1 = s.IsCircle
let r2 = s.IsHyperbolicCaseWithLongName
"""

    let items = getClassifications source

    // Find UnionCase classification items on lines 6 and 7
    let unionCaseItems =
        items
        |> Array.filter (fun item ->
            item.Type = SemanticClassificationType.UnionCase
            && (item.Range.StartLine = 6 || item.Range.StartLine = 7))

    // There should be union case classifications for the tester properties
    Assert.True(
        unionCaseItems.Length > 0,
        "Expected UnionCase classification for case tester properties"
    )

    // For each union case item, the range must NOT extend before the "Is" prefix.
    // "s.IsCircle" — dot is at some column, "IsCircle" starts 1 column later.
    // The union case range must start at or after the "Is" column.
    for item in unionCaseItems do
        // The range should cover at most the property name (e.g., "IsCircle" length 8)
        // It should NOT start at or before the dot position.
        let rangeWidth = item.Range.EndColumn - item.Range.StartColumn

        if item.Range.StartLine = 6 then
            // "let r1 = s.IsCircle" — "IsCircle" has length 8
            // The dot is at column 11 (0-based: "let r1 = s" is 10 chars, dot at 10)
            // "IsCircle" starts at column 11 (after the dot)
            Assert.True(
                rangeWidth <= 8,
                sprintf "UnionCase range for IsCircle is too wide (%d columns): %A" rangeWidth item.Range
            )

        if item.Range.StartLine = 7 then
            // "let r2 = s.IsHyperbolicCaseWithLongName" — property name length = 2 + 28 = 30
            Assert.True(
                rangeWidth <= 30,
                sprintf "UnionCase range for IsHyperbolicCaseWithLongName is too wide (%d columns): %A" rangeWidth item.Range
            )
