module FSharp.Compiler.Service.Tests.TypeChecker.TypeCheckerRecoveryTests

open FSharp.Compiler.Service.Tests
open FSharp.Test.Assert
open Xunit

[<Fact>]
let ``Let 01`` () =
    let _, checkResults = getParseAndCheckResults """
module Module

do
    let a = b.ToString()
"""

    dumpDiagnostics checkResults |> shouldEqual [
        "(5,4--5,7): The block following this 'let' is unfinished. Every code block is an expression and must have a result. 'let' cannot be the final code element in a block. Consider giving this block an explicit result."
        "(5,12--5,13): The value, namespace, type or module 'b' is not defined."
    ]


[<Fact>]
let ``Tuple 01`` () =
    let _, checkResults = getParseAndCheckResults """
module Module

open System

Math.Max(a,)
"""

    dumpDiagnostics checkResults |> shouldEqual [
        "(6,10--6,11): Expected an expression after this point"
        "(6,9--6,10): The value or constructor 'a' is not defined."
        "(6,0--6,12): A unique overload for method 'Max' could not be determined based on type information prior to this program point. A type annotation may be needed. Known types of arguments: 'a * 'a1 Candidates: - Math.Max(val1: byte, val2: byte) : byte - Math.Max(val1: decimal, val2: decimal) : decimal - Math.Max(val1: float, val2: float) : float - Math.Max(val1: float32, val2: float32) : float32 - Math.Max(val1: int, val2: int) : int - Math.Max(val1: int16, val2: int16) : int16 - Math.Max(val1: int64, val2: int64) : int64 - Math.Max(val1: nativeint, val2: nativeint) : nativeint - Math.Max(val1: sbyte, val2: sbyte) : sbyte - Math.Max(val1: uint16, val2: uint16) : uint16 - Math.Max(val1: uint32, val2: uint32) : uint32 - Math.Max(val1: uint64, val2: uint64) : uint64 - Math.Max(val1: unativeint, val2: unativeint) : unativeint"
    ]

    assertHasSymbolUsages ["Max"] checkResults


[<Fact>]
let ``Tuple 02`` () =
    let _, checkResults = getParseAndCheckResults """
module Module

open System

Math.Max(a,b,)
"""

    dumpDiagnostics checkResults |> shouldEqual [
        "(6,12--6,13): Expected an expression after this point"
        "(6,9--6,10): The value or constructor 'a' is not defined."
        "(6,11--6,12): The value or constructor 'b' is not defined."
        "(6,0--6,14): A member or object constructor 'Max' taking 3 arguments is not accessible from this code location. All accessible versions of method 'Max' take 2 arguments."
    ]

    assertHasSymbolUsages ["Max"] checkResults