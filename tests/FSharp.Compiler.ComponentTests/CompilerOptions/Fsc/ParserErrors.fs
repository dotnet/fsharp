// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test.Compiler

/// Tests for compiler options parser error handling across many options
module ParserErrors =

    // =================================================================
    // Missing argument — options that require a parameter (error 224)
    // =================================================================

    [<Theory>]
    [<InlineData("--out")>]
    [<InlineData("--doc")>]
    [<InlineData("--keyfile")>]
    [<InlineData("--platform")>]
    [<InlineData("--win32icon")>]
    [<InlineData("--win32res")>]
    [<InlineData("--win32manifest")>]
    [<InlineData("--resource")>]
    [<InlineData("--linkresource")>]
    [<InlineData("--baseaddress")>]
    [<InlineData("--pdb")>]
    [<InlineData("--lib")>]
    [<InlineData("--codepage")>]
    [<InlineData("--sourcelink")>]
    [<InlineData("--subsystemversion")>]
    [<InlineData("--checksumalgorithm")>]
    [<InlineData("--define")>]
    [<InlineData("--warn")>]
    [<InlineData("--maxerrors")>]
    let ``options requiring a parameter produce error 224 when missing`` (option: string) =
        Fs """module M"""
        |> withOptions [option]
        |> compile
        |> shouldFail
        |> withErrorCode 224
        |> withDiagnosticMessageMatches "Option requires parameter"
        |> ignore

    // =================================================================
    // Invalid integer argument — options expecting int get a non-integer
    // =================================================================

    [<Theory>]
    [<InlineData("--maxerrors:xyz")>]
    [<InlineData("--codepage:abc")>]
    let ``int options with non-integer argument produce error 241`` (option: string) =
        Fs """module M"""
        |> withOptions [option]
        |> compile
        |> shouldFail
        |> withErrorCode 241
        |> withDiagnosticMessageMatches "is not a valid integer argument"
        |> ignore

    // =================================================================
    // Invalid warn level — out of valid range 0-5
    // =================================================================

    [<Fact>]
    let ``warn with negative level produces error 1050`` () =
        Fs """module M"""
        |> withOptions ["--warn:-1"]
        |> compile
        |> shouldFail
        |> withErrorCode 1050
        |> withDiagnosticMessageMatches "Invalid warning level"
        |> ignore

    // =================================================================
    // Invalid target value
    // =================================================================

    [<Fact>]
    let ``target with unrecognized value produces error 1048`` () =
        Fs """module M"""
        |> withOptions ["--target:dll"]
        |> compile
        |> shouldFail
        |> withErrorCode 1048
        |> withDiagnosticMessageMatches "Unrecognized target"
        |> ignore

    // =================================================================
    // Invalid checksum algorithm
    // =================================================================

    [<Fact>]
    let ``checksumalgorithm with unsupported algorithm produces error 1065`` () =
        Fs """module M"""
        |> withOptions ["--checksumalgorithm:MD5"]
        |> compile
        |> shouldFail
        |> withErrorCode 1065
        |> withDiagnosticMessageMatches "Algorithm.*is not supported"
        |> ignore

    // =================================================================
    // Unrecognized option (error 243)
    // =================================================================

    [<Theory>]
    [<InlineData("--nonexistent")>]
    [<InlineData("--Optimize")>]      // case-sensitive: --optimize (parsed as --optimize+) works, --Optimize does not
    [<InlineData("---debug")>]
    let ``unrecognized options produce error 243`` (option: string) =
        Fs """module M"""
        |> withOptions [option]
        |> compile
        |> shouldFail
        |> withErrorCode 243
        |> withDiagnosticMessageMatches "Unrecognized option"
        |> ignore
