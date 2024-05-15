// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.BuildGraph

open System
open System.Threading
open System.Threading.Tasks
open System.Globalization

type Async with

    static member FlattenException(computation: Async<'T>) =
        async {
            try
                return! computation
            with :? AggregateException as ex when ex.InnerExceptions.Count = 1 ->
                return raise (ex.InnerExceptions[0])
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
            Thread.CurrentThread.CurrentUICulture <- culture
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

                            Async.StartWithContinuations(
                                async {
                                    Thread.CurrentThread.CurrentUICulture <- GraphNode.culture
                                    return! computation
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
            |> Async.FlattenException

    member _.TryPeekValue() = cachedResult

    member _.HasValue = cachedResult.IsSome

    member _.IsComputing = requestCount > 0

    static member FromResult(result: 'T) =
        let nodeResult = async.Return result
        GraphNode(nodeResult, ValueSome result, nodeResult)

    new(computation) = GraphNode(computation, ValueNone, Unchecked.defaultof<_>)
