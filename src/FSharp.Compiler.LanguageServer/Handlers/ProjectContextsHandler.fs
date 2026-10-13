namespace FSharp.Compiler.LanguageServer.Handlers

open System.IO
open System

open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.VisualStudio.LanguageServer.Protocol
open FSharp.Compiler.LanguageServer
open FSharp.Compiler.LanguageServer.Common
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open StreamJsonRpc

open System.Threading
open System.Threading.Tasks

#nowarn "57"
#nowarn "3261"

[<CLIMutable>]
type ActiveProjectContextParams =
    {
        Uri: Uri
        ProjectFileName: string
        OutputFileName: string
    }

type ActiveProjectContextHandler() =

    interface IMethodHandler with
        member _.MutatesSolutionState = true

    interface IRequestHandler<ActiveProjectContextParams, bool, FSharpRequestContext> with
        [<LanguageServerEndpoint("textDocument/fsharp/setActiveProjectContext", LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync
            (request: ActiveProjectContextParams, context: FSharpRequestContext, cancellationToken: CancellationToken)
            =
            task {
                if
                    isNull request.Uri
                    || not request.Uri.IsAbsoluteUri
                    || not request.Uri.IsFile
                    || String.IsNullOrWhiteSpace request.ProjectFileName
                    || String.IsNullOrWhiteSpace request.OutputFileName
                then
                    context.Logger.LogWarning("Invalid active project context.", [||])
                    return false
                else
                    let comparison =
                        if OperatingSystem.IsWindows() then
                            StringComparison.OrdinalIgnoreCase
                        else
                            StringComparison.Ordinal

                    let normalize projectFileName outputFileName =
                        Path.GetFullPath(outputFileName, Path.GetDirectoryName(Path.GetFullPath(projectFileName)))

                    let outputPath = normalize request.ProjectFileName request.OutputFileName
                    let projectPath = Path.GetFullPath request.ProjectFileName

                    let snapshot =
                        context.Workspace.Query.GetProjectSnapshotsForFile(request.Uri)
                        |> Array.tryFind (fun snapshot ->
                            let compilerOutput = snapshot.ProjectConfig.CommandLineOptions |> findOutputFileName

                            String.Equals(Path.GetFullPath(snapshot.ProjectFileName), projectPath, comparison)
                            && (String.Equals(
                                    normalize snapshot.ProjectFileName snapshot.Identifier.OutputFileName,
                                    outputPath,
                                    comparison
                                )
                                || (compilerOutput
                                    |> Option.exists (fun output ->
                                        String.Equals(normalize snapshot.ProjectFileName output, outputPath, comparison)))))

                    match snapshot with
                    | None ->
                        context.Logger.LogWarning(
                            "Active project context does not match a workspace snapshot.",
                            [| request.ProjectFileName :> obj; request.OutputFileName :> obj |]
                        )

                        return false
                    | Some snapshot ->
                        let changed =
                            context.TryGetActiveProjectContext(request.Uri) <> Some snapshot.Identifier

                        let config = context.LspServices.GetRequiredService<FSharpLanguageServerConfig>()

                        let initialize =
                            context.LspServices.GetRequiredService<IInitializeManager<InitializeParams, InitializeResult>>()

                        let capabilities = initialize.GetInitializeParams().Capabilities

                        let supportsRefresh =
                            not (isNull capabilities)
                            && not (isNull capabilities.Workspace)
                            && not (isNull capabilities.Workspace.SemanticTokens)
                            && capabilities.Workspace.SemanticTokens.RefreshSupport

                        if changed && config.EnabledFeatures.SemanticTokens && supportsRefresh then
                            let server = context.LspServices.GetRequiredService<JsonRpc>()

                            try
                                do! server.InvokeWithParameterObjectAsync("workspace/semanticTokens/refresh", null, cancellationToken)
                                context.SetActiveProjectContext(request.Uri, snapshot.Identifier)
                                return true
                            with error when (error :? RemoteInvocationException) || (error :? ConnectionLostException) ->
                                context.Logger.LogException(
                                    error,
                                    "Unable to refresh semantic tokens after a project context change.",
                                    [||]
                                )

                                return false
                        else
                            context.SetActiveProjectContext(request.Uri, snapshot.Identifier)
                            return true
            }

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
                            Id = makeProjectContextId snapshot.Identifier,
                            Kind = VSProjectKind.FSharp
                        ))

                telemetry.ReportEvent(TelemetryEvents.GetProjectContexts, [| "project_count", projectContexts.Length :> obj |])

                Task.FromResult(VSProjectContextList(ProjectContexts = projectContexts, DefaultIndex = 0))
