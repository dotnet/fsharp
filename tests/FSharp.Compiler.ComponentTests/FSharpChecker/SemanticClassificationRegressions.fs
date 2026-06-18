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

[<Fact>]
let ``Issue 19905 item 1 - tupled delegate decl has no Method classification`` () =
    let source = """
type SumDelegate = delegate of x: int * y: int -> int
"""
    let classifications = getClassifications source

    let badOnDeclLine =
        classifications
        |> Array.filter (fun c ->
            c.Range.StartLine = 2
            && (c.Type = SemanticClassificationType.Method
                || c.Type = SemanticClassificationType.ExtensionMethod))

    Assert.True(
        badOnDeclLine.Length = 0,
        sprintf
            "Expected no Method/ExtensionMethod classification on the delegate declaration line, but found: %A"
            (badOnDeclLine
             |> Array.map (fun c ->
                 c.Range.StartColumn, c.Range.EndColumn, c.Type, substringOfRange source c.Range))
    )

[<Fact(Skip = "Requires separating classification ranges from symbol-use ranges in the resolution sink - #19905 items 3/4/6")>]
let ``Issue 19905 item 6 - MailboxProcessor int dot Start classified correctly`` () =
    let source = """
let mbx = MailboxProcessor<int>.Start(fun mbx -> async { return () })
"""
    let classifications = getClassifications source

    let line2 = source.Replace("\r\n", "\n").Split('\n').[1]
    let openLt = line2.IndexOf('<')
    let closeGt = line2.IndexOf('>')
    Assert.True(openLt > 0 && closeGt > openLt, "snippet should contain `<int>` on line 2")

    let mailboxIdx = line2.IndexOf("MailboxProcessor")
    let mailboxLen = "MailboxProcessor".Length

    let typeHits =
        classifications
        |> Array.filter (fun c ->
            c.Range.StartLine = 2
            && c.Range.StartColumn = mailboxIdx
            && c.Range.EndColumn = mailboxIdx + mailboxLen
            && (c.Type = SemanticClassificationType.ReferenceType
                || c.Type = SemanticClassificationType.DisposableType
                || c.Type = SemanticClassificationType.Type))

    Assert.True(
        typeHits.Length >= 1,
        sprintf
            "MailboxProcessor identifier should be classified as a type. All classifications on line 2: %A"
            (classifications
             |> Array.filter (fun c -> c.Range.StartLine = 2)
             |> Array.map (fun c -> c.Range.StartColumn, c.Range.EndColumn, c.Type))
    )

    let badAcrossAngles =
        classifications
        |> Array.filter (fun c ->
            c.Range.StartLine = 2
            && (c.Type = SemanticClassificationType.Function
                || c.Type = SemanticClassificationType.Method
                || c.Type = SemanticClassificationType.ExtensionMethod)
            && c.Range.StartColumn <= openLt
            && c.Range.EndColumn > openLt)

    Assert.True(
        badAcrossAngles.Length = 0,
        sprintf
            "No Function/Method classification should cover the `<` of `<int>`, but found: %A"
            (badAcrossAngles
             |> Array.map (fun c ->
                 c.Range.StartColumn, c.Range.EndColumn, c.Type, substringOfRange source c.Range))
    )

    let badAfterAngle =
        classifications
        |> Array.filter (fun c ->
            c.Range.StartLine = 2
            && (c.Type = SemanticClassificationType.Function
                || c.Type = SemanticClassificationType.Method
                || c.Type = SemanticClassificationType.ExtensionMethod)
            && c.Range.StartColumn >= openLt
            && c.Range.StartColumn <= closeGt)

    Assert.True(
        badAfterAngle.Length = 0,
        sprintf
            "No Function/Method classification should start inside `<...>`, but found: %A"
            (badAfterAngle
             |> Array.map (fun c ->
                 c.Range.StartColumn, c.Range.EndColumn, c.Type, substringOfRange source c.Range))
    )

[<Fact>]
let ``Issue 19905 item 2 - CE inside list comprehension classified as ComputationExpression`` () =
    let source = """
type OptionalBuilder() =
    member _.Zero() = None
    member _.Bind(x, f) = Option.bind f x
    member _.Return(x) = Some x
    member _.ReturnFrom(x) = x

let optional = OptionalBuilder()
let myList = [1; 2; 3]
let myNewList = [
    for i in myList do
        optional {
            return! Some(i+1)
        }
]
"""
    let classifications = getClassifications source

    let lines = source.Replace("\r\n", "\n").Split('\n')
    let ceLineIdx =
        lines
        |> Array.findIndex (fun l -> l.TrimStart() = "optional {")
    let ceLine = ceLineIdx + 1

    let line = lines.[ceLineIdx]
    let optionalCol = line.IndexOf("optional")
    let optionalEndCol = optionalCol + "optional".Length

    let ceHits =
        classifications
        |> Array.filter (fun c ->
            c.Range.StartLine = ceLine
            && c.Range.StartColumn = optionalCol
            && c.Range.EndColumn = optionalEndCol
            && c.Type = SemanticClassificationType.ComputationExpression)

    Assert.True(
        ceHits.Length >= 1,
        sprintf
            "Expected ComputationExpression classification on `optional` at line %d col %d-%d, got: %A"
            ceLine optionalCol optionalEndCol
            (classifications
             |> Array.filter (fun c -> c.Range.StartLine = ceLine)
             |> Array.map (fun c -> c.Range.StartColumn, c.Range.EndColumn, c.Type, substringOfRange source c.Range))
    )

    let valueHits =
        classifications
        |> Array.filter (fun c ->
            c.Range.StartLine = ceLine
            && c.Range.StartColumn = optionalCol
            && c.Range.EndColumn = optionalEndCol
            && (c.Type = SemanticClassificationType.LocalValue
                || c.Type = SemanticClassificationType.Value))

    Assert.True(
        valueHits.Length = 0,
        sprintf
            "`optional` at line %d col %d-%d should not also be classified as LocalValue/Value, got: %A"
            ceLine optionalCol optionalEndCol
            (valueHits |> Array.map (fun c -> c.Type))
    )


[<Fact>]
let ``Issue 19905 item 5 - open-ended slice closing bracket not classified as Function`` () =
    let source = "\nlet list = [1; 2; 3]\nlet x    = list[0..]\n"
    let classifications = getClassifications source
    let lines = source.Replace("\r\n", "\n").Split('\n')

    let line3 = lines.[2]
    let openBr = line3.IndexOf('[')
    let closeBr = line3.IndexOf(']', openBr)
    Assert.True(openBr > 0 && closeBr > openBr, "snippet should have `[...]` on line 3")

    let badAtClosingBracket =
        classifications
        |> Array.filter (fun c ->
            c.Range.StartLine = 3
            && (c.Type = SemanticClassificationType.Function
                || c.Type = SemanticClassificationType.Method
                || c.Type = SemanticClassificationType.ExtensionMethod)
            && c.Range.StartColumn <= closeBr
            && c.Range.EndColumn > closeBr)

    Assert.True(
        badAtClosingBracket.Length = 0,
        sprintf
            "Closing `]` of `list[0..]` should not be inside a Function/Method classification, got: %A"
            (badAtClosingBracket
             |> Array.map (fun c -> c.Range.StartColumn, c.Range.EndColumn, c.Type, substringOfRange source c.Range))
    )

[<Fact>]
let ``Issue 19905 item 7 - open type not reported as unused`` () =
    let source = """module Test

[<AbstractClass; Sealed>]
type View =
    static member Window (x: int) = x
    static member TextBlock (s: string) = s

open type View

let w = Window 1
let t = TextBlock "hi"

type Choice = A | B

open type Choice

let c = A
"""

    let fileName, snapshot, checker = singleFileChecker source
    let parseResults, checkAnswer = checker.ParseAndCheckFileInProject(fileName, snapshot) |> Async.RunSynchronously
    let checkResults = getTypeCheckResult (parseResults, checkAnswer)

    let getSourceLineStr lineNo =
        let lines = source.Replace("\r\n", "\n").Split('\n')
        if lineNo >= 1 && lineNo <= lines.Length then lines.[lineNo - 1] else ""

    let unused =
        UnusedOpens.getUnusedOpens(checkResults, getSourceLineStr)
        |> Async.RunSynchronously

    let unusedLines = unused |> List.map (fun r -> r.StartLine) |> Set.ofList

    Assert.False(
        unusedLines.Contains 8,
        sprintf
            "`open type View` on line 8 should not be reported as unused. Full unused list: %A"
            (unused |> List.map (fun r -> r.StartLine, r.StartColumn, r.EndLine, r.EndColumn))
    )

    Assert.False(
        unusedLines.Contains 15,
        sprintf
            "`open type Choice` on line 15 should not be reported as unused. Full unused list: %A"
            (unused |> List.map (fun r -> r.StartLine, r.StartColumn, r.EndLine, r.EndColumn))
    )

[<Fact>]
let ``Issue 19905 item 7 - open type used via static field not reported as unused`` () =
    let source = """module Test
open type System.Math
let _ = PI
"""
    let fileName, snapshot, checker = singleFileChecker source
    let parseResults, checkAnswer = checker.ParseAndCheckFileInProject(fileName, snapshot) |> Async.RunSynchronously
    let checkResults = getTypeCheckResult (parseResults, checkAnswer)

    let getSourceLineStr lineNo =
        let lines = source.Replace("\r\n", "\n").Split('\n')
        if lineNo >= 1 && lineNo <= lines.Length then lines.[lineNo - 1] else ""

    let unused =
        UnusedOpens.getUnusedOpens(checkResults, getSourceLineStr)
        |> Async.RunSynchronously

    Assert.True(
        (unused |> List.filter (fun r -> r.StartLine = 2)).IsEmpty,
        sprintf "`open type System.Math` (line 2) should not be unused; got: %A"
            (unused |> List.map (fun r -> r.StartLine, r.StartColumn, r.EndColumn)))

[<Fact>]
let ``Issue 19905 item 5 - open-ended array slice closing bracket not classified`` () =
    let source = "\nlet arr = [|1;2;3|]\nlet x = arr[0..]\n"
    let items = getClassifications source
    let line3 = source.Replace("\r\n", "\n").Split('\n').[2]
    let closeBracket = line3.LastIndexOf(']')

    let bad =
        items
        |> Array.filter (fun c ->
            c.Range.StartLine = 3
            && (c.Type = SemanticClassificationType.Method || c.Type = SemanticClassificationType.Function)
            && c.Range.EndColumn >= closeBracket + 1)

    Assert.True(bad.Length = 0, sprintf "array slice ] painted: %A" (bad |> Array.map (fun c -> c.Range.StartColumn, c.Range.EndColumn, c.Type)))

[<Fact>]
let ``Issue 19905 item 7 - open type unused when only an instance record field is used`` () =
    let source = """module Test
type R = { X: int }
open type R
let r = { X = 1 }
let _ = r.X
"""
    let fileName, snapshot, checker = singleFileChecker source
    let parseResults, checkAnswer = checker.ParseAndCheckFileInProject(fileName, snapshot) |> Async.RunSynchronously
    let checkResults = getTypeCheckResult (parseResults, checkAnswer)

    let getSourceLineStr lineNo =
        let lines = source.Replace("\r\n", "\n").Split('\n')
        if lineNo >= 1 && lineNo <= lines.Length then lines.[lineNo - 1] else ""

    let unused =
        UnusedOpens.getUnusedOpens(checkResults, getSourceLineStr)
        |> Async.RunSynchronously

    Assert.False(
        (unused |> List.filter (fun r -> r.StartLine = 3)).IsEmpty,
        "`open type R` (line 3) should be unused when only an instance record field of R is used")
