// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.ObjectOrientedTypeDefinitions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module StructTypes =

    // Error tests

    [<Theory; FileInlineData("E_Overload_Equals.fs")>]
    let ``E_Overload_Equals_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 438

    [<Theory; FileInlineData("E_Overload_GetHashCode.fs")>]
    let ``E_Overload_GetHashCode_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 438

    [<Theory; FileInlineData("E_Regressions02.fs")>]
    let ``E_Regressions02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCodes [37; 881]

    [<Theory; FileInlineData("E_Regressions02b.fs")>]
    let ``E_Regressions02b_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 881

    [<Theory; FileInlineData("E_ImplicitCTorUse01.fs")>]
    let ``E_ImplicitCTorUse01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 688

    [<Theory; FileInlineData("E_ImplicitCTorUse02.fs")>]
    let ``E_ImplicitCTorUse02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 688

    [<Theory; FileInlineData("E_StructWithNoFields01.fs")>]
    let ``E_StructWithNoFields01_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 81

    [<Theory; FileInlineData("E_NoAbstractMembers.fs")>]
    let ``E_NoAbstractMembers_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 947

    [<Theory; FileInlineData("E_NoLetBindings.fs")>]
    let ``E_NoLetBindings_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 901

    [<Theory; FileInlineData("E_StructConstruction03.fs")>]
    let ``E_StructConstruction03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 954

    [<Theory; FileInlineData("E_NoDefaultCtor.fs")>]
    let ``E_NoDefaultCtor_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 81

    [<Theory; FileInlineData("E_CyclicInheritance01.fs")>]
    let ``E_CyclicInheritance01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 954

    [<Theory; FileInlineData("E_StructInheritance01.fs")>]
    let ``E_StructInheritance01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 945

    [<Theory; FileInlineData("E_StructInheritance01b.fs")>]
    let ``E_StructInheritance01b_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 945

    [<Theory; FileInlineData("E_AbstractClassStruct.fs")>]
    let ``E_AbstractClassStruct_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 926

    [<Theory; FileInlineData("E_Nullness01.fs")>]
    let ``E_Nullness01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 688

    [<Theory; FileInlineData("E_Nullness02.fs")>]
    let ``E_Nullness02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 688

    [<Theory; FileInlineData("E_InvalidRecursiveGeneric01.fs")>]
    let ``E_InvalidRecursiveGeneric01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 954

    [<Theory; FileInlineData("E_InvalidRecursiveGeneric02.fs")>]
    let ``E_InvalidRecursiveGeneric02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 954

    // Success tests

    [<Theory; FileInlineData("EqualAndBoxing01.fs")>]
    let ``EqualAndBoxing01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("Regressions01.fs")>]
    let ``Regressions01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("Regressions02.fs")>]
    let ``Regressions02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
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

    [<Theory; FileInlineData("Overload_Equals.fs")>]
    let ``Overload_Equals_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("Overload_GetHashCode.fs")>]
    let ``Overload_GetHashCode_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("Overload_ToString.fs")>]
    let ``Overload_ToString_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("StructWithNoFields01.fs")>]
    let ``StructWithNoFields01_fs`` compilation =
        compilation
        |> getCompilation
        |> asLibrary
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("MutableFields.fs")>]
    let ``MutableFields_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("DoStaticLetDo.fs")>]
    let ``DoStaticLetDo_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("ExplicitCtor.fs")>]
    let ``ExplicitCtor_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("IndexerProperties01.fs")>]
    let ``IndexerProperties01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("StructConstruction01.fs")>]
    let ``StructConstruction01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("StructConstruction02.fs")>]
    let ``StructConstruction02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("StructInstantiation01.fs")>]
    let ``StructInstantiation01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("StructAttribute01.fs")>]
    let ``StructAttribute01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    [<Theory; FileInlineData("StructAttribute02.fs")>]
    let ``StructAttribute02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 927

    [<Theory; FileInlineData("GenericStruct01.fs")>]
    let ``GenericStruct01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
