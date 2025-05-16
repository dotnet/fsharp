// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Constants =
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

    [<FileInlineData("decimal.fsx")>]
    [<FileInlineData("ieee32.fsx")>]
    [<FileInlineData("ieee64.fsx")>]
    [<FileInlineData("SpecialSyntax_.fsx")>]
    [<Theory>]
    let ``Constants - shouldSucceed`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceed

    [<Theory; FileInlineData("E_UnsupportedTypes01.fsx")>]
    let ``E_UnsupportedTypes01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 636, Line 22, Col 9, Line 22, Col 15, "Units-of-measure are only supported on float, float32, decimal, and integer types.")
            (Error 636, Line 23, Col 9, Line 23, Col 15, "Units-of-measure are only supported on float, float32, decimal, and integer types.")
        ]
