namespace FSharp.Compiler.HotReload

open FSharp.Compiler.TypedTreeDiff

/// Represents a user-facing diagnostic generated for a rude edit.
type internal RudeEditDiagnostic =
    { Id: string
      Message: string
      Kind: RudeEditKind
      SymbolName: string option }

module internal RudeEditDiagnostics =

    let private symbolDisplayName (symbol: SymbolId option) =
        symbol |> Option.map (fun s -> s.QualifiedName)

    let private formatMessage (kind: RudeEditKind) (symbolName: string option) fallback =
        let name = symbolName |> Option.defaultValue "the declaration"
        match kind with
        | RudeEditKind.SignatureChange ->
            $"Changing the signature of '{name}' is not supported during hot reload."
        | RudeEditKind.InlineChange ->
            $"Changing inline annotations for '{name}' requires a rebuild."
        | RudeEditKind.TypeLayoutChange ->
            $"Changing the representation of '{name}' requires a rebuild."
        | RudeEditKind.DeclarationAdded ->
            $"Adding a new declaration '{name}' requires a rebuild."
        | RudeEditKind.DeclarationRemoved ->
            $"Removing the declaration '{name}' requires a rebuild."
        | RudeEditKind.LambdaShapeChange ->
            $"Changing lowered lambda shape for '{name}' requires a rebuild."
        | RudeEditKind.StateMachineShapeChange ->
            $"Changing lowered state-machine shape for '{name}' requires a rebuild."
        | RudeEditKind.QueryExpressionShapeChange ->
            $"Changing lowered query-expression shape for '{name}' requires a rebuild."
        | RudeEditKind.SynthesizedDeclarationChange ->
            $"Changing synthesized compiler-generated declarations for '{name}' requires a rebuild."
        | RudeEditKind.InsertVirtual ->
            $"Adding virtual, abstract, or override method '{name}' is not supported."
        | RudeEditKind.InsertConstructor ->
            $"Adding constructor '{name}' is not supported."
        | RudeEditKind.InsertOperator ->
            $"Adding user-defined operator '{name}' is not supported."
        | RudeEditKind.InsertExplicitInterface ->
            $"Adding explicit interface implementation '{name}' is not supported."
        | RudeEditKind.InsertIntoInterface ->
            $"Adding member '{name}' to an interface is not supported."
        | RudeEditKind.FieldAdded ->
            $"Adding field '{name}' is not supported (changes type layout)."
        | RudeEditKind.Unsupported -> fallback

    let private diagnosticId kind =
        match kind with
        | RudeEditKind.SignatureChange -> "FSHRDL001"
        | RudeEditKind.InlineChange -> "FSHRDL002"
        | RudeEditKind.TypeLayoutChange -> "FSHRDL003"
        | RudeEditKind.DeclarationAdded -> "FSHRDL004"
        | RudeEditKind.DeclarationRemoved -> "FSHRDL005"
        | RudeEditKind.LambdaShapeChange -> "FSHRDL012"
        | RudeEditKind.StateMachineShapeChange -> "FSHRDL013"
        | RudeEditKind.QueryExpressionShapeChange -> "FSHRDL014"
        | RudeEditKind.SynthesizedDeclarationChange -> "FSHRDL015"
        | RudeEditKind.InsertVirtual -> "FSHRDL006"
        | RudeEditKind.InsertConstructor -> "FSHRDL007"
        | RudeEditKind.InsertOperator -> "FSHRDL008"
        | RudeEditKind.InsertExplicitInterface -> "FSHRDL009"
        | RudeEditKind.InsertIntoInterface -> "FSHRDL010"
        | RudeEditKind.FieldAdded -> "FSHRDL011"
        | RudeEditKind.Unsupported -> "FSHRDL099"

    let ofRudeEdit (edit: RudeEdit) : RudeEditDiagnostic =
        let symbolName = symbolDisplayName edit.Symbol
        { Id = diagnosticId edit.Kind
          Message = formatMessage edit.Kind symbolName edit.Message
          Kind = edit.Kind
          SymbolName = symbolName }

    let ofRudeEdits edits = edits |> Seq.map ofRudeEdit |> Seq.toList
