namespace FSharp.Compiler

open System
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks
open System.Diagnostics
open System.Diagnostics.Metrics

[<RequireQualifiedAccess>]
// Default Seq.* function have one issue - when doing `Seq.sortBy`, it will call a `ToArray` on the collection,
// which is *not* calling `ConcurrentDictionary.ToArray`, but uses a custom one instead (treating it as `ICollection`)
// this leads to and exception when trying to evict without locking (The index is equal to or greater than the length of the array,
// or the number of elements in the dictionary is greater than the available space from index to the end of the destination array.)
// this is casuedby insertions happened between reading the `Count` and doing the `CopyTo`.
// This solution introduces a custom `ConcurrentDictionary.sortBy` which will be calling a proper `CopyTo`, the one on the ConcurrentDictionary itself.
module internal ConcurrentDictionary =

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
type internal CachingStrategy =
    | LRU
    | LFU

[<Struct; RequireQualifiedAccess; NoComparison>]
type internal EvictionMethod =
    | Blocking
    | Background
    | KeepAll

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
type internal CachedEntity<'Value> =
    val Value: 'Value
    val mutable LastAccessed: int64
    val mutable AccessCount: int64

    new(value: 'Value) =
        {
            Value = value
            LastAccessed = DateTimeOffset.Now.Ticks
            AccessCount = 0L
        }

module internal CacheMetrics =
    let mutable cacheId = 0
    let addInstrumentation (store: ConcurrentDictionary<_, CachedEntity<'Value>>) =
        let meter = new Meter("FSharp.Compiler.Caches")
        let uid = Interlocked.Increment &cacheId

        let orZero f = fun () ->
            let vs = store.Values
            if vs |> Seq.isEmpty then 0L else f vs

        let _ = meter.CreateObservableGauge($"cache{uid}", (fun () -> int64 store.Count))
        //let _ = meter.CreateObservableGauge($"MRA{uid}", orZero (Seq.map _.LastAccessed >> Seq.max))
        //let _ = meter.CreateObservableGauge($"LRA{uid}", orZero (Seq.map _.LastAccessed >> Seq.min))
        let _ = meter.CreateObservableGauge($"MFA{uid}", orZero (Seq.map _.AccessCount >> Seq.max))
        let _ = meter.CreateObservableGauge($"LFA{uid}", orZero (Seq.map _.AccessCount >> Seq.min))
        ()

[<Sealed; NoComparison; NoEquality>]
[<DebuggerDisplay("{GetStats()}")>]
type internal Cache<'Key, 'Value> private (options: CacheOptions, capacity, cts) =

    let cacheHit = Event<_ * _>()
    let cacheMiss = Event<_>()
    let eviction = Event<_>()
    
    // Increase expected capacity by the percentage to evict, since we want to not resize the dictionary.
    let store = ConcurrentDictionary<_, CachedEntity<'Value>>(options.LevelOfConcurrency, capacity)

    do CacheMetrics.addInstrumentation store

    [<CLIEvent>]
    member val CacheHit = cacheHit.Publish

    [<CLIEvent>]
    member val CacheMiss = cacheMiss.Publish

    [<CLIEvent>]
    member val Eviction = eviction.Publish

    static member Create(options: CacheOptions) =
        let capacity =
            options.MaximumCapacity
            + (options.MaximumCapacity * options.PercentageToEvict / 100)

        let cts = new CancellationTokenSource()
        let cache = new Cache<'Key, 'Value>(options, capacity, cts)

        if options.EvictionMethod = EvictionMethod.Background then
            Task.Run(cache.TryEvictTask, cts.Token) |> ignore

        cache

    //member this.GetStats() =
    //    {|
    //        Capacity = options.MaximumCapacity
    //        PercentageToEvict = options.PercentageToEvict
    //        Strategy = options.Strategy
    //        LevelOfConcurrency = options.LevelOfConcurrency
    //        Count = this.Store.Count
    //        MostRecentlyAccesssed = this.Store.Values |> Seq.maxBy _.LastAccessed |> _.LastAccessed
    //        LeastRecentlyAccesssed = this.Store.Values |> Seq.minBy _.LastAccessed |> _.LastAccessed
    //        MostFrequentlyAccessed = this.Store.Values |> Seq.maxBy _.AccessCount |> _.AccessCount
    //        LeastFrequentlyAccessed = this.Store.Values |> Seq.minBy _.AccessCount |> _.AccessCount
    //    |}

    member private this.CalculateEvictionCount() =
        if store.Count >= options.MaximumCapacity then
            (store.Count - options.MaximumCapacity)
            + (options.MaximumCapacity * options.PercentageToEvict / 100)
        else
            0

    // TODO: All of these are proofs of concept, a very naive implementation of eviction strategies, it will always walk the dictionary to find the items to evict, this is not efficient.
    member private this.TryGetPickToEvict() =
        store
        |> match options.Strategy with
           | CachingStrategy.LRU -> ConcurrentDictionary.sortBy _.Value.LastAccessed
           | CachingStrategy.LFU -> ConcurrentDictionary.sortBy _.Value.AccessCount
        |> Seq.take (this.CalculateEvictionCount())
        |> Seq.map (fun x -> x.Key)

    // TODO: Explore an eviction shortcut, some sort of list of keys to evict first, based on the strategy.
    member private this.TryEvictItems() =
        if this.CalculateEvictionCount() > 0 then
            for key in this.TryGetPickToEvict() do
                match store.TryRemove(key) with
                | true, _ -> eviction.Trigger(key)
                | _ -> () // TODO: We probably want to count eviction misses as well?

    // TODO: Shall this be a safer task, wrapping everything in try .. with, so it's not crashing silently?
    member private this.TryEvictTask() =
        backgroundTask {
            while not cts.Token.IsCancellationRequested do
                let evictionCount = this.CalculateEvictionCount()

                if evictionCount > 0 then
                    this.TryEvictItems()

                    let utilization = (float store.Count / float options.MaximumCapacity)
                    // So, based on utilization this will scale the delay between 0 and 1 seconds.
                    // Worst case scenario would be when 1 second delay happens,
                    // if the cache will grow rapidly (or in bursts), it will go beyond the maximum capacity.
                    // In this case underlying dictionary will resize, AND we will have to evict items, which will likely be slow.
                    // In this case, cache stats should be used to adjust MaximumCapacity and PercentageToEvict.
                    let delay = 1000.0 - (1000.0 * utilization)

                    if delay > 0.0 then
                        do! Task.Delay(int delay)
        }

    member this.TryEvict() =
        if this.CalculateEvictionCount() > 0 then
            match options.EvictionMethod with
            | EvictionMethod.Blocking -> this.TryEvictItems()
            | EvictionMethod.Background
            | EvictionMethod.KeepAll -> ()

    member this.TryGet(key, value: outref<'Value>) =
        match store.TryGetValue(key) with
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
            let _ = store.AddOrUpdate(key, value, (fun _ _ -> value))
            true
        else
            store.TryAdd(key, value)

    interface IDisposable with
        member _.Dispose() = cts.Cancel()

    member this.Dispose() = (this :> IDisposable).Dispose()
