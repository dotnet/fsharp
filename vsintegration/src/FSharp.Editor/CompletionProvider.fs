// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Linq
open System.Runtime.CompilerServices

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
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

type internal FSharpCompletionProvider(workspace: Workspace, serviceProvider: SVsServiceProvider) =
    inherit CompletionProvider()

    let completionTriggers = [ '.' ]
    let declarationItemsCache = ConditionalWeakTable<string, FSharpDeclarationListItem>()
    
    let xmlMemberIndexService = serviceProvider.GetService(typeof<IVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)

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
    
    override this.ProvideCompletionsAsync(context: Microsoft.CodeAnalysis.Completion.CompletionContext) =
        let computation = async {
            match FSharpLanguageService.GetOptions(context.Document.Project.Id) with
            | Some(options) ->
                let! sourceText = context.Document.GetTextAsync(context.CancellationToken) |> Async.AwaitTask
                let! parseResults = FSharpChecker.Instance.ParseFileInProject(context.Document.FilePath, sourceText.ToString(), options)
                let! textVersion = context.Document.GetTextVersionAsync(context.CancellationToken) |> Async.AwaitTask
                let! checkFileAnswer = FSharpChecker.Instance.CheckFileInProject(parseResults, context.Document.FilePath, textVersion.GetHashCode(), sourceText.ToString(), options)
                let checkFileResults = match checkFileAnswer with
                                       | FSharpCheckFileAnswer.Aborted -> failwith "Compilation isn't complete yet"
                                       | FSharpCheckFileAnswer.Succeeded(results) -> results

                let textLine = sourceText.Lines.GetLineFromPosition(context.Position)
                let textLineNumber = textLine.LineNumber + 1 // Roslyn line numbers are zero-based
                let qualifyingNames, partialName = QuickParse.GetPartialLongNameEx(textLine.ToString(), context.Position - textLine.Start - 1) 
                let! declarations = checkFileResults.GetDeclarationListInfo(Some(parseResults), textLineNumber, context.Position, textLine.ToString(), qualifyingNames, partialName)

                for declarationItem in declarations.Items do
                    let completionItem = CompletionItem.Create(declarationItem.Name)
                    declarationItemsCache.Add(completionItem.DisplayText, declarationItem)
                    context.AddItem(completionItem)
            | None -> ()
        }
        
        Task.Run(CommonRoslynHelpers.GetTaskAction(computation), context.CancellationToken)

    override this.GetDescriptionAsync(_: Document, completionItem: CompletionItem, cancellationToken: CancellationToken): Task<CompletionDescription> =
        let computation = async {
            let exists, declarationItem = declarationItemsCache.TryGetValue(completionItem.DisplayText)
            if exists then
                let! description = declarationItem.DescriptionTextAsync
                let datatipText = XmlDocumentation.BuildDataTipText(documentationBuilder, description) 
                return CompletionDescription.FromText(datatipText)
            else
                return CompletionDescription.Empty
        }
        
        Async.StartAsTask(computation, TaskCreationOptions.None, cancellationToken)
            .ContinueWith(CommonRoslynHelpers.GetCompletedTaskResult, cancellationToken)