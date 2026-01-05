namespace Internal.Utilities.Library

open System
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices
open System.Runtime.ExceptionServices
open FSharp.Core.CompilerServices.StateMachineHelpers
open Microsoft.FSharp.Core.CompilerServices

#nowarn 3513

type IAsync2Invocation<'t> =
    abstract Task: Task<'t>
    abstract GetAwaiter: unit -> TaskAwaiter<'t>

and Async2<'t> =
    abstract Start: CancellationToken -> IAsync2Invocation<'t>
    abstract Await: CancellationToken -> IAsync2Invocation<'t>
    abstract TailCall: CancellationToken * TaskCompletionSource<'t> -> unit

module Async2Implementation =

    /// A structure that looks like an Awaiter
    type Awaiter<'Awaiter, 'TResult
        when 'Awaiter :> ICriticalNotifyCompletion
        and 'Awaiter: (member get_IsCompleted: unit -> bool)
        and 'Awaiter: (member GetResult: unit -> 'TResult)> = 'Awaiter

    type Awaitable<'Awaitable, 'Awaiter, 'TResult when 'Awaitable: (member GetAwaiter: unit -> Awaiter<'Awaiter, 'TResult>)> = 'Awaitable

    module Awaiter =
        let inline isCompleted (awaiter: Awaiter<_, _>) = awaiter.get_IsCompleted ()

        let inline getResult (awaiter: Awaiter<_, _>) = awaiter.GetResult()

        let inline onCompleted (awaiter: Awaiter<_, _>) continuation = awaiter.OnCompleted continuation

        let inline unsafeOnCompleted (awaiter: Awaiter<_, _>) continuation = awaiter.UnsafeOnCompleted continuation

    module Awaitable =
        let inline getAwaiter (awaitable: Awaitable<_, _, _>) = awaitable.GetAwaiter()

    type DynamicState =
        | Running
        | SetResult
        | SetException of ExceptionDispatchInfo
        | Awaiting of ICriticalNotifyCompletion
        | Bounce of DynamicState
        | Immediate of DynamicState

    type Trampoline private () =

        static let holder = new ThreadLocal<_>(fun () -> Trampoline())

        let mutable depth = 0

        [<Literal>]
        let MaxDepth = 50

        let mutable pending: Action voption = ValueNone
        let mutable running = false

        let start () =
            try
                running <- true

                while pending.IsSome do
                    let next = pending.Value
                    pending <- ValueNone
                    next.Invoke()
            finally
                running <- false

        let set action =
            pending <- ValueSome action

            if not running then
                start ()

        interface ICriticalNotifyCompletion with
            member _.OnCompleted continuation = set continuation
            member _.UnsafeOnCompleted continuation = set continuation

        member this.Ref: ICriticalNotifyCompletion ref = ref this

        static member Current = holder.Value

        member _.ShouldBounce =
            // We must check pending here because of MergeSources.
            not running
            || pending.IsNone
               && (depth <- depth + 1
                   depth % MaxDepth = 0)

    module ExceptionCache =
        let store = ConditionalWeakTable<exn, ExceptionDispatchInfo>()

        let inline CaptureOrRetrieve (exn: exn) =
            match store.TryGetValue exn with
            | true, edi when edi.SourceException = exn -> edi
            | _ ->
                let edi = ExceptionDispatchInfo.Capture exn

                try
                    store.Add(exn, edi)
                with _ ->
                    ()

                edi

        let inline Throw (exn: exn) =
            let edi = CaptureOrRetrieve exn
            edi.Throw()
            Unchecked.defaultof<_>

        let inline GetResultOrThrow awaiter =
            try
                Awaiter.getResult awaiter
            with exn ->
                Throw exn

    [<Struct>]
    type Async2Data<'t> =
        [<DefaultValue(false)>]
        val mutable Result: 't

        [<DefaultValue(false)>]
        val mutable MethodBuilder: AsyncTaskMethodBuilder<'t>

        [<DefaultValue(false)>]
        val mutable TailCallSource: TaskCompletionSource<'t> option

        [<DefaultValue(false)>]
        val mutable Awaited: bool

        [<DefaultValue(false)>]
        val mutable CancellationToken: CancellationToken

    type Async2StateMachine<'TOverall> = ResumableStateMachine<Async2Data<'TOverall>>
    type IAsync2StateMachine<'TOverall> = IResumableStateMachine<Async2Data<'TOverall>>
    type Async2ResumptionFunc<'TOverall> = ResumptionFunc<Async2Data<'TOverall>>
    type Async2ResumptionDynamicInfo<'TOverall> = ResumptionDynamicInfo<Async2Data<'TOverall>>

    type Async2Code<'TOverall, 'T> = ResumableCode<Async2Data<'TOverall>, 'T>

    [<Struct; NoComparison>]
    type Async2<'t, 'm when 'm :> IAsyncStateMachine and 'm :> IAsync2StateMachine<'t>> =
        [<DefaultValue(false)>]
        val mutable StateMachine: 'm

        member ts.Start(ct, awaited, ?tailCallSource) =
            let mutable copy = ts
            let mutable data = Async2Data()
            data.CancellationToken <- ct
            data.Awaited <- awaited
            data.TailCallSource <- tailCallSource
            data.MethodBuilder <- AsyncTaskMethodBuilder<'t>.Create()
            copy.StateMachine.Data <- data
            copy.StateMachine.Data.MethodBuilder.Start(&copy.StateMachine)
            copy :> IAsync2Invocation<'t>

        interface IAsync2Invocation<'t> with
            member ts.Task = ts.StateMachine.Data.MethodBuilder.Task

            member ts.GetAwaiter() =
                ts.StateMachine.Data.MethodBuilder.Task.GetAwaiter()

        interface Async2<'t> with
            member ts.Start ct = ts.Start(ct, false)

            member ts.Await ct = ts.Start(ct, true)

            member ts.TailCall(ct, tc) = ts.Start(ct, true, tc) |> ignore

    type Async2Dynamic<'t, 'm when 'm :> IAsyncStateMachine and 'm :> IAsync2StateMachine<'t>>(getCopy: bool -> 'm) =
        member ts.GetCopy(awaited) =
            Async2(StateMachine = getCopy awaited) :> Async2<_>

        interface Async2<'t> with
            member ts.Start ct = ts.GetCopy(false).Start(ct)

            member ts.Await ct = ts.GetCopy(true).Await(ct)

            member ts.TailCall(ct, tc) = ts.GetCopy(true).TailCall(ct, tc)

    [<AutoOpen>]
    module Async2Code =
        let inline filterCancellation ([<InlineIfLambda>] catch: exn -> Async2Code<_, _>) (exn: exn) =
            Async2Code(fun sm ->
                match exn with
                | :? OperationCanceledException as oce when
                    sm.Data.CancellationToken.IsCancellationRequested
                    || oce.CancellationToken = sm.Data.CancellationToken
                    ->
                    raise exn
                | _ -> (catch exn).Invoke(&sm))

        let inline throwIfCancellationRequested (code: Async2Code<_, _>) =
            Async2Code(fun sm ->
                sm.Data.CancellationToken.ThrowIfCancellationRequested()
                code.Invoke(&sm))

        let inline maybeBounce () =
            Async2Code(fun sm ->
                if sm.Data.Awaited && Trampoline.Current.ShouldBounce then
                    let __stack_yield_fin = ResumableCode.Yield().Invoke(&sm)

                    if not __stack_yield_fin then
                        sm.Data.MethodBuilder.AwaitOnCompleted(Trampoline.Current.Ref, &sm)

                    __stack_yield_fin
                else
                    true)

        let inline bindDynamic (sm: byref<Async2StateMachine<_>>, awaiter, [<InlineIfLambda>] continuation: _ -> Async2Code<_, _>) =
            if Awaiter.isCompleted awaiter then
                (Awaiter.getResult awaiter |> continuation).Invoke(&sm)
            else
                let resumptionFunc =
                    Async2ResumptionFunc(fun sm ->
                        let result = ExceptionCache.GetResultOrThrow awaiter
                        (continuation result).Invoke(&sm))

                sm.ResumptionDynamicInfo.ResumptionFunc <- resumptionFunc
                sm.ResumptionDynamicInfo.ResumptionData <- Awaiting awaiter
                false

        let inline bindAwaiter (awaiter, [<InlineIfLambda>] continuation: 'U -> Async2Code<'Data, 'T>) : Async2Code<'Data, 'T> =
            Async2Code(fun sm ->
                if __useResumableCode then
                    if Awaiter.isCompleted awaiter then
                        continuation(ExceptionCache.GetResultOrThrow awaiter).Invoke(&sm)
                    else
                        let __stack_yield_fin = ResumableCode.Yield().Invoke(&sm)

                        if __stack_yield_fin then
                            continuation(ExceptionCache.GetResultOrThrow awaiter).Invoke(&sm)
                        else
                            let mutable __stack_awaiter = awaiter
                            sm.Data.MethodBuilder.AwaitUnsafeOnCompleted(&__stack_awaiter, &sm)
                            false
                else
                    bindDynamic (&sm, awaiter, continuation))

        let inline bindAwaitable (awaitable, [<InlineIfLambda>] continuation) =
            bindAwaiter (Awaitable.getAwaiter awaitable, continuation)

        let inline bindCancellable
            ([<InlineIfLambda>] cancellable, [<InlineIfLambda>] continuation: 'U -> Async2Code<'Data, 'T>)
            : Async2Code<'Data, 'T> =
            Async2Code<'Data, 'T>(fun sm -> bindAwaitable(cancellable sm.Data.CancellationToken, continuation).Invoke(&sm))

        let inline bindCancellableAwaiter ([<InlineIfLambda>] getAwaiter, [<InlineIfLambda>] continuation) =
            Async2Code<'Data, 'T>(fun sm -> bindAwaiter(getAwaiter sm.Data.CancellationToken, continuation).Invoke(&sm))

    type Async2Builder() =

        member inline _.Delay(generator: unit -> Async2Code<'TOverall, 'T>) : Async2Code<'TOverall, 'T> =
            ResumableCode.Delay(fun () -> generator () |> throwIfCancellationRequested)

        [<DefaultValue>]
        member inline _.Zero() : Async2Code<'TOverall, unit> = ResumableCode.Zero()

        member inline _.Return(value: 'T) =
            Async2Code(fun sm ->
                sm.Data.Result <- value
                true)

        member inline _.Combine(code1: Async2Code<'TOverall, unit>, code2: Async2Code<'TOverall, 'T>) : Async2Code<'TOverall, 'T> =
            ResumableCode.Combine(code1, code2)

        member inline _.While([<InlineIfLambda>] condition: unit -> bool, body: Async2Code<'TOverall, unit>) : Async2Code<'TOverall, unit> =
            ResumableCode.While(condition, throwIfCancellationRequested body)

        member inline _.TryWith
            (body: Async2Code<'TOverall, 'T>, [<InlineIfLambda>] catch: exn -> Async2Code<'TOverall, 'T>)
            : Async2Code<'TOverall, 'T> =
            ResumableCode.TryWith(body, filterCancellation catch)

        member inline _.TryFinally
            (body: Async2Code<'TOverall, 'T>, [<InlineIfLambda>] compensation: unit -> unit)
            : Async2Code<'TOverall, 'T> =
            ResumableCode.TryFinally(
                body,
                ResumableCode<_, _>(fun _sm ->
                    compensation ()
                    true)
            )

        member inline _.Using<'Resource, 'TOverall, 'T when 'Resource :> IDisposable | null>
            (resource: 'Resource, body: 'Resource -> Async2Code<'TOverall, 'T>)
            : Async2Code<'TOverall, 'T> =
            ResumableCode.Using(resource, body)

        member inline _.For(sequence: seq<'T>, [<InlineIfLambda>] body: 'T -> Async2Code<'TOverall, unit>) : Async2Code<'TOverall, unit> =
            ResumableCode.For(sequence, fun x -> body x |> throwIfCancellationRequested)

        static member inline RunDynamic(code: Async2Code<'T, 'T>) : Async2<'T> =
            let initialResumptionFunc = Async2ResumptionFunc<'T>(fun sm -> code.Invoke &sm)

            let maybeBounce state =
                if Trampoline.Current.ShouldBounce then
                    Bounce state
                else
                    Immediate state

            let resumptionInfo awaited =
                let initialState =
                    if awaited then
                        maybeBounce Running
                    else
                        Immediate Running

                { new Async2ResumptionDynamicInfo<'T>(initialResumptionFunc, ResumptionData = initialState) with
                    member info.MoveNext(sm) =

                        let getCurrent () =
                            nonNull info.ResumptionData :?> DynamicState

                        let setState state = info.ResumptionData <- state

                        match getCurrent () with
                        | Immediate state ->
                            setState state
                            info.MoveNext &sm
                        | Running ->
                            let mutable keepGoing = true

                            try
                                if info.ResumptionFunc.Invoke(&sm) then
                                    setState (maybeBounce SetResult)
                                else
                                    keepGoing <- getCurrent () |> _.IsAwaiting
                            with exn ->
                                setState (maybeBounce <| SetException(ExceptionCache.CaptureOrRetrieve exn))

                            if keepGoing then
                                info.MoveNext &sm
                        | Awaiting awaiter ->
                            setState Running
                            let mutable awaiter = awaiter
                            sm.Data.MethodBuilder.AwaitUnsafeOnCompleted(&awaiter, &sm)
                        | Bounce next ->
                            setState next
                            sm.Data.MethodBuilder.AwaitOnCompleted(Trampoline.Current.Ref, &sm)
                        | SetResult ->
                            match sm.Data.TailCallSource with
                            | Some tcs -> tcs.SetResult sm.Data.Result
                            | _ -> sm.Data.MethodBuilder.SetResult sm.Data.Result
                        | SetException edi ->
                            match sm.Data.TailCallSource with
                            | Some tcs -> tcs.TrySetException(edi.SourceException) |> ignore
                            | _ -> sm.Data.MethodBuilder.SetException(edi.SourceException)

                    member _.SetStateMachine(sm, state) =
                        sm.Data.MethodBuilder.SetStateMachine(state)
                }

            Async2Dynamic<_, _>(fun awaited -> Async2StateMachine(ResumptionDynamicInfo = resumptionInfo awaited))

        member inline _.Run(code: Async2Code<'T, 'T>) : Async2<'T> =
            if __useResumableCode then
                __stateMachine<Async2Data<_>, _>

                    (MoveNextMethodImpl<_>(fun sm ->
                        __resumeAt sm.ResumptionPoint

                        let mutable error = ValueNone

                        let __stack_go1 = maybeBounce().Invoke(&sm)

                        if __stack_go1 then
                            try
                                let __stack_code_fin = code.Invoke(&sm)

                                if __stack_code_fin then
                                    let __stack_go2 = maybeBounce().Invoke(&sm)

                                    if __stack_go2 then
                                        match sm.Data.TailCallSource with
                                        | Some tcs -> tcs.SetResult sm.Data.Result
                                        | _ -> sm.Data.MethodBuilder.SetResult(sm.Data.Result)
                            with exn ->
                                error <- ValueSome(ExceptionCache.CaptureOrRetrieve exn)

                            if error.IsSome then
                                let __stack_go2 = maybeBounce().Invoke(&sm)

                                if __stack_go2 then
                                    match sm.Data.TailCallSource with
                                    | Some tcs -> tcs.SetException(error.Value.SourceException)
                                    | _ -> sm.Data.MethodBuilder.SetException(error.Value.SourceException)))

                    (SetStateMachineMethodImpl<_>(fun sm state -> sm.Data.MethodBuilder.SetStateMachine state))

                    (AfterCode<_, _>(fun sm -> Async2<_, _>(StateMachine = sm) :> Async2<'T>))
            else
                Async2Builder.RunDynamic(code)

open Async2Implementation

[<AutoOpen>]
module LowPriority =
    type Async2Builder with
        [<NoEagerConstraintApplication>]
        member inline this.Bind(awaitable, [<InlineIfLambda>] continuation) =
            bindAwaitable (awaitable, continuation)

        [<NoEagerConstraintApplication>]
        member inline this.ReturnFrom(awaitable) = this.Bind(awaitable, this.Return)

        [<NoEagerConstraintApplication>]
        member inline this.ReturnFromFinal(awaitable) = this.ReturnFrom(awaitable)

[<AutoOpen>]
module MediumPriority =
    type Async2Builder with
        member inline this.Bind(expr: Async<_>, [<InlineIfLambda>] continuation) =
            bindCancellableAwaiter ((fun ct -> Async.StartAsTask(expr, cancellationToken = ct).GetAwaiter()), continuation)

        member inline this.Bind(task: Task, [<InlineIfLambda>] continuation) =
            bindAwaiter (task.ConfigureAwait(false).GetAwaiter(), continuation)

        member inline this.Bind(task: Task<_>, [<InlineIfLambda>] continuation) =
            bindAwaiter (task.ConfigureAwait(false).GetAwaiter(), continuation)

        member inline this.ReturnFrom(task: Task) = this.Bind(task, this.Return)
        member inline this.ReturnFrom(task: Task<_>) = this.Bind(task, this.Return)
        member inline this.ReturnFrom(expr: Async<_>) = this.Bind(expr, this.Return)
        member inline this.ReturnFromFinal(task: Task) = this.ReturnFrom(task)
        member inline this.ReturnFromFinal(task: Task<_>) = this.ReturnFrom(task)
        member inline this.ReturnFromFinal(expr: Async<_>) = this.ReturnFrom(expr)

[<AutoOpen>]
module HighPriority =

    // When Async2 is bound, we know that it is not yet started (cold). This allows for some optimizations.
    type Async2Builder with
        member inline this.Bind(code: Async2<'U>, [<InlineIfLambda>] continuation) : Async2Code<'Data, 'T> =
            bindCancellable (code.Await, continuation)

        member inline this.ReturnFrom(code: Async2<'T>) : Async2Code<'T, 'T> = this.Bind(code, this.Return)

        member inline this.ReturnFromFinal(code: Async2<'T>) =
            Async2Code(fun sm ->
                match sm.Data.TailCallSource with
                | None ->
                    // This is the start of a tail call chain. we need to return here when the entire chain is done.
                    let __stack_tcs = TaskCompletionSource<_>()
                    code.TailCall(sm.Data.CancellationToken, __stack_tcs)
                    this.Bind(__stack_tcs.Task, this.Return).Invoke(&sm)
                | Some tcs ->
                    // We are already in a tail call chain.
                    code.TailCall(sm.Data.CancellationToken, tcs)
                    false // Return false to abandon this state machine and continue on the next one.
            )

[<AutoOpen>]
module Async2AutoOpens =

    let async2 = Async2Builder()

module Async2 =

    let CheckAndThrowToken = AsyncLocal<CancellationToken>()

    let startInThreadPool ct (code: Async2<_>) =
        Task.Run<'t>(fun () ->
            CheckAndThrowToken.Value <- ct
            code.Start ct |> _.Task)

    let inline startImmediate ct (code: Async2<_>) =
        let oldCt = CheckAndThrowToken.Value

        try
            CheckAndThrowToken.Value <- ct
            code.Start ct |> _.Task

        finally
            CheckAndThrowToken.Value <- oldCt

    let run ct (code: Async2<'t>) =
        startInThreadPool ct code |> _.GetAwaiter().GetResult()

    let runWithoutCancellation code = run CancellationToken.None code

    let startAsTaskWithoutCancellation code =
        startInThreadPool CancellationToken.None code

    let toAsync (code: Async2<'t>) =
        async {
            let! ct = Async.CancellationToken
            let task = startInThreadPool ct code
            return! Async.AwaitTask task
        }

    let fromValue (value: 't) : Async2<'t> = async2 { return value }

    let CancellationToken =
        async2.Run(
            Async2Code(fun sm ->
                sm.Data.Result <- sm.Data.CancellationToken
                true)
        )

type Async2 =
    static member Ignore(computation: Async2<_>) : Async2<unit> =
        async2 {
            let! _ = computation
            return ()
        }

    static member Start(computation: Async2<_>, ?cancellationToken: CancellationToken) : unit =
        let ct = defaultArg cancellationToken CancellationToken.None
        Async2.startInThreadPool ct computation |> ignore

    static member StartAsTask(computation: Async2<_>, ?cancellationToken: CancellationToken) : Task<_> =
        let ct = defaultArg cancellationToken CancellationToken.None
        Async2.startImmediate ct computation

    static member RunImmediate(computation: Async2<'T>, ?cancellationToken: CancellationToken) : 'T =
        let ct = defaultArg cancellationToken CancellationToken.None
        Async2.startImmediate ct computation |> _.GetAwaiter().GetResult()

    static member RunSynchronously(computation: Async2<'T>, ?cancellationToken: CancellationToken) : 'T =
        let ct = defaultArg cancellationToken CancellationToken.None
        Async2.run ct computation

    static member Parallel(computations: Async2<_> seq) =
        async2 {
            let! ct = Async2.CancellationToken
            use lcts = CancellationTokenSource.CreateLinkedTokenSource ct
            try 
                let tasks = computations |> Seq.map (Async2.startInThreadPool lcts.Token)
                return! Task.WhenAll tasks
            with exn ->
                lcts.Cancel()
                return raise exn
        }

    static member Sequential(computations: Async2<_> seq) =
        async2 {
            let results = ResizeArray()

            for c in computations do
                let! r = c
                results.Add r

            return results.ToArray()
        }

    static member Catch(computation: Async2<'T>) : Async2<Choice<'T, exn>> =
        async2 {
            try
                let! res = computation
                return Choice1Of2 res
            with exn ->
                return Choice2Of2 exn
        }

    static member TryCancelled(computation: Async2<'T>, compensation) =
        async2 {
            let! ct = Async2.CancellationToken
            let invocation = computation.Start ct

            try
                return! invocation
            finally
                if ct.IsCancellationRequested && invocation.Task.IsCanceled then
                    compensation ()
        }

    static member StartChild(computation: Async2<'T>) : Async2<Async2<'T>> =
        async2 {
            let! ct = Async2.CancellationToken
            return async2 { return! computation |> Async2.startInThreadPool ct }
        }

    static member StartChildAsTask(computation: Async2<'T>) : Async2<Task<'T>> =
        async2 {
            let! ct = Async2.CancellationToken
            let task = computation |> Async2.startInThreadPool ct
            return task
        }

    static member AwaitWaitHandle(waitHandle: WaitHandle) : Async2<bool> =
        async2 {
            let! ct = Async2.CancellationToken

            let tcs =
                TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously)

            use _ = ct.Register(fun () -> tcs.TrySetCanceled() |> ignore)

            let callback =
                WaitOrTimerCallback(fun _ timedOut -> tcs.TrySetResult(not timedOut) |> ignore)

            let handle =
                ThreadPool.RegisterWaitForSingleObject(waitHandle, callback, null, -1, true)

            try
                return! tcs.Task
            finally
                handle.Unregister(waitHandle) |> ignore
        }
