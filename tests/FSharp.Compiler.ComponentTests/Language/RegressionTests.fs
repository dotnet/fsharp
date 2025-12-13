// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test.Compiler

module RegressionTests =

    [<Fact>]
    let ``No internal errors should be raised``() =
        FSharp """
namespace FSharpBug

type TestItemSeq = 
    static member Test1 item = item
    static member Test2 item = match item with Typo2 x -> x
        """
        |> compile
        |> withErrorCodes [39]
        |> ignore

    // https://github.com/dotnet/fsharp/issues/19156
    [<Fact>]
    let ``Generic list comprehension with nested lambda should not cause duplicate entry in type index table``() =
        FSharp """
module Test
open System

let f (start: DateTime) (stop: DateTime) (input: (DateTime * 'a) list) =
    [
        for i in start.Ticks .. stop.Ticks ->
            input |> List.where (fun (k, v) -> true)
    ]
        """
        |> compile
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/19156
    [<Fact>]
    let ``Generic array comprehension with nested lambda should not cause duplicate entry in type index table``() =
        FSharp """
module Test
open System

let f (start: DateTime) (stop: DateTime) (input: (DateTime * 'a) list) =
    [|
        for i in start.Ticks .. stop.Ticks ->
            input |> List.where (fun (k, v) -> true)
    |]
        """
        |> compile
        |> shouldSucceed
        |> ignore
