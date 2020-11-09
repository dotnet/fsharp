// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalFiltering.Basic

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module ByExample =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/Basic/ByExample)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/LexicalFiltering/Basic/ByExample", Includes=[|"BasicCheck01.fs"|])>]
    let ``ByExample - BasicCheck01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

