namespace FSharp.Compiler.HotReload

open System.Text

open FSharp.Compiler.Diagnostics
open FSharp.Compiler.TypedTreeDiff

/// Preserves diagnostic details across the F# host boundary.
type internal RudeEditDiagnostic =
    {
        Id: string
        Severity: FSharpDiagnosticSeverity
        Message: string
        Kind: RudeEditKind
        SymbolName: string option
    }

module internal RudeEditDiagnostics =

    let private symbolDisplayName (symbol: SymbolId option) = symbol |> Option.map _.QualifiedName

    let private formatMessage (kind: RudeEditKind) (symbolName: string option) fallback =
        let name = symbolName |> Option.defaultValue "the declaration"

        match kind with
        | RudeEditKind.SignatureChange -> $"Changing the signature of '{name}' is not supported during hot reload."
        | RudeEditKind.InlineChange -> $"Changing inline annotations for '{name}' requires a rebuild."
        | RudeEditKind.TypeLayoutChange -> $"Changing the representation of '{name}' requires a rebuild."
        | RudeEditKind.DeclarationAdded ->
            // The diff message carries the precise reason (e.g. which type
            // representations can be added under NewTypeDefinition).
            $"Adding a new declaration '{name}' requires a rebuild. {fallback}"
        | RudeEditKind.DeclarationRemoved -> $"Removing the declaration '{name}' requires a rebuild."
        | RudeEditKind.LambdaShapeChange -> $"Changing lowered lambda shape for '{name}' requires a rebuild."
        | RudeEditKind.StateMachineShapeChange ->
            // The diff message carries the precise resume-point/step-sequence change.
            $"Changing the state-machine shape of '{name}' requires a rebuild. {fallback}"
        | RudeEditKind.QueryExpressionShapeChange -> $"Changing lowered query-expression shape for '{name}' requires a rebuild."
        | RudeEditKind.SynthesizedDeclarationChange ->
            $"Changing synthesized compiler-generated declarations for '{name}' requires a rebuild."
        | RudeEditKind.InsertVirtual -> $"Adding virtual, abstract, or override method '{name}' is not supported."
        | RudeEditKind.InsertConstructor -> $"Adding constructor '{name}' is not supported."
        | RudeEditKind.InsertOperator -> $"Adding user-defined operator '{name}' is not supported."
        | RudeEditKind.InsertExplicitInterface -> $"Adding explicit interface implementation '{name}' is not supported."
        | RudeEditKind.InsertIntoInterface -> $"Adding member '{name}' to an interface is not supported."
        | RudeEditKind.FieldAdded -> $"Adding field '{name}' is not supported (changes type layout)."
        | RudeEditKind.NotSupportedByRuntime ->
            // The diff message already names the symbol and the missing runtime capability.
            fallback
        | RudeEditKind.Unsupported -> fallback

    // FSHRDL identifiers belong to the F# host contract.
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
        | RudeEditKind.NotSupportedByRuntime -> "FSHRDL016"
        | RudeEditKind.Unsupported -> "FSHRDL099"

    let ofRudeEdit (edit: RudeEdit) : RudeEditDiagnostic =
        let symbolName = symbolDisplayName edit.Symbol

        {
            Id = diagnosticId edit.Kind
            Severity = FSharpDiagnosticSeverity.Error
            Message = formatMessage edit.Kind symbolName edit.Message
            Kind = edit.Kind
            SymbolName = symbolName
        }

    let ofRudeEdits edits =
        edits |> Seq.map ofRudeEdit |> Seq.toList

    /// Builds a single structured diagnostic from a kind and an already-formatted message,
    /// for the rude-edit paths that surface an ad-hoc reason (active statements, deleted
    /// symbols, mapping errors, emit-time HotReloadUnsupportedEditException) rather than a
    /// kind-derived message.
    let ofKindMessage (kind: RudeEditKind) (message: string) : RudeEditDiagnostic =
        {
            Id = diagnosticId kind
            Severity = FSharpDiagnosticSeverity.Error
            Message = message
            Kind = kind
            SymbolName = None
        }

    /// A single generic "this edit is unsupported" diagnostic (FSHRDL099) carrying an ad-hoc
    /// message, for reasons that do not map to a specific RudeEditKind.
    let unsupported (message: string) : RudeEditDiagnostic =
        ofKindMessage RudeEditKind.Unsupported message

    /// Renders a structured rude-edit list to the historical one-line-per-edit text
    /// ("{Id}: {Message}"), for logs and callers that still want a flat string.
    let format (diagnostics: RudeEditDiagnostic list) =
        let text = StringBuilder()

        for diagnostic in diagnostics do
            if text.Length > 0 then
                text.AppendLine() |> ignore

            text.Append(diagnostic.Id).Append(": ").Append(diagnostic.Message) |> ignore

        text.ToString()
