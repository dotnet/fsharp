// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module EvaluationOfElaboratedForms =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/EvaluationOfElaboratedForms)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/EvaluationOfElaboratedForms", Includes=[|"letbinding_precomutation01.fs"|])>]
    let ``EvaluationOfElaboratedForms - letbinding_precomutation01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

