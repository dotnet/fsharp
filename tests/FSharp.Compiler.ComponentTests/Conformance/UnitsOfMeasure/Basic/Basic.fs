// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Basic =
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

    [<FileInlineData("Calculus.fsx")>]
    [<FileInlineData("Mars.fsx")>]
    [<FileInlineData("MassForce.fsx")>]
    [<FileInlineData("Misc01.fsx")>]
    [<FileInlineData("Misc03.fsx")>]
    [<FileInlineData("Misc04.fsx")>]
    [<FileInlineData("Quotation04_hidden.fsx")>]
    [<FileInlineData("RationalExponents01.fsx")>]
    [<FileInlineData("Stats.fsx")>]
    [<FileInlineData("SI.fsx")>]
    [<Theory>]
    let ``Basic - shouldSucceed`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceed

    [<Theory; FileInlineData("E_MassForce.fsx")>]
    let ``Basic - E_MassForce`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 1, Line 15, Col 27, Line 15, Col 32, "The unit of measure 'N' does not match the unit of measure 'kg'")
            (Error 43, Line 15, Col 25, Line 15, Col 26, "The unit of measure 'N' does not match the unit of measure 'kg'")
        ]

    [<Theory; FileInlineData("Misc02.fsx")>]
    let ``Basic - Misc02_fs`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Warning 64, Line 36, Col 60, Line 36, Col 61, "This construct causes code to be less generic than indicated by the type annotations. The unit-of-measure variable 'v has been constrained to be measure ''u/'w'.")
        ]
