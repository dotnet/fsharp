// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Tests for camelCase functions in module Async

// Intentionally in same collection to help rule out potential flakiness due to concurrency re #20306
[<Xunit.Collection(nameof FSharp.Test.NotThreadSafeResourceCollection)>]
module FSharp.Core.UnitTests.Control.AsyncModuleFunctionsTests

open System
open System.Threading
open System.Threading.Tasks
open Xunit

#if NETFRAMEWORK // Polyfill for netstandard2.0
let cancelWithToken (tcs: TaskCompletionSource<'T>) =
    tcs.SetCanceled() // No CT overload available
    CancellationToken.None // so exception won't reference one
#else
let cancelWithToken (tcs: TaskCompletionSource<'T>) =
    let ct = CancellationToken true
    tcs.SetCanceled ct
    ct
#endif

let asyncWait (a: Async<'T>): 'T = Async.RunSynchronouslyImmediate a
let asyncWaitWithCt (ct: CancellationToken) (a: Async<'T>): 'T = Async.RunSynchronouslyImmediate(a, cancellationToken = ct)

[<Fact>]
let ``Async.result wraps value`` () =
    let actual = Async.result 42 |> asyncWait
    Assert.Equal(42, actual)


[<Fact>]
let ``Async.map transforms value`` () =
    let actual = Async.result 21 |> Async.map (fun x -> x * 2) |> asyncWait
    Assert.Equal(42, actual)

[<Fact>]
let ``Async.map propagates incoming exception`` () =
    let a = async { return failwith "boom" : int } |> Async.map (fun x -> x * 2)
    let e = Assert.Throws<exn>(fun () -> a |> asyncWait |> ignore)
    Assert.Equal("boom", e.Message)

[<Fact>]
let ``Async.map propagates mapper exception as Fault`` () =
    let a = Async.result () |> Async.map (fun () -> failwith "boom")
    let e = Assert.Throws<exn>(fun () -> a |> asyncWait |> ignore)
    Assert.Equal("boom", e.Message)

[<Fact>]
let ``Async.map propagates Cancellation (sync)`` () =
    let ct = CancellationToken true
    let a = Async.result 2 |> Async.map (fun x -> x * 2)
    let e = Assert.Throws<OperationCanceledException>(fun () -> a |> asyncWaitWithCt ct |> ignore)
    Assert.Equal(ct, e.CancellationToken)

[<Fact>]
let ``Async.map propagates Cancellation (async)`` () =
    let mutable mapperWasCalled = false
    let cts = new CancellationTokenSource()
    let a =
        async { do! Async.Sleep 5000 }
        |> Async.map (fun () -> async { mapperWasCalled <- true })
    let t = Async.StartAsTask(a, cancellationToken = cts.Token)
    cts.Cancel()
    let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t).Result
    Assert.NotEqual(cts.Token, e.CancellationToken)
    Assert.False mapperWasCalled


[<Fact>]
let ``Async.bind threads value`` () =
    let actual =
        Async.result 21
        |> Async.bind (fun x -> Async.result (x * 2))
        |> asyncWait
    Assert.Equal(42, actual)

[<Fact>]
let ``Async.bind propagates incoming exception (sync)`` () =
    let a = async { return failwith "boom" } |> Async.bind Async.result
    let e = Assert.Throws<exn>(fun () -> a |> asyncWait |> ignore)
    Assert.Equal("boom", e.Message)

[<Fact>]
let ``Async.bind propagates binder exception as Fault (async)`` () =
    let a = Async.result 5 |> Async.bind (fun x -> async { failwith $"boom {x}"})
    let e = Assert.Throws<exn>(fun () -> asyncWait a)
    Assert.Equal("boom 5", e.Message)

[<Fact>]
let ``Async.bind propagates Cancellation (sync)`` () =
    let ct = CancellationToken true
    let a = Async.result 2 |> Async.bind Async.result
    let e = Assert.Throws<OperationCanceledException>(fun () -> a |> asyncWaitWithCt ct |> ignore)
    Assert.Equal(ct, e.CancellationToken)

[<Fact>]
let ``Async.bind propagates Cancellation (async)`` () =
    let cts = new CancellationTokenSource()
    let mutable binderWasCalled = false
    let a =
        async { do! Async.Sleep 5000 }
        |> Async.bind (fun () -> async { binderWasCalled <- true })
    let t = Async.StartAsTask(a, cancellationToken = cts.Token)
    cts.Cancel()
    let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t).Result
    Assert.NotEqual(cts.Token, e.CancellationToken)
    Assert.False binderWasCalled


[<Fact>]
let ``Async.ignore discards result (sync)`` () =
    let actual = Async.result 42 |> Async.ignore<int> |> asyncWait
    Assert.Equal((), actual)

[<Fact>]
let ``Async.ignore discards result (async)`` () =
    let tcs = TaskCompletionSource<int>()
    let t = async { return! tcs.Task |> Async.AwaitTask } |> Async.ignore<int> |> Async.StartAsTask
    tcs.SetResult 42
    Assert.Equal((), t.Result)

[<Fact>]
let ``Async.ignore propagates incoming exception (sync)`` () =
    let a = async { return failwith "boom" : int } |> Async.ignore<int>
    let e = Assert.Throws<exn>(fun () -> a |> asyncWait)
    Assert.Equal("boom", e.Message)

[<Fact>]
let ``Async.ignore propagates incoming exception (async)`` () =
    let tcs = TaskCompletionSource<int>()
    let t = async { return! tcs.Task |> Async.AwaitTask } |> Async.ignore<int> |> Async.StartAsTask
    tcs.SetException(Exception "boom")
    let e = Assert.ThrowsAsync<AggregateException>(fun () -> t).Result.InnerException
    Assert.Equal("boom", e.Message)

[<Fact>]
let ``Async.ignore propagates Cancellation (sync)`` () =
    let ct = CancellationToken true
    let a = Async.result 2 |> Async.ignore<int>
    let e = Assert.Throws<OperationCanceledException>(fun () -> a |> asyncWaitWithCt ct)
    Assert.Equal(ct, e.CancellationToken)

[<Fact>]
let ``Async.ignore propagates Cancellation (async)`` () =
    let mutable cancellationFailed = false
    let cts = new CancellationTokenSource()
    let a =
        async { do! Async.Sleep 5000
                cancellationFailed <- true
                return 42 }
        |> Async.ignore<int>
    let t = Async.StartAsTask(a, cancellationToken = cts.Token)
    cts.Cancel()
    let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t).Result
    Assert.NotEqual(cts.Token, e.CancellationToken)
    Assert.False cancellationFailed


[<Fact>]
let ``Async.catchWith passes through success (sync)`` () =
    let source = Async.result 42
    let a = source |> Async.catchWith (fun _ -> -1)
    Assert.Equal(42, asyncWait a)

[<Fact>]
let ``Async.catchWith passes through success (async)`` () = async {
    let tcs = TaskCompletionSource<int>()
    let! a = async { return! tcs.Task |> Async.AwaitTask } |> Async.catchWith (fun _ -> -1) |> Async.StartChild
    tcs.SetResult 42
    let! res = a
    Assert.Equal(42, res) }

[<Fact>]
let ``Async.catchWith recovers from exception (sync)`` () = async {
    let! actual =
        async { return failwith "boom" : int }
        |> Async.catchWith (fun e -> Assert.Equal("boom", e.Message); -1)
    Assert.Equal(-1, actual) }

[<Fact>]
let ``Async.catchWith recovers from exception (async)`` () = async {
    let tcs = TaskCompletionSource<int>()
    let! a = async { return! tcs.Task |> Async.AwaitTask } |> Async.catchWith (fun _ -> -1) |> Async.StartChild
    tcs.SetException(Exception "boom")
    let! result = a
    Assert.Equal(-1, result) }

[<Fact>]
let ``Async.catchWith propagates Cancellation (sync)`` () =
    let mutable cancellationFailed = false
    let ct = CancellationToken true
    let a = async { do! Async.Sleep 5000
                    cancellationFailed <- true
                    return 42 }
            |> Async.catchWith (fun _ -> -1)
    let e = Assert.Throws<OperationCanceledException>(fun () -> a |> asyncWaitWithCt ct |> ignore)
    Assert.Equal(ct, e.CancellationToken)
    Assert.False cancellationFailed

[<Fact>]
let ``Async.catchWith propagates Cancellation (async)`` () =
    let mutable cancellationFailed = false
    let cts = new CancellationTokenSource()
    let a =
        async { do! Async.Sleep 5000
                cancellationFailed <- true
                return 42 }
        |> Async.catchWith (fun _ -> -1)
    let t = Async.StartAsTask(a, cancellationToken = cts.Token)
    cts.Cancel()
    let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t).Result
    Assert.NotEqual(cts.Token, e.CancellationToken)


[<Fact>]
let ``Async.catch returns Ok on success (sync)`` () =
    let actual = Async.result 42 |> Async.catch |> asyncWait
    Assert.Equal(Ok 42, actual)

[<Fact>]
let ``Async.catch returns Ok on success (async)`` () : unit =
    let tcs = TaskCompletionSource<int>()
    let t = async { return! tcs.Task |> Async.AwaitTask } |> Async.catch |> Async.StartAsTask
    tcs.SetResult 42
    Assert.Equal(Ok 42, t.Result)

[<Fact>]
let ``Async.catch returns Error on exception`` () =
    let a = async { return failwith "boom" : int } |> Async.catch
    match a |> asyncWait with
    | Error ex -> Assert.Equal("boom", ex.Message)
    | Ok _ -> failwith "unexpected success"

[<Fact>]
let ``Async.catch returns Error on exception (async)`` () : unit =
    let a = async { do! Async.Sleep 1
                    return failwith "boom" } |> Async.catch
    match a |> asyncWait with
    | Error ex -> Assert.Equal("boom", ex.Message)
    | Ok _ -> failwith "unexpected success"

[<Fact>]
let ``Async.catch propagates Cancellation (sync)`` () =
    let ct = CancellationToken true
    let a = async { do! Async.Sleep 5000 } |> Async.catch
    let e = Assert.Throws<OperationCanceledException>(fun () -> a |> asyncWaitWithCt ct |> ignore)
    Assert.Equal(ct, e.CancellationToken)

[<Fact>]
let ``Async.catch propagates Cancellation (async)`` () =
    let cts = new CancellationTokenSource()
    let a = async { do! Async.Sleep 5000 } |> Async.catch
    let t = Async.StartAsTask(a, cancellationToken = cts.Token)
    cts.Cancel()
    let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t).Result
    Assert.NotEqual(cts.Token, e.CancellationToken)


[<Fact>]
let ``Async.empty returns unit`` () =
    let actual = Async.empty |> asyncWait
    Assert.Equal((), actual)
