// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices
open System
open System.Diagnostics
open System.Globalization
open System.Threading
open Microsoft.FSharp.Control
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Lib

/// Represents the capability to schedule work in the compiler service operations queue for the compilation thread
type internal IReactorOperations = 
    abstract EnqueueAndAwaitOpAsync : string * (CancellationToken -> 'T) -> Async<'T>
    abstract EnqueueOp: string * (unit -> unit) -> unit

[<NoEquality; NoComparison>]
type internal ReactorCommands = 
    /// Kick off a build.
    | SetBackgroundOp of (unit -> bool)  option
    /// Do some work not synchronized in the mailbox.
    | Op of string * CancellationToken * (unit -> unit) * (unit -> unit)
    /// Finish the background building
    | WaitForBackgroundOpCompletion of AsyncReplyChannel<unit>            
    /// Finish all the queued ops
    | CompleteAllQueuedOps of AsyncReplyChannel<unit>            
        
[<AutoSerializable(false);Sealed>]
/// There is one global Reactor for the entire language service, no matter how many projects or files
/// are open. 
type Reactor() = 
    static let pauseBeforeBackgroundWorkDefault = GetEnvInteger "FCS_PauseBeforeBackgroundWorkMilliseconds" 1000
    static let theReactor = Reactor()
    let mutable pauseBeforeBackgroundWork = pauseBeforeBackgroundWorkDefault

    // We need to store the culture for the VS thread that is executing now,
    // so that when the reactor picks up a thread from the threadpool we can set the culture
    let culture = new CultureInfo(CultureInfo.CurrentUICulture.Name)

    /// Mailbox dispatch function.
    let builder = 
        MailboxProcessor<_>.Start <| fun inbox ->

        // Async workflow which receives messages and dispatches to worker functions.
        let rec loop (bgOpOpt, onComplete, bg) = 
            async { Trace.TraceInformation("Reactor: receiving..., remaining {0}, mem {1}, gc2 {2}", inbox.CurrentQueueLength, GC.GetTotalMemory(false)/1000000L, GC.CollectionCount(2))

                    // Messages always have priority over the background op.
                    let! msg = 
                        async { match bgOpOpt, onComplete with 
                                | None, None -> 
                                    let! msg = inbox.Receive() 
                                    return Some msg 
                                | _, Some _ -> 
                                    return! inbox.TryReceive(0) 
                                | Some _, _ -> 
                                    let timeout = (if bg then 0 else pauseBeforeBackgroundWork)
                                    return! inbox.TryReceive(timeout) }
#if FX_RESHAPED_GLOBALIZATION
                    CultureInfo.CurrentUICulture <- culture
#else
                    Thread.CurrentThread.CurrentUICulture <- culture
#endif
                    match msg with
                    | Some (SetBackgroundOp bgOpOpt) -> 
                        Trace.TraceInformation("Reactor: --> set background op, remaining {0}, mem {1}, gc2 {2}", inbox.CurrentQueueLength, GC.GetTotalMemory(false)/1000000L, GC.CollectionCount(2))
                        return! loop (bgOpOpt, onComplete, false)
                    | Some (Op (desc, ct, op, ccont)) -> 
                        if ct.IsCancellationRequested then ccont() else
                        Trace.TraceInformation("Reactor: --> {0}, remaining {1}, mem {2}, gc2 {3}", desc, inbox.CurrentQueueLength, GC.GetTotalMemory(false)/1000000L, GC.CollectionCount(2))
                        let time = System.DateTime.Now
                        op()
                        let span = System.DateTime.Now - time
                        //if span.TotalMilliseconds > 100.0 then 
                        Trace.TraceInformation("Reactor: <-- {0}, remaining {1}, took {2}ms", desc, inbox.CurrentQueueLength, span.TotalMilliseconds)
                        return! loop (bgOpOpt, onComplete, false)
                    | Some (WaitForBackgroundOpCompletion channel) -> 
                        Trace.TraceInformation("Reactor: --> wait for background (debug only), remaining {0}, mem {1}, gc2 {2}", inbox.CurrentQueueLength, GC.GetTotalMemory(false)/1000000L, GC.CollectionCount(2))
                        match bgOpOpt with 
                        | None -> ()
                        | Some bgOp -> while bgOp() do ()
                        channel.Reply(())
                        return! loop (None, onComplete, false)
                    | Some (CompleteAllQueuedOps channel) -> 
                        Trace.TraceInformation("Reactor: --> stop background work and complete all queued ops, remaining {0}, mem {1}, gc2 {2}", inbox.CurrentQueueLength, GC.GetTotalMemory(false)/1000000L, GC.CollectionCount(2))
                        return! loop (None, Some channel, false)
                    | None -> 
                        match bgOpOpt, onComplete with 
                        | _, Some onComplete -> onComplete.Reply()
                        | Some bgOp, None -> 
                            Trace.TraceInformation("Reactor: --> background step, remaining {0}, mem {1}, gc2 {2}", inbox.CurrentQueueLength, GC.GetTotalMemory(false)/1000000L, GC.CollectionCount(2))
                            let time = System.DateTime.Now
                            let res = bgOp()
                            let span = System.DateTime.Now - time
                            //if span.TotalMilliseconds > 100.0 then 
                            Trace.TraceInformation("Reactor: <-- background step, remaining {0}, took {1}ms", inbox.CurrentQueueLength, span.TotalMilliseconds)
                            return! loop ((if res then Some bgOp else None), onComplete, true)
                        | None, None -> failwith "unreachable, should have used inbox.Receive"
                    }
        async { 
            while true do 
                try 
                    do! loop (None, None, false)
                with e -> 
                    Debug.Assert(false,String.Format("unexpected failure in reactor loop {0}, restarting", e))
        }

    // [Foreground Mailbox Accessors] -----------------------------------------------------------                
    member r.SetBackgroundOp(build) = 
        Trace.TraceInformation("Reactor: enqueue start background, length {0}", builder.CurrentQueueLength)
        builder.Post(SetBackgroundOp build)

    member r.EnqueueOp(desc, op) =
        Trace.TraceInformation("Reactor: enqueue {0}, length {1}", desc, builder.CurrentQueueLength)
        builder.Post(Op(desc, CancellationToken.None, op, (fun () -> ()))) 

    member r.EnqueueOpPrim(desc, ct, op, ccont) =
        Trace.TraceInformation("Reactor: enqueue {0}, length {1}", desc, builder.CurrentQueueLength)
        builder.Post(Op(desc, ct, op, ccont)) 

    member r.CurrentQueueLength =
        builder.CurrentQueueLength

    // This is for testing only
    member r.WaitForBackgroundOpCompletion() =
        Trace.TraceInformation("Reactor: enqueue wait for background, length {0}", builder.CurrentQueueLength)
        builder.PostAndReply WaitForBackgroundOpCompletion 

    // This is for testing only
    member r.CompleteAllQueuedOps() =
        Trace.TraceInformation("Reactor: enqueue wait for all ops, length {0}", builder.CurrentQueueLength)
        builder.PostAndReply CompleteAllQueuedOps

    member r.EnqueueAndAwaitOpAsync (desc, f) = 
        async { 
            let! ct = Async.CancellationToken
            let resultCell = AsyncUtil.AsyncResultCell<_>()
            r.EnqueueOpPrim(desc, ct,
                op=(fun () ->
                    let result =
                        try
                            f ct |> AsyncUtil.AsyncOk
                        with
                        |   e -> e |> AsyncUtil.AsyncException
                    resultCell.RegisterResult(result)),
                    ccont=(fun () -> resultCell.RegisterResult (AsyncUtil.AsyncCanceled(OperationCanceledException())) )

            )
            return! resultCell.AsyncResult 
        }
    member __.PauseBeforeBackgroundWork with get() = pauseBeforeBackgroundWork and set v = pauseBeforeBackgroundWork <- v

    static member Singleton = theReactor 

