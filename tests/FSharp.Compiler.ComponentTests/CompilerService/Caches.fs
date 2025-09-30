module internal CompilerService.Caches

open FSharp.Compiler.Caches
open Xunit
open FSharp.Test.Assert
open System.Threading.Tasks
open Microsoft.FSharp.Collections
open System

#if DEBUG
let shouldNeverTimeout = 15_000
#else
// accomodate unpredictable CI thread scheduling
let shouldNeverTimeout = 200_000
#endif

let defaultStructural() = CacheOptions.getDefault HashIdentity.Structural

[<Fact>]
let ``Create and dispose many`` () =
    let caches = 
        [ for _  in 1 .. 100 do
            new Cache<string, int>(defaultStructural(), name = "Create and dispose many") :> IDisposable ]

    for c in caches do
        c.Dispose()

[<Fact>]
let ``Basic add and retrieve`` () =
    let name = "Basic_add_and_retrieve"
    use cache = new Cache<string, int>(defaultStructural(), name = name)
    use metricsListener = cache.CreateMetricsListener()

    cache.TryAdd("key1", 1) |> shouldBeTrue
    cache.TryAdd("key2", 2) |> shouldBeTrue

    let mutable value = 0

    cache.TryGetValue("key1", &value) |> shouldBeTrue
    value |> shouldEqual 1

    cache.TryGetValue("key2", &value) |> shouldBeTrue
    value |> shouldEqual 2

    cache.TryGetValue("key3", &value) |> shouldBeFalse

    // Metrics assertions
    let totals = metricsListener.GetTotals()
    totals.["adds"] |> shouldEqual 2L

[<Fact>]
let ``Eviction of least recently used`` () =
    let name = "Eviction_of_least_recently_used"
    use cache = new Cache<string, int>({ defaultStructural() with TotalCapacity = 2; HeadroomPercentage = 0 }, name = name)
    use metricsListener = cache.CreateMetricsListener()

    cache.TryAdd("key1", 1) |> shouldBeTrue
    cache.TryAdd("key2", 2) |> shouldBeTrue

    let mutable value = 0
    cache.TryGetValue("key1", &value) |> shouldBeTrue

    let evictionResult = TaskCompletionSource()
    cache.Evicted.Add evictionResult.SetResult
    cache.EvictionFailed.Add (fun _ -> evictionResult.SetException(Xunit.Sdk.FailException.ForFailure "eviction failed"))

    cache.TryAdd("key3", 3) |> shouldBeTrue
    evictionResult.Task.Wait shouldNeverTimeout |> shouldBeTrue

    cache.TryGetValue("key2", &value) |> shouldBeFalse

    cache.TryGetValue("key1", &value) |> shouldBeTrue
    value |> shouldEqual 1

    cache.TryGetValue("key3", &value) |> shouldBeTrue
    value |> shouldEqual 3

    // Metrics assertions
    let totals = metricsListener.GetTotals()
    totals.["adds"] |> shouldEqual 3L

[<Fact>]
let ``Stress test evictions`` () =
    let cacheSize = 100
    let iterations = 10_000
    let name = "Stress test evictions"

    use cache = new Cache<string, int>({ defaultStructural() with TotalCapacity = cacheSize; HeadroomPercentage = 0 }, name = name)
    use metricsListener = cache.CreateMetricsListener()

    let evictionsCompleted = new TaskCompletionSource<unit>()
    let expectedEvictions = iterations - cacheSize

    cache.Evicted.Add <| fun () ->
        if metricsListener.GetTotals().["evictions"] = expectedEvictions then
            evictionsCompleted.SetResult()

    cache.EvictionFailed.Add <| fun _ ->
        evictionsCompleted.SetException(Xunit.Sdk.FailException.ForFailure "eviction failed")

    for i in 1 .. iterations do
        cache.TryAdd($"key{i}", i) |> shouldBeTrue

    evictionsCompleted.Task.Wait shouldNeverTimeout |> shouldBeTrue

    let mutable value = 0

    cache.TryGetValue($"key{iterations - cacheSize}", &value) |> shouldBeFalse

    cache.TryGetValue($"key{iterations - cacheSize + 1}", &value) |> shouldBeTrue
    value |> shouldEqual (iterations - cacheSize + 1)

    cache.TryGetValue($"key{iterations}", &value) |> shouldBeTrue
    value |> shouldEqual iterations

    // Metrics assertions
    let totals = metricsListener.GetTotals()
    totals.["adds"] |> shouldEqual (int64 iterations)

[<Fact>]
let ``Metrics can be retrieved`` () =
    use cache = new Cache<string, int>({ defaultStructural() with TotalCapacity = 2; HeadroomPercentage = 0 }, name = "test_metrics")
    use metricsListener = cache.CreateMetricsListener()

    cache.TryAdd("key1", 1) |> shouldBeTrue
    cache.TryAdd("key2", 2) |> shouldBeTrue

    let mutable value = 0
    cache.TryGetValue("key1", &value) |> shouldBeTrue

    let evictionCompleted = TaskCompletionSource()
    cache.Evicted.Add(fun _ -> evictionCompleted.SetResult())
    cache.EvictionFailed.Add(fun _ -> evictionCompleted.SetException(Xunit.Sdk.FailException.ForFailure "eviction failed"))

    cache.TryAdd("key3", 3) |> shouldBeTrue
    evictionCompleted.Task.Wait shouldNeverTimeout |> shouldBeTrue

    let totals = metricsListener.GetTotals()

    metricsListener.Ratio |> shouldEqual 1.0
    totals.["evictions"] |> shouldEqual 1L
    totals.["adds"] |> shouldEqual 3L

[<Fact>]
let ``GetOrAdd basic usage`` () =
    let cacheName = "GetOrAdd_basic_usage"
    use cache = new Cache<string, int>(defaultStructural(), name = cacheName)
    use metricsListener = cache.CreateMetricsListener()
    let mutable factoryCalls = 0
    let factory k = factoryCalls <- factoryCalls + 1; String.length k
    let v1 = cache.GetOrAdd("abc", factory)
    v1 |> shouldEqual 3
    let v2 = cache.GetOrAdd("abc", factory)
    v2 |> shouldEqual 3
    factoryCalls |> shouldEqual 1
    let v3 = cache.GetOrAdd("defg", factory)
    v3 |> shouldEqual 4
    factoryCalls |> shouldEqual 2
    // Metrics assertions
    let totals = metricsListener.GetTotals()
    totals.["hits"] |> shouldEqual 1L
    totals.["misses"] |> shouldEqual 2L
    metricsListener.Ratio |> shouldEqual (1.0/3.0)
    totals.["adds"] |> shouldEqual 2L

[<Fact>]
let ``AddOrUpdate basic usage`` () =
    let cacheName = "AddOrUpdate_basic_usage"
    use cache = new Cache<string, int>(defaultStructural(), name = cacheName)
    use metricsListener = cache.CreateMetricsListener()
    cache.AddOrUpdate("x", 1)
    let mutable value = 0
    cache.TryGetValue("x", &value) |> shouldBeTrue
    value |> shouldEqual 1
    cache.AddOrUpdate("x", 42)
    cache.TryGetValue("x", &value) |> shouldBeTrue
    value |> shouldEqual 42
    cache.AddOrUpdate("y", 99)
    cache.TryGetValue("y", &value) |> shouldBeTrue
    value |> shouldEqual 99
    // Metrics assertions
    let totals = metricsListener.GetTotals()
    totals.["hits"] |> shouldEqual 3L // 3 cache hits
    totals.["misses"] |> shouldEqual 0L // 0 cache misses
    metricsListener.Ratio |> shouldEqual 1.0
    totals.["adds"] |> shouldEqual 2L // "x" and "y" added
    totals.["updates"] |> shouldEqual 1L // "x" updated

type BoxedKey = BoxedKey of int * int

[<Fact>]
let ``GetOrAdd with reference identity`` () =
    let cacheName = "GetOrAdd_with_Reference"
    use cache = new Cache<BoxedKey, int>(CacheOptions.getReferenceIdentity(), cacheName)
    use metricsListener = cache.CreateMetricsListener()
    let t1 = BoxedKey (1, 2)
    let t2 = BoxedKey (1, 2)
    let t3 = BoxedKey (1, 2)
    let mutable createdCOunter = 0
    let factory _ = 
            createdCOunter <- createdCOunter + 1
            createdCOunter

    let v1 = cache.GetOrAdd(t1, factory) // miss
    let v2 = cache.GetOrAdd(t2, factory) // miss (different reference)
    v1 |> shouldEqual 1
    v2 |> shouldEqual 2
    // Reference comparer: t1 and t2 are different keys
    t1 = t2 |> shouldBeTrue // value equality
    obj.ReferenceEquals(t1, t2) |> shouldBeFalse // reference inequality
    let mutable v1' = 0
    let mutable v2' = 0
    let mutable v3' = 0
    cache.TryGetValue(t1, &v1') |> shouldBeTrue // hit
    cache.TryGetValue(t2, &v2') |> shouldBeTrue // hit
    cache.TryGetValue(t3, &v3') |> shouldBeFalse // miss
    let v1'' = cache.GetOrAdd(t1, factory) // hit
    let v2'' = cache.GetOrAdd(t2, factory) // hit
    v1'' |> shouldEqual v1'
    v2'' |> shouldEqual v2'
    // Metrics assertions
    let totals = metricsListener.GetTotals()
    totals.["hits"] |> shouldEqual 4L
    totals.["misses"] |> shouldEqual 3L
    metricsListener.Ratio |> shouldEqual (4.0 / 7.0)
    totals.["adds"] |> shouldEqual 2L

[<Fact>]
let ``AddOrUpdate with reference identity`` () =
    let cacheName = "AddOrUpdate_with_Reference"
    use cache = new Cache<obj, int>(CacheOptions.getReferenceIdentity(), name = cacheName)
    use metricsListener = cache.CreateMetricsListener()
    let t1 = box (3, 4)
    let t2 = box (3, 4)
    cache.AddOrUpdate(t1, 7)
    let mutable value1 = 0
    cache.TryGetValue(t1, &value1) |> shouldBeTrue
    value1 |> shouldEqual 7
    cache.AddOrUpdate(t2, 8)
    let mutable value2 = 0
    cache.TryGetValue(t2, &value2) |> shouldBeTrue
    value2 |> shouldEqual 8
    // t1 and t2 are different keys under reference equality
    obj.ReferenceEquals(t1, t2) |> shouldBeFalse
    // Now update t1 and check value and metrics
    cache.AddOrUpdate(t1, 9)
    let mutable value1Updated = 0
    cache.TryGetValue(t1, &value1Updated) |> shouldBeTrue
    value1Updated |> shouldEqual 9
    // Metrics assertions
    let totals = metricsListener.GetTotals()
    totals.["hits"] |> shouldEqual 3L // 3 cache hits
    totals.["misses"] |> shouldEqual 0L // 0 cache misses
    metricsListener.Ratio |> shouldEqual 1.0
    totals.["adds"] |> shouldEqual 2L // t1 and t2 added
    totals.["updates"] |> shouldEqual 1L // t1 updated once
