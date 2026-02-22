// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Tailcalls =

    // Migrated from FSharpQA suite - CompilerOptions/fsc/tailcalls
    // Sanity check - simply check that the option is valid

    [<Theory; FileInlineData("tailcalls01.fs")>]
    let ``tailcalls - tailcalls01_fs - --tailcalls`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--tailcalls"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("tailcalls01.fs")>]
    let ``tailcalls - tailcalls01_fs - --tailcalls+`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--tailcalls+"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("tailcalls01.fs")>]
    let ``tailcalls - tailcalls01_fs - --tailcalls-`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--tailcalls-"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("tailcalls01.fs")>]
    let ``tailcalls - tailcalls01_fs - --tailcalls - fsi`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--tailcalls"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("tailcalls01.fs")>]
    let ``tailcalls - tailcalls01_fs - --tailcalls+ - fsi`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--tailcalls+"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("tailcalls01.fs")>]
    let ``tailcalls - tailcalls01_fs - --tailcalls- - fsi`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--tailcalls-"]
        |> compile
        |> shouldSucceed
        |> ignore
