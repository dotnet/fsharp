// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Debugger

open Xunit
open FSharp.Test.Compiler

/// https://github.com/dotnet/fsharp/issues/13504
module ForArrowDebugPoints =

    let private verifyComprehensionDebugPoints source expectedSequencePoints =
        FSharp source
        |> asLibrary
        |> withPortablePdb
        |> compile
        |> shouldSucceed
        |> verifyPdb [ VerifySequencePoints expectedSequencePoints ]

    [<Fact>]
    let ``For-arrow list comprehension body has debug point`` () =
        verifyComprehensionDebugPoints """
module TestModule

let squares = [
    for x in [1; 2; 3] ->
        x * x
    ]
        """ [
            (Line 4, Col 1, Line 7, Col 6)
            (Line 5, Col 5, Line 5, Col 8)
            (Line 5, Col 11, Line 5, Col 13)
            (Line 6, Col 9, Line 6, Col 14)
            (Line 16707566, Col 0, Line 16707566, Col 0)
        ]

    [<Fact>]
    let ``For-arrow array comprehension body has debug point`` () =
        verifyComprehensionDebugPoints """
module TestModule

let squares = [|
    for x in [|1; 2; 3|] ->
        x * x
    |]
        """ [
            (Line 4, Col 1, Line 7, Col 7)
            (Line 5, Col 5, Line 5, Col 8)
            (Line 5, Col 11, Line 5, Col 13)
            (Line 6, Col 9, Line 6, Col 14)
            (Line 16707566, Col 0, Line 16707566, Col 0)
        ]

    [<Fact>]
    let ``For-do-yield comprehension body has debug point`` () =
        verifyComprehensionDebugPoints """
module TestModule

let squares = [
    for x in [1; 2; 3] do
        yield x * x
    ]
        """ [
            (Line 4, Col 1, Line 7, Col 6)
            (Line 5, Col 5, Line 5, Col 8)
            (Line 5, Col 11, Line 5, Col 13)
            (Line 6, Col 15, Line 6, Col 20)
            (Line 16707566, Col 0, Line 16707566, Col 0)
        ]
