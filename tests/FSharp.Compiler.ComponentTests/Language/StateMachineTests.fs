// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test.Assert
open FSharp.Test.Compiler


// Inlined helper containing a "if __useResumableCode ..." construct failed to expand correctly,
// executing the dynmamic branch at runtime even when the state machine was compiled statically.
// see https://github.com/dotnet/fsharp/issues/19296
module FailingInlinedHelper =
    open FSharp.Core.CompilerServices
    open FSharp.Core.CompilerServices.StateMachineHelpers
    open System.Runtime.CompilerServices

    let inline MoveOnce(x: byref<'T> when 'T :> IAsyncStateMachine and 'T :> IResumableStateMachine<'Data>) =
        x.MoveNext()
        x.Data

    let inline helper x =
        ResumableCode<int, int>(fun sm ->
            if __useResumableCode then
                sm.Data <- x
                true
            else
                failwith "unexpected dynamic branch at runtime")

    #nowarn 3513 // Resumable code invocation.
    let inline repro x =
        if __useResumableCode then
            __stateMachine<int, int>
                (MoveNextMethodImpl<_>(fun sm -> (helper x).Invoke(&sm) |> ignore))
                (SetStateMachineMethodImpl<_>(fun _ _ -> ()))
                (AfterCode<_, _>(fun sm -> MoveOnce(&sm)))
        else
            failwith "dynamic state machine"
    #warnon 3513

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

    [<Fact>]
    let ``Nested __useResumableCode is expanded correctly`` () =
        FailingInlinedHelper.repro 42
        |> shouldEqual 42

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

    [<Fact>]
    let ``State machine defined as top level value is statically compiled`` () =
        Fsx """
let test = task { return 42 }
if test.Result <> 42 then failwith "expected 42"

task { printfn "Hello, World!"; return 42 }
"""
        |> runFsi
        |> shouldSucceed

    [<Fact>]
    let ``State machine defined as top level has a generated MoveNext method`` () =
        FSharp """
module TestStateMachine
let test = task { return 42 }
"""
        |> compile
        |> verifyIL [ ".override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext" ]

    // The original repro from https://github.com/dotnet/fsharp/pull/14930
    [<Fact>]
    let ``Task with for loop over tuples compiles statically`` () =
        FSharp """
module TestStateMachine
let what (f: seq<string * string>) = task {
    for name, _whatever in f do
        System.Console.Write name
}
    """
        |> compile
        |> verifyIL [ ".override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext" ]

    // The original repro from https://github.com/dotnet/fsharp/issues/12839#issuecomment-2562121004
    [<Fact>]
    let ``Task with for loop over tuples compiles statically 2`` () =
        FSharp """
module TestStateMachine
let test = task {
    for _ in [ "a", "b" ] do
        ()
}
    """
        |> compile
        |> verifyIL [ ".override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext" ]

    // see https://github.com/dotnet/fsharp/pull/14930#issuecomment-1528981395
    [<Fact>]
    let ``Task with some anonymous records`` () =
        FSharp """
module TestStateMachine
let bad () = task {
    let res = {| ResultSet2 = [| {| im = Some 1; lc = 3 |} |] |}

    match [| |] with
    | [| |] ->
        let c = res.ResultSet2 |> Array.map (fun x -> {| Name = x.lc |})
        let c = res.ResultSet2 |> Array.map (fun x -> {| Name = x.lc |})
        let c = res.ResultSet2 |> Array.map (fun x -> {| Name = x.lc |})
        return Some c
    | _ ->
        return None
}
"""
        |> compile
        |> verifyIL [ ".override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext" ]



    // repro of https://github.com/dotnet/fsharp/issues/12839
    [<Fact>]
    let ``Big record`` () =
        FSharp """
module TestStateMachine
type Foo = { X: int option }

type BigRecord =
    {
        a1: string
        a2: string
        a3: string
        a4: string
        a5: string
        a6: string
        a7: string
        a8: string
        a9: string
        a10: string
        a11: string
        a12: string
        a13: string
        a14: string
        a15: string
        a16: string
        a17: string
        a18: string
        a19: string
        a20: string
        a21: string
        a22: string
        a23: string
        a24: string
        a25: string
        a26: string
        a27: string
        a28: string
        a29: string
        a30: string
        a31: string
        a32: string
        a33: string
        a34: string
        a35: string
        a36: string // no warning if at least one field removed

        a37Optional: string option
    }

let testStateMachine (bigRecord: BigRecord) =
    task {
        match Some 5 with // no warn if this match removed and only inner one kept
        | Some _ ->
            match Unchecked.defaultof<Foo>.X with // no warning if replaced with `match Some 5 with`
            | Some _ ->
                let d = { bigRecord with a37Optional = None } // no warning if d renamed as _ or ignore function used
                ()
            | None -> ()
        | _ -> ()
    }
"""
        |> compile
        |> verifyIL [ ".override [runtime]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext" ]


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



