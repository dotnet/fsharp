// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module MemberDeclarations =

    [<Fact>]
    let ``Inline member with class-scope self identifier should compile`` () =
        FSharp """
module Test

type TestClass1() as SomeSelfIdentifier =
    member inline AnotherSelfIdentifier.test() = 5

type TestClass2() as self =
    member inline self.test() = 5

type TestClass3() as self =
    member inline _.test() = 5
"""
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Inline member referencing private function fails FS1113 at definition`` () =
        FSharp """
module Test

module private PrivHelpers =
    let secret x = x + 1

type Bag() =
    member inline _.Wrap(x) = PrivHelpers.secret x

type SelfBag() as self =
    member inline _.Wrap(x) = PrivHelpers.secret x
"""
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withErrorCode 1113

    [<Fact>]
    let ``Inline member referencing internal function fails FS1113 at definition`` () =
        FSharp """
module Test

module internal IntHelpers =
    let secret x = x + 1

type Bag() =
    member inline _.Wrap(x) = IntHelpers.secret x
"""
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withErrorCode 1113

    [<Fact>]
    let ``Inline member with class-scope self identifier referencing only public values compiles`` () =
        FSharp """
module Test

let publicHelper x = x + 1

type Bag() as self =
    member inline _.Wrap(x) = publicHelper x
"""
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Inline member referencing a public union case compiles`` () =
        FSharp """
module Test

type Bag() =
    member inline _.Wrap(x) = Some x
"""
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // A referenced value that is not inline must still fail with FS1113; only inline values,
    // which expand transitively when the outer member is inlined, are excluded.
    [<Fact>]
    let ``Inline member referencing an internal non-inline member fails FS1113`` () =
        FSharp """
module Test

type Bag() =
    member internal _.Helper(x) = x + 1
    member inline this.Wrap(x) = this.Helper(x)
"""
        |> asLibrary
        |> withNoOptimize
        |> ignoreWarnings
        |> compile
        |> shouldFail
        |> withErrorCode 1113

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
