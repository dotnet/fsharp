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

    // Regression tests for https://github.com/dotnet/fsharp/issues/16432:
    // inheriting from an unknown type should emit FS0039 only once,
    // not three times (once per name-resolution site in CheckDeclarations).

    let private assertSingleFS39 source =
        let result =
            FSharp source
            |> typecheck

        let fs39 =
            result.Output.Diagnostics
            |> List.filter (fun d -> d.Error = (Error 39))

        Assert.True(
            fs39.Length = 1,
            sprintf "Expected exactly 1 FS0039 but got %d. Diagnostics:\n%A" fs39.Length result.Output.Diagnostics
        )

    [<Theory>]
    [<InlineData("type MyClass() =\n    inherit NonExistentBase()\n")>]
    [<InlineData("type MyClass() =\n    inherit MissingGeneric<int>()\n")>]
    [<InlineData("type IMyInterface =\n    inherit INonExistent\n")>]
    let ``Inherit from undefined type reports single FS0039`` source =
        assertSingleFS39 source

    [<Fact>]
    let ``Two different undefined names still report separately`` () =
        let result =
            FSharp """
type MyClass() =
    inherit NonExistentBase()
    member _.X = undefinedValue
"""
            |> typecheck

        let fs39 =
            result.Output.Diagnostics
            |> List.filter (fun d -> d.Error = (Error 39))

        Assert.True(
            fs39.Length >= 2,
            sprintf "Expected >= 2 FS0039, got %d. Diagnostics:\n%A" fs39.Length result.Output.Diagnostics
        )

    [<Fact>]
    let ``Valid inherit produces no FS0039`` () =
        FSharp """
open System
type MyClass() =
    inherit Exception("test")
"""
        |> typecheck
        |> shouldSucceed
