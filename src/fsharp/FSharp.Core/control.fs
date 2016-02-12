// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#if FX_NO_CANCELLATIONTOKEN_CLASSES
namespace System
    open System
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections

    type [<Class>] AggregateException (exns : seq<exn>) =
        inherit Exception()
        let exnsList = new System.Collections.Generic.List<exn>(exns)        
        member this.InnerExceptions = new System.Collections.ObjectModel.ReadOnlyCollection<exn>(exnsList :> System.Collections.Generic.IList<exn>)

namespace System.Threading
    #nowarn "864"   // this is for typed Equals() in CancellationTokenRegistration and CancellationToken

    open System
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections
    
    
    module internal CancellationState =
        [<Literal>]
        let ACTIVE = 0
        [<Literal>]
        let DISPOSED_ACTIVE = 1
        [<Literal>]
        let CANCELED = 2
        [<Literal>]
        let DISPOSED_CANCELED = 3

    [<Struct>]
    [<CustomEquality; NoComparison>]
    type CancellationTokenRegistration =     
            val private source : CancellationTokenSource             
            val private id : int64        
            
            internal new(source,id) = { source = source; id = id }
            
            member this.Dispose() = 
                match this.source with
                |   null -> ()
                |   _ -> this.source.Deregister(this.id)
                    
            member this.Equals(ctr:CancellationTokenRegistration) =
                match this.source with
                |   null -> ctr.source = null
                |   _ ->  this.source.Equals(ctr.source) && this.id = ctr.id
                
            override this.Equals(o:obj) =
                match o with
                | :? CancellationTokenRegistration as ctr -> this.Equals(ctr)
                | _ -> false
            
            override this.GetHashCode() =
                match this.source with
                | null -> 0
                | _ ->  this.source.GetHashCode()^^^this.id.GetHashCode()
                
            static member (=) (left:CancellationTokenRegistration,right:CancellationTokenRegistration) = left.Equals(right)
            static member (<>) (left:CancellationTokenRegistration,right:CancellationTokenRegistration) = not (left.Equals(right))
            
            interface System.IDisposable with
                member this.Dispose() = this.Dispose()                   
                        
    and [<Struct>]
        [<CustomEquality; NoComparison>]
        CancellationToken =
        
            val private source : CancellationTokenSource 
            
            internal new (source) = { source = source }
            
            member this.IsCancellationRequested =  
                match this.source with
                | null -> false
                | source -> source.IsCancellationRequested
                
            member this.CanBeCanceled = this.source <> Unchecked.defaultof<_>
            
            member this.Register (action:Action<obj>, state:obj) = 
                match this.source with
                |   null -> Unchecked.defaultof<_>
                |   source -> source.Register(action, state)
                    
            member this.Equals(ct:CancellationToken) =
                match this.source with
                |   null -> ct.source = null
                |   _ -> this.source.Equals(ct.source)
                
            override this.Equals(o:obj) =
                match o with
                | :? CancellationToken as ct -> this.Equals(ct)
                | _ -> false
            
            override this.GetHashCode() =                
                match this.source with
                |   null -> 0
                |   _ -> this.source.GetHashCode()
                
            static member (=) (left:CancellationToken,right:CancellationToken) = left.Equals(right)
            static member (<>) (left:CancellationToken,right:CancellationToken) = not (left.Equals(right))
                    
            static member None = new CancellationToken(null)
                
    and [<Struct>] 
        [<NoEquality; NoComparison>]
        internal CallbackInfo =        
        val private id : int64        
        val private action : Action<obj> 
        val private state : obj
        
        new (id,action,state) = { id = id; action = action; state = state }
        
        member this.ID = this.id
        member this.Action = this.action
        member this.State = this.state
            
    and [<Class>][<Sealed>][<AllowNullLiteral>]
        CancellationTokenSource private (token1 : CancellationToken, token2 : CancellationToken) as this =
                        
            [<VolatileField>]
            let mutable state = CancellationState.ACTIVE
            
            // next registration id 
            let mutable nextID = 0L;
            // lazily initialized list of registrations
            let registrations = lazy (new System.Collections.Generic.List<CallbackInfo>())                
            
            // linking to tokens
            
            let mutable linkedCtr1 = Unchecked.defaultof<CancellationTokenRegistration>
            let mutable linkedCtr2 = Unchecked.defaultof<CancellationTokenRegistration>
            do 
                let handler  = Action<obj>(fun _ -> 
                        // Avoinding a race for Dispose versus Cancel for linked token sources:
                        //  - CTS.Dispose deregisters its CTRs and sets state to DISPOSED_*
                        //  - However if the cancellation is in progress in the source it is linked to, deregistration is a no-op and CTS may still receive cancellation notification
                        //  - That cancellation notification arrives in disposed state
                        // We ignore cancellation notifications from linked sources in disposed state (so if cancellation/disposal race happens, disposal wins). 
                        this.Cancel(dontThrowIfDisposed = true)
                    )
                linkedCtr1 <- token1.Register(handler,null)            
                linkedCtr2 <- token2.Register(handler,null)                
                            
            public new() = new CancellationTokenSource(Unchecked.defaultof<_>,Unchecked.defaultof<_>)
            
            member this.Token = new CancellationToken(this)
            
            member this.Cancel() = this.Cancel(dontThrowIfDisposed = false)
            member private this.Cancel (dontThrowIfDisposed) : unit =
                let oldState = Interlocked.CompareExchange(&state, CancellationState.CANCELED, CancellationState.ACTIVE)
                match oldState with
                | CancellationState.ACTIVE ->
                    if registrations.IsValueCreated then // we have at least one registration
                        let list = registrations.Value
                        let toRun =
                            // building a list of callback to run, in LIFO order
                            lock list (fun () ->
                                let toRun = list |> Seq.fold (fun l info -> (fun () -> info.Action.Invoke(info.State))::l) []                                                                
                                list.Clear()
                                toRun)                            
                        let doRun l f = // run callback, add any thrown exception to the list
                            try  f(); l 
                            with e -> e::l 
                        let exns = List.fold doRun [] toRun
                        match exns with 
                        |   [] -> ()
                        |   _ -> 
                            // exns are in reverse order to the callbacks in toRun
                            // we rev here; mainline case (no exceptions at all) runs without any allocations for exception list
                            new AggregateException(exns |> List.rev) |> raise
                    else () // no registrations - do nothing
                | CancellationState.CANCELED ->
                    () // cancellation already happened
                | _ -> 
                    // DISPOSED_ACTIVE or DISPOSED_CANCELED
                    if not dontThrowIfDisposed then
                        new ObjectDisposedException(typeof<CancellationTokenSource>.FullName) |> raise
                    else ()
                
            member this.Dispose() = 
                try
                    // Unregister from linked sources before changing state. Otherwise callback may still execute and we will be canceled in disposed state
                    // Multiple CTR disposal is a no-op
                    try
                        linkedCtr2.Dispose()
                    finally
                        linkedCtr1.Dispose()
                finally
                    let disposeNow =
                        let oldState = Interlocked.CompareExchange(&state, CancellationState.DISPOSED_ACTIVE, CancellationState.ACTIVE)
                        if oldState = CancellationState.ACTIVE then 
                            true // previous state was ACTIVE, now disposing
                        else 
                            let oldState = Interlocked.CompareExchange(&state, CancellationState.DISPOSED_CANCELED, CancellationState.CANCELED)
                            // if previous state was CANCELED, dispose now. Otherwise previous state was one of DISPOSED_* states, so already disposed
                            oldState = CancellationState.CANCELED 
                    if disposeNow then
                        if registrations.IsValueCreated then
                            let list = registrations.Value
                            lock list (fun () -> list.Clear())
            
            member private this.InternalIsCanceled throwOnDisposed = 
                match state with
                |   CancellationState.ACTIVE -> false
                |   CancellationState.CANCELED -> true
                |   CancellationState.DISPOSED_CANCELED -> 
                        if throwOnDisposed then
                            new ObjectDisposedException(typeof<CancellationTokenSource>.FullName) |> raise
                        else
                            true
                |   _ -> 
                        if throwOnDisposed then
                            new ObjectDisposedException(typeof<CancellationTokenSource>.FullName) |> raise
                        else
                            false

            
            member internal this.IsCancellationRequested = state = CancellationState.CANCELED || state = CancellationState.DISPOSED_CANCELED
            
            member internal this.Register(action:Action<obj>, state:obj) =
                if this.InternalIsCanceled true then // do not register, invoke immediately
                    action.Invoke(state)
                    Unchecked.defaultof<_>
                else
                    let list = registrations.Value
                    let invokeNow, r =
                        lock list (fun () ->
                            if this.InternalIsCanceled true then
                                true, new CancellationTokenRegistration(Unchecked.defaultof<_>, 0L)
                            else
                                let id = nextID
                                nextID <- nextID + 1L
                                list.Add(new CallbackInfo(id, action, state))
                                false, new CancellationTokenRegistration(this, id)
                        )           
                    if invokeNow then action.Invoke(state)
                    r
            
            member internal this.Deregister(id) =
                if this.InternalIsCanceled false then // ok to deregister after Dispose
                    () // After cancellation is requested no deregistration needed; 
                else 
                    let list = registrations.Value
                    lock list (fun () ->
                        if this.InternalIsCanceled false then // ok to deregister after Dispose
                            () 
                        else
                            let index = 
                                // Search backwards; we assume Register/Deregister are scoped 
                                // so registered last will be deregistred first
                                let rec loop i = 
                                    if i < 0 then (-1)
                                    else
                                        let callbackInfo = list.[i] 
                                        if callbackInfo.ID = id then i
                                        else loop (i-1)
                                loop (list.Count - 1)
                            if index >= 0 then
                                list.RemoveAt(index)
                            else
                                () // we do not punish double deregistering
                    )
                 
            
            interface System.IDisposable with
                member this.Dispose() = this.Dispose()
            static member CreateLinkedTokenSource (token1:CancellationToken,token2:CancellationToken) =
                new CancellationTokenSource(token1,token2)
#endif

namespace Microsoft.FSharp.Control

    #nowarn "40"
    #nowarn "21"
    #nowarn "47"
    #nowarn "44" // This construct is deprecated. 
    #nowarn "52" // The value has been copied to ensure the original is not mutated by this operation
    #nowarn "67" // This type test or downcast will always hold
    #nowarn "864" // IObservable.Subscribe
 
    open System
    open System.Diagnostics
    open System.Diagnostics.CodeAnalysis
    open System.Threading
    open System.IO
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections

#if FX_RESHAPED_REFLECTION
    open ReflectionAdapters
    type BindingFlags = ReflectionAdapters.BindingFlags
#else
    type BindingFlags = System.Reflection.BindingFlags
#endif

#if FX_NO_TASK
#else
    open System.Threading
    open System.Threading.Tasks
#endif

#if FX_NO_OPERATION_CANCELLED
    type OperationCanceledException(s : System.String) =
        inherit System.Exception(s)
        new () = new OperationCanceledException("The operation has been canceled")
#endif

    
#if FX_NO_EXCEPTIONDISPATCHINFO
    [<AllowNullLiteral>]
    type ExceptionDispatchInfo(exn: System.Exception) =
        static member Capture(exn) = ExceptionDispatchInfo(exn)
        member __.Throw() = raise exn; ()
        member __.SourceException = exn
#else
    open System.Runtime.ExceptionServices
#endif


    /// We use our own internal implementation of queues to avoid a dependency on System.dll
    type Queue<'T>() =  //: IEnumerable<T>, ICollection, IEnumerable
    
        let mutable array = [| |]
        let mutable head = 0
        let mutable size = 0
        let mutable tail = 0

        let SetCapacity(capacity) =
            let destinationArray = Array.zeroCreate capacity;
            if (size > 0) then 
                if (head < tail) then 
                    System.Array.Copy(array, head, destinationArray, 0, size);
        
                else
                    System.Array.Copy(array, head, destinationArray, 0, array.Length - head);
                    System.Array.Copy(array, 0, destinationArray, array.Length - head, tail);
            array <- destinationArray;
            head <- 0;
            tail <- if (size = capacity) then 0 else size;

        member x.Dequeue() =
            if (size = 0) then
                failwith "Dequeue"
            let local = array.[head];
            array.[head] <- Unchecked.defaultof<'T>
            head <- (head + 1) % array.Length;
            size <- size - 1;
            local

        member this.Enqueue(item) =
            if (size = array.Length) then 
                let capacity = int ((int64 array.Length * 200L) / 100L);
                let capacity = max capacity (array.Length + 4)
                SetCapacity(capacity);
            array.[tail] <- item;
            tail <- (tail + 1) % array.Length;
            size <- size + 1

        member x.Count = size

    type LinkedSubSource(ct : CancellationToken) =
        
        let failureCTS = new CancellationTokenSource()
        let linkedCTS = CancellationTokenSource.CreateLinkedTokenSource(ct, failureCTS.Token)
        
        member this.Token = linkedCTS.Token
        member this.Cancel() = failureCTS.Cancel()
        member this.Dispose() = 
            linkedCTS.Dispose()
            failureCTS.Dispose()
        
        interface IDisposable with
            member this.Dispose() = this.Dispose()

    
    // F# don't always take tailcalls to functions returning 'unit' because this
    // is represented as type 'void' in the underlying IL.
    // Hence we don't use the 'unit' return type here, and instead invent our own type.
    [<NoEquality; NoComparison>]
    type FakeUnitValue =
        | FakeUnit


    type cont<'T> = ('T -> FakeUnitValue)
    type econt = (ExceptionDispatchInfo -> FakeUnitValue)
    type ccont = (OperationCanceledException -> FakeUnitValue)



    //----------------------------------
    // PRIMITIVE ASYNC TRAMPOLINE

    [<AllowNullLiteral>]
    type Trampoline() = 

        [<Literal>]
        static let bindLimitBeforeHijack = 300 
#if FX_NO_THREAD_STATIC
#else
        [<ThreadStatic>]
        [<DefaultValue>]
        static val mutable private thisThreadHasTrampoline : bool
#endif

        static member ThisThreadHasTrampoline = 
#if FX_NO_THREAD_STATIC
            true
#else
            Trampoline.thisThreadHasTrampoline
#endif
        
        let mutable cont = None
        let mutable bindCount = 0
        
        static let unfake FakeUnit = ()

        // Install a trampolineStack if none exists
        member this.ExecuteAction (firstAction : unit -> FakeUnitValue) =
            let rec loop action = 
                action() |> unfake
                match cont with
                | None -> ()
                | Some newAction -> 
                    cont <- None
                    loop newAction
#if FX_NO_THREAD_STATIC
#else
            let thisIsTopTrampoline =
                if Trampoline.thisThreadHasTrampoline then
                    false
                else
                    Trampoline.thisThreadHasTrampoline <- true
                    true
#endif
            try
                loop firstAction
            finally
#if FX_NO_THREAD_STATIC
                ()
#else
                if thisIsTopTrampoline then
                    Trampoline.thisThreadHasTrampoline <- false
#endif       
            FakeUnit
            
        // returns true if time to jump on trampoline
        member this.IncrementBindCount() =
            bindCount <- bindCount + 1
            bindCount >= bindLimitBeforeHijack
            
        member this.Set action = 
            match cont with
            |   None -> 
                    bindCount <- 0
                    cont <- Some action
            |   _ -> failwith "Internal error: attempting to install continuation twice"


#if FSHARP_CORE_NETCORE_PORTABLE
    // Imitation of desktop functionality for .NETCore
    // 1. QueueUserWorkItem reimplemented as Task.Run
    // 2. Thread.CurrentThread type in the code is typically used to check if continuation is called on the same thread that initiated the async computation
    // if this condition holds we may decide to invoke continuation directly rather than queueing it.
    // Thread type here is barely a wrapper over CurrentManagedThreadId value - it should be enough to uniquely identify the actual thread

    [<NoComparison; NoEquality>]
    type internal WaitCallback = WaitCallback of (obj -> unit)

    type ThreadPool =
        static member QueueUserWorkItem(WaitCallback(cb), state : obj) = 
            System.Threading.Tasks.Task.Run (fun () -> cb(state)) |> ignore
            true

    [<AllowNullLiteral>]
    type Thread(threadId : int) = 
        static member CurrentThread = Thread(Environment.CurrentManagedThreadId)
        member this.ThreadId = threadId
        override this.GetHashCode() = threadId
        override this.Equals(other : obj) = 
            match other with
            | :? Thread as other -> threadId = other.ThreadId
            | _ -> false

#endif

    type TrampolineHolder() as this =
        let mutable trampoline = null
        
        static let unfake FakeUnit = ()
        // preallocate context-switching callbacks
#if FX_NO_SYNC_CONTEXT
#else
        // Preallocate the delegate
        // This should be the only call to SynchronizationContext.Post in this library. We must always install a trampoline.        
        let sendOrPostCallback = 
                SendOrPostCallback(fun o ->
                    let f = unbox o : unit -> FakeUnitValue
                    this.Protect f |> unfake
                    )
#endif

        // Preallocate the delegate
        // This should be the only call to QueueUserWorkItem in this library. We must always install a trampoline.
        let waitCallbackForQueueWorkItemWithTrampoline = 
                WaitCallback(fun o ->
                    let f = unbox o : unit -> FakeUnitValue
                    this.Protect f |> unfake
                    )

#if FX_NO_PARAMETERIZED_THREAD_START
#else        
        // This should be the only call to Thread.Start in this library. We must always install a trampoline.
        let threadStartCallbackForStartThreadWithTrampoline = 
                ParameterizedThreadStart(fun o ->
                    let f = unbox o : unit -> FakeUnitValue
                    this.Protect f |> unfake
                    )
#endif

#if FX_NO_SYNC_CONTEXT
#else                    
        member this.Post (ctxt: SynchronizationContext)  (f : unit -> FakeUnitValue) =
            ctxt.Post (sendOrPostCallback, state=(f |> box))
            FakeUnit
#endif

        member this.QueueWorkItem (f: unit -> FakeUnitValue) =            
                if not (ThreadPool.QueueUserWorkItem(waitCallbackForQueueWorkItemWithTrampoline, f |> box)) then
                    failwith "failed to queue user work item"
                FakeUnit
        
#if FX_NO_PARAMETERIZED_THREAD_START
        // This should be the only call to Thread.Start in this library. We must always install a trampoline.
        member this.StartThread (f : unit -> FakeUnitValue) =
#if FX_NO_THREAD
            this.QueueWorkItem(f)
#else
            (new Thread((fun _ -> this.Protect f |> unfake), IsBackground=true)).Start()
            FakeUnit
#endif

#else
        // This should be the only call to Thread.Start in this library. We must always install a trampoline.
        member this.StartThread (f : unit -> FakeUnitValue) =
            (new Thread(threadStartCallbackForStartThreadWithTrampoline,IsBackground=true)).Start(f|>box)
            FakeUnit
#endif
        
        member this.Protect firstAction =
            trampoline <- new Trampoline()
            trampoline.ExecuteAction(firstAction)
            
        member this.Trampoline = trampoline
        
    [<NoEquality; NoComparison>]
    [<AutoSerializable(false)>]
    type AsyncParamsAux =
        { token : CancellationToken;
          econt : econt;
          ccont : ccont;
          trampolineHolder : TrampolineHolder
        }
    
    [<NoEquality; NoComparison>]
    [<AutoSerializable(false)>]
    type AsyncParams<'T> =
        { cont : cont<'T>
          aux : AsyncParamsAux
        }
        
    [<NoEquality; NoComparison>]
    [<CompiledName("FSharpAsync`1")>]
    type Async<'T> =
        P of (AsyncParams<'T> -> FakeUnitValue)

    module AsyncBuilderImpl =
        // To consider: augment with more exception traceability information
        // To consider: add the ability to suspend running ps in debug mode
        // To consider: add the ability to trace running ps in debug mode
        open System
        open System.Threading
        open System.IO
        open Microsoft.FSharp.Core

        let fake () = FakeUnit
        let unfake FakeUnit = ()
        let ignoreFake _ = FakeUnit


        let mutable defaultCancellationTokenSource = new CancellationTokenSource()

        [<NoEquality; NoComparison>]
        type AsyncImplResult<'T>  =
        |   Ok of 'T
        |   Error of ExceptionDispatchInfo
        |   Canceled of OperationCanceledException

        let inline hijack (trampolineHolder:TrampolineHolder) res (cont : 'T -> FakeUnitValue) : FakeUnitValue =
            if trampolineHolder.Trampoline.IncrementBindCount() then
                trampolineHolder.Trampoline.Set(fun () -> cont res)
                FakeUnit
            else
                // NOTE: this must be a tailcall
                cont res

#if FX_NO_CONDITIONAL_WEAK_TABLE
#else
        /// Global mutable state used to associate Exception
        let associationTable = System.Runtime.CompilerServices.ConditionalWeakTable<exn, ExceptionDispatchInfo>()
#endif

        type ExceptionDispatchInfo with 

            member edi.GetAssociatedSourceException() = 
               let exn = edi.SourceException
#if FX_NO_CONDITIONAL_WEAK_TABLE
#else
               // Try to store the entry in the association table to allow us to recover it later.
               try lock associationTable (fun () -> associationTable.Add(exn, edi)) with _ -> ()
#endif
               exn

            // Capture, but prefer the saved information if available
            static member inline RestoreOrCapture(exn) = 
#if FX_NO_CONDITIONAL_WEAK_TABLE
#else
                match lock associationTable (fun () -> associationTable.TryGetValue(exn)) with 
                | true, edi -> edi
                | _ ->
#endif
                    ExceptionDispatchInfo.Capture(exn)

            member inline edi.ThrowAny() = 
                edi.Throw()
                Unchecked.defaultof<'T> // Note, this line should not be reached, but gives a generic return type

        // Apply f to x and call either the continuation or exception continuation depending what happens
        let inline protect (trampolineHolder:TrampolineHolder) econt f x (cont : 'T -> FakeUnitValue) : FakeUnitValue =
            // This is deliberately written in a allocation-free style, except when the trampoline is taken
            let mutable res = Unchecked.defaultof<_>
            let mutable edi = null

            try 
                res <- f x
            with exn -> 
                edi <- ExceptionDispatchInfo.RestoreOrCapture(exn)

            match edi with 
            | null -> 
                // NOTE: this must be a tailcall
                hijack trampolineHolder res cont
            | _ -> 
                // NOTE: this must be a tailcall
                hijack trampolineHolder edi econt

        // Apply f to x and call either the continuation or exception continuation depending what happens
        let inline protectNoHijack econt f x (cont : 'T -> FakeUnitValue) : FakeUnitValue =
            // This is deliberately written in a allocation-free style
            let mutable res = Unchecked.defaultof<_>
            let mutable edi = null

            try 
                res <- f x
            with exn -> 
                edi <- ExceptionDispatchInfo.RestoreOrCapture(exn)

            match edi with 
            | null -> 
                // NOTE: this must be a tailcall
                cont res
            | exn -> 
                // NOTE: this must be a tailcall
                econt exn



        // Reify exceptional results as exceptions
        let commit res =
            match res with
            | Ok res -> res
            | Error edi -> edi.ThrowAny()
            | Canceled exn -> raise exn

        // Reify exceptional results as exceptionsJIT 64 doesn't always take tailcalls correctly
        
        let commitWithPossibleTimeout res =
            match res with
            | None -> raise (System.TimeoutException())
            | Some res -> commit res


        //----------------------------------
        // PRIMITIVE ASYNC INVOCATION
        
        // Apply the underlying implementation of an async computation to its inputs
        let inline invokeA (P pf) args  = pf args


        let startA cancellationToken trampolineHolder cont econt ccont p =
            let args =
                    {   cont = cont 
                        aux = {  token = cancellationToken; 
                                 econt = econt 
                                 ccont = ccont 
                                 trampolineHolder = trampolineHolder
                              }
                    }
            invokeA p args 

                    
#if FX_NO_PARAMETERIZED_THREAD_START
        // Preallocate the delegate
        // This should be the only call to QueueUserWorkItem in this library. We must always install a trampoline.
        let waitCallbackForQueueWorkItemWithTrampoline(trampolineHolder : TrampolineHolder) = 
                WaitCallback(fun o ->
                    let f = unbox o : unit -> FakeUnitValue
                    trampolineHolder.Protect f |> unfake
                    )
                    
        let startThreadWithTrampoline (trampolineHolder:TrampolineHolder) (f : unit -> FakeUnitValue) =
#if FX_NO_THREADPOOL
                if not (ThreadPool.QueueUserWorkItem((waitCallbackForQueueWorkItemWithTrampoline trampolineHolder), f |> box)) then
                    failwith "failed to queue user work item"
                FakeUnit
#else        
            (new Thread((fun _ -> trampolineHolder.Protect f |> unfake), IsBackground=true)).Start()
            FakeUnit
#endif            

#else

        // Statically preallocate the delegate
        let threadStartCallbackForStartThreadWithTrampoline = 
                ParameterizedThreadStart(fun o ->
                    let (trampolineHolder,f) = unbox o : TrampolineHolder * (unit -> FakeUnitValue)
                    trampolineHolder.Protect f |> unfake
                    )

        // This should be the only call to Thread.Start in this library. We must always install a trampoline.
        let startThreadWithTrampoline (trampolineHolder:TrampolineHolder) (f : unit -> FakeUnitValue) =
            (new Thread(threadStartCallbackForStartThreadWithTrampoline,IsBackground=true)).Start((trampolineHolder,f)|>box)
            FakeUnit
#endif


        let startAsync cancellationToken cont econt ccont p =
            let trampolineHolder = new TrampolineHolder()
            trampolineHolder.Protect (fun () -> startA cancellationToken trampolineHolder cont econt ccont p)

        let queueAsync cancellationToken cont econt ccont p =
            let trampolineHolder = new TrampolineHolder()
            trampolineHolder.QueueWorkItem(fun () -> startA cancellationToken trampolineHolder cont econt ccont p)


        //----------------------------------
        // PRIMITIVE ASYNC CONSTRUCTORS

        // Use this to recover ExceptionDispatchInfo when outside the "with" part of a try/with block.
        // This indicates all the places where we lose a stack trace.
        //
        // Stack trace losses come when interoperating with other code that only provide us with an exception value,
        // notably .NET 4.x tasks and user exceptions passed to the exception continuation in Async.FromContinuations.
        let MayLoseStackTrace exn = ExceptionDispatchInfo.RestoreOrCapture(exn)
        
        // Call the exception continuation
        let errorT args edi = 
            args.aux.econt edi

        // Call the cancellation continuation
        let cancelT (args:AsyncParams<_>) =
            args.aux.ccont (new OperationCanceledException())
                   
        // Build a primitive without any exception of resync protection
        //
        // Use carefully!!
        let unprotectedPrimitive f = P f 

        let protectedPrimitiveCore args f =
            if args.aux.token.IsCancellationRequested then
                cancelT args
            else
                try 
                    f args
                with exn -> 
                    let edi = ExceptionDispatchInfo.RestoreOrCapture(exn)
                    errorT args edi

        // When run, ensures that any exceptions raised by the immediate execution of "f" are
        // sent to the exception continuation.
        //
        let protectedPrimitive f =
            unprotectedPrimitive (fun args -> protectedPrimitiveCore args f)

        let reify res =
            unprotectedPrimitive (fun args ->
                match res with
                |   AsyncImplResult.Ok r -> args.cont r
                |   AsyncImplResult.Error e -> args.aux.econt e
                |   AsyncImplResult.Canceled oce -> args.aux.ccont oce)

        //----------------------------------
        // BUILDER OPREATIONS

        // Generate async computation which calls its continuation with the given result
        let resultA x = 
            unprotectedPrimitive (fun ({ aux = aux } as args) -> 
                if aux.token.IsCancellationRequested then
                    cancelT args
                else
                    hijack aux.trampolineHolder x args.cont)
                    


        // The primitive bind operation. Generate a process that runs the first process, takes
        // its result, applies f and then runs the new process produced. Hijack if necessary and 
        // run 'f' with exception protection
        let bindA p1 f  =
            unprotectedPrimitive (fun args ->
                if args.aux.token.IsCancellationRequested then
                    cancelT args
                else

                    let args =
                        let cont a = protectNoHijack args.aux.econt f a (fun p2 -> invokeA p2 args)
                        { cont=cont;
                          aux = args.aux
                        }
                    // Trampoline the continuation onto a new work item every so often 
                    let trampoline = args.aux.trampolineHolder.Trampoline
                    if trampoline.IncrementBindCount() then
                        trampoline.Set(fun () -> invokeA p1 args)
                        FakeUnit
                    else
                        // NOTE: this must be a tailcall
                        invokeA p1 args)


        // callA = "bindA (return x) f"
        let callA f x =
            unprotectedPrimitive (fun args ->
                if args.aux.token.IsCancellationRequested then
                    cancelT args
                else                    
                    protect args.aux.trampolineHolder args.aux.econt f x (fun p2 -> invokeA p2 args)
            )

        // delayPrim = "bindA (return ()) f"
        let delayA f = callA f ()

        // Call p but augment the normal, exception and cancel continuations with a call to finallyFunction.
        // If the finallyFunction raises an exception then call the original exception continuation
        // with the new exception. If exception is raised after a cancellation, exception is ignored
        // and cancel continuation is called.
        let tryFinallyA finallyFunction p  =
            unprotectedPrimitive (fun args ->
                if args.aux.token.IsCancellationRequested then
                    cancelT args
                else
                    let trampolineHolder = args.aux.trampolineHolder
                    // The new continuation runs the finallyFunction and resumes the old continuation
                    // If an exception is thrown we continue with the previous exception continuation.
                    let cont b     = protect trampolineHolder args.aux.econt finallyFunction () (fun () -> args.cont b)
                    // The new exception continuation runs the finallyFunction and then runs the previous exception continuation.
                    // If an exception is thrown we continue with the previous exception continuation.
                    let econt exn  = protect trampolineHolder args.aux.econt finallyFunction () (fun () -> args.aux.econt exn)
                    // The cancellation continuation runs the finallyFunction and then runs the previous cancellation continuation.
                    // If an exception is thrown we continue with the previous cancellation continuation (the exception is lost)
                    let ccont cexn = protect trampolineHolder (fun _ -> args.aux.ccont cexn) finallyFunction () (fun () -> args.aux.ccont cexn)
                    invokeA p { args with cont = cont; aux = { args.aux with econt = econt; ccont = ccont } })

        // Re-route the exception continuation to call to catchFunction. If catchFunction or the new process fail
        // then call the original exception continuation with the failure.
        let tryWithDispatchInfoA catchFunction p =
            unprotectedPrimitive (fun args ->
                if args.aux.token.IsCancellationRequested then
                    cancelT args
                else 
                    let econt (edi: ExceptionDispatchInfo) = invokeA (callA catchFunction edi) args
                    invokeA p { args with aux = { args.aux with econt = econt } })

        let tryWithExnA catchFunction computation = 
            computation |> tryWithDispatchInfoA (fun edi -> catchFunction (edi.GetAssociatedSourceException()))

        /// Call the finallyFunction if the computation results in a cancellation
        let whenCancelledA (finallyFunction : OperationCanceledException -> unit) p =
            unprotectedPrimitive (fun ({ aux = aux } as args)->
                let ccont exn = protect aux.trampolineHolder (fun _ -> aux.ccont exn) finallyFunction exn (fun _ -> aux.ccont exn)
                invokeA p { args with aux = { aux with ccont = ccont } })

        let getCancellationToken()  =
            unprotectedPrimitive (fun ({ aux = aux } as args) -> args.cont aux.token)
        
        let getTrampolineHolder() =
            unprotectedPrimitive (fun ({ aux = aux } as args) -> args.cont aux.trampolineHolder)

        /// Return a unit result
        let doneA           = 
            resultA()

        /// Implement use/Dispose
        let usingA (r:'T :> IDisposable) f =  
            tryFinallyA (fun () -> r.Dispose()) (callA f r)

        let ignoreA p = 
            bindA p (fun _ -> doneA)

        /// Implement the while loop
        let rec whileA gd prog =
            if gd() then 
                bindA prog (fun () -> whileA gd prog) 
            else 
                doneA

        /// Implement the for loop
        let rec forA (e: seq<_>) prog =
            usingA (e.GetEnumerator()) (fun ie ->
                whileA
                    (fun () -> ie.MoveNext())
                    (delayA(fun () -> prog ie.Current)))


        let sequentialA p1 p2 = 
            bindA p1 (fun () -> p2)


    open AsyncBuilderImpl
    
    [<Sealed>]
    [<CompiledName("FSharpAsyncBuilder")>]
    type AsyncBuilder() =
        member b.Zero()                 = doneA
        member b.Delay(f)               = delayA(f)
        member b.Return(x)              = resultA(x)
        member b.ReturnFrom(x:Async<_>) = x
        member b.Bind(p1, p2)           = bindA p1 p2
        member b.Using(g, p)            = usingA g p
        member b.While(gd, prog)        = whileA gd prog
        member b.For(e, prog)           = forA e prog
        member b.Combine(p1, p2)        = sequentialA p1 p2
        member b.TryFinally(p, cf)      = tryFinallyA cf p
        member b.TryWith(p, cf)         = tryWithExnA cf p

    module AsyncImpl = 
        let async = AsyncBuilder()

        //----------------------------------
        // DERIVED SWITCH TO HELPERS

#if FX_NO_SYNC_CONTEXT
#else
        let switchTo (ctxt: SynchronizationContext) =
            protectedPrimitive(fun ({ aux = aux } as args) ->
                aux.trampolineHolder.Post ctxt  (fun () -> args.cont () ))
#endif

        let switchToNewThread() =
            protectedPrimitive(fun ({ aux = aux } as args) ->
                aux.trampolineHolder.StartThread (fun () -> args.cont () ) )

        let switchToThreadPool() =
            protectedPrimitive(fun ({ aux = aux } as args) -> 
                aux.trampolineHolder.QueueWorkItem (fun () -> args.cont ()) )

        //----------------------------------
        // DERIVED ASYNC RESYNC HELPERS

        let delimitContinuationsWith (delimiter : TrampolineHolder -> (unit -> FakeUnitValue) -> FakeUnitValue) ({ aux = aux } as args) =
            let trampolineHolder = aux.trampolineHolder
            {   args with
                    cont = (fun x -> delimiter trampolineHolder (fun () -> args.cont x))
                    aux = { aux with
                                econt = (fun x -> delimiter trampolineHolder (fun () -> aux.econt x ));
                                ccont = (fun x -> delimiter trampolineHolder (fun () -> aux.ccont x))                                  
                          }
            }

#if FX_NO_SYNC_CONTEXT
        let getSyncContext _ = null
        let delimitSyncContext args = args
        let postOrQueue _  (trampolineHolder:TrampolineHolder) f =
            trampolineHolder.QueueWorkItem f 
#else
        let getSyncContext () = System.Threading.SynchronizationContext.Current 
            
        let postOrQueue (ctxt : SynchronizationContext) (trampolineHolder:TrampolineHolder) f =
            match ctxt with 
            | null -> trampolineHolder.QueueWorkItem f 
            | _ -> trampolineHolder.Post ctxt f            


        let delimitSyncContext args =            
            match getSyncContext () with
            | null -> args
            | ctxt -> 
                let aux = args.aux                
                let trampolineHolder = aux.trampolineHolder
                {   args with
                         cont = (fun x -> trampolineHolder.Post ctxt (fun () -> args.cont x))
                         aux = { aux with
                                     econt = (fun x -> trampolineHolder.Post ctxt (fun () -> aux.econt x ));
                                     ccont = (fun x -> trampolineHolder.Post ctxt (fun () -> aux.ccont x))                                  
                               }
                }
                                    
#endif


        // When run, ensures that each of the continuations of the process are run in the same synchronization context.
        let protectedPrimitiveWithResync f = 
            protectedPrimitive(fun args -> 
                let args = delimitSyncContext args
                f args)

        let unprotectedPrimitiveWithResync f = 
            unprotectedPrimitive(fun args -> 
                let args = delimitSyncContext args
                f args)

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

        [<Sealed>]
        [<AutoSerializable(false)>]        
        type SuspendedAsync<'T>(args : AsyncParams<'T>) =
            let ctxt = getSyncContext ()
#if FX_NO_SYNC_CONTEXT
#else
            let thread = 
                match ctxt with
                |   null -> null // saving a thread-local access
                |   _ -> Thread.CurrentThread 
#endif
            let trampolineHolder = args.aux.trampolineHolder
            member this.ContinueImmediate res = 
                let action () = args.cont res
                let inline executeImmediately () = trampolineHolder.Protect action
#if FX_NO_SYNC_CONTEXT
                executeImmediately ()
#else
                let currentCtxt = System.Threading.SynchronizationContext.Current 
                match ctxt, currentCtxt with
                | null, null -> 
                    executeImmediately ()
                // See bug 370350; this logic is incorrect from the perspective of how SynchronizationContext is meant to work,
                // but the logic works for mainline scenarios (WinForms/WPF/ASP.NET) and we won't change it again.
                | _ when Object.Equals(ctxt, currentCtxt) && thread.Equals(Thread.CurrentThread) ->
                        executeImmediately ()
                | _ -> 
                    postOrQueue ctxt trampolineHolder action
#endif
                    
            member this.ContinueWithPostOrQueue res =
                postOrQueue ctxt trampolineHolder (fun () -> args.cont res)

            

        // A utility type to provide a synchronization point between an asynchronous computation 
        // and callers waiting on the result of that computation.
        //
        // Use with care! 
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
#if FX_EVENTWAITHANDLE_NO_IDISPOSABLE
                            ev.Dispose()                            
                            System.GC.SuppressFinalize(ev)
#else                        
                            ev.Close();
#endif                            
                            resEvent <- null)

            interface IDisposable with
                member x.Dispose() = x.Close() // ; System.GC.SuppressFinalize(x)


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
                |   [] -> FakeUnit
                |   [cont] -> 
                        if reuseThread then
                            cont.ContinueImmediate(res)
                        else
                            cont.ContinueWithPostOrQueue(res)
                |   otherwise ->
                        otherwise |> List.iter (fun cont -> cont.ContinueWithPostOrQueue(res) |> unfake) |> fake
            
            member x.ResultAvailable = result.IsSome

            member x.AwaitResult  =
                unprotectedPrimitive(fun args ->                    
                    // Check if a result is available synchronously                
                    let resOpt =
                        match result with
                        |   Some _ -> result
                        |   None ->                 
                                lock syncRoot (fun () ->
                                    match result with
                                    | Some _ ->
                                        result
                                    | None ->
                                        // Otherwise save the continuation and call it in RegisterResult                                        
                                        savedConts <- (SuspendedAsync<_>(args))::savedConts
                                        None
                                )
                    match resOpt with
                    |   Some res -> args.cont res
                    |   None -> FakeUnit
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

    open AsyncImpl
    
    type private Closure<'T>(f) =
        member x.Invoke(sender:obj, a:'T) : unit = ignore(sender); f(a)

    module CancellationTokenOps =
        /// Run the asynchronous workflow and wait for its result.
        let private RunSynchronouslyInAnotherThread (token:CancellationToken,computation,timeout) =
            let token,innerCTS = 
                // If timeout is provided, we govern the async by our own CTS, to cancel
                // when execution times out. Otherwise, the user-supplied token governs the async.
                match timeout with 
                |   None -> token,None
                |   Some _ ->
                        let subSource = new LinkedSubSource(token)
                        subSource.Token, Some subSource
                
            use resultCell = new ResultCell<AsyncImplResult<_>>()
            queueAsync 
                    token                        
                    (fun res -> resultCell.RegisterResult(Ok(res),reuseThread=true))
                    (fun edi -> resultCell.RegisterResult(Error(edi),reuseThread=true))
                    (fun exn -> resultCell.RegisterResult(Canceled(exn),reuseThread=true))
                    computation 
                |> unfake

            let res = resultCell.TryWaitForResultSynchronously(?timeout = timeout) in
            match res with
            | None -> // timed out
                // issue cancelaltion signal
                if innerCTS.IsSome then innerCTS.Value.Cancel() 
                // wait for computation to quiesce; drop result on the floor
                resultCell.TryWaitForResultSynchronously() |> ignore 
                // dispose the CancellationTokenSource
                if innerCTS.IsSome then innerCTS.Value.Dispose()
                raise (System.TimeoutException())
            | Some res ->
                match innerCTS with
                |   Some subSource -> subSource.Dispose()
                |   None -> ()
                commit res

        let private RunSynchronouslyInCurrentThread (token:CancellationToken,computation) =
            use resultCell = new ResultCell<AsyncImplResult<_>>()
            let trampolineHolder = TrampolineHolder()

            trampolineHolder.Protect
                (fun () ->
                    startA
                        token
                        trampolineHolder
                        (fun res -> resultCell.RegisterResult(Ok(res),reuseThread=true))
                        (fun edi -> resultCell.RegisterResult(Error(edi),reuseThread=true))
                        (fun exn -> resultCell.RegisterResult(Canceled(exn),reuseThread=true))
                        computation)
            |> unfake

            commit (resultCell.TryWaitForResultSynchronously() |> Option.get)

        let RunSynchronously (token:CancellationToken,computation,timeout) =
            // Reuse the current ThreadPool thread if possible. Unfortunately
            // Thread.IsThreadPoolThread isn't available on all profiles so
            // we approximate it by testing synchronization context for null.
            match SynchronizationContext.Current, timeout with
            | null, None -> RunSynchronouslyInCurrentThread (token, computation)
            // When the timeout is given we need a dedicated thread
            // which cancels the computation.
            // Performing the cancellation in the ThreadPool eg. by using
            // Timer from System.Threading or CancellationTokenSource.CancelAfter
            // (which internally uses Timer) won't work properly
            // when the ThreadPool is busy.
            //
            // And so when the timeout is given we always use the current thread
            // for the cancellation and run the computation in another thread.
            | _ -> RunSynchronouslyInAnotherThread (token, computation, timeout)

        let Start (token:CancellationToken,computation) =
            queueAsync 
                  token
                  (fun () -> FakeUnit)   // nothing to do on success
                  (fun edi -> edi.ThrowAny())   // raise exception in child
                  (fun _ -> FakeUnit)    // ignore cancellation in child
                  computation
               |> unfake

        let StartWithContinuations(token:CancellationToken, a:Async<'T>, cont, econt, ccont) : unit =
            startAsync token (cont >> fake) (econt >> fake) (ccont >> fake) a |> ignore
            
#if FX_NO_TASK
#else
        type VolatileBarrier() =
            [<VolatileField>]
            let mutable isStopped = false
            member __.Proceed = not isStopped
            member __.Stop() = isStopped <- true

        let StartAsTask (token:CancellationToken, computation : Async<_>,taskCreationOptions) : Task<_> =
            let taskCreationOptions = defaultArg taskCreationOptions TaskCreationOptions.None
            let tcs = new TaskCompletionSource<_>(taskCreationOptions)

            // The contract: 
            //      a) cancellation signal should always propagate to task
            //      b) CancellationTokenSource that produced a token must not be disposed until the task.IsComplete
            // We are:
            //      1) registering for cancellation signal here so that not to miss the signal
            //      2) disposing the registration just before setting result/exception on TaskCompletionSource -
            //              otherwise we run a chance of disposing registration on already disposed  CancellationTokenSource
            //              (See (b) above)
            //      3) ensuring if reg is disposed, we do SetResult
            let barrier = VolatileBarrier()
            let reg = token.Register(fun _ -> if barrier.Proceed then tcs.SetCanceled())
            let task = tcs.Task
            let disposeReg() =
                barrier.Stop()
                if not (task.IsCanceled) then reg.Dispose()

            let a = 
                async { 
                    try
                        let! result = computation
                        do 
                            disposeReg()
                            tcs.TrySetResult(result) |> ignore
                    with exn -> 
                        disposeReg()
                        tcs.TrySetException(exn) |> ignore
                }
            Start(token, a)
            task
            
#endif
            

    [<Sealed>]
    [<CompiledName("FSharpAsync")>]
    type Async =
    
        static member CancellationToken = getCancellationToken()

        static member CancelCheck () = doneA

        static member FromContinuations (f : ('T -> unit) * (exn -> unit) * (OperationCanceledException -> unit) -> unit) : Async<'T> = 
            unprotectedPrimitive (fun ({ aux = aux } as args) ->
                if args.aux.token.IsCancellationRequested then
                    cancelT args
                else
                    let underCurrentThreadStack = ref true
                    let contToTailCall = ref None
                    let thread = Thread.CurrentThread
                    let latch = Latch()
                    let once cont x = 
                        if not(latch.Enter()) then invalidOp(SR.GetString(SR.controlContinuationInvokedMultipleTimes))
                        if Thread.CurrentThread.Equals(thread) && !underCurrentThreadStack then
                            contToTailCall := Some(fun () -> cont x)
                        else if Trampoline.ThisThreadHasTrampoline then
                            let ctxt = getSyncContext()
                            postOrQueue ctxt aux.trampolineHolder (fun () -> cont x) |> unfake 
                        else
                            aux.trampolineHolder.Protect (fun () -> cont x ) |> unfake
                    try 
                        f (once args.cont, (fun exn -> once aux.econt (MayLoseStackTrace(exn))), once aux.ccont)
                    with exn -> 
                        if not(latch.Enter()) then invalidOp(SR.GetString(SR.controlContinuationInvokedMultipleTimes))
                        let edi = ExceptionDispatchInfo.RestoreOrCapture(exn)
                        aux.econt edi |> unfake

                    underCurrentThreadStack := false

                    match !contToTailCall with
                    | Some k -> k()
                    | _ -> FakeUnit
                    )
                
        static member DefaultCancellationToken = defaultCancellationTokenSource.Token

        static member CancelDefaultToken() =
            let cts = defaultCancellationTokenSource
            // set new CancellationTokenSource before calling Cancel - otherwise if Cancel throws token will stay unchanged
            defaultCancellationTokenSource <- new CancellationTokenSource()
            // we do not dispose the old default CTS - let GC collect it
            cts.Cancel()
            // we do not dispose the old default CTS - let GC collect it
            
        static member Catch (p: Async<'T>) =
            unprotectedPrimitive  (fun ({ aux = aux } as args) ->
                startA aux.token aux.trampolineHolder (Choice1Of2 >> args.cont) (fun edi -> args.cont (Choice2Of2 (edi.GetAssociatedSourceException()))) aux.ccont p)

        static member RunSynchronously (p: Async<'T>,?timeout,?cancellationToken:CancellationToken) =
            let timeout,token =
                match cancellationToken with
                |   None -> timeout,defaultCancellationTokenSource.Token
                |   Some token when not token.CanBeCanceled -> timeout, token
                |   Some token -> None, token
            CancellationTokenOps.RunSynchronously(token, p, timeout)

        static member Start (computation, ?cancellationToken) =
            let token = defaultArg cancellationToken defaultCancellationTokenSource.Token
            CancellationTokenOps.Start (token, computation)

#if FX_NO_TASK
#else
        static member StartAsTask (computation,?taskCreationOptions,?cancellationToken)=
            let token = defaultArg cancellationToken defaultCancellationTokenSource.Token        
            CancellationTokenOps.StartAsTask(token,computation,taskCreationOptions)
        
        static member StartChildAsTask (computation,?taskCreationOptions) =
            async { let! token = getCancellationToken()  
                    return CancellationTokenOps.StartAsTask(token,computation, taskCreationOptions) }
#endif

    type Async with
        static member Parallel (l: seq<Async<'T>>) =
            unprotectedPrimitive (fun args ->
                let tasks,result = 
                    try 
                        Seq.toArray l, None   // manually protect eval of seq
                    with exn -> 
                        let edi = ExceptionDispatchInfo.RestoreOrCapture(exn)
                        null, Some(errorT args edi)

                match result with
                | Some r -> r
                | None ->
                if tasks.Length = 0 then args.cont [| |] else  // must not be in a 'protect' if we call cont explicitly; if cont throws, it should unwind the stack, preserving Dev10 behavior
                protectedPrimitiveCore args (fun args ->
                    let ({ aux = aux } as args) = delimitSyncContext args  // manually resync
                    let count = ref tasks.Length
                    let firstExn = ref None
                    let results = Array.zeroCreate tasks.Length
                    // Attept to cancel the individual operations if an exception happens on any of the other threads
                    let innerCTS = new LinkedSubSource(aux.token)
                    let trampolineHolder = aux.trampolineHolder
                    
                    let finishTask(remaining) = 
                        if (remaining = 0) then 
                            innerCTS.Dispose()
                            match (!firstExn) with 
                            | None -> trampolineHolder.Protect(fun () -> args.cont results)
                            | Some (Choice1Of2 exn) -> trampolineHolder.Protect(fun () -> aux.econt exn)
                            | Some (Choice2Of2 cexn) -> trampolineHolder.Protect(fun () -> aux.ccont cexn)
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
                        queueAsync
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
            unprotectedPrimitive(fun args ->
                let result =
                    try Seq.toArray computations |> Choice1Of2
                    with exn -> ExceptionDispatchInfo.RestoreOrCapture exn |> Choice2Of2

                match result with
                | Choice2Of2 edi -> args.aux.econt edi
                | Choice1Of2 [||] -> args.cont None
                | Choice1Of2 computations ->
                    protectedPrimitiveCore args (fun args ->
                        let ({ aux = aux } as args) = delimitSyncContext args
                        let noneCount = ref 0
                        let exnCount = ref 0
                        let innerCts = new LinkedSubSource(aux.token)
                        let trampolineHolder = aux.trampolineHolder

                        let scont (result : 'T option) =
                            match result with
                            | Some _ -> 
                                if Interlocked.Increment exnCount = 1 then
                                    innerCts.Cancel(); trampolineHolder.Protect(fun () -> args.cont result)
                                else
                                    FakeUnit

                            | None ->
                                if Interlocked.Increment noneCount = computations.Length then
                                    innerCts.Cancel(); trampolineHolder.Protect(fun () -> args.cont None)
                                else
                                    FakeUnit
 
                        let econt (exn : ExceptionDispatchInfo) =
                            if Interlocked.Increment exnCount = 1 then 
                                innerCts.Cancel(); trampolineHolder.Protect(fun () -> args.aux.econt exn)
                            else
                                FakeUnit
 
                        let ccont (exn : OperationCanceledException) =
                            if Interlocked.Increment exnCount = 1 then
                                innerCts.Cancel(); trampolineHolder.Protect(fun () -> args.aux.ccont exn)
                            else
                                FakeUnit

                        for c in computations do
                            queueAsync innerCts.Token scont econt ccont c |> unfake

                        FakeUnit))

#if FX_NO_TASK
#else
    // Contains helpers that will attach continuation to the given task.
    // Should be invoked as a part of protectedPrimitive(withResync) call
    module TaskHelpers = 
        let continueWith (task : Task<'T>, args) = 

            let continuation (completedTask : Task<_>) : unit =
                args.aux.trampolineHolder.Protect((fun () ->
                    if completedTask.IsCanceled then
                        args.aux.ccont (new OperationCanceledException())
                    elif completedTask.IsFaulted then
                        args.aux.econt (MayLoseStackTrace(completedTask.Exception))
                    else
                        args.cont completedTask.Result)) |> unfake

            task.ContinueWith(Action<Task<'T>>(continuation), TaskContinuationOptions.None) |> ignore |> fake

        let continueWithUnit (task : Task, args) = 

            let continuation (completedTask : Task) : unit =
                args.aux.trampolineHolder.Protect((fun () ->
                    if completedTask.IsCanceled then
                        args.aux.ccont (new OperationCanceledException())
                    elif completedTask.IsFaulted then
                        args.aux.econt (MayLoseStackTrace(completedTask.Exception))
                    else
                        args.cont ())) |> unfake

            task.ContinueWith(Action<Task>(continuation), TaskContinuationOptions.None) |> ignore |> fake
#endif

#if FX_NO_REGISTERED_WAIT_HANDLES
    [<Sealed>]
    [<AutoSerializable(false)>]
    type internal WaitHandleIAsyncResult(wh : WaitHandle) = 
        interface System.IAsyncResult with
            member this.AsyncState = null
            member this.AsyncWaitHandle = wh
            member this.IsCompleted = 
#if FX_NO_WAITONE_MILLISECONDS
                wh.WaitOne(TimeSpan(0L))
#else
#if FX_NO_EXIT_CONTEXT_FLAGS
                wh.WaitOne(0)
#else
                wh.WaitOne(0,exitContext=false)
#endif
#endif
            member this.CompletedSynchronously = false // always reschedule
#endif

    type Async with

        /// StartWithContinuations, except the exception continuation is given an ExceptionDispatchInfo
        static member StartWithContinuationsUsingDispatchInfo(computation:Async<'T>, continuation, exceptionContinuation, cancellationContinuation, ?cancellationToken) : unit =
            let token = defaultArg cancellationToken defaultCancellationTokenSource.Token
            CancellationTokenOps.StartWithContinuations(token, computation, continuation, exceptionContinuation, cancellationContinuation)

        static member StartWithContinuations(computation:Async<'T>, continuation, exceptionContinuation, cancellationContinuation, ?cancellationToken) : unit =
            Async.StartWithContinuationsUsingDispatchInfo(computation, continuation, (fun edi -> exceptionContinuation (edi.GetAssociatedSourceException())), cancellationContinuation, ?cancellationToken=cancellationToken)

        static member StartImmediate(computation:Async<unit>, ?cancellationToken) : unit =
            let token = defaultArg cancellationToken defaultCancellationTokenSource.Token
            CancellationTokenOps.StartWithContinuations(token, computation, id, (fun edi -> edi.ThrowAny()), ignore)

#if FSHARP_CORE_NETCORE_PORTABLE
        static member Sleep(dueTime : int) : Async<unit> = 
            // use combo protectedPrimitiveWithResync + continueWith instead of AwaitTask so we can pass cancellation token to the Delay task
            unprotectedPrimitiveWithResync ( fun ({ aux = aux} as args) ->
                let mutable edi = null

                let task = 
                    try 
                        Task.Delay(dueTime, aux.token)
                    with exn -> 
                        edi <- ExceptionDispatchInfo.RestoreOrCapture(exn)
                        null

                match edi with
                | null -> TaskHelpers.continueWithUnit(task, args)
                | _ -> aux.econt edi
            )
#else
        static member Sleep(dueTime) : Async<unit> =
            unprotectedPrimitiveWithResync (fun ({ aux = aux } as args) ->
                let timer = ref (None : Timer option)
                let savedCont = args.cont
                let savedCCont = aux.ccont
                let latch = new Latch()
                let registration =
                    aux.token.Register(
                        (fun _ -> 
                            if latch.Enter() then
                                match !timer with
                                | None -> ()
                                | Some t -> t.Dispose()
                                aux.trampolineHolder.Protect(fun () -> savedCCont(new OperationCanceledException())) |> unfake
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
                                            // Note: there is a race here: the System.Threading.Timer time very occasionally
                                            // calls the callback _before_ the timer object has been recorded anywhere. This makes it difficult to dispose the
                                            // timer in this situation. In this case we just let the timer be collected by finalization.
                                            match !timer with
                                            |  None -> ()
                                            |  Some t -> t.Dispose()
                                            // Now we're done, so call the continuation
                                            aux.trampolineHolder.Protect (fun () -> savedCont()) |> unfake),
                                     null, dueTime=dueTime, period = -1) |> Some
                with exn -> 
                    if latch.Enter() then 
                        edi <- ExceptionDispatchInfo.RestoreOrCapture(exn) // post exception to econt only if we successfully enter the latch (no other continuations were called)

                match edi with 
                | null -> 
                    FakeUnit
                | _ -> 
                    aux.econt edi
                )
#endif
        
        static member AwaitWaitHandle(waitHandle:WaitHandle,?millisecondsTimeout:int) =
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
#if FX_NO_REGISTERED_WAIT_HANDLES
                protectedPrimitiveWithResync(fun ({ aux = aux } as args) ->
                    // The .NET Compact Framework doesn't support RegisterWaitForSingleObject
                    
                    // Latch is used to protect entrance to the cancelHandler/actual continuation/error continuation
                    let latch = Latch()

                    let scont = args.cont
                    let ccont = aux.ccont

                    // cancel action
                    let cancel e = 
                        if latch.Enter() then
                            Async.Start (async { do (ccont e |> unfake) })

                    // register cancellation handler
                    let registration = aux.token.Register(fun () -> cancel (OperationCanceledException()))

                    // run actual await routine
                    // callback will be executed on the thread pool so we need to use TrampolineHolder.Protect to install trampoline
                    try
                        Task.Factory.FromAsync
                            (
                                WaitHandleIAsyncResult(waitHandle),
                                fun _ -> 
                                    if latch.Enter() then
                                        registration.Dispose()
                                        aux.trampolineHolder.Protect(fun () -> scont true) 
                                        |> unfake
                            )
                            |> ignore
                        // if user has specified timeout different from Timeout.Infinite 
                        // then start another async to track timeout expiration
                        if millisecondsTimeout <> Timeout.Infinite then 
                            Async.StartWithContinuations
                                (
                                    computation = (Async.Sleep millisecondsTimeout),
                                    continuation = (fun () -> 
                                        if latch.Enter() then 
                                            registration.Dispose()
                                            aux.trampolineHolder.Protect(fun () ->  scont false)
                                            |> unfake),
                                    exceptionContinuation = ignore, // we do not expect exceptions here
                                    cancellationContinuation = cancel,
                                    cancellationToken = aux.token
                                )
                        FakeUnit
                    with e -> 
                        if latch.Enter() then 
                            registration.Dispose()
                            reraise() // exception will be intercepted by try..with in protectedPrimitiveWithResync
                        else FakeUnit
                    )

#else
                protectedPrimitiveWithResync(fun ({ aux = aux } as args) ->
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
                                Async.Start (async { do (aux.ccont (OperationCanceledException()) |> unfake) }))

                    and registration : CancellationTokenRegistration = aux.token.Register(cancelHandler, null)
                    
                    let savedCont = args.cont
                    try
                        lock rwh (fun () ->
                            rwh := Some(ThreadPool.RegisterWaitForSingleObject
                                          (waitObject=waitHandle,
                                           callBack=WaitOrTimerCallback(fun _ timeOut ->
                                                        if latch.Enter() then
                                                            lock rwh (fun () -> rwh.Value.Value.Unregister(null) |> ignore)
                                                            rwh := None
                                                            registration.Dispose()
                                                            aux.trampolineHolder.Protect (fun () -> savedCont (not timeOut)) |> unfake),
                                           state=null,
                                           millisecondsTimeOutInterval=millisecondsTimeout,
                                           executeOnlyOnce=true));
                            FakeUnit)
                    with _ -> 
                        if latch.Enter() then
                            registration.Dispose()
                            reraise() // reraise exception only if we successfully enter the latch (no other continuations were called)
                        else FakeUnit
                    )
#endif

        static member AwaitIAsyncResult(iar: IAsyncResult, ?millisecondsTimeout): Async<bool> =
            async { if iar.CompletedSynchronously then 
                        return true
                    else
                        return! Async.AwaitWaitHandle(iar.AsyncWaitHandle, ?millisecondsTimeout=millisecondsTimeout)  }


        /// Await the result of a result cell without a timeout
        static member ReifyResult(result:AsyncImplResult<'T>) : Async<'T> =
            unprotectedPrimitive(fun ({ aux = aux } as args) -> 
                   (match result with 
                    | Ok v -> args.cont v 
                    | Error exn -> aux.econt exn 
                    | Canceled exn -> aux.ccont exn) )

        /// Await the result of a result cell without a timeout       
        static member AwaitAndReifyResult(resultCell:ResultCell<AsyncImplResult<'T>>) : Async<'T> =
            async {
                let! result = resultCell.AwaitResult
                return! Async.ReifyResult(result)
            }
                    


        /// Await the result of a result cell without a timeout
        ///
        /// Always resyncs to the synchronization context if needed, by virtue of it being built
        /// from primitives which resync.
        static member AsyncWaitAsyncWithTimeout(innerCTS : CancellationTokenSource, resultCell:ResultCell<AsyncImplResult<'T>>,millisecondsTimeout) : Async<'T> =
            match millisecondsTimeout with
            | None | Some -1 -> 
                resultCell |> Async.AwaitAndReifyResult

            | Some 0 -> 
                async { if resultCell.ResultAvailable then 
                            return commit (resultCell.GrabResult())
                        else
                            return commitWithPossibleTimeout None }
            | _ ->
                async { try 
                           if resultCell.ResultAvailable then 
                             return commit (resultCell.GrabResult())
                           else
                             let! ok = Async.AwaitWaitHandle (resultCell.GetWaitHandle(),?millisecondsTimeout=millisecondsTimeout) 
                             if ok then
                                return commitWithPossibleTimeout (Some (resultCell.GrabResult())) 
                             else // timed out
                                // issue cancellation signal
                                innerCTS.Cancel()
                                // wait for computation to queisce
                                let! _ = Async.AwaitWaitHandle (resultCell.GetWaitHandle())                                
                                return commitWithPossibleTimeout None 
                         finally 
                           resultCell.Close() } 


        static member FromBeginEnd(beginAction,endAction,?cancelAction): Async<'T> =
            async { let! cancellationToken = getCancellationToken()
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
                                            let canceledResult = Canceled (OperationCanceledException())
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
                                    |   Some _ -> 
                                            registration.Dispose()
                                    |   None -> 
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
                                    resultCell.RegisterResult(res,reuseThread=true) |> unfake
                                else ())
                                

                    
                    let (iar:IAsyncResult) = beginAction (callback,(null:obj))
                    if iar.CompletedSynchronously then 
                        registration.Dispose()
                        return endAction iar 
                    else 
                        return! Async.AwaitAndReifyResult(resultCell) }


        static member FromBeginEnd(arg1,beginAction,endAction,?cancelAction): Async<'T> =
            Async.FromBeginEnd((fun (iar,state) -> beginAction(arg1,iar,state)), endAction, ?cancelAction=cancelAction)


        static member FromBeginEnd(arg1,arg2,beginAction,endAction,?cancelAction): Async<'T> =
            Async.FromBeginEnd((fun (iar,state) -> beginAction(arg1,arg2,iar,state)), endAction, ?cancelAction=cancelAction)

        static member FromBeginEnd(arg1,arg2,arg3,beginAction,endAction,?cancelAction): Async<'T> =
            Async.FromBeginEnd((fun (iar,state) -> beginAction(arg1,arg2,arg3,iar,state)), endAction, ?cancelAction=cancelAction)



    [<Sealed>]
    [<AutoSerializable(false)>]
    type AsyncIAsyncResult<'T>(callback: System.AsyncCallback,state:obj) =
         // This gets set to false if the result is not available by the 
         // time the IAsyncResult is returned to the caller of Begin
         let mutable completedSynchronously = true 

         let mutable disposed = false

         let cts = new CancellationTokenSource()

         let result = new ResultCell<AsyncImplResult<'T>>()

         member s.SetResult(v: AsyncImplResult<'T>) =  
             result.RegisterResult(v,reuseThread=true) |> unfake
             match callback with
             | null -> ()
             | d -> 
                 // The IASyncResult becomes observable here
                 d.Invoke (s :> System.IAsyncResult)

         member s.GetResult() = 
             match result.TryWaitForResultSynchronously (-1) with 
             | Some (Ok v) -> v
             | Some (Error edi) -> edi.ThrowAny()
             | Some (Canceled err) -> raise err
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
        let beginAction(computation,callback,state) = 
               let aiar = new AsyncIAsyncResult<'T>(callback,state)
               let cont v = aiar.SetResult (Ok v)
               let econt v = aiar.SetResult (Error v)
               let ccont v = aiar.SetResult (Canceled v)
               CancellationTokenOps.StartWithContinuations(aiar.Token,computation,cont,econt,ccont)
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


    type Async with 

                   

        static member AsBeginEnd<'Arg,'T> (computation:('Arg -> Async<'T>)) :
                // The 'Begin' member
                ('Arg * System.AsyncCallback * obj -> System.IAsyncResult) * 
                // The 'End' member
                (System.IAsyncResult -> 'T) * 
                // The 'Cancel' member
                (System.IAsyncResult -> unit) =
                    let beginAction = fun (a1,callback,state) -> AsBeginEndHelpers.beginAction ((computation a1), callback, state)
                    beginAction, AsBeginEndHelpers.endAction<'T>, AsBeginEndHelpers.cancelAction<'T>

#if FX_NO_CREATE_DELEGATE
#else
        static member AwaitEvent(event:IEvent<'Delegate,'T>, ?cancelAction) : Async<'T> =
            async { let! token = getCancellationToken()
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
                                once.Do(fun () -> resultCell.RegisterResult(Canceled (OperationCanceledException()),reuseThread=true) |> unfake) 
                            | Some cancel -> 
                                // If we get an exception from a cooperative cancellation function
                                // we assume the operation has already completed.
                                try cancel() with _ -> ()
                        token.Register(Action<obj>(onCancel), null)
                    
                    and obj = 
                        new Closure<'T>(fun eventArgs ->
                            // Stop listening to events
                            event.RemoveHandler(del)
                            // The callback has been activated, so ensure cancellation is not possible beyond this point
                            once.Do(fun () -> registration.Dispose())
                            let res = Ok(eventArgs) 
                            // Register the result. This may race with a cancellation result, but
                            // ResultCell allows a race and throws away whichever comes last.
                            resultCell.RegisterResult(res,reuseThread=true) |> unfake) 
                    and del = 
#if FX_ATLEAST_PORTABLE
                        let invokeMeth = (typeof<Closure<'T>>).GetMethod("Invoke", BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance)
                        System.Delegate.CreateDelegate(typeof<'Delegate>, obj, invokeMeth) :?> 'Delegate
#else                    
                        System.Delegate.CreateDelegate(typeof<'Delegate>, obj, "Invoke") :?> 'Delegate
#endif                        
                    
                    // Start listening to events
                    event.AddHandler(del)

                    // Return the async computation that allows us to await the result
                    return! Async.AwaitAndReifyResult(resultCell) }
#endif

    type Async with
        static member Ignore (p: Async<'T>) = bindA p (fun _ -> doneA)
        static member SwitchToNewThread() = switchToNewThread()
        static member SwitchToThreadPool() = switchToThreadPool()

    type Async with

        static member StartChild (computation:Async<'T>,?millisecondsTimeout) =
            async { 
                let resultCell = new ResultCell<_>()
                let! ct = getCancellationToken()
                let innerCTS = new CancellationTokenSource() // innerCTS does not require disposal
                let ctsRef = ref innerCTS
                let reg = ct.Register(
                                        (fun _ -> 
                                            match !ctsRef with
                                            |   null -> ()
                                            |   otherwise -> otherwise.Cancel()), 
                                        null)
                do queueAsync 
                       innerCTS.Token
                       // since innerCTS is not ever Disposed, can call reg.Dispose() without a safety Latch
                       (fun res -> ctsRef := null; reg.Dispose(); resultCell.RegisterResult (Ok res, reuseThread=true))   
                       (fun edi -> ctsRef := null; reg.Dispose(); resultCell.RegisterResult (Error edi,reuseThread=true))   
                       (fun err -> ctsRef := null; reg.Dispose(); resultCell.RegisterResult (Canceled err,reuseThread=true))    
                       computation
                     |> unfake
                                               
                return Async.AsyncWaitAsyncWithTimeout(innerCTS, resultCell,millisecondsTimeout) }

#if FX_NO_SYNC_CONTEXT
#else

        static member SwitchToContext syncContext =
            async { match syncContext with 
                    | null -> 
                        // no synchronization context, just switch to the thread pool
                        do! Async.SwitchToThreadPool()
                    | ctxt -> 
                        // post the continuation to the synchronization context
                        return! switchTo ctxt }
#endif

        static member OnCancel action =
            async { let! ct = getCancellationToken ()
                    // latch protects CancellationTokenRegistration.Dispose from being called twice
                    let latch = Latch()
                    let rec handler (_ : obj) = 
                        try 
                            if latch.Enter() then registration.Dispose()
                            action () 
                        with _ -> ()                        
                    and registration : CancellationTokenRegistration = ct.Register(Action<obj>(handler), null)
                    return { new System.IDisposable with
                                member this.Dispose() = 
                                    // dispose CancellationTokenRegistration only if cancellation was not requested.
                                    // otherwise - do nothing, disposal will be performed by the handler itself
                                    if not ct.IsCancellationRequested then
                                        if latch.Enter() then registration.Dispose() } }

        static member TryCancelled (p: Async<'T>,f) = 
            whenCancelledA f p

#if FX_NO_TASK
#else
        static member AwaitTask (task:Task<'T>) : Async<'T> = 
            protectedPrimitiveWithResync (fun args -> 
                TaskHelpers.continueWith(task, args)
                )

        static member AwaitTask (task:Task) : Async<unit> = 
            protectedPrimitiveWithResync (fun args -> 
                TaskHelpers.continueWithUnit(task, args)
                )
#endif

    module CommonExtensions =

        open AsyncBuilderImpl

        type System.IO.Stream with

            [<CompiledName("AsyncRead")>] // give the extension member a 'nice', unmangled compiled name, unique within this module
            member stream.AsyncRead(buffer: byte[],?offset,?count) =
                let offset = defaultArg offset 0
                let count  = defaultArg count buffer.Length
#if FSHARP_CORE_NETCORE_PORTABLE
                // use combo protectedPrimitiveWithResync + continueWith instead of AwaitTask so we can pass cancellation token to the ReadAsync task
                protectedPrimitiveWithResync (fun ({ aux = aux } as args) ->
                    TaskHelpers.continueWith(stream.ReadAsync(buffer, offset, count, aux.token), args)
                    )
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
#if FSHARP_CORE_NETCORE_PORTABLE
                // use combo protectedPrimitiveWithResync + continueWith instead of AwaitTask so we can pass cancellation token to the WriteAsync task
                protectedPrimitiveWithResync ( fun ({ aux = aux} as args) ->
                    TaskHelpers.continueWithUnit(stream.WriteAsync(buffer, offset, count, aux.token), args)
                    )
#else
                Async.FromBeginEnd (buffer,offset,count,stream.BeginWrite,stream.EndWrite)
#endif
                
        type System.Threading.WaitHandle with
            member waitHandle.AsyncWaitOne(?millisecondsTimeout:int) =  // only used internally, not a public API
                Async.AwaitWaitHandle(waitHandle,?millisecondsTimeout=millisecondsTimeout) 

        type IObservable<'Args> with 

            [<CompiledName("AddToObservable")>] // give the extension member a 'nice', unmangled compiled name, unique within this module
            member x.Add(f: 'Args -> unit) = x.Subscribe f |> ignore

            [<CompiledName("SubscribeToObservable")>] // give the extension member a 'nice', unmangled compiled name, unique within this module
            member x.Subscribe(f) = 
                x.Subscribe { new IObserver<'Args> with 
                                  member x.OnNext(args) = f args 
                                  member x.OnError(e) = () 
                                  member x.OnCompleted() = () } 

    module WebExtensions =
        open AsyncBuilderImpl

#if FX_NO_WEB_REQUESTS
#else
        
        type System.Net.WebRequest with
            [<CompiledName("AsyncGetResponse")>] // give the extension member a 'nice', unmangled compiled name, unique within this module
            member req.AsyncGetResponse() : Async<System.Net.WebResponse>= 
                
                let canceled = ref false // WebException with Status = WebExceptionStatus.RequestCanceled  can be raised in other situations except cancellation, use flag to filter out false positives

                // Use tryWithDispatchInfoA to allow propagation of ExceptionDispatchInfo
                Async.FromBeginEnd(beginAction=req.BeginGetResponse, 
                                   endAction = req.EndGetResponse, 
                                   cancelAction = fun() -> canceled := true; req.Abort())
                |> tryWithDispatchInfoA (fun edi ->
                    match edi.SourceException with 
                    | :? System.Net.WebException as webExn 
                            when webExn.Status = System.Net.WebExceptionStatus.RequestCanceled && !canceled -> 

                        Async.ReifyResult(AsyncImplResult.Canceled (OperationCanceledException webExn.Message))
                    | _ -> 
                        edi.ThrowAny())

#endif
     
#if FX_NO_WEB_CLIENT
#else
        
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
                                elif args.Error <> null then
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
                let c = new ResultCell<_>()
                let! ct = Async.CancellationToken
                let start a f =
                    Async.StartWithContinuationsUsingDispatchInfo(a, 
                        (fun res -> c.RegisterResult(f res |> AsyncImplResult.Ok, reuseThread=false) |> unfake),
                        (fun edi -> c.RegisterResult(edi |> AsyncImplResult.Error, reuseThread=false) |> unfake),
                        (fun oce -> c.RegisterResult(oce |> AsyncImplResult.Canceled, reuseThread=false) |> unfake),
                        cancellationToken = ct
                        )
                start a1 Choice1Of2
                start a2 Choice2Of2
                let! result = c.AwaitResult
                return! reify result
            }
        let timeout msec cancellationToken =
            if msec < 0 then
                unprotectedPrimitive(fun _ -> FakeUnit) // "block" forever
            else
                let c = new ResultCell<_>()
                Async.StartWithContinuations(
                    computation=Async.Sleep(msec),
                    continuation=(fun () -> c.RegisterResult((), reuseThread = false) |> unfake),
                    exceptionContinuation=ignore, 
                    cancellationContinuation=ignore, 
                    cancellationToken = cancellationToken)
                c.AwaitResult

    [<Sealed>]
    [<AutoSerializable(false)>]        
    type Mailbox<'Msg>() =  
        let mutable inboxStore  = null 
        let mutable arrivals = new Queue<'Msg>()
        let syncRoot = arrivals

        // Control elements indicating the state of the reader. When the reader is "blocked" at an 
        // asynchronous receive, either 
        //     -- "cont" is non-null and the reader is "activated" by re-scheduling cont in the thread pool; or
        //     -- "pulse" is non-null and the reader is "activated" by setting this event
        let mutable savedCont : ((bool -> FakeUnitValue) * TrampolineHolder) option = None
        let mutable pulse : AutoResetEvent = null
        let ensurePulse() = 
            match pulse with 
            | null -> 
                pulse <- new AutoResetEvent(false);
            | _ -> 
                ()
            pulse
                
        let waitOneNoTimeout = 
            unprotectedPrimitive (fun ({ aux = aux } as args) -> 
                match savedCont with 
                | None -> 
                    let descheduled = 
                        // An arrival may have happened while we're preparing to deschedule
                        lock syncRoot (fun () -> 
                            if arrivals.Count = 0 then 
                                // OK, no arrival so deschedule
                                savedCont <- Some(args.cont, aux.trampolineHolder);
                                true
                            else
                                false)
                    if descheduled then 
                        FakeUnit 
                    else 
                        // If we didn't deschedule then run the continuation immediately
                        args.cont true
                | Some _ -> 
                    failwith "multiple waiting reader continuations for mailbox")

        let waitOne(timeout) = 
            if timeout < 0  then 
                waitOneNoTimeout
            else 
                ensurePulse().AsyncWaitOne(millisecondsTimeout=timeout)

        member x.inbox = 
            match inboxStore with 
            | null -> inboxStore <- new System.Collections.Generic.List<'Msg>(1) // ResizeArray
            | _ -> () 
            inboxStore

        member x.CurrentQueueLength = 
            lock syncRoot (fun () -> x.inbox.Count + arrivals.Count)

        member x.scanArrivalsUnsafe(f) =
            if arrivals.Count = 0 then None
            else let msg = arrivals.Dequeue()
                 match f msg with
                 | None -> 
                     x.inbox.Add(msg); 
                     x.scanArrivalsUnsafe(f)
                 | res -> res
        // Lock the arrivals queue while we scan that
        member x.scanArrivals(f) = lock syncRoot (fun () -> x.scanArrivalsUnsafe(f))

        member x.scanInbox(f,n) =
            match inboxStore with
            | null -> None
            | inbox ->
                if n >= inbox.Count
                then None
                else
                    let msg = inbox.[n]
                    match f msg with
                    | None -> x.scanInbox (f,n+1)
                    | res -> inbox.RemoveAt(n); res

        member x.receiveFromArrivalsUnsafe() =
            if arrivals.Count = 0 then None
            else Some(arrivals.Dequeue())

        member x.receiveFromArrivals() = 
            lock syncRoot (fun () -> x.receiveFromArrivalsUnsafe())

        member x.receiveFromInbox() =
            match inboxStore with
            | null -> None
            | inbox ->
                if inbox.Count = 0
                then None
                else
                    let x = inbox.[0]
                    inbox.RemoveAt(0);
                    Some(x)

        member x.Post(msg) =
            lock syncRoot (fun () ->
                arrivals.Enqueue(msg);
                // This is called when we enqueue a message, within a lock
                // We cooperatively unblock any waiting reader. If there is no waiting
                // reader we just leave the message in the incoming queue
                match savedCont with
                | None -> 
                    match pulse with 
                    | null -> 
                        () // no one waiting, leaving the message in the queue is sufficient
                    | ev -> 
                        // someone is waiting on the wait handle
                        ev.Set() |> ignore
                | Some(action,trampolineHolder) -> 
                    savedCont <- None
                    trampolineHolder.QueueWorkItem(fun () -> action true) |> unfake)

        member x.TryScan ((f: 'Msg -> (Async<'T>) option), timeout) : Async<'T option> =
            let rec scan timeoutAsync (timeoutCts:CancellationTokenSource) =
                async { match x.scanArrivals(f) with
                        | None -> 
                            // Deschedule and wait for a message. When it comes, rescan the arrivals
                            let! ok = AsyncHelpers.awaitEither waitOneNoTimeout timeoutAsync
                            match ok with
                            | Choice1Of2 true -> 
                                return! scan timeoutAsync timeoutCts
                            | Choice1Of2 false ->
                                return failwith "should not happen - waitOneNoTimeout always returns true"
                            | Choice2Of2 () ->
                                lock syncRoot (fun () -> 
                                    // Cancel the outstanding wait for messages installed by waitOneNoTimeout
                                    //
                                    // HERE BE DRAGONS. This is bestowed on us because we only support
                                    // a single mailbox reader at any one time.
                                    // If awaitEither returned control because timeoutAsync has terminated, waitOneNoTimeout
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
                async { match x.scanArrivals(f) with
                        |   None -> let! ok = waitOneNoTimeout
                                    if ok then
                                        return! scanNoTimeout()
                                    else
                                        return (failwith "Timed out with infinite timeout??")
                        |   Some resP -> 
                            let! res = resP
                            return Some res
                }

            // Look in the inbox first
            async { match x.scanInbox(f,0) with
                    |   None  when timeout < 0 -> return! scanNoTimeout()
                    |   None -> 
                            let! ct = Async.CancellationToken
                            let timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct, CancellationToken.None)
                            let timeoutAsync = AsyncHelpers.timeout timeout timeoutCts.Token
                            return! scan timeoutAsync timeoutCts
                    |   Some resP -> 
                            let! res = resP
                            return Some res

            }

        member x.Scan((f: 'Msg -> (Async<'T>) option), timeout) =
            async { let! resOpt = x.TryScan(f,timeout)
                    match resOpt with
                    | None -> return raise(TimeoutException(SR.GetString(SR.mailboxScanTimedOut)))
                    | Some res -> return res }


        member x.TryReceive(timeout) =
            let rec processFirstArrival() =
                async { match x.receiveFromArrivals() with
                        | None -> 
                            // Wait until we have been notified about a message. When that happens, rescan the arrivals
                            let! ok = waitOne(timeout)
                            if ok then return! processFirstArrival()
                            else return None
                        | res -> return res }
            // look in the inbox first
            async { match x.receiveFromInbox() with
                    | None -> return! processFirstArrival()
                    | res -> return res }

        member x.Receive(timeout) =

            let rec processFirstArrival() =
                async { match x.receiveFromArrivals() with
                        | None -> 
                            // Wait until we have been notified about a message. When that happens, rescan the arrivals
                            let! ok = waitOne(timeout)
                            if ok then return! processFirstArrival()
                            else return raise(TimeoutException(SR.GetString(SR.mailboxReceiveTimedOut)))
                        | Some res -> return res }
            // look in the inbox first
            async { match x.receiveFromInbox() with
                    | None -> return! processFirstArrival() 
                    | Some res -> return res }

        interface System.IDisposable with
            member x.Dispose() =
                if pulse <> null then (pulse :> IDisposable).Dispose()

#if DEBUG
        member x.UnsafeContents =
            (x.inbox,arrivals,pulse,savedCont) |> box
#endif


    [<Sealed>]
    [<CompiledName("FSharpAsyncReplyChannel`1")>]
    type AsyncReplyChannel<'Reply>(replyf : 'Reply -> unit) =
        member x.Reply(reply) = replyf(reply)

    [<Sealed>]
    [<AutoSerializable(false)>]
    [<CompiledName("FSharpMailboxProcessor`1")>]
    type MailboxProcessor<'Msg>(initial, ?cancellationToken) =
        let cancellationToken = defaultArg cancellationToken Async.DefaultCancellationToken
        let mailbox = new Mailbox<'Msg>()
        let mutable defaultTimeout = Threading.Timeout.Infinite
        let mutable started = false
        let errorEvent = new Event<System.Exception>()

        member x.CurrentQueueLength = mailbox.CurrentQueueLength // nb. unprotected access gives an approximation of the queue length

        member x.DefaultTimeout 
            with get() = defaultTimeout 
            and set(v) = defaultTimeout <- v

        [<CLIEvent>]
        member x.Error = errorEvent.Publish

#if DEBUG
        member x.UnsafeMessageQueueContents = mailbox.UnsafeContents
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
                                do! initial x 
                            with exn -> 
                                errorEvent.Trigger exn }

                Async.Start(computation=p, cancellationToken=cancellationToken)

        member x.Post(msg) = mailbox.Post(msg)

        member x.TryPostAndReply(msgf : (_ -> 'Msg), ?timeout) : 'Reply option = 
            let timeout = defaultArg timeout defaultTimeout
            use resultCell = new ResultCell<_>()
            let msg = msgf (new AsyncReplyChannel<_>(fun reply ->
                                    // Note the ResultCell may have been disposed if the operation
                                    // timed out. In this case RegisterResult drops the result on the floor.                                                                        
                                    resultCell.RegisterResult(reply,reuseThread=false) |> unfake))
            mailbox.Post(msg)
            resultCell.TryWaitForResultSynchronously(timeout=timeout) 

        member x.PostAndReply(msgf, ?timeout) : 'Reply = 
            match x.TryPostAndReply(msgf,?timeout=timeout) with
            | None ->  raise (TimeoutException(SR.GetString(SR.mailboxProcessorPostAndReplyTimedOut)))
            | Some res -> res

        member x.PostAndTryAsyncReply(msgf, ?timeout) : Async<'Reply option> = 
            let timeout = defaultArg timeout defaultTimeout
            let resultCell = new ResultCell<_>()
            let msg = msgf (new AsyncReplyChannel<_>(fun reply ->
                                    // Note the ResultCell may have been disposed if the operation
                                    // timed out. In this case RegisterResult drops the result on the floor.
                                    resultCell.RegisterResult(reply,reuseThread=false) |> unfake))
            mailbox.Post(msg)
            match timeout with
            |   Threading.Timeout.Infinite -> 
                    async { let! result = resultCell.AwaitResult
                            return Some(result)
                          }  
                        
            |   _ ->
                    async { use _disposeCell = resultCell
                            let! ok =  resultCell.GetWaitHandle().AsyncWaitOne(millisecondsTimeout=timeout)
                            let res = (if ok then Some(resultCell.GrabResult()) else None)
                            return res }
                    
        member x.PostAndAsyncReply(msgf, ?timeout:int) =                 
            let timeout = defaultArg timeout defaultTimeout
            match timeout with
            |   Threading.Timeout.Infinite -> 
                    // Nothing to dispose, no wait handles used
                    let resultCell = new ResultCell<_>()
                    let msg = msgf (new AsyncReplyChannel<_>(fun reply -> resultCell.RegisterResult(reply,reuseThread=false) |> unfake))
                    mailbox.Post(msg)
                    resultCell.AwaitResult
            |   _ ->            
                    let asyncReply = x.PostAndTryAsyncReply(msgf,timeout=timeout) 
                    async { let! res = asyncReply
                            match res with 
                            | None ->  return! raise (TimeoutException(SR.GetString(SR.mailboxProcessorPostAndAsyncReplyTimedOut)))
                            | Some res -> return res
                    }
                           
        member x.Receive(?timeout)    = mailbox.Receive(timeout=defaultArg timeout defaultTimeout)
        member x.TryReceive(?timeout) = mailbox.TryReceive(timeout=defaultArg timeout defaultTimeout)
        member x.Scan(f: 'Msg -> (Async<'T>) option,?timeout)     = mailbox.Scan(f,timeout=defaultArg timeout defaultTimeout)
        member x.TryScan(f: 'Msg -> (Async<'T>) option,?timeout)  = mailbox.TryScan(f,timeout=defaultArg timeout defaultTimeout)

        interface System.IDisposable with
            member x.Dispose() = (mailbox :> IDisposable).Dispose()

        static member Start(initial,?cancellationToken) = 
            let mb = new MailboxProcessor<'Msg>(initial,?cancellationToken=cancellationToken)
            mb.Start();
            mb

 
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module Event =
        [<CompiledName("Create")>]
        let create<'T>() = 
            let ev = new Event<'T>() 
            ev.Trigger, ev.Publish

        [<CompiledName("Map")>]
        let map f (w: IEvent<'Delegate,'T>) =
            let ev = new Event<_>() 
            w.Add(fun x -> ev.Trigger(f x));
            ev.Publish

        [<CompiledName("Filter")>]
        let filter f (w: IEvent<'Delegate,'T>) =
            let ev = new Event<_>() 
            w.Add(fun x -> if f x then ev.Trigger x);
            ev.Publish

        [<CompiledName("Partition")>]
        let partition f (w: IEvent<'Delegate,'T>) =
            let ev1 = new Event<_>() 
            let ev2 = new Event<_>() 
            w.Add(fun x -> if f x then ev1.Trigger x else ev2.Trigger x);
            ev1.Publish,ev2.Publish

        [<CompiledName("Choose")>]
        let choose f (w: IEvent<'Delegate,'T>) =
            let ev = new Event<_>() 
            w.Add(fun x -> match f x with None -> () | Some r -> ev.Trigger r);
            ev.Publish

        [<CompiledName("Scan")>]
        let scan f z (w: IEvent<'Delegate,'T>) =
            let state = ref z
            let ev = new Event<_>() 
            w.Add(fun msg ->
                 let z = !state
                 let z = f z msg
                 state := z; 
                 ev.Trigger(z));
            ev.Publish

        [<CompiledName("Add")>]
        let add f (w: IEvent<'Delegate,'T>) = w.Add(f)

        [<CompiledName("Pairwise")>]
        let pairwise (inp : IEvent<'Delegate,'T>) : IEvent<'T * 'T> = 
            let ev = new Event<'T * 'T>() 
            let lastArgs = ref None
            inp.Add(fun args2 -> 
                (match !lastArgs with 
                 | None -> () 
                 | Some args1 -> ev.Trigger(args1,args2));
                lastArgs := Some args2); 

            ev.Publish

        [<CompiledName("Merge")>]
        let merge (w1: IEvent<'Del1,'T>) (w2: IEvent<'Del2,'T>) =
            let ev = new Event<_>() 
            w1.Add(fun x -> ev.Trigger(x));
            w2.Add(fun x -> ev.Trigger(x));
            ev.Publish

        [<CompiledName("Split")>]
        let split (f : 'T -> Choice<'U1,'U2>) (w: IEvent<'Delegate,'T>) =
            let ev1 = new Event<_>() 
            let ev2 = new Event<_>() 
            w.Add(fun x -> match f x with Choice1Of2 y -> ev1.Trigger(y) | Choice2Of2 z -> ev2.Trigger(z));
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


(*
        type AutoDetachObserver<'T>(o : IObserver<'T>, s : IObservable<System.IDisposable>) =
            inherit BasicObserver<'T>()
            override x.Next v = o.OnNext v
            override x.Error e = o.OnError e
                                 s.Add (fun d -> d.Dispose())
            override x.Completed () = o.OnCompleted ()
                                      s.Add (fun d -> d.Dispose())
                                  
        type MyObservable<'T>() =
          abstract MySubscribe : observer : IObserver<'T> -> System.IDisposable
          interface IObservable<'T>
            member x.Subscribe o = let (t, s) = create<System.IDisposable> ()
                                   let ado = new AutoDetachObserver<'T>(o, s)
                                   let d = x.MySubscribe ado
                                   t d
                                   d
*)

        [<CompiledName("Map")>]
        let map f (w: IObservable<'T>) =
            { new IObservable<'U> with 
                 member x.Subscribe(observer) =
                     w.Subscribe { new BasicObserver<'T>() with  
                                        member x.Next(v) = 
                                            protect (fun () -> f v) observer.OnNext observer.OnError
                                        member x.Error(e) = observer.OnError(e)
                                        member x.Completed() = observer.OnCompleted() } }

        [<CompiledName("Choose")>]
        let choose f (w: IObservable<'T>) =
            { new IObservable<'U> with 
                 member x.Subscribe(observer) =
                     w.Subscribe { new BasicObserver<'T>() with  
                                        member x.Next(v) = 
                                            protect (fun () -> f v) (function None -> () | Some v2 -> observer.OnNext v2) observer.OnError
                                        member x.Error(e) = observer.OnError(e)
                                        member x.Completed() = observer.OnCompleted() } }

        [<CompiledName("Filter")>]
        let filter f (w: IObservable<'T>) =
            choose (fun x -> if f x then Some x else None) w

        [<CompiledName("Partition")>]
        let partition f (w: IObservable<'T>) =
            filter f w, filter (f >> not) w


        [<CompiledName("Scan")>]
        let scan f z (w: IObservable<'T>) =
            { new IObservable<'U> with 
                 member x.Subscribe(observer) =
                     let state = ref z
                     w.Subscribe { new BasicObserver<'T>() with  
                                        member x.Next(v) = 
                                            let z = !state
                                            protect (fun () -> f z v) (fun z -> 
                                                state := z
                                                observer.OnNext z) observer.OnError
                                            
                                        member x.Error(e) = observer.OnError(e)
                                        member x.Completed() = observer.OnCompleted() } }

        [<CompiledName("Add")>]
        let add f (w: IObservable<'T>) = w.Add(f)

        [<CompiledName("Subscribe")>]
        let subscribe (f: 'T -> unit) (w: IObservable<'T>) = w.Subscribe(f)

        [<CompiledName("Pairwise")>]
        let pairwise (w : IObservable<'T>) : IObservable<'T * 'T> = 
            { new IObservable<_> with 
                 member x.Subscribe(observer) =
                     let lastArgs = ref None
                     w.Subscribe { new BasicObserver<'T>() with  
                                        member x.Next(args2) = 
                                            match !lastArgs with 
                                            | None -> ()
                                            | Some args1 -> observer.OnNext (args1,args2)
                                            lastArgs := Some args2
                                        member x.Error(e) = observer.OnError(e)
                                        member x.Completed() = observer.OnCompleted() } }


        [<CompiledName("Merge")>]
        let merge (w1: IObservable<'T>) (w2: IObservable<'T>) =
            { new IObservable<_> with 
                 member x.Subscribe(observer) =
                     let stopped = ref false
                     let completed1 = ref false
                     let completed2 = ref false
                     let h1 = 
                         w1.Subscribe { new IObserver<'T> with  
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
                         w2.Subscribe { new IObserver<'T> with  
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
        let split (f : 'T -> Choice<'U1,'U2>) (w: IObservable<'T>) =
            choose (fun v -> match f v with Choice1Of2 x -> Some x | _ -> None) w,
            choose (fun v -> match f v with Choice2Of2 x -> Some x | _ -> None) w

