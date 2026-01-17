// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AbstractMembers =

    // Error tests

    [<Theory; FileInlineData("E_CallToAbstractMember01.fs")>]
    let ``E_CallToAbstractMember01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1201

    [<Theory; FileInlineData("E_CallToAbstractMember02.fs")>]
    let ``E_CallToAbstractMember02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 365

    [<Theory; FileInlineData("E_CallToAbstractMember03.fs")>]
    let ``E_CallToAbstractMember03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 365

    [<Theory; FileInlineData("E_CallToAbstractMember04.fs")>]
    let ``E_CallToAbstractMember04_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1201

    [<Theory; FileInlineData("E_CallToUnimplementedMethod01.fs")>]
    let ``E_CallToUnimplementedMethod01_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1201

    [<Theory; FileInlineData("E_InlineVirtualMember01.fs")>]
    let ``E_InlineVirtualMember01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3151

    [<Theory; FileInlineData("E_InlineVirtualMember02.fs")>]
    let ``E_InlineVirtualMember02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3151

    // Success tests

    [<Theory; FileInlineData("DerivedClassSameAssembly.fs")>]
    let ``DerivedClassSameAssembly_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("CallToVirtualMember01.fs")>]
    let ``CallToVirtualMember01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("CallToVirtualMember02.fs")>]
    let ``CallToVirtualMember02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("CallToVirtualMember03.fs")>]
    let ``CallToVirtualMember03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
