// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.TypesAndTypeConstraints

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module CheckingSyntacticTypes =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects status="error" span="(7,17-7,18)" id="FS3136">Type 'byref<byref<'a>>' is illegal because in byref<T>, T cannot contain byref types\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_ByRef01.fs"|])>]
    let ``CheckingSyntacticTypes - E_ByRef01.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3136
        |> withDiagnosticMessageMatches ", T cannot contain byref types\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects status="error" span="(7,24-7,25)" id="FS3136">Type 'byref<byref<'a>>' is illegal because in byref<T>, T cannot contain byref types\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_ByRef02.fs"|])>]
    let ``CheckingSyntacticTypes - E_ByRef02.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3136
        |> withDiagnosticMessageMatches ", T cannot contain byref types\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects status="error" span="(7,17-7,18)" id="FS3136">Type 'byref<byref<byref<'a>>>' is illegal because in byref<T>, T cannot contain byref types\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_ByRef03.fs"|])>]
    let ``CheckingSyntacticTypes - E_ByRef03.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3136
        |> withDiagnosticMessageMatches ", T cannot contain byref types\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"ByRef04.fs"|])>]
    let ``CheckingSyntacticTypes - ByRef04.fs - -a --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"Regressions01.fs"|])>]
    let ``CheckingSyntacticTypes - Regressions01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0564" span="(53,46-53,53)" status="error">'inherit' declarations cannot have 'as' bindings\. To access members of the base class when overriding a method, the syntax 'base\.SomeMember' may be used; 'base' is a keyword\. Remove this 'as' binding\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_Regressions01.fs"|])>]
    let ``CheckingSyntacticTypes - E_Regressions01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0564
        |> withDiagnosticMessageMatches "'inherit' declarations cannot have 'as' bindings\. To access members of the base class when overriding a method, the syntax 'base\.SomeMember' may be used; 'base' is a keyword\. Remove this 'as' binding\.$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0039" span="(296,25)" status="error">The value or constructor 'P4' is not defined</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_Regression02.fs"|])>]
    let ``CheckingSyntacticTypes - E_Regression02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> withDiagnosticMessageMatches "The value or constructor 'P4' is not defined"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0700" span="(13,15-13,40)" status="error">'new'.+constraint</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"DefaultConstructorConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - DefaultConstructorConstraint01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0700
        |> withDiagnosticMessageMatches "'new'.+constraint"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0001" span="(18,11-18,15)" status="error">'C'.+default.+constructor</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"DefaultConstructorConstraint02.fs"|])>]
    let ``CheckingSyntacticTypes - DefaultConstructorConstraint02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "'C'.+default.+constructor"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0700" span="(13,15-13,38)" status="error">'new'.+constraint</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"DefaultConstructorConstraint03.fs"|])>]
    let ``CheckingSyntacticTypes - DefaultConstructorConstraint03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0700
        |> withDiagnosticMessageMatches "'new'.+constraint"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0700" span="(12,15-12,40)" status="error">'new'.+constrain</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"DefaultConstructorConstraint04.fs"|])>]
    let ``CheckingSyntacticTypes - DefaultConstructorConstraint04.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0700
        |> withDiagnosticMessageMatches "'new'.+constrain"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0700" span="(13,15-13,36)" status="error">'new'.+constraint</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"DefaultConstructorConstraint05.fs"|])>]
    let ``CheckingSyntacticTypes - DefaultConstructorConstraint05.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0700
        |> withDiagnosticMessageMatches "'new'.+constraint"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0001" status="error">The type 'byte' does not match the type 'int16'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_EnumConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - E_EnumConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'byte' does not match the type 'int16'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0001" status="error">The type 'string' is not a CLI enum type$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_EnumConstraint02.fs"|])>]
    let ``CheckingSyntacticTypes - E_EnumConstraint02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'string' is not a CLI enum type$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0001" status="error">A generic construct requires that the type 'string' is a CLI or F# struct type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_StructConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - E_StructConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "A generic construct requires that the type 'string' is a CLI or F# struct type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0001" status="error">A generic construct requires that the type 'StructRecd' have reference semantics, but it does not, i.e. it is a struct</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_RefConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - E_RefConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "A generic construct requires that the type 'StructRecd' have reference semantics, but it does not, i.e. it is a struct"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0001" status="error">The type 'Animal' is not compatible with the type 'Dog'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_SubtypeConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - E_SubtypeConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'Animal' is not compatible with the type 'Dog'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0001" status="error">The type 'D1' has a non-standard delegate type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_DelegateConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - E_DelegateConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'D1' has a non-standard delegate type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"NullnessConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - NullnessConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0064" span="(16,28-16,32)" status="warning">This construct causes code to be less generic than indicated by its type annotations. The type variable implied by the use of a '#', '_' or other type annotation at or near</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"W_LessGenericThanAnnotated01.fs"|])>]
    let ``CheckingSyntacticTypes - W_LessGenericThanAnnotated01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> withDiagnosticMessageMatches "This construct causes code to be less generic than indicated by its type annotations. The type variable implied by the use of a '#', '_' or other type annotation at or near"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    // <Expects status="error" id="FS0001" span="(15,9-15,16)">The type 'StdRecd' does not have 'null' as a proper value</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_NullnessConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - E_NullnessConstraint01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'StdRecd' does not have 'null' as a proper value"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects status="error" span="(4,5-4,52)" id="FS0071">Type constraint mismatch when applying the default type 'obj' for a type inference variable\. The type 'obj' does not support the operator 'someFunc' Consider adding further type constraints$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_MemberConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - E_MemberConstraint01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0071
        |> withDiagnosticMessageMatches "Type constraint mismatch when applying the default type 'obj' for a type inference variable\. The type 'obj' does not support the operator 'someFunc' Consider adding further type constraints$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects status="error" span="(10,19-10,25)" id="FS0001">The type 'Foo' has a method 'someFunc' \(full name 'someFunc'\), but the method is not static$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_MemberConstraint02.fs"|])>]
    let ``CheckingSyntacticTypes - E_MemberConstraint02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'Foo' has a method 'someFunc' \(full name 'someFunc'\), but the method is not static$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects status="error" span="(5,5-5,50)" id="FS0735">Expected 1 expressions, got 0$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_MemberConstraint03.fs"|])>]
    let ``CheckingSyntacticTypes - E_MemberConstraint03.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0735
        |> withDiagnosticMessageMatches "Expected 1 expressions, got 0$"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects status="error" span="(19,34-19,40)" id="FS0001">This expression was expected to have type.    'OtherFoo'    .but here has type.    'Foo'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_MemberConstraint04.fs"|])>]
    let ``CheckingSyntacticTypes - E_MemberConstraint04.fs - --test:ErrorRanges --flaterrors`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"; "--flaterrors"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "This expression was expected to have type.    'OtherFoo'    .but here has type.    'Foo'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"MemberConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - MemberConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"UnmanagedConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - UnmanagedConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects status="error" span="(23,11-23,14)" id="FS0001">A generic construct requires that the type 'C' is an unmanaged type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_UnmanagedConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - E_UnmanagedConstraint01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "A generic construct requires that the type 'C' is an unmanaged type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects status="error" span="(14,11-14,15)" id="FS0001">The type 'S' does not support the 'equality' constraint because it has the 'NoEquality' attribute</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_EqualityConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - E_EqualityConstraint01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'S' does not support the 'equality' constraint because it has the 'NoEquality' attribute"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects status="error" span="(21,11-21,16)" id="FS0001">The type 'S2' does not support the 'comparison' constraint because it has the 'NoComparison' attribute</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_ComparisonConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - E_ComparisonConstraint01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'S2' does not support the 'comparison' constraint because it has the 'NoComparison' attribute"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"ComparisonConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - ComparisonConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects status="error" id="FS3151" span="(12,5-12,35)">This member, function or value declaration may not be declared 'inline'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_CannotInlineVirtualMethods1.fs"|])>]
    let ``CheckingSyntacticTypes - E_CannotInlineVirtualMethods1.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3151
        |> withDiagnosticMessageMatches "This member, function or value declaration may not be declared 'inline'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects status="error" id="FS3151" span="(27,20-27,34)">This member, function or value declaration may not be declared 'inline'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_CannotInlineVirtualMethod2.fs"|])>]
    let ``CheckingSyntacticTypes - E_CannotInlineVirtualMethod2.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3151
        |> withDiagnosticMessageMatches "This member, function or value declaration may not be declared 'inline'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"ExplicitMemberConstraints1.fs"|])>]
    let ``CheckingSyntacticTypes - ExplicitMemberConstraints1.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"ExplicitMemberConstraints2.fs"|])>]
    let ``CheckingSyntacticTypes - ExplicitMemberConstraints2.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"ConstraintCall1.fs"|])>]
    let ``CheckingSyntacticTypes - ConstraintCall1.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"ConstraintCall2.fs"|])>]
    let ``CheckingSyntacticTypes - ConstraintCall2.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0001" status="error">The type 'int' does not support the operator 'get_M'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_ExplicitMemberConstraints1.fs"|])>]
    let ``CheckingSyntacticTypes - E_ExplicitMemberConstraints1.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'int' does not support the operator 'get_M'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0001" status="error">None of the types 'bool, int, string' support the operator 'get_M'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_ExplicitMemberConstraints2.fs"|])>]
    let ``CheckingSyntacticTypes - E_ExplicitMemberConstraints2.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "None of the types 'bool, int, string' support the operator 'get_M'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0001" status="error">None of the types 'int, bool, string' support the operator 'M'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_ConstraintCall1.fs"|])>]
    let ``CheckingSyntacticTypes - E_ConstraintCall1.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "None of the types 'int, bool, string' support the operator 'M'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    //<Expects id="FS0001" status="error">The type 'int' does not support the operator 'M'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"E_ConstraintCall2.fs"|])>]
    let ``CheckingSyntacticTypes - E_ConstraintCall2.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> withDiagnosticMessageMatches "The type 'int' does not support the operator 'M'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes", Includes=[|"StructConstraint01.fs"|])>]
    let ``CheckingSyntacticTypes - StructConstraint01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

