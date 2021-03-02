// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Control.Async type

namespace FSharp.Core.UnitTests.Control

open System
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit
open System.Threading
open System.Threading.Tasks

type RunWithContinuationsTest_WhatToDo =
    | Exit
    | Cancel
    | Throw

type AsyncType() =

    let ignoreSynchCtx f =
        f ()


    [<Fact>]
    member this.StartWithContinuations() =

        let whatToDo = ref Exit

        let asyncWorkflow() =
            async {
                let currentState = !whatToDo

                // Act
                let result =
                    match currentState with
                    | Exit   -> 1
                    | Cancel -> Async.CancelDefaultToken()
                                sleep(1 * 1000)
                                0
                    | Throw  -> raise <| System.Exception("You asked me to do it!")

                return result
            }

        let onSuccess x   =
            match !whatToDo with
            | Cancel | Throw
                -> Assert.Fail("Expected onSuccess but whatToDo was not Exit", [| whatToDo |])
            | Exit
                -> ()

        let onException x =
            match !whatToDo with
            | Exit | Cancel
                -> Assert.Fail("Expected onException but whatToDo was not Throw", [| whatToDo |])
            | Throw  -> ()

        let onCancel x    =
            match !whatToDo with
            | Exit | Throw
                -> Assert.Fail("Expected onCancel but whatToDo was not Cancel", [| whatToDo |])
            | Cancel -> ()

        // Run it once.
        whatToDo := Exit
        Async.StartWithContinuations(asyncWorkflow(), onSuccess, onException, onCancel)

        whatToDo := Cancel
        Async.StartWithContinuations(asyncWorkflow(), onSuccess, onException, onCancel)

        whatToDo := Throw
        Async.StartWithContinuations(asyncWorkflow(), onSuccess, onException, onCancel)

        ()

    [<Fact>]
    member this.AsyncRunSynchronouslyReusesThreadPoolThread() =
        let action = async { async { () } |> Async.RunSynchronously }
        let computation =
            [| for i in 1 .. 1000 -> action |]
            |> Async.Parallel
        // This test needs approximately 1000 ThreadPool threads
        // if Async.RunSynchronously doesn't reuse them.
        // In such case TimeoutException is raised
        // since ThreadPool cannot provide 1000 threads in 1 second
        // (the number of threads in ThreadPool is adjusted slowly).
        Async.RunSynchronously(computation, timeout = 1000) |> ignore

    [<Theory>]
    [<InlineData("int32")>]
    [<InlineData("timespan")>]
    member this.AsyncSleepCancellation1(sleepType) =
        ignoreSynchCtx (fun () ->
            let computation =
                match sleepType with
                | "int32"    -> Async.Sleep(10000000)
                | "timespan" -> Async.Sleep(10000000.0 |> TimeSpan.FromMilliseconds)
                | unknown    -> raise (NotImplementedException(unknown))
            let result = ref ""
            use cts = new CancellationTokenSource()
            Async.StartWithContinuations(computation,
                                            (fun _ -> result := "Ok"),
                                            (fun _ -> result := "Exception"),
                                            (fun _ -> result := "Cancel"),
                                            cts.Token)
            cts.Cancel()
            Async.Sleep(1000) |> Async.RunSynchronously
            Assert.AreEqual("Cancel", !result)
        )

    [<Theory>]
    [<InlineData("int32")>]
    [<InlineData("timespan")>]
    member this.AsyncSleepCancellation2(sleepType) =
        ignoreSynchCtx (fun () ->
            let computation =
                match sleepType with
                | "int32"    -> Async.Sleep(10)
                | "timespan" -> Async.Sleep(10.0 |> TimeSpan.FromMilliseconds)
                | unknown    -> raise (NotImplementedException(unknown))
            for i in 1..100 do
                let result = ref ""
                use completedEvent = new ManualResetEvent(false)
                use cts = new CancellationTokenSource()
                Async.StartWithContinuations(computation,
                                                (fun _ -> result := "Ok"; completedEvent.Set() |> ignore),
                                                (fun _ -> result := "Exception"; completedEvent.Set() |> ignore),
                                                (fun _ -> result := "Cancel"; completedEvent.Set() |> ignore),
                                                cts.Token)
                sleep(10)
                cts.Cancel()
                completedEvent.WaitOne() |> Assert.True
                Assert.True(!result = "Cancel" || !result = "Ok")
        )

    [<Theory>]
    [<InlineData("int32")>]
    [<InlineData("timespan")>]
    member this.AsyncSleepThrowsOnNegativeDueTimes(sleepType) =
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
    member this.AsyncSleepInfinitely() =
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

    member private this.WaitASec (t:Task) =
        let result = t.Wait(TimeSpan(hours=0,minutes=0,seconds=1))
        Assert.True(result, "Task did not finish after waiting for a second.")


    [<Fact>]
    member this.CreateTask () =
        let s = "Hello tasks!"
        let a = async { return s }
#if !NET46
        let t : Task<string> =
#else
        use t : Task<string> =
#endif
            Async.StartAsTask a
        this.WaitASec t
        Assert.True (t.IsCompleted)
        Assert.AreEqual(s, t.Result)

    [<Fact>]
    member this.StartAsTaskCancellation () =
        let cts = new CancellationTokenSource()
        let mutable spinloop = true
        let doSpinloop () = while spinloop do ()
        let a = async {
            cts.CancelAfter (100)
            doSpinloop()
        }
#if !NET46
        let t : Task<unit> =
#else
        use t : Task<unit> =
#endif
            Async.StartAsTask(a, cancellationToken = cts.Token)

        // Should not finish, we don't eagerly mark the task done just because it's been signaled to cancel.
        try
            let result = t.Wait(300)
            Assert.False (result)
        with :? AggregateException -> Assert.Fail "Task should not finish, yet"

        spinloop <- false

        try
            this.WaitASec t
        with :? AggregateException as a ->
            match a.InnerException with
            | :? TaskCanceledException as t -> ()
            | _ -> reraise()
        Assert.True (t.IsCompleted, "Task is not completed")

    [<Fact>]
    member this.``AwaitTask ignores Async cancellation`` () =
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
            this.WaitASec tcs.Task
        with :? AggregateException as a ->
            match a.InnerException with
            | :? TaskCanceledException -> ()
            | _ -> reraise()
        Assert.True (tcs.Task.IsCompleted, "Task is not completed")

    [<Fact>]
    member this.RunSynchronouslyCancellationWithDelayedResult () =
        let cts = new CancellationTokenSource()
        let tcs = TaskCompletionSource<int>()
        let _ = cts.Token.Register(fun () -> tcs.SetResult 42)
        let a = async {
            cts.CancelAfter (100)
            let! result = tcs.Task |> Async.AwaitTask
            return result }

        try
            Async.RunSynchronously(a, cancellationToken = cts.Token)
                |> ignore
        with :? OperationCanceledException as o -> ()

    [<Fact>]
    member this.ExceptionPropagatesToTask () =
        let a = async {
            do raise (Exception ())
         }
#if !NET46
        let t =
#else
        use t =
#endif
            Async.StartAsTask a
        let mutable exceptionThrown = false
        try
            this.WaitASec t
        with
            e -> exceptionThrown <- true
        Assert.True (t.IsFaulted)
        Assert.True(exceptionThrown)

    [<Fact>]
    member this.CancellationPropagatesToTask () =
        let a = async {
                while true do ()
            }
#if !NET46
        let t =
#else
        use t =
#endif
            Async.StartAsTask a
        Async.CancelDefaultToken ()
        let mutable exceptionThrown = false
        try
            this.WaitASec t
        with e -> exceptionThrown <- true
        Assert.True (exceptionThrown)
        Assert.True(t.IsCanceled)

    [<Fact>]
    member this.CancellationPropagatesToGroup () =
        let ewh = new ManualResetEvent(false)
        let cancelled = ref false
        let a = async {
                use! holder = Async.OnCancel (fun _ -> cancelled := true)
                ewh.Set() |> Assert.True
                while true do ()
            }
        let cts = new CancellationTokenSource()
        let token = cts.Token
#if !NET46
        let t =
#else
        use t =
#endif
            Async.StartAsTask(a, cancellationToken=token)
//        printfn "%A" t.Status
        ewh.WaitOne() |> Assert.True
        cts.Cancel()
//        printfn "%A" t.Status
        let mutable exceptionThrown = false
        try
            this.WaitASec t
        with e -> exceptionThrown <- true
        Assert.True (exceptionThrown)
        Assert.True(t.IsCanceled)
        Assert.True(!cancelled)

    [<Fact>]
    member this.CreateImmediateAsTask () =
        let s = "Hello tasks!"
        let a = async { return s }
#if !NET46
        let t : Task<string> =
#else
        use t : Task<string> =
#endif
            Async.StartImmediateAsTask a
        this.WaitASec t
        Assert.True (t.IsCompleted)
        Assert.AreEqual(s, t.Result)

    [<Fact>]
    member this.StartImmediateAsTask () =
        let s = "Hello tasks!"
        let a = async { return s }
#if !NET46
        let t =
#else
        use t =
#endif
            Async.StartImmediateAsTask a
        this.WaitASec t
        Assert.True (t.IsCompleted)
        Assert.AreEqual(s, t.Result)


    [<Fact>]
    member this.ExceptionPropagatesToImmediateTask () =
        let a = async {
            do raise (Exception ())
         }
#if !NET46
        let t =
#else
        use t =
#endif
            Async.StartImmediateAsTask a
        let mutable exceptionThrown = false
        try
            this.WaitASec t
        with
            e -> exceptionThrown <- true
        Assert.True (t.IsFaulted)
        Assert.True(exceptionThrown)

#if IGNORED
    [<Fact>]
    [<Ignore("https://github.com/Microsoft/visualfsharp/issues/4337")>]
    member this.CancellationPropagatesToImmediateTask () =
        let a = async {
                while true do ()
            }
#if !NET46
        let t =
#else
        use t =
#endif
            Async.StartImmediateAsTask a
        Async.CancelDefaultToken ()
        let mutable exceptionThrown = false
        try
            this.WaitASec t
        with e -> exceptionThrown <- true
        Assert.True (exceptionThrown)
        Assert.True(t.IsCanceled)
#endif

#if IGNORED
    [<Fact>]
    [<Ignore("https://github.com/Microsoft/visualfsharp/issues/4337")>]
    member this.CancellationPropagatesToGroupImmediate () =
        let ewh = new ManualResetEvent(false)
        let cancelled = ref false
        let a = async {
                use! holder = Async.OnCancel (fun _ -> cancelled := true)
                ewh.Set() |> Assert.True
                while true do ()
            }
        let cts = new CancellationTokenSource()
        let token = cts.Token
        use t =
            Async.StartImmediateAsTask(a, cancellationToken=token)
//        printfn "%A" t.Status
        ewh.WaitOne() |> Assert.True
        cts.Cancel()
//        printfn "%A" t.Status
        let mutable exceptionThrown = false
        try
            this.WaitASec t
        with e -> exceptionThrown <- true
        Assert.True (exceptionThrown)
        Assert.True(t.IsCanceled)
        Assert.True(!cancelled)
#endif

    [<Fact>]
    member this.TaskAsyncValue () =
        let s = "Test"
#if !NET46
        let t =
#else
        use t =
#endif
            Task.Factory.StartNew(Func<_>(fun () -> s))
        let a = async {
                let! s1 = Async.AwaitTask(t)
                return s = s1
            }
        Async.RunSynchronously(a, 1000) |> Assert.True

    [<Fact>]
    member this.AwaitTaskCancellation () =
        let test() = async {
            let tcs = new System.Threading.Tasks.TaskCompletionSource<unit>()
            tcs.SetCanceled()
            try
                do! Async.AwaitTask tcs.Task
                return false
            with :? System.OperationCanceledException -> return true
        }

        Async.RunSynchronously(test()) |> Assert.True

    [<Fact>]
    member this.AwaitCompletedTask() =
        let test() = async {
            let threadIdBefore = Thread.CurrentThread.ManagedThreadId
            do! Async.AwaitTask Task.CompletedTask
            let threadIdAfter = Thread.CurrentThread.ManagedThreadId
            return threadIdBefore = threadIdAfter
        }

        Async.RunSynchronously(test()) |> Assert.True

    [<Fact>]
    member this.AwaitTaskCancellationUntyped () =
        let test() = async {
            let tcs = new System.Threading.Tasks.TaskCompletionSource<unit>()
            tcs.SetCanceled()
            try
                do! Async.AwaitTask (tcs.Task :> Task)
                return false
            with :? System.OperationCanceledException -> return true
        }

        Async.RunSynchronously(test()) |> Assert.True

    [<Fact>]
    member this.TaskAsyncValueException () =
#if !NET46
        let t =
#else
        use t =
#endif
            Task.Factory.StartNew(Func<unit>(fun () -> raise <| Exception()))
        let a = async {
                try
                    let! v = Async.AwaitTask(t)
                    return false
                with e -> return true
              }
        Async.RunSynchronously(a, 1000) |> Assert.True

    [<Fact>]
    member this.TaskAsyncValueCancellation () =
        use ewh = new ManualResetEvent(false)
        let cts = new CancellationTokenSource()
        let token = cts.Token
#if !NET46
        let t : Task<unit>=
#else
        use t : Task<unit>=
#endif
          Task.Factory.StartNew(Func<unit>(fun () -> while not token.IsCancellationRequested do ()), token)
        let cancelled = ref true
        let a = async {
                    use! _holder = Async.OnCancel(fun _ -> ewh.Set() |> ignore)
                    let! v = Async.AwaitTask(t)
                    return v
            }
        Async.Start a
        cts.Cancel()
        ewh.WaitOne(10000) |> ignore

    [<Fact>]
    member this.NonGenericTaskAsyncValue () =
        let hasBeenCalled = ref false
#if !NET46
        let t =
#else
        use t =
#endif
            Task.Factory.StartNew(Action(fun () -> hasBeenCalled := true))
        let a = async {
                do! Async.AwaitTask(t)
                return true
            }
        let result =Async.RunSynchronously(a, 1000)
        (!hasBeenCalled && result) |> Assert.True

    [<Fact>]
    member this.NonGenericTaskAsyncValueException () =
#if !NET46
        let t =
#else
        use t =
#endif
            Task.Factory.StartNew(Action(fun () -> raise <| Exception()))
        let a = async {
                try
                    let! v = Async.AwaitTask(t)
                    return false
                with e -> return true
              }
        Async.RunSynchronously(a, 3000) |> Assert.True

    [<Fact>]
    member this.NonGenericTaskAsyncValueCancellation () =
        use ewh = new ManualResetEvent(false)
        let cts = new CancellationTokenSource()
        let token = cts.Token
#if !NET46
        let t =
#else
        use t =
#endif
            Task.Factory.StartNew(Action(fun () -> while not token.IsCancellationRequested do ()), token)
        let a = async {
                    use! _holder = Async.OnCancel(fun _ -> ewh.Set() |> ignore)
                    let! v = Async.AwaitTask(t)
                    return v
            }
        Async.Start a
        cts.Cancel()
        ewh.WaitOne(10000) |> ignore

    [<Fact>]
    member this.CancellationExceptionThrown () =
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
