namespace FSharp.Compiler

open System
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks
open System.Diagnostics

[<RequireQualifiedAccess>]
// Default Seq.* function have one issue - when doing `Seq.sortBy`, it will call a `ToArray` on the collection,
// which is *not* calling `ConcurrentDictionary.ToArray`, but uses a custom one instead (treating it as `ICollection`)
// this leads to and exception when trying to evict without locking (The index is equal to or greater than the length of the array,
// or the number of elements in the dictionary is greater than the available space from index to the end of the destination array.)
// this is casuedby insertions happened between reading the `Count` and doing the `CopyTo`.
// This solution introduces a custom `ConcurrentDictionary.sortBy` which will be calling a proper `CopyTo`, the one on the ConcurrentDictionary itself.
module ConcurrentDictionary =

    open System.Collections
    open System.Collections.Generic

    let inline mkSeq f =
        { new IEnumerable<'U> with
            member _.GetEnumerator() = f ()

          interface IEnumerable with
              member _.GetEnumerator() = (f () :> IEnumerator)
        }

    let inline mkDelayedSeq (f: unit -> IEnumerable<'T>) = mkSeq (fun () -> f().GetEnumerator())

    let inline sortBy ([<InlineIfLambda>] projection) (source: ConcurrentDictionary<_, _>) =
        mkDelayedSeq (fun () ->
            let array = source.ToArray()
            Array.sortInPlaceBy projection array
            array :> seq<_>)

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type CachingStrategy =
    | LRU
    | LFU

[<Struct; RequireQualifiedAccess; NoComparison>]
type EvictionMethod =
    | Blocking
    | Background

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
            MaximumCapacity = 100
            PercentageToEvict = 5
            Strategy = CachingStrategy.LRU
            LevelOfConcurrency = Environment.ProcessorCount
            EvictionMethod = EvictionMethod.Blocking
        }

[<Sealed; NoComparison; NoEquality>]
type CachedEntity<'Value> =
    val Value: 'Value
    val mutable LastAccessed: int64
    val mutable AccessCount: int64

    new(value: 'Value) =
        {
            Value = value
            LastAccessed = DateTimeOffset.Now.Ticks
            AccessCount = 0L
        }

[<Sealed; NoComparison; NoEquality>]
[<DebuggerDisplay("{GetStats()}")>]
type Cache<'Key, 'Value> private (options: CacheOptions, capacity, cts) =

    let cacheHit = Event<_ * _>()
    let cacheMiss = Event<_>()
    let eviction = Event<_>()

    [<CLIEvent>]
    member val CacheHit = cacheHit.Publish

    [<CLIEvent>]
    member val CacheMiss = cacheMiss.Publish

    [<CLIEvent>]
    member val Eviction = eviction.Publish

    // Increase expected capacity by the percentage to evict, since we want to not resize the dictionary.
    member val Store = ConcurrentDictionary<_, CachedEntity<'Value>>(options.LevelOfConcurrency, capacity)

    static member Create(options: CacheOptions) =
        let capacity =
            options.MaximumCapacity
            + (options.MaximumCapacity * options.PercentageToEvict / 100)

        let cts = new CancellationTokenSource()
        let cache = new Cache<'Key, 'Value>(options, capacity, cts)

        if options.EvictionMethod = EvictionMethod.Background then
            Task.Run(cache.TryEvictTask, cts.Token) |> ignore

        cache

    member this.GetStats() =
        {|
            Capacity = options.MaximumCapacity
            PercentageToEvict = options.PercentageToEvict
            Strategy = options.Strategy
            LevelOfConcurrency = options.LevelOfConcurrency
            Count = this.Store.Count
            MostRecentlyAccesssed = this.Store.Values |> Seq.maxBy _.LastAccessed |> _.LastAccessed
            LeastRecentlyAccesssed = this.Store.Values |> Seq.minBy _.LastAccessed |> _.LastAccessed
            MostFrequentlyAccessed = this.Store.Values |> Seq.maxBy _.AccessCount |> _.AccessCount
            LeastFrequentlyAccessed = this.Store.Values |> Seq.minBy _.AccessCount |> _.AccessCount
        |}

    member private this.CalculateEvictionCount() =
        if this.Store.Count >= options.MaximumCapacity then
            (this.Store.Count - options.MaximumCapacity)
            + (options.MaximumCapacity * options.PercentageToEvict / 100)
        else
            0

    // TODO: All of these are proofs of concept, a very naive implementation of eviction strategies, it will always walk the dictionary to find the items to evict, this is not efficient.
    member private this.TryGetPickToEvict() =
        this.Store
        |> match options.Strategy with
           | CachingStrategy.LRU -> ConcurrentDictionary.sortBy _.Value.LastAccessed
           | CachingStrategy.LFU -> ConcurrentDictionary.sortBy _.Value.AccessCount
        |> Seq.take (this.CalculateEvictionCount())
        |> Seq.map (fun x -> x.Key)

    // TODO: Explore an eviction shortcut, some sort of list of keys to evict first, based on the strategy.
    member private this.TryEvictItems() =
        if this.CalculateEvictionCount() > 0 then
            for key in this.TryGetPickToEvict() do
                match this.Store.TryRemove(key) with
                | true, _ -> eviction.Trigger(key)
                | _ -> () // TODO: We probably want to count eviction misses as well?

    // TODO: Shall this be a safer task, wrapping everything in try .. with, so it's not crashing silently?
    member private this.TryEvictTask() =
        backgroundTask {
            while not cts.Token.IsCancellationRequested do
                let evictionCount = this.CalculateEvictionCount()

                if evictionCount > 0 then
                    this.TryEvictItems()

                let utilization = (this.Store.Count / options.MaximumCapacity)
                // So, based on utilization this will scale the delay between 0 and 1 seconds.
                // Worst case scenario would be when 1 second delay happens,
                // if the cache will grow rapidly (or in bursts), it will go beyond the maximum capacity.
                // In this case underlying dictionary will resize, AND we will have to evict items, which will likely be slow.
                // In this case, cache stats should be used to adjust MaximumCapacity and PercentageToEvict.
                let delay = 1000 - (1000 * utilization)

                if delay > 0 then
                    do! Task.Delay(delay)
        }

    member this.TryEvict() =
        if this.CalculateEvictionCount() > 0 then
            match options.EvictionMethod with
            | EvictionMethod.Blocking -> this.TryEvictItems()
            | EvictionMethod.Background -> ()

    member this.TryGet(key, value: outref<'Value>) =
        match this.Store.TryGetValue(key) with
        | true, cachedEntity ->
            // this is fine to be non-atomic, I guess, we are okay with race if the time is within the time of multiple concurrent calls.
            cachedEntity.LastAccessed <- DateTimeOffset.Now.Ticks
            let _ = Interlocked.Increment(&cachedEntity.AccessCount)
            cacheHit.Trigger(key, cachedEntity.Value)
            value <- cachedEntity.Value
            true
        | _ ->
            cacheMiss.Trigger(key)
            value <- Unchecked.defaultof<'Value>
            false

    member this.TryAdd(key, value: 'Value, ?update: bool) =
        let update = defaultArg update false

        this.TryEvict()

        let value = CachedEntity<'Value>(value)

        if update then
            let _ = this.Store.AddOrUpdate(key, value, (fun _ _ -> value))
            true
        else
            this.Store.TryAdd(key, value)

    interface IDisposable with
        member _.Dispose() = cts.Cancel()

    member this.Dispose() = (this :> IDisposable).Dispose()
