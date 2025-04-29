module CompilerService.Caches

open FSharp.Compiler.Caches

open System.Threading
open Xunit

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
        
    cache.TryAdd("key1", 1) |> ignore
    cache.TryAdd("key2", 2) |> ignore
        
    let mutable value = 0
    Assert.True(cache.TryGetValue("key1", &value), "Should retrieve key1")
    Assert.Equal(1, value)
    Assert.True(cache.TryGetValue("key2", &value), "Should retrieve key2")
    Assert.Equal(2, value)
    Assert.False(cache.TryGetValue("key3", &value), "Should not retrieve non-existent key3")

[<Fact>]
let ``Eviction of least recently used`` () =
    use cache = Cache.Create<string, int>({ TotalCapacity = 2; HeadroomPercentage = 0 }, observeMetrics = true)
        
    cache.TryAdd("key1", 1) |> ignore
    cache.TryAdd("key2", 2) |> ignore
        
    // Make key1 recently used by accessing it
    let mutable value = 0
    cache.TryGetValue("key1", &value) |> ignore

    let evicted = new ManualResetEvent(false)
    cache.Evicted.Add(fun _ -> evicted.Set() |> ignore)
        
    // Add a third item, which should schedule key2 for eviction
    cache.TryAdd("key3", 3) |> ignore
        
    // Wait for eviction to complete using the event
    evicted.WaitOne() |> ignore
        
    Assert.False(cache.TryGetValue("key2", &value), "key2 should have been evicted")
    Assert.True(cache.TryGetValue("key1", &value), "key1 should still be in cache")
    Assert.Equal(1, value)
    Assert.True(cache.TryGetValue("key3", &value), "key3 should be in cache")
    Assert.Equal(3, value)

[<Fact>]
let ``Metrics can be retrieved`` () =
    use cache = Cache.Create<string, int>({ TotalCapacity = 2; HeadroomPercentage = 0 }, name = "test_metrics", observeMetrics = true)
        
    cache.TryAdd("key1", 1) |> ignore
    cache.TryAdd("key2", 2) |> ignore
        
    // Make key1 recently used by accessing it
    let mutable value = 0
    cache.TryGetValue("key1", &value) |> ignore

    let evicted = new ManualResetEvent(false)
    cache.Evicted.Add(fun _ -> evicted.Set() |> ignore)
        
    // Add a third item, which should schedule key2 for eviction
    cache.TryAdd("key3", 3) |> ignore
        
    // Wait for eviction to complete using the event
    evicted.WaitOne() |> ignore

    let metrics = CacheMetrics.GetStats "test_metrics"

    Assert.Contains("test_metrics | hit ratio", metrics)
