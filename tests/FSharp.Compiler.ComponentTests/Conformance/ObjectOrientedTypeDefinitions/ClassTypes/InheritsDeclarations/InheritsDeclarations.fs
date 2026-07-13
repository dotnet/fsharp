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

    [<Fact>]
    let ``Inherit from undefined non-generic type reports single FS0039`` () =
        FSharp "type MyClass() =\n    inherit NonExistentBase()\n"
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 2, Col 13, Line 2, Col 28, "The type 'NonExistentBase' is not defined.")
        ]

    [<Fact>]
    let ``Inherit from undefined generic type reports single FS0039`` () =
        FSharp "type MyClass() =\n    inherit MissingGeneric<int>()\n"
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 2, Col 13, Line 2, Col 27, "The type 'MissingGeneric' is not defined.")
        ]

    [<Fact>]
    let ``Interface inheriting undefined interface reports single FS0039`` () =
        FSharp "type IMyInterface =\n    inherit INonExistent\n"
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 2, Col 13, Line 2, Col 25, "The type 'INonExistent' is not defined.")
            (Error 887, Line 2, Col 5, Line 2, Col 25, "The type 'obj' is not an interface type")
        ]

    [<Fact>]
    let ``Undefined inherit and undefined value report independently`` () =
        FSharp """
type MyClass() =
    inherit NonExistentBase()
    member _.X = undefinedValue
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 3, Col 13, Line 3, Col 28, "The type 'NonExistentBase' is not defined.")
            (Error 39, Line 4, Col 18, Line 4, Col 32, "The value or constructor 'undefinedValue' is not defined.")
        ]

    [<Fact>]
    let ``Valid inherit produces no diagnostics`` () =
        FSharp """
open System
type MyClass() =
    inherit Exception("test")
"""
        |> typecheck
        |> shouldSucceed
