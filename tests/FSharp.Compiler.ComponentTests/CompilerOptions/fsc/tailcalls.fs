// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module tailcalls =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/tailcalls)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/tailcalls", Includes=[|"tailcalls01.fs"|])>]
    let ``tailcalls - tailcalls01.fs - --tailcalls`` compilation =
        compilation
        |> withOptions ["--tailcalls"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/tailcalls)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/tailcalls", Includes=[|"tailcalls01.fs"|])>]
    let ``tailcalls - tailcalls01.fs - --tailcalls+`` compilation =
        compilation
        |> withOptions ["--tailcalls+"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/tailcalls)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/tailcalls", Includes=[|"tailcalls01.fs"|])>]
    let ``tailcalls - tailcalls01.fs - --tailcalls-`` compilation =
        compilation
        |> withOptions ["--tailcalls-"]
        |> typecheck
        |> shouldSucceed

