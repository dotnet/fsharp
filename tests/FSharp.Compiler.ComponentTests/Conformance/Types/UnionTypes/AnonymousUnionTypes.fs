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

    [<Theory; FileInlineData("E_AnonTupleEliminationOverlap.fs")>]
    let ``E_TupleEliminationOverlap_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail

    [<Theory; FileInlineData("E_AnonFunctionEliminationOverlap.fs")>]
    let ``E_FunctionEliminationOverlap_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail

    [<Theory; FileInlineData("E_AnonGenericsOverlap1.fs")>]
    let ``E_GenericsOverlap1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail

    [<Theory; FileInlineData("E_AnonGenericsOverlap2.fs")>]
    let ``E_GenericsOverlap2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail

    [<Theory; FileInlineData("E_AnonNakedGenerics1.fs")>]
    let ``E_NakedGenerics1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail

    [<Theory; FileInlineData("E_AnonNakedGenerics2.fs")>]
    let ``E_NakedGenerics2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail

    [<Theory; FileInlineData("E_AnonTypeInclusion1.fs")>]
    let ``E_TypeInclusion1_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail

    [<Theory; FileInlineData("E_AnonTypeInclusion2.fs")>]
    let ``E_TypeInclusion2_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail

    [<Theory; FileInlineData("E_AnonTypeInclusion3.fs")>]
    let ``E_TypeInclusion3_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail

    [<Theory; FileInlineData("E_AnonTypeInclusion4.fs")>]
    let ``E_TypeInclusion4_fs`` compilation =
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