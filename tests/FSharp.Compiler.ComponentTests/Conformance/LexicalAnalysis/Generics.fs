// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Generics =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Shift/Generics)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Shift/Generics", Includes=[|"RightShift001.fs"|])>]
    let ``Generics - RightShift001_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Shift/Generics)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Shift/Generics", Includes=[|"RightShift002.fs"|])>]
    let ``Generics - RightShift002_fs`` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

