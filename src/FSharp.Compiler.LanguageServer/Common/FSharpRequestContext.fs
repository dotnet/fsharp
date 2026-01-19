namespace FSharp.Compiler.LanguageServer.Common

open Microsoft.CommonLanguageServerProtocol.Framework
open System.Threading
open System.Threading.Tasks

open System
open System.Collections.Generic
open Microsoft.VisualStudio.LanguageServer.Protocol

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Tokenization

open System.Threading
open FSharp.Compiler.CodeAnalysis.Workspace

#nowarn "57"

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

    inherit AbstractRequestContextFactory<FSharpRequestContext>()

    override _.CreateRequestContextAsync<'TRequestParam>
        (
            _queueItem: IQueueItem<FSharpRequestContext>,
            _methodHandler: IMethodHandler,
            _requestParam: 'TRequestParam,
            _cancellationToken: CancellationToken
        ) =
        lspServices.GetRequiredService<ContextHolder>()
        |> _.GetContext()
        |> Task.FromResult
