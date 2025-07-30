module CompilerService.Caches

open FSharp.Compiler.Caches

open Xunit
open FSharp.Test
open System.Threading.Tasks
open System.Diagnostics

let assertTrue msg b = Assert.True(b, msg)

#if DEBUG
let shouldNeverTimeout = 15_000
#else
// accomodate unpredictable CI thread scheduling
let shouldNeverTimeout = 200_000
#endif

[<Fact>]
let ``Create and dispose many`` () =
    let caches = 
        [
            for _  in 1 .. 100 do
                Cache.Create<string, int>(CacheOptions.Default, observeMetrics = true)
        ]

    for c in caches do c.Dispose()

[<Fact>]
let ``Create and dispose many named`` () =
    let caches = 
        [
            for i in 1 .. 100 do
                Cache.Create<string, int>(CacheOptions.Default, name = $"testCache{i}", observeMetrics = true)
        ]

    for c in caches do c.Dispose()

[<Fact>]
let ``Basic add and retrieve`` () =
    use cache = Cache.Create<string, int>(CacheOptions.Default, observeMetrics = true)
        
    cache.TryAdd("key1", 1) |> assertTrue "failed to add key1"
    cache.TryAdd("key2", 2) |> assertTrue "failed to add key2"
        
    let mutable value = 0
    Assert.True(cache.TryGetValue("key1", &value), "Should retrieve key1")
    Assert.Equal(1, value)
    Assert.True(cache.TryGetValue("key2", &value), "Should retrieve key2")
    Assert.Equal(2, value)
    Assert.False(cache.TryGetValue("key3", &value), "Should not retrieve non-existent key3")

[<Fact>]
let ``Eviction of least recently used`` () =
    use cache = Cache.Create<string, int>({ TotalCapacity = 2; HeadroomPercentage = 0 }, observeMetrics = true)
        
    cache.TryAdd("key1", 1) |> assertTrue "failed to add key1"
    cache.TryAdd("key2", 2) |> assertTrue "failed to add key2"
        
    // Make key1 recently used by accessing it
    let mutable value = 0
    cache.TryGetValue("key1", &value) |> assertTrue "failed to access key1"

    let evictionResult = TaskCompletionSource()

    cache.Evicted.Add evictionResult.SetResult
    cache.EvictionFailed.Add (fun _ -> evictionResult.SetException(Xunit.Sdk.FailException.ForFailure "eviction failed"))

    // Add a third item, which should schedule key2 for eviction
    cache.TryAdd("key3", 3) |> assertTrue "failed to add key3"
        
    // Wait for eviction to complete using the event
    evictionResult.Task.Wait shouldNeverTimeout |> assertTrue "eviction did not complete in time"
        
    Assert.False(cache.TryGetValue("key2", &value), "key2 should have been evicted")
    Assert.True(cache.TryGetValue("key1", &value), "key1 should still be in cache")
    Assert.Equal(1, value)
    Assert.True(cache.TryGetValue("key3", &value), "key3 should be in cache")
    Assert.Equal(3, value)

[<Fact>]
let ``Stress test evictions`` () =
    let cacheSize = 100
    let iterations = 10_000
    let name = "Stress test evictions"
    use cache = Cache.Create<string, int>({ TotalCapacity = cacheSize; HeadroomPercentage = 0 }, name = name, observeMetrics = true)

    let evictionsCompleted = new TaskCompletionSource<unit>()
    let expectedEvictions = iterations - cacheSize

    cache.Evicted.Add <| fun () ->
        if CacheMetrics.GetTotals(name).["evictions"] = expectedEvictions then evictionsCompleted.SetResult()

    // Should not fail, but if it does, we want to know
    cache.EvictionFailed.Add <| fun _ ->
        evictionsCompleted.SetException(Xunit.Sdk.FailException.ForFailure "eviction failed")

    for i in 1 .. iterations do
        cache.TryAdd($"key{i}", i) |> assertTrue ($"failed to add key{i}")

    // Wait for all expected evictions to complete
    evictionsCompleted.Task.Wait shouldNeverTimeout |> assertTrue "evictions did not complete in time"

    let mutable value = 0
    Assert.False(cache.TryGetValue($"key{iterations - cacheSize}", &value), "An old key should have been evicted")
    Assert.True(cache.TryGetValue($"key{iterations - cacheSize + 1}", &value), "The first of the newest keys should be in cache")
    Assert.Equal(iterations - cacheSize + 1, value)
    Assert.True(cache.TryGetValue($"key{iterations}", &value), "The last key should be in cache")
    Assert.Equal(iterations, value)

[<Fact>]
let ``Metrics can be retrieved`` () =
    use cache = Cache.Create<string, int>({ TotalCapacity = 2; HeadroomPercentage = 0 }, name = "test_metrics", observeMetrics = true)
        
    cache.TryAdd("key1", 1) |> assertTrue "failed to add key1"
    cache.TryAdd("key2", 2) |> assertTrue "failed to add key2"
        
    // Make key1 recently used by accessing it
    let mutable value = 0
    cache.TryGetValue("key1", &value) |> assertTrue "failed to access key1"

    let evictionCompleted = TaskCompletionSource()
    cache.Evicted.Add(fun _ -> evictionCompleted.SetResult())
    cache.EvictionFailed.Add(fun _ -> evictionCompleted.SetException(Xunit.Sdk.FailException.ForFailure "eviction failed"))
        
    // Add a third item, which should schedule key2 for eviction
    cache.TryAdd("key3", 3) |> assertTrue "failed to add key3"
        
    // Wait for eviction to complete
    evictionCompleted.Task.Wait shouldNeverTimeout |> assertTrue "eviction did not complete in time"

    let stats = CacheMetrics.GetStats "test_metrics"
    let totals = CacheMetrics.GetTotals "test_metrics"

    Assert.Equal( 1.0, stats["hit-ratio"])
    Assert.Equal(1L, totals["evictions"])
