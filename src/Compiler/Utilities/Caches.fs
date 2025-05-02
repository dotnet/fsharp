// LinkedList uses nulls, so we need to disable the nullability warnings for this file.
namespace FSharp.Compiler.Caches

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Threading
open System.Diagnostics
open System.Diagnostics.Metrics

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
    val mutable Key: 'Key
    val mutable Value: 'Value
    val mutable AccessCount: int64
    val mutable Node: LinkedListNode<CachedEntity<'Key, 'Value>> voption

    new(key, value) =
        {
            Key = key
            Value = value
            AccessCount = 0L
            Node = ValueNone
        }

    // This is one time initialization, outside of the constructor because of circular reference.
    // The contract is that each CachedEntity that the EntityPool produces, has Node assigned.
    member this.WithNode() =
        this.Node <- ValueSome(LinkedListNode this)
        this

    member this.ReUse(key, value) =
        this.Key <- key
        this.Value <- value
        this.AccessCount <- 0L
        this

    override this.ToString() = $"{this.Key}"

// Currently the Cache itself exposes Metrics.Counters that count raw cache events: hits, misses, evictions etc.
// This class observes those counters and keeps a snapshot of readings. For now this is used only to print cache stats in debug mode.
// TODO: We could add some System.Diagnostics.Metrics.Gauge instruments to this class, to get computed stats also exposed as metrics.
type CacheMetrics(cacheId) =
    static let meter = new Meter("FSharp.Compiler.Cache")

    static let observedCaches = ConcurrentDictionary<string, CacheMetrics>()

    let readings = ConcurrentDictionary<string, int64 ref>()

    let listener = new MeterListener()

    do
        listener.InstrumentPublished <-
            fun i l ->
                if i.Meter = meter && i.Description = cacheId then
                    l.EnableMeasurementEvents(i)

        listener.SetMeasurementEventCallback<int64>(fun k v _ _ -> Interlocked.Add(readings.GetOrAdd(k.Name, ref 0L), v) |> ignore)
        listener.Start()

    member this.Dispose() = listener.Dispose()

    member val CacheId = cacheId

    static member val Meter = meter

    member val RecentStats = "-" with get, set

    member this.TryUpdateStats(clearCounts) =
        let ratio =
            try
                float readings["hits"].Value
                / float (readings["hits"].Value + readings["misses"].Value)
                * 100.0
            with _ ->
                Double.NaN

        let stats =
            [
                for name in readings.Keys do
                    let v = readings[name].Value

                    if v > 0 then
                        $"{name}: {v}"
            ]
            |> String.concat ", "
            |> sprintf "%s | hit ratio: %s %s" this.CacheId (if Double.IsNaN(ratio) then "-" else $"%.1f{ratio}%%")

        if clearCounts then
            for r in readings.Values do
                Interlocked.Exchange(r, 0L) |> ignore

        if stats <> this.RecentStats then
            this.RecentStats <- stats
            true
        else
            false

    // TODO: Should return a Map, not a string
    static member GetStats(cacheId) =
        observedCaches[cacheId].TryUpdateStats(false) |> ignore
        observedCaches[cacheId].RecentStats

    static member GetStatsUpdateForAllCaches(clearCounts) =
        [
            for i in observedCaches.Values do
                if i.TryUpdateStats(clearCounts) then
                    i.RecentStats
        ]
        |> String.concat "\n"

    static member AddInstrumentation(cacheId) =
        if observedCaches.ContainsKey cacheId then
            invalidArg "cacheId" $"cache with name {cacheId} already exists"

        observedCaches[cacheId] <- new CacheMetrics(cacheId)

    static member RemoveInstrumentation(cacheId) =
        observedCaches[cacheId].Dispose()
        observedCaches.TryRemove(cacheId) |> ignore

// Creates and after reclaiming holds entities for reuse.
// More than totalCapacity can be created, but it will hold for reuse at most totalCapacity.
type EntityPool<'Key, 'Value>(totalCapacity, cacheId) =
    let pool = ConcurrentBag<CachedEntity<'Key, 'Value>>()
    let mutable created = 0

    let overCapacity =
        CacheMetrics.Meter.CreateCounter<int64>("over-capacity", "count", cacheId)

    member _.Acquire(key, value) =
        match pool.TryTake() with
        | true, entity -> entity.ReUse(key, value)
        | _ ->
            if Interlocked.Increment &created > totalCapacity then
                overCapacity.Add 1L

            // Associate a LinkedListNode with freshly created entity.
            // This is a one time initialization.
            CachedEntity(key, value).WithNode()

    member _.Reclaim(entity: CachedEntity<'Key, 'Value>) =
        if pool.Count < totalCapacity then
            pool.Add(entity)

module Cache =
    // During testing a lot of compilations are started in app domains and subprocesses.
    // This is a reliable way to pass the override to all of them.
    [<Literal>]
    let private overrideVariable = "FSHARP_CACHE_OVERRIDE"

    /// Use for testing purposes to reduce memory consumption in testhost and its subprocesses.
    let OverrideCapacityForTesting () =
        Environment.SetEnvironmentVariable(overrideVariable, "true", EnvironmentVariableTarget.Process)

    let applyOverride (capacity: int) =
        match Environment.GetEnvironmentVariable(overrideVariable) with
        | NonNull _ when capacity > 4096 -> 4096
        | _ -> capacity

[<Struct>]
type EvictionQueueMessage<'Key, 'Value> =
    | Add of CachedEntity<'Key, 'Value>
    | Update of CachedEntity<'Key, 'Value>

[<Sealed; NoComparison; NoEquality>]
[<DebuggerDisplay("{GetStats()}")>]
type Cache<'Key, 'Value when 'Key: not null and 'Key: equality> internal (totalCapacity, headroom, ?name, ?observeMetrics) =

    let instanceId = defaultArg name (Guid.NewGuid().ToString())

    let observeMetrics = defaultArg observeMetrics false

    do
        if observeMetrics then
            CacheMetrics.AddInstrumentation instanceId

    let meter = CacheMetrics.Meter
    let hits = meter.CreateCounter<int64>("hits", "count", instanceId)
    let misses = meter.CreateCounter<int64>("misses", "count", instanceId)
    let evictions = meter.CreateCounter<int64>("evictions", "count", instanceId)

    let evictionFails =
        meter.CreateCounter<int64>("eviction-fails", "count", instanceId)

    let pool = EntityPool<'Key, 'Value>(totalCapacity, instanceId)

    let store =
        ConcurrentDictionary<'Key, CachedEntity<'Key, 'Value>>(Environment.ProcessorCount, totalCapacity)

    let evictionQueue = LinkedList<CachedEntity<'Key, 'Value>>()

    // Non-evictable capacity.
    let capacity = totalCapacity - headroom

    let evicted = Event<_>()

    let cts = new CancellationTokenSource()

    let evictionProcessor =
        MailboxProcessor.Start(
            (fun mb ->
                let rec processNext () =
                    async {
                        match! mb.Receive() with
                        | EvictionQueueMessage.Add entity ->

                            assert entity.Node.IsSome

                            evictionQueue.AddLast(entity.Node.Value)

                            // Evict one immediately if necessary.
                            while evictionQueue.Count > capacity do
                                let first = nonNull evictionQueue.First

                                match store.TryRemove(first.Value.Key) with
                                | true, removed ->
                                    evictionQueue.Remove(first)
                                    pool.Reclaim(removed)
                                    evictions.Add 1L
                                    evicted.Trigger()
                                | _ -> evictionFails.Add 1L

                        | EvictionQueueMessage.Update entity ->
                            entity.AccessCount <- entity.AccessCount + 1L

                            assert entity.Node.IsSome

                            let node = entity.Node.Value
                            assert (node.List = evictionQueue)
                            // Just move this node to the end of the list.
                            evictionQueue.Remove(node)
                            evictionQueue.AddLast(node)

                        return! processNext ()
                    }

                processNext ()),
            cts.Token
        )

    member val Evicted = evicted.Publish

    member val Name = instanceId

    member _.TryGetValue(key: 'Key, value: outref<'Value>) =
        match store.TryGetValue(key) with
        | true, cachedEntity ->
            hits.Add 1L
            evictionProcessor.Post(EvictionQueueMessage.Update cachedEntity)
            value <- cachedEntity.Value
            true
        | _ ->
            misses.Add 1L
            value <- Unchecked.defaultof<'Value>
            false

    member _.TryAdd(key: 'Key, value: 'Value) =
        let cachedEntity = pool.Acquire(key, value)

        if store.TryAdd(key, cachedEntity) then
            evictionProcessor.Post(EvictionQueueMessage.Add cachedEntity)
            true
        else
            pool.Reclaim(cachedEntity)
            false

    interface IDisposable with
        member this.Dispose() =
            cts.Cancel()
            cts.Dispose()
            evictionProcessor.Dispose()
            store.Clear()

            if observeMetrics then
                CacheMetrics.RemoveInstrumentation instanceId

    member this.Dispose() = (this :> IDisposable).Dispose()

    member this.GetStats() = CacheMetrics.GetStats(this.Name)

    static member Create<'Key, 'Value>(options: CacheOptions, ?name, ?observeMetrics) =
        if options.TotalCapacity < 0 then
            invalidArg "Capacity" "Capacity must be positive"

        if options.HeadroomPercentage < 0 then
            invalidArg "HeadroomPercentage" "HeadroomPercentage must be positive"

        let totalCapacity = Cache.applyOverride options.TotalCapacity
        // Determine evictable headroom as the percentage of total capcity, since we want to not resize the dictionary.
        let headroom =
            int (float options.TotalCapacity * float options.HeadroomPercentage / 100.0)

        let cache =
            new Cache<_, _>(totalCapacity, headroom, ?name = name, ?observeMetrics = observeMetrics)

        cache
