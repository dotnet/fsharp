// Copyright (c) Microsoft Corporation. All Rights Reserved.

/// Connects typed-tree occurrence identities to closure names at the IlxGen stamp boundary.
/// Each CompilerGlobalState owns its capture and assignment tables through a weak reference.
/// IlxGen resolves the holder once per code generation run. No holder exists before capture or assignment.
module internal FSharp.Compiler.ClosureNameAllocationState

open System
open System.Collections.Generic
open System.Runtime.CompilerServices

/// The per-compilation closure-name state. Exposed (not private) so IlxGen can
/// resolve the owner's holder ONCE per codegen run and consult it directly at the
/// per-closure/per-state-machine call sites instead of probing the weak table for
/// every closure.
[<Sealed>]
type ClosureNameStateHolder() =
    // The lock protects capture lifecycle, name assignment, and independent snapshots.
    let syncRoot = obj ()
    let mutable recordedNamesByStamp: Dictionary<int64, string> voption = ValueNone
    let mutable assignedNamesByStamp: Map<int64, string> = Map.empty
    let synthesizedNameOverrides = Dictionary<string, string>(StringComparer.Ordinal)
    // State machine resume points recorded by IlxGen lowering, keyed by the
    // emitted state machine struct's full type name. Shares the recording lifecycle of
    // recordedNamesByStamp: both begin/clear together, so flag-off compiles never pay.
    let mutable recordedResumePointsByTypeName: Dictionary<string, int list> voption =
        ValueNone

    member _.BeginRecording() =
        lock syncRoot (fun () ->
            recordedNamesByStamp <- ValueSome(Dictionary<int64, string>())
            synthesizedNameOverrides.Clear()
            recordedResumePointsByTypeName <- ValueSome(Dictionary<string, int list>(StringComparer.Ordinal)))

    member _.Record(stamp: int64, name: string) =
        lock syncRoot (fun () ->
            match recordedNamesByStamp with
            | ValueSome recorded -> recorded[stamp] <- name
            | ValueNone -> ())

    member _.RecordSynthesizedNameOverride(replayName: string, emittedName: string) =
        lock syncRoot (fun () ->
            if replayName <> emittedName then
                synthesizedNameOverrides[replayName] <- emittedName)

    member _.RecordedNames() =
        lock syncRoot (fun () ->
            match recordedNamesByStamp with
            | ValueSome recorded -> recorded |> Seq.map (fun (KeyValue(key, value)) -> key, value) |> Map.ofSeq
            | ValueNone -> Map.empty)

    member _.IsClosureNameRecordingActive =
        lock syncRoot (fun () -> recordedNamesByStamp.IsSome)

    member _.IsResumePointRecordingActive =
        lock syncRoot (fun () -> recordedResumePointsByTypeName.IsSome)

    member _.RecordResumePoints(typeName: string, resumePoints: int list) =
        lock syncRoot (fun () ->
            match recordedResumePointsByTypeName with
            | ValueSome recorded -> recorded[typeName] <- resumePoints
            | ValueNone -> ())

    member _.RecordedResumePoints() =
        lock syncRoot (fun () ->
            match recordedResumePointsByTypeName with
            | ValueSome recorded -> recorded |> Seq.map (fun (KeyValue(key, value)) -> key, value) |> Map.ofSeq
            | ValueNone -> Map.empty)

    member _.SetAssignedNames(names: Map<int64, string>) =
        lock syncRoot (fun () ->
            assignedNamesByStamp <- names
            synthesizedNameOverrides.Clear())

    member _.TryGetAssignedName(stamp: int64) =
        lock syncRoot (fun () ->
            match assignedNamesByStamp.TryGetValue stamp with
            | true, name -> ValueSome name
            | _ -> ValueNone)

    member _.SynthesizedNameOverrides() =
        lock syncRoot (fun () ->
            synthesizedNameOverrides
            |> Seq.map (fun (KeyValue(key, value)) -> key, value)
            |> Map.ofSeq)

    member _.Clear() =
        lock syncRoot (fun () ->
            recordedNamesByStamp <- ValueNone
            assignedNamesByStamp <- Map.empty
            synthesizedNameOverrides.Clear()
            recordedResumePointsByTypeName <- ValueNone)

let private holders = ConditionalWeakTable<obj, ClosureNameStateHolder>()

/// Read path: never allocates a holder, so compiles that never install state pay a
/// single failed weak-table lookup and stay behaviorally untouched.
let private tryGetHolder (owner: obj) =
    match holders.TryGetValue owner with
    | true, holder -> ValueSome holder
    | _ -> ValueNone

let private getOrCreateHolder (owner: obj) =
    holders.GetValue(owner, fun _ -> ClosureNameStateHolder())

/// The installed closure-name state of the owner, when any was ever installed for
/// it; ValueNone on compiles that never installed state (flag-off, no emit hook).
/// Resolve once per codegen run so the per-closure call sites bypass the weak table.
let tryGetClosureNameState (owner: obj) : ClosureNameStateHolder voption = tryGetHolder owner

/// Starts (or restarts) stamp -> emitted-closure-name recording for the owner's
/// compile. Installed by the emit hook for baseline-capture compiles only.
let beginClosureStampNameRecording (owner: obj) =
    (getOrCreateHolder owner).BeginRecording()

/// The stamp -> closure-name recording of the owner's compile; empty when recording
/// was never begun.
let getRecordedClosureStampNames (owner: obj) : Map<int64, string> =
    match tryGetHolder owner with
    | ValueSome holder -> holder.RecordedNames()
    | ValueNone -> Map.empty

/// The state-machine-struct-name -> resume-point recording of the owner's compile;
/// empty when recording was never begun.
let getRecordedStateMachineResumePoints (owner: obj) : Map<string, int list> =
    match tryGetHolder owner with
    | ValueSome holder -> holder.RecordedResumePoints()
    | ValueNone -> Map.empty

/// The replay-name -> final-emitted-name overrides observed at the closure lowering
/// call site during this compile. Applying these to FSharpSynthesizedTypeMaps.Snapshot
/// preserves the allocation slots while recording the names that actually hit IL.
let getSynthesizedNameOverrides (owner: obj) : Map<string, string> =
    match tryGetHolder owner with
    | ValueSome holder -> holder.SynthesizedNameOverrides()
    | ValueNone -> Map.empty

let applySynthesizedNameOverrides
    (overrides: Map<string, string>)
    (snapshot: seq<struct (string * string[])>)
    : seq<struct (string * string[])> =

    snapshot
    |> Seq.map (fun struct (key, names) ->
        let names =
            names
            |> Array.map (fun name ->
                match overrides.TryGetValue name with
                | true, emittedName -> emittedName
                | _ -> name)

        struct (key, names))

/// Installs the allocator-assigned stamp -> closure-name table for a delta compile.
let setAssignedClosureNames (owner: obj) (names: Map<int64, string>) =
    (getOrCreateHolder owner).SetAssignedNames(names)

/// Clears all closure-name state for the owner (recording and assigned names).
let clearClosureNameState (owner: obj) =
    match tryGetHolder owner with
    | ValueSome holder -> holder.Clear()
    | ValueNone -> ()
