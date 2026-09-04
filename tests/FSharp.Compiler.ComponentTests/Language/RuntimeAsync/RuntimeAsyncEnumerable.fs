module RuntimeAsyncEnumerable

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Channels
open System.Threading.Tasks
open System.Threading.Tasks.Sources
open Microsoft.FSharp.Control
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
open RuntimeTaskBuilder

[<NoComparison; NoEquality>]
type AsyncSequenceEvent<'t> =
    | Item of 't
    | Completed
    | Faulted of exn

type AsyncManualResetSignal<'t>() =
    let mutable source = ManualResetValueTaskSourceCore<'t>()
    do source.RunContinuationsAsynchronously <- true

    member this.WaitAsync() = ValueTask<'t>(this, source.Version)
    member _.SetResult value = source.SetResult value
    member _.Reset() = source.Reset()

    interface IValueTaskSource<'t> with
        member _.GetResult(token) = source.GetResult(token)
        member _.GetStatus(token) = source.GetStatus(token)
        member _.OnCompleted(continuation, state, token, flags) =
            source.OnCompleted(continuation, state, token, flags)

[<NoComparison; NoEquality>]
type AsyncSequenceState<'T> = {
    MoveNextRequest: AsyncManualResetSignal<unit>
    ItemResponse: AsyncManualResetSignal<AsyncSequenceEvent<'T>>
    CancellationToken: CancellationToken
    KickOff: bool
}

module AsyncSequenceState =
    let publishItem state item = state.ItemResponse.SetResult(Item item)
    let publishCompleted state = state.ItemResponse.SetResult(Completed)
    let create cancellationToken =
        {
            MoveNextRequest = AsyncManualResetSignal<unit>()
            ItemResponse = AsyncManualResetSignal<AsyncSequenceEvent<'T>>()
            CancellationToken = cancellationToken
            KickOff = true
        }


type AsyncSeqEnumerator<'T>(state: AsyncSequenceState<'T>) =
    let mutable current = Unchecked.defaultof<'T>
    let mutable moveNextInProgress = 0

    interface IAsyncEnumerator<'T> with
        member _.Current = current

        member this.MoveNextAsync() =
            if Interlocked.Exchange(&moveNextInProgress, 1) = 1 then
                invalidOp "MoveNextAsync cannot be called concurrently."

            __runtimeAsyncReturnValueTask<bool>(
                try
                    state.MoveNextRequest.SetResult()

                    match AsyncHelpers.Await(state.ItemResponse.WaitAsync()) with
                    | Item value ->
                        current <- value
                        true
                    | Completed ->
                        current <- Unchecked.defaultof<'T>
                        false
                    | Faulted error ->
                        raise error
                finally
                    state.ItemResponse.Reset()
                    Interlocked.Exchange(&moveNextInProgress, 0) |> ignore
            )

        member this.DisposeAsync() = ValueTask.CompletedTask

type AsyncEnumerable<'T>(runProducer: AsyncSequenceState<'T> -> Task<unit>) =
    let getEnumerator ct =
        let state = AsyncSequenceState.create ct
        Task.Run<_>(fun () -> runProducer state) |> ignore
        AsyncSeqEnumerator<'T>(state)

    member _.RunProducer(state: AsyncSequenceState<'T>) = runProducer state

    interface IAsyncEnumerable<'T> with
        member _.GetAsyncEnumerator(cancellationToken: CancellationToken) =
            getEnumerator cancellationToken

type AsyncSequenceBody<'T> = AsyncSequenceState<'T> -> unit

type AsyncSeqBuilder() =
    member inline _.Delay([<InlineIfLambda>] generator: unit -> AsyncSequenceBody<'T>) : AsyncSequenceBody<'T> =
        fun state -> generator() state

    member inline _.Run([<InlineIfLambda>] code: AsyncSequenceBody<'T>) : IAsyncEnumerable<'T> =
        let runProducer state =
            __runtimeAsyncReturn(
                // wait for kick off.
                if state.KickOff then
                    AsyncHelpers.Await(state.MoveNextRequest.WaitAsync())
                    state.MoveNextRequest.Reset()

                code state

                if state.KickOff then AsyncSequenceState.publishCompleted state
            )

        AsyncEnumerable(runProducer)

    member inline _.Zero() : AsyncSequenceBody<'T> =
        fun _ -> ()

    member inline _.Return(_: unit) : AsyncSequenceBody<'T> =
        fun _ -> ()

    member inline _.ReturnFrom(task: Task) : AsyncSequenceBody<'T> =
        fun _ -> AsyncHelpers.Await task

    member inline _.ReturnFrom(task: Task<'U>) : AsyncSequenceBody<'T> =
        fun _ -> AsyncHelpers.Await task |> ignore

    member inline _.ReturnFrom(task: ValueTask) : AsyncSequenceBody<'T> =
        fun _ -> AsyncHelpers.Await task

    member inline _.ReturnFrom(task: ValueTask<'U>) : AsyncSequenceBody<'T> =
        fun _ -> AsyncHelpers.Await task |> ignore

    member inline _.ReturnFrom(computation: Async<'U>) : AsyncSequenceBody<'T> =
        fun _ -> AsyncHelpers.Await(Async.StartImmediateAsTask computation) |> ignore

    member inline _.ReturnFrom(computation: RuntimeTask<'U>) : AsyncSequenceBody<'T> =
        fun _ -> computation() |> ignore

    member inline _.Bind(task: Task, [<InlineIfLambda>] continuation: unit -> AsyncSequenceBody<'T>) : AsyncSequenceBody<'T> =
        fun state ->
            AsyncHelpers.Await task
            continuation() state

    member inline _.Bind(task: Task<'U>, [<InlineIfLambda>] continuation: 'U -> AsyncSequenceBody<'T>) : AsyncSequenceBody<'T> =
        fun state ->
            continuation (AsyncHelpers.Await task) state

    member inline _.Bind(task: ValueTask, [<InlineIfLambda>] continuation: unit -> AsyncSequenceBody<'T>) : AsyncSequenceBody<'T> =
        fun state ->
            AsyncHelpers.Await task
            continuation() state

    member inline _.Bind(task: ValueTask<'U>, [<InlineIfLambda>] continuation: 'U -> AsyncSequenceBody<'T>) : AsyncSequenceBody<'T> =
        fun state ->
            continuation (AsyncHelpers.Await task) state

    member inline _.Bind(computation: Async<'U>, [<InlineIfLambda>] continuation: 'U -> AsyncSequenceBody<'T>) : AsyncSequenceBody<'T> =
        fun state ->
            continuation (AsyncHelpers.Await(Async.StartImmediateAsTask computation)) state

    member inline _.Bind(computation: RuntimeTask<'U>, [<InlineIfLambda>] continuation: 'U -> AsyncSequenceBody<'T>) : AsyncSequenceBody<'T> =
        fun state ->
            continuation (computation()) state

    member inline _.Bind(
        values: struct ('U1 * 'U2),
        [<InlineIfLambda>] continuation: struct ('U1 * 'U2) -> AsyncSequenceBody<'T>
    ) : AsyncSequenceBody<'T> =
        fun state -> continuation values state

    member inline _.Combine(
        first: AsyncSequenceBody<'T>,
        [<InlineIfLambda>] second: AsyncSequenceBody<'T>
    ) : AsyncSequenceBody<'T> =
        fun state ->
            first state
            second state

    member inline _.TryWith(
        [<InlineIfLambda>] body: AsyncSequenceBody<'T>,
        [<InlineIfLambda>] handler: exn -> AsyncSequenceBody<'T>
    ) : AsyncSequenceBody<'T> =
        fun state ->
            try
                body state
            with error ->
                handler error state

    member inline _.TryFinally(
        [<InlineIfLambda>] body: AsyncSequenceBody<'T>,
        [<InlineIfLambda>] compensation: unit -> unit
    ) : AsyncSequenceBody<'T> =
        fun state ->
            try
                body state
            finally
                compensation()

    member inline _.Using(
        resource: 'Resource,
        [<InlineIfLambda>] body: 'Resource -> AsyncSequenceBody<'T>
    ) : AsyncSequenceBody<'T> =
        fun state ->
            try
                body resource state
            finally
                match box resource with
                | :? IAsyncDisposable as disposable -> AsyncHelpers.Await(disposable.DisposeAsync())
                | :? IDisposable as disposable -> disposable.Dispose()
                | _ -> ()

    member inline _.While(
        guard: unit -> bool,
        [<InlineIfLambda>] body: AsyncSequenceBody<'T>
    ) : AsyncSequenceBody<'T> =
        fun state ->
            while guard() do
                body state

    member inline _.For(
        sequence: seq<'U>,
        [<InlineIfLambda>] body: 'U -> AsyncSequenceBody<'T>
    ) : AsyncSequenceBody<'T> =
        fun state ->
            for item in sequence do
                body item state

    member inline _.For(
        sequence: IAsyncEnumerable<'U>,
        [<InlineIfLambda>] body: 'U -> AsyncSequenceBody<'T>
    ) : AsyncSequenceBody<'T> =
        fun state ->
            let innerEnumerator = sequence.GetAsyncEnumerator(state.CancellationToken)

            try
                while AsyncHelpers.Await(innerEnumerator.MoveNextAsync()) do
                    let value = innerEnumerator.Current
                    body value state
            finally
                AsyncHelpers.Await(innerEnumerator.DisposeAsync())

    member inline _.Yield(value: 'T) : AsyncSequenceBody<'T> =
        fun state ->
            AsyncSequenceState.publishItem state value
            AsyncHelpers.Await(state.MoveNextRequest.WaitAsync())
            state.MoveNextRequest.Reset()

    member inline _.YieldFrom(sequence: seq<'T>) : AsyncSequenceBody<'T> =
        fun state ->
            for value in sequence do
                AsyncSequenceState.publishItem state value
                AsyncHelpers.Await(state.MoveNextRequest.WaitAsync())
                state.MoveNextRequest.Reset()

    member inline _.YieldFrom(sequence: IAsyncEnumerable<'T>) : AsyncSequenceBody<'T> =
        fun state ->
            let innerEnumerator = sequence.GetAsyncEnumerator(state.CancellationToken)

            try
                while AsyncHelpers.Await(innerEnumerator.MoveNextAsync()) do
                    let value = innerEnumerator.Current
                    AsyncSequenceState.publishItem state value
                    AsyncHelpers.Await(state.MoveNextRequest.WaitAsync())
                    state.MoveNextRequest.Reset()
            finally
                AsyncHelpers.Await(innerEnumerator.DisposeAsync())

    member inline this.YieldFromFinal(sequence: IAsyncEnumerable<'T>) : AsyncSequenceBody<'T> =
        match sequence with
        | :? AsyncEnumerable<'T> as asyncSeq ->
            fun state ->
                AsyncHelpers.Await (asyncSeq.RunProducer { state with KickOff = false })
        | _ ->
            this.YieldFrom sequence

module AsyncSeqAwaitableExtensions =
    let inline awaitTaskLike
        ([<InlineIfLambda>] getAwaiter: unit -> 'Awaiter)
        ([<InlineIfLambda>] getResult: 'Awaiter -> 'T)
        =
        let awaiter = getAwaiter()
        AsyncHelpers.AwaitAwaiter awaiter
        getResult awaiter

    type AsyncSeqBuilder with
        [<NoEagerConstraintApplication>]
        member inline _.Bind< ^TaskLike, ^Awaiter, 'U, 'T
            when ^TaskLike: (member GetAwaiter: unit -> ^Awaiter)
            and ^Awaiter :> ICriticalNotifyCompletion
            and ^Awaiter: (member get_IsCompleted: unit -> bool)
            and ^Awaiter: (member GetResult: unit -> 'U)>
            (task: ^TaskLike, [<InlineIfLambda>] continuation: 'U -> AsyncSequenceBody<'T>)
            : AsyncSequenceBody<'T> =
            fun state ->
                let result =
                    awaitTaskLike
                        (fun () -> (^TaskLike: (member GetAwaiter: unit -> ^Awaiter) task))
                        (fun awaiter -> (^Awaiter: (member GetResult: unit -> 'U) awaiter))

                continuation result state

        [<NoEagerConstraintApplication>]
        member inline _.ReturnFrom< ^TaskLike, ^Awaiter, 'U, 'T
            when ^TaskLike: (member GetAwaiter: unit -> ^Awaiter)
            and ^Awaiter :> ICriticalNotifyCompletion
            and ^Awaiter: (member get_IsCompleted: unit -> bool)
            and ^Awaiter: (member GetResult: unit -> 'U)>
            (task: ^TaskLike)
            : AsyncSequenceBody<'T> =
            fun _ ->
                awaitTaskLike
                    (fun () -> (^TaskLike: (member GetAwaiter: unit -> ^Awaiter) task))
                    (fun awaiter -> (^Awaiter: (member GetResult: unit -> 'U) awaiter))
                |> ignore

        [<NoEagerConstraintApplication>]
        member inline _.MergeSources< ^TaskLike1, ^TaskLike2, ^Awaiter1, ^Awaiter2, 'U1, 'U2
            when ^TaskLike1: (member GetAwaiter: unit -> ^Awaiter1)
            and ^TaskLike2: (member GetAwaiter: unit -> ^Awaiter2)
            and ^Awaiter1 :> ICriticalNotifyCompletion
            and ^Awaiter2 :> ICriticalNotifyCompletion
            and ^Awaiter1: (member get_IsCompleted: unit -> bool)
            and ^Awaiter1: (member GetResult: unit -> 'U1)
            and ^Awaiter2: (member get_IsCompleted: unit -> bool)
            and ^Awaiter2: (member GetResult: unit -> 'U2)>
            (left: ^TaskLike1, right: ^TaskLike2)
            : struct ('U1 * 'U2) =
            let awaitLeft () =
                awaitTaskLike
                    (fun () -> (^TaskLike1: (member GetAwaiter: unit -> ^Awaiter1) left))
                    (fun awaiter -> (^Awaiter1: (member GetResult: unit -> 'U1) awaiter))

            let awaitRight () =
                awaitTaskLike
                    (fun () -> (^TaskLike2: (member GetAwaiter: unit -> ^Awaiter2) right))
                    (fun awaiter -> (^Awaiter2: (member GetResult: unit -> 'U2) awaiter))

            struct (awaitLeft(), awaitRight())

open AsyncSeqAwaitableExtensions

[<AutoOpen>]
module AsyncSeq =
    let asyncSeq = AsyncSeqBuilder()
