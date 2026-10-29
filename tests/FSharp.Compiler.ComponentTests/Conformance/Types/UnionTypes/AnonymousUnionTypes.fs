// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Types

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AnonymousUnionTypes =

    let verifyCompileAndRunNoOverlapWarning compilation =
        compilation
        |> withLangVersionPreview
        |> asExe
        |> withOptions ["--nowarn:988"; "--nowarn:4500"]
        |> compileAndRun

    let verifyCompileAndRunNoOverlapAndDegradeWarning compilation =
        compilation
        |> withLangVersionPreview
        |> asExe
        |> withOptions ["--nowarn:988"; "--nowarn:4500"; "--nowarn:4506"]
        |> compileAndRun

    let verifyCompile compilation =
        compilation
        |> withLangVersionPreview
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> withLangVersionPreview
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compileAndRun

    [<Theory; FileInlineData("AnonBasicSyntax.fs")>]
    let ``BasicSyntax_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonTypeInference.fs")>]
    let ``TypeInference_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonPatternMatching.fs")>]
    let ``PatternMatching_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonPatternMatchingSubtypeInclusion1.fs")>]
    let ``PatternMatchingSubtypeInclusion1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonPatternMatchingSubtypeInclusion2.fs")>]
    let ``PatternMatchingSubtypeInclusion2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonPatternMatchingReachability.fs")>]
    let ``PatternMatchingReachability_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonMethodOverloading.fs")>]
    let ``MethodOverloading_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonCommutativity.fs")>]
    let ``Commutativity_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonAssociativity.fs")>]
    let ``Associativity_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonSubsumption1.fs")>]
    let ``Subsumption1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonSubsumption2.fs")>]
    let ``Subsumption2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonGenerics.fs")>]
    let ``Generics_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonNakedGenerics.fs")>]
    let ``NakedGenerics_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonNakedGenericsWithNull.fs")>]
    let ``NakedGenericsWithNull_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonNonNakedGenerics.fs")>]
    let ``NonNakedGenerics_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonPatternMatching2Columns.fs")>]
    let ``PatternMatching2Columns_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonPatternMatchingWithNull.fs")>]
    let ``PatternMatchingWithNull_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonPatternMatchingWithNull2Columns.fs")>]
    let ``PatternMatchingWithNull2Columns_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonWithNullChain.fs")>]
    let ``WithNullChain_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonWithNullHoist.fs")>]
    let ``WithNullHoist_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonBarLexing.fs")>]
    let ``BarLexing_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AnonGenericAncestorRemap.fs")>]
    let ``GenericAncestorRemap_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRunNoOverlapWarning
        |> shouldSucceed

    [<Theory; FileInlineData("W_AnonDegrade.fs")>]
    let ``Degrade_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRunNoOverlapAndDegradeWarning
        |> shouldSucceed

    [<Theory; FileInlineData("W_AnonDegradeWithNull.fs")>]
    let ``DegradeWithNull_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRunNoOverlapAndDegradeWarning
        |> shouldSucceed

    [<Theory; FileInlineData("AnonErasedType.fs")>]
    let ``ErasedType_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("E_AnonWildcard.fs")>]
    let ``E_Wildcard_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 715, Line 4, Col 27, Line 4, Col 28, "Anonymous type variables are not permitted in this declaration")
        ]

    [<Theory; FileInlineData("E_AnonTypeInference.fs")>]
    let ``E_TypeInference_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 4, Col 39, Line 4, Col 46, "All branches of an 'if' expression must return values implicitly convertible to the type of the first branch, which here is 'int'. This branch returns a value of type 'string'.")
        ]

    [<Theory; FileInlineData("E_AnonNonNakedGenerics.fs")>]
    let ``E_NonNakedGenerics_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 193, Line 7, Col 17, Line 7, Col 19, "The type 'List<'a>' is ambiguous with respect to the anonymous union type '(int list | string list)' - multiple union cases match")
        ]

    [<Theory; FileInlineData("E_AnonSystemNullable.fs")>]
    let ``E_SystemNullable_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 4501, Line 4, Col 8, Line 4, Col 37, "The type System.Nullable<'T> is not allowed in an anonymous union type. Consider adding null case instead.")
        ]

    [<Theory; FileInlineData("E_AnonWithNullPosition1.fs")>]
    let ``E_WithNullPosition1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 4504, Line 4, Col 9, Line 4, Col 26, "'null' cannot be applied to a parenthesized anonymous union. Write (int | string | null).")
        ]

    [<Theory; FileInlineData("E_AnonWithNullPosition2.fs")>]
    let ``E_WithNullPosition2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3260, Line 4, Col 9, Line 4, Col 17, "The type 'int' does not support a nullness qualification.");
            (Error 43, Line 4, Col 9, Line 4, Col 17, "A generic construct requires that the type 'int' have reference semantics, but it does not, i.e. it is a struct")
        ]

    [<Theory; FileInlineData("E_AnonWithNullPosition3.fs")>]
    let ``E_WithNullPosition3_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 4502, Line 4, Col 8, Line 4, Col 25, "'null' may only appear as the last case of an outermost anonymous union, e.g. '(int | string | null)'.")
        ]

    [<Theory; FileInlineData("E_AnonWithNullPosition4.fs")>]
    let ``E_WithNullPosition4_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 618, Line 4, Col 9, Line 4, Col 13, "Invalid literal in type")
        ]

    [<Theory; FileInlineData("E_AnonWithNullRefTypeAncestor1.fs")>]
    let ``E_WithNullRefTypeAncestor1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 4503, Line 4, Col 8, Line 4, Col 23, "The type 'System.ValueType' does not support 'null' because it is not a reference type. A null case may only be added to an anonymous union whose common type is a reference type.")
        ]

    [<Theory; FileInlineData("E_AnonWithNullRefTypeAncestor2.fs")>]
    let ``E_WithNullRefTypeAncestor2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 4503, Line 5, Col 9, Line 5, Col 15, "The type 'System.ValueType' does not support 'null' because it is not a reference type. A null case may only be added to an anonymous union whose common type is a reference type.")
        ]

    [<Theory; FileInlineData("E_AnonDegradeNullValueType.fs")>]
    let ``E_DegradeNullValueType_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 5, Col 8, Line 5, Col 26, "The runtime representation of the type 'int<kg>' is a subtype of the runtime representation of 'int' and the case will be ignored.");
            (Error 4503, Line 5, Col 8, Line 5, Col 26, "The type 'int' does not support 'null' because it is not a reference type. A null case may only be added to an anonymous union whose common type is a reference type.")
        ]

    [<Theory; FileInlineData("E_AnonWithNullNested1.fs")>]
    let ``E_WithNullNested1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 4502, Line 4, Col 8, Line 4, Col 27, "'null' may only appear as the last case of an outermost anonymous union, e.g. '(int | string | null)'.")
        ]

    [<Theory; FileInlineData("E_AnonWithNullNested2.fs")>]
    let ``E_WithNullNested2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 4502, Line 4, Col 8, Line 4, Col 45, "'null' may only appear as the last case of an outermost anonymous union, e.g. '(int | string | null)'.")
        ]

    [<Theory; FileInlineData("E_AnonDirectlyDuplicateCases1.fs")>]
    let ``E_DirectlyDuplicateCases1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 4505, Line 4, Col 8, Line 4, Col 17, "This anonymous union type has multiple cases of the same type 'int'. Duplicate case types are not permitted in a directly-written anonymous union.")
        ]

    [<Theory; FileInlineData("E_AnonDirectlyDuplicateCases2.fs")>]
    let ``E_DirectlyDuplicateCases2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 4505, Line 4, Col 8, Line 4, Col 30, "This anonymous union type has multiple cases of the same type 'int'. Duplicate case types are not permitted in a directly-written anonymous union.")
        ]

    [<Theory; FileInlineData("E_AnonNested1.fs")>]
    let ``E_Nested1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 4507, Line 4, Col 8, Line 4, Col 28, "Anonymous union types may not be nested directly. Use a type alias for the nested anonymous union case instead.")
        ]

    [<Theory; FileInlineData("E_AnonNested2.fs")>]
    let ``E_Nested2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 4507, Line 4, Col 8, Line 4, Col 35, "Anonymous union types may not be nested directly. Use a type alias for the nested anonymous union case instead.")
        ]


    [<Theory; FileInlineData("E_AnonAsPattern.fs")>]
    let ``E_AnonAsPattern_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3062, Line 6, Col 7, Line 6, Col 23, "This type test with a provided type '(int8 | string)' is not allowed because this provided type will be erased to 'System.Object' at runtime.")
        ]

    [<Theory; FileInlineData("W_AnonPatternMatching.fs")>]
    let ``W_PatternMatching_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 5, Col 11, Line 5, Col 12, "Incomplete pattern matches on this expression. For example, the value '``some-other-subtype``' may indicate a case not covered by the pattern(s).")
        ]

    [<Theory; FileInlineData("W_AnonPatternMatchingSubtypeInclusion1.fs")>]
    let ``W_PatternMatchingSubtypeInclusion1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 5, Col 11, Line 5, Col 12, "Incomplete pattern matches on this expression. For example, the value '``some-other-subtype``' may indicate a case not covered by the pattern(s).")
        ]

    [<Theory; FileInlineData("W_AnonPatternMatchingSubtypeInclusion2.fs")>]
    let ``W_PatternMatchingSubtypeInclusion2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 26, Line 7, Col 7, Line 7, Col 19, "This rule will never be matched")
        ]

    [<Theory; FileInlineData("W_AnonPatternMatchingUnreachable.fs")>]
    let ``W_PatternMatchingUnreachable_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 26, Line 6, Col 7, Line 6, Col 21, "This rule will never be matched")
        ]

    [<Theory; FileInlineData("W_AnonPatternMatching2Columns.fs")>]
    let ``W_PatternMatching2Columns_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 5, Col 11, Line 5, Col 17, "Incomplete pattern matches on this expression. For example, the value '(``some-other-subtype``,``some-other-subtype``)' may indicate a case not covered by the pattern(s).")
        ]

    [<Theory; FileInlineData("W_AnonPatternMatchingWithNull.fs")>]
    let ``W_PatternMatchingWithNull_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 5, Col 11, Line 5, Col 12, "Incomplete pattern matches on this expression. For example, the value '``some-other-subtype``' may indicate a case not covered by the pattern(s).")
        ]

    [<Theory; FileInlineData("W_AnonPatternMatchingWithNull2Columns.fs")>]
    let ``W_PatternMatchingWithNull2Columns_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 5, Col 11, Line 5, Col 17, "Incomplete pattern matches on this expression. For example, the value '(_,``some-other-subtype``)' may indicate a case not covered by the pattern(s).")
        ]

    [<Theory; FileInlineData("W_AnonTypeInclusion1.fs")>]
    let ``W_TypeInclusion1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 4, Col 8, Line 4, Col 37, "The runtime representation of the type 'int' is a subtype of the runtime representation of 'System.ValueType' and the case will be ignored.")
        ]

    [<Theory; FileInlineData("W_AnonTypeInclusion2.fs")>]
    let ``W_TypeInclusion2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 4, Col 8, Line 4, Col 17, "The runtime representation of the type 'int' is a subtype of the runtime representation of 'obj' and the case will be ignored.")
            (Warning 4506, Line 4, Col 8, Line 4, Col 17, "This anonymous union type degrades to a single type 'obj'. Consider using the underlying type directly instead of an anonymous union.")
        ]

    [<Theory; FileInlineData("W_AnonTypeInclusion3.fs")>]
    let ``W_TypeInclusion3_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 4, Col 8, Line 4, Col 37, "The runtime representation of the type 'string' is a subtype of the runtime representation of 'System.IComparable' and the case will be ignored.")
            (Warning 4506, Line 4, Col 8, Line 4, Col 37, "This anonymous union type degrades to a single type 'System.IComparable'. Consider using the underlying type directly instead of an anonymous union.")
        ]

    [<Theory; FileInlineData("W_AnonTypeInclusionRuntime.fs")>]
    let ``W_TypeInclusionRuntime_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 5, Col 8, Line 5, Col 49, "The runtime representation of the type 'List<(float | string)>' is a subtype of the runtime representation of 'List<(int | string)>' and the case will be ignored.")
            (Warning 4506, Line 5, Col 8, Line 5, Col 49, "This anonymous union type degrades to a single type 'List<(int | string)>'. Consider using the underlying type directly instead of an anonymous union.")
        ]

    [<Theory; FileInlineData("W_AnonTupleEliminationOverlap.fs")>]
    let ``W_TupleEliminationOverlap_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 4, Col 8, Line 4, Col 45, "The runtime representation of the type 'int * int' is a subtype of the runtime representation of 'int * int' and the case will be ignored.")
            (Warning 4506, Line 4, Col 8, Line 4, Col 45, "This anonymous union type degrades to a single type 'int * int'. Consider using the underlying type directly instead of an anonymous union.")
        ]

    [<Theory; FileInlineData("W_AnonFunctionEliminationOverlap.fs")>]
    let ``W_FunctionEliminationOverlap_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 4, Col 10, Line 4, Col 46, "The runtime representation of the type 'int -> int' is a subtype of the runtime representation of 'int -> int' and the case will be ignored.")
            (Warning 4506, Line 4, Col 10, Line 4, Col 46, "This anonymous union type degrades to a single type 'int -> int'. Consider using the underlying type directly instead of an anonymous union.")
        ]

    [<Theory; FileInlineData("W_AnonUnitsOfMeasureOverlap.fs")>]
    let ``W_UnitsOfMeasureOverlap_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 5, Col 8, Line 5, Col 23, "The runtime representation of the type 'int<kg>' is a subtype of the runtime representation of 'int' and the case will be ignored.")
            (Warning 4506, Line 5, Col 8, Line 5, Col 23, "This anonymous union type degrades to a single type 'int'. Consider using the underlying type directly instead of an anonymous union.")
        ]

    [<Theory; FileInlineData("W_AnonDegrade.fs")>]
    let ``W_Degrade_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 6, Col 8, Line 6, Col 13, "The runtime representation of the type 'B' is a subtype of the runtime representation of 'A' and the case will be ignored.")
            (Warning 4506, Line 6, Col 8, Line 6, Col 13, "This anonymous union type degrades to a single type 'A'. Consider using the underlying type directly instead of an anonymous union.")
        ]

    [<Theory; FileInlineData("W_AnonDegradeWithNull.fs")>]
    let ``W_DegradeWithNull_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 4, Col 8, Line 4, Col 25, "The runtime representation of the type 'string' is a subtype of the runtime representation of 'obj' and the case will be ignored.")
            (Warning 4506, Line 4, Col 8, Line 4, Col 25, "This anonymous union type degrades to a single type 'obj | null'. Consider using the underlying type directly instead of an anonymous union.")
        ]

    [<Theory; FileInlineData("W_AnonAliasDup1.fs")>]
    let ``W_AliasDup1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 5, Col 8, Line 5, Col 15, "The runtime representation of the type 'int' is a subtype of the runtime representation of 'int' and the case will be ignored.")
        ]


    [<Theory; FileInlineData("W_AnonAliasDup2.fs")>]
    let ``W_AliasDup2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 5, Col 8, Line 5, Col 15, "The runtime representation of the type 'int' is a subtype of the runtime representation of 'int' and the case will be ignored.")
        ]