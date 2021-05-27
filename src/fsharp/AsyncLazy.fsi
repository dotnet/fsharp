// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal rec FSharp.Compiler.AsyncLazy

// This is a port of AsyncLazy from Roslyn.

open System
open System.Threading
open System.Threading.Tasks

/// <summary>
/// Represents a value that can be retrieved synchronously or asynchronously by many clients.
/// The value will be computed on-demand the moment the first client asks for it. While being
/// computed, more clients can request the value. As long as there are outstanding clients the
/// underlying computation will proceed.  If all outstanding clients cancel their request then
/// the underlying value computation will be cancelled as well.
/// 
/// Creators of an <see cref="AsyncLazy{T}" /> can specify whether the result of the computation is
/// cached for future requests or not. Choosing to not cache means the computation functions are kept
/// alive, whereas caching means the value (but not functions) are kept alive once complete.
/// </summary>
[<Sealed>]
type AsyncLazy<'T> =

    /// <summary>
    /// Creates an AsyncLazy that supports both asynchronous computation and inline synchronous
    /// computation.
    /// </summary>
    /// <param name="asynchronousComputeFunction">A function called to start the asynchronous
    /// computation. This function should be cheap and non-blocking.</param>
    /// <param name="synchronousComputeFunction">A function to do the work synchronously, which
    /// is allowed to block. This function should not be implemented by a simple Wait on the
    /// asynchronous value. If that's all you are doing, just don't pass a synchronous function
    /// in the first place.</param>
    /// <param name="cacheResult">Whether the result should be cached once the computation is
    /// complete.</param>
    new: asynchronousComputeFunction: Func<CancellationToken,Task<'T>> * 
         synchronousComputeFunction: Func<CancellationToken, 'T> * 
         cacheResult: bool -> AsyncLazy<'T>

    member TryGetValue: unit -> 'T voption

    member GetValue: CancellationToken -> 'T

    member GetValueAsync: CancellationToken -> Task<'T>