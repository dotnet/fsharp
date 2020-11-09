// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalAnalysis

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module Whitespace =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Whitespace)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Whitespace", Includes=[|"WhiteSpace01.fs"|])>]
    let ``Whitespace - WhiteSpace01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

