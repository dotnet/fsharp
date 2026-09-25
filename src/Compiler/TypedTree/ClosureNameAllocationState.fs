// Copyright (c) Microsoft Corporation. All Rights Reserved.

/// <summary>
/// Side-channel state for occurrence-keyed closure naming in hot reload compiles
/// (the lowering wiring described in docs/hot-reload-closure-mapping.md).
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

/// The per-compilation closure-name state. Exposed (not private) so IlxGen can
/// resolve the owner's holder ONCE per codegen run and consult it directly at the
/// per-closure/per-state-machine call sites instead of probing the weak table for
/// every closure.
[<Sealed>]
type ClosureNameStateHolder() =
    let syncRoot = obj ()
    let mutable recordedNamesByStamp: Dictionary<int64, string> option = None
    let mutable assignedNamesByStamp: Map<int64, string> = Map.empty
    let synthesizedNameOverrides = Dictionary<string, string>()
    // State machine resume points recorded by IlxGen lowering, keyed by the
    // emitted state machine struct's full type name. Shares the recording lifecycle of
    // recordedNamesByStamp: both begin/clear together, so flag-off compiles never pay.
    let mutable recordedResumePointsByTypeName: Dictionary<string, int list> option =
        None

    member _.BeginRecording() =
        lock syncRoot (fun () ->
            recordedNamesByStamp <- Some(Dictionary<int64, string>())
            synthesizedNameOverrides.Clear()
            recordedResumePointsByTypeName <- Some(Dictionary<string, int list>()))

    member _.Record(stamp: int64, name: string) =
        lock syncRoot (fun () ->
            match recordedNamesByStamp with
            | Some recorded -> recorded[stamp] <- name
            | None -> ())

    member _.RecordSynthesizedNameOverride(replayName: string, emittedName: string) =
        lock syncRoot (fun () ->
            if replayName <> emittedName then
                synthesizedNameOverrides[replayName] <- emittedName)

    member _.RecordedNames() =
        lock syncRoot (fun () ->
            match recordedNamesByStamp with
            | Some recorded -> recorded |> Seq.map (fun kvp -> kvp.Key, kvp.Value) |> Map.ofSeq
            | None -> Map.empty)

    member _.IsClosureNameRecordingActive =
        lock syncRoot (fun () -> recordedNamesByStamp.IsSome)

    member _.IsResumePointRecordingActive =
        lock syncRoot (fun () -> recordedResumePointsByTypeName.IsSome)

    member _.RecordResumePoints(typeName: string, resumePoints: int list) =
        lock syncRoot (fun () ->
            match recordedResumePointsByTypeName with
            | Some recorded -> recorded[typeName] <- resumePoints
            | None -> ())

    member _.RecordedResumePoints() =
        lock syncRoot (fun () ->
            match recordedResumePointsByTypeName with
            | Some recorded -> recorded |> Seq.map (fun kvp -> kvp.Key, kvp.Value) |> Map.ofSeq
            | None -> Map.empty)

    member _.SetAssignedNames(names: Map<int64, string>) =
        lock syncRoot (fun () ->
            assignedNamesByStamp <- names
            synthesizedNameOverrides.Clear())

    member _.TryGetAssignedName(stamp: int64) =
        lock syncRoot (fun () -> Map.tryFind stamp assignedNamesByStamp)

    member _.SynthesizedNameOverrides() =
        lock syncRoot (fun () -> synthesizedNameOverrides |> Seq.map (fun kvp -> kvp.Key, kvp.Value) |> Map.ofSeq)

    member _.Clear() =
        lock syncRoot (fun () ->
            recordedNamesByStamp <- None
            assignedNamesByStamp <- Map.empty
            synthesizedNameOverrides.Clear()
            recordedResumePointsByTypeName <- None)

let private holders = ConditionalWeakTable<obj, ClosureNameStateHolder>()

/// Read path: never allocates a holder, so compiles that never install state pay a
/// single failed weak-table lookup and stay behaviorally untouched.
let private tryGetHolder (owner: obj) =
    match holders.TryGetValue owner with
    | true, holder -> Some holder
    | _ -> None

let private getOrCreateHolder (owner: obj) =
    holders.GetValue(owner, fun _ -> ClosureNameStateHolder())

/// The installed closure-name state of the owner, when any was ever installed for
/// it; None on compiles that never installed state (flag-off, no emit hook).
/// Resolve once per codegen run so the per-closure call sites bypass the weak table.
let tryGetClosureNameState (owner: obj) : ClosureNameStateHolder option = tryGetHolder owner

/// Starts (or restarts) stamp -> emitted-closure-name recording for the owner's
/// compile. Installed by the emit hook for baseline-capture compiles only.
let beginClosureStampNameRecording (owner: obj) =
    (getOrCreateHolder owner).BeginRecording()

/// The stamp -> closure-name recording of the owner's compile; empty when recording
/// was never begun.
let getRecordedClosureStampNames (owner: obj) : Map<int64, string> =
    match tryGetHolder owner with
    | Some holder -> holder.RecordedNames()
    | None -> Map.empty

/// The state-machine-struct-name -> resume-point recording of the owner's compile;
/// empty when recording was never begun.
let getRecordedStateMachineResumePoints (owner: obj) : Map<string, int list> =
    match tryGetHolder owner with
    | Some holder -> holder.RecordedResumePoints()
    | None -> Map.empty

/// The replay-name -> final-emitted-name overrides observed at the closure lowering
/// call site during this compile. Applying these to FSharpSynthesizedTypeMaps.Snapshot
/// preserves the allocation slots while recording the names that actually hit IL.
let getSynthesizedNameOverrides (owner: obj) : Map<string, string> =
    match tryGetHolder owner with
    | Some holder -> holder.SynthesizedNameOverrides()
    | None -> Map.empty

let applySynthesizedNameOverrides
    (overrides: Map<string, string>)
    (snapshot: seq<struct (string * string[])>)
    : seq<struct (string * string[])> =

    snapshot
    |> Seq.map (fun struct (key, names) ->
        let names =
            names
            |> Array.map (fun name ->
                match Map.tryFind name overrides with
                | Some emittedName -> emittedName
                | None -> name)

        struct (key, names))

/// Installs the allocator-assigned stamp -> closure-name table for a delta compile.
let setAssignedClosureNames (owner: obj) (names: Map<int64, string>) =
    (getOrCreateHolder owner).SetAssignedNames(names)

/// Clears all closure-name state for the owner (recording and assigned names).
let clearClosureNameState (owner: obj) =
    match tryGetHolder owner with
    | Some holder -> holder.Clear()
    | None -> ()
