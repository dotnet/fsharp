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
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open System
open FSharp.Compiler.Tokenization

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

    interface IRequestHandler<CompletionParams, CompletionList, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentCompletionName, LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync(request: CompletionParams, context: FSharpRequestContext, cancellationToken: CancellationToken) =
            cancellableTask {
                let file = request.TextDocument.Uri
                let position = request.Position

                let! source = context.Workspace.Query.GetSource(file)
                let! parseAndCheckResults = context.Workspace.Query.GetParseAndCheckResultsForFile(file)

                match source, parseAndCheckResults with
                | Some source, (Some parseResults, Some checkFileResults) ->
                    try
                        // Convert LSP position to F# position
                        let fcsPosition = Position.mkPos (int position.Line + 1) (int position.Character)

                        // Get the line text at cursor position
                        let lineText =
                            if position.Line < source.GetLineCount() then
                                source.GetLineString(position.Line).TrimEnd([| '\r'; '\n' |])
                            else
                                ""

                        // Get partial name for completion
                        let partialName =
                            QuickParse.GetPartialLongNameEx(lineText, int position.Character - 1)

                        // Get completion context
                        let completionContext =
                            ParsedInput.TryGetCompletionContext(fcsPosition, parseResults.ParseTree, lineText)

                        // Get declaration list from compiler services
                        let declarations =
                            checkFileResults.GetDeclarationListInfo(
                                Some(parseResults),
                                Line.fromZ position.Line,
                                lineText,
                                partialName,
                                (fun _ -> []), // getAllSymbols - simplified for now
                                (fcsPosition, completionContext),
                                false // genBodyForOverriddenMeth
                            )

                        // Convert F# completion items to LSP completion items
                        let completionItems =
                            declarations.Items
                            |> Array.mapi (fun i item ->
                                let kind =
                                    match item.Kind with
                                    | FSharp.Compiler.EditorServices.CompletionItemKind.Method _ ->
                                        Microsoft.VisualStudio.LanguageServer.Protocol.CompletionItemKind.Method
                                    | FSharp.Compiler.EditorServices.CompletionItemKind.Property ->
                                        Microsoft.VisualStudio.LanguageServer.Protocol.CompletionItemKind.Property
                                    | FSharp.Compiler.EditorServices.CompletionItemKind.Field ->
                                        Microsoft.VisualStudio.LanguageServer.Protocol.CompletionItemKind.Field
                                    | FSharp.Compiler.EditorServices.CompletionItemKind.Event ->
                                        Microsoft.VisualStudio.LanguageServer.Protocol.CompletionItemKind.Event
                                    | FSharp.Compiler.EditorServices.CompletionItemKind.Argument ->
                                        Microsoft.VisualStudio.LanguageServer.Protocol.CompletionItemKind.Variable
                                    | FSharp.Compiler.EditorServices.CompletionItemKind.Other ->
                                        Microsoft.VisualStudio.LanguageServer.Protocol.CompletionItemKind.Value
                                    | _ -> Microsoft.VisualStudio.LanguageServer.Protocol.CompletionItemKind.Value

                                let completionItem = Microsoft.VisualStudio.LanguageServer.Protocol.CompletionItem()
                                completionItem.Label <- item.NameInList
                                completionItem.Kind <- kind
                                completionItem.Detail <- null
                                completionItem.SortText <- sprintf "%06d" i
                                completionItem.FilterText <- item.NameInList
                                completionItem.InsertText <- item.NameInCode
                                completionItem)

                        // Add keyword completions if appropriate
                        let keywordItems =
                            if
                                not declarations.IsForType
                                && not declarations.IsError
                                && List.isEmpty partialName.QualifyingIdents
                            then
                                match completionContext with
                                | None ->
                                    FSharpKeywords.KeywordsWithDescription
                                    |> List.filter (fun (keyword, _) -> not (PrettyNaming.IsOperatorDisplayName keyword))
                                    |> List.mapi (fun i (keyword, description) ->
                                        let completionItem = Microsoft.VisualStudio.LanguageServer.Protocol.CompletionItem()
                                        completionItem.Label <- keyword
                                        completionItem.Kind <- Microsoft.VisualStudio.LanguageServer.Protocol.CompletionItemKind.Keyword
                                        completionItem.Detail <- description
                                        completionItem.SortText <- sprintf "%06d" (1000000 + i) // Sort keywords after regular items
                                        completionItem.FilterText <- keyword
                                        completionItem.InsertText <- keyword
                                        completionItem)
                                    |> List.toArray
                                | _ -> [||]
                            else
                                [||]

                        let allItems = Array.append completionItems keywordItems

                        return CompletionList(IsIncomplete = false, Items = allItems)
                    with ex ->
                        context.Logger.LogError("Error in completion: " + ex.Message)
                        return CompletionList(IsIncomplete = false, Items = [||])
                | _ -> return CompletionList(IsIncomplete = false, Items = [||])
            }
            |> CancellableTask.start cancellationToken
