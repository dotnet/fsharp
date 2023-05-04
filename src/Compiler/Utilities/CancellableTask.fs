// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Internal.Utilities

// Don't warn about the resumable code invocation
#nowarn "3513"

module CancellableTasks =

    open System
    open System.Runtime.CompilerServices
    open System.Threading
    open System.Threading.Tasks
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.CompilerServices
    open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Collections

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
    type CancellableTask<'T> = CancellationToken -> Task<'T>

    /// CancellationToken -> Task
    type CancellableTask = CancellationToken -> Task

    /// The extra data stored in ResumableStateMachine for tasks
    [<Struct; NoComparison; NoEquality>]
    type CancellableTaskStateMachineData<'T> =
        [<DefaultValue(false)>]
        val mutable CancellationToken: CancellationToken

        [<DefaultValue(false)>]
        val mutable Result: 'T

        [<DefaultValue(false)>]
        val mutable MethodBuilder: AsyncTaskMethodBuilder<'T>

        member inline this.ThrowIfCancellationRequested() =
            this.CancellationToken.ThrowIfCancellationRequested()

    /// This is used by the compiler as a template for creating state machine structs
    and CancellableTaskStateMachine<'TOverall> =
        ResumableStateMachine<CancellableTaskStateMachineData<'TOverall>>

    /// Represents the runtime continuation of a CancellableTask state machine created dynamically
    and CancellableTaskResumptionFunc<'TOverall> =
        ResumptionFunc<CancellableTaskStateMachineData<'TOverall>>

    /// Represents the runtime continuation of a CancellableTask state machine created dynamically
    and CancellableTaskResumptionDynamicInfo<'TOverall> =
        ResumptionDynamicInfo<CancellableTaskStateMachineData<'TOverall>>

    /// A special compiler-recognised delegate type for specifying blocks of CancellableTask code with access to the state machine
    and CancellableTaskCode<'TOverall, 'T> =
        ResumableCode<CancellableTaskStateMachineData<'TOverall>, 'T>


    /// Contains methods to build CancellableTasks using the F# computation expression syntax
    [<NoComparison; NoEquality>]
    type CancellableTaskBuilderBase() =

        /// <summary>Creates a CancellableTask that runs generator</summary>
        /// <param name="generator">The function to run</param>
        /// <returns>A cancellableTask that runs generator</returns>
        member inline _.Delay
            ([<InlineIfLambda>] generator: unit -> CancellableTaskCode<'TOverall, 'T>)
            : CancellableTaskCode<'TOverall, 'T> =
            ResumableCode.Delay(fun () ->
                CancellableTaskCode(fun sm ->
                    sm.Data.ThrowIfCancellationRequested()
                    (generator ()).Invoke(&sm)
                )
            )


        /// <summary>Creates an CancellableTask that just returns ().</summary>
        /// <remarks>
        /// The existence of this method permits the use of empty else branches in the
        /// cancellableTask { ... } computation expression syntax.
        /// </remarks>
        /// <returns>An CancellableTask that returns ().</returns>
        [<DefaultValue>]
        member inline _.Zero() : CancellableTaskCode<'TOverall, unit> = ResumableCode.Zero()

        /// <summary>Creates an computation that returns the result v.</summary>
        ///
        /// <remarks>A cancellation check is performed when the computation is executed.
        ///
        /// The existence of this method permits the use of return in the
        /// cancellableTask { ... } computation expression syntax.</remarks>
        ///
        /// <param name="value">The value to return from the computation.</param>
        ///
        /// <returns>An CancellableTask that returns value when executed.</returns>
        member inline _.Return(value: 'T) : CancellableTaskCode<'T, 'T> =
            CancellableTaskCode<'T, _>(fun sm ->
                sm.Data.ThrowIfCancellationRequested()
                sm.Data.Result <- value
                true
            )

        /// <summary>Creates an CancellableTask that first runs task1
        /// and then runs computation2, returning the result of computation2.</summary>
        ///
        /// <remarks>
        ///
        /// The existence of this method permits the use of expression sequencing in the
        /// cancellableTask { ... } computation expression syntax.</remarks>
        ///
        /// <param name="task1">The first part of the sequenced computation.</param>
        /// <param name="task2">The second part of the sequenced computation.</param>
        ///
        /// <returns>An CancellableTask that runs both of the computations sequentially.</returns>
        member inline _.Combine
            (
                task1: CancellableTaskCode<'TOverall, unit>,
                task2: CancellableTaskCode<'TOverall, 'T>
            ) : CancellableTaskCode<'TOverall, 'T> =
            ResumableCode.Combine(
                CancellableTaskCode(fun sm ->
                    sm.Data.ThrowIfCancellationRequested()
                    task1.Invoke(&sm)
                ),

                CancellableTaskCode(fun sm ->
                    sm.Data.ThrowIfCancellationRequested()
                    task2.Invoke(&sm)
                )
            )

        /// <summary>Creates an CancellableTask that runs computation repeatedly
        /// until guard() becomes false.</summary>
        ///
        /// <remarks>
        ///
        /// The existence of this method permits the use of while in the
        /// cancellableTask { ... } computation expression syntax.</remarks>
        ///
        /// <param name="guard">The function to determine when to stop executing computation.</param>
        /// <param name="computation">The function to be executed.  Equivalent to the body
        /// of a while expression.</param>
        ///
        /// <returns>An CancellableTask that behaves similarly to a while loop when run.</returns>
        member inline _.While
            (
                [<InlineIfLambda>] guard: unit -> bool,
                computation: CancellableTaskCode<'TOverall, unit>
            ) : CancellableTaskCode<'TOverall, unit> =
            ResumableCode.While(
                guard,
                CancellableTaskCode(fun sm ->
                    sm.Data.ThrowIfCancellationRequested()
                    computation.Invoke(&sm)
                )
            )

        /// <summary>Creates an CancellableTask that runs computation and returns its result.
        /// If an exception happens then catchHandler(exn) is called and the resulting computation executed instead.</summary>
        ///
        /// <remarks>
        ///
        /// The existence of this method permits the use of try/with in the
        /// cancellableTask { ... } computation expression syntax.</remarks>
        ///
        /// <param name="computation">The input computation.</param>
        /// <param name="catchHandler">The function to run when computation throws an exception.</param>
        ///
        /// <returns>An CancellableTask that executes computation and calls catchHandler if an
        /// exception is thrown.</returns>
        member inline _.TryWith
            (
                computation: CancellableTaskCode<'TOverall, 'T>,
                [<InlineIfLambda>] catchHandler: exn -> CancellableTaskCode<'TOverall, 'T>
            ) : CancellableTaskCode<'TOverall, 'T> =
            ResumableCode.TryWith(
                CancellableTaskCode(fun sm ->
                    sm.Data.ThrowIfCancellationRequested()
                    computation.Invoke(&sm)
                ),
                catchHandler
            )

        /// <summary>Creates an CancellableTask that runs computation. The action compensation is executed
        /// after computation completes, whether computation exits normally or by an exception. If compensation raises an exception itself
        /// the original exception is discarded and the new exception becomes the overall result of the computation.</summary>
        ///
        /// <remarks>
        ///
        /// The existence of this method permits the use of try/finally in the
        /// cancellableTask { ... } computation expression syntax.</remarks>
        ///
        /// <param name="computation">The input computation.</param>
        /// <param name="compensation">The action to be run after computation completes or raises an
        /// exception (including cancellation).</param>
        ///
        /// <returns>An CancellableTask that executes computation and compensation afterwards or
        /// when an exception is raised.</returns>
        member inline _.TryFinally
            (
                computation: CancellableTaskCode<'TOverall, 'T>,
                [<InlineIfLambda>] compensation: unit -> unit
            ) : CancellableTaskCode<'TOverall, 'T> =
            ResumableCode.TryFinally(

                CancellableTaskCode(fun sm ->
                    sm.Data.ThrowIfCancellationRequested()
                    computation.Invoke(&sm)
                ),
                ResumableCode<_, _>(fun _ ->
                    compensation ()
                    true
                )
            )

        /// <summary>Creates an CancellableTask that enumerates the sequence seq
        /// on demand and runs body for each element.</summary>
        ///
        /// <remarks>A cancellation check is performed on each iteration of the loop.
        ///
        /// The existence of this method permits the use of for in the
        /// cancellableTask { ... } computation expression syntax.</remarks>
        ///
        /// <param name="sequence">The sequence to enumerate.</param>
        /// <param name="body">A function to take an item from the sequence and create
        /// an CancellableTask.  Can be seen as the body of the for expression.</param>
        ///
        /// <returns>An CancellableTask that will enumerate the sequence and run body
        /// for each element.</returns>
        member inline _.For
            (
                sequence: seq<'T>,
                [<InlineIfLambda>] body: 'T -> CancellableTaskCode<'TOverall, unit>
            ) : CancellableTaskCode<'TOverall, unit> =
            ResumableCode.For(
                sequence,
                fun item ->
                    CancellableTaskCode(fun sm ->
                        sm.Data.ThrowIfCancellationRequested()
                        (body item).Invoke(&sm)
                    )
            )

    /// Contains methods to build CancellableTasks using the F# computation expression syntax
    [<Sealed; NoComparison; NoEquality>]
    type CancellableTaskBuilder(runOnBackground: bool) =

        inherit CancellableTaskBuilderBase()

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
        static member inline RunDynamicAux(code: CancellableTaskCode<'T, 'T>) : CancellableTask<'T> =

            let mutable sm = CancellableTaskStateMachine<'T>()

            let initialResumptionFunc =
                CancellableTaskResumptionFunc<'T>(fun sm -> code.Invoke(&sm))

            let resumptionInfo =
                { new CancellableTaskResumptionDynamicInfo<'T>(initialResumptionFunc) with
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
        static member inline RunDynamic(code: CancellableTaskCode<'T, 'T>, runOnBackground: bool) : CancellableTask<'T> =
            // When runOnBackground is true, task escapes to a background thread where necessary
            // See spec of ConfigureAwait(false) at https://devblogs.microsoft.com/dotnet/configureawait-faq/

            if runOnBackground
                && not (isNull SynchronizationContext.Current
                        && obj.ReferenceEquals(TaskScheduler.Current, TaskScheduler.Default))
            then
                fun (ct) ->
                    // Warning: this will always try to yield even if on thread pool already.
                    Task.Run<'T>((fun () -> CancellableTaskBuilder.RunDynamicAux (code) (ct)), ct)
            else
                CancellableTaskBuilder.RunDynamicAux(code)


        /// Hosts the task code in a state machine and starts the task.
        member inline this.Run(code: CancellableTaskCode<'T, 'T>) : CancellableTask<'T> =
            if __useResumableCode then
                __stateMachine<CancellableTaskStateMachineData<'T>, CancellableTask<'T>>
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
                CancellableTaskBuilder.RunDynamic(code, this.IsBackground)

    /// Contains the cancellableTask computation expression builder.
    [<AutoOpen>]
    module CancellableTaskBuilder =

        /// <summary>
        /// Builds a cancellableTask using computation expression syntax.
        /// Default behaviour when binding (v)options is to return a cacnelled task.
        /// </summary>
        let foregroundCancellableTask = CancellableTaskBuilder(false)

        /// <summary>
        /// Builds a cancellableTask using computation expression syntax which switches to execute on a background thread if not already doing so.
        /// Default behaviour when binding (v)options is to return a cacnelled task.
        /// </summary>
        let cancellableTask = CancellableTaskBuilder(true)

    /// <exclude />
    [<AutoOpen>]
    module LowPriority =
        // Low priority extensions
        type CancellableTaskBuilderBase with

            /// <summary>
            /// The entry point for the dynamic implementation of the corresponding operation. Do not use directly, only used when executing quotations that involve tasks or other reflective execution of F# code.
            /// </summary>
            [<NoEagerConstraintApplication>]
            static member inline BindDynamic<'TResult1, 'TResult2, 'Awaiter, 'TOverall
                when Awaiter<'Awaiter, 'TResult1>>
                (
                    sm: byref<ResumableStateMachine<CancellableTaskStateMachineData<'TOverall>>>,
                    [<InlineIfLambda>] getAwaiter: CancellationToken -> 'Awaiter,
                    [<InlineIfLambda>] continuation: ('TResult1 -> CancellableTaskCode<'TOverall, 'TResult2>)
                ) : bool =
                sm.Data.ThrowIfCancellationRequested()

                let mutable awaiter = getAwaiter sm.Data.CancellationToken

                let cont: CancellableTaskResumptionFunc<'TOverall> =
                    (CancellableTaskResumptionFunc<'TOverall>(fun (sm: byref<ResumableStateMachine<CancellableTaskStateMachineData<'TOverall>>>) ->
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

            /// <summary>Creates an CancellableTask that runs computation, and when
            /// computation generates a result T, runs binder res.</summary>
            ///
            /// <remarks>A cancellation check is performed when the computation is executed.
            ///
            /// The existence of this method permits the use of let! in the
            /// cancellableTask { ... } computation expression syntax.</remarks>
            ///
            /// <param name="getAwaiter">The computation to provide an unbound result.</param>
            /// <param name="continuation">The function to bind the result of computation.</param>
            ///
            /// <returns>An CancellableTask that performs a monadic bind on the result
            /// of computation.</returns>
            [<NoEagerConstraintApplication>]
            member inline _.Bind<'TResult1, 'TResult2, 'Awaiter, 'TOverall
                when Awaiter<'Awaiter, 'TResult1>>
                (
                    [<InlineIfLambda>] getAwaiter: CancellationToken -> 'Awaiter,
                    [<InlineIfLambda>] continuation: ('TResult1 -> CancellableTaskCode<'TOverall, 'TResult2>)
                ) : CancellableTaskCode<'TOverall, 'TResult2> =

                CancellableTaskCode<'TOverall, _>(fun sm ->
                    if __useResumableCode then
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
                    else
                        CancellableTaskBuilderBase.BindDynamic<'TResult1, 'TResult2, 'Awaiter, 'TOverall>(
                            &sm,
                            getAwaiter,
                            continuation
                        )
                //-- RESUMABLE CODE END
                )


            /// <summary>Delegates to the input computation.</summary>
            ///
            /// <remarks>The existence of this method permits the use of return! in the
            /// cancellableTask { ... } computation expression syntax.</remarks>
            ///
            /// <param name="getAwaiter">The input computation.</param>
            ///
            /// <returns>The input computation.</returns>
            [<NoEagerConstraintApplication>]
            member inline this.ReturnFrom<'TResult1, 'TResult2, 'Awaiter, 'TOverall
                when Awaiter<'Awaiter, 'TResult1>>
                ([<InlineIfLambda>] getAwaiter: CancellationToken -> 'Awaiter)
                : CancellableTaskCode<_, _> =
                this.Bind(getAwaiter, this.Return)


            [<NoEagerConstraintApplication>]
            member inline this.BindReturn<'TResult1, 'TResult2, 'Awaiter, 'TOverall
                when Awaiter<'Awaiter, 'TResult1>>
                (
                    [<InlineIfLambda>] getAwaiter: CancellationToken -> 'Awaiter,
                    f
                ) : CancellableTaskCode<'TResult2, 'TResult2> =
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


            /// <summary>Creates an CancellableTask that runs binder(resource).
            /// The action resource.Dispose() is executed as this computation yields its result
            /// or if the CancellableTask exits by an exception or by cancellation.</summary>
            ///
            /// <remarks>
            ///
            /// The existence of this method permits the use of use and use! in the
            /// cancellableTask { ... } computation expression syntax.</remarks>
            ///
            /// <param name="resource">The resource to be used and disposed.</param>
            /// <param name="binder">The function that takes the resource and returns an asynchronous
            /// computation.</param>
            ///
            /// <returns>An CancellableTask that binds and eventually disposes resource.</returns>
            ///
            member inline _.Using<'Resource, 'TOverall, 'T when 'Resource :> IDisposable>
                (
                    resource: 'Resource,
                    [<InlineIfLambda>] binder: 'Resource -> CancellableTaskCode<'TOverall, 'T>
                ) =
                ResumableCode.Using(
                    resource,
                    fun resource ->
                        CancellableTaskCode<'TOverall, 'T>(fun sm ->
                            sm.Data.ThrowIfCancellationRequested()
                            (binder resource).Invoke(&sm)
                        )
                )

    /// <exclude />
    [<AutoOpen>]
    module HighPriority =

        type Control.Async with

            /// <summary>Return an asynchronous computation that will wait for the given task to complete and return
            /// its result.</summary>
            static member inline AwaitCancellableTask(t: CancellableTask<'T>) =
                async {
                    let! ct = Async.CancellationToken

                    return!
                        t ct
                        |> Async.AwaitTask
                }

            /// <summary>Return an asynchronous computation that will wait for the given task to complete and return
            /// its result.</summary>
            static member inline AwaitCancellableTask(t: CancellableTask) =
                async {
                    let! ct = Async.CancellationToken

                    return!
                        t ct
                        |> Async.AwaitTask
                }

            /// <summary>Runs an asynchronous computation, starting on the current operating system thread.</summary>
            static member inline AsCancellableTask(computation: Async<'T>) : CancellableTask<_> =
                fun ct -> Async.StartImmediateAsTask(computation, cancellationToken = ct)

        // High priority extensions
        type CancellableTaskBuilderBase with

            /// <summary>
            /// Turn option into "awaitable", will return cancelled task if None
            /// </summary>
            /// <param name="s">Option instance to bind on</param>
            member inline _.Source(s: 'T option) =
                (fun (_ct: CancellationToken) ->
                    match s with
                    | Some x -> Task.FromResult<'T>(x).GetAwaiter()
                    | None -> Task.FromCanceled<'T>(CancellationToken(true)).GetAwaiter()
                )

            /// <summary>
            /// Turn a value option into "awaitable", will return cancelled task if None
            /// </summary>
            /// <param name="s">Option instance to bind on</param>
            member inline _.Source(s: 'T voption) =
                (fun (_ct: CancellationToken) ->
                    match s with
                    | ValueSome x -> Task.FromResult<'T>(x).GetAwaiter()
                    | ValueNone -> Task.FromCanceled<'T>(CancellationToken(true)).GetAwaiter()
                )

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
            /// <remarks>This turns a CancellableTask&lt;'T&gt; into a CancellationToken -> 'Awaiter.</remarks>
            ///
            /// <returns>CancellationToken -> 'Awaiter</returns>
            member inline _.Source([<InlineIfLambda>] task: CancellableTask<'TResult1>) =
                (fun ct -> (task ct).GetAwaiter())

            /// <summary>Allows the computation expression to turn other types into CancellationToken -> 'Awaiter</summary>
            ///
            /// <remarks>This turns a Async&lt;'T&gt; into a CancellationToken -> 'Awaiter.</remarks>
            ///
            /// <returns>CancellationToken -> 'Awaiter</returns>
            member inline this.Source(computation: Async<'TResult1>) =
                this.Source(Async.AsCancellableTask(computation))

            /// <summary>Allows the computation expression to turn other types into CancellationToken -> 'Awaiter</summary>
            ///
            /// <remarks>This turns a CancellableTask&lt;'T&gt; into a CancellationToken -> 'Awaiter.</remarks>
            ///
            /// <returns>CancellationToken -> 'Awaiter</returns>
            member inline _.Source(awaiter: TaskAwaiter<'TResult1>) = (fun _ct -> awaiter)

    /// <summary>
    /// A set of extension methods making it possible to bind against <see cref='T:CancellableTask`1'/> in async computations.
    /// </summary>
    [<AutoOpen>]
    module AsyncExtenions =
        type Control.AsyncBuilder with

            member inline this.Bind(t: CancellableTask<'T>, [<InlineIfLambda>] binder: ('T -> Async<'U>)) : Async<'U> =
                this.Bind(Async.AwaitCancellableTask t, binder)

            member inline this.ReturnFrom([<InlineIfLambda>] t: CancellableTask<'T>) : Async<'T> =
                this.ReturnFrom(Async.AwaitCancellableTask t)

            member inline this.Bind([<InlineIfLambda>] t: CancellableTask, binder: (unit -> Async<'U>)) : Async<'U> =
                this.Bind(Async.AwaitCancellableTask t, binder)

            member inline this.ReturnFrom([<InlineIfLambda>] t: CancellableTask) : Async<unit> =
                this.ReturnFrom(Async.AwaitCancellableTask t)

    /// Contains a set of standard functional helper function
    [<RequireQualifiedAccess>]
    module CancellableTask =

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
        ///         cancellableTask {
        ///             let! cancellationToken = CancellableTask.getCurrentCancellationToken()
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
            cancellableTask.Run(
                CancellableTaskCode<_, _>(fun sm ->
                    sm.Data.Result <- sm.Data.CancellationToken
                    true
                )
            )

        /// <summary>Lifts an item to a CancellableTask.</summary>
        /// <param name="item">The item to be the result of the CancellableTask.</param>
        /// <returns>A CancellableTask with the item as the result.</returns>
        let inline singleton (item: 'item) : CancellableTask<'item> = fun _ -> Task.FromResult(item)


        /// <summary>Allows chaining of CancellableTasks.</summary>
        /// <param name="binder">The continuation.</param>
        /// <param name="cTask">The value.</param>
        /// <returns>The result of the binder.</returns>
        let inline bind
            ([<InlineIfLambda>] binder: 'input -> CancellableTask<'output>)
            ([<InlineIfLambda>] cTask: CancellableTask<'input>)
            =
            cancellableTask {
                let! cResult = cTask
                return! binder cResult
            }

        /// <summary>Allows chaining of CancellableTasks.</summary>
        /// <param name="mapper">The continuation.</param>
        /// <param name="cTask">The value.</param>
        /// <returns>The result of the mapper wrapped in a CancellableTasks.</returns>
        let inline map
            ([<InlineIfLambda>] mapper: 'input -> 'output)
            ([<InlineIfLambda>] cTask: CancellableTask<'input>)
            =
            cancellableTask {
                let! cResult = cTask
                return mapper cResult
            }

        /// <summary>Allows chaining of CancellableTasks.</summary>
        /// <param name="applicable">A function wrapped in a CancellableTasks</param>
        /// <param name="cTask">The value.</param>
        /// <returns>The result of the applicable.</returns>
        let inline apply
            ([<InlineIfLambda>] applicable: CancellableTask<'input -> 'output>)
            ([<InlineIfLambda>] cTask: CancellableTask<'input>)
            =
            cancellableTask {
                let! applier = applicable
                let! cResult = cTask
                return applier cResult
            }

        /// <summary>Takes two CancellableTasks, starts them serially in order of left to right, and returns a tuple of the pair.</summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>A tuple of the parameters passed in</returns>
        let inline zip
            ([<InlineIfLambda>] left: CancellableTask<'left>)
            ([<InlineIfLambda>] right: CancellableTask<'right>)
            =
            cancellableTask {
                let! r1 = left
                let! r2 = right
                return r1, r2
            }

        /// <summary>Takes two CancellableTask, starts them concurrently, and returns a tuple of the pair.</summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>A tuple of the parameters passed in.</returns>
        let inline parallelZip
            ([<InlineIfLambda>] left: CancellableTask<'left>)
            ([<InlineIfLambda>] right: CancellableTask<'right>)
            =
            cancellableTask {
                let! ct = getCurrentCancellationToken ()
                let r1 = left ct
                let r2 = right ct
                let! r1 = r1
                let! r2 = r2
                return r1, r2
            }


        /// <summary>Coverts a CancellableTask to a CancellableTask\&lt;unit\&gt;.</summary>
        /// <param name="unitCancellabletTask">The CancellableTask to convert.</param>
        /// <returns>a CancellableTask\&lt;unit\&gt;.</returns>
        let inline ofUnit ([<InlineIfLambda>] unitCancellabletTask: CancellableTask) =
            cancellableTask {
                return! unitCancellabletTask
            }

        /// <summary>Coverts a CancellableTask\&lt;_\&gt; to a CancellableTask.</summary>
        /// <param name="ctask">The CancellableTask to convert.</param>
        /// <param name="ct">A cancellation token.</param>
        /// <returns>a CancellableTask.</returns>
        let inline toUnit ([<InlineIfLambda>] ctask: CancellableTask<_>) : CancellableTask =
            fun ct -> ctask ct

        let inline getAwaiter ([<InlineIfLambda>] ctask: CancellableTask<_>) =
            fun ct -> (ctask ct).GetAwaiter()

        let inline start ct ([<InlineIfLambda>] ctask: CancellableTask<_>) = ctask ct

        let inline startAsTask ct ([<InlineIfLambda>] ctask: CancellableTask<_>) = (ctask ct) :> Task

    /// <exclude />
    [<AutoOpen>]
    module MergeSourcesExtensions =

        type CancellableTaskBuilderBase with

            [<NoEagerConstraintApplication>]
            member inline _.MergeSources<'TResult1, 'TResult2, 'Awaiter1, 'Awaiter2
                when Awaiter<'Awaiter1, 'TResult1> and Awaiter<'Awaiter2, 'TResult2>>
                (
                    [<InlineIfLambda>] left: CancellationToken -> 'Awaiter1,
                    [<InlineIfLambda>] right: CancellationToken -> 'Awaiter2
                ) : CancellationToken -> TaskAwaiter<'TResult1 * 'TResult2> =

                cancellableTask {
                    let! ct = CancellableTask.getCurrentCancellationToken ()
                    let leftStarted = left ct
                    let rightStarted = right ct
                    let! leftResult = leftStarted
                    let! rightResult = rightStarted
                    return leftResult, rightResult
                }
                |> CancellableTask.getAwaiter