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
    | GetValue of CancellationToken * AsyncReplyChannel<Result<'T, Exception>>

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
                | GetValue (cancellationToken, replyChannel) ->
                    try
                        cancellationToken.ThrowIfCancellationRequested ()
                        match tryGetResult () with
                        | ValueSome result ->
                            replyChannel.Reply (Ok result)
                        | _ ->
                            let! result = computation
                            cancellationToken.ThrowIfCancellationRequested ()
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
                    lock gate <| fun () ->
                        requestCount <- requestCount + 1
                        match agentInstance with
                        | Some agentInstance -> agentInstance
                        | _ ->
                            let cts = new CancellationTokenSource ()
                            let agent = new MailboxProcessor<AsyncLazyWeakMessage<'T>> ((fun x -> loop x), cancellationToken = cts.Token)
                            let newAgentInstance = (agent, cts)
                            agentInstance <- Some newAgentInstance
                            agent.Start ()
                            newAgentInstance
                        
               try
                   use! _onCancel = Async.OnCancel cts.Cancel
                   match! agent.PostAndAsyncReply (fun replyChannel -> GetValue (cancellationToken, replyChannel)) with
                   | Ok result -> return result
                   | Error ex -> return raise ex
                finally
                    lock gate <| fun () ->
                        requestCount <- requestCount - 1
                        if requestCount = 0 then
                             (agent :> IDisposable).Dispose ()
                             cts.Dispose ()
                             agentInstance <- None
       }

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

/// Thread safe.
[<Sealed>]
type private Lru<'Key, 'Value when 'Key : equality and 'Value : not struct> (size: int, equalityComparer: IEqualityComparer<'Key>) =

    let gate = obj ()

    let isKeyRef = not typeof<'Key>.IsValueType
    let isValueRef = not typeof<'Value>.IsValueType

    let data = LinkedList<KeyValuePair<'Key, 'Value>> ()
    let dataLookup = Dictionary<'Key, LinkedListNode<KeyValuePair<'Key, 'Value>>> (equalityComparer)

    let keyEquals key1 key2 =
        equalityComparer.GetHashCode key1 = equalityComparer.GetHashCode key2 && equalityComparer.Equals (key1, key2)

    member __.Set (key, value) =
        if isKeyRef && obj.ReferenceEquals (key, null) then
            nullArg "key"

        if isValueRef && obj.ReferenceEquals (value, null) then
            nullArg "value"

        lock gate <| fun () ->
            let pair = KeyValuePair (key, value)

            match dataLookup.TryGetValue key with
            | true, existingNode ->
                if existingNode <> data.First then
                    data.Remove existingNode
                    dataLookup.[key] <- data.AddFirst pair
            | _ ->
                if data.Count = size then
                    dataLookup.Remove data.Last.Value.Key |> ignore
                    data.RemoveLast ()
                dataLookup.[key] <- data.AddFirst pair

    member __.TryGetValue key =
        if isKeyRef && obj.ReferenceEquals (key, null) then
            nullArg "key"

        lock gate <| fun () ->
            if data.Count > 0 then
                if keyEquals data.First.Value.Key key then
                    ValueSome data.First.Value.Value
                else
                    match dataLookup.TryGetValue key with
                    | true, existingNode ->
                        data.Remove existingNode
                        dataLookup.[key] <- data.AddFirst existingNode.Value
                        ValueSome existingNode.Value.Value
                    | _ ->
                        ValueNone
            else
                ValueNone

    member __.Remove key =
        match dataLookup.TryGetValue key with
        | true, node ->
            dataLookup.Remove key |> ignore
            data.Remove node
            true
        | _ ->
            false

    member __.Count = data.Count

/// Thread safe.
[<Sealed>]
type MruCache<'Key, 'Value when 'Key : equality and 'Value : not struct> (cacheSize: int, maxWeakReferenceSize: int, equalityComparer: IEqualityComparer<'Key>) =

    let gate = obj ()

    let isKeyRef = not typeof<'Key>.IsValueType
    let isValueRef = not typeof<'Value>.IsValueType

    let cacheLookup = Dictionary<'Key, 'Value> (equalityComparer)
    let weakReferenceLookup = Lru<'Key, WeakReference<'Value>> (maxWeakReferenceSize, equalityComparer)
    let mutable mruKey = Unchecked.defaultof<'Key>
    let mutable mruValue = Unchecked.defaultof<'Value>

    let keyEquals key1 key2 =
        equalityComparer.GetHashCode key1 = equalityComparer.GetHashCode key2 && equalityComparer.Equals (key1, key2)

    let tryFindWeakReferenceCacheValue key =
        match weakReferenceLookup.TryGetValue key with
        | ValueSome value -> 
            match value.TryGetTarget () with
            | true, value ->
                ValueSome value
            | _ -> 
                weakReferenceLookup.Remove key |> ignore
                ValueNone
        | _ -> ValueNone

    let tryFindCacheValue key =
        match cacheLookup.TryGetValue key with
        | true, value -> ValueSome value
        | _ -> ValueNone

    let purgeWeakReferenceCache () =
        if weakReferenceLookup.Count > maxWeakReferenceSize then
            let arr = ResizeArray weakReferenceLookup.Count
            for pair in weakReferenceLookup do
                match pair.Value.TryGetTarget () with
                | false, _ -> arr.Add pair.Key
                | _ -> ()

            for i = 0 to arr.Count - 1 do
                let key = arr.[i]
                weakReferenceLookup.Remove key |> ignore

    let shrinkCache key =
        if cacheLookup.Count = cacheSize && not (keyEquals key mruKey) then
            if maxWeakReferenceSize > 0 then
                weakReferenceLookup.[mruKey] <- WeakReference<_> mruValue
            cacheLookup.Remove mruKey |> ignore

    member __.Set (key, value) =
        if isKeyRef && obj.ReferenceEquals (key, null) then
            nullArg "key"

        if isValueRef && obj.ReferenceEquals (value, null) then
            nullArg "value"

        lock gate <| fun () ->
            shrinkCache key
            purgeWeakReferenceCache ()

            mruKey <- key
            mruValue <- value
            cacheLookup.[key] <- value

    member __.TryGetValue key =
        if isKeyRef && obj.ReferenceEquals (key, null) then
            nullArg "key"

        lock gate <| fun () ->
            // fast path
            if cacheLookup.Count > 0 && keyEquals mruKey key then
                ValueSome mruValue
            else
                match tryFindCacheValue key with
                | ValueSome value ->
                    shrinkCache key
                    mruKey <- key
                    mruValue <- value
                    ValueSome value
                | _ ->
                    match tryFindWeakReferenceCacheValue key with
                    | ValueSome value ->
                        shrinkCache key
                        mruKey <- key
                        mruValue <- value
                        ValueSome value
                    | _ ->
                        ValueNone
