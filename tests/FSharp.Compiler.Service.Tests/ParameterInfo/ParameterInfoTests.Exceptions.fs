module FSharp.Compiler.Service.Tests.ParameterInfoExceptionsTests

open Xunit

[<Fact>]
let ``Single.Exception.Construction`` () =
    let exns = """
exception E1 of int * string
exception E2 of V1 : int * string * V3 : bool
exception E3 of ``Long Name`` : int * Data1 : string
"""
    assertParameterInfoOverloads [ ["int"; "string"] ] (exns + "let x1 = E1({caret}")
    assertParameterInfoOverloads [ ["V1: int"; "string"; "V3: bool"] ] (exns + "let x2 = E2({caret}")
    assertParameterInfoOverloads [ ["``Long Name`` : int"; "string"] ] (exns + "let x3 = E3({caret}")
