module internal FSharp.Compiler.SynthesizedTypeMaps

open System
open System.Collections.Generic

open FSharp.Compiler.CompilerGeneratedNameMapState
open FSharp.Compiler.GeneratedNames
open FSharp.Compiler.Syntax.PrettyNaming

/// <summary>
/// Provides stable compiler-generated names across hot reload sessions.
///
/// Replay buckets are keyed by line-normalized basic name. Bucket values remain the
/// original generation-0 full names, so a matched closure whose code moves from line
/// 28 to line 30 still gets its line-28 birth name back. That mirrors Roslyn EnC:
/// identity is established at first allocation and replayed exactly.
/// </summary>
type FSharpSynthesizedTypeMaps() =
    let syncLock = obj ()
    // Every access is protected by syncLock so allocation order and bucket updates stay atomic.
    let buckets = Dictionary<string, ResizeArray<string>>(StringComparer.Ordinal)
    let ordinals = Dictionary<string, int>(StringComparer.Ordinal)
    let mutable usesRecordedSnapshot = false

    let makeHotReloadName (baseName: string) ordinal =
        let suffix = if ordinal <= 0 then "hotreload" else $"hotreload-{ordinal}"

        CompilerGeneratedNameSuffix baseName suffix

    let createBucket (names: string[]) =
        let bucket = ResizeArray<string>()

        for name in names do
            bucket.Add(name)

        bucket

    let computeName basicName index = makeHotReloadName basicName index

    let getOrAddBucket mapKey =
        match buckets.TryGetValue mapKey with
        | true, bucket -> bucket
        | _ ->
            let bucket = ResizeArray<string>()
            buckets.Add(mapKey, bucket)
            bucket

    let tryGetHotReloadOrdinal (mapKey: string) (name: string) =
        match GeneratedNames.TryNormalizeHotReloadReplayName name with
        | Some replayName when replayName.NormalizedBasicName = mapKey -> Some replayName.ReplayOrdinal
        | _ -> None

    let tryGetStableOrdinal (mapKey: string) (name: string) =
        match GeneratedNames.TryNormalizeHotReloadReplayName name with
        | Some replayName when replayName.NormalizedBasicName = mapKey -> Some [ replayName.ReplayOrdinal ]
        | _ ->
            match GeneratedNames.TryNormalizeHotReloadGenerationName name with
            | Some generationName when generationName.NormalizedBasicName = mapKey -> Some generationName.OccurrenceOrdinal
            | _ -> None

    let canonicalizeSnapshotNames mapKey (names: string[]) =
        let parsed =
            names
            |> Array.mapi (fun index name -> index, name, tryGetHotReloadOrdinal mapKey name)

        if parsed |> Array.forall (fun (_, _, ordinalOpt) -> ordinalOpt.IsSome) then
            // IL metadata can enumerate synthesized helpers in a different order than allocation.
            // Normalize pure hot-reload buckets so replay always starts at ordinal 0, then 1, etc.
            let sorted =
                parsed
                |> Array.sortBy (fun (index, _, ordinalOpt) -> struct (ordinalOpt.Value, index))

            let ordinalsAreDistinct =
                let ordinals = sorted |> Array.map (fun (_, _, ordinalOpt) -> ordinalOpt.Value)
                (Array.distinct ordinals).Length = ordinals.Length

            if ordinalsAreDistinct && sorted.Length > 0 then
                // Place every name at the slot index its ordinal records, filling holes
                // with the computed name for that slot. Holes arise exactly where an
                // allocation's replay name never surfaced in IL (e.g. the slot was
                // consumed by a closure whose emitted name was overridden with an
                // occurrence-keyed name); the filler equals what GetOrAddName produced
                // for that slot originally, so replay positions are exact.
                let maxOrdinal =
                    sorted |> Array.map (fun (_, _, ordinalOpt) -> ordinalOpt.Value) |> Array.max

                let namesByOrdinal =
                    sorted
                    |> Array.map (fun (_, name, ordinalOpt) -> ordinalOpt.Value, name)
                    |> Map.ofArray

                let replayFillBasicName =
                    let rawBasicNames =
                        sorted
                        |> Array.choose (fun (_, name, _) ->
                            let rawBasicName = GetBasicNameOfPossibleCompilerGeneratedName name

                            if String.Equals(GeneratedNames.SynthesizedNameMapKey rawBasicName, mapKey, StringComparison.Ordinal) then
                                Some rawBasicName
                            else
                                None)
                        |> Array.distinct

                    match rawBasicNames with
                    | [| rawBasicName |] -> rawBasicName
                    | _ -> mapKey

                Array.init (maxOrdinal + 1) (fun slot ->
                    match Map.tryFind slot namesByOrdinal with
                    | Some name -> name
                    | None -> makeHotReloadName replayFillBasicName slot)
            else
                sorted |> Array.map (fun (_, name, _) -> name)
        else
            let parsed =
                names
                |> Array.mapi (fun index name -> index, name, tryGetStableOrdinal mapKey name)

            if parsed |> Array.forall (fun (_, _, ordinalOpt) -> ordinalOpt.IsSome) then
                let sorted =
                    parsed
                    |> Array.sortBy (fun (index, _, ordinalOpt) -> struct (ordinalOpt.Value, index))

                let ordinalsAreDistinct =
                    let ordinals = sorted |> Array.map (fun (_, _, ordinalOpt) -> ordinalOpt.Value)
                    (Array.distinct ordinals).Length = ordinals.Length

                if ordinalsAreDistinct then
                    sorted |> Array.map (fun (_, name, _) -> name)
                else
                    names
            else
                names

    let nameMapKeyFromSnapshotName (name: string) =
        GetBasicNameOfPossibleCompilerGeneratedName name
        |> GeneratedNames.SynthesizedNameMapKey

    /// Validates that a generated name belongs to the normalized map key.
    let validateName mapKey (name: string) index =
        // Snapshots can contain legacy/basic synthesized names (for example "@_instance")
        // alongside hot-reload-managed names. Accept both forms so existing sessions restore.
        let actualKey = nameMapKeyFromSnapshotName name

        if not (String.Equals(actualKey, mapKey, StringComparison.Ordinal)) then
            invalidArg "snapshot" $"Name '{name}' at index {index} belongs to normalized key '{actualKey}', not snapshot key '{mapKey}'"

    let loadSnapshotCore canonicalize (snapshot: seq<struct (string * string[])>) =
        lock syncLock (fun () ->
            buckets.Clear()
            ordinals.Clear()
            usesRecordedSnapshot <- not canonicalize

            let normalizedBuckets =
                Dictionary<string, ResizeArray<string>>(StringComparer.Ordinal)

            for struct (basicName, names) in snapshot do
                let mapKey = GeneratedNames.SynthesizedNameMapKey basicName

                if canonicalize then
                    // Validate each name matches the normalized key. Loading normalizes
                    // old raw-key snapshots, so on-disk baselines captured before this
                    // change replay through the same line-stable buckets.
                    names |> Array.iteri (fun i name -> validateName mapKey name i)
                else
                    // Recorded snapshots are allocation-key -> final-emitted-name slots.
                    // Occurrence-keyed closure overrides can intentionally move a final
                    // name into a bucket whose allocation key differs from the name's
                    // derived key, so only null validation applies here.
                    names
                    |> Array.iteri (fun i name ->
                        if isNull (box name) then
                            invalidArg "snapshot" $"Name at index {i} in snapshot key '{mapKey}' is null")

                let namesToLoad =
                    if canonicalize then
                        canonicalizeSnapshotNames mapKey names
                    else
                        // Recorded hot reload CDI snapshots are already the allocation
                        // order. Keep them identity-preserving after validation; old
                        // reconstructed snapshots continue through canonicalization.
                        Array.copy names

                let bucket =
                    match normalizedBuckets.TryGetValue mapKey with
                    | true, existing -> existing
                    | _ ->
                        let created = ResizeArray<string>()
                        normalizedBuckets[mapKey] <- created
                        created

                for name in namesToLoad do
                    if canonicalize then
                        if not (bucket.Contains name) then
                            bucket.Add name
                    else
                        bucket.Add name

            for KeyValue(mapKey, bucket) in normalizedBuckets do
                buckets[mapKey] <- createBucket (bucket.ToArray())
                ordinals[mapKey] <- 0)

    member _.GetOrAddName(basicName: string) =
        lock syncLock (fun () ->
            let mapKey = GeneratedNames.SynthesizedNameMapKey basicName
            let bucket = getOrAddBucket mapKey

            // Keep ordinal reservation and bucket mutation in one critical section so
            // concurrent callers cannot observe or produce out-of-order allocations.
            // The ordinal is intentionally the encounter order within the normalized
            // bucket. If same-bucket closures are reordered, the downstream
            // positional-pairing shape guard owns that concern; this allocator only
            // replays generation-0 names for matching allocation slots.
            let index =
                match ordinals.TryGetValue mapKey with
                | true, current ->
                    ordinals[mapKey] <- current + 1
                    current
                | _ ->
                    ordinals[mapKey] <- 1
                    0

            if index < bucket.Count then
                bucket[index]
            else
                let name = computeName basicName index
                bucket.Add(name)
                name)

    /// <summary>Resets allocation state so subsequent edits reuse the original name ordering.</summary>
    member _.BeginSession() =
        lock syncLock (fun () ->
            for KeyValue(key, _) in buckets do
                ordinals[key] <- 0)

    /// <summary>Captures the current stable names grouped by compiler-generated base name.</summary>
    member _.Snapshot: seq<struct (string * string[])> =
        lock syncLock (fun () ->
            // Copy and order the complete snapshot while holding the allocation lock.
            buckets
            |> Seq.map (fun (KeyValue(key, bucket)) -> struct (key, bucket.ToArray()))
            |> Seq.sortWith (fun struct (left, _) struct (right, _) -> StringComparer.Ordinal.Compare(left, right))
            |> Seq.toArray
            :> seq<struct (string * string[])>)

    member _.UsesRecordedSnapshot = lock syncLock (fun () -> usesRecordedSnapshot)

    /// <summary>Loads a previously captured snapshot, replacing any existing allocation state.</summary>
    member _.LoadSnapshot(snapshot: seq<struct (string * string[])>) = loadSnapshotCore true snapshot

    /// <summary>
    /// Loads a snapshot that was recorded from this allocator's own allocation slots.
    /// The bucket arrays are ground truth, so this intentionally skips IL-order
    /// reconstruction canonicalization and key-derived name validation.
    /// </summary>
    member _.LoadRecordedSnapshot(snapshot: seq<struct (string * string[])>) = loadSnapshotCore false snapshot

    interface ICompilerGeneratedNameMap with
        member this.BeginSession() = this.BeginSession()
        member this.GetOrAddName(basicName) = this.GetOrAddName(basicName)
        member this.Snapshot = this.Snapshot
        member this.LoadSnapshot(snapshot) = this.LoadSnapshot(snapshot)

/// <summary>Retrieves a stable compiler-generated name or falls back to the provided generator.</summary>
let nextName (mapOpt: ICompilerGeneratedNameMap option) basicName generate =
    match mapOpt with
    | Some(map: ICompilerGeneratedNameMap) -> map.GetOrAddName(basicName)
    | None -> generate ()
