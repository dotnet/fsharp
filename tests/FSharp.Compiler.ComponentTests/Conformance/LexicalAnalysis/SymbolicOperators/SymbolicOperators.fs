// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module SymbolicOperators =
    let shouldFailWithDiagnostics expectedDiagnostics compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics expectedDiagnostics

    let shouldSucceed compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    [<FileInlineData("GreaterThanColon001.fsx")>]
    [<FileInlineData("GreaterThanColon002.fsx")>]
    [<FileInlineData("GreaterThanDotParen01.fsx")>]
    [<FileInlineData("LessThanDotOpenParen001.fsx")>]
    [<FileInlineData("QMarkArguments.fsx")>]
    [<FileInlineData("QMarkAssignSimple.fsx")>]
    [<FileInlineData("QMarkExpressionAsArgument.fsx")>]
    [<FileInlineData("QMarkExpressionAsArgument2.fsx")>]
    [<FileInlineData("QMarkNested.fsx")>]
    [<FileInlineData("QMarkPrecedenceArray.fsx")>]
    [<FileInlineData("QMarkPrecedenceCurrying.fsx")>]
    [<FileInlineData("QMarkPrecedenceInArrays.fsx")>]
    [<FileInlineData("QMarkPrecedenceMethodCall.fsx")>]
    [<FileInlineData("QMarkPrecedenceMethodCallSpace.fsx")>]
    [<FileInlineData("QMarkPrecedenceSpace.fsx")>]
    [<FileInlineData("QMarkSimple.fsx")>]
    [<Theory>]
    let ``SymbolicOperators`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceed

    [<Theory; FileInlineData("E_CantUseDollarSign.fsx")>]
    let ``E_CantUseDollarSign_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 35, Line 7, Col 6, Line 7, Col 11, "This construct is deprecated: '$' is not permitted as a character in operator names and is reserved for future use")
        ]

    [<Theory; FileInlineData("E_GreaterThanDotParen01.fsx")>]
    let ``E_GreaterThanDotParen01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1208, Line 9, Col 7, Line 9, Col 10, "Invalid operator definition. Prefix operator definitions must use a valid prefix operator name.")
            (Error 1208, Line 10, Col 7, Line 10, Col 10, "Invalid operator definition. Prefix operator definitions must use a valid prefix operator name.")
            (Error 1208, Line 12, Col 7, Line 12, Col 11, "Invalid operator definition. Prefix operator definitions must use a valid prefix operator name.")
            (Error 1208, Line 13, Col 7, Line 13, Col 11, "Invalid operator definition. Prefix operator definitions must use a valid prefix operator name.")
        ]

    [<Theory; FileInlineData("E_QMarkGeneric.fsx")>]
    let ``E_QMarkGeneric_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 717, Line 16, Col 31, Line 16, Col 36, "Unexpected type arguments")
        ]

