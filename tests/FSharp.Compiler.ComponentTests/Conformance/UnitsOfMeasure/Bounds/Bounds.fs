// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.UnitsOfMeasure

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Bounds =
    let shouldFailWithDiagnostics expectedDiagnostics compilation =
        compilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics expectedDiagnostics


    [<Theory; FileInlineData("infinity_01.fsx")>]
    let ``infinity_01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 717, Line 12, Col 18, Line 12, Col 27, "Unexpected type arguments")
            (Error 717, Line 13, Col 18, Line 13, Col 26, "Unexpected type arguments")
            (Error 717, Line 14, Col 19, Line 14, Col 28, "Unexpected type arguments")
            (Error 717, Line 15, Col 19, Line 15, Col 27, "Unexpected type arguments")
        ]

    [<Theory;FileInlineData("nan_01.fsx")>]
    let ``nan_01_fsx`` compilation =
        compilation
        |> getCompilation
        |> shouldFailWithDiagnostics [
            (Error 717, Line 7, Col 19, Line 7, Col 22, "Unexpected type arguments")
        ]
