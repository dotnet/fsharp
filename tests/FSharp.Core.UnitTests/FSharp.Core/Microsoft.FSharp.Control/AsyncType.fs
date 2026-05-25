// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Control.Async type

namespace FSharp.Core.UnitTests.Control

open System
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit
open System.Threading
open System.Threading.Tasks
open Xunit.Internal

// Cancels default token.
[<Collection(nameof FSharp.Test.NotThreadSafeResourceCollection)>]
module AsyncType =

    type ExpectedContinuation = Success | Exception | Cancellation

    [<Fact>]
    let startWithContinuations() =

        let cont actual expected _ =
            if expected <> actual then
                failwith $"expected {expected} continuation, but ran {actual}"

        let onSuccess = cont Success
        let onException = cont Exception
        let onCancellation = cont Cancellation

        let expect expected computation =
            Async.StartWithContinuations(computation, onSuccess expected, onException expected, onCancellation expected)

        async {
            Async.CancelDefaultToken()
            return () 
        } |> expect Cancellation

        async { failwith "computation failed" } |> expect Exception

        async { return () } |> expect Success

[<Collection(nameof FSharp.Test.NotThreadSafeResourceCollection)>]
type AsyncType() =

    let ignoreSynchCtx f =
        f ()

    [<VolatileField>]
    let mutable spinloop = true
        
    // Use a generous timeout to avoid flaky failures on loaded CI machines where the thread pool may be saturated.
    let waitForCompletion (t: Task) =
        let result = t.Wait(TimeSpan.FromSeconds(30.0))
        Assert.True(result, "Task did not finish after waiting for 30 seconds.")

    [<Fact>]
    member _.AsyncRunSynchronouslyReusesThreadPoolThread() =
        let action _ =
            async {
                return
                    async { return Thread.CurrentThread.ManagedThreadId }
                    |> Async.RunSynchronously
            }
        // This test needs approximately 1000 ThreadPool threads
        // if Async.RunSynchronously doesn't reuse them.
        let usedThreads =
            Seq.init 1000 action
            |> Async.Parallel
            |> Async.RunSynchronously
            |> Set.ofArray
        printfn $"RunSynchronously used {usedThreads.Count} threads. Environment.ProcessorCount is {Environment.ProcessorCount}."
        // Some arbitrary large number but in practice it should not use more threads than there are CPU cores.
        Assert.True(usedThreads.Count < 256, $"RunSynchronously used {usedThreads.Count} threads.")

    [<Theory>]
    [<InlineData("int32")>]
    [<InlineData("timespan")>]
    member _.AsyncSleepCancellation1(sleepType) =
        ignoreSynchCtx (fun () ->
            let computation =
                match sleepType with
                | "int32"    -> Async.Sleep(10000000)
                | "timespan" -> Async.Sleep(10000000.0 |> TimeSpan.FromMilliseconds)
                | unknown    -> raise (NotImplementedException(unknown))
            let mutable result = ""
            use cts = new CancellationTokenSource()
            Async.StartWithContinuations(computation,
                                            (fun _ -> result <- "Ok"),
                                            (fun _ -> result <- "Exception"),
                                            (fun _ -> result <- "Cancel"),
                                            cts.Token)
            cts.Cancel()
            Async.Sleep(1000) |> Async.RunSynchronously
            Assert.AreEqual("Cancel", result)
        )

    [<Theory>]
    [<InlineData("int32")>]
    [<InlineData("timespan")>]
    member _.AsyncSleepCancellation2(sleepType) =
        ignoreSynchCtx (fun () ->
            let computation =
                match sleepType with
                | "int32"    -> Async.Sleep(10)
                | "timespan" -> Async.Sleep(10.0 |> TimeSpan.FromMilliseconds)
                | unknown    -> raise (NotImplementedException(unknown))
            for i in 1..100 do
                let mutable result = ""
                use completedEvent = new ManualResetEvent(false)
                use cts = new CancellationTokenSource()
                Async.StartWithContinuations(computation,
                                                (fun _ -> result <- "Ok"; completedEvent.Set() |> ignore),
                                                (fun _ -> result <- "Exception"; completedEvent.Set() |> ignore),
                                                (fun _ -> result <- "Cancel"; completedEvent.Set() |> ignore),
                                                cts.Token)
                sleep(10)
                cts.Cancel()
                completedEvent.WaitOne() |> Assert.True
                Assert.True(result = "Cancel" || result = "Ok")
        )

    [<Theory>]
    [<InlineData("int32")>]
    [<InlineData("timespan")>]
    member _.AsyncSleepThrowsOnNegativeDueTimes(sleepType) =
        async {
            try
                do! match sleepType with
                    | "int32"    -> Async.Sleep(-100)
                    | "timespan" -> Async.Sleep(-100.0 |> TimeSpan.FromMilliseconds)
                    | unknown    -> raise (NotImplementedException(unknown))
                failwith "Expected ArgumentOutOfRangeException"
            with
            | :? ArgumentOutOfRangeException -> ()
        } |> Async.RunSynchronously

    [<Fact>]
    member _.AsyncSleepInfinitely() =
        ignoreSynchCtx (fun () ->
            let computation = Async.Sleep(System.Threading.Timeout.Infinite)
            let result = TaskCompletionSource<string>()
            use cts = new CancellationTokenSource(TimeSpan.FromSeconds(1.0)) // there's a long way from 1 sec to infinity, but it'll have to do.
            Async.StartWithContinuations(computation,
                                            (fun _ -> result.TrySetResult("Ok")        |> ignore),
                                            (fun _ -> result.TrySetResult("Exception") |> ignore),
                                            (fun _ -> result.TrySetResult("Cancel")    |> ignore),
                                            cts.Token)
            let result = result.Task |> Async.AwaitTask |> Async.RunSynchronously
            Assert.AreEqual("Cancel", result)
        )

    [<Fact>]
    member _.CreateTask () =
        let s = "Hello tasks!"
        let a = async { return s }
        let t : Task<string> = Async.StartAsTask a
        waitForCompletion t
        Assert.True (t.IsCompleted)
        Assert.AreEqual(s, t.Result)

    [<Fact>]
    member _.StartAsTaskCancellation () =
        let cts = new CancellationTokenSource()
        let asyncStarted = new ManualResetEventSlim(false)
        let doSpinloop () = while spinloop do ()
        let a = async {
            asyncStarted.Set()
            cts.CancelAfter (100)
            doSpinloop()
        }

        let t : Task<unit> = Async.StartAsTask(a, cancellationToken = cts.Token)

        // Wait for the async body to actually start executing before checking timing.
        // Use a generous timeout to avoid flaky failures on loaded CI machines where the thread pool may be saturated.
        Assert.True(asyncStarted.Wait(30_000), "Async body did not start within 30 seconds")

        // Should not finish, we don't eagerly mark the task done just because it's been signaled to cancel.
        try
            let result = t.Wait(1000)
            Assert.False (result)
        with :? AggregateException -> Assert.Fail "Task should not finish, yet"

        spinloop <- false

        try
            let result = t.Wait(TimeSpan(hours=0,minutes=0,seconds=5))
            Assert.True(result, "Task did not finish after waiting for 5 seconds.")
        with :? AggregateException as a ->
            match a.InnerException with
            | :? TaskCanceledException -> ()
            | _ -> reraise()

        Assert.True (t.IsCompleted, "Task is not completed")


    [<Fact>]
    member _.``AwaitTask ignores Async cancellation`` () =
        let cts = new CancellationTokenSource()
        let tcs = new TaskCompletionSource<unit>()
        let innerTcs = new TaskCompletionSource<unit>()
        let a = innerTcs.Task |> Async.AwaitTask

        Async.StartWithContinuations(a, tcs.SetResult, tcs.SetException, ignore >> tcs.SetCanceled, cts.Token)

        cts.CancelAfter(100)
        try
            let result = tcs.Task.Wait(300)
            Assert.False (result)
        with :? AggregateException -> Assert.Fail "Should not finish, yet"

        innerTcs.SetResult ()

        try
            waitForCompletion tcs.Task
        with :? AggregateException as a ->
            match a.InnerException with
            | :? TaskCanceledException -> ()
            | _ -> reraise()
        Assert.True (tcs.Task.IsCompleted, "Task is not completed")

    [<Theory; InlineData(false); InlineData(true)>]
    member _.RunSynchronouslyCancellationWithDelayedResult(newAwait: bool) =
        let cts = new CancellationTokenSource()
        let tcs = TaskCompletionSource<int>()
        let _ = cts.Token.Register(fun () -> tcs.SetResult 42)
        let a = async {
            cts.CancelAfter(100)
            let! result = tcs.Task |> if newAwait then Async.Await else Async.AwaitTask
            return result }

        let cancelled =
            try
                Async.RunSynchronously(a, cancellationToken = cts.Token) |> ignore
                false
            with :? OperationCanceledException as o -> true
                 | _ -> false

        Assert.True (cancelled, "Task is not cancelled")

    [<Fact>]
    member _.ExceptionPropagatesToTask () =
        let a = async {
            do raise (Exception ())
         }
        let t = Async.StartAsTask a
        let mutable exceptionThrown = false
        try
            // waitForCompletion t
            t.Wait()
        with
            e -> exceptionThrown <- true
        Assert.True (t.IsFaulted)
        Assert.True(exceptionThrown)

    [<Fact>]
    member _.CancellationPropagatesToTask () =
        let ewh = new ManualResetEvent(false)
        let a = async {
                ewh.Set() |> Assert.True
                while true do ()
            }
        let t = Async.StartAsTask a
        ewh.WaitOne() |> Assert.True
        Async.CancelDefaultToken ()
        let mutable exceptionThrown = false
        try
            waitForCompletion t
        with e -> exceptionThrown <- true
        Assert.True (exceptionThrown)
        Assert.True(t.IsCanceled)

    [<Fact>]
    member _.CancellationPropagatesToGroup () =
        let ewh = new ManualResetEvent(false)
        let mutable cancelled = false
        let a = async {
                use! holder = Async.OnCancel (fun _ -> cancelled <- true)
                ewh.Set() |> Assert.True
                while true do ()
            }
        let cts = new CancellationTokenSource()
        let token = cts.Token
        let t = Async.StartAsTask(a, cancellationToken=token)
//        printfn "%A" t.Status
        ewh.WaitOne() |> Assert.True
        cts.Cancel()
//        printfn "%A" t.Status
        let mutable exceptionThrown = false
        try
            t.Wait()
        with e -> exceptionThrown <- true
        Assert.True (exceptionThrown)
        Assert.True(t.IsCanceled)
        Assert.True(cancelled)

    [<Fact>]
    member _.CreateImmediateAsTask () =
        let s = "Hello tasks!"
        let a = async { return s }
        let t : Task<string> = Async.StartImmediateAsTask a
        waitForCompletion t
        Assert.True (t.IsCompleted)
        Assert.AreEqual(s, t.Result)

    [<Fact>]
    member _.StartImmediateAsTask () =
        let s = "Hello tasks!"
        let a = async { return s }
        let t = Async.StartImmediateAsTask a
        waitForCompletion t
        Assert.True (t.IsCompleted)
        Assert.AreEqual(s, t.Result)


    [<Fact>]
    member _.ExceptionPropagatesToImmediateTask () =
        let a = async {
            do raise (Exception ())
         }
        let t = Async.StartImmediateAsTask a
        let mutable exceptionThrown = false
        try
            t.Wait()
        with
            e -> exceptionThrown <- true
        Assert.True (t.IsFaulted)
        Assert.True(exceptionThrown)

    [<Fact>]
    member _.CancellationPropagatesToImmediateTask () =
        let a = async {
                while true do
                    do! Async.Sleep 100
            }
        let t = Async.StartImmediateAsTask a
        Async.CancelDefaultToken ()
        let mutable exceptionThrown = false
        try
            t.Wait()
        with e -> exceptionThrown <- true
        Assert.True (exceptionThrown)
        Assert.True(t.IsCanceled)

    [<Fact>]
    member _.CancellationPropagatesToGroupImmediate () =
        let ewh = new ManualResetEvent(false)
        let mutable cancelled = false
        let a = async {
                use! holder = Async.OnCancel (fun _ -> cancelled <- true)
                ewh.Set() |> Assert.True
                while true do
                    do! Async.Sleep 100
            }
        let cts = new CancellationTokenSource()
        let token = cts.Token
        let t =
            Async.StartImmediateAsTask(a, cancellationToken=token)
        ewh.WaitOne() |> Assert.True
        cts.Cancel()
        let mutable exceptionThrown = false
        try
            t.Wait()
        with e -> exceptionThrown <- true
        Assert.True (exceptionThrown)
        Assert.True(t.IsCanceled)
        Assert.True(cancelled)

    [<Theory; InlineData(false); InlineData(true)>]
    member _.TaskAsyncValue(newAwait: bool) =
        let s = "Test"
        use t = Task.Factory.StartNew(Func<_>(fun () -> s))
        let a = async {
            let! s1 = t |> if newAwait then Async.Await else Async.AwaitTask
            return s = s1
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

    [<Theory; InlineData(false); InlineData(true)>]
    member _.AwaitTaskCancellation(newAwait: bool) =
        let a = async {
            let tcs = System.Threading.Tasks.TaskCompletionSource<unit>()
            tcs.SetCanceled()
            try
                do! tcs.Task |> if newAwait then Async.Await else Async.AwaitTask
                return false
            with :? OperationCanceledException -> return true
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

    [<Fact>]
    member _.AwaitCompletedTask() =
        let a = async {
            let threadIdBefore = Thread.CurrentThread.ManagedThreadId
            do! Async.AwaitTask Task.CompletedTask
            let threadIdAfter = Thread.CurrentThread.ManagedThreadId
            return threadIdBefore = threadIdAfter
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

    [<Theory; InlineData(false); InlineData(true)>]
    member _.AwaitTaskCancellationUntyped(newAwait: bool) =
        let a = async {
            let tcs = System.Threading.Tasks.TaskCompletionSource<unit>()
            tcs.SetCanceled()
            try
                do! tcs.Task :> Task |> if newAwait then Async.Await else Async.AwaitTask
                return false
            with :? OperationCanceledException -> return true
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

    [<Theory; InlineData(false); InlineData(true)>]
    member _.TaskAsyncValueException(newAwait: bool) =
        use t = Task.Factory.StartNew(Func<unit>(fun () -> raise <| Exception()))
        let a = async {
            try let! v = t |> if newAwait then Async.Await else Async.AwaitTask
                return false
            with e -> return true
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

    [<Theory; InlineData(false); InlineData(true)>]
    member _.TaskAsyncValueCancellation(newAwait: bool) =
        use ewh = new ManualResetEvent(false)
        let cts = new CancellationTokenSource()
        let token = cts.Token
        use t : Task<unit> = Task.Factory.StartNew(Func<unit>(fun () -> while not token.IsCancellationRequested do ()), token)
        let cancelled = ref true
        let a = async {
            try
                use! _holder = Async.OnCancel(fun _ -> ewh.Set() |> ignore)
                let! v = t |> if newAwait then Async.Await else Async.AwaitTask
                return v
            // A canceled task yields TaskCanceledException via the exception continuation
            with
               :? TaskCanceledException -> 
                  ewh.Set() |> ignore // this is ok
        }
        let t1 = Async.StartAsTask a
        cts.Cancel()
        ewh.WaitOne(10000) |> ignore
        // Don't leave unobserved background tasks, because they can crash the test run.
        t1.Wait()

    [<Theory; InlineData(false); InlineData(true)>]
    member _.NonGenericTaskAsyncValue(newAwait: bool) =
        let mutable hasBeenCalled = false
        use t = Task.Factory.StartNew(Action(fun () -> hasBeenCalled <- true))
        let a = async {
            do! t |> if newAwait then Async.Await else Async.AwaitTask
            return true
        }
        let ok = Async.RunSynchronously a
        Assert.True(hasBeenCalled && ok)

    [<Theory; InlineData(false); InlineData(true)>]
    member _.NonGenericTaskAsyncValueException(newAwait: bool) =
        use t = Task.Factory.StartNew(Action(fun () -> raise <| Exception()))
        let a = async {
            try
                let! v = t |> if newAwait then Async.Await else Async.AwaitTask
                return false
            with e -> return true
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

    [<Theory; InlineData(false); InlineData(true)>]
    member _.NonGenericTaskAsyncValueCancellation(newAwait: bool) =
        use ewh = new ManualResetEvent(false)
        let cts = new CancellationTokenSource()
        let token = cts.Token
        use t = Task.Factory.StartNew(Action(fun () -> while not token.IsCancellationRequested do ()), token)
        let a = async {
            try
                use! _holder = Async.OnCancel(fun _ -> ewh.Set() |> ignore)
                let! v = t |> if newAwait then Async.Await else Async.AwaitTask
                return v
            // A canceled task yields TaskCanceledException via the exception continuation
            with
               :? TaskCanceledException -> 
                  ewh.Set() |> ignore // this is ok
        }
        let t1 = Async.StartAsTask a
        cts.Cancel()
        ewh.WaitOne(10000) |> ignore
        t1.Wait()

    [<Fact>]
    member _.CancellationExceptionThrown () =
        use ewh = new ManualResetEventSlim(false)
        let cts = new CancellationTokenSource()
        let token = cts.Token
        let mutable hasThrown = false
        token.Register(fun () -> ewh.Set() |> ignore) |> ignore
        let a = async {
            try
                while true do token.ThrowIfCancellationRequested()
            with _ -> hasThrown <- true
        }
        Async.Start(a, token)
        cts.Cancel()
        ewh.Wait(10000) |> ignore
        Assert.False hasThrown

    [<Theory; InlineData(false); InlineData(true)>]
    member _.NoStackOverflowOnRecursion(newAwait: bool) =
        let mutable hasThrown = false
        let rec loop (x: int) = async {
            do! Task.CompletedTask |> if newAwait then Async.Await else Async.AwaitTask
            Console.WriteLine (if x = 10000 then failwith "finish" else x)
            return! loop(x+1)
        }
    
        try Async.RunSynchronously (loop 0)
            hasThrown <- false
        with Failure "finish" -> 
            hasThrown <- true
        Assert.True hasThrown

    // Both AwaitTask and Await ignore the ambient cancellation token while waiting
    // (Same goes for the typed variants)
    [<Theory; InlineData(false); InlineData(true)>]
    member _.``Both AwaitTask and Await ignore ambient cancellation while waiting``(newAwait) =
        let cts = new CancellationTokenSource()
        let tcs = TaskCompletionSource<unit>()  // task that never completes
        let res = TaskCompletionSource<bool>()

        let a = async {
            try do! tcs.Task |> if newAwait then Async.Await else Async.AwaitTask
                res.TrySetResult true |> ignore
            with _ -> res.TrySetResult false |> ignore
        }

        Async.Start(a, cts.Token)
        // NOTE we only cancel during the Await/AwaitTask - the initial check would throw if we canceled before the Start()
        cts.CancelAfter 100

        // AwaitTask should NOT honor the ambient CT trigger
        let taskCompleted = res.Task.Wait 500
        Assert.False(taskCompleted, "Await/AwaitTask should not have responded to ambient CT cancellation")
        tcs.TrySetResult() |> ignore // clean up
        res.Task.Wait()

    (* When an AggregateException has multiple inner exceptions, Await and AwaitTask behave identically *)
    
    [<Theory; InlineData(false); InlineData(true)>]
    member _.``Await and AwaitTask(Task<'T>) valid AggregateException is surfaced``(newAwait) =
        let tcs = TaskCompletionSource<int>()
        tcs.SetException [ ArgumentException "a" :> exn; InvalidOperationException "b" :> exn ]
        let a = async {
            try
                let! _ = tcs.Task |> if newAwait then Async.Await else Async.AwaitTask
                return false
            with :? AggregateException as ae -> return ae.InnerExceptions.Count = 2
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

    [<Theory; InlineData(false); InlineData(true)>]
    member _.``Await and AwaitTask(Task) valid AggregateException is surfaced``(newAwait) =
        let tcs = TaskCompletionSource<unit>()
        tcs.SetException [| ArgumentException "a" :> exn; InvalidOperationException "b" |]
        let a = async {
            try
                do! tcs.Task |> if newAwait then Async.Await else Async.AwaitTask
                return false
            with :? AggregateException as ae -> return ae.InnerExceptions.Count = 2
        }
        let ok = Async.RunSynchronously a
        Assert.True ok
        
    (* Async.Await behavioral differences
    
       The following tests demonstrate where Async.Await deliberately differs from Async.AwaitTask *)

    // Async.AwaitTask(Task) surfaces the wrapping AggregateException ...
    [<Fact>]
    member _.``AwaitTask(Task) egregious AggregateException is unchanged``() =
        let tcs = TaskCompletionSource<unit>()
        tcs.SetException(ArgumentException "original")
        let a = async {
            try do! Async.AwaitTask tcs.Task
                return false
            with :? AggregateException -> return true
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

    // ... whereas Async.Await(Task) surfaces the inner exception directly.
    [<Fact>]
    member _.``Await(Task) egregious AggregateException is unwrapped``() =
        let tcs = TaskCompletionSource<unit>()
        tcs.SetException(ArgumentException "original")
        let a = async {
            try do! Async.Await tcs.Task
                return false
            with :? ArgumentException as ae -> return ae.Message = "original"
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

    // Async.AwaitTask(Task<'T>) surfaces the wrapping AggregateException ...
    [<Fact>]
    member _.``AwaitTask(Task<'T>) egregious AggregateException is unchanged``() =
        let tcs = TaskCompletionSource<int>()
        tcs.SetException(ArgumentException "original")
        let a = async {
            try let! _ = Async.AwaitTask tcs.Task
                return false
            with :? AggregateException -> return true
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

    // ... whereas Async.Await(Task<'T>) surfaces the inner exception directly.
    [<Fact>]
    member _.``Await(Task<'T>) egregious AggregateException is unwrapped``() =
        let tcs = TaskCompletionSource<int>()
        tcs.SetException(ArgumentException "original")
        let a = async {
            try let! _ = Async.Await tcs.Task
                return false
            with :? ArgumentException as ae -> return ae.Message = "original"
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

    (* Await(Task/Task<'T>) overloads happy path *)
    
    [<Fact>]
    member _.``Await(Task<'T>) happy path``() =
        let a = async {
            let! v = Async.Await(System.Threading.Tasks.Task.FromResult(42))
            return v = 42
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

    [<Fact>]
    member _.``Await(Task) happy path``() =
        let a = async {
            do! Async.Await(System.Threading.Tasks.Task.CompletedTask)
            return true
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

#if NETSTANDARD2_1
    (* Await(ValueTask and ValueTask<'T>) overloads coverage of mainline behaviors *)

    [<Fact>]
    member _.``Await(ValueTask) happy path``() =
        let a = async {
            do! Async.Await(ValueTask())
            return true
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

    [<Fact>]
    member _.``Await(ValueTask<'T>) happy path``() =
        let a = async {
            let! v = Async.Await(ValueTask<int>(42))
            return v = 42
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

    [<Fact>]
    member _.``Await(ValueTask) exception unwraps``() =
        let tcs = TaskCompletionSource<unit>()
        tcs.SetException(ArgumentException "original")
        let task = ValueTask(tcs.Task :> Task)
        let a = async {
            try do! Async.Await task
                return false
            with :? ArgumentException as ae -> return ae.Message = "original"
        }
        let ok = Async.RunSynchronously a
        Assert.True ok

    [<Fact>]
    member _.``Await(ValueTask<'T>) exception unwraps``() =
        let tcs = TaskCompletionSource<int>()
        tcs.SetException(ArgumentException "original")
        let a = async {
            try let! _ = Async.Await(ValueTask<int>(tcs.Task))
                return false
            with :? ArgumentException as ae -> return ae.Message = "original"
        }
        let ok = Async.RunSynchronously a
        Assert.True ok
#endif

[<Collection(nameof FSharp.Test.NotThreadSafeResourceCollection)>]
module AsyncTaskLikeAwaitTests =

    // Minimal custom task-like type wrapping Task<'T>
    type MyTask<'T>(inner: Task<'T>) =
        member _.GetAwaiter() = inner.GetAwaiter()

    // Minimal custom unit-returning task-like
    type MyUnitTask(inner: Task) =
        member _.GetAwaiter() = inner.GetAwaiter()

    [<Fact>]
    let ``Await(task-like) happy path with result``() =
        let result =
            async {
                let! v = Async.Await(MyTask(Task.FromResult 99))
                return v
            }
            |> Async.RunSynchronously
        Assert.Equal(99, result)

    [<Fact>]
    let ``Await(task-like) happy path unit``() =
        async {
            do! Async.Await(MyUnitTask(Task.CompletedTask))
        }
        |> Async.RunSynchronously

    [<Fact>]
    let ``Await(task-like) deferred completion``() =
        let tcs = TaskCompletionSource<int>()
        let t =
            async {
                let! v = Async.Await(MyTask(tcs.Task))
                return v
            }
            |> Async.StartAsTask
        Assert.False(t.IsCompleted, "Should not be done before TCS is set")
        tcs.SetResult 7
        t.Wait(TimeSpan.FromSeconds 5.0) |> ignore
        Assert.Equal(7, t.Result)

    [<Fact>]
    let ``Await(task-like) exception propagation``() =
        let tcs = TaskCompletionSource<int>()
        let a =
            async {
                try let! _ = Async.Await(MyTask(tcs.Task))
                    return false
                with :? InvalidOperationException as e ->
                    return e.Message = "boom"
            }
        tcs.SetException(InvalidOperationException "boom")
        let ok = Async.RunSynchronously a
        Assert.True ok

    [<Fact>]
    let ``Await(YieldAwaitable) yields and resumes``() =
        // Task.Yield() returns a YieldAwaitable which is a struct — exercises the struct-awaiter path.
        let mutable before, after = false, false
        async {
            before <- true
            do! Async.Await(Task.Yield())
            after <- true
        }
        |> Async.RunSynchronously
        Assert.True(before && after)

    [<Fact>]
    let ``Await(ConfiguredTaskAwaitable) from ConfigureAwait``() =
        // task.ConfigureAwait(false) returns a ConfiguredTaskAwaitable — a common real-world task-like.
        let result =
            async {
                let! v = Async.Await(Task.FromResult(42).ConfigureAwait(false))
                return v
            }
            |> Async.RunSynchronously
        Assert.Equal(42, result)

[<Collection(nameof FSharp.Test.NotThreadSafeResourceCollection)>]
module AsyncAwaitStackTraceTests =

    open System.Runtime.CompilerServices

    // Minimal wrapper to route through the SRTP overload instead of the specific Task<'T> overload.
    // Task<'T>, Task, ValueTask<'T>, and ValueTask all have higher-priority intrinsic overloads.
    type TaskWrapper<'T>(inner: Task<'T>) =
        member _.GetAwaiter() = inner.GetAwaiter()

    // Plain function — provides a stable named frame at the outermost throw site.
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let throwAtLevel1 () : unit = invalidOp "boom"

    // Level-1 task: thin wrapper around the direct throw.
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let level1Task () : Task<unit> = task { throwAtLevel1 () }

    // Level-2 task: introduces a real async await boundary between levels 1 and 2.
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let level2Task () : Task<unit> = task { do! level1Task () }

    // Run via StartImmediateAsTask + .Wait() and return the inner exception.
    // Using StartImmediateAsTask (not RunSynchronously) ensures that the async-layer
    // exception machinery goes through TaskCompletionSource.SetException, which preserves
    // the stack trace rather than rethrowing synchronously and potentially truncating it.
    let runAndCaptureException (computation: Async<unit>) : exn =
        // TODO swap in usage of Async.RunSynchronouslyImmediate
        let t = Async.StartImmediateAsTask computation
        let ae = Assert.Throws<AggregateException>(fun () -> t.Wait())
        ae.InnerException

    // Template assertion: levels 1 and 2 must be traceable in the stack trace
    // regardless of which Async.Await overload is used.
    let checkTrace totalCount (e: exn) =
        let trace = e.StackTrace
        Assert.NotNull(trace)
        Assert.Contains("throwAtLevel1", trace)
        Assert.Contains("level1Task", trace)
        Assert.Contains("level2Task", trace)
#if !NETFRAMEWORK472 // downlevel has interstitial layers we are not seeking to characterize at this point
        Assert.True((totalCount = trace.Split('\n').Length), trace)
#endif

    // --- Tests per overload ---
    // The common skeleton is: build a 3-level chain (throwAtLevel1 → level1Task → level2Task),
    // wrap the outermost level in an async block using Async.Await, run via
    // StartImmediateAsTask + .Wait(), and assert on the resulting exception's stack trace.

    [<Fact>]
    let ``Await Task-of-T: all three levels visible in stack trace`` () =
        let e = runAndCaptureException (async { do! Async.Await(level2Task()) })
        checkTrace 3 e

    [<Fact>]
    let ``Await Task (non-generic): all three levels visible in stack trace`` () =
        let e = runAndCaptureException (async { do! Async.Await(level2Task() :> Task) })
        checkTrace 3 e
        // Same behavior as the Task<'T> overload — see comment there.

#if NETSTANDARD2_1
    [<Fact>]
    let ``Await ValueTask-of-T: all three levels visible in stack trace`` () =
        // For a faulted ValueTask<unit>, IsCompletedSuccessfully is false; the overload falls
        // through to AwaitTask, which takes the same path as the specific Task<'T> overload.
        let e = runAndCaptureException (async { do! Async.Await(ValueTask<unit>(level2Task())) }) 
        checkTrace 3 e

    [<Fact>]
    let ``Await ValueTask (non-generic): all three levels visible in stack trace`` () =
        // Same as ValueTask<'T>: falls through to AwaitUnitTask for the non-successfully-completed case.
        let e = runAndCaptureException (async { do! Async.Await(ValueTask(level2Task() :> Task)) })

        checkTrace 3 e
#endif

    [<Fact>]
    let ``Await task-like via SRTP overload: all three levels visible in stack trace`` () =
        let e = runAndCaptureException (async { do! Async.Await(TaskWrapper(level2Task())) })

        // 4 instead of 3 as current impl has an outer "at FSharp.Core.UnitTests.Control.AsyncAwaitStackTraceTests.e@836-9.Invoke(Tuple`3 tupledArg)
        checkTrace 4 e