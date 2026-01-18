// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Noframework =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/noframework)
    //<Expects status="success"></Expects>
    [<Theory; FileInlineData("noframework02.fs")>]
    let ``noframework - noframework02_fs - --noframework`` compilation =
        compilation
        |> getCompilation 
        |> asFsx
        |> withOptions ["--noframework"]
        |> compile
        |> shouldSucceed
        |> ignore

