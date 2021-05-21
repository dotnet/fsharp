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

module AsyncLazyTests =
    
    [<MethodImpl(MethodImplOptions.NoInlining)>]
    let private createLazyWork () =
        let o = obj ()
        AsyncLazy(async { 
            Assert.shouldBeTrue (o <> null)
            return 1 
        }), WeakReference(o)

    [<Fact>]
    let ``Intialization of async lazy should not have a computed value``() =
        let lazyWork = AsyncLazy(async { return 1 })
        Assert.shouldBeTrue(lazyWork.TryGetValue().IsNone)

    [<Fact>]
    let ``Intialization of async lazy should have a request count of zero``() =
        let lazyWork = AsyncLazy(async { return 1 })
        Assert.shouldBe 0 lazyWork.RequestCount

    [<Fact>]
    let ``A request to get a value asynchronously should increase the request count by 1``() =
        let resetEvent = new ManualResetEvent(false)
        let resetEventInAsync = new ManualResetEvent(false)

        let lazyWork = 
            AsyncLazy(async { 
                resetEventInAsync.Set() |> ignore
                let! _ = Async.AwaitWaitHandle(resetEvent)
                return 1 
            })

        async {
            let! _ = lazyWork.GetValueAsync()
            ()
        } |> Async.Start

        resetEventInAsync.WaitOne() |> ignore
        Assert.shouldBe 1 lazyWork.RequestCount
        resetEvent.Set()

    [<Fact>]
    let ``Two requests to get a value asynchronously should increase the request count by 2``() =
        let resetEvent = new ManualResetEvent(false)
        let resetEventInAsync = new ManualResetEvent(false)

        let lazyWork = 
            AsyncLazy(async { 
                resetEventInAsync.Set() |> ignore
                let! _ = Async.AwaitWaitHandle(resetEvent)
                return 1 
            })

        async {
            let! _ = lazyWork.GetValueAsync()
            ()
        } |> Async.Start

        async {
            let! _ = lazyWork.GetValueAsync()
            ()
        } |> Async.Start

        resetEventInAsync.WaitOne() |> ignore
        Thread.Sleep(100) // Give it just enough time so that two requests are waiting
        Assert.shouldBe 2 lazyWork.RequestCount
        resetEvent.Set()

    [<Fact>]
    let ``Many requests to get a value asynchronously should only evaluate the computation once``() =
        let requests = 10000
        let mutable computationCount = 0

        let lazyWork = 
            AsyncLazy(async { 
                computationCount <- computationCount + 1
                return 1 
            })

        let work = Async.Parallel(Array.init requests (fun _ -> lazyWork.GetValueAsync()))

        Async.RunSynchronously(work)
        |> ignore

        Assert.shouldBe 1 computationCount

    [<Fact>]
    let ``Many requests to get a value asynchronously should get the correct value``() =
        let requests = 10000

        let lazyWork = AsyncLazy(async { return 1 })

        let work = Async.Parallel(Array.init requests (fun _ -> lazyWork.GetValueAsync()))

        let result = Async.RunSynchronously(work)

        Assert.shouldNotBeEmpty result
        Assert.shouldBe requests result.Length
        result
        |> Seq.iter (Assert.shouldBe 1)

    [<Fact>]
    let ``A request to get a value asynchronously should have its computation cleaned up by the GC``() =
        let lazyWork, weak = createLazyWork ()

        GC.Collect(2, GCCollectionMode.Forced, true)

        Assert.shouldBeTrue weak.IsAlive

        Async.RunSynchronously(lazyWork.GetValueAsync())
        |> ignore

        GC.Collect(2, GCCollectionMode.Forced, true)

        Assert.shouldBeFalse weak.IsAlive

    [<Fact>]
    let ``Many requests to get a value asynchronously should have its computation cleaned up by the GC``() =
        let requests = 10000

        let lazyWork, weak = createLazyWork ()

        GC.Collect(2, GCCollectionMode.Forced, true)
        
        Assert.shouldBeTrue weak.IsAlive

        Async.RunSynchronously(Async.Parallel(Array.init requests (fun _ -> lazyWork.GetValueAsync())))
        |> ignore

        GC.Collect(2, GCCollectionMode.Forced, true)

        Assert.shouldBeFalse weak.IsAlive

    [<Fact>]
    let ``A request can cancel``() =
        let resetEvent = new ManualResetEvent(false)

        let lazyWork = 
            AsyncLazy(async { 
                let! _ = Async.AwaitWaitHandle(resetEvent)
                return 1 
            })

        use cts = new CancellationTokenSource()

        async {
            do! Async.Sleep(100) // Some buffer time
            cts.Cancel()
            resetEvent.Set() |> ignore
        }
        |> Async.Start

        let ex =
            try
                Async.RunSynchronously(lazyWork.GetValueAsync(), cancellationToken = cts.Token)
                |> ignore
                failwith "Should have canceled"
            with
            | :? OperationCanceledException as ex ->
                ex

        Assert.shouldBeTrue(ex <> null)

    [<Fact>]
    let ``Many requests to get a value asynchronously should only evaluate the computation once even when some requests get canceled``() =
        let requests = 10000
        let resetEvent = new ManualResetEvent(false)
        let mutable computationCountBeforeSleep = 0
        let mutable computationCount = 0

        let lazyWork = 
            AsyncLazy(async { 
                computationCountBeforeSleep <- computationCountBeforeSleep + 1
                let! _ = Async.AwaitWaitHandle(resetEvent)
                computationCount <- computationCount + 1
                return 1 
            })

        use cts = new CancellationTokenSource()

        let work = 
            async { 
                let! _ = lazyWork.GetValueAsync()
                ()
            }

        for i = 0 to requests - 1 do
            if i % 10 = 0 then
                Async.Start(work, cancellationToken = cts.Token)
            else
                Async.Start(work)

        Thread.Sleep(100) // Buffer some time
        cts.Cancel()
        resetEvent.Set() |> ignore
        Async.RunSynchronously(work)
        |> ignore

        Assert.shouldBeTrue cts.IsCancellationRequested
        Assert.shouldBe 1 computationCountBeforeSleep
        Assert.shouldBe 1 computationCount
