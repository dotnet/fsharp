// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.PatternMatching

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Null =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Null)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Null", Includes=[|"matchNull01.fs"|])>]
    let ``Null - matchNull01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

