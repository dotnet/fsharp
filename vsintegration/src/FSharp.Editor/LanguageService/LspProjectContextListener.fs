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

[<Export(typeof<IWpfTextViewCreationListener>)>]
[<ContentType(Constants.FSharpContentType)>]
[<TextViewRole(PredefinedTextViewRoles.Document)>]
type internal LspProjectContextListener [<ImportingConstructor>] (broker: ILanguageServiceBroker2, workspace: VisualStudioWorkspace) =
    interface IWpfTextViewCreationListener with
        member _.TextViewCreated(view) =
            let cancellation = new CancellationTokenSource()
            let requests = ThreadHelper.JoinableTaskContext.CreateCollection()
            let factory = ThreadHelper.JoinableTaskContext.CreateFactory requests

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
                        updateContext ())

            workspace.DocumentActiveContextChanged.AddHandler handler

            view.Closed.Add(fun _ ->
                workspace.DocumentActiveContextChanged.RemoveHandler handler
                cancellation.Cancel()

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
