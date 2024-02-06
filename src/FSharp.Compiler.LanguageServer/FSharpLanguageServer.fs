namespace FSharp.Compiler.LanguageServer

open System
open System.Threading.Tasks
open System.Threading
open Microsoft.CommonLanguageServerProtocol.Framework.Handlers
open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.Extensions.DependencyInjection
open Microsoft.VisualStudio.LanguageServer.Protocol

open StreamJsonRpc
open Nerdbank.Streams
open System.Diagnostics
open System.IO
open Workspace

[<AutoOpen>]
module Stuff =
    [<Literal>]
    let FSharpLanguageName = "F#"

type FSharpRequestContext(lspServices: ILspServices, logger: ILspLogger, workspace: FSharpWorkspace) =
    member _.LspServices = lspServices
    member _.Logger = logger
    member _.Workspace = workspace

type ContextHolder(intialWorkspace, lspServices: ILspServices) =

    let logger = lspServices.GetRequiredService<ILspLogger>()

    let mutable context = FSharpRequestContext(lspServices, logger, intialWorkspace)

    member _.GetContext() = context

    member _.UpdateWorkspace(f) =
        context <- FSharpRequestContext(lspServices, logger, f context.Workspace)

type FShapRequestContextFactory(lspServices: ILspServices) =
    inherit AbstractRequestContextFactory<FSharpRequestContext>()

    override _.CreateRequestContextAsync<'TRequestParam>
        (
            queueItem: IQueueItem<FSharpRequestContext>,
            methodHandler: IMethodHandler,
            requestParam: 'TRequestParam,
            cancellationToken: CancellationToken
        ) =
        lspServices.GetRequiredService<ContextHolder>()
        |> _.GetContext()
        |> Task.FromResult

type DocumentStateHandler() =
    interface IMethodHandler with
        member _.MutatesSolutionState = true

    interface IRequestHandler<DidOpenTextDocumentParams, SemanticTokensDeltaPartialResult, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentDidOpenName, LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync
            (
                request: DidOpenTextDocumentParams,
                context: FSharpRequestContext,
                cancellationToken: CancellationToken
            ) =
            let contextHolder = context.LspServices.GetRequiredService<ContextHolder>()

            contextHolder.UpdateWorkspace(fun w ->
                { w with
                    OpenFiles = Map.add request.TextDocument.Uri.AbsolutePath request.TextDocument.Text w.OpenFiles
                })

            Task.FromResult(SemanticTokensDeltaPartialResult())

    interface IRequestHandler<DidChangeTextDocumentParams, SemanticTokensDeltaPartialResult, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentDidChangeName, LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync
            (
                request: DidChangeTextDocumentParams,
                context: FSharpRequestContext,
                cancellationToken: CancellationToken
            ) =
            let contextHolder = context.LspServices.GetRequiredService<ContextHolder>()

            contextHolder.UpdateWorkspace(fun w ->
                { w with
                    OpenFiles = Map.add request.TextDocument.Uri.AbsolutePath request.ContentChanges.[0].Text w.OpenFiles
                })

            Task.FromResult(SemanticTokensDeltaPartialResult())

    interface INotificationHandler<DidCloseTextDocumentParams, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentDidCloseName, LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleNotificationAsync
            (
                request: DidCloseTextDocumentParams,
                context: FSharpRequestContext,
                cancellationToken: CancellationToken
            ) =
            let contextHolder = context.LspServices.GetRequiredService<ContextHolder>()

            contextHolder.UpdateWorkspace(fun w ->
                { w with
                    OpenFiles = Map.remove request.TextDocument.Uri.AbsolutePath w.OpenFiles
                })

            Task.CompletedTask

type LanguageFeaturesHandler() =
    interface IMethodHandler with
        member _.MutatesSolutionState = false

    // TODO: this is not getting called
    interface IRequestHandler<DocumentDiagnosticParams, SumType<RelatedFullDocumentDiagnosticReport, RelatedUnchangedDocumentDiagnosticReport>, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentDiagnosticName, LanguageServerConstants.DefaultLanguageName)>]
        member _.HandleRequestAsync
            (
                request: DocumentDiagnosticParams,
                context: FSharpRequestContext,
                cancellationToken: CancellationToken
            ) =
            Task.FromResult(new RelatedUnchangedDocumentDiagnosticReport())

type CapabilitiesManager() =

    let mutable initializeParams = None

    interface IInitializeManager<InitializeParams, InitializeResult> with
        member this.SetInitializeParams(request) = initializeParams <- Some request

        member this.GetInitializeResult() =
            let serverCapabilities =
                ServerCapabilities(
                    TextDocumentSync = TextDocumentSyncOptions(OpenClose = true, Change = TextDocumentSyncKind.Full),
                    DiagnosticOptions = DiagnosticOptions(InterFileDependencies = true, Identifier = "potato", WorkspaceDiagnostics = true)
                )

            InitializeResult(Capabilities = serverCapabilities)

        member this.GetInitializeParams() =
            match initializeParams with
            | Some params' -> params'
            | None -> failwith "InitializeParams is null"

type LspServiceLifeCycleManager() =

    interface ILifeCycleManager with
        member this.ShutdownAsync(message: string) =
            task {
                try
                    printfn "Shutting down"
                with
                | :? ObjectDisposedException
                | :? ConnectionLostException -> ()
            }

        member this.ExitAsync() = Task.CompletedTask

type FSharpLspServices(serviceCollection: IServiceCollection) as this =

    do serviceCollection.AddSingleton<ILspServices>(this) |> ignore

    let serviceProvider = serviceCollection.BuildServiceProvider()

    interface ILspServices with
        member this.GetRequiredService<'T>() : 'T =
            serviceProvider.GetRequiredService<'T>()

        member this.TryGetService(t) = serviceProvider.GetService(t)

        member this.GetRequiredServices() = serviceProvider.GetServices()

        member this.GetRegisteredServices() = failwith "Not implemented"

        member this.SupportsGetRegisteredServices() = false

        member this.Dispose() = ()

type FSharpLanguageServer
    (jsonRpc: JsonRpc, logger: ILspLogger, ?initialWorkspace: FSharpWorkspace, ?addExtraHandlers: Action<IServiceCollection>) =
    inherit AbstractLanguageServer<FSharpRequestContext>(jsonRpc, logger)

    let mutable _addExtraHandlers = addExtraHandlers

    let initialWorkspace =
        defaultArg
            initialWorkspace
            {
                Projects = Map.empty
                OpenFiles = Map.empty
            }

    do
        // This spins up the queue and ensure the LSP is ready to start receiving requests
        base.Initialize()

    member private this.GetBaseHandlerProvider() = base.HandlerProvider

    override this.ConstructLspServices() =
        let serviceCollection = new ServiceCollection()

        let _ =
            serviceCollection
                .AddSingleton(initialWorkspace)
                .AddSingleton<ContextHolder>()
                .AddSingleton<IMethodHandler, InitializeHandler<InitializeParams, InitializeResult, FSharpRequestContext>>()
                .AddSingleton<IMethodHandler, InitializedHandler<InitializedParams, FSharpRequestContext>>()
                .AddSingleton<IMethodHandler, DocumentStateHandler>()
                .AddSingleton<IMethodHandler, LanguageFeaturesHandler>()
                .AddSingleton<ILspLogger>(logger)
                .AddSingleton<AbstractRequestContextFactory<FSharpRequestContext>, FShapRequestContextFactory>()
                .AddSingleton<AbstractHandlerProvider>(fun _ -> this.GetBaseHandlerProvider())
                .AddSingleton<IInitializeManager<InitializeParams, InitializeResult>, CapabilitiesManager>()
                .AddSingleton(this)
                .AddSingleton<ILifeCycleManager>(new LspServiceLifeCycleManager())

        match _addExtraHandlers with
        | Some handler -> handler.Invoke(serviceCollection)
        | None -> ()

        let lspServices = new FSharpLspServices(serviceCollection)

        lspServices :> ILspServices

    static member Create() =
        FSharpLanguageServer.Create(LspLogger System.Diagnostics.Trace.TraceInformation)

    static member Create(logger: ILspLogger) =

        let struct (clientStream, serverStream) = FullDuplexStream.CreatePair()

        // TODO: handle disposal of these
        let formatter = new JsonMessageFormatter()

        let messageHandler =
            new HeaderDelimitedMessageHandler(serverStream, serverStream, formatter)

        let jsonRpc = new JsonRpc(messageHandler)

        let rpcTrace = new StringWriter()

        let listener = new TextWriterTraceListener(rpcTrace)

        jsonRpc.TraceSource.Listeners.Add(listener) |> ignore

        jsonRpc.TraceSource.Switch.Level <- SourceLevels.Information

        let server = new FSharpLanguageServer(jsonRpc, logger)

        jsonRpc.StartListening()

        (clientStream, clientStream), server, rpcTrace
