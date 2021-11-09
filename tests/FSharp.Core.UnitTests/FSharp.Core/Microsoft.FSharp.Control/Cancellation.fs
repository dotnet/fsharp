// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.Control
#nowarn "52"
open System
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit
open System.Threading


type CancellationType() =

    [<Fact>]
    member this.CancellationNoCallbacks() =
        let _ : CancellationTokenSource = null // compilation test
        use cts1 = new CancellationTokenSource()
        let token1 = cts1.Token
        Assert.False (token1.IsCancellationRequested)
        use cts2 = new CancellationTokenSource()
        let token2 = cts2.Token
        Assert.False (token2.IsCancellationRequested)
        cts1.Cancel()
        Assert.True(token1.IsCancellationRequested)
        Assert.False (token2.IsCancellationRequested)
        cts2.Cancel()
        Assert.True(token2.IsCancellationRequested)
        
    [<Fact>]
    member this.CancellationRegistration() =
        let cts = new CancellationTokenSource()
        let token = cts.Token
        let mutable called = false
        let r = token.Register(Action<obj>(fun _ -> called <- true), null)
        Assert.False(called)
        r.Dispose()
        cts.Cancel()
        Assert.False(called)
        
    [<Fact>]
    member this.CancellationWithCallbacks() =
        let cts1 = new CancellationTokenSource()
        let cts2 = new CancellationTokenSource()
        let is1Called = ref false
        let is2Called = ref false
        let is3Called = ref false
        let assertAndOff (expected:bool) (r:bool ref) = Assert.AreEqual(expected,r.Value); r.Value <- false
        let r1 = cts1.Token.Register(Action<obj>(fun _ -> is1Called.Value <- true), null)
        let r2 = cts1.Token.Register(Action<obj>(fun _ -> is2Called.Value <- true), null)
        let r3 = cts2.Token.Register(Action<obj>(fun _ -> is3Called.Value <- true), null) 
        Assert.False(is1Called.Value)
        Assert.False(is2Called.Value)
        r2.Dispose()
        
        // Cancelling cts1: r2 is disposed and r3 is for cts2, only r1 should be called
        cts1.Cancel()
        assertAndOff true   is1Called
        assertAndOff false  is2Called
        assertAndOff false  is3Called
        Assert.True(cts1.Token.IsCancellationRequested)
        
        let isAnotherOneCalled = ref false
        let _ = cts1.Token.Register(Action<obj>(fun _ -> isAnotherOneCalled.Value <- true), null)
        assertAndOff true isAnotherOneCalled
        
        // Cancelling cts2: only r3 should be called
        cts2.Cancel()
        assertAndOff false  is1Called
        assertAndOff false  is2Called
        assertAndOff true   is3Called
        Assert.True(cts2.Token.IsCancellationRequested)
        
        
        // Cancelling cts1 again: no one should be called
        cts1.Cancel()
        assertAndOff false is1Called
        assertAndOff false is2Called
        assertAndOff false is3Called
        
        // Disposing
        let token = cts2.Token
        cts2.Dispose()
        Assert.True(token.IsCancellationRequested)
        let () =
            let mutable odeThrown = false
            try
                r3.Dispose()
            with
            |   :? ObjectDisposedException -> odeThrown <- true
            Assert.False(odeThrown)
            
        let () =
            let mutable odeThrown = false
            try
                cts2.Token.Register(Action<obj>(fun _ -> ()), null) |> ignore
            with
            |   :? ObjectDisposedException -> odeThrown <- true
            Assert.True(odeThrown)
        ()
        
    [<Fact>]    
    member this.CallbackOrder() = 
        use cts = new CancellationTokenSource()
        let mutable current = 0
        let action (o:obj) = Assert.AreEqual(current, (unbox o : int)); current <- current + 1
        cts.Token.Register(Action<obj>(action), box 2) |> ignore
        cts.Token.Register(Action<obj>(action), box 1) |> ignore
        cts.Token.Register(Action<obj>(action), box 0) |> ignore
        cts.Cancel()
        
    [<Fact>]
    member this.CallbackExceptions() =
        use cts = new CancellationTokenSource()
        let action (o:obj) = new InvalidOperationException(String.Format("{0}", o)) |> raise
        cts.Token.Register(Action<obj>(action), box 0) |> ignore
        cts.Token.Register(Action<obj>(action), box 1) |> ignore
        cts.Token.Register(Action<obj>(action), box 2) |> ignore
        let mutable exnThrown = false
        try
            cts.Cancel()
        with
        | :? AggregateException as ae ->
                exnThrown <- true
                ae.InnerExceptions |> Seq.iter (fun e -> (e :? InvalidOperationException) |> Assert.True)
                let msgs = ae.InnerExceptions |> Seq.map (fun e -> e.Message) |> Seq.toList
                Assert.AreEqual(["2";"1";"0"], msgs)
        Assert.True exnThrown
        Assert.True cts.Token.IsCancellationRequested
        
    [<Fact>]
    member this.LinkedSources() =
        let () =
            use cts1 = new CancellationTokenSource()
            use cts2 = new CancellationTokenSource()
            use ctsLinked = CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, cts2.Token)
            let linkedToken = ctsLinked.Token
            Assert.False(linkedToken.IsCancellationRequested)
            cts1.Cancel()
            Assert.True(linkedToken.IsCancellationRequested)
            
        let () = 
            use cts1 = new CancellationTokenSource()
            use cts2 = new CancellationTokenSource()
            use ctsLinked = CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, cts2.Token)
            let linkedToken = ctsLinked.Token
            Assert.False(linkedToken.IsCancellationRequested)
            cts2.Cancel()
            Assert.True(linkedToken.IsCancellationRequested)
            
        let () =            
            use cts1 = new CancellationTokenSource()
            use cts2 = new CancellationTokenSource()
            cts1.Cancel()
            use ctsLinked = CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, cts2.Token)
            let linkedToken = ctsLinked.Token            
            Assert.True(linkedToken.IsCancellationRequested)
            let mutable doExec = false
            linkedToken.Register(Action<obj>(fun _ -> doExec <- true), null) |> ignore
            Assert.True(doExec)
            
        let () =
            use cts1 = new CancellationTokenSource()
            use cts2 = new CancellationTokenSource()
            use ctsLinked = CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, cts2.Token)
            let linkedToken = ctsLinked.Token            
            let mutable doExec = false
            linkedToken.Register(Action<obj>(fun _ -> doExec <- true), null) |> ignore
            Assert.False(doExec)
            cts1.Cancel()
            Assert.True(doExec)
            
        let () =
            use cts1 = new CancellationTokenSource()
            use cts2 = new CancellationTokenSource()
            let token1 = cts1.Token
            let token2 = cts2.Token
            use ctsLinked = CancellationTokenSource.CreateLinkedTokenSource(token1, token2)                        
            let linkedToken = ctsLinked.Token    
            Assert.False(linkedToken.IsCancellationRequested)            
            ctsLinked.Cancel()
            Assert.True(linkedToken.IsCancellationRequested)
            Assert.False(token1.IsCancellationRequested)            
            Assert.False(token2.IsCancellationRequested)            
            
        ()
    
    [<Fact>]  
    member this.TestCancellationRace() =
        use cts = new CancellationTokenSource()
        let token = cts.Token
        let lockObj = obj()
        let mutable callbackRun = false
        let reg = token.Register(Action<obj>(fun _ ->
                lock lockObj (fun() ->
                    Assert.False(callbackRun, "Callback should run only once")
                    callbackRun <- true
                )
            ), null)
        Assert.False(callbackRun)
        let asyncs = seq { for i in 1..1000 do yield async { cts.Cancel() } }
        asyncs |> Async.Parallel |> Async.RunSynchronously |> ignore
        Assert.True(callbackRun, "Callback should run at least once")

    [<Fact>]
    member this.TestRegistrationRace() =
        let asyncs =
            seq { for _ in 1..1000 do
                    let cts = new CancellationTokenSource()
                    let token = cts.Token
                    yield async { cts.Cancel() } 
                    let callback (_:obj) =
                        Assert.True(token.IsCancellationRequested)
                    yield async { 
                            do token.Register(Action<obj>(callback), null) |> ignore 
                        }                     
            }               
        (asyncs |> Async.Parallel |> Async.RunSynchronously |> ignore)

    [<Fact>]
    member this.LinkedSourceCancellationRace() =
        let asyncs =
            seq { for _ in 1..1000 do
                    let cts1 = new CancellationTokenSource()
                    let token1 = cts1.Token
                    let cts2 = new CancellationTokenSource()
                    let token2 = cts2.Token
                    let linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token1, token2)
                    yield async { do cts1.Cancel() } 
                    yield async { do linkedCts.Dispose() }                     
            }               
        asyncs |> Async.Parallel |> Async.RunSynchronously |> ignore

    [<Fact>]
    member this.AwaitTaskCancellationAfterAsyncTokenCancellation() =
        let StartCatchCancellation cancellationToken (work) =
            Async.FromContinuations(fun (cont, econt, _) ->
              // When the child is cancelled, report OperationCancelled
              // as an ordinary exception to "error continuation" rather
              // than using "cancellation continuation"
              let ccont e = econt e
              // Start the workflow using a provided cancellation token
              Async.StartWithContinuations( work, cont, econt, ccont,
                                            ?cancellationToken=cancellationToken) )

        /// Like StartAsTask but gives the computation time to so some regular cancellation work
        let StartAsTaskProperCancel taskCreationOptions  cancellationToken (computation : Async<_>) : System.Threading.Tasks.Task<_> =
            let token = defaultArg cancellationToken Async.DefaultCancellationToken
            let taskCreationOptions = defaultArg taskCreationOptions System.Threading.Tasks.TaskCreationOptions.None
            let tcs = new System.Threading.Tasks.TaskCompletionSource<_>("StartAsTaskProperCancel", taskCreationOptions)

            let a =
                async {
                    try
                        // To ensure we don't cancel this very async (which is required to properly forward the error condition)
                        let! result = StartCatchCancellation (Some token) computation
                        do
                            tcs.SetResult(result)
                    with exn ->
                        tcs.SetException(exn)
                }
            Async.Start(a)
            tcs.Task

        let cts = new CancellationTokenSource()
        let tcs = System.Threading.Tasks.TaskCompletionSource<_>()
        let t =
            async {
                do! tcs.Task |> Async.AwaitTask
            }
            |> StartAsTaskProperCancel None (Some cts.Token)

        // First cancel the token, then set the task as cancelled.
        async {
            do! Async.Sleep 100
            cts.Cancel()
            do! Async.Sleep 100
            tcs.TrySetException (TimeoutException "Task timed out after token.")
                |> ignore
        } |> Async.Start

        try
            let res = t.Wait(2000)
            let msg = sprintf "Excepted TimeoutException wrapped in an AggregateException, but got %A" res
            printfn "failure msg: %s" msg
            Assert.Fail (msg)
        with :? AggregateException as agg -> ()

    [<Fact>]
    member this.Equality() =
        let cts1 = new CancellationTokenSource()
        let cts2 = new CancellationTokenSource()
        let t1a = cts1.Token
        let t1b = cts1.Token
        let t2 = cts2.Token        
        Assert.True((t1a = t1b))
        Assert.False(t1a <> t1b)
        Assert.True(t1a <> t2)
        Assert.False((t1a = t2))
        
        let r1a = t1a.Register(Action<obj>(fun _ -> ()), null)
        let r1b = t1b.Register(Action<obj>(fun _ -> ()), null)
        let r2 = t2.Register(Action<obj>(fun _ -> ()), null)
        let r1a' = r1a
        Assert.True((r1a = r1a'))
        Assert.False((r1a = r1b))
        Assert.False((r1a = r2))
        
        Assert.False((r1a <> r1a'))
        Assert.True((r1a <> r1b))
        Assert.True((r1a <> r2))


