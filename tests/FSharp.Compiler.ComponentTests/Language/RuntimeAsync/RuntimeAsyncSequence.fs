module RuntimeAsyncSequence

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks
open System.Threading.Tasks.Sources
open Microsoft.FSharp.Core.CompilerServices.StateMachineHelpers
open RuntimeAsyncSequenceBuilder

let check message condition = if not condition then failwith message
let gate<'T> () = TaskCompletionSource<'T>(TaskCreationOptions.RunContinuationsAsynchronously)

type SingleConsumeSource() =
    let mutable core = ManualResetValueTaskSourceCore<int>()
    let mutable consumed = 0
    member this.Value = ValueTask<int>(this, core.Version)
    member this.GetAwaiter() = this.Value.GetAwaiter()
    member _.Complete() = core.SetResult 42
    member _.Consumed = consumed
    interface IValueTaskSource<int> with
        member _.GetStatus(token) = core.GetStatus token
        member _.OnCompleted(continuation, state, token, flags) = core.OnCompleted(continuation, state, token, flags)
        member _.GetResult(token) =
            consumed <- consumed + 1
            check "single consumption" (consumed = 1)
            core.GetResult token

let example (work: Task<int>) (resource: IAsyncDisposable) = runtimeAsyncSeq {
    use cleanup = resource
    let! value = work
    let! value = ValueTask.FromResult(value).ConfigureAwait(false)
    do! ValueTask.CompletedTask.ConfigureAwait(false)
    for offset in [1; 2] do
        yield value + offset
    yield! [10]
    for value in runtimeAsyncSeq { yield 11 } do
        yield value
    yield! runtimeAsyncSeq { yield 12 }
}

let run () = __runtimeAsyncReturnUnit (
    let work, cleanup = gate<int>(), gate<unit>()
    let mutable closed = 0
    let resource = { new IAsyncDisposable with member _.DisposeAsync() = closed <- closed + 1; ValueTask(cleanup.Task) }
    let source = example work.Task resource
    let iterator = source.GetAsyncEnumerator()
    let move = iterator.MoveNextAsync()
    check "pending move" (not move.IsCompleted && closed = 0)
    work.SetResult 40
    let moved = AsyncHelpers.Await move
    check "first value" (moved && iterator.Current = 41)
    let disposal = iterator.DisposeAsync()
    check "pending cleanup" (not disposal.IsCompleted && closed = 1)
    cleanup.SetResult()
    AsyncHelpers.Await disposal
    AsyncHelpers.Await(iterator.DisposeAsync())
    let moved = AsyncHelpers.Await(iterator.MoveNextAsync())
    check "terminal disposal" (not moved && closed = 1)

    let values = ResizeArray()
    let iterator = source.GetAsyncEnumerator()
    let mutable more = true
    while more do
        let moved = AsyncHelpers.Await(iterator.MoveNextAsync())
        more <- moved
        if moved then values.Add iterator.Current
    AsyncHelpers.Await(iterator.DisposeAsync())
    check "fresh enumeration" (Seq.toList values = [41; 42; 10; 11; 12] && closed = 2)

    let bodyError = InvalidOperationException("body")
    let cleanup = gate<unit>()
    let resource = { new IAsyncDisposable with member _.DisposeAsync() = ValueTask(cleanup.Task) }
    let source = runtimeAsyncSeq {
        use cleanup = resource
        yield 1
        raise bodyError
    }
    let iterator = source.GetAsyncEnumerator()
    let moved = AsyncHelpers.Await(iterator.MoveNextAsync())
    check "before fault" moved
    let fault = iterator.MoveNextAsync()
    check "fault awaits cleanup" (not fault.IsCompleted)
    cleanup.SetResult()
    try
        AsyncHelpers.Await fault |> ignore
        failwith "lost body exception"
    with error ->
        check "body origin" (obj.ReferenceEquals(error, bodyError) && error.StackTrace.Contains("MoveNextAsync"))
    AsyncHelpers.Await(iterator.DisposeAsync())

    for form in [0; 1; 2] do
        let pending = SingleConsumeSource()
        let source =
            match form with
            | 0 -> runtimeAsyncSeq { let! value = pending.Value in yield value }
            | 1 -> runtimeAsyncSeq { let! value = pending.Value.ConfigureAwait(false) in yield value }
            | _ -> runtimeAsyncSeq { let! value = pending in yield value }
        let iterator = source.GetAsyncEnumerator()
        let move = iterator.MoveNextAsync()
        check "pending source" (not move.IsCompleted && pending.Consumed = 0)
        pending.Complete()
        check "typed awaiter result" (AsyncHelpers.Await move && iterator.Current = 42 && pending.Consumed = 1)
        AsyncHelpers.Await(iterator.DisposeAsync())

    let callbacks = Queue<SendOrPostCallback * obj>()
    let context = { new SynchronizationContext() with override _.Post(callback, state) = callbacks.Enqueue(callback, state) }
    let source = runtimeAsyncSeq {
        do! Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding ||| ConfigureAwaitOptions.ContinueOnCapturedContext)
        yield 1
    }
    let iterator = source.GetAsyncEnumerator()
    let previous = SynchronizationContext.Current
    let move =
        try
            SynchronizationContext.SetSynchronizationContext context
            iterator.MoveNextAsync()
        finally
            SynchronizationContext.SetSynchronizationContext previous
    check "forced yield posts to captured context" (not move.IsCompleted && callbacks.Count = 1)
    let callback, state = callbacks.Dequeue()
    callback.Invoke state
    check "forced yield result" (AsyncHelpers.Await move && iterator.Current = 1)
    AsyncHelpers.Await(iterator.DisposeAsync())

    let mutable effects = 0
    let mutable entered = 0
    let source =
        runtimeAsyncSeq.Run(effects <- effects + 1; fun () ->
            entered <- entered + 1
            seq { yield entered })
    check "eager prefix, cold body" (effects = 1 && entered = 0)
    for enumeration in 1..2 do
        let iterator = source.GetAsyncEnumerator()
        let moved = AsyncHelpers.Await(iterator.MoveNextAsync())
        check "prefix once, fresh body" (moved && iterator.Current = enumeration && effects = 1 && entered = enumeration)
        AsyncHelpers.Await(iterator.DisposeAsync())

    let source = withCancellation (fun token -> runtimeAsyncSeq {
        token.ThrowIfCancellationRequested()
        yield token
    })
    use first = new CancellationTokenSource()
    use second = new CancellationTokenSource()
    for token in [first.Token; second.Token] do
        let iterator = source.GetAsyncEnumerator(token)
        check "token identity" (AsyncHelpers.Await(iterator.MoveNextAsync()) && iterator.Current = token)
        AsyncHelpers.Await(iterator.DisposeAsync())
    first.Cancel()
    let iterator = source.GetAsyncEnumerator(first.Token)
    try
        AsyncHelpers.Await(iterator.MoveNextAsync()) |> ignore
        failwith "lost cancellation"
    with :? OperationCanceledException as error ->
        check "cancellation identity" (error.CancellationToken = first.Token)
    AsyncHelpers.Await(iterator.DisposeAsync())
)

[<EntryPoint>]
let main _ = run().GetAwaiter().GetResult(); 0
