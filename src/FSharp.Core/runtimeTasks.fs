// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// RuntimeTaskBuilder - a computation expression builder for runtime-async methods.
// Only available on .NET 10.0+ where System.Runtime.CompilerServices.AsyncHelpers is available.
namespace Microsoft.FSharp.Control

#if NET10_0_OR_GREATER

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks

module internal RuntimeTaskBuilderUnsafe =
    let inline cast<'a, 'b> (a: 'a) : 'b = (# "" a : 'b #)

/// Computation expression builder for runtime-async methods.
/// Methods using this builder will have the async IL flag (0x2000) emitted.
/// All members are inline to produce flat method bodies (no state machine).
[<Sealed>]
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
    member inline _.Delay([<InlineIfLambda>] f: unit -> 'T) : 'T = f()
    member inline _.Zero() : unit = ()
    member inline _.Combine((), [<InlineIfLambda>] f: unit -> 'T) : 'T = f()
    member inline _.While([<InlineIfLambda>] guard: unit -> bool, [<InlineIfLambda>] body: unit -> unit) : unit =
        while guard() do body()
    member inline _.For(s: seq<'T>, [<InlineIfLambda>] body: 'T -> unit) : unit =
        for x in s do body(x)
    member inline _.TryWith([<InlineIfLambda>] body: unit -> 'T, [<InlineIfLambda>] handler: exn -> 'T) : 'T =
        try body() with e -> handler e
    member inline _.TryFinally([<InlineIfLambda>] body: unit -> 'T, [<InlineIfLambda>] comp: unit -> unit) : 'T =
        try body() finally comp()
    member inline _.Using(resource: 'T when 'T :> IDisposable, [<InlineIfLambda>] body: 'T -> 'U) : 'U =
        try body resource finally (resource :> IDisposable).Dispose()
    [<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
    member inline _.Run([<InlineIfLambda>] f: unit -> 'T) : Task<'T> =
        RuntimeTaskBuilderUnsafe.cast (f())

[<AutoOpen>]
module RuntimeTaskBuilderModule =
    /// Computation expression for runtime-async methods.
    /// Produces flat IL bodies using AsyncHelpers.Await (no state machine).
    let runtimeTask = RuntimeTaskBuilder()
#endif
