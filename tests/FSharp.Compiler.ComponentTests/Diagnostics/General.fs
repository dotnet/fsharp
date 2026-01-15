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

