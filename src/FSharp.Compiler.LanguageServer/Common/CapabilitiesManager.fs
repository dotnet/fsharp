namespace FSharp.Compiler.LanguageServer.Common

open Microsoft.VisualStudio.LanguageServer.Protocol
open Microsoft.CommonLanguageServerProtocol.Framework
open FSharp.Compiler.LanguageServer

type IServerCapabilitiesOverride =
    abstract member OverrideServerCapabilities: FSharpLanguageServerConfig * ServerCapabilities * ClientCapabilities -> ServerCapabilities

type CapabilitiesManager(config: FSharpLanguageServerConfig, scOverrides: IServerCapabilitiesOverride seq) =

    let mutable initializeParams = None

    let getInitializeParams () =
        match initializeParams with
        | Some params' -> params'
        | None -> failwith "InitializeParams is null"

    let addIf (enabled: bool) (capability: 'a) =
        if enabled then capability |> withNull else null

    let defaultCapabilities (_clientCapabilities: ClientCapabilities) =
        // TODO: don't register if dynamic registraion is supported
        ServerCapabilities(
            TextDocumentSync = TextDocumentSyncOptions(OpenClose = true, Change = TextDocumentSyncKind.Full),
            DiagnosticOptions =
                addIf
                    config.EnabledFeatures.Diagnostics
                    (DiagnosticOptions(
                        WorkDoneProgress = true,
                        InterFileDependencies = true,
                        Identifier = "potato",
                        WorkspaceDiagnostics = true
                    )),
            //CompletionProvider = CompletionOptions(TriggerCharacters = [| "."; " " |], ResolveProvider = true, WorkDoneProgress = true),
            //HoverProvider = SumType<bool, HoverOptions>(HoverOptions(WorkDoneProgress = true))
            SemanticTokensOptions =
                addIf
                    config.EnabledFeatures.SemanticHighlighting

                    (SemanticTokensOptions(
                        Legend =
                            SemanticTokensLegend(
                                TokenTypes = (SemanticTokenTypes.AllTypes |> Seq.toArray), // XXX should be extended
                                TokenModifiers = (SemanticTokenModifiers.AllModifiers |> Seq.toArray)
                            ),
                        Range = false
                    ))
        )

    interface IInitializeManager<InitializeParams, InitializeResult> with
        member this.SetInitializeParams(request) = initializeParams <- Some request

        member this.GetInitializeParams() = getInitializeParams ()

        member this.GetInitializeResult() =
            let clientCapabilities = getInitializeParams().Capabilities

            let serverCapabilities =
                (defaultCapabilities clientCapabilities, scOverrides)
                ||> Seq.fold (fun acc (x: IServerCapabilitiesOverride) -> x.OverrideServerCapabilities(config, acc, clientCapabilities))

            InitializeResult(Capabilities = serverCapabilities)
