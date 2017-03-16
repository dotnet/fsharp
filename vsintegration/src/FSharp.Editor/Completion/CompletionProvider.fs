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
open System.Globalization

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
    static let [<Literal>] IsExtensionMemberPropName = "IsExtensionMember"
    
    let xmlMemberIndexService = serviceProvider.GetService(typeof<IVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)
    static let attributeSuffixLength = "Attribute".Length

    static let noCommitOnSpaceRules = CompletionItemRules.Default.WithCommitCharacterRule(CharacterSetModificationRule.Create(CharacterSetModificationKind.Remove, ' '))
    static let getRules() = if IntelliSenseSettings.ShowAfterCharIsTyped then noCommitOnSpaceRules else CompletionItemRules.Default

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

    static let isLetterChar (cat: UnicodeCategory) =
        // letter-character:
        //   A Unicode character of classes Lu, Ll, Lt, Lm, Lo, or Nl 
        //   A Unicode-escape-sequence representing a character of classes Lu, Ll, Lt, Lm, Lo, or Nl

        match cat with
        | UnicodeCategory.UppercaseLetter
        | UnicodeCategory.LowercaseLetter
        | UnicodeCategory.TitlecaseLetter
        | UnicodeCategory.ModifierLetter
        | UnicodeCategory.OtherLetter
        | UnicodeCategory.LetterNumber -> true
        | _ -> false

    /// Defines a set of helper methods to classify Unicode characters.
    static let isIdentifierStartCharacter(ch: char) =
        // identifier-start-character:
        //   letter-character
        //   _ (the underscore character U+005F)

        if ch < 'a' then // '\u0061'
            if ch < 'A' then // '\u0041'
                false
            else ch <= 'Z'   // '\u005A'
                || ch = '_' // '\u005F'

        elif ch <= 'z' then // '\u007A'
            true
        elif ch <= '\u007F' then // max ASCII
            false
        
        else isLetterChar(CharUnicodeInfo.GetUnicodeCategory(ch))
 
        /// Returns true if the Unicode character can be a part of an identifier.
    static let isIdentifierPartCharacter(ch: char) =
        // identifier-part-character:
        //   letter-character
        //   decimal-digit-character
        //   connecting-character
        //   combining-character
        //   formatting-character

        if ch < 'a' then // '\u0061'
            if ch < 'A' then // '\u0041'
                ch >= '0'  // '\u0030'
                && ch <= '9' // '\u0039'
            else
                ch <= 'Z'  // '\u005A'
                || ch = '_' // '\u005F'
        elif ch <= 'z' then // '\u007A'
            true
        elif ch <= '\u007F' then // max ASCII
            false

        else
            let cat = CharUnicodeInfo.GetUnicodeCategory(ch)
            isLetterChar(cat)
            ||
            match cat with
            | UnicodeCategory.DecimalDigitNumber
            | UnicodeCategory.ConnectorPunctuation
            | UnicodeCategory.NonSpacingMark
            | UnicodeCategory.SpacingCombiningMark -> true
            | _ when int ch > 127 ->
                CharUnicodeInfo.GetUnicodeCategory(ch) = UnicodeCategory.Format
            | _ -> false
    
    static member ShouldTriggerCompletionAux(sourceText: SourceText, caretPosition: int, trigger: CompletionTriggerKind, getInfo: (unit -> DocumentId * string * string list)) =
        // Skip if we are at the start of a document
        if caretPosition = 0 then false
        // Skip if it was triggered by an operation other than insertion
        elif not (trigger = CompletionTriggerKind.Insertion) then false
        // Skip if we are not on a completion trigger
        else
            let triggerPosition = caretPosition - 1
            let c = sourceText.[triggerPosition]
            
            if completionTriggers |> Array.contains c then
                true
            
            // do not trigger completion if it's not single dot, i.e. range expression
            elif triggerPosition > 0 && sourceText.[triggerPosition - 1] = '.' then
                false
            
            // Trigger completion if we are on a valid classification type
            else
                let documentId, filePath, defines = getInfo()
                shouldProvideCompletion(documentId, filePath, defines, sourceText, triggerPosition) &&
                (IntelliSenseSettings.ShowAfterCharIsTyped && 
                 CommonCompletionUtilities.IsStartingNewWord(sourceText, triggerPosition, (fun ch -> isIdentifierStartCharacter ch), (fun ch -> isIdentifierPartCharacter ch)))

    static member ProvideCompletionsAsyncAux(checker: FSharpChecker, sourceText: SourceText, caretPosition: int, options: FSharpProjectOptions, filePath: string, textVersionHash: int) = 
        asyncMaybe {
            let! parseResults, parsedInput, checkFileResults = checker.ParseAndCheckDocument(filePath, textVersionHash, sourceText.ToString(), options, allowStaleResults = true)

            //Logging.Logging.logInfof "AST:\n%+A" parsedInput

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
            
            let sortedDeclItems =
                declarations.Items
                |> Array.sortBy (fun item ->
                    let kindPriority =
                        match item.Kind with
                        | CompletionItemKind.Property -> 0
                        | CompletionItemKind.Field -> 1
                        | CompletionItemKind.Method (isExtension = false) -> 2
                        | CompletionItemKind.Event -> 3
                        | CompletionItemKind.Argument -> 4
                        | CompletionItemKind.Other -> 5
                        | CompletionItemKind.Method (isExtension = true) -> 6
                    kindPriority, not item.IsOwnMember, item.Name.ToLowerInvariant(), item.MinorPriority)

            let maxHints = if mruItems.Values.Count = 0 then 0 else Seq.max mruItems.Values

            sortedDeclItems |> Array.iteri (fun number declarationItem ->
                let glyph = CommonRoslynHelpers.FSharpGlyphToRoslynGlyph (declarationItem.Glyph, declarationItem.Accessibility)
                let name =
                    match entityKind with
                    | Some EntityKind.Attribute when declarationItem.IsAttribute && declarationItem.Name.EndsWith "Attribute"  ->
                        declarationItem.Name.[0..declarationItem.Name.Length - attributeSuffixLength - 1] 
                    | _ -> declarationItem.Name

                let completionItem = CommonCompletionItem.Create(name, glyph = Nullable glyph, rules = getRules()).AddProperty(FullNamePropName, declarationItem.FullName)
                        
                let completionItem =
                    match declarationItem.Kind with
                    | CompletionItemKind.Method (isExtension = true) ->
                          completionItem.AddProperty(IsExtensionMemberPropName, "")
                    | _ -> completionItem
                
                let completionItem =
                    if declarationItem.Name <> declarationItem.NameInCode then
                        completionItem.AddProperty(NameInCodePropName, declarationItem.NameInCode)
                    else completionItem

                let priority = 
                    match mruItems.TryGetValue declarationItem.FullName with
                    | true, hints -> maxHints - hints
                    | _ -> number + maxHints + 1

                let sortText = sprintf "%06d" priority

                //Logging.Logging.logInfof "***** %s => %s" name sortText

                let completionItem = completionItem.WithSortText(sortText)

                declarationItemsCache.Remove(completionItem.DisplayText) |> ignore // clear out stale entries if they exist
                declarationItemsCache.Add(completionItem.DisplayText, declarationItem)
                results.Add(completionItem))
            
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
        match item.Properties.TryGetValue IsExtensionMemberPropName with
        | true, _ -> ()  // do not add extension members to the MRU list
        | _ ->
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