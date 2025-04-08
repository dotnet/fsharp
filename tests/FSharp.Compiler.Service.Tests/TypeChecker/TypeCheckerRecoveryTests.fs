module FSharp.Compiler.Service.Tests.TypeChecker.TypeCheckerRecoveryTests

open FSharp.Compiler.Service.Tests
open FSharp.Test.Assert
open Xunit

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