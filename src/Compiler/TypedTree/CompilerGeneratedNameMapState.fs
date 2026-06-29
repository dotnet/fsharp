module internal FSharp.Compiler.CompilerGeneratedNameMapState

open System.Runtime.CompilerServices
open FSharp.Compiler.GeneratedNames

// Keep optional name-map state external to CompilerGlobalState so core signatures can remain stable.
type private NameMapHolder() =
    // Reads vastly outnumber writes (every compiler-generated name consults the slot;
    // installs happen at most a handful of times per compile), so the slot is a single
    // volatile field rather than a lock-guarded one. Reference reads/writes are atomic
    // and the volatile semantics preserve the visibility ordering the lock provided.
    [<VolatileField>]
    let mutable current: ICompilerGeneratedNameMap option = None

    member _.TryGet() = current
    member _.Set(value: ICompilerGeneratedNameMap option) = current <- value

let private holders = ConditionalWeakTable<obj, NameMapHolder>()

let private getOrCreateHolder (owner: obj) =
    holders.GetValue(owner, fun _ -> NameMapHolder())

/// Pure read: never inserts, so a compile that never installs a map pays a single
/// failed weak-table lookup (mirrors ClosureNameAllocationState.tryGetHolder).
let private tryGetHolder (owner: obj) =
    match holders.TryGetValue owner with
    | true, holder -> Some holder
    | _ -> None

let tryGetCompilerGeneratedNameMap (owner: obj) =
    match tryGetHolder owner with
    | Some holder -> holder.TryGet()
    | None -> None

/// A reader for the owner's name-map slot. The holder is resolved exactly ONCE here and
/// captured by the returned closure, so each generated name costs a single volatile field
/// read (holder.TryGet()) rather than a ConditionalWeakTable probe and lock.
///
/// The holder is created eagerly (GetValue), not resolved lazily, on purpose: the emit
/// hook installs the map LATER in the compile (HotReloadEmitHook.PrepareForCodeGeneration),
/// after CompilerGlobalState — and therefore this accessor — has been constructed, and it
/// installs through the SAME owner. Pre-creating the holder means that later install mutates
/// the very object this closure captured, so the map is observed. Resolving lazily with a
/// mutable cache would reintroduce a torn read of the multi-field option across the parallel
/// IlxGen threads (volatile cannot make a multi-field struct write atomic); resolving via
/// TryGetValue up front would capture None and miss the install entirely. Exactly one holder
/// is allocated per CompilerGlobalState (once per compile), never per generated name.
let getCompilerGeneratedNameMapAccessor (owner: obj) : unit -> ICompilerGeneratedNameMap option =
    let holder = getOrCreateHolder owner
    fun () -> holder.TryGet()

let setCompilerGeneratedNameMap (owner: obj) (map: ICompilerGeneratedNameMap) = (getOrCreateHolder owner).Set(Some map)

let setCompilerGeneratedNameMapOpt (owner: obj) (map: ICompilerGeneratedNameMap option) = (getOrCreateHolder owner).Set(map)

let clearCompilerGeneratedNameMap (owner: obj) = (getOrCreateHolder owner).Set(None)
