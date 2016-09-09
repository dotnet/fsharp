// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Linq

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Implementation.Debugging
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Tagging

open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

type internal FSharpCompletionProvider(workspace: Workspace) =
    inherit CompletionProvider()

    let completionTriggers = [ '.' ]

    override this.ShouldTriggerCompletion(sourceText: SourceText, caretPosition: int, trigger: CompletionTrigger, _: OptionSet) =
        // Skip if we are at the start of a document
        if caretPosition = 0 then
            false

        // Skip if it was triggered by an operation other than insertion
        else if not (trigger.Kind = CompletionTriggerKind.Insertion) then
            false

        // Skip if we are not on a completion trigger
        else if not (completionTriggers |> Seq.contains(sourceText.[caretPosition - 1])) then
            false

        // Trigger completion if we are on a valid classification type
        else
            let documentId = workspace.GetDocumentIdInCurrentContext(sourceText.Container)
            let document = workspace.CurrentSolution.GetDocument(documentId)
        
            match FSharpLanguageService.GetOptions(document.Project.Id) with
            | Some(options) ->
        
                let triggerPosition = caretPosition - 1
                let textLine = sourceText.Lines.GetLineFromPosition(triggerPosition)
                let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
                let classifiedSpanOption =
                    FSharpColorizationService.GetColorizationData(sourceText, textLine.Span, Some(document.FilePath), defines, CancellationToken.None)
                    |> Seq.tryFind(fun classifiedSpan -> classifiedSpan.TextSpan.Contains(triggerPosition))

                match classifiedSpanOption with
                | None -> false
                | Some(classifiedSpan) ->
                    match classifiedSpan.ClassificationType with
                    | ClassificationTypeNames.Comment -> false
                    | ClassificationTypeNames.StringLiteral -> false
                    | ClassificationTypeNames.ExcludedCode -> false
                    | _ -> true // anything else is a valid classification type

            | None -> false
    
    override this.ProvideCompletionsAsync(context: Microsoft.CodeAnalysis.Completion.CompletionContext): Task =
        Task.CompletedTask

    override this.GetDescriptionAsync(document: Document, item: CompletionItem, cancellationToken: CancellationToken) =
        Task.FromResult(CompletionDescription.Empty)
