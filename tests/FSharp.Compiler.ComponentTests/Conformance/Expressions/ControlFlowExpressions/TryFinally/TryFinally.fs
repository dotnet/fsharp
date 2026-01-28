// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TryFinally =

    // Migrated from: tests/fsharpqa/Source/Conformance/Expressions/ControlFlowExpressions/TryFinally
    // Test count: 2

    // SOURCE=W-TryFinallyNotUnit.fs
    [<Theory; FileInlineData("W-TryFinallyNotUnit.fs")>]
    let ``W_TryFinallyNotUnit_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail
        |> withWarningCode 20
        |> withDiagnosticMessageMatches "implicitly ignored"

    // SOURCE=TryFinallyInSequence01.fs
    [<Theory; FileInlineData("TryFinallyInSequence01.fs")>]
    let ``TryFinallyInSequence01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed
