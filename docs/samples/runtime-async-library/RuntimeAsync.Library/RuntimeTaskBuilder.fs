namespace RuntimeAsync.Library

open System
open System.Runtime.CompilerServices
open System.Threading.Tasks

#nowarn "57"
#nowarn "42"

module internal RuntimeTaskBuilderUnsafe =
    let inline cast<'a, 'b> (a: 'a) : 'b = (# "" a : 'b #)

[<Sealed>]
type RuntimeTaskBuilder() =
    member inline _.Return(x: 'T) : 'T = x

    [<NoDynamicInvocation>]
    member inline _.Bind(t: Task<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =
        f(AsyncHelpers.Await t)

    [<NoDynamicInvocation>]
    member inline _.Bind(t: Task, [<InlineIfLambda>] f: unit -> 'U) : 'U =
        AsyncHelpers.Await t
        f()

    [<NoDynamicInvocation>]
    member inline _.Bind(t: ValueTask<'T>, [<InlineIfLambda>] f: 'T -> 'U) : 'U =
        f(AsyncHelpers.Await t)

    [<NoDynamicInvocation>]
    member inline _.Bind(t: ValueTask, [<InlineIfLambda>] f: unit -> 'U) : 'U =
        AsyncHelpers.Await t
        f()

    member inline _.Delay(f: unit -> 'T) : unit -> 'T = f
    member inline _.Zero() : unit = ()
    member inline _.Combine((): unit, [<InlineIfLambda>] f: unit -> 'T) : 'T = f()

    member inline _.While([<InlineIfLambda>] guard: unit -> bool, [<InlineIfLambda>] body: unit -> unit) : unit =
        while guard() do
            body()

    [<NoDynamicInvocation>]
    member inline _.For(s: seq<'T>, [<InlineIfLambda>] body: 'T -> unit) : unit =
        for x in s do
            body(x)

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

    [<NoDynamicInvocation>]
    member inline _.Using(resource: 'T when 'T :> IDisposable, [<InlineIfLambda>] body: 'T -> 'U) : 'U =
        try
            body resource
        finally
            (resource :> IDisposable).Dispose()

    [<MethodImplAttribute(enum<MethodImplOptions> 0x2000)>]
    member inline _.Run(f: unit -> 'T) : Task<'T> =
        RuntimeTaskBuilderUnsafe.cast (f())

[<AutoOpen>]
module RuntimeTaskBuilderModule =
    let runtimeTask = RuntimeTaskBuilder()
