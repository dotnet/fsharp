// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module internal rec FSharp.Compiler.AsyncLazy

// This is a port of AsyncLazy from Roslyn.

open System
open System.Threading
open System.Threading.Tasks
open System.Diagnostics
open System.Collections.Generic
open System.Runtime.ExceptionServices

[<Struct;NoEquality;NoComparison>]
type SemaphoreDisposer(semaphore: NonReentrantLock) =
    
    interface IDisposable with
        [<DebuggerHidden>]
        member _.Dispose() = semaphore.Release()

/// <summary>
/// A lightweight mutual exclusion object which supports waiting with cancellation and prevents
/// recursion (i.e. you may not call Wait if you already hold the lock)
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="NonReentrantLock"/> provides a lightweight mutual exclusion class that doesn't
/// use Windows kernel synchronization primitives.
/// </para>
/// <para>
/// The implementation is distilled from the workings of <see cref="SemaphoreSlim"/>
/// The basic idea is that we use a regular sync object (Monitor.Enter/Exit) to guard the setting
/// of an 'owning thread' field. If, during the Wait, we find the lock is held by someone else
/// then we register a cancellation callback and enter a "Monitor.Wait" loop. If the cancellation
/// callback fires, then it "pulses" all the waiters to wake them up and check for cancellation.
/// Waiters are also "pulsed" when leaving the lock.
/// </para>
/// <para>
/// All public members of <see cref="NonReentrantLock"/> are thread-safe and may be used concurrently
/// from multiple threads.
/// </para>
/// </remarks>
[<Sealed>]
type NonReentrantLock(useThisInstanceForSynchronization: bool) as this =

    /// <summary>
    /// A synchronization object to protect access to the <see cref="_owningThreadId"/> field and to be pulsed
    /// when <see cref="Release"/> is called and during cancellation.
    /// </summary>
    let _syncLock =
        if useThisInstanceForSynchronization then this :> obj
        else obj()

    /// <summary>
    /// The <see cref="Environment.CurrentManagedThreadId" /> of the thread that holds the lock. Zero if no thread is holding
    /// the lock.
    /// </summary>
    [<VolatileField>]
    let mutable _owningThreadId = Unchecked.defaultof<_>

    static let s_cancellationTokenCanceledEventHandler: Action<obj> = Action<_>(NonReentrantLock.CancellationTokenCanceledEventHandler)

    /// <summary>
    /// Checks if the lock is currently held.
    /// </summary>
    [<DebuggerHidden>]
    member this.IsLocked = _owningThreadId <> 0

    /// <summary>
    /// Checks if the lock is currently held by the calling thread.
    /// </summary>
    [<DebuggerHidden>]
    member this.IsOwnedByMe = _owningThreadId = Environment.CurrentManagedThreadId

    /// <summary>
    /// Take ownership of the lock (by the calling thread). The lock may not already
    /// be held by any other code.
    /// </summary>
    [<DebuggerHidden>]
    member this.TakeOwnership() =
        Debug.Assert(not this.IsLocked)
        _owningThreadId <- Environment.CurrentManagedThreadId

    /// <summary>
    /// Release ownership of the lock. The lock must already be held by the calling thread.
    /// </summary>
    [<DebuggerHidden>]
    member this.ReleaseOwnership() =
        Debug.Assert(this.IsOwnedByMe)
        _owningThreadId <- 0

    /// <summary>
    /// Determine if the lock is currently held by the calling thread.
    /// </summary>
    /// <returns>True if the lock is currently held by the calling thread.</returns>
    [<DebuggerHidden>]
    member this.LockHeldByMe() =
        this.IsOwnedByMe

    /// <summary>
    /// Throw an exception if the lock is not held by the calling thread.
    /// </summary>
    /// <exception cref="InvalidOperationException">The lock is not currently held by the calling thread.</exception>
    [<DebuggerHidden>]
    member this.AssertHasLock() =
        if not (this.LockHeldByMe()) then
            invalidOp "The lock is not currently held by the calling thread."

    /// <summary>
    /// Callback executed when a cancellation token is canceled during a Wait.
    /// </summary>
    /// <param name="o">The syncLock that protects a <see cref="NonReentrantLock"/> instance.</param>
    [<DebuggerHidden>]
    static member CancellationTokenCanceledEventHandler(o: obj) =
        Debug.Assert(o <> null)
        lock o (fun () -> 
            // Release all waiters to check their cancellation tokens.
            Monitor.PulseAll(o)
        )

    [<DebuggerHidden>]
    member this.DisposableWait(cancellationToken: CancellationToken) =
        this.Wait(cancellationToken)
        new SemaphoreDisposer(this)

    /// <summary>
    /// Blocks the current thread until it can enter the <see cref="NonReentrantLock"/>, while observing a
    /// <see cref="CancellationToken"/>.
    /// </summary>
    /// <remarks>
    /// Recursive locking is not supported. i.e. A thread may not call Wait successfully twice without an
    /// intervening <see cref="Release"/>.
    /// </remarks>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> token to
    /// observe.</param>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was
    /// canceled.</exception>
    /// <exception cref="LockRecursionException">The caller already holds the lock</exception>
    [<DebuggerHidden>]
    member this.Wait(cancellationToken: CancellationToken) =
        if this.IsOwnedByMe then
            raise(LockRecursionException())

        let mutable cancellationTokenRegistration = Unchecked.defaultof<CancellationTokenRegistration>

        let canReturn =
            if cancellationToken.CanBeCanceled then
                cancellationToken.ThrowIfCancellationRequested()

                // Fast path to try and avoid allocations in callback registration.
                lock _syncLock (fun () ->
                    if not this.IsLocked then
                        this.TakeOwnership()
                        true
                    else
                        false
                )
            else
                false

        if canReturn then ()
        else

        if cancellationToken.CanBeCanceled then
            cancellationTokenRegistration <- cancellationToken.Register(s_cancellationTokenCanceledEventHandler, _syncLock, useSynchronizationContext = false)
        
        try
            // PERF: First spin wait for the lock to become available, but only up to the first planned yield.
            // This additional amount of spinwaiting was inherited from SemaphoreSlim's implementation where
            // it showed measurable perf gains in test scenarios.
            let spin = new SpinWait()

            while this.IsLocked && not spin.NextSpinWillYield do
                spin.SpinOnce()

            lock _syncLock (fun () ->
                while this.IsLocked do
                    // If cancelled, we throw. Trying to wait could lead to deadlock
                    cancellationToken.ThrowIfCancellationRequested()

                    // Another thread holds the lock. Wait until we get awoken either
                    // by some code calling "Release" or by cancellation.
                    Monitor.Wait(_syncLock) |> ignore

                // We now hold the lock
                this.TakeOwnership()
            )
        finally
            cancellationTokenRegistration.Dispose()

    /// <summary>
    /// Exit the mutual exclusion.
    /// </summary>
    /// <remarks>
    /// The calling thread must currently hold the lock.
    /// </remarks>
    /// <exception cref="InvalidOperationException">The lock is not currently held by the calling thread.</exception>
    [<DebuggerHidden>]
    member this.Release() =
        this.AssertHasLock()

        lock _syncLock (fun () ->
            this.ReleaseOwnership()

            // Release one waiter
            Monitor.Pulse(_syncLock)
        )

/// <remarks>
/// This inherits from <see cref="TaskCompletionSource{TResult}"/> to avoid allocating two objects when we can just use one.
/// The public surface area of <see cref="TaskCompletionSource{TResult}"/> should probably be avoided in favor of the public
/// methods on this class for correct behavior.
/// </remarks>
[<AllowNullLiteral>]
type Request<'T> =
    inherit TaskCompletionSource<'T>

    /// <summary>
    /// The <see cref="CancellationToken"/> associated with this request. This field will be initialized before
    /// any cancellation is observed from the token.
    /// </summary>
    [<DefaultValue>] 
    val mutable private _cancellationToken: CancellationToken
    [<DefaultValue>] 
    val mutable private _cancellationTokenRegistration: CancellationTokenRegistration

    // We want to always run continuations asynchronously. Running them synchronously could result in deadlocks:
    // if we're looping through a bunch of Requests and completing them one by one, and the continuation for the
    // first Request was then blocking waiting for a later Request, we would hang. It also could cause performance
    // issues. If the first request then consumes a lot of CPU time, we're not letting other Requests complete that
    // could use another CPU core at the same time.
    [<DebuggerHidden>]
    new() =
        { inherit TaskCompletionSource<'T>(TaskCreationOptions.RunContinuationsAsynchronously) }

    [<DebuggerHidden>]
    member this.RegisterForCancellation(callback: Action<obj>, cancellationToken: CancellationToken) =
        this._cancellationToken <- cancellationToken
        this._cancellationTokenRegistration <- cancellationToken.Register(callback, this)

    [<DebuggerHidden>]
    member this.Cancel() = this.TrySetCanceled(this._cancellationToken)

    [<DebuggerHidden>]
    member this.CompleteFromTask(task: Task<'T>) =
        // As an optimization, we'll cancel the request even we did get a value for it.
        // That way things abort sooner.
        if task.IsCanceled || this._cancellationToken.IsCancellationRequested then
            this.Cancel() |> ignore
        elif task.IsFaulted then
            // TrySetException wraps its argument in an AggregateException, so we pass the inner exceptions from
            // the antecedent to avoid wrapping in two layers of AggregateException.
            Debug.Assert(task.Exception <> null)
            if task.Exception.InnerExceptions.Count > 0 then
                this.TrySetException(task.Exception.InnerExceptions) |> ignore
            else
                this.TrySetException(task.Exception) |> ignore
        else
            this.TrySetResult(task.Result) |> ignore

        this._cancellationTokenRegistration.Dispose()

[<Struct;NoEquality;NoComparison>]
type WaitThatValidatesInvariants<'T>(asyncLazy: AsyncLazy<'T>) =

    interface IDisposable with

        [<DebuggerHidden>]
        member this.Dispose() =
            asyncLazy.AssertInvariants_NoLock()
            AsyncLazy.s_gate.Release()
            
[<Struct;NoEquality;NoComparison>]
type AsynchronousComputationToStart<'T>(asynchronousComputeFunction: Func<CancellationToken, Task<'T>>, cancellationTokenSource: CancellationTokenSource) =

    member _.AsynchronousComputeFunction = asynchronousComputeFunction
    member _.CancellationTokenSource = cancellationTokenSource


type Task<'T> with

    [<DebuggerHidden>]
    member this.WaitAndGetResult_CanCallOnBackground(cancellationToken: CancellationToken) =
        try
            this.Wait(cancellationToken)
        with
        | :? AggregateException as ex ->
            ExceptionDispatchInfo.Capture(if ex.InnerException <> null then ex.InnerException else ex :> Exception).Throw()

        this.Result

[<AbstractClass;Sealed>]
type AsyncLazy private () =

    /// <summary>
    /// Mutex used to protect reading and writing to all mutable objects and fields.  Traces
    /// indicate that there's negligible contention on this lock, hence we can save some memory
    /// by using a single lock for all AsyncLazy instances.  Only trivial and non-reentrant work
    /// should be done while holding the lock.
    /// </summary>
    static let _s_gate = NonReentrantLock(useThisInstanceForSynchronization = true)

    // Remove unread private members - We want to hold onto last exception to make investigation easier
    static let mutable s_reportedException: Exception = null
    static let mutable s_reportedExceptionMessagge: string = null

    static member s_gate: NonReentrantLock = _s_gate

    static member Report(ex: Exception) =
        // hold onto last exception to make investigation easier
        s_reportedException <- ex
        s_reportedExceptionMessagge <- ex.ToString()
        false

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
    /// The underlying function that starts an asynchronous computation of the resulting value.
    /// Null'ed out once we've computed the result and we've been asked to cache it.  Otherwise,
    /// it is kept around in case the value needs to be computed again.
    /// </summary>
    val mutable private _asynchronousComputeFunction: Func<CancellationToken, Task<'T>>

    /// <summary>
    /// The underlying function that starts a synchronous computation of the resulting value.
    /// Null'ed out once we've computed the result and we've been asked to cache it, or if we
    /// didn't get any synchronous function given to us in the first place.
    /// </summary>
    val mutable private _synchronousComputeFunction: Func<CancellationToken, 'T>

    /// <summary>
    /// Whether or not we should keep the value around once we've computed it.
    /// </summary>
    val private _cacheResult: bool

    /// <summary>
    /// The Task that holds the cached result.
    /// </summary>
    [<DefaultValue>]
    val mutable private _cachedResult: Task<'T>

    /// <summary>
    /// The hash set of all currently outstanding asynchronous requests. Null if there are no requests,
    /// and will never be empty.
    /// </summary>
    [<DefaultValue>]
    val mutable private _requests: HashSet<Request<'T>>

    /// <summary>
    /// If an asynchronous request is active, the CancellationTokenSource that allows for
    /// cancelling the underlying computation.
    /// </summary>
    [<DefaultValue>]
    val mutable private _asynchronousComputationCancellationSource: CancellationTokenSource

    /// <summary>
    /// Whether a computation is active or queued on any thread, whether synchronous or
    /// asynchronous.
    /// </summary>
    [<DefaultValue>]
    val mutable private _computationActive: bool

    // #region Lock Wrapper for Invariant Checking   

    [<DebuggerHidden>]
    member this.AssertInvariants_NoLock() =
        // Invariant #1: thou shalt never have an asynchronous computation running without it
        // being considered a computation
        if this._asynchronousComputationCancellationSource <> null && not this._computationActive then
            failwith "Unexpected true"

        // Invariant #2: thou shalt never waste memory holding onto empty HashSets
        if this._requests <> null && this._requests.Count = 0 then
            failwith "Unexpected true"

        // Invariant #3: thou shalt never have an request if there is not
        // something trying to compute it
        if this._requests <> null && not this._computationActive then
            failwith "Unexpected true"

        // Invariant #4: thou shalt never have a cached value and any computation function
        if this._cachedResult <> null && (this._synchronousComputeFunction <> null || this._asynchronousComputeFunction <> null) then
            failwith "Unexpected true"

        // Invariant #5: thou shalt never have a synchronous computation function but not an
        // asynchronous one
        if this._asynchronousComputeFunction = null && this._synchronousComputeFunction <> null then
            failwith "Unexpected true"

    /// <summary>
    /// Takes the lock for this object and if acquired validates the invariants of this class.
    /// </summary>
    [<DebuggerHidden>]
    member this.TakeLock(cancellationToken: CancellationToken) =
        AsyncLazy.s_gate.Wait(cancellationToken)
        this.AssertInvariants_NoLock()
        new WaitThatValidatesInvariants<'T>(this)

    // #endregion

    [<DebuggerHidden>]
    member this.CreateNewRequest_NoLock() =
        if this._requests = null then
            this._requests <- HashSet()

        let request = new Request<'T>()
        this._requests.Add(request) |> ignore
        request

    [<DebuggerHidden>]
    member this.RegisterAsynchronousComputation_NoLock() =
        if this._computationActive then
            failwith "Unexpected true"

        if this._asynchronousComputeFunction = null then
            nullArg (nameof(this._asynchronousComputeFunction))

        this._asynchronousComputationCancellationSource <- new CancellationTokenSource()
        this._computationActive <- true

        new AsynchronousComputationToStart<'T>(this._asynchronousComputeFunction, this._asynchronousComputationCancellationSource)

    [<DebuggerHidden>]
    member this.OnAsynchronousRequestCancelled(o: obj) =
        let request = o :?> Request<'T>

        let mutable cancellationTokenSource = Unchecked.defaultof<CancellationTokenSource>

        using (this.TakeLock(CancellationToken.None)) (fun _ ->

            // Now try to remove it. It's possible that requests may already be null. You could
            // imagine that cancellation was requested, but before we could acquire the lock
            // here the computation completed and the entire CompleteWithTask synchronized
            // block ran. In that case, the requests collection may already be null, or it
            // (even scarier!) may have been replaced with another collection because another
            // computation has started.
            if this._requests <> null then
                if this._requests.Count = 0 then
                    this._requests <- null

                    if this._asynchronousComputationCancellationSource <> null then
                        cancellationTokenSource <- this._asynchronousComputationCancellationSource
                        this._asynchronousComputationCancellationSource <- null
                        this._computationActive <- false
        )

        request.Cancel() |> ignore
        if cancellationTokenSource <> null then
            cancellationTokenSource.Cancel()

    [<DebuggerHidden>]
    member this.GetCachedValueAndCacheThisValueIfNoneCached_NoLock(task: Task<'T>) =
        if this._cachedResult <> null then
            this._cachedResult
        else
            if this._cacheResult && task.Status = TaskStatus.RanToCompletion then
                // Hold onto the completed task. We can get rid of the computation functions for good
                this._cachedResult <- task

                this._asynchronousComputeFunction <- null
                this._synchronousComputeFunction <- null

            task

    [<DebuggerHidden>]
    member this.CompleteWithTask(task: Task<'T>, cancellationToken: CancellationToken) =
        let requestsToComplete, task =
            using (this.TakeLock(cancellationToken)) (fun _ ->
                // If the underlying computation was cancelled, then all state was already updated in OnAsynchronousRequestCancelled
                // and there is no new work to do here. We *must* use the local one since this completion may be running far after
                // the background computation was cancelled and a new one might have already been enqueued. We must do this
                // check here under the lock to ensure proper synchronization with OnAsynchronousRequestCancelled.
                cancellationToken.ThrowIfCancellationRequested()

                // The computation is complete, so get all requests to complete and null out the list. We'll create another one
                // later if it's needed
                let requestsToComplete: Request<'T> seq =
                    if this._requests = null then
                        Seq.empty
                    else
                        this._requests :> _ seq
                this._requests <- null

                // The computations are done
                this._asynchronousComputationCancellationSource <- null
                this._computationActive <- false

                let task = this.GetCachedValueAndCacheThisValueIfNoneCached_NoLock(task)

                requestsToComplete, task
            )

        // Complete the requests outside the lock. It's not necessary to do this (none of this is touching any shared state)
        // but there's no reason to hold the lock so we could reduce any theoretical lock contention.
        for requestToComplete in requestsToComplete do
            requestToComplete.CompleteFromTask(task)

    [<DebuggerHidden>]
    member this.StartAsynchronousComputation(computationToStart: AsynchronousComputationToStart<'T>, requestToCompleteSynchronously: Request<'T>, callerCancellationToken: CancellationToken) =
        let cancellationToken = computationToStart.CancellationTokenSource.Token

        // DO NOT ACCESS ANY FIELDS OR STATE BEYOND THIS POINT. Since this function
        // runs unsynchronized, it's possible that during this function this request
        // might be cancelled, and then a whole additional request might start and
        // complete inline, and cache the result. By grabbing state before we check
        // the cancellation token, we can be assured that we are only operating on
        // a state that was complete.
        try
            cancellationToken.ThrowIfCancellationRequested()

            let mutable task = computationToStart.AsynchronousComputeFunction.Invoke(cancellationToken)

            // As an optimization, if the task is already completed, mark the 
            // request as being completed as well.
            //
            // Note: we want to do this before we do the .ContinueWith below. That way, 
            // when the async call to CompleteWithTask runs, it sees that we've already
            // completed and can bail immediately. 
            if requestToCompleteSynchronously <> null && task.IsCompleted then
                using (this.TakeLock(CancellationToken.None)) (fun _ ->
                    task <- this.GetCachedValueAndCacheThisValueIfNoneCached_NoLock(task)
                )

                requestToCompleteSynchronously.CompleteFromTask(task)

            task.ContinueWith(
                (fun (t: Task<'T>) (s: obj) -> this.CompleteWithTask(t, (s :?> CancellationTokenSource).Token)),
                computationToStart.CancellationTokenSource,
                cancellationToken,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default
            ) |> ignore

            task.Start()
        with
        | :? OperationCanceledException as ex when ex.CancellationToken = cancellationToken ->
            // The underlying computation cancelled with the correct token, but we must ourselves ensure that the caller
            // on our stack gets an OperationCanceledException thrown with the right token
            callerCancellationToken.ThrowIfCancellationRequested()

            // We can only be here if the computation was cancelled, which means all requests for the value
            // must have been cancelled. Therefore, the ThrowIfCancellationRequested above must have thrown
            // because that token from the requester was cancelled.
            raise(InvalidOperationException("This program location is thought to be unreachable."))
        | ex when AsyncLazy.Report ex ->
            raise(InvalidOperationException("This program location is thought to be unreachable."))

    [<DebuggerHidden>]
    member this.TryGetValue() =
        // No need to lock here since this is only a fast check to 
        // see if the result is already computed.
        if this._cachedResult <> null then
            ValueSome this._cachedResult.Result
        else
            ValueNone

    [<DebuggerHidden>]
    member this.GetValue(cancellationToken: CancellationToken) =
        cancellationToken.ThrowIfCancellationRequested()

        // If the value is already available, return it immediately
        match this.TryGetValue() with
        | ValueSome value -> value
        | _ ->

        let mutable request = Unchecked.defaultof<Request<_>>
        let mutable newAsynchronousComputation = Unchecked.defaultof<Nullable<AsynchronousComputationToStart<'T>>>

        let resultOpt =
            using (this.TakeLock(cancellationToken)) (fun _ ->
                if this._cachedResult <> null then
                    ValueSome this._cachedResult.Result
                else                    
                    // If there is an existing computation active, we'll just create another request
                    if this._computationActive then
                        request <- this.CreateNewRequest_NoLock()
                    elif this._synchronousComputeFunction = null then
                        // A synchronous request, but we have no synchronous function. Start off the async work
                        request <- this.CreateNewRequest_NoLock()

                        newAsynchronousComputation <- this.RegisterAsynchronousComputation_NoLock() |> Nullable
                    else
                        // We will do the computation here
                        this._computationActive <- true
                    ValueNone
            )

        if resultOpt.IsSome then resultOpt.Value
        else
        
        // If we simply created a new asynchronous request, so wait for it. Yes, we're blocking the thread
        // but we don't want multiple threads attempting to compute the same thing.
        if request <> null then
            request.RegisterForCancellation(Action<_>(this.OnAsynchronousRequestCancelled), cancellationToken) |> ignore

            if newAsynchronousComputation.HasValue then
                this.StartAsynchronousComputation(newAsynchronousComputation.Value, requestToCompleteSynchronously = request, callerCancellationToken = cancellationToken)


            // The reason we have synchronous codepaths in AsyncLazy is to support the synchronous requests
            // that we may get from the compiler. Thus, it's entirely possible that this will be requested by the compiler or
            // an analyzer on the background thread when another part of the IDE is requesting the same tree asynchronously.
            // In that case we block the synchronous request on the asynchronous request, since that's better than alternatives.
            request.Task.WaitAndGetResult_CanCallOnBackground(cancellationToken)
        else

            if this._synchronousComputeFunction = null then
                nullArg (nameof(this._synchronousComputeFunction))

            let result =
                // We are the active computation, so let's go ahead and compute.
                try
                    this._synchronousComputeFunction.Invoke(cancellationToken)
                with
                | :? OperationCanceledException ->
                    using (this.TakeLock(CancellationToken.None)) (fun _ ->
                        this._computationActive <- false

                        if this._requests <> null then
                            // There's a possible improvement here: there might be another synchronous caller who
                            // also wants the value. We might consider stealing their thread rather than punting
                            // to the thread pool.
                            newAsynchronousComputation <- this.RegisterAsynchronousComputation_NoLock() |> Nullable
                    )

                    if newAsynchronousComputation.HasValue then
                        this.StartAsynchronousComputation(newAsynchronousComputation.Value, requestToCompleteSynchronously = null, callerCancellationToken = cancellationToken)

                    reraise()
                | ex ->
                    // We faulted for some unknown reason. We should simply fault everything.
                    this.CompleteWithTask(Task.FromException<'T>(ex), CancellationToken.None)

                    reraise()

            // We have a value, so complete
            this.CompleteWithTask(Task.FromResult(result), CancellationToken.None)

            // Optimization: if they did cancel and the computation never observed it, let's throw so we don't keep
            // processing a value somebody never wanted
            cancellationToken.ThrowIfCancellationRequested()

            result

    [<DebuggerHidden>]
    member this.GetValueAsync(cancellationToken: CancellationToken) =
        // Optimization: if we're already cancelled, do not pass go
        if cancellationToken.IsCancellationRequested then
            Task.FromCanceled<'T>(cancellationToken)
        else
            
        // Avoid taking the lock if a cached value is available
        let cachedResult = this._cachedResult
        if cachedResult <> null then
            cachedResult
        else

        let mutable newAsynchronousComputation = Unchecked.defaultof<Nullable<AsynchronousComputationToStart<'T>>>

        let request, resultOpt =
            using (this.TakeLock(cancellationToken)) (fun _ ->
                // If cached, get immediately
                if this._cachedResult <> null then
                    null, ValueSome this._cachedResult
                else
                    
                    let request = this.CreateNewRequest_NoLock()

                    // If we have either synchronous or asynchronous work current in flight, we don't need to do anything.
                    // Otherwise, we shall start an asynchronous computation for this
                    if not this._computationActive then
                        newAsynchronousComputation <- this.RegisterAsynchronousComputation_NoLock() |> Nullable

                    request, ValueNone
            )

        match resultOpt with
        | ValueSome result -> result
        | _ ->

        // We now have the request counted for, register for cancellation. It is critical this is
        // done outside the lock, as our registration may immediately fire and we want to avoid the
        // reentrancy
        request.RegisterForCancellation(Action<_>(this.OnAsynchronousRequestCancelled), cancellationToken)

        if newAsynchronousComputation.HasValue then
            this.StartAsynchronousComputation(newAsynchronousComputation.Value, requestToCompleteSynchronously = request, callerCancellationToken = cancellationToken)

        request.Task

    [<DebuggerHidden>]
    new(asynchronousComputeFunction, cacheResult) =
        AsyncLazy<'T>(asynchronousComputeFunction, null, cacheResult = cacheResult)

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
    [<DebuggerHidden>]
    new(asynchronousComputeFunction, synchronousComputeFunction, cacheResult) =
        if asynchronousComputeFunction = null then
            nullArg (nameof(asynchronousComputeFunction))

        {
            _asynchronousComputeFunction = asynchronousComputeFunction
            _synchronousComputeFunction = synchronousComputeFunction
            _cacheResult = cacheResult
        }




