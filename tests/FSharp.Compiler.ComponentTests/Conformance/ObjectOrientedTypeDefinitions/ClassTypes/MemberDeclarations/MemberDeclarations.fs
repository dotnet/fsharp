// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module MemberDeclarations =

    // Error tests

    [<Theory; FileInlineData("E_byref_two_arguments_curried.fsx")>]
    let ``E_byref_two_arguments_curried_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 440

    [<Theory; FileInlineData("E_optional_two_arguments_curried.fsx")>]
    let ``E_optional_two_arguments_curried_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 440

    [<Theory; FileInlineData("E_out_two_arguments_curried.fsx")>]
    let ``E_out_two_arguments_curried_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 440

    [<Theory; FileInlineData("E_paramarray_two_arguments_curried.fsx")>]
    let ``E_paramarray_two_arguments_curried_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 440

    [<Theory; FileInlineData("E_ClashingInstanceStaticProperties.fs")>]
    let ``E_ClashingInstanceStaticProperties_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 441

    [<Theory; FileInlineData("E_PropertySetterUnit01.fs")>]
    let ``E_PropertySetterUnit01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3172

    [<Theory; FileInlineData("E_PropertyInvalidGetter01.fs")>]
    let ``E_PropertyInvalidGetter01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 557

    [<Theory; FileInlineData("E_PropertyInvalidGetter02.fs")>]
    let ``E_PropertyInvalidGetter02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 555

    [<Theory; FileInlineData("E_PropertyInvalidGetterSetter01.fs")>]
    let ``E_PropertyInvalidGetterSetter01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3172

    [<Theory; FileInlineData("E_ImplementMemberNoExist.fs")>]
    let ``E_ImplementMemberNoExist_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 35

    [<Theory; FileInlineData("E_PropertySameNameDiffArity.fs")>]
    let ``E_PropertySameNameDiffArity_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 436

    [<Theory; FileInlineData("E_GeneratedPropertyNameClash01.fs")>]
    let ``E_GeneratedPropertyNameClash01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 23

    [<Theory; FileInlineData("E_CtorAndCCtor01.fs")>]
    let ``E_CtorAndCCtor01_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3066

    [<Theory; FileInlineData("E_CtorAndCCtor02.fs")>]
    let ``E_CtorAndCCtor02_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3066

    // Success tests

    [<Theory; FileInlineData("byref_one_argument.fsx")>]
    let ``byref_one_argument_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("byref_two_arguments_non_curried.fsx")>]
    let ``byref_two_arguments_non_curried_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("optional_one_argument.fsx")>]
    let ``optional_one_argument_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("optional_two_arguments_non_curried.fsx")>]
    let ``optional_two_arguments_non_curried_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("out_one_argument.fsx")>]
    let ``out_one_argument_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("out_two_arguments_non_curried.fsx")>]
    let ``out_two_arguments_non_curried_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("paramarray_one_argument.fsx")>]
    let ``paramarray_one_argument_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("paramarray_two_arguments_non_curried.fsx")>]
    let ``paramarray_two_arguments_non_curried_fsx`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("NoClashMemberIFaceMember.fs")>]
    let ``NoClashMemberIFaceMember_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("InlineProperties01.fs")>]
    let ``InlineProperties01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("CtorAndCCtor01.fs")>]
    let ``CtorAndCCtor01_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("CtorAndCCtor02.fs")>]
    let ``CtorAndCCtor02_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("QuestionOperatorAsMember01.fs")>]
    let ``QuestionOperatorAsMember01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
