// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ClrFx.PseudoCustomAttributes

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module AssemblyVersion =

    // This test was automatically generated (moved from FSharpQA suite - ClrFx/PseudoCustomAttributes/AssemblyVersion)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/ClrFx/PseudoCustomAttributes/AssemblyVersion", Includes=[|"AssemblyVersion_001.fs"|])>]
    let ``AssemblyVersion - AssemblyVersion_001.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - ClrFx/PseudoCustomAttributes/AssemblyVersion)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/ClrFx/PseudoCustomAttributes/AssemblyVersion", Includes=[|"AssemblyVersion02.fs"|])>]
    let ``AssemblyVersion - AssemblyVersion02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

