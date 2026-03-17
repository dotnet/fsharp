// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module InheritsDeclarations =

    // Error tests

    [<Theory; FileInlineData("E_DefaultNoAbstractInBaseClass.fs")>]
    let ``E_DefaultNoAbstractInBaseClass_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 859

    [<Theory; FileInlineData("E_MemberNoAbstractInBaseClass.fs")>]
    let ``E_MemberNoAbstractInBaseClass_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 859

    [<Theory; FileInlineData("E_InheritInterface01.fs")>]
    let ``E_InheritInterface01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 946

    [<Theory; FileInlineData("E_InheritFromGenericType01.fs")>]
    let ``E_InheritFromGenericType01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 753

    // Success tests

    [<Theory; FileInlineData("BaseValue01.fs")>]
    let ``BaseValue01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
