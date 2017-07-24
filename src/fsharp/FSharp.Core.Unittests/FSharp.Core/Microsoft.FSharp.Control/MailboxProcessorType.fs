// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Various tests for the:
// Microsoft.FSharp.Control.MailboxProcessor type

namespace FSharp.Core.Unittests.FSharp_Core.Microsoft_FSharp_Control

open System
open FSharp.Core.Unittests.LibraryTestFx
open NUnit.Framework
open System.Threading
open System.Collections.Generic

type Message = 
    | Increment of int 
    | Fetch of AsyncReplyChannel<int> 
    | Reset

[<TestFixture>]
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

    [<Test>]
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

    [<Test>]


    [<Test>]
    member this.``Scan handles cancellation token``() =
        let result = ref None

        // https://github.com/Microsoft/visualfsharp/issues/3337
        let cts = new CancellationTokenSource ()

        let addMsg msg =
            match !result with
            | Some text -> result := Some(text + " " + msg)
            | None -> result := Some msg

        let mb =
            MailboxProcessor.Start (
                fun inbox -> async {
                    use disp =
                        { new IDisposable with
                            member this.Dispose () =
                                addMsg "Disposed"
                        }

                    while true do
                        let! (msg : int) = inbox.Scan (fun msg -> Some(async { return msg }) )
                        addMsg (sprintf "Scanned %i" msg)
                }, cancellationToken = cts.Token)

        mb.Post 1
        Thread.Sleep 1000
        cts.Cancel ()
        Thread.Sleep 4000

        Assert.AreEqual(Some("Scanned 1 Disposed"), !result)

    member this.Dispose() =

        // No unit test actually hit the Dispose method for the Mailbox...
        let test() = 
            use mailbox = getSimpleMailbox()

            mailbox.Start()

            mailbox.Post(Reset)
            mailbox.Post(Increment(10))

        test()
