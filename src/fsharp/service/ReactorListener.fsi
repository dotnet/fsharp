// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.SourceCodeServices

open System

/// Interface for listening to events on the FCS reactor thread.
type public IReactorListener =
    /// Called when the reactor queue is empty, but there is background work to be done.
    /// If no foreground work appears in the queue in the next <paramref name="pauseMillis"/> milliseconds, the background work will start.
    /// Always called from the reactor thread.
    abstract OnReactorPauseBeforeBackgroundWork : pauseMillis: int -> unit

    /// Called when a foreground reactor operation starts.
    /// Always called from the reactor thread.
    abstract OnReactorOperationStart : userOpName: string -> opName: string -> opArg: string -> approxQueueLength: int -> unit

    /// Called when a foreground reactor operation ends.
    /// Always called from the reactor thread.
    abstract OnReactorOperationEnd : userOpName: string -> opName: string -> elapsed: TimeSpan -> unit

    /// Called when a background reactor operation starts.
    /// Always called from the reactor thread.
    abstract OnReactorBackgroundStart: bgUserOpName: string -> bgOpName: string -> bgOpArg: string -> unit

    /// Called when a background operation is cancelled.
    /// Always called from the reactor thread.
    abstract OnReactorBackgroundCancelled : bgUserOpName: string -> bgOpName: string -> unit

    /// Called when a background operation ends.
    /// This is still called even if the operation was cancelled.
    /// Always called from the reactor thread.
    abstract OnReactorBackgroundEnd : bgUserOpName: string -> bgOpName: string -> elapsed: TimeSpan -> unit

    /// Called when a background operation is set.
    /// This can be called from ANY thread - implementations must be thread safe.
    abstract OnSetBackgroundOp : approxQueueLength: int -> unit

    /// Called when a background operation is requested to be cancelled.
    /// This can be called from ANY thread - implementations must be thread safe.
    abstract OnCancelBackgroundOp : unit -> unit

    /// Called when an operation is queued to be ran on the reactor.
    /// This can be called from ANY thread - implementations must be thread safe.
    abstract OnEnqueueOp : userOpName: string -> opName: string -> opArg: string -> approxQueueLength: int -> unit

/// Reactor listener that does nothing.
/// Should be used as a base class for any implementers of IReactorListener.
[<Class>]
type public EmptyReactorListener =
    new : unit -> EmptyReactorListener
    interface IReactorListener

/// Default reactor listener.
/// Writes debug output using <see cref="System.Diagnostics.Trace" />
[<Class>]
type public DefaultReactorListener =
    new : unit -> DefaultReactorListener
    interface IReactorListener
