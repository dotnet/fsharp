// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TypeExtensionsBasic =

    // Error tests

    [<Theory; FileInlineData("E_ProtectedMemberInExtensionMember01.fs")>]
    let ``E_ProtectedMemberInExtensionMember01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 491

    [<Theory; FileInlineData("E_CantExtendTypeAbbrev.fs")>]
    let ``E_CantExtendTypeAbbrev_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 964

    [<Theory; FileInlineData("E_ConflictingMembers.fs")>]
    let ``E_ConflictingMembers_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 438

    [<Theory; FileInlineData("E_InvalidExtensions01.fs")>]
    let ``E_InvalidExtensions01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 912

    [<Theory; FileInlineData("E_InvalidExtensions02.fs")>]
    let ``E_InvalidExtensions02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 912

    [<Theory; FileInlineData("E_InvalidExtensions03.fs")>]
    let ``E_InvalidExtensions03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 925

    [<Theory; FileInlineData("E_InvalidExtensions04.fs")>]
    let ``E_InvalidExtensions04_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 39

    [<Theory; FileInlineData("E_ExtensionInNamespace01.fs")>]
    let ``E_ExtensionInNamespace01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 644

    [<Theory; FileInlineData("E_ExtendVirtualMethods01.fs")>]
    let ``E_ExtendVirtualMethods01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 854

    [<Theory; FileInlineData("E_InvalidForwardRef01.fs")>]
    let ``E_InvalidForwardRef01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 430

    [<Theory; FileInlineData("E_ExtensionOperator01.fs")>]
    let ``E_ExtensionOperator01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"; "--warnaserror+"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1215

    // Success tests

    [<Theory; FileInlineData("BasicExtensions.fs")>]
    let ``BasicExtensions_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("MultipleExtensions.fs")>]
    let ``MultipleExtensions_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("UnqualifiedName.fs")>]
    let ``UnqualifiedName_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // Tests temporarily skipped due to 'allows ref struct' constraint mismatch on IEnumerable
    // [<Theory; FileInlineData("ExtendHierarchy01.fs")>]
    // let ``ExtendHierarchy01_fs`` compilation =
    //     compilation
    //     |> getCompilation
    //     |> asExe
    //     |> withLangVersionPreview
    //     |> ignoreWarnings
    //     |> compile
    //     |> shouldSucceed

    [<Theory; FileInlineData("ExtendHierarchy02.fs")>]
    let ``ExtendHierarchy02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ExtensionInNamespace02.fs")>]
    let ``ExtensionInNamespace02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ExtendWithOperator01.fs")>]
    let ``ExtendWithOperator01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("NonConflictingIntrinsicMembers.fs")>]
    let ``NonConflictingIntrinsicMembers_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ExtendViaOverloading01.fs")>]
    let ``ExtendViaOverloading01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // Tests temporarily skipped due to 'allows ref struct' constraint mismatch on IEnumerable
    // [<Theory; FileInlineData("ExtendViaOverloading02.fs")>]
    // let ``ExtendViaOverloading02_fs`` compilation =
    //     compilation
    //     |> getCompilation
    //     |> asExe
    //     |> withLangVersionPreview
    //     |> ignoreWarnings
    //     |> compile
    //     |> shouldSucceed

    [<Theory; FileInlineData("fslib.fs")>]
    let ``fslib_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
