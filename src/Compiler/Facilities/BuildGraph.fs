// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal FSharp.Compiler.BuildGraph

open System
open System.Threading
open System.Threading.Tasks
open System.Diagnostics
open System.Globalization
open FSharp.Compiler.DiagnosticsLogger
open Internal.Utilities.Library
open System.Runtime.CompilerServices
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Collections

#nowarn "3513"

[<NoComparison; NoEquality>]
type VolatileBarrier() =
    [<VolatileField>]
    let mutable isStopped = false

    member _.Proceed = not isStopped
    member _.Stop() = isStopped <- true

/// A type that looks like an Awaiter
type Awaiter<'Awaiter, 'TResult
    when 'Awaiter :> ICriticalNotifyCompletion
    and 'Awaiter: (member IsCompleted: bool)
    and 'Awaiter: (member GetResult: unit -> 'TResult)> = 'Awaiter

/// A type that looks like an Awaitable
type Awaitable<'Awaitable, 'Awaiter, 'TResult
    when 'Awaitable: (member GetAwaiter: unit -> Awaiter<'Awaiter, 'TResult>)> = 'Awaitable

/// Functions for Awaiters
module Awaiter =
    /// Gets a value that indicates whether the asynchronous task has completed
    let inline isCompleted<'Awaiter, 'TResult when Awaiter<'Awaiter, 'TResult>> (x: 'Awaiter) =
        x.IsCompleted

    /// Ends the wait for the completion of the asynchronous task.
    let inline getResult<'Awaiter, 'TResult when Awaiter<'Awaiter, 'TResult>> (x: 'Awaiter) =
        x.GetResult()

/// Functions for Awaitables
module Awaitable =
    /// Creates an awaiter for this value.
    let inline getAwaiter<'Awaitable, 'Awaiter, 'TResult
        when Awaitable<'Awaitable, 'Awaiter, 'TResult>>
        (x: 'Awaitable)
        =
        x.GetAwaiter()

/// CancellationToken -> Task<'T>
type NodeCode<'T> = CancellationToken -> Task<'T>

/// CancellationToken -> Task
type NodeCode = CancellationToken -> Task

/// The extra data stored in ResumableStateMachine for tasks
[<Struct; NoComparison; NoEquality>]
type NodeCodeStateMachineData<'T> =
    [<DefaultValue(false)>]
    val mutable CancellationToken: CancellationToken

    [<DefaultValue(false)>]
    val mutable Result: 'T

    [<DefaultValue(false)>]
    val mutable MethodBuilder: AsyncTaskMethodBuilder<'T>

    member inline this.ThrowIfCancellationRequested() =
        this.CancellationToken.ThrowIfCancellationRequested()

/// This is used by the compiler as a template for creating state machine structs
and NodeCodeStateMachine<'TOverall> =
    ResumableStateMachine<NodeCodeStateMachineData<'TOverall>>

/// Represents the runtime continuation of a CancellableTask state machine created dynamically
and NodeCodeResumptionFunc<'TOverall> =
    ResumptionFunc<NodeCodeStateMachineData<'TOverall>>

/// Represents the runtime continuation of a CancellableTask state machine created dynamically
and NodeCodeResumptionDynamicInfo<'TOverall> =
    ResumptionDynamicInfo<NodeCodeStateMachineData<'TOverall>>

/// A special compiler-recognised delegate type for specifying blocks of CancellableTask code with access to the state machine
and NodeCodeCode<'TOverall, 'T> =
    ResumableCode<NodeCodeStateMachineData<'TOverall>, 'T>


/// Contains methods to build CancellableTasks using the F# computation expression syntax
[<NoComparison; NoEquality>]
type NodeCodeBuilderBase() =

    /// <summary>Creates a CancellableTask that runs generator</summary>
    /// <param name="generator">The function to run</param>
    /// <returns>A node that runs generator</returns>
    member inline _.Delay
        ([<InlineIfLambda>] generator: unit -> NodeCodeCode<'TOverall, 'T>)
        : NodeCodeCode<'TOverall, 'T> =
        ResumableCode.Delay(fun () ->
            NodeCodeCode(fun sm ->
                sm.Data.ThrowIfCancellationRequested()
                (generator ()).Invoke(&sm)
            )
        )


    /// <summary>Creates an NodeCode that just returns ().</summary>
    /// <remarks>
    /// The existence of this method permits the use of empty else branches in the
    /// node { ... } computation expression syntax.
    /// </remarks>
    /// <returns>An NodeCode that returns ().</returns>
    [<DefaultValue>]
    member inline _.Zero() : NodeCodeCode<'TOverall, unit> = ResumableCode.Zero()

    /// <summary>Creates an computation that returns the result v.</summary>
    ///
    /// <remarks>A cancellation check is performed when the computation is executed.
    ///
    /// The existence of this method permits the use of return in the
    /// node { ... } computation expression syntax.</remarks>
    ///
    /// <param name="value">The value to return from the computation.</param>
    ///
    /// <returns>An NodeCode that returns value when executed.</returns>
    member inline _.Return(value: 'T) : NodeCodeCode<'T, 'T> =
        NodeCodeCode<'T, _>(fun sm ->
            sm.Data.ThrowIfCancellationRequested()
            sm.Data.Result <- value
            true
        )

    /// <summary>Creates an NodeCode that first runs task1
    /// and then runs computation2, returning the result of computation2.</summary>
    ///
    /// <remarks>
    ///
    /// The existence of this method permits the use of expression sequencing in the
    /// node { ... } computation expression syntax.</remarks>
    ///
    /// <param name="task1">The first part of the sequenced computation.</param>
    /// <param name="task2">The second part of the sequenced computation.</param>
    ///
    /// <returns>An NodeCode that runs both of the computations sequentially.</returns>
    member inline _.Combine
        (
            task1: NodeCodeCode<'TOverall, unit>,
            task2: NodeCodeCode<'TOverall, 'T>
        ) : NodeCodeCode<'TOverall, 'T> =
        ResumableCode.Combine(
            NodeCodeCode(fun sm ->
                sm.Data.ThrowIfCancellationRequested()
                task1.Invoke(&sm)
            ),

            NodeCodeCode(fun sm ->
                sm.Data.ThrowIfCancellationRequested()
                task2.Invoke(&sm)
            )
        )

    /// <summary>Creates an NodeCode that runs computation repeatedly
    /// until guard() becomes false.</summary>
    ///
    /// <remarks>
    ///
    /// The existence of this method permits the use of while in the
    /// node { ... } computation expression syntax.</remarks>
    ///
    /// <param name="guard">The function to determine when to stop executing computation.</param>
    /// <param name="computation">The function to be executed.  Equivalent to the body
    /// of a while expression.</param>
    ///
    /// <returns>An NodeCode that behaves similarly to a while loop when run.</returns>
    member inline _.While
        (
            [<InlineIfLambda>] guard: unit -> bool,
            computation: NodeCodeCode<'TOverall, unit>
        ) : NodeCodeCode<'TOverall, unit> =
        ResumableCode.While(
            guard,
            NodeCodeCode(fun sm ->
                sm.Data.ThrowIfCancellationRequested()
                computation.Invoke(&sm)
            )
        )

    /// <summary>Creates an NodeCode that runs computation and returns its result.
    /// If an exception happens then catchHandler(exn) is called and the resulting computation executed instead.</summary>
    ///
    /// <remarks>
    ///
    /// The existence of this method permits the use of try/with in the
    /// node { ... } computation expression syntax.</remarks>
    ///
    /// <param name="computation">The input computation.</param>
    /// <param name="catchHandler">The function to run when computation throws an exception.</param>
    ///
    /// <returns>An NodeCode that executes computation and calls catchHandler if an
    /// exception is thrown.</returns>
    member inline _.TryWith
        (
            computation: NodeCodeCode<'TOverall, 'T>,
            [<InlineIfLambda>] catchHandler: exn -> NodeCodeCode<'TOverall, 'T>
        ) : NodeCodeCode<'TOverall, 'T> =
        ResumableCode.TryWith(
            NodeCodeCode(fun sm ->
                sm.Data.ThrowIfCancellationRequested()
                computation.Invoke(&sm)
            ),
            catchHandler
        )

    /// <summary>Creates an NodeCode that runs computation. The action compensation is executed
    /// after computation completes, whether computation exits normally or by an exception. If compensation raises an exception itself
    /// the original exception is discarded and the new exception becomes the overall result of the computation.</summary>
    ///
    /// <remarks>
    ///
    /// The existence of this method permits the use of try/finally in the
    /// node { ... } computation expression syntax.</remarks>
    ///
    /// <param name="computation">The input computation.</param>
    /// <param name="compensation">The action to be run after computation completes or raises an
    /// exception (including cancellation).</param>
    ///
    /// <returns>An NodeCode that executes computation and compensation afterwards or
    /// when an exception is raised.</returns>
    member inline _.TryFinally
        (
            computation: NodeCodeCode<'TOverall, 'T>,
            [<InlineIfLambda>] compensation: unit -> unit
        ) : NodeCodeCode<'TOverall, 'T> =
        ResumableCode.TryFinally(

            NodeCodeCode(fun sm ->
                sm.Data.ThrowIfCancellationRequested()
                computation.Invoke(&sm)
            ),
            ResumableCode<_, _>(fun _ ->
                compensation ()
                true
            )
        )

    /// <summary>Creates an NodeCode that enumerates the sequence seq
    /// on demand and runs body for each element.</summary>
    ///
    /// <remarks>A cancellation check is performed on each iteration of the loop.
    ///
    /// The existence of this method permits the use of for in the
    /// node { ... } computation expression syntax.</remarks>
    ///
    /// <param name="sequence">The sequence to enumerate.</param>
    /// <param name="body">A function to take an item from the sequence and create
    /// an NodeCode.  Can be seen as the body of the for expression.</param>
    ///
    /// <returns>An NodeCode that will enumerate the sequence and run body
    /// for each element.</returns>
    member inline _.For
        (
            sequence: seq<'T>,
            [<InlineIfLambda>] body: 'T -> NodeCodeCode<'TOverall, unit>
        ) : NodeCodeCode<'TOverall, unit> =
        ResumableCode.For(
            sequence,
            fun item ->
                NodeCodeCode(fun sm ->
                    sm.Data.ThrowIfCancellationRequested()
                    (body item).Invoke(&sm)
                )
        )

/// Contains methods to build NodeCodes using the F# computation expression syntax
[<Sealed; NoComparison; NoEquality>]
type NodeCodeBuilder(runOnBackground: bool) =

    inherit NodeCodeBuilderBase()

    member val IsBackground = runOnBackground

    // This is the dynamic implementation - this is not used
    // for statically compiled tasks.  An executor (resumptionFuncExecutor) is
    // registered with the state machine, plus the initial resumption.
    // The executor stays constant throughout the execution, it wraps each step
    // of the execution in a try/with.  The resumption is changed at each step
    // to represent the continuation of the computation.
    /// <summary>
    /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
    /// </summary>
    static member inline RunDynamicAux(code: NodeCodeCode<'T, 'T>) : NodeCode<'T> =

        let mutable sm = NodeCodeStateMachine<'T>()

        let initialResumptionFunc =
            NodeCodeResumptionFunc<'T>(fun sm -> code.Invoke(&sm))

        let resumptionInfo =
            { new NodeCodeResumptionDynamicInfo<'T>(initialResumptionFunc) with
                member info.MoveNext(sm) =
                    let mutable savedExn = null

                    try
                        sm.ResumptionDynamicInfo.ResumptionData <- null
                        let step = info.ResumptionFunc.Invoke(&sm)

                        if step then
                            sm.Data.MethodBuilder.SetResult(sm.Data.Result)
                        else
                            let mutable awaiter =
                                sm.ResumptionDynamicInfo.ResumptionData
                                :?> ICriticalNotifyCompletion

                            assert not (isNull awaiter)
                            sm.Data.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)

                    with exn ->
                        savedExn <- exn
                    // Run SetException outside the stack unwind, see https://github.com/dotnet/roslyn/issues/26567
                    match savedExn with
                    | null -> ()
                    | exn -> sm.Data.MethodBuilder.SetException exn

                member _.SetStateMachine(sm, state) =
                    sm.Data.MethodBuilder.SetStateMachine(state)
            }

        fun (ct) ->
            if ct.IsCancellationRequested then
                Task.FromCanceled<_>(ct)
            else
                sm.Data.CancellationToken <- ct
                sm.ResumptionDynamicInfo <- resumptionInfo
                sm.Data.MethodBuilder <- AsyncTaskMethodBuilder<'T>.Create()
                sm.Data.MethodBuilder.Start(&sm)
                sm.Data.MethodBuilder.Task

    /// <summary>
    /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
    /// </summary>
    static member inline RunDynamic(code: NodeCodeCode<'T, 'T>, runOnBackground: bool) : NodeCode<'T> =
        // When runOnBackground is true, task escapes to a background thread where necessary
        // See spec of ConfigureAwait(false) at https://devblogs.microsoft.com/dotnet/configureawait-faq/

        if runOnBackground
            && not (isNull SynchronizationContext.Current
                    && obj.ReferenceEquals(TaskScheduler.Current, TaskScheduler.Default))
        then
            fun (ct) ->
                // Warning: this will always try to yield even if on thread pool already.
                Task.Run<'T>((fun () -> NodeCodeBuilder.RunDynamicAux (code) (ct)), ct)
        else
            NodeCodeBuilder.RunDynamicAux(code)


    /// Hosts the task code in a state machine and starts the task.
    member inline this.Run(code: NodeCodeCode<'T, 'T>) : NodeCode<'T> =
        if __useResumableCode then
            __stateMachine<NodeCodeStateMachineData<'T>, NodeCode<'T>>
                (MoveNextMethodImpl<_>(fun sm ->
                    //-- RESUMABLE CODE START
                    __resumeAt sm.ResumptionPoint
                    let mutable __stack_exn: Exception = null

                    try
                        let __stack_code_fin = code.Invoke(&sm)

                        if __stack_code_fin then
                            sm.Data.MethodBuilder.SetResult(sm.Data.Result)
                    with exn ->
                        __stack_exn <- exn
                    // Run SetException outside the stack unwind, see https://github.com/dotnet/roslyn/issues/26567
                    match __stack_exn with
                    | null -> ()
                    | exn -> sm.Data.MethodBuilder.SetException exn
                //-- RESUMABLE CODE END
                ))
                (SetStateMachineMethodImpl<_>(fun sm state ->
                    sm.Data.MethodBuilder.SetStateMachine(state)
                ))
                (AfterCode<_, _>(fun sm ->
                    if this.IsBackground
                        && not (isNull SynchronizationContext.Current
                                && obj.ReferenceEquals(TaskScheduler.Current, TaskScheduler.Default))
                    then

                        let sm = sm // copy contents of state machine so we can capture it

                        fun (ct) ->
                            if ct.IsCancellationRequested then
                                Task.FromCanceled<_>(ct)
                            else
                                let diagnosticsLogger = DiagnosticsThreadStatics.DiagnosticsLogger
                                let phase = DiagnosticsThreadStatics.BuildPhase

                                try
                                    // Warning: this will always try to yield even if on thread pool already.
                                    Task.Run<'T>(
                                        (fun () ->
                                            let mutable sm = sm // host local mutable copy of contents of state machine on this thread pool thread
                                            sm.Data.CancellationToken <- ct

                                            sm.Data.MethodBuilder <-
                                                AsyncTaskMethodBuilder<'T>.Create()

                                            sm.Data.MethodBuilder.Start(&sm)
                                            sm.Data.MethodBuilder.Task
                                        ),
                                        ct
                                    )
                                finally
                                    DiagnosticsThreadStatics.DiagnosticsLogger <- diagnosticsLogger
                                    DiagnosticsThreadStatics.BuildPhase <- phase
                                
                    else
                        let mutable sm = sm

                        fun (ct) ->
                            if ct.IsCancellationRequested then
                                Task.FromCanceled<_>(ct)
                            else
                                sm.Data.CancellationToken <- ct
                                sm.Data.MethodBuilder <- AsyncTaskMethodBuilder<'T>.Create()
                                sm.Data.MethodBuilder.Start(&sm)
                                sm.Data.MethodBuilder.Task
                ))
        else
            NodeCodeBuilder.RunDynamic(code, this.IsBackground)

/// <summary>
/// Builds a node using computation expression syntax.
/// Default behaviour when binding (v)options is to return a cacnelled task.
/// </summary>
let foregroundNode = NodeCodeBuilder(false)

/// <summary>
/// Builds a node using computation expression syntax which switches to execute on a background thread if not already doing so.
/// Default behaviour when binding (v)options is to return a cacnelled task.
/// </summary>
let node = NodeCodeBuilder(true)

/// <exclude />
[<AutoOpen>]
module LowPriority =
    // Low priority extensions
    type NodeCodeBuilderBase with

        /// <summary>
        /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
        /// </summary>
        [<NoEagerConstraintApplication>]
        static member inline BindDynamic<'TResult1, 'TResult2, 'Awaiter, 'TOverall
            when Awaiter<'Awaiter, 'TResult1>>
            (
                sm: byref<ResumableStateMachine<NodeCodeStateMachineData<'TOverall>>>,
                [<InlineIfLambda>] getAwaiter: CancellationToken -> 'Awaiter,
                [<InlineIfLambda>] continuation: ('TResult1 -> NodeCodeCode<'TOverall, 'TResult2>)
            ) : bool =
            sm.Data.ThrowIfCancellationRequested()

            let mutable awaiter = getAwaiter sm.Data.CancellationToken

            let cont: NodeCodeResumptionFunc<'TOverall> =
                (NodeCodeResumptionFunc<'TOverall>(fun (sm: byref<ResumableStateMachine<NodeCodeStateMachineData<'TOverall>>>) ->
                    let result: 'TResult1 = Awaiter.getResult awaiter
                    (continuation result).Invoke(&sm)
                ))

            // shortcut to continue immediately
            if Awaiter.isCompleted awaiter then
                cont.Invoke(&sm)
            else
                sm.ResumptionDynamicInfo.ResumptionData <-
                    (awaiter :> ICriticalNotifyCompletion)

                sm.ResumptionDynamicInfo.ResumptionFunc <- cont
                false

        /// <summary>Creates an NodeCode that runs computation, and when
        /// computation generates a result T, runs binder res.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of let! in the
        /// node { ... } computation expression syntax.</remarks>
        ///
        /// <param name="getAwaiter">The computation to provide an unbound result.</param>
        /// <param name="continuation">The function to bind the result of computation.</param>
        ///
        /// <returns>An NodeCode that performs a monadic bind on the result
        /// of computation.</returns>
        [<NoEagerConstraintApplication>]
        member inline _.Bind<'TResult1, 'TResult2, 'Awaiter, 'TOverall
            when Awaiter<'Awaiter, 'TResult1>>
            (
                [<InlineIfLambda>] getAwaiter: CancellationToken -> 'Awaiter,
                [<InlineIfLambda>] continuation: ('TResult1 -> NodeCodeCode<'TOverall, 'TResult2>)
            ) : NodeCodeCode<'TOverall, 'TResult2> =

            NodeCodeCode<'TOverall, _>(fun sm ->
                if __useResumableCode then
                    let diagnosticsLogger = DiagnosticsThreadStatics.DiagnosticsLogger
                    let phase = DiagnosticsThreadStatics.BuildPhase
                    
                    try
                        //-- RESUMABLE CODE START
                        sm.Data.ThrowIfCancellationRequested()
                        // Get an awaiter from the Awaiter
                        let mutable awaiter = getAwaiter sm.Data.CancellationToken

                        let mutable __stack_fin = true

                        if not (Awaiter.isCompleted awaiter) then
                            // This will yield with __stack_yield_fin = false
                            // This will resume with __stack_yield_fin = true
                            let __stack_yield_fin = ResumableCode.Yield().Invoke(&sm)
                            __stack_fin <- __stack_yield_fin

                        if __stack_fin then
                            let result =
                                awaiter
                                |> Awaiter.getResult

                            (continuation result).Invoke(&sm)
                        else
                            sm.Data.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                            false
                    finally
                        DiagnosticsThreadStatics.DiagnosticsLogger <- diagnosticsLogger
                        DiagnosticsThreadStatics.BuildPhase <- phase
                else
                    NodeCodeBuilderBase.BindDynamic<'TResult1, 'TResult2, 'Awaiter, 'TOverall>(
                        &sm,
                        getAwaiter,
                        continuation
                    )
            //-- RESUMABLE CODE END
            )


        /// <summary>Delegates to the input computation.</summary>
        ///
        /// <remarks>The existence of this method permits the use of return! in the
        /// node { ... } computation expression syntax.</remarks>
        ///
        /// <param name="getAwaiter">The input computation.</param>
        ///
        /// <returns>The input computation.</returns>
        [<NoEagerConstraintApplication>]
        member inline this.ReturnFrom<'TResult1, 'TResult2, 'Awaiter, 'TOverall
            when Awaiter<'Awaiter, 'TResult1>>
            ([<InlineIfLambda>] getAwaiter: CancellationToken -> 'Awaiter)
            : NodeCodeCode<_, _> =
            this.Bind(getAwaiter, this.Return)


        [<NoEagerConstraintApplication>]
        member inline this.BindReturn<'TResult1, 'TResult2, 'Awaiter, 'TOverall
            when Awaiter<'Awaiter, 'TResult1>>
            (
                [<InlineIfLambda>] getAwaiter: CancellationToken -> 'Awaiter,
                f
            ) : NodeCodeCode<'TResult2, 'TResult2> =
            this.Bind(getAwaiter, (fun v -> this.Return(f v)))

        /// <summary>Allows the computation expression to turn other types into CancellationToken -> 'Awaiter</summary>
        ///
        /// <remarks>This is the identify function.</remarks>
        ///
        /// <returns>CancellationToken -> 'Awaiter</returns>
        [<NoEagerConstraintApplication>]
        member inline _.Source<'TResult1, 'TResult2, 'Awaiter, 'TOverall
            when Awaiter<'Awaiter, 'TResult1>>
            ([<InlineIfLambda>] getAwaiter: CancellationToken -> 'Awaiter)
            : CancellationToken -> 'Awaiter =
            getAwaiter


        /// <summary>Allows the computation expression to turn other types into CancellationToken -> 'Awaiter</summary>
        ///
        /// <remarks>This is the identify function.</remarks>
        ///
        /// <returns>CancellationToken -> 'Awaiter</returns>
        [<NoEagerConstraintApplication>]
        member inline _.Source<'TResult1, 'TResult2, 'Awaiter, 'TOverall
            when Awaiter<'Awaiter, 'TResult1>>
            (getAwaiter: 'Awaiter)
            : CancellationToken -> 'Awaiter =
            (fun _ct -> getAwaiter)


        /// <summary>Allows the computation expression to turn other types into CancellationToken -> 'Awaiter</summary>
        ///
        /// <remarks>This turns a 'Awaitable into a CancellationToken -> 'Awaiter.</remarks>
        ///
        /// <returns>CancellationToken -> 'Awaiter</returns>
        [<NoEagerConstraintApplication>]
        member inline _.Source<'Awaitable, 'TResult1, 'TResult2, 'Awaiter, 'TOverall
            when Awaitable<'Awaitable, 'Awaiter, 'TResult1>>
            (task: 'Awaitable)
            : CancellationToken -> 'Awaiter =
            (fun (_ct: CancellationToken) ->
                task
                |> Awaitable.getAwaiter
            )


        /// <summary>Allows the computation expression to turn other types into CancellationToken -> 'Awaiter</summary>
        ///
        /// <remarks>This turns a CancellationToken -> 'Awaitable into a CancellationToken -> 'Awaiter.</remarks>
        ///
        /// <returns>CancellationToken -> 'Awaiter</returns>
        [<NoEagerConstraintApplication>]
        member inline _.Source<'Awaitable, 'TResult1, 'TResult2, 'Awaiter, 'TOverall
            when Awaitable<'Awaitable, 'Awaiter, 'TResult1>>
            ([<InlineIfLambda>] task: CancellationToken -> 'Awaitable)
            : CancellationToken -> 'Awaiter =
            (fun ct ->
                task ct
                |> Awaitable.getAwaiter
            )


        /// <summary>Allows the computation expression to turn other types into CancellationToken -> 'Awaiter</summary>
        ///
        /// <remarks>This turns a unit -> 'Awaitable into a CancellationToken -> 'Awaiter.</remarks>
        ///
        /// <returns>CancellationToken -> 'Awaiter</returns>
        [<NoEagerConstraintApplication>]
        member inline _.Source<'Awaitable, 'TResult1, 'TResult2, 'Awaiter, 'TOverall
            when Awaitable<'Awaitable, 'Awaiter, 'TResult1>>
            ([<InlineIfLambda>] task: unit -> 'Awaitable)
            : CancellationToken -> 'Awaiter =
            (fun _ct ->
                task ()
                |> Awaitable.getAwaiter
            )


        /// <summary>Creates an NodeCode that runs binder(resource).
        /// The action resource.Dispose() is executed as this computation yields its result
        /// or if the NodeCode exits by an exception or by cancellation.</summary>
        ///
        /// <remarks>
        ///
        /// The existence of this method permits the use of use and use! in the
        /// node { ... } computation expression syntax.</remarks>
        ///
        /// <param name="resource">The resource to be used and disposed.</param>
        /// <param name="binder">The function that takes the resource and returns an asynchronous
        /// computation.</param>
        ///
        /// <returns>An NodeCode that binds and eventually disposes resource.</returns>
        ///
        member inline _.Using<'Resource, 'TOverall, 'T when 'Resource :> IDisposable>
            (
                resource: 'Resource,
                [<InlineIfLambda>] binder: 'Resource -> NodeCodeCode<'TOverall, 'T>
            ) =
            ResumableCode.Using(
                resource,
                fun resource ->
                    NodeCodeCode<'TOverall, 'T>(fun sm ->
                        sm.Data.ThrowIfCancellationRequested()
                        (binder resource).Invoke(&sm)
                    )
            )

        member inline _.Using<'TOverall, 'T>
            (
                resource: CompilationGlobalsScope,
                [<InlineIfLambda>] binder: CompilationGlobalsScope -> NodeCodeCode<'TOverall, 'T>
            ) =
            ResumableCode.Using(
                resource,
                fun resource ->
                    NodeCodeCode<'TOverall, 'T>(fun sm ->
                        DiagnosticsThreadStatics.DiagnosticsLogger <- resource.DiagnosticsLogger
                        DiagnosticsThreadStatics.BuildPhase <- resource.BuildPhase
                        try
                            sm.Data.ThrowIfCancellationRequested()
                            (binder resource).Invoke(&sm)
                        finally
                            (resource :> IDisposable).Dispose()
                )
        )

/// <exclude />
[<AutoOpen>]
module HighPriority =

    let inline startAsyncImmediateAsTask computation (cancellationToken: CancellationToken) =
            // Protect against blocking the UI thread by switching to thread pool
            let computation =
                match SynchronizationContext.Current with
                | null -> computation
                | _ ->
                    async {
                        do! Async.SwitchToThreadPool()
                        return! computation
                    }

            // try not to yield if on bg thread already
            let tcs = new TaskCompletionSource<_>(TaskCreationOptions.None)
            let barrier = VolatileBarrier()

            let reg =
                cancellationToken.Register(fun _ ->
                    if barrier.Proceed then
                        tcs.TrySetCanceled(cancellationToken) |> ignore)

            let task = tcs.Task

            let disposeReg () =
                barrier.Stop()

                if not task.IsCanceled then
                    reg.Dispose()

            Async.StartWithContinuations(
                computation,
                continuation =
                    (fun result ->
                        disposeReg ()
                        tcs.TrySetResult(result) |> ignore),
                exceptionContinuation =
                    (fun exn ->
                        disposeReg ()

                        match exn with
                        | :? OperationCanceledException -> tcs.TrySetCanceled(cancellationToken) |> ignore
                        | exn -> tcs.TrySetException(exn) |> ignore),
                cancellationContinuation =
                    (fun _oce ->
                        disposeReg ()
                        tcs.TrySetCanceled(cancellationToken) |> ignore),
                cancellationToken = cancellationToken
            )

            task

    type Control.Async with

        /// <summary>Return an asynchronous computation that will wait for the given task to complete and return
        /// its result.</summary>
        static member inline AwaitNodeCode(t: NodeCode<'T>) =
            async {
                let! ct = Async.CancellationToken

                return!
                    t ct
                    |> Async.AwaitTask
            }

        /// <summary>Return an asynchronous computation that will wait for the given task to complete and return
        /// its result.</summary>
        static member inline AwaitNodeCode(t: NodeCode) =
            async {
                let! ct = Async.CancellationToken

                return!
                    t ct
                    |> Async.AwaitTask
            }

        /// <summary>Runs an asynchronous computation, starting on the current operating system thread.</summary>
        static member inline AsNodeCode(computation: Async<'T>) : NodeCode<_> =
            fun ct -> startAsyncImmediateAsTask computation ct

    // High priority extensions
    type NodeCodeBuilderBase with

        (*/// <summary>
        /// Turn option into "awaitable", will return cancelled task if None
        /// </summary>
        /// <param name="s">Option instance to bind on</param>
        member inline _.Source(s: 'T option) =
            (fun (_ct: CancellationToken) ->
                match s with
                | Some x -> Task.FromResult<'T>(x).GetAwaiter()
                | None -> Task.FromCanceled<'T>(CancellationToken(true)).GetAwaiter()
            )*)

        (*/// <summary>
        /// Turn a value option into "awaitable", will return cancelled task if None
        /// </summary>
        /// <param name="s">Option instance to bind on</param>
        member inline _.Source(s: 'T voption) =
            (fun (_ct: CancellationToken) ->
                match s with
                | ValueSome x -> Task.FromResult<'T>(x).GetAwaiter()
                | ValueNone -> Task.FromCanceled<'T>(CancellationToken(true)).GetAwaiter()
            )*)

        /// <summary>Allows the computation expression to turn other types into other types</summary>
        ///
        /// <remarks>This is the identify function for For binds.</remarks>
        ///
        /// <returns>IEnumerable</returns>
        member inline _.Source(s: #seq<_>) : #seq<_> = s

        /// <summary>Allows the computation expression to turn other types into CancellationToken -> 'Awaiter</summary>
        ///
        /// <remarks>This turns a Task&lt;'T&gt; into a CancellationToken -> 'Awaiter.</remarks>
        ///
        /// <returns>CancellationToken -> 'Awaiter</returns>
        member inline _.Source(task: Task<'T>) =
            (fun (_ct: CancellationToken) -> task.GetAwaiter())

        /// <summary>Allows the computation expression to turn other types into CancellationToken -> 'Awaiter</summary>
        ///
        /// <remarks>This turns a ColdTask&lt;'T&gt; into a CancellationToken -> 'Awaiter.</remarks>
        ///
        /// <returns>CancellationToken -> 'Awaiter</returns>
        member inline _.Source([<InlineIfLambda>] task: unit -> Task<'TResult1>) =
            (fun (_ct: CancellationToken) -> (task ()).GetAwaiter())

        /// <summary>Allows the computation expression to turn other types into CancellationToken -> 'Awaiter</summary>
        ///
        /// <remarks>This turns a NodeCode&lt;'T&gt; into a CancellationToken -> 'Awaiter.</remarks>
        ///
        /// <returns>CancellationToken -> 'Awaiter</returns>
        member inline _.Source([<InlineIfLambda>] task: NodeCode<'TResult1>) =
            (fun ct -> (task ct).GetAwaiter())

        /// <summary>Allows the computation expression to turn other types into CancellationToken -> 'Awaiter</summary>
        ///
        /// <remarks>This turns a Async&lt;'T&gt; into a CancellationToken -> 'Awaiter.</remarks>
        ///
        /// <returns>CancellationToken -> 'Awaiter</returns>
        member inline this.Source(computation: Async<'TResult1>) =
            this.Source(Async.AsNodeCode(computation))

        /// <summary>Allows the computation expression to turn other types into CancellationToken -> 'Awaiter</summary>
        ///
        /// <remarks>This turns a NodeCode&lt;'T&gt; into a CancellationToken -> 'Awaiter.</remarks>
        ///
        /// <returns>CancellationToken -> 'Awaiter</returns>
        member inline _.Source(awaiter: TaskAwaiter<'TResult1>) = (fun _ct -> awaiter)

/// <summary>
/// A set of extension methods making it possible to bind against <see cref='T:NodeCode`1'/> in async computations.
/// </summary>
[<AutoOpen>]
module AsyncExtenions =
    type Control.AsyncBuilder with

        member inline this.Bind(t: NodeCode<'T>, [<InlineIfLambda>] binder: ('T -> Async<'U>)) : Async<'U> =
            this.Bind(Async.AwaitNodeCode t, binder)

        member inline this.ReturnFrom([<InlineIfLambda>] t: NodeCode<'T>) : Async<'T> =
            this.ReturnFrom(Async.AwaitNodeCode t)

        member inline this.Bind([<InlineIfLambda>] t: NodeCode, binder: (unit -> Async<'U>)) : Async<'U> =
            this.Bind(Async.AwaitNodeCode t, binder)

        member inline this.ReturnFrom([<InlineIfLambda>] t: NodeCode) : Async<unit> =
            this.ReturnFrom(Async.AwaitNodeCode t)

/// Contains a set of standard functional helper function
[<RequireQualifiedAccess>]
module NodeCode =

    /// <summary>Gets the default cancellation token for executing computations.</summary>
    ///
    /// <returns>The default CancellationToken.</returns>
    ///
    /// <category index="3">Cancellation and Exceptions</category>
    ///
    /// <example id="default-cancellation-token-1">
    /// <code lang="F#">
    /// use tokenSource = new CancellationTokenSource()
    /// let primes = [ 2; 3; 5; 7; 11 ]
    /// for i in primes do
    ///     let computation =
    ///         node {
    ///             let! cancellationToken = NodeCode.getCurrentCancellationToken()
    ///             do! Task.Delay(i * 1000, cancellationToken)
    ///             printfn $"{i}"
    ///         }
    ///     computation tokenSource.Token |> ignore
    /// Thread.Sleep(6000)
    /// tokenSource.Cancel()
    /// printfn "Tasks Finished"
    /// </code>
    /// This will print "2" 2 seconds from start, "3" 3 seconds from start, "5" 5 seconds from start, cease computation and then
    /// followed by "Tasks Finished".
    /// </example>
    let getCurrentCancellationToken () =
        node.Run(
            NodeCodeCode<_, _>(fun sm ->
                sm.Data.Result <- sm.Data.CancellationToken
                true
            )
        )

    /// <summary>Lifts an item to a NodeCode.</summary>
    /// <param name="item">The item to be the result of the NodeCode.</param>
    /// <returns>A NodeCode with the item as the result.</returns>
    let inline singleton (item: 'item) : NodeCode<'item> = fun _ -> Task.FromResult(item)

    /// <summary>Allows chaining of NodeCodes.</summary>
    /// <param name="binder">The continuation.</param>
    /// <param name="cTask">The value.</param>
    /// <returns>The result of the binder.</returns>
    let inline bind
        ([<InlineIfLambda>] binder: 'input -> NodeCode<'output>)
        ([<InlineIfLambda>] cTask: NodeCode<'input>)
        =
        node {
            let! cResult = cTask
            return! binder cResult
        }

    /// <summary>Allows chaining of NodeCodes.</summary>
    /// <param name="mapper">The continuation.</param>
    /// <param name="cTask">The value.</param>
    /// <returns>The result of the mapper wrapped in a NodeCodes.</returns>
    let inline map
        ([<InlineIfLambda>] mapper: 'input -> 'output)
        ([<InlineIfLambda>] cTask: NodeCode<'input>)
        =
        node {
            let! cResult = cTask
            return mapper cResult
        }

    /// <summary>Allows chaining of NodeCodes.</summary>
    /// <param name="applicable">A function wrapped in a NodeCodes</param>
    /// <param name="cTask">The value.</param>
    /// <returns>The result of the applicable.</returns>
    let inline apply
        ([<InlineIfLambda>] applicable: NodeCode<'input -> 'output>)
        ([<InlineIfLambda>] cTask: NodeCode<'input>)
        =
        node {
            let! applier = applicable
            let! cResult = cTask
            return applier cResult
        }

    /// <summary>Takes two NodeCodes, starts them serially in order of left to right, and returns a tuple of the pair.</summary>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    /// <returns>A tuple of the parameters passed in</returns>
    let inline zip
        ([<InlineIfLambda>] left: NodeCode<'left>)
        ([<InlineIfLambda>] right: NodeCode<'right>)
        =
        node {
            let! r1 = left
            let! r2 = right
            return r1, r2
        }

    /// <summary>Takes two NodeCode, starts them concurrently, and returns a tuple of the pair.</summary>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    /// <returns>A tuple of the parameters passed in.</returns>
    let inline parallelZip
        ([<InlineIfLambda>] left: NodeCode<'left>)
        ([<InlineIfLambda>] right: NodeCode<'right>)
        =
        node {
            let! ct = getCurrentCancellationToken ()
            let r1 = left ct
            let r2 = right ct
            let! r1 = r1
            let! r2 = r2
            return r1, r2
        }


    /// <summary>Coverts a NodeCode to a NodeCode\&lt;unit\&gt;.</summary>
    /// <param name="unitCancellabletTask">The NodeCode to convert.</param>
    /// <returns>a NodeCode\&lt;unit\&gt;.</returns>
    let inline ofUnit ([<InlineIfLambda>] unitCancellabletTask: NodeCode) =
        node {
            return! unitCancellabletTask
        }

    /// <summary>Coverts a NodeCode\&lt;_\&gt; to a NodeCode.</summary>
    /// <param name="ctask">The NodeCode to convert.</param>
    /// <param name="ct">A cancellation token.</param>
    /// <returns>a NodeCode.</returns>
    let inline toUnit ([<InlineIfLambda>] ctask: NodeCode<_>) : NodeCode =
        fun ct -> ctask ct

    let inline getAwaiter ([<InlineIfLambda>] ctask: NodeCode<_>) =
        fun ct -> (ctask ct).GetAwaiter()

    let inline wrapThreadStaticInfo ([<InlineIfLambda>] computation: NodeCode<'T>) =
        node {
            let diagnosticsLogger = DiagnosticsThreadStatics.DiagnosticsLogger
            let phase = DiagnosticsThreadStatics.BuildPhase

            try
                return! computation
            finally
                DiagnosticsThreadStatics.DiagnosticsLogger <- diagnosticsLogger
                DiagnosticsThreadStatics.BuildPhase <- phase
        }

    let inline propagateThreadStaticInfo ([<InlineIfLambda>] computation: NodeCode<'T>) =
        let diagnosticsLogger = DiagnosticsThreadStatics.DiagnosticsLogger
        let phase = DiagnosticsThreadStatics.BuildPhase

        try
            node {
                DiagnosticsThreadStatics.DiagnosticsLogger <- diagnosticsLogger
                DiagnosticsThreadStatics.BuildPhase <- phase
                return! computation
            }
        with :? AggregateException as ex when ex.InnerExceptions.Count = 1 ->
            raise (ex.InnerExceptions[0])

    let inline start ct ([<InlineIfLambda>] ctask: NodeCode<_>) = propagateThreadStaticInfo(ctask) ct

    let inline startWithoutCancellation ([<InlineIfLambda>] ctask: NodeCode<_>) = start CancellationToken.None ctask

    let inline runSyncronously ct ([<InlineIfLambda>] ctask: NodeCode<_>) =
        let task = start ct ctask
        task.GetAwaiter().GetResult()

    let inline runSyncronouslyWithoutCancellation  ([<InlineIfLambda>] ctask: NodeCode<_>) =
        let task = startWithoutCancellation ctask
        task.GetAwaiter().GetResult()

    let inline startAsTask ct ([<InlineIfLambda>] ctask: NodeCode<_>) = (propagateThreadStaticInfo(ctask) ct) :> Task

    let inline startAsTaskWithoutCancellation ([<InlineIfLambda>] ctask: NodeCode<_>) = (ctask CancellationToken.None) :> Task

    let inline runAsTaskSyncronously ct ([<InlineIfLambda>] ctask: NodeCode<_>) =
        let task = startAsTask ct ctask
        task.GetAwaiter().GetResult()

    let inline runAsTaskSyncronouslyWithoutCancellation  ([<InlineIfLambda>] ctask: NodeCode<_>) =
        let task = startAsTaskWithoutCancellation ctask
        task.GetAwaiter().GetResult()


    let inline Sequential (computations: NodeCode<'T> seq) =
        node {
            let results = ResizeArray()

            for computation in computations do
                let! res = computation
                results.Add(res)

            return results.ToArray()
        }

    let inline Parallel(computations: NodeCode<'T> seq) =
        node {
            let! ct = getCurrentCancellationToken ()
            let computations =
                [|
                    for computation in computations do
                        let computation = computation ct
                        yield computation
                |]
            return! Task.WhenAll(computations)
        }

type NodeCodeBuilderBase with
    member _.Source(Cancellable(cancellable): Cancellable<'T>)  =
        fun (ct: CancellationToken) ->
            let task =
                node {
                    let! ct = NodeCode.getCurrentCancellationToken ()

                    let res =
                        if ct.IsCancellationRequested then
                            ValueOrCancelled.Cancelled(OperationCanceledException ct)
                        else
                            cancellable ct

                    return!
                        Async.FromContinuations(fun (cont, _econt, ccont) ->
                            match res with
                            | ValueOrCancelled.Value v -> cont v
                            | ValueOrCancelled.Cancelled ce -> ccont ce)
                } |> NodeCode.start ct
            task.GetAwaiter()

/// <exclude />
[<AutoOpen>]
module MergeSourcesExtensions =

    type NodeCodeBuilderBase with

        [<NoEagerConstraintApplication>]
        member inline _.MergeSources<'TResult1, 'TResult2, 'Awaiter1, 'Awaiter2
            when Awaiter<'Awaiter1, 'TResult1> and Awaiter<'Awaiter2, 'TResult2>>
            (
                [<InlineIfLambda>] left: CancellationToken -> 'Awaiter1,
                [<InlineIfLambda>] right: CancellationToken -> 'Awaiter2
            ) : CancellationToken -> TaskAwaiter<'TResult1 * 'TResult2> =

            node {
                let! ct = NodeCode.getCurrentCancellationToken ()
                let leftStarted = left ct
                let rightStarted = right ct
                let! leftResult = leftStarted
                let! rightResult = rightStarted
                return leftResult, rightResult
            }
            |> NodeCode.getAwaiter

[<RequireQualifiedAccess>]
module GraphNode =

    // We need to store the culture for the VS thread that is executing now,
    // so that when the agent in the async lazy object picks up thread from the thread pool we can set the culture
    let mutable culture = CultureInfo(CultureInfo.CurrentUICulture.Name)

    let SetPreferredUILang (preferredUiLang: string option) =
        match preferredUiLang with
        | Some s ->
            culture <- CultureInfo s
#if FX_RESHAPED_GLOBALIZATION
            CultureInfo.CurrentUICulture <- culture
#else
            Thread.CurrentThread.CurrentUICulture <- culture
#endif
        | None -> ()

[<Sealed>]
type GraphNode<'T> private(computation: NodeCode<'T>, cachedResult: ValueOption<'T>, cachedResultNode: NodeCode<'T>) =
    
    static let computeValue (graphNode: GraphNode<'T>) =
        node {

            do Interlocked.Increment(&graphNode.RequestCount) |> ignore

            try
                let! ct = NodeCode.getCurrentCancellationToken ()

                // We must set 'taken' before any implicit cancellation checks
                // occur, making sure we are under the protection of the 'try'.
                // For example, NodeCode's 'try/finally' (TryFinally) uses async.TryFinally which does
                // implicit cancellation checks even before the try is entered, as do the
                // de-sugaring of 'do!' and other NodeCode constructs.
                let mutable taken = false

                try
                    do! graphNode.Semaphore.WaitAsync(ct)
                
                    taken <- true

                    match graphNode.CachedResult with
                    | ValueSome value ->
                        return value
                    | _ ->

                        Thread.CurrentThread.CurrentUICulture <- GraphNode.culture

                        let! res = graphNode.Computation
                        graphNode.CachedResult <- ValueSome res
                        graphNode.CachedResultNode <- graphNode.Computation
                        graphNode.Computation <- Unchecked.defaultof<_>

                        return res
                finally
                    if taken then
                        do graphNode.Semaphore.Release() |> ignore
            finally
                do Interlocked.Decrement(&graphNode.RequestCount) |> ignore
        }

    [<DefaultValue>] val mutable RequestCount : int

    member val private Computation : NodeCode<'T> = computation with get, set
    member val private CachedResult = cachedResult with get, set
    member val private CachedResultNode : NodeCode<'T> = cachedResultNode with get, set

    member val private Semaphore : SemaphoreSlim = new SemaphoreSlim(1, 1)

    member inline this.GetOrComputeValue() =
        // fast path
        if not (obj.ReferenceEquals(cachedResultNode, null)) then
            cachedResultNode
        else
            computeValue this
    member _.TryPeekValue() = cachedResult

    member _.HasValue = cachedResult.IsSome

    member this.IsComputing = this.RequestCount > 0

    static member FromResult(result: 'T) =
        let nodeResult = NodeCode.singleton result
        GraphNode(nodeResult, ValueSome result, nodeResult)

    new(computation) = GraphNode(computation, ValueNone, Unchecked.defaultof<_>)
