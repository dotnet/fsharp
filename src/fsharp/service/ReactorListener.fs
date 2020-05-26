// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open System
open System.Diagnostics

type public IReactorListener =
    abstract OnReactorPauseBeforeBackgroundWork : pauseMillis: int -> unit
    abstract OnReactorOperationStart : userOpName: string -> opName: string -> opArg: string -> approxQueueLength: int -> unit
    abstract OnReactorOperationEnd : userOpName: string -> opName: string -> opArg: string -> elapsed: TimeSpan -> unit
    abstract OnReactorBackgroundStart: bgUserOpName: string -> bgOpName: string -> bgOpArg: string -> unit
    abstract OnReactorBackgroundCancelled : bgUserOpName: string -> bgOpName: string -> bgOpArg: string -> unit
    abstract OnReactorBackgroundEnd : bgUserOpName: string -> bgOpName: string -> bgOpArg: string -> elapsed: TimeSpan -> unit

    abstract OnSetBackgroundOp : approxQueueLength: int -> unit
    abstract OnCancelBackgroundOp : unit -> unit
    abstract OnEnqueueOp : userOpName: string -> opName: string -> opArg: string -> approxQueueLength: int -> unit

type public EmptyReactorListener() =
    interface IReactorListener with
        override _.OnReactorPauseBeforeBackgroundWork _ = ()
        override _.OnReactorOperationStart _ _ _ _  = ()
        override _.OnReactorOperationEnd _ _ _ _ = ()
        override _.OnReactorBackgroundStart _ _ _ = ()
        override _.OnReactorBackgroundCancelled _ _ _ = ()
        override _.OnReactorBackgroundEnd _ _ _ _ = ()
        override _.OnSetBackgroundOp _ = ()
        override _.OnCancelBackgroundOp () = ()
        override _.OnEnqueueOp _ _ _ _ = ()

type public DefaultReactorListener() =
    interface IReactorListener with
        override _.OnReactorPauseBeforeBackgroundWork pauseMillis =
            Trace.TraceInformation("Reactor: {0:n3} pausing {1} milliseconds", DateTime.Now.TimeOfDay.TotalSeconds, pauseMillis)
        override _.OnReactorOperationStart userOpName opName opArg approxQueueLength =
            Trace.TraceInformation("Reactor: {0:n3} --> {1}.{2} ({3}), remaining {4}", DateTime.Now.TimeOfDay.TotalSeconds, userOpName, opName, opArg, approxQueueLength)
        override _.OnReactorOperationEnd userOpName opName _opArg elapsed =
            let taken = elapsed.TotalMilliseconds
            let msg = (if taken > 10000.0 then "BAD-OP: >10s " elif taken > 3000.0 then "BAD-OP: >3s " elif taken > 1000.0 then "BAD-OP: > 1s " elif taken > 500.0 then "BAD-OP: >0.5s " else "")
            Trace.TraceInformation("Reactor: {0:n3} {1}<-- {2}.{3}, took {4} ms", DateTime.Now.TimeOfDay.TotalSeconds, msg, userOpName, opName, taken)
        override _.OnReactorBackgroundStart bgUserOpName bgOpName bgOpArg =
            Trace.TraceInformation("Reactor: {0:n3} --> background step {1}.{2} ({3})", DateTime.Now.TimeOfDay.TotalSeconds, bgUserOpName, bgOpName, bgOpArg)
        override _.OnReactorBackgroundCancelled bgUserOpName bgOpName _bgOpArg =
            Trace.TraceInformation("FCS: <-- background step {0}.{1}, was cancelled", bgUserOpName, bgOpName)
        override _.OnReactorBackgroundEnd _bgUserOpName _bgOpName _bgOpArg elapsed =
            let taken = elapsed.TotalMilliseconds
            let msg = (if taken > 10000.0 then "BAD-BG-SLICE: >10s " elif taken > 3000.0 then "BAD-BG-SLICE: >3s " elif taken > 1000.0 then "BAD-BG-SLICE: > 1s " else "")
            Trace.TraceInformation("Reactor: {0:n3} {1}<-- background step, took {2} ms", DateTime.Now.TimeOfDay.TotalSeconds, msg, taken)
        override _.OnSetBackgroundOp approxQueueLength =
            Trace.TraceInformation("Reactor: {0:n3} enqueue start background, length {1}", DateTime.Now.TimeOfDay.TotalSeconds, approxQueueLength)
        override _.OnCancelBackgroundOp () =
            Trace.TraceInformation("FCS: trying to cancel any active background work")
        override _.OnEnqueueOp userOpName opName opArg approxQueueLength =
            Trace.TraceInformation("Reactor: {0:n3} enqueue {1}.{2} ({3}), length {4}", DateTime.Now.TimeOfDay.TotalSeconds, userOpName, opName, opArg, approxQueueLength)
