namespace Internal.Utilities.Library

open System
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

#nowarn 3513

type IAsync2Invocation<'t> =
    abstract Task: Task<'t>

and Async2<'t> =
    abstract StartImmediate: CancellationToken -> IAsync2Invocation<'t>
    abstract TailCall: CancellationToken * TaskCompletionSource<'t> voption -> unit
    abstract GetAwaiter: unit -> TaskAwaiter<'t>

module Async2Implementation =

    open FSharp.Core.CompilerServices.StateMachineHelpers

    open Microsoft.FSharp.Core.CompilerServices
    open System.Runtime.ExceptionServices

    let failIfNot condition message =
        if not condition then
            failwith message

    /// A structure that looks like an Awaiter
    type Awaiter<'Awaiter, 'TResult
        when 'Awaiter :> ICriticalNotifyCompletion
        and 'Awaiter: (member get_IsCompleted: unit -> bool)
        and 'Awaiter: (member GetResult: unit -> 'TResult)> = 'Awaiter

    type Awaitable<'Awaitable, 'Awaiter, 'TResult when 'Awaitable: (member GetAwaiter: unit -> Awaiter<'Awaiter, 'TResult>)> = 'Awaitable

    module Awaiter =
        let inline isCompleted (awaiter: ^Awaiter) : bool when ^Awaiter: (member get_IsCompleted: unit -> bool) = awaiter.get_IsCompleted ()

        let inline getResult (awaiter: ^Awaiter) : ^TResult when ^Awaiter: (member GetResult: unit -> ^TResult) = awaiter.GetResult()

        let inline onCompleted (awaiter: ^Awaiter) (continuation: Action) : unit when ^Awaiter :> INotifyCompletion =
            awaiter.OnCompleted continuation

        let inline unsafeOnCompleted (awaiter: ^Awaiter) (continuation: Action) : unit when ^Awaiter :> ICriticalNotifyCompletion =
            awaiter.UnsafeOnCompleted continuation

    type Trampoline private () =

        let ownerThreadId = Thread.CurrentThread.ManagedThreadId

        static let holder = new ThreadLocal<_>(fun () -> Trampoline())

        let mutable pending: Action voption = ValueNone
        let mutable running = false

        let start (action: Action) =
            try
                running <- true
                action.Invoke()

                while pending.IsSome do
                    let next = pending.Value
                    pending <- ValueNone
                    next.Invoke()
            finally
                running <- false

        let set action =
            failIfNot (Thread.CurrentThread.ManagedThreadId = ownerThreadId) "Trampoline used from wrong thread"
            failIfNot pending.IsNone "Trampoline used while already pending"

            if running then
                pending <- ValueSome action
            else
                start action

        interface ICriticalNotifyCompletion with
            member _.OnCompleted(continuation) = set continuation
            member _.UnsafeOnCompleted(continuation) = set continuation

        member this.Ref: ICriticalNotifyCompletion ref = ref this

        member this.Set action = set action

        static member Current = holder.Value

    type DynamicState =
        | Running
        | SetResult
        | SetException of ExceptionDispatchInfo
        | Awaiting of ICriticalNotifyCompletion
        | Bounce of DynamicState
        | Immediate of DynamicState

    module BindContext =
        [<Literal>]
        let bindLimit = 100

        let bindCount = new ThreadLocal<int>()

        let inline ResetBindCount () = bindCount.Value <- 0

        let inline IncrementBindCount () =
            bindCount.Value <- bindCount.Value + 1

            if bindCount.Value >= bindLimit then
                ResetBindCount()
                true
            else
                false

        let inline IncrementBindCountDynamic next =
            if IncrementBindCount() then Bounce next else Immediate next

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
        val mutable TailCallSource: TaskCompletionSource<'t> voption

        [<DefaultValue(false)>]
        val mutable CancellationToken: CancellationToken

    type Async2StateMachine<'TOverall> = ResumableStateMachine<Async2Data<'TOverall>>
    type IAsync2StateMachine<'TOverall> = IResumableStateMachine<Async2Data<'TOverall>>
    type Async2ResumptionFunc<'TOverall> = ResumptionFunc<Async2Data<'TOverall>>
    type Async2ResumptionDynamicInfo<'TOverall> = ResumptionDynamicInfo<Async2Data<'TOverall>>

    type Async2Code<'TOverall, 'T> = ResumableCode<Async2Data<'TOverall>, 'T>

    [<Struct; NoComparison>]
    type Async2Impl<'t, 'm when 'm :> IAsyncStateMachine and 'm :> IAsync2StateMachine<'t>> =
        [<DefaultValue(false)>]
        val mutable StateMachine: 'm

        member ts.Start(ct, tc) =
            let mutable copy = ts
            let mutable data = Async2Data()
            data.CancellationToken <- ct
            data.TailCallSource <- tc
            data.MethodBuilder <- AsyncTaskMethodBuilder<'t>.Create()
            copy.StateMachine.Data <- data
            copy.StateMachine.Data.MethodBuilder.Start(&copy.StateMachine)
            copy :> IAsync2Invocation<'t>

        interface IAsync2Invocation<'t> with
            member ts.Task = ts.StateMachine.Data.MethodBuilder.Task

        interface Async2<'t> with
            member ts.StartImmediate ct = ts.Start(ct, ValueNone)
            member ts.TailCall(ct, tc) = ts.Start(ct, tc) |> ignore

            member ts.GetAwaiter() =
                ts.Start(CancellationToken.None, ValueNone).Task.GetAwaiter()

    [<NoComparison>]
    type Async2ImplDynamic<'t, 'm when 'm :> IAsyncStateMachine and 'm :> IAsync2StateMachine<'t>>(getCopy: unit -> 'm) =

        member ts.Start(ct, tc) =
            let mutable copy = Async2Impl(StateMachine = getCopy ())
            copy.Start(ct, tc)

        interface Async2<'t> with
            member ts.StartImmediate ct = ts.Start(ct, ValueNone)
            member ts.TailCall(ct, tc) = ts.Start(ct, tc) |> ignore

            member ts.GetAwaiter() =
                ts.Start(CancellationToken.None, ValueNone).Task.GetAwaiter()

    [<AutoOpen>]
    module Async2Code =
        let inline filterCancellation ([<InlineIfLambda>] catch: exn -> Async2Code<_, _>) (exn: exn) =
            Async2Code(fun sm ->
                let ct = sm.Data.CancellationToken

                match exn with
                | :? OperationCanceledException as oce when ct.IsCancellationRequested || oce.CancellationToken = ct -> raise exn
                | _ -> (catch exn).Invoke(&sm))

        let inline throwIfCancellationRequested (code: Async2Code<_, _>) =
            Async2Code(fun sm ->
                sm.Data.CancellationToken.ThrowIfCancellationRequested()
                code.Invoke(&sm))

        let inline yieldOnBindLimit () =
            Async2Code<_, _>(fun sm ->
                if BindContext.IncrementBindCount() then
                    let __stack_yield_fin = ResumableCode.Yield().Invoke(&sm)

                    if not __stack_yield_fin then
                        sm.Data.MethodBuilder.AwaitOnCompleted(Trampoline.Current.Ref, &sm)

                    __stack_yield_fin
                else
                    true)

    type CancellableAwaiter<'t, 'a when Awaiter<'a, 't>> = CancellationToken -> 'a

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

        static member inline BindDynamic
            (sm: byref<Async2StateMachine<_>>, awaiter, [<InlineIfLambda>] continuation: _ -> Async2Code<_, _>)
            =
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

        member inline _.BindAwaiter
            (awaiter: Awaiter<_, _>, [<InlineIfLambda>] continuation: 'U -> Async2Code<'Data, 'T>)
            : Async2Code<'Data, 'T> =
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
                    Async2Builder.BindDynamic(&sm, awaiter, continuation))

        member inline this.BindCancellable
            ([<InlineIfLambda>] binding: CancellableAwaiter<'U, 'Awaiter>, [<InlineIfLambda>] continuation: 'U -> Async2Code<'Data, 'T>)
            : Async2Code<'Data, 'T> =
            Async2Code(fun sm -> this.BindAwaiter(binding sm.Data.CancellationToken, continuation).Invoke(&sm))

        member inline this.Bind(code: Async2<'U>, [<InlineIfLambda>] continuation: 'U -> Async2Code<'Data, 'T>) : Async2Code<'Data, 'T> =
            Async2Code(fun sm -> this.BindCancellable((fun ct -> code.StartImmediate(ct).Task.GetAwaiter()), continuation).Invoke(&sm))

        member inline this.Bind(awaiter, [<InlineIfLambda>] continuation) = this.BindAwaiter(awaiter, continuation)

        member inline this.Bind(cancellable, [<InlineIfLambda>] continuation) =
            this.BindCancellable(cancellable, continuation)

        member inline this.ReturnFrom(code: Async2<'T>) : Async2Code<'T, 'T> = this.Bind(code, this.Return)

        member inline this.ReturnFrom(awaiter) = this.BindAwaiter(awaiter, this.Return)

        member inline this.ReturnFrom(cancellable) =
            this.BindCancellable(cancellable, this.Return)

        member inline this.ReturnFromFinal(code: Async2<'T>) =
            Async2Code(fun sm ->
                let __stack_ct = sm.Data.CancellationToken

                match sm.Data.TailCallSource with
                | ValueNone ->
                    // This is the start of a tail call chain. we need to return here when the entire chain is done.
                    let __stack_tcs = TaskCompletionSource<_>()
                    code.TailCall(__stack_ct, ValueSome __stack_tcs)
                    //Trampoline.Current.Set(fun () -> code.TailCall(__stack_ct, ValueSome __stack_tcs))
                    this.BindAwaiter(__stack_tcs.Task.GetAwaiter(), this.Return).Invoke(&sm)
                | ValueSome tcs ->
                    // We are already in a tail call chain.
                    BindContext.ResetBindCount()
                    //code.TailCall(__stack_ct, ValueSome tcs)
                    Trampoline.Current.Set(fun () -> code.TailCall(__stack_ct, ValueSome tcs))
                    false // Return false to abandon this state machine and continue on the next one.
            )

        member inline this.ReturnFromFinal(awaiter) : Async2Code<'T, 'T> = this.BindAwaiter(awaiter, this.Return)

        member inline this.ReturnFromFinal(cancellable) : Async2Code<'T, 'T> =
            this.BindCancellable(cancellable, this.Return)

        static member inline RunDynamic(code: Async2Code<'T, 'T>) : Async2<'T> =
            let initialResumptionFunc = Async2ResumptionFunc<'T>(fun sm -> code.Invoke &sm)

            let resumptionInfo () =
                { new Async2ResumptionDynamicInfo<'T>(initialResumptionFunc,
                                                      ResumptionData = (BindContext.IncrementBindCountDynamic Running)) with
                    member info.MoveNext(sm) =

                        let getCurrent () =
                            nonNull info.ResumptionData :?> DynamicState

                        let setState state = info.ResumptionData <- box state

                        match getCurrent () with
                        | Immediate state ->
                            setState state
                            info.MoveNext &sm
                        | Running ->
                            let mutable keepGoing = true

                            try
                                if info.ResumptionFunc.Invoke(&sm) then
                                    setState (BindContext.IncrementBindCountDynamic SetResult)
                                else
                                    keepGoing <- getCurrent () |> _.IsAwaiting
                            with exn ->
                                setState (
                                    BindContext.IncrementBindCountDynamic
                                    <| SetException(ExceptionCache.CaptureOrRetrieve exn)
                                )

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
                            | ValueSome tcs -> tcs.SetResult sm.Data.Result
                            | _ -> sm.Data.MethodBuilder.SetResult sm.Data.Result
                        | SetException edi ->
                            match sm.Data.TailCallSource with
                            | ValueSome tcs -> tcs.TrySetException(edi.SourceException) |> ignore
                            | _ -> sm.Data.MethodBuilder.SetException(edi.SourceException)

                    member _.SetStateMachine(sm, state) =
                        sm.Data.MethodBuilder.SetStateMachine(state)
                }

            Async2ImplDynamic<_, _>(fun () -> Async2StateMachine(ResumptionDynamicInfo = resumptionInfo ()))

        member inline _.Run(code: Async2Code<'T, 'T>) : Async2<'T> =
            if __useResumableCode then
                __stateMachine<Async2Data<_>, _>

                    (MoveNextMethodImpl<_>(fun sm ->
                        __resumeAt sm.ResumptionPoint
                        let mutable error = ValueNone

                        let __stack_go1 = yieldOnBindLimit().Invoke(&sm)

                        if __stack_go1 then
                            try
                                let __stack_code_fin = code.Invoke(&sm)

                                if __stack_code_fin then
                                    let __stack_go2 = yieldOnBindLimit().Invoke(&sm)

                                    if __stack_go2 then
                                        match sm.Data.TailCallSource with
                                        | ValueSome tcs -> tcs.SetResult sm.Data.Result
                                        | _ -> sm.Data.MethodBuilder.SetResult(sm.Data.Result)
                            with exn ->
                                error <- ValueSome(ExceptionCache.CaptureOrRetrieve exn)

                            if error.IsSome then
                                let __stack_go2 = yieldOnBindLimit().Invoke(&sm)

                                if __stack_go2 then
                                    match sm.Data.TailCallSource with
                                    | ValueSome tcs -> tcs.SetException(error.Value.SourceException)
                                    | _ -> sm.Data.MethodBuilder.SetException(error.Value.SourceException)))

                    (SetStateMachineMethodImpl<_>(fun sm state -> sm.Data.MethodBuilder.SetStateMachine state))

                    (AfterCode<_, _>(fun sm -> Async2Impl<_, _>(StateMachine = sm) :> Async2<'T>))
            else
                Async2Builder.RunDynamic(code)

        member inline _.Source(code: Async2<_>) = code

[<AutoOpen>]
module Async2AutoOpens =
    open Async2Implementation

    let async2 = Async2Builder()

[<AutoOpen>]
module Async2LowPriority =
    open Async2Implementation

    type Async2Builder with
        member inline _.Source(awaitable: Awaitable<_, _, _>) = awaitable.GetAwaiter()

        member inline _.Source(items: _ seq) : _ seq = upcast items

[<AutoOpen>]
module Async2MediumPriority =
    open Async2Implementation

    type Async2Builder with
        member inline _.Source(task: Task) = task.ConfigureAwait(false).GetAwaiter()
        member inline _.Source(task: Task<_>) = task.ConfigureAwait(false).GetAwaiter()

        member inline this.Source(expr: Async<'T>) : CancellableAwaiter<_, _> =
            fun ct -> Async.StartAsTask(expr, cancellationToken = ct).GetAwaiter()

open Async2Implementation

module Async2 =

    let CheckAndThrowToken = AsyncLocal<CancellationToken>()

    let inline start (code: Async2<_>) ct =
        CheckAndThrowToken.Value <- ct
        // Only bound computations can participate in trampolining, otherwise we risk sync over async deadlocks.
        // To prevent this, we reset the bind count here.
        // This computation will not initially bounce, even if it is nested inside another async2 computation.
        BindContext.ResetBindCount()
        code.StartImmediate ct

    let run ct (code: Async2<'t>) =

        if
            isNull SynchronizationContext.Current
            && TaskScheduler.Current = TaskScheduler.Default
        then
            start code ct |> _.Task.GetAwaiter().GetResult()
        else
            Task.Run<'t>(fun () -> start code ct |> _.Task).GetAwaiter().GetResult()

    let runWithoutCancellation code = run CancellationToken.None code

    let startAsTaskWithoutCancellation code = start code CancellationToken.None

    let startAsTask ct code = start code ct |> _.Task

    let queue ct code = Task.Run(fun () -> start code ct)

    let queueTask ct code =
        Task.Run<'t>(fun () -> startAsTask ct code)

    let toAsync (code: Async2<'t>) =
        async {
            let! ct = Async.CancellationToken
            let task = startAsTask ct code
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
        Async2.queueTask ct computation |> ignore

    static member StartAsTask(computation: Async2<_>, ?cancellationToken: CancellationToken) : Task<_> =
        let ct = defaultArg cancellationToken CancellationToken.None
        Async2.startAsTask ct computation

    static member RunImmediate(computation: Async2<'T>, ?cancellationToken: CancellationToken) : 'T =
        let ct = defaultArg cancellationToken CancellationToken.None
        Async2.run ct computation

    static member Parallel(computations: Async2<_> seq) =
        async2 {
            let! ct = Async2.CancellationToken
            use lcts = CancellationTokenSource.CreateLinkedTokenSource ct

            let tasks =
                seq {
                    for c in computations do
                        async2 {
                            try
                                return! c
                            with exn ->
                                lcts.Cancel()
                                return raise exn
                        }
                        |> Async2.queueTask lcts.Token
                }

            return! Task.WhenAll tasks
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
            let task = computation |> Async2.startAsTask ct

            try
                return! task
            finally
                if task.IsCanceled then
                    compensation ()
        }

    static member StartChild(computation: Async2<'T>) : Async2<Async2<'T>> =
        async2 {
            let! ct = Async2.CancellationToken
            return async2 { return! computation |> Async2.queueTask ct }
        }

    static member StartChildAsTask(computation: Async2<'T>) : Async2<Task<'T>> =
        async2 {
            let! ct = Async2.CancellationToken
            let task = computation |> Async2.queueTask ct
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
