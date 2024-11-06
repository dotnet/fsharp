namespace FSharp.Compiler.LanguageServer.Common

open Microsoft.CommonLanguageServerProtocol.Framework
open System.Threading
open System.Threading.Tasks

type FSharpRequestContext(lspServices: ILspServices, logger: ILspLogger, workspace: FSharpWorkspace) =
    member _.LspServices = lspServices
    member _.Logger = logger
    member _.Workspace = workspace

type ContextHolder(workspace, lspServices: ILspServices) =

    let logger = lspServices.GetRequiredService<ILspLogger>()

    let context = FSharpRequestContext(lspServices, logger, workspace)

    member _.GetContext() = context

    member _.UpdateWorkspace(f) = f context.Workspace

type FShapRequestContextFactory(lspServices: ILspServices) =

    interface IRequestContextFactory<FSharpRequestContext> with

        member _.CreateRequestContextAsync<'TRequestParam>
            (
                queueItem: IQueueItem<FSharpRequestContext>,
                requestParam: 'TRequestParam,
                cancellationToken: CancellationToken
            ) =
            lspServices.GetRequiredService<ContextHolder>()
            |> _.GetContext()
            |> Task.FromResult
