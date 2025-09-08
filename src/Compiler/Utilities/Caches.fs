// LinkedList uses nulls, so we need to disable the nullability warnings for this file.
namespace FSharp.Compiler.Caches

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Threading
open System.Diagnostics
open System.Diagnostics.Metrics

module CacheMetrics =
    let Meter = new Meter("FSharp.Compiler.Cache")
    let adds = Meter.CreateCounter<int64>("adds", "count")
    let updates = Meter.CreateCounter<int64>("updates", "count")
    let hits = Meter.CreateCounter<int64>("hits", "count")
    let misses = Meter.CreateCounter<int64>("misses", "count")
    let evictions = Meter.CreateCounter<int64>("evictions", "count")
    let evictionFails = Meter.CreateCounter<int64>("eviction-fails", "count")

    let allCounters = [ adds; updates; hits; misses; evictions; evictionFails ]

    let creations = Meter.CreateCounter<int64>("creations", "count")
    let disposals = Meter.CreateCounter<int64>("disposals", "count")

    let mkTag name = KeyValuePair<_, obj>("name", name)

    let Add (tag: KeyValuePair<_, _>) = adds.Add(1L, tag)
    let Update (tag: KeyValuePair<_, _>) = updates.Add(1L, tag)
    let Hit (tag: KeyValuePair<_, _>) = hits.Add(1L, tag)
    let Miss (tag: KeyValuePair<_, _>) = misses.Add(1L, tag)
    let Eviction (tag: KeyValuePair<_, _>) = evictions.Add(1L, tag)
    let EvictionFail (tag: KeyValuePair<_, _>) = evictionFails.Add(1L, tag)
    let Created (tag: KeyValuePair<_, _>) = creations.Add(1L, tag)
    let Disposed (tag: KeyValuePair<_, _>) = disposals.Add(1L, tag)

// Currently the Cache emits telemetry for raw cache events: hits, misses, evictions etc.
// This class observes those counters and keeps a snapshot of readings. It is used in tests and can be used to print cache stats in debug mode.
type CacheMetricsListener(tag) =
    let totals = Map [ for counter in CacheMetrics.allCounters -> counter.Name, ref 0L ]

    let incr key v =
        Interlocked.Add(totals[key], v) |> ignore

    let total key = totals[key].Value

    let mutable ratio = Double.NaN

    let updateRatio () =
        ratio <-
            float (total CacheMetrics.hits.Name)
            / float (total CacheMetrics.hits.Name + total CacheMetrics.misses.Name)

    let listener = new MeterListener()

    do

        for instrument in CacheMetrics.allCounters do
            listener.EnableMeasurementEvents instrument

        listener.SetMeasurementEventCallback(fun instrument v tags _ ->
            if tags[0] = tag then
                incr instrument.Name v

                if instrument = CacheMetrics.hits || instrument = CacheMetrics.misses then
                    updateRatio ())

        listener.Start()

    interface IDisposable with
        member _.Dispose() = listener.Dispose()

    member _.GetTotals() =
        [ for k in totals.Keys -> k, total k ] |> Map.ofList

    member _.GetStats() = [ "hit-ratio", ratio ] |> Map.ofList

[<RequireQualifiedAccess>]
type EvictionMode =
    | NoEviction
    | Immediate
    | MailboxProcessor

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type CacheOptions<'Key> =
    {
        /// Total capacity, determines the size of the underlying store.
        TotalCapacity: int

        /// Safety margin size as a percentage of TotalCapacity.
        HeadroomPercentage: int

        /// Mechanism to use for evicting items from the cache.
        EvictionMode: EvictionMode

        /// Comparer to use for keys.
        Comparer: IEqualityComparer<'Key>
    }

module CacheOptions =
    let getDefault () =
        {
            CacheOptions.TotalCapacity = 1024
            CacheOptions.HeadroomPercentage = 50
            CacheOptions.EvictionMode = EvictionMode.MailboxProcessor
            CacheOptions.Comparer = HashIdentity.Structural
        }

    let getReferenceIdentity () =
        {
            CacheOptions.TotalCapacity = 1024
            CacheOptions.HeadroomPercentage = 50
            CacheOptions.EvictionMode = EvictionMode.MailboxProcessor
            CacheOptions.Comparer = HashIdentity.Reference
        }

    let withNoEviction options =
        { options with
            CacheOptions.EvictionMode = EvictionMode.NoEviction
        }

// It is important that this is not a struct, because LinkedListNode holds a reference to it,
// and it holds the reference to that Node, in a circular way.
[<Sealed; NoComparison; NoEquality>]
[<DebuggerDisplay("{ToString()}")>]
type CachedEntity<'Key, 'Value> =
    val mutable private key: 'Key
    val mutable private value: 'Value

    [<DefaultValue(false)>]
    val mutable private node: LinkedListNode<CachedEntity<'Key, 'Value>>

    private new(key, value) = { key = key; value = value }

    member this.Node = this.node
    member this.Key = this.key
    member this.Value = this.value

    member this.UpdateValue(value: 'Value) = this.value <- value

    static member Create(key: 'Key, value: 'Value) =
        let entity = CachedEntity(key, value)
        // The contract is that each CachedEntity always has a LinkedListNode referencing itself.
        entity.node <- LinkedListNode(entity)
        entity

    override this.ToString() = $"{this.Key}"

[<Struct>]
type EvictionQueueMessage<'Entity, 'Target> =
    | Add of 'Entity * 'Target
    | Update of 'Entity

[<Sealed; NoComparison; NoEquality>]
[<DebuggerDisplay("{GetStats()}")>]
type Cache<'Key, 'Value when 'Key: not null> internal (options: CacheOptions<'Key>, ?name) =

    do
        if options.TotalCapacity < 0 then
            invalidArg "Capacity" "Capacity must be positive"

        if options.HeadroomPercentage < 0 then
            invalidArg "HeadroomPercentage" "HeadroomPercentage must be positive"

    let name = defaultArg name (Guid.NewGuid().ToString())

    // Determine evictable headroom as the percentage of total capcity, since we want to not resize the dictionary.
    let headroom =
        int (float options.TotalCapacity * float options.HeadroomPercentage / 100.0)

    // Non-evictable capacity.
    let capacity = options.TotalCapacity - headroom

    let mutable store =
        ConcurrentDictionary<'Key, CachedEntity<'Key, 'Value>>(Environment.ProcessorCount, capacity, options.Comparer)

    let evictionQueue = LinkedList<CachedEntity<'Key, 'Value>>()

    let evicted = Event<_>()
    let evictionFailed = Event<_>()

    let tag = CacheMetrics.mkTag name

    // Track disposal state (0 = not disposed, 1 = disposed)
    let mutable disposed = 0

    let mutable deadKeysCount = 0

    // Keys with unreliable identity can prevent eviction, taking up space in the cache.
    // In such case we rebuild the store to remove dead keys.
    let rebuildStore () =
        let newStore =
            ConcurrentDictionary<'Key, CachedEntity<'Key, 'Value>>(Environment.ProcessorCount, capacity, options.Comparer)

        for entity in evictionQueue do
            newStore.TryAdd(entity.Key, entity) |> ignore

        Interlocked.Exchange(&store, newStore) |> ignore

    let processEvictionMessage msg =
        match msg with
        | EvictionQueueMessage.Add(entity: CachedEntity<_, _>, target) when isNull entity.Node.List ->
            evictionQueue.AddLast(entity.Node)
            // store has been rebuilt while this message was in the queue.
            if store <> target then
                store.TryAdd(entity.Key, entity) |> ignore

            // Evict one immediately if necessary.
            if evictionQueue.Count > capacity then
                let first = nonNull evictionQueue.First
                evictionQueue.Remove(first)

                match store.TryRemove(first.Value.Key) with
                | true, _ ->
                    CacheMetrics.Eviction tag
                    evicted.Trigger()
                | _ ->
                    CacheMetrics.EvictionFail tag
                    evictionFailed.Trigger()
                    deadKeysCount <- deadKeysCount + 1

                    if deadKeysCount > headroom / 2 then
                        rebuildStore ()

        | EvictionQueueMessage.Update entity when entity.Node.List = evictionQueue ->
            // Just move this node to the end of the list.
            evictionQueue.Remove(entity.Node)
            evictionQueue.AddLast(entity.Node)

        // Store updates are not synchronized with evictionQueue. It is possible the entity is no longer in the queue or already added.
        | _ -> ()

    let startEvictionProcessor ct =
        MailboxProcessor.Start(
            (fun mb ->
                let rec processNext () =
                    async {
                        let! message = mb.Receive()
                        processEvictionMessage message
                        return! processNext ()
                    }

                processNext ()),
            ct
        )

    let immediate msg =
        lock evictionQueue <| fun () -> processEvictionMessage msg

    let post, disposeEvictionProcessor =
        match options.EvictionMode with
        | EvictionMode.NoEviction -> ignore, ignore
        | EvictionMode.Immediate -> immediate, ignore
        | EvictionMode.MailboxProcessor ->
            let cts = new CancellationTokenSource()
            let evictionProcessor = startEvictionProcessor cts.Token
            let post = evictionProcessor.Post

            let dispose () =
                cts.Cancel()
                cts.Dispose()
                evictionProcessor.Dispose()

            post, dispose

    do CacheMetrics.Created tag

    member val Evicted = evicted.Publish
    member val EvictionFailed = evictionFailed.Publish

    member _.TryGetValue(key: 'Key, value: outref<'Value>) =
        match store.TryGetValue(key) with
        | true, entity ->
            CacheMetrics.Hit tag
            post (EvictionQueueMessage.Update entity)
            value <- entity.Value
            true
        | _ ->
            CacheMetrics.Miss tag
            value <- Unchecked.defaultof<'Value>
            false

    member _.TryAdd(key: 'Key, value: 'Value) =
        let entity = CachedEntity.Create(key, value)

        let added = store.TryAdd(key, entity)

        if added then
            CacheMetrics.Add tag
            post (EvictionQueueMessage.Add(entity, store))

        added

    member _.GetOrAdd(key, valueFactory) =
        let mutable wasMiss = false

        let makeEntity key =
            wasMiss <- true
            let entity = CachedEntity.Create(key, valueFactory key)
            entity

        let result = store.GetOrAdd(key, makeEntity)

        if wasMiss then
            post (EvictionQueueMessage.Add(result, store))
            CacheMetrics.Add tag
            CacheMetrics.Miss tag
        else
            post (EvictionQueueMessage.Update result)
            CacheMetrics.Hit tag

        result.Value

    member _.AddOrUpdate(key, value) =
        let addValue = CachedEntity.Create(key, value)

        let updateValue (_: 'Key) (oldEntity: CachedEntity<_, _>) =
            oldEntity.UpdateValue(value)
            oldEntity

        let result = store.AddOrUpdate(key, addValue, updateValue)

        // Returned value tells us if the entity was added or updated.
        if Object.ReferenceEquals(addValue, result) then
            CacheMetrics.Add tag
            post (EvictionQueueMessage.Add(addValue, store))
        else
            CacheMetrics.Update tag
            post (EvictionQueueMessage.Update result)

    member _.CreateMetricsListener() = new CacheMetricsListener(tag)

    member _.Dispose() =
        if Interlocked.Exchange(&disposed, 1) = 0 then
            disposeEvictionProcessor ()
            CacheMetrics.Disposed tag

    interface IDisposable with
        member this.Dispose() =
            this.Dispose()
            GC.SuppressFinalize(this)

    // Finalizer to ensure eviction loop is cancelled if Dispose wasn't called.
    override this.Finalize() = this.Dispose()
