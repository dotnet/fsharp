// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions.ClassTypes

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Misc =

    // Error tests

    [<Theory; FileInlineData("E_CyclicInheritance.fs")>]
    let ``E_CyclicInheritance_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 954

    [<Theory; FileInlineData("E_AbstractClass01.fs")>]
    let ``E_AbstractClass01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCodes [54; 365; 54; 365]

    [<Theory; FileInlineData("E_AbstractClass02.fs")>]
    let ``E_AbstractClass02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 759

    [<Theory; FileInlineData("E_AbstractClass03.fs")>]
    let ``E_AbstractClass03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 759

    [<Theory; FileInlineData("E_SealedClass01.fs")>]
    let ``E_SealedClass01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 945

    [<Theory; FileInlineData("E_NoNestedTypes.fs")>]
    let ``E_NoNestedTypes_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCodes [547; 10]

    [<Theory; FileInlineData("E_ExplicitConstructor.fs")>]
    let ``E_ExplicitConstructor_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCodes [787; 1; 696]

    [<Theory; FileInlineData("E_AbstractClassAttribute01.fs")>]
    let ``E_AbstractClassAttribute01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1

    [<Theory; FileInlineData("E_RestrictedSuperTypes.fs")>]
    let ``E_RestrictedSuperTypes_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 771

    [<Theory; FileInlineData("E_ZeroArity.fs")>]
    let ``E_ZeroArity_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 10

    // Success tests

    [<Theory; FileInlineData("GenericClass01.fs")>]
    let ``GenericClass01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("W_SealedClass03.fs")>]
    let ``W_SealedClass03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("AbstractClassAttribute01.fs")>]
    let ``AbstractClassAttribute01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("AbstractClassAttribute02.fs")>]
    let ``AbstractClassAttribute02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withErrorCodes [939; 939]

    [<Theory; FileInlineData("Decondensation.fs")>]
    let ``Decondensation_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ValueRestrictionCtor.fs")>]
    let ``ValueRestrictionCtor_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("AbstractClassMultipleConstructors01.fs")>]
    let ``AbstractClassMultipleConstructors01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed
