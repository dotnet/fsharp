// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module DelegateTypes =

    // Error tests

    [<Theory; FileInlineData("E_InvalidSignature01.fs")>]
    let ``E_InvalidSignature01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 949

    [<Theory; FileInlineData("E_InvalidSignature02.fs")>]
    let ``E_InvalidSignature02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 950

    // Success tests

    [<Theory; FileInlineData("ByrefArguments01.fs")>]
    let ``ByrefArguments01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ValidSignature_MultiArg01.fs")>]
    let ``ValidSignature_MultiArg01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ValidSignature_ReturningValues01.fs")>]
    let ``ValidSignature_ReturningValues01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
