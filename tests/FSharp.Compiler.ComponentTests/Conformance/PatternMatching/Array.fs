// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.PatternMatching

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Array =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Array)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Array", Includes=[|"arrayMatch01.fs"|])>]
    let ``Array - arrayMatch01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Array)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Array", Includes=[|"arrayMatch02.fs"|])>]
    let ``Array - arrayMatch02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Array)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/Array", Includes=[|"arrayMatch03.fs"|])>]
    let ``Array - arrayMatch03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

