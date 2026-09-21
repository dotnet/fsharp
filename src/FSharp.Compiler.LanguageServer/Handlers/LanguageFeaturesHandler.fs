namespace FSharp.Compiler.LanguageServer.Handlers

open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.LanguageServer.Protocol
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open FSharp.Compiler.LanguageServer.Common
open FSharp.Compiler.LanguageServer
open System.Threading.Tasks
open System.Threading
open Microsoft.VisualStudio.FSharp.Editor

#nowarn "57"

type LanguageFeaturesHandler() =
    interface IMethodHandler with
        member _.MutatesSolutionState = false

    interface IRequestHandler<
        DocumentDiagnosticParams,
        SumType<FullDocumentDiagnosticReport, UnchangedDocumentDiagnosticReport>,
        FSharpRequestContext
     > with
        [<LanguageServerEndpoint(Methods.TextDocumentDiagnosticName, LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync
            (request: DocumentDiagnosticParams, context: FSharpRequestContext, cancellationToken: CancellationToken)
            =
            cancellableTask {

                let! fsharpDiagnosticReport = context.Workspace.Query.GetDiagnosticsForFile request.TextDocument.Uri

                if request.PreviousResultId = fsharpDiagnosticReport.ResultId then
                    return
                        SumType<FullDocumentDiagnosticReport, UnchangedDocumentDiagnosticReport>(
                            UnchangedDocumentDiagnosticReport(ResultId = fsharpDiagnosticReport.ResultId)
                        )
                else
                    return
                        SumType<FullDocumentDiagnosticReport, UnchangedDocumentDiagnosticReport>(
                            FullDocumentDiagnosticReport(
                                Items = (fsharpDiagnosticReport.Diagnostics |> Array.map (_.ToLspDiagnostic())),
                                ResultId = fsharpDiagnosticReport.ResultId
                            )
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
