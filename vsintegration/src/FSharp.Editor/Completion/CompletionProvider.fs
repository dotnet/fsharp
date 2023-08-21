﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Completion

open Microsoft.VisualStudio.FSharp.Editor.Telemetry
open Microsoft.VisualStudio.Shell

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Tokenization
open CancellableTasks

module Logger = Microsoft.VisualStudio.FSharp.Editor.Logger

type internal FSharpCompletionProvider
    (
        workspace: Workspace,
        serviceProvider: SVsServiceProvider,
        assemblyContentProvider: AssemblyContentProvider
    ) =

    inherit FSharpCompletionProviderBase()

    // Save the backing data in a cache, we need to save for at least the length of the completion session
    // See https://github.com/dotnet/fsharp/issues/4714
    static let mutable declarationItems: DeclarationListItem[] = [||]

    [<Literal>]
    static let NameInCodePropName = "NameInCode"

    [<Literal>]
    static let FullNamePropName = "FullName"

    [<Literal>]
    static let IsExtensionMemberPropName = "IsExtensionMember"

    [<Literal>]
    static let NamespaceToOpenPropName = "NamespaceToOpen"

    [<Literal>]
    static let IndexPropName = "Index"

    [<Literal>]
    static let KeywordDescription = "KeywordDescription"

    // These are important.  They make sure we don't _commit_ autocompletion when people don't expect them to.  Some examples:
    //
    // * type Foo() =
    //       member val a = 12 with get, <<---- Don't commit autocomplete!
    //
    // * type MyRecord = { name: <<---- Don't commit autocomplete!
    //
    // * type My< <<---- Don't commit autocomplete!
    //
    // * let myClassInstance = MyClass(Date= <<---- Don't commit autocomplete!
    //
    // * let xs = [1..10] <<---- Don't commit autocomplete! (same for arrays)
    static let noCommitOnSpaceRules =
        let noCommitChars =
            [| ' '; '='; ','; '.'; '<'; '>'; '('; ')'; '!'; ':'; '['; ']'; '|' |]
                .ToImmutableArray()

        CompletionItemRules.Default.WithCommitCharacterRules(
            ImmutableArray.Create(CharacterSetModificationRule.Create(CharacterSetModificationKind.Remove, noCommitChars))
        )

    static let keywordCompletionItems =
        FSharpKeywords.KeywordsWithDescription
        |> List.filter (fun (keyword, _) -> not (PrettyNaming.IsOperatorDisplayName keyword))
        |> List.sortBy (fun (keyword, _) -> keyword)
        |> List.mapi (fun n (keyword, description) ->
            FSharpCommonCompletionItem
                .Create(
                    displayText = keyword,
                    displayTextSuffix = "",
                    rules = noCommitOnSpaceRules,
                    glyph = Nullable Glyph.Keyword,
                    sortText = sprintf "%06d" (1000000 + n)
                )
                .AddProperty(KeywordDescription, description))

    let settings: EditorOptions = workspace.Services.GetService()

    let documentationBuilder =
        XmlDocumentation.CreateDocumentationBuilder(serviceProvider.XMLMemberIndexService)

    static let mruItems = Dictionary< (* Item.FullName *) string (* hints *) , int>()

    static member ShouldTriggerCompletionAux
        (
            sourceText: SourceText,
            caretPosition: int,
            trigger: CompletionTriggerKind,
            getInfo: (unit -> DocumentId * string * string list * string option * bool option),
            intelliSenseOptions: IntelliSenseOptions,
            cancellationToken: CancellationToken
        ) =
        if caretPosition = 0 then
            false
        else
            let triggerPosition = caretPosition - 1
            let triggerChar = sourceText.[triggerPosition]

            if
                trigger = CompletionTriggerKind.Deletion
                && intelliSenseOptions.ShowAfterCharIsDeleted
            then
                Char.IsLetterOrDigit(sourceText.[triggerPosition]) || triggerChar = '.'
            elif not (trigger = CompletionTriggerKind.Insertion) then
                false
            else
            // Do not trigger completion if it's not single dot, i.e. range expression
            if
                not intelliSenseOptions.ShowAfterCharIsTyped
                && triggerPosition > 0
                && sourceText.[triggerPosition - 1] = '.'
            then
                false
            else
                let documentId, filePath, defines, langVersion, strictIndentation = getInfo ()

                CompletionUtils.shouldProvideCompletion (
                    documentId,
                    filePath,
                    defines,
                    langVersion,
                    strictIndentation,
                    sourceText,
                    triggerPosition,
                    cancellationToken
                )
                && (triggerChar = '.'
                    || (intelliSenseOptions.ShowAfterCharIsTyped
                        && CompletionUtils.isStartingNewWord (sourceText, triggerPosition)))

    static member ProvideCompletionsAsyncAux
        (
            document: Document,
            caretPosition: int,
            getAllSymbols: FSharpCheckFileResults -> AssemblySymbol list
        ) =

        cancellableTask {
            let! parseResults, checkFileResults = document.GetFSharpParseAndCheckResultsAsync("ProvideCompletionsAsyncAux")

            let! ct = CancellableTask.getCancellationToken ()

            let! sourceText = document.GetTextAsync(ct)
            let textLines = sourceText.Lines
            let caretLinePos = textLines.GetLinePosition(caretPosition)
            let caretLine = textLines.GetLineFromPosition(caretPosition)
            let fcsCaretLineNumber = Line.fromZ caretLinePos.Line // Roslyn line numbers are zero-based, FSharp.Compiler.Service line numbers are 1-based
            let caretLineColumn = caretLinePos.Character
            let line = caretLine.ToString()
            let partialName = QuickParse.GetPartialLongNameEx(line, caretLineColumn - 1)

            let inline getAllSymbols () =
                [
                    for assemblySymbol in getAllSymbols checkFileResults do
                        if
                            assemblySymbol.FullName.Contains(".")
                            && not (PrettyNaming.IsOperatorDisplayName assemblySymbol.Symbol.DisplayName)
                        then
                            yield assemblySymbol
                ]

            let completionContextPos = Position.fromZ caretLinePos.Line caretLinePos.Character

            let completionContext =
                ParsedInput.TryGetCompletionContext(completionContextPos, parseResults.ParseTree, line)

            let declarations =
                checkFileResults.GetDeclarationListInfo(
                    Some(parseResults),
                    fcsCaretLineNumber,
                    line,
                    partialName,
                    getAllSymbols,
                    (completionContextPos, completionContext)
                )

            let results = List<Completion.CompletionItem>()

            declarationItems <-
                declarations.Items
                |> Array.sortWith (fun x y ->
                    let mutable n = (not x.IsResolved).CompareTo(not y.IsResolved)

                    if n <> 0 then
                        n
                    else
                        n <-
                            (CompletionUtils.getKindPriority x.Kind)
                                .CompareTo(CompletionUtils.getKindPriority y.Kind)

                        if n <> 0 then
                            n
                        else
                            n <- (not x.IsOwnMember).CompareTo(not y.IsOwnMember)

                            if n <> 0 then
                                n
                            else
                                n <- String.Compare(x.NameInList, y.NameInList, StringComparison.OrdinalIgnoreCase)

                                if n <> 0 then
                                    n
                                else
                                    x.MinorPriority.CompareTo(y.MinorPriority))

            let maxHints =
                if mruItems.Values.Count = 0 then
                    0
                else
                    Seq.max mruItems.Values

            declarationItems
            |> Array.iteri (fun number declarationItem ->
                let glyph =
                    Tokenizer.FSharpGlyphToRoslynGlyph(declarationItem.Glyph, declarationItem.Accessibility)

                let namespaceName =
                    match declarationItem.NamespaceToOpen with
                    | Some namespaceToOpen -> namespaceToOpen
                    | _ -> null // Icky, but this is how roslyn handles it

                let filterText =
                    match declarationItem.NamespaceToOpen, declarationItem.NameInList.Split '.' with
                    // There is no namespace to open and the item name does not contain dots, so we don't need to pass special FilterText to Roslyn.
                    | None, [| _ |] -> null
                    // Either we have a namespace to open ("DateTime (open System)") or item name contains dots ("Array.map"), or both.
                    // We are passing last part of long ident as FilterText.
                    | _, idents -> Array.last idents

                let completionItem =
                    FSharpCommonCompletionItem
                        .Create(
                            declarationItem.NameInList,
                            null,
                            rules = noCommitOnSpaceRules,
                            glyph = Nullable glyph,
                            filterText = filterText,
                            inlineDescription = namespaceName
                        )
                        .AddProperty(FullNamePropName, declarationItem.FullName)

                let completionItem =
                    match declarationItem.Kind with
                    | CompletionItemKind.Method(isExtension = true) -> completionItem.AddProperty(IsExtensionMemberPropName, "")
                    | _ -> completionItem

                let completionItem =
                    if declarationItem.NameInList <> declarationItem.NameInCode then
                        completionItem.AddProperty(NameInCodePropName, declarationItem.NameInCode)
                    else
                        completionItem

                let completionItem =
                    match declarationItem.NamespaceToOpen with
                    | Some ns -> completionItem.AddProperty(NamespaceToOpenPropName, ns)
                    | None -> completionItem

                let completionItem = completionItem.AddProperty(IndexPropName, string number)

                let priority =
                    match mruItems.TryGetValue declarationItem.FullName with
                    | true, hints -> maxHints - hints
                    | _ -> number + maxHints + 1

                let sortText = priority.ToString("D6")
                let completionItem = completionItem.WithSortText(sortText)
                results.Add(completionItem))

            if
                results.Count > 0
                && not declarations.IsForType
                && not declarations.IsError
                && List.isEmpty partialName.QualifyingIdents
            then
                match completionContext with
                | None -> results.AddRange(keywordCompletionItems)
                | _ -> ()

            return results
        }

    override _.ShouldTriggerCompletionImpl(sourceText: SourceText, caretPosition: int, trigger: CompletionTrigger) =
        use _logBlock = Logger.LogBlock LogEditorFunctionId.Completion_ShouldTrigger

        let getInfo () =
            let documentId = workspace.GetDocumentIdInCurrentContext(sourceText.Container)
            let document = workspace.CurrentSolution.GetDocument(documentId)

            let defines, langVersion, strictIndentation = document.GetFsharpParsingOptions()

            (documentId, document.FilePath, defines, Some langVersion, strictIndentation)

        FSharpCompletionProvider.ShouldTriggerCompletionAux(
            sourceText,
            caretPosition,
            trigger.Kind,
            getInfo,
            settings.IntelliSense,
            CancellationToken.None
        )

    override _.ProvideCompletionsAsync(context: Completion.CompletionContext) =
        cancellableTask {
            use _logBlock =
                Logger.LogBlockMessage context.Document.Name LogEditorFunctionId.Completion_ProvideCompletionsAsync

            let! ct = CancellableTask.getCancellationToken ()

            let document = context.Document

            let eventProps: (string * obj) array =
                [|
                    "context.document.project.id", document.Project.Id.Id.ToString()
                    "context.document.id", document.Id.Id.ToString()
                |]

            use _eventDuration =
                TelemetryReporter.ReportSingleEventWithDuration(TelemetryEvents.ProvideCompletions, eventProps)

            let! sourceText = context.Document.GetTextAsync(ct)

            let defines, langVersion, strictIndentation = document.GetFsharpParsingOptions()

            let shouldProvideCompetion =
                CompletionUtils.shouldProvideCompletion (
                    document.Id,
                    document.FilePath,
                    defines,
                    Some langVersion,
                    strictIndentation,
                    sourceText,
                    context.Position,
                    ct
                )

            if shouldProvideCompetion then
                let getAllSymbols (fileCheckResults: FSharpCheckFileResults) =
                    if settings.IntelliSense.IncludeSymbolsFromUnopenedNamespacesOrModules then
                        assemblyContentProvider.GetAllEntitiesInProjectAndReferencedAssemblies(fileCheckResults)
                    else
                        []

                let! results = FSharpCompletionProvider.ProvideCompletionsAsyncAux(context.Document, context.Position, getAllSymbols)

                context.AddItems results

        }
        |> CancellableTask.startAsTask context.CancellationToken

    override _.GetDescriptionAsync
        (
            document: Document,
            completionItem: Completion.CompletionItem,
            cancellationToken: CancellationToken
        ) : Task<CompletionDescription> =

        match completionItem.Properties.TryGetValue IndexPropName with
        | true, completionItemIndexStr when int completionItemIndexStr >= declarationItems.Length ->
            Task.FromResult CompletionDescription.Empty
        | true, completionItemIndexStr ->
            // TODO: Not entirely sure why do we use tasks here, since everything here is synchronous.
            cancellableTask {
                use _logBlock =
                    Logger.LogBlockMessage document.Name LogEditorFunctionId.Completion_GetDescriptionAsync

                let completionItemIndex = int completionItemIndexStr

                let declarationItem = declarationItems.[completionItemIndex]
                let description = declarationItem.Description
                let documentation = List()
                let collector = RoslynHelpers.CollectTaggedText documentation
                // mix main description and xmldoc by using one collector
                XmlDocumentation.BuildDataTipText(documentationBuilder, collector, collector, collector, collector, collector, description)

                return CompletionDescription.Create(documentation.ToImmutableArray())
            }
            |> CancellableTask.start cancellationToken

        | _ ->
            match completionItem.Properties.TryGetValue KeywordDescription with
            | true, keywordDescription -> Task.FromResult(CompletionDescription.FromText(keywordDescription))
            | false, _ -> Task.FromResult(CompletionDescription.Empty)

    override _.GetChangeAsync(document, item, _, cancellationToken) : Task<CompletionChange> =
        cancellableTask {
            use _logBlock =
                Logger.LogBlockMessage document.Name LogEditorFunctionId.Completion_GetChangeAsync

            let fullName =
                match item.Properties.TryGetValue FullNamePropName with
                | true, x -> Some x
                | _ -> None

            // do not add extension members and unresolved symbols to the MRU list
            if
                not (item.Properties.ContainsKey NamespaceToOpenPropName)
                && not (item.Properties.ContainsKey IsExtensionMemberPropName)
            then
                match fullName with
                | Some fullName ->
                    match mruItems.TryGetValue fullName with
                    | true, hints -> mruItems.[fullName] <- hints + 1
                    | _ -> mruItems.[fullName] <- 1
                | _ -> ()

            let nameInCode =
                match item.Properties.TryGetValue NameInCodePropName with
                | true, x -> x
                | _ -> item.DisplayText

            match item.Properties.TryGetValue NamespaceToOpenPropName with
            | false, _ -> return CompletionChange.Create(TextChange(item.Span, nameInCode))
            | true, ns ->
                let! sourceText = document.GetTextAsync(cancellationToken)

                let textWithItemCommitted =
                    sourceText.WithChanges(TextChange(item.Span, nameInCode))

                let line = sourceText.Lines.GetLineFromPosition(item.Span.Start)

                let! parseResults = document.GetFSharpParseResultsAsync(nameof (FSharpCompletionProvider))

                let fullNameIdents =
                    fullName |> Option.map (fun x -> x.Split '.') |> Option.defaultValue [||]

                let insertionPoint =
                    if settings.CodeFixes.AlwaysPlaceOpensAtTopLevel then
                        OpenStatementInsertionPoint.TopLevel
                    else
                        OpenStatementInsertionPoint.Nearest

                let ctx =
                    ParsedInput.FindNearestPointToInsertOpenDeclaration line.LineNumber parseResults.ParseTree fullNameIdents insertionPoint

                let finalSourceText, changedSpanStartPos =
                    OpenDeclarationHelper.insertOpenDeclaration textWithItemCommitted ctx ns

                let fullChangingSpan = TextSpan.FromBounds(changedSpanStartPos, item.Span.End)

                let changedSpan =
                    TextSpan.FromBounds(changedSpanStartPos, item.Span.End + (finalSourceText.Length - sourceText.Length))

                let changedText = finalSourceText.ToString(changedSpan)

                return
                    CompletionChange
                        .Create(TextChange(fullChangingSpan, changedText))
                        .WithNewPosition(Nullable(changedSpan.End))
        }
        |> CancellableTask.start cancellationToken
