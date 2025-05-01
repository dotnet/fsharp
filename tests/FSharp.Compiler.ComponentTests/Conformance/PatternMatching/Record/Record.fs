// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Record =
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Record)
    [<Theory; FileInlineData("E_RecordFieldNotDefined01.fs")>]
    let ``Record - E_RecordFieldNotDefined01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 39, Line 9, Col 13, Line 9, Col 14, "The record label 'X' is not defined.")
            (Error 39, Line 9, Col 20, Line 9, Col 21, "The record label 'Y' is not defined.")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Record)
    [<Theory; FileInlineData("E_RecTypesNotMatch01.fs")>]
    let ``Record - E_RecTypesNotMatch01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1129, Line 11, Col 13, Line 11, Col 14, "The record type 'R1' does not contain a label 'A'.")
            (Error 1129, Line 11, Col 20, Line 11, Col 21, "The record type 'R1' does not contain a label 'B'.")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Record)
    [<Theory; FileInlineData("E_SyntaxError01.fs")>]
    let ``Record - E_SyntaxError01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 10, Line 9, Col 14, Line 9, Col 15, "Unexpected symbol '}' in pattern. Expected '.', '=' or other token.")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Record)
    [<Theory; FileInlineData("recordPatterns01.fs")>]
    let ``Record - recordPatterns01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Record)
    [<Theory; FileInlineData("recordPatterns02.fs")>]
    let ``Record - recordPatterns02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Record)
    [<Theory; FileInlineData("structRecordPatterns01.fs")>]
    let ``Record - structRecordPatterns01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Record)
    [<Theory; FileInlineData("structRecordPatterns02.fs")>]
    let ``Record - structRecordPatterns02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
    