module FSharp.Compiler.Service.Tests.TypeChecker.TypeCheckerRecoveryTests

open FSharp.Compiler.Service.Tests
open FSharp.Compiler.Text
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
        "(4,0--4,12)", 41
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
    [<Fact>]
    let ``Method type 01`` () =
        assertHasSymbolUsageAtCaret "ToString" """
if true then
    "".ToString{caret}
"""
        

    [<Fact>]
    let ``Method type 02`` () =
        assertHasSymbolUsageAtCaret "M" """
type T =
    static member M() = ""

if true then
    T.M{caret}
"""

    [<Fact>]
    let ``Method type 03`` () =
        assertHasSymbolUsageAtCaret "M" """
type T =
    static member M(i: int) = ""
    static member M(s: string) = ""

if true then
    T.M{caret}
"""

    [<Fact>]
    let ``Method type 04`` () =
        assertHasSymbolUsageAtCaret "GetHashCode" """
let o: obj = null
if true then
    o.GetHashCode{caret}
"""
