// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalAnalysis.Shift

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Generics =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Shift/Generics)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/LexicalAnalysis/Shift/Generics", Includes=[|"RightShift001.fs"|])>]
    let ``Generics - RightShift001.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalAnalysis/Shift/Generics)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/LexicalAnalysis/Shift/Generics", Includes=[|"RightShift002.fs"|])>]
    let ``Generics - RightShift002.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

