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

/// (#16268) IDisposable appearing in `interface IDisposable with` should be
/// classified as Interface, not DisposableType.
[<Fact>]
let ``IDisposable in interface impl classified as interface`` () =
    let source = """
open System
type MyClass() =
    interface IDisposable with
        member _.Dispose() = ()
"""
    let classifications = getClassifications source
    let idisposableOnLine4 =
        classifications
        |> Array.filter (fun c ->
            c.Range.StartLine = 4 && substringOfRange source c.Range = "IDisposable")
    Assert.True(idisposableOnLine4.Length > 0, "Expected at least one classification covering IDisposable on line 4")
    Assert.True(
        idisposableOnLine4
        |> Array.forall (fun c -> c.Type = SemanticClassificationType.Interface),
        sprintf "Expected IDisposable to be classified as Interface, got: %A"
            (idisposableOnLine4 |> Array.map (fun c -> c.Type)))

/// (#16268) Negative: a concrete disposable class (MemoryStream) in a type-occurrence
/// position must still be classified as DisposableType - the reorder of isInterfaceTy
/// before isDisposableTy must not regress non-interface disposables.
/// A type annotation is used (not `new MemoryStream()`, which is Item.CtorGroup) so the
/// occurrence actually flows through the modified Item.Types/LegitTypeOccurrence arm.
[<Fact>]
let ``Concrete disposable class classified as disposable type`` () =
    let source = """
open System.IO
let length (s: MemoryStream) = s.Length
"""
    let classifications = getClassifications source
    let memStream =
        classifications
        |> Array.filter (fun c ->
            substringOfRange source c.Range = "MemoryStream" && c.Range.StartLine = 3)
    Assert.True(memStream.Length > 0, "Expected at least one classification covering MemoryStream")
    Assert.True(
        memStream
        |> Array.forall (fun c -> c.Type = SemanticClassificationType.DisposableType),
        sprintf "MemoryStream type occurrence must be classified as DisposableType, got: %A"
            (memStream |> Array.map (fun c -> c.Type)))

/// (#16268) IDisposable used as a type constraint should be Interface.
[<Fact>]
let ``IDisposable as type constraint classified as interface`` () =
    let source = """
open System
let dispose (x: #IDisposable) = x.Dispose()
"""
    let classifications = getClassifications source
    let idisposable =
        classifications
        |> Array.filter (fun c -> substringOfRange source c.Range = "IDisposable")
    Assert.True(idisposable.Length > 0, "Expected at least one classification covering IDisposable")
    Assert.True(
        idisposable
        |> Array.forall (fun c -> c.Type = SemanticClassificationType.Interface),
        sprintf "Expected IDisposable type-constraint occurrence to be Interface, got: %A"
            (idisposable |> Array.map (fun c -> c.Type)))

/// (#16268) Non-IDisposable interface in `interface ... with` position stays Interface.
/// This guards against accidentally narrowing the fix to IDisposable only.
[<Fact>]
let ``Non-IDisposable interface classified as interface`` () =
    let source = """
type IMyInterface =
    abstract member DoStuff: unit -> unit
type MyClass() =
    interface IMyInterface with
        member _.DoStuff() = ()
"""
    let classifications = getClassifications source
    let iface =
        classifications
        |> Array.filter (fun c ->
            substringOfRange source c.Range = "IMyInterface" && c.Range.StartLine = 5)
    Assert.True(iface.Length > 0, "Expected at least one IMyInterface classification on line 5")
    Assert.True(
        iface
        |> Array.forall (fun c -> c.Type = SemanticClassificationType.Interface),
        sprintf "Expected IMyInterface on line 5 to be Interface, got: %A"
            (iface |> Array.map (fun c -> c.Type)))

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

/// (#19960 review) `open type` whose static members are only ever used through a fully-qualified
/// path is treated as used, consistently with how `open <module>` already behaves for a
/// fully-qualified `Module.member` use. This is intentional: the static-member case shares the
/// declaring-entity fast path, so it inherits the same (pre-existing) behavior modules have.
[<Fact>]
let ``Issue 19905 item 7 - open type used only via fully-qualified static member is consistent with module opens`` () =
    let source =
        """module Test
module M =
    let value = 1
open M

[<AbstractClass; Sealed>]
type View =
    static member Window (x: int) = x
open type View

let usedModuleQualified = M.value
let usedTypeQualified = View.Window 1
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

    // `open M` (line 4) is used only via the fully-qualified `M.value`; it is treated as used.
    let openModuleReportedUnused = unusedLines.Contains 4
    // `open type View` (line 9) is used only via the fully-qualified `View.Window`.
    let openTypeReportedUnused = unusedLines.Contains 9
    let behavesConsistently = (openModuleReportedUnused = openTypeReportedUnused)

    Assert.True(
        behavesConsistently,
        sprintf
            "`open type` used only fully-qualified must behave like `open <module>` used only fully-qualified. open M unused=%b, open type View unused=%b. Full unused: %A"
            openModuleReportedUnused
            openTypeReportedUnused
            (unused |> List.map (fun r -> r.StartLine, r.StartColumn, r.EndColumn))
    )

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
