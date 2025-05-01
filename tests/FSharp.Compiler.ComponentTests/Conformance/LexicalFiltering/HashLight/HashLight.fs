// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalFiltering

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module HashLight =

    // This test was automatically generated (moved from FSharpQA suite - Conformance/LexicalFiltering/HashLight)
    [<Theory; FileInlineData("IndentationWithComputationExpression01.fs")>]
    let ``IndentationWithComputationExpression01_fs`` compilation =
        compilation
        |> getCompilation
        |> asFsx
        |> withOptions ["--warnaserror+"]
        |> typecheck
        |> shouldSucceed
        |> ignore

