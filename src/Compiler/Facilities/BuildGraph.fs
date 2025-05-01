// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.BuildGraph

open System.Threading
open System.Globalization

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
                let! ct = Async.CancellationToken
                Interlocked.Increment(&requestCount) |> ignore
                let enter = semaphore.WaitAsync(ct)

                try
                    do! enter |> Async.AwaitTask

                    match cachedResult with
                    | ValueSome value -> return value
                    | _ ->
                        Thread.CurrentThread.CurrentUICulture <- GraphNode.culture
                        let! result = computation
                        cachedResult <- ValueSome result
                        cachedResultNode <- async.Return result
                        computation <- Unchecked.defaultof<_>
                        return result
                finally
                    // At this point, the semaphore awaiter is either already completed or about to get canceled.
                    // If calling Wait() does not throw an exception it means the semaphore was successfully taken and needs to be released.
                    try
                        enter.Wait()
                        semaphore.Release() |> ignore
                    with _ ->
                        ()

                    Interlocked.Decrement(&requestCount) |> ignore
            }

    member _.TryPeekValue() = cachedResult

    member _.HasValue = cachedResult.IsSome

    member _.IsComputing = requestCount > 0

    static member FromResult(result: 'T) =
        let nodeResult = async.Return result
        GraphNode(nodeResult, ValueSome result, nodeResult)

    new(computation) = GraphNode(computation, ValueNone, Unchecked.defaultof<_>)
