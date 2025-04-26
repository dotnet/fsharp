// LinkedList uses nulls, so we need to disable the nullability warnings for this file.
namespace FSharp.Compiler

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks
open System.Diagnostics
open System.Diagnostics.Metrics

open FSharp.Compiler.Diagnostics

[<Struct; RequireQualifiedAccess; NoComparison>]
type EvictionMethod =
    | Background
    | NoEviction

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type CacheOptions =
    {
        MaximumCapacity: int
        PercentageToEvict: int
        EvictionMethod: EvictionMethod
        LevelOfConcurrency: int
    }

    static member Default =
        {
            MaximumCapacity = 1024
            PercentageToEvict = 5
            LevelOfConcurrency = Environment.ProcessorCount
            EvictionMethod = EvictionMethod.Background
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

module CacheMetrics =
    let meter = new Meter("FSharp.Compiler.Cache")

type CacheMetrics(cacheId) =

    static let instrumentedCaches = ConcurrentDictionary<string, CacheMetrics>()

    let readings = ConcurrentDictionary<string, int64 ref>()

#if DEBUG
    let listener =
        new MeterListener(
            InstrumentPublished =
                fun i l ->
                    if i.Meter = CacheMetrics.meter && i.Description = cacheId then
                        l.EnableMeasurementEvents(i)
        )

    do
        listener.SetMeasurementEventCallback<int64>(fun k v _ _ -> Interlocked.Add(readings.GetOrAdd(k.Name, ref 0L), v) |> ignore)
        listener.Start()

    member this.Dispose() = listener.Dispose()
#else
    member this.Dispose() = ()
#endif

    member val CacheId = cacheId

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
        CacheMetrics.meter.CreateCounter<int64>("over-capacity", "count", cacheId)

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

type IEvictionQueue<'Key, 'Value> =
    abstract member Add: CachedEntity<'Key, 'Value> -> unit
    abstract member Update: CachedEntity<'Key, 'Value> -> unit
    abstract member GetKeysToEvict: int -> 'Key[]
    abstract member Remove: CachedEntity<'Key, 'Value> -> unit

[<Sealed; NoComparison; NoEquality>]
type EvictionQueue<'Key, 'Value>() =

    let list = LinkedList<CachedEntity<'Key, 'Value>>()

    interface IEvictionQueue<'Key, 'Value> with

        member _.Add(entity: CachedEntity<'Key, 'Value>) =
            lock list
            <| fun () ->
                if isNull entity.Node.List then
                    list.AddLast(entity.Node)
                else
                    assert false

        member _.Update(entity: CachedEntity<'Key, 'Value>) =
            lock list
            <| fun () ->
                Interlocked.Increment(&entity.AccessCount) |> ignore

                let node = entity.Node

                // Sync between store and the eviction queue is not atomic. It might be already evicted or not yet added.
                if node.List = list then
                    // Just move this node to the end of the list.
                    list.Remove(node)
                    list.AddLast(node)

        member _.GetKeysToEvict(count) =
            lock list
            <| fun () -> list |> Seq.map _.Key |> Seq.truncate count |> Seq.toArray

        member this.Remove(entity: CachedEntity<_, _>) =
            lock list
            <| fun () ->
                if entity.Node.List = list then
                    list.Remove(entity.Node)

    member _.Count = list.Count

    static member NoEviction =
        { new IEvictionQueue<'Key, 'Value> with
            member _.Add(_) = ()

            member _.Update(entity) =
                Interlocked.Increment(&entity.AccessCount) |> ignore

            member _.GetKeysToEvict(_) = [||]
            member _.Remove(_) = ()
        }

[<Sealed; NoComparison; NoEquality>]
[<DebuggerDisplay("{GetStats()}")>]
type Cache<'Key, 'Value when 'Key: not null and 'Key: equality>
    internal (options: CacheOptions, capacity, cts: CancellationTokenSource, ?name) =

    static let mutable cacheId = 0

    let instanceId = defaultArg name $"cache-{Interlocked.Increment(&cacheId)}"

    let hits = CacheMetrics.meter.CreateCounter<int64>("hits", "count", instanceId)
    let misses = CacheMetrics.meter.CreateCounter<int64>("misses", "count", instanceId)

    let evictions =
        CacheMetrics.meter.CreateCounter<int64>("evictions", "count", instanceId)

    let evictionFails =
        CacheMetrics.meter.CreateCounter<int64>("eviction-fails", "count", instanceId)

    let eviction = Event<'Value>()

    let pool = EntityPool<'Key, 'Value>(capacity, instanceId)

    let store =
        ConcurrentDictionary<'Key, CachedEntity<'Key, 'Value>>(options.LevelOfConcurrency, capacity)

    let evictionQueue: IEvictionQueue<'Key, 'Value> =
        match options.EvictionMethod with
        | EvictionMethod.NoEviction -> EvictionQueue.NoEviction
        | _ -> EvictionQueue()

    let tryEvictItems () =
        let count =
            if store.Count > options.MaximumCapacity then
                (store.Count - options.MaximumCapacity)
                + int (float options.MaximumCapacity * float options.PercentageToEvict / 100.0)
            else
                0

        for key in evictionQueue.GetKeysToEvict(count) do
            match store.TryRemove(key) with
            | true, removed ->
                evictionQueue.Remove(removed)
                pool.Reclaim(removed)
                eviction.Trigger(removed.Value)
                evictions.Add 1L
            | _ -> evictionFails.Add 1L

    let rec backgroundEviction () =
        async {
            tryEvictItems ()

            let utilization = (float store.Count / float options.MaximumCapacity)
            // So, based on utilization this will scale the delay between 0 and 1 seconds.
            // Worst case scenario would be when 1 second delay happens,
            // if the cache will grow rapidly (or in bursts), it will go beyond the maximum capacity.
            // In this case underlying dictionary will resize, AND we will have to evict items, which will likely be slow.
            // In this case, cache stats should be used to adjust MaximumCapacity and PercentageToEvict.
            let delay = 1000.0 - (1000.0 * utilization)

            if delay > 0.0 then
                do! Async.Sleep(int delay)

            return! backgroundEviction ()
        }

    do
        if options.EvictionMethod = EvictionMethod.Background then
            Async.Start(backgroundEviction (), cancellationToken = cts.Token)

    member val Name = instanceId

    member _.TryGetValue(key: 'Key, value: outref<'Value>) =
        match store.TryGetValue(key) with
        | true, cachedEntity ->
            hits.Add 1L
            evictionQueue.Update(cachedEntity)
            value <- cachedEntity.Value
            true
        | _ ->
            misses.Add 1L
            value <- Unchecked.defaultof<'Value>
            false

    member _.TryAdd(key: 'Key, value: 'Value) =
        let cachedEntity = pool.Acquire(key, value)

        if store.TryAdd(key, cachedEntity) then
            evictionQueue.Add(cachedEntity)
            true
        else
            pool.Reclaim(cachedEntity)
            false

    member this.GetOrCreate(key: 'Key, valueFactory: 'Key -> 'Value) =
        match this.TryGetValue(key) with
        | true, value -> value
        | _ ->
            let value = valueFactory key
            this.TryAdd(key, value) |> ignore
            value

    [<CLIEvent>]
    member val ValueEvicted = eviction.Publish

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
