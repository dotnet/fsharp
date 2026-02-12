// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions.ClassTypes

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module StaticLetDoDeclarations =

    // Error tests

    [<Theory; FileInlineData("E_LexicalScoping01.fs")>]
    let ``E_LexicalScoping01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 39

    [<Theory; FileInlineData("E_RecMutable01.fs")>]
    let ``E_RecMutable01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 874

    // Success tests

    [<Theory; FileInlineData("WithValueType.fs")>]
    let ``WithValueType_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("WithReferenceType.fs")>]
    let ``WithReferenceType_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("WithStringType.fs")>]
    let ``WithStringType_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("Attributes01.fs")>]
    let ``Attributes01_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("LexicalScoping01.fs")>]
    let ``LexicalScoping01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("RecNonMutable01.fs")>]
    let ``RecNonMutable01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("Offside01.fs")>]
    let ``Offside01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed
