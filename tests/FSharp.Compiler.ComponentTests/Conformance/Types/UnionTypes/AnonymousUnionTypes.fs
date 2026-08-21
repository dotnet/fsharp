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

    [<Theory; FileInlineData("AnonPatternMatchingSubtypeInclusion.fs")>]
    let ``PatternMatchingSubtypeInclusion_fs`` compilation =
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

    [<Theory; FileInlineData("E_AnonWildcard.fs")>]
    let ``E_Wildcard_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 715, Line 4, Col 27, Line 4, Col 28, "Anonymous type variables are not permitted in this declaration")
            (Error 35, Line 4, Col 6, Line 4, Col 23, "This construct is deprecated: This type abbreviation has one or more declared type parameters that do not appear in the type being abbreviated. Type abbreviations must use all declared type parameters in the type being abbreviated. Consider removing one or more type parameters, or use a concrete type definition that wraps an underlying type, such as 'type C<'a> = C of ...'.")
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

    [<Theory; FileInlineData("E_AnonSystemNullable1.fs")>]
    let ``E_SystemNullable1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 4501, Line 4, Col 8, Line 4, Col 37, "The type System.Nullable<'T> is not allowed in an anonymous union type. Consider adding null case instead.")
        ]

    [<Theory; FileInlineData("E_AnonSystemNullable2.fs")>]
    let ``E_SystemNullable2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 4501, Line 4, Col 15, Line 4, Col 44, "The type System.Nullable<'T> is not allowed in an anonymous union type. Consider adding null case instead.")
        ]

    [<Theory; FileInlineData("E_AnonWithNullPosition1.fs")>]
    let ``E_WithNullPosition1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3260, Line 4, Col 9, Line 4, Col 26, "'null' cannot be applied to a standalone anonymous union (int | string). Add 'null' as the last case of the outer union instead, e.g. '(int|string|null)'.")
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

    [<Theory; FileInlineData("E_AnonWithNullPosition5.fs")>]
    let ``E_WithNullPosition5_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3260, Line 5, Col 9, Line 5, Col 15, "'null' cannot be applied to a standalone anonymous union X. Add 'null' as the last case of the outer union instead, e.g. '(int|string|null)'.")
        ]

    [<Theory; FileInlineData("E_AnonWithNullRefTypeAncestor.fs")>]
    let ``E_WithNullRefTypeAncestor_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 4503, Line 4, Col 8, Line 4, Col 23, "The type 'System.ValueType' does not support 'null' because it is not a reference type. A null case may only be added to an anonymous union whose common type is a reference type.")
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

    [<Theory; FileInlineData("W_AnonPatternMatchingSubtypeInclusion.fs")>]
    let ``W_PatternMatchingSubtypeInclusion_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 5, Col 11, Line 5, Col 12, "Incomplete pattern matches on this expression. For example, the value '``some-other-subtype``' may indicate a case not covered by the pattern(s).")
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
            (Warning 4500, Line 4, Col 8, Line 4, Col 17, "The type 'int' is a subtype of 'int' and will be ignored")
        ]

    [<Theory; FileInlineData("W_AnonTypeInclusion2.fs")>]
    let ``W_TypeInclusion2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 4, Col 8, Line 4, Col 30, "The type 'int' is a subtype of 'System.ValueType' and will be ignored")
        ]

    [<Theory; FileInlineData("W_AnonTypeInclusion3.fs")>]
    let ``W_TypeInclusion3_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 4, Col 8, Line 4, Col 17, "The type 'int' is a subtype of 'obj' and will be ignored")
        ]

    [<Theory; FileInlineData("W_AnonTypeInclusion4.fs")>]
    let ``W_TypeInclusion4_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 4, Col 8, Line 4, Col 37, "The type 'string' is a subtype of 'System.IComparable' and will be ignored")
        ]

    [<Theory; FileInlineData("W_AnonUnitsOfMeasureOverlap.fs")>]
    let ``W_UnitsOfMeasureOverlap_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 4, Col 8, Line 4, Col 20, "The type 'int' is a subtype of 'int' and will be ignored")
        ]

    [<Theory; FileInlineData("W_AnonTupleEliminationOverlap.fs")>]
    let ``W_TupleEliminationOverlap_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 4, Col 8, Line 4, Col 45, "The type 'int * int' is a subtype of 'int * int' and will be ignored")
        ]

    [<Theory; FileInlineData("W_AnonFunctionEliminationOverlap.fs")>]
    let ``W_FunctionEliminationOverlap_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 4500, Line 4, Col 10, Line 4, Col 46, "The type 'int -> int' is a subtype of 'int -> int' and will be ignored")
        ]