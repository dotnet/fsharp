// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module RangeExpressions =

    // Migrated from: tests/fsharpqa/Source/Conformance/Expressions/DataExpressions/RangeExpressions
    // Test count: 4

    // SOURCE=FloatingPointRangeExp01.fs
    [<Theory; FileInlineData("FloatingPointRangeExp01.fs")>]
    let ``FloatingPointRangeExp01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // SOURCE=W-FloatingPointRangeExp01.fs - warning test for FS0191
    // Note: Warning 191 may no longer be emitted in modern F#
    [<Theory; FileInlineData("W-FloatingPointRangeExp01.fs")>]
    let ``W_FloatingPointRangeExp01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> ignoreWarnings
        |> compile
        |> shouldSucceed

    // SOURCE=CustomType01.fs
    [<Theory; FileInlineData("CustomType01.fs")>]
    let ``CustomType01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=CustomType02.fs
    [<Theory; FileInlineData("CustomType02.fs")>]
    let ``CustomType02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed
