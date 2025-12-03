// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.BuildGraph

open System.Threading
open System.Globalization

open Internal.Utilities.Library

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
type internal GraphNode<'T> private (computation: Async2<'T>, cachedResult: ValueOption<'T>, cachedResultNode: Async2<'T>) =

    let mutable computation = computation
    let mutable requestCount = 0

    let mutable cachedResult = cachedResult
    let mutable cachedResultNode: Async2<'T> = cachedResultNode

    let isCachedResultNodeNotNull () =
        not (obj.ReferenceEquals(cachedResultNode, null))

    let semaphore = new SemaphoreSlim(1, 1)

    member _.GetOrComputeValue() =
        // fast path
        if isCachedResultNodeNotNull () then
            cachedResultNode
        else
            async2 {
                let! ct = Async2.CancellationToken
                Interlocked.Increment(&requestCount) |> ignore

                let mutable acquired = false

                try
                    do! semaphore.WaitAsync(ct)
                    acquired <- true

                    match cachedResult with
                    | ValueSome value -> return value
                    | _ ->
                        Thread.CurrentThread.CurrentUICulture <- GraphNode.culture
                        let! result = computation
                        cachedResult <- ValueSome result
                        cachedResultNode <- Async2.fromValue result
                        computation <- Unchecked.defaultof<_>
                        return result
                finally
                    if acquired then
                        try
                            semaphore.Release() |> ignore
                        with _ ->
                            ()

                    Interlocked.Decrement(&requestCount) |> ignore
            }

    member _.TryPeekValue() = cachedResult

    member _.HasValue = cachedResult.IsSome

    member _.IsComputing = requestCount > 0

    static member FromResult(result: 'T) =
        let nodeResult = Async2.fromValue result
        GraphNode(nodeResult, ValueSome result, nodeResult)

    new(computation) = GraphNode(computation, ValueNone, Unchecked.defaultof<_>)
