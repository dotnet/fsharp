module RuntimeTaskBuilder

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

open Microsoft.FSharp.Core.CompilerServices

module TasklikeHelpers =

    /// A structure that looks like an Awaiter
    type Awaiter<'Awaiter, 'TResult
        when 'Awaiter :> ICriticalNotifyCompletion
        and 'Awaiter: (member get_IsCompleted: unit -> bool)
        and 'Awaiter: (member GetResult: unit -> 'TResult)> = 'Awaiter

    type Awaitable<'Awaitable, 'Awaiter, 'TResult
        when 'Awaitable: (member GetAwaiter: unit -> Awaiter<'Awaiter, 'TResult>)> = 'Awaitable

    module Awaiter =
        let inline isCompleted (awaiter: Awaiter<_, _>) = awaiter.get_IsCompleted ()
        let inline getResult (awaiter: Awaiter<_, _>) = awaiter.GetResult()
        let inline onCompleted (awaiter: Awaiter<_, _>) continuation = awaiter.OnCompleted continuation
        let inline unsafeOnCompleted (awaiter: Awaiter<_, _>) continuation = awaiter.UnsafeOnCompleted continuation

    module Awaitable =
        let inline getAwaiter (awaitable: Awaitable<_, _, _>) = awaitable.GetAwaiter()

open TasklikeHelpers

module RuntimeAsyncBuilderHelpers =

    // A delegate to unify dissimilar builder source types, this allows us to have no additional Bind or MergeSources overloads.
    // The delegate's invocation is inlined, so this is zero cost.
    type Started<'T> = delegate of unit -> 'T

    let inline startAwaitable awaitable =
        // Make sure the delegate captures only started awaitables to make MergeSources concurrent.
        let awaiter = Awaitable.getAwaiter awaitable
        Started(fun () ->
            AsyncHelpers.UnsafeAwaitAwaiter awaiter
            Awaiter.getResult awaiter)

open RuntimeAsyncBuilderHelpers

module RuntimeAsyncBuilder =
    let inline isAlreadyBackground () =
        isNull SynchronizationContext.Current && obj.ReferenceEquals(TaskScheduler.Current, TaskScheduler.Default)

type RuntimeAsyncBuilder() =

    member inline _.Delay([<InlineIfLambda>] generator: unit -> 'T) = generator

    member inline _.Zero() = ()
    member inline _.Return(value: 'T) = value

    member inline _.Combine(first: unit, [<InlineIfLambda>] second) =
        ignore first
        second()

    member inline _.Combine(first, [<InlineIfLambda>] second) =
        first()
        second()

    member inline _.TryWith([<InlineIfLambda>] body, [<InlineIfLambda>] handler) =
        try body() with error -> handler error

    member inline _.TryFinally([<InlineIfLambda>] body, [<InlineIfLambda>] compensation) =
        try body() finally compensation()

    member inline _.Using(resource, [<InlineIfLambda>] body) =
        try
            body resource
        finally
            match box resource with
            | :? IAsyncDisposable as disposable -> AsyncHelpers.Await(disposable.DisposeAsync())
            | :? IDisposable as disposable -> disposable.Dispose()
            | _ -> ()

    member inline _.While(guard, [<InlineIfLambda>] body) =
        while guard() do body()

    member inline _.For(sequence, [<InlineIfLambda>] body) =
        for item in sequence do body item

    member inline this.For(sequence: IAsyncEnumerable<'T>, [<InlineIfLambda>] body) =
        this.Using(sequence.GetAsyncEnumerator(), fun enumerator ->
            while enumerator.MoveNextAsync() |> AsyncHelpers.Await do
                body enumerator.Current)

    member inline _.Bind([<InlineIfLambda>] await: Started<'T>, [<InlineIfLambda>] continuation) =
        await.Invoke() |> continuation

    member inline _.ReturnFrom([<InlineIfLambda>]  await: Started<'T>) =
        await.Invoke()

    member inline _.MergeSources([<InlineIfLambda>] left: Started<'A>, [<InlineIfLambda>] right: Started<'B>) =
        Started(fun () ->
            let left = left.Invoke()
            let right = right.Invoke()
            struct (left, right))

[<AutoOpen>]
module SourceExtensionsLowPriority =
    type RuntimeAsyncBuilder with
        // sources consumed by For method
        member inline _.Source(sequence: 'T seq) = sequence
        member inline _.Source(sequence: IAsyncEnumerable<'T>) = sequence

[<AutoOpen>]
module SourceExtensionsMediumPriority =
    type RuntimeAsyncBuilder with
        // Bind tasklike awaitables not matching any of the above source types, with lower priority. 
        member inline _.Source(awaitable) = startAwaitable awaitable

[<AutoOpen>]
module SourceExtensionsHighPriority =
    type RuntimeAsyncBuilder with
        // Cannonical runtime async sources 
        member inline _.Source(task: Task<'T>) = Started(fun () -> task |> AsyncHelpers.Await)
        member inline _.Source(task: Task) = Started(fun () -> task |> AsyncHelpers.Await)
        member inline _.Source(task: ValueTask<'T>) = Started(fun () -> task |> AsyncHelpers.Await)
        member inline _.Source(task: ValueTask) = Started(fun () -> task |> AsyncHelpers.Await)
        // Bind also cold-start async computations
        member inline _.Source(computation: Async<'T>) =
            let task = Async.StartImmediateAsTask computation
            Started(fun () -> task |> AsyncHelpers.Await)

[<AutoOpen>]
module RuntimeTask =

    open RuntimeAsyncBuilder
    
    type RuntimeTaskBuilder() =
        inherit RuntimeAsyncBuilder()
        member inline _.Run([<InlineIfLambda>] code) : Task<'T> =
            __runtimeAsyncReturn(code())

    let runtimeTask = RuntimeTaskBuilder()

    type BackgroundRuntimeTaskBuilder() =
        inherit RuntimeAsyncBuilder()
        member inline _.Run([<InlineIfLambda>] code: unit -> 'T) : Task<'T> =
            if isAlreadyBackground() then
                __runtimeAsyncReturn(code())
            else
            Task.Run<'T>(fun () -> __runtimeAsyncReturn (code()))

    let backgroundRuntimeTask = BackgroundRuntimeTaskBuilder()
