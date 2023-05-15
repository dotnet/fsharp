module FSharp.Compiler.ComponentTests.CompilerService.AsyncMemoize

open System
open System.Threading
open Xunit
open FSharp.Test
open FSharp.Compiler.BuildGraph
open Internal.Utilities.Collections
open System.Threading.Tasks

[<Fact>]
let ``Basics``() =

    let computation key = node {
        do! Async.Sleep 1 |> NodeCode.AwaitAsync
        return key * 2
    }

    let eventLog = ResizeArray()

    let memoize = AsyncMemoize(eventLog.Add)

    let task =
        NodeCode.Parallel(seq {
            memoize.Get(5, computation)
            memoize.Get(5, computation)
            memoize.Get(2, computation)
            memoize.Get(5, computation)
            memoize.Get(3, computation)
            memoize.Get(2, computation)
        }) |> NodeCode.StartAsTask_ForTesting

    let result = task.Result
    let expected = [| 10; 10; 4; 10; 6; 4|]

    Assert.Equal<int array>(expected, result)

    let groups = eventLog |> Seq.groupBy snd |> Seq.toList
    Assert.Equal(3, groups.Length)
    for key, events in groups do
        Assert.Equal<(JobEventType * int) array>([| Started, key; Finished, key |], events |> Seq.toArray)

[<Fact>]
let ``We can cancel a job`` () =

    let resetEvent = new ManualResetEvent(false)

    let computation key = node {
        resetEvent.Set() |> ignore
        do! Async.Sleep 1000 |> NodeCode.AwaitAsync
        failwith "Should be canceled before it gets here"
        return key * 2
    }

    let eventLog = ResizeArray()
    let memoize = AsyncMemoize(eventLog.Add)

    use cts1 = new CancellationTokenSource()
    use cts2 = new CancellationTokenSource()
    use cts3 = new CancellationTokenSource()

    let key = 1

    let _task1 = NodeCode.StartAsTask_ForTesting(memoize.Get(key, computation), cts1.Token)
    let _task2 = NodeCode.StartAsTask_ForTesting(memoize.Get(key, computation), cts2.Token)
    let _task3 = NodeCode.StartAsTask_ForTesting(memoize.Get(key, computation), cts3.Token)

    resetEvent.WaitOne() |> ignore

    Assert.Equal<(JobEventType * int) array>([| Started, key |], eventLog |> Seq.toArray )

    cts1.Cancel()
    cts2.Cancel()

    Thread.Sleep 10

    Assert.Equal<(JobEventType * int) array>([| Started, key |], eventLog |> Seq.toArray )

    cts3.Cancel()

    Thread.Sleep 100

    Assert.Equal<(JobEventType * int) array>([| Started, key; Canceled, key |], eventLog |> Seq.toArray )

    try
        Task.WaitAll(_task1, _task2, _task3)
    with :? AggregateException as ex ->
        Assert.Equal(3, ex.InnerExceptions.Count)
        Assert.True(ex.InnerExceptions |> Seq.forall (fun e -> e :? TaskCanceledException))

[<Fact>]
let ``Job keeps running even if first requestor cancels`` () =
    let computation key = node {
        do! Async.Sleep 100 |> NodeCode.AwaitAsync
        return key * 2
    }

    let eventLog = ResizeArray()
    let memoize = AsyncMemoize(eventLog.Add)

    use cts1 = new CancellationTokenSource()
    use cts2 = new CancellationTokenSource()
    use cts3 = new CancellationTokenSource()

    let key = 1

    let _task1 = NodeCode.StartAsTask_ForTesting(memoize.Get(key, computation), cts1.Token)
    let _task2 = NodeCode.StartAsTask_ForTesting(memoize.Get(key, computation), cts2.Token)
    let _task3 = NodeCode.StartAsTask_ForTesting(memoize.Get(key, computation), cts3.Token)

    Thread.Sleep 10

    cts1.Cancel()
    cts3.Cancel()

    let result = _task2.Result
    Assert.Equal(2, result)

    Thread.Sleep 1 // Wait for event log to be updated
    Assert.Equal<(JobEventType * int) array>([| Started, key; Finished, key |], eventLog |> Seq.toArray )

