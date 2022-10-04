// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalFiltering

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module HighPrecedenceApplication =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HighPrecedenceApplication)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"RangeOperator01.fs"|])>]
    let ``RangeOperator01_fs`` compilation =
        compilation
        |> asFs
        |> withOptions ["-a"]
        |> compile
        |> shouldSucceed
        |> ignore

