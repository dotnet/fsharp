// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

namespace Microsoft.FSharp.Control.RuntimeAsync

#if NET
open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading.Tasks
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.CompilerServices
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Control

type AwaitableBuilderBase =
    new: unit -> AwaitableBuilderBase

    [<NoDynamicInvocation; NoEagerConstraintApplication>]
    member inline Bind:
        source: ^Awaitable * [<InlineIfLambda>] continuation: ('T -> 'U) -> 'U
            when ^Awaitable: (member GetAwaiter: unit -> ^Awaiter)
            and ^Awaiter :> ICriticalNotifyCompletion
            and ^Awaiter: (member get_IsCompleted: unit -> bool)
            and ^Awaiter: (member GetResult: unit -> 'T)

    member inline Source: source: 'T -> 'T
    member inline Source: source: Task<'T> -> ValueTask<'T>
    member inline Source: source: Task -> ValueTask
    member inline Source: source: Async<'T> -> ValueTask<'T>

    /// <summary>Gets both source awaiters before waiting for either result.</summary>
    [<NoDynamicInvocation; NoEagerConstraintApplication>]
    member inline MergeSources:
        left: ^Left * right: ^Right -> ValueTask<struct ('T * 'U)>
            when ^Left: (member GetAwaiter: unit -> ^LeftAwaiter)
            and ^LeftAwaiter :> ICriticalNotifyCompletion
            and ^LeftAwaiter: (member get_IsCompleted: unit -> bool)
            and ^LeftAwaiter: (member GetResult: unit -> 'T)
            and ^Right: (member GetAwaiter: unit -> ^RightAwaiter)
            and ^RightAwaiter :> ICriticalNotifyCompletion
            and ^RightAwaiter: (member get_IsCompleted: unit -> bool)
            and ^RightAwaiter: (member GetResult: unit -> 'U)

/// <summary>Builds tasks whose suspension and resumption are managed by .NET.</summary>
/// <remarks>Requires .NET 11 or later.</remarks>
type TaskBuilder =
    inherit AwaitableBuilderBase
    new: unit -> TaskBuilder
    member inline Delay: [<InlineIfLambda>] body: (unit -> 'T) -> (unit -> 'T)

    [<NoDynamicInvocation>]
    member inline Run: [<InlineIfLambda>] body: (unit -> 'T) -> Task<'T>

    member inline Zero: unit -> unit
    member inline Return: value: 'T -> 'T

    [<NoDynamicInvocation>]
    member inline ReturnFrom:
        source: ^Awaitable -> 'T
            when ^Awaitable: (member GetAwaiter: unit -> ^Awaiter)
            and ^Awaiter :> ICriticalNotifyCompletion
            and ^Awaiter: (member get_IsCompleted: unit -> bool)
            and ^Awaiter: (member GetResult: unit -> 'T)

    member inline Combine: first: unit * [<InlineIfLambda>] rest: (unit -> 'T) -> 'T
    member inline While: [<InlineIfLambda>] guard: (unit -> bool) * [<InlineIfLambda>] body: (unit -> unit) -> unit
    member inline For: source: seq<'T> * [<InlineIfLambda>] body: ('T -> unit) -> unit

    [<NoDynamicInvocation>]
    member inline For: source: IAsyncEnumerable<'T> * [<InlineIfLambda>] body: ('T -> unit) -> unit

    member inline TryWith: [<InlineIfLambda>] body: (unit -> 'T) * [<InlineIfLambda>] handler: (exn -> 'T) -> 'T

    member inline TryFinally:
        [<InlineIfLambda>] body: (unit -> 'T) * [<InlineIfLambda>] compensation: (unit -> unit) -> 'T

    [<NoDynamicInvocation>]
    member inline Using: resource: 'R * [<InlineIfLambda>] body: ('R -> 'T) -> 'T when 'R :> IAsyncDisposable | null

/// <summary>Builds cold, pull-driven asynchronous sequences using the .NET runtime.</summary>
/// <remarks>Requires .NET 11 or later.</remarks>
type TaskSeqBuilder =
    inherit AwaitableBuilderBase
    new: unit -> TaskSeqBuilder

    /// <summary>Starts an asynchronous computation with the enumeration cancellation token.</summary>
    [<NoDynamicInvocation>]
    member inline Source: source: Async<'T> -> ValueTask<'T>

    member inline Zero: unit -> seq<'T>
    member inline Return: unit -> seq<'T>
    member inline Yield: value: 'T -> seq<'T>
    member inline YieldFrom: source: seq<'T> -> seq<'T>

    [<NoDynamicInvocation>]
    member inline YieldFrom: source: IAsyncEnumerable<'T> -> seq<'T>

    member inline Delay: [<InlineIfLambda>] body: (unit -> seq<'T>) -> (unit -> seq<'T>)

    [<NoDynamicInvocation>]
    member inline Run: [<InlineIfLambda>] recipe: (unit -> seq<'T>) -> IAsyncEnumerable<'T>

    member inline Combine: first: seq<'T> * [<InlineIfLambda>] rest: (unit -> seq<'T>) -> seq<'T>

    member inline While:
        [<InlineIfLambda>] guard: (unit -> bool) * [<InlineIfLambda>] body: (unit -> seq<'T>) -> seq<'T>

    member inline For: source: seq<'T> * [<InlineIfLambda>] body: ('T -> seq<'U>) -> seq<'U>

    [<NoDynamicInvocation>]
    member inline For: source: IAsyncEnumerable<'T> * [<InlineIfLambda>] body: ('T -> seq<'U>) -> seq<'U>

    member inline TryWith:
        [<InlineIfLambda>] body: (unit -> seq<'T>) * [<InlineIfLambda>] handler: (exn -> seq<'T>) -> seq<'T>

    member inline TryFinally:
        [<InlineIfLambda>] body: (unit -> seq<'T>) * [<InlineIfLambda>] compensation: (unit -> unit) -> seq<'T>

    [<NoDynamicInvocation>]
    member inline Using:
        resource: 'R * [<InlineIfLambda>] body: ('R -> seq<'T>) -> seq<'T> when 'R :> IAsyncDisposable | null

/// <summary>Runtime-managed builders selected by --features:runtime-async=on in preview language mode.
/// The builders can also be selected by qualified name.</summary>
module TaskExpressions =
    val task: TaskBuilder
    val taskSeq: TaskSeqBuilder
#endif
