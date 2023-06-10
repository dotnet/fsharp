// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ElseIfRegression =

    [<Fact>]
    let ``if 1 then () else if 42   -> this should fail parsing already`` () =
        Fsx """if 1 then () else if 42"""
        |> parse
        |> shouldFail
        |> withErrorCode 3562

    [<Fact>]
    let ``if 2 then () elif 42   -> this should fail parsing already`` () =
        Fsx """if 2 then () elif 42"""
        |> parse
        |> shouldFail
        |> withErrorCode 3562

    [<Fact>]
    let ``strange if-then-else fails if it is not EOF`` () =
        Fsx """let y = if false then () else if 42
let x = 15"""
        |> parse
        |> shouldFail
        |> withErrorCode 10

    [<Fact>]
    let ``strange if-then-else fails if it is  EOF`` () =
        Fsx """let y = if false then () else if 42"""
        |> parse
        |> shouldFail
        |> withErrorCode 10





