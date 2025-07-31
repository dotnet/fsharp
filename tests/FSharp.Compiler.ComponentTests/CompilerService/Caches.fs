module CompilerService.Caches

open FSharp.Compiler.Caches
open Xunit
open FSharp.Test.Assert
open System.Threading.Tasks
open System.Diagnostics

#if DEBUG
let shouldNeverTimeout = 15_000
#else
// accomodate unpredictable CI thread scheduling
let shouldNeverTimeout = 200_000
#endif

[<Fact>]
let ``Create and dispose many`` () =
    let caches = 
        [ for _  in 1 .. 100 do
            Cache.Create<string, int>(CacheOptions.Default, observeMetrics = true) ]

    for c in caches do
        c.Dispose()

[<Fact>]
let ``Create and dispose many named`` () =
    let caches = 
        [ for i in 1 .. 100 do
            Cache.Create<string, int>(CacheOptions.Default, name = $"testCache{i}", observeMetrics = true) ]

    for c in caches do
        c.Dispose()

[<Fact>]
let ``Basic add and retrieve`` () =
    use cache = Cache.Create<string, int>(CacheOptions.Default, observeMetrics = true)

    cache.TryAdd("key1", 1) |> shouldBeTrue
    cache.TryAdd("key2", 2) |> shouldBeTrue

    let mutable value = 0

    cache.TryGetValue("key1", &value) |> shouldBeTrue
    value |> shouldEqual 1

    cache.TryGetValue("key2", &value) |> shouldBeTrue
    value |> shouldEqual 2

    cache.TryGetValue("key3", &value) |> shouldBeFalse

[<Fact>]
let ``Eviction of least recently used`` () =
    use cache = Cache.Create<string, int>({ TotalCapacity = 2; HeadroomPercentage = 0 }, observeMetrics = true)

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

[<Fact>]
let ``Stress test evictions`` () =
    let cacheSize = 100
    let iterations = 10_000
    let name = "Stress test evictions"

    use cache = Cache.Create<string, int>({ TotalCapacity = cacheSize; HeadroomPercentage = 0 }, name = name, observeMetrics = true)

    let evictionsCompleted = new TaskCompletionSource<unit>()
    let expectedEvictions = iterations - cacheSize

    cache.Evicted.Add <| fun () ->
        if CacheMetrics.GetTotals(name).["evictions"] = expectedEvictions then
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

[<Fact>]
let ``Metrics can be retrieved`` () =
    use cache = Cache.Create<string, int>({ TotalCapacity = 2; HeadroomPercentage = 0 }, name = "test_metrics", observeMetrics = true)

    cache.TryAdd("key1", 1) |> shouldBeTrue
    cache.TryAdd("key2", 2) |> shouldBeTrue

    let mutable value = 0
    cache.TryGetValue("key1", &value) |> shouldBeTrue

    let evictionCompleted = TaskCompletionSource()
    cache.Evicted.Add(fun _ -> evictionCompleted.SetResult())
    cache.EvictionFailed.Add(fun _ -> evictionCompleted.SetException(Xunit.Sdk.FailException.ForFailure "eviction failed"))

    cache.TryAdd("key3", 3) |> shouldBeTrue
    evictionCompleted.Task.Wait shouldNeverTimeout |> shouldBeTrue

    let stats = CacheMetrics.GetStats "test_metrics"
    let totals = CacheMetrics.GetTotals "test_metrics"

    stats.["hit-ratio"] |> shouldEqual 1.0
    totals.["evictions"] |> shouldEqual 1L
