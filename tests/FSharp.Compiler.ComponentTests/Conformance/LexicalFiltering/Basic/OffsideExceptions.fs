// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalFiltering.Basic

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Xunit.Attributes

module OffsideExceptions =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/Basic/OffsideExceptions)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/LexicalFiltering/Basic/OffsideExceptions", Includes=[|"InfixTokenPlusOne.fs"|])>]
    let ``OffsideExceptions - InfixTokenPlusOne.fs - `` compilation =
        compilation
        |> asFsx
        |> typecheck
        |> shouldSucceed
        |> ignore

