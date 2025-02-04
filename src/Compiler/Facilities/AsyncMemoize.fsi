namespace Internal.Utilities.Collections

open System.Threading.Tasks
open FSharp.Compiler.BuildGraph

[<AutoOpen>]
module internal Utils =

    /// Return file name with one directory above it
    val shortPath: path: string -> string

type internal JobEvent =
    | Requested
    | Started
    | Restarted
    | Finished
    | Canceled
    | Evicted
    | Collected
    | Weakened
    | Strengthened
    | Failed
    | Cleared

type internal ICacheKey<'TKey, 'TVersion> =

    abstract GetKey: unit -> 'TKey

    abstract GetLabel: unit -> string

    abstract GetVersion: unit -> 'TVersion

[<System.Runtime.CompilerServices.Extension; Class>]
type Extensions =

    [<System.Runtime.CompilerServices.Extension>]
    static member internal WithExtraVersion: cacheKey: ICacheKey<'a, 'b> * extraVersion: 'c -> ICacheKey<'a, ('b * 'c)>

/// <summary>
/// A cache/memoization for computations that makes sure that the same computation will only be computed once even if it's needed
/// at multiple places/times.
///
/// Strongly holds at most one result per key.
/// </summary>
type internal AsyncMemoize<'TKey, 'TVersion, 'TValue when 'TKey: equality and 'TVersion: equality and 'TKey:not null and 'TVersion:not null> =

    /// <param name="keepStrongly">Maximum number of strongly held results to keep in the cache</param>
    /// <param name="keepWeakly">Maximum number of weakly held results to keep in the cache</param>
    /// <param name="name">Name of the cache - used in tracing messages</param>
    /// <param name="cancelUnawaitedJobs">Cancels a job when all the awaiting requests are canceled. If set to false, unawaited job will run to completion and it's result will be cached.</param>
    /// <param name="cancelDuplicateRunningJobs">If true, when a job is started, all other jobs with the same key will be canceled.</param>
    new:
        ?keepStrongly: int * ?keepWeakly: int * ?name: string * ?cancelUnawaitedJobs: bool * ?cancelDuplicateRunningJobs: bool ->
            AsyncMemoize<'TKey, 'TVersion, 'TValue>

    member Clear: unit -> unit

    member Clear: predicate: ('TKey -> bool) -> unit

    member Get: key: ICacheKey<'TKey, 'TVersion> * computation: Async<'TValue> -> Async<'TValue>

    member TryGet: key: 'TKey * predicate: ('TVersion -> bool) -> 'TValue option

    member Event: IEvent<JobEvent * (string * 'TKey * 'TVersion)>

    member OnEvent: ((JobEvent * (string * 'TKey * 'TVersion) -> unit) -> unit)

    member Count: int

/// A drop-in replacement for AsyncMemoize that disables caching and just runs the computation every time.
type internal AsyncMemoizeDisabled<'TKey, 'TVersion, 'TValue when 'TKey: equality and 'TVersion: equality> =

    new:
        ?keepStrongly: obj * ?keepWeakly: obj * ?name: string * ?cancelDuplicateRunningJobs: bool ->
            AsyncMemoizeDisabled<'TKey, 'TVersion, 'TValue>

    member Get: _key: ICacheKey<'a, 'b> * computation: 'c -> 'c
