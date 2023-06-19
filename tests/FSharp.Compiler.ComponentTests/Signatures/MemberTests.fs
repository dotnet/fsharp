module Signatures.MemberTests

open Xunit
open FsUnit
open FSharp.Test.Compiler
open Signatures.TestHelpers

[<Fact>]
let ``Verify that the visibility difference between the getter and setter results in two distinct member signatures`` () =
    FSharp
        """
module Foo

type Foo() =
    member f.X with internal get (key1, key2) = true and public set (key1, key2) value = ()
"""
    |> printSignatures
    |> should
        equal
        """
module Foo

type Foo =

  new: unit -> Foo

  member internal X: key1: obj * key2: obj -> bool with get

  member X: key1: obj * key2: obj -> obj with set"""
