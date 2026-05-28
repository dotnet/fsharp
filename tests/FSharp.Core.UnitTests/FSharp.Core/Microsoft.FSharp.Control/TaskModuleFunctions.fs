// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Tests for camelCase functions in module Task and module ValueTask

namespace FSharp.Core.UnitTests.Control

open System
open System.Threading
open System.Threading.Tasks
open Xunit

module TaskModuleFunctionsTests =

    let pendingTaskSource () = TaskCompletionSource<int>()

    [<Fact>]
    let ``Task.result wraps value`` () =
        let t = Task.result 42
        Assert.Equal(42, t.Result)

    [<Fact>]
    let ``Task.map transforms value`` () =
        let t = Task.result 21 |> Task.map (fun x -> x * 2)
        Assert.Equal(42, t.Result)

    [<Fact>]
    let ``Task.map propagates exception`` () =
        let t = Task.FromException<int>(Exception "boom") |> Task.map (fun x -> x * 2)
        let ex = Assert.Throws<Exception>(fun () -> t.GetAwaiter().GetResult() |> ignore)
        Assert.Equal("boom", ex.Message)

    [<Fact>]
    let ``Task.bind threads value`` () =
        let t = Task.result 21 |> Task.bind (fun x -> Task.result (x * 2))
        Assert.Equal(42, t.Result)

    [<Fact>]
    let ``Task.ignore discards result`` () =
        let t = Task.result 42 |> Task.ignore<int>
        t.Result
        Assert.True(t.IsCompletedSuccessfully)

    [<Fact>]
    let ``Task.catchWith recovers from exception (sync)`` () =
        let source = Task.FromException<int>(Exception "boom")
        Assert.True(source.IsCompleted)
        let t = source |> Task.catchWith (fun _ -> -1)
        Assert.Equal(-1, t.Result)

    [<Fact>]
    let ``Task.catchWith recovers from exception (async)`` () : Task =
        let tcs = pendingTaskSource()
        Assert.False(tcs.Task.IsCompleted)
        let t = tcs.Task |> Task.catchWith (fun _ -> -1)
        Assert.False(t.IsCompleted)
        tcs.SetException(Exception "boom")
        task {
            let! result = t
            Assert.Equal(-1, result)
        }

    [<Fact>]
    let ``Task.catchWith passes through success (sync)`` () =
        let source = Task.result 42
        Assert.True(source.IsCompleted)
        let t = source |> Task.catchWith (fun _ -> -1)
        Assert.Equal(42, t.Result)

    [<Fact>]
    let ``Task.catchWith passes through success (async)`` () : Task =
        let tcs = pendingTaskSource()
        Assert.False(tcs.Task.IsCompleted)
        let t = tcs.Task |> Task.catchWith (fun _ -> -1)
        Assert.False(t.IsCompleted)
        tcs.SetResult(42)
        task {
            let! result = t
            Assert.Equal(42, result)
        }

    [<Fact>]
    let ``Task.catch returns Ok on success (sync)`` () =
        let source = Task.result 42
        Assert.True(source.IsCompleted)
        let t = source |> Task.catch
        Assert.Equal(Ok 42, t.Result)

    [<Fact>]
    let ``Task.catch returns Ok on success (async)`` () : Task =
        let tcs = pendingTaskSource()
        Assert.False(tcs.Task.IsCompleted)
        let t = tcs.Task |> Task.catch
        Assert.False(t.IsCompleted)
        tcs.SetResult(42)
        task {
            let! result = t
            Assert.Equal(Ok 42, result)
        }

    [<Fact>]
    let ``Task.catch returns Error on exception (sync)`` () =
        let source = Task.FromException<int>(Exception "boom")
        Assert.True(source.IsCompleted)
        let t = source |> Task.catch
        match t.Result with
        | Error ex -> Assert.Equal("boom", ex.Message)
        | Ok _ -> failwith "expected Error"

    [<Fact>]
    let ``Task.catch returns Error on exception (async)`` () : Task =
        let tcs = pendingTaskSource()
        Assert.False(tcs.Task.IsCompleted)
        let t = tcs.Task |> Task.catch
        Assert.False(t.IsCompleted)
        tcs.SetException(Exception "boom")
        task {
            let! result = t
            match result with
            | Error ex -> Assert.Equal("boom", ex.Message)
            | Ok _ -> failwith "expected Error"
        }

    [<Fact>]
    let ``Task.catch returns Error on cancellation (sync)`` () =
        let source = Task.FromCanceled<int>(CancellationToken(true))
        Assert.True(source.IsCompleted)
        let t = source |> Task.catch
        match t.Result with
        | Error (:? TaskCanceledException) -> ()
        | r -> failwithf "expected Error(OperationCanceledException) but got %A" r

    [<Fact>]
    let ``Task.catch returns Error on cancellation (async)`` () : Task =
        let tcs = pendingTaskSource()
        Assert.False(tcs.Task.IsCompleted)
        let t = tcs.Task |> Task.catch
        Assert.False(t.IsCompleted)
        tcs.SetCanceled()
        task {
            match! t with
            | Error (:? TaskCanceledException) -> ()
            | r -> failwithf "expected Error(OperationCanceledException) but got %A" r
        }

    [<Fact>]
    let ``Task.ignore runs the computation`` () : Task =
        let t = Task.FromException<int>(Exception "boom") |> Task.ignore<int>
        task {
            let! e = Assert.ThrowsAsync<exn>(fun () -> t)
            Assert.Equal("boom", e.Message)
        }

    [<Fact>]
    let ``Task.ignore runs the computation (async)`` () : Task =
        let tcs = pendingTaskSource()
        Assert.False(tcs.Task.IsCompleted)
        let t = tcs.Task |> Task.ignore<int>
        Assert.False(t.IsCompleted)
        tcs.SetException(Exception "boom")
        task {
            let! e = Assert.ThrowsAsync<exn>(fun () -> t)
            Assert.Equal("boom", e.Message)
        }

    [<Fact>]
    let ``Task.empty returns completed unit task`` () =
        let t = Task.empty
        t.Result
        Assert.True(t.IsCompletedSuccessfully)

#if NETSTANDARD2_1
    [<Fact>]
    let ``Task.ofValueTask converts value task`` () =
        let vt = ValueTask<int>(42)
        let t = Task.ofValueTask vt
        Assert.Equal(42, t.Result)

module ValueTaskModuleFunctionsTests =

    let pendingTaskSource () = TaskCompletionSource<int>()

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
        let vt = ValueTask<int>(Task.FromResult 21) |> ValueTask.map (fun x -> x * 2)
        Assert.Equal(42, vt.Result)

    [<Fact>]
    let ``ValueTask.bind threads value (sync)`` () =
        let vt = ValueTask.result 21 |> ValueTask.bind (fun x -> ValueTask.result (x * 2))
        Assert.Equal(42, vt.Result)

    [<Fact>]
    let ``ValueTask.bind threads value (async)`` () =
        let vt = ValueTask<int>(Task.FromResult 21) |> ValueTask.bind (fun x -> ValueTask.result (x * 2))
        Assert.Equal(42, vt.Result)

    [<Fact>]
    let ``ValueTask.ignore discards result (sync)`` () : unit =
        let vt = ValueTask.result 42 |> ValueTask.ignore<int>
        Assert.True(vt.IsCompletedSuccessfully)
        vt.Result

    [<Fact>]
    let ``ValueTask.ignore discards result (async)`` () : unit =
        let vt = ValueTask<int>(Task.FromResult 42) |> ValueTask.ignore<int>
        vt.Result

    [<Fact>]
    let ``ValueTask.catchWith recovers from exception (sync)`` () =
        let source = ValueTask<int>(Task.FromException<int>(Exception "boom"))
        Assert.True(source.IsCompleted)
        let vt = source |> ValueTask.catchWith (fun _ -> -1)
        Assert.Equal(-1, vt.Result)

    [<Fact>]
    let ``ValueTask.catchWith recovers from exception (async)`` () : Task =
        let tcs = pendingTaskSource()
        let source = ValueTask<int>(tcs.Task)
        Assert.False(source.IsCompletedSuccessfully)
        let vt = source |> ValueTask.catchWith (fun _ -> -1)
        Assert.False(vt.IsCompletedSuccessfully)
        tcs.SetException(Exception "boom")
        task {
            let! result = vt
            Assert.Equal(-1, result)
        }

    [<Fact>]
    let ``ValueTask.catchWith passes through success (sync)`` () =
        let source = ValueTask.result 42
        Assert.True(source.IsCompletedSuccessfully)
        let vt = source |> ValueTask.catchWith (fun _ -> -1)
        Assert.Equal(42, vt.Result)

    [<Fact>]
    let ``ValueTask.catchWith passes through success (async)`` () : Task =
        let tcs = pendingTaskSource()
        let source = ValueTask<int>(tcs.Task)
        Assert.False(source.IsCompletedSuccessfully)
        let vt = source |> ValueTask.catchWith (fun _ -> -1)
        Assert.False(vt.IsCompletedSuccessfully)
        tcs.SetResult(42)
        task {
            let! result = vt
            Assert.Equal(42, result)
        }

    [<Fact>]
    let ``ValueTask.catch returns Ok on success (sync)`` () =
        let source = ValueTask.result 42
        Assert.True(source.IsCompletedSuccessfully)
        let vt = source |> ValueTask.catch
        Assert.Equal(Ok 42, vt.Result)

    [<Fact>]
    let ``ValueTask.catch returns Ok on success (async)`` () : Task =
        let tcs = pendingTaskSource()
        let source = ValueTask<int>(tcs.Task)
        Assert.False(source.IsCompletedSuccessfully)
        let vt = source |> ValueTask.catch
        Assert.False(vt.IsCompletedSuccessfully)
        tcs.SetResult(42)
        task {
            let! result = vt
            Assert.Equal(Ok 42, result)
        }

    [<Fact>]
    let ``ValueTask.catch returns Error on exception (sync)`` () =
        let source = ValueTask<int>(Task.FromException<int>(Exception "boom"))
        Assert.True(source.IsCompleted)
        let vt = source |> ValueTask.catch
        match vt.Result with
        | Error ex -> Assert.Equal("boom", ex.Message)
        | Ok _ -> failwith "expected Error"

    [<Fact>]
    let ``ValueTask.catch returns Error on exception (async)`` () : Task =
        let tcs = pendingTaskSource()
        let source = ValueTask<int>(tcs.Task)
        Assert.False(source.IsCompletedSuccessfully)
        let vt = source |> ValueTask.catch
        Assert.False(vt.IsCompletedSuccessfully)
        tcs.SetException(Exception "boom")
        task {
            let! result = vt
            match result with
            | Error ex -> Assert.Equal("boom", ex.Message)
            | Ok _ -> failwith "expected Error"
        }

    [<Fact>]
    let ``ValueTask.catch returns Error on cancellation (sync)`` () =
        let source = ValueTask<int>(Task.FromCanceled<int>(CancellationToken(true)))
        Assert.True(source.IsCompleted)
        let vt = source |> ValueTask.catch
        match vt.Result with
        | Error (:? TaskCanceledException) -> ()
        | r -> failwithf "expected Error(TaskCanceledException) but got %A" r

    [<Fact>]
    let ``ValueTask.catch returns Error on cancellation (async)`` () : Task =
        let tcs = pendingTaskSource()
        let source = ValueTask<int>(tcs.Task)
        Assert.False(source.IsCompletedSuccessfully)
        let vt = source |> ValueTask.catch
        Assert.False(vt.IsCompletedSuccessfully)
        tcs.SetCanceled()
        task {
            match!  with
            | Error (:? TaskCanceledException) -> ()
            | r -> failwithf "expected Error(TaskCanceledException) but got %A" r
        }

    [<Fact>]
    let ``ValueTask.ignore runs the computation`` () : Task =
        let faulted = ValueTask<int>(Task.FromException<int>(Exception "boom"))
        let vt = faulted |> ValueTask.ignore<int>
        task {
            let! e = Assert.ThrowsAsync<exn>(fun () -> vt.AsTask())
            Assert.Equal("boom", e.Message)
        }

    [<Fact>]
    let ``ValueTask.ignore runs the computation (async)`` () : Task =
        let tcs = pendingTaskSource()
        let source = ValueTask<int>(tcs.Task)
        Assert.False(source.IsCompletedSuccessfully)
        let vt = source |> ValueTask.ignore<int>
        Assert.False(vt.IsCompletedSuccessfully)
        tcs.SetException(Exception "boom")
        task {
            let! e = Assert.ThrowsAsync<exn>(fun () -> vt.AsTask())
            Assert.Equal("boom", e.Message)
        }

    [<Fact>]
    let ``ValueTask.empty returns completed unit value task`` () : unit =
        let vt = ValueTask.empty
        Assert.True(vt.IsCompletedSuccessfully)
        vt.Result

    [<Fact>]
    let ``ValueTask.ofTask wraps task`` () =
        let t = Task.FromResult 42
        let vt = ValueTask.ofTask t
        Assert.Equal(42, vt.Result)
#endif
