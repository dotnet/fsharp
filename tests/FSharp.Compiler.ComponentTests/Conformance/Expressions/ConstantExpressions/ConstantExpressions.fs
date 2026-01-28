// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ConstantExpressions =

    // Migrated from: tests/fsharpqa/Source/Conformance/Expressions/ConstantExpressions
    // Test count: 22
    // These are compile-and-run tests verifying constant expression literals for various types

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> compile
        |> shouldSucceed

    let verifyCompileAndRunWithWarnAsError compilation =
        compilation
        |> asExe
        |> withOptions ["--warnaserror+"; "--test:ErrorRanges"]
        |> compile
        |> shouldSucceed

    // SOURCE=bigint.fs SCFLAGS="--warnaserror+ --test:ErrorRanges"
    [<Theory; FileInlineData("bigint.fs")>]
    let ``bigint_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunWithWarnAsError

    // SOURCE=bigint02.fs SCFLAGS="--warnaserror+ --test:ErrorRanges"
    [<Theory; FileInlineData("bigint02.fs")>]
    let ``bigint02_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRunWithWarnAsError

    // SOURCE=bool.fs
    [<Theory; FileInlineData("bool.fs")>]
    let ``bool_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=byte.fs
    [<Theory; FileInlineData("byte.fs")>]
    let ``byte_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=byteArr.fs
    [<Theory; FileInlineData("byteArr.fs")>]
    let ``byteArr_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=char.fs
    [<Theory; FileInlineData("char.fs")>]
    let ``char_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=double.fs
    [<Theory; FileInlineData("double.fs")>]
    let ``double_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=float.fs
    [<Theory; FileInlineData("float.fs")>]
    let ``float_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=float32.fs
    [<Theory; FileInlineData("float32.fs")>]
    let ``float32_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=int.fs
    [<Theory; FileInlineData("int.fs")>]
    let ``int_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=int16.fs
    [<Theory; FileInlineData("int16.fs")>]
    let ``int16_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=int32.fs
    [<Theory; FileInlineData("int32.fs")>]
    let ``int32_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=int64.fs
    [<Theory; FileInlineData("int64.fs")>]
    let ``int64_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=nativenint.fs
    [<Theory; FileInlineData("nativenint.fs")>]
    let ``nativenint_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=sbyte.fs
    [<Theory; FileInlineData("sbyte.fs")>]
    let ``sbyte_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=single.fs
    [<Theory; FileInlineData("single.fs")>]
    let ``single_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=string.fs
    [<Theory; FileInlineData("string.fs")>]
    let ``string_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=uint16.fs
    [<Theory; FileInlineData("uint16.fs")>]
    let ``uint16_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=uint32.fs
    [<Theory; FileInlineData("uint32.fs")>]
    let ``uint32_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=uint64.fs
    [<Theory; FileInlineData("uint64.fs")>]
    let ``uint64_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=unativenint.fs
    [<Theory; FileInlineData("unativenint.fs")>]
    let ``unativenint_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun

    // SOURCE=unit.fs
    [<Theory; FileInlineData("unit.fs")>]
    let ``unit_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompileAndRun
