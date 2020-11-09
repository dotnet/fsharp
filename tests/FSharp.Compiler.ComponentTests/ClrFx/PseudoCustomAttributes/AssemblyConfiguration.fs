// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ClrFx.PseudoCustomAttributes

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module AssemblyConfiguration =

    // This test was automatically generated (moved from FSharpQA suite - ClrFx/PseudoCustomAttributes/AssemblyConfiguration)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/ClrFx/PseudoCustomAttributes/AssemblyConfiguration", Includes=[|"AssemblyConfiguration_001.fs"|])>]
    let ``AssemblyConfiguration - AssemblyConfiguration_001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

