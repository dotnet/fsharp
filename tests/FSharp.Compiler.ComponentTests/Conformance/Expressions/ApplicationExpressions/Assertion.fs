// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.ApplicationExpressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Assertion =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ApplicationExpressions/Assertion)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ApplicationExpressions/Assertion", Includes=[|"Assert_true.fs"|])>]
    let ``Assertion - Assert_true.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ApplicationExpressions/Assertion)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ApplicationExpressions/Assertion", Includes=[|"Assert_false_DEBUG.fs"|])>]
    let ``Assertion - Assert_false_DEBUG.fs - --define:DEBUG`` compilation =
        compilation
        |> withOptions ["--define:DEBUG"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ApplicationExpressions/Assertion)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ApplicationExpressions/Assertion", Includes=[|"Assert_false.fs"|])>]
    let ``Assertion - Assert_false.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

