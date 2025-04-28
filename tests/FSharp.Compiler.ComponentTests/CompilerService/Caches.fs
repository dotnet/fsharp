module CompilerService.Caches

open FSharp.Compiler.Caches

open System.Threading
open Xunit

[<Fact>]
let ``Basic add and retrieve`` () =
    use cache = Cache.Create<string, int>(CacheOptions.Default)
        
    cache.TryAdd("key1", 1) |> ignore
    cache.TryAdd("key2", 2) |> ignore
        
    let mutable value = 0
    Assert.True(cache.TryGetValue("key1", &value), "Should retrieve key1")
    Assert.Equal(1, value)
    Assert.True(cache.TryGetValue("key2", &value), "Should retrieve key2")
    Assert.Equal(2, value)
    Assert.False(cache.TryGetValue("key3", &value), "Should not retrieve non-existent key3")
        
    cache.Dispose()

[<Fact>]
let ``Eviction of least recently used`` () =
    use cache = Cache.Create<string, int>({ TotalCapacity = 2; HeadroomPercentage = 0 })
        
    cache.TryAdd("key1", 1) |> ignore
    cache.TryAdd("key2", 2) |> ignore
        
    // Make key1 recently used by accessing it
    let mutable value = 0
    cache.TryGetValue("key1", &value) |> ignore

    let evictionComplete = new ManualResetEvent(false)
    cache.BackgroundEvictionComplete.Add(fun _ -> evictionComplete.Set() |> ignore)
        
    // Add a third item, which should schedule key2 for eviction
    cache.TryAdd("key3", 3) |> ignore
        
    // Wait for eviction to complete using the event
    evictionComplete.WaitOne() |> ignore
        
    Assert.False(cache.TryGetValue("key2", &value), "key2 should have been evicted")
    Assert.True(cache.TryGetValue("key1", &value), "key1 should still be in cache")
    Assert.Equal(1, value)
    Assert.True(cache.TryGetValue("key3", &value), "key3 should be in cache")
    Assert.Equal(3, value)
