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
    let ``Nested anonymous records where outer label = concatenated inner labels (see secondary issue reported in 6411)`` () =
        FSharp """
module NestedAnonRecds

let x = {| abcd = {| ab = 4; cd = 1 |} |}
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Wrong update syntax`` () =
        Fsx """
let f (r: {| A: int |}) =
    { r with A = 1 }
"""
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 3, Col 14, Line 3, Col 15, "The record label 'A' is not defined.")
        ]
