module Test
let memoizeBy (getKey : 'a -> string) (f: 'a -> 'b) : 'a -> 'b =
   let cache = System.Collections.Concurrent.ConcurrentDictionary<string, 'b>()
   fun (x: 'a) -> cache.GetOrAdd(getKey x, f)
