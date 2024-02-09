// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.BuildGraph

open System
open System.Threading
open System.Threading.Tasks
open System.Globalization
open Internal.Utilities.Library
open FSharp.Compiler.DiagnosticsLogger

[<AbstractClass; Sealed>]
type Async =
    static member CompilationScope(computation: Async<'T>) =
        async {
            use _ =
                new CompilationGlobalsScope(DiagnosticsAsyncState.DiagnosticsLogger, DiagnosticsAsyncState.BuildPhase)

            return! computation
        }

    static member RunImmediateWithoutCancellation(computation) =
        try
            Async
                .StartImmediateAsTask(computation |> Async.CompilationScope, cancellationToken = CancellationToken.None)
                .Result

        with :? AggregateException as ex when ex.InnerExceptions.Count = 1 ->
            raise (ex.InnerExceptions[0])

    static member FromCancellableWithScope(computation: Cancellable<'T>) =
        computation |> Cancellable.toAsync |> Async.CompilationScope

    static member StartAsTask_ForTesting(computation: Async<'T>, ?ct: CancellationToken) =
        Async.StartAsTask(computation |> Async.CompilationScope, cancellationToken = defaultArg ct CancellationToken.None)

    static member SequentialImmediate(computations: Async<'T> seq) =
        async {
            let results = ResizeArray()

            for computation in computations do
                let! result = computation
                results.Add result

            return results.ToArray()
        }

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
type GraphNode<'T> private (computation: Async<'T>, cachedResult: ValueOption<'T>, cachedResultNode: Async<'T>) =

    let mutable computation = computation
    let mutable requestCount = 0

    let mutable cachedResult = cachedResult
    let mutable cachedResultNode: Async<'T> = cachedResultNode

    let isCachedResultNodeNotNull () =
        not (obj.ReferenceEquals(cachedResultNode, null))

    let semaphore = new SemaphoreSlim(1, 1)

    member _.GetOrComputeValue() =
        // fast path
        if isCachedResultNodeNotNull () then
            cachedResultNode
        else
            async {
                Interlocked.Increment(&requestCount) |> ignore

                try
                    let! ct = Async.CancellationToken

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
                            |> Async.AwaitTask

                        match cachedResult with
                        | ValueSome value -> return value
                        | _ ->
                            let tcs = TaskCompletionSource<'T>()
                            let p = computation

                            Async.StartWithContinuations(
                                async {
                                    Thread.CurrentThread.CurrentUICulture <- GraphNode.culture
                                    return! p |> Async.CompilationScope
                                },
                                (fun res ->
                                    cachedResult <- ValueSome res
                                    cachedResultNode <- async.Return res
                                    computation <- Unchecked.defaultof<_>
                                    tcs.SetResult(res)),
                                (fun ex -> tcs.SetException(ex)),
                                (fun _ -> tcs.SetCanceled()),
                                ct
                            )

                            return! tcs.Task |> Async.AwaitTask
                    finally
                        if taken then
                            semaphore.Release() |> ignore
                finally
                    Interlocked.Decrement(&requestCount) |> ignore
            }

    member _.TryPeekValue() = cachedResult

    member _.HasValue = cachedResult.IsSome

    member _.IsComputing = requestCount > 0

    static member FromResult(result: 'T) =
        let nodeResult = async.Return result
        GraphNode(nodeResult, ValueSome result, nodeResult)

    new(computation) = GraphNode(computation, ValueNone, Unchecked.defaultof<_>)
