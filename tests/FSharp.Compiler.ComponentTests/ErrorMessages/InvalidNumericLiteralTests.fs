// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.AbstractIL

module ``Numeric Literals`` =

    [<Theory>]
    [<InlineData("1up")>]
    [<InlineData("3._1415F")>]
    [<InlineData("999_99_9999_L")>]
    [<InlineData("52_")>]
    [<InlineData("0_x52")>]
    [<InlineData("0x_52")>]
    [<InlineData("0x52_")>]
    [<InlineData("052_")>]
    [<InlineData("0_o52")>]
    [<InlineData("0o_52")>]
    [<InlineData("0o52_")>]
    [<InlineData("2.1_e2F")>]
    [<InlineData("2.1e_2F")>]
    [<InlineData("1.0_F")>]
    let ``Invalid Numeric Literals`` literal =
        FSharp  ("let x = " + literal)
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1156, Line 1, Col 9, Line 1, Col (9 + (String.length literal)),
                                 "This is not a valid numeric literal. Valid numeric literals include 1, 0x1, 0o1, 0b1, 1l (int/int32), 1u (uint/uint32), 1L (int64), 1UL (uint64), 1s (int16), 1us (uint16), 1y (int8/sbyte), 1uy (uint8/byte), 1.0 (float/double), 1.0f (float32/single), 1.0m (decimal), 1I (bigint).")

    [<Fact>]
    let ``3_(dot)1415F is invalid numeric literal``() =
        FSharp "let x = 3_.1415F"
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1156, Line 1, Col 9,  Line 1, Col 11, "This is not a valid numeric literal. Valid numeric literals include 1, 0x1, 0o1, 0b1, 1l (int/int32), 1u (uint/uint32), 1L (int64), 1UL (uint64), 1s (int16), 1us (uint16), 1y (int8/sbyte), 1uy (uint8/byte), 1.0 (float/double), 1.0f (float32/single), 1.0m (decimal), 1I (bigint).")
            (Error 599,  Line 1, Col 11, Line 1, Col 12,"Missing qualification after '.'")]

    [<Fact>]
    let ``_52 is invalid numeric literal``() =
        FSharp "let x = _52"
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 39, Line 1, Col 9, Line 1, Col 12, "The value or constructor '_52' is not defined.")


    [<Fact>]
    let ``1N is invalid numeric literal``() =
        FSharp "let x = 1N"
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 0784, Line 1, Col 9, Line 1, Col 11,
                                 "This numeric literal requires that a module 'NumericLiteralN' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope")

    [<Fact>]
    let ``1N is invalid numeric literal in FSI``() =
            CompilerAssert.RunScriptWithOptions [| "--langversion:5.0"; "--test:ErrorRanges" |]
                """
let x = 1N
                """
                [
                    "This numeric literal requires that a module 'NumericLiteralN' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope";
                    "Operation could not be completed due to earlier error"
                ]

    // Regressiont test for FSharp1.0: 2543 - Decimal literals do not support exponents
    [<Theory>]
    [<InlineData("1.0E28M")>]
    [<InlineData("1.0E-28M")>]
    let ``Valid Numeric Literals`` literal =
        FSharp ("let x = " + literal)
        |> typecheck |> shouldSucceed
