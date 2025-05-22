// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Parsing =
    let shouldFailWithDiagnostics expectedDiagnostics compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics expectedDiagnostics

    let shouldSucceedWithDiagnostics expectedDiagnostics compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> withDiagnostics expectedDiagnostics

    let shouldSucceed compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    [<FileInlineData("GreaterBarRBrack01.fsx")>]
    [<FileInlineData("PowerSynonym.fsx")>]
    [<FileInlineData("Quotient.fsx")>]
    [<FileInlineData("QuotientAssoc.fsx")>]
    [<FileInlineData("Reciprocal01.fsx")>]
    [<Theory>]
    let ``Parsing - shouldSucceed`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceed

    [<Theory; FileInlineData("E_KnownTypeAsUnit01b.fsx")>]
    let ``E_KnownTypeAsUnit01b_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 705, Line 14, Col 9, Line 14, Col 14, "Expected unit-of-measure, not type")
            (Error 705, Line 17, Col 17, Line 17, Col 20, "Expected unit-of-measure, not type")
            (Error 705, Line 20, Col 24, Line 20, Col 27, "Expected unit-of-measure, not type")
            (Error 705, Line 23, Col 37, Line 23, Col 42, "Expected unit-of-measure, not type")
            (Error 705, Line 26, Col 38, Line 26, Col 41, "Expected unit-of-measure, not type")
        ]

    [<Theory; FileInlineData("E_Nesting01.fsx")>]
    let ``E_Nesting01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 714, Line 12, Col 19, Line 12, Col 20, "Anonymous unit-of-measure cannot be nested inside another unit-of-measure expression")
            (Error 707, Line 17, Col 17, Line 17, Col 21, "Unit-of-measure cannot be used in type constructor application")
        ]

    [<Theory; FileInlineData("W_find_gtdef.fsx")>]
    let ``W_find_gtdef_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceedWithDiagnostics [
            (Warning 64, Line 16, Col 21, Line 16, Col 22, "This construct causes code to be less generic than indicated by the type annotations. The unit-of-measure variable 'u has been constrained to be measure 'Continuous'.")
        ]
