// LinkedList uses nulls, so we need to disable the nullability warnings for this file.
namespace FSharp.Compiler

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Threading
open System.Diagnostics
open System.Diagnostics.Metrics

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type CachingStrategy =
    | LRU
    | LFU

[<Struct; RequireQualifiedAccess; NoComparison>]
type EvictionMethod =
    | Blocking
    | Background
    | NoEviction

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type CacheOptions =
    {
        MaximumCapacity: int
        PercentageToEvict: int
        Strategy: CachingStrategy
        EvictionMethod: EvictionMethod
        LevelOfConcurrency: int
    }

    static member Default =
        {
            MaximumCapacity = 1024
            PercentageToEvict = 5
            Strategy = CachingStrategy.LRU
            LevelOfConcurrency = Environment.ProcessorCount
            EvictionMethod = EvictionMethod.Blocking
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

type IEvictionQueue<'Key, 'Value> =
    abstract member Acquire: 'Key * 'Value -> CachedEntity<'Key, 'Value>
    abstract member Add: CachedEntity<'Key, 'Value> * CachingStrategy -> unit
    abstract member Update: CachedEntity<'Key, 'Value> -> unit
    abstract member GetKeysToEvict: int -> 'Key[]
    abstract member Remove: CachedEntity<'Key, 'Value> -> unit

[<Sealed; NoComparison; NoEquality>]
type EvictionQueue<'Key, 'Value>(strategy: CachingStrategy, maximumCapacity, overCapacity: Event<_>) =

    let list = LinkedList<CachedEntity<'Key, 'Value>>()
    let pool = ConcurrentBag<CachedEntity<'Key, 'Value>>()
    let mutable created = 0

    interface IEvictionQueue<'Key, 'Value> with

        member _.Acquire(key, value) =
            match pool.TryTake() with
            | true, entity -> entity.ReUse(key, value)
            | _ ->
                if Interlocked.Increment &created > maximumCapacity then
                    overCapacity.Trigger()

                CachedEntity(key, value).WithNode()

        member _.Add(entity: CachedEntity<'Key, 'Value>, strategy) =
            lock list
            <| fun () ->
                if isNull entity.Node.List then
                    match strategy with
                    | CachingStrategy.LRU -> list.AddLast(entity.Node)
                    | CachingStrategy.LFU -> list.AddLast(entity.Node)
        // list.AddFirst(entity.Node)

        member _.Update(entity: CachedEntity<'Key, 'Value>) =
            lock list
            <| fun () ->
                Interlocked.Increment(&entity.AccessCount) |> ignore

                let node = entity.Node

                // Sync between store and the eviction queue is not atomic. It might be already evicted or not yet added.
                if node.List = list then

                    match strategy with
                    | CachingStrategy.LRU ->
                        // Just move this node to the end of the list.
                        list.Remove(node)
                        list.AddLast(node)
                    | CachingStrategy.LFU ->
                        // Bubble up the node in the list, linear time.
                        // TODO: frequency list approach would be faster.
                        let rec bubbleUp (current: LinkedListNode<CachedEntity<'Key, 'Value>>) =
                            match current.Next with
                            | NonNull next when next.Value.AccessCount < entity.AccessCount -> bubbleUp next
                            | _ -> current

                        let next = bubbleUp node

                        if next <> node then
                            list.Remove(node)
                            list.AddAfter(next, node)

        member _.GetKeysToEvict(count) =
            lock list
            <| fun () -> list |> Seq.map _.Key |> Seq.truncate count |> Seq.toArray

        member this.Remove(entity: CachedEntity<_, _>) =
            lock list <| fun () -> list.Remove(entity.Node)
            // Return to the pool for reuse.
            if pool.Count < maximumCapacity then
                pool.Add(entity)

    member _.Count = list.Count

    static member NoEviction =
        { new IEvictionQueue<'Key, 'Value> with
            member _.Acquire(key, value) = CachedEntity(key, value)
            member _.Add(_, _) = ()

            member _.Update(entity) =
                Interlocked.Increment(&entity.AccessCount) |> ignore

            member _.GetKeysToEvict(_) = [||]
            member _.Remove(_) = ()
        }

type ICacheEvents =
    [<CLIEvent>]
    abstract member CacheHit: IEvent<unit>

    [<CLIEvent>]
    abstract member CacheMiss: IEvent<unit>

    [<CLIEvent>]
    abstract member Eviction: IEvent<unit>

    [<CLIEvent>]
    abstract member EvictionFail: IEvent<unit>

    [<CLIEvent>]
    abstract member OverCapacity: IEvent<unit>

[<Sealed; NoComparison; NoEquality>]
[<DebuggerDisplay("{GetStats()}")>]
type Cache<'Key, 'Value when 'Key: not null and 'Key: equality> internal (options: CacheOptions, capacity, cts: CancellationTokenSource) =

    let cacheHit = Event<unit>()
    let cacheMiss = Event<unit>()
    let eviction = Event<unit>()
    let evictionFail = Event<unit>()
    let overCapacity = Event<unit>()

    let store =
        ConcurrentDictionary<'Key, CachedEntity<'Key, 'Value>>(options.LevelOfConcurrency, capacity)

    let evictionQueue: IEvictionQueue<'Key, 'Value> =
        match options.EvictionMethod with
        | EvictionMethod.NoEviction -> EvictionQueue.NoEviction
        | _ -> EvictionQueue(options.Strategy, capacity, overCapacity)

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
                eviction.Trigger()
            | _ ->
                failwith "eviction fail"
                evictionFail.Trigger()

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

    member _.TryGetValue(key: 'Key, value: outref<'Value>) =
        match store.TryGetValue(key) with
        | true, cachedEntity ->
            cacheHit.Trigger()
            evictionQueue.Update(cachedEntity)
            value <- cachedEntity.Value
            true
        | _ ->
            cacheMiss.Trigger()
            value <- Unchecked.defaultof<'Value>
            false

    member _.TryAdd(key: 'Key, value: 'Value) =
        if options.EvictionMethod.IsBlocking then
            tryEvictItems ()

        let cachedEntity = evictionQueue.Acquire(key, value)

        if store.TryAdd(key, cachedEntity) then
            evictionQueue.Add(cachedEntity, options.Strategy)
            true
        else
            false

    member _.AddOrUpdate(key: 'Key, value: 'Value) =
        if options.EvictionMethod.IsBlocking then
            tryEvictItems ()

        let entity =
            store.AddOrUpdate(
                key,
                (fun _ -> evictionQueue.Acquire(key, value)),
                (fun _ (current: CachedEntity<_, _>) ->
                    current.Value <- value
                    current)
            )

        evictionQueue.Add(entity, options.Strategy)

    interface ICacheEvents with

        [<CLIEvent>]
        member val CacheHit = cacheHit.Publish

        [<CLIEvent>]
        member val CacheMiss = cacheMiss.Publish

        [<CLIEvent>]
        member val Eviction = eviction.Publish

        [<CLIEvent>]
        member val EvictionFail = evictionFail.Publish

        [<CLIEvent>]
        member val OverCapacity = overCapacity.Publish

    interface IDisposable with
        member this.Dispose() =
            cts.Cancel()
            CacheInstrumentation.RemoveInstrumentation(this)
            GC.SuppressFinalize(this)

    member this.Dispose() = (this :> IDisposable).Dispose()

    override this.Finalize() : unit = this.Dispose()

    static member Create<'Key, 'Value>(options: CacheOptions) =
        // Increase expected capacity by the percentage to evict, since we want to not resize the dictionary.
        let capacity =
            options.MaximumCapacity
            + int (float options.MaximumCapacity * float options.PercentageToEvict / 100.0)

        let cts = new CancellationTokenSource()
        let cache = new Cache<'Key, 'Value>(options, capacity, cts)
        CacheInstrumentation.AddInstrumentation cache |> ignore
        cache

    member this.GetStats() = CacheInstrumentation.GetStats(this)

and CacheInstrumentation(cache: ICacheEvents) =
    static let mutable cacheId = 0

    static let instrumentedCaches = ConcurrentDictionary<ICacheEvents, CacheInstrumentation>()

    static let meter = new Meter(nameof CacheInstrumentation)
    let hits = meter.CreateCounter<int64>("hits")
    let misses = meter.CreateCounter<int64>("misses")
    let evictions = meter.CreateCounter<int64>("evictions")
    let evictionFails = meter.CreateCounter<int64>("eviction-fails")
    let overCapacity = meter.CreateCounter<int64>("over-capacity")

    do
        cache.CacheHit.Add <| fun _ -> hits.Add(1L)
        cache.CacheMiss.Add <| fun _ -> misses.Add(1L)
        cache.Eviction.Add <| fun _ -> evictions.Add(1L)
        cache.EvictionFail.Add <| fun _ -> evictionFails.Add(1L)
        cache.OverCapacity.Add <| fun _ -> overCapacity.Add(1L)

    let current = ConcurrentDictionary<Instrument, int64 ref>()

    let listener =
        new MeterListener(
            InstrumentPublished =
                fun i l ->
                    if i.Meter = meter then
                        l.EnableMeasurementEvents(i)
        )

    do
        listener.SetMeasurementEventCallback<int64>(fun k v _ _ -> Interlocked.Add(current.GetOrAdd(k, ref 0L), v) |> ignore)
        listener.Start()

    member val CacheId = $"cache-{Interlocked.Increment(&cacheId)}"

    member val RecentStats = "-" with get, set

    member this.TryUpdateStats(clearCounts) =
        let stats =
            try
                let ratio =
                    float current[hits].Value / float (current[hits].Value + current[misses].Value) * 100.0

                [ for i in current.Keys do
                    let v = current[i].Value
                    if v > 0 then $"{i.Name}: {v}" ]
                |> String.concat ", "
                |> sprintf "%s | hit ratio: %s %s" this.CacheId (if Double.IsNaN(ratio) then "-" else $"%.1f{ratio}%%")
            with _ ->
                "!"

        if clearCounts then
            for r in current.Values do
                Interlocked.Exchange(r, 0L) |> ignore

        if stats <> this.RecentStats then
            this.RecentStats <- stats
            true
        else
            false

    member this.Dispose() =
        listener.Dispose()

    static member GetStats(cache: ICacheEvents) =
        instrumentedCaches[cache].TryUpdateStats(false) |> ignore
        instrumentedCaches[cache].RecentStats

    static member GetStatsUpdateForAllCaches(clearCounts) =
        [
            for i in instrumentedCaches.Values do
                if i.TryUpdateStats(clearCounts) then
                    i.RecentStats
        ]
        |> String.concat "\n"

    static member AddInstrumentation(cache: ICacheEvents) =
        instrumentedCaches[cache] <- new CacheInstrumentation(cache)

    static member RemoveInstrumentation(cache: ICacheEvents) =
        instrumentedCaches[cache].Dispose()
        instrumentedCaches.TryRemove(cache) |> ignore
