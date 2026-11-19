module FSharp.Compiler.Service.Tests.TypeChecker.TypeCheckerRecoveryTests

open System
open FSharp.Compiler.Service.Tests
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open FSharp.Compiler.Service.Tests.CompletionTests
open FSharp.Compiler.Service.Tests.TooltipTests
open FSharp.Test.Assert
open Xunit

let assertHasSymbolUsageAtCaret name source =
    let context, checkResults = Checker.getCheckedResolveContext source

    getSymbolUses checkResults
    |> Seq.exists (fun symbolUse ->
        Range.rangeContainsPos symbolUse.Range context.Pos &&
        symbolUse.Symbol.DisplayNameCore = name
    )
    |> shouldEqual true

[<Fact>]
let ``Let 01`` () =
    let _, checkResults = getParseAndCheckResults """
do
    let a = b.ToString()
"""

    dumpDiagnosticNumbers checkResults |> shouldEqual [
        "(3,4--3,7)", 588
        "(3,12--3,13)", 39
    ]

[<Fact>]
let ``Tuple 01`` () =
    let _, checkResults = getParseAndCheckResults """
open System

Math.Max(a,)
"""

    dumpDiagnosticNumbers checkResults |> shouldEqual [
        "(4,10--4,11)", 3100
        "(4,9--4,10)", 39
        "(4,5--4,8)", 41
    ]

    assertHasSymbolUsages ["Max"] checkResults

[<Fact>]
let ``Tuple 02`` () =
    let _, checkResults = getParseAndCheckResults """
open System

Math.Max(a,b,)
"""

    dumpDiagnosticNumbers checkResults |> shouldEqual [
        "(4,12--4,13)", 3100
        "(4,9--4,10)", 39
        "(4,11--4,12)", 39
        "(4,0--4,14)", 503
    ]

    assertHasSymbolUsages ["Max"] checkResults

let private types = """
[<AutoOpen>]
module Types

type T(?a: int, ?b: string) =
    member _.M(a: int, b: string) = ()
    member _.M(a: int, b: string, c: float) = ()
    member _.N(a: int, b: string) = ()
    member _.N(a: string, b: int) = ()
    member _.O(?a: int, ?b: string) = ()
    member _.R(a: int, b: string) = ()
    member _.R(x: int, y: int, z: int, b: string, c: float) = ()
    member val A = 0 with get, set
    member val B = "" with get, set

let x = T()

type Attr(x: int) =
    inherit System.Attribute()
    new() = Attr(0)
    member val A = 0 with get, set
    member val B = "" with get, set
    member val G = 0 with get
    [<DefaultValue>] val mutable F: int
"""

let private check source =
    let options = createProjectOptionsFromNamedSources [ "Types.fs", types; "Test.fs", source ] [ "--target:exe" ]
    let filePath = options.SourceFiles |> Array.find _.EndsWith("Test.fs", StringComparison.Ordinal)
    let _, checkResults = parseAndCheckFile filePath source options
    checkResults

let private checkAtCaret markedSource =
    let context = Checker.getResolveContext markedSource
    let checkResults = check context.SourceContext.Source
    checkResults.GetSymbolUsesAtLocation(context.Pos.Line, context.Pos.Column, context.SourceContext.LineText, context.Names)
    |> List.exactlyOne

module NamedArgs =
    [<Fact>]
    let ``Named arg 01 - Unfinished`` () =
        check "x.M(a = 1, b =)" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,0--1,1)", "val x"
            "(1,0--1,3)", "member M"
            "(1,4--1,5)", "parameter a"
            "(1,11--1,12)", "parameter b"
        ]

    [<Fact>]
    let ``Named arg 02 - Unfinished, EOF`` () =
        check "x.M(a = 1, b =" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,0--1,1)", "val x"
            "(1,0--1,3)", "member M"
            "(1,4--1,5)", "parameter a"
            "(1,11--1,12)", "parameter b"
        ]

    [<Fact>]
    let ``Named arg 03 - Missing required arg`` () =
        check "x.M(a =)" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,0--1,1)", "val x"
            "(1,0--1,3)", "member M"
            "(1,4--1,5)", "parameter a"
        ]

        let symbolUse = checkAtCaret "x.M{caret}(a =)"
        let m = symbolUse.Symbol :?> FSharpMemberOrFunctionOrValue
        m.CurriedParameterGroups |> Seq.concat |> Seq.map _.DisplayName |> List.ofSeq |> shouldEqual [ "a"; "b" ]

    [<Fact>]
    let ``Named arg 04 - Unknown`` () =
        check "x.M(a = 1, z = 2)" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,0--1,1)", "val x"
            "(1,0--1,3)", "member M"
            "(1,4--1,5)", "parameter a"
        ]

    [<Fact>]
    let ``Named arg 05 - Overload by assigned names`` () =
        check "x.M(a = 1, c = 2)" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,0--1,1)", "val x"
            "(1,0--1,3)", "member M"
            "(1,4--1,5)", "parameter a"
            "(1,11--1,12)", "parameter c"
        ]

        let symbolUse = checkAtCaret "x.M{caret}(a = 1, c = 2)"
        let m = symbolUse.Symbol :?> FSharpMemberOrFunctionOrValue
        m.CurriedParameterGroups |> Seq.concat |> Seq.map _.DisplayName |> List.ofSeq |> shouldEqual [ "a"; "b"; "c" ]

    [<Fact>]
    let ``Named arg 06 - Ident, first arg`` () =
        check "x.M(a)" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,0--1,1)", "val x"
            "(1,0--1,3)", "member M"
        ]

    [<Fact>]
    let ``Named arg 07 - Ident, first arg, value in scope`` () =
        check "let a = 1 in x.O(a)" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,13--1,14)", "val x"
            "(1,13--1,16)", "member O"
            "(1,17--1,18)", "val a"
        ]

    [<Fact>]
    let ``Named arg 08 - Ident after named`` () =
        let checkResults = check "x.M(a = 1, b)"

        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(1,11--1,12)", 691
        ]

        dumpSymbolUses 1 checkResults |> shouldEqual [
            "(1,0--1,1)", "val x"
            "(1,0--1,3)", "member M"
            "(1,4--1,5)", "parameter a"
            "(1,11--1,12)", "parameter b"
        ]

    [<Fact>]
    let ``Named arg 09 - Ident after named, unknown`` () =
        let checkResults = check "x.M(a = 1, z)"

        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(1,11--1,12)", 691
            "(1,0--1,13)", 505
        ]

        dumpSymbolUses 1 checkResults |> shouldEqual [
            "(1,0--1,1)", "val x"
            "(1,0--1,3)", "member M"
            "(1,4--1,5)", "parameter a"
        ]

    [<Fact>]
    let ``Named arg 10 - Lambda mismatch`` () =
        check "x.O(a = 1, b = fun () -> \"\")" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,0--1,1)", "val x"
            "(1,0--1,3)", "member O"
            "(1,4--1,5)", "parameter a"
            "(1,11--1,12)", "parameter b"
        ]

    [<Fact>]
    let ``Named arg 11 - No overload`` () =
        check "x.N(a = 1.0, b = 1.0)" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,0--1,1)", "val x"
            "(1,0--1,3)", "member N"
            "(1,4--1,5)", "parameter a"
            "(1,13--1,14)", "parameter b"
        ]

    [<Fact>]
    let ``Named arg 12 - Positional fit`` () =
        check "x.R(1, 2, 3, b =)" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,0--1,1)", "val x"
            "(1,0--1,3)", "member R"
            "(1,13--1,14)", "parameter b"
        ]

        let symbolUse = checkAtCaret "x.R{caret}(1, 2, 3, b =)"
        let m = symbolUse.Symbol :?> FSharpMemberOrFunctionOrValue
        m.CurriedParameterGroups |> Seq.concat |> Seq.map _.DisplayName |> List.ofSeq |> shouldEqual [ "x"; "y"; "z"; "b"; "c" ]

    [<Fact>]
    let ``Setter 01 - Unfinished`` () =
        check "new T(a = 1, B =)" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,4--1,5)", "T"
            "(1,4--1,5)", "member .ctor"
            "(1,6--1,7)", "parameter a"
            "(1,13--1,14)", "property B"
        ]

    [<Fact>]
    let ``Setter 02 - Ident after named`` () =
        check "new T(A = 1, B)" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,4--1,5)", "T"
            "(1,4--1,5)", "member .ctor"
            "(1,6--1,7)", "property A"
            "(1,13--1,14)", "property B"
        ]

    [<Fact>]
    let ``Setter 03 - No overload`` () =
        check "new T(z = 1, B = \"\")" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,4--1,5)", "T"
            "(1,4--1,5)", "member .ctor"
            "(1,13--1,14)", "property B"
        ]

module Attributes =
    [<Fact>]
    let ``Attribute 01 - Property and field`` () =
        check "[<Attr(A = 1, F = 2)>]\ndo ()" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,2--1,6)", "Attr"
            "(1,2--1,6)", "member .ctor"
            "(1,7--1,8)", "property A"
            "(1,14--1,15)", "field F"
        ]

        let symbolUse = checkAtCaret "[<Attr(A{caret} = 1, F = 2)>]\ndo ()"
        (symbolUse.Symbol :?> FSharpMemberOrFunctionOrValue).IsProperty |> shouldEqual true

    [<Fact>]
    let ``Attribute 02 - Unfinished`` () =
        let checkResults = check "[<Attr(A = 1, B =)>]\ndo ()"

        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(1,16--1,17)", 3156
        ]

        dumpSymbolUses 1 checkResults |> shouldEqual [
            "(1,2--1,6)", "Attr"
            "(1,2--1,6)", "member .ctor"
            "(1,7--1,8)", "property A"
            "(1,14--1,15)", "property B"
        ]

    [<Fact>]
    let ``Attribute 03 - Unfinished, EOF`` () =
        let checkResults = check "[<Attr(A = 1, B ="

        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(1,16--1,17)", 3156
            "(1,6--1,7)", 583
            "(1,0--1,2)", 3102
            "(1,0--1,17)", 3158
        ]

        dumpSymbolUses 1 checkResults |> shouldEqual [
            "(1,2--1,6)", "Attr"
            "(1,2--1,6)", "member .ctor"
            "(1,7--1,8)", "property A"
            "(1,14--1,15)", "property B"
        ]

    [<Fact>]
    let ``Attribute 04 - Unclosed`` () =
        let checkResults = check "[<Attr(A = 1>]\ndo ()"

        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(1,12--1,14)", 10
        ]

        dumpSymbolUses 1 checkResults |> shouldEqual [
            "(1,2--1,6)", "Attr"
            "(1,2--1,6)", "member .ctor"
            "(1,7--1,8)", "property A"
        ]

    [<Fact>]
    let ``Attribute 05 - Ident after named`` () =
        let checkResults = check "[<Attr(A = 1, B)>]\ndo ()"

        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(1,14--1,15)", 691
        ]

        dumpSymbolUses 1 checkResults |> shouldEqual [
            "(1,2--1,6)", "Attr"
            "(1,2--1,6)", "member .ctor"
            "(1,7--1,8)", "property A"
            "(1,14--1,15)", "property B"
        ]

    [<Fact>]
    let ``Attribute 06 - Ident, first arg`` () =
        let checkResults = check "[<Attr(A)>]\ndo ()"

        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(1,7--1,8)", 39
            "(1,7--1,8)", 267
        ]

        dumpSymbolUses 1 checkResults |> shouldEqual [
            "(1,2--1,6)", "Attr"
            "(1,2--1,6)", "member .ctor"
        ]

    [<Fact>]
    let ``Attribute 07 - Unknown`` () =
        let checkResults = check "[<Attr(Z = 1, A = 2)>]\ndo ()"

        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(1,2--1,20)", 508
        ]

        dumpSymbolUses 1 checkResults |> shouldEqual [
            "(1,2--1,6)", "Attr"
            "(1,2--1,6)", "member .ctor"
            "(1,14--1,15)", "property A"
        ]

    [<Fact>]
    let ``Attribute 08 - Not settable`` () =
        let checkResults = check "[<Attr(G = 1, A = 2)>]\ndo ()"

        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(1,2--1,20)", 508
        ]

        dumpSymbolUses 1 checkResults |> shouldEqual [
            "(1,2--1,6)", "Attr"
            "(1,2--1,6)", "member .ctor"
            "(1,14--1,15)", "property A"
        ]

    [<Fact>]
    let ``Attribute 09 - No overload`` () =
        let checkResults = check "[<Attr(1, 2, A = 1)>]\ndo ()"

        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(1,2--1,19)", 505
        ]

        dumpSymbolUses 1 checkResults |> shouldEqual [
            "(1,2--1,6)", "Attr"
            "(1,2--1,6)", "member .ctor"
            "(1,13--1,14)", "property A"
        ]

    [<Fact>]
    let ``Attribute 10 - Parameter and property`` () =
        check "[<Attr(x = 1, A = 2)>]\ndo ()" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,2--1,6)", "Attr"
            "(1,2--1,6)", "member .ctor"
            "(1,7--1,8)", "parameter x"
            "(1,14--1,15)", "property A"
        ]

    [<Fact>]
    let ``Attribute 11 - Type mismatch`` () =
        let checkResults = check "[<Attr(A = \"\", B = 2)>]\ndo ()"

        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(1,11--1,13)", 1
            "(1,19--1,20)", 1
            "(1,11--1,13)", 267
            "(1,19--1,20)", 267
        ]

        dumpSymbolUses 1 checkResults |> shouldEqual [
            "(1,2--1,6)", "Attr"
            "(1,2--1,6)", "member .ctor"
            "(1,7--1,8)", "property A"
            "(1,15--1,16)", "property B"
        ]

    [<Fact>]
    let ``Attribute 12 - Optional`` () =
        let checkResults = check "[<Attr(?A = 1, B = \"\")>]\ndo ()"

        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(1,8--1,9)", 843
        ]

        dumpSymbolUses 1 checkResults |> shouldEqual [
            "(1,2--1,6)", "Attr"
            "(1,2--1,6)", "member .ctor"
            "(1,8--1,9)", "property A"
            "(1,15--1,16)", "property B"
        ]

    [<Fact>]
    let ``Attribute 13 - IL property`` () =
        check "[<System.Diagnostics.DebuggerDisplay(\"x\", Name = \"y\")>]\ntype T = class end" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,2--1,8)", "System"
            "(1,2--1,36)", "DebuggerDisplayAttribute"
            "(1,9--1,20)", "Diagnostics"
            "(1,21--1,36)", "member .ctor"
            "(1,42--1,46)", "property Name"
        ]

    [<Fact>]
    let ``Attribute 14 - IL field`` () =
        check "[<System.Runtime.InteropServices.DllImport(\"x\", EntryPoint = \"y\")>]\nextern void f()" |> dumpSymbolUses 1 |> shouldEqual [
            "(1,2--1,8)", "System"
            "(1,2--1,42)", "DllImportAttribute"
            "(1,9--1,16)", "Runtime"
            "(1,17--1,32)", "InteropServices"
            "(1,33--1,42)", "member .ctor"
            "(1,48--1,58)", "field EntryPoint"
        ]

module Constraints =
    [<Fact>]
    let ``Type 01`` () =
        assertHasSymbolUsageAtCaret "f" """
let f (x: string) =
    x + 1

{caret}f ""
"""

    [<Fact>]
    let ``Type 02`` () =
        assertHasSymbolUsageAtCaret "M" """
type T =
    static member M(x: string) =
        x + 1

T.M{caret} ""
"""

module Expressions =
    [<Theory>]
    [<InlineData("ToString", """
    if true then
        "".ToString{caret}
    """)>]
    [<InlineData("M", """
    type T =
        static member M() = ""

    if true then
        T.M{caret}
    """)>]
    [<InlineData("M", """
    type T =
        static member M(i: int) = ""
        static member M(s: string) = ""

    if true then
        T.M{caret}
    """)>]
    [<InlineData("GetHashCode", """
    let o: obj = null
    if true then
        o.GetHashCode{caret}
    """)>]
    let ``Method type`` (name: string) (source: string) =
        assertHasSymbolUsageAtCaret name source

module Patterns =
    [<Theory>]
    [<InlineData("""
    type E =
        | A = 1

    match E.A with
    | E{caret}.A -> ()
    """)>]
    [<InlineData("""
    type E =
        | A = 1

    match E.A with
    | E{caret} -> ()
    """)>]
    [<InlineData("""
    type E =
        | A = 1

    match E.A with
    | E{caret}
    """)>]
    [<InlineData("""
    type E =
        | A = 1

    match E.A with
    | E{caret}.
    """)>]
    [<InlineData("""
    type E =
        | A = 1

    match E.A with
    | E{caret}. -> ()
    """)>]
    [<InlineData("""
    type E =
        | A = 1

    match E.A with
    | E{caret}.B
    """, Skip = "Improve name resolution recovery")>]
    [<InlineData("""
    type E =
        | A = 1

    match E.A with
    | E{caret}.

    ()
    """)>]
    let ``Enum - Type`` (source: string) =
        assertHasSymbolUsageAtCaret "E" source

module ErrorRecovery =

    [<Theory>]
    [<InlineData("""
    let s = ""
    if true then
        ()
    elif s.{caret}
    else ()
    """)>]
    [<InlineData("""
    let s = ""
    if true then
        ()
    elif true
    elif s.{caret}
    else ()
    """)>]
    [<InlineData("""
    let s = ""
    if true then
        ()
    elif s.{caret}
    elif true
    else ()
    """)>]
    [<InlineData("""
    let s = ""
    if true then
        ()
    elif s.{caret}
    """)>]
    let ``Bug4881 - member completion after dot in elif on broken code`` (source: string) =
        let info = Checker.getCompletionInfo source
        assertHasItemWithNames ["Split"] info

    [<Fact>]
    let ``NotFixing4538_1 - completion offers type after partial 'new MyT'`` () =
        let info = Checker.getCompletionInfo """
type MyType() =
    override x.ToString() = ""
let Main() =
    let _ = new MyT{caret}
    ()
"""
        assertHasItemWithNames ["MyType"] info

    [<Theory>]
    [<InlineData("""
    type MyType() =
        override x.ToString() = ""
    let Main() =
        let _ = MyT{caret}
        ()
    """)>]
    [<InlineData("""
    type MyType() =
        override x.ToString() = ""
    let Main() =
        let _ = MyT{caret}
    """)>]
    let ``NotFixing4538_2_3 - completion offers type after partial 'MyT'`` (source: string) =
        let info = Checker.getCompletionInfo source
        assertHasItemWithNames ["MyType"] info

    [<Fact>]
    let ``Bug4538_2 - completion offers type after a preceding valid binding`` () =
        let info = Checker.getCompletionInfo """
type MyType() =
    override x.ToString() = ""
let Main() =
    let x = MyType()
    let _ = MyT{caret}
"""
        assertHasItemWithNames ["MyType"] info

    [<Fact>]
    let ``Bug4538_5 - completion offers type after partial 'MyT' in use binding`` () =
        let info = Checker.getCompletionInfo """
type MyType() =
    override x.ToString() = ""
let Main() =
    use x = null
    use _ = MyT{caret}
"""
        assertHasItemWithNames ["MyType"] info

    [<Fact>]
    let ``5878_1 - member data tip available for Module dot at end of file`` () =
        let info = Checker.getCompletionInfo """
module Module =
    /// Union comment
    type Union =
        /// Case comment
        | Case of int
Module.{caret}
"""
        let caseItem =
            info.Items
            |> Array.find (fun item -> item.NameInCode = "Case")

        let description, xmlDoc, _ = assertAndExtractTooltip caseItem.Description

        Assert.Contains("union case Module.Union.Case: int -> Module.Union", description)

        match xmlDoc with
        | FSharpXmlDoc.FromXmlText t ->
            Assert.Contains("Case comment", String.concat "\n" t.UnprocessedLines)
        | other -> failwith $"Expected FSharpXmlDoc.FromXmlText, got {other}"

module ExhaustivelyScrutinize =

    [<Fact>]
    let ``ThisOnceAsserted - if/elif/else returning malformed tuples`` () =
        let _, checkResults = getParseAndCheckResults """
let F() =                 
    if true then [],      
    elif true then [],""  
    else [],""            
"""
        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(4,4--4,8)", 58
            "(3,19--3,20)", 3100
        ]

    [<Fact>]
    let ``ThisOnceAssertedToo - interface implementation`` () =
        let _, checkResults = getParseAndCheckResults """
type C() = 
    member this.F() = ()
    interface System.IComparable with 
        member _.CompareTo(v:obj) = 1
"""
        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(2,5--2,6)", 343
        ]

    [<Fact>]
    let ``ThisOnceAssertedThree - property with get and set`` () =
        let _, checkResults = getParseAndCheckResults """
type Foo =
    { mutable Data: string }
    member x.XmlDocSig 
        with get() = x.Data
        and set(v) = x.Data <- v
"""
        dumpDiagnosticNumbers checkResults |> shouldEqual []

    [<Fact>]
    let ``ThisOnceAssertedFour - unfinished new`` () =
        let _, checkResults = getParseAndCheckResults """
let y=new
let z=4
"""
        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(3,0--3,3)", 10
        ]

    [<Fact>]
    let ``ThisOnceAssertedFive - type application with quotation token`` () =
        let _, checkResults = getParseAndCheckResults """
CSV.File<@"File1.txt">.[0].
"""
        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(2,10--2,21)", 10
            "(2,10--2,21)", 1241
            "(2,21--2,22)", 3156
            "(2,0--2,3)", 39
        ]

    [<Fact>]
    let ``Bug2277 - open of non-existent namespace`` () =
        let _, checkResults = getParseAndCheckResults """
open Microsoft.FSharp.Plot.Excel
open Microsoft.FSharp.Plot.Interactive
let ps = [| (1.,"c"); (-2.,"p") |]
plot (Bars(ps))
let xs = [| 1.0 .. 20.0 |]
let ys = [| 2.0 .. 21.0 |]
let pp= plot(Area(xs,ys))
"""
        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(2,22--2,26)", 39
            "(3,22--3,26)", 39
            "(5,0--5,4)", 39
            "(8,8--8,12)", 39
        ]

    [<Fact>]
    let ``Bug2283 - missing reference and nested generic classes`` () =
        let _, checkResults = getParseAndCheckResultsUniqueName """
#r "NestedClasses.dll"
//753 atomType -> atomType DOT path typeArgs
let specificIdent (x : RootNamespace.ClassOfT<int>.NestedClassOfU<string>) = x
let x = new RootNamespace.ClassOfT<int>.NestedClassOfU<string>()
if specificIdent x <> x then exit 1
exit 0
"""
#if NETCOREAPP
        dumpDiagnosticNumbers checkResults |> shouldEqual [
            "(4,23--4,36)", 39
            "(5,12--5,25)", 39
        ]
#else
        dumpDiagnosticNumbers checkResults
        |> List.distinct
        |> List.sort
        |> shouldEqual [
            "(2,0--2,22)", 84
            "(4,23--4,36)", 39
            "(5,12--5,25)", 39
        ]
#endif
