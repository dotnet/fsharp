﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.UnitTests

open System
open System.Threading
open System.Runtime.CompilerServices
open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Compiler.BuildGraph
open FSharp.Compiler.DiagnosticsLogger
open Internal.Utilities.Library

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
        Assert.shouldBeTrue(node.TryPeekValue().IsNone)
        Assert.shouldBeFalse(node.HasValue)

    [<Fact>]
    let ``Two requests to get a value asynchronously should be successful``() =
        let resetEvent = new ManualResetEvent(false)
        let resetEventInAsync = new ManualResetEvent(false)

        let graphNode = 
            GraphNode(node { 
                resetEventInAsync.Set() |> ignore
                let! _ = NodeCode.AwaitWaitHandle_ForTesting(resetEvent)
                return 1 
            })

        let task1 =
            node {
                let! _ = graphNode.GetOrComputeValue()
                ()
            } |> NodeCode.StartAsTask_ForTesting

        let task2 =
            node {
                let! _ = graphNode.GetOrComputeValue()
                ()
            } |> NodeCode.StartAsTask_ForTesting

        resetEventInAsync.WaitOne() |> ignore
        resetEvent.Set() |> ignore
        try
            task1.Wait(1000) |> ignore
            task2.Wait() |> ignore
        with
        | :? TimeoutException -> reraise()
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

        let work = Async.Parallel(Array.init requests (fun _ -> graphNode.GetOrComputeValue() |> Async.AwaitNodeCode))

        Async.RunImmediate(work)
        |> ignore

        Assert.shouldBe 1 computationCount

    [<Fact>]
    let ``Many requests to get a value asynchronously should get the correct value``() =
        let requests = 10000

        let graphNode = GraphNode(node { return 1 })

        let work = Async.Parallel(Array.init requests (fun _ -> graphNode.GetOrComputeValue() |> Async.AwaitNodeCode))

        let result = Async.RunImmediate(work)

        Assert.shouldNotBeEmpty result
        Assert.shouldBe requests result.Length
        result
        |> Seq.iter (Assert.shouldBe 1)

    [<Fact>]
    let ``A request to get a value asynchronously should have its computation cleaned up by the GC``() =
        let graphNode, weak = createNode ()

        GC.Collect(2, GCCollectionMode.Forced, true)

        Assert.shouldBeTrue weak.IsAlive

        NodeCode.RunImmediateWithoutCancellation(graphNode.GetOrComputeValue())
        |> ignore

        GC.Collect(2, GCCollectionMode.Forced, true)

        Assert.shouldBeFalse weak.IsAlive

    [<Fact>]
    let ``Many requests to get a value asynchronously should have its computation cleaned up by the GC``() =
        let requests = 10000

        let graphNode, weak = createNode ()

        GC.Collect(2, GCCollectionMode.Forced, true)
        
        Assert.shouldBeTrue weak.IsAlive

        Async.RunImmediate(Async.Parallel(Array.init requests (fun _ -> graphNode.GetOrComputeValue() |> Async.AwaitNodeCode)))
        |> ignore

        GC.Collect(2, GCCollectionMode.Forced, true)

        Assert.shouldBeFalse weak.IsAlive

    [<Fact>]
    let ``A request can cancel``() =
        let graphNode = 
            GraphNode(node { 
                return 1 
            })

        use cts = new CancellationTokenSource()

        let work =
            node {
                cts.Cancel()
                return! graphNode.GetOrComputeValue()
            }

        let ex =
            try
                NodeCode.RunImmediate(work, ct = cts.Token)
                |> ignore
                failwith "Should have canceled"
            with
            | :? OperationCanceledException as ex ->
                ex

        Assert.shouldBeTrue(ex <> null)

    [<Fact>]
    let ``A request can cancel 2``() =
        let resetEvent = new ManualResetEvent(false)

        let graphNode = 
            GraphNode(node { 
                let! _ = NodeCode.AwaitWaitHandle_ForTesting(resetEvent)
                return 1 
            })

        use cts = new CancellationTokenSource()

        let task =
            node {
                cts.Cancel()
                resetEvent.Set() |> ignore
            }
            |> NodeCode.StartAsTask_ForTesting

        let ex =
            try
                NodeCode.RunImmediate(graphNode.GetOrComputeValue(), ct = cts.Token)
                |> ignore
                failwith "Should have canceled"
            with
            | :? OperationCanceledException as ex ->
                ex

        Assert.shouldBeTrue(ex <> null)
        try task.Wait(1000) |> ignore with | :? TimeoutException -> reraise() | _ -> ()

    [<Fact>]
    let ``Many requests to get a value asynchronously might evaluate the computation more than once even when some requests get canceled``() =
        let requests = 10000
        let resetEvent = new ManualResetEvent(false)
        let mutable computationCountBeforeSleep = 0
        let mutable computationCount = 0

        let graphNode = 
            GraphNode(node { 
                computationCountBeforeSleep <- computationCountBeforeSleep + 1
                let! _ = NodeCode.AwaitWaitHandle_ForTesting(resetEvent)
                computationCount <- computationCount + 1
                return 1 
            })

        use cts = new CancellationTokenSource()

        let work = 
            node { 
                let! _ = graphNode.GetOrComputeValue()
                ()
            }

        let tasks = ResizeArray()

        for i = 0 to requests - 1 do
            if i % 10 = 0 then
                NodeCode.StartAsTask_ForTesting(work, ct = cts.Token)
                |> tasks.Add
            else
                NodeCode.StartAsTask_ForTesting(work)
                |> tasks.Add

        cts.Cancel()
        resetEvent.Set() |> ignore
        NodeCode.RunImmediateWithoutCancellation(work)
        |> ignore

        Assert.shouldBeTrue cts.IsCancellationRequested
        Assert.shouldBeTrue(computationCountBeforeSleep > 0)
        Assert.shouldBeTrue(computationCount >= 0)

        tasks
        |> Seq.iter (fun x -> 
            try x.Wait(1000) |> ignore with | :? TimeoutException -> reraise() | _ -> ())

    [<Fact>]
    let ``GraphNode created from an already computed result will return it in tryPeekValue`` () =
        let graphNode = GraphNode.FromResult 1

        Assert.shouldBeTrue graphNode.HasValue
        Assert.shouldBe (ValueSome 1) (graphNode.TryPeekValue())

    type ExampleException(msg) = inherit System.Exception(msg)

    [<Fact>]
    let internal ``NodeCode preserves DiagnosticsThreadStatics`` () =
        let random =
            let rng = Random()
            fun n -> rng.Next n
    
        let job phase i = node {
            do! random 10 |> Async.Sleep |> NodeCode.AwaitAsync
            Assert.Equal(phase, DiagnosticsThreadStatics.BuildPhase)
            DiagnosticsThreadStatics.DiagnosticsLogger.DebugDisplay()
            |> Assert.shouldBe $"DiagnosticsLogger(NodeCode.Parallel {i})"

            errorR (ExampleException $"job {i}")
        }
    
        let work (phase: BuildPhase) =
            node {
                let n = 8
                let logger = CapturingDiagnosticsLogger("test NodeCode")
                use _ = new CompilationGlobalsScope(logger, phase)
                let! _ = Seq.init n (job phase) |> NodeCode.Parallel

                let diags = logger.Diagnostics |> List.map fst

                diags |> List.map _.Phase |> Set |> Assert.shouldBe (Set.singleton phase)
                diags |> List.map _.Exception.Message
                |> Assert.shouldBe (List.init n <| sprintf "job %d")

                Assert.Equal(phase, DiagnosticsThreadStatics.BuildPhase)
            }
    
        let phases = [|
            BuildPhase.DefaultPhase
            BuildPhase.Compile
            BuildPhase.Parameter
            BuildPhase.Parse
            BuildPhase.TypeCheck
            BuildPhase.CodeGen
            BuildPhase.Optimize
            BuildPhase.IlxGen
            BuildPhase.IlGen
            BuildPhase.Output
            BuildPhase.Interactive
        |]
    
        let pickRandomPhase _ = phases[random phases.Length]
        Seq.init 100 pickRandomPhase
        |> Seq.map (work >> Async.AwaitNodeCode)
        |> Async.Parallel
        |> Async.RunSynchronously
