module TypeChecks.CheckTypeTests

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Recursive type with attribute`` () =
    FSharp """
namespace Foo

type Bar = | Bar of int -> int
type Other = int
"""
    |> typecheck
    |> withSingleDiagnostic (Error 3578, Line 4, Col 21, Line 4, Col 31, "Unexpected function type in union case field definition")
