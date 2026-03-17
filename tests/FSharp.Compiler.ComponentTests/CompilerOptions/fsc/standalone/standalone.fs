// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Standalone =

    // Migrated from FSharpQA suite - CompilerOptions/fsc/standalone
    // --standalone option tests (DesktopOnly)
    // Note: The original tests required multi-file compilation with PRECMD.
    // These tests just verify the --standalone option is recognized.

    [<Fact>]
    let ``standalone - option is recognized`` () =
        FSharp """
module TestModule
let x = 1
        """
        |> asLibrary
        |> withOptions ["--standalone"]
        |> compile
        |> shouldSucceed
        |> ignore
