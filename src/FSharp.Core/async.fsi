// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Control

    open System
    open System.Threading
    open System.Threading.Tasks
    open System.Runtime.ExceptionServices

    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Control
    open Microsoft.FSharp.Collections

    /// <summary>
    /// An asynchronous computation, which, when run, will eventually produce a value  of type T, or else raises an exception.
    /// </summary> 
    ///
    /// <remarks>
    ///  This type has no members. Asynchronous computations are normally specified either by using an async expression
    ///  or the static methods in the <see cref="T:Microsoft.FSharp.Control.FSharpAsync`1"/> type.
    ///
    ///  See also <a href="https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/asynchronous-workflows">F# Language Guide - Async Workflows</a>.
    /// </remarks> 
    ///
    /// <namespacedoc><summary>
    ///   Library functionality for asynchronous programming, events and agents. See also
    ///   <a href="https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/asynchronous-workflows">Asynchronous Programming</a>, 
    ///   <a href="https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/members/events">Events</a> and
    ///   <a href="https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/lazy-expressions">Lazy Expressions</a> in the
    ///   F# Language Guide.
    /// </summary></namespacedoc>
    ///
    /// <category index="1">Async Programming</category>
     
    [<Sealed; NoEquality; NoComparison; CompiledName("FSharpAsync`1")>]
    type Async<'T>

    /// <summary>Holds static members for creating and manipulating asynchronous computations.</summary>
    ///
    /// <remarks>
    ///  See also <a href="https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/asynchronous-workflows">F# Language Guide - Async Workflows</a>.
    /// </remarks>
    ///
    /// <category index="1">Async Programming</category>

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
        /// The computation is started on the current thread if <see cref="P:System.Threading.SynchronizationContext.Current"/> is null,
        /// <see cref="P:System.Threading.Thread.CurrentThread"/> has  <see cref="P:System.Threading.Thread.IsThreadPoolThread"/>
        /// of <c>true</c>, and no timeout is specified. Otherwise the computation is started by queueing a new work item in the thread pool,
        /// and the current thread is blocked awaiting the completion of the computation.
        ///
        /// The timeout parameter is given in milliseconds.  A value of -1 is equivalent to
        /// <see cref="F:System.Threading.Timeout.Infinite"/>.
        /// </remarks>
        ///
        /// <param name="computation">The computation to run.</param>
        /// <param name="timeout">The amount of time in milliseconds to wait for the result of the
        /// computation before raising a <see cref="T:System.TimeoutException"/>.  If no value is provided
        /// for timeout then a default of -1 is used to correspond to <see cref="F:System.Threading.Timeout.Infinite"/>.</param>
        /// <param name="cancellationToken">The cancellation token to be associated with the computation.
        /// If one is not supplied, the default cancellation token is used.</param>
        ///
        /// <returns>The result of the computation.</returns>
        ///
        /// <category index="0">Starting Async Computations</category>
        ///
        /// <example id="run-synchronously-1">
        /// <code lang="fsharp">
        /// printfn "A"
        ///
        /// async {
        ///     printfn "B"
        ///     do! Async.Sleep(1000)
        ///     printfn "C"
        /// } |> Async.RunSynchronously
        ///
        /// printfn "D"
        /// </code>
        /// Prints "A", "B" immediately, then "C", "D" in 1 second.
        /// </example>
        static member RunSynchronously : computation:Async<'T> * ?timeout : int * ?cancellationToken:CancellationToken-> 'T
        
        /// <summary>Starts the asynchronous computation in the thread pool. Do not await its result.</summary>
        ///
        /// <remarks>If no cancellation token is provided then the default cancellation token is used.</remarks>
        ///
        /// <param name="computation">The computation to run asynchronously.</param>
        /// <param name="cancellationToken">The cancellation token to be associated with the computation.
        /// If one is not supplied, the default cancellation token is used.</param>
        ///
        /// <category index="0">Starting Async Computations</category>
        ///
        /// <example id="start-1">
        /// <code lang="fsharp">
        /// printfn "A"
        ///
        /// async {
        ///     printfn "B"
        ///     do! Async.Sleep(1000)
        ///     printfn "C"
        /// } |> Async.Start
        ///
        /// printfn "D"
        /// </code>
        /// Prints "A", "D" immediately, then "B" quickly, and then "C" in 1 second.
        /// </example>
        /// <example-tbd></example-tbd>
        static member Start : computation:Async<unit> * ?cancellationToken:CancellationToken -> unit

        /// <summary>Executes a computation in the thread pool.</summary>
        ///
        /// <remarks>If no cancellation token is provided then the default cancellation token is used.</remarks>
        ///
        /// <returns>A <see cref="T:System.Threading.Tasks.Task`1"/> that will be completed
        /// in the corresponding state once the computation terminates (produces the result, throws exception or gets canceled)</returns>
        ///
        /// <category index="0">Starting Async Computations</category>
        ///
        /// <example id="start-as-task-1">
        /// <code lang="fsharp">
        /// printfn "A"
        ///
        /// let t =
        ///     async {
        ///         printfn "B"
        ///         do! Async.Sleep(1000)
        ///         printfn "C"
        ///     } |> Async.StartAsTask
        ///
        /// printfn "D"
        /// t.Wait()
        /// printfn "E"
        /// </code>
        /// Prints "A", "D" immediately, then "B" quickly, then "C", "E" in 1 second.
        /// </example>
        static member StartAsTask : computation:Async<'T> * ?taskCreationOptions:TaskCreationOptions * ?cancellationToken:CancellationToken -> Task<'T>

        /// <summary>Creates an asynchronous computation which starts the given computation as a <see cref="T:System.Threading.Tasks.Task`1"/></summary>
        ///
        /// <category index="0">Starting Async Computations</category>
        ///
        /// <example-tbd></example-tbd>
        static member StartChildAsTask : computation:Async<'T> * ?taskCreationOptions:TaskCreationOptions -> Async<Task<'T>>

        /// <summary>Creates an asynchronous computation that executes <c>computation</c>.
        /// If this computation completes successfully then return <c>Choice1Of2</c> with the returned
        /// value. If this computation raises an exception before it completes then return <c>Choice2Of2</c>
        /// with the raised exception.</summary>
        ///
        /// <param name="computation">The input computation that returns the type T.</param>
        ///
        /// <returns>A computation that returns a choice of type T or exception.</returns>
        ///
        /// <category index="3">Cancellation and Exceptions</category>
        ///
        /// <example id="catch-example-1">
        /// <code lang="fsharp">
        /// async { return someRiskyBusiness() }
        /// |> Async.Catch
        /// |> Async.RunSynchronously
        /// |> function
        ///     | Choice1Of2 result -> printfn $"Result: {result}"
        ///     | Choice2Of2 e -> printfn $"Exception: {e}"
        /// </code>
        /// Prints the returned value of someRiskyBusiness() or the exception if there is one.
        /// </example>
        static member Catch : computation:Async<'T> -> Async<Choice<'T,exn>>

        /// <summary>Creates an asynchronous computation that executes <c>computation</c>.
        /// If this computation is cancelled before it completes then the computation generated by 
        /// running <c>compensation</c> is executed.</summary>
        ///
        /// <param name="computation">The input asynchronous computation.</param>
        /// <param name="compensation">The function to be run if the computation is cancelled.</param>
        ///
        /// <returns>An asynchronous computation that runs the compensation if the input computation
        /// is cancelled.</returns>
        ///
        /// <category index="3">Cancellation and Exceptions</category>
        ///
        /// <example id="try-cancelled-1">
        /// <code lang="fsharp">
        /// [ 2; 3; 5; 7; 11 ]
        /// |> List.map
        ///     (fun i ->
        ///         Async.TryCancelled(
        ///             async {
        ///                 do! Async.Sleep(i * 1000)
        ///                 printfn $"{i}"
        ///             },
        ///             fun oce -> printfn $"Computation Cancelled: {i}"
        ///         ))
        /// |> List.iter Async.Start
        ///
        /// Thread.Sleep(6000)
        /// Async.CancelDefaultToken()
        /// printfn "Tasks Finished"
        /// </code>
        /// This will print "2" 2 seconds from start, "3" 3 seconds from start, "5" 5 seconds from start, cease computation
        /// and then print "Computation Cancelled: 7", "Computation Cancelled: 11" and "Tasks Finished" in any order.
        /// </example>
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
        ///
        /// <param name="interruption">The function that is executed on the thread performing the
        /// cancellation.</param>
        ///
        /// <returns>An asynchronous computation that triggers the interruption if it is cancelled
        /// before being disposed.</returns>
        ///
        /// <category index="3">Cancellation and Exceptions</category>
        ///
        /// <example id="on-cancel-1">
        /// <code lang="fsharp">
        ///     [ 2; 3; 5; 7; 11 ]
        /// |> List.iter
        ///     (fun i ->
        ///         async {
        ///             use! holder = Async.OnCancel(fun () -> printfn $"Computation Cancelled: {i}")
        ///             do! Async.Sleep(i * 1000)
        ///             printfn $"{i}"
        ///         } |> Async.Start)
        ///
        /// Thread.Sleep(6000)
        /// Async.CancelDefaultToken()
        /// printfn "Tasks Finished"
        /// </code>
        /// This will print "2" 2 seconds from start, "3" 3 seconds from start, "5" 5 seconds from start, cease computation
        /// and then print "Computation Cancelled: 7", "Computation Cancelled: 11" and "Tasks Finished" in any order.
        /// </example>
        static member OnCancel : interruption: (unit -> unit) -> Async<System.IDisposable>
        
        /// <summary>Creates an asynchronous computation that returns the CancellationToken governing the execution 
        /// of the computation.</summary>
        ///
        /// <remarks>In <c>async { let! token = Async.CancellationToken ...}</c> token can be used to initiate other 
        /// asynchronous operations that will cancel cooperatively with this workflow.</remarks>
        ///
        /// <returns>An asynchronous computation capable of retrieving the CancellationToken from a computation
        /// expression.</returns>
        ///
        /// <category index="3">Cancellation and Exceptions</category>
        ///
        /// <example-tbd></example-tbd>
        static member CancellationToken : Async<CancellationToken>

        /// <summary>Raises the cancellation condition for the most recent set of asynchronous computations started 
        /// without any specific CancellationToken. Replaces the global CancellationTokenSource with a new 
        /// global token source for any asynchronous computations created after this point without any 
        /// specific CancellationToken.</summary>
        ///
        /// <category index="3">Cancellation and Exceptions</category>
        ///
        /// <example id="cancel-default-token-1">
        /// <code lang="fsharp">
        /// try
        ///     let computations =
        ///         [ 2; 3; 5; 7; 11 ]
        ///         |> List.map
        ///             (fun i ->
        ///                 async {
        ///                     do! Async.Sleep(i * 1000)
        ///                     printfn $"{i}"
        ///                 })
        ///
        ///     let t =
        ///         Async.Parallel(computations, 3)
        ///         |> Async.StartAsTask
        ///
        ///     Thread.Sleep(6000)
        ///     Async.CancelDefaultToken()
        ///     printfn $"Tasks Finished: %A{t.Result}"
        /// with
        /// | :? System.AggregateException as ae -> printfn $"Tasks Not Finished: {ae.Message}"
        /// </code>
        /// This will print "2" 2 seconds from start, "3" 3 seconds from start, "5" 5 seconds from start, cease computation and
        /// then print "Tasks Not Finished: One or more errors occurred. (A task was canceled.)".
        /// </example>
        static member CancelDefaultToken :  unit -> unit 

        /// <summary>Gets the default cancellation token for executing asynchronous computations.</summary>
        ///
        /// <returns>The default CancellationToken.</returns>
        ///
        /// <category index="3">Cancellation and Exceptions</category>
        ///
        /// <example id="default-cancellation-token-1">
        /// <code lang="fsharp">
        /// Async.DefaultCancellationToken.Register(fun () -> printfn "Computation Cancelled") |> ignore
        /// [ 2; 3; 5; 7; 11 ]
        /// |> List.map
        ///     (fun i ->
        ///         async {
        ///             do! Async.Sleep(i * 1000)
        ///             printfn $"{i}"
        ///         })
        /// |> List.iter Async.Start
        ///
        /// Thread.Sleep(6000)
        /// Async.CancelDefaultToken()
        /// printfn "Tasks Finished"
        /// </code>
        /// This will print "2" 2 seconds from start, "3" 3 seconds from start, "5" 5 seconds from start, cease computation and then
        /// print "Computation Cancelled", followed by "Tasks Finished".
        /// </example>
        static member DefaultCancellationToken : CancellationToken

        //---------- Parallelism

        /// <summary>Starts a child computation within an asynchronous workflow. 
        /// This allows multiple asynchronous computations to be executed simultaneously.</summary>
        ///
        /// <remarks>This method should normally be used as the immediate 
        /// right-hand-side of a <c>let!</c> binding in an F# asynchronous workflow, that is,
        /// <code lang="fsharp">
        ///        async { ...
        ///                let! completor1 = childComputation1 |> Async.StartChild  
        ///                let! completor2 = childComputation2 |> Async.StartChild  
        ///                ... 
        ///                let! result1 = completor1 
        ///                let! result2 = completor2 
        ///                ... }
        /// </code>
        ///
        /// When used in this way, each use of <c>StartChild</c> starts an instance of <c>childComputation</c> 
        /// and returns a completor object representing a computation to wait for the completion of the operation.
        /// When executed, the completor awaits the completion of <c>childComputation</c>.</remarks>
        ///
        /// <param name="computation">The child computation.</param>
        /// <param name="millisecondsTimeout">The timeout value in milliseconds.  If one is not provided
        /// then the default value of -1 corresponding to <see cref="F:System.Threading.Timeout.Infinite"/>.</param>
        ///
        /// <returns>A new computation that waits for the input computation to finish.</returns>
        ///
        /// <category index="3">Cancellation and Exceptions</category>
        ///
        /// <example id="start-child-1">
        /// <code lang="fsharp">
        /// let computeWithTimeout timeout =
        /// async {
        ///     let! completor1 =
        ///         Async.StartChild(
        ///             (async {
        ///                 do! Async.Sleep(1000)
        ///                 return 1
        ///              }),
        ///             millisecondsTimeout = timeout
        ///         )
        ///
        ///     let! completor2 =
        ///         Async.StartChild(
        ///             (async {
        ///                 do! Async.Sleep(2000)
        ///                 return 2
        ///              }),
        ///             millisecondsTimeout = timeout
        ///         )
        ///
        ///     let! v1 = completor1
        ///     let! v2 = completor2
        ///     printfn $"Result: {v1 + v2}"
        /// } |> Async.RunSynchronously
        /// </code>
        /// Will throw a System.TimeoutException if called with a timeout less than 2000, otherwise will print "Result: 3".
        /// </example>
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
        ///
        /// <param name="computations">A sequence of distinct computations to be parallelized.</param>
        ///
        /// <returns>A computation that returns an array of values from the sequence of input computations.</returns>
        ///
        /// <category index="1">Composing Async Computations</category>
        ///
        /// <example id="parallel-1">
        /// <code lang="fsharp">
        /// let t =
        ///     [ 2; 3; 5; 7; 10; 11 ]
        ///     |> List.map
        ///         (fun i ->
        ///             async {
        ///                 do! Async.Sleep(System.Random().Next(1000, 2000))
        ///
        ///                 if i % 2 > 0 then
        ///                     printfn $"{i}"
        ///                     return true
        ///                 else
        ///                     return false
        ///             })
        ///     |> Async.Parallel
        ///     |> Async.StartAsTask
        ///
        /// t.Wait()
        /// printfn $"%A{t.Result}"
        /// </code>
        /// This will print "3", "5", "7", "11" (in any order) in 1-2 seconds and then [| false; true; true; true; false; true |].
        /// </example>
        static member Parallel : computations:seq<Async<'T>> -> Async<'T[]>

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
        ///
        /// <param name="computations">A sequence of distinct computations to be parallelized.</param>
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism in the parallel execution.</param>
        ///
        /// <returns>A computation that returns an array of values from the sequence of input computations.</returns>
        ///
        /// <category index="1">Composing Async Computations</category>
        ///
        /// <example id="parallel-2">
        /// <code lang="fsharp">
        /// let computations =
        ///     [ 2; 3; 5; 7; 10; 11 ]
        ///     |> List.map
        ///         (fun i ->
        ///             async {
        ///                 do! Async.Sleep(System.Random().Next(1000, 2000))
        ///
        ///                 return
        ///                     if i % 2 > 0 then
        ///                         printfn $"{i}"
        ///                         true
        ///                     else
        ///                         false
        ///             })
        ///
        /// let t =
        ///     Async.Parallel(computations, 3)
        ///     |> Async.StartAsTask
        ///
        /// t.Wait()
        /// printfn $"%A{t.Result}"
        /// </code>
        /// This will print "3", "5" (in any order) in 1-2 seconds, and then "7", "11" (in any order) in 1-2 more seconds and then
        /// [| false; true; true; true; false; true |].
        /// </example>
        static member Parallel : computations:seq<Async<'T>> * ?maxDegreeOfParallelism : int -> Async<'T[]>

        /// <summary>Creates an asynchronous computation that executes all the given asynchronous computations sequentially.</summary>
        ///
        /// <remarks>If all child computations succeed, an array of results is passed to the success continuation.
        ///
        /// If any child computation raises an exception, then the overall computation will trigger an
        /// exception, and cancel the others.
        ///
        /// The overall computation will respond to cancellation while executing the child computations.
        /// If cancelled, the computation will cancel any remaining child computations but will still wait
        /// for the other child computations to complete.</remarks>
        ///
        /// <param name="computations">A sequence of distinct computations to be run in sequence.</param>
        ///
        /// <returns>A computation that returns an array of values from the sequence of input computations.</returns>
        ///
        /// <category index="1">Composing Async Computations</category>
        ///
        /// <example id="sequential-1">
        /// <code lang="fsharp">
        /// let computations =
        ///     [ 2; 3; 5; 7; 10; 11 ]
        ///     |> List.map
        ///         (fun i ->
        ///             async {
        ///                 do! Async.Sleep(System.Random().Next(1000, 2000))
        ///
        ///                 if i % 2 > 0 then
        ///                     printfn $"{i}"
        ///                     return true
        ///                 else
        ///                     return false
        ///             })
        ///
        /// let t =
        ///     Async.Sequential(computations)
        ///     |> Async.StartAsTask
        ///
        /// t.Wait()
        /// printfn $"%A{t.Result}" 
        /// </code>
        /// This will print "3", "5", "7", "11" with ~1-2 seconds between them except for pauses where even numbers would be and then
        /// prints [| false; true; true; true; false; true |].
        /// </example>
        static member Sequential : computations:seq<Async<'T>> -> Async<'T[]>

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
        ///
        /// <param name="computations">A sequence of computations to be parallelized.</param>
        ///
        /// <returns>A computation that returns the first succeeding computation.</returns>
        ///
        /// <category index="1">Composing Async Computations</category>
        ///
        /// <example id="choice-example-1">
        /// <code lang="fsharp">
        /// printfn "Starting"
        /// [ 2; 3; 5; 7 ]
        /// |> List.map
        ///     (fun i ->
        ///         async {
        ///             do! Async.Sleep(System.Random().Next(1000, 2000))
        ///             return if i % 2 > 0 then Some(i) else None
        ///         })
        /// |> Async.Choice
        /// |> Async.RunSynchronously
        /// |> function
        ///     | Some (i) -> printfn $"{i}"
        ///     | None -> printfn "No Result"
        /// </code>
        /// Prints one randomly selected odd number in 1-2 seconds. If the list is changed to all even numbers, it will
        /// instead print "No Result".
        /// </example>
        ///
        /// <example id="choice-example-2">
        /// <code lang="fsharp">
        /// [ 2; 3; 5; 7 ]
        /// |> List.map
        ///     (fun i ->
        ///         async {
        ///             do! Async.Sleep(System.Random().Next(1000, 2000))
        ///
        ///             return
        ///                 if i % 2 > 0 then
        ///                     Some(i)
        ///                 else
        ///                     failwith $"Even numbers not supported: {i}"
        ///         })
        /// |> Async.Choice
        /// |> Async.RunSynchronously
        /// |> function
        ///     | Some (i) -> printfn $"{i}"
        ///     | None -> printfn "No Result"
        /// </code>
        /// Will sometimes print one randomly selected odd number, sometimes throw System.Exception("Even numbers not supported: 2").
        /// </example>
        static member Choice : computations:seq<Async<'T option>> -> Async<'T option>

        //---------- Thread Control
        
        /// <summary>Creates an asynchronous computation that creates a new thread and runs
        /// its continuation in that thread.</summary>
        ///
        /// <returns>A computation that will execute on a new thread.</returns>
        ///
        /// <category index="4">Threads and Contexts</category>
        ///
        /// <example id="switch-to-new-thread">
        /// <code lang="fsharp">
        /// async {
        ///     do! Async.SwitchToNewThread()
        ///     do! someLongRunningComputation()
        /// } |> Async.StartImmediate
        /// </code>
        /// This will run someLongRunningComputation() without blocking the threads in the threadpool.
        /// </example>
        static member SwitchToNewThread : unit -> Async<unit> 
        
        /// <summary>Creates an asynchronous computation that queues a work item that runs
        /// its continuation.</summary>
        ///
        /// <returns>A computation that generates a new work item in the thread pool.</returns>
        ///
        /// <category index="4">Threads and Contexts</category>
        ///
        /// <example id="switch-to-thread-pool-1">
        /// <code lang="fsharp">
        /// async {
        ///     do! Async.SwitchToNewThread()
        ///     do! someLongRunningComputation()
        ///     do! Async.SwitchToThreadPool()
        ///
        ///     for i in 1 .. 10 do
        ///         do! someShortRunningComputation()
        /// } |> Async.StartImmediate
        /// </code>
        /// This will run someLongRunningComputation() without blocking the threads in the threadpool, and then switch to the
        /// threadpool for shorter computations.
        /// </example>
        static member SwitchToThreadPool :  unit -> Async<unit> 

        /// <summary>Creates an asynchronous computation that runs
        /// its continuation using syncContext.Post. If syncContext is null 
        /// then the asynchronous computation is equivalent to SwitchToThreadPool().</summary>
        ///
        /// <param name="syncContext">The synchronization context to accept the posted computation.</param>
        ///
        /// <returns>An asynchronous computation that uses the syncContext context to execute.</returns>
        ///
        /// <category index="4">Threads and Contexts</category>
        ///
        /// <example-tbd></example-tbd>
        static member SwitchToContext :  syncContext:System.Threading.SynchronizationContext -> Async<unit> 

        /// <summary>Creates an asynchronous computation that captures the current
        /// success, exception and cancellation continuations. The callback must 
        /// eventually call exactly one of the given continuations.</summary>
        ///
        /// <param name="callback">The function that accepts the current success, exception, and cancellation
        /// continuations.</param>
        ///
        /// <returns>An asynchronous computation that provides the callback with the current continuations.</returns>
        ///
        /// <category index="1">Composing Async Computations</category>
        ///
        /// <example id="from-continuations-1">
        /// <code lang="fsharp">
        /// let computation =
        ///     (fun (successCont, exceptionCont, cancellationCont) ->
        ///         try
        ///             someRiskyBusiness () |> successCont
        ///         with
        ///         | :? OperationCanceledException as oce -> cancellationCont oce
        ///         | e -> exceptionCont e)
        ///     |> Async.FromContinuations
        ///
        /// Async.StartWithContinuations(
        ///     computation,
        ///     (fun result -> printfn $"Result: {result}"),
        ///     (fun e -> printfn $"Exception: {e}"),
        ///     (fun oce -> printfn $"Cancelled: {oce}")
        ///  )
        /// </code>
        /// This anonymous function will call someRiskyBusiness() and be a good citizen with regards to the continuations
        /// defined to report the outcome.
        /// </example>
        static member FromContinuations : callback:(('T -> unit) * (exn -> unit) * (OperationCanceledException -> unit) -> unit) -> Async<'T>

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
        ///
        /// <param name="event">The event to handle once.</param>
        /// <param name="cancelAction">An optional function to execute instead of cancelling when a
        /// cancellation is issued.</param>
        ///
        /// <returns>An asynchronous computation that waits for the event to be invoked.</returns>
        ///
        /// <category index="2">Awaiting Results</category>
        ///
        /// <example-tbd></example-tbd>
        static member AwaitEvent: event:IEvent<'Del,'T> * ?cancelAction : (unit -> unit) -> Async<'T> when 'Del : delegate<'T,unit> and 'Del :> System.Delegate 

        /// <summary>Creates an asynchronous computation that will wait on the given WaitHandle.</summary>
        ///
        /// <remarks>The computation returns true if the handle indicated a result within the given timeout.</remarks>
        ///
        /// <param name="waitHandle">The <c>WaitHandle</c> that can be signalled.</param>
        /// <param name="millisecondsTimeout">The timeout value in milliseconds.  If one is not provided
        /// then the default value of -1 corresponding to <see cref="F:System.Threading.Timeout.Infinite"/>.</param>
        ///
        /// <returns>An asynchronous computation that waits on the given <c>WaitHandle</c>.</returns>
        ///
        /// <category index="2">Awaiting Results</category>
        ///
        /// <example-tbd></example-tbd>
        static member AwaitWaitHandle: waitHandle: WaitHandle * ?millisecondsTimeout:int -> Async<bool>

        /// <summary>Creates an asynchronous computation that will wait on the IAsyncResult.</summary>
        ///
        /// <remarks>The computation returns true if the handle indicated a result within the given timeout.</remarks>
        ///
        /// <param name="iar">The IAsyncResult to wait on.</param>
        /// <param name="millisecondsTimeout">The timeout value in milliseconds.  If one is not provided
        /// then the default value of -1 corresponding to <see cref="F:System.Threading.Timeout.Infinite"/>.</param>
        ///
        /// <returns>An asynchronous computation that waits on the given <c>IAsyncResult</c>.</returns>
        ///
        /// <category index="2">Awaiting Results</category>
        ///
        /// <example-tbd></example-tbd>
        static member AwaitIAsyncResult: iar: System.IAsyncResult * ?millisecondsTimeout:int -> Async<bool>

        /// <summary>Return an asynchronous computation that will wait for the given task to complete and return
        /// its result.</summary>
        ///
        /// <param name="task">The task to await.</param>
        ///
        /// <remarks>If an exception occurs in the asynchronous computation then an exception is re-raised by this
        /// function.
        ///
        /// If the task is cancelled then <see cref="F:System.Threading.Tasks.TaskCanceledException"/> is raised. Note
        /// that the task may be governed by a different cancellation token to the overall async computation
        /// where the AwaitTask occurs. In practice you should normally start the task with the
        /// cancellation token returned by <c>let! ct = Async.CancellationToken</c>, and catch
        /// any <see cref="F:System.Threading.Tasks.TaskCanceledException"/> at the point where the
        /// overall async is started.
        /// </remarks>
        ///
        /// <category index="2">Awaiting Results</category>
        ///
        /// <example-tbd></example-tbd>
        static member AwaitTask: task: Task<'T> -> Async<'T>

        /// <summary>Return an asynchronous computation that will wait for the given task to complete and return
        /// its result.</summary>
        ///
        /// <param name="task">The task to await.</param>
        ///
        /// <remarks>If an exception occurs in the asynchronous computation then an exception is re-raised by this
        /// function.
        ///
        /// If the task is cancelled then <see cref="F:System.Threading.Tasks.TaskCanceledException"/> is raised. Note
        /// that the task may be governed by a different cancellation token to the overall async computation
        /// where the AwaitTask occurs. In practice you should normally start the task with the
        /// cancellation token returned by <c>let! ct = Async.CancellationToken</c>, and catch
        /// any <see cref="F:System.Threading.Tasks.TaskCanceledException"/> at the point where the
        /// overall async is started.
        /// </remarks>
        ///
        /// <category index="2">Awaiting Results</category>
        ///
        /// <example-tbd></example-tbd>
        static member AwaitTask: task: Task -> Async<unit>

        /// <summary>
        ///  Creates an asynchronous computation that will sleep for the given time. This is scheduled
        ///  using a System.Threading.Timer object. The operation will not block operating system threads
        ///  for the duration of the wait.
        /// </summary>
        ///
        /// <param name="millisecondsDueTime">The number of milliseconds to sleep.</param>
        ///
        /// <returns>An asynchronous computation that will sleep for the given time.</returns>
        ///
        /// <exception cref="T:System.ArgumentOutOfRangeException">Thrown when the due time is negative
        /// and not infinite.</exception>
        ///
        /// <category index="2">Awaiting Results</category>
        ///
        /// <example id="sleep-1">
        /// <code lang="fsharp">
        /// async {
        ///     printfn "A"
        ///     do! Async.Sleep(1000)
        ///     printfn "B"
        /// } |> Async.Start
        ///
        /// printfn "C"
        /// </code>
        /// Prints "C", then "A" quickly, and then "B" 1 second later
        /// </example>
        static member Sleep: millisecondsDueTime:int -> Async<unit>

        /// <summary>
        ///  Creates an asynchronous computation that will sleep for the given time. This is scheduled
        ///  using a System.Threading.Timer object. The operation will not block operating system threads
        ///  for the duration of the wait.
        /// </summary>
        ///
        /// <param name="dueTime">The amount of time to sleep.</param>
        ///
        /// <returns>An asynchronous computation that will sleep for the given time.</returns>
        ///
        /// <exception cref="T:System.ArgumentOutOfRangeException">Thrown when the due time is negative.</exception>
        ///
        /// <category index="2">Awaiting Results</category>
        ///
        /// <example id="sleep-2">
        /// <code lang="fsharp">
        /// async {
        ///     printfn "A"
        ///     do! Async.Sleep(TimeSpan(0, 0, 1))
        ///     printfn "B"
        /// } |> Async.Start
        /// printfn "C"
        /// </code>
        /// Prints "C", then "A" quickly, and then "B" 1 second later.
        /// </example>
        static member Sleep: dueTime:TimeSpan -> Async<unit>

        /// <summary>
        ///  Creates an asynchronous computation in terms of a Begin/End pair of actions in 
        ///  the style used in CLI APIs.
        /// </summary>
        ///
        /// <remarks>
        /// The computation will respond to cancellation while waiting for the completion
        /// of the operation. If a cancellation occurs, and <c>cancelAction</c> is specified, then it is 
        /// executed, and the computation continues to wait for the completion of the operation.
        ///
        /// If <c>cancelAction</c> is not specified, then cancellation causes the computation
        /// to stop immediately, and subsequent invocations of the callback are ignored.</remarks>
        ///
        /// <param name="beginAction">The function initiating a traditional CLI asynchronous operation.</param>
        /// <param name="endAction">The function completing a traditional CLI asynchronous operation.</param>
        /// <param name="cancelAction">An optional function to be executed when a cancellation is requested.</param>
        ///
        /// <returns>An asynchronous computation wrapping the given Begin/End functions.</returns>
        ///
        /// <category index="5">Legacy .NET Async Interoperability</category>
        ///
        /// <example-tbd></example-tbd>
        static member FromBeginEnd : beginAction:(System.AsyncCallback * obj -> System.IAsyncResult) * endAction:(System.IAsyncResult -> 'T) * ?cancelAction : (unit -> unit) -> Async<'T>

        /// <summary>
        ///  Creates an asynchronous computation in terms of a Begin/End pair of actions in 
        ///  the style used in .NET 2.0 APIs.
        /// </summary>
        ///
        /// <remarks>The computation will respond to cancellation while waiting for the completion
        /// of the operation. If a cancellation occurs, and <c>cancelAction</c> is specified, then it is 
        /// executed, and the computation continues to wait for the completion of the operation.
        ///
        ///  If <c>cancelAction</c> is not specified, then cancellation causes the computation
        ///  to stop immediately, and subsequent invocations of the callback are ignored.
        ///</remarks>
        ///
        /// <param name="arg">The argument for the operation.</param>
        /// <param name="beginAction">The function initiating a traditional CLI asynchronous operation.</param>
        /// <param name="endAction">The function completing a traditional CLI asynchronous operation.</param>
        /// <param name="cancelAction">An optional function to be executed when a cancellation is requested.</param>
        ///
        /// <returns>An asynchronous computation wrapping the given Begin/End functions.</returns>
        ///
        /// <category index="5">Legacy .NET Async Interoperability</category>
        ///
        /// <example-tbd></example-tbd>
        static member FromBeginEnd : arg:'Arg1 * beginAction:('Arg1 * System.AsyncCallback * obj -> System.IAsyncResult) * endAction:(System.IAsyncResult -> 'T) * ?cancelAction : (unit -> unit) -> Async<'T>

        /// <summary>
        /// Creates an asynchronous computation in terms of a Begin/End pair of actions in 
        /// the style used in .NET 2.0 APIs.</summary>
        ///
        /// <remarks>The computation will respond to cancellation while waiting for the completion
        /// of the operation. If a cancellation occurs, and <c>cancelAction</c> is specified, then it is 
        /// executed, and the computation continues to wait for the completion of the operation.
        ///
        /// If <c>cancelAction</c> is not specified, then cancellation causes the computation
        /// to stop immediately, and subsequent invocations of the callback are ignored.</remarks>
        ///
        /// <param name="arg1">The first argument for the operation.</param>
        /// <param name="arg2">The second argument for the operation.</param>
        /// <param name="beginAction">The function initiating a traditional CLI asynchronous operation.</param>
        /// <param name="endAction">The function completing a traditional CLI asynchronous operation.</param>
        /// <param name="cancelAction">An optional function to be executed when a cancellation is requested.</param>
        ///
        /// <returns>An asynchronous computation wrapping the given Begin/End functions.</returns>
        ///
        /// <category index="5">Legacy .NET Async Interoperability</category>
        ///
        /// <example-tbd></example-tbd>
        static member FromBeginEnd : arg1:'Arg1 * arg2:'Arg2 * beginAction:('Arg1 * 'Arg2 * System.AsyncCallback * obj -> System.IAsyncResult) * endAction:(System.IAsyncResult -> 'T) * ?cancelAction : (unit -> unit) -> Async<'T>

        /// <summary>Creates an asynchronous computation in terms of a Begin/End pair of actions in 
        /// the style used in .NET 2.0 APIs.</summary>
        ///
        /// <remarks>The computation will respond to cancellation while waiting for the completion
        /// of the operation. If a cancellation occurs, and <c>cancelAction</c> is specified, then it is 
        /// executed, and the computation continues to wait for the completion of the operation.
        ///
        /// If <c>cancelAction</c> is not specified, then cancellation causes the computation
        /// to stop immediately, and subsequent invocations of the callback are ignored.</remarks>
        ///
        /// <param name="arg1">The first argument for the operation.</param>
        /// <param name="arg2">The second argument for the operation.</param>
        /// <param name="arg3">The third argument for the operation.</param>
        /// <param name="beginAction">The function initiating a traditional CLI asynchronous operation.</param>
        /// <param name="endAction">The function completing a traditional CLI asynchronous operation.</param>
        /// <param name="cancelAction">An optional function to be executed when a cancellation is requested.</param>
        ///
        /// <returns>An asynchronous computation wrapping the given Begin/End functions.</returns>
        ///
        /// <category index="5">Legacy .NET Async Interoperability</category>
        ///
        /// <example-tbd></example-tbd>
        static member FromBeginEnd : arg1:'Arg1 * arg2:'Arg2 * arg3:'Arg3 * beginAction:('Arg1 * 'Arg2 * 'Arg3 * System.AsyncCallback * obj -> System.IAsyncResult) * endAction:(System.IAsyncResult -> 'T) * ?cancelAction : (unit -> unit) -> Async<'T>

        /// <summary>Creates three functions that can be used to implement the .NET 1.0 Asynchronous 
        /// Programming Model (APM) for a given asynchronous computation.</summary>
        ///
        /// <param name="computation">A function generating the asynchronous computation to split into the traditional
        /// .NET Asynchronous Programming Model.</param>
        ///
        /// <returns>A tuple of the begin, end, and cancel members.</returns>
        ///
        /// <category index="5">Legacy .NET Async Interoperability</category>
        ///
        /// <example-tbd></example-tbd>
        static member AsBeginEnd : computation:('Arg -> Async<'T>) -> 
                                     // The 'Begin' member
                                     ('Arg * System.AsyncCallback * obj -> System.IAsyncResult) * 
                                     // The 'End' member
                                     (System.IAsyncResult -> 'T) * 
                                     // The 'Cancel' member
                                     (System.IAsyncResult -> unit)

        /// <summary>Creates an asynchronous computation that runs the given computation and ignores 
        /// its result.</summary>
        ///
        /// <param name="computation">The input computation.</param>
        ///
        /// <returns>A computation that is equivalent to the input computation, but disregards the result.</returns>
        ///
        /// <category index="1">Composing Async Computations</category>
        ///
        /// <example id="ignore-1">
        /// <code lang="fsharp">
        /// let readFile filename numBytes =
        ///     async {
        ///         use file = System.IO.File.OpenRead(filename)
        ///         printfn "Reading from file %s." filename
        ///         // Throw away the data being read.
        ///         do! file.AsyncRead(numBytes) |> Async.Ignore
        ///     }
        /// readFile "example.txt" 42 |> Async.Start
        /// </code>
        /// Reads bytes from a given file asynchronously and then ignores the result, allowing the do! to be used with functions
        /// that return an unwanted value.
        /// </example>
        static member Ignore : computation: Async<'T> -> Async<unit>

        /// <summary>Runs an asynchronous computation, starting immediately on the current operating system
        /// thread. Call one of the three continuations when the operation completes.</summary>
        ///
        /// <remarks>If no cancellation token is provided then the default cancellation token
        /// is used.</remarks>
        ///
        /// <param name="computation">The asynchronous computation to execute.</param>
        /// <param name="continuation">The function called on success.</param>
        /// <param name="exceptionContinuation">The function called on exception.</param>
        /// <param name="cancellationContinuation">The function called on cancellation.</param>
        /// <param name="cancellationToken">The <c>CancellationToken</c> to associate with the computation.
        /// The default is used if this parameter is not provided.</param>
        ///
        /// <category index="0">Starting Async Computations</category>
        ///
        /// <example-tbd></example-tbd>
        static member StartWithContinuations: 
            computation:Async<'T> * 
            continuation:('T -> unit) * exceptionContinuation:(exn -> unit) * cancellationContinuation:(OperationCanceledException -> unit) *  
            ?cancellationToken:CancellationToken-> unit

        ///
        /// <example-tbd></example-tbd>
        static member internal StartWithContinuationsUsingDispatchInfo: 
            computation:Async<'T> * 
            continuation:('T -> unit) * exceptionContinuation:(ExceptionDispatchInfo -> unit) * cancellationContinuation:(OperationCanceledException -> unit) *  
            ?cancellationToken:CancellationToken-> unit

        /// <summary>Runs an asynchronous computation, starting immediately on the current operating system
        /// thread.</summary>
        ///
        /// <remarks>If no cancellation token is provided then the default cancellation token is used.</remarks>
        ///
        /// <param name="computation">The asynchronous computation to execute.</param>
        /// <param name="cancellationToken">The <c>CancellationToken</c> to associate with the computation.
        /// The default is used if this parameter is not provided.</param>
        ///
        /// <category index="0">Starting Async Computations</category>
        ///
        /// <example id="start-immediate-1">
        /// <code lang="fsharp">
        /// printfn "A"
        ///
        /// async {
        ///     printfn "B"
        ///     do! Async.Sleep(1000)
        ///     printfn "C"
        /// } |> Async.StartImmediate
        ///
        /// printfn "D"
        /// </code>
        /// Prints "A", "B", "D" immediately, then "C" in 1 second
        /// </example>
        static member StartImmediate: 
            computation:Async<unit> * ?cancellationToken:CancellationToken-> unit

        /// <summary>Runs an asynchronous computation, starting immediately on the current operating system
        /// thread, but also returns the execution as <see cref="T:System.Threading.Tasks.Task`1"/>
        /// </summary>
        ///
        /// <remarks>If no cancellation token is provided then the default cancellation token is used.
        /// You may prefer using this method if you want to achive a similar behviour to async await in C# as 
        /// async computation starts on the current thread with an ability to return a result.
        /// </remarks>
        ///
        /// <param name="computation">The asynchronous computation to execute.</param>
        /// <param name="cancellationToken">The <c>CancellationToken</c> to associate with the computation.
        /// The default is used if this parameter is not provided.</param>
        ///
        /// <returns>A <see cref="T:System.Threading.Tasks.Task"/> that will be completed
        /// in the corresponding state once the computation terminates (produces the result, throws exception or gets canceled)</returns>
        ///
        /// <category index="0">Starting Async Computations</category>
        ///
        /// <example id="start-immediate-as-task-1">
        /// <code lang="fsharp">
        /// printfn "A"
        ///
        /// let t =
        ///     async {
        ///         printfn "B"
        ///         do! Async.Sleep(1000)
        ///         printfn "C"
        ///     } |> Async.StartImmediateAsTask
        ///
        /// printfn "D"
        /// t.Wait()
        /// printfn "E"
        /// </code>
        /// Prints "A", "B", "D" immediately, then "C", "E" in 1 second.
        /// </example>
        static member StartImmediateAsTask: 
            computation:Async<'T> * ?cancellationToken:CancellationToken-> Task<'T>


    /// <summary>The F# compiler emits references to this type to implement F# async expressions.</summary>
    ///
    /// <category index="5">Async Internals</category>
    type AsyncReturn

    /// <summary>The F# compiler emits references to this type to implement F# async expressions.</summary>
    ///
    /// <category index="5">Async Internals</category>
    [<Struct; NoEquality; NoComparison>]
    type AsyncActivation<'T> =

        /// <summary>The F# compiler emits calls to this function to implement F# async expressions.</summary>
        ///
        /// <returns>A value indicating asynchronous execution.</returns>
        ///
        /// <example-tbd></example-tbd>
        member IsCancellationRequested: bool

        /// <summary>The F# compiler emits calls to this function to implement F# async expressions.</summary>
        ///
        /// <returns>A value indicating asynchronous execution.</returns>
        ///
        /// <example-tbd></example-tbd>
        static member Success: AsyncActivation<'T> -> result: 'T -> AsyncReturn

        /// <summary>The F# compiler emits calls to this function to implement F# async expressions.</summary>
        ///
        /// <returns>A value indicating asynchronous execution.</returns>
        ///
        /// <example-tbd></example-tbd>
        member OnSuccess: result: 'T -> AsyncReturn

        /// <summary>The F# compiler emits calls to this function to implement F# async expressions.</summary>
        ///
        /// <example-tbd></example-tbd>
        member OnExceptionRaised: unit -> unit

        /// <summary>The F# compiler emits calls to this function to implement F# async expressions.</summary>
        ///
        /// <returns>A value indicating asynchronous execution.</returns>
        ///
        /// <example-tbd></example-tbd>
        member OnCancellation: unit -> AsyncReturn

        /// Used by MailboxProcessor
        member internal QueueContinuationWithTrampoline: 'T -> AsyncReturn

        /// Used by MailboxProcessor
        member internal CallContinuation: 'T -> AsyncReturn

    [<NoEquality; NoComparison>]
    // Internals used by MailboxProcessor
    type internal AsyncResult<'T>  =
        | Ok of 'T
        | Error of ExceptionDispatchInfo
        | Canceled of OperationCanceledException

    /// <summary>Entry points for generated code</summary>
    ///
    /// <category index="5">Async Internals</category>
    [<Sealed>]
    module AsyncPrimitives =

        /// <summary>The F# compiler emits calls to this function to implement F# async expressions.</summary>
        ///
        /// <param name="body">The body of the async computation.</param>
        ///
        /// <returns>The async computation.</returns>
        val MakeAsync: body:(AsyncActivation<'T> -> AsyncReturn) -> Async<'T>

        /// <summary>The F# compiler emits calls to this function to implement constructs for F# async expressions.</summary>
        ///
        /// <param name="computation">The async computation.</param>
        /// <param name="ctxt">The async activation.</param>
        ///
        /// <returns>A value indicating asynchronous execution.</returns>
        val Invoke: computation: Async<'T> -> ctxt:AsyncActivation<'T> -> AsyncReturn

        /// <summary>The F# compiler emits calls to this function to implement constructs for F# async expressions.</summary>
        ///
        /// <param name="ctxt">The async activation.</param>
        /// <param name="result1">The result of the first part of the computation.</param>
        /// <param name="part2">A function returning the second part of the computation.</param>
        ///
        /// <returns>A value indicating asynchronous execution.</returns>
        val CallThenInvoke: ctxt:AsyncActivation<'T> -> result1:'U -> part2:('U -> Async<'T>) -> AsyncReturn

        /// <summary>The F# compiler emits calls to this function to implement the <c>let!</c> construct for F# async expressions.</summary>
        ///
        /// <param name="ctxt">The async activation.</param>
        /// <param name="part1">The first part of the computation.</param>
        /// <param name="part2">A function returning the second part of the computation.</param>
        ///
        /// <returns>An async activation suitable for running part1 of the asynchronous execution.</returns>
        val Bind: ctxt:AsyncActivation<'T> -> part1:Async<'U> -> part2:('U -> Async<'T>) -> AsyncReturn

        /// <summary>The F# compiler emits calls to this function to implement the <c>try/finally</c> construct for F# async expressions.</summary>
        ///
        /// <param name="ctxt">The async activation.</param>
        /// <param name="computation">The computation to protect.</param>
        /// <param name="finallyFunction">The finally code.</param>
        ///
        /// <returns>A value indicating asynchronous execution.</returns>
        val TryFinally: ctxt:AsyncActivation<'T> -> computation: Async<'T> -> finallyFunction: (unit -> unit) -> AsyncReturn

        /// <summary>The F# compiler emits calls to this function to implement the <c>try/with</c> construct for F# async expressions.</summary>
        ///
        /// <param name="ctxt">The async activation.</param>
        /// <param name="computation">The computation to protect.</param>
        /// <param name="catchFunction">The exception filter.</param>
        ///
        /// <returns>A value indicating asynchronous execution.</returns>
        val TryWith: ctxt:AsyncActivation<'T> -> computation: Async<'T> -> catchFunction: (Exception -> Async<'T> option) -> AsyncReturn

        [<Sealed; AutoSerializable(false)>]        
        // Internals used by MailboxProcessor
        type internal ResultCell<'T> =
            new : unit -> ResultCell<'T>
            member GetWaitHandle: unit -> WaitHandle
            member Close: unit -> unit
            interface IDisposable
            member RegisterResult: 'T * reuseThread: bool -> AsyncReturn
            member GrabResult: unit -> 'T
            member ResultAvailable : bool
            member AwaitResult_NoDirectCancelOrTimeout : Async<'T>
            member TryWaitForResultSynchronously: ?timeout: int -> 'T option

        // Internals used by MailboxProcessor
        val internal CreateAsyncResultAsync : AsyncResult<'T> -> Async<'T>

    /// <summary>The type of the <c>async</c> operator, used to build workflows for asynchronous computations.</summary>
    ///
    /// <category index="1">Async Programming</category>
    [<CompiledName("FSharpAsyncBuilder")>]
    [<Sealed>]
    type AsyncBuilder =
        /// <summary>Creates an asynchronous computation that enumerates the sequence <c>seq</c>
        /// on demand and runs <c>body</c> for each element.</summary>
        ///
        /// <remarks>A cancellation check is performed on each iteration of the loop.
        ///
        /// The existence of this method permits the use of <c>for</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        ///
        /// <param name="sequence">The sequence to enumerate.</param>
        /// <param name="body">A function to take an item from the sequence and create
        /// an asynchronous computation.  Can be seen as the body of the <c>for</c> expression.</param>
        ///
        /// <returns>An asynchronous computation that will enumerate the sequence and run <c>body</c>
        /// for each element.</returns>
        ///
        /// <example-tbd></example-tbd>
        member For: sequence:seq<'T> * body:('T -> Async<unit>) -> Async<unit>

        /// <summary>Creates an asynchronous computation that just returns <c>()</c>.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of empty <c>else</c> branches in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        /// <returns>An asynchronous computation that returns <c>()</c>.</returns>
        ///
        /// <example-tbd></example-tbd>
        member Zero : unit -> Async<unit> 

        /// <summary>Creates an asynchronous computation that first runs <c>computation1</c>
        /// and then runs <c>computation2</c>, returning the result of <c>computation2</c>.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of expression sequencing in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        ///
        /// <param name="computation1">The first part of the sequenced computation.</param>
        /// <param name="computation2">The second part of the sequenced computation.</param>
        ///
        /// <returns>An asynchronous computation that runs both of the computations sequentially.</returns>
        ///
        /// <example-tbd></example-tbd>
        member inline Combine : computation1:Async<unit> * computation2:Async<'T> -> Async<'T>

        /// <summary>Creates an asynchronous computation that runs <c>computation</c> repeatedly 
        /// until <c>guard()</c> becomes false.</summary>
        ///
        /// <remarks>A cancellation check is performed whenever the computation is executed.
        ///
        /// The existence of this method permits the use of <c>while</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        ///
        /// <param name="guard">The function to determine when to stop executing <c>computation</c>.</param>
        /// <param name="computation">The function to be executed.  Equivalent to the body
        /// of a <c>while</c> expression.</param>
        ///
        /// <returns>An asynchronous computation that behaves similarly to a while loop when run.</returns>
        ///
        /// <example-tbd></example-tbd>
        member While : guard:(unit -> bool) * computation:Async<unit> -> Async<unit>

        /// <summary>Creates an asynchronous computation that returns the result <c>v</c>.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of <c>return</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        ///
        /// <param name="value">The value to return from the computation.</param>
        ///
        /// <returns>An asynchronous computation that returns <c>value</c> when executed.</returns>
        ///
        /// <example-tbd></example-tbd>
        member inline Return : value:'T -> Async<'T>

        /// <summary>Delegates to the input computation.</summary>
        ///
        /// <remarks>The existence of this method permits the use of <c>return!</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        ///
        /// <param name="computation">The input computation.</param>
        ///
        /// <returns>The input computation.</returns>
        ///
        /// <example-tbd></example-tbd>
        member inline ReturnFrom : computation:Async<'T> -> Async<'T>

        /// <summary>Creates an asynchronous computation that runs <c>generator</c>.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.</remarks>
        ///
        /// <param name="generator">The function to run.</param>
        ///
        /// <returns>An asynchronous computation that runs <c>generator</c>.</returns>
        ///
        /// <example-tbd></example-tbd>
        member Delay : generator:(unit -> Async<'T>) -> Async<'T>

        /// <summary>Creates an asynchronous computation that runs <c>binder(resource)</c>. 
        /// The action <c>resource.Dispose()</c> is executed as this computation yields its result
        /// or if the asynchronous computation exits by an exception or by cancellation.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of <c>use</c> and <c>use!</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        ///
        /// <param name="resource">The resource to be used and disposed.</param>
        /// <param name="binder">The function that takes the resource and returns an asynchronous
        /// computation.</param>
        ///
        /// <returns>An asynchronous computation that binds and eventually disposes <c>resource</c>.</returns>
        ///
        /// <example-tbd></example-tbd>
        member Using: resource:'T * binder:('T -> Async<'U>) -> Async<'U> when 'T :> System.IDisposable

        /// <summary>Creates an asynchronous computation that runs <c>computation</c>, and when 
        /// <c>computation</c> generates a result <c>T</c>, runs <c>binder res</c>.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of <c>let!</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        ///
        /// <param name="computation">The computation to provide an unbound result.</param>
        /// <param name="binder">The function to bind the result of <c>computation</c>.</param>
        ///
        /// <returns>An asynchronous computation that performs a monadic bind on the result
        /// of <c>computation</c>.</returns>
        ///
        /// <example-tbd></example-tbd>
        member inline Bind: computation: Async<'T> * binder: ('T -> Async<'U>) -> Async<'U>
        
        /// <summary>Creates an asynchronous computation that runs <c>computation</c>. The action <c>compensation</c> is executed 
        /// after <c>computation</c> completes, whether <c>computation</c> exits normally or by an exception. If <c>compensation</c> raises an exception itself
        /// the original exception is discarded and the new exception becomes the overall result of the computation.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of <c>try/finally</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        ///
        /// <param name="computation">The input computation.</param>
        /// <param name="compensation">The action to be run after <c>computation</c> completes or raises an
        /// exception (including cancellation).</param>
        ///
        /// <returns>An asynchronous computation that executes computation and compensation afterwards or
        /// when an exception is raised.</returns>
        ///
        /// <example-tbd></example-tbd>
        member inline TryFinally : computation:Async<'T> * compensation:(unit -> unit) -> Async<'T>

        /// <summary>Creates an asynchronous computation that runs <c>computation</c> and returns its result.
        /// If an exception happens then <c>catchHandler(exn)</c> is called and the resulting computation executed instead.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of <c>try/with</c> in the 
        /// <c>async { ... }</c> computation expression syntax.</remarks>
        ///
        /// <param name="computation">The input computation.</param>
        /// <param name="catchHandler">The function to run when <c>computation</c> throws an exception.</param>
        ///
        /// <returns>An asynchronous computation that executes <c>computation</c> and calls <c>catchHandler</c> if an
        /// exception is thrown.</returns>
        ///
        /// <example-tbd></example-tbd>
        member inline TryWith : computation:Async<'T> * catchHandler:(exn -> Async<'T>) -> Async<'T>

        // member inline TryWithFilter : computation:Async<'T> * catchHandler:(exn -> Async<'T> option) -> Async<'T>

        /// Generate an object used to build asynchronous computations using F# computation expressions. The value
        /// 'async' is a pre-defined instance of this type.
        ///
        /// A cancellation check is performed when the computation is executed.
        internal new : unit -> AsyncBuilder

    /// <summary>A module of extension members providing asynchronous operations for some basic CLI types related to concurrency and I/O.</summary>
    ///
    /// <category index="1">Async Programming</category>
    [<AutoOpen>]
    module CommonExtensions =
        
        type System.IO.Stream with 
            
            /// <summary>Returns an asynchronous computation that will read from the stream into the given buffer.</summary>
            /// <param name="buffer">The buffer to read into.</param>
            /// <param name="offset">An optional offset as a number of bytes in the stream.</param>
            /// <param name="count">An optional number of bytes to read from the stream.</param>
            ///
            /// <returns>An asynchronous computation that will read from the stream into the given buffer.</returns>
            ///
            /// <exception cref="T:System.ArgumentException">Thrown when the sum of offset and count is longer than
            /// the buffer length.</exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">Thrown when offset or count is negative.</exception>
            /// 
            /// <example-tbd></example-tbd>
            [<CompiledName("AsyncRead")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member AsyncRead : buffer:byte[] * ?offset:int * ?count:int -> Async<int>
            
            /// <summary>Returns an asynchronous computation that will read the given number of bytes from the stream.</summary>
            ///
            /// <param name="count">The number of bytes to read.</param>
            ///
            /// <returns>An asynchronous computation that returns the read byte[] when run.</returns> 
            /// 
            /// <example-tbd></example-tbd>
            [<CompiledName("AsyncReadBytes")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member AsyncRead : count:int -> Async<byte[]>
            
            /// <summary>Returns an asynchronous computation that will write the given bytes to the stream.</summary>
            ///
            /// <param name="buffer">The buffer to write from.</param>
            /// <param name="offset">An optional offset as a number of bytes in the stream.</param>
            /// <param name="count">An optional number of bytes to write to the stream.</param>
            ///
            /// <returns>An asynchronous computation that will write the given bytes to the stream.</returns>
            ///
            /// <exception cref="T:System.ArgumentException">Thrown when the sum of offset and count is longer than
            /// the buffer length.</exception>
            /// <exception cref="T:System.ArgumentOutOfRangeException">Thrown when offset or count is negative.</exception>
            /// 
            /// <example-tbd></example-tbd>
            [<CompiledName("AsyncWrite")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member AsyncWrite : buffer:byte[] * ?offset:int * ?count:int -> Async<unit>


        ///<summary>The family of first class event values for delegate types that satisfy the F# delegate constraint.</summary>
        type IObservable<'T> with
            /// <summary>Permanently connects a listener function to the observable. The listener will
            /// be invoked for each observation.</summary>
            ///
            /// <param name="callback">The function to be called for each observation.</param>
            /// 
            /// <example-tbd></example-tbd>
            [<CompiledName("AddToObservable")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member Add: callback:('T -> unit) -> unit

            /// <summary>Connects a listener function to the observable. The listener will
            /// be invoked for each observation. The listener can be removed by
            /// calling Dispose on the returned IDisposable object.</summary>
            ///
            /// <param name="callback">The function to be called for each observation.</param>
            ///
            /// <returns>An object that will remove the listener if disposed.</returns>
            /// 
            /// <example-tbd></example-tbd>
            [<CompiledName("SubscribeToObservable")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member Subscribe: callback:('T -> unit) -> System.IDisposable

    /// <summary>A module of extension members providing asynchronous operations for some basic Web operations.</summary>
    ///
    /// <category index="1">Async Programming</category>
    [<AutoOpen>]
    module WebExtensions = 

        type System.Net.WebRequest with 
            /// <summary>Returns an asynchronous computation that, when run, will wait for a response to the given WebRequest.</summary>
            /// <returns>An asynchronous computation that waits for response to the <c>WebRequest</c>.</returns>
            /// 
            /// <example id="get-response">
            /// <code lang="fsharp">
            /// open System.Net
            /// open System.IO
            /// let responseStreamToString = fun (responseStream : WebResponse) ->
            ///     let reader = new StreamReader(responseStream.GetResponseStream())
            ///     reader.ReadToEnd()
            /// let webRequest = WebRequest.Create("https://www.w3.org")
            /// let result = webRequest.AsyncGetResponse() |> Async.RunSynchronously |> responseStreamToString
            /// </code>
            /// </example>
            /// Gets the web response asynchronously and converts response stream to string
            [<CompiledName("AsyncGetResponse")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member AsyncGetResponse : unit -> Async<System.Net.WebResponse>

        type System.Net.WebClient with

            /// <summary>Returns an asynchronous computation that, when run, will wait for the download of the given URI.</summary>
            ///
            /// <param name="address">The URI to retrieve.</param>
            ///
            /// <returns>An asynchronous computation that will wait for the download of the URI.</returns>
            /// 
            /// <example id="async-download-string">
            /// <code lang="fsharp">
            /// open System
            /// let client = new WebClient()
            /// Uri("https://www.w3.org") |> client.AsyncDownloadString |> Async.RunSynchronously
            /// </code>
            /// This will download the server response from https://www.w3.org
            /// </example>
            [<CompiledName("AsyncDownloadString")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member AsyncDownloadString : address:System.Uri -> Async<string>

            /// <summary>Returns an asynchronous computation that, when run, will wait for the download of the given URI.</summary>
            ///
            /// <param name="address">The URI to retrieve.</param>
            ///
            /// <returns>An asynchronous computation that will wait for the download of the URI.</returns>
            /// 
            /// <example id="async-download-data">
            /// <code lang="fsharp">
            /// open System.Net
            /// open System.Text
            /// open System
            /// let client = new WebClient()
            /// client.AsyncDownloadData(Uri("https://www.w3.org")) |> Async.RunSynchronously |> Encoding.ASCII.GetString 
            /// </code>
            /// </example>
            /// Downloads the data in bytes and decodes it to a string.
            [<CompiledName("AsyncDownloadData")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member AsyncDownloadData : address:System.Uri -> Async<byte[]>

            /// <summary>Returns an asynchronous computation that, when run, will wait for the download of the given URI to specified file.</summary>
            ///
            /// <param name="address">The URI to retrieve.</param>
            /// <param name="fileName">The file name to save download to.</param>
            ///
            /// <returns>An asynchronous computation that will wait for the download of the URI to specified file.</returns>
            /// 
            /// <example id="async-download-file">
            /// <code lang="fsharp">
            /// open System.Net
            /// open System
            /// let client = new WebClient()
            /// Uri("https://www.w3.com") |> fun x -> client.AsyncDownloadFile(x, "output.html") |> Async.RunSynchronously
            /// </code>
            /// This will download the server response as a file and output it as output.html
            /// </example>
            [<CompiledName("AsyncDownloadFile")>] // give the extension member a nice, unmangled compiled name, unique within this module
            member AsyncDownloadFile : address:System.Uri * fileName: string -> Async<unit>

    // Internals used by MailboxProcessor
    module internal AsyncBuilderImpl = 
        val async : AsyncBuilder

