
// Tests for camelCase functions in module Task and module ValueTask

namespace FSharp.Core.UnitTests.Control

open System
open System.Threading
open System.Threading.Tasks
open Xunit

module TaskModuleFunctionsTests =

#if NETFRAMEWORK // Polyfill for netstandard2.0
    type Task<'T> with member x.IsCompletedSuccessfully = x.Status = TaskStatus.RanToCompletion
    let cancelWithToken (tcs: TaskCompletionSource<'T>) =
        tcs.SetCanceled() // No CT overload available
        CancellationToken.None // so exception won't reference one
#else
    let cancelWithToken (tcs: TaskCompletionSource<'T>) =
        let ct = CancellationToken true
        tcs.SetCanceled ct
        ct
#endif

    [<Fact>]
    let ``Task.result wraps value`` () =
        let t = Task.result 42
        Assert.Equal(42, t.Result)


    [<Fact>]
    let ``Task.map transforms value (sync)`` () =
        let t = Task.result 21 |> Task.map (fun x -> x * 2)
        Assert.True t.IsCompleted
        Assert.Equal(42, t.Result)

    [<Fact>]
    let ``Task.map transforms value (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.map (fun x -> x * 2)
        Assert.False t.IsCompleted
        tcs.SetResult 21
        Assert.Equal(42, t.Result)

    [<Fact>]
    let ``Task.map propagates incoming exception (sync)`` () =
        let t = Task.FromException<int>(Exception "boom") |> Task.map (fun x -> x * 2)
        let! e = Assert.ThrowsAsync<exn>(fun () -> t).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``Task.map propagates incoming exception (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.map (fun x -> x * 2)
        tcs.SetException(Exception "boom")
        let! e = Assert.ThrowsAsync<exn>(fun () -> t).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``Task.map propagates mapper exception as Fault (sync)`` () =
        let t = Task.result () |> Task.map (fun () -> failwith "boom")
        let! e = Assert.ThrowsAsync<exn>(fun () -> t).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``Task.map propagates mapper exception as Fault (async)`` () =
        let tcs = TaskCompletionSource<unit>()
        let t = tcs.Task |> Task.map (fun () -> failwith "boom")
        tcs.SetResult ()
        let! e = Assert.ThrowsAsync<exn>(fun () -> t).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``Task.map propagates Cancellation (sync)`` () =
        let ct = CancellationToken true
        let t = Task.FromCanceled<int>(ct) |> Task.map (fun x -> x * 2)
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled

    [<Fact>]
    let ``Task.map propagates Cancellation (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.map (fun x -> x * 2)
        let ct = cancelWithToken tcs
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled


    [<Fact>]
    let ``Task.bind threads value (sync)`` () =
        let t = Task.result 21 |> Task.bind (fun x -> Task.result (x * 2))
        Assert.True t.IsCompleted
        Assert.Equal(42, t.Result)

    [<Fact>]
    let ``Task.bind threads value (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.bind (fun x -> Task.result (x * 2))
        Assert.False t.IsCompleted
        tcs.SetResult 21
        Assert.Equal(42, t.Result)

    [<Fact>]
    let ``Task.bind propagates incoming exception (sync)`` () =
        let t = Task.FromException<int>(Exception "boom") |> Task.bind (fun x -> Task.result (x * 2))
        let e = Assert.ThrowsAsync<exn>(fun () -> t).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``Task.bind propagates incoming exception (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.bind (fun x -> Task.result (x * 2))
        tcs.SetException(Exception "boom")
        let e = Assert.ThrowsAsync<exn>(fun () -> t).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``Task.bind propagates binder exception as Fault (sync)`` () =
        let t = Task.result () |> Task.bind (fun () -> failwith "boom")
        let e = Assert.ThrowsAsync<exn>(fun () -> t).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``Task.bind propagates binder exception as Fault (async)`` () =
        let tcs = TaskCompletionSource<unit>()
        let t = tcs.Task |> Task.bind (fun () -> failwith "boom")
        tcs.SetResult ()
        let e = Assert.ThrowsAsync<exn>(fun () -> t).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``Task.bind propagates Cancellation (sync)`` () =
        let ct = CancellationToken true
        let t = Task.FromCanceled<int>(ct) |> Task.bind (fun x -> Task.result (x * 2))
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled

    [<Fact>]
    let ``Task.bind propagates Cancellation (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.bind (fun x -> Task.result (x * 2))
        let ct = cancelWithToken tcs
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled


    [<Fact>]
    let ``Task.ignore discards result (sync)`` () : unit =
        let t = Task.result 42 |> Task.ignore<int>
        Assert.True t.IsCompletedSuccessfully
        t.Result : unit

    [<Fact>]
    let ``Task.ignore discards result (async)`` () : unit =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.ignore<int>
        Assert.False t.IsCompleted
        tcs.SetResult 42
        Assert.True t.IsCompletedSuccessfully
        t.Result : unit

    [<Fact>]
    let ``Task.ignore propagates incoming exception (sync)`` () =
        let t = Task.FromException<int>(Exception "boom") |> Task.ignore<int>
        Assert.True t.IsCompleted
        let e = Assert.ThrowsAsync<exn>(fun () -> t).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``Task.ignore propagates incoming exception (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.ignore<int>
        Assert.False t.IsCompleted
        tcs.SetException(Exception "boom")
        let e = Assert.ThrowsAsync<exn>(fun () -> t).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``Task.ignore propagates Cancellation (sync)`` () =
        let ct = CancellationToken true
        let t = Task.FromCanceled<int>(ct) |> Task.ignore<int>
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled

    [<Fact>]
    let ``Task.ignore propagates Cancellation (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.ignore<int>
        let ct = cancelWithToken tcs
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled


    [<Fact>]
    let ``Task.catchWith recovers from exception (sync)`` () =
        let source = Task.FromException<int>(Exception "boom")
        let t = source |> Task.catchWith (fun _ -> -1)
        Assert.Equal(-1, t.Result)

    [<Fact>]
    let ``Task.catchWith recovers from exception (async)`` () : Task =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.catchWith (fun _ -> -1)
        tcs.SetException(Exception "boom")
        task {
            let! result = t
            Assert.Equal(-1, result)
        }

    [<Fact>]
    let ``Task.catchWith passes through success (sync)`` () =
        let source = Task.result 42
        let t = source |> Task.catchWith (fun _ -> -1)
        Assert.Equal(42, t.Result)

    [<Fact>]
    let ``Task.catchWith passes through success (async)`` () : Task =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.catchWith (fun _ -> -1)
        Assert.False t.IsCompleted
        tcs.SetResult 42
        task {
            let! result = t
            Assert.Equal(42, result)
        }

    [<Fact>]
    let ``Task.catchWith propagates Cancellation (sync)`` () =
        let ct = CancellationToken true
        let t = Task.FromCanceled<int>(ct) |> Task.catchWith (fun _ -> -1)
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled

    [<Fact>]
    let ``Task.catchWith propagates Cancellation (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.catchWith (fun _ -> -1)
        let ct = cancelWithToken tcs
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled


    [<Fact>]
    let ``Task.catch returns Ok on success (sync)`` () : unit=
        let t = Task.result 42 |> Task.catch
        Assert.Equal(Ok 42, t.Result)

    [<Fact>]
    let ``Task.catch returns Ok on success (async)`` () : unit =
        let tcs = TaskCompletionSource<int>()
        let t = Task.catch tcs.Task
        tcs.SetResult 42
        Assert.Equal(Ok 42, t.Result)

    [<Fact>]
    let ``Task.catch returns Error on exception (sync)`` () =
        let t = Task.FromException<int>(Exception "boom") |> Task.catch
        match t.Result with
        | Error ex -> Assert.Equal("boom", ex.Message)
        | Ok _ -> failwith "unexpected success"

    [<Fact>]
    let ``Task.catch returns Error on exception (async)`` () : unit =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.catch
        tcs.SetException(Exception "boom")
        match t.Result with
        | Error ex -> Assert.Equal("boom", ex.Message)
        | Ok _ -> failwith "unexpected success"

    [<Fact>]
    let ``Task.catch propagates cancellation (sync)`` () =
        let ct = CancellationToken true
        let t = Task.FromCanceled<int>(ct) |> Task.catch
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled

    [<Fact>]
    let ``Task.catch propagates cancellation (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.catch
        let ct = CancellationToken true
        let ct = cancelWithToken tcs
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled


    [<Fact>]
    let ``Task.empty returns completed unit task`` () =
        let t = Task.empty
        Assert.True t.IsCompletedSuccessfully
        Assert.Equal((), t.Result)


#if NETSTANDARD2_1 || NET
    [<Fact>]
    let ``Task.ofValueTask converts ValueTask`` () =
        let vt = ValueTask<int>(42)
        let t = Task.ofValueTask vt
        Assert.Equal(42, t.Result)

    [<Fact>]
    let ``Task.ofValueTask converts faulted ValueTask`` () =
        let vt = ValueTask<int>(Task.FromException<int>(Exception "boom"))
        let t = Task.ofValueTask vt
        let e = Assert.ThrowsAsync<Exception>(fun () -> t).Result
        Assert.Equal("boom", e.Message)

module ValueTaskModuleFunctionsTests =

    let cancelWithToken (tcs: TaskCompletionSource<'T>) =
        let ct = CancellationToken true
        tcs.SetCanceled ct
        ct

    [<Fact>]
    let ``ValueTask.result wraps value`` () =
        let vt = ValueTask.result 42
        Assert.Equal(42, vt.Result)

    [<Fact>]
    let ``ValueTask.map transforms value (sync)`` () =
        let t = ValueTask.result 21 |> ValueTask.map (fun x -> x * 2)
        Assert.True t.IsCompleted
        Assert.Equal(42, t.Result)

    [<Fact>]
    let ``ValueTask.map transforms value (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.map (fun x -> x * 2)
        Assert.False t.IsCompleted
        tcs.SetResult 21
        Assert.Equal(42, t.Result)

    [<Fact>]
    let ``ValueTask.map propagates incoming exception (sync)`` () =
        let t = ValueTask.FromException<int>(Exception "boom") |> ValueTask.map (fun x -> x * 2)
        task {
            let! e = Assert.ThrowsAnyAsync<Exception>(fun () -> t.AsTask())
            Assert.Equal("boom", e.Message)
        }

    [<Fact>]
    let ``ValueTask.map propagates incoming exception (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.map (fun x -> x * 2)
        tcs.SetException(Exception "boom")
        let e = Assert.ThrowsAsync<exn>(fun () -> t.AsTask()).Result
        Assert.Equal("boom", e.Message)
            
    [<Fact>]
    let ``ValueTask.map propagates mapper exception as Fault (sync)`` () =
        let t = ValueTask.result () |> ValueTask.map (fun () -> failwith "boom")
        task {
            let! e = Assert.ThrowsAnyAsync<Exception>(fun () -> t.AsTask())
            Assert.Equal("boom", e.Message)
        }

    [<Fact>]
    let ``ValueTask.map propagates mapper exception as Fault (async)`` () =
        let tcs = TaskCompletionSource<unit>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.map (fun () -> failwith "boom")
        tcs.SetResult ()
        let e = Assert.ThrowsAsync<exn>(fun () -> t.AsTask()).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``ValueTask.map propagates Cancellation (sync)`` () =
        let ct = CancellationToken true
        let t = Task.FromCanceled<int>(ct) |> ValueTask.ofTask |> ValueTask.map (fun x -> x * 2)
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t.AsTask()).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled

    [<Fact>]
    let ``ValueTask.map propagates Cancellation (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.map (fun x -> x * 2)
        let ct = cancelWithToken tcs
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t.AsTask()).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled


    [<Fact>]
    let ``ValueTask.bind threads value (sync)`` () =
        let t = ValueTask.result 21 |> ValueTask.bind (fun x -> ValueTask.result (x * 2))
        Assert.True t.IsCompleted
        Assert.Equal(42, t.Result)

    [<Fact>]
    let ``ValueTask.bind threads value (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.bind (fun x -> ValueTask.result (x * 2))
        Assert.False t.IsCompleted
        tcs.SetResult 21
        Assert.Equal(42, t.Result)
    
    [<Fact>]
    let ``ValueTask.bind propagates incoming exception (sync)`` () =
        let t = Task.FromException<int>(Exception "boom") |> ValueTask.ofTask |> ValueTask.bind (fun x -> ValueTask.result (x * 2))
        task {
            let! e = Assert.ThrowsAnyAsync<Exception>(fun () -> t.AsTask())
            Assert.Equal("boom", e.Message)
        }

    [<Fact>]
    let ``ValueTask.bind propagates incoming exception (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.bind (fun x -> ValueTask.result (x * 2))
        tcs.SetException(Exception "boom")
        let e = Assert.ThrowsAsync<exn>(fun () -> t.AsTask()).Result
        Assert.Equal("boom", e.Message)
            
    [<Fact>]
    let ``ValueTask.bind propagates binder exception as Fault (sync)`` () =
        let t = ValueTask.result () |> ValueTask.bind (fun () -> failwith "boom")
        let e = Assert.ThrowsAsync<exn>(fun () -> t.AsTask()).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``ValueTask.bind propagates binder exception as Fault (async)`` () =
        let tcs = TaskCompletionSource<unit>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.bind (fun () -> failwith "boom")
        tcs.SetResult ()
        let e = Assert.ThrowsAsync<exn>(fun () -> t.AsTask()).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``ValueTask.bind propagates Cancellation (sync)`` () =
        let ct = CancellationToken true
        let t = Task.FromCanceled<int>(ct) |> ValueTask.ofTask |> ValueTask.bind (fun x -> ValueTask.result (x * 2))
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t.AsTask()).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled

    [<Fact>]
    let ``ValueTask.bind propagates Cancellation (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.bind (fun x -> ValueTask.result (x * 2))
        let ct = cancelWithToken tcs
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t.AsTask()).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled

    
    [<Fact>]
    let ``ValueTask.ignore discards result (sync)`` () : unit =
        let t = ValueTask.result 42 |> ValueTask.ignore<int>
        Assert.True t.IsCompletedSuccessfully
        t.Result : unit

    [<Fact>]
    let ``ValueTask.ignore discards result (async)`` () : unit =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.ignore<int>
        Assert.False t.IsCompleted
        tcs.SetResult 42
        Assert.True t.IsCompletedSuccessfully
        t.Result : unit
        
    [<Fact>]
    let ``ValueTask.ignore propagates incoming exception (sync)`` () =
        let t = Task.FromException<int>(Exception "boom") |> ValueTask.ofTask |> ValueTask.ignore<int>
        Assert.True t.IsCompleted
        let e = Assert.ThrowsAsync<Exception>(fun () -> t.AsTask()).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``ValueTask.ignore propagates incoming exception (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.ignore<int>
        Assert.False t.IsCompleted
        tcs.SetException(Exception "boom")
        let e = Assert.ThrowsAsync<Exception>(fun () -> t.AsTask()).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``ValueTask.ignore propagates Cancellation (sync)`` () =
        let ct = CancellationToken true
        let t = Task.FromCanceled<int>(ct) |> ValueTask.ofTask |> ValueTask.ignore<int>
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t.AsTask()).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled

    [<Fact>]
    let ``ValueTask.ignore propagates Cancellation (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.ignore<int>
        let ct = cancelWithToken tcs
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t.AsTask()).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled

    
    [<Fact>]
    let ``ValueTask.catchWith recovers from exception (sync)`` () =
        let source = Task.FromException<int>(Exception "boom")
        let t = source |> ValueTask.ofTask |> ValueTask.catchWith (fun _ -> -1)
        Assert.Equal(-1, t.Result)

    [<Fact>]
    let ``ValueTask.catchWith recovers from exception (async)`` () : Task =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.catchWith (fun _ -> -1)
        tcs.SetException(Exception "boom")
        task {
            let! result = t
            Assert.Equal(-1, result)
        }

    [<Fact>]
    let ``ValueTask.catchWith passes through success (sync)`` () =
        let source = ValueTask.result 42
        let t = source |> ValueTask.catchWith (fun _ -> -1)
        Assert.Equal(42, t.Result)

    [<Fact>]
    let ``ValueTask.catchWith passes through success (async)`` () : Task =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.catchWith (fun _ -> -1)
        Assert.False t.IsCompleted
        tcs.SetResult 42
        task {
            let! result = t
            Assert.Equal(42, result)
        }

    [<Fact>]
    let ``ValueTask.catchWith propagates Cancellation (sync)`` () =
        let ct = CancellationToken true
        let t = Task.FromCanceled<int>(ct) |> ValueTask.ofTask |> ValueTask.catchWith (fun _ -> -1)
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t.AsTask()).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled

    [<Fact>]
    let ``ValueTask.catchWith propagates Cancellation (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.catchWith (fun _ -> -1)
        let ct = cancelWithToken tcs
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t.AsTask()).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled

    [<Fact>]
    let ``ValueTask.catch returns Ok on success (sync)`` () : unit=
        let t = ValueTask.result 42 |> ValueTask.catch
        Assert.Equal(Ok 42, t.Result)

    [<Fact>]
    let ``ValueTask.catch returns Ok on success (async)`` () : unit =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.catch
        tcs.SetResult 42
        Assert.Equal(Ok 42, t.Result)

    [<Fact>]
    let ``ValueTask.catch returns Error on exception (sync)`` () =
        let t = ValueTask.FromException<int>(Exception "boom") |> ValueTask.catch
        match t.Result with
        | Error ex -> Assert.Equal("boom", ex.Message)
        | Ok _ -> failwith "unexpected success"

    [<Fact>]
    let ``ValueTask.catch returns Error on exception (async)`` () : unit =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.catch
        tcs.SetException(Exception "boom")
        match t.Result with
        | Error ex -> Assert.Equal("boom", ex.Message)
        | Ok _ -> failwith "unexpected success"

    [<Fact>]
    let ``ValueTask.catch propagates cancellation (sync)`` () =
        let ct = CancellationToken true
        let t = Task.FromCanceled<int>(ct) |> ValueTask.ofTask |> ValueTask.catch
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t.AsTask()).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled

    [<Fact>]
    let ``ValueTask.catch propagates cancellation (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> ValueTask.ofTask |> ValueTask.catch
        let ct = CancellationToken true
        let ct = cancelWithToken tcs
        let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t.AsTask()).Result
        Assert.Equal(ct, e.CancellationToken)
        Assert.True t.IsCanceled


    [<Fact>]
    let ``ValueTask.empty returns completed unit value task`` () : unit =
        let vt = ValueTask.empty
        Assert.True vt.IsCompletedSuccessfully
        vt.Result

    [<Fact>]
    let ``ValueTask.ofTask wraps Task`` () =
        let t = Task.FromResult 42
        let vt = ValueTask.ofTask t
        Assert.Equal(42, vt.Result)
        
    let ``ValueTask.ofTask converts faulted Task`` () =
        let t = Task.FromException<int>(Exception "boom")
        let vt = ValueTask.ofTask t
        let e = Assert.ThrowsAsync<Exception>(fun () -> vt.AsTask()).Result
        Assert.Equal("boom", e.Message)
   
#endif
