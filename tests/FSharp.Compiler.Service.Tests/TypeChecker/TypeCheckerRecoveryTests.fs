module FSharp.Compiler.Service.Tests.TypeChecker.TypeCheckerRecoveryTests

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

    [<Fact>]
    let ``Bug4538_3 - unfinished let block reports a diagnostic`` () =
        let _, checkResults = getParseAndCheckResults """
type MyType() =
    override x.ToString() = ""
let Main() =
    let x = MyType()
"""
        let expected =
            "The block following this 'let' is unfinished. Every code block is an expression and must have a result. 'let' cannot be the final code element in a block. Consider giving this block an explicit result."

        Assert.Contains(expected, dumpDiagnostics checkResults |> String.concat "\n")

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
    let ``Bug4594_1 - completion offers enclosing parameter in unfinished if`` () =
        let info = Checker.getCompletionInfo """
let Bar(xyz) =
    let hello =
        if x{caret}
"""
        assertHasItemWithNames ["xyz"] info

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
        let _, checkResults = getParseAndCheckResults """
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
