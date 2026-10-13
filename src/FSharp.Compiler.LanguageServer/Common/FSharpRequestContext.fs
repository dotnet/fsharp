namespace FSharp.Compiler.LanguageServer.Common

open Microsoft.CommonLanguageServerProtocol.Framework
open System
open System.Collections.Concurrent
open System.Threading
open System.Threading.Tasks

open FSharp.Compiler.CodeAnalysis.Workspace
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot

#nowarn "57"

type FSharpRequestContext(lspServices: ILspServices, logger: ILspLogger, workspace: FSharpWorkspace) =
    let pathComparer =
        if OperatingSystem.IsWindows() then
            StringComparer.OrdinalIgnoreCase
        else
            StringComparer.Ordinal

    let activeProjectContexts =
        ConcurrentDictionary<string, FSharpProjectIdentifier>(pathComparer)

    member _.LspServices = lspServices
    member _.Logger = logger
    member _.Workspace = workspace

    member _.TryGetActiveProjectContext(uri: Uri) =
        match activeProjectContexts.TryGetValue uri.LocalPath with
        | true, identifier -> Some identifier
        | _ -> None

    member _.SetActiveProjectContext(uri: Uri, identifier) =
        activeProjectContexts[uri.LocalPath] <- identifier

    member _.ClearActiveProjectContext(uri: Uri) =
        activeProjectContexts.TryRemove uri.LocalPath |> ignore

type ContextHolder(workspace, lspServices: ILspServices) =

    let logger = lspServices.GetRequiredService<ILspLogger>()

    let context = FSharpRequestContext(lspServices, logger, workspace)

    member _.GetContext() = context

    member _.UpdateWorkspace(f) = f context.Workspace

type FShapRequestContextFactory(lspServices: ILspServices) =

    inherit AbstractRequestContextFactory<FSharpRequestContext>()

    override _.CreateRequestContextAsync<'TRequestParam>
        (
            _queueItem: QueueItem<FSharpRequestContext>,
            _methodHandler: IMethodHandler,
            _requestParam: 'TRequestParam,
            _cancellationToken: CancellationToken
        ) =
        lspServices.GetRequiredService<ContextHolder>()
        |> _.GetContext()
        |> Task.FromResult
