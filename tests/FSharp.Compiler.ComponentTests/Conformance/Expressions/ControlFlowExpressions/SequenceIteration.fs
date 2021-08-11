// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.ControlFlowExpressions

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Xunit.Attributes

module SequenceIteration =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/SequenceIteration)
    //<Expects id="FS0025" span="(27,20-27,28)" status="warning">Incomplete pattern matches on this expression\. For example, the value 'None' may indicate a case not covered by the pattern\(s\)\.$</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ControlFlowExpressions/SequenceIteration", Includes=[|"W_IncompleteMatchFor01.fs"|])>]
    let ``SequenceIteration - W_IncompleteMatchFor01.fs - --test:ErrorRanges`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--test:ErrorRanges"]
        |> compile
        |> shouldFail
        |> withWarningCode 0025
        |> withDiagnosticMessageMatches "Incomplete pattern matches on this expression\. For example, the value 'None' may indicate a case not covered by the pattern\(s\)\.$"
        |> ignore

