// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.Service.Tests

open System
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices
open Xunit
open FSharp.Test
open FSharp.Compiler.BuildGraph
open FSharp.Compiler.DiagnosticsLogger
open Internal.Utilities.Library
open FSharp.Compiler.Diagnostics

module BuildGraphTests =
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let private createNode () =
        let o = obj ()
        GraphNode(async2 { 
            Assert.shouldBeTrue (o <> null)
            return 1 
        }), WeakReference(o)

    [<Fact>]
    let ``Initialization of graph node should not have a computed value``() =
        let node = GraphNode(async2 { return 1 })
        Assert.shouldBeTrue(node.TryPeekValue().IsNone)
        Assert.shouldBeFalse(node.HasValue)

    [<Fact>]
    let ``Two requests to get a value asynchronously should be successful``() =
        let resetEvent = new ManualResetEvent(false)
        let resetEventInAsync = new ManualResetEvent(false)

        let graphNode = 
            GraphNode(async2 { 
                resetEventInAsync.Set() |> ignore
                let! _ = Async2.AwaitWaitHandle(resetEvent)
                return 1 
            })

        let task1 =
            async2 {
                let! _ = graphNode.GetOrComputeValue()
                ()
            } |> Async2.StartAsTask

        let task2 =
            async2 {
                let! _ = graphNode.GetOrComputeValue()
                ()
            } |> Async2.StartAsTask

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
            GraphNode(async2 { 
                computationCount <- computationCount + 1
                return 1 
            })

        let work = Async2.Parallel(Array.init requests (fun _ -> graphNode.GetOrComputeValue() ))

        Async2.RunImmediate(work)
        |> ignore

        Assert.shouldBe 1 computationCount

    [<Fact>]
    let ``Many requests to get a value asynchronously should get the correct value``() =
        let requests = 10000

        let graphNode = GraphNode(async2 { return 1 })

        let work = Async2.Parallel(Array.init requests (fun _ -> graphNode.GetOrComputeValue() ))

        let result = Async2.RunImmediate(work)

        Assert.shouldNotBeEmpty result
        Assert.shouldBe requests result.Length
        result
        |> Seq.iter (Assert.shouldBe 1)

    [<Fact>]
    let ``A request to get a value asynchronously should have its computation cleaned up by the GC``() =
        let graphNode, weak = createNode ()

        GC.Collect(2, GCCollectionMode.Forced, true)

        Assert.shouldBeTrue weak.IsAlive

        Async2.RunImmediate(graphNode.GetOrComputeValue())
        |> ignore

        GC.Collect(2, GCCollectionMode.Forced, true)

        Assert.shouldBeFalse weak.IsAlive

    [<Fact>]
    let ``Many requests to get a value asynchronously should have its computation cleaned up by the GC``() =
        let requests = 10000

        let graphNode, weak = createNode ()

        GC.Collect(2, GCCollectionMode.Forced, true)
        
        Assert.shouldBeTrue weak.IsAlive

        Async2.RunImmediate(Async2.Parallel(Array.init requests (fun _ -> graphNode.GetOrComputeValue() )))
        |> ignore

        GC.Collect(2, GCCollectionMode.Forced, true)

        Assert.shouldBeFalse weak.IsAlive

    [<Fact>]
    let ``A request can cancel``() =
        let graphNode = 
            GraphNode(async2 { 
                return 1 
            })

        use cts = new CancellationTokenSource()

        let work =
            async2 {
                cts.Cancel()
                return! graphNode.GetOrComputeValue()
            }

        let ex =
            try
                Async2.RunImmediate(work, cancellationToken = cts.Token)
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
            GraphNode(async2 { 
                let! _ = Async2.AwaitWaitHandle(resetEvent)
                return 1 
            })

        use cts = new CancellationTokenSource()

        let task =
            async2 {
                cts.Cancel()
                resetEvent.Set() |> ignore
            }
            |> Async2.StartAsTask

        let ex =
            try
                Async2.RunImmediate(graphNode.GetOrComputeValue(), cancellationToken = cts.Token)
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
            GraphNode(async2 { 
                computationCountBeforeSleep <- computationCountBeforeSleep + 1
                let! _ = Async2.AwaitWaitHandle(resetEvent)
                computationCount <- computationCount + 1
                return 1 
            })

        use cts = new CancellationTokenSource()

        let work = 
            async2 { 
                let! _ = graphNode.GetOrComputeValue()
                ()
            }

        let tasks = ResizeArray()

        for i = 0 to requests - 1 do
            if i % 10 = 0 then
                Async2.StartAsTask(work, cancellationToken = cts.Token)
                |> tasks.Add
            else
                Async2.StartAsTask(work)
                |> tasks.Add

        cts.Cancel()
        resetEvent.Set() |> ignore
        Async2.RunImmediate(work)
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
    let internal ``DiagnosticsThreadStatics preserved in async`` () =
        let random =
            let rng = Random()
            fun n -> rng.Next n
    
        let job phase i = async2 {
            do! random 10 |> Task.Delay
            Assert.Equal(phase, DiagnosticsThreadStatics.BuildPhase)
            DiagnosticsThreadStatics.DiagnosticsLogger.DebugDisplay()
            |> Assert.shouldBe $"DiagnosticsLogger(CaptureDiagnosticsConcurrently {i})"

            errorR (ExampleException $"job {i}")
        }
    
        let work (phase: BuildPhase) =
            async2 {
                let n = 8
                let logger = CapturingDiagnosticsLogger("test NodeCode")
                use _ = new CompilationGlobalsScope(logger, phase)
                let! _ = Seq.init n (job phase) |> MultipleDiagnosticsLoggers.Parallel

                let diags = logger.Diagnostics |> List.map fst

                diags |> List.map _.Phase |> List.distinct |> Assert.shouldBe [ phase ]
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
        |> Async2.Parallel
        |> Async2.RunImmediate

    exception TestException

    type internal SimpleConcurrentLogger(name) =
        inherit DiagnosticsLogger(name)

        let mutable errorCount = 0

        override _.DiagnosticSink(d, s) =
            if s = FSharpDiagnosticSeverity.Error then Interlocked.Increment(&errorCount) |> ignore

        override this.ErrorCount = errorCount

    let loggerShouldBe logger =
        DiagnosticsThreadStatics.DiagnosticsLogger |> Assert.shouldBe logger

    let errorCountShouldBe ec =
        DiagnosticsThreadStatics.DiagnosticsLogger.ErrorCount |> Assert.shouldBe ec

    [<Fact>]
    let ``AsyncLocal diagnostics context works with TPL`` () =

        let task1 () = 
            List.init 20 (sprintf "ListParallel logger %d")
            |> Extras.ListParallel.map (fun name -> 
                let logger = CapturingDiagnosticsLogger(name)
                use _ = UseDiagnosticsLogger logger
                for _ in 1 .. 10 do
                    errorR TestException
                    Thread.Sleep 5
                errorCountShouldBe 10
                loggerShouldBe logger )
            |> ignore

        let task2 () =         
            let commonLogger = SimpleConcurrentLogger "ListParallel concurrent logger"
            use _ = UseDiagnosticsLogger commonLogger

            [1 .. 20]
            |> Extras.ListParallel.map (fun _ -> 
                for _ in 1 .. 10 do
                    errorR TestException
                    Thread.Sleep 5
                loggerShouldBe commonLogger )
            |> ignore
            errorCountShouldBe 200
            loggerShouldBe commonLogger

        Parallel.Invoke(task1, task2)


    type internal DiagnosticsLoggerWithCallback(callback) =
        inherit CapturingDiagnosticsLogger("test")
        override _.DiagnosticSink(e, s) =
            base.DiagnosticSink(e, s)
            callback e.Exception.Message |> ignore

    [<Fact>]
    let ``MultipleDiagnosticsLoggers capture diagnostics in correct order`` () =

        let mutable prevError = "000."

        let errorCommitted msg =
            // errors come in correct order
            Assert.shouldBeTrue (msg > prevError)
            prevError <- msg

        let work i = async2 {
            for c in 'A' .. 'F' do
                do! Async.SwitchToThreadPool()
                errorR (ExampleException $"%03d{i}{c}")
        }

        let tasks = Seq.init 100 work

        let logger = DiagnosticsLoggerWithCallback errorCommitted
        use _ = UseDiagnosticsLogger logger
        tasks |> Seq.take 50 |> MultipleDiagnosticsLoggers.Parallel |> Async2.Ignore |> Async2.RunImmediate

        // all errors committed
        errorCountShouldBe 300

        tasks |> Seq.skip 50 |> MultipleDiagnosticsLoggers.Sequential |> Async2.Ignore |> Async2.RunImmediate

        errorCountShouldBe 600

    [<Fact>]
    let ``MultipleDiagnosticsLoggers.Parallel finishes when any computation throws`` () =

        let mutable count = 0
        use _ = UseDiagnosticsLogger (CapturingDiagnosticsLogger "test logger")

        let tasks = [
            async2 { failwith "computation failed" }

            for i in 1 .. 300 do
                async2 {
                    errorR (ExampleException $"{Interlocked.Increment(&count)}")
                    error (ExampleException $"{Interlocked.Increment(&count)}")
                }
        ]

        task {
            do! tasks |> MultipleDiagnosticsLoggers.Parallel |> Async2.Catch |> Async2.Ignore |> Async2.StartAsTask

            // Diagnostics from all started tasks should be collected despite the exception.
            errorCountShouldBe count
        }

    [<Fact>]
    let ``AsyncLocal diagnostics context flows correctly`` () =

        let work logger = async2 {
            SetThreadDiagnosticsLoggerNoUnwind logger

            errorR TestException

            loggerShouldBe logger
            errorCountShouldBe 1

            do! Async.SwitchToNewThread()

            errorR TestException

            loggerShouldBe logger
            errorCountShouldBe 2

            do! Async.SwitchToThreadPool()

            errorR TestException

            loggerShouldBe logger
            errorCountShouldBe 3

            let workInner = async2 {
                    do! async2 { }
                    errorR TestException
                    loggerShouldBe logger
                }

            let! child = workInner |> Async2.StartChild
            let! childTask = workInner |> Async2.StartChildAsTask

            do! child
            do! childTask
            errorCountShouldBe 5
        }

        let init n =
            let name = $"AsyncLocal test {n}"
            let logger = SimpleConcurrentLogger name
            work logger

        Seq.init 10 init |> Async2.Parallel |> Async2.runWithoutCancellation |> ignore

        let logger = SimpleConcurrentLogger "main"
        use _ =  UseDiagnosticsLogger logger

        errorCountShouldBe 0

        let btask = backgroundTask {
            errorR TestException
            do! Task.Yield()
            errorR TestException
            loggerShouldBe logger
        }

        let noErrorsTask = backgroundTask {
            SetThreadDiagnosticsLoggerNoUnwind DiscardErrorsLogger
            errorR TestException
            do! Task.Yield()
            errorR TestException
            loggerShouldBe DiscardErrorsLogger
        }

        let task = task {
            errorR TestException
            do! Task.Yield()
            errorR TestException
            loggerShouldBe logger
        }

        // A thread with inner logger.
        let thread = Thread(ThreadStart(fun () ->
            use _ = UseDiagnosticsLogger (CapturingDiagnosticsLogger("Thread logger"))
            errorR TestException
            errorR TestException
            errorCountShouldBe 2
            ))
        thread.Start()
        thread.Join()

        loggerShouldBe logger

        // Ambient logger flows into this thread.
        let thread = Thread(ThreadStart(fun () ->
            errorR TestException
            errorR TestException
            ))
        thread.Start()
        thread.Join()

        Task.WaitAll(noErrorsTask, btask, task)

        Seq.init 11 (fun _ -> async2 { errorR TestException; loggerShouldBe logger } ) |> Async2.Parallel |> Async2.RunImmediate |> ignore

        loggerShouldBe logger
        errorCountShouldBe 17

        async2 {
            do! 
                [ async2 {
                    SetThreadDiagnosticsLoggerNoUnwind DiscardErrorsLogger } ]
                |> Async2.Parallel
                |> Async2.Ignore

            loggerShouldBe logger
        }
        |> Async2.RunImmediate

        SetThreadDiagnosticsLoggerNoUnwind logger
        // This runs in async continuation, so the context is forked.
        async2 {
            do! Async.Sleep 0
            SetThreadDiagnosticsLoggerNoUnwind DiscardErrorsLogger
            do! Async.SwitchToNewThread()
            loggerShouldBe DiscardErrorsLogger
        }
        |> Async2.RunImmediate
        loggerShouldBe logger







