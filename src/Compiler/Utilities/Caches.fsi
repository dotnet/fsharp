namespace FSharp.Compiler

open System
open System.Threading

[<Struct; RequireQualifiedAccess; NoComparison>]
type internal EvictionMethod =
    | Blocking
    | Background
    | NoEviction

[<Struct; RequireQualifiedAccess; NoComparison; NoEquality>]
type internal CacheOptions =
    { MaximumCapacity: int
      PercentageToEvict: int
      EvictionMethod: EvictionMethod
      LevelOfConcurrency: int }

    static member Default: CacheOptions

[<Sealed; NoComparison; NoEquality>]
type internal CachedEntity<'Key, 'Value> =
    new: key: 'Key * value: 'Value -> CachedEntity<'Key, 'Value>
    member WithNode: unit -> CachedEntity<'Key, 'Value>
    member ReUse: key: 'Key * value: 'Value -> CachedEntity<'Key, 'Value>
    override ToString: unit -> string

type internal IEvictionQueue<'Key, 'Value> =
    abstract member Add: CachedEntity<'Key, 'Value> -> unit
    abstract member Update: CachedEntity<'Key, 'Value> -> unit
    abstract member GetKeysToEvict: int -> 'Key[]
    abstract member Remove: CachedEntity<'Key, 'Value> -> unit

[<Sealed; NoComparison; NoEquality>]
type internal EvictionQueue<'Key, 'Value> =
    new: unit -> EvictionQueue<'Key, 'Value>
    member Count: int
    static member NoEviction: IEvictionQueue<'Key, 'Value>
    interface IEvictionQueue<'Key, 'Value>

type internal ICacheEvents =
    [<CLIEvent>]
    abstract member CacheHit: IEvent<unit>

    [<CLIEvent>]
    abstract member CacheMiss: IEvent<unit>

    [<CLIEvent>]
    abstract member Eviction: IEvent<unit>

    [<CLIEvent>]
    abstract member EvictionFail: IEvent<unit>

    [<CLIEvent>]
    abstract member OverCapacity: IEvent<unit>

[<Sealed; NoComparison; NoEquality>]
type internal Cache<'Key, 'Value when 'Key: not null and 'Key: equality> =
    new: options: CacheOptions * capacity: int * cts: CancellationTokenSource -> Cache<'Key, 'Value>
    member TryGetValue: key: 'Key * value: outref<'Value> -> bool
    member TryAdd: key: 'Key * value: 'Value -> bool
    member AddOrUpdate: key: 'Key * value: 'Value -> unit
    member Dispose: unit -> unit
    member GetStats: unit -> string

    static member Create<'Key, 'Value> : options: CacheOptions -> Cache<'Key, 'Value>

    interface ICacheEvents
    interface IDisposable

type internal CacheInstrumentation =
    new: cache: ICacheEvents -> CacheInstrumentation
    member CacheId: string
    member RecentStats: string
    member TryUpdateStats: clearCounts: bool -> bool
    static member GetStats: cache: ICacheEvents -> string
    static member GetStatsUpdateForAllCaches: clearCounts: bool -> string
    static member AddInstrumentation: cache: ICacheEvents -> unit
    static member RemoveInstrumentation: cache: ICacheEvents -> unit
