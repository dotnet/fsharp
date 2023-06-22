// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Conformance.ThomasTests.UntilThingsAreFixed

open System
open Xunit
open FSharp.Test.Compiler
        
[<Fact>]
let ``Underscore Dot ToString`` () =
    Fsx """
let x = "a" |> _.ToString()
printfn "%s" x"""
    |> withLangVersionPreview
    |> typecheck
    |> shouldSucceed
        
[<Fact>]
let ``Types`` () =
    Fsx """
let a : (string array -> _) = _.Length
let b = _.ToString()
let c = _.ToString().Length
//let c = _.ToString()[0] """
    |> withLangVersionPreview
    |> typecheck
    |> shouldSucceed
        
[<Fact>]
let ``Regression with an underscore using pattern match`` () =
            Fsx """
type MyDU = A | B
let getnumberOutOfDU x = 
    match x with
    | A -> 42
    | _ -> 43""" 
            |> typecheck 
            |> shouldSucceed       
        
         
[<Fact>]
let ``Underscore Dot Length on string`` () =         
    Fsx """ 
let x = "a" |> _.Length
printfn "%i" x
"""
    |> withLangVersionPreview
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Regression: Empty Interpolated String properly typechecks with explicit type on binding`` () =
    Fsx """ let a:byte = $"string" """
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withSingleDiagnostic (Error 1, Line 1, Col 15, Line 1, Col 24, "This expression was expected to have type" + Environment.NewLine + "    'byte'    " + Environment.NewLine + "but here has type" + Environment.NewLine + "    'string'    ")

[<Fact>]
let ``Interpolated String with hole properly typechecks with explicit type on binding`` () =
    Fsx """ let a:byte = $"strin{'g'}" """
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withSingleDiagnostic (Error 1, Line 1, Col 15, Line 1, Col 28, "This expression was expected to have type" + Environment.NewLine + "    'byte'    " + Environment.NewLine + "but here has type" + Environment.NewLine + "    'string'    ")

[<Fact>]
let ``Interpolated String without holes properly typeckecks with explicit type on binding`` () = 
    Fsx """
let a: obj = $"string"
let b: System.IComparable = $"string"
let c: System.IFormattable = $"string"
    """
    |> withLangVersionPreview
    |> typecheck
    |> shouldSucceed

