namespace FSharp.Compiler.LanguageServer.Handlers

open System
open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.LanguageServer.Protocol
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open FSharp.Compiler.LanguageServer.Common
open FSharp.Compiler.LanguageServer
open System.Threading.Tasks
open System.Threading
open System.Collections.Generic
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.EditorServices

#nowarn "57"

type LanguageFeaturesHandler() =
    interface IMethodHandler with
        member _.MutatesSolutionState = false

    interface IRequestHandler<
        DocumentDiagnosticParams,
        SumType<RelatedFullDocumentDiagnosticReport, RelatedUnchangedDocumentDiagnosticReport>,
        FSharpRequestContext
     > with
        [<LanguageServerEndpoint(Methods.TextDocumentDiagnosticName, LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync
            (request: DocumentDiagnosticParams, context: FSharpRequestContext, cancellationToken: CancellationToken)
            =
            cancellableTask {

                let! fsharpDiagnosticReport = context.Workspace.Query.GetDiagnosticsForFile request.TextDocument.Uri

                let report =
                    FullDocumentDiagnosticReport(
                        Items = (fsharpDiagnosticReport.Diagnostics |> Array.map (_.ToLspDiagnostic())),
                        ResultId = fsharpDiagnosticReport.ResultId
                    )

                let relatedDocuments = Dictionary()

                relatedDocuments.Add(
                    request.TextDocument.Uri,
                    SumType<FullDocumentDiagnosticReport, UnchangedDocumentDiagnosticReport> report
                )

                return
                    SumType<RelatedFullDocumentDiagnosticReport, RelatedUnchangedDocumentDiagnosticReport>(
                        RelatedFullDocumentDiagnosticReport(RelatedDocuments = relatedDocuments)
                    )
            }
            |> CancellableTask.start cancellationToken

    interface IRequestHandler<SemanticTokensParams, SemanticTokens, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentSemanticTokensFullName, LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync(request: SemanticTokensParams, context: FSharpRequestContext, cancellationToken: CancellationToken) =
            cancellableTask {
                let! tokens = context.GetSemanticTokensForFile(request.TextDocument.Uri)
                return SemanticTokens(Data = tokens)
            }
            |> CancellableTask.start cancellationToken

    interface IRequestHandler<TextDocumentPositionParams, SumType<Location[], Location>, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentDefinitionName, LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync
            (request: TextDocumentPositionParams, context: FSharpRequestContext, cancellationToken: CancellationToken)
            =
            cancellableTask {
                let file = request.TextDocument.Uri
                let line = int request.Position.Line
                let col = int request.Position.Character

                let! _, checkResults = context.Workspace.Query.GetParseAndCheckResultsForFile file

                match checkResults with
                | Some checkResults ->
                    let! sourceText = context.Workspace.Query.GetSource file

                    match sourceText with
                    | Some sourceText ->
                        try
                            // Convert LSP position to F# compiler position
                            let lineText = sourceText.GetLineString(line)
                            let fcsLine = FSharp.Compiler.Text.Line.fromZ line

                            // Try to find the identifier at the position using QuickParse
                            let qualifyingIdents, partialIdent = QuickParse.GetPartialLongName(lineText, col)
                            let fullIdent = qualifyingIdents @ [ partialIdent ]

                            if not (List.isEmpty fullIdent) && not (String.IsNullOrEmpty partialIdent) then
                                // Get declaration location using F# Compiler Service
                                let declarationResult =
                                    checkResults.GetDeclarationLocation(fcsLine, col, lineText, fullIdent, false)

                                match declarationResult with
                                | FindDeclResult.DeclFound targetRange ->
                                    let location =
                                        Microsoft.VisualStudio.LanguageServer.Protocol.Location(
                                            Uri = System.Uri(targetRange.FileName),
                                            Range =
                                                Microsoft.VisualStudio.LanguageServer.Protocol.Range(
                                                    Start =
                                                        Microsoft.VisualStudio.LanguageServer.Protocol.Position(
                                                            Line = targetRange.StartLine - 1,
                                                            Character = targetRange.StartColumn
                                                        ),
                                                    End =
                                                        Microsoft.VisualStudio.LanguageServer.Protocol.Position(
                                                            Line = targetRange.EndLine - 1,
                                                            Character = targetRange.EndColumn
                                                        )
                                                )
                                        )

                                    return SumType<Location[], Location>([| location |])
                                | _ -> return SumType<Location[], Location>([||])
                            else
                                return SumType<Location[], Location>([||])
                        with _ ->
                            return SumType<Location[], Location>([||])
                    | None -> return SumType<Location[], Location>([||])
                | _ -> return SumType<Location[], Location>([||])
            }
            |> CancellableTask.start cancellationToken
