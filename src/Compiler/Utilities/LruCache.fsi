namespace Internal.Utilities.Collections

[<RequireQualifiedAccess>]
type internal CacheEvent =
    | Evicted
    | Collected
    | Weakened
    | Strengthened
    | Cleared

/// A cache where least recently used items are removed when the cache is full.
///
/// It's also versioned, meaning each key can have multiple versions and only the latest one is kept strongly.
/// Older versions are kept weakly and can be collected by GC.
type internal LruCache<'TKey, 'TVersion, 'TValue when 'TKey: equality and 'TVersion: equality and 'TValue: not struct
#if !NO_CHECKNULLS
    and 'TKey:not null
    and 'TVersion:not null
#endif
    > =

    /// <param name="keepStrongly">Maximum number of strongly held results to keep in the cache</param>
    /// <param name="keepWeakly">Maximum number of weakly held results to keep in the cache</param>
    /// <param name="requiredToKeep">A predicate that determines if a value should be kept strongly (no matter what)</param>
    /// <param name="event">An event that is called when an item is evicted, collected, weakened or strengthened</param>
    new:
        keepStrongly: int *
        ?keepWeakly: int *
        ?requiredToKeep: ('TValue -> bool) *
        ?event: (CacheEvent -> string * 'TKey * 'TVersion -> unit) ->
            LruCache<'TKey, 'TVersion, 'TValue>

    member Clear: unit -> unit

    /// Clear any keys that match the given predicate
    member Clear: predicate: ('TKey -> bool) -> unit

    /// Returns an option of a value for given key and version, and also a list of all other versions for given key
    member GetAll: key: 'TKey * version: 'TVersion -> 'TValue option * ('TVersion * 'TValue) list

    /// Returns a list of version * value pairs for a given key. The strongly held value is first in the list.
    member GetAll: key: 'TKey -> ('TVersion * 'TValue) seq

    member GetValues: unit -> (string * 'TVersion * 'TValue) seq

    /// Gets the number of items in the cache
    member Count: int

    member Remove: key: 'TKey -> unit

    member Remove: key: 'TKey * version: 'TVersion -> unit

    member Set: key: 'TKey * value: 'TValue -> unit

    member Set: key: 'TKey * version: 'TVersion * value: 'TValue -> unit

    member Set: key: 'TKey * version: 'TVersion * label: string * value: 'TValue -> unit

    member TryGet: key: 'TKey -> 'TValue option

    member TryGet: key: 'TKey * version: 'TVersion -> 'TValue option

    member DebuggerDisplay: string
