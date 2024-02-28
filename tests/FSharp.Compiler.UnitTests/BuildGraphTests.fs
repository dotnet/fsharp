﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.UnitTests

open System
open System.Threading
open System.Threading.Tasks
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
        GraphNode(async { 
            Assert.shouldBeTrue (o <> null)
            return 1 
        }), WeakReference(o)

    [<Fact>]
    let ``Intialization of graph node should not have a computed value``() =
        let node = GraphNode(async { return 1 })
        Assert.shouldBeTrue(node.TryPeekValue().IsNone)
        Assert.shouldBeFalse(node.HasValue)

    [<Fact>]
    let ``Two requests to get a value asynchronously should be successful``() =
        let resetEvent = new ManualResetEvent(false)
        let resetEventInAsync = new ManualResetEvent(false)

        let graphNode = 
            GraphNode(async { 
                resetEventInAsync.Set() |> ignore
                let! _ = Async.AwaitWaitHandle(resetEvent)
                return 1 
            })

        let task1 =
            async {
                let! _ = graphNode.GetOrComputeValue()
                ()
            } |> Async.StartAsTask

        let task2 =
            async {
                let! _ = graphNode.GetOrComputeValue()
                ()
            } |> Async.StartAsTask

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
            GraphNode(async { 
                computationCount <- computationCount + 1
                return 1 
            })

        let work = Async.Parallel(Array.init requests (fun _ -> graphNode.GetOrComputeValue()))

        Async.RunSynchronously(work)
        |> ignore

        Assert.shouldBe 1 computationCount

    [<Fact>]
    let ``Many requests to get a value asynchronously should get the correct value``() =
        let requests = 10000

        let graphNode = GraphNode(async { return 1 })

        let work = Async.Parallel(Array.init requests (fun _ -> graphNode.GetOrComputeValue()))

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

        Async.RunSynchronously(graphNode.GetOrComputeValue())
        |> ignore

        GC.Collect(2, GCCollectionMode.Forced, true)

        Assert.shouldBeFalse weak.IsAlive

    [<Fact>]
    let ``Many requests to get a value asynchronously should have its computation cleaned up by the GC``() =
        let requests = 10000

        let graphNode, weak = createNode ()

        GC.Collect(2, GCCollectionMode.Forced, true)
        
        Assert.shouldBeTrue weak.IsAlive

        Async.RunSynchronously(Async.Parallel(Array.init requests (fun _ -> graphNode.GetOrComputeValue())))
        |> ignore

        GC.Collect(2, GCCollectionMode.Forced, true)

        Assert.shouldBeFalse weak.IsAlive

    // [<Fact>]
    let ``A request can cancel``() =
        let graphNode = 
            GraphNode(async { 
                return 1 
            })

        use cts = new CancellationTokenSource()

        cts.Cancel()

        let work(): Task = Async.StartAsTask(
            async {
                return! graphNode.GetOrComputeValue()
            }, cancellationToken = cts.Token)

        Assert.ThrowsAnyAsync<OperationCanceledException>(work).Wait(TimeSpan.FromSeconds 10)

    // [<Fact>]
    let ``A request can cancel 2``() =
        let resetEvent = new ManualResetEvent(false)

        let graphNode = 
            GraphNode(async { 
                let! _ = Async.AwaitWaitHandle(resetEvent)
                failwith "Should have canceled" 
            })

        use cts = new CancellationTokenSource()

        Assert.ThrowsAnyAsync<OperationCanceledException>(fun () ->
            Async.StartImmediateAsTask(graphNode.GetOrComputeValue(), cancellationToken = cts.Token)      
        ) |> ignore

        cts.Cancel()
        resetEvent.Set() |> ignore

    [<Fact>]
    let ``Many requests to get a value asynchronously might evaluate the computation more than once even when some requests get canceled``() =
        let requests = 10000
        let resetEvent = new ManualResetEvent(false)
        let mutable computationCountBeforeSleep = 0
        let mutable computationCount = 0

        let graphNode = 
            GraphNode(async { 
                computationCountBeforeSleep <- computationCountBeforeSleep + 1
                let! _ = Async.AwaitWaitHandle(resetEvent)
                computationCount <- computationCount + 1
                return 1 
            })

        use cts = new CancellationTokenSource()

        let work = 
            async { 
                let! _ = graphNode.GetOrComputeValue()
                ()
            }

        let tasks = ResizeArray()

        for i = 0 to requests - 1 do
            if i % 10 = 0 then
                Async.StartAsTask(work, cancellationToken = cts.Token)
                |> tasks.Add
            else
                Async.StartAsTask(work)
                |> tasks.Add

        cts.Cancel()
        resetEvent.Set() |> ignore
        Async.RunSynchronously(work)
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
    
        let job phase i = async {
            do! random 10 |> Async.Sleep
            Assert.Equal(phase, DiagnosticsThreadStatics.BuildPhase)

            errorR (ExampleException $"job {i}")
        }
    
        let work (phase: BuildPhase) =
            async {
                let n = 8
                let logger = CapturingDiagnosticsLogger("test NodeCode")
                use _ = new CompilationGlobalsScope(logger, phase)
                let! _ = Seq.init n (job phase) |> MultipleDiagnosticsLoggers.Parallel

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
        |> Seq.map work
        |> Async.Parallel
        |> Async.RunSynchronously
