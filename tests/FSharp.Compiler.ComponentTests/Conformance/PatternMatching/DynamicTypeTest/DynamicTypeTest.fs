// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module DynamicTypeTest =
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ArrayTypeTest01.fs"|])>]
    let ``DynamicTypeTest - consPattern01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"dynamicTypeTest01.fs"|])>]
    let ``DynamicTypeTest - dynamicTypeTest01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"dynamicTypeTest02.fs"|])>]
    let ``DynamicTypeTest - dynamicTypeTest02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 26, Line 31, Col 7, Line 31, Col 17, "This rule will never be matched")
            (Warning 26, Line 32, Col 7, Line 32, Col 16, "This rule will never be matched")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"dynamicTypeTest03.fs"|])>]
    let ``DynamicTypeTest - dynamicTypeTest03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"dynamicTypeTest04.fs"|])>]
    let ``DynamicTypeTest - dynamicTypeTest04_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 64, Line 8, Col 7, Line 8, Col 11, "This construct causes code to be less generic than indicated by its type annotations. The type variable implied by the use of a '#', '_' or other type annotation at or near 'dynamicTypeTest04.fs(8,9)-(8,10)' has been constrained to be type 'exn'.")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"dynTestSealedType01.fs"|])>]
    let ``DynamicTypeTest - dynTestSealedType01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_DynamicTestPrimType01.fs"|])>]
    let ``DynamicTypeTest - E_DynamicTestPrimType01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 16, Line 11, Col 7, Line 11, Col 15, "The type 'int' does not have any proper subtypes and cannot be used as the source of a type test or runtime coercion.")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_DynamTyTestVarType01.fs"|])>]
    let ``DynamicTypeTest - E_DynamTyTestVarType01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 8, Line 12, Col 7, Line 12, Col 13, "This runtime coercion or type test from type
    'a    
 to 
    obj    
involves an indeterminate type based on information prior to this program point. Runtime type tests are not allowed on some types. Further type annotations are needed.")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"genericType01.fs"|])>]
    let ``DynamicTypeTest - genericType01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Regression01.fs"|])>]
    let ``DynamicTypeTest - Regression01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Regression02.fs"|])>]
    let ``DynamicTypeTest - Regression02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"TwoAtOnce01.fs"|])>]
    let ``DynamicTypeTest - TwoAtOnce01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_RedundantPattern01.fs"|])>]
    let ``DynamicTypeTest - W_RedundantPattern01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 26, Line 12, Col 7, Line 12, Col 16, "This rule will never be matched")
            (Warning 26, Line 18, Col 7, Line 18, Col 16, "This rule will never be matched")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_TypeTestWillAlwaysHold01.fs"|])>]
    let ``DynamicTypeTest - W_TypeTestWillAlwaysHold01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 67, Line 13, Col 7, Line 13, Col 13, "This type test or downcast will always hold")// This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/DynamicTypeTest)
    
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_DynamicTest01.fs"|])>]
    let ``DynamicTypeTest - E_DynamicTest01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 26, Line 3, Col 3, Line 3, Col 54, "This rule will never be matched")
        ]
    
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_DynamicTest02.fs"|])>]
    let ``DynamicTypeTest - E_DynamicTest02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 26, Line 4, Col 7, Line 4, Col 11, "This rule will never be matched")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_DynamicTest03.fs"|])>]
    let ``DynamicTypeTest - E_DynamicTest03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 26, Line 8, Col 7, Line 8, Col 12, "This rule will never be matched")
        ]
        