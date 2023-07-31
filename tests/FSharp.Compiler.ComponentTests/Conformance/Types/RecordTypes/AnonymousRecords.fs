// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Types

open Xunit
open FSharp.Test.Compiler

module AnonymousRecord =

    [<Fact>]
    let ``Anonymous Records with duplicate labels`` () =
        FSharp """
namespace FSharpTest

module AnonRecd =
    let v = {| A = 1; A = 2 |}
"""
        |> compile
        |> shouldFail
        |> withErrorCode 3522
        |> withMessage "The field 'A' appears multiple times in this record expression."
    
    [<Fact>]
    let ``Anonymous Record missing single field`` () =
        Fsx """
let x () : {| A: int; B: string  |} =  {| A = 123 |}
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 2, Col 40, Line 2, Col 53, "This anonymous record is missing field 'B'.")
        ]
        
    [<Fact>]
    let ``Anonymous Record missing multiple fields`` () =
        Fsx """
let x () : {| A: int; B: string; C: int  |} =  {| A = 123 |}
"""
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 2, Col 48, Line 2, Col 61, "This anonymous record is missing fields 'B, C'.")
        ]

    [<Fact>]
    let ``Anonymous Records with duplicate labels - Copy and update expression`` () =
        FSharp """
namespace FSharpTest

module AnonRecd =
    let v = {| {| y = 3 |} with y = 2; y = 4 |}
"""
        |> compile
        |> shouldFail
        |> withErrorCode 3522

    [<Fact>]
    let ``Anonymous Record type annotation with duplicate labels`` () =
        FSharp """
namespace FSharpTest

module AnonRecd =
    let (f : {| A : int; A : string |} option) = None
"""
        |> compile
        |> shouldFail
        |> withErrorCode 3523

    [<Fact>]
    let ``Anonymous record types with parser errors or no fields do not produce overlapping diagnostics`` () =
        FSharp """
module AnonRecd

type ContactMethod =
    | Address of {| Line1 : string; Line 2 : string; Postcode : string |}

let (x: {| |}) = ()

type ErrorResponse =
    { error: {| type : string
                message : string |} }
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            Error 10, Line 5, Col 42, Line 5, Col 43, "Unexpected integer literal in field declaration. Expected ':' or other token."
            Error 10, Line 7, Col 12, Line 7, Col 14, "Unexpected symbol '|}' in field declaration. Expected identifier or other token."
            Error 10, Line 10, Col 17, Line 10, Col 21, "Incomplete structured construct at or before this point in field declaration. Expected identifier or other token."
        ]
        
    [<Fact>]
    let ``Anonymous Record type annotation with with fields defined in a record`` () =
        Fsx """
type T = { ff : int }

let t3 (t1: {| gu: string; ff: int |}) = { t1 with ff = 3 }
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3578, Line 4, Col 42, Line 4, Col 43, "This expression is an anonymous record, use {|...|} instead of {...}.")
            (Error 3578, Line 4, Col 59, Line 4, Col 60, "This expression is an anonymous record, use {|...|} instead of {...}.")
        ]
        
    [<Fact>]
    let ``This expression was expected to have an anonymous Record but has a record`` () =
        Fsx """
let t3 (t1: {| gu: string; ff: int |}) = { t1 with ff = 3 }
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3578, Line 2, Col 42, Line 2, Col 43, "This expression is an anonymous record, use {|...|} instead of {...}.")
            (Error 3578, Line 2, Col 59, Line 2, Col 60, "This expression is an anonymous record, use {|...|} instead of {...}.")
        ]
        
    [<Fact>]
    let ``This expression was expected to have an struct anonymous Record but has a record`` () =
        Fsx """
let t3 (t1: struct {| gu: string; ff: int |}) = { t1 with ff = 3 }
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3578, Line 2, Col 49, Line 2, Col 50, "This expression is an anonymous record, use {|...|} instead of {...}.")
            (Error 3578, Line 2, Col 66, Line 2, Col 67, "This expression is an anonymous record, use {|...|} instead of {...}.")
        ]
        
    [<Fact>]
    let ``This expression was expected to have an anonymous with various properties Record but has a record`` () =
        Fsx """
let f (r: {| A: int; C: int |}) = { r with A = 1; B = 2; C = 3 }
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 3578, Line 2, Col 35, Line 2, Col 36, "This expression is an anonymous record, use {|...|} instead of {...}.")
            (Error 3578, Line 2, Col 64, Line 2, Col 65, "This expression is an anonymous record, use {|...|} instead of {...}.")
        ]
    
    [<Fact>]
    let ``Nested anonymous records where outer label = concatenated inner labels (see secondary issue reported in 6411)`` () =
        FSharp """
module NestedAnonRecds

let x = {| abcd = {| ab = 4; cd = 1 |} |}
"""
        |> compile
        |> shouldSucceed
