// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test.Compiler

/// Smoke tests for compiler options that have no dedicated test coverage
module UncoveredOptions =

    // =================================================================
    // Switch options (+/-) — verify parser accepts both polarities
    // =================================================================

    [<Theory>]
    [<InlineData("--realsig+")>]
    [<InlineData("--realsig-")>]
    [<InlineData("--compressmetadata+")>]
    [<InlineData("--compressmetadata-")>]
    [<InlineData("--checknulls+")>]
    [<InlineData("--checknulls-")>]
    [<InlineData("--strict-indentation+")>]
    [<InlineData("--strict-indentation-")>]
    [<InlineData("--quotations-debug+")>]
    [<InlineData("--quotations-debug-")>]
    [<InlineData("--tailcalls+")>]
    [<InlineData("--tailcalls-")>]
    [<InlineData("--deterministic+")>]
    [<InlineData("--deterministic-")>]
    let ``switch options are accepted by the parser`` (option: string) =
        Fs """module M"""
        |> withOptions [option]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // =================================================================
    // Unit options (no argument) — verify parser accepts them
    // =================================================================

    [<Theory>]
    [<InlineData("--nooptimizationdata")>]
    [<InlineData("--nointerfacedata")>]
    [<InlineData("--nocopyfsharpcore")>]
    [<InlineData("--nowin32manifest")>]
    [<InlineData("--allsigs")>]         // typecheck only — compile would write .fsi files
    [<InlineData("--utf8output")>]
    [<InlineData("--fullpaths")>]
    let ``unit options are accepted by the parser`` (option: string) =
        Fs """module M"""
        |> withOptions [option]
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // =================================================================
    // String options with valid values — verify parser + compilation
    // =================================================================

    [<Theory>]
    [<InlineData("SHA1")>]
    [<InlineData("SHA256")>]
    let ``checksumalgorithm with valid algorithm is accepted`` (algorithm: string) =
        Fs """module M"""
        |> withOptions [$"--checksumalgorithm:{algorithm}"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Theory>]
    [<InlineData("--target:exe")>]
    [<InlineData("--target:winexe")>]
    [<InlineData("--target:library")>]
    [<InlineData("--target:module")>]
    let ``target with valid values is accepted`` (option: string) =
        Fs """module M"""
        |> withOptions [option]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> ignore

    // =================================================================
    // Compilation modes
    // =================================================================

    [<Fact>]
    let ``parseonly does not report type errors`` () =
        Fs """let x: int = "not an int" """
        |> asExe
        |> withOptions ["--parseonly"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> ignore

    // =================================================================
    // Diagnostic format options
    // =================================================================

    [<Fact>]
    let ``maxerrors with valid value is accepted`` () =
        Fs """module M"""
        |> withOptions ["--maxerrors:10"]
        |> compile
        |> shouldSucceed
        |> ignore
