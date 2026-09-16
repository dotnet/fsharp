// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.ComponentModel.Composition
open System.IO
open System.Diagnostics
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Navigation
open Microsoft.CodeAnalysis.Text
open Microsoft.ServiceHub.Framework
open Microsoft.VisualStudio.Copilot
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.ServiceBroker
open Microsoft.VisualStudio.Text.PatternMatching

open FSharp.Compiler.EditorServices
open CancellableTasks

/// Where a declaration sits relative to what the user is working on, best first. The picker merges
/// answers from every provider and ranks them by the priority each one reports, so a match has to say
/// where it sits rather than rely on its position.
[<RequireQualifiedAccess>]
type internal DocumentFocus =
    /// Declared around the caret or selection of the focused file.
    | Selected
    | Focused
    | Open
    | Elsewhere

/// Solution-wide lookup of F# declarations behind the Copilot chat "#" mention picker.
/// Kept apart from the brokered service so it can be exercised without a Visual Studio workspace.
module internal CopilotSymbolQuery =

    /// Also the point at which the search stops parsing files nobody has opened, so it bounds the cold
    /// scan as much as the answer.
    [<Literal>]
    let private MaxMentions = 20

    /// Overloads and partial definitions share one fully qualified name; a handful of them is plenty of context.
    [<Literal>]
    let private MaxDeclarations = 4

    [<Literal>]
    let private UserOpName = "CopilotSymbolContext"

    /// How long a query keeps parsing files nobody has opened yet. Copilot cancels on its own schedule
    /// and takes no partial results, so an answer from what is already parsed beats a complete answer.
    [<Literal>]
    let private ColdSearchBudgetMs = 1500L

    /// A shorter text matches too much of the solution to be worth parsing it for, so it is looked up in the
    /// files the user has open.
    [<Literal>]
    let private MinSolutionWideSearchLength = 3

    let private parallelism = max 1 (Environment.ProcessorCount - 1)

    /// A bare "#" asks with no text at all, and is answered with every declaration of the open files.
    let private matcherFor (cache: FSharpNavigableItemsCache) (searchText: string) =
        match searchText with
        | "" -> fun (_: NavigableItem) -> ValueSome(PatternMatch(PatternMatchKind.Exact, false, false))
        | searchText -> cache.CreateMatcherFor searchText

    let private fsharpDocuments (solution: Solution) =
        solution.Projects |> Seq.where _.IsFSharp |> Seq.collect _.Documents

    let private isFocused (focus: EditorFocus) (document: Document) =
        String.Equals(focus.FilePath, document.FilePath, StringComparison.OrdinalIgnoreCase)

    /// Focus counts only while the file is still open: the tracker is not told when its tab closes.
    let private focusOf
        (focus: EditorFocus voption)
        (openIds: HashSet<DocumentId>)
        (isSelected: NavigableItem -> bool)
        (document: Document)
        (item: NavigableItem)
        =
        if not (openIds.Contains document.Id) then
            DocumentFocus.Elsewhere
        else
            match focus with
            | ValueSome focus when isFocused focus document ->
                if isSelected item then
                    DocumentFocus.Selected
                else
                    DocumentFocus.Focused
            | _ -> DocumentFocus.Open

    let private rankOf focus =
        match focus with
        | DocumentFocus.Selected -> 0
        | DocumentFocus.Focused -> 1
        | DocumentFocus.Open -> 2
        | DocumentFocus.Elsewhere -> 3

    /// The source of a document and the outlining of its declarations.
    [<Struct>]
    type private Outline =
        {
            Text: SourceText
            Lines: string array
            Scopes: Structure.ScopeRange array
        }

    let private outlineOf (document: Document) =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            let! sourceText = document.GetTextAsync ct
            let! parseResults = document.GetFSharpParseResultsAsync UserOpName

            let sourceLines =
                Array.init sourceText.Lines.Count (fun line -> sourceText.Lines[line].ToString())

            return
                {
                    Text = sourceText
                    Lines = sourceLines
                    Scopes = Structure.getOutliningRanges sourceLines parseResults.ParseTree |> Seq.toArray
                }
        }

    let private notSelected (_: NavigableItem) = false

    /// Whether a declaration of the focused document spans a line the caret or selection is on - the whole
    /// declaration, so the caret in a member's body selects the member, its type and the modules around them.
    let private selectionIn (focus: EditorFocus) (document: Document) =
        cancellableTask {
            let! outline = outlineOf document

            return
                fun (item: NavigableItem) ->
                    let struct (firstLine, lastLine) =
                        CopilotSymbolSnippets.declarationLines outline.Lines outline.Scopes item

                    firstLine <= focus.LastLine && focus.FirstLine <= lastLine
        }

    /// The documents in the order a query visits them: the ones the user has open, the ones already
    /// parsed into the cache, and the ones that would have to be parsed to answer.
    let private tiers (cache: FSharpNavigableItemsCache) (openIds: HashSet<DocumentId>) (solution: Solution) =
        let opened = ResizeArray()
        let cached = ResizeArray()
        let cold = ResizeArray()

        for document in fsharpDocuments solution do
            if openIds.Contains document.Id then
                opened.Add document
            else
                match cache.TryGetCachedNavigableItems document.Id with
                | ValueSome items -> cached.Add(struct (document, items))
                | ValueNone -> cold.Add document

        struct (opened, cached, cold)

    /// Visits the documents tier by tier until `enough` answers, parsing the cold ones for at most
    /// `budgetMs`. The budget stops handing out documents rather than cancelling a parse under way:
    /// the first query of a session has to survive its first parse to answer at all.
    let private scanTiers
        (cache: FSharpNavigableItemsCache)
        (openIds: HashSet<DocumentId>)
        (solution: Solution)
        (budgetMs: int64)
        (enough: unit -> bool)
        (collect: Document -> NavigableItem array -> unit)
        =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            let struct (opened, cached, cold) = tiers cache openIds solution

            let parseAndCollect (document: Document) =
                cancellableTask {
                    let! items = cache.GetNavigableItems document
                    collect document items
                }

            do! opened |> CancellableTask.forEachThrottled parallelism parseAndCollect

            if not (enough ()) then
                for struct (document, items) in cached do
                    ct.ThrowIfCancellationRequested()
                    collect document items

            if not (enough ()) then
                let budget = Stopwatch.StartNew()

                let scan (document: Document) =
                    cancellableTask {
                        if budget.ElapsedMilliseconds < budgetMs && not (enough ()) then
                            do! parseAndCollect document
                    }

                do! cold |> CancellableTask.forEachThrottled parallelism scan
        }

    [<Struct>]
    type private Hit =
        {
            Kind: PatternMatchKind
            Item: NavigableItem
            Document: Document
            Name: string
        }

    let private compareRanked (struct (rank1: int, hit1: Hit)) (struct (rank2: int, hit2: Hit)) =
        match compare rank1 rank2 with
        | 0 ->
            match compare (int hit1.Kind) (int hit2.Kind) with
            | 0 -> compare hit1.Item.Name.Length hit2.Item.Name.Length
            | order -> order
        | order -> order

    /// Declarations whose fully qualified name matches each search text, best match first, one entry
    /// per name. Every document is visited once for all of the texts.
    let search
        (cache: FSharpNavigableItemsCache)
        (openDocumentIds: DocumentId seq)
        (focus: EditorFocus voption)
        (solution: Solution)
        (searchTexts: string[])
        =
        cancellableTask {
            let openIds = HashSet openDocumentIds

            let queries =
                searchTexts
                |> Array.map (fun text ->
                    struct {|
                        TryMatch = matcherFor cache text
                        SolutionWide = text.Length >= MinSolutionWideSearchLength
                        Hits = ResizeArray<Hit>()
                        Names = HashSet StringComparer.Ordinal
                    |})

            let solutionWide = queries |> Array.filter _.SolutionWide
            let gate = obj ()
            let mutable focusedDocument = ValueNone

            // Matching runs outside the lock; a document's hits join the shared lists in one step.
            let collect (document: Document) (items: NavigableItem array) =
                let queries =
                    if openIds.Contains document.Id then
                        queries
                    else
                        solutionWide

                let matched =
                    queries
                    |> Array.map (fun query ->
                        let hits = ResizeArray()

                        for item in items do
                            match query.TryMatch item with
                            | ValueSome patternMatch ->
                                hits.Add
                                    {
                                        Kind = patternMatch.Kind
                                        Item = item
                                        Document = document
                                        Name = CopilotSymbolMapping.fullyQualifiedName item
                                    }
                            | ValueNone -> ()

                        struct (query, hits))

                lock gate (fun () ->
                    for struct (query, hits) in matched do
                        query.Hits.AddRange hits

                        if query.SolutionWide then
                            for hit in hits do
                                if query.Names.Count < MaxMentions then
                                    query.Names.Add hit.Name |> ignore

                    match focus with
                    | ValueSome focus when
                        isFocused focus document
                        && matched |> Array.exists (fun (struct (_, hits)) -> hits.Count > 0)
                        ->
                        focusedDocument <- ValueSome document
                    | _ -> ())

            let enough () =
                lock gate (fun () -> solutionWide |> Array.forall (fun query -> query.Names.Count >= MaxMentions))

            do! scanTiers cache openIds solution ColdSearchBudgetMs enough collect

            let! isSelected =
                match focus, focusedDocument with
                | ValueSome focus, ValueSome document -> selectionIn focus document
                | _ -> CancellableTask.singleton notSelected

            return
                queries
                |> Array.map (fun query ->
                    let seen = HashSet StringComparer.Ordinal
                    let mentions = ResizeArray MaxMentions

                    let ranked =
                        query.Hits
                        |> Seq.map (fun hit ->
                            let focus = focusOf focus openIds isSelected hit.Document hit.Item
                            struct (rankOf focus * 2 + (if hit.Document.IsFSharpSignatureFile then 1 else 0), hit), focus)
                        |> Seq.sortWith (fun (ranked1, _) (ranked2, _) -> compareRanked ranked1 ranked2)

                    for struct (_, hit), focus in ranked do
                        if mentions.Count < MaxMentions && seen.Add hit.Name then
                            mentions.Add(struct (hit.Item, hit.Document, focus))

                    mentions.ToArray())
        }

    /// Declarations carrying exactly this fully qualified name. Signature files answer only when no
    /// implementation declares the name, so the search stops once an implementation has answered.
    let declarationsOf
        (cache: FSharpNavigableItemsCache)
        (openDocumentIds: DocumentId seq)
        (solution: Solution)
        (fullyQualifiedName: string)
        =
        cancellableTask {
            let hits = ResizeArray()
            let mutable declaredInImplementation = false

            let collect (document: Document) (items: NavigableItem array) =
                let declared =
                    items
                    |> Array.filter (CopilotSymbolMapping.hasFullyQualifiedName fullyQualifiedName)

                if declared.Length > 0 then
                    lock hits (fun () ->
                        for item in declared do
                            hits.Add(struct (item, document))

                        if not document.IsFSharpSignatureFile then
                            declaredInImplementation <- true)

            let enough () =
                lock hits (fun () -> declaredInImplementation)

            do! scanTiers cache (HashSet openDocumentIds) solution Int64.MaxValue enough collect

            let implementations =
                hits
                |> Seq.filter (fun (struct (_, document: Document)) -> not document.IsFSharpSignatureFile)
                |> Seq.truncate MaxDeclarations
                |> Seq.toArray

            return
                match implementations with
                | [||] -> hits |> Seq.truncate MaxDeclarations |> Seq.toArray
                | implementations -> implementations
        }

    /// The source of the whole declaration `item` names, together with the span it occupies.
    let private snippetOf (outline: Outline) (item: NavigableItem) =
        let struct (firstLine, lastLine) =
            CopilotSymbolSnippets.definitionLines outline.Lines outline.Scopes item

        let text = outline.Text
        let firstLine = max 1 firstLine
        let lastLine = min text.Lines.Count lastLine

        let span =
            TextSpan.FromBounds(text.Lines[firstLine - 1].Start, text.Lines[lastLine - 1].End)

        struct (text.ToString span, span)

    let symbolContext
        (cache: FSharpNavigableItemsCache)
        (openDocumentIds: DocumentId seq)
        (solution: Solution)
        (fullyQualifiedName: string)
        =
        cancellableTask {
            let! declarations = declarationsOf cache openDocumentIds solution fullyQualifiedName

            match Array.tryHeadV declarations with
            | ValueNone -> return ValueNone
            | ValueSome(struct (first, _)) ->
                let snippets = ResizeArray()
                let locations = ResizeArray()
                let mutable outlined: struct (DocumentId * Outline) voption = ValueNone

                // Overloads and partial definitions of one name mostly share a file: outline it once.
                for struct (item, document: Document) in declarations do
                    let! outline =
                        match outlined with
                        | ValueSome(struct (id, outline)) when id = document.Id -> CancellableTask.singleton outline
                        | _ -> outlineOf document

                    outlined <- ValueSome(struct (document.Id, outline))
                    let struct (text, span) = snippetOf outline item
                    snippets.Add text
                    locations.Add(SnippetLocation(document.FilePath, CopilotSpan(span.Start, span.Length)))

                return
                    ValueSome(
                        CopilotSymbolContext(
                            fullyQualifiedName,
                            first.Name,
                            String.Join(Environment.NewLine + Environment.NewLine, snippets),
                            CopilotSymbolMapping.symbolContextType first.Kind,
                            locations.ToArray()
                        )
                    )
        }

/// Offers F# declarations to Copilot chat, which merges them into the picker shown for "#".
/// Copilot's own symbol provider reads the Roslyn compilation, which F# projects do not have.
[<ExportBrokeredService(FSharpConstants.copilotSymbolProviderName,
                        CopilotDescriptors.CurrentContextProviderVersion,
                        [| typeof<ICopilotMentionQueryable>; typeof<ICopilotMentionBatchQueryable> |],
                        Audience = (ServiceAudience.PublicSdk ||| ServiceAudience.Local))>]
type internal FSharpCopilotContextProvider
    [<ImportingConstructor>]
    (
        cache: FSharpNavigableItemsCache,
        activeDocument: FSharpActiveDocumentTracker,
        [<Import(AllowDefault = true)>] workspace: VisualStudioWorkspace | null
    ) =

    static let moniker =
        ServiceMoniker(FSharpConstants.copilotSymbolProviderName, Version CopilotDescriptors.CurrentContextProviderVersion)

    static let descriptor =
        CopilotContextDescriptor(
            CopilotSymbolMapping.SymbolMember,
            "An F# type, module, member or value declared in the current solution.",
            CopilotDefaultTypes.SymbolContextName,
            [|
                CopilotInputDescriptor(
                    CopilotSymbolMapping.FullyQualifiedNameInput,
                    "Fully qualified name of the F# declaration.",
                    CopilotDefaultTypes.StringName,
                    IsRequired = true
                )
            |]
        )

    static let members = [| descriptor |] :> IReadOnlyList<CopilotContextDescriptor>

    static let memberNames = [| CopilotSymbolMapping.SymbolMember |] :> IReadOnlyList<string>

    static let noMentions =
        Array.empty<CopilotQueriedMention> :> IReadOnlyCollection<CopilotQueriedMention>

    let priorityOf focus =
        match focus with
        | DocumentFocus.Selected -> CopilotQueriedMentionPriority.Selection
        | DocumentFocus.Focused -> CopilotQueriedMentionPriority.High
        | DocumentFocus.Open -> CopilotQueriedMentionPriority.Low
        | DocumentFocus.Elsewhere -> CopilotQueriedMentionPriority.None

    let mentionFor (item: NavigableItem) (document: Document) focus =
        let inputs = Dictionary<string, CopilotValue>(1, StringComparer.Ordinal)

        inputs[CopilotSymbolMapping.FullyQualifiedNameInput] <-
            CopilotValue(CopilotDefaultTypes.StringName, CopilotSymbolMapping.fullyQualifiedName item)

        let fileName = Path.GetFileName document.FilePath

        let tooltip =
            String.Format(
                SR.CopilotSymbolTooltip(),
                CopilotSymbolMapping.symbolContextType item.Kind,
                fileName,
                CopilotSymbolMapping.tooltipName item
            )

        CopilotQueriedContextMention(
            moniker,
            descriptor,
            inputs,
            item.Name,
            Description = fileName,
            Tooltip = tooltip,
            Icon = Nullable(CopilotSymbolMapping.icon item.Kind),
            IsNavigable = true,
            Priority = priorityOf focus
        )
        :> CopilotQueriedMention

    /// One pass over the solution for the whole batch: Copilot's picker asks for several texts at once
    /// and each of them would otherwise walk the same documents.
    let mentionsFor (searchTexts: string voption[]) =
        cancellableTask {
            match workspace, searchTexts |> Seq.chooseV id |> Seq.distinct |> Seq.toArray with
            | null, _
            | _, [||] -> return Array.create searchTexts.Length noMentions
            | workspace, distinct ->
                let! hits =
                    CopilotSymbolQuery.search cache (workspace.GetOpenDocumentIds()) activeDocument.Focus workspace.CurrentSolution distinct

                let byText = Dictionary(StringComparer.Ordinal)

                for index in 0 .. distinct.Length - 1 do
                    byText[distinct[index]] <-
                        hits[index]
                        |> Array.map (fun (struct (item, document, focus)) -> mentionFor item document focus)
                        :> IReadOnlyCollection<CopilotQueriedMention>

                return
                    searchTexts
                    |> Array.map (function
                        | ValueSome text -> byText[text]
                        | ValueNone -> noMentions)
        }

    let fullyQualifiedNameOf (inputs: IReadOnlyDictionary<string, CopilotValue> | null) =
        match inputs with
        | null -> ValueNone
        | inputs ->
            match inputs.TryGetValue CopilotSymbolMapping.FullyQualifiedNameInput with
            | true, value ->
                match value.TryGetValue<string>() with
                | true, name when not (String.IsNullOrWhiteSpace name) -> ValueSome name
                | _ -> ValueNone
            | _ -> ValueNone

    interface IExportedBrokeredService with
        member _.Descriptor = CopilotDescriptors.CreateContextProviderDescriptor moniker

        member _.InitializeAsync _cancellationToken = Task.CompletedTask

    interface ICopilotContextReducer with
        member _.ReduceAsync(context, _reduction, _counter, _cancellationToken) = Task.FromResult context

    interface ICopilotContextProvider with
        member _.GetMembersAsync _cancellationToken =
            ValueTask<IReadOnlyList<CopilotContextDescriptor>> members

        member _.GetMembersAsync(_requestId, _cancellationToken) = Task.FromResult memberNames

        member _.StoreAsync(_requestId, _cancellationToken) = ValueTask()

        member _.ReleaseAsync(_requestId, _cancellationToken) = ValueTask()

        member _.GetContextAsync(requestId, memberName, inputs, cancellationToken) : Task<CopilotContext> =
            match workspace, fullyQualifiedNameOf inputs with
            | null, _
            | _, ValueNone -> Task.FromResult null
            | workspace, ValueSome fullyQualifiedName when
                String.Equals(memberName, CopilotSymbolMapping.SymbolMember, StringComparison.Ordinal)
                ->
                cancellableTask {
                    let! symbol =
                        CopilotSymbolQuery.symbolContext cache (workspace.GetOpenDocumentIds()) workspace.CurrentSolution fullyQualifiedName

                    match symbol with
                    | ValueNone -> return null
                    | ValueSome symbol -> return CopilotContext(moniker, descriptor, requestId, symbol, CanReduce = false)
                }
                |> CancellableTask.start cancellationToken
            | _ -> Task.FromResult null

    interface ICopilotMentionQueryable with
        member _.QueryMentionAsync(query, cancellationToken) : Task<IReadOnlyCollection<CopilotQueriedMention>> =
            mentionsFor [| CopilotSymbolMapping.searchTextOf query |]
            |> CancellableTask.map Array.head
            |> CancellableTask.start cancellationToken

        member _.NavigateToMentionableAsync(mention, cancellationToken) : Task<bool> =
            match workspace, fullyQualifiedNameOf mention.Inputs with
            | null, _
            | _, ValueNone -> Task.FromResult false
            | workspace, ValueSome fullyQualifiedName ->
                cancellableTask {
                    let! ct = CancellableTask.getCancellationToken ()
                    let solution = workspace.CurrentSolution

                    let! declarations = CopilotSymbolQuery.declarationsOf cache (workspace.GetOpenDocumentIds()) solution fullyQualifiedName

                    match Array.tryHeadV declarations with
                    | ValueNone -> return false
                    | ValueSome(struct (item, document)) ->
                        let! sourceText = document.GetTextAsync ct

                        match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, item.Range) with
                        | ValueNone -> return false
                        | ValueSome span ->
                            do! ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync ct

                            let navigation =
                                solution.Workspace.Services.GetService<IFSharpDocumentNavigationService>()

                            return navigation.TryNavigateToSpan(solution.Workspace, document.Id, span, ct)
                }
                |> CancellableTask.start cancellationToken

    // Copilot's own picker providers answer through the batch interface, one result collection per query.
    interface ICopilotMentionBatchQueryable with
        member _.QueryMentionBatchAsync(queries, cancellationToken) : Task<IReadOnlyList<IReadOnlyCollection<CopilotQueriedMention>>> =
            mentionsFor (queries |> Seq.map CopilotSymbolMapping.searchTextOf |> Seq.toArray)
            |> CancellableTask.map (fun mentions -> mentions :> IReadOnlyList<IReadOnlyCollection<CopilotQueriedMention>>)
            |> CancellableTask.start cancellationToken
