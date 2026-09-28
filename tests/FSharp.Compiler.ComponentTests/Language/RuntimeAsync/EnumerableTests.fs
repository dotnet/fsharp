module EnumerableTests

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Threading
open System.Threading.Tasks

open Microsoft.FSharp.Control
open Microsoft.FSharp.Control.AsyncSeq2Implementation

let private assertEqual name expected actual =
    if expected <> actual then
        failwithf "%s failed. Expected %A, got %A." name expected actual

let private assertTrue name condition =
    if not condition then
        failwithf "%s failed." name

let private collect (source: IAsyncEnumerable<'T>) =
    runtimeTask {
        use enumerator = source.GetAsyncEnumerator(CancellationToken.None)
        let values = ResizeArray<'T>()

        while! enumerator.MoveNextAsync() do
            //printfn "Collected value: %A" enumerator.Current
            values.Add enumerator.Current

        return Seq.toArray values
    }

type CustomAwaitable(value: int) =
    member _.GetAwaiter() = Task.FromResult(value).GetAwaiter()

type private TrackingDisposable(onDispose: unit -> unit) =
    interface IDisposable with
        member _.Dispose() = onDispose ()

type private TestAsyncSource(values: int[]) =
    interface IAsyncEnumerable<int> with
        member _.GetAsyncEnumerator(_cancellationToken: CancellationToken) =
            let mutable index = -1

            { new IAsyncEnumerator<int> with
                member _.Current =
                    if index < 0 || index >= values.Length then
                        invalidOp "Current is not available."

                    values[index]

                member _.MoveNextAsync() =
                    index <- index + 1
                    ValueTask<bool>(index < values.Length)

                member _.DisposeAsync() = ValueTask() }

let private basicSequence () =
    asyncSeq2 {
        do! Task.Delay(5)
        yield "1"
        do! Task.Delay(5)
        yield "x"
        do! Task.Delay(5)
        yield "2"
    }

let private testBasicSequence () =
    runtimeTask {
        let! values = collect (basicSequence ())
        assertEqual "basic sequence" [| "1"; "x"; "2" |] values
    }

let private testAwaitableKinds () =
    runtimeTask {
        let source =
            asyncSeq2 {
                let! taskValue = Task.FromResult 1
                let! valueTaskValue = ValueTask<int>(2)
                let! asyncValue = async2 { return 3 }
                let! customValue = CustomAwaitable 4
                let! runtimeTaskValue = runtimeTask { return 5 }
                yield taskValue + valueTaskValue + asyncValue + customValue + runtimeTaskValue
            }

        let! values = collect source
        assertEqual "awaitable kinds" [| 15 |] values
    }

let private testMergedAwaitables () =
    runtimeTask {
        let source =
            asyncSeq2 {
                do! Task.Delay(25)
                let! taskValue = Task.FromResult 1
                let! valueTaskValue = ValueTask<int>(2)
                let! asyncValue = async2 { return 3 }
                do! Task.Delay(25)
                yield taskValue + valueTaskValue + asyncValue
            }

        let! values = collect source
        assertEqual "merged awaitables" [| 6 |] values
    }

let private testTryWith () =
    runtimeTask {
        let source =
            asyncSeq2 {
                try
                    yield 1
                    do! Task.Delay(10)
                    raise (InvalidOperationException("expected"))
                with :? InvalidOperationException ->
                    yield 2
            }

        let! values = collect source
        assertEqual "try/with" [| 1; 2 |] values
    }

let private testTryFinally () =
    runtimeTask {
        let mutable cleanedUp = false

        let source =
            asyncSeq2 {
                try
                    yield 3
                finally
                    cleanedUp <- true
            }

        let! values = collect source
        assertEqual "try/finally values" [| 3 |] values
        assertTrue "try/finally cleanup" cleanedUp
    }

let private testUsing () =
    runtimeTask {
        let mutable disposed = false

        let source =
            asyncSeq2 {
                use resource = new TrackingDisposable(fun () -> disposed <- true)
                yield 4
            }

        let! values = collect source
        assertEqual "using values" [| 4 |] values
        assertTrue "using disposal" disposed
    }

let private testWhile () =
    runtimeTask {
        let source =
            asyncSeq2 {
                let mutable value = 0

                while value < 3 do
                    do! Task.Delay(10)
                    yield value
                    value <- value + 1
            }

        let! values = collect source
        assertEqual "while loop" [| 0; 1; 2 |] values
    }

let private testYieldFrom () =
    runtimeTask {
        let source =
            asyncSeq2 {
                yield! [ 5; 6 ]
                yield! (TestAsyncSource [| 7; 8 |] :> IAsyncEnumerable<int>)
            }

        let! values = collect source
        assertEqual "yield!" [| 5; 6; 7; 8 |] values

        let recovered =
            asyncSeq2 {
                try
                    yield!
                        asyncSeq2 {
                            do! Task.FromException(InvalidOperationException("nested"))
                            yield 1
                        }
                with :? InvalidOperationException ->
                    yield 42
            }

        let! values = collect recovered
        assertEqual "yield! inside try/with" [| 42 |] values

        let mutable closed = 0

        let nested =
            asyncSeq2 {
                try
                    yield 1
                    yield 2
                finally
                    closed <- closed + 1
            }

        let outer =
            asyncSeq2 {
                try
                    yield! nested
                finally
                    closed <- closed + 10
            }

        let iterator = outer.GetAsyncEnumerator()
        let! moved = iterator.MoveNextAsync()
        assertTrue "nested first value" (moved && iterator.Current = 1)
        do! iterator.DisposeAsync()
        assertEqual "nested early disposal" 11 closed

        let mutable disposalCount = 0

        let failingChild =
            { new IAsyncEnumerable<int> with
                member _.GetAsyncEnumerator(_) =
                    { new IAsyncEnumerator<int> with
                        member _.Current = 0
                        member _.MoveNextAsync() = ValueTask<bool>(false)

                        member _.DisposeAsync() =
                            disposalCount <- disposalCount + 1
                            ValueTask(Task.FromException(InvalidOperationException("child disposal"))) } }

        let recoveredCleanup =
            asyncSeq2 {
                try
                    yield! failingChild
                with :? InvalidOperationException ->
                    yield 43
            }

        let! values = collect recoveredCleanup
        assertEqual "forwarded disposal failure" [| 43 |] values
        assertEqual "child disposed once" 1 disposalCount

    }

let private testTailHandoff () =
    runtimeTask {
        let child = asyncSeq2 { yield 7 }
        let final = asyncSeq2 { yield! child }
        use raw = final.RawEnumerator(CancellationToken.None)
        let! hasStep = raw.MoveNextAsync()
        assertTrue "final yield! emits a tail handoff" hasStep

        match raw.Current with
        | TailSource source -> assertTrue "tail handoff keeps its source" (obj.ReferenceEquals(source, child))
        | _ -> failwith "final yield! did not select YieldFromFinal"

        let continuing =
            asyncSeq2 {
                yield! child
                yield 8
            }

        use continuingRaw = continuing.RawEnumerator(CancellationToken.None)
        let! hasStep = continuingRaw.MoveNextAsync()
        assertTrue "non-final yield! emits a step" hasStep

        match continuingRaw.Current with
        | Source _ -> ()
        | _ -> failwith "non-final yield! lost its continuation"

        let mutable rootDisposals = 0

        let root =
            { new IAsyncEnumerable<AsyncSeq2Step<int>> with
                member _.GetAsyncEnumerator(_) =
                    let mutable moved = false

                    { new IAsyncEnumerator<AsyncSeq2Step<int>> with
                        member _.Current = TailSource(TestAsyncSource [| 9; 10 |])

                        member _.MoveNextAsync() =
                            if moved then
                                failwith "resumed a discarded tail parent"

                            moved <- true
                            ValueTask<bool>(true)

                        member _.DisposeAsync() =
                            rootDisposals <- rootDisposals + 1
                            ValueTask() } }

        let driver = AsyncSeq2<int>.Create root
        use iterator = driver.GetAsyncEnumerator()
        assertTrue "initial enumerator drives the tail" (iterator :? AsyncSeq2Enumerator<int>)
        let! first = iterator.MoveNextAsync()
        assertTrue "first tail element" (first && iterator.Current = 9)
        assertEqual "tail parent disposed before first element" 1 rootDisposals
        let! second = iterator.MoveNextAsync()
        assertTrue "second tail element" (second && iterator.Current = 10)
        let! completed = iterator.MoveNextAsync()
        assertTrue "tail completed" (not completed)
        assertEqual "completed enumerator resets Current" 0 iterator.Current
        assertEqual "tail parent disposed exactly once" 1 rootDisposals

        let mutable earlyClosed = 0

        let early =
            asyncSeq2 {
                yield!
                    asyncSeq2 {
                        try
                            yield 11
                            yield 12
                        finally
                            earlyClosed <- earlyClosed + 1
                    }
            }

        let earlyIterator = early.GetAsyncEnumerator()
        let! earlyFirst = earlyIterator.MoveNextAsync()
        assertTrue "tail early first element" (earlyFirst && earlyIterator.Current = 11)
        do! earlyIterator.DisposeAsync()
        do! earlyIterator.DisposeAsync()
        assertEqual "tail early disposal runs once" 1 earlyClosed

        let! listValues = collect (asyncSeq2 { yield! [ 11; 12 ] })
        assertEqual "final synchronous yield!" [| 11; 12 |] listValues

        let! continuedValues = collect continuing
        assertEqual "non-final continuation" [| 7; 8 |] continuedValues

        let mutable liveParents = 0
        let mutable peakParents = 0

        let rec chain n : AsyncSeq2<int> =
            AsyncSeq2<int>
                .Create(
                    { new IAsyncEnumerable<AsyncSeq2Step<int>> with
                        member _.GetAsyncEnumerator(_) =
                            liveParents <- liveParents + 1
                            peakParents <- max peakParents liveParents
                            let mutable moved = false

                            { new IAsyncEnumerator<AsyncSeq2Step<int>> with
                                member _.Current = if n = 0 then Value 17 else TailSource(chain (n - 1))

                                member _.MoveNextAsync() =
                                    if moved then
                                        ValueTask<bool>(false)
                                    else
                                        moved <- true
                                        ValueTask<bool>(true)

                                member _.DisposeAsync() =
                                    liveParents <- liveParents - 1
                                    ValueTask() } }
                )

        let! chainedValues = collect (chain 50_000)
        assertEqual "deep tail chain result" [| 17 |] chainedValues
        assertEqual "tail handoff keeps one raw parent alive" 1 peakParents
        assertEqual "deep tail chain disposes its leaf" 0 liveParents

        let nested =
            asyncSeq2 {
                try
                    yield! (asyncSeq2 { yield! (TestAsyncSource [| 13 |] :> IAsyncEnumerable<int>) })
                with :? InvalidOperationException ->
                    yield -1

                yield 14
            }

        let! nestedValues = collect nested
        assertEqual "tail child resumes its non-tail parent" [| 13; 14 |] nestedValues

        for failOnDispose in [ false; true ] do
            let mutable childDisposals = 0

            let failing =
                { new IAsyncEnumerable<int> with
                    member _.GetAsyncEnumerator(_) =
                        { new IAsyncEnumerator<int> with
                            member _.Current = 0

                            member _.MoveNextAsync() =
                                if failOnDispose then
                                    ValueTask<bool>(false)
                                else
                                    ValueTask<bool>(Task.FromException<bool>(InvalidOperationException("tail")))

                            member _.DisposeAsync() =
                                childDisposals <- childDisposals + 1

                                if failOnDispose then
                                    ValueTask(Task.FromException(InvalidOperationException("tail disposal")))
                                else
                                    ValueTask() } }

            let handled =
                asyncSeq2 {
                    try
                        yield! (asyncSeq2 { yield! failing })
                    with :? InvalidOperationException ->
                        yield 15

                    yield 16
                }

            let! handledValues = collect handled
            assertEqual "tail fault reaches non-tail handler" [| 15; 16 |] handledValues
            assertEqual "tail fault disposes child once" 1 childDisposals

            let terminal = asyncSeq2 { yield! failing }
            use failedIterator = terminal.GetAsyncEnumerator()

            let faulted =
                try
                    failedIterator.MoveNextAsync().GetAwaiter().GetResult() |> ignore
                    false
                with :? InvalidOperationException ->
                    true

            assertTrue "root tail fault propagates" faulted
            let! finished = failedIterator.MoveNextAsync()
            assertTrue "root tail fault is terminal" (not finished)
            assertEqual "root tail fault disposes child once" 2 childDisposals

        let nullTail =
            asyncSeq2 {
                yield 1
                yield! (null: IAsyncEnumerable<int>)
            }

        let nullFaulted =
            try
                (collect nullTail).GetAwaiter().GetResult() |> ignore
                false
            with :? NullReferenceException ->
                true

        assertTrue "null tail source faults" nullFaulted
    }

let private testForAsyncEnumerable () =
    runtimeTask {
        let source =
            asyncSeq2 {
                for value in (TestAsyncSource [| 9; 10 |] :> IAsyncEnumerable<int>) do
                    yield value + 1
            }

        let! values = collect source
        assertEqual "async for loop" [| 10; 11 |] values
    }

let private testPullDrivenEnumeration () =
    runtimeTask {
        let mutable sideEffects = 0

        let source =
            asyncSeq2 {
                sideEffects <- sideEffects + 1
                yield 1
                sideEffects <- sideEffects + 1
                yield 2
            }

        use enumerator = source.GetAsyncEnumerator(CancellationToken.None)
        assertEqual "pull before first move" 0 sideEffects
        let! firstMove = enumerator.MoveNextAsync()
        assertTrue "pull first move" firstMove
        assertEqual "pull first side effect" 1 sideEffects
        assertEqual "pull first value" 1 enumerator.Current
        let! secondMove = enumerator.MoveNextAsync()
        assertTrue "pull second move" secondMove
        assertEqual "pull second side effect" 2 sideEffects
        assertEqual "pull second value" 2 enumerator.Current
        let! completed = enumerator.MoveNextAsync()
        assertTrue "pull completion" (not completed)
    }

let private testConcurrentMoveNext () =
    runtimeTask {
        let gate =
            TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)

        let source =
            asyncSeq2 {
                do! gate.Task
                yield 1
            }

        use enumerator = source.GetAsyncEnumerator(CancellationToken.None)
        let firstMove = enumerator.MoveNextAsync()

        let rejected =
            try
                enumerator.MoveNextAsync() |> ignore
                false
            with :? InvalidOperationException ->
                true

        gate.SetResult(())
        assertTrue "concurrent MoveNext rejection" rejected
        let! firstMoveResult = firstMove
        assertTrue "concurrent MoveNext result" firstMoveResult
        let! finished = enumerator.MoveNextAsync()
        assertTrue "MoveNext after pending completion" (not finished)

        let failedGate =
            TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)

        let failingSource =
            asyncSeq2 {
                do! failedGate.Task
                yield 1
            }

        use failedEnumerator = failingSource.GetAsyncEnumerator()
        let failedMove = failedEnumerator.MoveNextAsync()
        failedGate.SetException(InvalidOperationException("failed"))

        let faulted =
            try
                failedMove.AsTask().GetAwaiter().GetResult() |> ignore
                false
            with :? InvalidOperationException ->
                true

        assertTrue "pending MoveNext fault" faulted
        let! completedAfterFault = failedEnumerator.MoveNextAsync()
        assertTrue "MoveNext after pending fault" (not completedAfterFault)
    }

[<MethodImpl(MethodImplOptions.NoInlining)>]
let private failDeep () : unit = invalidOp "deep"

let private testYieldFromFaults () =
    runtimeTask {
        let throwingOpen =
            { new IAsyncEnumerable<int> with
                member _.GetAsyncEnumerator(_) = invalidOp "open" }

        let mutable parentClosed = 0

        let source =
            asyncSeq2 {
                try
                    try
                        yield! throwingOpen
                    with :? InvalidOperationException ->
                        yield 1

                    yield 2
                finally
                    parentClosed <- parentClosed + 1
            }

        let! values = collect source
        assertEqual "child open fault reaches parent handler" [| 1; 2 |] values
        assertEqual "child open fault keeps parent alive" 1 parentClosed

        let tailOpenFault =
            asyncSeq2 {
                try
                    yield!
                        asyncSeq2 {
                            yield 3
                            yield! throwingOpen
                        }
                with :? InvalidOperationException ->
                    yield 4
            }

        let! tailValues = collect tailOpenFault
        assertEqual "tail open fault reaches the frame below" [| 3; 4 |] tailValues

        let throwingChild () =
            asyncSeq2 {
                do! Task.Yield()
                failDeep ()
                yield 0
            }

        let mutable trace = ""

        let observed =
            asyncSeq2 {
                try
                    yield! throwingChild ()
                with error ->
                    trace <- error.StackTrace
            }

        let! _ = collect observed
        assertTrue "child fault keeps its origin stack" (trace.Contains "failDeep")

        let gate = TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously)

        let pending =
            asyncSeq2 {
                do! gate.Task
                yield 1
            }

        let enumerator = pending.GetAsyncEnumerator()
        let move = enumerator.MoveNextAsync()

        let rejected =
            try
                enumerator.DisposeAsync() |> ignore
                false
            with :? InvalidOperationException ->
                true

        gate.SetResult()
        let! moved = move
        do! enumerator.DisposeAsync()
        assertTrue "dispose during pending move is rejected" (rejected && moved)
    }

let testTailRecursion () =
    runtimeTask {
        let rec loop n =
            asyncSeq2 {
                if n > 0 then
                    if n % 10000 = 0 then
                        do! Task.Delay 1 // simulate some async work

                    yield n
                    yield! loop (n - 1)
            }

        let! values = collect (loop 1000_000)
        assertEqual "tail recursion" [| for i in 1000_000..-1..1 -> i |] values
    }

let runTests () =
    let tests: (string * Task<unit>) list =
        [ "basic sequence", testBasicSequence ()
          "awaitable kinds", testAwaitableKinds ()
          "merged awaitables", testMergedAwaitables ()
          "try/with", testTryWith ()
          "try/finally", testTryFinally ()
          "using", testUsing ()
          "while", testWhile ()
          "yield!", testYieldFrom ()
          "tail handoff", testTailHandoff ()
          "yield! faults", testYieldFromFaults ()
          "async for loop", testForAsyncEnumerable ()
          "pull-driven enumeration", testPullDrivenEnumeration ()
          "concurrent MoveNext", testConcurrentMoveNext ()
          "tail recursion", testTailRecursion () ]

    runtimeTask {
        for name, test in tests do
            do! test
            printfn "PASS: %s" name

        return 0
    }
