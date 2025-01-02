namespace FSharp.Compiler

open System
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type CachingStrategy =
    /// Least Recently Used - replaces/evicts the item not requested for the longest time.
    | LRU
    /// Most Recently Used - replaces/evicts the item requested most recently.
    | MRU
    /// Least Frequently Used - replaces/evicts the item with the least number of requests.
    | LFU

[<Struct; RequireQualifiedAccess; NoComparison>]
type EvictionMethod = Blocking | Background

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type EvictionReason = Evicted | Collected

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type CacheOptions =
    { MaximumCapacity: int
      PercentageToEvict: int
      Strategy: CachingStrategy
      EvictionMethod: EvictionMethod
      LevelOfConcurrency: int
      Weak: bool } with
    static member Default = { MaximumCapacity = 100; PercentageToEvict = 5; Strategy = CachingStrategy.LRU; LevelOfConcurrency = Environment.ProcessorCount; EvictionMethod = EvictionMethod.Blocking; Weak = false }

[<NoComparison; NoEquality>]
type CachedEntity<'Key, 'Value> =
    val Key: 'Key
    val Value: 'Value
    val mutable LastAccessed: int64
    val mutable AccessCount: uint64

    new(key: 'Key, value: 'Value) = { Key = key; Value = value; LastAccessed = DateTimeOffset.Now.Ticks; AccessCount = 0UL }

[<NoComparison; NoEquality>]
type Weak<'K>(key) =
    let collected = new Event<'K>()
    [<CLIEvent>]
    member val Collected = collected.Publish

    // TODO: Do we want to store it as WeakReference here?
    override _.Finalize() = collected.Trigger key

// TODO: This has a very naive and straightforward implementation for managing lifetimes, when evicting, will have to traverse the dictionary.
[<NoComparison; NoEquality>]
type Cache<'Key, 'Value> (options: CacheOptions) as this =

    // Increase expected capacity by the percentage to evict, since we want to not resize the dictionary.
    let capacity = options.MaximumCapacity + (options.MaximumCapacity * options.PercentageToEvict / 100)
    let store = ConcurrentDictionary<'Key, CachedEntity<'Key,'Value>>(options.LevelOfConcurrency, capacity)

    let conditionalWeakTable = new ConditionalWeakTable<_, Weak<_>>();

    let cts = new CancellationTokenSource()

    do
        if options.EvictionMethod = EvictionMethod.Background then
            Task.Run(this.TryEvictTask, cts.Token) |> ignore

    let cacheHit = Event<'Key * 'Value>()
    let cacheMiss = Event<'Key>()
    let eviction = Event<'Key * EvictionReason>()

    [<CLIEvent>]
    member val CacheHit = cacheHit.Publish

    [<CLIEvent>]
    member val CacheMiss = cacheMiss.Publish

    [<CLIEvent>]

    member val Eviction = eviction.Publish


    member _.GetStats() =
        {|
            Capacity = options.MaximumCapacity
            PercentageToEvict = options.PercentageToEvict
            Strategy = options.Strategy
            LevelOfConcurrency = options.LevelOfConcurrency
            Count = store.Count
            LeastRecentlyAccesssed = store.Values |> Seq.minBy _.LastAccessed |> _.LastAccessed
            MostRecentlyAccesssed = store.Values |> Seq.maxBy _.LastAccessed |> _.LastAccessed
            LeastFrequentlyAccessed = store.Values |> Seq.minBy _.AccessCount |> _.AccessCount
            MostFrequentlyAccessed = store.Values |> Seq.maxBy _.AccessCount |> _.AccessCount
        |}


    member private _.GetEvictCount() =
        if store.Count >= options.MaximumCapacity then
            store.Count * options.PercentageToEvict / 100
        else
            0

    // TODO: All of these are proofs of concept, a very naive implementation of eviction strategies, it will always walk the dictionary to find the items to evict, this is not efficient.
    member private this.TryGetItemsToEvict () =
        match options.Strategy with
        | CachingStrategy.LRU -> store.Values |> Seq.sortBy _.LastAccessed |> Seq.take (this.GetEvictCount()) |> Seq.map (fun x -> x.Key)
        | CachingStrategy.MRU -> store.Values |> Seq.sortByDescending _.LastAccessed |> Seq.take (this.GetEvictCount()) |> Seq.map (fun x -> x.Key)
        | CachingStrategy.LFU -> store.Values |> Seq.sortBy _.AccessCount |> Seq.take (this.GetEvictCount()) |> Seq.map (fun x -> x.Key)

    member private this.TryEvictItems () =
        if this.GetEvictCount() > 0 then
            for key in this.TryGetItemsToEvict () do
                let (removed, value) = store.TryRemove(key)
                if removed then
                    eviction.Trigger(key, EvictionReason.Evicted)

    member private this.TryEvictTask () =
        // This will spin in the background trying to evict items.
        // One of the issues is that if the delay is high (>100ms), it will not be able to evict items in time, and the cache will grow beyond the maximum capacity.
        backgroundTask {
            while not cts.Token.IsCancellationRequested do
                let evictCount = this.GetEvictCount()
                if evictCount > 0 then
                    this.TryEvictItems ()
                // do! Task.Delay(100, cts.Token)
        }

    // TODO: Explore an eviction shortcut, some sort of list of keys to evict first, based on the strategy.
    member this.TryEvict() =
        if this.GetEvictCount() > 0 then
            match options.EvictionMethod with
            | EvictionMethod.Blocking -> this.TryEvictItems ()
            | EvictionMethod.Background -> ()


    member _.TryGet(key: 'Key) =
        match store.TryGetValue(key) with
        | true, value ->
            // this is fine to be non-atomic, I guess, we are okay with race if the time is within the time of multiple concurrent calls.
            value.LastAccessed <- DateTimeOffset.Now.Ticks
            value.AccessCount <- Interlocked.Increment(&value.AccessCount)
            cacheHit.Trigger(key, value.Value)
            ValueSome value
        | _ ->
            cacheMiss.Trigger(key)
            ValueNone

    member this.Add(key: 'Key, value: 'Value) = let _ = this.TryAdd(key, value) in ()
    member this.TryAdd<'Key>(key: 'Key, value: 'Value) =

        if options.Weak then
            let weak = new Weak<'Key>(key)
            conditionalWeakTable.TryAdd(key :> obj, weak) |> ignore
            weak.Collected.Add(this.RemoveCollected)

        this.TryEvict()
        store.TryAdd(key, CachedEntity<'Key, 'Value>(key, value))

    // TODO: This needs heavy testing to ensure we aren't leaking anything.
    member private _.RemoveCollected(key: 'Key) =
        store.TryRemove(key) |> ignore
        eviction.Trigger(key, EvictionReason.Collected);

    interface IDisposable with
        member _.Dispose() = cts.Cancel()

    member this.Dispose() = (this :> IDisposable).Dispose()