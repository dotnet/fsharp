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
        [<LanguageServerEndpoint("textDocument/definition", LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync(request: TextDocumentPositionParams, context: FSharpRequestContext, cancellationToken: CancellationToken) =
            cancellableTask {
                try
                    let uri = request.TextDocument.Uri
                    let position = request.Position
                    
                    // Get the parse and check results for the file
                    let! parseAndCheckResults = context.Workspace.Query.GetParseAndCheckResultsForFile(uri) |> Async.StartAsTask
                    
                    match parseAndCheckResults with
                    | _, Some checkResults ->
                        // Convert LSP position to F# position
                        let line = position.Line + 1 // LSP is 0-based, F# is 1-based
                        let column = position.Character
                        
                        // Get the source text to extract the line
                        let! sourceOpt = context.Workspace.Query.GetSource(uri) |> Async.StartAsTask
                        
                        match sourceOpt with
                        | Some source ->
                            let lines = source.GetLines()
                            if line <= lines.Length then
                                let lineText = lines.[line - 1].ToString()
                                
                                // Better approach: try to extract symbol at exact position
                                // Find the word boundaries around the position
                                let rec findWordStart pos =
                                    if pos <= 0 || not (System.Char.IsLetterOrDigit(lineText.[pos - 1])) then pos
                                    else findWordStart (pos - 1)
                                
                                let rec findWordEnd pos =
                                    if pos >= lineText.Length || not (System.Char.IsLetterOrDigit(lineText.[pos])) then pos
                                    else findWordEnd (pos + 1)
                                
                                let adjustedColumn = min column lineText.Length
                                let wordStart = findWordStart adjustedColumn
                                let wordEnd = findWordEnd adjustedColumn
                                let symbolAtPos = if wordEnd > wordStart then lineText.Substring(wordStart, wordEnd - wordStart) else ""
                                
                                // Create the names list for the symbol
                                let symbolNames = if System.String.IsNullOrEmpty(symbolAtPos) then [] else [symbolAtPos]
                                
                                // Use GetSymbolUseAtLocation to get more accurate information
                                let symbolUseOpt = 
                                    checkResults.GetSymbolUseAtLocation(
                                        line,
                                        adjustedColumn,
                                        lineText,
                                        symbolNames
                                    )
                                
                                match symbolUseOpt with
                                | Some symbolUse ->
                                    // If we have a symbol use, we can also get the declaration location
                                    match symbolUse.Symbol.DeclarationLocation with
                                    | Some range ->
                                        // Convert F# range to LSP Location
                                        let location = Location(
                                            Uri = System.Uri(range.FileName),
                                            Range = range.ToLspRange()
                                        )
                                        
                                        return [| location |]
                                    | None ->
                                        // Fallback to GetDeclarationLocation
                                        let declarations = 
                                            checkResults.GetDeclarationLocation(
                                                line,
                                                adjustedColumn,
                                                lineText,
                                                symbolNames,
                                                false // preferSignature
                                            )
                                        
                                        match declarations with
                                        | FindDeclResult.DeclFound range ->
                                            let location = Location(
                                                Uri = System.Uri(range.FileName),
                                                Range = range.ToLspRange()
                                            )
                                            
                                            return [| location |]
                                        | _ ->
                                            return [||]
                                | None ->
                                    // Fallback to the original approach
                                    let declarations = 
                                        checkResults.GetDeclarationLocation(
                                            line,
                                            adjustedColumn,
                                            lineText,
                                            symbolNames,
                                            false // preferSignature
                                        )
                                    
                                    match declarations with
                                    | FindDeclResult.DeclFound range ->
                                        let location = Location(
                                            Uri = System.Uri(range.FileName),
                                            Range = range.ToLspRange()
                                        )
                                        
                                        return [| location |]
                                    | _ ->
                                        return [||]
                            else
                                // Line out of bounds
                                return [||]
                        | None ->
                            // No source available
                            return [||]
                    | _ ->
                        // No check results available
                        return [||]
                        
                with ex ->
                    // Log error and return empty result
                    context.Logger.LogError("Error in textDocument/definition: {0}", [| ex.Message |])
                    return [||]
            }
            |> CancellableTask.start cancellationToken
