// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Tuple =
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Tuple)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"SimpleTuples01.fs"|])>]
    let ``Tuple - SimpleTuples01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Tuple)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"tuples01.fs"|])>]
    let ``Tuple - tuples01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"tuples02.fs"|])>]
    let ``Tuple - tuples02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"tuples03.fs"|])>]
    let ``Tuple - tuples03_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            Error 410, Line 6, Col 12, Line 6, Col 14, "The type 'T' is less accessible than the value, member or type 'val t': T' it is used in."
            Error 410, Line 6, Col 9, Line 6, Col 10, "The type 'T' is less accessible than the value, member or type 'val t: T' it is used in."
        ]

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"tuples04.fs"|])>]
    let ``Tuple - tuples04_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            Error 410, Line 6, Col 12, Line 6, Col 14, "The type 'T' is less accessible than the value, member or type 'val internal t': T' it is used in."
            Error 410, Line 6, Col 9, Line 6, Col 10, "The type 'T' is less accessible than the value, member or type 'val internal t: T' it is used in."
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Tuple)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"W_IncompleteMatches01.fs"|])>]
    let ``Tuple - W_IncompleteMatches01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 25, Line 8, Col 5, Line 8, Col 13, "Incomplete pattern matches on this expression. For example, the value '(0,1)' may indicate a case not covered by the pattern(s).")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Tuple)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"W_RedundantPattern01.fs"|])>]
    let ``Tuple - W_RedundantPattern01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 26, Line 11, Col 7, Line 11, Col 14, "This rule will never be matched")
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Tuple)
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes = [|"W_RedundantPattern02.fs"|])>]
    let ``Tuple - W_RedundantPattern02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 26, Line 12, Col 28, Line 12, Col 29, "This rule will never be matched")
        