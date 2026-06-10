module internal FSharp.Compiler.SynthesizedTypeMaps

open System
open System.Collections.Concurrent
open System.Collections.Generic

open FSharp.Compiler.GeneratedNames
open FSharp.Compiler.Syntax.PrettyNaming

/// <summary>Provides stable compiler-generated names across hot reload sessions.</summary>
type FSharpSynthesizedTypeMaps() =
    let syncLock = obj ()
    let buckets = ConcurrentDictionary<string, ResizeArray<string>>()
    let ordinals = ConcurrentDictionary<string, int>()

    let makeHotReloadName (baseName: string) ordinal =
        let suffix =
            if ordinal <= 0 then
                "hotreload"
            else
                $"hotreload-{ordinal}"

        CompilerGeneratedNameSuffix baseName suffix

    let createBucket (names: string[]) =
        let bucket = ResizeArray<string>()
        for name in names do
            bucket.Add(name)
        bucket

    let computeName basicName index =
        makeHotReloadName basicName index

    let tryGetHotReloadOrdinal (basicName: string) (name: string) =
        let hotReloadPrefix = basicName + "@hotreload"

        if name.Equals(hotReloadPrefix, StringComparison.Ordinal) then
            Some 0
        elif name.StartsWith(hotReloadPrefix + "-", StringComparison.Ordinal) then
            let suffix = name.Substring(hotReloadPrefix.Length + 1)
            match Int32.TryParse suffix with
            | true, ordinal when ordinal > 0 -> Some ordinal
            | _ -> None
        else
            None

    let canonicalizeSnapshotNames basicName (names: string[]) =
        let parsed =
            names
            |> Array.mapi (fun index name -> index, name, tryGetHotReloadOrdinal basicName name)

        if parsed |> Array.forall (fun (_, _, ordinalOpt) -> ordinalOpt.IsSome) then
            // IL metadata can enumerate synthesized helpers in a different order than allocation.
            // Normalize pure hot-reload buckets so replay always starts at ordinal 0, then 1, etc.
            parsed
            |> Array.sortBy (fun (index, _, ordinalOpt) -> struct (ordinalOpt.Value, index))
            |> Array.map (fun (_, name, _) -> name)
        else
            names

    /// Validates that a generated name starts with the basicName followed by '@'.
    let validateName basicName (name: string) index =
        // Snapshots can contain legacy/basic synthesized names (for example "@_instance")
        // alongside hot-reload-managed names. Accept both forms so existing sessions restore.
        let expectedPrefix = basicName + "@"
        if not (name.Equals(basicName, StringComparison.Ordinal) || name.StartsWith(expectedPrefix, StringComparison.Ordinal)) then
            invalidArg "snapshot" $"Name '{name}' at index {index} should equal '{basicName}' or start with '{expectedPrefix}' for basicName '{basicName}'"

    member _.GetOrAddName(basicName: string) =
        lock syncLock (fun () ->
            let bucket = buckets.GetOrAdd(basicName, fun _ -> ResizeArray())

            // Keep ordinal reservation and bucket mutation in one critical section so
            // concurrent callers cannot observe or produce out-of-order allocations.
            let index =
                match ordinals.TryGetValue basicName with
                | true, current ->
                    ordinals[basicName] <- current + 1
                    current
                | _ ->
                    ordinals[basicName] <- 1
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
            // Materialize the snapshot under the lock to avoid race conditions
            [| for KeyValue(key, bucket) in buckets do yield struct (key, bucket.ToArray()) |]
            :> seq<struct (string * string[])>)

    /// <summary>Loads a previously captured snapshot, replacing any existing allocation state.</summary>
    member _.LoadSnapshot(snapshot: seq<struct (string * string[])>) =
        lock syncLock (fun () ->
            buckets.Clear()
            ordinals.Clear()

            for struct (basicName, names) in snapshot do
                // Validate each name matches expected pattern
                names |> Array.iteri (fun i name -> validateName basicName name i)
                let canonicalNames = canonicalizeSnapshotNames basicName names
                let bucket = createBucket canonicalNames
                buckets[basicName] <- bucket
                ordinals[basicName] <- 0)

    interface ICompilerGeneratedNameMap with
        member this.BeginSession() = this.BeginSession()
        member this.GetOrAddName(basicName) = this.GetOrAddName(basicName)
        member this.Snapshot = this.Snapshot
        member this.LoadSnapshot(snapshot) = this.LoadSnapshot(snapshot)

/// <summary>Retrieves a stable compiler-generated name or falls back to the provided generator.</summary>
let nextName (mapOpt: ICompilerGeneratedNameMap option) basicName generate =
    match mapOpt with
    | Some (map: ICompilerGeneratedNameMap) -> map.GetOrAddName(basicName)
    | None -> generate ()
