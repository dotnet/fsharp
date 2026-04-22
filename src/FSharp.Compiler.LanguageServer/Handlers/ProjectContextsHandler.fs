namespace FSharp.Compiler.LanguageServer.Handlers

open System.IO

open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.LanguageServer.Protocol
open FSharp.Compiler.LanguageServer
open FSharp.Compiler.LanguageServer.Common

open System.Threading
open System.Threading.Tasks

#nowarn "57"
#nowarn "3261"

type ProjectContextsHandler() =

    interface IMethodHandler with
        member _.MutatesSolutionState = false

    interface IRequestHandler<VSGetProjectContextsParams, VSProjectContextList, FSharpRequestContext> with
        [<LanguageServerEndpoint("textDocument/_vs_getProjectContexts", LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync
            (request: VSGetProjectContextsParams, context: FSharpRequestContext, _cancellationToken: CancellationToken)
            =
            let telemetry = context.LspServices.GetRequiredService<ILspTelemetry>()
            let empty = VSProjectContextList(ProjectContexts = [||], DefaultIndex = 0)

            match request.TextDocument with
            | null -> Task.FromResult(empty)
            | textDoc when isNull textDoc.Uri -> Task.FromResult(empty)
            | textDoc ->
                let snapshots = context.Workspace.Query.GetProjectSnapshotsForFile(textDoc.Uri)

                let projectContexts =
                    snapshots
                    |> Array.map (fun snapshot ->
                        VSProjectContext(
                            Label = Path.GetFileNameWithoutExtension(snapshot.ProjectFileName),
                            Id = makeProjectContextId(snapshot.ProjectFileName, snapshot.ProjectId),
                            Kind = VSProjectKind.FSharp
                        ))

                telemetry.ReportEvent(
                    TelemetryEvents.GetProjectContexts,
                    [| "project_count", projectContexts.Length :> obj |]
                )

                Task.FromResult(
                    VSProjectContextList(
                        ProjectContexts = projectContexts,
                        DefaultIndex = 0
                    )
                )
