// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module EvaluationOfElaboratedForms =

    // Migrated from: tests/fsharpqa/Source/Conformance/Expressions/EvaluationOfElaboratedForms
    // Test count: 1

    // SOURCE=letbinding_precomutation01.fs - Regression test for FSHARP1.0:3330
    [<Theory; FileInlineData("letbinding_precomutation01.fs")>]
    let ``letbinding_precomutation01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--nowarn:3873"]  // Deprecated sequence expression warning
        |> compile
        |> shouldSucceed
