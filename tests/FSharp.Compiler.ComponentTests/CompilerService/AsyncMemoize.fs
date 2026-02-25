module CompilerService.AsyncMemoize

open System
open System.Threading
open Internal.Utilities.Collections
open System.Threading.Tasks
open System.Diagnostics

open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Diagnostics
open Internal.Utilities.Library

open Xunit


type FactWithTimeoutAttribute() = inherit FactAttribute(Timeout = 300_000) // 5 minutes for good measure.

let internal observe (cache: AsyncMemoize<_,_,_>) =

    let events = System.Collections.Generic.List<_>()
    let gate = new SemaphoreSlim(0)

    cache.Event.Add(fun (e, (_, k, _)) ->
        printfn $"{k}: {e}"
        lock events (fun () -> events.Add((e, k)))
        gate.Release() |> ignore
    )

    let mutable position = 0

    let expectEvent (expectedE, expectedK) =
        let rec waitForPosition n =
            async2 {
                let count = lock events (fun () -> events.Count)

                if count > n then
                    return ()
                else
                    do! gate.WaitAsync()
                    return! waitForPosition n
            }

        async2 {
            let n = position
            position <- position + 1

            do! waitForPosition n

            let actual = lock events (fun () -> events[n])
            Assert.Equal((expectedE, expectedK), actual)
        }

    expectEvent

let internal wrapKey key =
    { new ICacheKey<_, _> with
        member _.GetKey() = key
        member _.GetVersion() = Unchecked.defaultof<_>
        member _.GetLabel() = match key.ToString() with | null -> "" | s -> s
    }

let assertTaskCanceled (task: Task<_>) =
    Assert.ThrowsAnyAsync<OperationCanceledException>(fun () -> task) :> Task


let awaitHandle h = h |> Async2.AwaitWaitHandle |> Async2.Ignore

[<FactWithTimeout>]
let ``Basics``() =
    let computation key = async2 {
        do! Async.Sleep 1
        return key * 2
    }

    let memoize = AsyncMemoize<int, int, int>()

    let gets =
        seq {
            memoize.Get(wrapKey 5, computation 5)
            memoize.Get(wrapKey 5, computation 5)
            memoize.Get(wrapKey 2, computation 2)
            memoize.Get(wrapKey 5, computation 5)
            memoize.Get(wrapKey 3, computation 3)
            memoize.Get(wrapKey 2, computation 2)
        }

    async2 {

        let expected = [| 10; 10; 4; 10; 6; 4|]

        let! result = gets |> Async2.Parallel

        Assert.Equal<int array>(expected, result)
    }
    |> Async2.StartAsTask

[<FactWithTimeout>]
let ``We can disconnect a request from a running job`` () =

    let cts = new CancellationTokenSource()
    let canFinish = new ManualResetEvent(false)

    let computation = async2 {
        do! awaitHandle canFinish
    }

    let memoize = AsyncMemoize<_, int, _>(cancelUnawaitedJobs = false)
    let expectEvent = observe memoize

    let key = 1

    let task1 = Async2.StartAsTask( memoize.Get(wrapKey 1, computation), cancellationToken = cts.Token)

    async2 {
        do! expectEvent (Requested, key)
        do! expectEvent (Started, key)
        cts.Cancel()

        do! assertTaskCanceled task1

        canFinish.Set() |> ignore

        do! expectEvent (Finished, key)
    }
    |> Async2.StartAsTask

[<FactWithTimeout>]
let ``We can cancel a job`` () =

    let cts = new CancellationTokenSource()

    let computation = async2 {
        while true do
            do! Async.Sleep 1000
    }

    let memoize = AsyncMemoize<_, int, _>()
    let expectEvent = observe memoize

    let key = 1

    let task1 = Async2.StartAsTask( memoize.Get(wrapKey 1, computation), cancellationToken = cts.Token)

    async2 {
        do! expectEvent (Requested, key)
        do! expectEvent (Started, key)
        cts.Cancel()

        do! assertTaskCanceled task1

        do! expectEvent (Canceled, key)
    }
    |> Async2.StartAsTask

[<FactWithTimeout>]
let ``Job is restarted if first requestor cancels`` () =
    let jobCanComplete = new ManualResetEvent(false)

    let computation key = async2 {
        do! awaitHandle jobCanComplete
        return key * 2
    }

    let memoize = AsyncMemoize<_, int, _>()
    let expectEvent = observe memoize

    let cts1 = new CancellationTokenSource()

    let key = 1

    let task1 = Async2.StartAsTask( memoize.Get(wrapKey key, computation key), cancellationToken = cts1.Token)

    async2 {
        do! expectEvent (Requested, key)
        do! expectEvent (Started, key)
        cts1.Cancel()

        do! assertTaskCanceled task1

        do! expectEvent (Canceled, key)

        let task2 = Async2.StartAsTask( memoize.Get(wrapKey key, computation key))

        do! expectEvent (Requested, key)
        do! expectEvent (Started, key)

        jobCanComplete.Set() |> ignore

        let! result = task2

        Assert.Equal(2, result)

        do! expectEvent (Finished, key)
    }
    |> Async2.StartAsTask

[<FactWithTimeout>]
let ``Job is actually cancelled and restarted`` () =
    let jobCanComplete = new ManualResetEvent(false)
    let mutable finishedCount = 0

    let computation = async2 {
        do! awaitHandle jobCanComplete
        Interlocked.Increment &finishedCount |> ignore
        return 42
    }

    let memoize = AsyncMemoize<_, int, _>()
    let expectEvent = observe memoize

    let key = wrapKey 1

    async2 {
        for _ in 1 .. 10 do
            let cts = new CancellationTokenSource()
            let task = Async2.StartAsTask( memoize.Get(key, computation), cancellationToken = cts.Token)
            do! expectEvent (Requested, 1)
            do! expectEvent (Started, 1)
            cts.Cancel()
            do! assertTaskCanceled task
            do! expectEvent (Canceled, 1)
            Assert.Equal(1, memoize.Count)

        Async2.Start( memoize.Get(key, computation))

        do! expectEvent (Requested, 1)
        do! expectEvent (Started, 1)

        jobCanComplete.Set() |> ignore

        do! expectEvent (Finished, 1)

        Assert.Equal(1, finishedCount)
    }
    |> Async2.StartAsTask

[<FactWithTimeout>]
let ``Job keeps running if only one requestor cancels`` () =

    let jobCanComplete = new ManualResetEvent(false)

    let computation key = async2 {
        do! awaitHandle jobCanComplete
        return key * 2
    }
        
    let memoize = AsyncMemoize<_, int, _>()
    let expectEvent = observe memoize

    let cts = new CancellationTokenSource()

    let key = 1

    let task1 = Async2.StartAsTask( memoize.Get(wrapKey key, computation key))

    async2 {
        do! expectEvent (Requested, key)
        do! expectEvent (Started, key)

        let task2 = Async2.StartAsTask( memoize.Get(wrapKey key, computation key) |> Async2.Ignore, cancellationToken = cts.Token)

        do! expectEvent (Requested, key)
        cts.Cancel()

        do! assertTaskCanceled task2

        jobCanComplete.Set() |> ignore

        let! result1 = task1

        Assert.Equal(2, result1)

        do! expectEvent (Finished, key)
    }
    |> Async2.StartAsTask

type ExpectedException() =
    inherit Exception()

[<FactWithTimeout>]
let ``Stress test`` () =

    let seed = System.Random().Next()

    let rng = System.Random seed
    let threads = 30
    let iterations = 30
    let maxDuration = 100
    let minTimeout = 0
    let maxTimeout = 500
    let exceptionProbability = 0.01
    let gcProbability = 0.1
    let stepMs = 10
    let keyCount = rng.Next(5, 200)
    let keys = [| 1 .. keyCount |]

    let intenseComputation durationMs result =
        async2 {
            if rng.NextDouble() < exceptionProbability then
                raise (ExpectedException())
            let s = Stopwatch.StartNew()
            let mutable number = 0
            while (int s.ElapsedMilliseconds) < durationMs do
                number <- number + 1 % 12345
            return [result]
        }

    let rec sleepyComputation durationMs result =
        async2 {
            if rng.NextDouble() < (exceptionProbability / (float durationMs / float stepMs)) then
                raise (ExpectedException())
            if durationMs > 0 then
                do! Async.Sleep (min stepMs durationMs)
                return! sleepyComputation (durationMs - stepMs) result
            else
                return [result]
        }

    let rec mixedComputation durationMs result =
        async2 {
            if durationMs > 0 then
                if rng.NextDouble() < 0.5 then
                    let! _ = intenseComputation (min stepMs durationMs) ()
                    ()
                else
                    let! _ = sleepyComputation (min stepMs durationMs) ()
                    ()
                return! mixedComputation (durationMs - stepMs) result
            else
                return [result]
        }

    let computations = [|
        intenseComputation
        sleepyComputation
        mixedComputation
    |]

    let cache = AsyncMemoize<int, int, int list>(keepStrongly=5, keepWeakly=10)

    let mutable started = 0
    let mutable canceled = 0
    let mutable timeout = 0
    let mutable failed = 0
    let mutable completed = 0

    let test =
        seq {
            for _ in 1..threads do
                let rec loop iteration =
                    task {
                        if gcProbability > rng.NextDouble() then
                            GC.Collect(2, GCCollectionMode.Forced, false)

                        let computation = computations[rng.Next computations.Length]
                        let durationMs = rng.Next maxDuration
                        let timeoutMs = rng.Next(minTimeout, maxTimeout)
                        let key = keys[rng.Next keys.Length]
                        let result = key * 2
                        let job = cache.Get(wrapKey key, computation durationMs result)
                        let cts = new CancellationTokenSource()
                        let runningJob = Async2.StartAsTask(job, cancellationToken = cts.Token)
                        cts.CancelAfter timeoutMs
                        Interlocked.Increment &started |> ignore
                        try
                            let! actual = runningJob
                            Assert.Equal(result, actual.Head)
                            Interlocked.Increment &completed |> ignore
                        with
                            | :? TaskCanceledException as _e ->
                                Interlocked.Increment &canceled |> ignore
                            | :? OperationCanceledException as _e ->
                                Interlocked.Increment &canceled |> ignore
                            | :? TimeoutException -> Interlocked.Increment &timeout |> ignore
                            | :? ExpectedException -> Interlocked.Increment &failed |> ignore
                            | :? AggregateException as ex when
                                ex.Flatten().InnerExceptions |> Seq.exists (fun e -> e :? ExpectedException) ->
                                Interlocked.Increment &failed |> ignore
                            | e ->
                                failwith $"Seed {seed} failed on iteration {iteration}: %A{e}"
                        if iteration < iterations then
                            return! loop (iteration + 1)
                        return ()
                    }
                loop 1
        }

    task {
        let! _ = test |> Task.WhenAll
    
        Assert.Equal (threads * iterations, started)
        // Assert.Equal<int * int * int * int * int>((0,0,0,0,0),(started, completed, canceled, failed, timeout))
        Assert.Equal (started, completed + canceled + failed + timeout)

        Assert.True ((float completed) > ((float started) * 0.1), "Less than 10 % completed jobs")
    }


type DummyException(msg) =
    inherit Exception(msg)

[<FactWithTimeout>]
let ``Preserve thread static diagnostics`` () = 

    let seed = System.Random().Next()

    let rng = System.Random seed
    
    let job1Cache = AsyncMemoize()
    let job2Cache = AsyncMemoize()

    let job1 (input: string) = async2 {
        let! _ = Async.Sleep (rng.Next(1, 30))
        let ex = DummyException("job1 error")
        DiagnosticsThreadStatics.DiagnosticsLogger.ErrorR(ex)
        return Ok input
    }

    let job2 (input: int) = async2 {
       
        DiagnosticsThreadStatics.DiagnosticsLogger.Warning(DummyException("job2 error 1"))

        let! _ = Async.Sleep (rng.Next(1, 30))

        let key = { new ICacheKey<_, _> with
                        member _.GetKey() = "job1"
                        member _.GetVersion() = input
                        member _.GetLabel() = "job1" }

        let! result = job1Cache.Get(key, job1 "${input}" )

        DiagnosticsThreadStatics.DiagnosticsLogger.Warning(DummyException("job2 error 2"))

        return input, result

    }

    let tasks = seq {
        for i in 1 .. 100 do

            task {
                let diagnosticsLogger =
                    CompilationDiagnosticLogger($"Testing task {i}", FSharpDiagnosticOptions.Default)

                use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Optimize)

                DiagnosticsThreadStatics.DiagnosticsLogger.Warning(DummyException("task error"))


                let key = { new ICacheKey<_, _> with
                                member _.GetKey() = "job2"
                                member _.GetVersion() = rng.Next(1, 10)
                                member _.GetLabel() = "job2" }

                let! result = job2Cache.Get(key, job2 (i % 10)) |> Async2.StartAsTask

                let diagnostics = diagnosticsLogger.GetDiagnostics()

                Assert.Equal(4, diagnostics.Length)

                return result, diagnostics
            }
    }

    task {
        let! results = (Task.WhenAll tasks)

        let diagnosticCounts = results |> Seq.map snd |> Seq.map Array.length |> Seq.groupBy id |> Seq.map (fun (k, v) -> k, v |> Seq.length) |> Seq.sortBy fst |> Seq.toList

        Assert.Equal<(int * int) list>([4, 100], diagnosticCounts)

        let diagnosticMessages = results |> Seq.map snd |> Seq.map (Array.map _.Exception.Message >> Array.toList) |> Set

        Assert.Equal<Set<_>>(Set [["task error"; "job2 error 1"; "job1 error"; "job2 error 2"; ]], diagnosticMessages)
    }


[<FactWithTimeout>]
let ``Preserve thread static diagnostics already completed job`` () =

    let cache = AsyncMemoize()

    let key = { new ICacheKey<_, _> with
                    member _.GetKey() = "job1"
                    member _.GetVersion() = 1
                    member _.GetLabel() = "job1" }

    let job (input: string) = async2 {
        let ex = DummyException($"job {input} error")
        DiagnosticsThreadStatics.DiagnosticsLogger.ErrorR(ex)
        return Ok input
    }

    async2 {
        let diagnosticsLogger = CompilationDiagnosticLogger($"Testing", FSharpDiagnosticOptions.Default)

        use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Optimize)

        let! _ = cache.Get(key, job "1" )
        let! _ = cache.Get(key, job "2" )

        let diagnosticMessages = diagnosticsLogger.GetDiagnostics() |> Array.map _.Exception.Message |> Array.toList

        Assert.Equal<_ list>(["job 1 error"; "job 1 error"], diagnosticMessages)
    }
    |> Async2.StartAsTask


[<FactWithTimeout>]
let ``We get diagnostics from the job that failed`` () =

    let cache = AsyncMemoize()

    let key = { new ICacheKey<_, _> with
                    member _.GetKey() = "job1"
                    member _.GetVersion() = 1
                    member _.GetLabel() = "job1" }

    let job = async2 {
        let ex = DummyException($"job error")

        // no recovery
        DiagnosticsThreadStatics.DiagnosticsLogger.Error ex
        return 5
    }

    async2 {
        let logger = CapturingDiagnosticsLogger("AsyncMemoize diagnostics test")

        SetThreadDiagnosticsLoggerNoUnwind logger

        do! cache.Get(key, job ) |> Async2.Catch |> Async2.Ignore

        let messages = logger.Diagnostics |> List.map _.Exception.Message

        Assert.Equal<_ list>(["job error"], messages)
    }
    |> Async2.StartAsTask
