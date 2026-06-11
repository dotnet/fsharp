namespace FSharp.Compiler.LanguageServer.Handlers

open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open Microsoft.VisualStudio.LanguageServer.Protocol

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.LanguageServer
open FSharp.Compiler.LanguageServer.Common

open System.Threading

#nowarn "57"

type SemanticTokensHandler() =
    interface IMethodHandler with
        member _.MutatesSolutionState = false

    member private _.GetParsingOptions(uri: System.Uri, context: FSharpRequestContext, projectContext: ProjectSnapshot.FSharpProjectIdentifier option) =
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
            let uri = textDocument.Uri
            let projectContext = tryGetProjectContext textDocument
            let telemetry = context.LspServices.GetRequiredService<ILspTelemetry>()

            use _scope =
                telemetry.ReportEventWithDuration(TelemetryEvents.GetSemanticTokens, [| "uri_hash", hash uri :> obj |])

            let! source = context.Workspace.Query.GetSource uri

            match source with
            | None -> return LspSemanticTokens.emptyResponse ()
            | Some source ->
                let! semanticClassification = context.Workspace.Query.GetSemanticClassification(uri, ?context = projectContext)

                let semanticItems =
                    match semanticClassification with
                    | None -> [||]
                    | Some view ->
                        let items = ResizeArray<SemanticClassificationItem>()
                        view.ForEach items.Add
                        items.ToArray()

                let parsingOptions = this.GetParsingOptions(uri, context, projectContext)

                return LspSemanticTokens.responseFor source uri.LocalPath semanticItems range parsingOptions
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
