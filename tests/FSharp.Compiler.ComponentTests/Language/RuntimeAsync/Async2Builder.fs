namespace Microsoft.FSharp.Control

open System
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open System.Collections.Generic
open Microsoft.FSharp.Core.CompilerServices

module AwaitableHelpers =

    /// A structure that looks like an Awaiter
    type Awaiter<'Awaiter, 'TResult
        when 'Awaiter :> ICriticalNotifyCompletion
        and 'Awaiter: (member get_IsCompleted: unit -> bool)
        and 'Awaiter: (member GetResult: unit -> 'TResult)> = 'Awaiter

    type Awaitable<'Awaitable, 'Awaiter, 'TResult
        when 'Awaitable: (member GetAwaiter: unit -> Awaiter<'Awaiter, 'TResult>)> = 'Awaitable

    type ColdAwaitable<'Awaitable, 'Awaiter, 'TResult
        when 'Awaitable: (member GetAwaiter: unit -> Awaiter<'Awaiter, 'TResult>)> =
        unit -> 'Awaitable

    type CancellableAwaitable<'Awaitable, 'Awaiter, 'TResult
        when 'Awaitable: (member GetAwaiter: unit -> Awaiter<'Awaiter, 'TResult>)> =
        CancellationToken -> 'Awaitable

    module Awaiter =
        let inline isCompleted (awaiter: Awaiter<_, _>) = awaiter.get_IsCompleted ()
        let inline getResult (awaiter: Awaiter<_, _>) = awaiter.GetResult()

        let inline onCompleted (awaiter: Awaiter<_, _>) continuation =
            awaiter.OnCompleted continuation

        let inline unsafeOnCompleted (awaiter: Awaiter<_, _>) continuation =
            awaiter.UnsafeOnCompleted continuation

    module Awaitable =
        let inline getAwaiter (awaitable: Awaitable<_, _, _>) = awaitable.GetAwaiter()

open AwaitableHelpers

module Async2BuilderSources =

    // A delegate to unify dissimilar builder source types, this allows us to have no additional Bind or MergeSources overloads.
    // The delegate takes the cancellation token threaded through the builder; its invocation is inlined, so this is zero cost.
    type Started<'T> = delegate of unit -> 'T
    // We need to distinguish between hot and cold awaitables, we can pass the cancellation token only to the cold ones.
    // Ideally the signature should be CancellationToken -> Started<'T>, but the Started<_> delegates execute AsyncHelpers.Await
    // and must be inlined unconditionally into async method body.
    type Cold<'T> = delegate of CancellationToken -> 'T

    let inline startAwaitable awaitable =
        let awaiter = Awaitable.getAwaiter awaitable

        Started(fun () ->
            if not (Awaiter.isCompleted awaiter) then
                AsyncHelpers.UnsafeAwaitAwaiter awaiter
            Awaiter.getResult awaiter
        )

    let inline startCancellableAwaitable cancellableAwaitable =
        Cold(fun ct ->
            let awaiter =
                cancellableAwaitable ct
                |> Awaitable.getAwaiter

            if not (Awaiter.isCompleted awaiter) then
                AsyncHelpers.UnsafeAwaitAwaiter awaiter
            Awaiter.getResult awaiter
        )

open Async2BuilderSources

module internal Async2StartTrampoline =
    type private State() =
        let queue = Queue<unit -> unit>()
        member _.Queue = queue
        member val IsRunning = false with get, set

    let private state = new ThreadLocal<State>(fun () -> State())

    let private drain (current: State) =
        while current.Queue.Count > 0 do
            current.Queue.Dequeue()()

    let private enqueueCompletion action =
        let current = state.Value

        if current.IsRunning then
            current.Queue.Enqueue(action)
        else
            current.IsRunning <- true

            try
                current.Queue.Enqueue(action)
                drain current
            finally
                current.IsRunning <- false

    let private complete<'T> (task: Task<'T>) (completion: TaskCompletionSource<'T>) =
        if task.IsCanceled then
            let cancellationToken =
                try
                    task.GetAwaiter().GetResult() |> ignore
                    CancellationToken.None
                with
                | :? OperationCanceledException as error -> error.CancellationToken

            completion.TrySetCanceled(cancellationToken) |> ignore
        elif task.IsFaulted then
            completion.TrySetException(task.Exception.InnerExceptions) |> ignore
        else
            completion.TrySetResult(task.Result) |> ignore

    let private startQueued<'T> (start: unit -> Task<'T>) (completion: TaskCompletionSource<'T>) =
        try
            let task = start ()

            if task.IsCompleted then
                enqueueCompletion (fun () -> complete task completion)
            else
                task.ContinueWith(
                    Action<Task<'T>>(fun _ ->
                        enqueueCompletion (fun () -> complete task completion)),
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default
                )
                |> ignore
        with error ->
            enqueueCompletion (fun () -> completion.TrySetException(error) |> ignore)

    let start<'T> (start: unit -> Task<'T>) =
        let current = state.Value

        if current.IsRunning then
            let completion = TaskCompletionSource<'T>()
            current.Queue.Enqueue(fun () -> startQueued start completion)
            completion.Task
        else
            current.IsRunning <- true

            try
                start ()
            finally
                try
                    drain current
                finally
                    current.IsRunning <- false

    let startIsolated<'T> (startComputation: unit -> Task<'T>) =
        let previous = state.Value
        state.Value <- State()

        try
            startComputation ()
        finally
            state.Value <- previous

[<Sealed; NoEquality; NoComparison; CompiledName("FSharpAsync2`1")>]
type Async2<'T> (start: CancellationToken -> ValueTask<'T>) =
    member _.Start ct = start ct |> _.AsTask()

    [<System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)>]
    member _.StartTrampolined ct =
        Async2StartTrampoline.start (fun () -> start ct |> _.AsTask())

    member internal _.StartInIsolatedTrampoline ct =
        Async2StartTrampoline.startIsolated (fun () -> start ct |> _.AsTask())

type Async2Code<'T> = CancellationToken -> 'T

module Async2Builder =
    let inline isAlreadyBackground () =
        isNull SynchronizationContext.Current
        && obj.ReferenceEquals(TaskScheduler.Current, TaskScheduler.Default)

    let inline check (cancellationToken: CancellationToken) =
        cancellationToken.ThrowIfCancellationRequested()

open Async2Builder

type Async2Builder() =

    // The code type of the builder is `CancellationToken -> 'T`, i.e. the cancellation token is
    // passed along as state to every delayed continuation.
    member inline _.Delay([<InlineIfLambda>] generator) : Async2Code<'T> =
        fun ct ->
            check ct
            generator () ct

    member inline _.Zero() : Async2Code<unit> =
        fun ct ->
            check ct
            ()

    member inline _.Return(value: 'T) : Async2Code<'T> =
        fun ct ->
            check ct
            value

    member inline _.Combine([<InlineIfLambda>] first, [<InlineIfLambda>] second) : Async2Code<'T> =
        fun ct ->
            check ct
            first ct |> ignore
            second ct

    member inline _.TryWith ([<InlineIfLambda>] body, [<InlineIfLambda>] handler) : Async2Code<'T> =
        fun ct ->
            check ct
            try
                body ct
            with error ->
                check ct
                handler error ct

    member inline _.TryFinally ([<InlineIfLambda>] body, [<InlineIfLambda>] compensation) : Async2Code<'T> =
        fun ct ->
            try
                check ct
                body ct
            finally
                compensation ()

    member inline _.Using(resource: 'T :> IDisposable | null, [<InlineIfLambda>] body) : Async2Code<'U> =
        fun ct ->
            try
                check ct
                body resource ct
            finally
                if not (isNull (box resource)) then resource.Dispose()

    member inline _.While(guard: unit -> bool, [<InlineIfLambda>] body) : Async2Code<unit> =
        fun ct ->
            while guard () do
                check ct
                body ct
            check ct

    member inline _.For(sequence: seq<'T>, [<InlineIfLambda>] body) : Async2Code<unit> =
        fun ct ->
            for item in sequence do
                check ct
                body item ct
            check ct

    member inline _.Bind([<InlineIfLambda>] awaited: Started<'T>,[<InlineIfLambda>] continuation) : Async2Code<'U> =
        fun ct ->
            check ct
            continuation (awaited.Invoke()) ct

    member inline this.Bind([<InlineIfLambda>] cancellable: Cold<'T>,[<InlineIfLambda>] continuation) : Async2Code<'U> =
        fun ct ->
            check ct
            continuation (cancellable.Invoke ct) ct

    member inline _.ReturnFrom([<InlineIfLambda>] awaited: Started<'T>) : Async2Code<'T> =
        fun _ -> awaited.Invoke()

    member inline _.ReturnFrom([<InlineIfLambda>] cancellable: Cold<'T>) : Async2Code<'T> =
        fun ct -> cancellable.Invoke ct

    member inline _.MergeSources([<InlineIfLambda>] left: Started<'A>, [<InlineIfLambda>] right: Started<'B>) =
        let left = left.Invoke()
        let right = right.Invoke()
        Started(fun () -> struct (left, right))

    member inline this.MergeSources([<InlineIfLambda>] left: Cold<'A>, [<InlineIfLambda>] right: Cold<'B>) =
        Cold(fun ct ->
            let right = __runtimeAsyncReturnValueTask (right.Invoke ct)
            let left = left.Invoke ct

            struct (left,
                    right
                    |> AsyncHelpers.Await)
        )

    member inline this.MergeSources ([<InlineIfLambda>] left: Started<'A>, [<InlineIfLambda>] right: Cold<'B>) =
        Cold(fun ct ->
            let right = right.Invoke ct
            let left = left.Invoke()
            struct (left, right)
        )

    member inline this.MergeSources ([<InlineIfLambda>] left: Cold<'A>, [<InlineIfLambda>] right: Started<'B>) =
        Cold(fun ct ->
            let left = left.Invoke ct
            let right = right.Invoke()
            struct (left, right)
        )

    member inline this.Source(computation: Async2<'T>) =
        Cold(fun ct -> computation.StartTrampolined ct |> AsyncHelpers.Await)

    member inline _.Run([<InlineIfLambda>] code: Async2Code<'T>) : Async2<'T> =
        Async2(fun ct -> __runtimeAsyncReturnValueTask (code ct))

[<AutoOpen>]
module Async2BuilderAsyncDisposableExtensions =
    type Async2Builder with
        member inline _.Using(resource: 'T :> IAsyncDisposable | null, [<InlineIfLambda>] body) : Async2Code<'U> =
            fun ct ->
                try
                    check ct
                    body resource ct
                finally
                    if not (isNull (box resource)) then
                        resource.DisposeAsync() |> AsyncHelpers.Await

        member inline this.For(sequence: IAsyncEnumerable<'T>, [<InlineIfLambda>] body) : Async2Code<unit> =
            fun ct ->
                this.Using
                    (sequence.GetAsyncEnumerator ct,
                     fun enumerator ct ->
                         while enumerator.MoveNextAsync()
                               |> AsyncHelpers.Await do
                             check ct
                             body enumerator.Current ct
                         check ct)
                    ct

[<AutoOpen>]
module Async2BuilderAwaitableExtensions =
    type Async2Builder with
        member inline _.Source(awaitable) = startAwaitable awaitable

        member inline this.Source([<InlineIfLambda>] coldAwaitable) =
            startAwaitable (coldAwaitable ())

        member inline this.Source([<InlineIfLambda>] cancellableAwaitable) =
            startCancellableAwaitable cancellableAwaitable

[<AutoOpen>]
module Async2BuilderSourceExtensions =
    type Async2Builder with

        // Accepted sources for For
        member inline _.Source(sequence: 'T seq) = sequence
        member inline _.Source(sequence: IAsyncEnumerable<'T>) = sequence

        // Cannonical runtime async Bind sources
        member inline _.Source(task: Task<'T>) = Started(fun () -> task |> AsyncHelpers.Await)
        member inline _.Source(task: Task) = Started(fun () -> task |> AsyncHelpers.Await)
        member inline _.Source(task: ValueTask<'T>) = Started(fun () -> task |> AsyncHelpers.Await)
        member inline _.Source(task: ValueTask) = Started(fun () -> task |> AsyncHelpers.Await)

         //Cold start sources
        member inline this.Source(computation: Async<'T>) =
            Cold(fun ct -> Async.StartImmediateAsTask(computation, ct) |> AsyncHelpers.Await)

[<AutoOpen>]
module Async2BuilderImpl =
    let async2 = Async2Builder()
