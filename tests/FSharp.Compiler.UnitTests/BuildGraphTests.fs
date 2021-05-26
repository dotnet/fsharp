// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.UnitTests

open System
open System.Diagnostics
open System.Globalization
open System.Threading
open Xunit
open FSharp.Test.Utilities
open Internal.Utilities.Library
open System.Runtime.CompilerServices
open FSharp.Compiler.BuildGraph

module BuildGraphTests =
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let private createNode () =
        let o = obj ()
        GraphNode(node { 
            Assert.shouldBeTrue (o <> null)
            return 1 
        }), WeakReference(o)

    [<Fact>]
    let ``Intialization of graph node should not have a computed value``() =
        let node = GraphNode(node { return 1 })
        Assert.shouldBeTrue(node.TryGetValue().IsNone)

    [<Fact>]
    let ``Intialization of graph node should have a request count of zero``() =
        let node = GraphNode(node { return 1 })
        Assert.shouldBe 0 node.RequestCount

    [<Fact>]
    let ``A request to get a value asynchronously should increase the request count by 1``() =
        let resetEvent = new ManualResetEvent(false)
        let resetEventInAsync = new ManualResetEvent(false)

        let graphNode = 
            GraphNode(node { 
                resetEventInAsync.Set() |> ignore
                let! _ = NodeCode.AwaitWaitHandle(resetEvent)
                return 1 
            })

        let task =
            node {
                let! _ = graphNode.GetValue()
                ()
            } |> NodeCode.StartAsTask

        resetEventInAsync.WaitOne() |> ignore
        Assert.shouldBe 1 graphNode.RequestCount
        resetEvent.Set() |> ignore
        try task.Wait() with | _ -> ()

    [<Fact>]
    let ``Two requests to get a value asynchronously should increase the request count by 2``() =
        let resetEvent = new ManualResetEvent(false)
        let resetEventInAsync = new ManualResetEvent(false)

        let graphNode = 
            GraphNode(node { 
                resetEventInAsync.Set() |> ignore
                let! _ = NodeCode.AwaitWaitHandle(resetEvent)
                return 1 
            })

        let task1 =
            node {
                let! _ = graphNode.GetValue()
                ()
            } |> NodeCode.StartAsTask

        let task2 =
            node {
                let! _ = graphNode.GetValue()
                ()
            } |> NodeCode.StartAsTask

        resetEventInAsync.WaitOne() |> ignore
        Thread.Sleep(100) // Give it just enough time so that two requests are waiting
        Assert.shouldBe 2 graphNode.RequestCount
        resetEvent.Set() |> ignore
        try
            task1.Wait()
            task2.Wait()
        with
        | _ -> ()

    [<Fact>]
    let ``Many requests to get a value asynchronously should only evaluate the computation once``() =
        let requests = 10000
        let mutable computationCount = 0

        let graphNode = 
            GraphNode(node { 
                computationCount <- computationCount + 1
                return 1 
            })

        let work = Async.Parallel(Array.init requests (fun _ -> graphNode.GetValue() |> Async.AwaitNode))

        Async.RunSynchronously(work)
        |> ignore

        Assert.shouldBe 1 computationCount

    [<Fact>]
    let ``Many requests to get a value asynchronously should get the correct value``() =
        let requests = 10000

        let graphNode = GraphNode(node { return 1 })

        let work = Async.Parallel(Array.init requests (fun _ -> graphNode.GetValue() |> Async.AwaitNode))

        let result = Async.RunSynchronously(work)

        Assert.shouldNotBeEmpty result
        Assert.shouldBe requests result.Length
        result
        |> Seq.iter (Assert.shouldBe 1)

    [<Fact>]
    let ``A request to get a value asynchronously should have its computation cleaned up by the GC``() =
        let graphNode, weak = createNode ()

        GC.Collect(2, GCCollectionMode.Forced, true)

        Assert.shouldBeTrue weak.IsAlive

        NodeCode.RunImmediate(graphNode.GetValue())
        |> ignore

        GC.Collect(2, GCCollectionMode.Forced, true)

        Assert.shouldBeFalse weak.IsAlive

    [<Fact>]
    let ``Many requests to get a value asynchronously should have its computation cleaned up by the GC``() =
        let requests = 10000

        let graphNode, weak = createNode ()

        GC.Collect(2, GCCollectionMode.Forced, true)
        
        Assert.shouldBeTrue weak.IsAlive

        Async.RunSynchronously(Async.Parallel(Array.init requests (fun _ -> graphNode.GetValue() |> Async.AwaitNode)))
        |> ignore

        GC.Collect(2, GCCollectionMode.Forced, true)

        Assert.shouldBeFalse weak.IsAlive

    [<Fact>]
    let ``A request can cancel``() =
        let resetEvent = new ManualResetEvent(false)

        let graphNode = 
            GraphNode(node { 
                let! _ = NodeCode.AwaitWaitHandle(resetEvent)
                return 1 
            })

        use cts = new CancellationTokenSource()

        let task =
            async {
                do! Async.Sleep(100) // Some buffer time
                cts.Cancel()
                resetEvent.Set() |> ignore
            }
            |> Async.StartAsTask

        let ex =
            try
                Async.RunSynchronously(graphNode.GetValue() |> Async.AwaitNode, cancellationToken = cts.Token)
                |> ignore
                failwith "Should have canceled"
            with
            | :? OperationCanceledException as ex ->
                ex

        Assert.shouldBeTrue(ex <> null)
        try task.Wait() with | _ -> ()

    [<Fact>]
    let ``Many requests to get a value asynchronously should only evaluate the computation once even when some requests get canceled``() =
        let requests = 10000
        let resetEvent = new ManualResetEvent(false)
        let mutable computationCountBeforeSleep = 0
        let mutable computationCount = 0

        let graphNode = 
            GraphNode(node { 
                computationCountBeforeSleep <- computationCountBeforeSleep + 1
                let! _ = NodeCode.AwaitWaitHandle(resetEvent)
                computationCount <- computationCount + 1
                return 1 
            })

        use cts = new CancellationTokenSource()

        let work = 
            node { 
                let! _ = graphNode.GetValue()
                ()
            }

        let tasks = ResizeArray()

        for i = 0 to requests - 1 do
            if i % 10 = 0 then
                NodeCode.StartAsTask(work, ct = cts.Token)
                |> tasks.Add
            else
                NodeCode.StartAsTask(work)
                |> tasks.Add

        Thread.Sleep(100) // Buffer some time
        cts.Cancel()
        resetEvent.Set() |> ignore
        NodeCode.RunImmediate(work)
        |> ignore

        Assert.shouldBeTrue cts.IsCancellationRequested
        Assert.shouldBe 1 computationCountBeforeSleep
        Assert.shouldBe 1 computationCount

        tasks
        |> Seq.iter (fun x -> 
            try x.Wait() with | _ -> ())
