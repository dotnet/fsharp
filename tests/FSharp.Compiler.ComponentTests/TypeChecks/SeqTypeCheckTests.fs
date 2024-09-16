module TypeChecks.SeqTypeCheckTests

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Seq expr with implicit yield type checks correctly when two identical record types are present`` () =
    FSharp """
module SeqInference

type A = { X: int }
type B = { X: int }

let l: A list = [ if true then { X = 42 } ]
"""
    |> typecheck
    |> shouldSucceed