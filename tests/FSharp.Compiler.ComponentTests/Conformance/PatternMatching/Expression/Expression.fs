// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Expression =
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; FileInlineData("E_CounterExample01.fs")>]
    let ``Expression - E_CounterExample01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail 
        |> withSingleDiagnostic (Warning 25, Line 7, Col 11, Line 7, Col 12, "Incomplete pattern matches on this expression. For example, the value '0' may indicate a case not covered by the pattern(s).")

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; FileInlineData("NoCounterExampleTryWith01.fs")>]
    let ``Expression - NoCounterExampleTryWith01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; FileInlineData("patterns01.fs")>]
    let ``Expression - patterns01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; FileInlineData("patterns02.fs")>]
    let ``Expression - patterns02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Warning 25, Line 6, Col 24, Line 6, Col 25, "Incomplete pattern matches on this expression. For example, the value 'false' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 7, Col 24, Line 7, Col 25, "Incomplete pattern matches on this expression. For example, the value '0' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 8, Col 35, Line 8, Col 36, "Incomplete pattern matches on this expression.")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; FileInlineData("W_CounterExampleWithEnum01.fs")>]
    let ``Expression - W_CounterExampleWithEnum01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail 
        |> withDiagnostics [
            (Warning 25, Line 14, Col 10, Line 14, Col 18, "Incomplete pattern matches on this expression. For example, the value '\"a\"' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 18, Col 10, Line 18, Col 18, "Incomplete pattern matches on this expression. For example, the value '0.0' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 22, Col 10, Line 22, Col 18, "Incomplete pattern matches on this expression. For example, the value '' '' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 26, Col 10, Line 26, Col 18, "Incomplete pattern matches on this expression. For example, the value '1y' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 30, Col 10, Line 30, Col 18, "Incomplete pattern matches on this expression. For example, the value '[_;_;_]' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 34, Col 10, Line 34, Col 18, "Incomplete pattern matches on this expression. For example, the value '[|_; 1|]' may indicate a case not covered by the pattern(s).")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; FileInlineData("W_CounterExampleWithEnum02.fs")>]
    let ``Expression - W_CounterExampleWithEnum02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail 
        |> withDiagnostics [
            (Warning 104, Line 14, Col 10, Line 14, Col 18, "Enums may take values outside known cases. For example, the value 'enum<T> (2)' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 18, Col 10, Line 18, Col 18, "Incomplete pattern matches on this expression. For example, the value 'T.Y' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 21, Col 10, Line 21, Col 18, "Incomplete pattern matches on this expression. For example, the value 'T.Y' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 24, Col 10, Line 24, Col 18, "Incomplete pattern matches on this expression. For example, the value 'T.Y' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 27, Col 10, Line 27, Col 18, "Incomplete pattern matches on this expression. For example, the value 'T.Y' may indicate a case not covered by the pattern(s).")
            (Warning 25, Line 30, Col 10, Line 30, Col 18, "Incomplete pattern matches on this expression. For example, the value 'T.Y' may indicate a case not covered by the pattern(s).")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; FileInlineData("W_whenGuards01.fs")>]
    let ``Expression - W_whenGuards01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail 
        |> withDiagnostics [
            (Warning 25, Line 11, Col 9, Line 11, Col 10, "Incomplete pattern matches on this expression. For example, the value '1' may indicate a case not covered by the pattern(s). However, a pattern rule with a 'when' clause might successfully match this value.")
        ]

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; FileInlineData("whenGuards01.fs")>]
    let ``Expression - whenGuards01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; FileInlineData("whenGuards02.fs")>]
    let ``Expression - whenGuards02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; FileInlineData("whenGuardss01.fs")>]
    let ``Expression - whenGuardss01_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
        
    // This test was automatically generated (moved from FSharpQA suite - Conformance/PatternMatching/Expression)
    [<Theory; FileInlineData("whenGuardss02.fs")>]
    let ``Expression - whenGuardss02_fs - --test:ErrorRanges`` compilation =
        compilation
        |> getCompilation
        |> asFs
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldSucceed
