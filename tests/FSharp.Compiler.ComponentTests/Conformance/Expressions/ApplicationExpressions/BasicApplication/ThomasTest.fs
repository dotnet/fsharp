﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module Conformance.ThomasTests.UntilThingsAreFixed

open System
open Xunit
open FSharp.Test.Compiler


[<Fact>]
let ``Passes`` () =
    Fsx """ let x = 1 """
    |> typecheck
    |> shouldSucceed
        
[<Fact>]
let ``Fails`` () =
    Fsx """let x = "a" in x |> _.ToString()"""
    |> typecheck
    |> shouldSucceed
        
[<Fact>]
let ``Types`` () =
    Fsx """
let a : (string array -> _) = _.Length
let b = _.ToString()
let c = _.ToString().Length
//let c = _.ToString()[0] """
    |> typecheck
    |> shouldSucceed
        
[<Fact>]
let ``Failing`` () =
            Fsx """
        let rec GenericHashParamObj (iec : IEqualityComparer) (x: obj) : int =
                match x with 
                | null -> 0 
                | (:? System.Array as a) -> 
                    match a with 
                    | :? (obj[]) as oa -> GenericHashObjArray iec oa 
                    | :? (byte[]) as ba -> GenericHashByteArray ba 
                    | :? (int[]) as ba -> GenericHashInt32Array ba 
                    | :? (int64[]) as ba -> GenericHashInt64Array ba 
                    | _ -> GenericHashArbArray iec a 
                | :? IStructuralEquatable as a ->    
                    a.GetHashCode(iec)
                | _ -> 
                    x.GetHashCode()
            """ |> typecheck |> shouldSucceed
        
        
         
[<Fact>]
let ``Fails2`` () =         
    Fsx """ let x = "a" in a |> _.Length"""
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Regression: Empty Interpolated String properly typechecks with explicit type on binding`` () =
    Fsx """ let a:byte = $"string" """
    |> typecheck
    |> shouldFail
    |> withSingleDiagnostic (Error 1, Line 1, Col 15, Line 1, Col 24, "This expression was expected to have type" + Environment.NewLine + "    'byte'    " + Environment.NewLine + "but here has type" + Environment.NewLine + "    'string'    ")

[<Fact>]
let ``Interpolated String with hole properly typechecks with explicit type on binding`` () =
    Fsx """ let a:byte = $"strin{'g'}" """
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
    |> typecheck
    |> shouldSucceed

