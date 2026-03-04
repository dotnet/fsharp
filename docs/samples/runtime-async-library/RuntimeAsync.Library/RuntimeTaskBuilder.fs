namespace RuntimeAsync.Library

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

open Microsoft.FSharp.Control

#nowarn "57"

/// Computation expression builder for runtime-async methods.
/// Annotated with [<RuntimeAsync>] so the compiler:
///   (1) Implicitly applies NoDynamicInvocation to all public inline members
///   (2) Gates optimizer anti-inlining behind this attribute
///
/// Design (Architecture B): Delay creates a closure that wraps the CE body.
/// [<InlineIfLambda>] on f inlines the CE body into the Delay closure, so there is only ONE
/// closure containing all AsyncHelpers.Await calls. The compiler automatically injects a sentinel
/// to ensure the Delay closure is always 'cil managed async', and automatically handles the
/// 'T → Task<'T> bridging for [<RuntimeAsync>] builders — no cast is needed.
/// Run is non-inline with [<MethodImplAttribute(0x2000)>] and takes unit -> Task<'T>.
/// This enables true inline-nested runtimeTask { ... } CEs.
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

    /// Delay creates a closure that wraps the CE body.
    /// [<InlineIfLambda>] on f inlines the CE body into the Delay closure, so there is only ONE
    /// closure containing all AsyncHelpers.Await calls. The compiler automatically injects a
    /// sentinel to ensure the Delay closure is always 'cil managed async', even when the CE body
    /// has no let!/do! bindings. The compiler also handles the 'T → Task<'T> bridging automatically
    /// for [<RuntimeAsync>] builders, so no cast is needed.
    member inline _.Delay([<InlineIfLambda>] f: unit -> 'T) : unit -> Task<'T> =
        fun () -> f()

    member inline _.Zero() : unit = ()
    /// Combine sequences two CE expressions. The second expression is wrapped in Delay,
    /// so f returns unit -> Task<'T>. f must NOT be [<InlineIfLambda>]: if it were, the Delay
    /// closure body would be inlined and f() would push 'T (not Task<'T>) at IL level, making
    /// AsyncHelpers.Await(f()) fail (Await expects Task<'T> but gets 'T). By not inlining f,
    /// f() calls the Delay closure Invoke → Task<'T> via cil managed async, so Await works.
    member inline _.Combine((): unit, f: unit -> Task<'T>) : 'T =
        AsyncHelpers.Await(f())

    /// While loops. The body is wrapped in Delay, so body returns unit -> Task<unit>.
    /// Each iteration awaits body() so the async body completes before the next iteration.
    member inline _.While([<InlineIfLambda>] guard: unit -> bool, body: unit -> Task<unit>) : unit =
        while guard() do
            AsyncHelpers.Await(body())

    /// TryWith handles try/with in CEs. The body is wrapped in Delay (unit -> Task<'T>).
    /// We await body() inside the try block so that exceptions from the async body are caught
    /// by the with clause. Returns 'T (not Task<'T>) — the outer Delay closure wraps in Task<'T>.
    member inline _.TryWith(body: unit -> Task<'T>, [<InlineIfLambda>] handler: exn -> 'T) : 'T =
        try
            // Await body() inside try so async exceptions are caught by the with clause.
            AsyncHelpers.Await(body())
        with e ->
            handler e

    /// TryFinally handles try/finally in CEs. The body is wrapped in Delay (unit -> Task<'T>).
    /// We await body() inside the try block so the finally clause runs after async completion.
    /// Returns 'T (not Task<'T>) — the outer Delay closure wraps in Task<'T>.
    member inline _.TryFinally(body: unit -> Task<'T>, [<InlineIfLambda>] comp: unit -> unit) : 'T =
        try
            // Await body() inside try so the finally clause runs after async completion.
            AsyncHelpers.Await(body())
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

    /// Run is non-inline with [<MethodImplAttribute(0x2000)>] — the compiler emits it as
    /// 'cil managed async'. Run takes the Delay closure (unit -> Task<'T>) and awaits it.
    /// The Delay closure is 'cil managed async' and returns Task<'T> at runtime.
    /// Run awaits f() to get Task<'T>, then AsyncHelpers.Await unwraps it to 'T, then Run
    /// wraps 'T back to Task<'T>. This enables true inline-nested runtimeTask { ... } CEs.
    [<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
    member _.Run(f: unit -> Task<'T>) : Task<'T> =
        // f() returns Task<'T> (the Delay closure is 'cil managed async').
        // AsyncHelpers.Await unwraps Task<'T> to 'T. Run wraps 'T back to Task<'T>.
        AsyncHelpers.Await(f())

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
