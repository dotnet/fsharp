// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Parenthesis =
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

    [<Theory; FileInlineData("E_Error02.fsx")>]
    let ``E_Error02_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
        (Error 620, Line 11, Col 19, Line 11, Col 20, "Unexpected integer literal in unit-of-measure expression")
        ]

    [<Theory; FileInlineData("E_Error03.fsx")>]
    let ``E_Error03_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 11, Col 31, Line 11, Col 32, "Unexpected symbol ')' in binding")
        ]

    [<Theory; FileInlineData("E_Error04.fsx")>]
    let ``E_Error04_fs`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 11, Col 34, Line 11, Col 35, "Unexpected symbol '_' in binding")
        ]

    [<Theory; FileInlineData("E_Error05.fsx")>]
    let ``E_Error05_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 11, Col 28, Line 11, Col 29, "Unexpected symbol ')' in binding")
        ]

    [<Theory; FileInlineData("E_Error06.fsx")>]
    let ``E_Error06_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 620, Line 11, Col 20, Line 11, Col 23, "Unexpected integer literal in unit-of-measure expression")
        ]

    [<Theory; FileInlineData("E_IncompleteParens01.fsx")>]
    let ``E_IncompleteParens01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 10, Line 11, Col 1, Line 11, Col 1, "Incomplete structured construct at or before this point in expression. Expected ')' or other token.")
            (Error 583, Line 10, Col 26, Line 10, Col 27, "Unmatched '('")
        ]

    [<Theory; FileInlineData("E_IncompleteParens02.fsx")>]
    let ``E_IncompleteParens02_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 3156, Line 11, Col 24, Line 11, Col 25, "Unexpected token '/' or incomplete expression")
            (Error 10, Line 11, Col 26, Line 11, Col 27, "Unexpected symbol ')' in binding. Expected incomplete structured construct at or before this point or other token.")
        ]

    [<Theory; FileInlineData("W_ImplicitProduct01.fsx")>]
    let ``W_ImplicitProduct01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceedWithDiagnostics [
            (Warning 632, Line 11, Col 25, Line 11, Col 32, "Implicit product of measures following /")
        ]

    [<Theory; FileInlineData("W_Positive01.fsx")>]
    let ``W_Positive01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldSucceedWithDiagnostics [
            (Warning 464, Line 24, Col 33, Line 24, Col 39, "This code is less generic than indicated by its annotations. A unit-of-measure specified using '_' has been determined to be '1', i.e. dimensionless. Consider making the code generic, or removing the use of '_'.")
        ]
