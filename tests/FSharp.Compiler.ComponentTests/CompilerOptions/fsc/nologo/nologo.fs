// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Nologo =

    // Migrated from FSharpQA suite - CompilerOptions/fsc/nologo
    // Tests that the --nologo option is recognized and works

    [<Theory; FileInlineData("dummy.fsx")>]
    let ``nologo - compile without nologo`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory; FileInlineData("dummy.fsx")>]
    let ``nologo - compile with nologo`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--nologo"]
        |> compile
        |> shouldSucceed
        |> ignore
