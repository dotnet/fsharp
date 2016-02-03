// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open System.Threading

/// Represents the capability to schedule work in the compiler service operations queue for the compilation thread
type internal IReactorOperations = 

    /// Put the operation in thq queue, and return an async handle to its result. 
    abstract EnqueueAndAwaitOpAsync : description: string * action: (CancellationToken -> 'T) -> Async<'T>

    /// Enqueue an operation and return immediately. 
    abstract EnqueueOp: description: string * action: (unit -> unit) -> unit

/// Reactor is intended for long-running but interruptible operations, interleaved
/// with one-off asynchronous operations. 
///
/// It is used to guard the global compiler state while maintaining  responsiveness on 
/// the UI thread.
/// Reactor operations
[<Sealed>]
type internal Reactor =

    /// Set the background building function, which is called repeatedly
    /// until it returns 'false'.  If None then no background operation is used.
    member SetBackgroundOp : build:(unit -> bool) option -> unit

    /// Block until the current implicit background build is complete. Unit test only.
    member WaitForBackgroundOpCompletion : unit -> unit

    /// Block until all operations in the queue are complete
    member CompleteAllQueuedOps : unit -> unit

    /// Enqueue an uncancellable operation and return immediately. 
    member EnqueueOp : description: string * op:(unit -> unit) -> unit

    /// For debug purposes
    member CurrentQueueLength : int

    /// Put the operation in the queue, and return an async handle to its result. 
    member EnqueueAndAwaitOpAsync : description: string * (CancellationToken -> 'T) -> Async<'T>

    /// The timespan in milliseconds before background work begins after the operations queue is empty
    member PauseBeforeBackgroundWork : int with get, set

    /// Get the reactor for FSharp.Compiler.dll
    static member Singleton : Reactor
  
