// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.PatternMatching

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module As =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/As)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/As", Includes=[|"asPattern01.fs"|])>]
    let ``As - asPattern01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

