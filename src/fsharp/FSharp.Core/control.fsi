// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

#if FX_NO_CANCELLATIONTOKEN_CLASSES
namespace System
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Collections
    
    /// <summary>Represents one or more errors that occur during application execution.</summary>
    [<Class>]
    type AggregateException =
        inherit Exception
        /// <summary>Gets a read-only collection of the <c>Exception</c> instances that caused
        /// the current exception.</summary>
        member InnerExceptions : System.Collections.ObjectModel.ReadOnlyCollection<exn>
    
namespace System.Threading
    open System
    open Microsoft.FSharp.Core
    /// <summary>Represents a registration to a Cancellation token source.</summary>
    type [<Struct>] 
         [<CustomEquality; NoComparison>]
         CancellationTokenRegistration =
            val private source : CancellationTokenSource
            val private id : int64
            /// <summary>Equality comparison against another registration.</summary>
            /// <param name="registration">The target for comparison.</param>
            /// <returns>True if the two registrations are equal.</returns>
            member Equals : registration: CancellationTokenRegistration -> bool
            /// <summary>Equality operator for registrations.</summary>
            /// <param name="registration1">The first input registration.</param>
            /// <param name="registration2">The second input registration.</param>
            /// <returns>True if the two registrations are equal.</returns>
            static member (=) : registration1: CancellationTokenRegistration * registration2: CancellationTokenRegistration -> bool
            /// <summary>Inequality operator for registrations.</summary>
            /// <param name="registration1">The first input registration.</param>
            /// <param name="registration2">The second input registration.</param>
            /// <returns>False if the two registrations are equal.</returns>
            static member (<>) : registration1: CancellationTokenRegistration * registration2: CancellationTokenRegistration -> bool
            /// <summary>Frees resources associated with the registration.</summary>
            member Dispose : unit -> unit
            interface IDisposable
    
    /// <summary>Represents a capability to detect cancellation of an operation.</summary>
    and [<Struct>] 
        [<CustomEquality; NoComparison>]
        CancellationToken =
            val private source : CancellationTokenSource
            /// <summary>Flags whether an operation should be cancelled.</summary>
            member IsCancellationRequested : bool
            /// <summary>Registers an action to perform with the CancellationToken.</summary>
            /// <param name="action">The action to associate with the token.</param>
            /// <param name="state">The state associated with the action.</param>
            /// <returns>The created registration object.</returns>
            member Register : action: Action<obj> * state: obj -> CancellationTokenRegistration            
            /// <summary>Equality comparison against another token.</summary>
            /// <param name="token">The target for comparison.</param>
            /// <returns>True if the two tokens are equal.</returns>
            member Equals : token: CancellationToken -> bool
            /// <summary>Equality operator for tokens.</summary>
            /// <param name="registration1">The first input token.</param>
            /// <param name="registration2">The second input token.</param>
            /// <returns>True if the two tokens are equal.</returns>
            static member (=) : token1: CancellationToken * token2: CancellationToken -> bool
            /// <summary>Inequality operator for tokens.</summary>
            /// <param name="registration1">The first input token.</param>
            /// <param name="registration2">The second input token.</param>
            /// <returns>False if the two tokens are equal.</returns>
            static member (<>) : token1: CancellationToken * token2: CancellationToken -> bool           
        
        
    /// <summary>Signals to a <c>CancellationToken</c> that it should be cancelled.</summary>
    and [<Class>]
        [<Sealed>]
        [<AllowNullLiteral>] 
        CancellationTokenSource =
            /// <summary>Creates a new cancellation capability.</summary>
            new : unit -> CancellationTokenSource
            /// <summary>Fetches the token representing the capability to detect cancellation of an operation.</summary>
            member Token : CancellationToken
            /// <summary>Cancels the operation.</summary>
            member Cancel : unit -> unit
            /// <summary>Creates a cancellation capability linking two tokens.</summary>
            /// <param name="token1">The first input token.</param>
            /// <param name="token2">The second input token.</param>
            /// <returns>The created CancellationTokenSource.</returns>
            static member CreateLinkedTokenSource : token1: CancellationToken * token2: CancellationToken -> CancellationTokenSource
            /// <summary>Discards resources associated with this capability.</summary>
            member Dispose : unit -> unit
            interface IDisposable
#endif            

namespace Microsoft.FSharp.Control

    open System
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections
    open System.Threading
#if FX_NO_TASK
#else
    open System.Runtime.CompilerServices
    open System.Threading.Tasks
#endif

#if FX_NO_OPERATION_CANCELLED

    type OperationCanceledException =
        inherit System.Exception
        new : System.String -> OperationCanceledException
#endif


    /// <summary>A compositional asynchronous computation, which, when run, will eventually produce a value 
    /// of type T, or else raises an exception.</summary> 
    ///
    /// <remarks>Asynchronous computations are normally specified using an F# computation expression.
    ///
    /// When run, asynchronous computations have two modes: as a work item (executing synchronous 
    /// code), or as a wait item (waiting for an event or I/O completion). 
    ///
    /// When run, asynchronous computations can be governed by CancellationToken. This can usually 
    /// be specified when the async computation is started. The associated CancellationTokenSource 
    /// may be used to cancel the asynchronous computation. Asynchronous computations built using 
    /// computation expressions can check the cancellation condition regularly. Synchronous 
    /// computations within an asynchronous computation do not automatically check this condition.</remarks> 
     
    [<Sealed>]
    [<NoEquality; NoComparison>]
    [<CompiledName("FSharpAsync`1")>]
    type Async<'T>

    /// <summary>This static class holds members for creating and manipulating asynchronous computations.</summary>

    [<Sealed>]
    [<CompiledName("FSharpAsync")>]
    type Async =

        /// <summary>Runs the asynchronous computation and await its result.</summary>
        ///
        /// <remarks>If an exception occurs in the asynchronous computation then an exception is re-raised by this
        /// function.
        ///        
        /// If no cancellation token is provided then the default cancellation token is used.
        ///
        /// The timeout parameter is given in milliseconds.  A value of -1 is equivalent to
        /// System.Threading.Timeout.Infinite.</remarks>
        /// <param name="computation">The computation to run.</param>
        /// <param name="timeout">The amount of time in milliseconds to wait for the result of the
        /// computation before raising a <c>System.TimeoutException</c>.  If no value is provided
        /// for timeout then a default of -1 is used to correspond to System.Threading.Timeout.Infinite.</param>
        ////If a cancellable cancellationToken is provided, timeout parameter will be ignored</param>
        /// <param name="cancellationToken">The cancellation token to be associated with the computation.
        /// If one is not supplied, the default cancellation token is used.</param>
        /// <returns>The result of the computation.</returns>
        static member RunSynchronously : computation:Async<'T> * ?timeout : int * ?cancellationToken:CancellationToken-> 'T
        
        /// <summary>Starts the asynchronous computation in the thread pool. Do not await its result.</summary>
        ///
        /// <remarks>If no cancellation token is provided then the default cancellation token is used.</remarks>
        /// <param name="computation">The computation to run asynchronously.</param>
        /// <param name="cancellationToken">The cancellation token to be associated with the computation.
        /// If one is not supplied, the default cancellation token is used.</param>
        static member Start : computation:Async<unit> * ?cancellationToken:CancellationToken -> unit

#if FX_NO_TASK
#else
        /// <summary>Executes a computation in the thread pool.</summary>
        /// <remarks>If no cancellation token is provided then the default cancellation token is used.</remarks>
        /// <returns>A <c>System.Threading.Tasks.Task</c> that will be completed
        /// in the corresponding state once the computation terminates (produces the result, throws exception or gets canceled)</returns>
        ///        
        static member StartAsTask : computation:Async<'T> * ?taskCreationOptions:TaskCreationOptions * ?cancellationToken:CancellationToken -> Task<'T>

        /// <summary>Creates an asynchronous computation which starts the given computation as a <c>System.Threading.Tasks.Task</c></summary>
        static member StartChildAsTask : computation:Async<'T> * ?taskCreationOptions:TaskCreationOptions -> Async<Task<'T>>
#endif
    
        /// <summary>Creates an asynchronous computation that executes <c>computation</c>.
        /// If this computation completes successfully then return <c>Choice1Of2</c> with the returned
        /// value. If this computation raises an exception before it completes then return <c>Choice2Of2</c>
        /// with the raised exception.</summary>
        /// <param name="computation">The input computation that returns the type T.</param>
        /// <returns>A computation that returns a choice of type T or exception.</returns>
        static member Catch : computation:Async<'T> -> Async<Choice<'T,exn>>

        /// <summary>Creates an asynchronous computation that executes <c>computation</c>.
        /// If this computation is cancelled before it completes then the computation generated by 
        /// running <c>compensation</c> is executed.</summary>
        /// <param name="computation">The input asynchronous computation.</param>
        /// <param name="compensation">The function to be run if the computation is cancelled.</param>
        /// <returns>An asynchronous computation that runs the compensation if the input computation
        /// is cancelled.</returns>
        static member TryCancelled : computation:Async<'T> * compensation:(OperationCanceledException -> unit) -> Async<'T>

        /// <summary>Generates a scoped, cooperative cancellation handler for use within an asynchronous workflow.</summary>
        ///
        /// <remarks>For example,
        ///     <c>async { use! holder = Async.OnCancel interruption ... }</c> 
        /// generates an asynchronous computation where, if a cancellation happens any time during 
        /// the execution of the asynchronous computation in the scope of <c>holder</c>, then action 
        /// <c>interruption</c> is executed on the thread that is performing the cancellation. This can 
        /// be used to arrange for a computation to be asynchronously notified that a cancellation 
        /// has occurred, e.g. by setting a flag, or deregistering a pending I/O action.</remarks>
        /// <param name="interruption">The function that is executed on the thread performing the
        /// cancellation.</param>
        /// <returns>An asynchronous computation that triggers the interruption if it is cancelled
        /// before being disposed.</returns>
        static member OnCancel : interruption: (unit -> unit) -> Async<System.IDisposable>
        
        /// <summary>Creates an asynchronous computation that returns the CancellationToken governing the execution 
        /// of the computation.</summary>
        /// <remarks>In <c>async { let! token = Async.CancellationToken ...}</c> token can be used to initiate other 
        /// asynchronous operations that will cancel cooperatively with this workflow.</remarks>
        /// <returns>An asynchronous computation capable of retrieving the CancellationToken from a computation
        /// expression.</returns>
        static member CancellationToken : Async<CancellationToken>

        /// <summary>Raises the cancellation condition for the most recent set of asynchronous computations started 
        /// without any specific CancellationToken. Replaces the global CancellationTokenSource with a new 
        /// global token source for any asynchronous computations created after this point without any 
        /// specific CancellationToken.</summary>
        static member CancelDefaultToken :  unit -> unit 

        /// <summary>Gets the default cancellation token for executing asynchronous computations.</summary>
        /// <returns>The default CancellationToken.</returns>
        static member DefaultCancellationToken : CancellationToken

        //---------- Parallelism

        /// <summary>Starts a child computation within an asynchronous workflow. 
        /// This allows multiple asynchronous computations to be executed simultaneously.</summary>
        /// 
        /// <remarks>This method should normally be used as the immediate 
        /// right-hand-side of a <c>let!</c> binding in an F# asynchronous workflow, that is,
        /// 
        ///        async { ...
        ///                let! completor1 = childComputation1 |> Async.StartChild  
        ///                let! completor2 = childComputation2 |> Async.StartChild  
        ///                ... 
        ///                let! result1 = completor1 
        ///                let! result2 = completor2 
        ///                ... }
        /// 
        /// When used in this way, each use of <c>StartChild</c> starts an instance of <c>childComputation</c> 
        /// and returns a completor object representing a computation to wait for the completion of the operation.
        /// When executed, the completor awaits the completion of <c>childComputation</c>.</remarks>
        /// <param name="computation">The child computation.</param>
        /// <param name="millisecondsTimeout">The timeout value in milliseconds.  If one is not provided
        /// then the default value of -1 corresponding to <c>System.Threading.Timeout.Infinite</c>.</param>
        /// <returns>A new computation that waits for the input computation to finish.</returns>
        static member StartChild : computation:Async<'T> * ?millisecondsTimeout : int -> Async<Async<'T>>
                
        /// <summary>Creates an asynchronous computation that executes all the given asynchronous computations, 
        /// initially queueing each as work items and using a fork/join pattern.</summary>
        ///
        /// <remarks>If all child computations succeed, an array of results is passed to the success continuation.
        /// 
        /// If any child computation raises an exception, then the overall computation will trigger an 
        /// exception, and cancel the others. 
        ///
        /// The overall computation will respond to cancellation while executing the child computations.
        /// If cancelled, the computation will cancel any remaining child computations but will still wait
        /// for the other child computations to complete.</remarks>
        /// <param name="computations">A sequence of distinct computations to be parallelized.</param>
        /// <returns>A computation that returns an array of values from the sequence of input computations.</returns>
        static member Parallel : computations:seq<Async<'T>> -> Async<'T[]>

        /// <summary>Creates an asynchronous computation that executes all given asynchronous computations in parallel, 
        /// returning the result of the first succeeding computation (one whose result is 'Some x').
        /// If all child computations complete with None, the parent computation also returns None.</summary>
        ///
        /// <remarks>
        /// If any child computation raises an exception, then the overall computation will trigger an 
        /// exception, and cancel the others. 
        ///
        /// The overall computation will respond to cancellation while executing the child computations.
        /// If cancelled, the computation will cancel any remaining child computations but will still wait
        /// for the other child computations to complete.</remarks>
        /// <param name="computations">A sequence of computations to be parallelized.</param>
        /// <returns>A computation that returns the first succeeding computation.</returns>
        static member Choice : computations:seq<Async<'T option>> -> Async<'T option>

        //---------- Thread Control
        
        /// <summary>Creates an asynchronous computation that creates a new thread and runs
        /// its continuation in that thread.</summary>
        /// <returns>A computation that will execute on a new thread.</returns>
        static member SwitchToNewThread : unit -> Async<unit> 
        
        /// <summary>Creates an asynchronous computation that queues a work item that runs
        /// its continuation.</summary>
        /// <returns>A computation that generates a new work item in the thread pool.</returns>
        static member SwitchToThreadPool :  unit -> Async<unit> 

#if FX_NO_SYNC_CONTEXT
#else
        /// <summary>Creates an asynchronous computation that runs
        /// its continuation using syncContext.Post. If syncContext is null 
        /// then the asynchronous computation is equivalent to SwitchToThreadPool().</summary>
        /// <param name="syncContext">The synchronization context to accept the posted computation.</param>
        /// <returns>An asynchronous computation that uses the syncContext context to execute.</returns>
        static member SwitchToContext :  syncContext:System.Threading.SynchronizationContext -> Async<unit> 
#endif

                    
        /// <summary>Creates an asynchronous computation that captures the current
        /// success, exception and cancellation continuations. The callback must 
        /// eventually call exactly one of the given continuations.</summary>
        /// <param name="callback">The function that accepts the current success, exception, and cancellation
        /// continuations.</param>
        /// <returns>An asynchronous computation that provides the callback with the current continuations.</returns>
        static member FromContinuations : callback:(('T -> unit) * (exn -> unit) * (OperationCanceledException -> unit) -> unit) -> Async<'T>

#if FX_NO_CREATE_DELEGATE
#else
        /// <summary>Creates an asynchronous computation that waits for a single invocation of a CLI 
        /// event by adding a handler to the event. Once the computation completes or is 
        /// cancelled, the handler is removed from the event.</summary>
        ///
        /// <remarks>The computation will respond to cancellation while waiting for the event. If a 
        /// cancellation occurs, and <c>cancelAction</c> is specified, then it is executed, and 
        /// the computation continues to wait for the event.
        /// 
        /// If <c>cancelAction</c> is not specified, then cancellation causes the computation
        /// to cancel immediately.</remarks>
        /// <param name="event">The event to handle once.</param>
        /// <param name="cancelAction">An optional function to execute instead of cancelling when a
        /// cancellation is issued.</param>
        /// <returns>An asynchronous computation that waits for the event to be invoked.</returns>
        static member AwaitEvent: event:IEvent<'Del,'T> * ?cancelAction : (unit -> unit) -> Async<'T> when 'Del : delegate<'T,unit> and 'Del :> System.Delegate 
#endif

        /// <summary>Creates an asynchronous computation that will wait on the given WaitHandle.</summary>
        ///
        /// <remarks>The computation returns true if the handle indicated a result within the given timeout.</remarks>
        /// <param name="waitHandle">The <c>WaitHandle</c> that can be signalled.</param>
        /// <param name="millisecondsTimeout">The timeout value in milliseconds.  If one is not provided
        /// then the default value of -1 corresponding to <c>System.Threading.Timeout.Infinite</c>.</param>
        /// <returns>An asynchronous computation that waits on the given <c>WaitHandle</c>.</returns>
        static member AwaitWaitHandle: waitHandle: WaitHandle * ?millisecondsTimeout:int -> Async<bool>

        /// <summary>Creates an asynchronous computation that will wait on the IAsyncResult.</summary>
        ///
        /// <remarks>The computation returns true if the handle indicated a result within the given timeout.</remarks>
        /// <param name="iar">The IAsyncResult to wait on.</param>
        /// <param name="millisecondsTimeout">The timeout value in milliseconds.  If one is not provided
        /// then the default value of -1 corresponding to <c>System.Threading.Timeout.Infinite</c>.</param>
        /// <returns>An asynchronous computation that waits on the given <c>IAsyncResult</c>.</returns>
        static member AwaitIAsyncResult: iar: System.IAsyncResult * ?millisecondsTimeout:int -> Async<bool>

#if FX_NO_TASK
#else
        /// Return an asynchronous computation that will wait for the given task to complete and return
        /// its result.
        static member AwaitTask: task: Task<'T> -> Async<'T>
        /// Return an asynchronous computation that will wait for the given task to complete and return
        /// its result.
        static member AwaitTask: task: Task -> Async<unit>
#endif            

        /// <summary>Creates an asynchronous computation that will sleep for the given time. This is scheduled
        /// using a System.Threading.Timer object. The operation will not block operating system threads
        /// for the duration of the wait.</summary>
        /// <param name="millisecondsDueTime">The number of milliseconds to sleep.</param>
        /// <returns>An asynchronous computation that will sleep for the given time.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the due time is negative
        /// and not infinite.</exception>
        static member Sleep: millisecondsDueTime:int -> Async<unit>

        /// <summary>Creates an asynchronous computation in terms of a Begin/End pair of actions in 
        /// the style used in CLI APIs. For example, 
        ///     <c>Async.FromBeginEnd(ws.BeginGetWeather,ws.EndGetWeather)</c>
        /// When the computation is run, <c>beginFunc</c> is executed, with
        /// a callback which represents the continuation of the computation. 
        /// When the callback is invoked, the overall result is fetched using <c>endFunc</c>.</summary>
        ///
        /// <remarks>The computation will respond to cancellation while waiting for the completion
        /// of the operation. If a cancellation occurs, and <c>cancelAction</c> is specified, then it is 
        /// executed, and the computation continues to wait for the completion of the operation.
        /// 
        /// If <c>cancelAction</c> is not specified, then cancellation causes the computation
        /// to stop immediately, and subsequent invocations of the callback are ignored.</remarks>
        /// <param name="beginAction">The function initiating a traditional CLI asynchronous operation.</param>
        /// <param name="endAction">The function completing a traditional CLI asynchronous operation.</param>
        /// <param name="cancelAction">An optional function to be executed when a cancellation is requested.</param>
        /// <returns>An asynchronous computation wrapping the given Begin/End functions.</returns>
        static member FromBeginEnd : beginAction:(System.AsyncCallback * obj -> System.IAsyncResult) * endAction:(System.IAsyncResult -> 'T) * ?cancelAction : (unit -> unit) -> Async<'T>

        /// <summary>Creates an asynchronous computation in terms of a Begin/End pair of actions in 
        /// the style used in CLI APIs. This overload should be used if the operation is 
        /// qualified by one argument. For example, 
        ///     <c>Async.FromBeginEnd(place,ws.BeginGetWeather,ws.EndGetWeather)</c>
        /// When the computation is run, <c>beginFunc</c> is executed, with
        /// a callback which represents the continuation of the computation. 
        /// When the callback is invoked, the overall result is fetched using <c>endFunc</c>.</summary>
        ///
        /// <remarks>The computation will respond to cancellation while waiting for the completion
        /// of the operation. If a cancellation occurs, and <c>cancelAction</c> is specified, then it is 
        /// executed, and the computation continues to wait for the completion of the operation.
        /// 
        /// If <c>cancelAction</c> is not specified, then cancellation causes the computation
        /// to stop immediately, and subsequent invocations of the callback are ignored.</remarks>
        /// <param name="arg">The argument for the operation.</param>
        /// <param name="beginAction">The function initiating a traditional CLI asynchronous operation.</param>
        /// <param name="endAction">The function completing a traditional CLI asynchronous operation.</param>
        /// <param name="cancelAction">An optional function to be executed when a cancellation is requested.</param>
        /// <returns>An asynchronous computation wrapping the given Begin/End functions.</returns>
        static member FromBeginEnd : arg:'Arg1 * beginAction:('Arg1 * System.AsyncCallback * obj -> System.IAsyncResult) * endAction:(System.IAsyncResult -> 'T) * ?cancelAction : (unit -> unit) -> Async<'T>

        /// <summary>Creates an asynchronous computation in terms of a Begin/End pair of actions in 
        /// the style used in CLI APIs. This overload should be used if the operation is 
        /// qualified by two arguments. For example, 
        ///     <c>Async.FromBeginEnd(arg1,arg2,ws.BeginGetWeather,ws.EndGetWeather)</c>
        /// When the computation is run, <c>beginFunc</c> is executed, with
        /// a callback which represents the continuation of the computation. 
        /// When the callback is invoked, the overall result is fetched using <c>endFunc</c>.</summary>
        ///
        /// <remarks>The computation will respond to cancellation while waiting for the completion
        /// of the operation. If a cancellation occurs, and <c>cancelAction</c> is specified, then it is 
        /// executed, and the computation continues to wait for the completion of the operation.
        /// 
        /// If <c>cancelAction</c> is not specified, then cancellation causes the computation
        /// to stop immediately, and subsequent invocations of the callback are ignored.</remarks>
        /// <param name="arg1">The first argument for the operation.</param>
        /// <param name="arg2">The second argument for the operation.</param>
        /// <param name="beginAction">The function initiating a traditional CLI asynchronous operation.</param>
        /// <param name="endAction">The function completing a traditional CLI asynchronous operation.</param>
        /// <param name="cancelAction">An optional function to be executed when a cancellation is requested.</param>
        /// <returns>An asynchronous computation wrapping the given Begin/End functions.</returns>
        static member FromBeginEnd : arg1:'Arg1 * arg2:'Arg2 * beginAction:('Arg1 * 'Arg2 * System.AsyncCallback * obj -> System.IAsyncResult) * endAction:(System.IAsyncResult -> 'T) * ?cancelAction : (unit -> unit) -> Async<'T>

        /// <summary>Creates an asynchronous computation in terms of a Begin/End pair of actions in 
        /// the style used in CLI APIs. This overload should be used if the operation is 
        /// qualified by three arguments. For example, 
        ///     <c>Async.FromBeginEnd(arg1,arg2,arg3,ws.BeginGetWeather,ws.EndGetWeather)</c>
        /// When the computation is run, <c>beginFunc</c> is executed, with
        /// a callback which represents the continuation of the computation. 
        /// When the callback is invoked, the overall result is fetched using <c>endFunc</c>.</summary>
        ///
        /// <remarks>The computation will respond to cancellation while waiting for the completion
        /// of the operation. If a cancellation occurs, and <c>cancelAction</c> is specified, then it is 
        /// executed, and the computation continues to wait for the completion of the operation.
        /// 
        /// If <c>cancelAction</c> is not specified, then cancellation causes the computation
        /// to stop immediately, and subsequent invocations of the callback are ignored.</remarks>
        /// <param name="arg1">The first argument for the operation.</param>
        /// <param name="arg2">The second argument for the operation.</param>
        /// <param name="arg3">The third argument for the operation.</param>
        /// <param name="beginAction">The function initiating a traditional CLI asynchronous operation.</param>
        /// <param name="endAction">The function completing a traditional CLI asynchronous operation.</param>
        /// <param name="cancelAction">An optional function to be executed when a cancellation is requested.</param>
        /// <returns>An asynchronous computation wrapping the given Begin/End functions.</returns>
        static member FromBeginEnd : arg1:'Arg1 * arg2:'Arg2 * arg3:'Arg3 * beginAction:('Arg1 * 'Arg2 * 'Arg3 * System.AsyncCallback * obj -> System.IAsyncResult) * endAction:(System.IAsyncResult -> 'T) * ?cancelAction : (unit -> unit) -> Async<'T>

        /// <summary>Creates three functions that can be used to implement the .NET Asynchronous 
        /// Programming Model (APM) for a given asynchronous computation.</summary>
        /// 
        /// <remarks>The functions should normally be published as members with prefix <c>Begin</c>,
        /// <c>End</c> and <c>Cancel</c>, and can be used within a type definition as follows:
        /// <c>
        ///   let beginAction,endAction,cancelAction = Async.AsBeginEnd (fun arg -&gt; computation)
        ///   member x.BeginSomeOperation(arg,callback,state:obj) = beginAction(arg,callback,state)
        ///   member x.EndSomeOperation(iar) = endAction(iar)
        ///   member x.CancelSomeOperation(iar) = cancelAction(iar)
        /// </c>
        ///
        /// If the asynchronous computation takes no arguments, then AsBeginEnd is used as follows:
        /// <c>
        ///   let beginAction,endAction,cancelAction = Async.AsBeginEnd (fun () -&gt; computation)
        ///   member x.BeginSomeOperation(callback,state:obj) = beginAction((),callback,state)
        ///   member x.EndSomeOperation(iar) = endAction(iar)
        ///   member x.CancelSomeOperation(iar) = cancelAction(iar)
        /// </c>
        ///
        ///
        /// If the asynchronous computation takes two arguments, then AsBeginEnd is used as follows:
        /// <c>
        ///   let beginAction,endAction,cancelAction = Async.AsBeginEnd (fun arg1 arg2 -&gt; computation)
        ///   member x.BeginSomeOperation(arg1,arg2,callback,state:obj) = beginAction((),callback,state)
        ///   member x.EndSomeOperation(iar) = endAction(iar)
        ///   member x.CancelSomeOperation(iar) = cancelAction(iar)
        /// </c>
        ///
        /// In each case, the resulting API will be familiar to programmers in other CLI languages and 
        /// is a useful way to publish asynchronous computations in CLI components.</remarks>
        /// <param name="computation">A function generating the asynchronous computation to split into the traditional
        /// .NET Asynchronous Programming Model.</param>
        /// <returns>A tuple of the begin, end, and cancel members.</returns>
        static member AsBeginEnd : computation:('Arg -> Async<'T>) -> 
                                     // The 'Begin' member
                                     ('Arg * System.AsyncCallback * obj -> System.IAsyncResult) * 
                                     // The 'End' member
                                     (System.IAsyncResult -> 'T) * 
                                     // The 'Cancel' member
                                     (System.IAsyncResult -> unit)

        /// <summary>Creates an asynchronous computation that runs the given computation and ignores 
        /// its result.</summary>
        /// <param name="computation">The input computation.</param>
        /// <returns>A computation that is equivalent to the input computation, but disregards the result.</returns>
        static member Ignore : computation: Async<'T> -> Async<unit>

        /// <summary>Runs an asynchronous computation, starting immediately on the current operating system
        /// thread. Call one of the three continuations when the operation completes.</summary>
        /// <remarks>If no cancellation token is provided then the default cancellation token
        /// is used.</remarks>
        /// <param name="computation">The asynchronous computation to execute.</param>
        /// <param name="continuation">The function called on success.</param>
        /// <param name="exceptionContinuation">The function called on exception.</param>
        /// <param name="cancellationContinuation">The function called on cancellation.</param>
        /// <param name="cancellationToken">The <c>CancellationToken</c> to associate with the computation.
        /// The default is used if this parameter is not provided.</param>
        static member StartWithContinuations: 
            computation:Async<'T> * 
            continuation:('T -> unit) * exceptionContinuation:(exn -> unit) * cancellationContinuation:(OperationCanceledException -> unit) *  
            ?cancellationToken:CancellationToken-> unit

        /// <summary>Runs an asynchronous computation, starting immediately on the current operating system
        /// thread.</summary>
        /// <remarks>If no cancellation token is provided then the default cancellation token is used.</remarks>
        /// <param name="computation">The asynchronous computation to execute.</param>
        /// <param name="cancellationToken">The <c>CancellationToken</c> to associate with the computation.
        /// The default is used if this parameter is not provided.</param>
        static member StartImmediate: 
            computation:Async<unit> * ?cancellationToken:CancellationToken-> unit



    [<CompiledName("FSharpAsyncBuilder")>]
    [<Sealed>]
    /// <summary>The type of the <c>async</c> operator, used to build workflows for asynchronous computations.</summary>
    type AsyncBuilder =
        /// <summary>Creates an asynchronous computation that enumerates the sequence <c>seq</c>
        /// on demand and runs <c>body</c> for each element.</summary>
        ///
        /// <remarks>A cancellation check is performed on each iteration of the loop.
        ///
        /// The existence of this method permits the use of <c>for</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        /// <param name="sequence">The sequence to enumerate.</param>
        /// <param name="body">A function to take an item from the sequence and create
        /// an asynchronous computation.  Can be seen as the body of the <c>for</c> expression.</param>
        /// <returns>An asynchronous computation that will enumerate the sequence and run <c>body</c>
        /// for each element.</returns>
        member For: sequence:seq<'T> * body:('T -> Async<unit>) -> Async<unit>

        /// <summary>Creates an asynchronous computation that just returns <c>()</c>.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of empty <c>else</c> branches in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        /// <returns>An asynchronous computation that returns <c>()</c>.</returns>
        member Zero : unit -> Async<unit> 

        /// <summary>Creates an asynchronous computation that first runs <c>computation1</c>
        /// and then runs <c>computation2</c>, returning the result of <c>computation2</c>.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of expression sequencing in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        /// <param name="computation1">The first part of the sequenced computation.</param>
        /// <param name="computation2">The second part of the sequenced computation.</param>
        /// <returns>An asynchronous computation that runs both of the computations sequentially.</returns>
        member Combine : computation1:Async<unit> * computation2:Async<'T> -> Async<'T>

        /// <summary>Creates an asynchronous computation that runs <c>computation</c> repeatedly 
        /// until <c>guard()</c> becomes false.</summary>
        ///
        /// <remarks>A cancellation check is performed whenever the computation is executed.
        ///
        /// The existence of this method permits the use of <c>while</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        /// <param name="guard">The function to determine when to stop executing <c>computation</c>.</param>
        /// <param name="computation">The function to be executed.  Equivalent to the body
        /// of a <c>while</c> expression.</param>
        /// <returns>An asynchronous computation that behaves similarly to a while loop when run.</returns>
        member While : guard:(unit -> bool) * computation:Async<unit> -> Async<unit>

        /// <summary>Creates an asynchronous computation that returns the result <c>v</c>.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of <c>return</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        /// <param name="value">The value to return from the computation.</param>
        /// <returns>An asynchronous computation that returns <c>value</c> when executed.</returns>
        member Return : value:'T -> Async<'T>

        /// <summary>Delegates to the input computation.</summary>
        ///
        /// <remarks>The existence of this method permits the use of <c>return!</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        /// <param name="computation">The input computation.</param>
        /// <returns>The input computation.</returns>
        member ReturnFrom : computation:Async<'T> -> Async<'T>

        /// <summary>Creates an asynchronous computation that runs <c>generator</c>.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.</remarks>
        /// <param name="generator">The function to run.</param>
        /// <returns>An asynchronous computation that runs <c>generator</c>.</returns>
        member Delay : generator:(unit -> Async<'T>) -> Async<'T>

        /// <summary>Creates an asynchronous computation that runs <c>binder(resource)</c>. 
        /// The action <c>resource.Dispose()</c> is executed as this computation yields its result
        /// or if the asynchronous computation exits by an exception or by cancellation.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of <c>use</c> and <c>use!</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        /// <param name="resource">The resource to be used and disposed.</param>
        /// <param name="binder">The function that takes the resource and returns an asynchronous
        /// computation.</param>
        /// <returns>An asynchronous computation that binds and eventually disposes <c>resource</c>.</returns>
        member Using: resource:'T * binder:('T -> Async<'U>) -> Async<'U> when 'T :> System.IDisposable

        /// <summary>Creates an asynchronous computation that runs <c>computation</c>, and when 
        /// <c>computation</c> generates a result <c>T</c>, runs <c>binder res</c>.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of <c>let!</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        /// <param name="computation">The computation to provide an unbound result.</param>
        /// <param name="binder">The function to bind the result of <c>computation</c>.</param>
        /// <returns>An asynchronous computation that performs a monadic bind on the result
        /// of <c>computation</c>.</returns>
        member Bind: computation: Async<'T> * binder: ('T -> Async<'U>) -> Async<'U>
        
        /// <summary>Creates an asynchronous computation that runs <c>computation</c>. The action <c>compensation</c> is executed 
        /// after <c>computation</c> completes, whether <c>computation</c> exits normally or by an exception. If <c>compensation</c> raises an exception itself
        /// the original exception is discarded and the new exception becomes the overall result of the computation.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of <c>try/finally</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        /// <param name="computation">The input computation.</param>
        /// <param name="compensation">The action to be run after <c>computation</c> completes or raises an
        /// exception (including cancellation).</param>
        /// <returns>An asynchronous computation that executes computation and compensation afterwards or
        /// when an exception is raised.</returns>
        member TryFinally : computation:Async<'T> * compensation:(unit -> unit) -> Async<'T>

        /// <summary>Creates an asynchronous computation that runs <c>computation</c> and returns its result.
        /// If an exception happens then <c>catchHandler(exn)</c> is called and the resulting computation executed instead.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of <c>try/with</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        /// <param name="computation">The input computation.</param>
        /// <param name="catchHandler">The function to run when <c>computation</c> throws an exception.</param>
        /// <returns>An asynchronous computation that executes <c>computation</c> and calls <c>catchHandler</c> if an
        /// exception is thrown.</returns>
        member TryWith : computation:Async<'T> * catchHandler:(exn -> Async<'T>) -> Async<'T>

        /// Generate an object used to build asynchronous computations using F# computation expressions. The value
        /// 'async' is a pre-defined instance of this type.
        ///
        /// A cancellation check is performed when the computation is executed.
        internal new : unit -> AsyncBuilder

    [<AutoOpen>]
    /// <summary>A module of extension members providing asynchronous operations for some basic CLI types related to concurrency and I/O.</summary>
    module CommonExtensions =
        open System.IO
        
        type System.IO.Stream with 
            
            /// <summary>Returns an asynchronous computation that will read from the stream into the given buffer.</summary>
            /// <param name="buffer">The buffer to read into.</param>
            /// <param name="offset">An optional offset as a number of bytes in the stream.</param>
            /// <param name="count">An optional number of bytes to read from the stream.</param>
            /// <returns>An asynchronous computation that will read from the stream into the given buffer.</returns>
            /// <exception cref="System.ArgumentException">Thrown when the sum of offset and count is longer than
            /// the buffer length.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown when offset or count is negative.</exception>
            [<CompiledName("AsyncRead")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member AsyncRead : buffer:byte[] * ?offset:int * ?count:int -> Async<int>
            
            /// <summary>Returns an asynchronous computation that will read the given number of bytes from the stream.</summary>
            /// <param name="count">The number of bytes to read.</param>
            /// <returns>An asynchronous computation that returns the read byte[] when run.</returns> 
            [<CompiledName("AsyncReadBytes")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member AsyncRead : count:int -> Async<byte[]>
            
            /// <summary>Returns an asynchronous computation that will write the given bytes to the stream.</summary>
            /// <param name="buffer">The buffer to write from.</param>
            /// <param name="offset">An optional offset as a number of bytes in the stream.</param>
            /// <param name="count">An optional number of bytes to write to the stream.</param>
            /// <returns>An asynchronous computation that will write the given bytes to the stream.</returns>
            /// <exception cref="System.ArgumentException">Thrown when the sum of offset and count is longer than
            /// the buffer length.</exception>
            /// <exception cref="System.ArgumentOutOfRangeException">Thrown when offset or count is negative.</exception>
            [<CompiledName("AsyncWrite")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member AsyncWrite : buffer:byte[] * ?offset:int * ?count:int -> Async<unit>


        ///<summary>The family of first class event values for delegate types that satisfy the F# delegate constraint.</summary>
        type IObservable<'T> with
            /// <summary>Permanently connects a listener function to the observable. The listener will
            /// be invoked for each observation.</summary>
            /// <param name="callback">The function to be called for each observation.</param>
            [<CompiledName("AddToObservable")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member Add: callback:('T -> unit) -> unit

            /// <summary>Connects a listener function to the observable. The listener will
            /// be invoked for each observation. The listener can be removed by
            /// calling Dispose on the returned IDisposable object.</summary>
            /// <param name="callback">The function to be called for each observation.</param>
            /// <returns>An object that will remove the listener if disposed.</returns>
            [<CompiledName("SubscribeToObservable")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member Subscribe: callback:('T -> unit) -> System.IDisposable

    /// <summary>A module of extension members providing asynchronous operations for some basic Web operations.</summary>
    [<AutoOpen>]
    module WebExtensions = 
     begin

#if FX_NO_WEB_REQUESTS
#else
        type System.Net.WebRequest with 
            /// <summary>Returns an asynchronous computation that, when run, will wait for a response to the given WebRequest.</summary>
            /// <returns>An asynchronous computation that waits for response to the <c>WebRequest</c>.</returns>
            [<CompiledName("AsyncGetResponse")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member AsyncGetResponse : unit -> Async<System.Net.WebResponse>
#endif
    
#if FX_NO_WEB_CLIENT
#else
        type System.Net.WebClient with
            /// <summary>Returns an asynchronous computation that, when run, will wait for the download of the given URI.</summary>
            /// <param name="address">The URI to retrieve.</param>
            /// <returns>An asynchronous computation that will wait for the download of the URI.</returns>
            [<CompiledName("AsyncDownloadString")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member AsyncDownloadString : address:System.Uri -> Async<string>
            /// <summary>Returns an asynchronous computation that, when run, will wait for the download of the given URI.</summary>
            /// <param name="address">The URI to retrieve.</param>
            /// <returns>An asynchronous computation that will wait for the download of the URI.</returns>
            [<CompiledName("AsyncDownloadData")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member AsyncDownloadData : address:System.Uri -> Async<byte[]>
            /// <summary>Returns an asynchronous computation that, when run, will wait for the download of the given URI to specified file.</summary>
            /// <param name="address">The URI to retrieve.</param>
            /// <param name="fileName">The filename to save download to.</param>
            /// <returns>An asynchronous computation that will wait for the download of the URI to specified file.</returns>
            [<CompiledName("AsyncDownloadFile")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member AsyncDownloadFile : address:System.Uri * fileName: string -> Async<unit>
#endif

     end
    
    
    [<Sealed>]
    [<CompiledName("FSharpAsyncReplyChannel`1")>]
    /// <summary>A handle to a capability to reply to a PostAndReply message.</summary>
    type AsyncReplyChannel<'Reply> =
        /// <summary>Sends a reply to a PostAndReply message.</summary>
        /// <param name="value">The value to send.</param>
        member Reply : value:'Reply -> unit

        
    /// <summary>A message-processing agent which executes an asynchronous computation.</summary>
    ///
    /// <remarks>The agent encapsulates a message queue that supports multiple-writers and 
    /// a single reader agent. Writers send messages to the agent by using the Post 
    /// method and its variations.
    ///
    /// The agent may wait for messages using the Receive or TryReceive methods or
    /// scan through all available messages using the Scan or TryScan method.</remarks>

    [<Sealed>]
    [<AutoSerializable(false)>]    
    [<CompiledName("FSharpMailboxProcessor`1")>]
    type MailboxProcessor<'Msg> =

        /// <summary>Creates an agent. The <c>body</c> function is used to generate the asynchronous 
        /// computation executed by the agent. This function is not executed until 
        /// <c>Start</c> is called.</summary>
        /// <param name="body">The function to produce an asynchronous computation that will be executed
        /// as the read loop for the MailboxProcessor when Start is called.</param>
        /// <param name="cancellationToken">An optional cancellation token for the <c>body</c>.
        /// Defaults to <c>Async.DefaultCancellationToken</c>.</param>
        /// <returns>The created MailboxProcessor.</returns>
        new :  body:(MailboxProcessor<'Msg> -> Async<unit>) * ?cancellationToken: CancellationToken -> MailboxProcessor<'Msg>

        /// <summary>Creates and starts an agent. The <c>body</c> function is used to generate the asynchronous 
        /// computation executed by the agent.</summary>
        /// <param name="body">The function to produce an asynchronous computation that will be executed
        /// as the read loop for the MailboxProcessor when Start is called.</param>
        /// <param name="cancellationToken">An optional cancellation token for the <c>body</c>.
        /// Defaults to <c>Async.DefaultCancellationToken</c>.</param>
        /// <returns>The created MailboxProcessor.</returns>
        static member Start  :  body:(MailboxProcessor<'Msg> -> Async<unit>) * ?cancellationToken: CancellationToken -> MailboxProcessor<'Msg>

        /// <summary>Posts a message to the message queue of the MailboxProcessor, asynchronously.</summary>
        /// <param name="message">The message to post.</param>
        member Post : message:'Msg -> unit

        /// <summary>Posts a message to an agent and await a reply on the channel, synchronously.</summary>
        ///
        /// <remarks>The message is generated by applying <c>buildMessage</c> to a new reply channel 
        /// to be incorporated into the message. The receiving agent must process this 
        /// message and invoke the Reply method on this reply channel precisely once.</remarks>
        /// <param name="buildMessage">The function to incorporate the AsyncReplyChannel into
        /// the message to be sent.</param>
        /// <param name="timeout">An optional timeout parameter (in milliseconds) to wait for a reply message.
        /// Defaults to -1 which corresponds to <c>System.Threading.Timeout.Infinite</c>.</param>
        /// <returns>The reply from the agent.</returns>
        member PostAndReply : buildMessage:(AsyncReplyChannel<'Reply> -> 'Msg) * ?timeout : int -> 'Reply

        /// <summary>Posts a message to an agent and await a reply on the channel, asynchronously.</summary> 
        ///
        /// <remarks>The message is generated by applying <c>buildMessage</c> to a new reply channel 
        /// to be incorporated into the message. The receiving agent must process this 
        /// message and invoke the Reply method on this reply channel precisely once.</remarks>
        /// <param name="buildMessage">The function to incorporate the AsyncReplyChannel into
        /// the message to be sent.</param>
        /// <param name="timeout">An optional timeout parameter (in milliseconds) to wait for a reply message.
        /// Defaults to -1 which corresponds to <c>System.Threading.Timeout.Infinite</c>.</param>
        /// <returns>An asynchronous computation that will wait for the reply from the agent.</returns>
        member PostAndAsyncReply : buildMessage:(AsyncReplyChannel<'Reply> -> 'Msg) * ?timeout : int -> Async<'Reply>

        /// <summary>Like PostAndReply, but returns None if no reply within the timeout period.</summary>
        /// <param name="buildMessage">The function to incorporate the AsyncReplyChannel into
        /// the message to be sent.</param>
        /// <param name="timeout">An optional timeout parameter (in milliseconds) to wait for a reply message.
        /// Defaults to -1 which corresponds to <c>System.Threading.Timeout.Infinite</c>.</param>
        /// <returns>The reply from the agent or None if the timeout expires.</returns> 
        member TryPostAndReply : buildMessage:(AsyncReplyChannel<'Reply> -> 'Msg) * ?timeout : int -> 'Reply option

        /// <summary>Like AsyncPostAndReply, but returns None if no reply within the timeout period.</summary>
        /// <param name="buildMessage">The function to incorporate the AsyncReplyChannel into
        /// the message to be sent.</param>
        /// <param name="timeout">An optional timeout parameter (in milliseconds) to wait for a reply message.
        /// Defaults to -1 which corresponds to <c>System.Threading.Timeout.Infinite</c>.</param>
        /// <returns>An asynchronous computation that will return the reply or None if the timeout expires.</returns> 
        member PostAndTryAsyncReply : buildMessage:(AsyncReplyChannel<'Reply> -> 'Msg) * ?timeout : int -> Async<'Reply option>

        /// <summary>Waits for a message. This will consume the first message in arrival order.</summary> 
        ///
        /// <remarks>This method is for use within the body of the agent. 
        ///
        /// This method is for use within the body of the agent. For each agent, at most 
        /// one concurrent reader may be active, so no more than one concurrent call to 
        /// Receive, TryReceive, Scan and/or TryScan may be active.</remarks>
        /// <param name="timeout">An optional timeout in milliseconds. Defaults to -1 which corresponds
        /// to <c>System.Threading.Timeout.Infinite</c>.</param>
        /// <returns>An asynchronous computation that returns the received message.</returns>
        /// <exception cref="System.TimeoutException">Thrown when the timeout is exceeded.</exception>
        member Receive : ?timeout:int -> Async<'Msg>

        /// <summary>Waits for a message. This will consume the first message in arrival order.</summary> 
        ///
        /// <remarks>This method is for use within the body of the agent. 
        ///
        /// Returns None if a timeout is given and the timeout is exceeded.
        ///
        /// This method is for use within the body of the agent. For each agent, at most 
        /// one concurrent reader may be active, so no more than one concurrent call to 
        /// Receive, TryReceive, Scan and/or TryScan may be active.</remarks>
        /// <param name="timeout">An optional timeout in milliseconds. Defaults to -1 which
        /// corresponds to <c>System.Threading.Timeout.Infinite</c>.</param>
        /// <returns>An asynchronous computation that returns the received message or
        /// None if the timeout is exceeded.</returns>
        member TryReceive : ?timeout:int -> Async<'Msg option>
        
        /// <summary>Scans for a message by looking through messages in arrival order until <c>scanner</c> 
        /// returns a Some value. Other messages remain in the queue.</summary>
        ///
        /// <remarks>Returns None if a timeout is given and the timeout is exceeded.
        ///
        /// This method is for use within the body of the agent. For each agent, at most 
        /// one concurrent reader may be active, so no more than one concurrent call to 
        /// Receive, TryReceive, Scan and/or TryScan may be active.</remarks>
        /// <param name="scanner">The function to return None if the message is to be skipped
        /// or Some if the message is to be processed and removed from the queue.</param>
        /// <param name="timeout">An optional timeout in milliseconds. Defaults to -1 which corresponds
        /// to <c>System.Threading.Timeout.Infinite</c>.</param>
        /// <returns>An asynchronous computation that <c>scanner</c> built off the read message.</returns>
        /// <exception cref="System.TimeoutException">Thrown when the timeout is exceeded.</exception>
        member Scan : scanner:('Msg -> (Async<'T>) option) * ?timeout:int -> Async<'T>

        /// <summary>Scans for a message by looking through messages in arrival order until <c>scanner</c> 
        /// returns a Some value. Other messages remain in the queue.</summary>
        ///
        /// <remarks>This method is for use within the body of the agent. For each agent, at most 
        /// one concurrent reader may be active, so no more than one concurrent call to 
        /// Receive, TryReceive, Scan and/or TryScan may be active.</remarks>
        /// <param name="scanner">The function to return None if the message is to be skipped
        /// or Some if the message is to be processed and removed from the queue.</param>
        /// <param name="timeout">An optional timeout in milliseconds. Defaults to -1 which corresponds
        /// to <c>System.Threading.Timeout.Infinite</c>.</param>
        /// <returns>An asynchronous computation that <c>scanner</c> built off the read message.</returns>
        member TryScan : scanner:('Msg -> (Async<'T>) option) * ?timeout:int -> Async<'T option>

        /// <summary>Starts the agent.</summary>
        member Start : unit -> unit

        /// <summary>Raises a timeout exception if a message not received in this amount of time. By default
        /// no timeout is used.</summary>
        member DefaultTimeout : int with get, set
        
        /// <summary>Occurs when the execution of the agent results in an exception.</summary>
        [<CLIEvent>]
        member Error : IEvent<System.Exception>

        interface System.IDisposable

        /// <summary>Returns the number of unprocessed messages in the message queue of the agent.</summary>
        member CurrentQueueLength : int


    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    /// <summary>Basic operations on first class event and other observable objects.</summary>
    module Observable = 

        /// <summary>Returns an observable for the merged observations from the sources. 
        /// The returned object propagates success and error values arising 
        /// from either source and completes when both the sources have completed.</summary>
        ///
        /// <remarks>For each observer, the registered intermediate observing object is not 
        /// thread safe. That is, observations arising from the sources must not 
        /// be triggered concurrently on different threads.</remarks>
        /// <param name="source1">The first Observable.</param>
        /// <param name="source2">The second Observable.</param>
        /// <returns>An Observable that propagates information from both sources.</returns>
        [<CompiledName("Merge")>]
        val merge: source1:IObservable<'T> -> source2:IObservable<'T> -> IObservable<'T>

        /// <summary>Returns an observable which transforms the observations of the source by the 
        /// given function. The transformation function is executed once for each 
        /// subscribed observer. The returned object also propagates error observations 
        /// arising from the source and completes when the source completes.</summary>
        /// <param name="mapping">The function applied to observations from the source.</param>
        /// <param name="source">The input Observable.</param>
        /// <returns>An Observable of the type specified by <c>mapping</c>.</returns> 
        [<CompiledName("Map")>]
        val map: mapping:('T -> 'U) -> source:IObservable<'T> -> IObservable<'U>

        /// <summary>Returns an observable which filters the observations of the source 
        /// by the given function. The observable will see only those observations
        /// for which the predicate returns true. The predicate is executed once for 
        /// each subscribed observer. The returned object also propagates error 
        /// observations arising from the source and completes when the source completes.</summary>
        /// <param name="filter">The function to apply to observations to determine if it should
        /// be kept.</param>
        /// <param name="source">The input Observable.</param>
        /// <returns>An Observable that filters observations based on <c>filter</c>.</returns>
        [<CompiledName("Filter")>]
        val filter: predicate:('T -> bool) -> source:IObservable<'T> -> IObservable<'T>

        /// <summary>Returns two observables which partition the observations of the source by 
        /// the given function. The first will trigger observations for those values 
        /// for which the predicate returns true. The second will trigger observations 
        /// for those values where the predicate returns false. The predicate is 
        /// executed once for each subscribed observer. Both also propagate all error 
        /// observations arising from the source and each completes when the source 
        /// completes.</summary>
        /// <param name="predicate">The function to determine which output Observable will trigger
        /// a particular observation.</param>
        /// <param name="source">The input Observable.</param>
        /// <returns>A tuple of Observables.  The first triggers when the predicate returns true, and
        /// the second triggers when the predicate returns false.</returns> 
        [<CompiledName("Partition")>]
        val partition: predicate:('T -> bool) -> source:IObservable<'T> -> (IObservable<'T> * IObservable<'T>)

        /// <summary>Returns two observables which split the observations of the source by the 
        /// given function. The first will trigger observations <c>x</c> for which the 
        /// splitter returns <c>Choice1Of2 x</c>. The second will trigger observations 
        /// <c>y</c> for which the splitter returns <c>Choice2Of2 y</c> The splitter is 
        /// executed once for each subscribed observer. Both also propagate error 
        /// observations arising from the source and each completes when the source 
        /// completes.</summary>
        /// <param name="splitter">The function that takes an observation an transforms
        /// it into one of the two output Choice types.</param>
        /// <param name="source">The input Observable.</param>
        /// <returns>A tuple of Observables.  The first triggers when <c>splitter</c> returns Choice1of2
        /// and the second triggers when <c>splitter</c> returns Choice2of2.</returns> 
        [<CompiledName("Split")>]
        val split: splitter:('T -> Choice<'U1,'U2>) -> source:IObservable<'T> -> (IObservable<'U1> * IObservable<'U2>)

        /// <summary>Returns an observable which chooses a projection of observations from the source 
        /// using the given function. The returned object will trigger observations <c>x</c>
        /// for which the splitter returns <c>Some x</c>. The returned object also propagates 
        /// all errors arising from the source and completes when the source completes.</summary>
        /// <param name="chooser">The function that returns Some for observations to be propagated
        /// and None for observations to ignore.</param>
        /// <param name="source">The input Observable.</param>
        /// <returns>An Observable that only propagates some of the observations from the source.</returns>
        [<CompiledName("Choose")>]
        val choose: chooser:('T -> 'U option) -> source:IObservable<'T> -> IObservable<'U>

        /// <summary>Returns an observable which, for each observer, allocates an item of state
        /// and applies the given accumulating function to successive values arising from
        /// the input. The returned object will trigger observations for each computed 
        /// state value, excluding the initial value. The returned object propagates 
        /// all errors arising from the source and completes when the source completes.</summary>
        ///
        /// <remarks>For each observer, the registered intermediate observing object is not thread safe.
        /// That is, observations arising from the source must not be triggered concurrently 
        /// on different threads.</remarks>
        /// <param name="collector">The function to update the state with each observation.</param>
        /// <param name="state">The initial state.</param>
        /// <param name="source">The input Observable.</param>
        /// <returns>An Observable that triggers on the updated state values.</returns>
        [<CompiledName("Scan")>]
        val scan: collector:('U -> 'T -> 'U) -> state:'U -> source:IObservable<'T> -> IObservable<'U> 

        /// <summary>Creates an observer which permanently subscribes to the given observable and which calls
        /// the given function for each observation.</summary>
        /// <param name="callback">The function to be called on each observation.</param>
        /// <param name="source">The input Observable.</param>
        [<CompiledName("Add")>]
        val add : callback:('T -> unit) -> source:IObservable<'T> -> unit

        /// <summary>Creates an observer which subscribes to the given observable and which calls
        /// the given function for each observation.</summary>
        /// <param name="callback">The function to be called on each observation.</param>
        /// <param name="source">The input Observable.</param>
        /// <returns>An object that will remove the callback if disposed.</returns>
        [<CompiledName("Subscribe")>]
        val subscribe : callback:('T -> unit) -> source:IObservable<'T> -> System.IDisposable

        /// <summary>Returns a new observable that triggers on the second and subsequent triggerings of the input observable.
        /// The Nth triggering of the input observable passes the arguments from the N-1th and Nth triggering as
        /// a pair. The argument passed to the N-1th triggering is held in hidden internal state until the 
        /// Nth triggering occurs.</summary>
        ///
        /// <remarks>For each observer, the registered intermediate observing object is not thread safe.
        /// That is, observations arising from the source must not be triggered concurrently 
        /// on different threads.</remarks>
        /// <param name="source">The input Observable.</param>
        /// <returns>An Observable that triggers on successive pairs of observations from the input Observable.</returns>
        [<CompiledName("Pairwise")>]
        val pairwise: source:IObservable<'T> -> IObservable<'T * 'T>

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module Event = 

        /// <summary>Fires the output event when either of the input events fire.</summary>
        /// <param name="event1">The first input event.</param>
        /// <param name="event2">The second input event.</param>
        /// <returns>An event that fires when either of the input events fire.</returns>
        [<CompiledName("Merge")>]
        val merge: event1:IEvent<'Del1,'T> -> event2:IEvent<'Del2,'T> -> IEvent<'T>

        /// <summary>Returns a new event that passes values transformed by the given function.</summary>
        /// <param name="map">The function to transform event values.</param>
        /// <param name="sourceEvent">The input event.</param>
        /// <returns>An event that passes the transformed values.</returns>
        [<CompiledName("Map")>]
        val map: mapping:('T -> 'U) -> sourceEvent:IEvent<'Del,'T> -> IEvent<'U>

        /// <summary>Returns a new event that listens to the original event and triggers the resulting
        /// event only when the argument to the event passes the given function.</summary>
        /// <param name="predicate">The function to determine which triggers from the event to propagate.</param>
        /// <param name="sourceEvent">The input event.</param>
        /// <returns>An event that only passes values that pass the predicate.</returns>
        [<CompiledName("Filter")>]
        val filter: predicate:('T -> bool) -> sourceEvent:IEvent<'Del,'T> -> IEvent<'T>

        /// <summary>Returns a new event that listens to the original event and triggers the 
        /// first resulting event if the application of the predicate to the event arguments
        /// returned true, and the second event if it returned false.</summary>
        /// <param name="predicate">The function to determine which output event to trigger.</param>
        /// <param name="sourceEvent">The input event.</param>
        /// <returns>A tuple of events.  The first is triggered when the predicate evaluates to true
        /// and the second when the predicate evaluates to false.</returns>
        [<CompiledName("Partition")>]
        val partition: predicate:('T -> bool) -> sourceEvent:IEvent<'Del,'T> -> (IEvent<'T> * IEvent<'T>)

        /// <summary>Returns a new event that listens to the original event and triggers the 
        /// first resulting event if the application of the function to the event arguments
        /// returned a Choice1Of2, and the second event if it returns a Choice2Of2.</summary>
        /// <param name="splitter">The function to transform event values into one of two types.</param>
        /// <param name="sourceEvent">The input event.</param>
        /// <returns>A tuple of events.  The first fires whenever <c>splitter</c> evaluates to Choice1of1 and
        /// the second fires whenever <c>splitter</c> evaluates to Choice2of2.</returns>
        [<CompiledName("Split")>]
        val split: splitter:('T -> Choice<'U1,'U2>) -> sourceEvent:IEvent<'Del,'T> -> (IEvent<'U1> * IEvent<'U2>)

        /// <summary>Returns a new event which fires on a selection of messages from the original event.
        /// The selection function takes an original message to an optional new message.</summary>
        /// <param name="chooser">The function to select and transform event values to pass on.</param>
        /// <param name="sourceEvent">The input event.</param>
        /// <returns>An event that fires only when the chooser returns Some.</returns>
        [<CompiledName("Choose")>]
        val choose: chooser:('T -> 'U option) -> sourceEvent:IEvent<'Del,'T> -> IEvent<'U>

        [<CompiledName("Scan")>]
        /// <summary>Returns a new event consisting of the results of applying the given accumulating function
        /// to successive values triggered on the input event.  An item of internal state
        /// records the current value of the state parameter.  The internal state is not locked during the
        /// execution of the accumulation function, so care should be taken that the 
        /// input IEvent not triggered by multiple threads simultaneously.</summary>
        /// <param name="collector">The function to update the state with each event value.</param>
        /// <param name="state">The initial state.</param>
        /// <param name="sourceEvent">The input event.</param>
        /// <returns>An event that fires on the updated state values.</returns>
        val scan: collector:('U -> 'T -> 'U) -> state:'U -> sourceEvent:IEvent<'Del,'T> -> IEvent<'U> 

        /// <summary>Runs the given function each time the given event is triggered.</summary>
        /// <param name="callback">The function to call when the event is triggered.</param>
        /// <param name="sourceEvent">The input event.</param>
        [<CompiledName("Add")>]
        val add : callback:('T -> unit) -> sourceEvent:IEvent<'Del,'T> -> unit

        /// <summary>Returns a new event that triggers on the second and subsequent triggerings of the input event.
        /// The Nth triggering of the input event passes the arguments from the N-1th and Nth triggering as
        /// a pair. The argument passed to the N-1th triggering is held in hidden internal state until the 
        /// Nth triggering occurs.</summary>
        /// <param name="sourceEvent">The input event.</param>
        /// <returns>An event that triggers on pairs of consecutive values passed from the source event.</returns>
        [<CompiledName("Pairwise")>]
        val pairwise: sourceEvent:IEvent<'Del,'T> -> IEvent<'T * 'T>


