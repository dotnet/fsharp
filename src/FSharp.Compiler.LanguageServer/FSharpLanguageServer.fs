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
open FSharp.Compiler.CodeAnalysis

[<AutoOpen>]
module Stuff =
    [<Literal>]
    let FSharpLanguageName = "F#"

type FSharpRequestContext(lspServices: ILspServices, logger: ILspLogger, workspace: FSharpWorkspace, checker) =
    member _.LspServices = lspServices
    member _.Logger = logger
    member _.Workspace = workspace
    member _.Checker = checker

type ContextHolder(intialWorkspace, lspServices: ILspServices) =

    let logger = lspServices.GetRequiredService<ILspLogger>()
    let checker =
        FSharpChecker.Create(
            keepAssemblyContents=true,
            keepAllBackgroundResolutions=true,
            keepAllBackgroundSymbolUses=true,
            enableBackgroundItemKeyStoreAndSemanticClassification=true,
            enablePartialTypeChecking=true,
            parallelReferenceResolution=true,
            captureIdentifiersWhenParsing=true,
            useSyntaxTreeCache=true,
            useTransparentCompiler=true)

    let mutable context = FSharpRequestContext(lspServices, logger, intialWorkspace, checker)

    member _.GetContext() = context

    member _.UpdateWorkspace(f) =
        context <- FSharpRequestContext(lspServices, logger, f context.Workspace, checker)

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

type DocumentStateHandler() =
    interface IMethodHandler with
        member _.MutatesSolutionState = true

    interface IRequestHandler<DidOpenTextDocumentParams, SemanticTokensDeltaPartialResult, FSharpRequestContext> with
        [<LanguageServerEndpoint(Methods.TextDocumentDidOpenName)>]
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
        [<LanguageServerEndpoint(Methods.TextDocumentDidChangeName)>]
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
        [<LanguageServerEndpoint(Methods.TextDocumentDidCloseName)>]
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
        [<LanguageServerEndpoint(Methods.TextDocumentDiagnosticName)>]
        member _.HandleRequestAsync
            (
                request: DocumentDiagnosticParams,
                context: FSharpRequestContext,
                cancellationToken: CancellationToken
            ) =
            Task.FromResult(new RelatedUnchangedDocumentDiagnosticReport())

type IServerCapabilitiesOverride =
    abstract member OverrideServerCapabilities: ServerCapabilities -> ServerCapabilities

type CapabilitiesManager(scOverrides: IServerCapabilitiesOverride seq) =

    let mutable initializeParams = None

    let defaultCapabilities =
        ServerCapabilities(
            TextDocumentSync = TextDocumentSyncOptions(OpenClose = true, Change = TextDocumentSyncKind.Full),
            DiagnosticOptions =
                DiagnosticOptions(WorkDoneProgress = true, InterFileDependencies = true, Identifier = "potato", WorkspaceDiagnostics = true)
        // CompletionProvider = CompletionOptions(TriggerCharacters=[|"."; " "|], ResolveProvider=true, WorkDoneProgress=true)
        )

    interface IInitializeManager<InitializeParams, InitializeResult> with
        member this.SetInitializeParams(request) = initializeParams <- Some request

        member this.GetInitializeResult() =
            let serverCapabilities =
                (defaultCapabilities, scOverrides)
                ||> Seq.fold (fun acc (x: IServerCapabilitiesOverride) -> x.OverrideServerCapabilities acc)

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

    let initialWorkspace = defaultArg initialWorkspace (FSharpWorkspace.Create [])

    do
        // This spins up the queue and ensure the LSP is ready to start receiving requests
        base.Initialize()

    member _.JsonRpc: JsonRpc = jsonRpc

    member private this.GetBaseHandlerProvider() = base.GetHandlerProvider()

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
                .AddSingleton<IRequestContextFactory<FSharpRequestContext>, FShapRequestContextFactory>()
                .AddSingleton<IHandlerProvider>(fun _ -> this.GetBaseHandlerProvider())
                .AddSingleton<IInitializeManager<InitializeParams, InitializeResult>, CapabilitiesManager>()
                .AddSingleton(this)
                .AddSingleton<ILifeCycleManager>(new LspServiceLifeCycleManager())

        match addExtraHandlers with
        | Some handler -> handler.Invoke(serviceCollection)
        | None -> ()

        let lspServices = new FSharpLspServices(serviceCollection)

        lspServices :> ILspServices

    static member Create() =
        FSharpLanguageServer.Create(FSharpWorkspace.Create Seq.empty, (fun _ -> ()))

    static member Create(initialWorkspace, addExtraHandlers: Action<IServiceCollection>) =
        FSharpLanguageServer.Create(LspLogger System.Diagnostics.Trace.TraceInformation, initialWorkspace, addExtraHandlers)

    static member Create(logger: ILspLogger, initialWorkspace, ?addExtraHandlers: Action<IServiceCollection>) =

        let struct (clientStream, serverStream) = FullDuplexStream.CreatePair()

        // TODO: handle disposal of these
        let formatter = new JsonMessageFormatter()

        let messageHandler =
            new HeaderDelimitedMessageHandler(serverStream, serverStream, formatter)

        let jsonRpc = new JsonRpc(messageHandler)

        let listener = new TextWriterTraceListener(Console.Out)

        jsonRpc.TraceSource.Listeners.Add(listener) |> ignore

        jsonRpc.TraceSource.Switch.Level <- SourceLevels.Information

        let server =
            new FSharpLanguageServer(jsonRpc, logger, initialWorkspace, ?addExtraHandlers = addExtraHandlers)

        jsonRpc.StartListening()

        (clientStream, clientStream), server
