module RuntimeTaskBuilder

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Control
open Microsoft.FSharp.Core.CompilerServices

type RuntimeTask<'T> = unit -> 'T

let inline bindAwaiter
    ([<InlineIfLambda>] getAwaiter: unit -> 'Awaiter)
    ([<InlineIfLambda>] getResult: 'Awaiter -> 'T)
    ([<InlineIfLambda>] continuation: 'T -> 'U)
    =
    let awaiter = getAwaiter()
    AsyncHelpers.AwaitAwaiter awaiter
    let result = getResult awaiter
    continuation result

type RuntimeTaskBuilder() =
    member inline _.Delay([<InlineIfLambda>] generator: unit -> 'T) : unit -> 'T = generator
    member inline _.Run([<InlineIfLambda>] code: unit -> 'T) : Task<'T> =
        StateMachineHelpers.__runtimeAsyncReturn (code())
    member inline _.Zero() = ()
    member inline _.Return(value: 'T) = value
    member inline _.ReturnFrom(task: Task<'T>) = AsyncHelpers.Await task
    member inline _.ReturnFrom(task: Task) = AsyncHelpers.Await task
    member inline _.ReturnFrom(task: ValueTask<'T>) = AsyncHelpers.Await task
    member inline _.ReturnFrom(task: ValueTask) = AsyncHelpers.Await task
    member inline _.ReturnFrom(computation: Async<'T>) = AsyncHelpers.Await(Async.StartImmediateAsTask computation)
    member inline _.Bind(task: Task, [<InlineIfLambda>] continuation: unit -> 'U) =
        AsyncHelpers.Await task
        continuation()
    member inline _.Bind(task: Task<'T>, [<InlineIfLambda>] continuation: 'T -> 'U) =
        continuation (AsyncHelpers.Await task)
    member inline _.Bind(code: struct ('T1 * 'T2), [<InlineIfLambda>] continuation: struct ('T1 * 'T2) -> 'U) =
        continuation code
    member inline _.Bind(computation: RuntimeTask<'T>, [<InlineIfLambda>] continuation: 'T -> 'U) =
        continuation (computation ())
    member inline _.Bind(task: ValueTask, [<InlineIfLambda>] continuation: unit -> 'U) =
        AsyncHelpers.Await task
        continuation()
    member inline _.Bind(task: ValueTask<'T>, [<InlineIfLambda>] continuation: 'T -> 'U) =
        continuation (AsyncHelpers.Await task)
    member inline _.Bind(computation: Async<'T>, [<InlineIfLambda>] continuation: 'T -> 'U) =
        continuation (AsyncHelpers.Await(Async.StartImmediateAsTask computation))
    member inline _.Combine(first, [<InlineIfLambda>] second) =
        first()
        second()
    member inline _.Combine(first: unit, [<InlineIfLambda>] second: unit -> 'T) = second()
    member inline _.TryWith([<InlineIfLambda>] body: unit -> 'T, [<InlineIfLambda>] handler: exn -> 'T) =
        try body() with error -> handler error
    member inline _.TryFinally([<InlineIfLambda>] body: unit -> 'T, compensation: unit -> unit) =
        try body() finally compensation()
    member inline _.Using(resource: 'Resource, [<InlineIfLambda>] body: 'Resource -> 'T) =
        // Awaiting in a finally region is forbidden by the runtime-async contract.
        // Hoist the DisposeAsync suspension out of the region: capture any exception
        // from the body in a catch-all, run disposal (possibly suspending) outside
        // the handler, then restore the pending exception. Mirrors the Roslyn
        // runtime-async lowering for `await` in `finally`.
        let mutable pendingException: exn = null

        let result =
            try
                Choice1Of2(body resource)
            with error ->
                pendingException <- error
                Choice2Of2()

        match box resource with
        | :? IAsyncDisposable as disposable -> AsyncHelpers.Await(disposable.DisposeAsync())
        | :? IDisposable as disposable -> disposable.Dispose()
        | _ -> ()

        match pendingException with
        | null -> ()
        | error -> raise error

        match result with
        | Choice1Of2 value -> value
        | Choice2Of2() -> Unchecked.defaultof<'T>
    member inline _.While(guard: unit -> bool, [<InlineIfLambda>] body: unit -> unit) =
        while guard() do body()
    member inline _.For(sequence: seq<'T>, [<InlineIfLambda>] body: 'T -> unit) =
        for item in sequence do body item
    member inline _.MergeSources(left: Task<'T1>, right: Task<'T2>) =
        struct (AsyncHelpers.Await left, AsyncHelpers.Await right)
    member inline _.MergeSources(left: ValueTask<'T1>, right: ValueTask<'T2>) =
        struct (AsyncHelpers.Await left, AsyncHelpers.Await right)
    member inline _.MergeSources(left: Task<'T1>, right: ValueTask<'T2>) =
        struct (AsyncHelpers.Await left, AsyncHelpers.Await right)
    member inline _.MergeSources(left: ValueTask<'T1>, right: Task<'T2>) =
        struct (AsyncHelpers.Await left, AsyncHelpers.Await right)
    member inline _.MergeSources(left: Task<'T1>, right: Async<'T2>) =
        struct (AsyncHelpers.Await left, AsyncHelpers.Await(Async.StartImmediateAsTask right))
    member inline _.MergeSources(left: Async<'T1>, right: Task<'T2>) =
        struct (AsyncHelpers.Await(Async.StartImmediateAsTask left), AsyncHelpers.Await right)
    member inline _.MergeSources(left: Async<'T1>, right: Async<'T2>) =
        struct (AsyncHelpers.Await(Async.StartImmediateAsTask left), AsyncHelpers.Await(Async.StartImmediateAsTask right))
    member inline _.MergeSources(left: Async<'T1>, right: ValueTask<'T2>) =
        struct (AsyncHelpers.Await(Async.StartImmediateAsTask left), AsyncHelpers.Await right)
    member inline _.MergeSources(left: ValueTask<'T1>, right: Async<'T2>) =
        struct (AsyncHelpers.Await left, AsyncHelpers.Await(Async.StartImmediateAsTask right))
    member inline _.MergeSources(left: YieldAwaitable, right: Task<'T2>) =
        AsyncHelpers.AwaitAwaiter(left.GetAwaiter())
        struct ((), AsyncHelpers.Await right)
    member inline _.MergeSources(left: Task<'T1>, right: YieldAwaitable) =
        let leftResult = AsyncHelpers.Await left
        AsyncHelpers.AwaitAwaiter(right.GetAwaiter())
        struct (leftResult, ())
    member inline _.MergeSources(left: YieldAwaitable, right: ValueTask<'T2>) =
        AsyncHelpers.AwaitAwaiter(left.GetAwaiter())
        struct ((), AsyncHelpers.Await right)
    member inline _.MergeSources(left: ValueTask<'T1>, right: YieldAwaitable) =
        let leftResult = AsyncHelpers.Await left
        AsyncHelpers.AwaitAwaiter(right.GetAwaiter())
        struct (leftResult, ())
    member inline _.MergeSources(left: YieldAwaitable, right: Async<'T2>) =
        AsyncHelpers.AwaitAwaiter(left.GetAwaiter())
        struct ((), AsyncHelpers.Await(Async.StartImmediateAsTask right))
    member inline _.MergeSources(left: Async<'T1>, right: YieldAwaitable) =
        let leftResult = AsyncHelpers.Await(Async.StartImmediateAsTask left)
        AsyncHelpers.AwaitAwaiter(right.GetAwaiter())
        struct (leftResult, ())
    member inline _.MergeSources(left: YieldAwaitable, right: struct ('T2 * 'T3)) =
        AsyncHelpers.AwaitAwaiter(left.GetAwaiter())
        struct ((), right)
    member inline _.MergeSources(left: struct ('T1 * 'T2), right: YieldAwaitable) =
        AsyncHelpers.AwaitAwaiter(right.GetAwaiter())
        struct (left, ())
    member inline _.MergeSources(left: Task<'T1>, right: struct ('T2 * 'T3)) =
        struct (AsyncHelpers.Await left, right)
    member inline _.MergeSources(left: ValueTask<'T1>, right: struct ('T2 * 'T3)) =
        struct (AsyncHelpers.Await left, right)
    member inline _.MergeSources(left: Async<'T1>, right: struct ('T2 * 'T3)) =
        struct (AsyncHelpers.Await(Async.StartImmediateAsTask left), right)
    member inline _.MergeSources(left: struct ('T1 * 'T2), right: Task<'T3>) =
        struct (left, AsyncHelpers.Await right)
    member inline _.MergeSources(left: struct ('T1 * 'T2), right: ValueTask<'T3>) =
        struct (left, AsyncHelpers.Await right)
    member inline _.MergeSources(left: struct ('T1 * 'T2), right: Async<'T3>) =
        struct (left, AsyncHelpers.Await(Async.StartImmediateAsTask right))

module RuntimeTaskAwaitableExtensions =
    type RuntimeTaskBuilder with
        // SRTP fallbacks mirroring the task builder's task-like Bind/ReturnFrom/MergeSources,
        // so custom awaitables compose without dedicated overloads.
        [<NoEagerConstraintApplication>]
        member inline _.ReturnFrom< ^TaskLike, ^Awaiter, 'T
            when ^TaskLike: (member GetAwaiter: unit -> ^Awaiter)
            and ^Awaiter :> ICriticalNotifyCompletion
            and ^Awaiter: (member get_IsCompleted: unit -> bool)
            and ^Awaiter: (member GetResult: unit -> 'T)>
            (task: ^TaskLike)
            : 'T =
            bindAwaiter
                (fun () -> (^TaskLike: (member GetAwaiter: unit -> ^Awaiter) task))
                (fun awaiter -> (^Awaiter: (member GetResult: unit -> 'T) awaiter))
                id

        [<NoEagerConstraintApplication>]
        member inline _.MergeSources< ^TaskLike1, ^TaskLike2, ^Awaiter1, ^Awaiter2, 'T1, 'T2
            when ^TaskLike1: (member GetAwaiter: unit -> ^Awaiter1)
            and ^TaskLike2: (member GetAwaiter: unit -> ^Awaiter2)
            and ^Awaiter1 :> ICriticalNotifyCompletion
            and ^Awaiter2 :> ICriticalNotifyCompletion
            and ^Awaiter1: (member get_IsCompleted: unit -> bool)
            and ^Awaiter1: (member GetResult: unit -> 'T1)
            and ^Awaiter2: (member get_IsCompleted: unit -> bool)
            and ^Awaiter2: (member GetResult: unit -> 'T2)>
            (task1: ^TaskLike1, task2: ^TaskLike2)
            : struct ('T1 * 'T2) =
            let await1 () =
                bindAwaiter
                    (fun () -> (^TaskLike1: (member GetAwaiter: unit -> ^Awaiter1) task1))
                    (fun awaiter -> (^Awaiter1: (member GetResult: unit -> 'T1) awaiter))
                    id

            let await2 () =
                bindAwaiter
                    (fun () -> (^TaskLike2: (member GetAwaiter: unit -> ^Awaiter2) task2))
                    (fun awaiter -> (^Awaiter2: (member GetResult: unit -> 'T2) awaiter))
                    id
            // Sequential awaits, matching the task builder's MergeSources; concurrency
            // comes from the sources being already-started hot tasks.
            struct (await1 (), await2 ())

        [<NoEagerConstraintApplication>]
        member inline _.Bind< ^TaskLike, ^Awaiter, 'T, 'U
            when ^TaskLike: (member GetAwaiter: unit -> ^Awaiter)
            and ^Awaiter :> ICriticalNotifyCompletion
            and ^Awaiter: (member get_IsCompleted: unit -> bool)
            and ^Awaiter: (member GetResult: unit -> 'T)>
            (task: ^TaskLike, [<InlineIfLambda>] continuation: 'T -> 'U)
            : 'U =
            bindAwaiter
                (fun () -> (^TaskLike: (member GetAwaiter: unit -> ^Awaiter) task))
                (fun awaiter -> (^Awaiter: (member GetResult: unit -> 'T) awaiter))
                continuation

open RuntimeTaskAwaitableExtensions

[<AutoOpen>]
module RuntimeTask =
    let runtimeTask = RuntimeTaskBuilder()
