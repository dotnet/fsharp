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

    [<Theory; FileInlineData("E_AnonTypeInference.fs")>]
    let ``E_TypeInference_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail

    [<Theory; FileInlineData("E_AnonPatternMatching.fs")>]
    let ``E_PatternMatching_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail

    [<Theory; FileInlineData("E_AnonPatternMatchingSubtypeInclusion.fs")>]
    let ``E_PatternMatchingSubtypeInclusion_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail

    [<Theory; FileInlineData("W_AnonTypeInclusion1.fs")>]
    let ``W_TypeInclusion1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> withWarningCode 3875

    [<Theory; FileInlineData("W_AnonTypeInclusion2.fs")>]
    let ``W_TypeInclusion2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> withWarningCode 3875

    [<Theory; FileInlineData("W_AnonTypeInclusion3.fs")>]
    let ``W_TypeInclusion3_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> withWarningCode 3875

    [<Theory; FileInlineData("W_AnonTypeInclusion4.fs")>]
    let ``W_TypeInclusion4_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> withWarningCode 3875

    [<Theory; FileInlineData("W_AnonUnitsOfMeasureOverlap.fs")>]
    let ``W_UnitsOfMeasureOverlap_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> withWarningCode 3875

    [<Theory; FileInlineData("W_AnonTupleEliminationOverlap.fs")>]
    let ``W_TupleEliminationOverlap_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> withWarningCode 3875

    [<Theory; FileInlineData("W_AnonFunctionEliminationOverlap.fs")>]
    let ``W_FunctionEliminationOverlap_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> withWarningCode 3875