module CompilerService.AsyncMemoize

open System
open System.Threading
open Internal.Utilities.Collections
open System.Threading.Tasks
open System.Diagnostics

open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Diagnostics

open Xunit

let internal observe (cache: AsyncMemoize<_,_,_>) =

    let collected = new MailboxProcessor<_>(fun _ -> async {})

    let arrivals = MailboxProcessor.Start(fun inbox ->
        let rec loop events = async {
            let! (e, (_, k, _)) = inbox.Receive()
            let events = (e, k) :: events
            printfn $"{k}: {e}"
            collected.Post events
            do! loop events
        }
        loop []
    )

    cache.Event.Add arrivals.Post

    let next () = collected.Receive()

    next

let rec awaitEvents next condition =
    async {
        match! next () with
        | events when condition events -> return events
        | _ -> return! awaitEvents next condition
    }

let rec eventsWhen next condition =
    awaitEvents next condition |> Async.RunSynchronously

let waitUntil next condition =
    eventsWhen next condition |> ignore

let expect next (expected: 't list) =
    let actual = eventsWhen next (List.length >> (=) expected.Length)
    Assert.Equal<'t list>(expected, actual |> List.rev)

let countOf value events =
    events |> Seq.filter (fst >> (=) value) |> Seq.length

let received event = function (a, _) :: _ when a = event -> true | _ -> false

let internal wrapKey key =
    { new ICacheKey<_, _> with
        member _.GetKey() = key
        member _.GetVersion() = Unchecked.defaultof<_>
        member _.GetLabel() = match key.ToString() with | null -> "" | s -> s
    }

let assertTaskCanceled (task: Task<_>) =
    Assert.ThrowsAnyAsync<OperationCanceledException>(fun () -> task).Result |> ignore

let awaitHandle h = h |> Async.AwaitWaitHandle |> Async.Ignore

[<Fact>]
let ``Basics``() =
    let computation key = async {
        do! Async.Sleep 1
        return key * 2
    }

    let memoize = AsyncMemoize<int, int, int>()
    let events = observe memoize

    let result =
        seq {
            memoize.Get(wrapKey 5, computation 5)
            memoize.Get(wrapKey 5, computation 5)
            memoize.Get(wrapKey 2, computation 2)
            memoize.Get(wrapKey 5, computation 5)
            memoize.Get(wrapKey 3, computation 3)
            memoize.Get(wrapKey 2, computation 2)
        }
        |> Async.Parallel
        |> Async.RunSynchronously

    let expected = [| 10; 10; 4; 10; 6; 4|]

    Assert.Equal<int array>(expected, result)

    let events = eventsWhen events (countOf Finished >> (=) 3)

    let groups = events |> Seq.groupBy snd |> Seq.toList
    Assert.Equal(3, groups.Length)
    for key, events in groups do
        Assert.Equal<Set<(JobEvent * int)>>(Set [ Requested, key; Started, key; Finished, key ], Set events)

[<Fact>]
let ``We can disconnect a request from a running job`` () =

    let cts = new CancellationTokenSource()
    let canFinish = new ManualResetEvent(false)

    let computation = async {
        do! awaitHandle canFinish
    }

    let memoize = AsyncMemoize<_, int, _>(cancelUnawaitedJobs = false)
    let events = observe memoize

    let key = 1

    let task1 = Async.StartAsTask( memoize.Get(wrapKey 1, computation), cancellationToken = cts.Token)

    waitUntil events (received Started)
    cts.Cancel()

    assertTaskCanceled task1

    canFinish.Set() |> ignore

    expect events
          [ Requested, key
            Started, key
            Finished, key ]

[<Fact>]
let ``We can cancel a job`` () =

    let cts = new CancellationTokenSource()

    let computation = async {
        while true do
            do! Async.Sleep 1000
    }

    let memoize = AsyncMemoize<_, int, _>()
    let events = observe memoize

    let key = 1

    let task1 = Async.StartAsTask( memoize.Get(wrapKey 1, computation), cancellationToken = cts.Token)

    waitUntil events (received Started)

    cts.Cancel()

    assertTaskCanceled task1

    expect events
          [ Requested, key
            Started, key
            Canceled, key ]

[<Fact>]
let ``Job is restarted if first requestor cancels`` () =
    let jobCanComplete = new ManualResetEvent(false)

    let computation key = async {
        do! awaitHandle jobCanComplete
        return key * 2
    }

    let memoize = AsyncMemoize<_, int, _>()
    let events = observe memoize

    use cts1 = new CancellationTokenSource()

    let key = 1

    let task1 = Async.StartAsTask( memoize.Get(wrapKey key, computation key), cancellationToken = cts1.Token)

    waitUntil events (received Started)
    cts1.Cancel()

    assertTaskCanceled task1

    waitUntil events (received Canceled)

    let task2 = Async.StartAsTask( memoize.Get(wrapKey key, computation key))

    waitUntil events (countOf Started >> (=) 2)

    jobCanComplete.Set() |> ignore

    Assert.Equal(2, task2.Result)

    expect events
      [ Requested, key
        Started, key
        Canceled, key
        Requested, key
        Started, key
        Finished, key ]

[<Fact>]
let ``Job is actually cancelled and restarted`` () =
    let jobCanComplete = new ManualResetEvent(false)
    let mutable finishedCount = 0

    let computation = async {
        do! awaitHandle jobCanComplete
        Interlocked.Increment &finishedCount |> ignore
        return 42
    }

    let memoize = AsyncMemoize<_, int, _>()
    let events = observe memoize

    let key = wrapKey 1

    for i in 1 .. 10 do
        use cts = new CancellationTokenSource()
        let task = Async.StartAsTask( memoize.Get(key, computation), cancellationToken = cts.Token)
        waitUntil events (received Started)
        cts.Cancel()
        assertTaskCanceled task
        waitUntil events (received Canceled)
        Assert.Equal(1, memoize.Count)

    let _task2 = Async.StartAsTask( memoize.Get(key, computation))

    waitUntil events (received Started)

    jobCanComplete.Set() |> ignore

    waitUntil events (received Finished)

    Assert.Equal(1, finishedCount)

[<Fact>]
let ``Job keeps running if only one requestor cancels`` () =

    let jobCanComplete = new ManualResetEvent(false)

    let computation key = async {
        do! awaitHandle jobCanComplete
        return key * 2
    }
        
    let memoize = AsyncMemoize<_, int, _>()
    let events = observe memoize

    use cts = new CancellationTokenSource()

    let key = 1

    let task1 = Async.StartAsTask( memoize.Get(wrapKey key, computation key))

    waitUntil events (received Started)

    let task2 = Async.StartAsTask( memoize.Get(wrapKey key, computation key) |> Async.Ignore, cancellationToken = cts.Token)

    waitUntil events (countOf Requested >> (=) 2)

    cts.Cancel()

    assertTaskCanceled task2

    jobCanComplete.Set() |> ignore

    Assert.Equal(2, task1.Result)

    expect events
      [ Requested, key
        Started, key
        Requested, key
        Finished, key ]

type ExpectedException() =
    inherit Exception()

[<Fact>]
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

    let testTimeoutMs = threads * iterations * maxDuration * 2

    let intenseComputation durationMs result =
        async {
            if rng.NextDouble() < exceptionProbability then
                raise (ExpectedException())
            let s = Stopwatch.StartNew()
            let mutable number = 0
            while (int s.ElapsedMilliseconds) < durationMs do
                number <- number + 1 % 12345
            return [result]
        }

    let rec sleepyComputation durationMs result =
        async {
            if rng.NextDouble() < (exceptionProbability / (float durationMs / float stepMs)) then
                raise (ExpectedException())
            if durationMs > 0 then
                do! Async.Sleep (min stepMs durationMs)
                return! sleepyComputation (durationMs - stepMs) result
            else
                return [result]
        }

    let rec mixedComputation durationMs result =
        async {
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
                        let runningJob = Async.StartAsTask(job, cancellationToken = cts.Token)
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
        |> Task.WhenAll

    if not (test.Wait testTimeoutMs) then failwith "Test timed out - most likely deadlocked"
    
    Assert.Equal (threads * iterations, started)
    // Assert.Equal<int * int * int * int * int>((0,0,0,0,0),(started, completed, canceled, failed, timeout))
    Assert.Equal (started, completed + canceled + failed + timeout)

    Assert.True ((float completed) > ((float started) * 0.1), "Less than 10 % completed jobs")


[<Fact>]
let ``Cancel running jobs with the same key`` () =
    let cache = AsyncMemoize(cancelUnawaitedJobs = false, cancelDuplicateRunningJobs = true)

    let events = observe cache

    let jobCanContinue = new ManualResetEvent(false)

    let work = async {
        do! awaitHandle jobCanContinue
    }

    let key version =
        { new ICacheKey<_, _> with
                member _.GetKey() = 1
                member _.GetVersion() = version
                member _.GetLabel() = $"key1 {version}" }

    let cts = new CancellationTokenSource()

    let jobsToCancel = 
        [ for i in 1 .. 10 -> Async.StartAsTask(cache.Get(key i , work), cancellationToken = cts.Token) ]

    waitUntil events (countOf Started >> (=) 10)

    // detach requests from their running computations
    cts.Cancel()

    for job in jobsToCancel do assertTaskCanceled job

    let job = cache.Get(key 11, work) |> Async.StartAsTask

    // up til now the jobs should have been running unobserved
    let current = eventsWhen events (received Requested)
    Assert.Equal(0, current |> countOf Canceled)

    waitUntil events (countOf Canceled >> (=) 10)

    waitUntil events (received Started)

    jobCanContinue.Set() |> ignore

    job.Wait()

    let events = eventsWhen events (received Finished)

    Assert.Equal(0, events |> countOf Failed)

    Assert.Equal(10, events |> countOf Canceled)

    Assert.Equal(1, events |> countOf Finished)

type DummyException(msg) =
    inherit Exception(msg)

[<Fact>]
let ``Preserve thread static diagnostics`` () = 

    let seed = System.Random().Next()

    let rng = System.Random seed
    
    let job1Cache = AsyncMemoize()
    let job2Cache = AsyncMemoize()

    let job1 (input: string) = async {
        let! _ = Async.Sleep (rng.Next(1, 30))
        let ex = DummyException("job1 error")
        DiagnosticsThreadStatics.DiagnosticsLogger.ErrorR(ex)
        return Ok input
    }

    let job2 (input: int) = async {
        
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

                let! result = job2Cache.Get(key, job2 (i % 10))

                let diagnostics = diagnosticsLogger.GetDiagnostics()

                Assert.Equal(4, diagnostics.Length)

                return result, diagnostics
            }
    }

    let results = (Task.WhenAll tasks).Result

    let diagnosticCounts = results |> Seq.map snd |> Seq.map Array.length |> Seq.groupBy id |> Seq.map (fun (k, v) -> k, v |> Seq.length) |> Seq.sortBy fst |> Seq.toList

    Assert.Equal<(int * int) list>([4, 100], diagnosticCounts)

    let diagnosticMessages = results |> Seq.map snd |> Seq.map (Array.map (fun (d, _) -> d.Exception.Message) >> Array.toList) |> Set

    Assert.Equal<Set<_>>(Set [["task error"; "job2 error 1"; "job1 error"; "job2 error 2"; ]], diagnosticMessages)


[<Fact>]
let ``Preserve thread static diagnostics already completed job`` () =

    let cache = AsyncMemoize()

    let key = { new ICacheKey<_, _> with
                    member _.GetKey() = "job1"
                    member _.GetVersion() = 1
                    member _.GetLabel() = "job1" }

    let job (input: string) = async {
        let ex = DummyException($"job {input} error")
        DiagnosticsThreadStatics.DiagnosticsLogger.ErrorR(ex)
        return Ok input
    }

    task {

        let diagnosticsLogger = CompilationDiagnosticLogger($"Testing", FSharpDiagnosticOptions.Default)

        use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Optimize)

        let! _ = cache.Get(key, job "1" )
        let! _ = cache.Get(key, job "2" )

        let diagnosticMessages = diagnosticsLogger.GetDiagnostics() |> Array.map (fun (d, _) -> d.Exception.Message) |> Array.toList

        Assert.Equal<_ list>(["job 1 error"; "job 1 error"], diagnosticMessages)

    }


[<Fact>]
let ``We get diagnostics from the job that failed`` () =

    let cache = AsyncMemoize()

    let key = { new ICacheKey<_, _> with
                    member _.GetKey() = "job1"
                    member _.GetVersion() = 1
                    member _.GetLabel() = "job1" }

    let job = async {
        let ex = DummyException($"job error")

        // no recovery
        DiagnosticsThreadStatics.DiagnosticsLogger.Error ex
        return 5
    }

    task {
        let logger = CapturingDiagnosticsLogger("AsyncMemoize diagnostics test")

        SetThreadDiagnosticsLoggerNoUnwind logger

        do! cache.Get(key, job ) |> Async.Catch |> Async.Ignore

        let messages = logger.Diagnostics |> List.map fst |> List.map _.Exception.Message

        Assert.Equal<_ list>(["job error"], messages)
    }
