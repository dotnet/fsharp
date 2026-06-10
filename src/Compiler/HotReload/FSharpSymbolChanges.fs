module internal FSharp.Compiler.HotReload.SymbolChanges

open FSharp.Compiler.HotReload.DefinitionMap
open FSharp.Compiler.TypedTreeDiff

/// Represents a single synthesized member edit along with hash metadata.
type SynthesizedMemberChange =
    { Symbol: SymbolId
      EditKind: SymbolEditKind
      BaselineHash: int option
      UpdatedHash: int option
      ContainingEntity: string option }

type UpdatedSymbolChange =
    { Symbol: SymbolId
      Kind: SemanticEditKind
      ContainingEntity: string option }

/// Aggregated symbol changes derived from the typed-tree diff and definition map.
type FSharpSymbolChanges =
    { Added: SymbolId list
      Updated: UpdatedSymbolChange list
      Deleted: SymbolId list
      Synthesized: SynthesizedMemberChange list
      RudeEdits: RudeEdit list }

module FSharpSymbolChanges =
    /// Builds `FSharpSymbolChanges` from a definition map, mirroring Roslyn's `SymbolChanges`.
    let ofDefinitionMap (definitionMap: FSharpDefinitionMap) : FSharpSymbolChanges =
        let synthesized =
            definitionMap
            |> FSharpDefinitionMap.synthesized
            |> List.map (fun change ->
                { Symbol = change.Symbol
                  EditKind = change.EditKind
                  BaselineHash = change.BaselineHash
                  UpdatedHash = change.UpdatedHash
                  ContainingEntity = change.ContainingEntity })

        let updated =
            definitionMap
            |> FSharpDefinitionMap.updated
            |> List.map (fun (change, kind) ->
                { Symbol = change.Symbol
                  Kind = kind
                  ContainingEntity = change.ContainingEntity })

        { Added = FSharpDefinitionMap.added definitionMap
          Updated = updated
          Deleted = FSharpDefinitionMap.deleted definitionMap
          Synthesized = synthesized
          RudeEdits = definitionMap.RudeEdits }

    /// Collects entity symbols (types/modules) impacted by adds/updates/deletes, including synthesized members promoted to entities.
    let entitySymbolsWithChanges (changes: FSharpSymbolChanges) : SymbolId list =
        let updatedEntities =
            changes.Updated
            |> Seq.choose (fun change -> if change.Symbol.Kind = SymbolKind.Entity then Some change.Symbol else None)

        let addedEntities =
            changes.Added
            |> Seq.filter (fun symbol -> symbol.Kind = SymbolKind.Entity)

        let deletedEntities =
            changes.Deleted
            |> Seq.filter (fun symbol -> symbol.Kind = SymbolKind.Entity)

        let synthesizedEntities =
            changes.Synthesized
            |> Seq.choose (fun change -> if change.Symbol.Kind = SymbolKind.Entity then Some change.Symbol else None)

        seq {
            yield! updatedEntities
            yield! addedEntities
            yield! deletedEntities
            yield! synthesizedEntities
        }
        |> Seq.distinctBy (fun symbol -> struct (symbol.Path, symbol.LogicalName, symbol.Stamp))
        |> Seq.toList
    /// Extracts synthesized members classified as added.
    let synthesizedAdded (changes: FSharpSymbolChanges) : SymbolId list =
        changes.Synthesized
        |> List.choose (fun change ->
            match change.EditKind with
            | SymbolEditKind.Added -> Some change.Symbol
            | _ -> None)

    /// Extracts synthesized members classified as updated.
    let synthesizedUpdated (changes: FSharpSymbolChanges) : (SymbolId * SemanticEditKind) list =
        changes.Synthesized
        |> List.choose (fun change ->
            match change.EditKind with
            | SymbolEditKind.Updated kind -> Some(change.Symbol, kind)
            | _ -> None)

    /// Extracts synthesized members classified as deleted.
    let synthesizedDeleted (changes: FSharpSymbolChanges) : SymbolId list =
        changes.Synthesized
        |> List.choose (fun change ->
            match change.EditKind with
            | SymbolEditKind.Deleted -> Some change.Symbol
            | _ -> None)

    let private isPropertySymbol symbol =
        match symbol.MemberKind with
        | Some (SymbolMemberKind.PropertyGet _)
        | Some (SymbolMemberKind.PropertySet _) -> true
        | _ -> false

    let private isEventSymbol symbol =
        match symbol.MemberKind with
        | Some (SymbolMemberKind.EventAdd _)
        | Some (SymbolMemberKind.EventRemove _)
        | Some (SymbolMemberKind.EventInvoke _) -> true
        | _ -> false

    let propertyAccessorsAdded (changes: FSharpSymbolChanges) : SymbolId list =
        changes.Added |> List.filter isPropertySymbol

    let propertyAccessorsUpdated (changes: FSharpSymbolChanges) : UpdatedSymbolChange list =
        changes.Updated |> List.filter (fun change -> isPropertySymbol change.Symbol)

    let propertyAccessorsDeleted (changes: FSharpSymbolChanges) : SymbolId list =
        changes.Deleted |> List.filter isPropertySymbol

    let eventAccessorsAdded (changes: FSharpSymbolChanges) : SymbolId list =
        changes.Added |> List.filter isEventSymbol

    let eventAccessorsUpdated (changes: FSharpSymbolChanges) : UpdatedSymbolChange list =
        changes.Updated |> List.filter (fun change -> isEventSymbol change.Symbol)

    let eventAccessorsDeleted (changes: FSharpSymbolChanges) : SymbolId list =
        changes.Deleted |> List.filter isEventSymbol
