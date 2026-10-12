namespace FSharp.Compiler.LanguageServer

open System
open System.Diagnostics
open System.Runtime.CompilerServices
open System.Text.Json
open System.Text.Json.Serialization

open FSharp.Compiler.LanguageServer.Common
open FSharp.Compiler.LanguageServer.Handlers

open Microsoft.CommonLanguageServerProtocol.Framework.Handlers
open Microsoft.CommonLanguageServerProtocol.Framework
open Microsoft.Extensions.DependencyInjection
open Microsoft.VisualStudio.LanguageServer.Protocol

open StreamJsonRpc
open Nerdbank.Streams
open FSharp.Compiler.CodeAnalysis.Workspace

#nowarn "57"

[<AutoOpen>]
module Stuff =
    [<Literal>]
    let FSharpLanguageName = "F#"

    // Protocol.Extensions exposes its System.Text.Json converter registration only internally.
    type private VSExtensionConverter<'TBase when 'TBase: not struct and 'TBase: not null>(extensionType: Type) =
        inherit JsonConverter<'TBase>()

        do
            if not (typeof<'TBase>.IsAssignableFrom(extensionType)) then
                invalidArg (nameof extensionType) "The extension type must derive from the protocol type."

        override _.Read(reader: byref<Utf8JsonReader>, _typeToConvert: Type, options: JsonSerializerOptions) : 'TBase | null =
            JsonSerializer.Deserialize(&reader, extensionType, options) :?> ('TBase | null)

        override this.Write(writer: Utf8JsonWriter, value: 'TBase, options: JsonSerializerOptions) =
            let runtimeType = value.GetType()

            if runtimeType = typeof<'TBase> then
                let baseOptions = JsonSerializerOptions(options)
                baseOptions.Converters.Remove(this) |> ignore
                JsonSerializer.Serialize(writer, value, baseOptions)
            else
                JsonSerializer.Serialize(writer, value, runtimeType, options)

    let createConfiguredFormatter () =
        let formatter = new SystemTextJsonFormatter()
        let options = formatter.JsonSerializerOptions
        options.Converters.Add(VSExtensionConverter<Diagnostic>(typeof<VSDiagnostic>))
        options.Converters.Add(VSExtensionConverter<Location>(typeof<VSLocation>))
        options.Converters.Add(VSExtensionConverter<ServerCapabilities>(typeof<VSServerCapabilities>))
        options.Converters.Add(VSExtensionConverter<SymbolInformation>(typeof<VSSymbolInformation>))
        options.Converters.Add(VSExtensionConverter<TextDocumentIdentifier>(typeof<VSTextDocumentIdentifier>))
        formatter

[<Extension>]
type Extensions =

    [<Extension>]
    static member Please(this: Async<'t>, ct) =
        Async.StartAsTask(this, cancellationToken = ct)

type FSharpLanguageServer
    (
        jsonRpc: JsonRpc,
        serializerOptions: JsonSerializerOptions,
        logger: ILspLogger,
        ?initialWorkspace: FSharpWorkspace,
        ?addExtraHandlers: Action<IServiceCollection>,
        ?config: FSharpLanguageServerConfig
    ) =

    inherit SystemTextJsonLanguageServer<FSharpRequestContext>(jsonRpc, serializerOptions)

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
                .AddSingleton<IMethodHandler, SemanticTokensHandler>()
                .AddSingleton<IMethodHandler, CodeActionHandler>()
                .AddSingleton<IMethodHandler, ProjectContextsHandler>()
                .AddSingleton<ILspLogger>(logger)
                .AddSingleton<ILspTelemetry>(NullTelemetry.Instance)
                .AddSingleton<AbstractRequestContextFactory<FSharpRequestContext>, FShapRequestContextFactory>()
                .AddSingleton<IInitializeManager<InitializeParams, InitializeResult>, CapabilitiesManager>()
                .AddSingleton(this)
                .AddSingleton<IOnServerShutdown>(new LspServiceLifeCycleManager(logger))

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
        let formatter = createConfiguredFormatter ()

        let messageHandler =
            new HeaderDelimitedMessageHandler(serverStream, serverStream, formatter)

        let jsonRpc = new JsonRpc(messageHandler)

        let consoleListener = new TextWriterTraceListener(Console.Out)
        jsonRpc.TraceSource.Listeners.Add(consoleListener) |> ignore

        jsonRpc.TraceSource.Switch.Level <- SourceLevels.All

        let server =
            new FSharpLanguageServer(
                jsonRpc,
                formatter.JsonSerializerOptions,
                logger,
                initialWorkspace,
                ?addExtraHandlers = addExtraHandlers,
                ?config = config
            )

        jsonRpc.StartListening()

        (clientStream, clientStream), server
