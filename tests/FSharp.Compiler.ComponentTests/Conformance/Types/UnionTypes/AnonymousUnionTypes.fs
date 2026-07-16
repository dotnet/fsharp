// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Types

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AnonymousUnionTypes =

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

    [<Theory; FileInlineData("AnonNonNakedGenerics.fs")>]
    let ``NonNakedGenerics_fs`` compilation =
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

    [<Theory; FileInlineData("E_AnonPatternMatching.fs")>]
    let ``E_PatternMatching_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 5, Col 11, Line 5, Col 12, "Incomplete pattern matches on this expression. For example, the value '``some-other-subtype``' may indicate a case not covered by the pattern(s).")
        ]

    [<Theory; FileInlineData("E_AnonPatternMatchingSubtypeInclusion.fs")>]
    let ``E_PatternMatchingSubtypeInclusion_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 5, Col 11, Line 5, Col 12, "Incomplete pattern matches on this expression. For example, the value '``some-other-subtype``' may indicate a case not covered by the pattern(s).")
        ]

    [<Theory; FileInlineData("W_AnonTypeInclusion1.fs")>]
    let ``W_TypeInclusion1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 3891, Line 0, Col 1, Line 0, Col 1, "The type 'int' is a subtype of 'int' and will be ignored")
        ]

    [<Theory; FileInlineData("W_AnonTypeInclusion2.fs")>]
    let ``W_TypeInclusion2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 3891, Line 4, Col 8, Line 4, Col 30, "The type 'int' is a subtype of 'System.ValueType' and will be ignored")
        ]

    [<Theory; FileInlineData("W_AnonTypeInclusion3.fs")>]
    let ``W_TypeInclusion3_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 3891, Line 0, Col 1, Line 0, Col 1, "The type 'int' is a subtype of 'System.Object' and will be ignored")
        ]

    [<Theory; FileInlineData("W_AnonTypeInclusion4.fs")>]
    let ``W_TypeInclusion4_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 3891, Line 4, Col 8, Line 4, Col 37, "The type 'string' is a subtype of 'System.IComparable' and will be ignored")
        ]

    [<Theory; FileInlineData("W_AnonUnitsOfMeasureOverlap.fs")>]
    let ``W_UnitsOfMeasureOverlap_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 3891, Line 4, Col 18, Line 4, Col 19, "The type 'int' is a subtype of 'int' and will be ignored")
        ]

    [<Theory; FileInlineData("W_AnonTupleEliminationOverlap.fs")>]
    let ``W_TupleEliminationOverlap_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 3891, Line 4, Col 8, Line 4, Col 45, "The type 'int * int' is a subtype of 'int * int' and will be ignored")
        ]

    [<Theory; FileInlineData("W_AnonFunctionEliminationOverlap.fs")>]
    let ``W_FunctionEliminationOverlap_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Warning 3891, Line 4, Col 10, Line 4, Col 46, "The type 'int -> int' is a subtype of 'int -> int' and will be ignored")
        ]