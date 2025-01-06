namespace FSharp.Compiler

open System
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks
open System.Diagnostics

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type CachingStrategy = LRU | MRU | LFU

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
      LevelOfConcurrency: int } with
    static member Default = { MaximumCapacity = 100; PercentageToEvict = 5; Strategy = CachingStrategy.LRU; LevelOfConcurrency = Environment.ProcessorCount; EvictionMethod = EvictionMethod.Blocking; }

[<Sealed; NoComparison; NoEquality>]
type CachedEntity<'Value> =
    val Value: 'Value
    val mutable LastAccessed: int64
    val mutable AccessCount: uint64

    new(value: 'Value) = { Value = value; LastAccessed = DateTimeOffset.Now.Ticks; AccessCount = 0UL }

[<Sealed; NoComparison; NoEquality>]
[<DebuggerDisplay("{GetStats()}")>]
type Cache<'Key, 'Value> (options: CacheOptions) as this =

    let capacity = options.MaximumCapacity + (options.MaximumCapacity * options.PercentageToEvict / 100)
    let cts = new CancellationTokenSource()

    do
        if options.EvictionMethod = EvictionMethod.Background then
            Task.Run(this.TryEvictTask, cts.Token) |> ignore

    let cacheHit = Event<_ * _>()
    let cacheMiss = Event<_>()
    let eviction = Event<_ * EvictionReason>()

    [<CLIEvent>] member val CacheHit = cacheHit.Publish
    [<CLIEvent>] member val CacheMiss = cacheMiss.Publish
    [<CLIEvent>] member val Eviction = eviction.Publish

    // Increase expected capacity by the percentage to evict, since we want to not resize the dictionary.
    member val Store = ConcurrentDictionary<_, CachedEntity<'Value>>(options.LevelOfConcurrency, capacity)

    member _.GetStats() =
        {|
            Capacity = options.MaximumCapacity
            PercentageToEvict = options.PercentageToEvict
            Strategy = options.Strategy
            LevelOfConcurrency = options.LevelOfConcurrency
            Count = this.Store.Count
            LeastRecentlyAccesssed = this.Store.Values |> Seq.minBy _.LastAccessed |> _.LastAccessed
            MostRecentlyAccesssed = this.Store.Values |> Seq.maxBy _.LastAccessed |> _.LastAccessed
            LeastFrequentlyAccessed = this.Store.Values |> Seq.minBy _.AccessCount |> _.AccessCount
            MostFrequentlyAccessed = this.Store.Values |> Seq.maxBy _.AccessCount |> _.AccessCount
        |}


    member private _.GetEvictCount() =
        if this.Store.Count >= options.MaximumCapacity then
            this.Store.Count * options.PercentageToEvict / 100
        else
            0

    // TODO: All of these are proofs of concept, a very naive implementation of eviction strategies, it will always walk the dictionary to find the items to evict, this is not efficient.
    member private _.TryGetItemsToEvict () =
        match options.Strategy with
        | CachingStrategy.LRU ->
            this.Store |> Seq.sortBy _.Value.LastAccessed |> Seq.take (this.GetEvictCount()) |> Seq.map (fun x -> x.Key)
        | CachingStrategy.MRU ->
            this.Store |> Seq.sortByDescending _.Value.LastAccessed |> Seq.take (this.GetEvictCount()) |> Seq.map (fun x -> x.Key)
        | CachingStrategy.LFU ->
            this.Store |> Seq.sortBy _.Value.AccessCount |> Seq.take (this.GetEvictCount()) |> Seq.map (fun x -> x.Key)

    member private _.TryEvictItems () =
        if this.GetEvictCount() > 0 then
            for key in this.TryGetItemsToEvict () do
                match this.Store.TryRemove(key) with
                | true, _ -> eviction.Trigger(key, EvictionReason.Evicted)
                | _ -> () // TODO: We probably want to count eviction misses as well?

    member private _.TryEvictTask () =
        // This will spin in the background trying to evict items.
        // One of the issues is that if the delay is high (>100ms), it will not be able to evict items in time, and the cache will grow beyond the maximum capacity.
        backgroundTask {
            while not cts.Token.IsCancellationRequested do
                if this.GetEvictCount() > 0 then
                    this.TryEvictItems ()
                // do! Task.Delay(100, cts.Token)
        }

    // TODO: Explore an eviction shortcut, some sort of list of keys to evict first, based on the strategy.
    member _.TryEvict() =
        if this.GetEvictCount() > 0 then
            match options.EvictionMethod with
            | EvictionMethod.Blocking -> this.TryEvictItems ()
            | EvictionMethod.Background -> ()

    member _.TryGet(key) =
        match this.Store.TryGetValue(key) with
        | true, value ->
            // this is fine to be non-atomic, I guess, we are okay with race if the time is within the time of multiple concurrent calls.
            value.LastAccessed <- DateTimeOffset.Now.Ticks
            value.AccessCount <- Interlocked.Increment(&value.AccessCount)
            cacheHit.Trigger(key, value.Value)
            ValueSome value
        | _ ->
            cacheMiss.Trigger(key)
            ValueNone

    member _.TryAdd(key, value: 'Value, ?update: bool) =

        let update = defaultArg update false

        this.TryEvict()

        let value = CachedEntity<'Value>(value)

        if update then
            this.Store.AddOrUpdate(key, value, (fun _ _ -> value)) |> ignore
            true
        else
            this.Store.TryAdd(key, value)

    interface IDisposable with
        member _.Dispose() = cts.Cancel()

    member this.Dispose() = (this :> IDisposable).Dispose()