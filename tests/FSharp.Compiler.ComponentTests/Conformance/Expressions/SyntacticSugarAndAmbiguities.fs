// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module SyntacticSugarAndAmbiguities =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/SyntacticSugarAndAmbiguities)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/Expressions/SyntacticSugarAndAmbiguities", Includes=[|"SyntacticSugar01.fs"|])>]
    let ``SyntacticSugarAndAmbiguities - SyntacticSugar01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

