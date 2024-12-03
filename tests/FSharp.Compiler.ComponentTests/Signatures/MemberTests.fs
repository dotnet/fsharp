module Signatures.MemberTests

open Xunit
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
    |> assertEqualIgnoreLineEnding
        """
module Foo

type Foo =

  new: unit -> Foo

  member internal X: key1: obj * key2: obj -> bool with get

  member X: key1: obj * key2: obj -> obj with set"""

[<Fact>]
let ``Getter should have explicit with get suffix`` () =
    FSharp
        """
module Foo

type Foo() =
    member f.Y with public get () = 'y' and internal set y = ignore<char> y
"""
    |> printSignatures
    |> assertEqualIgnoreLineEnding
        """
module Foo

type Foo =

  new: unit -> Foo

  member Y: char with get

  member internal Y: char with set"""
