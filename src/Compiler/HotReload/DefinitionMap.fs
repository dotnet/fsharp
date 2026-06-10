module internal FSharp.Compiler.HotReload.DefinitionMap

open FSharp.Compiler.TypedTreeDiff

[<RequireQualifiedAccess>]
/// Classifies how a symbol changed between the baseline and the updated compilation.
type SymbolEditKind =
    | Added
    | Updated of SemanticEditKind
    | Deleted

[<RequireQualifiedAccess>]
/// Captures the change metadata for a single symbol, including hashes for change detection.
type SymbolChange =
    { Symbol: SymbolId
      EditKind: SymbolEditKind
      BaselineHash: int option
      UpdatedHash: int option
      IsSynthesized: bool
      ContainingEntity: string option }

[<RequireQualifiedAccess>]
/// Aggregates semantic edits and rude edits for the current compilation unit.
type FSharpDefinitionMap =
    { Changes: SymbolChange list
      RudeEdits: RudeEdit list }

module FSharpDefinitionMap =
    /// Convert a typed-tree diff result into a definition map suitable for downstream delta emission.
    let ofTypedTreeDiff (diff: TypedTreeDiffResult) : FSharpDefinitionMap =
        let changes: SymbolChange list =
            diff.SemanticEdits
            |> List.map (fun edit ->
                let editKind =
                    match edit.Kind with
                    | SemanticEditKind.Insert -> SymbolEditKind.Added
                    | SemanticEditKind.MethodBody
                    | SemanticEditKind.TypeDefinition -> SymbolEditKind.Updated edit.Kind
                    | SemanticEditKind.Delete -> SymbolEditKind.Deleted

                { Symbol = edit.Symbol
                  EditKind = editKind
                  BaselineHash = edit.BaselineHash
                  UpdatedHash = edit.UpdatedHash
                  IsSynthesized = edit.IsSynthesized
                  ContainingEntity = edit.ContainingEntity })

        { Changes = changes; RudeEdits = diff.RudeEdits }

    /// Retrieves all symbols newly added in the updated compilation.
    let added (map: FSharpDefinitionMap) : SymbolId list =
        map.Changes
        |> Seq.choose (fun (change: SymbolChange) ->
            match change.EditKind with
            | SymbolEditKind.Added -> Some change.Symbol
            | _ -> None)
        |> Seq.toList

    /// Retrieves all updated symbols along with the semantic edit classification.
    let updated (map: FSharpDefinitionMap) : (SymbolChange * SemanticEditKind) list =
        map.Changes
        |> Seq.choose (fun (change: SymbolChange) ->
            match change.EditKind with
            | SymbolEditKind.Updated kind -> Some(change, kind)
            | _ -> None)
        |> Seq.toList

    /// Retrieves all symbols deleted from the updated compilation.
    let deleted (map: FSharpDefinitionMap) : SymbolId list =
        map.Changes
        |> Seq.choose (fun (change: SymbolChange) ->
            match change.EditKind with
            | SymbolEditKind.Deleted -> Some change.Symbol
            | _ -> None)
        |> Seq.toList

    /// Retrieves changes that correspond to compiler-synthesized members.
    let synthesized (map: FSharpDefinitionMap) : SymbolChange list =
        map.Changes |> List.filter (fun change -> change.IsSynthesized)

    /// Retrieves synthesized symbols classified as added.
    let synthesizedAdded (map: FSharpDefinitionMap) : SymbolId list =
        synthesized map
        |> List.choose (fun change ->
            match change.EditKind with
            | SymbolEditKind.Added -> Some change.Symbol
            | _ -> None)

    /// Retrieves synthesized symbols classified as updated.
    let synthesizedUpdated (map: FSharpDefinitionMap) : (SymbolChange * SemanticEditKind) list =
        synthesized map
        |> List.choose (fun change ->
            match change.EditKind with
            | SymbolEditKind.Updated kind -> Some(change, kind)
            | _ -> None)

    /// Retrieves synthesized symbols classified as deleted.
    let synthesizedDeleted (map: FSharpDefinitionMap) : SymbolId list =
        synthesized map
        |> List.choose (fun change ->
            match change.EditKind with
            | SymbolEditKind.Deleted -> Some change.Symbol
            | _ -> None)
