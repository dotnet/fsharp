// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module TupleExpressions =

    // Migrated from: tests/fsharpqa/Source/Conformance/Expressions/DataExpressions/TupleExpressions
    // Test count: 3

    // SOURCE=EqualityDifferentRuntimeType01.fs
    [<Theory; FileInlineData("EqualityDifferentRuntimeType01.fs")>]
    let ``EqualityDifferentRuntimeType01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Tuples01.fs
    [<Theory; FileInlineData("Tuples01.fs")>]
    let ``Tuples01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed

    // SOURCE=Tuples02.fs
    [<Theory; FileInlineData("Tuples02.fs")>]
    let ``Tuples02_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed
