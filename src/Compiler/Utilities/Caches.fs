// LinkedList uses nulls, so we need to disable the nullability warnings for this file.
namespace FSharp.Compiler.Caches

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Threading
open System.Diagnostics
open System.Diagnostics.Metrics

module CacheMetrics =
    let Meter = FSharp.Compiler.Diagnostics.Metrics.Meter
    let adds = Meter.CreateCounter<int64>("adds", "count")
    let updates = Meter.CreateCounter<int64>("updates", "count")
    let hits = Meter.CreateCounter<int64>("hits", "count")
    let misses = Meter.CreateCounter<int64>("misses", "count")
    let evictions = Meter.CreateCounter<int64>("evictions", "count")
    let evictionFails = Meter.CreateCounter<int64>("eviction-fails", "count")

    let allCounters = [ adds; updates; hits; misses; evictions; evictionFails ]

    let creations = Meter.CreateCounter<int64>("creations", "count")
    let disposals = Meter.CreateCounter<int64>("disposals", "count")

    let mutable private nextCacheId = 0

    let mkTags (name: string) =
        let cacheId = Interlocked.Increment &nextCacheId
        // Avoid TagList(ReadOnlySpan<...>) to support net472 runtime
        let mutable tags = TagList()
        tags.Add("name", box name)
        tags.Add("cacheId", box cacheId)
        tags

    let Add (tags: inref<TagList>) = adds.Add(1L, &tags)
    let Update (tags: inref<TagList>) = updates.Add(1L, &tags)
    let Hit (tags: inref<TagList>) = hits.Add(1L, &tags)
    let Miss (tags: inref<TagList>) = misses.Add(1L, &tags)
    let Eviction (tags: inref<TagList>) = evictions.Add(1L, &tags)
    let EvictionFail (tags: inref<TagList>) = evictionFails.Add(1L, &tags)
    let Created (tags: inref<TagList>) = creations.Add(1L, &tags)
    let Disposed (tags: inref<TagList>) = disposals.Add(1L, &tags)

    type Stats() =
        let totals = Map [ for counter in allCounters -> counter.Name, ref 0L ]
        let total key = totals[key].Value

        let mutable ratio = Double.NaN

        let updateRatio () =
            ratio <- float (total hits.Name) / float (total hits.Name + total misses.Name)

        member _.Incr key v =
            assert (totals.ContainsKey key)
            Interlocked.Add(totals[key], v) |> ignore

            if key = hits.Name || key = misses.Name then
                updateRatio ()

        member _.GetTotals() =
            [ for k in totals.Keys -> k, total k ] |> Map.ofList

        member _.Ratio = ratio

        override _.ToString() =
            let parts =
                [
                    for kv in totals do
                        yield $"{kv.Key}={kv.Value.Value}"
                    if not (Double.IsNaN ratio) then
                        yield $"hit-ratio={ratio:P2}"
                ]

            String.Join(", ", parts)

    let statsByName = ConcurrentDictionary<string, Stats>()

    let getStatsByName name =
        statsByName.GetOrAdd(name, fun _ -> Stats())

    let ListenToAll () =
        let listener = new MeterListener()

        for instrument in allCounters do
            listener.EnableMeasurementEvents instrument

        listener.SetMeasurementEventCallback(fun instrument v tags _ ->
            match tags[0].Value with
            | :? string as name ->
                let stats = getStatsByName name
                stats.Incr instrument.Name v
            | _ -> assert false)

        listener.Start()
        listener :> IDisposable

    let StatsToString () =
        let headers = [ "Cache name"; "hit-ratio" ] @ (allCounters |> List.map _.Name)

        let rows =
            [
                for kv in statsByName do
                    let name = kv.Key
                    let stats = kv.Value
                    let totals = stats.GetTotals()

                    [
                        yield name
                        yield $"{stats.Ratio:P2}"
                        for c in allCounters do
                            yield $"{totals[c.Name]}"
                    ]
            ]

        FSharp.Compiler.Diagnostics.Metrics.printTable headers rows

    let CaptureStatsAndWriteToConsole () =
        let listener = ListenToAll()

        { new IDisposable with
            member _.Dispose() =
                listener.Dispose()
                Console.WriteLine(StatsToString())
        }

    [<Sealed>]
    type CacheMetricsListener(cacheTags: TagList, ?nameOnlyFilter: string) =

        let stats = Stats()
        let listener = new MeterListener()

        do
            for instrument in allCounters do
                listener.EnableMeasurementEvents instrument

            listener.SetMeasurementEventCallback(fun instrument v tags _ ->
                let shouldIncrement =
                    match nameOnlyFilter with
                    | Some filterName ->
                        match tags[0].Value with
                        | :? string as name when name = filterName -> true
                        | _ -> false
                    | None -> tags[0] = cacheTags[0] && tags[1] = cacheTags[1]

                if shouldIncrement then
                    stats.Incr instrument.Name v)

            listener.Start()

        /// Creates a listener that aggregates metrics across all cache instances with the given name.
        new(cacheName: string) = new CacheMetricsListener(TagList(), nameOnlyFilter = cacheName)

        interface IDisposable with
            member _.Dispose() = listener.Dispose()

        /// Gets the current totals for each metric type.
        member _.GetTotals() = stats.GetTotals()

        /// Gets the current hit ratio (hits / (hits + misses)).
        member _.Ratio = stats.Ratio

        /// Gets the total number of cache hits.
        member _.Hits = stats.GetTotals().[hits.Name]

        /// Gets the total number of cache misses.
        member _.Misses = stats.GetTotals().[misses.Name]

        override _.ToString() = stats.ToString()

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
    let forceImmediate =
        try
            Environment.GetEnvironmentVariable("FSharp_CacheEvictionImmediate") <> null
        with _ ->
            false

    let defaultEvictionMode =
        if forceImmediate then
            EvictionMode.Immediate
        else
            EvictionMode.MailboxProcessor

    let getDefault comparer =
        {
            CacheOptions.TotalCapacity = 1024
            CacheOptions.HeadroomPercentage = 50
            CacheOptions.EvictionMode = defaultEvictionMode
            CacheOptions.Comparer = comparer
        }

    let getReferenceIdentity () = getDefault HashIdentity.Reference

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
[<DebuggerDisplay("{DebugDisplay()}")>]
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

    let tags = CacheMetrics.mkTags name

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
                    CacheMetrics.Eviction &tags
                    evicted.Trigger()
                | _ ->
                    CacheMetrics.EvictionFail &tags
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

#if DEBUG
    let debugListener = new CacheMetrics.CacheMetricsListener(tags)
#endif

    do CacheMetrics.Created &tags

    member val Evicted = evicted.Publish
    member val EvictionFailed = evictionFailed.Publish

    member _.TryGetValue(key: 'Key, value: outref<'Value>) =
        match store.TryGetValue(key) with
        | true, entity ->
            CacheMetrics.Hit &tags
            post (EvictionQueueMessage.Update entity)
            value <- entity.Value
            true
        | _ ->
            CacheMetrics.Miss &tags
            value <- Unchecked.defaultof<'Value>
            false

    member _.TryAdd(key: 'Key, value: 'Value) =
        let entity = CachedEntity.Create(key, value)

        let added = store.TryAdd(key, entity)

        if added then
            CacheMetrics.Add &tags
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
            CacheMetrics.Add &tags
            CacheMetrics.Miss &tags
        else
            post (EvictionQueueMessage.Update result)
            CacheMetrics.Hit &tags

        result.Value

    member _.AddOrUpdate(key, value) =
        let addValue = CachedEntity.Create(key, value)

        let updateValue (_: 'Key) (oldEntity: CachedEntity<_, _>) =
            oldEntity.UpdateValue(value)
            oldEntity

        let result = store.AddOrUpdate(key, addValue, updateValue)

        // Returned value tells us if the entity was added or updated.
        if Object.ReferenceEquals(addValue, result) then
            CacheMetrics.Add &tags
            post (EvictionQueueMessage.Add(addValue, store))
        else
            CacheMetrics.Update &tags
            post (EvictionQueueMessage.Update result)

    member _.CreateMetricsListener() =
        new CacheMetrics.CacheMetricsListener(tags)

    member _.Dispose() =
        if Interlocked.Exchange(&disposed, 1) = 0 then
            disposeEvictionProcessor ()
            CacheMetrics.Disposed &tags

    interface IDisposable with
        member this.Dispose() =
            this.Dispose()
            GC.SuppressFinalize(this)

    // Finalizer to ensure eviction loop is cancelled if Dispose wasn't called.
    override this.Finalize() = this.Dispose()

#if DEBUG
    member _.DebugDisplay() = debugListener.ToString()
#endif
