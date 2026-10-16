namespace FSharp.Compiler.Caches

open System
open System.Collections.Generic
open System.Diagnostics.Metrics

module CacheMetrics =
    /// Global telemetry Meter for all caches. Exposed for testing purposes.
    /// Set FSHARP_OTEL_EXPORT environment variable to enable OpenTelemetry export to external collectors in tests.
    val Meter: Meter

    /// Current metric totals aggregated across all cache instances with the given name.
    /// Totals only accumulate while a listener from ListenToAll is running.
    val internal getTotalsByName: name: string -> Map<string, int64>

    /// Current hit ratio (hits / (hits + misses)) aggregated across all cache instances with the given name.
    val internal getRatioByName: name: string -> float

    val internal ListenToAll: unit -> IDisposable
    val internal StatsToString: unit -> string
    val internal CaptureStatsAndWriteToConsole: unit -> IDisposable

[<RequireQualifiedAccess; NoComparison>]
type internal EvictionMode =
    /// Do not evict items, cache is effectively a ConcurrentDictionary.
    | NoEviction
    /// Evict items immediately on the caller's thread when adding a new item that would exceed capacity.
    | Immediate
    /// Evict items in the background using a MailboxProcessor to queue eviction requests. This may lag behind during heavy load but avoids blocking callers.
    | MailboxProcessor

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type internal CacheOptions<'Key> =
    {
        /// Total capacity, determines the size of the underlying store.
        TotalCapacity: int

        /// Safety margin size as a percentage of TotalCapacity.
        HeadroomPercentage: int

        /// Mechanism to use for evicting items from the cache.
        EvictionMode: EvictionMode

        /// Comparer to use for keys.
        Comparer: IEqualityComparer<'Key>
    }

module internal CacheOptions =
    /// Default options, using structural equality for keys and queued eviction.
    val getDefault: IEqualityComparer<'Key> -> CacheOptions<'Key>
    /// Default options, using reference equality for keys and queued eviction.
    val getReferenceIdentity: unit -> CacheOptions<'Key> when 'Key: not struct
    /// Set eviction mode to NoEviction.
    val withNoEviction: CacheOptions<'Key> -> CacheOptions<'Key>

[<Sealed; NoComparison; NoEquality>]
type internal Cache<'Key, 'Value when 'Key: not null> =
    new: options: CacheOptions<'Key> * ?name: string -> Cache<'Key, 'Value>
    member TryGetValue: key: 'Key * value: outref<'Value> -> bool
    member TryAdd: key: 'Key * value: 'Value -> bool
    member GetOrAdd: key: 'Key * valueFactory: ('Key -> 'Value) -> 'Value
    member AddOrUpdate: key: 'Key * value: 'Value -> unit

    interface IDisposable

    /// For testing only.
    member Evicted: IEvent<unit>
    /// For testing only.
    member EvictionFailed: IEvent<unit>
