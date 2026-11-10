namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.LanguageServer.Client
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Threading
open Microsoft.VisualStudio.Utilities

#nowarn "44"

[<Export(typeof<IWpfTextViewCreationListener>)>]
[<ContentType(Constants.FSharpContentType)>]
[<TextViewRole(PredefinedTextViewRoles.Document)>]
[<PartCreationPolicy(CreationPolicy.Shared)>]
type internal LspProjectContextListener [<ImportingConstructor>] (broker: ILanguageServiceBroker2, workspace: VisualStudioWorkspace) =
    let options = workspace.Services.GetService<EditorOptions>()

    let telemetry =
        new LspTelemetryConsentForwarder(
            HighlightingTelemetry.instance,
            options,
            ThreadHelper.JoinableTaskContext,
            (fun parameters cancellationToken ->
                let request =
                    GeneralRequest<_, HighlightingTelemetryConfigurationResult>(
                        Method = "fsharp/telemetry/configure",
                        LanguageServerName = "FSharp Analyzer LSP server",
                        Request = parameters
                    )

                broker.RequestAsync(request, cancellationToken))
        )

    let loaded =
        EventHandler<LanguageClientLoadedEventArgs>(fun _ args ->
            if args.ClientInstance.Client.Name = "FSharp Analyzer LSP server" then
                telemetry.Configure true)

    do broker.LanguageClientLoaded.AddHandler loaded

    interface IDisposable with
        member _.Dispose() =
            broker.LanguageClientLoaded.RemoveHandler loaded
            (telemetry :> IDisposable).Dispose()

    interface IWpfTextViewCreationListener with
        member _.TextViewCreated(view) =
            let cancellation = new CancellationTokenSource()
            let requests = ThreadHelper.JoinableTaskContext.CreateCollection()
            let factory = ThreadHelper.JoinableTaskContext.CreateFactory requests

            let struct (settingValue, provider) =
                HighlightingTelemetry.readProvider ServiceProvider.GlobalProvider

            telemetry.RegisterView(settingValue, provider, view.Closed)

            let focused =
                EventHandler(fun _ _ -> telemetry.Configure(retry = (provider = "lsp")))

            view.GotAggregateFocus.AddHandler focused

            let updateContext () =
                factory
                    .RunAsync(fun () ->
                        task {
                            let document = view.TextSnapshot.GetOpenDocumentInCurrentContextWithChanges()

                            if not (isNull document) && not (isNull document.Project.OutputFilePath) then
                                // The Roslyn navbar does not populate the LSP client's navbar context.
                                let parameters =
                                    {|
                                        Uri = Uri(document.FilePath)
                                        ProjectFileName = document.Project.FilePath
                                        OutputFileName = document.Project.OutputFilePath
                                    |}

                                let request =
                                    DocumentRequest<_, bool>(
                                        TextBuffer = view.TextBuffer,
                                        Method = "textDocument/fsharp/setActiveProjectContext",
                                        LanguageServerName = "FSharp Analyzer LSP server",
                                        ParameterFactory = (fun _ -> parameters)
                                    )

                                let! accepted = broker.RequestAsync(request, cancellation.Token)

                                if not accepted then
                                    System.Diagnostics.Trace.TraceWarning("F# LSP active project context was not accepted.")
                        }
                        :> Task)
                    .FileAndForget("vs/fsharp/lsp/projectContext")

            let handler =
                EventHandler<DocumentActiveContextChangedEventArgs>(fun _ args ->
                    if args.SourceTextContainer = view.TextBuffer.AsTextContainer() then
                        telemetry.Configure false
                        updateContext ())

            workspace.DocumentActiveContextChanged.AddHandler handler

            view.Closed.Add(fun _ ->
                workspace.DocumentActiveContextChanged.RemoveHandler handler
                cancellation.Cancel()
                view.GotAggregateFocus.RemoveHandler focused

                ThreadHelper.JoinableTaskFactory
                    .RunAsync(fun () ->
                        task {
                            try
                                do! requests.JoinTillEmptyAsync()
                            finally
                                cancellation.Dispose()
                        }
                        :> Task)
                    .FileAndForget("vs/fsharp/lsp/projectContext/close"))

            updateContext ()
