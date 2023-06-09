module FSharp.Compiler.ComponentTests.Signatures.HashConstraintTests

open Xunit
open FsUnit
open FSharp.Test.Compiler
open FSharp.Compiler.ComponentTests.Signatures.TestHelpers

[<Fact>]
let ``Optional hash constraint`` () =
    FSharp
        """
module Foo

[<Interface>]
type Node =
    abstract Children: int array

let noa<'n when 'n :> Node> (n: 'n option) =
    match n with
    | None -> Array.empty
    | Some n -> [| n :> Node |]
"""
    |> printSignatures
    |> should
        equal
        """
module Foo

type Node =

  abstract Children: int array

val noa: n: #Node option -> Node array"""
