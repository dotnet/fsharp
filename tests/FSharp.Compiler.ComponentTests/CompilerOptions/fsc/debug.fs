// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module debug =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/debug)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/debug", Includes=[|"debug02.fs"|])>]
    let ``debug - debug02.fs - --debug-`` compilation =
        compilation
        |> withOptions ["--debug-"]
        |> compile
        |> shouldSucceed

