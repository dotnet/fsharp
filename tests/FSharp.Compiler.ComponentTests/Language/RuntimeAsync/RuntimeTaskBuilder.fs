module RuntimeTaskBuilder

open System
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open System.Collections.Generic
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Collections

module InternalHelpers =

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

open InternalHelpers

type RuntimeTaskBuilder() =

    member inline this.ReturnFrom(source) = source

    member inline _.Bind(source, continuation) = continuation source

    member inline _.Delay([<InlineIfLambda>] generator: unit -> 'T) : unit -> 'T = generator

    member inline _.Run([<InlineIfLambda>] code: unit -> 'T) : Task<'T> =
        __runtimeAsyncReturn (code())

    member inline _.Zero() = ()
    member inline _.Return(value: 'T) = value

    member inline _.Combine(first: unit, [<InlineIfLambda>] second) =
        ignore first
        second()

    member inline _.Combine(first, [<InlineIfLambda>] second) =
        first()
        second()
    member inline _.TryWith([<InlineIfLambda>] body: unit -> 'T, [<InlineIfLambda>] handler: exn -> 'T) =
        try body() with error -> handler error
    member inline _.TryFinally([<InlineIfLambda>] body: unit -> 'T, [<InlineIfLambda>] compensation: unit -> unit) =
        try body() finally compensation()
    member inline _.Using(resource, [<InlineIfLambda>] body) =
        try
            body resource
        finally
            match box resource with
            | :? IAsyncDisposable as disposable -> AsyncHelpers.Await(disposable.DisposeAsync())
            | :? IDisposable as disposable -> disposable.Dispose()
            | _ -> ()

    member inline _.While(guard: unit -> bool, [<InlineIfLambda>] body: unit -> unit) =
        while guard() do body()

    member inline _.For(sequence: seq<'T>, [<InlineIfLambda>] body: 'T -> unit) =
        for item in sequence do body item

    member inline this.For(sequence: IAsyncEnumerable<'T>, [<InlineIfLambda>] body: 'T -> unit) =
        this.Using(sequence.GetAsyncEnumerator(), fun enumerator ->
            while enumerator.MoveNextAsync() |> AsyncHelpers.Await do
                body enumerator.Current)

    member inline _.MergeSources(left, right) = struct(left, right)

    member inline _.Source(sequence: seq<'T>) = sequence // sketchy
    member inline _.Source(sequence: IAsyncEnumerable<'T>) = sequence // sketchy
    member inline _.Source(task: Task<'T>) = AsyncHelpers.Await task
    member inline _.Source(task: Task) = AsyncHelpers.Await task
    member inline _.Source(task: ValueTask<'T>) = AsyncHelpers.Await task
    member inline _.Source(task: ValueTask) = AsyncHelpers.Await task
    member inline _.Source(computation: Async<'T>) = AsyncHelpers.Await(Async.StartImmediateAsTask computation)

type BackgroundTaskBuilder() =
    inherit RuntimeTaskBuilder()
    member inline _.Run([<InlineIfLambda>] code: unit -> 'T) : Task<'T> =
        Task.Run<'T>(fun () -> __runtimeAsyncReturn (code()))

module RuntimeTask =
    let runtimeTask = RuntimeTaskBuilder()
    let backgroundRuntimeTask = RuntimeTaskBuilder()

[<AutoOpen>]
module Extensions =
    open InternalHelpers
    type RuntimeTaskBuilder with
        member inline _.Source(awaitable: Awaitable<_, _, _>) =
            let awaiter = Awaitable.getAwaiter awaitable
            if not (Awaiter.isCompleted awaiter) then
                AsyncHelpers.AwaitAwaiter awaiter       
            Awaiter.getResult awaiter
