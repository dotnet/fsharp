// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions.ClassTypes

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AutoProperties =

    // Error tests - should fail with expected error codes

    [<Theory; FileInlineData("E_OnlyGetter01.fs")>]
    let ``E_OnlyGetter01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 810

    [<Theory; FileInlineData("E_OnlySetter01.fs")>]
    let ``E_OnlySetter01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3135

    [<Theory; FileInlineData("E_BadType01.fs")>]
    let ``E_BadType01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 39

    [<Theory; FileInlineData("E_IllegalName01.fs")>]
    let ``E_IllegalName01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 10

    [<Theory; FileInlineData("E_PrivateProperty01.fs")>]
    let ``E_PrivateProperty01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 410

    [<Theory; FileInlineData("E_PrivateProperty02.fs")>]
    let ``E_PrivateProperty02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 491

    [<Theory; FileInlineData("E_PrivateProperty03.fs")>]
    let ``E_PrivateProperty03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 491

    // Note: E_PrivateProperty04.fs and E_InternalProperty01.fs require pre-compiled Library.dll
    // Skipping these for now as they need PRECMD

    // Success tests - should compile successfully

    [<Theory; FileInlineData("AliasedType01.fs")>]
    let ``AliasedType01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("Array01.fs")>]
    let ``Array01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("Attributes01.fs")>]
    let ``Attributes01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("List01.fs")>]
    let ``List01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ReferenceType01.fs")>]
    let ``ReferenceType01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("Tuple01.fs")>]
    let ``Tuple01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("UnitsOfMeasure01.fs")>]
    let ``UnitsOfMeasure01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ValueType01.fs")>]
    let ``ValueType01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ExtensionProperty01.fs")>]
    let ``ExtensionProperty01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("Do01.fs")>]
    let ``Do01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("Then01.fs")>]
    let ``Then01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("InitializeInConstructor01.fs")>]
    let ``InitializeInConstructor01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("PublicProperty01.fs")>]
    let ``PublicProperty01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // Note: PublicProperty02.fs and CrossLangCSharp01.fs require pre-compiled dependencies
    // Skipping these for now as they need PRECMD
