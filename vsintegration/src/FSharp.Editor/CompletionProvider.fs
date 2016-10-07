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

    static let completionTriggers = [ '.' ]
    static let declarationItemsCache = ConditionalWeakTable<string, FSharpDeclarationListItem>()
    
    let xmlMemberIndexService = serviceProvider.GetService(typeof<IVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)

    static member ShouldTriggerCompletionAux(sourceText: SourceText, caretPosition: int, trigger: CompletionTriggerKind, filePath: string, defines: string list) =
        // Skip if we are at the start of a document
        if caretPosition = 0 then
            false

        // Skip if it was triggered by an operation other than insertion
        else if not (trigger = CompletionTriggerKind.Insertion) then
            false

        // Skip if we are not on a completion trigger
        else if not (completionTriggers |> Seq.contains(sourceText.[caretPosition - 1])) then
            false

        // Trigger completion if we are on a valid classification type
        else
            let triggerPosition = caretPosition - 1
            let textLine = sourceText.Lines.GetLineFromPosition(triggerPosition)
            let classifiedSpanOption =
                FSharpColorizationService.GetColorizationData(sourceText, textLine.Span, Some(filePath), defines, CancellationToken.None)
                |> Seq.tryFind(fun classifiedSpan -> classifiedSpan.TextSpan.Contains(triggerPosition))

            match classifiedSpanOption with
            | None -> false
            | Some(classifiedSpan) ->
                match classifiedSpan.ClassificationType with
                | ClassificationTypeNames.Comment -> false
                | ClassificationTypeNames.StringLiteral -> false
                | ClassificationTypeNames.ExcludedCode -> false
                | _ -> true // anything else is a valid classification type

    static member ProvideCompletionsAsyncAux(sourceText: SourceText, caretPosition: int, options: FSharpProjectOptions, filePath: string, textVersionHash: int) = async {
        let! parseResults = FSharpChecker.Instance.ParseFileInProject(filePath, sourceText.ToString(), options)
        let! checkFileAnswer = FSharpChecker.Instance.CheckFileInProject(parseResults, filePath, textVersionHash, sourceText.ToString(), options)
        let checkFileResults = match checkFileAnswer with
                                | FSharpCheckFileAnswer.Aborted -> failwith "Compilation isn't complete yet"
                                | FSharpCheckFileAnswer.Succeeded(results) -> results

        let textLine = sourceText.Lines.GetLineFromPosition(caretPosition)
        let textLineNumber = textLine.LineNumber + 1 // Roslyn line numbers are zero-based
        let qualifyingNames, partialName = QuickParse.GetPartialLongNameEx(textLine.ToString(), caretPosition - textLine.Start - 1) 
        let! declarations = checkFileResults.GetDeclarationListInfo(Some(parseResults), textLineNumber, caretPosition, textLine.ToString(), qualifyingNames, partialName)

        let results = List<CompletionItem>()

        for declarationItem in declarations.Items do
            let completionItem = CompletionItem.Create(declarationItem.Name)
            declarationItemsCache.Remove(completionItem.DisplayText) |> ignore // clear out stale entries if they exist
            declarationItemsCache.Add(completionItem.DisplayText, declarationItem)
            results.Add(completionItem)

        return results
    }


    override this.ShouldTriggerCompletion(sourceText: SourceText, caretPosition: int, trigger: CompletionTrigger, _: OptionSet) =
        let documentId = workspace.GetDocumentIdInCurrentContext(sourceText.Container)
        let document = workspace.CurrentSolution.GetDocument(documentId)
        
        match FSharpLanguageService.GetOptions(document.Project.Id) with
        | None -> false
        | Some(options) ->
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.Name, options.OtherOptions |> Seq.toList)
            FSharpCompletionProvider.ShouldTriggerCompletionAux(sourceText, caretPosition, trigger.Kind, document.FilePath, defines)
    
    override this.ProvideCompletionsAsync(context: Microsoft.CodeAnalysis.Completion.CompletionContext) =
        let computation = async {
            match FSharpLanguageService.GetOptions(context.Document.Project.Id) with
            | Some(options) ->
                let! sourceText = context.Document.GetTextAsync(context.CancellationToken) |> Async.AwaitTask
                let! textVersion = context.Document.GetTextVersionAsync(context.CancellationToken) |> Async.AwaitTask
                let! results = FSharpCompletionProvider.ProvideCompletionsAsyncAux(sourceText, context.Position, options, context.Document.FilePath, textVersion.GetHashCode())
                context.AddItems(results)
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