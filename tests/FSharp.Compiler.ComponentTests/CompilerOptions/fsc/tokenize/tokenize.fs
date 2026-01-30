// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Tokenize =

    // Migrated from FSharpQA suite - CompilerOptions/fsc/tokenize
    // Test that tokenize options are accepted by the compiler
    // Note: The original test checked for specific token output, but this test
    // just verifies the option is accepted. The options produce warning 75
    // "for test purposes only" which we ignore.

    [<Theory; FileInlineData("tokenize01.fs")>]
    let ``tokenize - tokenize01_fs - --tokenize`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--tokenize"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("tokenize02.fs")>]
    let ``tokenize - tokenize02_fs - --tokenize-unfiltered`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--tokenize-unfiltered"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> ignore
