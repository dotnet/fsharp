module RuntimeAsyncSequenceBuilder

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open Microsoft.FSharp.Core.CompilerServices

let inline cancellationToken () =
    StateMachineHelpers.__runtimeAsyncSequenceCancellationToken()

type RuntimeAsyncSequenceBuilder() =
    member inline _.Zero() = Seq.empty
    member inline _.Yield(value) = Seq.singleton value
    member inline _.Delay([<InlineIfLambda>] body: unit -> seq<'T>) = body
    member inline _.Combine(first, [<InlineIfLambda>] rest) =
        Seq.append first (Seq.delay (fun () -> rest()))
    member inline _.For(source: seq<'U>, [<InlineIfLambda>] body: 'U -> seq<'T>) =
        Seq.collect (fun value -> body value) source
    member inline _.While([<InlineIfLambda>] guard, [<InlineIfLambda>] body) =
        RuntimeHelpers.EnumerateWhile (fun () -> guard()) (Seq.delay (fun () -> body()))
    member inline _.TryFinally([<InlineIfLambda>] body, [<InlineIfLambda>] compensation) =
        RuntimeHelpers.EnumerateThenFinally (Seq.delay (fun () -> body())) (fun () -> compensation())
    member inline _.Using(resource: 'R when 'R :> IAsyncDisposable, [<InlineIfLambda>] body: 'R -> seq<'T>) =
        RuntimeHelpers.EnumerateThenFinally
            (Seq.delay (fun () -> body resource))
            (fun () ->
                if not (isNull (box resource)) then
                    AsyncHelpers.Await(resource.DisposeAsync()))
    member inline this.For(source: IAsyncEnumerable<'U>, [<InlineIfLambda>] body: 'U -> seq<'T>) =
        Seq.delay (fun () ->
            let iterator = source.GetAsyncEnumerator()
            this.Using(iterator, fun iterator ->
                this.While(
                    (fun () -> AsyncHelpers.Await(iterator.MoveNextAsync())),
                    (fun () -> body iterator.Current))))
    member inline this.YieldFrom(source: seq<'T>) = this.For(source, fun value -> this.Yield value)
    member inline this.YieldFrom(source: IAsyncEnumerable<'T>) = this.For(source, fun value -> this.Yield value)
    member inline _.Bind(source: Task<'U>, [<InlineIfLambda>] continuation: 'U -> seq<'T>) =
        continuation (AsyncHelpers.Await source)
    member inline _.Bind(source: Task, [<InlineIfLambda>] continuation: unit -> seq<'T>) =
        AsyncHelpers.Await source
        continuation()
    member inline _.Bind(source: ValueTask<'U>, [<InlineIfLambda>] continuation: 'U -> seq<'T>) =
        continuation (AsyncHelpers.Await source)
    member inline _.Bind(source: ValueTask, [<InlineIfLambda>] continuation: unit -> seq<'T>) =
        AsyncHelpers.Await source
        continuation()
    member inline _.Bind(source: ConfiguredValueTaskAwaitable<'U>, [<InlineIfLambda>] continuation: 'U -> seq<'T>) =
        continuation (AsyncHelpers.Await source)
    member inline _.Bind(source: ConfiguredValueTaskAwaitable, [<InlineIfLambda>] continuation: unit -> seq<'T>) =
        AsyncHelpers.Await source
        continuation()
    member inline _.Bind(source: ConfiguredTaskAwaitable<'U>, [<InlineIfLambda>] continuation: 'U -> seq<'T>) =
        continuation (AsyncHelpers.Await source)
    member inline _.Bind(source: ConfiguredTaskAwaitable, [<InlineIfLambda>] continuation: unit -> seq<'T>) =
        AsyncHelpers.Await source
        continuation()
    member inline _.Run([<InlineIfLambda>] recipe: unit -> seq<'T>) : IAsyncEnumerable<'T> =
        StateMachineHelpers.__runtimeAsyncSequence recipe

let runtimeAsyncSeq = RuntimeAsyncSequenceBuilder()

[<AutoOpen>]
module Extensions =
    type RuntimeAsyncSequenceBuilder with
        member inline _.Bind(source: ^Awaitable, [<InlineIfLambda>] continuation: 'T -> seq<'U>) =
            let awaiter = (^Awaitable: (member GetAwaiter: unit -> ^Awaiter) source)
            if not (^Awaiter: (member get_IsCompleted: unit -> bool) awaiter) then
                AsyncHelpers.UnsafeAwaitAwaiter awaiter
            continuation (^Awaiter: (member GetResult: unit -> 'T) awaiter)

let inline withCancellation ([<InlineIfLambda>] create: CancellationToken -> IAsyncEnumerable<'T>) =
    { new IAsyncEnumerable<'T> with
        member _.GetAsyncEnumerator(token) = (create token).GetAsyncEnumerator() }
