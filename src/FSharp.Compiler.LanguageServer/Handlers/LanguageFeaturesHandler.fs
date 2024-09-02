namespace FSharp.Compiler.LanguageServer.Handlers

open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.LanguageServer.Protocol
open FSharp.Compiler.LanguageServer.Common
open System.Threading.Tasks
open System.Threading

type LanguageFeaturesHandler() =
    interface IMethodHandler with
        member _.MutatesSolutionState = false

    // TODO: this is not getting called
    interface IRequestHandler<DocumentDiagnosticParams, SumType<RelatedFullDocumentDiagnosticReport, RelatedUnchangedDocumentDiagnosticReport>, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentDiagnosticName)>]
        member _.HandleRequestAsync
            (
                request: DocumentDiagnosticParams,
                context: FSharpRequestContext,
                cancellationToken: CancellationToken
            ) =
            Task.FromResult(new RelatedUnchangedDocumentDiagnosticReport())