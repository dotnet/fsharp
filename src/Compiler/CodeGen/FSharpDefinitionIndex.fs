module internal FSharp.Compiler.CodeGen.FSharpDefinitionIndex

open System.Collections.Generic

/// Represents the status of a definition row tracked in the index.
type private EntryStatus<'T> =
    | Added of rowId: int * item: 'T
    | Existing of rowId: int * item: 'T

/// F# analogue of Roslyn's DefinitionIndex<T>
/// Track row ids for definitions reused from the baseline or added in this generation.
type DefinitionIndex<'T when 'T : not null and 'T : equality>(getExistingRowId: 'T -> int option, lastRowId: int) =
    let added = Dictionary<'T, int>()
    let rows = ResizeArray<EntryStatus<'T>>()
    let map = Dictionary<int, 'T>()
    let firstRowId = lastRowId + 1
    let mutable frozen = false

    let tryGetExistingRowId item =
        match getExistingRowId item with
        | Some rowId when rowId > 0 ->
            map[rowId] <- item
            Some rowId
        | _ -> None

    let getRowIdCore item =
        match added.TryGetValue item with
        | true, rowId -> rowId
        | false, _ ->
            match tryGetExistingRowId item with
            | Some rowId -> rowId
            | None -> invalidOp "Row id not found for definition."

    let ensureNotFrozen () =
        if frozen then invalidOp "Definition index has been frozen."

    let freeze () =
        if not frozen then
            frozen <- true
            rows.Sort(fun left right ->
                let rowId entry =
                    match entry with
                    | Added(rowId, _) -> rowId
                    | Existing(rowId, _) -> rowId

                compare (rowId left) (rowId right))

    member _.Add(item: 'T) =
        ensureNotFrozen ()
        if added.ContainsKey item then
            invalidOp "Definition has already been added."

        let rowId = firstRowId + added.Count
        added.Add(item, rowId)
        map[rowId] <- item
        rows.Add(Added(rowId, item))
        rowId

    member _.AddExisting(item: 'T) =
        ensureNotFrozen ()
        match tryGetExistingRowId item with
        | Some rowId -> rows.Add(Existing(rowId, item))
        | None -> invalidOp "Existing row id not found for definition."

    member _.GetRowId(item: 'T) =
        getRowIdCore item

    member _.Contains(item: 'T) =
        match added.TryGetValue item with
        | true, _ -> true
        | _ -> Option.isSome (tryGetExistingRowId item)

    member _.IsAdded(item: 'T) =
        added.ContainsKey item

    member _.TryGetDefinition(rowId: int) =
        match map.TryGetValue rowId with
        | true, item -> Some item
        | _ -> None

    member _.FirstRowId = firstRowId

    member _.NextRowId = firstRowId + added.Count

    member _.IsFrozen = frozen

    member _.Rows =
        freeze ()
        rows
        |> Seq.map (fun entry ->
            match entry with
            | Added(rowId, item) -> struct (rowId, item, true)
            | Existing(rowId, item) -> struct (rowId, item, false))
        |> Seq.toList

    member _.Added =
        freeze ()
        added
        |> Seq.map (fun kvp -> struct (kvp.Value, kvp.Key))
        |> Seq.sortBy (fun struct (rowId, _) -> rowId)
        |> Seq.toList
