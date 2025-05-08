// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module NumericLiterals =
    let shouldFailWithDiagnostics expectedDiagnostics compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> withNoWarn 52
        |> typecheck
        |> shouldFail
        |> withDiagnostics expectedDiagnostics

    let shouldSucceed compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    [<FileInlineData("BigIntConversion02.fsx")>]
    [<FileInlineData("BigIntConversion02b.fsx")>]
    [<FileInlineData("BigNums01.fsx")>]
    [<FileInlineData("casingBin.fsx")>]
    [<FileInlineData("casingHex.fsx")>]
    [<FileInlineData("casingOct.fsx")>]
    [<FileInlineData("enumNegativeLiterals001.fsx")>]
    [<FileInlineData("MaxLiterals01.fsx")>]
    [<FileInlineData("NumericLiterals01.fsx")>]
    [<Theory>]
    let ``NumericLiterals`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceed

    [<Theory;FileInlineData("E_BigIntConversion01.fsx")>]
    let ``E_BigIntConversion01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1, Line 29, Col 14, Line 29, Col 17, "The type 'System.Numerics.BigInteger' does not support a conversion to the type 'char'")
        ]

    [<Theory;FileInlineData("E_BigIntConversion01b.fsx")>]
    let ``E_BigIntConversion01b_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1, Line 8, Col 14, Line 8, Col 17, "The type 'System.Numerics.BigInteger' does not support a conversion to the type 'char'")
        ]

    [<Theory;FileInlineData("E_BigNumNotImpl01.fsx")>]
    let ``E_BigNumNotImpl01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 39, Line 11, Col 23, Line 11, Col 34, "The value, constructor, namespace or type 'FromInt32' is not defined.")
            (Error 39, Line 12, Col 23, Line 12, Col 43, "The value, constructor, namespace or type 'FromInt64' is not defined.")
            (Error 39, Line 13, Col 23, Line 13, Col 44, "The value, constructor, namespace or type 'FromString' is not defined.")
        ]

    [<Theory;FileInlineData("E_BigNums01.fsx")>]
    let ``E_BigNums01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 784, Line 10, Col 9, Line 10, Col 20, "This numeric literal requires that a module 'NumericLiteralN' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope")
            (Error 784, Line 11, Col 9, Line 11, Col 20, "This numeric literal requires that a module 'NumericLiteralZ' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope")
            (Error 784, Line 12, Col 9, Line 12, Col 20, "This numeric literal requires that a module 'NumericLiteralQ' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope")
            (Error 784, Line 13, Col 9, Line 13, Col 20, "This numeric literal requires that a module 'NumericLiteralR' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope")
            (Error 784, Line 14, Col 9, Line 14, Col 20, "This numeric literal requires that a module 'NumericLiteralG' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope")
        ]

    [<Theory;FileInlineData("E_BigNums40.fsx")>]
    let ``E_BigNums40_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 784, Line 10, Col 9, Line 10, Col 20, "This numeric literal requires that a module 'NumericLiteralN' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope")
            (Error 784, Line 11, Col 9, Line 11, Col 20, "This numeric literal requires that a module 'NumericLiteralZ' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope")
            (Error 784, Line 12, Col 9, Line 12, Col 20, "This numeric literal requires that a module 'NumericLiteralQ' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope")
            (Error 784, Line 13, Col 9, Line 13, Col 20, "This numeric literal requires that a module 'NumericLiteralR' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope")
            (Error 784, Line 14, Col 9, Line 14, Col 20, "This numeric literal requires that a module 'NumericLiteralG' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope")
        ]

    [<FileInlineData("E_casingIEEE-lf-LF01.fsx")>]
    let ``E_casingIEEE-lf-LF01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1, Line 8, Col 30, Line 8, Col 31, "Unexpected symbol '-' in union case")
        ]

    [<FileInlineData("E_casingIEEE-lf-LF02.fsx")>]
    let ``E_casingIEEE-lf-LF02_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1, Line 8, Col 30, Line 8, Col 31, "Unexpected symbol '-' in union case")
        ]
    [<FileInlineData("E_casingIEEE-lf-LF03a.fsx")>]
    let ``E_casingIEEE-lf-LF03a_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1, Line 8, Col 30, Line 8, Col 31, "Unexpected symbol '-' in union case")
        ]
    [<FileInlineData("E_casingIEEE-lf-LF03b.fsx")>]
    let ``E_casingIEEE-lf-LF03b_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1, Line 8, Col 30, Line 8, Col 31, "Unexpected symbol '-' in union case")
        ]

    [<Theory;FileInlineData("E_DecimalWO0Prefix.fsx")>]
    let ``E_DecimalWO0Prefix_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 6, Col 9, Line 6, Col 10, "Unexpected symbol '.' in binding")
        ]

    [<Theory;FileInlineData("E_enumNegativeLiterals001.fsx")>]
    let ``E_enumNegativeLiterals001_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 8, Col 30, Line 8, Col 31, "Unexpected symbol '-' in union case")
        ]

    [<Theory;FileInlineData("E_enumNegativeLiterals002.fsx")>]
    let ``E_enumNegativeLiterals002_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 8, Col 30, Line 8, Col 31, "Unexpected symbol '-' in union case")
        ]

    [<Theory;FileInlineData("E_enumNegativeLiterals003.fsx")>]
    let ``E_enumNegativeLiterals003_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 8, Col 30, Line 8, Col 31, "Unexpected symbol '-' in union case")
        ]

    [<Theory;FileInlineData("E_enumNegativeLiterals004.fsx")>]
    let ``E_enumNegativeLiterals004_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 8, Col 30, Line 8, Col 31, "Unexpected symbol '-' in union case")
        ]

    [<Theory;FileInlineData("E_enumNegativeLiterals005.fsx")>]
    let ``E_enumNegativeLiterals005_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 8, Col 30, Line 8, Col 31, "Unexpected symbol '-' in union case")
        ]

    [<Theory;FileInlineData("E_enumNegativeLiterals006.fsx")>]
    let ``E_enumNegativeLiterals006_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 8, Col 30, Line 8, Col 31, "Unexpected symbol '-' in union case")
        ]

    [<Theory;FileInlineData("E_enumNegativeLiterals007.fsx")>]
    let ``E_enumNegativeLiterals007_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 8, Col 30, Line 8, Col 31, "Unexpected symbol '-' in union case")
        ]

    [<Theory;FileInlineData("E_enumNegativeLiterals008.fsx")>]
    let ``E_enumNegativeLiterals008_fs`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 951, Line 8, Col 23, Line 8, Col 34, "Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char")
        ]

    [<Theory;FileInlineData("E_enumNegativeLiterals009.fsx")>]
    let ``E_enumNegativeLiterals009_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 951, Line 10, Col 23, Line 10, Col 34, "Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char")
            (Error 951, Line 11, Col 23, Line 11, Col 35, "Literal enumerations must have type int, uint, int16, uint16, int64, uint64, byte, sbyte or char")
        ]

    [<Theory;FileInlineData("E_InvalidIEEE64.fsx")>]
    let ``E_InvalidIEEE64_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1153, Line 7, Col 20, Line 7, Col 46, "Invalid floating point number")
        ]

    [<Theory;FileInlineData("E_MaxLiterals01.fsx")>]
    let ``E_MaxLiterals01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1142, Line 18, Col 16, Line 18, Col 20, "This number is outside the allowable range for 8-bit signed integers")
            (Error 1145, Line 21, Col 17, Line 21, Col 23, "This number is outside the allowable range for 16-bit signed integers")
            (Error 1147, Line 24, Col 17, Line 24, Col 27, "This number is outside the allowable range for 32-bit signed integers")
            (Error 1149, Line 27, Col 17, Line 27, Col 37, "This number is outside the allowable range for 64-bit signed integers")
            (Error 1142, Line 19, Col 16, Line 19, Col 20, "This number is outside the allowable range for 8-bit signed integers")
            (Error 1145, Line 22, Col 17, Line 22, Col 23, "This number is outside the allowable range for 16-bit signed integers")
            (Error 1147, Line 25, Col 17, Line 25, Col 27, "This number is outside the allowable range for 32-bit signed integers")
            (Error 1149, Line 28, Col 17, Line 28, Col 37, "This number is outside the allowable range for 64-bit signed integers")
        ]

    [<Theory;FileInlineData("E_MaxLiterals02.fsx")>]
    let ``E_MaxLiterals02_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1144, Line 12, Col 16, Line 12, Col 21, "This number is outside the allowable range for 8-bit unsigned integers")
            (Error 1146, Line 14, Col 17, Line 14, Col 24, "This number is outside the allowable range for 16-bit unsigned integers")
            (Error 1148, Line 16, Col 17, Line 16, Col 28, "This number is outside the allowable range for 32-bit unsigned integers")
            (Error 1150, Line 18, Col 17, Line 18, Col 39, "This number is outside the allowable range for 64-bit unsigned integers")
        ]

    [<Theory;FileInlineData("E_MaxLiterals03.fsx")>]
    let ``E_MaxLiterals03_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1, Line 12, Col 18, Line 12, Col 21, "The type 'byte' does not support the operator '~-'")
            (Error 1, Line 14, Col 18, Line 14, Col 21, "The type 'uint16' does not support the operator '~-'")
            (Error 1, Line 16, Col 18, Line 16, Col 20, "The type 'uint32' does not support the operator '~-'")
            (Error 1, Line 18, Col 18, Line 18, Col 21, "The type 'uint64' does not support the operator '~-'")
        ]

    [<Theory;FileInlineData("E_MaxLiterals04.fsx")>]
    let ``E_MaxLiterals04_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1149, Line 10, Col 24, Line 10, Col 108, "This number is outside the allowable range for 64-bit signed integers")
            (Error 1149, Line 11, Col 24, Line 11, Col 56, "This number is outside the allowable range for 64-bit signed integers")
        ]

