// LinkedList uses nulls, so we need to disable the nullability warnings for this file.
namespace FSharp.Compiler

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Threading
open System.Diagnostics
open System.Diagnostics.Metrics

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type CacheOptions =
    {
        MaximumCapacity: int
        PercentageToEvict: int
        LevelOfConcurrency: int
    }

    static member Default =
        {
            MaximumCapacity = 1024
            PercentageToEvict = 5
            LevelOfConcurrency = Environment.ProcessorCount
        }

[<Sealed; NoComparison; NoEquality>]
[<DebuggerDisplay("{ToString()}")>]
type CachedEntity<'Key, 'Value> =
    val mutable Key: 'Key
    val mutable Value: 'Value
    val mutable AccessCount: int64
    val mutable Node: LinkedListNode<CachedEntity<'Key, 'Value>>

    new(key, value) =
        {
            Key = key
            Value = value
            AccessCount = 0L
            Node = Unchecked.defaultof<_>
        }

    member this.WithNode() =
        this.Node <- LinkedListNode(this)
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

    static let instrumentedCaches = ConcurrentDictionary<string, CacheMetrics>()

    let readings = ConcurrentDictionary<string, int64 ref>()

#if DEBUG
    let listener = new MeterListener()

    do
        listener.InstrumentPublished <-
            fun i l ->
                if i.Meter = meter && i.Description = cacheId then
                    l.EnableMeasurementEvents(i)

        listener.SetMeasurementEventCallback<int64>(fun k v _ _ -> Interlocked.Add(readings.GetOrAdd(k.Name, ref 0L), v) |> ignore)
        listener.Start()

    member this.Dispose() = listener.Dispose()
#else
    member this.Dispose() = ()
#endif

    member val CacheId = cacheId

    static member val Meter = meter

    member val RecentStats = "-" with get, set

    member this.TryUpdateStats(clearCounts) =
        let stats =
            try
                let ratio =
                    float readings["hits"].Value
                    / float (readings["hits"].Value + readings["misses"].Value)
                    * 100.0

                [
                    for name in readings.Keys do
                        let v = readings[name].Value

                        if v > 0 then
                            $"{name}: {v}"
                ]
                |> String.concat ", "
                |> sprintf "%s | hit ratio: %s %s" this.CacheId (if Double.IsNaN(ratio) then "-" else $"%.1f{ratio}%%")
            with _ ->
                "!"

        if clearCounts then
            for r in readings.Values do
                Interlocked.Exchange(r, 0L) |> ignore

        if stats <> this.RecentStats then
            this.RecentStats <- stats
            true
        else
            false

    static member GetStats(cacheId) =
        instrumentedCaches[cacheId].TryUpdateStats(false) |> ignore
        instrumentedCaches[cacheId].RecentStats

    static member GetStatsUpdateForAllCaches(clearCounts) =
        [
            for i in instrumentedCaches.Values do
                if i.TryUpdateStats(clearCounts) then
                    i.RecentStats
        ]
        |> String.concat "\n"

    static member AddInstrumentation(cacheId) =
        instrumentedCaches[cacheId] <- new CacheMetrics(cacheId)

    static member RemoveInstrumentation(cacheId) =
        instrumentedCaches[cacheId].Dispose()
        instrumentedCaches.TryRemove(cacheId) |> ignore

type EntityPool<'Key, 'Value>(maximumCapacity, cacheId) =
    let pool = ConcurrentBag<CachedEntity<'Key, 'Value>>()
    let mutable created = 0

    let overCapacity =
        CacheMetrics.Meter.CreateCounter<int64>("over-capacity", "count", cacheId)

    member _.Acquire(key, value) =
        match pool.TryTake() with
        | true, entity -> entity.ReUse(key, value)
        | _ ->
            if Interlocked.Increment &created > maximumCapacity then
                overCapacity.Add 1L

            CachedEntity(key, value).WithNode()

    member _.Reclaim(entity: CachedEntity<'Key, 'Value>) =
        if pool.Count < maximumCapacity then
            pool.Add(entity)

[<Sealed; NoComparison; NoEquality>]
[<DebuggerDisplay("{GetStats()}")>]
type Cache<'Key, 'Value when 'Key: not null and 'Key: equality>
    internal (options: CacheOptions, capacity, cts: CancellationTokenSource, ?name) =

    static let mutable cacheId = 0

    let instanceId = defaultArg name $"cache-{Interlocked.Increment(&cacheId)}"

    let hits = CacheMetrics.Meter.CreateCounter<int64>("hits", "count", instanceId)
    let misses = CacheMetrics.Meter.CreateCounter<int64>("misses", "count", instanceId)

    let evictions =
        CacheMetrics.Meter.CreateCounter<int64>("evictions", "count", instanceId)

    let evictionFails =
        CacheMetrics.Meter.CreateCounter<int64>("eviction-fails", "count", instanceId)

    let pool = EntityPool<'Key, 'Value>(capacity, instanceId)

    let store =
        ConcurrentDictionary<'Key, CachedEntity<'Key, 'Value>>(options.LevelOfConcurrency, capacity)

    let evictionQueue = LinkedList<CachedEntity<'Key, 'Value>>()

    let addToEvictionQueue (entity: CachedEntity<'Key, 'Value>) =
        lock evictionQueue
        <| fun () ->
            if isNull entity.Node.List then
                evictionQueue.AddLast(entity.Node)
            else
                assert false

    // Only LRU currrently. We can add other strategies when needed.
    let updateEvictionQueue (entity: CachedEntity<'Key, 'Value>) =
        lock evictionQueue
        <| fun () ->

            let node = entity.Node

            // Sync between store and the eviction queue is not atomic. It might be already evicted or not yet added.
            if node.List = evictionQueue then
                // Just move this node to the end of the list.
                evictionQueue.Remove(node)
                evictionQueue.AddLast(node)

    // If items count exceeds this, evictions ensue.
    let targetCount =
        options.MaximumCapacity
        - int (float options.MaximumCapacity * float options.PercentageToEvict / 100.0)

    do assert (targetCount >= 0)

    let tryEvictOne () =
        match evictionQueue.First with
        | null -> evictionFails.Add 1L
        | first ->
            match store.TryRemove(first.Value.Key) with
            | true, removed ->
                lock evictionQueue <| fun () -> evictionQueue.Remove(removed.Node)
                pool.Reclaim(removed)
                evictions.Add 1L
            | _ -> evictionFails.Add 1L

    let rec backgroundEviction () =
        async {
            let utilization = (float store.Count / float options.MaximumCapacity)
            // So, based on utilization this will scale the delay between 0 and 1 seconds.
            // Worst case scenario would be when 1 second delay happens,
            // if the cache will grow rapidly (or in bursts), it will go beyond the maximum capacity.
            // In this case underlying dictionary will resize, AND we will have to evict items, which will likely be slow.
            // In this case, cache stats should be used to adjust MaximumCapacity and PercentageToEvict.
            let delay = 1000.0 - (1000.0 * utilization)

            if delay > 0.0 then
                do! Async.Sleep(int delay)

            while store.Count > targetCount && evictionQueue.Count > 0 do
                tryEvictOne ()

            return! backgroundEviction ()
        }

    do
        if options.PercentageToEvict > 0 then
            Async.Start(backgroundEviction (), cancellationToken = cts.Token)

    member val Name = instanceId

    member _.TryGetValue(key: 'Key, value: outref<'Value>) =
        match store.TryGetValue(key) with
        | true, cachedEntity ->
            hits.Add 1L
            Interlocked.Increment(&cachedEntity.AccessCount) |> ignore
            updateEvictionQueue cachedEntity
            value <- cachedEntity.Value
            true
        | _ ->
            misses.Add 1L
            value <- Unchecked.defaultof<'Value>
            false

    member _.TryAdd(key: 'Key, value: 'Value) =
        let cachedEntity = pool.Acquire(key, value)

        if store.TryAdd(key, cachedEntity) then
            addToEvictionQueue cachedEntity
            true
        else
            pool.Reclaim(cachedEntity)
            false

    interface IDisposable with
        member this.Dispose() =
            store.Clear()
            cts.Cancel()
            CacheMetrics.RemoveInstrumentation(this.Name)

    member this.Dispose() = (this :> IDisposable).Dispose()

    member this.GetStats() = CacheMetrics.GetStats(this.Name)

module Cache =

    // During testing a lot of compilations are started in app domains and subprocesses.
    // This is a reliable way to pass the override to all of them.
    [<Literal>]
    let private overrideVariable = "FSHARP_CACHE_OVERRIDE"

    /// Use for testing purposes to reduce memory consumption in testhost and its subprocesses.
    let OverrideMaxCapacityForTesting () =
        Environment.SetEnvironmentVariable(overrideVariable, "true", EnvironmentVariableTarget.Process)

    let applyOverride (options: CacheOptions) =
        let capacity =
            match Environment.GetEnvironmentVariable(overrideVariable) with
            | NonNull _ when options.MaximumCapacity > 1024 -> 1024
            | _ -> options.MaximumCapacity

        { options with
            MaximumCapacity = capacity
        }

    let Create<'Key, 'Value when 'Key: not null and 'Key: equality> (options: CacheOptions) =
        let options = applyOverride options
        // Increase expected capacity by the percentage to evict, since we want to not resize the dictionary.
        let capacity =
            options.MaximumCapacity
            + int (float options.MaximumCapacity * float options.PercentageToEvict / 100.0)

        let cts = new CancellationTokenSource()
        let cache = new Cache<'Key, 'Value>(options, capacity, cts)
        CacheMetrics.AddInstrumentation cache.Name |> ignore
        cache
