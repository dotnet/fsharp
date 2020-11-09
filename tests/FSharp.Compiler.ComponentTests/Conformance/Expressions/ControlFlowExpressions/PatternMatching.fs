// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.ControlFlowExpressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module PatternMatching =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/PatternMatching)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ControlFlowExpressions/PatternMatching", Includes=[|"LiteralNull01.fs"|])>]
    let ``PatternMatching - LiteralNull01.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldSucceed

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/PatternMatching)
    //<Expects id="FS0025" span="(8,14-8,24)" status="warning">'0.0'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ControlFlowExpressions/PatternMatching", Includes=[|"W_Function01.fs"|])>]
    let ``PatternMatching - W_Function01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0025
        |> withDiagnosticMessageMatches "'0.0'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/PatternMatching)
    //<Expects id="FS0025" span="(7,9-7,17)" status="warning">'Some \(\(_,true\)\)'</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ControlFlowExpressions/PatternMatching", Includes=[|"W_PatternMatchingCounterExample01.fs"|])>]
    let ``PatternMatching - W_PatternMatchingCounterExample01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0025
        |> withDiagnosticMessageMatches "'Some \(\(_,true\)\)'"

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/PatternMatching)
    //<Expects id="FS0025" span="(7,9-7,17)" status="warning">Incomplete pattern matches on this expression\. For example, the value '\[_;true\]' may indicate a case not covered by the pattern\(s\)</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ControlFlowExpressions/PatternMatching", Includes=[|"W_PatternMatchingCounterExample02.fs"|])>]
    let ``PatternMatching - W_PatternMatchingCounterExample02.fs - --test:ErrorRanges`` compilation =
        compilation
        |> withOptions ["--test:ErrorRanges"]
        |> typecheck
        |> shouldFail
        |> withWarningCode 0025
        |> withDiagnosticMessageMatches "Incomplete pattern matches on this expression\. For example, the value '\[_;true\]' may indicate a case not covered by the pattern\(s\)"

