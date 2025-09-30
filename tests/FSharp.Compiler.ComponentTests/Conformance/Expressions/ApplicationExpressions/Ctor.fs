module FSharp.Compiler.ComponentTests.Conformance.Expressions.ApplicationExpressions.Ctor

open FSharp.Test.Compiler
open Xunit

[<Fact>]
let ``Nullable 01`` () =
    FSharp """
module Module

let _ = System.Nullable()
"""
    |> typecheck
    |> shouldSucceed
