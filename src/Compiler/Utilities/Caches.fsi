namespace FSharp.Compiler.Caches

open System
open System.Diagnostics.Metrics
open System.Threading

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type internal CacheOptions =
    {
        /// Total capacity, determines the size of the underlying store.
        TotalCapacity: int

        /// Safety margin size as a percentage of TotalCapacity.
        HeadroomPercentage: int
    }

    static member Default: CacheOptions

module internal Cache =
    val OverrideCapacityForTesting: unit -> unit

[<Sealed; NoComparison; NoEquality>]
type internal Cache<'Key, 'Value when 'Key: not null and 'Key: equality> =
    new: totalCapacity: int * headroom: int * ?name: string * ?observeMetrics: bool -> Cache<'Key, 'Value>

    member TryGetValue: key: 'Key * value: outref<'Value> -> bool
    member TryAdd: key: 'Key * value: 'Value -> bool
    /// Cancels the background eviction task.
    member Dispose: unit -> unit

    interface IDisposable

    /// For testing only
    member Evicted: IEvent<unit>

    static member Create<'Key, 'Value> :
        options: CacheOptions * ?name: string * ?observeMetrics: bool -> Cache<'Key, 'Value>

[<Class>]
type internal CacheMetrics =
    static member Meter: Meter
    static member GetStats: cacheId: string -> string
    /// Retrieves current hit ratio, hits, misses, evictions etc. formatted for printing or logging.
    static member GetStatsUpdateForAllCaches: clearCounts: bool -> string
