// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module SymbolicKeywords =

    // SOURCE: GreaterThanClosedParenthesis01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicKeywords", Includes=[|"GreaterThanClosedParenthesis01.fs"|])>]
    let ``SymbolicKeywords - GreaterThanClosedParenthesis01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: GreaterThanClosedSquare02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicKeywords", Includes=[|"GreaterThanClosedSquare02.fs"|])>]
    let ``SymbolicKeywords - GreaterThanClosedSquare02_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore
