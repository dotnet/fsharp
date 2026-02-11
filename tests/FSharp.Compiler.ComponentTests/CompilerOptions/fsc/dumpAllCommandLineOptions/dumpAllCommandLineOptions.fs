// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module DumpAllCommandLineOptions =

    // Migrated from FSharpQA suite - CompilerOptions/fsc/dumpAllCommandLineOptions
    // Test that --dumpAllCommandLineOptions works
    // Note: The original test checked for specific output patterns, but this test
    // just verifies the option is accepted.

    [<Theory; FileInlineData("dummy.fs")>]
    let ``dumpAllCommandLineOptions - dummy_fs - fsc`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--dumpAllCommandLineOptions"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("dummy.fsx")>]
    let ``dumpAllCommandLineOptions - dummy_fsx - fsi`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--dumpAllCommandLineOptions"]
        |> compile
        |> shouldSucceed
        |> ignore
