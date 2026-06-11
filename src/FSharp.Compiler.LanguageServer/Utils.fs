namespace FSharp.Compiler.LanguageServer

open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.LanguageServer.Protocol

open System
open System.Diagnostics
open System.Security.Cryptography
open System.Text

open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open FSharp.Compiler.Diagnostics
open System.Runtime.CompilerServices

#nowarn "57"

[<AutoOpen>]
module Utils =

    type LspRange = Microsoft.VisualStudio.LanguageServer.Protocol.Range

    /// Encodes a stable, round-trippable identifier for a project context. The encoding includes the
    /// output file name so that different target frameworks of a multi-targeting project produce
    /// distinct ids (their project file name is the same but their output file name differs).
    /// Format: "{guid}|{outputFileName}|{projectFileName}". The GUID is derived from the output file
    /// name so that the same TFM of the same project always maps to the same context id.
    let makeProjectContextId (identifier: FSharpProjectIdentifier) =
        let (FSharpProjectIdentifier(projectFileName, outputFileName)) = identifier
        let guid = Guid(MD5.HashData(Encoding.UTF8.GetBytes(outputFileName)))
        $"{guid}|{outputFileName}|{projectFileName}"

    /// Recovers an FSharpProjectIdentifier from a context id produced by makeProjectContextId.
    /// Returns None for malformed ids.
    let tryParseProjectContextId (contextId: string) : FSharpProjectIdentifier option =
        if String.IsNullOrEmpty contextId then
            None
        else
            let parts = contextId.Split('|', 3)
            if parts.Length = 3 then
                Some(FSharpProjectIdentifier(parts[2], parts[1]))
            else
                None

    /// Extracts the FSharpProjectIdentifier from the VS project context attached to a text document
    /// identifier, if any. Returns None when the client didn't send a context (e.g. non-VS clients
    /// or when navbar isn't available) or when the id is malformed.
    let tryGetProjectContext (textDocument: TextDocumentIdentifier) : FSharpProjectIdentifier option =
        match textDocument with
        | :? VSTextDocumentIdentifier as vsDoc ->
            match vsDoc.ProjectContext with
            | null -> None
            | ctx -> tryParseProjectContextId ctx.Id
        | _ -> None

    let snapshotsToProjectInfos (snapshots: FSharpProjectSnapshot array) =
        snapshots
        |> Array.map (fun s ->
            VSDiagnosticProjectInformation(
                ProjectName = IO.Path.GetFileNameWithoutExtension(s.ProjectFileName),
                ProjectIdentifier = makeProjectContextId s.Identifier
            ))

    let LspLogger (output: string -> unit) =
        { new ILspLogger with
            member this.LogEndContext(message: string, ``params``: obj array) : unit =
                output $"EndContext :: {message} %A{``params``}"

            member this.LogError(message: string, ``params``: obj array) : unit =
                output $"ERROR :: {message} %A{``params``}"

            member this.LogException(``exception``: exn, message: string, ``params``: obj array) : unit =
                output $"EXCEPTION :: %A{``exception``} {message} %A{``params``}"

            member this.LogInformation(message: string, ``params``: obj array) : unit =
                output $"INFO :: {message} %A{``params``}"

            member this.LogStartContext(message: string, ``params``: obj array) : unit =
                output $"StartContext :: {message} %A{``params``}"

            member this.LogWarning(message: string, ``params``: obj array) : unit =
                output $"WARNING :: {message} %A{``params``}"
        }

    type FSharp.Compiler.Text.Range with

        member this.ToLspRange() =
            LspRange(
                Start = Position(Line = this.StartLine - 1, Character = this.StartColumn),
                End = Position(Line = this.EndLine - 1, Character = this.EndColumn)
            )

[<Extension>]
type FSharpDiagnosticExtensions =

    [<Extension>]
    static member ToVsDiagnostic(this: FSharpDiagnostic, projects: VSDiagnosticProjectInformation[]) =
        let severity =
            match this.Severity with
            | FSharpDiagnosticSeverity.Error -> DiagnosticSeverity.Error
            | FSharpDiagnosticSeverity.Warning -> DiagnosticSeverity.Warning
            | FSharpDiagnosticSeverity.Info -> DiagnosticSeverity.Information
            | FSharpDiagnosticSeverity.Hidden -> DiagnosticSeverity.Hint
        VSDiagnostic(
            Range = this.Range.ToLspRange(),
            Severity = severity,
            Message = $"LSP: {this.Message}",
            Code = SumType<int, _> this.ErrorNumberText,
            Projects = projects,
            Identifier = string (hash (this.ErrorNumberText, this.Range, this.Message))
        )

module Activity =
    let listen (filter) logMsg =
        let indent (activity: Activity) =
            let rec loop (activity: Activity) n =
                match activity.Parent with
                | Null -> n
                | NonNull parent -> loop (parent) (n + 1)

            String.replicate (loop activity 0) "    "

        let collectTags (activity: Activity) =
            [ for tag in activity.Tags -> $"{tag.Key}: %A{tag.Value}" ]
            |> String.concat ", "

        let listener =
            new ActivityListener(
                ShouldListenTo = (fun source -> source.Name = FSharp.Compiler.Diagnostics.ActivityNames.FscSourceName),
                Sample =
                    (fun context ->
                        if filter context.Name then
                            ActivitySamplingResult.AllDataAndRecorded
                        else
                            ActivitySamplingResult.None),
                ActivityStarted = (fun a -> logMsg $"{indent a}{a.OperationName}     {collectTags a}")
            )

        ActivitySource.AddActivityListener(listener)

    let listenToAll () =
        listen (fun _ -> true) Trace.TraceInformation

    let listenToSome () =
        listen (fun x -> not <| x.Contains "StackGuard") Trace.TraceInformation
