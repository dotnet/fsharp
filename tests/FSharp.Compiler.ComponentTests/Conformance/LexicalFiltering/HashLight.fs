// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Conformance.LexicalFiltering

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module HashLight =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HashLight)
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalFiltering/HashLight", Includes=[|"IndentationWithComputationExpression01.fs"|])>]
    let ``HashLight - IndentationWithComputationExpression01.fs - --warnaserror+`` compilation =
        compilation
        |> asFsx
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed
        |> ignore

