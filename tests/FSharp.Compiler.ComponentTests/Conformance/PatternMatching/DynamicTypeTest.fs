// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.PatternMatching

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module DynamicTypeTest =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/DynamicTypeTest", Includes=[|"dynamicTypeTest01.fs"|])>]
    let ``DynamicTypeTest - dynamicTypeTest01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/DynamicTypeTest", Includes=[|"dynamicTypeTest02.fs"|])>]
    let ``DynamicTypeTest - dynamicTypeTest02.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/DynamicTypeTest", Includes=[|"dynamicTypeTest03.fs"|])>]
    let ``DynamicTypeTest - dynamicTypeTest03.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/DynamicTypeTest", Includes=[|"dynamicTypeTest04.fs"|])>]
    let ``DynamicTypeTest - dynamicTypeTest04.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    //<Expects id="FS0026" status="warning">This rule will never be matched</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/DynamicTypeTest", Includes=[|"W_RedundantPattern01.fs"|])>]
    let ``DynamicTypeTest - W_RedundantPattern01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 0026
        |> withDiagnosticMessageMatches "This rule will never be matched"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    //<Expects id="FS0067" status="warning">This type test or downcast will always hold</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/DynamicTypeTest", Includes=[|"W_TypeTestWillAlwaysHold01.fs"|])>]
    let ``DynamicTypeTest - W_TypeTestWillAlwaysHold01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 0067
        |> withDiagnosticMessageMatches "This type test or downcast will always hold"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/DynamicTypeTest", Includes=[|"ArrayTypeTest01.fs"|])>]
    let ``DynamicTypeTest - ArrayTypeTest01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/DynamicTypeTest", Includes=[|"dynTestSealedType01.fs"|])>]
    let ``DynamicTypeTest - dynTestSealedType01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    //<Expects id="FS0016" status="error">The type 'int' does not have any proper subtypes and cannot be used as the source of a type test or runtime coercion</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/DynamicTypeTest", Includes=[|"E_DynamicTestPrimType01.fs"|])>]
    let ``DynamicTypeTest - E_DynamicTestPrimType01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0016
        |> withDiagnosticMessageMatches "The type 'int' does not have any proper subtypes and cannot be used as the source of a type test or runtime coercion"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/DynamicTypeTest", Includes=[|"genericType01.fs"|])>]
    let ``DynamicTypeTest - genericType01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    //<Expects id="FS0008" status="error">This runtime coercion or type test from type</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/DynamicTypeTest", Includes=[|"E_DynamTyTestVarType01.fs"|])>]
    let ``DynamicTypeTest - E_DynamTyTestVarType01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withErrorCode 0008
        |> withDiagnosticMessageMatches "This runtime coercion or type test from type"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/PatternMatching/DynamicTypeTest", Includes=[|"Regression01.fs"|])>]
    let ``DynamicTypeTest - Regression01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

