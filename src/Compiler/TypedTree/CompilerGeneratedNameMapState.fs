module internal FSharp.Compiler.CompilerGeneratedNameMapState

open System.Runtime.CompilerServices

/// Minimal abstraction for compiler-generated name replay/state.
/// Implementations can be hot-reload aware without coupling core compiler paths
/// to a concrete synthesized-name map type.
type ICompilerGeneratedNameMap =
    /// Resets allocation cursors so the next serialized code-generation pass replays the snapshot from its first slot.
    abstract BeginSession: unit -> unit

    /// Returns the next name in deterministic encounter order for this basic name.
    /// Consumers must serialize code generation while a map is installed: synchronization prevents data races,
    /// but concurrent callers cannot make encounter order independent of thread scheduling.
    abstract GetOrAddName: basicName: string -> string

    /// Captures the names in allocation order, grouped by normalized basic name.
    abstract Snapshot: seq<struct (string * string[])>

    /// Replaces the current replay state with a previously captured allocation-order snapshot.
    abstract LoadSnapshot: snapshot: seq<struct (string * string[])> -> unit

// Keep optional name-map state external to CompilerGlobalState so core signatures can remain stable.
type private NameMapHolder() =
    // Reads vastly outnumber writes. Installs happen at most a handful of times per
    // compile, so the slot is a single volatile field rather than a lock-guarded one.
    // Reference reads and writes are atomic, and the volatile semantics preserve the
    // visibility ordering the lock provided.
    [<VolatileField>]
    let mutable current: ICompilerGeneratedNameMap option = None

    member _.TryGet() = current
    member _.Set(value: ICompilerGeneratedNameMap option) = current <- value

let private holders = ConditionalWeakTable<obj, NameMapHolder>()

let private getOrCreateHolder (owner: obj) =
    holders.GetValue(owner, fun _ -> NameMapHolder())

/// Pure read: never inserts, so a compile that never installs a map pays a single
/// failed weak-table lookup.
let private tryGetHolder (owner: obj) =
    match holders.TryGetValue owner with
    | true, holder -> Some holder
    | _ -> None

let tryGetCompilerGeneratedNameMap (owner: obj) =
    match tryGetHolder owner with
    | Some holder -> holder.TryGet()
    | None -> None

/// A reader for the owner's name-map slot. The holder is resolved exactly once here
/// and captured by the returned closure, so each generated name costs a single
/// volatile field read rather than a ConditionalWeakTable probe and lock.
///
/// The holder is created eagerly on purpose: the emit hook can install the map later
/// in the compile, after CompilerGlobalState and therefore this accessor have been
/// constructed, and it installs through the same owner. Pre-creating the holder means
/// that later install mutates the object this closure captured, so the map is observed.
let getCompilerGeneratedNameMapAccessor (owner: obj) : unit -> ICompilerGeneratedNameMap option =
    let holder = getOrCreateHolder owner
    fun () -> holder.TryGet()

let setCompilerGeneratedNameMap (owner: obj) (map: ICompilerGeneratedNameMap) = (getOrCreateHolder owner).Set(Some map)

let setCompilerGeneratedNameMapOpt (owner: obj) (map: ICompilerGeneratedNameMap option) = (getOrCreateHolder owner).Set(map)

let clearCompilerGeneratedNameMap (owner: obj) = (getOrCreateHolder owner).Set(None)
