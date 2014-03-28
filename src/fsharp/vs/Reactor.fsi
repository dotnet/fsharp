// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

/// Reactor is intended to long-running, but interruptible operations to be interleaved
/// with one-off synchronous or asynchronous operations. 
///
/// It is used to guard the global compiler state while maintaining  responsiveness on 
/// the UI thread.
module internal Reactor = 
    
    /// Does one unit of work and returns true if there is more work to do.
    type BuildStepper = unit -> (* keep building *) bool
    
    /// A synchronous or asynchronous operation to perform
    type Operation = unit -> unit

    /// Reactor operations
    [<Sealed>]
    type Reactor =
        /// Start building. The build function will return true if there is more work to do.
        member StartBuilding : build:BuildStepper -> unit
        /// Start building the most recently building thing.
        member StartBuildingRecent : unit -> unit
        /// Halt the current build.
        member StopBuilding : unit -> unit
        /// Block until the current build is complete.
        member WaitForBackgroundCompile : unit -> unit
        /// Block while performing and operation. Restart the most recent build afterward.
        member SyncOp : op:Operation -> unit
        /// Start an operation and return immediately. Restart the most recent build after the operation is complete.
        member AsyncOp : op:Operation -> unit
    
        /// Block while performing and operation. Restart the most recent build afterward.
        member RunSyncOp : (unit -> 'T) -> 'T

        /// Start an operation and return an async handle to its result. 
        member RunAsyncOp : (unit -> 'T) -> Async<'T>

    /// Get the reactor for FSharp.Compiler.dll
    val Reactor : unit -> Reactor
  
