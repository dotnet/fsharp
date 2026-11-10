namespace FSharp.Compiler.LanguageServer.Handlers

open System
open System.Diagnostics
open System.Runtime.ExceptionServices
open System.Threading

open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open Microsoft.VisualStudio.LanguageServer.Protocol

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.LanguageServer
open FSharp.Compiler.LanguageServer.Common

#nowarn "57"

type SemanticTokensHandler() =
    interface IMethodHandler with
        member _.MutatesSolutionState = false

    member private _.GetParsingOptions
        (uri: System.Uri, context: FSharpRequestContext, projectContext: ProjectSnapshot.FSharpProjectIdentifier option)
        =
        context.Workspace.Query.GetProjectSnapshotForFile(uri, ?context = projectContext)
        |> Option.map (fun snapshot ->
            let checker = context.Workspace.Checker

            let opts, _ =
                // Should these be cached somewhere?
                checker.GetParsingOptionsFromCommandLineArgs(
                    [ uri.LocalPath ],
                    snapshot.ProjectConfig.CommandLineOptions,
                    isEditing = true
                )

            opts)

    member private this.GetSemanticTokens
        (textDocument: TextDocumentIdentifier, context: FSharpRequestContext, range: Range option, cancellationToken: CancellationToken)
        =
        cancellableTask {
            let stopwatch = Stopwatch.StartNew()
            let uri = textDocument.Uri
            let explicitContext = tryGetProjectContext textDocument
            let activeContext = context.TryGetActiveProjectContext uri
            let projectContext = explicitContext |> Option.orElse activeContext
            let telemetry = context.LspServices.GetRequiredService<ILspTelemetry>()
            let handlerStopwatch = Stopwatch.StartNew()
            let mutable outcome = "faulted"
            let mutable semanticData = false
            let mutable documentLength = -1
            let mutable spanLength = -1
            let mutable tokenCount = 0

            try
                try
                    let! source = context.Workspace.Query.GetSource uri

                    match source with
                    | None ->
                        outcome <- "no_source"
                        return LspSemanticTokens.emptyResponse ()
                    | Some source ->
                        documentLength <- source.Length

                        spanLength <-
                            match range with
                            | None -> source.Length
                            | Some r when r.Start.Line = r.End.Line -> max 0 (r.End.Character - r.Start.Character)
                            | Some _ -> -1

                        let! semanticClassification = context.Workspace.Query.GetSemanticClassification(uri, ?context = projectContext)
                        semanticData <- semanticClassification.IsSome

                        let semanticItems =
                            match semanticClassification with
                            | None -> [||]
                            | Some view ->
                                let items = ResizeArray<SemanticClassificationItem>()
                                view.ForEach items.Add
                                items.ToArray()

                        let parsingOptions = this.GetParsingOptions(uri, context, projectContext)

                        let response =
                            LspSemanticTokens.responseFor source uri.LocalPath semanticItems range parsingOptions

                        tokenCount <- response.Data.Length / 5
                        outcome <- "completed"
                        return response
                with :? OperationCanceledException as error ->
                    outcome <- "cancelled"
                    ExceptionDispatchInfo.Capture(error).Throw()
                    return Unchecked.defaultof<_>
            finally
                Telemetry.reportEvent
                    telemetry
                    TelemetryEvents.GetSemanticTokens
                    [|
                        "uri_hash", hash uri :> obj
                        "request_kind", (if range.IsSome then "range" else "full") :> obj
                        "context_source",
                        (if explicitContext.IsSome then "explicit"
                         elif activeContext.IsSome then "active"
                         else "default")
                        :> obj
                        "isOpenDocument", context.IsDocumentOpen uri :> obj
                        "outcome", outcome :> obj
                        "semantic_data_available", semanticData :> obj
                        "token_count", tokenCount :> obj
                        "provider_duration_ms", stopwatch.ElapsedMilliseconds :> obj
                        "duration_ms", handlerStopwatch.ElapsedMilliseconds :> obj
                        if documentLength >= 0 then
                            "document_length", documentLength :> obj
                        if spanLength >= 0 then
                            "textSpanLength", spanLength :> obj
                        match range with
                        | Some r ->
                            "range_start_line", r.Start.Line :> obj
                            "range_start_character", r.Start.Character :> obj
                            "range_end_line", r.End.Line :> obj
                            "range_end_character", r.End.Character :> obj
                        | None -> ()
                    |]
        }
        |> CancellableTask.start cancellationToken

    interface IRequestHandler<SemanticTokensParams, SemanticTokens, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentSemanticTokensFullName, LanguageServerConstants.DefaultLanguageName)>]
        member this.HandleRequestAsync(request: SemanticTokensParams, context: FSharpRequestContext, cancellationToken: CancellationToken) =
            this.GetSemanticTokens(request.TextDocument, context, None, cancellationToken)

    interface IRequestHandler<SemanticTokensRangeParams, SemanticTokens, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentSemanticTokensRangeName, LanguageServerConstants.DefaultLanguageName)>]
        member this.HandleRequestAsync
            (request: SemanticTokensRangeParams, context: FSharpRequestContext, cancellationToken: CancellationToken)
            =
            this.GetSemanticTokens(request.TextDocument, context, Some request.Range, cancellationToken)
