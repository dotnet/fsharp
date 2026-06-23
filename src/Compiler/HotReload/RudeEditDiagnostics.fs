namespace FSharp.Compiler.HotReload

open FSharp.Compiler.Diagnostics
open FSharp.Compiler.TypedTreeDiff

/// Represents a user-facing diagnostic generated for a rude edit.
///
/// The structured shape (Id + Severity + Message + SymbolName) is carried through the
/// hot reload error channel rather than being flattened to a single string, so the host
/// (dotnet-watch) can report the reason and distinguish warning-vs-error severity. The
/// Id namespace is owned by `RudeEditDiagnostics.diagnosticId` alone; nothing else in the
/// channel depends on its spelling, so aligning FSHRDL* with Roslyn's ENC* codes later is
/// a change to that one function.
type internal RudeEditDiagnostic =
    {
        Id: string
        Severity: FSharpDiagnosticSeverity
        Message: string
        Kind: RudeEditKind
        SymbolName: string option
    }

module internal RudeEditDiagnostics =

    let private symbolDisplayName (symbol: SymbolId option) =
        symbol |> Option.map (fun s -> s.QualifiedName)

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

    /// Severity of a rude-edit kind. Every kind is blocking (Error) today; this is the single
    /// place to mark a kind as a non-blocking Warning (e.g. a future "might not take effect"
    /// edit that applies but needs a restart to be observed), mirroring Roslyn's ENC severities.
    let private severityOf (_kind: RudeEditKind) = FSharpDiagnosticSeverity.Error

    let ofRudeEdit (edit: RudeEdit) : RudeEditDiagnostic =
        let symbolName = symbolDisplayName edit.Symbol

        {
            Id = diagnosticId edit.Kind
            Severity = severityOf edit.Kind
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
            Severity = severityOf kind
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
        diagnostics
        |> List.map (fun d -> $"{d.Id}: {d.Message}")
        |> String.concat System.Environment.NewLine
