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

let private getHolder (owner: obj) =
    holders.GetValue(owner, fun _ -> NameMapHolder())

let tryGetCompilerGeneratedNameMap (owner: obj) =
    getHolder owner |> fun holder -> holder.TryGet()

/// A reader for the owner's name-map slot that resolves the weak-table entry once (on
/// first use) and afterwards reads a single field per call. The per-name path of
/// NiceNameGenerator goes through this so compiles without an installed map (no
/// --enable:hotreloaddeltas hook) pay one None check per generated name instead of a
/// ConditionalWeakTable probe and lock.
let getCompilerGeneratedNameMapAccessor (owner: obj) : unit -> ICompilerGeneratedNameMap option =
    let mutable resolvedHolder = ValueNone

    fun () ->
        match resolvedHolder with
        | ValueSome(holder: NameMapHolder) -> holder.TryGet()
        | ValueNone ->
            // Benign race: GetValue returns the same holder for concurrent resolvers.
            let holder = getHolder owner
            resolvedHolder <- ValueSome holder
            holder.TryGet()

let setCompilerGeneratedNameMap (owner: obj) (map: ICompilerGeneratedNameMap) =
    getHolder owner |> fun holder -> holder.Set(Some map)

let setCompilerGeneratedNameMapOpt (owner: obj) (map: ICompilerGeneratedNameMap option) =
    getHolder owner |> fun holder -> holder.Set(map)

let clearCompilerGeneratedNameMap (owner: obj) =
    getHolder owner |> fun holder -> holder.Set(None)
