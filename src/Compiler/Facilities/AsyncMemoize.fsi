namespace Internal.Utilities.Collections

open System.Threading.Tasks
open FSharp.Compiler.BuildGraph

[<AutoOpen>]
module internal Utils =

    /// Return file name with one directory above it
    val shortPath: path: string -> string

    val (|TaskCancelled|_|): ex: exn -> TaskCanceledException option

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

type internal AsyncLock =
    interface System.IDisposable

    new: unit -> AsyncLock

    member Do: f: (unit -> #Task<'b>) -> Task<'b>

/// <summary>
/// A cache/memoization for computations that makes sure that the same computation wil only be computed once even if it's needed
/// at multiple places/times.
///
/// Strongly holds at most one result per key.
/// </summary>
type internal AsyncMemoize<'TKey, 'TVersion, 'TValue when 'TKey: equality and 'TVersion: equality> =

    /// <param name="keepStrongly">Maximum number of strongly held results to keep in the cache</param>
    /// <param name="keepWeakly">Maximum number of weakly held results to keep in the cache</param>
    /// <param name="name">Name of the cache - used in tracing messages</param>
    /// <param name="cancelDuplicateRunningJobs">If true, when a job is started, all other jobs with the same key will be canceled.</param>
    new:
        ?keepStrongly: int * ?keepWeakly: int * ?name: string * ?cancelDuplicateRunningJobs: bool ->
            AsyncMemoize<'TKey, 'TVersion, 'TValue>

    member Clear: unit -> unit

    member Clear: predicate: ('TKey -> bool) -> unit

    member Get: key: ICacheKey<'TKey, 'TVersion> * computation: NodeCode<'TValue> -> NodeCode<'TValue>

    member Get': key: 'TKey * computation: NodeCode<'TValue> -> NodeCode<'TValue>

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
