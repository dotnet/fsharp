// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Union =
     // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_CapturesDiffVal01.fs"|])>]
    let ``Union - E_CapturesDiffVal01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 18, Line 10, Col 7, Line 10, Col 28, "The two sides of this 'or' pattern bind different sets of variables")
            (Error 18, Line 10, Col 7, Line 10, Col 40, "The two sides of this 'or' pattern bind different sets of variables")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_NotAllCaptureSameVal01.fs"|])>]
    let ``Union - E_NotAllCaptureSameVal01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 18, Line 10, Col 7, Line 10, Col 31, "The two sides of this 'or' pattern bind different sets of variables")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"E_UnionCapturesDiffType01.fs"|])>]
    let ``Union - E_UnionCapturesDiffType01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 1, Line 9, Col 10, Line 9, Col 11, "This expression was expected to have type
    'int'    
but here has type
    'float'    ")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"unionPattern01.fs"|])>]
    let ``Union - unionPattern01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"unionPattern02.fs"|])>]
    let ``Union - unionPattern02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"unionPattern03.fs"|])>]
    let ``Union - unionPattern03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 26, Line 12, Col 7, Line 12, Col 17, "This rule will never be matched")
            (Warning 26, Line 21, Col 7, Line 21, Col 17, "This rule will never be matched")
        ]
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Union)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"unionPattern04.fs"|])>]
    let ``Union - unionPattern04_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed