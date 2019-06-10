module internal FSharp.Compiler.Compilation.Utilities

open System
open System.Threading
open System.Collections
open System.Collections.Immutable
open System.Collections.Generic
open FSharp.Compiler.AbstractIL.Internal.Library

[<RequireQualifiedAccess>]
module internal Cancellable =

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
module internal ImmutableArray =

    let inline iter f (arr: ImmutableArray<_>) =
        for i = 0 to arr.Length - 1 do
            f arr.[i]

    let inline iteri f (arr: ImmutableArray<_>) =
        for i = 0 to arr.Length - 1 do
            f i arr.[i]

    let inline map f (arr: ImmutableArray<_>) =
        let builder = ImmutableArray.CreateBuilder (arr.Length)
        builder.Count <- arr.Length
        for i = 0 to arr.Length - 1 do
            builder.[i] <- f arr.[i]
        builder.ToImmutable ()

[<RequireQualifiedAccess>]
type internal ValueStrength<'T when 'T : not struct> =
    | None
    | Strong of 'T
    | Weak of WeakReference<'T>

    member this.TryGetTarget (value: outref<'T>) =
        match this with
        | ValueStrength.None -> 
            false
        | ValueStrength.Strong v ->
            value <- v
            true
        | ValueStrength.Weak v ->
            v.TryGetTarget &value

type private AsyncLazyWeakMessage<'T> =
    | GetValue of AsyncReplyChannel<Result<'T, Exception>> * CancellationToken

type private AgentInstance<'T> = (MailboxProcessor<AsyncLazyWeakMessage<'T>> * CancellationTokenSource)

[<RequireQualifiedAccess>]
type private AgentAction<'T> =
    | GetValue of AgentInstance<'T>
    | CachedValue of 'T

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
                | GetValue (replyChannel, ct) ->
                    try
                        use _reg = 
                            ct.Register (fun () -> 
                                let ex = OperationCanceledException() :> exn
                                replyChannel.Reply (Error ex)
                            )
                        ct.ThrowIfCancellationRequested ()

                        match tryGetResult () with
                        | ValueSome result ->
                            replyChannel.Reply (Ok result)
                        | _ ->
                            let! result = computation
                            cachedResult <- ValueSome (WeakReference<_> result)

                            if not ct.IsCancellationRequested then
                                replyChannel.Reply (Ok result) 
                    with 
                    | ex ->
                        replyChannel.Reply (Error ex)
        }

    let mutable agentInstance: (MailboxProcessor<AsyncLazyWeakMessage<'T>> * CancellationTokenSource) option = None

    member __.GetValueAsync () =
       async {
           // fast path
           // TODO: Perhaps we could make the fast path non-allocating since we create a new async everytime.
           match tryGetResult () with
           | ValueSome result -> return result
           | _ ->
               let action =
                    lock gate <| fun () ->
                        // We try to get the cached result after the lock so we don't spin up a new mailbox processor.
                        match tryGetResult () with
                        | ValueSome result -> AgentAction<'T>.CachedValue result
                        | _ ->
                            requestCount <- requestCount + 1
                            match agentInstance with
                            | Some agentInstance -> AgentAction<'T>.GetValue agentInstance
                            | _ ->
                                let cts = new CancellationTokenSource ()
                                let agent = new MailboxProcessor<AsyncLazyWeakMessage<'T>> ((fun x -> loop x), cancellationToken = cts.Token)
                                let newAgentInstance = (agent, cts)
                                agentInstance <- Some newAgentInstance
                                agent.Start ()
                                AgentAction<'T>.GetValue newAgentInstance

               match action with
               | AgentAction.CachedValue result -> return result
               | AgentAction.GetValue (agent, cts) ->
                        
                   try
                       let! ct = Async.CancellationToken
                       match! agent.PostAndAsyncReply (fun replyChannel -> GetValue(replyChannel, ct)) with
                       | Ok result -> return result
                       | Error ex -> return raise ex
                    finally
                        lock gate <| fun () ->
                            requestCount <- requestCount - 1
                            if requestCount = 0 then
                                 cts.Cancel () // cancel computation when all requests are cancelled
                                 (agent :> IDisposable).Dispose ()
                                 cts.Dispose ()
                                 agentInstance <- None
       }

       member __.TryGetValue () = tryGetResult ()

[<Sealed>]
type AsyncLazy<'T> (computation) =
    
    let computation =
        async {
            let! result = computation
            return ref result
        }
    let gate = obj ()
    let mutable asyncLazyWeak = ValueSome (AsyncLazyWeak<'T ref> computation)
    let mutable cachedResult = ValueNone // hold strongly

    member __.GetValueAsync () =
        async {
            // fast path
            // TODO: Perhaps we could make the fast path non-allocating since we create a new async everytime.
            match cachedResult, asyncLazyWeak with
            | ValueSome result, _ -> return result
            | _, ValueSome weak ->
                let! result = weak.GetValueAsync ()
                lock gate <| fun () ->
                    // Make sure we set it only once.
                    if cachedResult.IsNone then
                        cachedResult <- ValueSome result.contents
                        asyncLazyWeak <- ValueNone // null out computation function so we don't strongly hold onto any references once we finished computing.
                return cachedResult.Value
            | _ -> 
                return failwith "should not happen"
        }

    member __.TryGetValue () = cachedResult

/// Thread safe.
[<Sealed>]
type LruCache<'Key, 'Value when 'Key : equality and 'Value : not struct> (cacheSize: int, equalityComparer: IEqualityComparer<'Key>) =

    let size = if cacheSize <= 0 then invalidArg "cacheSize" "Cache size cannot be less than or equal to zero." else cacheSize

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
        if isKeyRef && obj.ReferenceEquals (key, null) then
            nullArg "key"

        lock gate <| fun () ->
            match dataLookup.TryGetValue key with
            | true, node ->
                dataLookup.Remove key |> ignore
                data.Remove node
                true
            | _ ->
                false

    member __.Count = dataLookup.Count

    interface IEnumerable<KeyValuePair<'Key, 'Value>> with

        member __.GetEnumerator () : IEnumerator<KeyValuePair<'Key, 'Value>> = (data :> IEnumerable<KeyValuePair<'Key, 'Value>>).GetEnumerator ()

        member __.GetEnumerator () : IEnumerator = (data :> IEnumerable).GetEnumerator ()

/// Thread safe.
/// Same as MruCache, but when it evicts an item out of the cache, it turns the value into a weak reference and puts it into a LruCache.
/// If a weak reference item is still alive and the MruWeakCache touches it, then the item is promoted to Mru and removed from the LruCache; no longer a weak reference.
[<Sealed>]
type MruWeakCache<'Key, 'Value when 'Key : equality and 'Value : not struct> (cacheSize: int, weakReferenceCacheSize: int, equalityComparer: IEqualityComparer<'Key>) =

    let cacheSize = if cacheSize <= 0 then invalidArg "cacheSize" "Cache size cannot be less than or equal to zero." else cacheSize
    let weakReferenceCacheSize = if weakReferenceCacheSize <= 0 then invalidArg "weakReferenceCacheSize" "Weak reference cache size cannot be less than or equal to zero." else cacheSize

    let gate = obj ()

    let isKeyRef = not typeof<'Key>.IsValueType
    let isValueRef = not typeof<'Value>.IsValueType

    let cacheLookup = Dictionary<'Key, 'Value> (equalityComparer)
    let weakReferenceLookup = LruCache<'Key, WeakReference<'Value>> (weakReferenceCacheSize, equalityComparer)
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

    let setMru key value =
        if cacheLookup.Count = cacheSize && (not (keyEquals key mruKey) && not (cacheLookup.ContainsKey key)) then
            weakReferenceLookup.Set (mruKey, WeakReference<_> mruValue)
            cacheLookup.Remove mruKey |> ignore
            weakReferenceLookup.Remove key |> ignore
        mruKey <- key
        mruValue <- value
        cacheLookup.[key] <- value

    member __.Set (key, value) =
        if isKeyRef && obj.ReferenceEquals (key, null) then
            nullArg "key"

        if isValueRef && obj.ReferenceEquals (value, null) then
            nullArg "value"

        lock gate <| fun () ->
            setMru key value

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
                    setMru key value
                    ValueSome value
                | _ ->
                    match tryFindWeakReferenceCacheValue key with
                    | ValueSome value ->
                        setMru key value
                        ValueSome value
                    | _ ->
                        ValueNone

    member __.WeakReferenceCount = weakReferenceLookup.Count

    member __.Count = cacheLookup.Count
