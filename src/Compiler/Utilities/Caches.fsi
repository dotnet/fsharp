namespace FSharp.Compiler.Caches

open System
open System.Collections.Generic
open System.Diagnostics.Metrics

module internal CacheMetrics =
    /// Global telemetry Meter for all caches. Exposed for testing purposes.
    /// Set FSHARP_OTEL_EXPORT environment variable to enable OpenTelemetry export to external collectors in tests.
    val Meter: Meter
    val ListenToAll: unit -> IDisposable
    val StatsToString: unit -> string
    val CaptureStatsAndWriteToConsole: unit -> IDisposable

    /// A local listener that can be created for a specific Cache instance to get its metrics. For testing purposes only.
    [<Class>]
    type internal CacheMetricsListener =
        member Ratio: float
        member GetTotals: unit -> Map<string, int64>
        interface IDisposable

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
    /// For testing only. Creates a local telemetry listener for this cache instance.
    member CreateMetricsListener: unit -> CacheMetrics.CacheMetricsListener
