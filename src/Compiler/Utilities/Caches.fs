namespace FSharp.Compiler

open System
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

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
      LevelOfConcurrency: int
      Weak: bool } with
    static member Default = { MaximumCapacity = 100; PercentageToEvict = 5; Strategy = CachingStrategy.LRU; LevelOfConcurrency = Environment.ProcessorCount; EvictionMethod = EvictionMethod.Blocking; Weak = false }

[<Sealed; NoComparison; NoEquality>]
type CachedEntity<'Key, 'Value> =
    val Key: 'Key
    val Value: 'Value
    val mutable LastAccessed: int64
    val mutable AccessCount: uint64

    new(key: 'Key, value: 'Value) = { Key = key; Value = value; LastAccessed = DateTimeOffset.Now.Ticks; AccessCount = 0UL }

[<Sealed; NoComparison; NoEquality>]
type Weak<'K>(key) =
    let collected = new Event<'K>()
    [<CLIEvent>]
    member val Collected = collected.Publish

    // TODO: Do we want to store it as WeakReference here?
    override _.Finalize() = collected.Trigger key

// TODO: This has a very naive and straightforward implementation for managing lifetimes, when evicting, will have to traverse the dictionary.
[<Sealed; NoComparison; NoEquality>]
type Cache<'Key, 'Value> (options: CacheOptions) as this =

    let capacity = options.MaximumCapacity + (options.MaximumCapacity * options.PercentageToEvict / 100)
    let cts = new CancellationTokenSource()

    do
        if options.EvictionMethod = EvictionMethod.Background then
            Task.Run(this.TryEvictTask, cts.Token) |> ignore

    let cacheHit = Event<'Key * 'Value>()
    let cacheMiss = Event<'Key>()
    let eviction = Event<'Key * EvictionReason>()

    [<CLIEvent>] member val CacheHit = cacheHit.Publish
    [<CLIEvent>] member val CacheMiss = cacheMiss.Publish
    [<CLIEvent>] member val Eviction = eviction.Publish

    // Increase expected capacity by the percentage to evict, since we want to not resize the dictionary.
    member val Store = ConcurrentDictionary<'Key, CachedEntity<'Key,'Value>>(options.LevelOfConcurrency, capacity)
    member val ConditionalWeakTable = new ConditionalWeakTable<_, Weak<_>>();

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
    member private this.TryGetItemsToEvict () =
        match options.Strategy with
        | CachingStrategy.LRU ->
            this.Store.Values |> Seq.sortBy _.LastAccessed |> Seq.take (this.GetEvictCount()) |> Seq.map (fun x -> x.Key)
        | CachingStrategy.MRU ->
            this.Store.Values |> Seq.sortByDescending _.LastAccessed |> Seq.take (this.GetEvictCount()) |> Seq.map (fun x -> x.Key)
        | CachingStrategy.LFU ->
            this.Store.Values |> Seq.sortBy _.AccessCount |> Seq.take (this.GetEvictCount()) |> Seq.map (fun x -> x.Key)

    member private this.TryEvictItems () =
        if this.GetEvictCount() > 0 then
            for key in this.TryGetItemsToEvict () do
                let (removed, _) = this.Store.TryRemove(key)
                if removed then
                    eviction.Trigger(key, EvictionReason.Evicted)

    member private this.TryEvictTask () =
        // This will spin in the background trying to evict items.
        // One of the issues is that if the delay is high (>100ms), it will not be able to evict items in time, and the cache will grow beyond the maximum capacity.
        backgroundTask {
            while not cts.Token.IsCancellationRequested do
                if this.GetEvictCount() > 0 then
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

    [<NoCompilerInlining>]
    member this.Add(key: 'Key, value: 'Value) = let _ = this.TryAdd(key, value) in ()

    [<NoCompilerInlining>]
    member this.TryAdd<'Key>(key: 'Key, value: 'Value) =

        // Weak table/references only make sense if we work with reference types (for obvious reasons).
        // So, if this collection is storing value types as keys, it will simply box them.
        // GC-based eviction shall not be used with value types as keys.
        if options.Weak then
            let weak = Weak<'Key>(key)
            this.ConditionalWeakTable.TryAdd(key :> obj, weak) |> ignore
            weak.Collected.Add(this.RemoveCollected)

        this.TryEvict()
        this.Store.TryAdd(key, CachedEntity<'Key, 'Value>(key, value))

    // TODO: This needs heavy testing to ensure we aren't leaking anything.
    member private _.RemoveCollected(key: 'Key) =
        this.Store.TryRemove(key) |> ignore
        eviction.Trigger(key, EvictionReason.Collected);

    interface IDisposable with
        member _.Dispose() = cts.Cancel()

    member this.Dispose() = (this :> IDisposable).Dispose()