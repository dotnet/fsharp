// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Control.MailboxProcessor type

namespace FSharp.Core.UnitTests.Control

open System
open System.Threading
open System.Threading.Tasks
open Xunit

type Message = 
    | Increment of int 
    | Fetch of AsyncReplyChannel<int> 
    | Reset

/// Bundles thread information into a type for testing StartImmediate
type StartImmediateThreadInfo =
    { Id: int
      Name: string }

/// MailboxProcessor message type for testing StartImmediate
type StartImmediateMessage =
    | GetThreadInfo of AsyncReplyChannel<StartImmediateThreadInfo>

[<Collection(nameof FSharp.Test.NotThreadSafeResourceCollection)>]
type MailboxProcessorType() =

    let getSimpleMailbox() =
        let mailbox =
            new MailboxProcessor<Message>(fun inbox ->
                    let rec loop n =
                         async { 
                            let! msg = inbox.Receive()
                                 
                            // Sleep 100ms - to validate timing out later
                            do! Async.Sleep(100)

                            match msg with
                            | Increment m -> return! loop (n + m)
                            | Reset       -> return! loop 0
                            | Fetch chan  -> do chan.Reply(n)
                                             return! loop n 
                            ()
                        }
                    loop 0
            )
        mailbox

    [<Fact>]
    member this.DefaultTimeout() =

        let mailbox = getSimpleMailbox()
        mailbox.Start()

        // Verify default is infinite
        Assert.AreEqual(mailbox.DefaultTimeout, -1)

        mailbox.Post(Reset)
        mailbox.Post(Increment(1))
        let result = mailbox.TryPostAndReply(fun chan -> Fetch chan)
        match result with 
        | Some(1) -> ()
        | None    -> Assert.Fail("Timed out")
        | _       -> Assert.Fail("Did not reply with expected value.")

        // Verify timeout when updating default timeout
        // We expect this to fail because of the 100ms sleep in the mailbox
        mailbox.DefaultTimeout <- 10
        mailbox.Post(Reset)
        mailbox.Post(Increment(1))
        let result = mailbox.TryPostAndReply(fun chan -> Fetch chan)
        match result with 
        | None    -> ()
        | _       -> Assert.Fail("Replied with a value, expected to time out.")

        ()

    [<Fact>]
    member this.``Receive handles cancellation token``() =
        let mutable result = None
        use mre1 = new ManualResetEventSlim(false)
        use mre2 = new ManualResetEventSlim(false)
    
        // https://github.com/dotnet/fsharp/issues/3337
        let cts = new CancellationTokenSource ()
    
        let addMsg msg =
            match result with
            | Some text ->
                //printfn "Got some, adding %s" msg
                result <- Some(text + " " + msg)
            | None ->
                //printfn "got none, setting %s" msg
                result <- Some msg
    
        let mb =
            MailboxProcessor.Start (
                fun inbox -> async {
                    use disp =
                        { new IDisposable with
                            member this.Dispose () =
                                addMsg "Disposed"
                                mre2.Set()
                        }
    
                    while true do
                        let! (msg : int) = inbox.Receive()
                        addMsg (sprintf "Received %i" msg)
                        mre1.Set()
                }, cancellationToken = cts.Token)
    
        mb.Post(1)
        mre1.Wait()
    
        cts.Cancel()
        mre2.Wait()

        Assert.AreEqual(Some("Received 1 Disposed"), result)

    [<Fact>]
    member this.``Receive with timeout argument handles cancellation token``() =
        let mutable result = None
        use mre1 = new ManualResetEventSlim(false)
        use mre2 = new ManualResetEventSlim(false)
    
        // https://github.com/dotnet/fsharp/issues/3337
        use cts = new CancellationTokenSource ()
    
        let addMsg msg =
            match result with
            | Some text ->
                //printfn "Got some, adding %s" msg
                result <- Some(text + " " + msg)
            | None ->
                //printfn "got none, setting %s" msg
                result <- Some msg
    
        let mb =
            MailboxProcessor.Start (
                fun inbox -> async {
                    use disp =
                        { new IDisposable with
                            member this.Dispose () =
                                addMsg "Disposed"
                                mre2.Set()
                        }
    
                    while true do
                        let! (msg : int) = inbox.Receive(100000)
                        addMsg (sprintf "Received %i" msg)
                        mre1.Set()
                }, cancellationToken = cts.Token)
    
        mb.Post(1)
        mre1.Wait()
    
        cts.Cancel()
        mre2.Wait()

        Assert.AreEqual(Some("Received 1 Disposed"), result)

    [<Fact>]
    member this.``Scan handles cancellation token``() =
        let mutable result = None
        use mre1 = new ManualResetEventSlim(false)
        use mre2 = new ManualResetEventSlim(false)

        // https://github.com/dotnet/fsharp/issues/3337
        let cts = new CancellationTokenSource ()

        let addMsg msg =
            match result with
            | Some text ->
                //printfn "Got some, adding %s" msg
                result <- Some(text + " " + msg)
            | None ->
                //printfn "got none, setting %s" msg
                result <- Some msg

        let mb =
            MailboxProcessor.Start (
                fun inbox -> async {
                    use disp =
                        { new IDisposable with
                            member this.Dispose () =
                                addMsg "Disposed"
                                mre2.Set()
                        }

                    while true do
                        let! (msg : int) = inbox.Scan (fun msg -> Some(async { return msg }) )
                        addMsg (sprintf "Scanned %i" msg)
                        mre1.Set()
                }, cancellationToken = cts.Token)

        mb.Post(1)
        mre1.Wait()

        cts.Cancel()
        mre2.Wait()

        Assert.AreEqual(Some("Scanned 1 Disposed"), result)

    [<Fact>]
    member this.``Receive Races with Post``() =
        let receiveEv = new AutoResetEvent(false)
        let postEv = new AutoResetEvent(false)
        let finishedEv = new AutoResetEvent(false)
        use cts = new CancellationTokenSource()
        let mb =
            MailboxProcessor.Start (
                fun inbox -> async {
                    while true do
                        receiveEv.WaitOne() |> ignore
                        let! (msg) = inbox.Receive ()
                        finishedEv.Set() |> ignore
                })
        let post =
            async {
                while not cts.IsCancellationRequested do
                    postEv.WaitOne() |> ignore
                    mb.Post(fun () -> ())
            } |> Async.StartAsTask
        for i in 0 .. 100000 do
            if i % 2 = 0 then
                receiveEv.Set() |> ignore
                postEv.Set() |> ignore
            else
                postEv.Set() |> ignore
                receiveEv.Set() |> ignore

            finishedEv.WaitOne() |> ignore

        cts.Cancel()
        // Let the post task finish.
        postEv.Set() |> ignore
        post.Wait()

    [<Fact>]
    member this.``Receive Races with Post on timeout``() =
        let receiveEv = new AutoResetEvent(false)
        let postEv = new AutoResetEvent(false)
        let finishedEv = new AutoResetEvent(false)
        use cts = new CancellationTokenSource()
        let mb =
            MailboxProcessor.Start (
                fun inbox -> async {
                    while true do
                        receiveEv.WaitOne() |> ignore
                        let! (msg) = inbox.Receive (5000)
                        finishedEv.Set() |> ignore
                })

        let isErrored = mb.Error |> Async.AwaitEvent |> Async.StartAsTask

        let post =
            backgroundTask {
                while not cts.IsCancellationRequested do
                    postEv.WaitOne() |> ignore
                    mb.Post(fun () -> ())
            }

        for i in 0 .. 10000 do
            if i % 2 = 0 then
                receiveEv.Set() |> ignore
                postEv.Set() |> ignore
            else
                postEv.Set() |> ignore
                receiveEv.Set() |> ignore

            while not (finishedEv.WaitOne(100)) do
                if isErrored.IsCompleted then
                    raise <| Exception("Mailbox should not fail!", isErrored.Result)

        cts.Cancel()
        // Let the post task finish.
        postEv.Set() |> ignore
        post.Wait()

    [<Fact>]
    member this.``TryReceive Races with Post on timeout``() =
        let receiveEv = new AutoResetEvent(false)
        let postEv = new AutoResetEvent(false)
        let finishedEv = new AutoResetEvent(false)
        use cts = new CancellationTokenSource()
        let mb =
            MailboxProcessor.Start (
                fun inbox -> async {
                    while true do
                        receiveEv.WaitOne() |> ignore
                        let! (msg) = inbox.TryReceive (5000)
                        finishedEv.Set() |> ignore
                }
            )

        let isErrored = mb.Error |> Async.AwaitEvent |> Async.StartAsTask

        let post =
            backgroundTask {
                while not cts.IsCancellationRequested do
                    postEv.WaitOne() |> ignore
                    mb.Post(fun () -> ())
            }

        for i in 0 .. 10000 do
            if i % 2 = 0 then
                receiveEv.Set() |> ignore
                postEv.Set() |> ignore
            else
                postEv.Set() |> ignore
                receiveEv.Set() |> ignore

            while not (finishedEv.WaitOne(100)) do
                if isErrored.IsCompleted then
                    raise <| Exception("Mailbox should not fail!", isErrored.Result)

        cts.Cancel()
        postEv.Set() |> ignore
        post.Wait()

    [<Fact>]
    member this.``After dispose is called, mailbox should stop receiving and processing messages``() = task {
        let mutable isSkip = false
        let mutable actualSkipMessagesCount = 0
        let mutable actualMessagesCount = 0
        let sleepDueTime = 100
        let expectedMessagesCount = 2
        use mre = new ManualResetEventSlim(false)
        let mb =
            MailboxProcessor.Start(fun b ->
                let rec loop() =
                     async {
                        match! b.Receive() with
                        | Increment _ ->
                            if isSkip then
                                actualSkipMessagesCount <- actualSkipMessagesCount + 1
                                return! loop()
                            else
                                do! Async.Sleep sleepDueTime
                                if not isSkip then
                                    actualMessagesCount <- actualMessagesCount + 1
                                    if actualMessagesCount = expectedMessagesCount then mre.Set()
                                    do! Async.Sleep sleepDueTime
                                return! loop()
                        | _ -> ()
                    }
                loop()
            )
        let post() = Increment 1 |> mb.Post

        [1..4] |> Seq.iter (fun x -> post())
        do! task {
            mre.Wait()
            isSkip <- true
            (mb :> IDisposable).Dispose()
            post()
        }

        Assert.Equal(expectedMessagesCount, actualMessagesCount)
        Assert.Equal(0, actualSkipMessagesCount)
        Assert.Equal(0, mb.CurrentQueueLength)
    }

    [<Fact>]
    member this.``After dispose is called, mailbox should stop receiving and processing messages with exception``() = task {
        let mutable isSkip = false
        let mutable actualSkipMessagesCount = 0
        let mutable actualMessagesCount = 0
        let sleepDueTime = 100
        let expectedMessagesCount = 2
        use mre = new ManualResetEventSlim(false)
        let mb =
            MailboxProcessor.Start((fun b ->
                let rec loop() =
                     async {
                        match! b.Receive() with
                        | Increment _ ->
                            if isSkip then
                                actualSkipMessagesCount <- actualSkipMessagesCount + 1
                                return! loop()
                            else
                                do! Async.Sleep sleepDueTime
                                if not isSkip then
                                    actualMessagesCount <- actualMessagesCount + 1
                                    if actualMessagesCount = expectedMessagesCount then mre.Set()
                                    do! Async.Sleep sleepDueTime
                                return! loop()
                        | _ -> ()
                    }
                loop()),
                true
            )
        let post() = Increment 1 |> mb.Post

        [1..4] |> Seq.iter (fun x -> post())
        do! task {
            mre.Wait()
            isSkip <- true
            (mb :> IDisposable).Dispose()
            Assert.Throws<ObjectDisposedException>(fun _ -> post()) |> ignore
        }

        Assert.Equal(expectedMessagesCount, actualMessagesCount)
        Assert.Equal(0, actualSkipMessagesCount)
        Assert.Equal(0, mb.CurrentQueueLength)

    }

    [<Fact>]
    member this.Dispose() =

        // No unit test actually hit the Dispose method for the Mailbox...
        let test() = 
            use mailbox = getSimpleMailbox()

            mailbox.Start()

            mailbox.Post(Reset)
            mailbox.Post(Increment(10))

        test()

    [<Fact(Skip="This test fails all the time in CI, likely due to magic sleeps. Need to re-evaluate.")>]
    member this.PostAndAsyncReply_Cancellation() =

        use cancel = new CancellationTokenSource(500)
        let mutable gotGood = false
        let mutable gotBad = false

        let goodAsync = async {
            try
              for i in Seq.initInfinite (fun i -> i) do
                if i % 10000000 = 0 then
                  printfn "good async working..."
            finally
              printfn "good async exited - that's what we want"
              gotGood <- true
          }

        let badAsync (mbox:MailboxProcessor<AsyncReplyChannel<int>>) = async {
            try
              printfn "bad async working..."
              let! result = mbox.PostAndAsyncReply id // <- got stuck in here forever 
              printfn "%d" result
            finally
              printfn "bad async exited - that's what we want" // <- we never got here
              gotBad <- true
          }

        let mbox = MailboxProcessor.Start(fun inbox -> async {
            let! (reply : AsyncReplyChannel<int>) = inbox.Receive()
            do! Async.Sleep 1000000
            reply.Reply (200)
          }, cancel.Token)

        [goodAsync; badAsync mbox]
        |> Async.Parallel
        |> Async.Ignore
        |> fun x -> Async.Start(x, cancel.Token)
        System.Threading.Thread.Sleep(5000) // cancellation after 500 pause for 5 seconds 
        if not gotGood || not gotBad then 
            failwith <| sprintf "Expected both good and bad async's to be cancelled afterMailbox should not fail!  gotGood: %A, gotBad: %A" gotGood gotBad

    [<Fact>]
    member this.StartImmediateStartsOnCurrentThread() =
        /// Gets the current thread's ID and name
        let getThreadInfo () =
            let currentThread = Thread.CurrentThread
            
            { Id = currentThread.ManagedThreadId
              Name = currentThread.Name }

        // Although the ManagedThreadId should be unique, go ahead and set the calling thread
        // name to something specific prior to starting the MailboxProcessor
        Thread.CurrentThread.Name <- "This is the thread that starts the MailboxProcessor"

        // Capture the ID and name of the calling thread
        let callingThreadInfo = getThreadInfo ()

        // Start a MailboxProcessor with StartImmediate and have it wait for a single message
        // requesting the information of the thread that it is running on
        let mailbox = MailboxProcessor<StartImmediateMessage>.StartImmediate(fun inbox -> async{
            // Get the MailboxProcessor's thread immediately after starting it
            let threadInfo = getThreadInfo ()
            
            // Block until a single message is received
            let! message = inbox.Receive()

            // Because the let! forces the asynchronous call Receive, the MailboxProcessor
            // is not guaranteed to be on the same thread that it started on afterwards.
            // In other words, placing getThreadInfo here or afterward will cause this test
            // to fail.

            // Reply with the MailboxProcessor's thread information
            match message with
            | GetThreadInfo reply -> reply.Reply threadInfo
        })

        // Get the MailboxProcessor's thread information
        let mailboxThreadInfo = mailbox.PostAndReply GetThreadInfo

        // Compare the calling thread information to that of the MailboxProcessor.
        // If StartImmediate worked correctly, the information should be identical since
        // the threads should be the same.
        Assert.Equal(callingThreadInfo, mailboxThreadInfo)

module MailboxProcessorType =

    [<Fact>]
    let TryScan () =
        let tcs = TaskCompletionSource<_>()
        use mailbox =
            new MailboxProcessor<Message>(fun inbox -> async {
                do! 
                    inbox.TryScan( function
                        | Reset -> async { tcs.SetResult "Reset processed" } |> Some
                        | _ -> None)
                    |> Async.Ignore
            })
        mailbox.Start()

        for i in 1 .. 100 do
            mailbox.Post(Increment i)
        mailbox.Post Reset

        Assert.Equal("Reset processed", tcs.Task.Result)
        Assert.Equal(100, mailbox.CurrentQueueLength)

    [<Fact>]
    let ``TryScan with timeout`` () =
        let tcs = TaskCompletionSource<_>()
        use mailbox =
            new MailboxProcessor<Message>(fun inbox ->
                let rec loop i = async {
                    match!                     
                        inbox.TryScan( function
                            | Reset -> async { tcs.SetResult i } |> Some
                            | _ -> None)
                    with
                    | None -> do! loop (i + 1)
                    | _ -> ()
                }
                loop 1
            )
        mailbox.DefaultTimeout <- 10
        mailbox.Start()

        let iteration = 
            task { 
                for i in 1 .. 100 do
                    mailbox.Post(Increment 1)
                    do! Task.Delay 10
                mailbox.Post Reset

                return! tcs.Task
            }

        Assert.True(iteration.Result > 1, "TryScan did not timeout")

    let ordered() =
        let mutable current = 1
        fun n ->
            if n < current then failwith $"step {n} already happened"
            SpinWait.SpinUntil(fun () -> n = current)
            current <- n + 1

    // See https://github.com/dotnet/fsharp/issues/17849
    [<Fact>]
    let ``Disposed MailboxProcessor does not throw on Post`` () =
        task {
            let step = ordered()
      
            let cts = new CancellationTokenSource()
            let mb =
                MailboxProcessor.Start( (fun inbox ->
                    async {
                        step 1
                        do! inbox.Receive()
                        do! inbox.Receive()
                        return ()
                    }),
                    cancellationToken = cts.Token
                )

            step 2
            // ensure pulse gets created
            do! Task.Delay 100
            mb.Post()

            mb.Dispose()

            do! Task.Delay 100

            mb.Post()

            cts.Cancel()
        }
