// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ApplicationExpressions_Assertion =

    // Migrated from: tests/fsharpqa/Source/Conformance/Expressions/ApplicationExpressions/Assertion
    // Test count: 3

    // SOURCE=Assert_true.fs
    [<Theory; FileInlineData("Assert_true.fs")>]
    let ``Assert_true_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Assert_false_DEBUG.fs SCFLAGS="--define:DEBUG"
    [<Theory; FileInlineData("Assert_false_DEBUG.fs")>]
    let ``Assert_false_DEBUG_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withDefines ["DEBUG"]
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // SOURCE=Assert_false.fs
    [<Theory; FileInlineData("Assert_false.fs")>]
    let ``Assert_false_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed
