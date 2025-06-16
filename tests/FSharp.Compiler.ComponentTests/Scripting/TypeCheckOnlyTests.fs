module FSharp.Compiler.ComponentTests.Scripting.TypeCheckOnlyTests

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

[<Fact>]
let ``typecheck-only flag works for valid script``() =
    Fsx """
let x = 42
printfn "This should not execute"
"""
    |> withOptions ["--typecheck-only"]
    |> compile
    |> shouldSucceed

[<Fact>]
let ``typecheck-only flag catches type errors``() =
    Fsx """
let x: int = "string"  // Type error
"""
    |> withOptions ["--typecheck-only"]
    |> compile
    |> shouldFail
    |> withDiagnostics [
        (Error 1, Line 2, Col 14, Line 2, Col 22, "This expression was expected to have type\n    'int'    \nbut here has type\n    'string'")
    ]

[<Fact>]
let ``typecheck-only flag prevents execution side effects``() =
    Fsx """
printfn "MyCrazyString"
let x = 42
"""
    |> withOptions ["--typecheck-only"]
    |> runFsi
    |> shouldSucceed
    |> VerifyNotInOutput "MyCrazyString"

[<Fact>]
let ``script executes without typecheck-only flag``() =
    Fsx """
printfn "MyCrazyString"
let x = 42
"""
    |> runFsi
    |> shouldSucceed
    |> verifyOutput "MyCrazyString"