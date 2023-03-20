// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.BuildGraph

open System
open System.Threading
open System.Threading.Tasks
open System.Diagnostics
open System.Globalization
open FSharp.Compiler.DiagnosticsLogger
open Internal.Utilities.Library

[<NoEquality; NoComparison>]
type NodeCode<'T> = Node of Async<'T>

let wrapThreadStaticInfo computation =
    async {
        let diagnosticsLogger = DiagnosticsThreadStatics.DiagnosticsLogger
        let phase = DiagnosticsThreadStatics.BuildPhase

        try
            return! computation
        finally
            DiagnosticsThreadStatics.DiagnosticsLogger <- diagnosticsLogger
            DiagnosticsThreadStatics.BuildPhase <- phase
    }

type Async<'T> with

    static member AwaitNodeCode(node: NodeCode<'T>) =
        match node with
        | Node (computation) -> wrapThreadStaticInfo computation

[<Sealed>]
type NodeCodeBuilder() =

    static let zero = Node(async.Zero())

    [<DebuggerHidden; DebuggerStepThrough>]
    member _.Zero() : NodeCode<unit> = zero

    [<DebuggerHidden; DebuggerStepThrough>]
    member _.Delay(f: unit -> NodeCode<'T>) =
        Node(
            async.Delay(fun () ->
                match f () with
                | Node (p) -> p)
        )

    [<DebuggerHidden; DebuggerStepThrough>]
    member _.Return value = Node(async.Return(value))

    [<DebuggerHidden; DebuggerStepThrough>]
    member _.ReturnFrom(computation: NodeCode<_>) = computation

    [<DebuggerHidden; DebuggerStepThrough>]
    member _.Bind(Node (p): NodeCode<'a>, binder: 'a -> NodeCode<'b>) : NodeCode<'b> =
        Node(
            async.Bind(
                p,
                fun x ->
                    match binder x with
                    | Node p -> p
            )
        )

    [<DebuggerHidden; DebuggerStepThrough>]
    member _.TryWith(Node (p): NodeCode<'T>, binder: exn -> NodeCode<'T>) : NodeCode<'T> =
        Node(
            async.TryWith(
                p,
                fun ex ->
                    match binder ex with
                    | Node p -> p
            )
        )

    [<DebuggerHidden; DebuggerStepThrough>]
    member _.TryFinally(Node (p): NodeCode<'T>, binder: unit -> unit) : NodeCode<'T> = Node(async.TryFinally(p, binder))

    [<DebuggerHidden; DebuggerStepThrough>]
    member _.For(xs: 'T seq, binder: 'T -> NodeCode<unit>) : NodeCode<unit> =
        Node(
            async.For(
                xs,
                fun x ->
                    match binder x with
                    | Node p -> p
            )
        )

    [<DebuggerHidden; DebuggerStepThrough>]
    member _.Combine(Node (p1): NodeCode<unit>, Node (p2): NodeCode<'T>) : NodeCode<'T> = Node(async.Combine(p1, p2))

    [<DebuggerHidden; DebuggerStepThrough>]
    member _.Using(value: CompilationGlobalsScope, binder: CompilationGlobalsScope -> NodeCode<'U>) =
        Node(
            async {
                DiagnosticsThreadStatics.DiagnosticsLogger <- value.DiagnosticsLogger
                DiagnosticsThreadStatics.BuildPhase <- value.BuildPhase

                try
                    return! binder value |> Async.AwaitNodeCode
                finally
                    (value :> IDisposable).Dispose()
            }
        )
    
    [<DebuggerHidden; DebuggerStepThrough>]
    member _.Using(value: IDisposable, binder: IDisposable -> NodeCode<'U>) =
        Node(
            async {
                use _ = value
                return! binder value |> Async.AwaitNodeCode
            }
        )
        

let node = NodeCodeBuilder()

[<AbstractClass; Sealed>]
type NodeCode private () =

    static let cancellationToken = Node(wrapThreadStaticInfo Async.CancellationToken)

    static member RunImmediate(computation: NodeCode<'T>, ct: CancellationToken) =
        let diagnosticsLogger = DiagnosticsThreadStatics.DiagnosticsLogger
        let phase = DiagnosticsThreadStatics.BuildPhase

        try
            try
                let work =
                    async {
                        DiagnosticsThreadStatics.DiagnosticsLogger <- diagnosticsLogger
                        DiagnosticsThreadStatics.BuildPhase <- phase
                        return! computation |> Async.AwaitNodeCode
                    }

                Async.StartImmediateAsTask(work, cancellationToken = ct).Result
            finally
                DiagnosticsThreadStatics.DiagnosticsLogger <- diagnosticsLogger
                DiagnosticsThreadStatics.BuildPhase <- phase
        with :? AggregateException as ex when ex.InnerExceptions.Count = 1 ->
            raise (ex.InnerExceptions[0])

    static member RunImmediateWithoutCancellation(computation: NodeCode<'T>) =
        NodeCode.RunImmediate(computation, CancellationToken.None)

    static member StartAsTask_ForTesting(computation: NodeCode<'T>, ?ct: CancellationToken) =
        let diagnosticsLogger = DiagnosticsThreadStatics.DiagnosticsLogger
        let phase = DiagnosticsThreadStatics.BuildPhase

        try
            let work =
                async {
                    DiagnosticsThreadStatics.DiagnosticsLogger <- diagnosticsLogger
                    DiagnosticsThreadStatics.BuildPhase <- phase
                    return! computation |> Async.AwaitNodeCode
                }

            Async.StartAsTask(work, cancellationToken = defaultArg ct CancellationToken.None)
        finally
            DiagnosticsThreadStatics.DiagnosticsLogger <- diagnosticsLogger
            DiagnosticsThreadStatics.BuildPhase <- phase

    static member CancellationToken = cancellationToken

    static member FromCancellable(computation: Cancellable<'T>) =
        Node(wrapThreadStaticInfo (Cancellable.toAsync computation))

    static member AwaitAsync(computation: Async<'T>) = Node(wrapThreadStaticInfo computation)

    static member AwaitTask(task: Task<'T>) =
        Node(wrapThreadStaticInfo (Async.AwaitTask task))

    static member AwaitTask(task: Task) =
        Node(wrapThreadStaticInfo (Async.AwaitTask task))

    static member AwaitWaitHandle_ForTesting(waitHandle: WaitHandle) =
        Node(wrapThreadStaticInfo (Async.AwaitWaitHandle(waitHandle)))

    static member Sleep(ms: int) =
        Node(wrapThreadStaticInfo (Async.Sleep(ms)))

    static member Sequential(computations: NodeCode<'T> seq) =
        node {
            let results = ResizeArray()

            for computation in computations do
                let! res = computation
                results.Add(res)

            return results.ToArray()
        }
    
    static member Parallel (computations: NodeCode<'T> seq) =
        computations
        |> Seq.map (fun (Node x) -> x)
        |> Async.Parallel
        |> Node

[<RequireQualifiedAccess>]
module GraphNode =

    // We need to store the culture for the VS thread that is executing now,
    // so that when the agent in the async lazy object picks up thread from the thread pool we can set the culture
    let mutable culture = CultureInfo(CultureInfo.CurrentUICulture.Name)

    let SetPreferredUILang (preferredUiLang: string option) =
        match preferredUiLang with
        | Some s ->
            culture <- CultureInfo s
#if FX_RESHAPED_GLOBALIZATION
            CultureInfo.CurrentUICulture <- culture
#else
            Thread.CurrentThread.CurrentUICulture <- culture
#endif
        | None -> ()

[<Sealed>]
type GraphNode<'T> private (computation: NodeCode<'T>, cachedResult: ValueOption<'T>, cachedResultNode: NodeCode<'T>) =

    let mutable computation = computation
    let mutable requestCount = 0

    let mutable cachedResult = cachedResult
    let mutable cachedResultNode: NodeCode<'T> = cachedResultNode

    let isCachedResultNodeNotNull () =
        not (obj.ReferenceEquals(cachedResultNode, null))

    let semaphore = new SemaphoreSlim(1, 1)

    member _.GetOrComputeValue() =
        // fast path
        if isCachedResultNodeNotNull () then
            cachedResultNode
        else
            node {
                Interlocked.Increment(&requestCount) |> ignore
                try
                    let! ct = NodeCode.CancellationToken

                    // We must set 'taken' before any implicit cancellation checks
                    // occur, making sure we are under the protection of the 'try'.
                    // For example, NodeCode's 'try/finally' (TryFinally) uses async.TryFinally which does
                    // implicit cancellation checks even before the try is entered, as do the
                    // de-sugaring of 'do!' and other NodeCode constructs.
                    let mutable taken = false

                    try
                        do!
                            semaphore
                                .WaitAsync(ct)
                                .ContinueWith(
                                    (fun _ -> taken <- true),
                                    (TaskContinuationOptions.NotOnCanceled
                                        ||| TaskContinuationOptions.NotOnFaulted
                                        ||| TaskContinuationOptions.ExecuteSynchronously)
                                )
                            |> NodeCode.AwaitTask

                        match cachedResult with
                        | ValueSome value -> return value
                        | _ ->
                            let tcs = TaskCompletionSource<'T>()
                            let (Node (p)) = computation

                            Async.StartWithContinuations(
                                async {
                                    Thread.CurrentThread.CurrentUICulture <- GraphNode.culture
                                    return! p
                                },
                                (fun res ->
                                    cachedResult <- ValueSome res
                                    cachedResultNode <- node.Return res
                                    computation <- Unchecked.defaultof<_>
                                    tcs.SetResult(res)),
                                (fun ex -> tcs.SetException(ex)),
                                (fun _ -> tcs.SetCanceled()),
                                ct
                            )

                            return! tcs.Task |> NodeCode.AwaitTask
                    finally
                        if taken then semaphore.Release() |> ignore
                finally
                    Interlocked.Decrement(&requestCount) |> ignore
            }

    member _.TryPeekValue() = cachedResult

    member _.HasValue = cachedResult.IsSome

    member _.IsComputing = requestCount > 0

    static member FromResult(result: 'T) =
        let nodeResult = node.Return result
        GraphNode(nodeResult, ValueSome result, nodeResult)

    new(computation) = GraphNode(computation, ValueNone, Unchecked.defaultof<_>)
