// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace Microsoft.FSharp.Control.RuntimeAsync

#if NET && !BUILDING_WITH_LKG
open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Control

type AwaitableBuilderBase() =
    [<NoDynamicInvocation; NoEagerConstraintApplication>]
    member inline _.Bind(source: ^Awaitable, [<InlineIfLambda>] continuation: 'T -> 'U) : 'U =
        let awaiter = (^Awaitable: (member GetAwaiter: unit -> ^Awaiter) source)

        if not (^Awaiter: (member get_IsCompleted: unit -> bool) awaiter) then
            AsyncHelpers.UnsafeAwaitAwaiter awaiter

        continuation (^Awaiter: (member GetResult: unit -> 'T) awaiter)

    member inline _.Source(source: 'T) =
        source

    member inline _.Source(source: Task<'T>) =
        ValueTask<'T>(source)

    member inline _.Source(source: Task) =
        ValueTask(source)

    member inline _.Source(source: Async<'T>) =
        ValueTask<'T>(Async.StartImmediateAsTask source)

    /// <summary>Gets both source awaiters before waiting for either result.</summary>
    [<NoDynamicInvocation; NoEagerConstraintApplication>]
    member inline _.MergeSources(left: ^Left, right: ^Right) : ValueTask<struct ('T * 'U)> =
        __runtimeAsyncReturnValueTask (
            let left = (^Left: (member GetAwaiter: unit -> ^LeftAwaiter) left)
            let right = (^Right: (member GetAwaiter: unit -> ^RightAwaiter) right)

            if not (^LeftAwaiter: (member get_IsCompleted: unit -> bool) left) then
                AsyncHelpers.UnsafeAwaitAwaiter left

            let first = (^LeftAwaiter: (member GetResult: unit -> 'T) left)

            if not (^RightAwaiter: (member get_IsCompleted: unit -> bool) right) then
                AsyncHelpers.UnsafeAwaitAwaiter right

            struct (first, (^RightAwaiter: (member GetResult: unit -> 'U) right))
        )

/// <summary>Builds tasks whose suspension and resumption are managed by .NET.</summary>
/// <remarks>Requires .NET 11 or later.</remarks>
type TaskBuilder() =
    inherit AwaitableBuilderBase()

    member inline _.Delay([<InlineIfLambda>] body: unit -> 'T) =
        body

    [<NoDynamicInvocation>]
    member inline _.Run([<InlineIfLambda>] body: unit -> 'T) =
        __runtimeAsyncReturn (body ())

    member inline _.Zero() =
        ()

    member inline _.Return(value: 'T) =
        value

    [<NoDynamicInvocation>]
    member inline this.ReturnFrom(source: ^Awaitable) : 'T =
        this.Bind< ^Awaitable, ^Awaiter, 'T, 'T>(source, id)

    member inline _.Combine(first: unit, [<InlineIfLambda>] rest: unit -> 'T) =
        ignore first
        rest ()

    member inline _.While([<InlineIfLambda>] guard: unit -> bool, [<InlineIfLambda>] body: unit -> unit) =
        while guard () do
            body ()

    member inline _.For(source: seq<'T>, [<InlineIfLambda>] body: 'T -> unit) =
        for value in source do
            body value

    member inline _.TryWith([<InlineIfLambda>] body: unit -> 'T, [<InlineIfLambda>] handler: exn -> 'T) =
        try
            body ()
        with error ->
            handler error

    member inline _.TryFinally([<InlineIfLambda>] body: unit -> 'T, [<InlineIfLambda>] compensation: unit -> unit) =
        try
            body ()
        finally
            compensation ()

    [<NoDynamicInvocation>]
    member inline _.Using(resource: 'R :> IAsyncDisposable | null, [<InlineIfLambda>] body: 'R -> 'T) =
        try
            body resource
        finally
            if not (isNull (box resource)) then
                AsyncHelpers.Await(resource.DisposeAsync())

    [<NoDynamicInvocation>]
    member inline this.For(source: IAsyncEnumerable<'T>, [<InlineIfLambda>] body: 'T -> unit) =
        this.Using(
            source.GetAsyncEnumerator(),
            fun iterator ->
                while AsyncHelpers.Await(iterator.MoveNextAsync()) do
                    body iterator.Current
        )

/// <summary>Builds cold, pull-driven asynchronous sequences using the .NET runtime.</summary>
/// <remarks>Requires .NET 11 or later.</remarks>
type TaskSeqBuilder() =
    inherit AwaitableBuilderBase()

    /// <summary>Starts an asynchronous computation with the enumeration cancellation token.</summary>
    [<NoDynamicInvocation>]
    member inline _.Source(source: Async<'T>) =
        ValueTask<'T>(Async.StartImmediateAsTask(source, __runtimeAsyncSequenceCancellationToken ()))

    member inline _.Zero() : seq<'T> =
        Seq.empty

    member inline _.Return() : seq<'T> =
        Seq.empty

    member inline _.Yield(value: 'T) =
        Seq.singleton value

    member inline _.Delay([<InlineIfLambda>] body: unit -> seq<'T>) =
        body

    [<NoDynamicInvocation>]
    member inline _.Run([<InlineIfLambda>] recipe: unit -> seq<'T>) =
        __runtimeAsyncSequence recipe

    member inline _.Combine(first: seq<'T>, [<InlineIfLambda>] rest: unit -> seq<'T>) =
        Seq.append first (Seq.delay rest)

    member inline _.For(source: seq<'T>, [<InlineIfLambda>] body: 'T -> seq<'U>) =
        Seq.collect body source

    member inline _.While([<InlineIfLambda>] guard: unit -> bool, [<InlineIfLambda>] body: unit -> seq<'T>) =
        RuntimeHelpers.EnumerateWhile guard (Seq.delay body)

    member inline _.TryWith([<InlineIfLambda>] body: unit -> seq<'T>, [<InlineIfLambda>] handler: exn -> seq<'T>) =
        RuntimeHelpers.EnumerateTryWith (Seq.delay body) (fun _ -> 1) handler

    member inline _.TryFinally
        ([<InlineIfLambda>] body: unit -> seq<'T>, [<InlineIfLambda>] compensation: unit -> unit)
        =
        RuntimeHelpers.EnumerateThenFinally (Seq.delay body) compensation

    [<NoDynamicInvocation>]
    member inline _.Using(resource: 'R :> IAsyncDisposable | null, [<InlineIfLambda>] body: 'R -> seq<'T>) =
        RuntimeHelpers.EnumerateThenFinally (Seq.delay (fun () -> body resource)) (fun () ->
            if not (isNull (box resource)) then
                AsyncHelpers.Await(resource.DisposeAsync()))

    [<NoDynamicInvocation>]
    member inline this.For(source: IAsyncEnumerable<'T>, [<InlineIfLambda>] body: 'T -> seq<'U>) =
        Seq.delay (fun () ->
            let iterator = source.GetAsyncEnumerator(__runtimeAsyncSequenceCancellationToken ())

            this.Using(
                iterator,
                fun iterator ->
                    this.While(
                        (fun () -> AsyncHelpers.Await(iterator.MoveNextAsync())),
                        (fun () -> body iterator.Current)
                    )
            ))

    member inline this.YieldFrom(source: seq<'T>) =
        this.For(source, fun value -> this.Yield value)

    [<NoDynamicInvocation>]
    member inline this.YieldFrom(source: IAsyncEnumerable<'T>) =
        this.For(source, fun value -> this.Yield value)

/// <summary>Runtime-managed builders selected by --features:runtime-async=on in preview language mode.
/// The builders can also be selected by qualified name.</summary>
module TaskExpressions =
    let task = TaskBuilder()
    let taskSeq = TaskSeqBuilder()
#endif
