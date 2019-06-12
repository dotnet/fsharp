// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.LanguageServer

open System.Threading

module TextDocument =

    let mutable publishDiagnosticsCancellationTokenSource = new CancellationTokenSource()

    let Hover (state: State) (textDocument: TextDocumentIdentifier) (position: Position) =
        async {
            eprintfn "hover at %d, %d" position.line position.character
            if not state.Options.usePreviewTextHover then return None
            else
                let startCol, endCol =
                    if position.character = 0 then 0, 1
                    else position.character, position.character + 1
                return Some { contents = { kind = MarkupKind.PlainText
                                           value = "serving textDocument/hover from LSP" }
                              range = Some { start = { line = position.line; character = startCol }
                                             ``end`` = { line = position.line; character = endCol } }
                }
        }

    let PublishDiagnostics(state: State, projectOptions: FSharp.Compiler.SourceCodeServices.FSharpProjectOptions) =
        // TODO: honor TextDocumentClientCapabilities.publishDiagnostics.relatedInformation
        // cancel any existing request to publish diagnostics
        publishDiagnosticsCancellationTokenSource.Cancel()
        publishDiagnosticsCancellationTokenSource <- new CancellationTokenSource()
        async {
            if not state.Options.usePreviewDiagnostics then return ()
            else
                eprintfn "starting diagnostics computation"
                match state.JsonRpc with
                | None -> eprintfn "state.JsonRpc was null; should not be?"
                | Some jsonRpc ->
                    let! results = state.Checker.ParseAndCheckProject projectOptions
                    let diagnostics = results.Errors
                    let diagnosticsPerFile =
                        diagnostics
                        |> Array.fold (fun state t ->
                            let existing = Map.tryFind t.FileName state |> Option.defaultValue []
                            Map.add t.FileName (t :: existing) state) Map.empty
                    for sourceFile in projectOptions.SourceFiles do
                        let diagnostics =
                            Map.tryFind sourceFile diagnosticsPerFile
                            |> Option.defaultValue []
                            |> List.map (fun d ->
                                // F# errors count lines starting at 1, but LSP starts at 0
                                let range: Range =
                                    { start = { line = d.StartLineAlternate - 1; character = d.StartColumn }
                                      ``end`` = { line = d.EndLineAlternate - 1; character = d.EndColumn } }
                                let severity =
                                    match d.Severity with
                                    | FSharp.Compiler.SourceCodeServices.FSharpErrorSeverity.Warning -> Diagnostic.Warning
                                    | FSharp.Compiler.SourceCodeServices.FSharpErrorSeverity.Error -> Diagnostic.Error
                                let res: Diagnostic =
                                    { range = range
                                      severity = Some severity
                                      code = "FS" + d.ErrorNumber.ToString("0000")
                                      source = Some d.FileName
                                      message = d.Message
                                      relatedInformation = None }
                                res)
                            |> List.toArray
                        let args: PublishDiagnosticsParams =
                            { uri = System.Uri(sourceFile).AbsoluteUri
                              diagnostics = diagnostics }

                        // fire each notification separately
                        jsonRpc.NotifyAsync(TextDocumentPublishDiagnostics, args) |> Async.AwaitTask |> Async.Start
        }
        |> (fun computation -> Async.StartAsTask(computation, cancellationToken=publishDiagnosticsCancellationTokenSource.Token))
        |> Async.AwaitTask
