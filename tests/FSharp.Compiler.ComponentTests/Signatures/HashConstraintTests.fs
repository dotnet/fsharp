module Signatures.HashConstraintTests

open Xunit
open FSharp.Test.Compiler
open Signatures.TestHelpers

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
    |> assertEqualIgnoreLineEnding
        """
module Foo

type Node =

  abstract Children: int array

val noa: n: #Node option -> Node array"""
