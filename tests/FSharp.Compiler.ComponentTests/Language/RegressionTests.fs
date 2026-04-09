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

    // https://github.com/dotnet/fsharp/issues/14152
    [<Fact>]
    let ``Issue 14152 - nowarn directive before module declaration should compile`` () =
        FSharp
            """
#nowarn "20"

module XXX.MyModule

let x = 15
            """
        |> asLibrary
        |> typecheck
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/16007
    [<Fact>]
    let ``Issue 16007 - SRTP ctor constraint should not cause value restriction error`` () =
        FSharp """
type T() = class end
let dosmth (a: T) = System.Console.WriteLine(a.ToString())
let inline NEW () = (^a : (new : unit -> ^a) ())
let x = NEW ()
dosmth x
        """
        |> asLibrary
        |> typecheck
        |> shouldSucceed
