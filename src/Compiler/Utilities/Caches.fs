// LinkedList uses nulls, so we need to disable the nullability warnings for this file.
namespace FSharp.Compiler.Caches

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Threading
open System.Diagnostics
open System.Diagnostics.Metrics
open System.Collections.Immutable

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type CacheOptions =
    {
        /// Total capacity, determines the size of the underlying store.
        TotalCapacity: int

        /// Safety margin size as a percentage of TotalCapacity.
        HeadroomPercentage: int
    }

    static member Default =
        {
            TotalCapacity = 128
            HeadroomPercentage = 50
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

// Currently the Cache itself exposes Metrics.Counters that count raw cache events: hits, misses, evictions etc.
// This class observes those counters and keeps a snapshot of readings. For now this is used only to print cache stats in debug mode.
// TODO: We could add some System.Diagnostics.Metrics.Gauge instruments to this class, to get computed stats also exposed as metrics.
type CacheMetrics(cacheId: string) =
    static let meter = new Meter("FSharp.Compiler.Cache")
    static let observedCaches = ConcurrentDictionary<string, CacheMetrics>()

    let adds = meter.CreateCounter<int64>("adds", "count", cacheId)
    let updates = meter.CreateCounter<int64>("updates", "count", cacheId)
    let hits = meter.CreateCounter<int64>("hits", "count", cacheId)
    let misses = meter.CreateCounter<int64>("misses", "count", cacheId)
    let evictions = meter.CreateCounter<int64>("evictions", "count", cacheId)
    let evictionFails = meter.CreateCounter<int64>("eviction-fails", "count", cacheId)
    let allCounters = [ adds; updates; hits; misses; evictions; evictionFails ]

    let totals = Map [ for counter in allCounters -> counter.Name, ref 0L ]

    let incr key v =
        Interlocked.Add(totals[key], v) |> ignore

    let total key = totals[key].Value

    let mutable ratio = Double.NaN

    let updateRatio () =
        ratio <- float (total hits.Name) / float (total hits.Name + total misses.Name)

    let listener = new MeterListener()

    let startListening () =
        for i in allCounters do
            listener.EnableMeasurementEvents i

        listener.SetMeasurementEventCallback(fun instrument v _ _ ->
            incr instrument.Name v

            if instrument = hits || instrument = misses then
                updateRatio ())

        listener.Start()

    member val Adds = adds
    member val Updates = updates
    member val Hits = hits
    member val Misses = misses
    member val Evictions = evictions
    member val EvictionFails = evictionFails

    member this.ObserveMetrics() =
        observedCaches[cacheId] <- this
        startListening ()

    member this.Dispose() =
        observedCaches.TryRemove cacheId |> ignore
        listener.Dispose()

    member _.GetInstanceTotals() =
        [ for k in totals.Keys -> k, total k ] |> Map.ofList

    member _.GetInstanceStats() = [ "hit-ratio", ratio ] |> Map.ofList

    static member val Meter = meter

    static member GetTotals(cacheId) =
        observedCaches[cacheId].GetInstanceTotals()

    static member GetStats(cacheId) =
        observedCaches[cacheId].GetInstanceStats()

module Cache =
    // During testing a lot of compilations are started in app domains and subprocesses.
    // This is a reliable way to pass the override to all of them.
    [<Literal>]
    let private overrideVariable = "FSHARP_CACHE_OVERRIDE"

    /// Use for testing purposes to reduce memory consumption in testhost and its subprocesses.
    let OverrideCapacityForTesting () =
        Environment.SetEnvironmentVariable(overrideVariable, "4096", EnvironmentVariableTarget.Process)

    let applyOverride (capacity: int) =
        match Int32.TryParse(Environment.GetEnvironmentVariable(overrideVariable)) with
        | true, n when capacity > n -> n
        | _ -> capacity

[<Struct>]
type EvictionQueueMessage<'Key, 'Value> =
    | Add of CachedEntity<'Key, 'Value>
    | Update of CachedEntity<'Key, 'Value>

[<Sealed; NoComparison; NoEquality>]
[<DebuggerDisplay("{GetStats()}")>]
type Cache<'Key, 'Value when 'Key: not null> internal (totalCapacity: int, headroom, comparer, name, listen, noEviction) =

    let metrics = new CacheMetrics(name)

    do
        if listen then
            metrics.ObserveMetrics()

    let store =
        ConcurrentDictionary<'Key, CachedEntity<'Key, 'Value>>(Environment.ProcessorCount, totalCapacity, comparer)

    let evictionQueue = LinkedList<CachedEntity<'Key, 'Value>>()

    // Non-evictable capacity.
    let capacity = totalCapacity - headroom

    let evicted = Event<_>()
    let evictionFailed = Event<_>()

    let startEvictionProcessor ct =
        MailboxProcessor.Start(
            (fun mb ->
                let rec processNext () =
                    async {
                        match! mb.Receive() with
                        | EvictionQueueMessage.Add entity ->
                            evictionQueue.AddLast(entity.Node)

                            // Evict one immediately if necessary.
                            if evictionQueue.Count > capacity then
                                let first = nonNull evictionQueue.First

                                match store.TryRemove(first.Value.Key) with
                                | true, _ ->
                                    evictionQueue.Remove(first)
                                    metrics.Evictions.Add 1L
                                    evicted.Trigger()
                                | _ ->
                                    // This should not be possible to happen, but if it does, we want to know.
                                    metrics.EvictionFails.Add 1L
                                    evictionFailed.Trigger()

                        // Store updates are not synchronized. It is possible the entity is no longer in the queue.
                        | EvictionQueueMessage.Update entity when isNull entity.Node.List -> ()

                        | EvictionQueueMessage.Update entity ->
                            // Just move this node to the end of the list.
                            evictionQueue.Remove(entity.Node)
                            evictionQueue.AddLast(entity.Node)

                        return! processNext ()
                    }

                processNext ()),
            ct
        )

    let post, disposeEvictionProcessor =
        if noEviction then ignore, ignore else          
            let cts = new CancellationTokenSource()
            let evictionProcessor = startEvictionProcessor cts.Token
            (fun message -> evictionProcessor.Post(message)),
            (fun () ->
                cts.Cancel()
                cts.Dispose()
                evictionProcessor.Dispose())

    member val Evicted = evicted.Publish
    member val EvictionFailed = evictionFailed.Publish

    member _.TryGetValue(key: 'Key, value: outref<'Value>) =
        match store.TryGetValue(key) with
        | true, entity ->
            metrics.Hits.Add 1L
            post (EvictionQueueMessage.Update entity)
            value <- entity.Value
            true
        | _ ->
            metrics.Misses.Add 1L
            value <- Unchecked.defaultof<'Value>
            false

    member _.TryAdd(key: 'Key, value: 'Value) =
        let entity = CachedEntity.Create(key, value)

        let added = store.TryAdd(key, entity)

        if added then
            metrics.Adds.Add 1L
            post (EvictionQueueMessage.Add entity)

        added

    member _.GetOrAdd(key, valueFactory) =
        let mutable wasMiss = false

        let makeEntity key =
            wasMiss <- true
            let entity = CachedEntity.Create(key, valueFactory key)
            post (EvictionQueueMessage.Add entity)
            entity

        let result = store.GetOrAdd(key, makeEntity)

        if wasMiss then
            metrics.Adds.Add 1L
            metrics.Misses.Add 1L
        else
            metrics.Hits.Add 1L

        post (EvictionQueueMessage.Update result)
        result.Value

    member _.AddOrUpdate(key, value) =
        let addValue = CachedEntity.Create(key, value)

        let updateValue _ (oldEntity: CachedEntity<_, _>) =
            oldEntity.UpdateValue(value)
            oldEntity

        let result = store.AddOrUpdate(key, addValue, updateValue)

        // Returned value tells us if the entity was added or updated.
        if Object.ReferenceEquals(addValue, result) then
            metrics.Adds.Add 1L
            post (EvictionQueueMessage.Add addValue)
        else
            metrics.Updates.Add 1L
            post (EvictionQueueMessage.Update result)

    interface IDisposable with
        member this.Dispose() =
            disposeEvictionProcessor()
            store.Clear()
            metrics.Dispose()

    member this.Dispose() = (this :> IDisposable).Dispose()

    static member Create<'Key, 'Value>(options: CacheOptions, ?comparer: IEqualityComparer<'Key>, ?name, ?observeMetrics, ?noEviction) =
        if options.TotalCapacity < 0 then
            invalidArg "Capacity" "Capacity must be positive"

        if options.HeadroomPercentage < 0 then
            invalidArg "HeadroomPercentage" "HeadroomPercentage must be positive"

        let totalCapacity = Cache.applyOverride options.TotalCapacity
        // Determine evictable headroom as the percentage of total capcity, since we want to not resize the dictionary.
        let headroom =
            int (float options.TotalCapacity * float options.HeadroomPercentage / 100.0)

        let name = defaultArg name (Guid.NewGuid().ToString())
        let observeMetrics = defaultArg observeMetrics false
        let comparer = defaultArg comparer EqualityComparer<'Key>.Default
        let noEviction = defaultArg noEviction false

        let cache =
            new Cache<'Key, 'Value>(totalCapacity, headroom, comparer, name, observeMetrics, noEviction)

        cache
