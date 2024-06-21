namespace FSharp.Compiler.LanguageServer.Common

open Microsoft.VisualStudio.LanguageServer.Protocol
open Microsoft.CommonLanguageServerProtocol.Framework

type IServerCapabilitiesOverride =
    abstract member OverrideServerCapabilities: ServerCapabilities -> ServerCapabilities

type CapabilitiesManager(scOverrides: IServerCapabilitiesOverride seq) =

    let mutable initializeParams = None

    let defaultCapabilities =
        ServerCapabilities(
            TextDocumentSync = TextDocumentSyncOptions(OpenClose = true, Change = TextDocumentSyncKind.Full),
            DiagnosticOptions =
                DiagnosticOptions(WorkDoneProgress = true, InterFileDependencies = true, Identifier = "potato", WorkspaceDiagnostics = true),
            CompletionProvider =
                CompletionOptions(TriggerCharacters=[|"."; " "|], ResolveProvider=true, WorkDoneProgress=true),
            HoverProvider = SumType<bool, HoverOptions>(HoverOptions(WorkDoneProgress = true))
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