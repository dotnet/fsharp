// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.InferenceProcedures

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ResolvingApplicationExpressions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/InferenceProcedures/ResolvingApplicationExpressions)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/InferenceProcedures/ResolvingApplicationExpressions", Includes=[|"ComplexExpression01.fs"|])>]
    let ``ResolvingApplicationExpressions - ComplexExpression01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

