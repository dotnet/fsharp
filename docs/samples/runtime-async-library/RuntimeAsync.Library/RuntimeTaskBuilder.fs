namespace RuntimeAsync.Library

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

open Microsoft.FSharp.Control

#nowarn "57"
#nowarn "42"

/// Internal helpers for RuntimeTaskBuilder.
/// Not intended for direct use by consumers.
module internal RuntimeTaskBuilderHelpers =
    /// Reinterpret cast with no runtime overhead.
    /// Used by Run to cast the raw return value of f() to Task<T> so the runtime
    /// wraps it correctly for 'cil managed async' consumer methods.
    let inline cast<'a, 'b> (a: 'a) : 'b = (# "" a : 'b #)

/// Computation expression builder for runtime-async methods.
/// Annotated with [<RuntimeAsync>] so the compiler:
///   (1) Implicitly applies NoDynamicInvocation to all public inline members
///   (2) Gates optimizer anti-inlining behind this attribute
///
/// Design: Run is fully inline — its body (including the Await sentinel and cast) gets inlined
/// into each consumer function. The consumer's body then contains AsyncHelpers.Await calls,
/// so the compiler marks the consumer as 'cil managed async'. No [<MethodImplAttribute(0x2000)>]
/// is needed on Run itself — the compiler detects AsyncHelpers.Await in the inlined body.
[<RuntimeAsync; Sealed>]
type RuntimeTaskBuilder() =
    member inline _.Return(x: 'T) : 'T = x

    member inline _.Bind(t: Task<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =
        f(AsyncHelpers.Await t)

    member inline _.Bind(t: Task, [<InlineIfLambda>] f: unit -> 'U) : 'U =
        AsyncHelpers.Await t
        f()

    member inline _.Bind(t: ValueTask<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =
        f(AsyncHelpers.Await t)

    member inline _.Bind(t: ValueTask, [<InlineIfLambda>] f: unit -> 'U) : 'U =
        AsyncHelpers.Await t
        f()

    // ConfiguredTaskAwaitable — allows task.ConfigureAwait(false) in runtimeTask
    member inline _.Bind(cta: ConfiguredTaskAwaitable, [<InlineIfLambda>] f: unit -> 'U) : 'U =
        AsyncHelpers.Await cta
        f()

    member inline _.Bind(cta: ConfiguredTaskAwaitable<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =
        f(AsyncHelpers.Await cta)

    // ConfiguredValueTaskAwaitable — allows valueTask.ConfigureAwait(false) in runtimeTask
    member inline _.Bind(cvta: ConfiguredValueTaskAwaitable, [<InlineIfLambda>] f: unit -> 'U) : 'U =
        AsyncHelpers.Await cvta
        f()

    member inline _.Bind(cvta: ConfiguredValueTaskAwaitable<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =
        f(AsyncHelpers.Await cvta)

    member inline _.Delay(f: unit -> 'T) : unit -> 'T = f
    member inline _.Zero() : unit = ()
    member inline _.Combine((): unit, [<InlineIfLambda>] f: unit -> 'T) : 'T = f()

    member inline _.While([<InlineIfLambda>] guard: unit -> bool, [<InlineIfLambda>] body: unit -> unit) : unit =
        while guard() do
            body()

    member inline _.TryWith([<InlineIfLambda>] body: unit -> 'T, [<InlineIfLambda>] handler: exn -> 'T) : 'T =
        try
            body()
        with e ->
            handler e

    member inline _.TryFinally([<InlineIfLambda>] body: unit -> 'T, [<InlineIfLambda>] comp: unit -> unit) : 'T =
        try
            body()
        finally
            comp()

    /// TryFinally with async compensation — awaits a ValueTask in the finally block.
    /// Used by Using(IAsyncDisposable) to await DisposeAsync().
    member inline _.TryFinallyAsync
        ([<InlineIfLambda>] body: unit -> 'T, [<InlineIfLambda>] compensation: unit -> ValueTask)
        : 'T =
        try
            body()
        finally
            AsyncHelpers.Await(compensation())

    /// IAsyncDisposable — intrinsic member so it is preferred over the IDisposable extension
    /// when a type implements both interfaces.
    member inline this.Using
        (resource: 'T when 'T :> IAsyncDisposable, [<InlineIfLambda>] body: 'T -> 'U)
        : 'U =
        this.TryFinallyAsync(
            (fun () -> body resource),
            (fun () ->
                if not (isNull (box resource)) then
                    resource.DisposeAsync()
                else
                    ValueTask.CompletedTask
            )
        )

    /// IAsyncEnumerable — intrinsic member so it is preferred over the seq extension.
    /// Awaits MoveNextAsync() and DisposeAsync() on the enumerator.
    member inline _.For(sequence: IAsyncEnumerable<'T>, [<InlineIfLambda>] body: 'T -> unit) : unit =
        let enumerator = sequence.GetAsyncEnumerator(CancellationToken.None)

        try
            while AsyncHelpers.Await(enumerator.MoveNextAsync()) do
                body(enumerator.Current)
        finally
            AsyncHelpers.Await(enumerator.DisposeAsync())

    /// Run is fully inline — its body (including the Await sentinel and cast) gets inlined
    /// into each consumer function. The consumer's body then contains AsyncHelpers.Await calls,
    /// so the compiler marks the consumer as 'cil managed async'. No [<MethodImplAttribute(0x2000)>]
    /// is needed on Run itself.
    member inline _.Run([<InlineIfLambda>] f: unit -> 'T) : Task<'T> =
        // Sentinel: ensures the consumer method always gets 'cil managed async' even when
        // the CE body has no let!/do! bindings (e.g. runtimeTask { return 42 }).
        // This is a no-op at runtime — CompletedTask is already complete.
        AsyncHelpers.Await(ValueTask.CompletedTask)
        RuntimeTaskBuilderHelpers.cast (f())

/// IDisposable Using and seq For as type extensions.
/// These have lower priority than the intrinsic IAsyncDisposable/IAsyncEnumerable members above,
/// so when a type implements both IDisposable and IAsyncDisposable, the async variant wins.
[<AutoOpen>]
module RuntimeTaskBuilderExtensions =
    type RuntimeTaskBuilder with

        member inline _.Using
            (resource: 'T when 'T :> IDisposable, [<InlineIfLambda>] body: 'T -> 'U)
            : 'U =
            try
                body resource
            finally
                (resource :> IDisposable).Dispose()

        [<NoDynamicInvocation>]
        member inline _.For(s: seq<'T>, [<InlineIfLambda>] body: 'T -> unit) : unit =
            for x in s do
                body(x)

        /// Generic Bind for any awaitable type that has a GetAwaiter() method returning
        /// an awaiter implementing ICriticalNotifyCompletion.
        /// This handles types like YieldAwaitable, custom awaitables, etc.
        /// Lower priority than the intrinsic Bind overloads for Task/ValueTask/ConfiguredTask.
        [<NoDynamicInvocation>]
        member inline _.Bind(awaitable: ^Awaitable, [<InlineIfLambda>] f: ^TResult -> 'U) : 'U
            when ^Awaitable : (member GetAwaiter: unit -> ^Awaiter)
            and ^Awaiter :> ICriticalNotifyCompletion
            and ^Awaiter : (member get_IsCompleted: unit -> bool)
            and ^Awaiter : (member GetResult: unit -> ^TResult) =
            let awaiter = (^Awaitable : (member GetAwaiter: unit -> ^Awaiter) awaitable)
            if not ((^Awaiter : (member get_IsCompleted: unit -> bool) awaiter)) then
                AsyncHelpers.UnsafeAwaitAwaiter(awaiter)
            f ((^Awaiter : (member GetResult: unit -> ^TResult) awaiter))

[<AutoOpen>]
module RuntimeTaskBuilderModule =
    let runtimeTask = RuntimeTaskBuilder()
