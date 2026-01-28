// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Gccerrors =

    // Migrated from FSharpQA suite - CompilerOptions/fsc/gccerrors
    // Test that --gccerrors outputs errors in gcc format

    [<Theory; FileInlineData("gccerrors01.fs")>]
    let ``gccerrors - gccerrors01_fs - --gccerrors`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--gccerrors"; "--nologo"]
        |> compile
        // The file has incomplete pattern match, should produce warnings in gcc format
        |> withWarningCode 25
        |> withDiagnosticMessageMatches "Incomplete pattern matches"
        |> ignore
