module Tests

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open RuntimeAsyncEnumerable
open RuntimeTaskBuilder
open RuntimeTaskBuilder.RuntimeTask
open AsyncSeqAwaitableExtensions

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
        member _.Dispose() = onDispose()

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
    asyncSeq {
        do! Task.Delay(5)
        yield "1"
        do! Task.Delay(5)
        yield "x"
        do! Task.Delay(5)
        yield "2"
    }

let private testBasicSequence () =
    runtimeTask {
        let! values = collect (basicSequence())
        assertEqual "basic sequence" [| "1"; "x"; "2" |] values
    }

let private testAwaitableKinds () =
    runtimeTask {
        let source =
            asyncSeq {
                let! taskValue = Task.FromResult 1
                let! valueTaskValue = ValueTask<int>(2)
                let! asyncValue = async { return 3 }
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
            asyncSeq {
                do! Task.Delay(25)
                let! taskValue = Task.FromResult 1
                let! valueTaskValue = ValueTask<int>(2)
                let! asyncValue = async { return 3 }
                do! Task.Delay(25)
                yield taskValue + valueTaskValue + asyncValue
            }

        let! values = collect source
        assertEqual "merged awaitables" [| 6 |] values
    }

let private testTryWith () =
    runtimeTask {
        let source =
            asyncSeq {
                try
                    yield 1
                    do! Task.Delay(10)
                    raise (InvalidOperationException("expected"))
                with
                | :? InvalidOperationException -> yield 2
            }

        let! values = collect source
        assertEqual "try/with" [| 1; 2 |] values
    }

let private testTryFinally () =
    runtimeTask {
        let mutable cleanedUp = false
        let source =
            asyncSeq {
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
            asyncSeq {
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
            asyncSeq {
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
            asyncSeq {
                yield! [ 5; 6 ]
                yield! (TestAsyncSource [| 7; 8 |] :> IAsyncEnumerable<int>)
            }

        let! values = collect source
        assertEqual "yield!" [| 5; 6; 7; 8 |] values
    }

let private testForAsyncEnumerable () =
    runtimeTask {
        let source =
            asyncSeq {
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
            asyncSeq {
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
        let gate = TaskCompletionSource<unit>(TaskCreationOptions.RunContinuationsAsynchronously)
        let source =
            asyncSeq {
                do! gate.Task
                yield 1
            }

        use enumerator = source.GetAsyncEnumerator(CancellationToken.None)
        let firstMove = enumerator.MoveNextAsync()
        let rejected =
            try
                enumerator.MoveNextAsync() |> ignore
                false
            with
            | :? InvalidOperationException -> true

        gate.SetResult(())
        assertTrue "concurrent MoveNext rejection" rejected
        let! firstMoveResult = firstMove
        assertTrue "concurrent MoveNext result" firstMoveResult
    }

let testTailRecursion () =
    runtimeTask {
        let rec loop n =
            asyncSeq {
                if n > 0 then
                    if n % 10000 = 0 then
                        do! Task.Delay 1 // simulate some async work
                    yield n
                    yield! loop (n - 1)
            }
        let! values = collect (loop 100_000)
        assertEqual "tail recursion" [| for i in 100_000 .. -1 .. 1 -> i |] values
    }

let runTests () =
    let tests : (string * Task<unit>) list =
        [ "basic sequence", testBasicSequence ()
          "awaitable kinds", testAwaitableKinds ()
          "merged awaitables", testMergedAwaitables ()
          "try/with", testTryWith ()
          "try/finally", testTryFinally ()
          "using", testUsing ()
          "while", testWhile ()
          "yield!", testYieldFrom ()
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

[<EntryPoint>]
let main _ =
    runTests() |> _.Result
