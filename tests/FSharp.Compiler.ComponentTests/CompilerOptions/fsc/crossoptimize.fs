// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module crossoptimize =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/crossoptimize)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/crossoptimize", Includes=[|"crossoptimize01.fs"|])>]
    let ``crossoptimize - crossoptimize01.fs - --crossoptimize`` compilation =
        compilation
        |> withOptions ["--crossoptimize"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/crossoptimize)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/crossoptimize", Includes=[|"crossoptimize01.fs"|])>]
    let ``crossoptimize - crossoptimize01.fs - --crossoptimize+`` compilation =
        compilation
        |> withOptions ["--crossoptimize+"]
        |> compile
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/crossoptimize)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/crossoptimize", Includes=[|"crossoptimize01.fs"|])>]
    let ``crossoptimize - crossoptimize01.fs - --crossoptimize-`` compilation =
        compilation
        |> withOptions ["--crossoptimize-"]
        |> compile
        |> shouldSucceed

