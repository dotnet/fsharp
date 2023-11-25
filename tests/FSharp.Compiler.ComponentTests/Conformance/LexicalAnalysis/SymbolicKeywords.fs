// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module SymbolicKeywords =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicKeywords)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicKeywords", Includes=[|"GreaterThanClosedCurly01.fs"|])>]
    let ``SymbolicKeywords - GreaterThanClosedCurly01_fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicKeywords)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicKeywords", Includes=[|"GreaterThanClosedCurly02.fs"|])>]
    let ``SymbolicKeywords - GreaterThanClosedCurly02_fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicKeywords)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicKeywords", Includes=[|"GreaterThanClosedCurly03.fs"|])>]
    let ``SymbolicKeywords - GreaterThanClosedCurly03_fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicKeywords)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicKeywords", Includes=[|"GreaterThanClosedCurly04.fs"|])>]
    let ``SymbolicKeywords - GreaterThanClosedCurly04_fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicKeywords)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicKeywords", Includes=[|"GreaterThanClosedCurly05.fs"|])>]
    let ``SymbolicKeywords - GreaterThanClosedCurly05_fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/SymbolicKeywords)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/SymbolicKeywords", Includes=[|"GreaterThanClosedSquare01.fs"|])>]
    let ``SymbolicKeywords - GreaterThanClosedSquare01_fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

