// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open System
open System.Diagnostics
open System.Globalization
open System.Threading

open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Lib

/// Represents the capability to schedule work in the compiler service operations queue for the compilation thread
type internal IReactorOperations = 
    abstract EnqueueAndAwaitOpAsync : userOpName:string * opName:string * opArg:string * (CompilationThreadToken -> Cancellable<'T>) -> Async<'T>
    abstract EnqueueOp: userOpName:string * opName:string * opArg:string * (CompilationThreadToken -> unit) -> unit

[<NoEquality; NoComparison>]
type internal ReactorCommands = 
    /// Kick off a build.
    | SetBackgroundOp of ( (* userOpName: *) string * (* opName: *) string * (* opArg: *) string * (CompilationThreadToken -> CancellationToken -> bool)) option

    /// Do some work not synchronized in the mailbox.
    | Op of userOpName: string * opName: string * opArg: string * CancellationToken * (CompilationThreadToken -> unit) * (unit -> unit)

    /// Finish the background building
    | WaitForBackgroundOpCompletion of AsyncReplyChannel<unit>            

    /// Finish all the queued ops
    | CompleteAllQueuedOps of AsyncReplyChannel<unit>            
        
[<AutoSerializable(false);Sealed>]
/// There is one global Reactor for the entire language service, no matter how many projects or files
/// are open. 
type Reactor() = 
    static let pauseBeforeBackgroundWorkDefault = GetEnvInteger "FCS_PauseBeforeBackgroundWorkMilliseconds" 10
    static let theReactor = Reactor()

    let mutable listener: IReactorListener = DefaultReactorListener() :> _

    let mutable pauseBeforeBackgroundWork = pauseBeforeBackgroundWorkDefault

    // We need to store the culture for the VS thread that is executing now, 
    // so that when the reactor picks up a thread from the thread pool we can set the culture
    let mutable culture = CultureInfo(CultureInfo.CurrentUICulture.Name)

    let mutable bgOpCts = new CancellationTokenSource()
    /// Mailbox dispatch function.
    let builder = 
        MailboxProcessor<_>.Start <| fun inbox ->

        // Async workflow which receives messages and dispatches to worker functions.
        let rec loop (bgOpOpt, onComplete, bg) = 
            async { //Trace.TraceInformation("Reactor: receiving..., remaining {0}", inbox.CurrentQueueLength)

                    // Explanation: The reactor thread acts as the compilation thread in hosted scenarios
                    let ctok = AssumeCompilationThreadWithoutEvidence()

                    // Messages always have priority over the background op.
                    let! msg = 
                        async { match bgOpOpt, onComplete with 
                                | None, None -> 
                                    let! msg = inbox.Receive() 
                                    return Some msg 
                                | _, Some _ -> 
                                    return! inbox.TryReceive(0) 
                                | Some _, _ -> 
                                    let timeout = 
                                        if bg then 0 
                                        else
                                            listener.OnReactorPauseBeforeBackgroundWork pauseBeforeBackgroundWork
                                            pauseBeforeBackgroundWork
                                    return! inbox.TryReceive(timeout) }
                    Thread.CurrentThread.CurrentUICulture <- culture
                    match msg with
                    | Some (SetBackgroundOp bgOpOpt) -> 
                        //Trace.TraceInformation("Reactor: --> set background op, remaining {0}", inbox.CurrentQueueLength)
                        return! loop (bgOpOpt, onComplete, false)

                    | Some (Op (userOpName, opName, opArg, ct, op, ccont)) -> 
                        if ct.IsCancellationRequested then ccont() else
                        listener.OnReactorOperationStart userOpName opName opArg inbox.CurrentQueueLength
                        let time = Stopwatch()
                        try
                            time.Start()
                            op ctok
                            time.Stop()
                        finally
                            listener.OnReactorOperationEnd userOpName opName opArg time.Elapsed
                        return! loop (bgOpOpt, onComplete, false)
                    | Some (WaitForBackgroundOpCompletion channel) -> 
                        match bgOpOpt with 
                        | None -> ()
                        | Some (bgUserOpName, bgOpName, bgOpArg, bgOp) -> 
                            Trace.TraceInformation("Reactor: {0:n3} --> wait for background {1}.{2} ({3}), remaining {4}", DateTime.Now.TimeOfDay.TotalSeconds, bgUserOpName, bgOpName, bgOpArg, inbox.CurrentQueueLength)
                            bgOpCts.Dispose()
                            bgOpCts <- new CancellationTokenSource()
                            while not bgOpCts.IsCancellationRequested && bgOp ctok bgOpCts.Token do 
                                ()

                            if bgOpCts.IsCancellationRequested then 
                                Trace.TraceInformation("FCS: <-- wait for background was cancelled {0}.{1}", bgUserOpName, bgOpName)

                        channel.Reply(())
                        return! loop (None, onComplete, false)

                    | Some (CompleteAllQueuedOps channel) -> 
                        Trace.TraceInformation("Reactor: {0:n3} --> stop background work and complete all queued ops, remaining {1}", DateTime.Now.TimeOfDay.TotalSeconds, inbox.CurrentQueueLength)
                        return! loop (None, Some channel, false)

                    | None -> 
                        match bgOpOpt, onComplete with 
                        | _, Some onComplete -> onComplete.Reply()
                        | Some  (bgUserOpName, bgOpName, bgOpArg, bgOp), None ->
                            listener.OnReactorBackgroundStart bgUserOpName bgOpName bgOpArg
                            let time = Stopwatch()
                            let res =
                                try
                                    time.Start()
                                    bgOpCts.Dispose()
                                    bgOpCts <- new CancellationTokenSource()
                                    let res = bgOp ctok bgOpCts.Token
                                    time.Stop()
                                    if bgOpCts.IsCancellationRequested then 
                                        listener.OnReactorBackgroundCancelled bgUserOpName bgOpName bgOpArg
                                    res
                                finally
                                    listener.OnReactorBackgroundEnd bgUserOpName bgOpName bgOpArg time.Elapsed

                            return! loop ((if res then bgOpOpt else None), onComplete, true)
                        | None, None -> failwith "unreachable, should have used inbox.Receive"
                    }
        async { 
            while true do 
                try 
                    do! loop (None, None, false)
                with e -> 
                    Debug.Assert(false, String.Format("unexpected failure in reactor loop {0}, restarting", e))
        }

    member __.SetPreferredUILang(preferredUiLang: string option) = 
        match preferredUiLang with
        | Some s -> 
            culture <- CultureInfo s
#if FX_RESHAPED_GLOBALIZATION
            CultureInfo.CurrentUICulture <- culture
#else
            Thread.CurrentThread.CurrentUICulture <- culture
#endif
        | None -> ()

    // [Foreground Mailbox Accessors] -----------------------------------------------------------                
    member r.SetBackgroundOp(bgOpOpt) =
        listener.OnSetBackgroundOp builder.CurrentQueueLength
        bgOpCts.Cancel()
        builder.Post(SetBackgroundOp bgOpOpt)

    member r.CancelBackgroundOp() = 
        listener.OnCancelBackgroundOp()
        bgOpCts.Cancel()

    member r.EnqueueOp(userOpName, opName, opArg, op) =
        listener.OnEnqueueOp userOpName opName opArg builder.CurrentQueueLength
        builder.Post(Op(userOpName, opName, opArg, CancellationToken.None, op, (fun () -> ()))) 

    member r.EnqueueOpPrim(userOpName, opName, opArg, ct, op, ccont) =
        listener.OnEnqueueOp userOpName opName opArg builder.CurrentQueueLength
        builder.Post(Op(userOpName, opName, opArg, ct, op, ccont)) 

    member r.CurrentQueueLength =
        builder.CurrentQueueLength

    member r.SetListener(newListener) =
        listener <- newListener

    // This is for testing only
    member r.WaitForBackgroundOpCompletion() =
        Trace.TraceInformation("Reactor: {0:n3} enqueue wait for background, length {0}", DateTime.Now.TimeOfDay.TotalSeconds, builder.CurrentQueueLength)
        builder.PostAndReply WaitForBackgroundOpCompletion 

    // This is for testing only
    member r.CompleteAllQueuedOps() =
        Trace.TraceInformation("Reactor: {0:n3} enqueue wait for all ops, length {0}", DateTime.Now.TimeOfDay.TotalSeconds, builder.CurrentQueueLength)
        builder.PostAndReply CompleteAllQueuedOps

    member r.EnqueueAndAwaitOpAsync (userOpName, opName, opArg, f) = 
        async { 
            let! ct = Async.CancellationToken
            let resultCell = AsyncUtil.AsyncResultCell<_>()
            r.EnqueueOpPrim(userOpName, opName, opArg, ct, 
                op=(fun ctok ->
                    let result =
                        try 
                          match Cancellable.run ct (f ctok) with 
                          | ValueOrCancelled.Value r -> AsyncUtil.AsyncOk r
                          | ValueOrCancelled.Cancelled e -> AsyncUtil.AsyncCanceled e
                        with e -> e |> AsyncUtil.AsyncException

                    resultCell.RegisterResult(result)), 
                    ccont=(fun () -> resultCell.RegisterResult (AsyncUtil.AsyncCanceled(OperationCanceledException(ct))) )

            )
            return! resultCell.AsyncResult 
        }
    member __.PauseBeforeBackgroundWork with get() = pauseBeforeBackgroundWork and set v = pauseBeforeBackgroundWork <- v

    static member Singleton = theReactor 

