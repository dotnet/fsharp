// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module InterfaceTypes =

    // Error tests

    [<Theory; FileInlineData("E_InheritInterface.fs")>]
    let ``E_InheritInterface_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1207

    [<Theory; FileInlineData("E_InterfaceNotFullyImpl01.fs")>]
    let ``E_InterfaceNotFullyImpl01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 366

    [<Theory; FileInlineData("E_InterfaceNotFullyImpl02.fs")>]
    let ``E_InterfaceNotFullyImpl02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 855

    [<Theory; FileInlineData("E_InterfaceNotFullyImpl03.fs")>]
    let ``E_InterfaceNotFullyImpl03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 366

    [<Theory; FileInlineData("E_MultipleInterfaceInheritance.fs")>]
    let ``E_MultipleInterfaceInheritance_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 39

    [<Theory; FileInlineData("E_AnonymousTypeInInterface01.fs")>]
    let ``E_AnonymousTypeInInterface01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 715

    // Lang version 4.7 error tests - skipped: test framework doesn't correctly apply older langversions
    [<Theory(Skip = "Test framework doesn't correctly apply langversion:4.7")>]
    [<FileInlineData("E_MultipleInst01.4.7.fs")>]
    let ``E_MultipleInst01_4_7_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--langversion:4.7"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3350

    [<Theory(Skip = "Test framework doesn't correctly apply langversion:4.7")>]
    [<FileInlineData("E_MultipleInst04.4.7.fs")>]
    let ``E_MultipleInst04_4_7_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--langversion:4.7"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3350

    [<Theory(Skip = "Test framework doesn't correctly apply langversion:4.7")>]
    [<FileInlineData("E_MultipleInst07.4.7.fs")>]
    let ``E_MultipleInst07_4_7_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--langversion:4.7"; "--nowarn:221"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3350

    [<Theory; FileInlineData("E_MultipleInst07.5.0.fs")>]
    let ``E_MultipleInst07_5_0_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--langversion:5.0"; "--nowarn:221"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3360

    [<Theory(Skip = "Test framework doesn't correctly apply langversion:4.7")>]
    [<FileInlineData("E_ImplementGenIFaceTwice01_4.7.fs")>]
    let ``E_ImplementGenIFaceTwice01_4_7_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--langversion:4.7"; "--nowarn:221"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3350

    [<Theory; FileInlineData("E_ImplementGenIFaceTwice01_5.0.fs")>]
    let ``E_ImplementGenIFaceTwice01_5_0_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--langversion:5.0"; "--nowarn:221"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3360

    [<Theory(Skip = "Test framework doesn't correctly apply langversion:4.7")>]
    [<FileInlineData("E_ImplementGenIFaceTwice02_4.7.fs")>]
    let ``E_ImplementGenIFaceTwice02_4_7_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--langversion:4.7"; "--nowarn:221"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3350

    // Success tests

    [<Theory; FileInlineData("ObjImplementsInterfaceGenWithConstraint.fs")>]
    let ``ObjImplementsInterfaceGenWithConstraint_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("InterfaceMember_NameCollisions.fs")>]
    let ``InterfaceMember_NameCollisions_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("MultipleInst01.5.0.fs")>]
    let ``MultipleInst01_5_0_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--langversion:5.0"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("MultipleInst02.fs")>]
    let ``MultipleInst02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 362

    [<Theory; FileInlineData("MultipleInst03.fs")>]
    let ``MultipleInst03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCodes [362; 855; 888]

    [<Theory; FileInlineData("MultipleInst05.fs")>]
    let ``MultipleInst05_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 362

    [<Theory; FileInlineData("MultipleInst06.fs")>]
    let ``MultipleInst06_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCodes [362; 855; 888]

    [<Theory; FileInlineData("MultipleInst04.5.0.fs")>]
    let ``MultipleInst04_5_0_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--langversion:5.0"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("Inheritance_OverrideInterface.fs")>]
    let ``Inheritance_OverrideInterface_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--langversion:5.0"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("InheritFromIComparable01.fs")>]
    let ``InheritFromIComparable01_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("InheritedInterface.fs")>]
    let ``InheritedInterface_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ObjImplementsInterface.fs")>]
    let ``ObjImplementsInterface_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("interface001.fs")>]
    let ``interface001_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("interface002.fs")>]
    let ``interface002_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("interface001e.fs")>]
    let ``interface001e_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 887

    [<Theory; FileInlineData("interface002e.fs")>]
    let ``interface002e_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 954

    [<Theory; FileInlineData("interface003.fs")>]
    let ``interface003_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ImplementGenIFaceTwice02_5.0.fs")>]
    let ``ImplementGenIFaceTwice02_5_0_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--langversion:5.0"; "--nowarn:221"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("EmptyInterface01.fs")>]
    let ``EmptyInterface01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("InheritDotNetInterface.fs")>]
    let ``InheritDotNetInterface_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
