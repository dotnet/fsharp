// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Threading

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.Commanding
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.LanguageServices.Implementation.Snippets
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Utilities

[<Export(typeof<ICommandHandler>)>]
[<ContentType(FSharpConstants.FSharpLanguageName)>]
[<Name("F# Snippets")>]
//[<Order(After=Microsoft.CodeAnalysis.Editor.PredefinedCommandHandlerNames.Completion)>]
[<Order(After=Microsoft.CodeAnalysis.Editor.PredefinedCommandHandlerNames.IntelliSense)>]
type SnippetCommandHandler
    [<ImportingConstructor>]
    (
        editorAdaptersFactoryService: IVsEditorAdaptersFactoryService,
        serviceProvider: SVsServiceProvider
    ) =
    inherit AbstractSnippetCommandHandler(editorAdaptersFactoryService, serviceProvider)
        override __.IsSnippetExpansionContext(_document: Document, _startPosition: int, _cancellationToken: CancellationToken) =
            // TODO: only true if not in a string or comment
            true
        override __.GetSnippetExpansionClient(textView: ITextView, _subjectBuffer: ITextBuffer) =
            upcast FSharpSnippetExpansionClient(textView, editorAdaptersFactoryService)
        override this.TryInvokeInsertionUI(textView: ITextView, subjectBuffer: ITextBuffer, _surroundWith: bool) =
            match this.TryGetExpansionManager() with
            | (true, expansionManager) ->
                let types = [|"Expansion"; "SurroundsWith"|]
                let client = this.GetSnippetExpansionClient(textView, subjectBuffer)
                expansionManager.InvokeInsertionUI(
                    editorAdaptersFactoryService.GetViewAdapter(textView), // pView
                    client, // pClient
                    Guid(FSharpConstants.languageServiceGuidString), // guidLang
                    types, // bstrTypes
                    types.Length, // iCountTypes
                    1, // fIncludeNULLType
                    null, // bstrKinds
                    0, // iCountKinds
                    0, // fIncludeNULLKind
                    SR.InsertSnippet(), //bstrPrefixText
                    ">" // bstrCompletionChar
                ) |> ignore
                true
            | _ -> false
