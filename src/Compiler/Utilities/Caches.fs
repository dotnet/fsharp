// LinkedList uses nulls, so we need to disable the nullability warnings for this file.
#nowarn 3261
namespace FSharp.Compiler

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Threading
open System.Diagnostics
open System.Diagnostics.Metrics
open Internal.Utilities.Library

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type internal CachingStrategy =
    | LRU
    | LFU

[<Struct; RequireQualifiedAccess; NoComparison>]
type internal EvictionMethod =
    | Blocking
    | Background
    | NoEviction

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type internal CacheOptions =
    {
        MaximumCapacity: int
        PercentageToEvict: int
        Strategy: CachingStrategy
        EvictionMethod: EvictionMethod
        LevelOfConcurrency: int
    }

    static member Default =
        {
            MaximumCapacity = 100
            PercentageToEvict = 5
            Strategy = CachingStrategy.LRU
            LevelOfConcurrency = Environment.ProcessorCount
            EvictionMethod = EvictionMethod.Blocking
        }

[<Sealed; NoComparison; NoEquality>]
[<DebuggerDisplay("{ToString()}")>]
type internal CachedEntity<'Key, 'Value> =
    val mutable Key: 'Key
    val mutable Value: 'Value
    val mutable AccessCount: int64
    val mutable Node: LinkedListNode<CachedEntity<'Key, 'Value>>

    private new(key, value) =
        {
            Key = key
            Value = value
            AccessCount = 0L
            Node = Unchecked.defaultof<_>
        }

    static member Create(key, value) =
        let entity = CachedEntity(key, value)
        entity.Node <- LinkedListNode(entity)
        entity

    member this.ReUse(key, value) =
        this.Key <- key
        this.Value <- value
        this.AccessCount <- 0L
        this

    override this.ToString() = $"{this.Key}"

type internal EvictionQueue<'Key, 'Value>(strategy: CachingStrategy) =

    let list = LinkedList<CachedEntity<'Key, 'Value>>()
    let pool = ConcurrentBag<CachedEntity<'Key, 'Value>>()

    member _.Acquire(key, value) =
        match pool.TryTake() with
        | true , entity -> entity.ReUse(key, value)
        | _ -> CachedEntity.Create<_, _>(key, value)

    member _.Add(entity: CachedEntity<'Key, 'Value>, strategy) =
        lock list
        <| fun () ->
            if isNull entity.Node.List then
                match strategy with
                | CachingStrategy.LRU ->
                    list.AddLast(entity.Node)
                | CachingStrategy.LFU ->
                    list.AddLast(entity.Node)
                    // list.AddFirst(entity.Node)

    member _.Update(entity: CachedEntity<'Key, 'Value>) =
        lock list
        <| fun () ->
            entity.AccessCount <- entity.AccessCount + 1L

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
                        if isNotNull current.Next && current.Next.Value.AccessCount < entity.AccessCount then
                            bubbleUp current.Next
                        else
                            current

                    let next = bubbleUp node

                    if next <> node then
                        list.Remove(node)
                        list.AddAfter(next, node)

    member _.GetKeysToEvict(count) =
        lock list
        <| fun () -> list |> Seq.map _.Key |> Seq.truncate count |> Seq.toArray

    member _.Remove(entity: CachedEntity<_, _>) =
        lock list <| fun () -> list.Remove(entity.Node)
        // Return to the pool for reuse.
        pool.Add(entity)

    member _.Count = list.Count

[<Sealed; NoComparison; NoEquality>]
[<DebuggerDisplay("{GetStats()}")>]
type internal Cache<'Key, 'Value when 'Key: not null and 'Key: equality>
    internal (options: CacheOptions, capacity, cts: CancellationTokenSource) =

    let cacheHit = Event<_ * _>()
    let cacheMiss = Event<_>()
    let eviction = Event<_>()
    let evictionFail = Event<_>()

    let store =
        ConcurrentDictionary<'Key, CachedEntity<'Key, 'Value>>(options.LevelOfConcurrency, capacity)

    let evictionQueue = EvictionQueue<'Key, 'Value>(options.Strategy)

    let tryEvictItems () =
        let count =
            if store.Count > options.MaximumCapacity then
                store.Count - options.MaximumCapacity
            else
                0

        for key in evictionQueue.GetKeysToEvict(count) do
            match store.TryRemove(key) with
            | true, removed ->
                evictionQueue.Remove(removed)
                eviction.Trigger(key)
            | _ -> evictionFail.Trigger(key)

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

    let tryEvict () =
        if options.EvictionMethod.IsBlocking then
            tryEvictItems ()

    let tryGet (key: 'Key) =
        match store.TryGetValue(key) with
        | true, cachedEntity ->
            evictionQueue.Update(cachedEntity)
            Some cachedEntity
        | _ -> None

    member _.TryGetValue(key: 'Key, value: outref<'Value>) =
        match tryGet key with
        | Some cachedEntity ->
            cacheHit.Trigger(key, cachedEntity.Value)
            value <- cachedEntity.Value
            true
        | _ ->
            cacheMiss.Trigger(key)
            value <- Unchecked.defaultof<'Value>
            false

    member _.TryAdd(key: 'Key, value: 'Value) =
        tryEvict ()

        let cachedEntity = evictionQueue.Acquire(key, value)

        if store.TryAdd(key, cachedEntity) then
            evictionQueue.Add(cachedEntity, options.Strategy)
            true
        else
            false

    member _.AddOrUpdate(key: 'Key, value: 'Value) =
        tryEvict ()

        let entity =
            store.AddOrUpdate(
                key,
                (fun _ -> evictionQueue.Acquire(key, value)),
                (fun _ (current: CachedEntity<_, _>) ->
                    current.Value <- value
                    current)
            )

        evictionQueue.Add(entity, options.Strategy)

    [<CLIEvent>]
    member val CacheHit = cacheHit.Publish

    [<CLIEvent>]
    member val CacheMiss = cacheMiss.Publish

    [<CLIEvent>]
    member val Eviction = eviction.Publish

    [<CLIEvent>]
    member val EvictionFail = evictionFail.Publish

    interface IDisposable with
        member this.Dispose() =
            cts.Cancel()
            GC.SuppressFinalize(this)

    member this.Dispose() = (this :> IDisposable).Dispose()

    override this.Finalize (): unit =
        this.Dispose()


module internal Cache =      
    let mutable cacheId = 0
    
    [<Literal>]
    let MeterName = "FSharp.Compiler.Caches"
        
    let addInstrumentation (cache: Cache<_, _>) =
        let meter = new Meter(MeterName)
        let cacheId = Interlocked.Increment &cacheId
        
        let mutable evictions = 0L
        let mutable fails = 0L
        let mutable hits = 0L
        let mutable misses = 0L
    
        let mutable allEvictions = 0L
        let mutable allFails = 0L
        let mutable allHits = 0L
        let mutable allMisses = 0L
    
        cache.CacheHit |> Event.add (fun _ ->
            Interlocked.Increment &hits |> ignore
            Interlocked.Increment &allHits |> ignore
        )
    
        cache.CacheMiss |> Event.add (fun _ ->
            Interlocked.Increment &misses |> ignore
            Interlocked.Increment &allMisses |> ignore
        )
    
        cache.Eviction |> Event.add (fun _ ->
            Interlocked.Increment &evictions |> ignore
            Interlocked.Increment &allEvictions |> ignore
        )
    
        cache.EvictionFail |> Event.add (fun _ ->
            Interlocked.Increment &fails |> ignore
            Interlocked.Increment &allFails |> ignore
        )
    
    
        let hitRatio () =
            let misses = Interlocked.Exchange(&misses, 0L)
            let hits = Interlocked.Exchange(&hits, 0L)
            float hits / float (hits + misses)
    
        meter.CreateObservableGauge($"hit ratio {cacheId}", hitRatio) |> ignore

    let Create<'Key, 'Value when 'Key: not null and 'Key: equality>(options: CacheOptions) =
        // Increase expected capacity by the percentage to evict, since we want to not resize the dictionary.
        let capacity =
            options.MaximumCapacity
            + (options.MaximumCapacity * options.PercentageToEvict / 100)

        let cts = new CancellationTokenSource()
        let cache = new Cache<'Key, 'Value>(options, capacity, cts)
    #if DEBUG
        addInstrumentation cache
    #endif
        cache

