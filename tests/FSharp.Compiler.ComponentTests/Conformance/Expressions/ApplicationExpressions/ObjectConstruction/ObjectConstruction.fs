// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ApplicationExpressions_ObjectConstruction =

    // Migrated from: tests/fsharpqa/Source/Conformance/Expressions/ApplicationExpressions/ObjectConstruction
    // Test count: 2

    // SOURCE=ObjectConstruction01.fs
    [<Theory; FileInlineData("ObjectConstruction01.fs")>]
    let ``ObjectConstruction01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=E_ObjectConstruction01.fs
    // <Expects id="FS0039" status="error">...</Expects>
    [<Theory; FileInlineData("E_ObjectConstruction01.fs")>]
    let ``E_ObjectConstruction01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> typecheck
        |> shouldFail
        |> withErrorCode 39
        |> withDiagnosticMessageMatches "BitArrayEnumeratorSimple"
