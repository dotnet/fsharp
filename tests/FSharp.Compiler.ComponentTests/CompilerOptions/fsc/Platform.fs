// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test.Compiler

/// Tests for --platform compiler option (migrated from FSharpQA suite - CompilerOptions/fsc/platform)
module PlatformErrors =

    // =================================================================
    // Platform option error tests - incorrect platform values
    // =================================================================

    [<Theory>]
    [<InlineData("ITANIUM")>]
    [<InlineData("ANYCPU")>]
    [<InlineData("X86")>]
    [<InlineData("X64")>]
    [<InlineData("IA64")>]
    [<InlineData("i386")>]
    [<InlineData("AMD64")>]
    [<InlineData("PPC")>]
    [<InlineData("ARM")>]
    let ``platform - unrecognized platform value produces error 1064`` (platform: string) =
        Fsx """printfn "test" """
        |> withOptions [$"--platform:{platform}"]
        |> compile
        |> shouldFail
        |> withErrorCode 1064
        |> withDiagnosticMessageMatches $"Unrecognized platform '{platform}', valid values are 'x86', 'x64', 'Arm', 'Arm64', 'Itanium', 'anycpu32bitpreferred', and 'anycpu'"
        |> ignore

    // =================================================================
    // Platform option error tests - case sensitivity (option is case-sensitive)
    // =================================================================

    [<Theory>]
    [<InlineData("--PLATFORM:anycpu")>]
    [<InlineData("--PlatForm:anycpu")>]
    let ``platform - case-sensitive option name produces error 243`` (option: string) =
        Fsx """printfn "test" """
        |> withOptions [option]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> ignore

    // =================================================================
    // Platform option error tests - misspelled options
    // =================================================================

    [<Theory>]
    [<InlineData("--platform-:anycpu", "--platform-")>]
    [<InlineData("--PLATFORM+:anycpu", "--PLATFORM+")>]
    [<InlineData("---platform:anycpu", "---platform")>]
    let ``platform - misspelled option produces error 243`` (option: string, expectedOption: string) =
        Fsx """printfn "test" """
        |> withOptions [option]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> withDiagnosticMessageMatches $"Unrecognized option: '{System.Text.RegularExpressions.Regex.Escape(expectedOption)}'"
        |> ignore

    // =================================================================
    // Platform option error tests - missing argument
    // =================================================================

    [<Fact>]
    let ``platform - missing argument produces error 224`` () =
        Fsx """printfn "test" """
        |> withOptions ["--platform"]
        |> compile
        |> shouldFail
        |> withErrorCode 224
        |> withDiagnosticMessageMatches "Option requires parameter: --platform"
        |> ignore

    // =================================================================
    // Platform option error tests - anycpu32bitpreferred with library
    // =================================================================

    [<Fact>]
    let ``platform - anycpu32bitpreferred with library target produces error 3150`` () =
        Fs """module M"""
        |> withOptions ["--target:library"; "--platform:anycpu32bitpreferred"]
        |> compile
        |> shouldFail
        |> withErrorCode 3150
        |> withDiagnosticMessageMatches "The 'anycpu32bitpreferred' platform can only be used with EXE targets. You must use 'anycpu' instead."
        |> ignore

