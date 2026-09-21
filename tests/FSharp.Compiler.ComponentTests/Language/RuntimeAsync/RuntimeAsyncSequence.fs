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

let checkDispatch (iterator: IAsyncEnumerator<'T>) =
    let generatedType = iterator.GetType()
    for interfaceType in [typeof<IAsyncEnumerator<'T>>; typeof<IAsyncDisposable>] do
        let mapping = generatedType.GetInterfaceMap interfaceType
        let targets = mapping.TargetMethods |> Array.map (fun method -> $"{method.DeclaringType.FullName}.{method.Name}")
        check (String.Join(", ", targets))
            (mapping.TargetMethods |> Array.forall (fun method -> method.DeclaringType = generatedType))

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
    check "reuse first enumeration" (obj.ReferenceEquals(source, iterator))
    checkDispatch iterator
    let unused = source.GetAsyncEnumerator()
    let nestedFresh = (unused :?> IAsyncEnumerable<int>).GetAsyncEnumerator()
    check "fresh clones are already acquired" (not (obj.ReferenceEquals(iterator, unused)) && not (obj.ReferenceEquals(unused, nestedFresh)))
    AsyncHelpers.Await(unused.DisposeAsync())
    AsyncHelpers.Await(nestedFresh.DisposeAsync())
    check "unstarted disposal is cold" (closed = 0)
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

    let ordering = ResizeArray<int>()
    let next () = ordering.Add 1; Task.FromResult 40
    let receiver () = ordering.Add 2; fun value -> value + 2
    let value =
        let pending = next()
        (receiver()) (AsyncHelpers.Await pending)
    check "source precedes effectful receiver" (value = 42 && List.ofSeq ordering = [1; 2])
    ordering.Clear()
    let shared = next()
    let total = AsyncHelpers.Await shared + AsyncHelpers.Await shared
    check "shared source is evaluated once" (total = 80 && List.ofSeq ordering = [1])

    let mutable entered = 0
    let concurrent = runtimeAsyncSeq { entered <- entered + 1; yield 1 }
    let acquisitions = Array.init 8 (fun _ -> Task.Run(fun () -> concurrent.GetAsyncEnumerator()))
    let iterators = AsyncHelpers.Await(Task.WhenAll acquisitions)
    let identities = HashSet<IAsyncEnumerator<int>>(HashIdentity.Reference)
    for iterator in iterators do
        check "independent concurrent acquisitions" (identities.Add iterator)
        AsyncHelpers.Await(iterator.DisposeAsync())
    check "one first-enumeration reuse" ((iterators |> Array.filter (fun iterator -> obj.ReferenceEquals(concurrent, iterator))).Length = 1)
    check "concurrent acquisition is cold" (entered = 0)
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

    for prefix, cleanupFails in [false, false; true, false; false, true; true, true] do
        let bodyError = InvalidOperationException("body")
        let cleanupError = InvalidOperationException("cleanup")
        let work, cleanup, cleanupEntered = gate<int>(), gate<unit>(), gate<unit>()
        let resource =
            { new IAsyncDisposable with
                member _.DisposeAsync() =
                    cleanupEntered.SetResult()
                    ValueTask(cleanup.Task) }
        let source = runtimeAsyncSeq {
            use cleanup = resource
            if prefix then yield 1
            let! value = work.Task
            yield value
        }
        let iterator = source.GetAsyncEnumerator()
        if prefix then check "before fault" (AsyncHelpers.Await(iterator.MoveNextAsync()))
        let fault = iterator.MoveNextAsync()
        check "pending before fault" (not fault.IsCompleted)
        work.SetException bodyError
        AsyncHelpers.Await cleanupEntered.Task
        check "fault awaits cleanup" (not fault.IsCompleted)
        if cleanupFails then cleanup.SetException cleanupError else cleanup.SetResult()
        try
            AsyncHelpers.Await fault |> ignore
            failwith "lost body exception"
        with error ->
            let expected = if cleanupFails then cleanupError else bodyError
            check "fault origin" (obj.ReferenceEquals(error, expected) && error.StackTrace.Contains("MoveNextAsync"))
        check "terminal fault" (not (AsyncHelpers.Await(iterator.MoveNextAsync())))
        AsyncHelpers.Await(iterator.DisposeAsync())

    for cleanupFails in [false; true] do
        let work = gate<int>()
        let bodyError, cleanupError = InvalidOperationException("body"), InvalidOperationException("cleanup")
        let mutable closed = 0
        let source = runtimeAsyncSeq {
            try
                let! value = work.Task
                yield value
            finally
                closed <- closed + 1
                if cleanupFails then raise cleanupError
        }
        let iterator = source.GetAsyncEnumerator()
        let pending = iterator.MoveNextAsync()
        check "pending before synchronous cleanup" (not pending.IsCompleted)
        work.SetException bodyError
        try
            AsyncHelpers.Await pending |> ignore
            failwith "lost synchronous cleanup fault"
        with error ->
            check "synchronous cleanup exception" (obj.ReferenceEquals(error, if cleanupFails then cleanupError else bodyError))
            check "synchronous cleanup stack" (error.StackTrace.Contains("MoveNextAsync"))
        check "synchronous cleanup is terminal" (closed = 1 && not (AsyncHelpers.Await(iterator.MoveNextAsync())))
        AsyncHelpers.Await(iterator.DisposeAsync())
        check "synchronous cleanup runs once" (closed = 1)

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

    let source = withCancellation(fun token -> runtimeAsyncSeq {
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
