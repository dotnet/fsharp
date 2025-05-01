// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

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

    [<Fact>] // https://github.com/dotnet/fsharp/issues/16068
    let ``Decision tree with 32+ binds with nested expression is not getting splitted and state machine is successfully statically compiles``() = 
        FSharp """
module Testing

let test () =
    task {
        if true then
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            let c = failwith ""
            ()
    }

[<EntryPoint>]
let main _ =
    test () |> ignore
    printfn "Hello, World!"
    0
"""
        |> ignoreWarnings
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed