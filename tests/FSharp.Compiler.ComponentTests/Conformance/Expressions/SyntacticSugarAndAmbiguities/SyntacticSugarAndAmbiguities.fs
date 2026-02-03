// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module SyntacticSugarAndAmbiguities =

    // Migrated from: tests/fsharpqa/Source/Conformance/Expressions/SyntacticSugarAndAmbiguities
    // Test count: 1

    // SOURCE=SyntacticSugar01.fs - Verify e1.[e2] is just syntactic sugar for calling the 'item' property
    [<Theory; FileInlineData("SyntacticSugar01.fs")>]
    let ``SyntacticSugar01_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldSucceed
