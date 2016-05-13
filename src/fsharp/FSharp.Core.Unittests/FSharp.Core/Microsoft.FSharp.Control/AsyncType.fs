// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Control.Async type

namespace FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Control

open System
open FSharp.Core.Unittests.LibraryTestFx
open NUnit.Framework
open System.Threading

#if FX_NO_TPL_PARALLEL
#else
open System.Threading.Tasks
#endif

type RunWithContinuationsTest_WhatToDo =
    | Exit
    | Cancel
    | Throw

[<TestFixture>]
type AsyncType() =

    let ignoreSynchCtx f =
        f ()


    [<Test>]
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

    [<Test>]
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
        Assert.DoesNotThrow(fun () ->
            Async.RunSynchronously(computation, timeout = 1000)
            |> ignore)

    [<Test>]
    member this.AsyncSleepCancellation1() =
        ignoreSynchCtx (fun () ->
            let computation = Async.Sleep(10000000)
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

    [<Test>]
    member this.AsyncSleepCancellation2() =
        ignoreSynchCtx (fun () ->
            let computation = Async.Sleep(10)
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
                completedEvent.WaitOne() |> Assert.IsTrue
                Assert.IsTrue(!result = "Cancel" || !result = "Ok")
        )

#if FX_NO_TPL_PARALLEL
#else

    member private this.WaitASec (t:Task) =
        let result = t.Wait(TimeSpan(hours=0,minutes=0,seconds=1))
        Assert.IsTrue(result)        
      
    
    [<Test>]
    member this.CreateTask () =
        let s = "Hello tasks!"
        let a = async { return s }
#if FSHARP_CORE_NETCORE_PORTABLE || coreclr
        let t : Task<string> =
#else
        use t : Task<string> =
#endif
            Async.StartAsTask a
        this.WaitASec t
        Assert.IsTrue (t.IsCompleted)
        Assert.AreEqual(s, t.Result)    
        
    [<Test>]
    member this.StartTask () =
        let s = "Hello tasks!"
        let a = async { return s }
#if FSHARP_CORE_NETCORE_PORTABLE || coreclr
        let t = 
#else
        use t =
#endif
            Async.StartAsTask a
        this.WaitASec t
        Assert.IsTrue (t.IsCompleted)
        Assert.AreEqual(s, t.Result)    
      
    [<Test>]
    member this.ExceptionPropagatesToTask () =
        let a = async { 
            do raise (Exception ())
         }
#if FSHARP_CORE_NETCORE_PORTABLE || coreclr
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
        Assert.IsTrue (t.IsFaulted)
        Assert.IsTrue(exceptionThrown)
        
    [<Test>]
    member this.CancellationPropagatesToTask () =
        let a = async {
                while true do ()
            }
#if FSHARP_CORE_NETCORE_PORTABLE || coreclr
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
        Assert.IsTrue (exceptionThrown)   
        Assert.IsTrue(t.IsCanceled)            
        
    [<Test>]
    member this.CancellationPropagatesToGroup () =
        let ewh = new ManualResetEvent(false)
        let cancelled = ref false
        let a = async { 
                use! holder = Async.OnCancel (fun _ -> cancelled := true)
                ewh.Set() |> Assert.IsTrue
                while true do ()
            }
        let cts = new CancellationTokenSource()
        let token = cts.Token
#if FSHARP_CORE_NETCORE_PORTABLE || coreclr
        let t = 
#else
        use t =
#endif
            Async.StartAsTask(a, cancellationToken=token)
//        printfn "%A" t.Status
        ewh.WaitOne() |> Assert.IsTrue
        cts.Cancel()
//        printfn "%A" t.Status        
        let mutable exceptionThrown = false
        try
            this.WaitASec t
        with e -> exceptionThrown <- true
        Assert.IsTrue (exceptionThrown)   
        Assert.IsTrue(t.IsCanceled)      
        Assert.IsTrue(!cancelled)      


    [<Test>]
    member this.TaskAsyncValue () =
        let s = "Test"
#if FSHARP_CORE_NETCORE_PORTABLE || coreclr
        let t = 
#else
        use t =
#endif
            Task.Factory.StartNew(Func<_>(fun () -> s))
        let a = async {
                let! s1 = Async.AwaitTask(t)
                return s = s1
            }
        Async.RunSynchronously(a, 1000) |> Assert.IsTrue        

    [<Test>]
    member this.AwaitTaskCancellation () =
        let test() = async {
            let tcs = new System.Threading.Tasks.TaskCompletionSource<unit>()
            tcs.SetCanceled()
            try 
                do! Async.AwaitTask tcs.Task
                return false
            with :? System.OperationCanceledException -> return true
        }

        Async.RunSynchronously(test()) |> Assert.IsTrue      
        
    [<Test>]
    member this.TaskAsyncValueException () =
#if FSHARP_CORE_NETCORE_PORTABLE || coreclr
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
        Async.RunSynchronously(a, 1000) |> Assert.IsTrue  
        
    [<Test>]
    member this.TaskAsyncValueCancellation () =
        use ewh = new ManualResetEvent(false)    
        let cts = new CancellationTokenSource()
        let token = cts.Token
#if FSHARP_CORE_NETCORE_PORTABLE || coreclr
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

    [<Test>]
    member this.NonGenericTaskAsyncValue () =
        let hasBeenCalled = ref false
#if FSHARP_CORE_NETCORE_PORTABLE || coreclr
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
        (!hasBeenCalled && result) |> Assert.IsTrue
        
    [<Test>]
    member this.NonGenericTaskAsyncValueException () =
#if FSHARP_CORE_NETCORE_PORTABLE || coreclr
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
        Async.RunSynchronously(a, 3000) |> Assert.IsTrue  
        
    [<Test>]
    member this.NonGenericTaskAsyncValueCancellation () =
        use ewh = new ManualResetEvent(false)    
        let cts = new CancellationTokenSource()
        let token = cts.Token
#if FSHARP_CORE_NETCORE_PORTABLE || coreclr
        let t = 
#else
        use t =
#endif   
            Task.Factory.StartNew(Action(fun () -> while not token.IsCancellationRequested do ()), token)
        let cancelled = ref true
        let a = async {
                    use! _holder = Async.OnCancel(fun _ -> ewh.Set() |> ignore)
                    let! v = Async.AwaitTask(t)
                    return v
            }        
        Async.Start a
        cts.Cancel()
        ewh.WaitOne(10000) |> ignore        

#endif