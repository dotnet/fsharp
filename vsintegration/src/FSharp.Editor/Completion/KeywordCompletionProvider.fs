// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.Text

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SourceCodeServices.ItemDescriptionIcons

type internal FSharpKeywordCompletionProvider
    (
        workspace: Workspace,
        projectInfoManager: ProjectInfoManager
    ) =
    
    inherit CompletionProvider()

    let completionItems =
        Lexhelp.Keywords.keywordsWithDescription
        |> List.map (fun (keyword, description) -> 
             CommonCompletionItem.Create(keyword, Nullable(Glyph.Keyword)).AddProperty("description", description))

    override this.ShouldTriggerCompletion(sourceText: SourceText, caretPosition: int, trigger: CompletionTrigger, _: OptionSet) =
        let getInfo() = 
            let documentId = workspace.GetDocumentIdInCurrentContext(sourceText.Container)
            let document = workspace.CurrentSolution.GetDocument(documentId)
            let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)  
            (documentId, document.FilePath, defines)

        CompletionUtils.shouldTriggerCompletion(sourceText, caretPosition, trigger.Kind, getInfo)
    
    override this.ProvideCompletionsAsync(context: Microsoft.CodeAnalysis.Completion.CompletionContext) =
        context.AddItems(completionItems)
        Task.CompletedTask

    override this.GetDescriptionAsync(_: Document, completionItem: CompletionItem, _: CancellationToken): Task<CompletionDescription> =
        Task.FromResult(CompletionDescription.FromText(completionItem.Properties.["description"]))
