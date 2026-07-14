
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
        CaCancellationToken.None // so exception won't reference one
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
    let ``Task.map propagates exception (sync)`` () =
        let t = Task.FromException<int>(Exception "boom") |> Task.map (fun x -> x * 2)
        task {
            let! e = Assert.ThrowsAnyAsync<Exception>(fun () -> t)
            Assert.Equal("boom", e.Message)
        }

    [<Fact>]
    let ``Task.map propagates exception (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.map (fun x -> x * 2)
        tcs.SetException(Exception "boom")
        task {
            let! e = Assert.ThrowsAnyAsync<Exception>(fun () -> t)
            Assert.Equal("boom", e.Message)
        }
            
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
    let ``Task.bind propagates exception (sync)`` () =
        let t = Task.FromException<int>(Exception "boom") |> Task.bind (fun x -> Task.result (x * 2))
        task {
            let! e = Assert.ThrowsAnyAsync<Exception>(fun () -> t)
            Assert.Equal("boom", e.Message)
        }

    [<Fact>]
    let ``Task.bind propagates exception (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.bind (fun x -> Task.result (x * 2))
        tcs.SetException(Exception "boom")
        task {
            let! e = Assert.ThrowsAnyAsync<Exception>(fun () -> t)
            Assert.Equal("boom", e.Message)
        }
            
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
    let ``Task.ignore runs the computation (sync)`` () =
        let t = Task.FromException<int>(Exception "boom") |> Task.ignore<int>
        Assert.True t.IsCompleted
        let e = Assert.ThrowsAsync<Exception>(fun () -> t).Result
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``Task.ignore runs the computation (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.ignore<int>
        Assert.False t.IsCompleted
        tcs.SetException(Exception "boom")
        let e = Assert.ThrowsAsync<Exception>(fun () -> t).Result
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

#if NETSTANDARD2_1
    [<Fact>]
    let ``Task.ofValueTask converts value task`` () =
        let vt = ValueTask<int>(42)
        let t = Task.ofValueTask vt
        Assert.Equal(42, t.Result)

module ValueTaskModuleFunctionsTests =

    [<Fact>]
    let ``ValueTask.result wraps value`` () =
        let vt = ValueTask.result 42
        Assert.Equal(42, vt.Result)

    [<Fact>]
    let ``ValueTask.map transforms value (sync)`` () =
        let vt = ValueTask.result 21 |> ValueTask.map (fun x -> x * 2)
        Assert.Equal(42, vt.Result)

    [<Fact>]
    let ``ValueTask.map transforms value (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let vt = ValueTask<int>(tcs.Task) |> ValueTask.map (fun x -> x * 2)
        Assert.False vt.IsCompleted
        tcs.SetResult 21
        Assert.Equal(42, vt.Result)

    [<Fact>]
    let ``ValueTask.bind threads value (sync)`` () =
        let vt = ValueTask.result 21 |> ValueTask.bind (fun x -> ValueTask.result (x * 2))
        Assert.Equal(42, vt.Result)

    [<Fact>]
    let ``ValueTask.bind threads value (async)`` () =
        let tcs = TaskCompletionSource<int>()
        let vt = ValueTask<int>(tcs.Task) |> ValueTask.map (fun x -> ValueTask.result (x * 2))
        Assert.False vt.IsCompleted
        tcs.SetResult 21
        Assert.Equal(42, vt.Result)

    [<Fact>]
    let ``ValueTask.ignore discards result (sync)`` () : unit =
        let vt = ValueTask.result 42 |> ValueTask.ignore<int>
        Assert.True vt.IsCompletedSuccessfully
        vt.Result

    [<Fact>]
    let ``ValueTask.ignore discards result (async)`` () : unit =
        let tcs = TaskCompletionSource<int>()
        let vt = ValueTask<int>(tcs.Task) |> ValueTask.ignore<int>
        Assert.False vt.IsCompleted
        tcs.SetResult 42
        vt.Result

    [<Fact>]
    let ``ValueTask.ignore runs the computation`` () : Task =
        let faulted = ValueTask<int>(Task.FromException<int>(Exception "boom"))
        let vt = faulted |> ValueTask.ignore<int>
        task {
            let! e = Assert.ThrowsAsync<Exception>(fun () -> vt.AsTask())
            Assert.Equal("boom", e.Message)
        }

    [<Fact>]
    let ``ValueTask.ignore runs the computation (async)`` () : Task =
        let tcs = TaskCompletionSource<int>()
        let source = ValueTask<int>(tcs.Task)
        Assert.False source.IsCompleted
        let vt = source |> ValueTask.ignore<int>
        Assert.False vt.IsCompleted
        tcs.SetException(Exception "boom")
        task {
            let! e = Assert.ThrowsAsync<Exception>(fun () -> vt.AsTask())
            Assert.Equal("boom", e.Message)
        }

    [<Fact>]
    let ``ValueTask.catchWith recovers from exception (sync)`` () =
        let source = ValueTask<int>(Task.FromException<int>(Exception "boom"))
        let vt = source |> ValueTask.catchWith (fun _ -> -1)
        Assert.Equal(-1, vt.Result)

    [<Fact>]
    let ``ValueTask.catchWith recovers from exception (async)`` () : Task =
        let tcs = TaskCompletionSource<int>()
        let source = ValueTask<int>(tcs.Task)
        Assert.False source.IsCompleted
        let vt = source |> ValueTask.catchWith (fun _ -> -1)
        Assert.False vt.IsCompleted
        tcs.SetException(Exception "boom")
        task {
            let! result = vt
            Assert.Equal(-1, result)
        }

    [<Fact>]
    let ``ValueTask.catchWith passes through success (sync)`` () =
        let source = ValueTask.result 42
        Assert.True source.IsCompletedSuccessfully
        let vt = source |> ValueTask.catchWith (fun _ -> -1)
        Assert.Equal(42, vt.Result)

    [<Fact>]
    let ``ValueTask.catchWith passes through success (async)`` () : Task =
        let tcs = TaskCompletionSource<int>()
        let source = ValueTask<int>(tcs.Task)
        Assert.False source.IsCompleted
        let vt = source |> ValueTask.catchWith (fun _ -> -1)
        Assert.False vt.IsCompleted
        tcs.SetResult 42
        task {
            let! result = vt
            Assert.Equal(42, result)
        }

    [<Fact>]
    let ``ValueTask.catch returns Ok on success (sync)`` () =
        let source = ValueTask.result 42
        Assert.True source.IsCompletedSuccessfully
        let vt = source |> ValueTask.catch
        Assert.Equal(Ok 42, vt.Result)

    [<Fact>]
    let ``ValueTask.catch returns Ok on success (async)`` () : Task =
        let tcs = TaskCompletionSource<int>()
        let source = ValueTask<int>(tcs.Task)
        Assert.False source.IsCompleted
        let vt = source |> ValueTask.catch
        Assert.False vt.IsCompleted
        tcs.SetResult 42
        task {
            let! result = vt
            Assert.Equal(Ok 42, result)
        }

    [<Fact>]
    let ``ValueTask.catch returns Error on exception (sync)`` () =
        let source = ValueTask<int>(Task.FromException(Exception "boom"))
        let vt = source |> ValueTask.catch
        match vt.Result with
        | Error ex -> Assert.Equal("boom", ex.Message)
        | Ok _ -> failwith "expected Error"

    [<Fact>]
    let ``ValueTask.catch returns Error on exception (async)`` () : Task =
        let tcs = TaskCompletionSource<int>()
        let source = ValueTask<int>(tcs.Task)
        Assert.False source.IsCompleted
        let vt = source |> ValueTask.catch
        Assert.False vt.IsCompleted
        tcs.SetException(Exception "boom")
        task {
            match! vt with
            | Error ex -> Assert.Equal("boom", ex.Message)
            | Ok _ -> failwith "expected Error"
        }

    [<Fact>]
    let ``ValueTask.catch returns Error on cancellation (sync)`` () =
        let source = ValueTask<int>(Task.FromCanceled<int>(CancellationToken(true)))
        Assert.True source.IsCompleted
        let vt = source |> ValueTask.catch
        match vt.Result with
        | Error (:? TaskCanceledException) -> ()
        | r -> failwithf "expected Error(TaskCanceledException) but got %A" r

    [<Fact>]
    let ``ValueTask.catch returns Error on cancellation (async)`` () : Task =
        let tcs = TaskCompletionSource<int>()
        let source = ValueTask<int>(tcs.Task)
        Assert.False source.IsCompleted
        let vt = source |> ValueTask.catch
        Assert.False vt.IsCompleted
        tcs.SetCanceled()
        task {
            match! vt with
            | Error (:? TaskCanceledException) -> ()
            | r -> failwithf "expected Error(TaskCanceledException) but got %A" r
        }

    [<Fact>]
    let ``ValueTask.empty returns completed unit value task`` () : unit =
        let vt = ValueTask.empty
        Assert.True vt.IsCompletedSuccessfully
        vt.Result

    [<Fact>]
    let ``ValueTask.ofTask wraps task`` () =
        let t = Task.FromResult 42
        let vt = ValueTask.ofTask t
        Assert.Equal(42, vt.Result)
#endif
