module CompilerService.AsyncMemoize

open System
open System.Threading
open Xunit
open Internal.Utilities.Collections
open System.Threading.Tasks
open System.Diagnostics
open System.Collections.Concurrent
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.BuildGraph


let timeout = TimeSpan.FromSeconds 10

let waitFor (mre: ManualResetEvent) = 
    if not <| mre.WaitOne timeout then 
        failwith "waitFor timed out"

let waitUntil condition value =
    task {
        let sw = Stopwatch.StartNew()
        while not <| condition value do
            if sw.Elapsed > timeout then
                failwith "waitUntil timed out"
            do! Task.Delay 10
    }

let rec internal spinFor (duration: TimeSpan) =
    node {
        let sw = Stopwatch.StartNew()
        do! Async.Sleep 10 |> NodeCode.AwaitAsync
        let remaining = duration - sw.Elapsed
        if remaining > TimeSpan.Zero then
            return! spinFor remaining
    }


type internal EventRecorder<'a, 'b, 'c when 'a : equality and 'b : equality>(memoize: AsyncMemoize<'a,'b,'c>) as self =

    let events = ConcurrentQueue()

    do memoize.OnEvent self.Add

    member _.Add (e, (_label, k, _version)) = events.Enqueue (e, k)

    member _.Received value = events |> Seq.exists (fst >> (=) value)

    member _.CountOf value count = events |> Seq.filter (fst >> (=) value) |> Seq.length |> (=) count

    member _.ShouldBe (expected) =
        let expected = expected |> Seq.toArray
        let actual = events |> Seq.toArray
        Assert.Equal<_ array>(expected, actual)


[<Fact>]
let ``Basics``() =

    let computation key = node {
        do! Async.Sleep 1 |> NodeCode.AwaitAsync
        return key * 2
    }

    let eventLog = ConcurrentBag()

    let memoize = AsyncMemoize<int, int, int>()
    memoize.OnEvent(fun (e, (_label, k, _version)) -> eventLog.Add (e, k))

    let result =
        seq {
            memoize.Get'(5, computation 5)
            memoize.Get'(5, computation 5)
            memoize.Get'(2, computation 2)
            memoize.Get'(5, computation 5)
            memoize.Get'(3, computation 3)
            memoize.Get'(2, computation 2)
        }
        |> NodeCode.Parallel
        |> NodeCode.RunImmediateWithoutCancellation

    let expected = [| 10; 10; 4; 10; 6; 4|]

    Assert.Equal<int array>(expected, result)

    let groups = eventLog |> Seq.groupBy snd |> Seq.toList
    Assert.Equal(3, groups.Length)
    for key, events in groups do
        Assert.Equal<Set<(JobEvent * int)>>(Set [ Requested, key; Started, key; Finished, key ], Set events)

[<Fact>]
let ``We can cancel a job`` () =
    task {

        let jobStarted = new ManualResetEvent(false)

        let computation action = node {
            action() |> ignore
            do! spinFor timeout 
            failwith "Should be canceled before it gets here"
        }

        let memoize = AsyncMemoize<_, int, _>()
        let events = EventRecorder(memoize)

        use cts1 = new CancellationTokenSource()
        use cts2 = new CancellationTokenSource()
        use cts3 = new CancellationTokenSource()

        let key = 1

        let _task1 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation jobStarted.Set), ct = cts1.Token)

        waitFor jobStarted
        jobStarted.Reset() |> ignore

        let _task2 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation ignore), ct = cts2.Token)
        let _task3 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation ignore), ct = cts3.Token)

        do! waitUntil (events.CountOf Requested) 3

        cts1.Cancel()
        cts2.Cancel()

        waitFor jobStarted

        cts3.Cancel()

        do! waitUntil events.Received Canceled

        events.ShouldBe [
            Requested, key
            Started, key
            Requested, key
            Requested, key
            Restarted, key
            Canceled, key
        ]
    }

[<Fact>]
let ``Job is restarted if first requestor cancels`` () =
    task {
        let jobStarted = new ManualResetEvent(false)

        let jobCanComplete = new ManualResetEvent(false)

        let computation key = node {
            jobStarted.Set() |> ignore
            waitFor jobCanComplete
            return key * 2
        }

        let memoize = AsyncMemoize<_, int, _>()
        let events = EventRecorder(memoize)


        use cts1 = new CancellationTokenSource()
        use cts2 = new CancellationTokenSource()
        use cts3 = new CancellationTokenSource()

        let key = 1

        let _task1 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation key), ct = cts1.Token)

        waitFor jobStarted
        jobStarted.Reset() |> ignore

        let _task2 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation key), ct = cts2.Token)
        let _task3 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation key), ct = cts3.Token)

        do! waitUntil (events.CountOf Requested) 3

        cts1.Cancel()

        waitFor jobStarted

        jobCanComplete.Set() |> ignore

        let! result = _task2
        Assert.Equal(2, result)

        events.ShouldBe [
            Requested, key
            Started, key
            Requested, key
            Requested, key
            Restarted, key
            Finished, key ]
    }

[<Fact>]
let ``Job is restarted if first requestor cancels but keeps running if second requestor cancels`` () =
    task {
        let jobStarted = new ManualResetEvent(false)

        let jobCanComplete = new ManualResetEvent(false)

        let computation key = node {
            jobStarted.Set() |> ignore
            waitFor jobCanComplete
            return key * 2
        }
        
        let memoize = AsyncMemoize<_, int, _>()
        let events = EventRecorder(memoize)


        use cts1 = new CancellationTokenSource()
        use cts2 = new CancellationTokenSource()
        use cts3 = new CancellationTokenSource()

        let key = 1

        let _task1 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation key), ct = cts1.Token)

        waitFor jobStarted
        jobStarted.Reset() |> ignore

        let _task2 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation key), ct = cts2.Token)
        let _task3 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation key), ct = cts3.Token)

        do! waitUntil (events.CountOf Requested) 3

        cts1.Cancel()

        waitFor jobStarted

        cts2.Cancel()

        jobCanComplete.Set() |> ignore

        let! result = _task3
        Assert.Equal(2, result)

        events.ShouldBe [
            Requested, key
            Started, key
            Requested, key
            Requested, key
            Restarted, key
            Finished, key ]
    }


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
        } |> NodeCode.AwaitAsync

    let rec sleepyComputation durationMs result =
        node {
            if rng.NextDouble() < (exceptionProbability / (float durationMs / float stepMs)) then
                raise (ExpectedException())
            if durationMs > 0 then
                do! Async.Sleep (min stepMs durationMs) |> NodeCode.AwaitAsync
                return! sleepyComputation (durationMs - stepMs) result
            else
                return [result]
        }

    let rec mixedComputation durationMs result =
        node {
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
                        let job = cache.Get'(key, computation durationMs result)
                        let cts = new CancellationTokenSource()
                        let runningJob = NodeCode.StartAsTask_ForTesting(job, ct = cts.Token)
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


[<Theory>]
[<InlineData(true, 1)>]
[<InlineData(false, 2)>]
let ``Cancel running jobs with the same key`` cancelDuplicate expectFinished =
    task {
        let cache = AsyncMemoize(cancelDuplicateRunningJobs=cancelDuplicate)

        let mutable started = 0
        let mutable finished = 0

        let job1started = new ManualResetEvent(false)
        let job1finished = new ManualResetEvent(false)

        let jobCanContinue = new ManualResetEvent(false)

        let job2started = new ManualResetEvent(false)
        let job2finished = new ManualResetEvent(false)

        let work onStart onFinish = node {
            Interlocked.Increment &started |> ignore
            onStart() |> ignore
            waitFor jobCanContinue
            do! spinFor (TimeSpan.FromMilliseconds 100)
            Interlocked.Increment &finished |> ignore
            onFinish() |> ignore
        }

        let key1 =
            { new ICacheKey<_, _> with
                  member _.GetKey() = 1
                  member _.GetVersion() = 1
                  member _.GetLabel() = "key1" }

        cache.Get(key1, work job1started.Set job1finished.Set) |> Async.AwaitNodeCode |> Async.Start

        waitFor job1started

        let key2 =
            { new ICacheKey<_, _> with
                  member _.GetKey() = key1.GetKey()
                  member _.GetVersion() = key1.GetVersion() + 1
                  member _.GetLabel() = "key2" }

        cache.Get(key2, work job2started.Set job2finished.Set ) |> Async.AwaitNodeCode |> Async.Start

        waitFor job2started

        jobCanContinue.Set() |> ignore

        waitFor job2finished
        
        if not cancelDuplicate then
            waitFor job1finished

        Assert.Equal((2, expectFinished), (started, finished))
    }


type DummyException(msg) =
    inherit Exception(msg)

[<Fact>]
let ``Preserve thread static diagnostics`` () = 

    let seed = System.Random().Next()

    let rng = System.Random seed
    
    let job1Cache = AsyncMemoize()
    let job2Cache = AsyncMemoize()

    let job1 (input: string) = node {
        let! _ = Async.Sleep (rng.Next(1, 30)) |> NodeCode.AwaitAsync
        let ex = DummyException("job1 error")
        DiagnosticsThreadStatics.DiagnosticsLogger.ErrorR(ex)
        return Ok input
    }

    let job2 (input: int) = node {
        
        DiagnosticsThreadStatics.DiagnosticsLogger.Warning(DummyException("job2 error 1"))

        let! _ = Async.Sleep (rng.Next(1, 30)) |> NodeCode.AwaitAsync

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

                let! result = job2Cache.Get(key, job2 (i % 10)) |> Async.AwaitNodeCode

                let diagnostics = diagnosticsLogger.GetDiagnostics()

                //Assert.Equal(3, diagnostics.Length)

                return result, diagnostics
            }
    }

    let results = (Task.WhenAll tasks).Result

    let _diagnosticCounts = results |> Seq.map snd |> Seq.map Array.length |> Seq.groupBy id |> Seq.map (fun (k, v) -> k, v |> Seq.length) |> Seq.sortBy fst |> Seq.toList

    //Assert.Equal<(int * int) list>([4, 100], diagnosticCounts)

    let diagnosticMessages = results |> Seq.map snd |> Seq.map (Array.map (fun (d, _) -> d.Exception.Message) >> Array.toList) |> Set

    Assert.Equal<Set<_>>(Set [["task error"; "job2 error 1"; "job1 error"; "job2 error 2"; ]], diagnosticMessages)


[<Fact>]
let ``Preserve thread static diagnostics already completed job`` () =

    let cache = AsyncMemoize()

    let key = { new ICacheKey<_, _> with
                    member _.GetKey() = "job1"
                    member _.GetVersion() = 1
                    member _.GetLabel() = "job1" }

    let job (input: string) = node {
        let ex = DummyException($"job {input} error")
        DiagnosticsThreadStatics.DiagnosticsLogger.ErrorR(ex)
        return Ok input
    }

    async {

        let diagnosticsLogger = CompilationDiagnosticLogger($"Testing", FSharpDiagnosticOptions.Default)

        use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Optimize)

        let! _ = cache.Get(key, job "1" ) |> Async.AwaitNodeCode
        let! _ = cache.Get(key, job "2" ) |> Async.AwaitNodeCode

        let diagnosticMessages = diagnosticsLogger.GetDiagnostics() |> Array.map (fun (d, _) -> d.Exception.Message) |> Array.toList

        Assert.Equal<list<_>>(["job 1 error"; "job 1 error"], diagnosticMessages)

    }
    |> Async.StartAsTask


[<Fact>]
let ``We get diagnostics from the job that failed`` () =

    let cache = AsyncMemoize()

    let key = { new ICacheKey<_, _> with
                    member _.GetKey() = "job1"
                    member _.GetVersion() = 1
                    member _.GetLabel() = "job1" }

    let job (input: int) = node {
        let ex = DummyException($"job {input} error")
        do! Async.Sleep 100 |> NodeCode.AwaitAsync
        DiagnosticsThreadStatics.DiagnosticsLogger.Error(ex)
        return 5
    }

    let result =
        [1; 2]
        |> Seq.map (fun i ->
            async {
            let diagnosticsLogger = CompilationDiagnosticLogger($"Testing", FSharpDiagnosticOptions.Default)

            use _ = new CompilationGlobalsScope(diagnosticsLogger, BuildPhase.Optimize)
            try
                let! _ = cache.Get(key, job i ) |> Async.AwaitNodeCode
                ()
            with _ ->
                ()
            let diagnosticMessages = diagnosticsLogger.GetDiagnostics() |> Array.map (fun (d, _) -> d.Exception.Message) |> Array.toList

            return diagnosticMessages
        })
        |> Async.Parallel
        |> Async.StartAsTask
        |> (fun t -> t.Result)
        |> Array.toList

    Assert.True(
        result = [["job 1 error"]; ["job 1 error"]] ||
        result = [["job 2 error"]; ["job 2 error"]] )
