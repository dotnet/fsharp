module FSharp.Compiler.Service.Tests.ParameterInfoDiscriminatedUnionsTests

open Xunit

[<Fact>]
let ``Single.DiscriminatedUnion.Construction`` () =
    let du = """
type MyDU =
  | Case1 of int * string
  | Case2 of V1 : int * string * V3 : bool
  | Case3 of ``Long Name`` : int * Item2 : string
  | Case4 of int
"""
    assertParameterInfoOverloads [ ["int"; "string"] ] (du + "let x1 = Case1({caret}")
    assertParameterInfoOverloads [ ["V1: int"; "string"; "V3: bool"] ] (du + "let x2 = Case2({caret}")
    assertParameterInfoOverloads [ ["``Long Name`` : int"; "string"] ] (du + "let x3 = Case3({caret}")
    assertParameterInfoOverloads [ ["int"] ] (du + "let x4 = Case4({caret}")

[<Fact>]
let ``LocationOfParams.Unions1`` () =
    assertHasParameterInfo """
type MyDU =
    | FOO of int * string
let r = F{caret}OO(42,"") """
