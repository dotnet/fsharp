// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.ControlFlowExpressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module PatternMatching =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/PatternMatching)
    //<Expects id="FS0025" span="(7,9-7,17)" status="warning">'Some \(\(_,true\)\)'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_PatternMatchingCounterExample01.fs"|])>]
    let ``W_PatternMatchingCounterExample01_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0025
        |> withDiagnosticMessageMatches "'Some \(\(_,true\)\)'"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/PatternMatching)
    //<Expects id="FS0025" span="(7,9-7,17)" status="warning">Incomplete pattern matches on this expression\. For example, the value '\[_;true\]' may indicate a case not covered by the pattern\(s\)</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_PatternMatchingCounterExample02.fs"|])>]
    let ``W_PatternMatchingCounterExample02_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0025
        |> withDiagnosticMessageMatches "Incomplete pattern matches on this expression\. For example, the value '\[_;true\]' may indicate a case not covered by the pattern\(s\)"
        |> ignore

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/PatternMatching)
    //<Expects id="FS0025" span="(5,9-5,17)" status="warning">Incomplete pattern matches on this expression</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"W_PatternMatchingCounterExample03.fs"|])>]
    let ``W_PatternMatchingCounterExample03_fs`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 0025
        |> withDiagnosticMessageMatches "Incomplete pattern matches on this expression"
        |> ignore

