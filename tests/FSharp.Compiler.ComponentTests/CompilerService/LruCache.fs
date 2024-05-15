module CompilerService.LruCache
open Internal.Utilities.Collections

open Xunit
open System

[<Fact>]
let ``Adding an item to the cache should make it retrievable``() =
    let cache = new LruCache<int, int, string>(keepStrongly = 2)
    cache.Set(1, "one")
    let result = cache.TryGet(1)
    Assert.Equal("one", result.Value)

[<Fact>]
let ``Adding an item to the cache should evict the least recently used item if the cache is full``() =
    let cache = new LruCache<int, int, string>(keepStrongly = 2, keepWeakly = 0)
    cache.Set(1, "one")
    cache.Set(2, "two")
    cache.Set(3, "three")
    let result = cache.TryGet(1)
    Assert.Null(result)

[<Fact>]
let ``Adding an item to the cache should not evict a required item``() =
    let cache = new LruCache<int, int, string>(keepStrongly = 2, requiredToKeep = (fun v -> v = "one"))
    cache.Set(1, "one")
    cache.Set(2, "two")
    cache.Set(3, "three")
    let result = cache.TryGet(1)
    Assert.Equal("one", result.Value)

[<Fact>]
let ``Adding an item to the cache should not evict a strongly kept item``() =
    let cache = new LruCache<int, int, string>(keepStrongly = 2, keepWeakly = 0)
    cache.Set(1, "one")
    cache.Set(2, "two")
    cache.Set(1, "one")
    cache.Set(3, "three")
    let result = cache.TryGet(1)
    Assert.Equal("one", result.Value)

[<Fact>]
let ``Adding an item to the cache should not evict a strongly kept item, even if it is the least recently used``() =
    let cache = new LruCache<int, int, string>(keepStrongly = 2, keepWeakly = 0, requiredToKeep = (fun v -> v = "one"))
    cache.Set(1, "one")
    cache.Set(2, "two")
    cache.Set(3, "three")
    let result = cache.TryGet(1)
    Assert.Equal("one", result.Value)

[<Fact>]
let ``Adding an item to the cache should not evict a weakly kept item if its reference is still valid``() =
    let cache = new LruCache<int, int, string>(keepStrongly = 2, keepWeakly = 1)
    let value = "one"
    cache.Set(1, value)
    cache.Set(2, "two")
    GC.Collect(2, GCCollectionMode.Forced, true)
    let result = cache.TryGet(1)
    Assert.Equal(value, result.Value)


// Doing this directly in the test prevents GC for some reason
let private addObjToCache (cache: LruCache<_, int,_>) key =
    let o = obj ()
    cache.Set(key, o)

[<Fact>]
let ``Adding an item to the cache should evict a weakly kept item if its reference is no longer valid``() =
    let cache = new LruCache<_, int, _>(keepStrongly = 2, keepWeakly = 1)
    addObjToCache cache 1
    addObjToCache cache 2
    addObjToCache cache 3
    GC.Collect(2, GCCollectionMode.Forced, true)

    let result = cache.TryGet(1)
    Assert.True(result.IsNone)


[<Fact>]
let ``When a new version is added other versions get weakened`` () =
    let eventLog = ResizeArray()

    let cache = new LruCache<_, int, _>(keepStrongly = 2, keepWeakly = 2, event = (fun e v -> eventLog.Add(e, v)))

    cache.Set(1, 1, "one1")
    cache.Set(1, 2, "one2")
    cache.Set(1, 3, "one3")
    cache.Set(1, 4, "one4")

    let expected = [
        CacheEvent.Weakened, ("[no label]", 1, 1)
        CacheEvent.Weakened, ("[no label]", 1, 2)
        CacheEvent.Weakened, ("[no label]", 1, 3)
        CacheEvent.Evicted, ("[no label]", 1, 1)
    ]

    Assert.Equal<list<_>>(expected, eventLog |> Seq.toList)

[<Fact>]
let ``When a new version is added other versions don't get weakened when they're required to keep`` () =
    let eventLog = ResizeArray()

    let cache = new LruCache<_, int, _>(keepStrongly = 2, keepWeakly = 2, requiredToKeep = ((=) "one1"), event = (fun e v -> eventLog.Add(e, v)))

    cache.Set(1, 1, "one1")
    cache.Set(1, 2, "one2")
    cache.Set(1, 3, "one3")
    cache.Set(1, 4, "one4")

    let expected = [
        CacheEvent.Weakened, ("[no label]", 1, 2)
        CacheEvent.Weakened, ("[no label]", 1, 3)
    ]

    Assert.Equal<list<_>>(expected, eventLog |> Seq.toList)

[<Fact>]
let ``Looking up a weakened item will strengthen it`` () =
    let eventLog = ResizeArray()

    let cache = new LruCache<_, int, _>(keepStrongly = 2, keepWeakly = 2, event = (fun e v -> eventLog.Add(e, v)))

    cache.Set(1, 1, "one1")
    cache.Set(1, 2, "one2")
    cache.Set(1, 3, "one3")
    cache.Set(1, 4, "one4")

    let result = cache.TryGet(1, 2)
    Assert.Equal("one2", result.Value)

    let expected = [
        CacheEvent.Weakened, ("[no label]", 1, 1)
        CacheEvent.Weakened, ("[no label]", 1, 2)
        CacheEvent.Weakened, ("[no label]", 1, 3)
        CacheEvent.Evicted, ("[no label]", 1, 1)
        CacheEvent.Strengthened, ("[no label]", 1, 2)
    ]

    Assert.Equal<list<_>>(expected, eventLog |> Seq.toList)


[<Fact>]
let ``New version doesn't push other keys out of strong list``() =

    let eventLog = ResizeArray()

    let cache = new LruCache<_, int, _>(keepStrongly = 2, keepWeakly = 2, event = (fun e v -> eventLog.Add(e, v)))

    cache.Set(1, 1, "one1")
    cache.Set(1, 2, "one2")
    cache.Set(1, 3, "one3")
    cache.Set(1, 4, "one4")
    cache.Set(2, 1, "two1")
    cache.Set(2, 2, "two2")

    let expected = [
        CacheEvent.Weakened, ("[no label]", 1, 1)
        CacheEvent.Weakened, ("[no label]", 1, 2)
        CacheEvent.Weakened, ("[no label]", 1, 3)
        CacheEvent.Evicted, ("[no label]", 1, 1)
        CacheEvent.Weakened, ("[no label]", 2, 1)
        CacheEvent.Evicted, ("[no label]", 1, 2)
    ]

    Assert.Equal<list<_>>(expected, eventLog |> Seq.toList)

[<Fact>]
let ``We can clear specific keys based on a predicate``() =

    let eventLog = ResizeArray()

    let cache = new LruCache<_, int, _>(keepStrongly = 2, keepWeakly = 2, event = (fun e v -> eventLog.Add(e, v)))

    cache.Set(1, 1, "one1")
    cache.Set(1, 2, "one2")
    cache.Set(1, 3, "one3")
    cache.Set(1, 4, "one4")
    cache.Set(2, 1, "two1")
    cache.Set(2, 2, "two2")

    cache.Clear((=) 1)

    let result = cache.TryGet(1, 2)
    Assert.True(result.IsNone)

    let expected = [
        CacheEvent.Weakened, ("[no label]", 1, 1)
        CacheEvent.Weakened, ("[no label]", 1, 2)
        CacheEvent.Weakened, ("[no label]", 1, 3)
        CacheEvent.Evicted, ("[no label]", 1, 1)
        CacheEvent.Weakened, ("[no label]", 2, 1)
        CacheEvent.Evicted, ("[no label]", 1, 2)
        CacheEvent.Cleared, ("[no label]", 1, 3)
        CacheEvent.Cleared, ("[no label]", 1, 4)
    ]

    Assert.Equal<list<_>>(expected, eventLog |> Seq.toList)
