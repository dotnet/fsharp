namespace FSharp.Compiler.LanguageServer.Handlers

open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.LanguageServer.Protocol
open Microsoft.VisualStudio.FSharp.Editor.CancellableTasks
open FSharp.Compiler.LanguageServer.Common
open FSharp.Compiler.LanguageServer
open System.Threading.Tasks
open System.Threading

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
                let telemetry = context.LspServices.GetRequiredService<ILspTelemetry>()

                use _scope =
                    telemetry.ReportEventWithDuration(
                        TelemetryEvents.GetDiagnostics,
                        [| "uri_hash", hash request.TextDocument.Uri :> obj |]
                    )

                let! fsharpDiagnosticReport = context.Workspace.Query.GetDiagnosticsForFile request.TextDocument.Uri

                let snapshots = context.Workspace.Query.GetProjectSnapshotsForFile(request.TextDocument.Uri)
                let projects = snapshotsToProjectInfos snapshots

                return
                    SumType<RelatedFullDocumentDiagnosticReport, RelatedUnchangedDocumentDiagnosticReport>(
                        RelatedFullDocumentDiagnosticReport(
                            Items = (fsharpDiagnosticReport.Diagnostics |> Array.map (_.ToVsDiagnostic(projects))),
                            ResultId = fsharpDiagnosticReport.ResultId
                        )
                    )
            }
            |> CancellableTask.start cancellationToken
