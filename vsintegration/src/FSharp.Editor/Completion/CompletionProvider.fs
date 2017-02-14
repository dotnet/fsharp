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
open Microsoft.CodeAnalysis.Host
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
open Microsoft.FSharp.Compiler.SourceCodeServices.ItemDescriptionIcons

open Microsoft.VisualStudio.FSharp.Editor.Logging
open System.Diagnostics

type internal FSharpCompletionProvider
    (
        workspace: Workspace,
        serviceProvider: SVsServiceProvider,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =
    inherit CompletionProvider()

    static let declarationItemsCache = ConditionalWeakTable<string, FSharpDeclarationListItem>()
    static let [<Literal>] NameInCodePropName = "NameInCode"
    
    let xmlMemberIndexService = serviceProvider.GetService(typeof<IVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)
    static let attributeSuffixLength = "Attribute".Length

    static member ProvideCompletionsAsyncAux(checker: FSharpChecker, sourceText: SourceText, caretPosition: int, options: FSharpProjectOptions, filePath: string, textVersionHash: int) = 
        asyncMaybe {
            let! parseResults, parsedInput, checkFileResults = checker.ParseAndCheckDocument(filePath, textVersionHash, sourceText.ToString(), options, allowStaleResults = true)

            let textLines = sourceText.Lines
            let caretLinePos = textLines.GetLinePosition(caretPosition)
            let entityKind = UntypedParseImpl.GetEntityKind(Pos.fromZ caretLinePos.Line caretLinePos.Character, parsedInput)
            
            let caretLine = textLines.GetLineFromPosition(caretPosition)
            let fcsCaretLineNumber = Line.fromZ caretLinePos.Line  // Roslyn line numbers are zero-based, FSharp.Compiler.Service line numbers are 1-based
            let caretLineColumn = caretLinePos.Character
            
            let qualifyingNames, partialName = QuickParse.GetPartialLongNameEx(caretLine.ToString(), caretLineColumn - 1) 
            
            let! declarations =
                checkFileResults.GetDeclarationListInfo(Some(parseResults), fcsCaretLineNumber, caretLineColumn, caretLine.ToString(), qualifyingNames, partialName) |> liftAsync
            
            let results = List<CompletionItem>()
            
            for declarationItem in declarations.Items do
                let glyph = CommonRoslynHelpers.FSharpGlyphToRoslynGlyph declarationItem.GlyphMajor
                let name =
                    match entityKind with
                    | Some EntityKind.Attribute when declarationItem.IsAttribute && declarationItem.Name.EndsWith "Attribute"  ->
                        declarationItem.Name.[0..declarationItem.Name.Length - attributeSuffixLength - 1] 
                    | _ -> declarationItem.Name
                let completionItem = CommonCompletionItem.Create(name, glyph = Nullable glyph)
                
                let completionItem =
                    if declarationItem.Name <> declarationItem.NameInCode then
                        completionItem.AddProperty(NameInCodePropName, declarationItem.NameInCode)
                    else completionItem

                declarationItemsCache.Remove(completionItem.DisplayText) |> ignore // clear out stale entries if they exist
                declarationItemsCache.Add(completionItem.DisplayText, declarationItem)
                results.Add(completionItem)
            
            return results
        }

    override this.ShouldTriggerCompletion(sourceText: SourceText, caretPosition: int, trigger: CompletionTrigger, _: OptionSet) =
        let getInfo() = 
            let documentId = workspace.GetDocumentIdInCurrentContext(sourceText.Container)
            let document = workspace.CurrentSolution.GetDocument(documentId)
            let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)  
            (documentId, document.FilePath, defines)

        CompletionUtils.shouldTriggerCompletion(sourceText, caretPosition, trigger.Kind, getInfo)
    
    override this.ProvideCompletionsAsync(context: Microsoft.CodeAnalysis.Completion.CompletionContext) =
        asyncMaybe {
            let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(context.Document)
            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)
            let! textVersion = context.Document.GetTextVersionAsync(context.CancellationToken)
            let! results = FSharpCompletionProvider.ProvideCompletionsAsyncAux(checkerProvider.Checker, sourceText, context.Position, options, context.Document.FilePath, textVersion.GetHashCode())
            context.AddItems(results)
        } |> Async.Ignore |> CommonRoslynHelpers.StartAsyncUnitAsTask context.CancellationToken
        

    override this.GetDescriptionAsync(_: Document, completionItem: CompletionItem, cancellationToken: CancellationToken): Task<CompletionDescription> =
        async {
            let exists, declarationItem = declarationItemsCache.TryGetValue(completionItem.DisplayText)
            if exists then
                let! description = declarationItem.StructuredDescriptionTextAsync
                let documentation = List()
                let collector = CommonRoslynHelpers.CollectTaggedText documentation
                // mix main description and xmldoc by using one collector
                XmlDocumentation.BuildDataTipText(documentationBuilder, collector, collector, description) 
                return CompletionDescription.Create(documentation.ToImmutableArray())
            else
                return CompletionDescription.Empty
        } |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken

    override this.GetChangeAsync(_, item, _, _) : Task<CompletionChange> =
        let nameInCode =
            match item.Properties.TryGetValue NameInCodePropName with
            | true, x -> x
            | _ -> item.DisplayText

        Task.FromResult(CompletionChange.Create(new TextChange(item.Span, nameInCode)))