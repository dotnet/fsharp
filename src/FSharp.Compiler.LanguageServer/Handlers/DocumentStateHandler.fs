namespace FSharp.Compiler.LanguageServer.Handlers

open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.LanguageServer.Protocol
open FSharp.Compiler.LanguageServer
open FSharp.Compiler.LanguageServer.Common

open System.Threading
open System.Threading.Tasks

#nowarn "57"

type DocumentStateHandler() =
    interface IMethodHandler with
        member _.MutatesSolutionState = true

    interface IRequestHandler<DidOpenTextDocumentParams, SemanticTokensDeltaPartialResult, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentDidOpenName, LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync
            (request: DidOpenTextDocumentParams, context: FSharpRequestContext, _cancellationToken: CancellationToken)
            =
            let telemetry = context.LspServices.GetRequiredService<ILspTelemetry>()
            let contextHolder = context.LspServices.GetRequiredService<ContextHolder>()

            contextHolder.UpdateWorkspace _.Files.Open(request.TextDocument.Uri, request.TextDocument.Text)

            telemetry.ReportEvent(
                TelemetryEvents.DocumentOpened,
                [| "uri_hash", hash request.TextDocument.Uri :> obj |]
            )

            Task.FromResult(SemanticTokensDeltaPartialResult())

    interface IRequestHandler<DidChangeTextDocumentParams, SemanticTokensDeltaPartialResult, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentDidChangeName, LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync
            (request: DidChangeTextDocumentParams, context: FSharpRequestContext, _cancellationToken: CancellationToken)
            =
            let contextHolder = context.LspServices.GetRequiredService<ContextHolder>()

            contextHolder.UpdateWorkspace _.Files.Edit(request.TextDocument.Uri, request.ContentChanges.[0].Text)

            Task.FromResult(SemanticTokensDeltaPartialResult())

    interface INotificationHandler<DidCloseTextDocumentParams, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentDidCloseName, LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleNotificationAsync
            (request: DidCloseTextDocumentParams, context: FSharpRequestContext, _cancellationToken: CancellationToken)
            =
            let telemetry = context.LspServices.GetRequiredService<ILspTelemetry>()
            let contextHolder = context.LspServices.GetRequiredService<ContextHolder>()

            contextHolder.UpdateWorkspace _.Files.Close(request.TextDocument.Uri)

            telemetry.ReportEvent(
                TelemetryEvents.DocumentClosed,
                [| "uri_hash", hash request.TextDocument.Uri :> obj |]
            )

            Task.CompletedTask
