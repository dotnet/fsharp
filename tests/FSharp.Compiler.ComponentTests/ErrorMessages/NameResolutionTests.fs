// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler

module NameResolutionTests =

    [<Fact>]
    let FieldNotInRecord () =
        FSharp """
type A = { Hello:string; World:string }
type B = { Size:int; Height:int }
type C = { Wheels:int }
type D = { Size:int; Height:int; Walls:int }
type E = { Unknown:string }
type F = { Wallis:int; Size:int; Height:int; }

let r:F = { Size=3; Height=4; Wall=1 }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1129, Line 9, Col 31, Line 9, Col 35, "The record type 'F' does not contain a label 'Wall'. Maybe you want one of the following:" + System.Environment.NewLine + "   Wallis")
            (Error 764, Line 9, Col 11, Line 9, Col 39, "No assignment given for field 'Wallis' of type 'Test.F'")
        ]

    [<Fact>]
    let RecordFieldProposal () =
        FSharp """
type A = { Hello:string; World:string }
type B = { Size:int; Height:int }
type C = { Wheels:int }
type D = { Size:int; Height:int; Walls:int }
type E = { Unknown:string }
type F = { Wallis:int; Size:int; Height:int; }

let r = { Size=3; Height=4; Wall=1 }
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 9, Col 29, Line 9, Col 33, "The record label 'Wall' is not defined. Maybe you want one of the following:" + System.Environment.NewLine + "   Walls" + System.Environment.NewLine + "   Wallis")
            (Error 764, Line 9, Col 9, Line 9, Col 37, "No assignment given for field 'Wallis' of type 'Test.F'")
        ]
