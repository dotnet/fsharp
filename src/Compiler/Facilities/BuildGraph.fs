// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.BuildGraph

open System
open System.Threading
open System.Threading.Tasks
open System.Globalization
open Internal.Utilities.Library

[<AbstractClass; Sealed>]
type Async =
    static member RunImmediateWithoutCancellation(computation) =
        Async.RunImmediate(computation, CancellationToken.None)

    static member FromCancellable(computation: Cancellable<'T>) = Cancellable.toAsync computation

    static member StartAsTask_ForTesting(computation: Async<'T>, ?ct: CancellationToken) =
        Async.StartAsTask(computation, cancellationToken = defaultArg ct CancellationToken.None)

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
type GraphNode<'T> private (compute: unit -> unit, tcs: TaskCompletionSource<'T>, cts: CancellationTokenSource) =

    let mutable requestCount = 0
    let mutable started = false

    // Any locking we do is for very short synchronous state updates.
    let gate = obj

    new(computation) =
        // Apparently a trick to force GC of the original computation:
        let mutable computation = computation

        let tcs = TaskCompletionSource<'T>()
        let cts = new CancellationTokenSource()

        let compute () =
            Async.StartWithContinuations(
                async {
                    Thread.CurrentThread.CurrentUICulture <- GraphNode.culture
                    return! computation
                },
                (fun result ->
                    tcs.SetResult result
                    // Allow GC of the original computation.
                    computation <- Unchecked.defaultof<_>),
                (tcs.SetException),
                (ignore >> tcs.SetCanceled),
                // This is not a requestor's CancellationToken.
                cts.Token)

        GraphNode(compute, tcs, cts)

    member _.GetOrComputeValue() =

        // Lock for the sake of `started` flag.
        let startNew = lock gate <| fun () ->
            Interlocked.Increment &requestCount = 1 && not started
        
        // The cancellation of the computation is not governed by the requestor's CancellationToken. 
        // It will continue to run as long as there are requests.
        if startNew then started <- true; compute()

        async {
            try 
                return! tcs.Task |> Async.AwaitTask
            finally
                if Interlocked.Decrement &requestCount = 0 then
                    // All requestors either finished or cancelled, so it is safe to cancel either way.
                    cts.Cancel()
        }


    member _.TryPeekValue() = if tcs.Task.IsCompleted then ValueSome tcs.Task.Result else ValueNone

    member _.HasValue = tcs.Task.IsCompleted

    member _.IsComputing = requestCount > 0

    static member FromResult(result: 'T) =
        let tcs = TaskCompletionSource()
        tcs.SetResult result
        GraphNode(ignore, tcs, new CancellationTokenSource())
      
