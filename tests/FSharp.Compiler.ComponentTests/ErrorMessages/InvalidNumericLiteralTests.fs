// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ErrorMessages.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.AbstractIL.Internal

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
        CompilerAssert.TypeCheckSingleError 
            ("let x = " + literal)
            FSharpErrorSeverity.Error
            1156
            (1, 9, 1, 9 + (String.length literal))
            "This is not a valid numeric literal. Valid numeric literals include 1, 0x1, 0o1, 0b1, 1l (int), 1u (uint32), 1L (int64), 1UL (uint64), 1s (int16), 1y (sbyte), 1uy (byte), 1.0 (float), 1.0f (float32), 1.0m (decimal), 1I (BigInteger)."

    [<Fact>]
    let ``3_(dot)1415F is invalid numeric literal``() =
        CompilerAssert.TypeCheckWithErrors
            """
let x = 3_.1415F
            """
            [|
                FSharpErrorSeverity.Error, 1156, (2, 9, 2, 11), "This is not a valid numeric literal. Valid numeric literals include 1, 0x1, 0o1, 0b1, 1l (int), 1u (uint32), 1L (int64), 1UL (uint64), 1s (int16), 1y (sbyte), 1uy (byte), 1.0 (float), 1.0f (float32), 1.0m (decimal), 1I (BigInteger).";
                FSharpErrorSeverity.Error, 599, (2, 11, 2, 12),"Missing qualification after '.'"
            |]

    [<Fact>]
    let ``_52 is invalid numeric literal``() =
        CompilerAssert.TypeCheckSingleError
            """
let x = _52
            """
            FSharpErrorSeverity.Error
            39
            (2, 9, 2, 12)
            "The value or constructor '_52' is not defined."


    [<Fact>]
    let ``1N is invalid numeric literal``() =
        CompilerAssert.TypeCheckSingleError
            """
let x = 1N
            """
            FSharpErrorSeverity.Error
            0784
            (2, 9, 2, 11)
            "This numeric literal requires that a module 'NumericLiteralN' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope"

    [<Fact>]
    let ``1N is invalid numeric literal in FSI``() =
        if Utils.runningOnMono then ()
        else 
            CompilerAssert.RunScriptWithOptions [| "--langversion:preview"; "--test:ErrorRanges" |]
                """
let x = 1N
                """
                [
                    "This numeric literal requires that a module 'NumericLiteralN' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope";
                    "Operation could not be completed due to earlier error"
                ]

    [<Theory>]
    [<InlineData("1.0E28M")>]
    [<InlineData("1.0E-28M")>]
    let ``Valid Numeric Literals`` literal =
        // Regressiont test for FSharp1.0: 2543 - Decimal literals do not support exponents

        CompilerAssert.Pass ("let x = " + literal)