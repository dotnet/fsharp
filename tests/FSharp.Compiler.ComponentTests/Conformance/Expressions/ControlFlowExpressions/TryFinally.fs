// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.Expressions.ControlFlowExpressions

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module TryFinally =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/Expressions/ControlFlowExpressions/TryFinally)
    //<Expects id="FS0020" status="warning">The result of this expression has type 'bool' and is implicitly ignored</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../../resources/tests/Conformance/Expressions/ControlFlowExpressions/TryFinally", Includes=[|"W-TryFinallyNotUnit.fs"|])>]
    let ``TryFinally - W-TryFinallyNotUnit.fs - `` compilation =
        compilation
        |> typecheck
        |> shouldFail
        |> withWarningCode 0020
        |> withDiagnosticMessageMatches "The result of this expression has type 'bool' and is implicitly ignored"

