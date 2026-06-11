// Copyright (c) Microsoft Corporation. All Rights Reserved.

/// <summary>
/// Side-channel state for occurrence-keyed closure naming in hot reload compiles
/// (Phase C3 lowering wiring, docs/hot-reload-closure-mapping.md).
///
/// IlxGen names every closure class at a single call site
/// (<c>GetIlxClosureFreeVars</c>), where the only identity shared with the typed-tree
/// lambda occurrence model is the lambda expression's unique stamp
/// (<c>Expr.Lambda(uniq, ...)</c>). This module carries two stamp-keyed tables across
/// that seam, attached to the compilation's <c>CompilerGlobalState</c> with a
/// <c>ConditionalWeakTable</c> exactly like <c>CompilerGeneratedNameMapState</c> so
/// core compiler signatures stay stable:
///
///  - a <b>recorder</b> (baseline capture, flag-gated by the emit hook): the closure
///    call site records stamp -> emitted closure type name, and the fsc emit path
///    joins the recording with the occurrence extraction of the same typed tree to
///    produce the baseline occurrence-chain -> name tables stored on the session
///    baseline;
///  - an <b>assigned-name table</b> (delta compiles): the emit hook runs the
///    occurrence-keyed allocator before lowering and installs stamp -> assigned name;
///    the closure call site consults it FIRST and falls back to the replayable name
///    map when the stamp is absent.
///
/// When no state is installed for an owner every operation is a no-op/None, so
/// flag-off and non-session compiles are byte-identical to upstream behavior.
/// </summary>
module internal FSharp.Compiler.ClosureNameAllocationState

open System.Collections.Generic
open System.Runtime.CompilerServices

[<Sealed>]
type private ClosureNameStateHolder() =
    let syncRoot = obj ()
    let mutable recordedNamesByStamp: Dictionary<int64, string> option = None
    let mutable assignedNamesByStamp: Map<int64, string> = Map.empty

    member _.BeginRecording() =
        lock syncRoot (fun () -> recordedNamesByStamp <- Some(Dictionary<int64, string>()))

    member _.Record(stamp: int64, name: string) =
        lock syncRoot (fun () ->
            match recordedNamesByStamp with
            | Some recorded -> recorded[stamp] <- name
            | None -> ())

    member _.RecordedNames() =
        lock syncRoot (fun () ->
            match recordedNamesByStamp with
            | Some recorded ->
                recorded |> Seq.map (fun kvp -> kvp.Key, kvp.Value) |> Map.ofSeq
            | None -> Map.empty)

    member _.SetAssignedNames(names: Map<int64, string>) =
        lock syncRoot (fun () -> assignedNamesByStamp <- names)

    member _.TryGetAssignedName(stamp: int64) =
        lock syncRoot (fun () -> Map.tryFind stamp assignedNamesByStamp)

    member _.Clear() =
        lock syncRoot (fun () ->
            recordedNamesByStamp <- None
            assignedNamesByStamp <- Map.empty)

let private holders = ConditionalWeakTable<obj, ClosureNameStateHolder>()

/// Read path: never allocates a holder, so compiles that never install state pay a
/// single failed weak-table lookup and stay behaviorally untouched.
let private tryGetHolder (owner: obj) =
    match holders.TryGetValue owner with
    | true, holder -> Some holder
    | _ -> None

let private getOrCreateHolder (owner: obj) =
    holders.GetValue(owner, fun _ -> ClosureNameStateHolder())

/// Starts (or restarts) stamp -> emitted-closure-name recording for the owner's
/// compile. Installed by the emit hook for baseline-capture compiles only.
let beginClosureStampNameRecording (owner: obj) =
    (getOrCreateHolder owner).BeginRecording()

/// Records the closure type name emitted for a lambda stamp. No-op unless recording
/// was begun for the owner.
let recordClosureStampName (owner: obj) (stamp: int64) (name: string) =
    match tryGetHolder owner with
    | Some holder -> holder.Record(stamp, name)
    | None -> ()

/// The stamp -> closure-name recording of the owner's compile; empty when recording
/// was never begun.
let getRecordedClosureStampNames (owner: obj) : Map<int64, string> =
    match tryGetHolder owner with
    | Some holder -> holder.RecordedNames()
    | None -> Map.empty

/// Installs the allocator-assigned stamp -> closure-name table for a delta compile.
let setAssignedClosureNames (owner: obj) (names: Map<int64, string>) =
    (getOrCreateHolder owner).SetAssignedNames(names)

/// The allocator-assigned name for a lambda stamp, when a table is installed and the
/// stamp was allocated; None means the caller keeps its existing naming behavior.
let tryGetAssignedClosureName (owner: obj) (stamp: int64) : string option =
    match tryGetHolder owner with
    | Some holder -> holder.TryGetAssignedName stamp
    | None -> None

/// Clears all closure-name state for the owner (recording and assigned names).
let clearClosureNameState (owner: obj) =
    match tryGetHolder owner with
    | Some holder -> holder.Clear()
    | None -> ()
