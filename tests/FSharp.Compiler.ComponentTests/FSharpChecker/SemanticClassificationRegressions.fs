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
    checkResults.GetSemanticClassification(None, RelatedSymbolUseKind.All)

/// Extract the source substring covered by a classification item's range (single-line ranges).
let private substringOfRange (source: string) (r: Range) =
    let lines = source.Replace("\r\n", "\n").Split('\n')
    let line = lines[r.StartLine - 1]
    line.Substring(r.StartColumn, r.EndColumn - r.StartColumn)

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

/// (#16621) Helper: assert UnionCase classifications on expected lines.
/// Each entry is (line, expectedCount, maxRangeWidth).
/// maxRangeWidth guards against dot-coloring regressions (range including "x." prefix).
let expectUnionCaseClassifications source (expectations: (int * int * int) list) =
    let items = getClassifications source

    for (line, expectedCount, maxWidth) in expectations do
        let found =
            items
            |> Array.filter (fun item ->
                item.Type = SemanticClassificationType.UnionCase
                && item.Range.StartLine = line)

        Assert.True(
            found.Length = expectedCount,
            sprintf "Line %d: expected %d UnionCase classification(s), got %d. Items on that line: %A" line expectedCount found.Length
                (items
                 |> Array.filter (fun i -> i.Range.StartLine = line)
                 |> Array.map (fun i -> i.Range.StartColumn, i.Range.EndColumn, i.Type))
        )

        for item in found do
            let width = item.Range.EndColumn - item.Range.StartColumn

            Assert.True(
                width <= maxWidth,
                sprintf "Line %d: UnionCase range is too wide (%d columns, max %d): %A" line width maxWidth item.Range
            )

/// (#16621 regression) Union case tester classification must not include the dot.
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
    //                        line, count, maxWidth
    expectUnionCaseClassifications source [ (6, 1, 8); (7, 1, 30) ]

/// (#16621) Union case tester classification across scenarios: chaining, RequireQualifiedAccess,
/// multiple testers on one line, and self-referential members.
[<Fact>]
let ``Union case tester classification across scenarios`` () =
    let source =
        """
module Test

type Shape = Circle | Square
let s = Circle
let chained = s.IsCircle.ToString()
let both = s.IsCircle && s.IsSquare

[<RequireQualifiedAccess>]
type Token = Ident of string | Keyword
let t = Token.Keyword
let rqa = t.IsIdent

type Animal =
    | Cat
    | Dog
    member this.IsFeline = this.IsCat
"""
    //                        line, count, maxWidth
    expectUnionCaseClassifications source
        [ (6, 1, 8)    // s.IsCircle.ToString() — chained
          (7, 2, 8)    // s.IsCircle && s.IsSquare — two on same line
          (12, 1, 7)   // t.IsIdent — RequireQualifiedAccess
          (17, 1, 5) ] // this.IsCat — self-referential member

/// (#18009 regression) Static method on a generic type with a *qualified* type argument
/// must still classify the type name as a type.
[<Fact>]
let ``Static method on generic type should classify type name as type`` () =
    let source =
        """
module Test

type MyType<'T> =
    static member S = 1

let _ = MyType<int>.S
let _ = MyType<System.Int32>.S
"""

    let items = getClassifications source

    let isMyTypeRefOnLine line (item: SemanticClassificationItem) =
        item.Type = SemanticClassificationType.ReferenceType
        && item.Range.StartLine = line
        && item.Range.StartColumn = 8
        && item.Range.EndColumn = 14

    let unqualified = items |> Array.filter (isMyTypeRefOnLine 7)
    Assert.True(
        unqualified.Length = 1,
        sprintf
            "Expected exactly one ReferenceType classification for MyType on line 7, got: %A"
            (items |> Array.filter (fun i -> i.Range.StartLine = 7)
                   |> Array.map (fun i -> i.Range.StartColumn, i.Range.EndColumn, i.Type))
    )

    let qualified = items |> Array.filter (isMyTypeRefOnLine 8)
    Assert.True(
        qualified.Length = 1,
        sprintf
            "Expected exactly one ReferenceType classification for MyType on line 8, got: %A"
            (items |> Array.filter (fun i -> i.Range.StartLine = 8)
                   |> Array.map (fun i -> i.Range.StartColumn, i.Range.EndColumn, i.Type))
    )

/// (#18009 follow-up) Accepting ItemOccurrence.InvalidUse in LegitTypeOccurrence must
/// not cause unresolved identifiers to be classified as types.
[<Fact>]
let ``Undeclared identifier in expression position is not classified as a type`` () =
    let source =
        """
module Test

let _ = NotDeclaredAnywhere.S
"""

    let items = getClassifications source

    let badSpans =
        items
        |> Array.filter (fun item ->
            item.Range.StartLine = 4
            && item.Range.StartColumn = 8
            && item.Range.EndColumn = 27
            && (item.Type = SemanticClassificationType.ReferenceType
                || item.Type = SemanticClassificationType.ValueType
                || item.Type = SemanticClassificationType.Type))

    Assert.True(
        badSpans.Length = 0,
        sprintf
            "Undeclared identifier should not be classified as a type, but found: %A"
            (badSpans |> Array.map (fun i -> i.Range.StartColumn, i.Range.EndColumn, i.Type))
    )

/// (#16982) Delegate `Invoke` synthesized in a delegate declaration must not be classified as Method.
[<Fact>]
let ``Delegate Invoke in declaration not classified as method`` () =
    let source = """
type MyDelegate = delegate of int -> string
"""
    let classifications = getClassifications source
    let invokeMethods =
        classifications
        |> Array.filter (fun c ->
            c.Type = SemanticClassificationType.Method
            && substringOfRange source c.Range = "Invoke")
    Assert.Empty(invokeMethods)

/// (#16982) Negative: at a real call site, `Invoke` must still classify as Method.
[<Fact>]
let ``Delegate Invoke at call site classified as method`` () =
    let source = """
type MyDelegate = delegate of int -> string
let d = MyDelegate(fun i -> string i)
let result = d.Invoke(42)
"""
    let classifications = getClassifications source
    let invokeCallSite =
        classifications
        |> Array.filter (fun c ->
            c.Type = SemanticClassificationType.Method && c.Range.StartLine = 4)
    Assert.NotEmpty(invokeCallSite)

/// (#16982) Generic delegate variant.
[<Fact>]
let ``Generic delegate Invoke not classified as method in decl`` () =
    let source = """
type MyGenDelegate<'T> = delegate of 'T -> 'T
"""
    let classifications = getClassifications source
    let invokeMethods =
        classifications
        |> Array.filter (fun c ->
            c.Type = SemanticClassificationType.Method
            && substringOfRange source c.Range = "Invoke")
    Assert.Empty(invokeMethods)

/// (#16982) The synthesized async-pattern members `BeginInvoke`/`EndInvoke` must also be suppressed in the declaration.
[<Fact>]
let ``BeginInvoke EndInvoke also not classified in decl`` () =
    let source = """
type MyDelegate = delegate of int -> string
"""
    let classifications = getClassifications source
    let asyncInvokeMethods =
        classifications
        |> Array.filter (fun c ->
            c.Type = SemanticClassificationType.Method
            && (let text = substringOfRange source c.Range
                text = "BeginInvoke" || text = "EndInvoke"))
    Assert.Empty(asyncInvokeMethods)

// --------------------------------------------------------------------------
// #5229: recursive object self-reference must NOT be classified as MutableVar.
// The compiler internally compiles `as this` / recursive `let rec` self refs
// through a ref cell; that compiler artefact must not leak into colorization.
// Parallel logic already lives in Symbols.fs (FSharpMemberOrFunctionOrValue.IsMutable
// excludes vref.IsCtorThisVal from the isRefCellTy branch). Sprint 02 mirrors
// that exclusion in SemanticClassification.isValRefMutable.
// --------------------------------------------------------------------------

/// Return every classification item whose source span equals exactly `ident`.
let private classificationsForIdent (source: string) (ident: string) =
    getClassifications source
    |> Array.filter (fun item ->
        item.Range.StartLine = item.Range.EndLine
        && substringOfRange source item.Range = ident)

/// Assert no occurrence of `ident` in `source` is classified as MutableVar.
let private assertIdentNeverMutable (source: string) (ident: string) =
    let mutableHits =
        classificationsForIdent source ident
        |> Array.filter (fun item -> item.Type = SemanticClassificationType.MutableVar)
    Assert.True(
        mutableHits.Length = 0,
        sprintf
            "#5229: %A occurrences of `%s` were classified as MutableVar, expected none. Items: %A"
            mutableHits.Length
            ident
            (mutableHits |> Array.map (fun i -> i.Range.StartLine, i.Range.StartColumn, i.Range.EndColumn, i.Type)))

/// (#5229) Primary case: `as this` on a class binds `this` to a CtorThisVal whose
/// internal compilation is a ref cell. That compiler artefact must NOT leak as MutableVar.
[<Fact>]
let ``5229 class as this is not MutableVar`` () =
    let source = """
module Test
type C() as this =
    member _.F() = this
"""
    assertIdentNeverMutable source "this"

/// (#5229) Variants where the `as this` self-reference must not classify as MutableVar.
/// Each row is (label, source, identifier-that-must-not-be-MutableVar).
[<Theory>]
[<InlineData("as-this-on-generic-class",
    """
module Test
type C<'T>(value: 'T) as this =
    member _.Value = value
    member _.Self = this
""", "this")>]
[<InlineData("as-this-with-multiple-members",
    """
module Test
type C() as this =
    member _.A() = this
    member _.B() = this
    member _.C() = this
""", "this")>]
let ``5229 as this variants are not MutableVar`` (_label: string) (source: string) (ident: string) =
    assertIdentNeverMutable source ident

/// (#5229 negative) Real mutable / ref-cell / byref bindings MUST still be classified as MutableVar.
/// Each row is (label, source, identifier-that-must-be-MutableVar).
[<Theory>]
[<InlineData("let-mutable",
    """
module Test
let mutable y = 0
let () = y <- y + 1
""", "y")>]
[<InlineData("ref-cell-deref-and-set",
    """
module Test
let r = ref 0
let () = r := !r + 1
""", "r")>]
let ``5229 real mutable bindings remain MutableVar`` (_label: string) (source: string) (ident: string) =
    let items = classificationsForIdent source ident
    let mutableHits =
        items |> Array.filter (fun item -> item.Type = SemanticClassificationType.MutableVar)
    Assert.True(
        mutableHits.Length > 0,
        sprintf
            "#5229 negative guard: expected at least one MutableVar classification for `%s`. Items: %A"
            ident
            (items |> Array.map (fun i -> i.Range.StartLine, i.Range.StartColumn, i.Range.EndColumn, i.Type)))

/// (#5229 negative) Mutable record fields are an orthogonal code path
/// (isRecdFieldMutable, not isValRefMutable). They MUST be unaffected by the upcoming fix.
[<Fact>]
let ``5229 mutable record field updates remain MutableRecordField`` () =
    let source = """
module Test
type R = { mutable Count: int }
let r = { Count = 0 }
let () = r.Count <- r.Count + 1
"""
    let items = getClassifications source
    let mutableFieldHits =
        items
        |> Array.filter (fun item ->
            item.Type = SemanticClassificationType.MutableRecordField
            && substringOfRange source item.Range = "Count")
    Assert.True(
        mutableFieldHits.Length > 0,
        sprintf "Expected MutableRecordField classification for `Count`, found: %A"
            (items |> Array.map (fun i -> i.Range.StartLine, i.Range.StartColumn, i.Range.EndColumn, i.Type)))
