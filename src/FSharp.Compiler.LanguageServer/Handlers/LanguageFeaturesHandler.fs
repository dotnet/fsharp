namespace FSharp.Compiler.LanguageServer.Handlers

open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.LanguageServer.Protocol
open FSharp.Compiler.LanguageServer.Common
open System.Threading
open System.Threading.Tasks
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.CodeAnalysis

#nowarn "57"

/// Handler for F# language features in LSP server
type LanguageFeaturesHandler() =
    interface IMethodHandler with
        member _.MutatesSolutionState = false

    /// Handles textDocument/definition requests to provide "go to definition" functionality
    /// Following LSP 3.17 specification: https://microsoft.github.io/language-server-protocol/specifications/lsp/3.17/specification/#textDocument_definition
    interface IRequestHandler<TextDocumentPositionParams, SumType<Location[], LocationLink[]>, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentDefinitionName, LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync
            (request: TextDocumentPositionParams, context: FSharpRequestContext, cancellationToken: CancellationToken)
            =
            task {
                try
                    context.Logger.LogInformation("textDocument/definition request received for {uri} at {line}:{character}", 
                        request.TextDocument.Uri, request.Position.Line, request.Position.Character)

                    let uri = System.Uri(request.TextDocument.Uri)
                    
                    // Get source text
                    let! sourceOpt = context.Workspace.Query.GetSource uri
                    match sourceOpt with
                    | None -> 
                        context.Logger.LogWarning("Could not get source for {uri}", request.TextDocument.Uri)
                        return SumType<Location[], LocationLink[]>([||])
                    | Some source ->
                        
                    // Convert LSP position (0-based) to F# position (1-based)
                    let line = request.Position.Line + 1
                    let column = request.Position.Character
                    
                    // Get the line text
                    let lines = source.ToString().Split('\n')
                    if line > lines.Length then
                        context.Logger.LogWarning("Position {line}:{column} is out of bounds for {uri}", line, column, request.TextDocument.Uri)
                        return SumType<Location[], LocationLink[]>([||])
                    
                    let lineText = if line <= lines.Length then lines.[line - 1] else ""
                    
                    // TODO: Consider using FSharpLexer.Tokenize for more accurate identifier extraction
                    // This would be more robust than manual text parsing, similar to how the VS editor implementation works
                    // Extract identifier at position - improved approach with better edge case handling
                    let getIdentifierAtPosition (text: string) (col: int) =
                        if col < 0 || col >= text.Length then []
                        else
                            // If we're on whitespace or punctuation (except dots), return empty
                            let charAtPos = text.[col]
                            if System.Char.IsWhiteSpace(charAtPos) || 
                               (System.Char.IsPunctuation(charAtPos) && charAtPos <> '.' && charAtPos <> '_') then
                                []
                            else
                                let mutable start = col
                                let mutable endPos = col
                                
                                // Move start backward while we have identifier characters or dots
                                while start > 0 && (System.Char.IsLetterOrDigit(text.[start - 1]) || text.[start - 1] = '_' || text.[start - 1] = '.') do
                                    start <- start - 1
                                    
                                // Move end forward while we have identifier characters or dots
                                while endPos < text.Length && (System.Char.IsLetterOrDigit(text.[endPos]) || text.[endPos] = '_' || text.[endPos] = '.') do
                                    endPos <- endPos + 1
                                    
                                if start = endPos then []
                                else 
                                    let identifier = text.Substring(start, endPos - start)
                                    // Handle dotted identifiers like Module.Function
                                    // Remove empty parts and leading/trailing dots
                                    identifier.Split('.') 
                                    |> Array.toList 
                                    |> List.filter (fun s -> not (System.String.IsNullOrWhiteSpace(s)))
                    
                    let identifiers = getIdentifierAtPosition lineText column
                    
                    if identifiers.IsEmpty then
                        context.Logger.LogInformation("No identifier found at position {line}:{column} in {uri}", line, column, request.TextDocument.Uri)
                        return SumType<Location[], LocationLink[]>([||])
                    
                    context.Logger.LogInformation("Found identifiers: {identifiers} at {line}:{column}", identifiers, line, column)
                    
                    // Get parse and check results
                    let! parseAndCheckResults = context.Workspace.Query.GetParseAndCheckResultsForFile uri
                    let _, checkResults = parseAndCheckResults
                    
                    // Use F# compiler service to get declaration location
                    // Note: F# uses 1-based line numbers, LSP uses 0-based
                    let fcsLine = line // We already converted to 1-based above
                    let colAtEndOfNames = column + (if identifiers.IsEmpty then 0 else identifiers |> List.last |> String.length)
                    let declarations = checkResults.GetDeclarationLocation(fcsLine, colAtEndOfNames, lineText, identifiers, false)
                    
                    match declarations with
                    | FindDeclResult.DeclFound range ->
                        // Convert F# range to LSP location with proper URI conversion
                        let fileUri = 
                            if System.IO.Path.IsPathRooted(range.FileName) then
                                // Convert absolute path to file URI
                                System.Uri(range.FileName).ToString()
                            else
                                // Handle relative paths
                                let fullPath = System.IO.Path.GetFullPath(range.FileName)
                                System.Uri(fullPath).ToString()
                        
                        let location = Location(
                            Uri = fileUri,
                            Range = Range(
                                Start = Position(Line = range.StartLine - 1, Character = range.StartColumn),
                                End = Position(Line = range.EndLine - 1, Character = range.EndColumn)
                            )
                        )
                        context.Logger.LogInformation("Found definition at {fileName}:{startLine}:{startColumn}", 
                            range.FileName, range.StartLine, range.StartColumn)
                        return SumType<Location[], LocationLink[]>([| location |])
                    
                    | FindDeclResult.DeclNotFound reason ->
                        context.Logger.LogInformation("Declaration not found: {reason}", reason)
                        return SumType<Location[], LocationLink[]>([||])
                    
                    | FindDeclResult.ExternalDecl(assembly, externalSym) ->
                        context.Logger.LogInformation("Found external declaration in {assembly}: {symbol}", assembly, externalSym)
                        // For external declarations, we don't have a source location to return
                        return SumType<Location[], LocationLink[]>([||])
                        
                with ex ->
                    context.Logger.LogError(ex, "Error in textDocument/definition: {message}", ex.Message)
                    return SumType<Location[], LocationLink[]>([||])
            }