// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

type internal FSharpCompletionProvider
    (
        workspace: Workspace,
        serviceProvider: SVsServiceProvider,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =
    inherit CompletionProvider()

    static let completionTriggers = [| '.' |]
    static let declarationItemsCache = ConditionalWeakTable<string, FSharpDeclarationListItem>()
    static let [<Literal>] NameInCodePropName = "NameInCode"
    static let [<Literal>] FullNamePropName = "FullName"
    
    let xmlMemberIndexService = serviceProvider.GetService(typeof<IVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)
    static let attributeSuffixLength = "Attribute".Length

    static let shouldProvideCompletion (documentId: DocumentId, filePath: string, defines: string list, text: SourceText, position: int) : bool =
        let textLines = text.Lines
        let triggerLine = textLines.GetLineFromPosition position
        let colorizationData = CommonHelpers.getColorizationData(documentId, text, triggerLine.Span, Some filePath, defines, CancellationToken.None)
        colorizationData.Count = 0 || // we should provide completion at the start of empty line, where there are no tokens at all
        colorizationData.Exists (fun classifiedSpan -> 
            classifiedSpan.TextSpan.IntersectsWith position &&
            (
                match classifiedSpan.ClassificationType with
                | ClassificationTypeNames.Comment
                | ClassificationTypeNames.StringLiteral
                | ClassificationTypeNames.ExcludedCode
                | ClassificationTypeNames.NumericLiteral -> false
                | _ -> true // anything else is a valid classification type
            ))

    static let mruItems = Dictionary<(* Item.FullName *) string, (* hints *) int>()

    /// Normalizes hints to monothonically increasing sequence ("name1" => 1, "name2" => 110, "name3" => 25) to ("name1" => 1, "name2" => 3, "name3" => 2)
    static let getNormalizedMruHints () =
        let items = mruItems |> Seq.map (fun (KeyValue(fullName, hints)) -> fullName, hints) |> Seq.sortBy snd |> Seq.toList
        match items with
        | [] -> mruItems
        | _ ->
            // items with no hints are not represented in the dictionary, so we start from 1
            ((1, 1, Dictionary()), items)
            ||> List.fold (fun (lastRealHints, lastNormalizedHints, acc: Dictionary<_,_>) (fullName, hints) ->
                if hints = lastRealHints then
                    acc.[fullName] <- lastNormalizedHints
                    lastRealHints, lastNormalizedHints, acc
                else
                    let lastRealHints = hints
                    let lastNormalizedHints = lastNormalizedHints + 1
                    acc.[fullName] <- lastNormalizedHints
                    lastRealHints, lastNormalizedHints, acc

            ) 
            |> fun (_, _, acc) -> acc
    
    static member ShouldTriggerCompletionAux(sourceText: SourceText, caretPosition: int, trigger: CompletionTriggerKind, getInfo: (unit -> DocumentId * string * string list)) =
        // Skip if we are at the start of a document
        if caretPosition = 0 then false
        // Skip if it was triggered by an operation other than insertion
        elif not (trigger = CompletionTriggerKind.Insertion) then  false
        // Skip if we are not on a completion trigger
        else
            let triggerPosition = caretPosition - 1
            let c = sourceText.[triggerPosition]
            
            if not (completionTriggers |> Array.contains c) then
                false
            
            // do not trigger completion if it's not single dot, i.e. range expression
            elif triggerPosition > 0 && sourceText.[triggerPosition - 1] = '.' then
                false
            
            // Trigger completion if we are on a valid classification type
            else
                let documentId, filePath, defines = getInfo()
                shouldProvideCompletion(documentId, filePath, defines, sourceText, triggerPosition)

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
            
            let results = List<Completion.CompletionItem>()
            let mormalizedMruItems = getNormalizedMruHints()
            
            let longestNameLength = 
                match declarations.Items with
                | [||] -> 0
                | items -> items |> Array.map (fun x -> x.Name.Length) |> Array.max

            for declarationItem in declarations.Items do
                let glyph = CommonRoslynHelpers.FSharpGlyphToRoslynGlyph (declarationItem.Glyph, declarationItem.Accessibility)
                let name =
                    match entityKind with
                    | Some EntityKind.Attribute when declarationItem.IsAttribute && declarationItem.Name.EndsWith "Attribute"  ->
                        declarationItem.Name.[0..declarationItem.Name.Length - attributeSuffixLength - 1] 
                    | _ -> declarationItem.Name

                let completionItem = CommonCompletionItem.Create(name, glyph = Nullable glyph).AddProperty(FullNamePropName, declarationItem.FullName)
                
                let completionItem =
                    if declarationItem.Name <> declarationItem.NameInCode then
                        completionItem.AddProperty(NameInCodePropName, declarationItem.NameInCode)
                    else completionItem

                let sortText =
                    let prefixLength =
                        match declarationItem.Kind with
                        | CompletionItemKind.Property -> 10
                        | CompletionItemKind.Field -> 8
                        | CompletionItemKind.Method -> 6
                        | CompletionItemKind.Event -> 4
                        | CompletionItemKind.Argument -> 2
                        | CompletionItemKind.Other -> 0
                    
                    let prefixLength = if declarationItem.IsOwnMember then prefixLength + 1 else prefixLength
                    //String.replicate prefixLength "a" + name + string declarationItem.MinorPriority
                
                    let hints = 
                        match mormalizedMruItems.TryGetValue declarationItem.FullName with
                        | true, hints ->
                            // for MRU items "foo" => 2, "longLongLong" => 1 to make "foo" appear on top, we 
                            // should prefix it with as many "a" symbols as ("foo" hints + <the longest item name in entire list>.Length - "foo".Length)
                            hints + (longestNameLength - name.Length)
                        | _ -> 0

                    let prefixLength = prefixLength + hints
                    String.replicate prefixLength "a" + name + string declarationItem.MinorPriority

                //Logging.Logging.logInfof "***** %s => %s" name sortText

                let completionItem = completionItem.WithSortText(sortText)

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

        FSharpCompletionProvider.ShouldTriggerCompletionAux(sourceText, caretPosition, trigger.Kind, getInfo)
    
    override this.ProvideCompletionsAsync(context: Completion.CompletionContext) =
        asyncMaybe {
            let document = context.Document
            let! sourceText = context.Document.GetTextAsync(context.CancellationToken)
            let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)
            do! Option.guard (shouldProvideCompletion(document.Id, document.FilePath, defines, sourceText, context.Position))
            let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
            let! textVersion = context.Document.GetTextVersionAsync(context.CancellationToken)
            let! results = FSharpCompletionProvider.ProvideCompletionsAsyncAux(checkerProvider.Checker, sourceText, context.Position, options, document.FilePath, textVersion.GetHashCode())
            context.AddItems(results)
        } |> Async.Ignore |> CommonRoslynHelpers.StartAsyncUnitAsTask context.CancellationToken
        

    override this.GetDescriptionAsync(_: Document, completionItem: Completion.CompletionItem, cancellationToken: CancellationToken): Task<CompletionDescription> =
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
        match item.Properties.TryGetValue FullNamePropName with
        | true, fullName ->
            match mruItems.TryGetValue fullName with
            | true, hints -> mruItems.[fullName] <- hints + 1
            | _ -> mruItems.[fullName] <- 1
        | _ -> ()
        
        let nameInCode =
            match item.Properties.TryGetValue NameInCodePropName with
            | true, x -> x
            | _ -> item.DisplayText
        
        Task.FromResult(CompletionChange.Create(new TextChange(item.Span, nameInCode)))