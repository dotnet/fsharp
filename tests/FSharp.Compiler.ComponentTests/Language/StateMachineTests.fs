// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Language

open Xunit
open FSharp.Test.Compiler

module StateMachineTests =

    let verify3511AndRun code = 
        Fsx code
        |> withNoOptimize
        |> compile
        |> shouldFail
        |> withWarningCode 3511
        |> ignore

        Fsx code
        |> withNoOptimize
        |> withOptions ["--nowarn:3511"]
        |> compileExeAndRun

    [<Fact>] // https://github.com/dotnet/fsharp/issues/13067
    let ``Local function with a flexible type``() = 
        """
task {
    let m1 f s = Seq.map f s
    do! Async.Sleep 1
    do! System.Threading.Tasks.Task.Delay 1

    let m2 f (s: #seq<_>) = Seq.map f s
    do! Async.Sleep 1
    do! System.Threading.Tasks.Task.Delay 1

    return 1
}
|> fun f -> f.Wait()
"""
        |> verify3511AndRun
        |> shouldSucceed

    [<Fact>] // https://github.com/dotnet/fsharp/issues/14806
    let ``Explicit returns types + constraints on generics``() = 
        """
module Foo

open System.Threading.Tasks

let run2(): Task =
    task {
        return ()
    }

let run() =
    task {
        let a = null
        do! run2()
    }

run()
|> fun f -> f.Wait()
"""
        |> verify3511AndRun
        |> shouldSucceed
        

    [<Fact>] // https://github.com/dotnet/fsharp/issues/14807
    let ``let _ = null``() = 
        """
module TestProject1

let bar() = task {
    let! _ = async { return [| 1 |] } |> Async.StartAsTask
    ()
}

let foo() = task {
    let _ = null
    do! bar()
}

foo()
|> fun f -> f.Wait()
"""
        |> verify3511AndRun
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>] // https://github.com/dotnet/fsharp/issues/13386
    let ``SkipLocalsInit does not cause an exception``() =
        FSharp """
module TestProject1

[<System.Runtime.CompilerServices.SkipLocalsInit>]
let compute () =
    task {
        try
            do! System.Threading.Tasks.Task.Delay 10
        with e ->
            printfn "%s" (e.ToString())
    }

// multiple invocations to trigger tiered compilation
for i in 1 .. 100 do
    compute().Wait ()
"""
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>] // https://github.com/dotnet/fsharp/issues/12839#issuecomment-1292310944
    let ``Tasks with a for loop over tuples are statically compilable``() =
        FSharp """
module TestProject1

let ret i = task { return i }

let one (f: seq<string * string * int>) = task {
    let mutable sum = 0

    let! x = ret 1
    sum <- sum + x

    for name, _whatever, i in f do
        let! x = ret i
        sum <- sum + x

        System.Console.Write name

        let! x = ret i
        sum <- sum + x

    let! x = ret 1
    sum <- sum + x

    return sum
}

let two (f: seq<string * string * int>) = task {
    let mutable sum = 0

    let! x = ret 1
    sum <- sum + x

    for name, _whatever, i in f do
        let! x = ret i
        sum <- sum + x

        System.Console.Write name

    let! x = ret 1
    sum <- sum + x

    return sum
}

let three (f: seq<string * string * int>) = task {
    let mutable sum = 0

    let! x = ret 1
    sum <- sum + x

    for name, _whatever, i in f do
        let! x = ret i
        sum <- sum + x

        System.Console.Write name

    return sum
}

let four (f: seq<string * int>) = task {
    let mutable sum = 0

    let! x = ret 5
    sum <- sum + x

    for name, _i in f do
        System.Console.Write name

    let! x = ret 1
    sum <- sum + x

    return sum
}

if (one [ ("", "", 1); ("", "", 2) ]).Result <> 8 then
    failwith "unexpected result one"
if (one []).Result <> 2 then
    failwith "unexpected result one"
if (two [ ("", "", 2) ]).Result <> 4 then
    failwith "unexpected result two"
if (three [ ("", "", 5) ]).Result <> 6 then
    failwith "unexpected result three"
if (four [ ("", 10) ]).Result <> 6 then
    failwith "unexpected result four"
"""
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed