// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Staticlink =

    // Migrated from FSharpQA suite - CompilerOptions/fsc/staticlink
    // --staticlink option tests (DesktopOnly)
    // Note: The original tests required multi-file compilation with PRECMD.
    // These tests just verify the --staticlink option is recognized.

    // Test: --staticlink with non-existent assembly produces expected error
    [<Theory; FileInlineData("E_FileNotFound.fs")>]
    let ``staticlink - E_FileNotFound_fs - error`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--staticlink:IDontExist.dll"]
        |> compile
        |> shouldFail
        |> withErrorCode 2012
        |> withDiagnosticMessageMatches "Assembly 'IDontExist\\.dll' not found in dependency set"
        |> ignore
