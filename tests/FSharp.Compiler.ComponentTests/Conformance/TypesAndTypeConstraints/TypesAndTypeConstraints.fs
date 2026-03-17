// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Migrated from: tests/fsharpqa/Source/Conformance/TypesAndTypeConstraints
// Test count: 80 (55 CheckingSyntacticTypes + 23 LogicalPropertiesOfTypes + 2 TypeConstraints + 8 TypeParameterDefinitions)
// Note: FSIMODE=PIPE tests (E_ByRef01.fsx, etc.) are skipped - FSI pipe mode not supported in ComponentTests

namespace Conformance.TypesAndTypeConstraints

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module CheckingSyntacticTypes =

    let private resourcePath =
        __SOURCE_DIRECTORY__
        + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes"

    // SOURCE=E_ByRef01.fs SCFLAGS="-a --test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_ByRef01.fs" |])>]
    let ``E_ByRef01_fs`` compilation =
        compilation
        |> asLibrary
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3136
        |> withDiagnosticMessageMatches "byref types"
        |> ignore

    // SOURCE=E_ByRef02.fs SCFLAGS="-a --test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_ByRef02.fs" |])>]
    let ``E_ByRef02_fs`` compilation =
        compilation
        |> asLibrary
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3136
        |> withDiagnosticMessageMatches "byref types"
        |> ignore

    // SOURCE=E_ByRef03.fs SCFLAGS="-a --test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_ByRef03.fs" |])>]
    let ``E_ByRef03_fs`` compilation =
        compilation
        |> asLibrary
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3136
        |> withDiagnosticMessageMatches "byref types"
        |> ignore

    // SOURCE=ByRef04.fs SCFLAGS="-a --test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "ByRef04.fs" |])>]
    let ``ByRef04_fs`` compilation =
        compilation
        |> asLibrary
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE=Regressions01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "Regressions01.fs" |])>]
    let ``Regressions01_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=E_Regressions01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_Regressions01.fs" |])>]
    let ``E_Regressions01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 564
        |> withDiagnosticMessageMatches "inherit.*as.*bindings"
        |> ignore

    // SOURCE=E_Regression02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_Regression02.fs" |])>]
    let ``E_Regression02_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCodes [ 1; 39 ]
        |> ignore

    // SOURCE=DefaultConstructorConstraint01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "DefaultConstructorConstraint01.fs" |])>]
    let ``DefaultConstructorConstraint01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 700
        |> withDiagnosticMessageMatches "'new'.+constraint"
        |> ignore

    // SOURCE=DefaultConstructorConstraint02.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "DefaultConstructorConstraint02.fs" |])>]
    let ``DefaultConstructorConstraint02_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0071
        |> withDiagnosticMessageMatches "default.+constructor"
        |> ignore

    // SOURCE=DefaultConstructorConstraint03.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "DefaultConstructorConstraint03.fs" |])>]
    let ``DefaultConstructorConstraint03_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 700
        |> withDiagnosticMessageMatches "'new'.+constrain"
        |> ignore

    // SOURCE=DefaultConstructorConstraint04.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "DefaultConstructorConstraint04.fs" |])>]
    let ``DefaultConstructorConstraint04_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 700
        |> withDiagnosticMessageMatches "'new'.+constraint"
        |> ignore

    // SOURCE=DefaultConstructorConstraint05.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "DefaultConstructorConstraint05.fs" |])>]
    let ``DefaultConstructorConstraint05_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 700
        |> withDiagnosticMessageMatches "'new'.+constraint"
        |> ignore

    // SOURCE=E_EnumConstraint01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_EnumConstraint01.fs" |])>]
    let ``E_EnumConstraint01_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "byte.*int16"
        |> ignore

    // SOURCE=E_EnumConstraint02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_EnumConstraint02.fs" |])>]
    let ``E_EnumConstraint02_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "not a CLI enum type"
        |> ignore

    // SOURCE=E_StructConstraint01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_StructConstraint01.fs" |])>]
    let ``E_StructConstraint01_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "struct"
        |> ignore

    // SOURCE=E_RefConstraint01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_RefConstraint01.fs" |])>]
    let ``E_RefConstraint01_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "reference semantics"
        |> ignore

    // SOURCE=E_SubtypeConstraint01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_SubtypeConstraint01.fs" |])>]
    let ``E_SubtypeConstraint01_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> ignore

    // SOURCE=E_DelegateConstraint01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_DelegateConstraint01.fs" |])>]
    let ``E_DelegateConstraint01_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "non-standard delegate"
        |> ignore

    // SOURCE=NullnessConstraint01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "NullnessConstraint01.fs" |])>]
    let ``NullnessConstraint01_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=W_LessGenericThanAnnotated01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "W_LessGenericThanAnnotated01.fs" |])>]
    let ``W_LessGenericThanAnnotated01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withWarningCode 64
        |> ignore

    // SOURCE=E_NullnessConstraint01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_NullnessConstraint01.fs" |])>]
    let ``E_NullnessConstraint01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "null.*proper value"
        |> ignore

    // SOURCE=E_MemberConstraint01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_MemberConstraint01.fs" |])>]
    let ``E_MemberConstraint01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 71
        |> withDiagnosticMessageMatches "someFunc"
        |> ignore

    // SOURCE=E_MemberConstraint02.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_MemberConstraint02.fs" |])>]
    let ``E_MemberConstraint02_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0193
        |> withDiagnosticMessageMatches "not static"
        |> ignore

    // SOURCE=E_MemberConstraint03.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_MemberConstraint03.fs" |])>]
    let ``E_MemberConstraint03_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 735
        |> withDiagnosticMessageMatches "Expected 1 expressions"
        |> ignore

    // SOURCE=E_MemberConstraint04.fs SCFLAGS="--test:ErrorRanges --flaterrors"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_MemberConstraint04.fs" |])>]
    let ``E_MemberConstraint04_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges"; "--flaterrors" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0193
        |> ignore

    // SOURCE=MemberConstraint01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "MemberConstraint01.fs" |])>]
    let ``MemberConstraint01_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=UnmanagedConstraint01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "UnmanagedConstraint01.fs" |])>]
    let ``UnmanagedConstraint01_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=E_UnmanagedConstraint01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_UnmanagedConstraint01.fs" |])>]
    let ``E_UnmanagedConstraint01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> ignore

    // SOURCE=E_EqualityConstraint01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_EqualityConstraint01.fs" |])>]
    let ``E_EqualityConstraint01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0193
        |> withDiagnosticMessageMatches "equality"
        |> ignore

    // SOURCE=E_ComparisonConstraint01.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_ComparisonConstraint01.fs" |])>]
    let ``E_ComparisonConstraint01_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0193
        |> withDiagnosticMessageMatches "comparison"
        |> ignore

    // SOURCE=ComparisonConstraint01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "ComparisonConstraint01.fs" |])>]
    let ``ComparisonConstraint01_fs`` compilation =
        compilation |> asExe |> ignoreWarnings |> compile |> shouldSucceed |> ignore

    // x SOURCE=NativePtrArrayElementUsage.fs SCFLAGS="--test:ErrorRanges" COMPILE_ONLY=1 PEVER=/MD
    // Disabled with 'x' prefix in env.lst
    [<Fact(Skip = "Disabled in original env.lst with 'x' prefix")>]
    let ``NativePtrArrayElementUsage_fs`` () = ()

    // SOURCE=ExplicitMemberConstraints1.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "ExplicitMemberConstraints1.fs" |])>]
    let ``ExplicitMemberConstraints1_fs`` compilation =
        compilation |> asExe |> typecheck |> shouldSucceed |> ignore

    // SOURCE=ExplicitMemberConstraints2.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "ExplicitMemberConstraints2.fs" |])>]
    let ``ExplicitMemberConstraints2_fs`` compilation =
        compilation |> asExe |> typecheck |> shouldSucceed |> ignore

    // SOURCE=ConstraintCall1.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "ConstraintCall1.fs" |])>]
    let ``ConstraintCall1_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=ConstraintCall2.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "ConstraintCall2.fs" |])>]
    let ``ConstraintCall2_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=E_ExplicitMemberConstraints1.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_ExplicitMemberConstraints1.fs" |])>]
    let ``E_ExplicitMemberConstraints1_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "does not support the operator 'get_M'"
        |> ignore

    // SOURCE=E_ExplicitMemberConstraints2.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_ExplicitMemberConstraints2.fs" |])>]
    let ``E_ExplicitMemberConstraints2_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "support the operator 'get_M'"
        |> ignore

    // SOURCE=E_ConstraintCall1.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_ConstraintCall1.fs" |])>]
    let ``E_ConstraintCall1_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> ignore

    // SOURCE=E_ConstraintCall2.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_ConstraintCall2.fs" |])>]
    let ``E_ConstraintCall2_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> ignore

    // SOURCE=StructConstraint01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "StructConstraint01.fs" |])>]
    let ``StructConstraint01_fs`` compilation =
        compilation |> asExe |> typecheck |> shouldSucceed |> ignore

    // SOURCE=StructConstraint02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "StructConstraint02.fs" |])>]
    let ``StructConstraint02_fs`` compilation =
        compilation |> asExe |> typecheck |> shouldSucceed |> ignore

    // SOURCE=E_CannotInlineVirtualMethods1.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes = [| "E_CannotInlineVirtualMethods1.fs" |])>]
    let ``E_CannotInlineVirtualMethods1_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 3151
        |> withDiagnosticMessageMatches "may not be declared 'inline'"
        |> ignore

    // FSIMODE=PIPE tests - skipped as FSI pipe mode not supported
    [<Fact(Skip = "FSIMODE=PIPE not supported in ComponentTests")>]
    let ``E_ByRef01_fsx`` () = ()

    [<Fact(Skip = "FSIMODE=PIPE not supported in ComponentTests")>]
    let ``E_ByRef02_fsx`` () = ()

    [<Fact(Skip = "FSIMODE=PIPE not supported in ComponentTests")>]
    let ``E_ByRef03_fsx`` () = ()

    [<Fact(Skip = "FSIMODE=PIPE not supported in ComponentTests")>]
    let ``ByRef04_fsx`` () = ()


module LogicalPropertiesOfTypes =

    let private resourcePath =
        __SOURCE_DIRECTORY__
        + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes"

    // SOURCE=TypeWithNullAsAbnormalValue.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "TypeWithNullAsAbnormalValue.fsx" |])>]
    let ``TypeWithNullAsAbnormalValue_fsx`` compilation =
        compilation |> asFsx |> compile |> shouldSucceed |> ignore

    // SOURCE=E_TypeWithNullAsAbnormalValue.fsx SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "E_TypeWithNullAsAbnormalValue.fsx" |])>]
    let ``E_TypeWithNullAsAbnormalValue_fsx`` compilation =
        compilation
        |> asFsx
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 43
        |> withDiagnosticMessageMatches "does not have 'null' as a proper value"
        |> ignore

    // SOURCE=TypeWithNullAsRepresentationValue.fsx
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "TypeWithNullAsRepresentationValue.fsx" |])>]
    let ``TypeWithNullAsRepresentationValue_fsx`` compilation =
        compilation |> asFsx |> compile |> shouldSucceed |> ignore

    // SOURCE=E_TypeWithNullAsRepresentationValue.fsx SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "E_TypeWithNullAsRepresentationValue.fsx" |])>]
    let ``E_TypeWithNullAsRepresentationValue_fsx`` compilation =
        compilation
        |> asFsx
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 43
        |> withDiagnosticMessageMatches "does not have 'null' as a proper value"
        |> ignore

    // SOURCE=E_TypeWithNullLiteral_NetVal.fsx SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "E_TypeWithNullLiteral_NetVal.fsx" |])>]
    let ``E_TypeWithNullLiteral_NetVal_fsx`` compilation =
        compilation
        |> asFsx
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 43
        |> withDiagnosticMessageMatches "does not have 'null' as a proper value"
        |> ignore

    // SOURCE=GenericTypeDef.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "GenericTypeDef.fs" |])>]
    let ``GenericTypeDef_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=BaseTypes_Abstract.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "BaseTypes_Abstract.fs" |])>]
    let ``BaseTypes_Abstract_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=BaseTypes_Array.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "BaseTypes_Array.fs" |])>]
    let ``BaseTypes_Array_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=BaseTypes_Class.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "BaseTypes_Class.fs" |])>]
    let ``BaseTypes_Class_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=BaseTypes_Delegate.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "BaseTypes_Delegate.fs" |])>]
    let ``BaseTypes_Delegate_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=BaseTypes_DerivedClass.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "BaseTypes_DerivedClass.fs" |])>]
    let ``BaseTypes_DerivedClass_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=BaseTypes_DiscriminatedUnion.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "BaseTypes_DiscriminatedUnion.fs" |])>]
    let ``BaseTypes_DiscriminatedUnion_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=BaseTypes_Exception.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "BaseTypes_Exception.fs" |])>]
    let ``BaseTypes_Exception_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=BaseTypes_Interface.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "BaseTypes_Interface.fs" |])>]
    let ``BaseTypes_Interface_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=BaseTypes_Record.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "BaseTypes_Record.fs" |])>]
    let ``BaseTypes_Record_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=BaseTypes_Struct.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "BaseTypes_Struct.fs" |])>]
    let ``BaseTypes_Struct_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=BaseTypes_Variable.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "BaseTypes_Variable.fs" |])>]
    let ``BaseTypes_Variable_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // NoMT SOURCE=BaseTypes_Tuple1.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "BaseTypes_Tuple1.fs" |])>]
    let ``BaseTypes_Tuple1_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=FSharpType_IsRecord.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "FSharpType_IsRecord.fs" |])>]
    let ``FSharpType_IsRecord_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=AllowNullLiteral01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "AllowNullLiteral01.fs" |])>]
    let ``AllowNullLiteral01_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=E_AllowNullLiteral01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "E_AllowNullLiteral01.fs" |])>]
    let ``E_AllowNullLiteral01_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 43
        |> withDiagnosticMessageMatches "does not have 'null' as a proper value"
        |> ignore

    // SOURCE=E_AllowNullLiteral02.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "E_AllowNullLiteral02.fs" |])>]
    let ``E_AllowNullLiteral02_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 934
        |> withDiagnosticMessageMatches "AllowNullLiteral"
        |> ignore

    // SOURCE=SubtypeCoercion01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/LogicalPropertiesOfTypes", Includes = [| "SubtypeCoercion01.fs" |])>]
    let ``SubtypeCoercion01_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore


module TypeConstraints =

    let private resourcePath =
        __SOURCE_DIRECTORY__
        + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeConstraints"

    // SOURCE=E_ConstructorConstraint01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeConstraints", Includes = [| "E_ConstructorConstraint01.fs" |])>]
    let ``E_ConstructorConstraint01_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 3066
        |> withDiagnosticMessageMatches "Invalid member name"
        |> ignore

    // SOURCE=Constraints01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeConstraints", Includes = [| "Constraints01.fs" |])>]
    let ``Constraints01_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore


module TypeParameterDefinitions =

    let private resourcePath =
        __SOURCE_DIRECTORY__
        + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions"

    // SOURCE=BasicTypeParam01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes = [| "BasicTypeParam01.fs" |])>]
    let ``BasicTypeParam01_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=HashConstraint01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes = [| "HashConstraint01.fs" |])>]
    let ``HashConstraint01_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=HashConstraint02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes = [| "HashConstraint02.fs" |])>]
    let ``HashConstraint02_fs`` compilation =
        compilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> withDiagnosticMessageMatches "not compatible"
        |> ignore

    // SOURCE=E_GenericTypeConstraint.fs SCFLAGS="--test:ErrorRanges --flaterrors"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes = [| "E_GenericTypeConstraint.fs" |])>]
    let ``E_GenericTypeConstraint_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges"; "--flaterrors" ]
        |> typecheck
        |> shouldFail
        |> withErrorCode 1
        |> ignore

    // SOURCE=E_LazyInType02.fs SCFLAGS="--test:ErrorRanges"
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes = [| "E_LazyInType02.fs" |])>]
    let ``E_LazyInType02_fs`` compilation =
        compilation
        |> asExe
        |> withOptions [ "--test:ErrorRanges" ]
        |> typecheck
        |> shouldFail
        |> withErrorCodes [ 10; 583 ]
        |> ignore

    // SOURCE=MultipleConstraints01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes = [| "MultipleConstraints01.fs" |])>]
    let ``MultipleConstraints01_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=ValueTypesWithConstraints01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes = [| "ValueTypesWithConstraints01.fs" |])>]
    let ``ValueTypesWithConstraints01_fs`` compilation =
        compilation |> asExe |> compile |> shouldSucceed |> ignore

    // SOURCE=UnitSpecialization.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/TypeParameterDefinitions", Includes = [| "UnitSpecialization.fs" |])>]
    let ``UnitSpecialization_fs`` compilation =
        compilation |> asExe |> typecheck |> shouldSucceed |> ignore
