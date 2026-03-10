// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsi

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

// Migrated from FSharpQA suite - CompilerOptions/fsi/nologo
// Tests for FSI --nologo option behavior.
// Note: The original tests check FSI console banner output using runFsi.

module Nologo =

    // Test: --nologo option is recognized by FSI
    // Original: SOURCE=nologo01.fsx FSIMODE=PIPE SCFLAGS="--nologo"
    [<Fact>]
    let ``fsi nologo - option is recognized`` () =
        Fsx """1+1"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldSucceed

    // Test: FSI startup without --nologo shows copyright
    // Original: SOURCE=nologo02.fsx FSIMODE=PIPE (no --nologo flag)
    // The original test checks for Microsoft copyright message in output.
    [<Fact>]
    let ``fsi nologo - startup without nologo shows copyright`` () =
        Fsx """1+1"""
        |> runFsi
        |> shouldSucceed
        |> withStdOutContains "Microsoft"
        |> withStdOutContains "Copyright"
