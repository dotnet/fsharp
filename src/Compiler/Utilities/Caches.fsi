namespace FSharp.Compiler

open System
open System.Threading

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type internal CacheOptions =
    { MaximumCapacity: int
      PercentageToEvict: int
      LevelOfConcurrency: int }

    static member Default: CacheOptions

[<Sealed; NoComparison; NoEquality>]
type internal CachedEntity<'Key, 'Value> =
    new: key: 'Key * value: 'Value -> CachedEntity<'Key, 'Value>
    member WithNode: unit -> CachedEntity<'Key, 'Value>
    member ReUse: key: 'Key * value: 'Value -> CachedEntity<'Key, 'Value>
    override ToString: unit -> string

[<Sealed; NoComparison; NoEquality>]
type internal Cache<'Key, 'Value when 'Key: not null and 'Key: equality> =
    new: options: CacheOptions * capacity: int * cts: CancellationTokenSource * ?name: string -> Cache<'Key, 'Value>
    member TryGetValue: key: 'Key * value: outref<'Value> -> bool
    member TryAdd: key: 'Key * value: 'Value -> bool
    member GetOrCreate: key: 'Key * valueFactory: ('Key -> 'Value) -> 'Value
    member Dispose: unit -> unit

    interface IDisposable

type internal CacheMetrics =
    new: cacheId: string -> CacheMetrics
    member CacheId: string
    member RecentStats: string
    member TryUpdateStats: clearCounts: bool -> bool
    static member GetStats: cacheId: string -> string
    static member GetStatsUpdateForAllCaches: clearCounts: bool -> string
    static member AddInstrumentation: cacheId: string -> unit
    static member RemoveInstrumentation: cacheId: string -> unit

module internal Cache =
    val OverrideMaxCapacityForTesting: unit -> unit
    val Create<'Key, 'Value when 'Key: not null and 'Key: equality> : options: CacheOptions -> Cache<'Key, 'Value>
