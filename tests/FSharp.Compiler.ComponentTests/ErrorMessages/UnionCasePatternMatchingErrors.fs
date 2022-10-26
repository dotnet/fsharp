module FSharp.Compiler.ComponentTests.ErrorMessages.UnionCasePatternMatchingErrors

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Union matching error - Incomplete union fields`` () =
    FSharp """
module Tests
type U =
    | B of  f1:int list * {|X:string|} * f3:U * f4: (int * System.String)

let x : U = failwith ""
let myVal = 
    match x with
    | B  -> 42"""
    |> typecheck
    |> shouldFail   
    |> withSingleDiagnostic (Error 727, Line 9, Col 7, Line 9, Col 8,
                                "This union case expects 4 arguments in tupled form, but was given 0. The missing field arguments may be any of:
\tf1: int list
\t{| X: string |}
\tf3: U
\tf4: (int * System.String)")

[<Fact>]
let ``Union matching error - Named args - Name used twice`` () =
    FSharp """
module Tests
type U =    
    | B of field: int * int
let x : U = failwith ""
let myVal = 
    match x with
    | B (field = x; field = z) -> let y = x + z + 1 in ()"""
    |> typecheck
    |> shouldFail   
    |> withSingleDiagnostic (Error 3175, Line 8, Col 21, Line 8, Col 26, "Union case/exception field 'field' cannot be used more than once.")

[<Fact>]
let ``Union matching error - Multiple tupled args`` () =
    FSharp """
module Tests
type U =
    | B of field: int * int

let x : U = failwith ""
let myVal = 
    match x with
    | B x z -> let y = x + z + 1 in ()"""
    |> typecheck
    |> shouldFail   
    |> withSingleDiagnostic (Error 727, Line 9, Col 7, Line 9, Col 12, "This union case expects 2 arguments in tupled form, but was given 0. The missing field arguments may be any of:
\tfield: int
\tint")

[<Fact>]
let ``Union matching error - Missing field`` () =
     FSharp """
module Tests
type U =
    | A
    | B of int * int * int

let myVal = 
    match A with
    | A -> 15
    | B (x, _) -> 16"""
    |> typecheck
    |> shouldFail   
    |> withSingleDiagnostic (Error 727, Line 10, Col 7, Line 10, Col 15, "This union case expects 3 arguments in tupled form, but was given 2. The missing field arguments may be any of:
\tint")