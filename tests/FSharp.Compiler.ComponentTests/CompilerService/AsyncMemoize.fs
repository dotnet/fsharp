module CompilerService.AsyncMemoize

open System
open System.Threading
open Xunit
open FSharp.Test
open Internal.Utilities.Collections
open System.Threading.Tasks
open System.Diagnostics
open System.Collections.Concurrent


[<Fact>]
let ``Stack trace`` () =

    let memoize = AsyncMemoize<int, int, int>()

    let computation key = async {
       // do! Async.Sleep 1 |> NodeCode.AwaitAsync

        let! result = memoize.Get'(key * 2, async {
            //do! Async.Sleep 1 |> NodeCode.AwaitAsync
            return key * 5
        })

        return result * 2
    }

    //let _r2 = computation 10

    let result = memoize.Get'(1, computation 1) |> Async.RunSynchronously

    Assert.Equal(10, result)


[<Fact>]
let ``Basics``() =

    let computation key = async {
        do! Async.Sleep 1
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
        |> Async.Parallel
        |> Async.RunSynchronously

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

        let computation key = async {
            jobStarted.Set() |> ignore
            do! Async.Sleep 1000
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

        let _task1 = Async.StartAsTask( memoize.Get'(key, computation key), cancellationToken = cts1.Token)
        let _task2 = Async.StartAsTask( memoize.Get'(key, computation key), cancellationToken = cts2.Token)
        let _task3 = Async.StartAsTask( memoize.Get'(key, computation key), cancellationToken = cts3.Token)

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

        let computation key = async {
            jobStarted.Set() |> ignore

            for _ in 1 .. 5 do
                do! Async.Sleep 100

            return key * 2
        }

        let eventLog = ConcurrentBag()
        let memoize = AsyncMemoize<int, int, int>()
        memoize.OnEvent(fun (e, (_, k, _version)) -> eventLog.Add (DateTime.Now.Ticks, (e, k)))

        use cts1 = new CancellationTokenSource()
        use cts2 = new CancellationTokenSource()
        use cts3 = new CancellationTokenSource()

        let key = 1

        let _task1 = Async.StartAsTask( memoize.Get'(key, computation key), cancellationToken = cts1.Token)
        let _task2 = Async.StartAsTask( memoize.Get'(key, computation key), cancellationToken = cts2.Token)
        let _task3 = Async.StartAsTask( memoize.Get'(key, computation key), cancellationToken = cts3.Token)


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

        let computation key = async {
            jobStarted.Set() |> ignore

            for _ in 1 .. 5 do
                do! Async.Sleep 100

            return key * 2
        }

        let eventLog = ConcurrentBag()
        let memoize = AsyncMemoize<int, int, int>()
        memoize.OnEvent(fun (e, (_label, k, _version)) -> eventLog.Add (DateTime.Now.Ticks, (e, k)))

        use cts1 = new CancellationTokenSource()
        use cts2 = new CancellationTokenSource()
        use cts3 = new CancellationTokenSource()

        let key = 1

        let _task1 = Async.StartAsTask( memoize.Get'(key, computation key), cancellationToken = cts1.Token)

        jobStarted.WaitOne() |> ignore
        jobStarted.Reset() |> ignore
        let _task2 = Async.StartAsTask( memoize.Get'(key, computation key), cancellationToken = cts2.Token)
        let _task3 = Async.StartAsTask( memoize.Get'(key, computation key), cancellationToken = cts3.Token)

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

    let testTimeoutMs = threads * iterations * maxDuration / 2

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
                        let job = cache.Get'(key, computation durationMs result)
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

    let cache = AsyncMemoize(cancelDuplicateRunningJobs=false)

    let mutable started = 0
    let mutable cancelled = 0

    let work () = async {
        Interlocked.Increment &started |> ignore
        let! ct = Async.CancellationToken
        use _ = ct.Register(fun () -> Interlocked.Increment &cancelled |> ignore)
        for _ in 1..10 do
            do! Async.Sleep 30
    }

    let key1 =
        { new ICacheKey<_, _> with
              member _.GetKey() = 1
              member _.GetVersion() = 1
              member _.GetLabel() = "key1" }

    cache.Get(key1, work()) |> Async.Start

    let key2 =
        { new ICacheKey<_, _> with
              member _.GetKey() = key1.GetKey()
              member _.GetVersion() = key1.GetVersion() + 1
              member _.GetLabel() = "key2" }

    cache.Get(key2, work()) |> Async.Start

    Async.Sleep 100 |> Async.RunSynchronously

    Assert.Equal((2, 1), (started, cancelled))