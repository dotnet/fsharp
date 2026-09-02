
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
        | Error ex ->
            Assert.IsType<exn>(ex) |> ignore // We don't want an AggregateException
            Assert.Equal("boom", ex.Message)
        | Ok _ -> failwith "unexpected success"

    [<Fact>]
    let ``Task.catch does not Unwrap an AggregateException with a single inner`` () =
        let inner = exn "inner" 
        let t = Task.FromException<int>(AggregateException("boom", inner)) |> Task.catch
        match t.Result with
        | Error (:? AggregateException as ex) ->
            Assert.Equal(1, ex.InnerExceptions.Count)
            // on net48, Message renders as "boom", on others it's "boom (inner)"
            Assert.True(ex.Message.StartsWith "boom", ex.Message) 
        | x -> failwith $"unexpected %A{x}"

    [<Fact>]
    let ``Task.catch does not Unwrap an AggregateException with multiple inners`` () =
        let inner = exn "inner" 
        let t = Task.FromException<int>(AggregateException("boom", inner, inner)) |> Task.catch
        match t.Result with
        | Error (:? AggregateException as ex) ->
            Assert.Equal(2, ex.InnerExceptions.Count)
            // on net48, Message renders as "boom", on others it's "boom (inner) (inner)"
            Assert.True(ex.Message.StartsWith "boom", ex.Message) 
        | x -> failwith $"unexpected %A{x}"

    [<Fact>]
    let ``Task.catch returns Error on exception (async)`` () : unit =
        let tcs = TaskCompletionSource<int>()
        let t = tcs.Task |> Task.catch
        tcs.SetException(Exception "boom")
        match t.Result with
        | Error ex ->
            Assert.IsType<exn>(ex) |> ignore // We don't want an AggregateException
            Assert.Equal("boom", ex.Message)
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


    [<Fact>]
    let ``Task.sequential runs all tasks in order and collects results`` () : Task =
        task {
            use cts = new CancellationTokenSource()
            let order = ResizeArray()
            let! results =
                [for i in 1..5 do
                    fun (ct: CancellationToken) ->
                        Assert.Equal(cts.Token, ct)
                        task {
                            order.Add i
                            return i * i
                        }]
                |> Task.sequential cts.Token
            Assert.Equal([| 1; 4; 9; 16; 25 |], results)
            Assert.Equal<int seq>([ 1; 2; 3; 4; 5 ], order)
        }

    [<Fact>]
    let ``Task.sequential runs computations one at a time`` () : Task =
        task {
            use cts = new CancellationTokenSource()
            let mutable concurrent = 0
            let mutable maxConcurrent = 0
            let computations =
                [for _ in 1..5 ->
                    fun (_: CancellationToken) ->
                        task {
                            let n = Interlocked.Increment &concurrent
                            if n > maxConcurrent then maxConcurrent <- n
                            do! Task.Delay(1)
                            Interlocked.Decrement &concurrent |> ignore
                            return n
                        }]
            let! _ = Task.sequential cts.Token computations
            Assert.Equal(1, maxConcurrent)
        }


    [<Fact>]
    let ``Task.sequentialDo runs all tasks in order and returns unit`` () : Task =
        task {
            use cts = new CancellationTokenSource()
            let order = ResizeArray()
            let computations =
                [for i in 1..5 do
                    fun (ct: CancellationToken) ->
                        Assert.Equal(cts.Token, ct)
                        task { order.Add i }]
            do! Task.sequentialDo cts.Token computations
            Assert.Equal<int seq>([ 1; 2; 3; 4; 5 ], order)
        }

    [<Fact>]
    let ``Task.sequentialDo runs computations one at a time`` () : Task =
        task {
            use cts = new CancellationTokenSource()
            let mutable concurrent = 0
            let mutable maxConcurrent = 0
            let computations =
                [for _ in 1..5 ->
                    fun (_: CancellationToken) ->
                        task {
                            let n = Interlocked.Increment &concurrent
                            if n > maxConcurrent then maxConcurrent <- n
                            do! Task.Delay(1)
                            Interlocked.Decrement &concurrent |> ignore
                        }]
            do! Task.sequentialDo cts.Token computations
            Assert.Equal(1, maxConcurrent)
        }

    
    [<Fact>]
    let ``Task.parallelLimit runs all tasks and collects results`` () : Task =
        task {
            use cts = new CancellationTokenSource()
            let! results =
                [for i in 1..5 do fun (ct: CancellationToken) ->
                    Assert.NotEqual(cts.Token, ct)
                    Task.result (i * i)]
                |> Task.parallelLimit 2 cts.Token
            Assert.Equal([| 1; 4; 9; 16; 25 |], results)
        }

    [<Fact>]
    let ``Task.parallelLimit limits concurrency`` () : Task =
        task {
            use cts = new CancellationTokenSource()
            let mutable concurrent = 0
            let mutable maxConcurrent = 0
            let lockObj = obj()
            let computations =
                [for _ in 1..10 ->
                    fun (ct: CancellationToken) ->
                        Assert.NotEqual(cts.Token, ct)
                        task {
                            let n =
                                lock lockObj (fun () ->
                                    concurrent <- concurrent + 1
                                    if concurrent > maxConcurrent then maxConcurrent <- concurrent
                                    concurrent)
                            do! Task.Delay(1)
                            lock lockObj (fun () -> concurrent <- concurrent - 1)
                            return n
                        }]
            let! _ = Task.parallelLimit 3 cts.Token computations
            Assert.True(maxConcurrent <= 3, $"max concurrent was {maxConcurrent}, expected <= 3")
        }

    [<Fact>]
    let ``Task.parallelLimit raises ArgumentException for non-positive maxDegreeOfParallelism`` () =
        let ex =
            Assert.Throws<ArgumentException>(fun () ->
                [ fun (_: CancellationToken) -> Task.result 1 ]
                |> Task.parallelLimit 0 CancellationToken.None
                |> ignore<Task<int[]>>)
        Assert.Equal("maxDegreeOfParallelism", ex.ParamName)

    [<Fact>]
    let ``Task.parallelLimit passes a linked CancellationToken to computations when DOP > 1 and > 1 computation`` () : Task =
        task {
            use cts = new CancellationTokenSource()
            let started = TaskCompletionSource<CancellationToken>()

            let t =
                [ fun (ct: CancellationToken) ->
                    task {
                        started.TrySetResult ct |> ignore
                        do! Task.Delay(30_000, ct)
                        return 1
                    }
                  fun (ct: CancellationToken) ->
                    task {
                        started.TrySetResult ct |> ignore
                        do! Task.Delay(30_000, ct)
                        return 2
                    }]
                |> Task.parallelLimit 2 cts.Token

            let! childCt = started.Task
            Assert.NotEqual(cts.Token, childCt)
            cts.Cancel()
            let e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t :> Task).Result
            Assert.NotEqual(cts.Token, e.CancellationToken)
            Assert.True childCt.IsCancellationRequested
        }

    [<Theory; InlineData true; InlineData false>]
    let ``Task.parallelLimit throws TaskCanceledException when ct is already cancelled`` empty : Task =
        task {
            let work =
                if empty then []
                // Guard against any regressions if guard removed and e.g. body yields zeroCreate'd results
                else [ fun (_: CancellationToken) -> Task.result 1
                       fun (_: CancellationToken) -> Task.result 2 ]

            use cts = new CancellationTokenSource()
            cts.Cancel()
            let t = work |> Task.parallelLimit 2 cts.Token

            let! e = Assert.ThrowsAsync<TaskCanceledException>(fun () -> t :> Task)
            Assert.Equal(cts.Token, e.CancellationToken)
        }


    [<Fact>]
    let ``Task.parallelLimit throws TaskCanceledException when ct is cancelled, even if work does not honor it`` () : Task = task {
        let waitForChildStarted = TaskCompletionSource<unit>()
        let waitForUnpause = TaskCompletionSource<unit>()
        // Note DOP 1 or single computation are treated as sequential
        // That implies they are passed the outer CancellationToken and entrusted to do the right thing
        // Hence these more complex semantics would not apply without a second computation and DOP > 1
        let work = seq {
            fun (ct: CancellationToken) -> task {
                waitForChildStarted.SetResult ()
                do! waitForUnpause.Task
                // Validate we heard about the cancellation
                Assert.True(ct.IsCancellationRequested)
                // ... but don't honor it (... but sibling workers may do so they should not rely on us to ThrowIfCancellationRequested etc)
                return 1
            }
            fun (_: CancellationToken) -> Task.result 2
        }

        use cts = new CancellationTokenSource()
        let t = work |> Task.parallelLimit 2 cts.Token
        do! waitForChildStarted.Task
        cts.Cancel()
        waitForUnpause.SetResult()

        do! Assert.ThrowsAsync<OperationCanceledException>(fun () -> t :> Task) |> Task.ignore<OperationCanceledException>
    }

    [<Fact>]
    let ``Task.parallelLimit cancels sibling computations when one fails`` () : Task =
        task {
            use cts = new CancellationTokenSource()
            let siblingStarted = TaskCompletionSource<unit>()
            let siblingCancelled = TaskCompletionSource<unit>()

            let t =
                [ fun (ct: CancellationToken) ->
                    task {
                        siblingStarted.SetResult ()
                        try
                            do! Task.Delay(30_000, ct)
                            return 1
                        with
                        | :? OperationCanceledException when ct.IsCancellationRequested ->
                            siblingCancelled.SetResult ()
                            return! Task.FromCanceled<int>(ct)
                    }
                  fun (_: CancellationToken) ->
                    task {
                        do! siblingStarted.Task
                        return invalidOp "boom"
                    } ]
                |> Task.parallelLimit 2 cts.Token

            let! ex = Assert.ThrowsAsync<InvalidOperationException>(fun () -> t :> Task)
            Assert.Equal("boom", ex.Message)
            let! completed = Task.WhenAny(siblingCancelled.Task :> Task, Task.Delay(30_000))
            Assert.Same(siblingCancelled.Task :> Task, completed)
        }

    [<Fact>]
    let ``Task.parallelLimit with one failure throws the exception directly`` () : Task =
        task {
            use cts = new CancellationTokenSource()
            let t =
                [ fun (_: CancellationToken) -> Task.result 1
                  fun (_: CancellationToken) -> Task.FromException<int>(InvalidOperationException "boom") ]
                |> Task.parallelLimit 2 cts.Token

            let! ex = Assert.ThrowsAsync<InvalidOperationException>(fun () -> t :> Task)
            Assert.Equal("boom", ex.Message)
        }

    [<Fact>]
    let ``Task.parallelLimit with multiple failures yields single exception, not AggregateException`` () : Task =
        task {
            let firstStarted, secondStarted = TaskCompletionSource<unit>(), TaskCompletionSource<unit>()
            let releaseBoth = TaskCompletionSource<unit>()

            let sut = Task.parallelLimit 2 CancellationToken.None [
                fun (_: CancellationToken) ->
                    task {
                        firstStarted.SetResult ()
                        do! releaseBoth.Task
                        return invalidOp "boom1"
                    }
                fun (_: CancellationToken) ->
                    task {
                        secondStarted.SetResult ()
                        do! releaseBoth.Task
                        return invalidArg "a" "boom2"
                    } ]
            let! _ = Task.WhenAll(firstStarted.Task, secondStarted.Task)
            releaseBoth.SetResult ()

            match! Task.catch sut with
            | Error (:? InvalidOperationException) | Error (:? ArgumentException) -> ()  // either sibling may win
            | Error (:? AggregateException) -> failwith "should be a single exception, not an AggregateException"
            | x -> failwith $"unexpected %A{x}"
        }

    [<Fact>]
    let ``Task.parallelDoLimit runs all tasks and returns unit`` () : Task =
        task {
            use cts = new CancellationTokenSource()
            let mutable count = 0
            let computations =
                [for _ in 1..5 ->
                    fun (ct: CancellationToken) ->
                        Assert.NotEqual(cts.Token, ct)
                        task { Interlocked.Increment &count |> ignore }]
            do! Task.parallelDoLimit 2 cts.Token computations
            Assert.Equal(5, count)
        }

    [<Fact>]
    let ``Task.startAsyncImmediate flows result`` () : Task =
        task {
            use cts = new CancellationTokenSource()
            let! result = Async.result 42 |> Task.startAsyncImmediate cts.Token
            Assert.Equal(42, result)
        }

    [<Fact>]
    let ``Task.startAsyncImmediate flows CancellationToken`` () : Task =
        task {
            use cts = new CancellationTokenSource()
            let! capturedCt = Async.CancellationToken |> Task.startAsyncImmediate cts.Token 
            Assert.Equal(cts.Token, capturedCt)
        }

    [<Fact>]
    let ``Task.startAsyncImmediate cancellation cancels the task`` () =
        use cts = new CancellationTokenSource()
        let t =
            async { do! Async.Sleep(30_000) }
            |> Task.startAsyncImmediate cts.Token
        cts.Cancel()
        Assert.ThrowsAsync<TaskCanceledException>(fun () -> t :> Task).Result |> ignore
        Assert.True t.IsCanceled

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
