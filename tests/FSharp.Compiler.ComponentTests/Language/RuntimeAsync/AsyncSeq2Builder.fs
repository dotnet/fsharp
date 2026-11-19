namespace Microsoft.FSharp.Control

#nowarn "57"
#nowarn "1204"

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Runtime.ExceptionServices
open System.Threading
open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices

/// A non-tail `yield!` of an asynchronous source, and an entry on the driver's stack while the
/// source runs. When the source ends, its fault is stored here so the yielding parent rethrows it
/// at the `yield!` site, where the parent's handlers can observe it.
[<Sealed; AllowNullLiteral; ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>]
type AsyncSeq2Frame<'T>(source: IAsyncEnumerable<'T>) =
    /// The open enumerator running this frame's source, while the frame is on the stack.
    [<DefaultValue>]
    val mutable internal Enumerator: IAsyncEnumerator<AsyncSeq2Step<'T>>

    [<DefaultValue>]
    val mutable internal Next: AsyncSeq2Frame<'T>

    [<DefaultValue>]
    val mutable internal Error: exn

    member _.Source = source

    member this.ThrowIfFailed() =
        if not (isNull this.Error) then
            ExceptionDispatchInfo.Throw this.Error

/// One step of a raw AsyncSeq2 source; consumed by AsyncSeq2Enumerator.
and [<Struct; NoEquality; NoComparison; ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>] AsyncSeq2Step<'T> =
    | Value of value: 'T
    | Source of frame: AsyncSeq2Frame<'T>
    | TailSource of source: IAsyncEnumerable<'T>

/// Presents a foreign async enumerator as a step producer, so the driver handles one enumerator shape.
[<Sealed>]
type internal AsyncSeq2External<'T>(inner: IAsyncEnumerator<'T>) =
    interface IAsyncEnumerator<AsyncSeq2Step<'T>> with
        member _.Current = Value inner.Current
        member _.MoveNextAsync() = inner.MoveNextAsync()
        member _.DisposeAsync() = inner.DisposeAsync()


/// Lifecycle of an AsyncSeq2 enumeration: Idle <-> Moving, then Done.
/// An enum rather than a union so transitions can use Interlocked.CompareExchange.
type internal AsyncSeq2State =
    | Idle = 0
    | Moving = 1
    /// Completed, faulted or disposed; every enumerator has been closed.
    | Done = 2

/// An asynchronous sequence, compatible with IAsyncEnumerable<'T>.
/// Nested `AsyncSeq2` sources are driven iteratively by the outermost enumerator, so deep or
/// recursive `yield!` does not grow the call stack.
[<Sealed>]
type AsyncSeq2<'T> private (source: IAsyncEnumerable<AsyncSeq2Step<'T>>) =

    [<ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)>]
    static member Create(source: IAsyncEnumerable<AsyncSeq2Step<'T>>) = AsyncSeq2 source

    member internal _.RawEnumerator(cancellationToken) =
        source.GetAsyncEnumerator(cancellationToken)

    member this.GetAsyncEnumerator(?cancellationToken: CancellationToken) =
        (this :> IAsyncEnumerable<'T>).GetAsyncEnumerator(defaultArg cancellationToken CancellationToken.None)

    interface IAsyncEnumerable<'T> with
        member _.GetAsyncEnumerator(cancellationToken) =
            new AsyncSeq2Enumerator<'T>(source.GetAsyncEnumerator cancellationToken, cancellationToken)
            :> IAsyncEnumerator<'T>

/// One enumeration of an AsyncSeq2: drives the root and every nested source it yields.
/// Live enumerators form a stack of frames, innermost on top; the root is the bottom frame.
/// A frame is pushed only once its enumerator is open and popped before that enumerator is
/// disposed, so a failing open or dispose never leaves a dead enumerator on the stack.
and [<Sealed>] internal AsyncSeq2Enumerator<'T>(root: IAsyncEnumerator<AsyncSeq2Step<'T>>, token: CancellationToken) =
    let mutable state = AsyncSeq2State.Idle
    let mutable top = AsyncSeq2Frame<'T>(null)
    let mutable current = Unchecked.defaultof<'T>
    do top.Enumerator <- root

    member private _.Open(source: IAsyncEnumerable<'T>) : IAsyncEnumerator<AsyncSeq2Step<'T>> =
        match source with
        | :? AsyncSeq2<'T> as nested -> nested.RawEnumerator token
        | _ -> AsyncSeq2External(source.GetAsyncEnumerator token)

    /// Runs a non-tail `yield!` child above its parent. If the child cannot be opened, the parent
    /// stays on top and rethrows the fault at the `yield!` site.
    member private this.Push(frame: AsyncSeq2Frame<'T>) =
        try
            frame.Enumerator <- this.Open frame.Source
            frame.Next <- top
            top <- frame
        with e ->
            frame.Error <- e

    member private this.MoveNextCore() : ValueTask<bool> =
        __runtimeAsyncReturnValueTask (
            let mutable produced = false
            let mutable completed = false

            try
                while not (produced || completed) do
                    let frame = top
                    let mutable ended = false
                    let mutable handoff = false
                    let mutable error: exn = null
                    let mutable tail: IAsyncEnumerable<'T> = null

                    try
                        if AsyncHelpers.Await(frame.Enumerator.MoveNextAsync()) then
                            match frame.Enumerator.Current with
                            | Value value ->
                                current <- value
                                produced <- true
                            | Source child -> this.Push child
                            | TailSource next ->
                                ended <- true
                                handoff <- true
                                tail <- next
                        else
                            ended <- true
                    with e ->
                        error <- e
                        ended <- true

                    if ended then
                        top <- frame.Next

                        try
                            AsyncHelpers.Await(frame.Enumerator.DisposeAsync())
                        with e ->
                            error <- e

                        frame.Enumerator <- null

                        // A tail `yield!` hands the frame to its child once the parent is closed.
                        if handoff && isNull error then
                            try
                                frame.Enumerator <- this.Open tail
                                top <- frame
                            with e ->
                                error <- e

                        // Not handed off: report the outcome to the frame below, or finish at the root.
                        if isNull frame.Enumerator then
                            frame.Next <- null
                            completed <- isNull top

                            if not completed then
                                frame.Error <- error
                            else
                                current <- Unchecked.defaultof<'T>

                                if not (isNull error) then
                                    ExceptionDispatchInfo.Throw error

                produced
            finally
                // Published to the next caller by completion of the returned task.
                state <- if completed then AsyncSeq2State.Done else AsyncSeq2State.Idle
        )

    member private _.DisposeCore() : ValueTask =
        __runtimeAsyncReturnValueTaskUnit (
            let mutable error: exn = null

            while not (isNull top) do
                let frame = top
                top <- frame.Next
                frame.Next <- null

                try
                    AsyncHelpers.Await(frame.Enumerator.DisposeAsync())
                with e ->
                    error <- e

                frame.Enumerator <- null

            if not (isNull error) then
                ExceptionDispatchInfo.Throw error
        )

    interface IAsyncEnumerator<'T> with
        member _.Current = current

        member this.MoveNextAsync() =
            match Interlocked.CompareExchange(&state, AsyncSeq2State.Moving, AsyncSeq2State.Idle) with
            | AsyncSeq2State.Idle -> this.MoveNextCore()
            | AsyncSeq2State.Done -> ValueTask<bool>(false)
            | _ -> invalidOp "Concurrent MoveNextAsync calls are not supported."

        member this.DisposeAsync() =
            match Interlocked.CompareExchange(&state, AsyncSeq2State.Done, AsyncSeq2State.Idle) with
            | AsyncSeq2State.Idle -> this.DisposeCore()
            | AsyncSeq2State.Moving -> invalidOp "DisposeAsync cannot be called while MoveNextAsync is pending."
            | _ -> ValueTask()

/// Builds a cold asynchronous sequence whose bindings share the enumerator's cancellation token.
type AsyncSeq2Builder() =

    member inline _.Zero() : seq<AsyncSeq2Step<'T>> = Seq.empty

    member inline _.Yield(value) = Seq.singleton (Value value)

    member inline _.Delay([<InlineIfLambda>] body: unit -> seq<AsyncSeq2Step<'T>>) = body

    member inline _.Combine(first, [<InlineIfLambda>] rest) = Seq.append first (Seq.delay rest)

    member inline _.For(source: seq<'U>, [<InlineIfLambda>] body: 'U -> seq<AsyncSeq2Step<'T>>) =
        Seq.collect body source

    member inline _.While([<InlineIfLambda>] guard, [<InlineIfLambda>] body) =
        RuntimeHelpers.EnumerateWhile guard (Seq.delay body)

    member inline _.TryFinally([<InlineIfLambda>] body, [<InlineIfLambda>] compensation) =
        RuntimeHelpers.EnumerateThenFinally (Seq.delay body) compensation

    member inline _.TryWith([<InlineIfLambda>] body, [<InlineIfLambda>] handler) =
        RuntimeHelpers.EnumerateTryWith (Seq.delay body) (fun _ -> 1) handler

    member inline _.Using(resource: 'R, [<InlineIfLambda>] body: 'R -> seq<'T>) =
        RuntimeHelpers.EnumerateThenFinally (Seq.delay (fun () -> body resource)) (fun () ->
            match box resource with
            | :? IAsyncDisposable as disposable -> AsyncHelpers.Await(disposable.DisposeAsync())
            | :? IDisposable as disposable -> disposable.Dispose()
            | _ -> ())

    member inline this.For(source: IAsyncEnumerable<'U>, [<InlineIfLambda>] body: 'U -> seq<'T>) =
        Seq.delay (fun () ->
            let enumerator =
                source.GetAsyncEnumerator(StateMachineHelpers.__runtimeAsyncSequenceCancellationToken ())

            RuntimeHelpers.EnumerateThenFinally
                (Seq.delay (fun () ->
                    this.While(
                        (fun () -> AsyncHelpers.Await(enumerator.MoveNextAsync())),
                        (fun () -> body enumerator.Current)
                    )))
                (fun () -> AsyncHelpers.Await(enumerator.DisposeAsync())))

    member inline this.YieldFrom(source: seq<'T>) =
        this.For(source, fun value -> this.Yield value)

    member inline this.YieldFromFinal(source: seq<'T>) = this.YieldFrom source

    member inline _.YieldFrom(source: IAsyncEnumerable<'T>) =
        Seq.delay (fun () ->
            let frame = AsyncSeq2Frame<'T>(source)

            Seq.append
                (Seq.singleton (Source frame))
                (Seq.delay (fun () ->
                    frame.ThrowIfFailed()
                    Seq.empty)))

    member inline _.YieldFromFinal(source: IAsyncEnumerable<'T>) = Seq.singleton (TailSource source)

    member inline _.Bind
        (
            [<InlineIfLambda>] source: Async2BuilderSources.Started<'U>,
            [<InlineIfLambda>] continuation: 'U -> seq<AsyncSeq2Step<'T>>
        ) =
        continuation (source.Invoke())

    member inline _.Bind
        (
            [<InlineIfLambda>] source: Async2BuilderSources.Cold<'U>,
            [<InlineIfLambda>] continuation: 'U -> seq<AsyncSeq2Step<'T>>
        ) =
        continuation (source.Invoke(StateMachineHelpers.__runtimeAsyncSequenceCancellationToken ()))

    member inline _.Run([<InlineIfLambda>] recipe: unit -> seq<AsyncSeq2Step<'T>>) : AsyncSeq2<'T> =
        StateMachineHelpers.__runtimeAsyncSequence recipe |> AsyncSeq2<'T>.Create

[<AutoOpen>]
module AsyncSeq2BuilderSourceExtensionsLowPriority =
    type AsyncSeq2Builder with

        member inline _.Source(awaitable) =
            Async2BuilderSources.startAwaitable awaitable

        member inline _.Source([<InlineIfLambda>] coldAwaitable) =
            Async2BuilderSources.startAwaitable (coldAwaitable ())

        member inline _.Source([<InlineIfLambda>] cancellableAwaitable) =
            Async2BuilderSources.startCancellableAwaitable cancellableAwaitable

[<AutoOpen>]
module AsyncSeq2BuilderSourceExtensionsHighPriority =
    type AsyncSeq2Builder with

        member inline _.Source(computation: Async2<'T>) =
            Async2BuilderSources.Cold(fun ct -> computation.StartTrampolined ct |> AsyncHelpers.Await)

        member inline _.Source(computation: Async<'T>) =
            Async2BuilderSources.Cold(fun ct -> Async.StartImmediateAsTask(computation, ct) |> AsyncHelpers.Await)

        member inline _.Source(source: seq<'T>) = source
        member inline _.Source(source: IAsyncEnumerable<'T>) = source

        member inline _.Source(task: Task<'T>) =
            Async2BuilderSources.Started(fun () -> AsyncHelpers.Await task)

        member inline _.Source(task: Task) =
            Async2BuilderSources.Started(fun () -> AsyncHelpers.Await task)

        member inline _.Source(task: ValueTask<'T>) =
            Async2BuilderSources.Started(fun () -> AsyncHelpers.Await task)

        member inline _.Source(task: ValueTask) =
            Async2BuilderSources.Started(fun () -> AsyncHelpers.Await task)

        member inline _.Source([<InlineIfLambda>] createTask: CancellationToken -> Task<'T>) =
            Async2BuilderSources.Cold(fun ct -> AsyncHelpers.Await(createTask ct))

        member inline _.Source([<InlineIfLambda>] createTask: CancellationToken -> Task) =
            Async2BuilderSources.Cold(fun ct -> AsyncHelpers.Await(createTask ct))

        member inline _.Source([<InlineIfLambda>] createTask: CancellationToken -> ValueTask<'T>) =
            Async2BuilderSources.Cold(fun ct -> AsyncHelpers.Await(createTask ct))

        member inline _.Source([<InlineIfLambda>] createTask: CancellationToken -> ValueTask) =
            Async2BuilderSources.Cold(fun ct -> AsyncHelpers.Await(createTask ct))

[<AutoOpen>]
module AsyncSeq2BuilderInstance =
    /// Builds an asynchronous sequence that can be consumed by async2 or .NET async enumeration.
    let asyncSeq2 = AsyncSeq2Builder()
