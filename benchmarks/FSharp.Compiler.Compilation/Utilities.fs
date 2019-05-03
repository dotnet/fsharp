module internal FSharp.Compiler.Compilation.Utilities

open System
open System.Threading
open System.Collections.Immutable
open System.Collections.Generic
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
               let cancellationToken = CancellationToken.None //Async.CancellationToken
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

[<Sealed>]
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

[<StructuralEquality; NoComparison>]
type internal ValueStrength<'T when 'T : not struct> =
   | Strong of 'T
   | Weak of WeakReference<'T>

[<Sealed>]
type MruCache<'Key, 'Value when 'Value : not struct> (cacheSize: int, maxWeakReferenceSize: int, equalityComparer: IEqualityComparer<'Key>) =

    let gate = obj ()

    let mutable mruIndex = -1
    let mutable mruKey = Unchecked.defaultof<'Key>
    let mutable mruValue = Unchecked.defaultof<'Value>
    let mutable cacheCount = 0
    let cache = Array.zeroCreate<'Key * 'Value> cacheSize

    let mutable weakReferencesIndex = 0
    let weakReferences = Array.zeroCreate<'Key * WeakReference<'Value>> maxWeakReferenceSize

    let keyEquality key1 key2 =
        equalityComparer.GetHashCode key1 = equalityComparer.GetHashCode key2 && equalityComparer.Equals (key1, key2)

    let setWeakReference key value =
        weakReferences.[weakReferencesIndex] <- (key, WeakReference<_> value)
        weakReferencesIndex <- weakReferencesIndex + 1
        if weakReferencesIndex >= maxWeakReferenceSize then
            weakReferencesIndex <- 0

    let tryFindWeakReference key =
        let mutable value = ValueNone
        let mutable count = 0
        while count <> maxWeakReferenceSize do
            let i = 
                let i = weakReferencesIndex - count
                if i < 0 then
                    maxWeakReferenceSize - 1
                else
                    i
            count <- count + 1
            let weakItem = weakReferences.[i]
            if not (obj.ReferenceEquals (weakItem, null)) then
                match (snd weakItem).TryGetTarget () with
                | true, v ->
                    if keyEquality key (fst weakItem) then
                        if value.IsNone then
                            value <- ValueSome v
                        else
                            // remove possible duplicate
                            weakReferences.[i] <- Unchecked.defaultof<_>
                | _ ->
                    weakReferences.[i] <- Unchecked.defaultof<_>

        value

    member this.Set (key, value) =
        lock gate <| fun () ->
            // TODO: Remove allocation.
            match cache |> Array.tryFindIndex (fun pair -> not (obj.ReferenceEquals (pair, null)) && keyEquality key (fst pair)) with
            | Some index -> 
                mruIndex <- index
            | _ ->
                if cacheCount = cacheSize then
                    setWeakReference mruKey mruValue
                    cache.[mruIndex] <- (key, value)
                else
                    cache.[cacheCount] <- (key, value)
                    mruIndex <- cacheCount
                    cacheCount <- cacheCount + 1

            mruKey <- key
            mruValue <- value

    member this.TryGetValue key =
        lock gate <| fun () ->
            // fast path
            if not (obj.ReferenceEquals (mruKey, null)) && keyEquality mruKey key then
                ValueSome mruValue
            else
                // TODO: Remove allocation.
                match cache |> Array.tryFind (fun pair -> not (obj.ReferenceEquals (pair, null)) && keyEquality key (fst pair)) with
                | Some (_, value) -> ValueSome value
                | _ ->
                    tryFindWeakReference key
