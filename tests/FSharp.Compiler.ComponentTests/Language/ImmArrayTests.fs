// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.ComponentTests.Language.ImmArrayTests

open Xunit
open FSharp.Test.Compiler

[<Fact>]
let ``Check creation of a single-element immarray``() =
    FSharp """
module ImmarrayTest

let x = [: 42 :]
    """
    |> compile
    |> shouldSucceed

[<Fact>]
let ``Check creation of an immarray with yields``() =
    FSharp """
module ImmarrayTest

let x = [: for i = 0 to 100 do yield 42 :]
let y = x |> Seq.sum
    """
    |> compile
    |> shouldSucceed