// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TryWith =

    // Migrated from: tests/fsharpqa/Source/Conformance/Expressions/ControlFlowExpressions/TryWith
    // Test count: 4

    // SOURCE=TryWith01.fs
    [<Theory; FileInlineData("TryWith01.fs")>]
    let ``TryWith01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=TryWith02.fs
    [<Theory; FileInlineData("TryWith02.fs")>]
    let ``TryWith02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=TryWith03.fs
    [<Theory; FileInlineData("TryWith03.fs")>]
    let ``TryWith03_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=E_RethrowOutsideWith01.fs
    [<Theory; FileInlineData("E_RethrowOutsideWith01.fs")>]
    let ``E_RethrowOutsideWith01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 413
        |> withDiagnosticMessageMatches "reraise"
