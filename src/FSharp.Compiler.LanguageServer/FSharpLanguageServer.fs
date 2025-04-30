namespace FSharp.Compiler.LanguageServer

open System.Runtime.CompilerServices
open FSharp.Compiler.LanguageServer.Common
open FSharp.Compiler.LanguageServer.Handlers

open System
open Microsoft.CommonLanguageServerProtocol.Framework.Handlers
open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.Extensions.DependencyInjection
open Microsoft.VisualStudio.LanguageServer.Protocol

open StreamJsonRpc
open Nerdbank.Streams
open System.Diagnostics
open FSharp.Compiler.CodeAnalysis.Workspace

#nowarn "57"

[<AutoOpen>]
module Stuff =
    [<Literal>]
    let FSharpLanguageName = "F#"

[<Extension>]
type Extensions =

    [<Extension>]
    static member Please(this: Async<'t>, ct) =
        Async.StartAsTask(this, cancellationToken = ct)

type FSharpLanguageServer
    (
        jsonRpc: JsonRpc,
        logger: ILspLogger,
        ?initialWorkspace: FSharpWorkspace,
        ?addExtraHandlers: Action<IServiceCollection>,
        ?config: FSharpLanguageServerConfig
    ) =

    // TODO: Switch to SystemTextJsonLanguageServer
    inherit NewtonsoftLanguageServer<FSharpRequestContext>(jsonRpc, Newtonsoft.Json.JsonSerializer.CreateDefault(), logger)

    let config = defaultArg config FSharpLanguageServerConfig.Default
    let initialWorkspace = defaultArg initialWorkspace (FSharpWorkspace())

    do
        // This spins up the queue and ensure the LSP is ready to start receiving requests
        base.Initialize()

    member _.JsonRpc: JsonRpc = jsonRpc

    override this.ConstructLspServices() =
        let serviceCollection = new ServiceCollection()

        let _ =
            serviceCollection
                .AddSingleton(initialWorkspace)
                .AddSingleton<ContextHolder>()
                .AddSingleton<FSharpLanguageServerConfig>(config)
                .AddSingleton<IMethodHandler, InitializeHandler<InitializeParams, InitializeResult, FSharpRequestContext>>()
                .AddSingleton<IMethodHandler, InitializedHandler<InitializedParams, FSharpRequestContext>>()
                .AddSingleton<IMethodHandler, DocumentStateHandler>()
                .AddSingleton<IMethodHandler, LanguageFeaturesHandler>()
                .AddSingleton<ILspLogger>(logger)
                .AddSingleton<AbstractRequestContextFactory<FSharpRequestContext>, FShapRequestContextFactory>()
                .AddSingleton<IInitializeManager<InitializeParams, InitializeResult>, CapabilitiesManager>()
                .AddSingleton(this)
                .AddSingleton<ILifeCycleManager>(new LspServiceLifeCycleManager())

        match addExtraHandlers with
        | Some handler -> handler.Invoke(serviceCollection)
        | None -> ()

        let lspServices = new FSharpLspServices(serviceCollection)

        lspServices :> ILspServices

    static member Create() =
        FSharpLanguageServer.Create(FSharpWorkspace())

    static member Create(initialWorkspace) =
        FSharpLanguageServer.Create(initialWorkspace, (fun _ -> ()))

    static member Create(initialWorkspace, addExtraHandlers: Action<IServiceCollection>) =
        FSharpLanguageServer.Create(LspLogger System.Diagnostics.Trace.TraceInformation, initialWorkspace, addExtraHandlers)

    static member Create(initialWorkspace, config: FSharpLanguageServerConfig, addExtraHandlers: Action<IServiceCollection>) =
        FSharpLanguageServer.Create(LspLogger System.Diagnostics.Trace.TraceInformation, initialWorkspace, addExtraHandlers, config)

    static member Create
        (logger: ILspLogger, initialWorkspace, ?addExtraHandlers: Action<IServiceCollection>, ?config: FSharpLanguageServerConfig)
        =

        let struct (clientStream, serverStream) = FullDuplexStream.CreatePair()

        // TODO: handle disposal of these
        let formatter = new JsonMessageFormatter()

        let messageHandler =
            new HeaderDelimitedMessageHandler(serverStream, serverStream, formatter)

        let jsonRpc = new JsonRpc(messageHandler)

        let listener = new TextWriterTraceListener(Console.Out)

        jsonRpc.TraceSource.Listeners.Add(listener) |> ignore

        jsonRpc.TraceSource.Switch.Level <- SourceLevels.All

        let server =
            new FSharpLanguageServer(jsonRpc, logger, initialWorkspace, ?addExtraHandlers = addExtraHandlers, ?config = config)

        jsonRpc.StartListening()

        (clientStream, clientStream), server
