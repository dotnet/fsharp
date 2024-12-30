namespace FSharp.Compiler

open System
open System.Collections.Concurrent
open System.Threading

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type CachingStrategy =
    /// Least Recently Used - replaces/evicts the item not requested for the longest time.
    | LRU
    /// Most Recently Used - replaces/evicts the item requested most recently.
    | MRU
    /// Least Frequently Used - replaces/evicts the item with the least number of requests.
    | LFU

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type EvictionMethod = Blocking | ThreadPool

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type CacheOptions =
    { Capacity: int
      PercentageToEvict: int
      Strategy: CachingStrategy
      EvictionMethod: EvictionMethod
      LevelOfConcurrency: int } with
    static member Default = { Capacity = 100; PercentageToEvict = 5; Strategy = CachingStrategy.LRU; LevelOfConcurrency = Environment.ProcessorCount; EvictionMethod = EvictionMethod.Blocking }

[<NoComparison; NoEquality>]
type CachedEntity<'Key, 'Value> =
    val Key: 'Key
    val Value: 'Value
    val mutable LastAccessed: DateTimeOffset
    val mutable AccessCount: uint64

    new(key: 'Key, value: 'Value) = { Key = key; Value = value; LastAccessed = DateTimeOffset.Now; AccessCount = 0UL }


// TODO: This has a very naive and straightforward implementation for managing lifetimes, when evicting, will have to traverse the dictionary.
[<NoComparison; NoEquality>]
type Cache<'Key, 'Value when 'Key: struct> (options: CacheOptions) =

    // Increase expected capacity by the percentage to evict, since we want to not resize the dictionary.
    let capacity = options.Capacity + (options.Capacity * options.PercentageToEvict / 100)
    let store = ConcurrentDictionary<'Key, CachedEntity<'Key,'Value>>(options.LevelOfConcurrency, capacity)
    // TODO: Explore an eviction shortcut, some sort of list of keys to evict first, based on the strategy.

    member _.GetStats() = {|
        Capacity = options.Capacity
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
        let count = store.Count
        count * options.PercentageToEvict / 100

    // TODO: All of these are proofs of concept, a very naive implementation of eviction strategies.
    member private this.TryEvictLRU () =

        printfn $"Evicting {this.GetEvictCount()} items using LRU strategy."

        let evictKeys = store.Values |> Seq.sortByDescending _.LastAccessed |> Seq.take (this.GetEvictCount()) |> Seq.map (fun x -> x.Key)

        printfn $"""  Evicting keys: {{{String.Join(", ", evictKeys)}}}"""

        if this.GetEvictCount() > 0 then
            for key in evictKeys do
                let _ = store.TryRemove(key) in ()

    member private this.TryEvictMRU () =
        printfn $"Evicting {this.GetEvictCount()} items using MRU strategy."

        let evictKeys = store.Values |> Seq.sortBy _.LastAccessed |> Seq.take (this.GetEvictCount()) |> Seq.map (fun x -> x.Key)

        printfn $"""  Evicting keys: {{{String.Join(", ", evictKeys)}}}"""

        if this.GetEvictCount() > 0 then
            for key in evictKeys do
                let _ = store.TryRemove(key) in ()

    member private this.TryEvictLFU () =
        printfn $"Evicting {this.GetEvictCount()} items using MRU strategy."

        let evictKeys = store.Values |> Seq.sortBy _.AccessCount |> Seq.take (this.GetEvictCount()) |> Seq.map (fun x -> x.Key)

        printfn $"""  Evicting keys: {{{String.Join(", ", evictKeys)}}}"""

        if this.GetEvictCount() > 0 then
            for key in evictKeys do
                let _ = store.TryRemove(key) in ()

    member this.TryEvict() =

        let evictCount = this.GetEvictCount()

        if evictCount <= 0 then
            ()
        else

            printfn $"Need to evict {evictCount} items."

            let evictionJob =
                match options.Strategy with
                | CachingStrategy.LRU -> this.TryEvictLRU
                | CachingStrategy.MRU -> this.TryEvictMRU
                | CachingStrategy.LFU -> fun () -> ()


            if store.Count <= options.Capacity then
                ()
            else
                // TODO: Handle any already running eviction jobs (?)

                match options.EvictionMethod with
                | EvictionMethod.Blocking -> evictionJob ()
                | EvictionMethod.ThreadPool -> ThreadPool.QueueUserWorkItem (fun _ -> evictionJob ()) |> ignore


    member _.TryGet(key: 'Key) =
        match store.TryGetValue(key) with
        | true, value ->
            // this is fine to be non-atomic, I guess, we are okay with race if the time is within the time of multiple concurrent calls.
            value.LastAccessed <- DateTimeOffset.Now
            value.AccessCount <- Interlocked.Increment(&value.AccessCount)
            ValueSome value
        | _ ->
            ValueNone

    member this.Add(key: 'Key, value: 'Value) = let _ = this.TryAdd(key, value) in ()

    member this.TryAdd(key: 'Key, value: 'Value) =

        if store.Count >= options.Capacity then
            let _ = this.TryEvict() in ()

        store.TryAdd(key, CachedEntity<'Key, 'Value>(key, value))