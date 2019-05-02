module internal FSharp.Compiler.Compilation.Utilities

open System
open System.Threading
open System.Collections.Immutable
open FSharp.Compiler.AbstractIL.Internal.Library

[<RequireQualifiedAccess>]
module Cancellable =

    let toAsync e = 
        async { 
          let! ct = Async.CancellationToken
          return! 
             Async.FromContinuations(fun (cont, econt, ccont) -> 
               // Run the computation synchronously using the given cancellation token
               let res = try Choice1Of2 (Cancellable.run ct e) with err -> Choice2Of2 err
               match res with 
               | Choice1Of2 (ValueOrCancelled.Value v) -> cont v
               | Choice1Of2 (ValueOrCancelled.Cancelled err) -> ccont err
               | Choice2Of2 err -> econt err) 
        }

[<RequireQualifiedAccess>]
module ImmutableArray =

    let inline iter f (arr: ImmutableArray<_>) =
        for i = 0 to arr.Length - 1 do
            f arr.[i]

    let inline iteri f (arr: ImmutableArray<_>) =
        for i = 0 to arr.Length - 1 do
            f i arr.[i]

type private AsyncLazyWeakMessage<'T> =
    | GetValue of AsyncReplyChannel<Result<'T, Exception>> * CancellationToken

[<Sealed>]
type AsyncLazyWeak<'T when 'T : not struct> (computation: Async<'T>) =

    let gate = obj ()
    let mutable requestCount = 0
    let mutable cachedResult: WeakReference<'T> voption = ValueNone

    let tryGetResult () =
        match cachedResult with
        | ValueSome weak ->
            match weak.TryGetTarget () with
            | true, result -> ValueSome result
            | _ -> ValueNone
        | _ -> ValueNone

    let loop (agent: MailboxProcessor<AsyncLazyWeakMessage<'T>>) =
        async {
            while true do
                match! agent.Receive() with
                | GetValue (replyChannel, cancellationToken) ->
                    try
                        cancellationToken.ThrowIfCancellationRequested ()
                        match tryGetResult () with
                        | ValueSome result ->
                            replyChannel.Reply (Ok result)
                        | _ ->
                            let! result = computation
                            cachedResult <- ValueSome (WeakReference<_> result)
                            replyChannel.Reply (Ok result) 
                    with 
                    | ex ->
                        replyChannel.Reply (Error ex)
        }

    let mutable agentInstance: (MailboxProcessor<AsyncLazyWeakMessage<'T>> * CancellationTokenSource) option = None

    member __.GetValueAsync () =
       async {
           match tryGetResult () with
           | ValueSome result -> return result
           | _ ->
               let! cancellationToken = Async.CancellationToken
               let agent, cts =
                    lock gate <| fun() ->
                        requestCount <- requestCount + 1
                        match agentInstance with
                        | Some agentInstance -> agentInstance
                        | _ ->
                            let cts = new CancellationTokenSource ()
                            let agent = new MailboxProcessor<AsyncLazyWeakMessage<'T>>((fun x -> loop x), cancellationToken = cts.Token)
                            let newAgentInstance = (agent, cts)
                            agentInstance <- Some newAgentInstance
                            agent.Start ()
                            newAgentInstance
                        
               try
                   match! agent.PostAndAsyncReply (fun replyChannel -> GetValue (replyChannel, cancellationToken)) with
                   | Ok result -> return result
                   | Error ex -> return raise ex
                finally
                    lock gate <| fun () ->
                        requestCount <- requestCount - 1
                        if requestCount = 0 then
                             cts.Cancel ()
                             (agent :> IDisposable).Dispose ()
                             cts.Dispose ()
                             agentInstance <- None
       }

    member __.CancelIfNotComplete () =
        lock gate <| fun () ->
            match agentInstance with
            | Some (_, cts) -> cts.Cancel ()
            | _ -> ()

type AsyncLazy<'T when 'T : not struct> (computation) =
    
    let weak = AsyncLazyWeak<'T> computation
    let mutable cachedResult = ValueNone // hold strongly

    member __.GetValueAsync () =
        async {
            match cachedResult with
            | ValueSome result -> return result
            | _ ->
                let! result = weak.GetValueAsync ()
                cachedResult <- ValueSome result
                return result
        }

    member __.CancelIfNotComplete () =
        weak.CancelIfNotComplete ()