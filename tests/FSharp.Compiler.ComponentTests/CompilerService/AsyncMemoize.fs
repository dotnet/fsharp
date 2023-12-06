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

[<Fact>]
let ``Stack trace`` () =

    let memoize = AsyncMemoize<int, int, int>()

    let computation key = node {
       // do! Async.Sleep 1 |> NodeCode.AwaitAsync

        let! result = memoize.Get'(key * 2, node {
            //do! Async.Sleep 1 |> NodeCode.AwaitAsync
            return key * 5
        })

        return result * 2
    }

    //let _r2 = computation 10

    let result = memoize.Get'(1, computation 1) |> NodeCode.RunImmediateWithoutCancellation

    Assert.Equal(10, result)


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
        Assert.Equal<Set<(JobEvent * int)>>(Set [ Started, key; Finished, key ], Set events)

[<Fact>]
let ``We can cancel a job`` () =
    task {

        let jobStarted = new ManualResetEvent(false)

        let computation key = node {
            jobStarted.Set() |> ignore
            do! Async.Sleep 1000 |> NodeCode.AwaitAsync
            failwith "Should be canceled before it gets here"
            return key * 2
        }

        let eventLog = ResizeArray()
        let memoize = AsyncMemoize<int, int, int>()
        memoize.OnEvent(fun (e, (_label, k, _version)) -> eventLog.Add (e, k))

        use cts1 = new CancellationTokenSource()
        use cts2 = new CancellationTokenSource()
        use cts3 = new CancellationTokenSource()

        let key = 1

        let _task1 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation key), ct = cts1.Token)
        let _task2 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation key), ct = cts2.Token)
        let _task3 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation key), ct = cts3.Token)

        jobStarted.WaitOne() |> ignore

        jobStarted.Reset() |> ignore

        Assert.Equal<(JobEvent * int) array>([| Started, key |], eventLog |> Seq.toArray )

        cts1.Cancel()
        cts2.Cancel()

        jobStarted.WaitOne() |> ignore

        Assert.Equal<(JobEvent * int) array>([| Started, key; Started, key |], eventLog |> Seq.toArray )

        cts3.Cancel()

        do! Task.Delay 100 

        Assert.Equal<(JobEvent * int) array>([| Started, key; Started, key; Canceled, key |], eventLog |> Seq.toArray )
    }

[<Fact>]
let ``Job is restarted if first requestor cancels`` () =
    task {
        let jobStarted = new ManualResetEvent(false)

        let computation key = node {
            jobStarted.Set() |> ignore

            for _ in 1 .. 5 do
                do! Async.Sleep 100 |> NodeCode.AwaitAsync

            return key * 2
        }

        let eventLog = ConcurrentBag()
        let memoize = AsyncMemoize<int, int, int>()
        memoize.OnEvent(fun (e, (_, k, _version)) -> eventLog.Add (DateTime.Now.Ticks, (e, k)))

        use cts1 = new CancellationTokenSource()
        use cts2 = new CancellationTokenSource()
        use cts3 = new CancellationTokenSource()

        let key = 1

        let _task1 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation key), ct = cts1.Token)
        let _task2 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation key), ct = cts2.Token)
        let _task3 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation key), ct = cts3.Token)

        jobStarted.WaitOne() |> ignore

        cts1.Cancel()

        do! Task.Delay 100
        cts3.Cancel()

        let! result = _task2
        Assert.Equal(2, result)

        Assert.Equal(TaskStatus.Canceled, _task1.Status)

        let orderedLog = eventLog |> Seq.sortBy fst |> Seq.map snd |> Seq.toList
        let expected = [ Started, key; Started, key; Finished, key ]

        Assert.Equal<_ list>(expected, orderedLog)
    }

[<Fact>]
let ``Job is restarted if first requestor cancels but keeps running if second requestor cancels`` () =
    task {
        let jobStarted = new ManualResetEvent(false)

        let computation key = node {
            jobStarted.Set() |> ignore

            for _ in 1 .. 5 do
                do! Async.Sleep 100 |> NodeCode.AwaitAsync

            return key * 2
        }

        let eventLog = ConcurrentBag()
        let memoize = AsyncMemoize<int, int, int>()
        memoize.OnEvent(fun (e, (_label, k, _version)) -> eventLog.Add (DateTime.Now.Ticks, (e, k)))

        use cts1 = new CancellationTokenSource()
        use cts2 = new CancellationTokenSource()
        use cts3 = new CancellationTokenSource()

        let key = 1

        let _task1 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation key), ct = cts1.Token)

        jobStarted.WaitOne() |> ignore
        jobStarted.Reset() |> ignore
        let _task2 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation key), ct = cts2.Token)
        let _task3 = NodeCode.StartAsTask_ForTesting( memoize.Get'(key, computation key), ct = cts3.Token)

        cts1.Cancel()

        jobStarted.WaitOne() |> ignore

        cts2.Cancel()

        let! result = _task3
        Assert.Equal(2, result)

        Assert.Equal(TaskStatus.Canceled, _task1.Status)

        let orderedLog = eventLog |> Seq.sortBy fst |> Seq.map snd |> Seq.toList
        let expected = [ Started, key; Started, key; Finished, key ]

        Assert.Equal<_ list>(expected, orderedLog)
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

    let testTimeoutMs = threads * iterations * maxDuration

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

        let work () = node {
            Interlocked.Increment &started |> ignore
            for _ in 1..10 do
                do! Async.Sleep 10 |> NodeCode.AwaitAsync
            Interlocked.Increment &finished |> ignore
        }

        let key1 =
            { new ICacheKey<_, _> with
                  member _.GetKey() = 1
                  member _.GetVersion() = 1
                  member _.GetLabel() = "key1" }

        cache.Get(key1, work()) |> Async.AwaitNodeCode |> Async.Start

        do! Task.Delay 50

        let key2 =
            { new ICacheKey<_, _> with
                  member _.GetKey() = key1.GetKey()
                  member _.GetVersion() = key1.GetVersion() + 1
                  member _.GetLabel() = "key2" }

        cache.Get(key2, work()) |> Async.AwaitNodeCode |> Async.Start

        do! Task.Delay 500

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

    Assert.Equal<list<_>>([["job 1 error"]; ["job 1 error"]], result)


[<Fact>]
let ``What if requestor cancels and their diagnosticsLogger gets disposed?``() =
    failwith "TODO"
    ()