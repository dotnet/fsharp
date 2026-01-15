// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Diagnostics

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

/// Tests for General diagnostics - migrated from tests/fsharpqa/Source/Diagnostics/General/
module General =

    let private resourcePath = __SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General"

    // This test was automatically generated (moved from FSharpQA suite - Diagnostics/General)
    //<Expects>\(18,22-18,30\).+warning FS0046: The keyword 'tailcall' is reserved for future use by F#</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_Keyword_tailcall01.fs"|])>]
    let ``General - W_Keyword_tailcall01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> ignore

    // E_NullableOperators01.fs - FS0043 errors for nullable operators without opening module
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_NullableOperators01.fs"|])>]
    let ``E_NullableOperators01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0043
        |> ignore

    // E_FormattingStringBadPrecision01.fs - FS0741 bad precision in format specifier
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_FormattingStringBadPrecision01.fs"|])>]
    let ``E_FormattingStringBadPrecision01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0741
        |> ignore

    // E_FormattingStringBadSpecifier01.fs - FS0741 bad format specifier
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_FormattingStringBadSpecifier01.fs"|])>]
    let ``E_FormattingStringBadSpecifier01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0741
        |> ignore

    // E_FormattingStringFlagSetTwice01.fs - FS0741 flag set twice in format specifier
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_FormattingStringFlagSetTwice01.fs"|])>]
    let ``E_FormattingStringFlagSetTwice01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0741
        |> ignore

    // E_FormattingStringInvalid01.fs - FS0741 invalid formatting modifier
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_FormattingStringInvalid01.fs"|])>]
    let ``E_FormattingStringInvalid01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0741
        |> ignore

    // E_FormattingStringPrecision01.fs - FS0741 format does not support precision
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_FormattingStringPrecision01.fs"|])>]
    let ``E_FormattingStringPrecision01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0741
        |> ignore

    // MutatingImmutable01.fs - FS0027 value is not mutable
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"MutatingImmutable01.fs"|])>]
    let ``MutatingImmutable01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0027
        |> ignore

    // MutatingImmutable02.fs - FS0027 value is not mutable
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"MutatingImmutable02.fs"|])>]
    let ``MutatingImmutable02_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0027
        |> ignore

    // E_Big_int01.fs - FS0039 Big_int module not defined
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_Big_int01.fs"|])>]
    let ``E_Big_int01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> ignore

    // E_Multiline01.fs - FS0001 type mismatch across multiple lines
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_Multiline01.fs"|])>]
    let ``E_Multiline01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> ignore

    // W_Multiline02.fs - FS0020 result implicitly ignored across multiple lines
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_Multiline02.fs"|])>]
    let ``W_Multiline02_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0020
        |> ignore

    // E_Multiline03.fs - FS0003 value is not a function, spans multiple lines
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_Multiline03.fs"|])>]
    let ``E_Multiline03_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0003
        |> ignore

    // W_NoValueHasBeenCopiedWarning.fs - should succeed (warning suppressed)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_NoValueHasBeenCopiedWarning.fs"|])>]
    let ``W_NoValueHasBeenCopiedWarning_fs`` compilation =
        compilation
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // E_TryFinallyIncompleteStructuredConstruct.fs - FS0599 missing qualification after '.'
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_TryFinallyIncompleteStructuredConstruct.fs"|])>]
    let ``E_TryFinallyIncompleteStructuredConstruct_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0599
        |> ignore

    // E_SpanExtendsToNextToken01.fs - FS0201 namespaces cannot contain values
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_SpanExtendsToNextToken01.fs"|])>]
    let ``E_SpanExtendsToNextToken01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0201
        |> ignore

    // TypecheckSignature01.fs - FS0193 type mismatch with signature reference
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"TypecheckSignature01.fs"|])>]
    let ``TypecheckSignature01_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0193
        |> ignore

    // W_HashOfSealedType01.fs - FS0064 warning about hash on sealed type
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_HashOfSealedType01.fs"|])>]
    let ``W_HashOfSealedType01_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0064
        |> ignore

    // Generic_Subtype01.fs - should succeed (no errors expected)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"Generic_Subtype01.fs"|])>]
    let ``Generic_Subtype01_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--warnaserror+"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // E_BaseInObjectExpression01.fs - FS0564 inherit cannot have 'as' bindings
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_BaseInObjectExpression01.fs"|])>]
    let ``E_BaseInObjectExpression01_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0564
        |> ignore

    // E_AsBindingOnInheritDecl01.fs - FS0564 inherit cannot have 'as' bindings
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_AsBindingOnInheritDecl01.fs"|])>]
    let ``E_AsBindingOnInheritDecl01_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0564
        |> ignore

    // E_NoPoundRDirectiveInFSFile01.fs - FS0076 #r directive only in script files
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_NoPoundRDirectiveInFSFile01.fs"|])>]
    let ``E_NoPoundRDirectiveInFSFile01_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0076
        |> ignore

    // W_InstantiationOfGenericTypeMissing01.fs - FS1125 missing generic type instantiation
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_InstantiationOfGenericTypeMissing01.fs"|])>]
    let ``W_InstantiationOfGenericTypeMissing01_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 1125
        |> ignore

    // W_InstantiationOfGenericTypeMissing02.fs - should succeed (warning 1125 is not an error here)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_InstantiationOfGenericTypeMissing02.fs"|])>]
    let ``W_InstantiationOfGenericTypeMissing02_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--warnaserror+"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // W_redefineOperator01.fs - FS0086 operator should not be redefined
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator01.fs"|])>]
    let ``W_redefineOperator01_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> ignore

    // W_redefineOperator02.fs - FS0086 operator should not be redefined
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator02.fs"|])>]
    let ``W_redefineOperator02_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> ignore

    // W_redefineOperator03.fs - FS0086 <= operator should not be redefined
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator03.fs"|])>]
    let ``W_redefineOperator03_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> ignore

    // W_redefineOperator04.fs - FS0086 >= operator should not be redefined
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator04.fs"|])>]
    let ``W_redefineOperator04_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> ignore

    // W_redefineOperator05.fs - FS0086 <> operator should not be redefined
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator05.fs"|])>]
    let ``W_redefineOperator05_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> ignore

    // W_redefineOperator06.fs - FS0086 = operator should not be redefined
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator06.fs"|])>]
    let ``W_redefineOperator06_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> ignore

    // W_redefineOperator08.fs - FS0086 && operator should not be redefined
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator08.fs"|])>]
    let ``W_redefineOperator08_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> ignore

    // W_redefineOperator10.fs - FS0086 || operator should not be redefined
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_redefineOperator10.fs"|])>]
    let ``W_redefineOperator10_fs`` compilation =
        compilation
        |> withOptions ["-a"; "--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0086
        |> ignore

    // E_matrix_class01.fs - FS0039 type 'matrix' not defined in Math
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_matrix_class01.fs"|])>]
    let ``E_matrix_class01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> ignore

    // E_matrix_interface01.fs - FS0039 type 'matrix' not defined in Math
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_matrix_interface01.fs"|])>]
    let ``E_matrix_interface01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> ignore

    // E_matrix_LetBinding01.fs - FS0039 type 'matrix' not defined in Math
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_matrix_LetBinding01.fs"|])>]
    let ``E_matrix_LetBinding01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> ignore

    // E_matrix_struct01.fs - FS0039 type 'matrix' not defined in Math
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_matrix_struct01.fs"|])>]
    let ``E_matrix_struct01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> ignore

    // E_ExpressionHasType_FullPath01.fs - FS0001 type mismatch with full path
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_ExpressionHasType_FullPath01.fs"|])>]
    let ``E_ExpressionHasType_FullPath01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> ignore

    // W_GenericTypeProvideATypeInstantiation01.fs - FS1125 generic type instantiation missing
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_GenericTypeProvideATypeInstantiation01.fs"|])>]
    let ``W_GenericTypeProvideATypeInstantiation01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 1125
        |> ignore

    // E_ObjectConstructorAndTry01.fs - Regression test for FSHARP1.0:1980 (class with for loop in constructor)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_ObjectConstructorAndTry01.fs"|])>]
    let ``E_ObjectConstructorAndTry01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // E_ObjectConstructorAndTry02.fs - Regression test for FSHARP1.0:1980 (struct with for loop in constructor)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_ObjectConstructorAndTry02.fs"|])>]
    let ``E_ObjectConstructorAndTry02_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // X-DontWarnOnImplicitModule01.fsx - Regression test for FSHARP1.0:2893 (no implicit module warning for .fsx)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"X-DontWarnOnImplicitModule01.fsx"|])>]
    let ``X-DontWarnOnImplicitModule01_fsx`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // X-DontWarnOnImplicitModule01.fsscript - Regression test for FSHARP1.0:2893 (no implicit module warning for .fsscript)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"X-DontWarnOnImplicitModule01.fsscript"|])>]
    let ``X-DontWarnOnImplicitModule01_fsscript`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // E_AreYouMissingAnArgumentToAFunction01.fs - Regression test for FSHARP1.0:2804
    // FS0001 type mismatch errors (make sure we don't emit ?.)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_AreYouMissingAnArgumentToAFunction01.fs"|])>]
    let ``E_AreYouMissingAnArgumentToAFunction01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> ignore

    // E_AreYouMissingAnArgumentToAFunction01b.fs - Regression test for FSHARP1.0:2804
    // FS0001 type mismatch (make sure we don't emit ?.)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_AreYouMissingAnArgumentToAFunction01b.fs"|])>]
    let ``E_AreYouMissingAnArgumentToAFunction01b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0001
        |> ignore

    // E_ConsiderAddingSealedAttribute01 - Multi-file test (fsi + fs)
    // FS0297 type definitions not compatible because implementation is not sealed but signature implies it is
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_ConsiderAddingSealedAttribute01.fsi"|])>]
    let ``E_ConsiderAddingSealedAttribute01_fsi_fs`` compilation =
        compilation
        |> withAdditionalSourceFile (SourceFromPath (resourcePath ++ "E_ConsiderAddingSealedAttribute01.fs"))
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withErrorCode 0297
        |> ignore

    // E_LiteralEnumerationMustHaveType01.fs - Regression test for FSHARP1.0:1729
    // FS0886 not a valid value for enumeration literal (BigInt and NatNum not allowed)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_LiteralEnumerationMustHaveType01.fs"|])>]
    let ``E_LiteralEnumerationMustHaveType01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0886
        |> ignore

    // E_IndexedPropertySetter01.fs - FS0554 invalid declaration syntax
    // Regression test for FSHARP1.0:1185
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_IndexedPropertySetter01.fs"|])>]
    let ``E_IndexedPropertySetter01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0554
        |> ignore

    // E_PropertyIsNotReadable01.fs - FS0807 property is not readable
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_PropertyIsNotReadable01.fs"|])>]
    let ``E_PropertyIsNotReadable01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0807
        |> ignore

    // E_MemberConstraintsWithSpecialStatus01.fs - FS0077 member constraint with 'Pow' name given special status
    // Regression test for FSHARP1.0:2890
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_MemberConstraintsWithSpecialStatus01.fs"|])>]
    let ``E_MemberConstraintsWithSpecialStatus01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0077
        |> ignore

    // E_FoundInPowerPack_Matrix01.fs - FS0039 type 'Matrix' is not defined
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_FoundInPowerPack_Matrix01.fs"|])>]
    let ``E_FoundInPowerPack_Matrix01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0039
        |> ignore

    // E_UnexpectedKeywordAs01.fs - FS0010 unexpected keyword 'as' in expression
    // Regression test for FSHARP1.0:1698
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_UnexpectedKeywordAs01.fs"|])>]
    let ``E_UnexpectedKeywordAs01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // E_IncompleteConstruct01.fs - FS3567 Expecting member body
    // Regression test for FSHARP1.0:1181
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_IncompleteConstruct01.fs"|])>]
    let ``E_IncompleteConstruct01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3567
        |> ignore

    // E_IncompleteConstruct01b.fs - FS3567 Expecting member body (no syntax error message)
    // Regression test for FSHARP1.0:1181
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_IncompleteConstruct01b.fs"|])>]
    let ``E_IncompleteConstruct01b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 3567
        |> ignore

    // E_UnexpectedKeyworkWith01.fs - FS0010 unexpected keyword 'with'
    // Regression test for FSHARP1.0:1872
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_UnexpectedKeyworkWith01.fs"|])>]
    let ``E_UnexpectedKeyworkWith01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // E_MemberObjectctorTakeGiven.fs - FS0502 member/object constructor wrong type arguments
    // Regression test for FSHARP1.0:1423
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_MemberObjectctorTakeGiven.fs"|])>]
    let ``E_MemberObjectctorTakeGiven_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0502
        |> ignore

    // E_StructMustHaveAtLeastOneField.fs - struct can now be empty
    // Related to FSHARP1.0:3143
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_StructMustHaveAtLeastOneField.fs"|])>]
    let ``E_StructMustHaveAtLeastOneField_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        |> ignore

    // E_UnexpectedSymbol01.fs - FS0010 unexpected symbol, FS0588 unfinished let block
    // Regression test for FSHARP1.0:2099, FSHARP1.0:2670
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_UnexpectedSymbol01.fs"|])>]
    let ``E_UnexpectedSymbol01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0010
        |> ignore

    // W_OverrideImplementationInAugmentation01a.fs - FS0060 warning on override in augmentation
    // Regression test for FSHARP1.0:1273
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_OverrideImplementationInAugmentation01a.fs"|])>]
    let ``W_OverrideImplementationInAugmentation01a_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0060
        |> ignore

    // W_OverrideImplementationInAugmentation02b.fs - FS0060 warning + FS0001 error (type mismatch)
    // Regression test for FSHARP1.0:1273
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_OverrideImplementationInAugmentation02b.fs"|])>]
    let ``W_OverrideImplementationInAugmentation02b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0060
        |> withErrorCode 0001
        |> ignore

    // W_OverrideImplementationInAugmentation03a.fs - FS0060 warning on default in augmentation
    // Regression test for FSHARP1.0:1273
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_OverrideImplementationInAugmentation03a.fs"|])>]
    let ``W_OverrideImplementationInAugmentation03a_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0060
        |> ignore

    // W_OverrideImplementationInAugmentation03b.fs - FS0060 warning on override in augmentation
    // Regression test for FSHARP1.0:1273
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_OverrideImplementationInAugmentation03b.fs"|])>]
    let ``W_OverrideImplementationInAugmentation03b_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0060
        |> ignore

    // E_Quotation_UnresolvedGenericConstruct01.fs - FS0331 and FS0071 errors
    // Regression test for FSHARP1.0:1278
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_Quotation_UnresolvedGenericConstruct01.fs"|])>]
    let ``E_Quotation_UnresolvedGenericConstruct01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0331
        |> withErrorCode 0071
        |> ignore

    // E_InvalidObjectExpression01.fs - FS0251, FS0767, FS0035 errors
    // Regression test for DevDiv:4858
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"E_InvalidObjectExpression01.fs"|])>]
    let ``E_InvalidObjectExpression01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withErrorCode 0251
        |> withErrorCode 0767
        |> withErrorCode 0035
        |> ignore

    // W_CreateIDisposable.fs - FS0760 IDisposable creation should use 'new Type(args)'
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_CreateIDisposable.fs"|])>]
    let ``W_CreateIDisposable_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0760
        |> ignore

    // W_FailwithRedundantArgs.fs - FS3189 redundant args in failwith
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_FailwithRedundantArgs.fs"|])>]
    let ``W_FailwithRedundantArgs_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3189
        |> ignore

    // W_FailwithfRedundantArgs.fs - FS3189 redundant args in failwithf
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_FailwithfRedundantArgs.fs"|])>]
    let ``W_FailwithfRedundantArgs_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3189
        |> ignore

    // W_RaiseRedundantArgs.fs - FS3189 redundant args in raise
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_RaiseRedundantArgs.fs"|])>]
    let ``W_RaiseRedundantArgs_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3189
        |> ignore

    // W_InvalidArgRedundantArgs.fs - FS3189 redundant args in invalidArg
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_InvalidArgRedundantArgs.fs"|])>]
    let ``W_InvalidArgRedundantArgs_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3189
        |> ignore

    // W_NullArgRedundantArgs.fs - FS3189 redundant args in nullArg
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_NullArgRedundantArgs.fs"|])>]
    let ``W_NullArgRedundantArgs_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3189
        |> ignore

    // W_InvalidOpRedundantArgs.fs - FS3189 redundant args in invalidOp
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_InvalidOpRedundantArgs.fs"|])>]
    let ``W_InvalidOpRedundantArgs_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3189
        |> ignore

    // W_LowercaseLiteralIgnored.fs - FS3190 lowercase literal shadowed warning
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_LowercaseLiteralIgnored.fs"|])>]
    let ``W_LowercaseLiteralIgnored_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 3190
        |> ignore

    // W_LowercaseLiteralNotIgnored.fs - FS0026 rule will never be matched
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_LowercaseLiteralNotIgnored.fs"|])>]
    let ``W_LowercaseLiteralNotIgnored_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0026
        |> ignore

    // W_IndexedPropertySetter01.fs - FS0191 indexed property setter should be curried
    // Regression test for FSHARP1.0:1185
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/General", Includes=[|"W_IndexedPropertySetter01.fs"|])>]
    let ``W_IndexedPropertySetter01_fs`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0191
        |> ignore
