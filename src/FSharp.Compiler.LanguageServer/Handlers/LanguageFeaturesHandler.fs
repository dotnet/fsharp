namespace FSharp.Compiler.LanguageServer.Handlers

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
open System

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

    interface IRequestHandler<TextDocumentPositionParams, Location[], FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentDefinitionName, LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync(request: TextDocumentPositionParams, context: FSharpRequestContext, cancellationToken: CancellationToken) =
            cancellableTask {
                let uri = request.TextDocument.Uri
                let position = request.Position
                
                // Convert LSP position to F# line/column (LSP is 0-based, F# is 1-based for lines)
                let fcsLine = position.Line + 1
                let fcsColumn = position.Character
                
                let! parseAndCheckResults = context.Workspace.Query.GetParseAndCheckResultsForFile uri
                
                match parseAndCheckResults with
                | _, Some checkFileResults ->
                    let! sourceOpt = context.Workspace.Query.GetSource uri
                    
                    match sourceOpt with
                    | Some source ->
                        // Get the line text using ISourceText API
                        let lineText = 
                            if position.Line < source.GetLineCount() then 
                                source.GetLineString(position.Line) 
                            else 
                                ""
                        
                        // Use QuickParse to get identifiers at the position
                        let names, _activeName = FSharp.Compiler.EditorServices.QuickParse.GetPartialLongName(lineText, fcsColumn)
                        
                        let declResult = checkFileResults.GetDeclarationLocation(fcsLine, lineText, fcsColumn, names, false)
                        
                        match declResult with
                        | FindDeclResult.DeclFound range ->
                            let location = Location(
                                Uri = System.Uri(range.FileName),
                                Range = range.ToLspRange()
                            )
                            return [| location |]
                        | FindDeclResult.ExternalDecl(assembly, externalSym) ->
                            // For external declarations, we might not be able to navigate to source
                            // Return empty for now - could be enhanced to support metadata as source
                            return [||]
                        | FindDeclResult.DeclNotFound _ ->
                            return [||]
                    | None ->
                        return [||]
                | _ ->
                    return [||]
            }
            |> CancellableTask.start cancellationToken
