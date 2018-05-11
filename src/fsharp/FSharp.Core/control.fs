// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Control

    #nowarn "40"
    #nowarn "21"
    #nowarn "47"
    #nowarn "52" // The value has been copied to ensure the original is not mutated by this operation
    #nowarn "67" // This type test or downcast will always hold
    #nowarn "864" // IObservable.Subscribe
 
    open System
    open System.Diagnostics
    open System.Diagnostics.CodeAnalysis
    open System.IO
    open System.Reflection
    open System.Runtime.CompilerServices
    open System.Runtime.ExceptionServices
    open System.Threading
    open System.Threading.Tasks
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections

#if FX_RESHAPED_REFLECTION
    open ReflectionAdapters
#endif


    /// We use our own internal implementation of queues to avoid a dependency on System.dll
    type Queue<'T>() =  //: IEnumerable<T>, ICollection, IEnumerable
    
        let mutable array = [| |]
        let mutable head = 0
        let mutable size = 0
        let mutable tail = 0

        let SetCapacity(capacity) =
            let destinationArray = Array.zeroCreate capacity
            if (size > 0) then 
                if (head < tail) then 
                    Array.Copy(array, head, destinationArray, 0, size)
        
                else
                    Array.Copy(array, head, destinationArray, 0, array.Length - head)
                    Array.Copy(array, 0, destinationArray, array.Length - head, tail)
            array <- destinationArray
            head <- 0
            tail <- if (size = capacity) then 0 else size

        member x.Dequeue() =
            if (size = 0) then
                failwith "Dequeue"
            let local = array.[head]
            array.[head] <- Unchecked.defaultof<'T>
            head <- (head + 1) % array.Length
            size <- size - 1
            local

        member this.Enqueue(item) =
            if (size = array.Length) then 
                let capacity = int ((int64 array.Length * 200L) / 100L)
                let capacity = max capacity (array.Length + 4)
                SetCapacity(capacity)
            array.[tail] <- item
            tail <- (tail + 1) % array.Length
            size <- size + 1

        member x.Count = size

    type LinkedSubSource(cancellationToken : CancellationToken) =
        
        let failureCTS = new CancellationTokenSource()

        let linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, failureCTS.Token)
        
        member this.Token = linkedCTS.Token

        member this.Cancel() = failureCTS.Cancel()

        member this.Dispose() = 
            linkedCTS.Dispose()
            failureCTS.Dispose()
        
        interface IDisposable with
            member this.Dispose() = this.Dispose()

    /// Global mutable state used to associate Exception
    [<AutoOpen>]
    module ExceptionDispatchInfoHelpers =

        let associationTable = ConditionalWeakTable<exn, ExceptionDispatchInfo>()

        type ExceptionDispatchInfo with 

            member edi.GetAssociatedSourceException() = 
                let exn = edi.SourceException
                // Try to store the entry in the association table to allow us to recover it later.
                try associationTable.Add(exn, edi) with _ -> ()
                exn

            // Capture, but prefer the saved information if available
            [<DebuggerHidden>]
            static member RestoreOrCapture(exn) = 
                match associationTable.TryGetValue(exn) with 
                | true, edi -> edi
                | _ ->
                    ExceptionDispatchInfo.Capture(exn)

            member inline edi.ThrowAny() = 
                edi.Throw()
                Unchecked.defaultof<'T> // Note, this line should not be reached, but gives a generic return type

    // F# don't always take tailcalls to functions returning 'unit' because this
    // is represented as type 'void' in the underlying IL.
    // Hence we don't use the 'unit' return type here, and instead invent our own type.
    [<NoEquality; NoComparison>]
    type AsyncReturn =
        | FakeUnit

    type cont<'T> = ('T -> AsyncReturn)
    type econt = (ExceptionDispatchInfo -> AsyncReturn)
    type ccont = (OperationCanceledException -> AsyncReturn)

    [<AllowNullLiteral>]
    type Trampoline() = 

        let unfake FakeUnit = ()

        [<Literal>]
        static let bindLimitBeforeHijack = 300 

        [<ThreadStatic>]
        [<DefaultValue>]
        static val mutable private thisThreadHasTrampoline : bool

        static member ThisThreadHasTrampoline = 
            Trampoline.thisThreadHasTrampoline
        
        let mutable storedCont = None
        let mutable storedExnCont = None
        let mutable bindCount = 0
        
        /// Use this trampoline on the synchronous stack if none exists, and execute
        /// the given function. The function might write its continuation into the trampoline.
        [<DebuggerHidden>]
        member __.Execute (firstAction : unit -> AsyncReturn) =
            let rec loop action = 
                action() |> unfake
                match storedCont with
                | None -> ()
                | Some newAction -> 
                    storedCont <- None
                    loop newAction

            let thisIsTopTrampoline =
                if Trampoline.thisThreadHasTrampoline then
                    false
                else
                    Trampoline.thisThreadHasTrampoline <- true
                    true
            try
                let mutable keepGoing = true
                let mutable action = firstAction
                while keepGoing do 
                    try 
                        action() |> unfake
                        match storedCont with
                        | None ->
                            keepGoing <- false
                        | Some cont -> 
                            storedCont <- None
                            action <- cont
                    // Let the exception propagate all the way to the trampoline to get a full .StackTrace entry
                    with exn -> 
                        match storedExnCont with
                        | None ->
                            reraise()
                        | Some econt -> 
                            storedExnCont <- None
                            let edi = ExceptionDispatchInfo.RestoreOrCapture exn
                            action <- (fun () -> econt edi)
                        
            finally
                if thisIsTopTrampoline then
                    Trampoline.thisThreadHasTrampoline <- false
            FakeUnit
            
        /// Increment the counter estimating the size of the synchronous stack and
        /// return true if time to jump on trampoline.
        member __.IncrementBindCount() =
            bindCount <- bindCount + 1
            bindCount >= bindLimitBeforeHijack
            
        /// Prepare to abandon the synchronous stack of the current execution and save the continuation in the trampoline.
        member __.Set action = 
            match storedCont with
            | None -> 
                bindCount <- 0
                storedCont <- Some action
            | _ -> failwith "Internal error: attempting to install continuation twice"
            FakeUnit

        /// Save the exception continuation during propagation of an exception, or prior to raising an exception
        member __.SaveExceptionContinuation (action: econt) = 
            storedExnCont <- Some action

    type TrampolineHolder() as this =
        let mutable trampoline = null
        
        static let unfake FakeUnit = ()

        // Preallocate this delegate and keep it in the trampoline holder.
        let sendOrPostCallbackWithTrampoline = 
            SendOrPostCallback (fun o ->
                let f = unbox<(unit -> AsyncReturn)> o
                this.ExecuteWithTrampoline f |> unfake)

        // Preallocate this delegate and keep it in the trampoline holder.
        let waitCallbackForQueueWorkItemWithTrampoline = 
            WaitCallback (fun o ->
                let f = unbox<(unit -> AsyncReturn)> o
                this.ExecuteWithTrampoline f |> unfake)

#if !FX_NO_PARAMETERIZED_THREAD_START
        // Preallocate this delegate and keep it in the trampoline holder.
        let threadStartCallbackForStartThreadWithTrampoline = 
            ParameterizedThreadStart (fun o ->
                let f = unbox<(unit -> AsyncReturn)> o
                this.ExecuteWithTrampoline f |> unfake)
#endif

        /// Execute an async computation after installing a trampoline on its synchronous stack.
        [<DebuggerHidden>]
        member __.ExecuteWithTrampoline firstAction =
            trampoline <- new Trampoline()
            trampoline.Execute firstAction
            
        member this.PostWithTrampoline (syncCtxt: SynchronizationContext)  (f : unit -> AsyncReturn) =
            syncCtxt.Post (sendOrPostCallbackWithTrampoline, state=(f |> box))
            FakeUnit

        member this.QueueWorkItemWithTrampoline (f: unit -> AsyncReturn) =            
            if not (ThreadPool.QueueUserWorkItem(waitCallbackForQueueWorkItemWithTrampoline, f |> box)) then
                failwith "failed to queue user work item"
            FakeUnit

        member this.PostOrQueueWithTrampoline (syncCtxt : SynchronizationContext) f =
            match syncCtxt with 
            | null -> this.QueueWorkItemWithTrampoline f 
            | _ -> this.PostWithTrampoline syncCtxt f            
        
#if FX_NO_PARAMETERIZED_THREAD_START
        // This should be the only call to Thread.Start in this library. We must always install a trampoline.
        member this.StartThreadWithTrampoline (f : unit -> AsyncReturn) =
#if FX_NO_THREAD
            this.QueueWorkItemWithTrampoline(f)
#else
            (new Thread((fun _ -> this.Execute f |> unfake), IsBackground=true)).Start()
            FakeUnit
#endif

#else
        // This should be the only call to Thread.Start in this library. We must always install a trampoline.
        member __.StartThreadWithTrampoline (f : unit -> AsyncReturn) =
            (new Thread(threadStartCallbackForStartThreadWithTrampoline,IsBackground=true)).Start(f|>box)
            FakeUnit
#endif
        
        /// Save the exception continuation during propagation of an exception, or prior to raising an exception
        member __.SaveExceptionContinuation(econt) =
            trampoline.SaveExceptionContinuation econt

        /// Call a continuation, but first check if an async computation should trampoline on its synchronous stack.
        member inline __.HijackCheckThenCall (cont : 'T -> AsyncReturn) res =
            if trampoline.IncrementBindCount() then
                trampoline.Set (fun () -> cont res)
            else
                // NOTE: this must be a tailcall
                cont res
        
    [<NoEquality; NoComparison>]
    [<AutoSerializable(false)>]
    /// Represents rarely changing components of an in-flight async computation
    type AsyncActivationAux =
        { token : CancellationToken
          econt : econt
          ccont : ccont
          trampolineHolder : TrampolineHolder }
    
    [<NoEquality; NoComparison>]
    [<AutoSerializable(false)>]
    /// Represents an in-flight async computation
    type AsyncActivation<'T> =
        { cont : cont<'T>
          aux : AsyncActivationAux }

        member ctxt.IsCancellationRequested = ctxt.aux.token.IsCancellationRequested

        /// Call the cancellation continuation of the active computation
        member ctxt.OnCancellation () =
            ctxt.aux.ccont (new OperationCanceledException (ctxt.aux.token))

        member inline ctxt.HijackCheckThenCall cont arg =
            ctxt.aux.trampolineHolder.HijackCheckThenCall cont arg

        member ctxt.OnSuccess result =
            if ctxt.IsCancellationRequested then
                ctxt.OnCancellation ()
            else
                ctxt.HijackCheckThenCall ctxt.cont result

        /// Call the exception continuation directly
        member ctxt.CallExceptionContinuation edi =
            ctxt.aux.econt edi

        /// Save the exception continuation during propagation of an exception, or prior to raising an exception
        member ctxt.SaveExceptionContinuation() =
            ctxt.aux.trampolineHolder.SaveExceptionContinuation ctxt.aux.econt

    [<NoEquality; NoComparison>]
    [<CompiledName("FSharpAsync`1")>]
    type Async<'T> =
        { Invoke : (AsyncActivation<'T> -> AsyncReturn) }

    type VolatileBarrier() =
        [<VolatileField>]
        let mutable isStopped = false
        member __.Proceed = not isStopped
        member __.Stop() = isStopped <- true

    [<Sealed>]
    [<AutoSerializable(false)>]
    type Latch() = 
        let mutable i = 0
        member this.Enter() = Interlocked.CompareExchange(&i, 1, 0) = 0

    [<Sealed>]
    [<AutoSerializable(false)>]
    type Once() =
        let latch = Latch()
        member this.Do f =
            if latch.Enter() then
                f()

    [<NoEquality; NoComparison>]
    type AsyncResult<'T>  =
        | Ok of 'T
        | Error of ExceptionDispatchInfo
        | Canceled of OperationCanceledException

        [<DebuggerHidden>]
        member res.Commit () =
            match res with
            | AsyncResult.Ok res -> res
            | AsyncResult.Error edi -> edi.ThrowAny()
            | AsyncResult.Canceled exn -> raise exn

    module AsyncPrimitives =

        let fake () = FakeUnit
        let unfake FakeUnit = ()

        let mutable defaultCancellationTokenSource = new CancellationTokenSource()

        /// Apply userCode to x and call either the continuation or exception continuation depending what happens
        [<DebuggerHidden>]
        let ProtectUserCodePlusHijackCheck (ctxt: AsyncActivation<_>) userCode arg : AsyncReturn =
            let mutable result = Unchecked.defaultof<_>
            let mutable ok = false

            try 
                result <- userCode arg
                ok <- true
            finally
                if not ok then 
                    ctxt.SaveExceptionContinuation()

            if ok then 
                ctxt.HijackCheckThenCall ctxt.cont result
            else
                FakeUnit

        /// Apply 'userCode' to 'arg' and invoke the resulting computation.
        [<DebuggerHidden>]
        let ProtectUserCodePlusHijackCheckThenInvoke (ctxt: AsyncActivation<_>) userCode arg : AsyncReturn =
            let mutable result = Unchecked.defaultof<_>
            let mutable ok = false

            try 
                result <- userCode arg
                ok <- true
            finally
                if not ok then 
                    ctxt.SaveExceptionContinuation()

            if ok then 
                ctxt.HijackCheckThenCall result.Invoke ctxt
            else
                FakeUnit

        /// Apply 'catchFilter' to 'arg'. If the result is 'Some' invoke the resulting computation. If the result is 'None'
        /// then send 'result1' to the exception continuation.  
        [<DebuggerHidden>]
        let ProtectUserCodePlusHijackCheckThenTryWithFilterFunctionInvoke (ctxt: AsyncActivation<_>) catchFilter (edi: ExceptionDispatchInfo) : AsyncReturn =
            let mutable resOpt = Unchecked.defaultof<_>
            let mutable ok = false

            try 
                resOpt <- catchFilter (edi.GetAssociatedSourceException())
                ok <- true
            finally
                if not ok then 
                    ctxt.SaveExceptionContinuation()

            if ok then 
                match resOpt with 
                | None -> 
                    ctxt.HijackCheckThenCall ctxt.aux.econt edi
                | Some res -> 
                    ctxt.HijackCheckThenCall res.Invoke ctxt
            else
                FakeUnit

        /// Apply userCode to x and invoke the resulting computation.
        /// Does not do a hijack check.
        [<DebuggerHidden>]
        let ProtectUserCodeThenInvoke (ctxt: AsyncActivation<_>) userCode result1 =
            // This is deliberately written in a allocation-free style
            let mutable res = Unchecked.defaultof<_>
            let mutable ok = false

            try 
                res <- userCode result1
                ok <- true
            finally
                if not ok then 
                    ctxt.SaveExceptionContinuation()

            if ok then 
                res.Invoke ctxt
            else FakeUnit

        /// Perform a cancellation check and ensure that any exceptions raised by 
        /// the immediate execution of "userCode" are sent to the exception continuation.
        [<DebuggerHidden>]
        let ProtectUserCode (ctxt: AsyncActivation<_>) userCode =
            if ctxt.IsCancellationRequested then
                ctxt.OnCancellation ()
            else
                let mutable ok = false
                try 
                    let res = userCode ctxt
                    ok <- true
                    res
                finally
                    if not ok then 
                        ctxt.SaveExceptionContinuation()

        /// Make an initial asyc activation.  
        [<DebuggerHidden>]
        let CreateAsyncActivation cancellationToken trampolineHolder cont econt ccont =
            { cont = cont; aux = { token = cancellationToken; econt = econt; ccont = ccont; trampolineHolder = trampolineHolder } }
                    
        /// Build a primitive without any exception or resync protection
        let MakeAsync body = { Invoke = body }

        [<DebuggerHidden>]
        // Note: direct calls to this function end up in user assemblies via inlining
        let Bind (ctxt: AsyncActivation<_>) part1 part2 =
            if ctxt.IsCancellationRequested then
                ctxt.OnCancellation ()
            else
                let ctxtPart1ThenPart2 = 
                    let cont result1 = ProtectUserCodeThenInvoke ctxt part2 result1 
                    { cont=cont; aux = ctxt.aux }

                ctxt.HijackCheckThenCall part1.Invoke ctxtPart1ThenPart2

        [<DebuggerHidden>]
        /// Execute user code but first check for trampoline and cancellation.
        //
        // Note: direct calls to this function end up in user assemblies via inlining
        let Call (ctxt: AsyncActivation<'T>) result1 (part2: 'U -> Async<'T>)  =
            if ctxt.IsCancellationRequested then
                ctxt.OnCancellation ()
            else                    
                ProtectUserCodePlusHijackCheckThenInvoke ctxt part2 result1 

        /// Execute the with-filter part of a try-with-filer but first check for trampoline and cancellation.
        //
        // Note: direct calls to this function end up in user assemblies via inlining
        let CallTryWithFilterFunction (ctxt: AsyncActivation<'T>) result1 (part2: Exception -> Async<'T> option)  =
            if ctxt.IsCancellationRequested then
                ctxt.OnCancellation ()
            else                    
                ProtectUserCodePlusHijackCheckThenTryWithFilterFunctionInvoke ctxt part2 result1 

        [<DebuggerHidden>]
        let TryFinally (ctxt: AsyncActivation<'T>) computation finallyFunction =
            if ctxt.IsCancellationRequested then
                ctxt.OnCancellation ()
            else
                // The new continuation runs the finallyFunction and resumes the old continuation
                // If an exception is thrown we continue with the previous exception continuation.
                let cont b     = 
                    let ctxt = { cont = (fun () -> ctxt.cont b); aux = ctxt.aux }
                    ProtectUserCodePlusHijackCheck ctxt finallyFunction () 
                // The new exception continuation runs the finallyFunction and then runs the previous exception continuation.
                // If an exception is thrown we continue with the previous exception continuation.
                let econt exn  = 
                    let ctxt = { cont =  (fun () -> ctxt.aux.econt exn); aux = ctxt.aux }
                    ProtectUserCodePlusHijackCheck ctxt finallyFunction ()
                // The cancellation continuation runs the finallyFunction and then runs the previous cancellation continuation.
                // If an exception is thrown we continue with the previous cancellation continuation (the exception is lost)
                let ccont cexn = 
                    let ctxt = { cont =  (fun () -> ctxt.aux.ccont cexn); aux = { ctxt.aux with econt = (fun _ -> ctxt.aux.ccont cexn) } }
                    ProtectUserCodePlusHijackCheck ctxt finallyFunction () 
                computation.Invoke { ctxt with cont = cont; aux = { ctxt.aux with econt = econt; ccont = ccont } } 

        // Re-route the exception continuation to call to catchFunction. If catchFunction or the new process fail
        // then call the original exception continuation with the failure.
        [<DebuggerHidden>]
        let TryWith (ctxt: AsyncActivation<'T>) computation catchFunction = 
            if ctxt.IsCancellationRequested then
                ctxt.OnCancellation ()
            else 
                let econt (edi: ExceptionDispatchInfo) = CallTryWithFilterFunction ctxt edi catchFunction
                let newCtxt = { ctxt with aux = { ctxt.aux with econt = econt } }
                computation.Invoke newCtxt

        /// When run, ensures that any exceptions raised by the immediate execution of "f" are
        /// sent to the exception continuation.
        let CreateProtectedAsync f =
            MakeAsync (fun ctxt -> ProtectUserCode ctxt f)

        let CreateAsyncResultAsync res =
            MakeAsync (fun ctxt ->
                match res with
                | AsyncResult.Ok r -> ctxt.cont r
                | AsyncResult.Error edi -> ctxt.CallExceptionContinuation edi
                | AsyncResult.Canceled oce -> ctxt.aux.ccont oce)

        // Generate async computation which calls its continuation with the given result
        let inline CreateReturnAsync res = 
            // Note: this code ends up in user assemblies via inlining
            MakeAsync (fun ctxt -> ctxt.OnSuccess res)
                    
        // The primitive bind operation. Generate a process that runs the first process, takes
        // its result, applies f and then runs the new process produced. Hijack if necessary and 
        // run 'f' with exception protection
        let inline CreateBindAsync part1 part2  =
            // Note: this code ends up in user assemblies via inlining
            MakeAsync (fun ctxt -> Bind ctxt part1 part2)

        // Call the given function with exception protection, but first 
        // check for cancellation.
        let inline CreateCallAsync part2 result1 =
            // Note: this code ends up in user assemblies via inlining
            MakeAsync (fun ctxt -> Call ctxt result1 part2)

        let inline CreateDelayAsync computation =
            // Note: this code ends up in user assemblies via inlining
            MakeAsync (fun ctxt -> Call ctxt () computation)

        /// Implements the sequencing construct of async computation expressions
        let inline CreateSequentialAsync part1 part2 = 
            // Note: this code ends up in user assemblies via inlining
            CreateBindAsync part1 (fun () -> part2)

        // Call p but augment the normal, exception and cancel continuations with a call to finallyFunction.
        // If the finallyFunction raises an exception then call the original exception continuation
        // with the new exception. If exception is raised after a cancellation, exception is ignored
        // and cancel continuation is called.
        let inline CreateTryFinallyAsync finallyFunction computation =
            MakeAsync (fun ctxt -> TryFinally ctxt computation finallyFunction)

        // Re-route the exception continuation to call to catchFunction. If catchFunction or the new process fail
        // then call the original exception continuation with the failure.
        let inline CreateTryWithFilterAsync catchFunction computation =
            MakeAsync (fun ctxt -> TryWith ctxt computation (fun edi -> catchFunction edi))

        // Re-route the exception continuation to call to catchFunction. If catchFunction or the new process fail
        // then call the original exception continuation with the failure.
        let inline CreateTryWithAsync catchFunction computation = 
            CreateTryWithFilterAsync (fun exn -> Some (catchFunction exn)) computation

        /// Call the finallyFunction if the computation results in a cancellation
        let CreateWhenCancelledAsync (finallyFunction : OperationCanceledException -> unit) computation =
            MakeAsync (fun ctxt ->
                let aux = ctxt.aux
                let ccont exn = 
                    let ctxt = { cont = (fun _ -> aux.ccont exn); aux = { aux with econt = (fun _ -> aux.ccont exn) } }
                    ProtectUserCodePlusHijackCheck ctxt finallyFunction exn
                let newCtxt = { ctxt with aux = { aux with ccont = ccont } }
                computation.Invoke newCtxt)

        /// A single pre-allocated computation that fetched the current cancellation token
        let cancellationTokenAsync  =
            MakeAsync (fun ctxt -> ctxt.cont ctxt.aux.token)
        
        /// A single pre-allocated computation that returns a unit result
        let unitAsync =
            CreateReturnAsync()

        /// Implement use/Dispose
        let CreateUsingAsync (resource:'T :> IDisposable) (computation:'T -> Async<'a>) : Async<'a> =
            let mutable x = 0
            let disposeFunction _ =
                if Interlocked.CompareExchange(&x, 1, 0) = 0 then
                    Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicFunctions.Dispose resource
            CreateTryFinallyAsync disposeFunction (CreateCallAsync computation resource) |> CreateWhenCancelledAsync disposeFunction

        let inline CreateIgnoreAsync computation = 
            CreateBindAsync computation (fun _ -> unitAsync)

        /// Implement the while loop construct of async computation expressions
        let CreateWhileAsync guardFunc computation =
            let mutable whileAsync = Unchecked.defaultof<_>
            if guardFunc() then 
                whileAsync <- CreateBindAsync computation (fun () -> if guardFunc() then whileAsync else unitAsync) 
                whileAsync
            else 
                unitAsync

        /// Implement the for loop construct of async commputation expressions
        let CreateForLoopAsync (source: seq<_>) computation =
            CreateUsingAsync (source.GetEnumerator()) (fun ie ->
                CreateWhileAsync
                    (fun () -> ie.MoveNext())
                    (CreateDelayAsync (fun () -> computation ie.Current)))

        let CreateSwitchToAsync (syncCtxt: SynchronizationContext) =
            CreateProtectedAsync (fun ctxt ->
                ctxt.aux.trampolineHolder.PostWithTrampoline syncCtxt ctxt.cont)

        let CreateSwitchToNewThreadAsync() =
            CreateProtectedAsync (fun ctxt ->
                ctxt.aux.trampolineHolder.StartThreadWithTrampoline ctxt.cont)

        let CreateSwitchToThreadPoolAsync() =
            CreateProtectedAsync (fun ctxt -> 
                ctxt.aux.trampolineHolder.QueueWorkItemWithTrampoline ctxt.cont)

        let delimitSyncContext ctxt =            
            match SynchronizationContext.Current with
            | null -> ctxt
            | syncCtxt -> 
                let aux = ctxt.aux                
                let trampolineHolder = aux.trampolineHolder
                {   ctxt with
                         cont = (fun x -> trampolineHolder.PostWithTrampoline syncCtxt (fun () -> ctxt.cont x))
                         aux = { aux with
                                     econt = (fun x -> trampolineHolder.PostWithTrampoline syncCtxt (fun () -> aux.econt x))
                                     ccont = (fun x -> trampolineHolder.PostWithTrampoline syncCtxt (fun () -> aux.ccont x)) }
                }

        // When run, ensures that each of the continuations of the process are run in the same synchronization context.
        let CreateDelimitedUserCodeAsync f = 
            CreateProtectedAsync (fun ctxt -> 
                let ctxtWithSync = delimitSyncContext ctxt
                f ctxtWithSync)

        [<Sealed>]
        [<AutoSerializable(false)>]        
        type SuspendedAsync<'T>(ctxt : AsyncActivation<'T>) =

            let syncCtxt = SynchronizationContext.Current

            let thread = 
                match syncCtxt with
                | null -> null // saving a thread-local access
                | _ -> Thread.CurrentThread 

            let trampolineHolder = ctxt.aux.trampolineHolder

            member __.ContinueImmediate res = 
                let action () = ctxt.cont res
                let inline executeImmediately () = trampolineHolder.ExecuteWithTrampoline action
                let currentSyncCtxt = SynchronizationContext.Current 
                match syncCtxt, currentSyncCtxt with
                | null, null -> 
                    executeImmediately ()
                // See bug 370350; this logic is incorrect from the perspective of how SynchronizationContext is meant to work,
                // but the logic works for mainline scenarios (WinForms/WPF/ASP.NET) and we won't change it again.
                | _ when Object.Equals(syncCtxt, currentSyncCtxt) && thread.Equals(Thread.CurrentThread) ->
                    executeImmediately ()
                | _ -> 
                    trampolineHolder.PostOrQueueWithTrampoline syncCtxt action

            member __.ContinueWithPostOrQueue res =
                trampolineHolder.PostOrQueueWithTrampoline syncCtxt (fun () -> ctxt.cont res)

        /// A utility type to provide a synchronization point between an asynchronous computation 
        /// and callers waiting on the result of that computation.
        ///
        /// Use with care! 
        [<Sealed>]        
        [<AutoSerializable(false)>]        
        type ResultCell<'T>() =

            let mutable result = None

            // The continuations for the result
            let mutable savedConts : list<SuspendedAsync<'T>> = []

            // The WaitHandle event for the result. Only created if needed, and set to null when disposed.
            let mutable resEvent = null

            let mutable disposed = false

            // All writers of result are protected by lock on syncRoot.
            let syncRoot = new Object()

            member x.GetWaitHandle() =
                lock syncRoot (fun () -> 
                    if disposed then 
                        raise (System.ObjectDisposedException("ResultCell"));
                    match resEvent with 
                    | null ->
                        // Start in signalled state if a result is already present.
                        let ev = new ManualResetEvent(result.IsSome)
                        resEvent <- ev
                        (ev :> WaitHandle)
                    | ev -> 
                        (ev :> WaitHandle))

            member x.Close() =
                lock syncRoot (fun () ->
                    if not disposed then 
                        disposed <- true;
                        match resEvent with
                        | null -> ()
                        | ev -> 
#if FX_NO_EVENTWAITHANDLE_IDISPOSABLE
                            ev.Dispose()                            
                            System.GC.SuppressFinalize(ev)
#else                        
                            ev.Close();
#endif                            
                            resEvent <- null)

            interface IDisposable with
                member x.Dispose() = x.Close()

            member x.GrabResult() =
                match result with
                | Some res -> res
                | None -> failwith "Unexpected no result"

            /// Record the result in the ResultCell.
            member x.RegisterResult (res:'T, reuseThread) =
                let grabbedConts = 
                    lock syncRoot (fun () ->
                        // Ignore multiple sets of the result. This can happen, e.g. for a race between a cancellation and a success
                        if x.ResultAvailable then 
                            [] // invalidOp "multiple results registered for asynchronous operation"
                        else
                            // In this case the ResultCell has already been disposed, e.g. due to a timeout.
                            // The result is dropped on the floor.
                            if disposed then 
                                []
                            else
                                result <- Some res;
                                // If the resEvent exists then set it. If not we can skip setting it altogether and it won't be
                                // created
                                match resEvent with
                                | null -> 
                                    ()
                                | ev ->
                                    // Setting the event need to happen under lock so as not to race with Close()
                                    ev.Set () |> ignore
                                List.rev savedConts)

                // Run the action outside the lock
                match grabbedConts with
                | [] -> FakeUnit
                | [cont] -> 
                    if reuseThread then
                        cont.ContinueImmediate(res)
                    else
                        cont.ContinueWithPostOrQueue(res)
                | otherwise ->
                    otherwise |> List.iter (fun cont -> cont.ContinueWithPostOrQueue(res) |> unfake) |> fake
            
            member x.ResultAvailable = result.IsSome

            /// Await the result of a result cell, without a direct timeout or direct
            /// cancellation. That is, the underlying computation must fill the result
            /// if cancellation or timeout occurs.
            member x.AwaitResult_NoDirectCancelOrTimeout  =
                MakeAsync (fun ctxt ->                    
                    // Check if a result is available synchronously                
                    let resOpt =
                        match result with
                        | Some _ -> result
                        | None ->                 
                            lock syncRoot (fun () ->
                                match result with
                                | Some _ ->
                                    result
                                | None ->
                                    // Otherwise save the continuation and call it in RegisterResult                                        
                                    savedConts <- (SuspendedAsync<_>(ctxt))::savedConts
                                    None
                            )
                    match resOpt with
                    | Some res -> ctxt.cont res
                    | None -> FakeUnit
                )

            member x.TryWaitForResultSynchronously (?timeout) : 'T option =
                // Check if a result is available.
                match result with
                | Some _ as r ->
                    r
                | None ->
                    // Force the creation of the WaitHandle
                    let resHandle = x.GetWaitHandle()
                    // Check again. While we were in GetWaitHandle, a call to RegisterResult may have set result then skipped the
                    // Set because the resHandle wasn't forced.
                    match result with
                    | Some _ as r ->
                        r
                    | None ->
                        // OK, let's really wait for the Set signal. This may block.
                        let timeout = defaultArg timeout Threading.Timeout.Infinite 
#if FX_NO_EXIT_CONTEXT_FLAGS
#if FX_NO_WAITONE_MILLISECONDS
                        let ok = resHandle.WaitOne(TimeSpan(int64(timeout)*10000L))
#else
                        let ok = resHandle.WaitOne(millisecondsTimeout= timeout) 
#endif                        
#else
                        let ok = resHandle.WaitOne(millisecondsTimeout= timeout,exitContext=true) 
#endif
                        if ok then
                            // Now the result really must be available
                            result
                        else
                            // timed out
                            None


        /// Create an instance of an arbitrary delegate type delegating to the given F# function
        type FuncDelegate<'T>(f) =
            member __.Invoke(sender:obj, a:'T) : unit = ignore(sender); f(a)
            static member Create<'Delegate when 'Delegate :> Delegate>(f) = 
                let obj = FuncDelegate<'T>(f)
                let invokeMeth = (typeof<FuncDelegate<'T>>).GetMethod("Invoke", BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance)
                System.Delegate.CreateDelegate(typeof<'Delegate>, obj, invokeMeth) :?> 'Delegate

        [<DebuggerHidden>]
        let QueueAsync cancellationToken cont econt ccont computation =
            let trampolineHolder = new TrampolineHolder()
            trampolineHolder.QueueWorkItemWithTrampoline (fun () -> 
                let ctxt = CreateAsyncActivation cancellationToken trampolineHolder cont econt ccont
                computation.Invoke ctxt)

        /// Run the asynchronous workflow and wait for its result.
        [<DebuggerHidden>]
        let RunSynchronouslyInAnotherThread (token:CancellationToken,computation,timeout) =
            let token,innerCTS = 
                // If timeout is provided, we govern the async by our own CTS, to cancel
                // when execution times out. Otherwise, the user-supplied token governs the async.
                match timeout with 
                | None -> token, None
                | Some _ ->
                    let subSource = new LinkedSubSource(token)
                    subSource.Token, Some subSource
                
            use resultCell = new ResultCell<AsyncResult<_>>()
            QueueAsync 
                    token                        
                    (fun res -> resultCell.RegisterResult(AsyncResult.Ok(res),reuseThread=true))
                    (fun edi -> resultCell.RegisterResult(AsyncResult.Error(edi),reuseThread=true))
                    (fun exn -> resultCell.RegisterResult(AsyncResult.Canceled(exn),reuseThread=true))
                    computation 
                |> unfake

            let res = resultCell.TryWaitForResultSynchronously(?timeout = timeout)
            match res with
            | None -> // timed out
                // issue cancellation signal
                if innerCTS.IsSome then innerCTS.Value.Cancel() 
                // wait for computation to quiesce; drop result on the floor
                resultCell.TryWaitForResultSynchronously() |> ignore 
                // dispose the CancellationTokenSource
                if innerCTS.IsSome then innerCTS.Value.Dispose()
                raise (System.TimeoutException())
            | Some res ->
                match innerCTS with
                | Some subSource -> subSource.Dispose()
                | None -> ()
                res.Commit()

        [<DebuggerHidden>]
        let RunSynchronouslyInCurrentThread (cancellationToken:CancellationToken,computation) =
            use resultCell = new ResultCell<AsyncResult<_>>()
            let trampolineHolder = new TrampolineHolder()

            trampolineHolder.ExecuteWithTrampoline (fun () ->
                let ctxt = 
                    CreateAsyncActivation
                        cancellationToken
                        trampolineHolder
                        (fun res -> resultCell.RegisterResult(AsyncResult.Ok(res),reuseThread=true))
                        (fun edi -> resultCell.RegisterResult(AsyncResult.Error(edi),reuseThread=true))
                        (fun exn -> resultCell.RegisterResult(AsyncResult.Canceled(exn),reuseThread=true))
                computation.Invoke ctxt)
            |> unfake

            let res = resultCell.TryWaitForResultSynchronously().Value
            res.Commit()

        [<DebuggerHidden>]
        let RunSynchronously cancellationToken (computation: Async<'T>) timeout =
            // Reuse the current ThreadPool thread if possible. Unfortunately
            // Thread.IsThreadPoolThread isn't available on all profiles so
            // we approximate it by testing synchronization context for null.
            match SynchronizationContext.Current, timeout with
            | null, None -> RunSynchronouslyInCurrentThread (cancellationToken, computation)
            // When the timeout is given we need a dedicated thread
            // which cancels the computation.
            // Performing the cancellation in the ThreadPool eg. by using
            // Timer from System.Threading or CancellationTokenSource.CancelAfter
            // (which internally uses Timer) won't work properly
            // when the ThreadPool is busy.
            //
            // And so when the timeout is given we always use the current thread
            // for the cancellation and run the computation in another thread.
            | _ -> RunSynchronouslyInAnotherThread (cancellationToken, computation, timeout)

        [<DebuggerHidden>]
        let Start cancellationToken (computation:Async<unit>) =
            QueueAsync 
                cancellationToken
                (fun () -> FakeUnit)   // nothing to do on success
                (fun edi -> edi.ThrowAny())   // raise exception in child
                (fun _ -> FakeUnit)    // ignore cancellation in child
                computation
            |> unfake

        [<DebuggerHidden>]
        let StartWithContinuations cancellationToken (computation:Async<'T>) cont econt ccont =
            let trampolineHolder = new TrampolineHolder()
            trampolineHolder.ExecuteWithTrampoline (fun () -> 
                let ctxt = CreateAsyncActivation cancellationToken trampolineHolder (cont >> fake) (econt >> fake) (ccont >> fake)
                computation.Invoke ctxt)
            |> unfake

        [<DebuggerHidden>]
        let StartAsTask cancellationToken (computation:Async<'T>) taskCreationOptions =
            let taskCreationOptions = defaultArg taskCreationOptions TaskCreationOptions.None
            let tcs = new TaskCompletionSource<_>(taskCreationOptions)

            // The contract: 
            //      a) cancellation signal should always propagate to the computation
            //      b) when the task IsCompleted -> nothing is running anymore
            let task = tcs.Task
            QueueAsync
                cancellationToken
                (fun r -> tcs.SetResult r |> fake)
                (fun edi -> tcs.SetException edi.SourceException |> fake)
                (fun _ -> tcs.SetCanceled() |> fake)
                computation
            |> unfake
            task

        // Helper to attach continuation to the given task.
        let taskContinueWith (task : Task<'T>) ctxt useCcontForTaskCancellation = 

            let continuation (completedTask: Task<_>) : unit =
                ctxt.aux.trampolineHolder.ExecuteWithTrampoline (fun () ->
                    if completedTask.IsCanceled then
                        if useCcontForTaskCancellation then
                            ctxt.OnCancellation ()
                        else 
                            let edi = ExceptionDispatchInfo.Capture(new TaskCanceledException(completedTask))
                            ctxt.CallExceptionContinuation edi
                    elif completedTask.IsFaulted then
                        let edi = ExceptionDispatchInfo.RestoreOrCapture(completedTask.Exception)
                        ctxt.CallExceptionContinuation edi
                    else
                        ctxt.cont completedTask.Result) |> unfake

            task.ContinueWith(Action<Task<'T>>(continuation)) |> ignore |> fake

        [<DebuggerHidden>]
        let taskContinueWithUnit (task: Task) ctxt useCcontForTaskCancellation = 

            let continuation (completedTask: Task) : unit =
                ctxt.aux.trampolineHolder.ExecuteWithTrampoline (fun () ->
                    if completedTask.IsCanceled then
                        if useCcontForTaskCancellation then
                            ctxt.OnCancellation ()
                        else 
                            let edi = ExceptionDispatchInfo.Capture(new TaskCanceledException(completedTask))
                            ctxt.CallExceptionContinuation edi
                    elif completedTask.IsFaulted then
                        let edi = ExceptionDispatchInfo.RestoreOrCapture(completedTask.Exception)
                        ctxt.CallExceptionContinuation edi
                    else
                        ctxt.cont ()) |> unfake

            task.ContinueWith(Action<Task>(continuation)) |> ignore |> fake

        [<Sealed>]
        [<AutoSerializable(false)>]
        type AsyncIAsyncResult<'T>(callback: System.AsyncCallback,state:obj) =
             // This gets set to false if the result is not available by the 
             // time the IAsyncResult is returned to the caller of Begin
             let mutable completedSynchronously = true 

             let mutable disposed = false

             let cts = new CancellationTokenSource()

             let result = new ResultCell<AsyncResult<'T>>()

             member s.SetResult(v: AsyncResult<'T>) =  
                 result.RegisterResult(v,reuseThread=true) |> unfake
                 match callback with
                 | null -> ()
                 | d -> 
                     // The IASyncResult becomes observable here
                     d.Invoke (s :> System.IAsyncResult)

             member s.GetResult() = 
                 match result.TryWaitForResultSynchronously (-1) with 
                 | Some (AsyncResult.Ok v) -> v
                 | Some (AsyncResult.Error edi) -> edi.ThrowAny()
                 | Some (AsyncResult.Canceled err) -> raise err
                 | None -> failwith "unreachable"

             member x.IsClosed = disposed

             member x.Close() = 
                 if not disposed  then
                     disposed <- true 
                     cts.Dispose()
                     result.Close()
                 
             member x.Token = cts.Token

             member x.CancelAsync() = cts.Cancel()

             member x.CheckForNotSynchronous() = 
                 if not result.ResultAvailable then 
                     completedSynchronously <- false

             interface System.IAsyncResult with
                  member x.IsCompleted = result.ResultAvailable
                  member x.CompletedSynchronously = completedSynchronously
                  member x.AsyncWaitHandle = result.GetWaitHandle()
                  member x.AsyncState = state

             interface System.IDisposable with
                 member x.Dispose() = x.Close()
    
        module AsBeginEndHelpers =
            let beginAction (computation, callback, state) = 
                let aiar = new AsyncIAsyncResult<'T>(callback, state)
                let cont v = aiar.SetResult (AsyncResult.Ok v)
                let econt v = aiar.SetResult (AsyncResult.Error v)
                let ccont v = aiar.SetResult (AsyncResult.Canceled v)
                StartWithContinuations aiar.Token computation cont econt ccont
                aiar.CheckForNotSynchronous()
                (aiar :> IAsyncResult)
               
            let endAction<'T> (iar:IAsyncResult) =
                match iar with 
                | :? AsyncIAsyncResult<'T> as aiar ->
                    if aiar.IsClosed then 
                        raise (System.ObjectDisposedException("AsyncResult"))
                    else
                        let res = aiar.GetResult()
                        aiar.Close ()
                        res
                | _ -> 
                    invalidArg "iar" (SR.GetString(SR.mismatchIAREnd))

            let cancelAction<'T>(iar:IAsyncResult) =
                match iar with 
                | :? AsyncIAsyncResult<'T> as aiar ->
                    aiar.CancelAsync()
                | _ -> 
                    invalidArg "iar" (SR.GetString(SR.mismatchIARCancel))

    open AsyncPrimitives
    
    [<Sealed>]
    [<CompiledName("FSharpAsyncBuilder")>]
    type AsyncBuilder() =
        member __.Zero () = unitAsync

        member __.Delay generator = CreateDelayAsync generator

        member inline __.Return value = CreateReturnAsync value

        member inline __.ReturnFrom (computation:Async<_>) = computation

        member inline __.Bind (computation, binder) = CreateBindAsync computation binder

        member __.Using (resource, binder) = CreateUsingAsync resource binder

        member __.While (guard, computation) = CreateWhileAsync guard computation

        member __.For (sequence, body) = CreateForLoopAsync sequence body

        member inline __.Combine (computation1, computation2) = CreateSequentialAsync computation1 computation2

        member inline __.TryFinally (computation, compensation) = CreateTryFinallyAsync compensation computation

        member inline __.TryWith (computation, catchHandler) = CreateTryWithAsync catchHandler computation

        // member inline __.TryWithFilter (computation, catchHandler) = CreateTryWithFilterAsync catchHandler computation

    [<AutoOpen>]
    module AsyncBuilderImpl = 
        let async = AsyncBuilder()

    [<Sealed>]
    [<CompiledName("FSharpAsync")>]
    type Async =
    
        static member CancellationToken = cancellationTokenAsync

        static member CancelCheck () = unitAsync

        static member FromContinuations (callback : ('T -> unit) * (exn -> unit) * (OperationCanceledException -> unit) -> unit) : Async<'T> = 
            MakeAsync (fun ctxt ->
                if ctxt.IsCancellationRequested then
                    ctxt.OnCancellation ()
                else
                    let mutable underCurrentThreadStack = true
                    let mutable contToTailCall = None
                    let thread = Thread.CurrentThread
                    let latch = Latch()
                    let aux = ctxt.aux
                    let once cont x = 
                        if not(latch.Enter()) then invalidOp(SR.GetString(SR.controlContinuationInvokedMultipleTimes))
                        if Thread.CurrentThread.Equals(thread) && underCurrentThreadStack then
                            contToTailCall <- Some(fun () -> cont x)
                        else if Trampoline.ThisThreadHasTrampoline then
                            let syncCtxt = SynchronizationContext.Current
                            aux.trampolineHolder.PostOrQueueWithTrampoline syncCtxt (fun () -> cont x) |> unfake 
                        else
                            aux.trampolineHolder.ExecuteWithTrampoline (fun () -> cont x ) |> unfake
                    try 
                        callback (once ctxt.cont, (fun exn -> once aux.econt (ExceptionDispatchInfo.RestoreOrCapture(exn))), once aux.ccont)
                    with exn -> 
                        if not(latch.Enter()) then invalidOp(SR.GetString(SR.controlContinuationInvokedMultipleTimes))
                        let edi = ExceptionDispatchInfo.RestoreOrCapture(exn)
                        aux.econt edi |> unfake

                    underCurrentThreadStack <- false

                    match contToTailCall with
                    | Some k -> k()
                    | _ -> FakeUnit)
                
        static member DefaultCancellationToken = defaultCancellationTokenSource.Token

        static member CancelDefaultToken() =

            let cts = defaultCancellationTokenSource

            // set new CancellationTokenSource before calling Cancel - otherwise if Cancel throws token will stay unchanged
            defaultCancellationTokenSource <- new CancellationTokenSource()

            cts.Cancel()

            // we do not dispose the old default CTS - let GC collect it
            
        static member Catch (computation: Async<'T>) =
            MakeAsync (fun ctxt ->
                let cont = (Choice1Of2 >> ctxt.cont)
                let econt (edi: ExceptionDispatchInfo) = ctxt.cont (Choice2Of2 (edi.GetAssociatedSourceException()))
                let ctxt = { cont = cont; aux = { ctxt.aux with econt = econt } }
                computation.Invoke ctxt)

        static member RunSynchronously (computation: Async<'T>,?timeout,?cancellationToken:CancellationToken) =
            let timeout, cancellationToken =
                match cancellationToken with
                | None -> timeout,defaultCancellationTokenSource.Token
                | Some token when not token.CanBeCanceled -> timeout, token
                | Some token -> None, token
            AsyncPrimitives.RunSynchronously cancellationToken computation timeout

        static member Start (computation, ?cancellationToken) =
            let cancellationToken = defaultArg cancellationToken defaultCancellationTokenSource.Token
            AsyncPrimitives.Start cancellationToken computation

        static member StartAsTask (computation,?taskCreationOptions,?cancellationToken)=
            let cancellationToken = defaultArg cancellationToken defaultCancellationTokenSource.Token        
            AsyncPrimitives.StartAsTask cancellationToken computation taskCreationOptions
        
        static member StartChildAsTask (computation,?taskCreationOptions) =
            async { let! cancellationToken = cancellationTokenAsync  
                    return AsyncPrimitives.StartAsTask cancellationToken computation taskCreationOptions }

        static member Parallel (computations: seq<Async<'T>>) =
            MakeAsync (fun ctxt ->
                let tasks, result = 
                    try 
                        Seq.toArray computations, None   // manually protect eval of seq
                    with exn -> 
                        let edi = ExceptionDispatchInfo.RestoreOrCapture(exn)
                        null, Some (ctxt.CallExceptionContinuation edi)

                match result with
                | Some r -> r
                | None ->
                if tasks.Length = 0 then 
                    // must not be in a 'protect' if we call cont explicitly; if cont throws, it should unwind the stack, preserving Dev10 behavior
                    ctxt.cont [| |] 
                else  
                  ProtectUserCode ctxt (fun ctxt ->
                    let ctxtWithSync = delimitSyncContext ctxt  // manually resync
                    let aux = ctxtWithSync.aux
                    let count = ref tasks.Length
                    let firstExn = ref None
                    let results = Array.zeroCreate tasks.Length
                    // Attempt to cancel the individual operations if an exception happens on any of the other threads
                    let innerCTS = new LinkedSubSource(aux.token)
                    let trampolineHolder = aux.trampolineHolder
                    
                    let finishTask(remaining) = 
                        if (remaining = 0) then 
                            innerCTS.Dispose()
                            match (!firstExn) with 
                            | None -> trampolineHolder.ExecuteWithTrampoline (fun () -> ctxtWithSync.cont results)
                            | Some (Choice1Of2 exn) -> trampolineHolder.ExecuteWithTrampoline (fun () -> aux.econt exn)
                            | Some (Choice2Of2 cexn) -> trampolineHolder.ExecuteWithTrampoline (fun () -> aux.ccont cexn)
                        else
                            FakeUnit

                    // recordSuccess and recordFailure between them decrement count to 0 and 
                    // as soon as 0 is reached dispose innerCancellationSource
                
                    let recordSuccess i res = 
                        results.[i] <- res;
                        finishTask(Interlocked.Decrement count) 

                    let recordFailure exn = 
                        // capture first exception and then decrement the counter to avoid race when
                        // - thread 1 decremented counter and preempted by the scheduler
                        // - thread 2 decremented counter and called finishTask
                        // since exception is not yet captured - finishtask will fall into success branch
                        match Interlocked.CompareExchange(firstExn, Some exn, None) with
                        | None -> 
                            // signal cancellation before decrementing the counter - this guarantees that no other thread can sneak to finishTask and dispose innerCTS
                            // NOTE: Cancel may introduce reentrancy - i.e. when handler registered for the cancellation token invokes cancel continuation that will call 'recordFailure'
                            // to correctly handle this we need to return decremented value, not the current value of 'count' otherwise we may invoke finishTask with value '0' several times
                            innerCTS.Cancel()
                        | _ -> ()
                        finishTask(Interlocked.Decrement count)
                
                    tasks |> Array.iteri (fun i p ->
                        QueueAsync
                                innerCTS.Token
                                // on success, record the result
                                (fun res -> recordSuccess i res)
                                // on exception...
                                (fun edi -> recordFailure (Choice1Of2 edi))
                                // on cancellation...
                                (fun cexn -> recordFailure (Choice2Of2 cexn))
                                p
                            |> unfake);
                    FakeUnit))

        static member Choice(computations : Async<'T option> seq) : Async<'T option> =
            MakeAsync (fun ctxt ->
                let result =
                    try Seq.toArray computations |> Choice1Of2
                    with exn -> ExceptionDispatchInfo.RestoreOrCapture exn |> Choice2Of2

                match result with
                | Choice2Of2 edi -> ctxt.CallExceptionContinuation edi
                | Choice1Of2 [||] -> ctxt.cont None
                | Choice1Of2 computations ->
                    ProtectUserCode ctxt (fun ctxt ->
                        let ctxtWithSync = delimitSyncContext ctxt
                        let aux = ctxtWithSync.aux
                        let noneCount = ref 0
                        let exnCount = ref 0
                        let innerCts = new LinkedSubSource(aux.token)
                        let trampolineHolder = aux.trampolineHolder

                        let scont (result : 'T option) =
                            match result with
                            | Some _ -> 
                                if Interlocked.Increment exnCount = 1 then
                                    innerCts.Cancel(); trampolineHolder.ExecuteWithTrampoline (fun () -> ctxtWithSync.cont result)
                                else
                                    FakeUnit

                            | None ->
                                if Interlocked.Increment noneCount = computations.Length then
                                    innerCts.Cancel(); trampolineHolder.ExecuteWithTrampoline (fun () -> ctxtWithSync.cont None)
                                else
                                    FakeUnit
 
                        let econt (exn : ExceptionDispatchInfo) =
                            if Interlocked.Increment exnCount = 1 then 
                                innerCts.Cancel(); trampolineHolder.ExecuteWithTrampoline (fun () -> ctxtWithSync.aux.econt exn)
                            else
                                FakeUnit
 
                        let ccont (exn : OperationCanceledException) =
                            if Interlocked.Increment exnCount = 1 then
                                innerCts.Cancel(); trampolineHolder.ExecuteWithTrampoline (fun () -> ctxtWithSync.aux.ccont exn)
                            else
                                FakeUnit

                        for c in computations do
                            QueueAsync innerCts.Token scont econt ccont c |> unfake

                        FakeUnit))

    type Async with

        /// StartWithContinuations, except the exception continuation is given an ExceptionDispatchInfo
        static member StartWithContinuationsUsingDispatchInfo(computation:Async<'T>, continuation, exceptionContinuation, cancellationContinuation, ?cancellationToken) : unit =
            let cancellationToken = defaultArg cancellationToken defaultCancellationTokenSource.Token
            AsyncPrimitives.StartWithContinuations cancellationToken computation continuation exceptionContinuation cancellationContinuation

        static member StartWithContinuations(computation:Async<'T>, continuation, exceptionContinuation, cancellationContinuation, ?cancellationToken) : unit =
            Async.StartWithContinuationsUsingDispatchInfo(computation, continuation, (fun edi -> exceptionContinuation (edi.GetAssociatedSourceException())), cancellationContinuation, ?cancellationToken=cancellationToken)

        static member StartImmediateAsTask (computation : Async<'T>, ?cancellationToken ) : Task<'T>=
            let cancellationToken = defaultArg cancellationToken defaultCancellationTokenSource.Token
            let ts = new TaskCompletionSource<'T>()
            let task = ts.Task
            Async.StartWithContinuations(
                computation, 
                (fun (k) -> ts.SetResult(k)), 
                (fun exn -> ts.SetException(exn)), 
                (fun _ -> ts.SetCanceled()), 
                cancellationToken)
            task

        static member StartImmediate(computation:Async<unit>, ?cancellationToken) : unit =
            let cancellationToken = defaultArg cancellationToken defaultCancellationTokenSource.Token
            AsyncPrimitives.StartWithContinuations cancellationToken computation id (fun edi -> edi.ThrowAny()) ignore

        static member Sleep(millisecondsDueTime) : Async<unit> =
            CreateDelimitedUserCodeAsync (fun ctxt ->
                let aux = ctxt.aux
                let timer = ref (None : Timer option)
                let savedCont = ctxt.cont
                let savedCCont = aux.ccont
                let latch = new Latch()
                let registration =
                    aux.token.Register(
                        (fun _ -> 
                            if latch.Enter() then
                                match !timer with
                                | None -> ()
                                | Some t -> t.Dispose()
                                aux.trampolineHolder.ExecuteWithTrampoline (fun () -> savedCCont(new OperationCanceledException(aux.token))) |> unfake
                            ),
                        null)
                let mutable edi = null
                try
                    timer := new Timer((fun _ ->
                                        if latch.Enter() then
                                            // NOTE: If the CTS for the token would have been disposed, disposal of the registration would throw
                                            // However, our contract is that until async computation ceases execution (and invokes ccont) 
                                            // the CTS will not be disposed. Execution of savedCCont is guarded by latch, so we are safe unless
                                            // user violates the contract.
                                            registration.Dispose()
                                            // Try to Dispose of the TImer.
                                            // Note: there is a race here: the Timer time very occasionally
                                            // calls the callback _before_ the timer object has been recorded anywhere. This makes it difficult to dispose the
                                            // timer in this situation. In this case we just let the timer be collected by finalization.
                                            match !timer with
                                            | None -> ()
                                            | Some t -> t.Dispose()
                                            // Now we're done, so call the continuation
                                            aux.trampolineHolder.ExecuteWithTrampoline (fun () -> savedCont()) |> unfake),
                                     null, dueTime=millisecondsDueTime, period = -1) |> Some
                with exn -> 
                    if latch.Enter() then 
                        edi <- ExceptionDispatchInfo.RestoreOrCapture(exn) // post exception to econt only if we successfully enter the latch (no other continuations were called)

                match edi with 
                | null -> 
                    FakeUnit
                | _ -> 
                    aux.econt edi
                )
        
        /// Wait for a wait handle. Both timeout and cancellation are supported
        static member AwaitWaitHandle(waitHandle: WaitHandle, ?millisecondsTimeout:int) =
            let millisecondsTimeout = defaultArg millisecondsTimeout Threading.Timeout.Infinite
            if millisecondsTimeout = 0 then 
                async.Delay(fun () ->
#if FX_NO_EXIT_CONTEXT_FLAGS
#if FX_NO_WAITONE_MILLISECONDS
                    let ok = waitHandle.WaitOne(TimeSpan(0L))
#else
                    let ok = waitHandle.WaitOne(0)
#endif                    
#else
                    let ok = waitHandle.WaitOne(0,exitContext=false)
#endif
                    async.Return ok)
            else
                CreateDelimitedUserCodeAsync(fun ctxt ->
                    let aux = ctxt.aux
                    let rwh = ref (None : RegisteredWaitHandle option)
                    let latch = Latch()
                    let rec cancelHandler =
                        Action<obj>(fun _ ->
                            if latch.Enter() then
                                // if we got here - then we need to unregister RegisteredWaitHandle + trigger cancellation
                                // entrance to TP callback is protected by latch - so savedCont will never be called
                                lock rwh (fun () ->
                                    match !rwh with
                                    | None -> ()
                                    | Some rwh -> rwh.Unregister(null) |> ignore)
                                Async.Start (async { do (aux.ccont (OperationCanceledException(aux.token)) |> unfake) }))

                    and registration : CancellationTokenRegistration = aux.token.Register(cancelHandler, null)
                    
                    let savedCont = ctxt.cont
                    try
                        lock rwh (fun () ->
                            rwh := Some(ThreadPool.RegisterWaitForSingleObject
                                          (waitObject=waitHandle,
                                           callBack=WaitOrTimerCallback(fun _ timeOut ->
                                                        if latch.Enter() then
                                                            lock rwh (fun () -> rwh.Value.Value.Unregister(null) |> ignore)
                                                            rwh := None
                                                            registration.Dispose()
                                                            aux.trampolineHolder.ExecuteWithTrampoline (fun () -> savedCont (not timeOut)) |> unfake),
                                           state=null,
                                           millisecondsTimeOutInterval=millisecondsTimeout,
                                           executeOnlyOnce=true));
                            FakeUnit)
                    with _ -> 
                        if latch.Enter() then
                            registration.Dispose()
                            reraise() // reraise exception only if we successfully enter the latch (no other continuations were called)
                        else 
                            FakeUnit
                    )

        static member AwaitIAsyncResult(iar: IAsyncResult, ?millisecondsTimeout): Async<bool> =
            async { if iar.CompletedSynchronously then 
                        return true
                    else
                        return! Async.AwaitWaitHandle(iar.AsyncWaitHandle, ?millisecondsTimeout=millisecondsTimeout)  }


        /// Bind the result of a result cell, calling the appropriate continuation.
        static member BindResult (result: AsyncResult<'T>) : Async<'T> =
            MakeAsync (fun ctxt -> 
                   (match result with 
                    | Ok v -> ctxt.cont v 
                    | Error exn -> ctxt.CallExceptionContinuation exn 
                    | Canceled exn -> ctxt.aux.ccont exn) )

        /// Await and use the result of a result cell. The resulting async doesn't support cancellation
        /// or timeout directly, rather the underlying computation must fill the result if cancellation
        /// or timeout occurs.
        static member AwaitAndBindResult_NoDirectCancelOrTimeout(resultCell: ResultCell<AsyncResult<'T>>) : Async<'T> =
            async {
                let! result = resultCell.AwaitResult_NoDirectCancelOrTimeout
                return! Async.BindResult result
            }

        /// Await the result of a result cell belonging to a child computation.  The resulting async supports timeout and if
        /// it happens the child computation will be cancelled.   The resulting async doesn't support cancellation
        /// directly, rather the underlying computation must fill the result if cancellation occurs.
        static member AwaitAndBindChildResult(innerCTS: CancellationTokenSource, resultCell: ResultCell<AsyncResult<'T>>, millisecondsTimeout) : Async<'T> =
            match millisecondsTimeout with
            | None | Some -1 -> 
                resultCell |> Async.AwaitAndBindResult_NoDirectCancelOrTimeout

            | Some 0 -> 
                async { if resultCell.ResultAvailable then 
                            let res = resultCell.GrabResult()
                            return res.Commit()
                        else
                            return raise (System.TimeoutException()) }
            | _ ->
                async { try 
                           if resultCell.ResultAvailable then 
                             let res = resultCell.GrabResult()
                             return res.Commit()
                           else
                             let! ok = Async.AwaitWaitHandle (resultCell.GetWaitHandle(), ?millisecondsTimeout=millisecondsTimeout) 
                             if ok then
                                let res = resultCell.GrabResult()
                                return res.Commit()
                             else // timed out
                                // issue cancellation signal
                                innerCTS.Cancel()
                                // wait for computation to quiesce
                                let! _ = Async.AwaitWaitHandle (resultCell.GetWaitHandle())                                
                                return raise (System.TimeoutException())
                         finally 
                           resultCell.Close() } 


        static member FromBeginEnd(beginAction, endAction, ?cancelAction): Async<'T> =
            async { let! cancellationToken = cancellationTokenAsync
                    let resultCell = new ResultCell<_>()

                    let once = Once()

                    let registration : CancellationTokenRegistration = 

                        let onCancel (_:obj) = 
                            // Call the cancellation routine
                            match cancelAction with 
                            | None -> 
                                // Register the result. This may race with a successful result, but
                                // ResultCell allows a race and throws away whichever comes last.
                                once.Do(fun () ->
                                            let canceledResult = Canceled (OperationCanceledException(cancellationToken))
                                            resultCell.RegisterResult(canceledResult,reuseThread=true) |> unfake
                                )
                            | Some cancel -> 
                                // If we get an exception from a cooperative cancellation function
                                // we assume the operation has already completed.
                                try cancel() with _ -> ()

                        cancellationToken.Register(Action<obj>(onCancel), null)
                                
                    let callback = 
                        new System.AsyncCallback(fun iar -> 
                                if not iar.CompletedSynchronously then 
                                    // The callback has been activated, so ensure cancellation is not possible
                                    // beyond this point. 
                                    match cancelAction with
                                    | Some _ -> 
                                            registration.Dispose()
                                    | None -> 
                                            once.Do(fun () -> registration.Dispose())

                                    // Run the endAction and collect its result.
                                    let res = 
                                        try 
                                            Ok(endAction iar) 
                                        with exn -> 
                                            let edi = ExceptionDispatchInfo.RestoreOrCapture(exn)
                                            Error edi

                                    // Register the result. This may race with a cancellation result, but
                                    // ResultCell allows a race and throws away whichever comes last.
                                    resultCell.RegisterResult(res,reuseThread=true) |> unfake)
                    
                    let (iar:IAsyncResult) = beginAction (callback,(null:obj))
                    if iar.CompletedSynchronously then 
                        registration.Dispose()
                        return endAction iar 
                    else 
                        // Note: ok to use "NoDirectCancel" here because cancellation has been registered above
                        // Note: ok to use "NoDirectTimeout" here because no timeout parameter to this method
                        return! Async.AwaitAndBindResult_NoDirectCancelOrTimeout(resultCell) }


        static member FromBeginEnd(arg,beginAction,endAction,?cancelAction): Async<'T> =
            Async.FromBeginEnd((fun (iar,state) -> beginAction(arg,iar,state)), endAction, ?cancelAction=cancelAction)

        static member FromBeginEnd(arg1,arg2,beginAction,endAction,?cancelAction): Async<'T> =
            Async.FromBeginEnd((fun (iar,state) -> beginAction(arg1,arg2,iar,state)), endAction, ?cancelAction=cancelAction)

        static member FromBeginEnd(arg1,arg2,arg3,beginAction,endAction,?cancelAction): Async<'T> =
            Async.FromBeginEnd((fun (iar,state) -> beginAction(arg1,arg2,arg3,iar,state)), endAction, ?cancelAction=cancelAction)

        static member AsBeginEnd<'Arg,'T> (computation:('Arg -> Async<'T>)) :
                // The 'Begin' member
                ('Arg * System.AsyncCallback * obj -> System.IAsyncResult) * 
                // The 'End' member
                (System.IAsyncResult -> 'T) * 
                // The 'Cancel' member
                (System.IAsyncResult -> unit) =
                    let beginAction = fun (a1,callback,state) -> AsBeginEndHelpers.beginAction ((computation a1), callback, state)
                    beginAction, AsBeginEndHelpers.endAction<'T>, AsBeginEndHelpers.cancelAction<'T>

        static member AwaitEvent(event:IEvent<'Delegate,'T>, ?cancelAction) : Async<'T> =
            async { let! cancellationToken = cancellationTokenAsync
                    let resultCell = new ResultCell<_>()
                    // Set up the handlers to listen to events and cancellation
                    let once = new Once()
                    let rec registration : CancellationTokenRegistration= 
                        let onCancel _ =
                            // We've been cancelled. Call the given cancellation routine
                            match cancelAction with 
                            | None -> 
                                // We've been cancelled without a cancel action. Stop listening to events
                                event.RemoveHandler(del)
                                // Register the result. This may race with a successful result, but
                                // ResultCell allows a race and throws away whichever comes last.
                                once.Do(fun () -> resultCell.RegisterResult(Canceled (OperationCanceledException(cancellationToken)),reuseThread=true) |> unfake) 
                            | Some cancel -> 
                                // If we get an exception from a cooperative cancellation function
                                // we assume the operation has already completed.
                                try cancel() with _ -> ()
                        cancellationToken.Register(Action<obj>(onCancel), null)
                    
                    and del = 
                        FuncDelegate<'T>.Create<'Delegate>(fun eventArgs ->
                            // Stop listening to events
                            event.RemoveHandler(del)
                            // The callback has been activated, so ensure cancellation is not possible beyond this point
                            once.Do(fun () -> registration.Dispose())
                            let res = Ok(eventArgs) 
                            // Register the result. This may race with a cancellation result, but
                            // ResultCell allows a race and throws away whichever comes last.
                            resultCell.RegisterResult(res,reuseThread=true) |> unfake) 

                    // Start listening to events
                    event.AddHandler(del)

                    // Return the async computation that allows us to await the result
                    // Note: ok to use "NoDirectCancel" here because cancellation has been registered above
                    // Note: ok to use "NoDirectTimeout" here because no timeout parameter to this method
                    return! Async.AwaitAndBindResult_NoDirectCancelOrTimeout(resultCell) }

        static member Ignore (computation: Async<'T>) = CreateIgnoreAsync computation

        static member SwitchToNewThread() = CreateSwitchToNewThreadAsync()

        static member SwitchToThreadPool() = CreateSwitchToThreadPoolAsync()

        static member StartChild (computation:Async<'T>,?millisecondsTimeout) =
            async { 
                let resultCell = new ResultCell<_>()
                let! cancellationToken = cancellationTokenAsync
                let innerCTS = new CancellationTokenSource() // innerCTS does not require disposal
                let ctsRef = ref innerCTS
                let reg = cancellationToken.Register(
                                        (fun _ -> 
                                            match !ctsRef with
                                            | null -> ()
                                            | otherwise -> otherwise.Cancel()), 
                                        null)
                do QueueAsync 
                       innerCTS.Token
                       // since innerCTS is not ever Disposed, can call reg.Dispose() without a safety Latch
                       (fun res -> ctsRef := null; reg.Dispose(); resultCell.RegisterResult (Ok res, reuseThread=true))   
                       (fun edi -> ctsRef := null; reg.Dispose(); resultCell.RegisterResult (Error edi,reuseThread=true))   
                       (fun err -> ctsRef := null; reg.Dispose(); resultCell.RegisterResult (Canceled err,reuseThread=true))    
                       computation
                     |> unfake
                                               
                return Async.AwaitAndBindChildResult(innerCTS, resultCell, millisecondsTimeout) }

        static member SwitchToContext syncContext =
            async { match syncContext with 
                    | null -> 
                        // no synchronization context, just switch to the thread pool
                        do! Async.SwitchToThreadPool()
                    | syncCtxt -> 
                        // post the continuation to the synchronization context
                        return! CreateSwitchToAsync syncCtxt }

        static member OnCancel interruption =
            async { let! cancellationToken = cancellationTokenAsync
                    // latch protects CancellationTokenRegistration.Dispose from being called twice
                    let latch = Latch()
                    let rec handler (_ : obj) = 
                        try 
                            if latch.Enter() then registration.Dispose()
                            interruption () 
                        with _ -> ()                        
                    and registration : CancellationTokenRegistration = cancellationToken.Register(Action<obj>(handler), null)
                    return { new System.IDisposable with
                                member this.Dispose() = 
                                    // dispose CancellationTokenRegistration only if cancellation was not requested.
                                    // otherwise - do nothing, disposal will be performed by the handler itself
                                    if not cancellationToken.IsCancellationRequested then
                                        if latch.Enter() then registration.Dispose() } }

        static member TryCancelled (computation: Async<'T>,compensation) = 
            CreateWhenCancelledAsync compensation computation

        static member AwaitTask (task:Task<'T>) : Async<'T> = 
            CreateDelimitedUserCodeAsync (fun ctxt -> taskContinueWith task ctxt false)

        static member AwaitTask (task:Task) : Async<unit> = 
            CreateDelimitedUserCodeAsync (fun ctxt -> taskContinueWithUnit task ctxt false)

    module CommonExtensions =

        type System.IO.Stream with

            [<CompiledName("AsyncRead")>] // give the extension member a 'nice', unmangled compiled name, unique within this module
            member stream.AsyncRead(buffer: byte[],?offset,?count) =
                let offset = defaultArg offset 0
                let count  = defaultArg count buffer.Length
#if FX_NO_BEGINEND_READWRITE
                // use combo CreateDelimitedUserCodeAsync + taskContinueWith instead of AwaitTask so we can pass cancellation token to the ReadAsync task
                CreateDelimitedUserCodeAsync (fun ctxt -> taskContinueWith (stream.ReadAsync(buffer, offset, count, ctxt.aux.token)) ctxt false)
#else
                Async.FromBeginEnd (buffer,offset,count,stream.BeginRead,stream.EndRead)
#endif

            [<CompiledName("AsyncReadBytes")>] // give the extension member a 'nice', unmangled compiled name, unique within this module
            member stream.AsyncRead(count) =
                async { let buffer = Array.zeroCreate count
                        let i = ref 0
                        while !i < count do
                            let! n = stream.AsyncRead(buffer,!i,count - !i)
                            i := !i + n
                            if n = 0 then 
                                raise(System.IO.EndOfStreamException(SR.GetString(SR.failedReadEnoughBytes)))
                        return buffer }
            
            [<CompiledName("AsyncWrite")>] // give the extension member a 'nice', unmangled compiled name, unique within this module
            member stream.AsyncWrite(buffer:byte[], ?offset:int, ?count:int) =
                let offset = defaultArg offset 0
                let count  = defaultArg count buffer.Length
#if FX_NO_BEGINEND_READWRITE
                // use combo CreateDelimitedUserCodeAsync + taskContinueWithUnit instead of AwaitTask so we can pass cancellation token to the WriteAsync task
                CreateDelimitedUserCodeAsync (fun ctxt -> taskContinueWithUnit (stream.WriteAsync(buffer, offset, count, ctxt.aux.token)) ctxt false)
#else
                Async.FromBeginEnd (buffer,offset,count,stream.BeginWrite,stream.EndWrite)
#endif
                
        type IObservable<'Args> with 

            [<CompiledName("AddToObservable")>] // give the extension member a 'nice', unmangled compiled name, unique within this module
            member x.Add(callback: 'Args -> unit) = x.Subscribe callback |> ignore

            [<CompiledName("SubscribeToObservable")>] // give the extension member a 'nice', unmangled compiled name, unique within this module
            member x.Subscribe(callback) = 
                x.Subscribe { new IObserver<'Args> with 
                                  member x.OnNext(args) = callback args 
                                  member x.OnError(e) = () 
                                  member x.OnCompleted() = () } 

    module WebExtensions =
        open AsyncPrimitives

        type System.Net.WebRequest with
            [<CompiledName("AsyncGetResponse")>] // give the extension member a 'nice', unmangled compiled name, unique within this module
            member req.AsyncGetResponse() : Async<System.Net.WebResponse>= 
                
                let canceled = ref false // WebException with Status = WebExceptionStatus.RequestCanceled  can be raised in other situations except cancellation, use flag to filter out false positives

                // Use CreateTryWithFilterAsync to allow propagation of exception without losing stack
                Async.FromBeginEnd(beginAction=req.BeginGetResponse, 
                                   endAction = req.EndGetResponse, 
                                   cancelAction = fun() -> canceled := true; req.Abort())
                |> CreateTryWithFilterAsync (fun exn ->
                    match exn with 
                    | :? System.Net.WebException as webExn 
                            when webExn.Status = System.Net.WebExceptionStatus.RequestCanceled && !canceled -> 

                        Some (Async.BindResult(AsyncResult.Canceled (OperationCanceledException webExn.Message)))
                    | _ -> 
                        None)

#if !FX_NO_WEB_CLIENT
        
        type System.Net.WebClient with
            member inline private this.Download(event: IEvent<'T, _>, handler: _ -> 'T, start, result) =
                let downloadAsync =
                    Async.FromContinuations (fun (cont, econt, ccont) ->
                        let userToken = new obj()
                        let rec delegate' (_: obj) (args : #ComponentModel.AsyncCompletedEventArgs) =
                            // ensure we handle the completed event from correct download call
                            if userToken = args.UserState then
                                event.RemoveHandler handle
                                if args.Cancelled then
                                    ccont (new OperationCanceledException())
                                elif isNotNull args.Error then
                                    econt args.Error
                                else
                                    cont (result args)
                        and handle = handler delegate'
                        event.AddHandler handle
                        start userToken
                    )

                async {
                    use! _holder = Async.OnCancel(fun _ -> this.CancelAsync())
                    return! downloadAsync
                 }

            [<CompiledName("AsyncDownloadString")>] // give the extension member a 'nice', unmangled compiled name, unique within this module
            member this.AsyncDownloadString (address:Uri) : Async<string> =
                this.Download(
                    event   = this.DownloadStringCompleted,
                    handler = (fun action    -> Net.DownloadStringCompletedEventHandler(action)),
                    start   = (fun userToken -> this.DownloadStringAsync(address, userToken)),
                    result  = (fun args      -> args.Result)
                )

            [<CompiledName("AsyncDownloadData")>] // give the extension member a 'nice', unmangled compiled name, unique within this module
            member this.AsyncDownloadData (address:Uri) : Async<byte[]> =
                this.Download(
                    event   = this.DownloadDataCompleted,
                    handler = (fun action    -> Net.DownloadDataCompletedEventHandler(action)),
                    start   = (fun userToken -> this.DownloadDataAsync(address, userToken)),
                    result  = (fun args      -> args.Result)
                )

            [<CompiledName("AsyncDownloadFile")>] // give the extension member a 'nice', unmangled compiled name, unique within this module
            member this.AsyncDownloadFile (address:Uri, fileName:string) : Async<unit> =
                this.Download(
                    event   = this.DownloadFileCompleted,
                    handler = (fun action    -> ComponentModel.AsyncCompletedEventHandler(action)),
                    start   = (fun userToken -> this.DownloadFileAsync(address, fileName, userToken)),
                    result  = (fun _         -> ())
                )
#endif


    open CommonExtensions

    module AsyncHelpers =

        let awaitEither a1 a2 =
            async {
                let resultCell = new ResultCell<_>()
                let! cancellationToken = Async.CancellationToken
                let start a f =
                    Async.StartWithContinuationsUsingDispatchInfo(a, 
                        (fun res -> resultCell.RegisterResult(f res |> AsyncResult.Ok, reuseThread=false) |> unfake),
                        (fun edi -> resultCell.RegisterResult(edi |> AsyncResult.Error, reuseThread=false) |> unfake),
                        (fun oce -> resultCell.RegisterResult(oce |> AsyncResult.Canceled, reuseThread=false) |> unfake),
                        cancellationToken = cancellationToken
                        )
                start a1 Choice1Of2
                start a2 Choice2Of2
                // Note: It is ok to use "NoDirectCancel" here because the started computations use the same
                //       cancellation token and will register a cancelled result if cancellation occurs.
                // Note: It is ok to use "NoDirectTimeout" here because there is no specific timeout log to this routine.
                let! result = resultCell.AwaitResult_NoDirectCancelOrTimeout
                return! CreateAsyncResultAsync result
            }

        let timeout msec cancellationToken =
            if msec < 0 then
                MakeAsync (fun _ -> FakeUnit) // "block" forever
            else
                let resultCell = new ResultCell<_>()
                Async.StartWithContinuations(
                    computation=Async.Sleep(msec),
                    continuation=(fun () -> resultCell.RegisterResult((), reuseThread = false) |> unfake),
                    exceptionContinuation=ignore, 
                    cancellationContinuation=ignore, 
                    cancellationToken = cancellationToken)
                // Note: It is ok to use "NoDirectCancel" here because the started computations use the same
                //       cancellation token and will register a cancelled result if cancellation occurs.
                // Note: It is ok to use "NoDirectTimeout" here because the child compuation above looks after the timeout.
                resultCell.AwaitResult_NoDirectCancelOrTimeout

    [<Sealed>]
    [<AutoSerializable(false)>]        
    type Mailbox<'Msg>(cancellationSupported: bool) =  
        let mutable inboxStore  = null 
        let mutable arrivals = new Queue<'Msg>()
        let syncRoot = arrivals

        // Control elements indicating the state of the reader. When the reader is "blocked" at an 
        // asynchronous receive, either 
        //     -- "cont" is non-null and the reader is "activated" by re-scheduling cont in the thread pool; or
        //     -- "pulse" is non-null and the reader is "activated" by setting this event
        let mutable savedCont : ((bool -> AsyncReturn) * TrampolineHolder) option = None

        // Readers who have a timeout use this event
        let mutable pulse : AutoResetEvent = null

        // Make sure that the "pulse" value is created
        let ensurePulse() = 
            match pulse with 
            | null -> 
                pulse <- new AutoResetEvent(false);
            | _ -> 
                ()
            pulse
                
        let waitOneNoTimeoutOrCancellation = 
            MakeAsync (fun ctxt -> 
                match savedCont with 
                | None -> 
                    let descheduled = 
                        // An arrival may have happened while we're preparing to deschedule
                        lock syncRoot (fun () -> 
                            if arrivals.Count = 0 then 
                                // OK, no arrival so deschedule
                                savedCont <- Some(ctxt.cont, ctxt.aux.trampolineHolder);
                                true
                            else
                                false)
                    if descheduled then 
                        FakeUnit 
                    else 
                        // If we didn't deschedule then run the continuation immediately
                        ctxt.cont true
                | Some _ -> 
                    failwith "multiple waiting reader continuations for mailbox")

        let waitOneWithCancellation timeout = 
            Async.AwaitWaitHandle(ensurePulse(), millisecondsTimeout=timeout)

        let waitOne timeout = 
            if timeout < 0 && not cancellationSupported then 
                waitOneNoTimeoutOrCancellation
            else 
                waitOneWithCancellation(timeout)

        member __.inbox = 
            match inboxStore with 
            | null -> inboxStore <- new System.Collections.Generic.List<'Msg>(1)
            | _ -> () 
            inboxStore

        member x.CurrentQueueLength = 
            lock syncRoot (fun () -> x.inbox.Count + arrivals.Count)

        member x.ScanArrivalsUnsafe(f) =
            if arrivals.Count = 0 then 
                None
            else 
                let msg = arrivals.Dequeue()
                match f msg with
                | None -> 
                    x.inbox.Add(msg)
                    x.ScanArrivalsUnsafe(f)
                | res -> res

        // Lock the arrivals queue while we scan that
        member x.ScanArrivals(f) = 
            lock syncRoot (fun () -> x.ScanArrivalsUnsafe(f))

        member x.ScanInbox(f,n) =
            match inboxStore with
            | null -> None
            | inbox ->
                if n >= inbox.Count
                then None
                else
                    let msg = inbox.[n]
                    match f msg with
                    | None -> x.ScanInbox (f,n+1)
                    | res -> inbox.RemoveAt(n); res

        member x.ReceiveFromArrivalsUnsafe() =
            if arrivals.Count = 0 then 
                None
            else 
                Some(arrivals.Dequeue())

        member x.ReceiveFromArrivals() = 
            lock syncRoot (fun () -> x.ReceiveFromArrivalsUnsafe())

        member x.ReceiveFromInbox() =
            match inboxStore with
            | null -> None
            | inbox ->
                if inbox.Count = 0 then 
                    None
                else
                    let x = inbox.[0]
                    inbox.RemoveAt(0)
                    Some(x)

        member x.Post(msg) =
            lock syncRoot (fun () ->

                // Add the message to the arrivals queue
                arrivals.Enqueue(msg)

                // Cooperatively unblock any waiting reader. If there is no waiting
                // reader we just leave the message in the incoming queue
                match savedCont with
                | None -> 
                    match pulse with 
                    | null -> 
                        () // no one waiting, leaving the message in the queue is sufficient
                    | ev -> 
                        // someone is waiting on the wait handle
                        ev.Set() |> ignore

                | Some (action, trampolineHolder) -> 
                    savedCont <- None
                    trampolineHolder.QueueWorkItemWithTrampoline(fun () -> action true) |> unfake)

        member x.TryScan ((f: 'Msg -> (Async<'T>) option), timeout) : Async<'T option> =
            let rec scan timeoutAsync (timeoutCts:CancellationTokenSource) =
                async { 
                    match x.ScanArrivals(f) with
                    | None -> 
                        // Deschedule and wait for a message. When it comes, rescan the arrivals
                        let! ok = AsyncHelpers.awaitEither waitOneNoTimeoutOrCancellation timeoutAsync
                        match ok with
                        | Choice1Of2 true -> 
                            return! scan timeoutAsync timeoutCts
                        | Choice1Of2 false ->
                            return failwith "should not happen - waitOneNoTimeoutOrCancellation always returns true"
                        | Choice2Of2 () ->
                            lock syncRoot (fun () -> 
                                // Cancel the outstanding wait for messages installed by waitOneWithCancellation
                                //
                                // HERE BE DRAGONS. This is bestowed on us because we only support
                                // a single mailbox reader at any one time.
                                // If awaitEither returned control because timeoutAsync has terminated, waitOneNoTimeoutOrCancellation
                                // might still be in-flight. In practical terms, it means that the push-to-async-result-cell 
                                // continuation that awaitEither registered on it is still pending, i.e. it is still in savedCont.
                                // That continuation is a no-op now, but it is still a registered reader for arriving messages.
                                // Therefore we just abandon it - a brutal way of canceling.
                                // This ugly non-compositionality is only needed because we only support a single mailbox reader
                                // (i.e. the user is not allowed to run several Receive/TryReceive/Scan/TryScan in parallel) - otherwise 
                                // we would just have an extra no-op reader in the queue.
                                savedCont <- None)

                            return None
                    | Some resP ->                     
                        timeoutCts.Cancel() // cancel the timeout watcher
                        let! res = resP
                        return Some res
                }
            let rec scanNoTimeout () =
                async { 
                    match x.ScanArrivals(f) with
                    | None -> 
                        let! ok = waitOne(Timeout.Infinite)
                        if ok then
                            return! scanNoTimeout()
                        else
                            return (failwith "Timed out with infinite timeout??")
                    | Some resP -> 
                        let! res = resP
                        return Some res
                }

            // Look in the inbox first
            async { 
                match x.ScanInbox(f,0) with
                | None  when timeout < 0 -> 
                    return! scanNoTimeout()
                | None -> 
                    let! cancellationToken = Async.CancellationToken
                    let timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, CancellationToken.None)
                    let timeoutAsync = AsyncHelpers.timeout timeout timeoutCts.Token
                    return! scan timeoutAsync timeoutCts
                | Some resP -> 
                    let! res = resP
                    return Some res
            }

        member x.Scan((f: 'Msg -> (Async<'T>) option), timeout) =
            async { 
                let! resOpt = x.TryScan(f,timeout)
                match resOpt with
                | None -> return raise(TimeoutException(SR.GetString(SR.mailboxScanTimedOut)))
                | Some res -> return res 
            }

        member x.TryReceive(timeout) =
            let rec processFirstArrival() =
                async { 
                    match x.ReceiveFromArrivals() with
                    | None -> 
                        // Make sure the pulse is created if it is going to be needed. 
                        // If it isn't, then create it, and go back to the start to 
                        // check arrivals again.
                        match pulse with
                        | null when timeout >= 0 || cancellationSupported ->
                            ensurePulse() |> ignore
                            return! processFirstArrival()
                        | _ -> 
                            // Wait until we have been notified about a message. When that happens, rescan the arrivals
                            let! ok = waitOne(timeout)
                            if ok then 
                                return! processFirstArrival()
                            else 
                                return None
                    | res -> return res 
                }

            // look in the inbox first
            async { 
                match x.ReceiveFromInbox() with
                | None -> return! processFirstArrival()
                | res -> return res 
            }

        member x.Receive(timeout) =

            let rec processFirstArrival() =
                async { 
                    match x.ReceiveFromArrivals() with
                    | None -> 
                        // Make sure the pulse is created if it is going to be needed. 
                        // If it isn't, then create it, and go back to the start to 
                        // check arrivals again.
                        match pulse with
                        | null when timeout >= 0 || cancellationSupported ->
                            ensurePulse() |> ignore
                            return! processFirstArrival()
                        | _ -> 
                            // Wait until we have been notified about a message. When that happens, rescan the arrivals
                            let! ok = waitOne(timeout)
                            if ok then 
                                return! processFirstArrival()
                            else 
                                return raise(TimeoutException(SR.GetString(SR.mailboxReceiveTimedOut)))
                    | Some res -> return res 
                }

            // look in the inbox first
            async { 
                match x.ReceiveFromInbox() with
                | None -> return! processFirstArrival() 
                | Some res -> return res 
            }

        interface System.IDisposable with
            member __.Dispose() =
                if isNotNull pulse then (pulse :> IDisposable).Dispose()

#if DEBUG
        member x.UnsafeContents =
            (x.inbox,arrivals,pulse,savedCont) |> box
#endif


    [<Sealed>]
    [<CompiledName("FSharpAsyncReplyChannel`1")>]
    type AsyncReplyChannel<'Reply>(replyf : 'Reply -> unit) =
        member x.Reply(value) = replyf(value)

    [<Sealed>]
    [<AutoSerializable(false)>]
    [<CompiledName("FSharpMailboxProcessor`1")>]
    type MailboxProcessor<'Msg>(body, ?cancellationToken) =

        let cancellationSupported = cancellationToken.IsSome
        let cancellationToken = defaultArg cancellationToken Async.DefaultCancellationToken
        let mailbox = new Mailbox<'Msg>(cancellationSupported)
        let mutable defaultTimeout = Threading.Timeout.Infinite
        let mutable started = false
        let errorEvent = new Event<Exception>()

        member __.CurrentQueueLength = mailbox.CurrentQueueLength // nb. unprotected access gives an approximation of the queue length

        member __.DefaultTimeout 
            with get() = defaultTimeout 
            and set(v) = defaultTimeout <- v

        [<CLIEvent>]
        member __.Error = errorEvent.Publish

#if DEBUG
        member __.UnsafeMessageQueueContents = mailbox.UnsafeContents
#endif

        member x.Start() =
            if started then
                raise (new InvalidOperationException(SR.GetString(SR.mailboxProcessorAlreadyStarted)))
            else
                started <- true

                // Protect the execution and send errors to the event.
                // Note that exception stack traces are lost in this design - in an extended design
                // the event could propagate an ExceptionDispatchInfo instead of an Exception.
                let p = 
                    async { try 
                                do! body x 
                            with exn -> 
                                errorEvent.Trigger exn }

                Async.Start(computation=p, cancellationToken=cancellationToken)

        member __.Post(message) = mailbox.Post(message)

        member __.TryPostAndReply(buildMessage : (_ -> 'Msg), ?timeout) : 'Reply option = 
            let timeout = defaultArg timeout defaultTimeout
            use resultCell = new ResultCell<_>()
            let msg = buildMessage (new AsyncReplyChannel<_>(fun reply ->
                                    // Note the ResultCell may have been disposed if the operation
                                    // timed out. In this case RegisterResult drops the result on the floor.                                                                        
                                    resultCell.RegisterResult(reply,reuseThread=false) |> unfake))
            mailbox.Post(msg)
            resultCell.TryWaitForResultSynchronously(timeout=timeout) 

        member x.PostAndReply(buildMessage, ?timeout) : 'Reply = 
            match x.TryPostAndReply(buildMessage,?timeout=timeout) with
            | None ->  raise (TimeoutException(SR.GetString(SR.mailboxProcessorPostAndReplyTimedOut)))
            | Some res -> res

        member __.PostAndTryAsyncReply(buildMessage, ?timeout) : Async<'Reply option> = 
            let timeout = defaultArg timeout defaultTimeout
            let resultCell = new ResultCell<_>()
            let msg = buildMessage (new AsyncReplyChannel<_>(fun reply ->
                                    // Note the ResultCell may have been disposed if the operation
                                    // timed out. In this case RegisterResult drops the result on the floor.
                                    resultCell.RegisterResult(reply, reuseThread=false) |> unfake))
            mailbox.Post(msg)
            match timeout with
            | Threading.Timeout.Infinite when not cancellationSupported -> 
                async { let! result = resultCell.AwaitResult_NoDirectCancelOrTimeout
                        return Some result }  
                        
            | _ ->
                async { use _disposeCell = resultCell
                        let! ok =  Async.AwaitWaitHandle(resultCell.GetWaitHandle(), millisecondsTimeout=timeout)
                        let res = (if ok then Some(resultCell.GrabResult()) else None)
                        return res }
                    
        member x.PostAndAsyncReply(buildMessage, ?timeout:int) =                 
            let timeout = defaultArg timeout defaultTimeout
            match timeout with
            | Threading.Timeout.Infinite when not cancellationSupported -> 
                // Nothing to dispose, no wait handles used
                let resultCell = new ResultCell<_>()
                let msg = buildMessage (new AsyncReplyChannel<_>(fun reply -> resultCell.RegisterResult(reply,reuseThread=false) |> unfake))
                mailbox.Post(msg)
                resultCell.AwaitResult_NoDirectCancelOrTimeout
            | _ ->            
                let asyncReply = x.PostAndTryAsyncReply(buildMessage,timeout=timeout) 
                async { let! res = asyncReply
                        match res with 
                        | None ->  return! raise (TimeoutException(SR.GetString(SR.mailboxProcessorPostAndAsyncReplyTimedOut)))
                        | Some res -> return res }
                           
        member __.Receive(?timeout) = 
            mailbox.Receive(timeout=defaultArg timeout defaultTimeout)

        member __.TryReceive(?timeout) = 
            mailbox.TryReceive(timeout=defaultArg timeout defaultTimeout)

        member __.Scan(scanner: 'Msg -> (Async<'T>) option,?timeout) = 
            mailbox.Scan(scanner,timeout=defaultArg timeout defaultTimeout)

        member __.TryScan(scanner: 'Msg -> (Async<'T>) option,?timeout) = 
            mailbox.TryScan(scanner,timeout=defaultArg timeout defaultTimeout)

        interface System.IDisposable with
            member __.Dispose() = (mailbox :> IDisposable).Dispose()

        static member Start(body,?cancellationToken) = 
            let mailboxProcessor = new MailboxProcessor<'Msg>(body,?cancellationToken=cancellationToken)
            mailboxProcessor.Start()
            mailboxProcessor
 
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module Event =
        [<CompiledName("Create")>]
        let create<'T>() = 
            let ev = new Event<'T>() 
            ev.Trigger, ev.Publish

        [<CompiledName("Map")>]
        let map mapping (sourceEvent: IEvent<'Delegate,'T>) =
            let ev = new Event<_>() 
            sourceEvent.Add(fun x -> ev.Trigger(mapping x));
            ev.Publish

        [<CompiledName("Filter")>]
        let filter predicate (sourceEvent: IEvent<'Delegate,'T>) =
            let ev = new Event<_>() 
            sourceEvent.Add(fun x -> if predicate x then ev.Trigger x);
            ev.Publish

        [<CompiledName("Partition")>]
        let partition predicate (sourceEvent: IEvent<'Delegate,'T>) =
            let ev1 = new Event<_>() 
            let ev2 = new Event<_>() 
            sourceEvent.Add(fun x -> if predicate x then ev1.Trigger x else ev2.Trigger x);
            ev1.Publish,ev2.Publish

        [<CompiledName("Choose")>]
        let choose chooser (sourceEvent: IEvent<'Delegate,'T>) =
            let ev = new Event<_>() 
            sourceEvent.Add(fun x -> match chooser x with None -> () | Some r -> ev.Trigger r);
            ev.Publish

        [<CompiledName("Scan")>]
        let scan collector state (sourceEvent: IEvent<'Delegate,'T>) =
            let state = ref state
            let ev = new Event<_>() 
            sourceEvent.Add(fun msg ->
                 let z = !state
                 let z = collector z msg
                 state := z; 
                 ev.Trigger(z));
            ev.Publish

        [<CompiledName("Add")>]
        let add callback (sourceEvent: IEvent<'Delegate,'T>) = sourceEvent.Add(callback)

        [<CompiledName("Pairwise")>]
        let pairwise (sourceEvent : IEvent<'Delegate,'T>) : IEvent<'T * 'T> = 
            let ev = new Event<'T * 'T>() 
            let lastArgs = ref None
            sourceEvent.Add(fun args2 -> 
                (match !lastArgs with 
                 | None -> () 
                 | Some args1 -> ev.Trigger(args1,args2))
                lastArgs := Some args2)

            ev.Publish

        [<CompiledName("Merge")>]
        let merge (event1: IEvent<'Del1,'T>) (event2: IEvent<'Del2,'T>) =
            let ev = new Event<_>() 
            event1.Add(fun x -> ev.Trigger(x))
            event2.Add(fun x -> ev.Trigger(x))
            ev.Publish

        [<CompiledName("Split")>]
        let split (splitter : 'T -> Choice<'U1,'U2>) (sourceEvent: IEvent<'Delegate,'T>) =
            let ev1 = new Event<_>() 
            let ev2 = new Event<_>() 
            sourceEvent.Add(fun x -> match splitter x with Choice1Of2 y -> ev1.Trigger(y) | Choice2Of2 z -> ev2.Trigger(z));
            ev1.Publish,ev2.Publish


    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module Observable =
        let obs x =  (x :> IObservable<_>)


        let inline protect f succeed fail =
          match (try Choice1Of2 (f ()) with e -> Choice2Of2 e) with
            | Choice1Of2 x -> (succeed x)
            | Choice2Of2 e -> (fail e)

        [<AbstractClass>]
        type BasicObserver<'T>() =
          let mutable stopped = false
          abstract Next : value : 'T -> unit
          abstract Error : error : exn -> unit
          abstract Completed : unit -> unit
          interface IObserver<'T> with
              member x.OnNext value = if not stopped then x.Next value
              member x.OnError e = if not stopped then stopped <- true
                                                       x.Error e
              member x.OnCompleted () = if not stopped then stopped <- true
                                                            x.Completed ()

        [<CompiledName("Map")>]
        let map mapping (source: IObservable<'T>) =
            { new IObservable<'U> with 
                 member x.Subscribe(observer) =
                     source.Subscribe { new BasicObserver<'T>() with  
                                        member x.Next(v) = 
                                            protect (fun () -> mapping v) observer.OnNext observer.OnError
                                        member x.Error(e) = observer.OnError(e)
                                        member x.Completed() = observer.OnCompleted() } }

        [<CompiledName("Choose")>]
        let choose chooser (source: IObservable<'T>) =
            { new IObservable<'U> with 
                 member x.Subscribe(observer) =
                     source.Subscribe { new BasicObserver<'T>() with  
                                        member x.Next(v) = 
                                            protect (fun () -> chooser v) (function None -> () | Some v2 -> observer.OnNext v2) observer.OnError
                                        member x.Error(e) = observer.OnError(e)
                                        member x.Completed() = observer.OnCompleted() } }

        [<CompiledName("Filter")>]
        let filter predicate (source: IObservable<'T>) =
            choose (fun x -> if predicate x then Some x else None) source

        [<CompiledName("Partition")>]
        let partition predicate (source: IObservable<'T>) =
            filter predicate source, filter (predicate >> not) source


        [<CompiledName("Scan")>]
        let scan collector state (source: IObservable<'T>) =
            { new IObservable<'U> with 
                 member x.Subscribe(observer) =
                     let state = ref state
                     source.Subscribe { new BasicObserver<'T>() with  
                                        member x.Next(v) = 
                                            let z = !state
                                            protect (fun () -> collector z v) (fun z -> 
                                                state := z
                                                observer.OnNext z) observer.OnError
                                            
                                        member x.Error(e) = observer.OnError(e)
                                        member x.Completed() = observer.OnCompleted() } }

        [<CompiledName("Add")>]
        let add callback (source: IObservable<'T>) = source.Add(callback)

        [<CompiledName("Subscribe")>]
        let subscribe (callback: 'T -> unit) (source: IObservable<'T>) = source.Subscribe(callback)

        [<CompiledName("Pairwise")>]
        let pairwise (source : IObservable<'T>) : IObservable<'T * 'T> = 
            { new IObservable<_> with 
                 member x.Subscribe(observer) =
                     let lastArgs = ref None
                     source.Subscribe { new BasicObserver<'T>() with  
                                        member x.Next(args2) = 
                                            match !lastArgs with 
                                            | None -> ()
                                            | Some args1 -> observer.OnNext (args1,args2)
                                            lastArgs := Some args2
                                        member x.Error(e) = observer.OnError(e)
                                        member x.Completed() = observer.OnCompleted() } }


        [<CompiledName("Merge")>]
        let merge (source1: IObservable<'T>) (source2: IObservable<'T>) =
            { new IObservable<_> with 
                 member x.Subscribe(observer) =
                     let stopped = ref false
                     let completed1 = ref false
                     let completed2 = ref false
                     let h1 = 
                         source1.Subscribe { new IObserver<'T> with  
                                            member x.OnNext(v) = 
                                                    if not !stopped then 
                                                        observer.OnNext v
                                            member x.OnError(e) = 
                                                    if not !stopped then 
                                                        stopped := true; 
                                                        observer.OnError(e)
                                            member x.OnCompleted() = 
                                                    if not !stopped then 
                                                        completed1 := true; 
                                                        if !completed1 && !completed2 then 
                                                            stopped := true
                                                            observer.OnCompleted() } 
                     let h2 = 
                         source2.Subscribe { new IObserver<'T> with  
                                            member x.OnNext(v) = 
                                                    if not !stopped then 
                                                        observer.OnNext v
                                            member x.OnError(e) = 
                                                    if not !stopped then 
                                                        stopped := true; 
                                                        observer.OnError(e)
                                            member x.OnCompleted() = 
                                                    if not !stopped then 
                                                        completed2 := true; 
                                                        if !completed1 && !completed2 then 
                                                            stopped := true
                                                            observer.OnCompleted() } 

                     { new IDisposable with 
                           member x.Dispose() = 
                               h1.Dispose(); 
                               h2.Dispose() } }

        [<CompiledName("Split")>]
        let split (splitter : 'T -> Choice<'U1,'U2>) (source: IObservable<'T>) =
            choose (fun v -> match splitter v with Choice1Of2 x -> Some x | _ -> None) source,
            choose (fun v -> match splitter v with Choice2Of2 x -> Some x | _ -> None) source

