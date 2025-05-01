// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module PatternMatching =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/PatternMatching)
    
    [<Theory; FileInlineData("W_PatternMatchingCounterExample01.fs")>]
    let ``W_PatternMatchingCounterExample01_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 7, Col 9, Line 7, Col 17, "Incomplete pattern matches on this expression. For example, the value 'Some ((_,true))' may indicate a case not covered by the pattern(s).")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/PatternMatching)
    
    [<Theory; FileInlineData("W_PatternMatchingCounterExample02.fs")>]
    let ``W_PatternMatchingCounterExample02_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 7, Col 9, Line 7, Col 17, "Incomplete pattern matches on this expression. For example, the value '[_;true]' may indicate a case not covered by the pattern(s).")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/PatternMatching)
    
    [<Theory; FileInlineData("W_PatternMatchingCounterExample03.fs")>]
    let ``W_PatternMatchingCounterExample03_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 5, Col 9, Line 5, Col 17, "Incomplete pattern matches on this expression.")
        ]

