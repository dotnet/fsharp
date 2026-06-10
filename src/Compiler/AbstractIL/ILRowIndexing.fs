// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Abstractions for metadata row ID assignment.
/// Used by both full assembly emission (ilwrite.fs) and delta emission (IlxDeltaEmitter.fs)
/// to provide a unified interface for assigning and tracking row IDs.
module internal FSharp.Compiler.AbstractIL.ILRowIndexing

/// Strategy for assigning row IDs to definitions.
/// Implementations differ for full assembly (sequential) vs delta (baseline-aware).
type IRowIndexStrategy<'TKey when 'TKey : equality> =
    /// Try to get an existing row ID for an item from a previous generation.
    /// Returns None if this is a new item.
    abstract TryGetExistingRowId: 'TKey -> int option

    /// Assign a new row ID for an item.
    /// For full assembly: sequential from 1
    /// For delta: sequential from lastRowId + 1
    abstract AssignNewRowId: 'TKey -> int

    /// Get the row ID for an item (existing or added).
    abstract GetRowId: 'TKey -> int

    /// Check if an item is a new addition (not from baseline).
    abstract IsAdded: 'TKey -> bool

    /// Check if an item is present (either added or existing).
    abstract Contains: 'TKey -> bool

/// Full assembly row indexing - all items are new, assigned sequentially.
/// Used by ilwrite.fs for baseline assembly emission.
type FullAssemblyRowIndex<'TKey when 'TKey : equality and 'TKey : not null>() =
    let items = System.Collections.Generic.Dictionary<'TKey, int>()
    let mutable nextRowId = 1

    interface IRowIndexStrategy<'TKey> with
        member _.TryGetExistingRowId(_key) = None

        member _.AssignNewRowId(key) =
            if items.ContainsKey(key) then
                invalidOp "Item already has a row ID assigned"
            let rowId = nextRowId
            items.[key] <- rowId
            nextRowId <- nextRowId + 1
            rowId

        member _.GetRowId(key) =
            match items.TryGetValue(key) with
            | true, rowId -> rowId
            | false, _ -> invalidOp "Row ID not found for item"

        member _.IsAdded(_key) = true  // All items are "added" in full assembly

        member _.Contains(key) = items.ContainsKey(key)

    /// The next row ID that will be assigned.
    member _.NextRowId = nextRowId

    /// Total number of rows assigned.
    member _.Count = nextRowId - 1

/// Delta row indexing - tracks baseline items and additions separately.
/// Generic implementation that can be used with any getExistingRowId function.
type DeltaRowIndex<'TKey when 'TKey : equality and 'TKey : not null>(getExistingRowId: 'TKey -> int option, lastRowId: int) =
    let added = System.Collections.Generic.Dictionary<'TKey, int>()
    let existing = System.Collections.Generic.Dictionary<'TKey, int>()
    let firstAddedRowId = lastRowId + 1

    interface IRowIndexStrategy<'TKey> with
        member _.TryGetExistingRowId(key) =
            match existing.TryGetValue(key) with
            | true, rowId -> Some rowId
            | false, _ ->
                match getExistingRowId key with
                | Some rowId when rowId > 0 ->
                    existing.[key] <- rowId
                    Some rowId
                | _ -> None

        member this.AssignNewRowId(key) =
            if added.ContainsKey(key) then
                invalidOp "Item already has a row ID assigned"
            let iface = this :> IRowIndexStrategy<'TKey>
            if iface.TryGetExistingRowId(key).IsSome then
                invalidOp "Cannot assign new row ID to existing item"
            let rowId = firstAddedRowId + added.Count
            added.[key] <- rowId
            rowId

        member this.GetRowId(key) =
            match added.TryGetValue(key) with
            | true, rowId -> rowId
            | false, _ ->
                let iface = this :> IRowIndexStrategy<'TKey>
                match iface.TryGetExistingRowId(key) with
                | Some rowId -> rowId
                | None -> invalidOp "Row ID not found for item"

        member _.IsAdded(key) =
            added.ContainsKey(key)

        member this.Contains(key) =
            let iface = this :> IRowIndexStrategy<'TKey>
            added.ContainsKey(key) || iface.TryGetExistingRowId(key).IsSome

    /// The first row ID for added items.
    member _.FirstAddedRowId = firstAddedRowId

    /// The next row ID that will be assigned.
    member _.NextRowId = firstAddedRowId + added.Count

    /// Number of items added in this generation.
    member _.AddedCount = added.Count

    /// Get all added items as (rowId, key) pairs sorted by rowId.
    member _.AddedItems =
        added
        |> Seq.map (fun kvp -> struct (kvp.Value, kvp.Key))
        |> Seq.sortBy (fun struct (rowId, _) -> rowId)
        |> Seq.toList
