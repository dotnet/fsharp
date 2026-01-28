// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Whitespace =

    // SOURCE: WhiteSpace01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Whitespace", Includes=[|"WhiteSpace01.fs"|])>]
    let ``Whitespace - WhiteSpace01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore
