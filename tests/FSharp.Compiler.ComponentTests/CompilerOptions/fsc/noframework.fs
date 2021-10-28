// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Xunit.Attributes

module noframework =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/noframework)
    //<Expects status="success"></Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/noframework", Includes=[|"noframework02.fs"|])>]
    let ``noframework - noframework02.fs - --noframework`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--noframework"]
        |> compile
        |> shouldSucceed
        |> ignore

