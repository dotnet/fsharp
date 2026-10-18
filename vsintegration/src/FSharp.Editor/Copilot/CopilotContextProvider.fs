// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.ComponentModel.Composition
open System.IO
open System.Diagnostics
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

[<Struct>]
type internal PickedDeclaration =
    {
        File: string
        ProjectName: string
        Line: int
    }

[<RequireQualifiedAccess>]
type internal DocumentFocus =
    | Selected
    | Focused
    | Open
    | Elsewhere

module internal CopilotSymbolQuery =

    [<Literal>]
    let private MaxMentions = 20

    [<Literal>]
    let private MaxDeclarations = 4

    [<Literal>]
    let private UserOpName = "CopilotSymbolContext"

    /// Copilot drops the partial answer of a cancelled query, so a long cold scan stops itself instead.
    [<Literal>]
    let private ColdSearchBudgetMs = 1500L

    [<Literal>]
    let private MinSolutionWideSearchLength = 3

    let private parallelism = max 1 (Environment.ProcessorCount - 1)

    let private matcherFor (cache: FSharpNavigableItemsCache) (searchText: string) =
        match searchText with
        | "" -> fun (_: NavigableItem) -> ValueSome(PatternMatch(PatternMatchKind.Exact, false, false))
        | searchText -> cache.CreateMatcherFor searchText

    let private fsharpDocuments (solution: Solution) =
        solution.Projects |> Seq.where _.IsFSharp |> Seq.collect _.Documents

    let private isFocused (focus: EditorFocus) (document: Document) =
        String.Equals(focus.FilePath, document.FilePath, StringComparison.OrdinalIgnoreCase)

    let private documentFocusOf
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

    [<Struct>]
    type private Outline =
        {
            Text: SourceText
            Lines: string array
            Declared: Dictionary<int, Structure.ScopeRange>
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
                    Declared =
                        Structure.getOutliningRanges sourceLines parseResults.ParseTree
                        |> CopilotSymbolSnippets.declaredByLine
                }
        }

    let private notSelected (_: NavigableItem) = false

    let private isSelectedIn (focus: EditorFocus) (document: Document) =
        cancellableTask {
            let! outline = outlineOf document

            return
                fun (item: NavigableItem) ->
                    let struct (firstLine, lastLine) =
                        CopilotSymbolSnippets.declarationLines outline.Lines outline.Declared item

                    firstLine <= focus.LastLine && focus.FirstLine <= lastLine
        }

    let tiers (cache: FSharpNavigableItemsCache) (openIds: HashSet<DocumentId>) (solution: Solution) =
        let opened =
            openIds
            |> Seq.chooseV (fun id ->
                match solution.GetDocument id with
                | null -> ValueNone
                | document when document.Project.IsFSharp -> ValueSome document
                | _ -> ValueNone)
            |> ResizeArray

        let unopened =
            seq {
                for document in fsharpDocuments solution do
                    if not (openIds.Contains document.Id) then
                        document
            }

        // Navigate To fills the same cache between the two walks: cold is what the cached walk did not answer.
        let answered = HashSet<DocumentId>()

        let cached =
            seq {
                for document in unopened do
                    match cache.TryGetCachedNavigableItems document with
                    | ValueSome items ->
                        answered.Add document.Id |> ignore
                        struct (document, items)
                    | ValueNone -> ()
            }

        let cold =
            seq {
                for document in unopened do
                    if not (answered.Contains document.Id) then
                        document
            }

        struct (opened, cached, cold)

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

            do! Task.parallelDoLimit parallelism ct (opened |> Seq.map parseAndCollect)

            if not (enough ()) then
                for struct (document, items) in cached do
                    ct.ThrowIfCancellationRequested()
                    collect document items

            if not (enough ()) then
                let budget = Stopwatch.StartNew()

                let scan (document: Document) =
                    cancellableTask {
                        // Gates the next parse rather than cancelling one: a session's first query must survive its first parse.
                        if budget.ElapsedMilliseconds < budgetMs && not (enough ()) then
                            do! parseAndCollect document
                    }

                do! Task.parallelDoLimit parallelism ct (cold |> Seq.map scan)
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
                | ValueSome focus, ValueSome document -> isSelectedIn focus document
                | _ -> CancellableTask.singleton notSelected

            return
                queries
                |> Array.map (fun query ->
                    let seen = HashSet StringComparer.Ordinal
                    let mentions = ResizeArray MaxMentions

                    let ranked =
                        query.Hits
                        |> Seq.map (fun hit ->
                            let focus = documentFocusOf focus openIds isSelected hit.Document hit.Item
                            struct (rankOf focus * 2 + (if hit.Document.IsFSharpSignatureFile then 1 else 0), hit), focus)
                        |> Seq.sortWith (fun (ranked1, _) (ranked2, _) -> compareRanked ranked1 ranked2)

                    for struct (_, hit), focus in ranked do
                        if mentions.Count < MaxMentions && seen.Add hit.Name then
                            mentions.Add(struct (hit.Item, hit.Document, focus))

                    mentions.ToArray())
        }

    let private declarationsFound
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
                |> Seq.toArray

            return
                match implementations with
                | [||] -> hits.ToArray()
                | implementations -> implementations
        }

    /// The picked project's copy of a linked file answers first: its defines decide what is declared where.
    let private declarationsPickedAt
        (cache: FSharpNavigableItemsCache)
        (solution: Solution)
        (fullyQualifiedName: string)
        (picked: PickedDeclaration)
        =
        cancellableTask {
            let inFile =
                solution.GetDocumentIdsWithFilePath picked.File
                |> Seq.map (fun documentId -> solution.GetDocument documentId)
                |> Seq.filter (fun document -> document.Project.IsFSharp && not document.IsFSharpSignatureFile)
                |> Seq.toArray

            let isPickedProject (document: Document) =
                String.Equals(document.Project.Name, picked.ProjectName, StringComparison.Ordinal)

            let documents =
                match Array.filter isPickedProject inFile with
                | [||] -> inFile
                | inPickedProject -> inPickedProject

            let found = ResizeArray()

            for document in documents do
                let! items = cache.GetNavigableItems document

                for item in items do
                    if CopilotSymbolMapping.hasFullyQualifiedName fullyQualifiedName item then
                        found.Add(struct (item, document))

            return
                found
                |> Seq.sortBy (fun (struct (item, _)) -> item.Range.StartLine <> picked.Line)
                |> Seq.toArray
        }

    let declarationsOf
        (cache: FSharpNavigableItemsCache)
        (openDocumentIds: DocumentId seq)
        (solution: Solution)
        (fullyQualifiedName: string)
        (picked: PickedDeclaration voption)
        =
        cancellableTask {
            let! pickedDeclarations =
                match picked with
                | ValueSome picked -> declarationsPickedAt cache solution fullyQualifiedName picked
                | ValueNone -> CancellableTask.singleton Array.empty

            match pickedDeclarations with
            | [||] -> return! declarationsFound cache openDocumentIds solution fullyQualifiedName
            | declarations -> return declarations
        }

    let private snippetOf (outline: Outline) (item: NavigableItem) =
        let struct (firstLine, lastLine) =
            CopilotSymbolSnippets.definitionLines outline.Lines outline.Declared item

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
        (picked: PickedDeclaration voption)
        =
        cancellableTask {
            let! declarations = declarationsOf cache openDocumentIds solution fullyQualifiedName picked
            let declarations = Array.truncate MaxDeclarations declarations

            match Array.tryHeadV declarations with
            | ValueNone -> return ValueNone
            | ValueSome(struct (first, _)) ->
                let snippets = ResizeArray()
                let locations = ResizeArray()
                let mutable outlined: struct (DocumentId * Outline) voption = ValueNone

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

[<ExportBrokeredService(FSharpConstants.copilotSymbolProviderName,
                        CopilotDescriptors.CurrentContextProviderVersion,
                        [| typeof<ICopilotMentionQueryable>; typeof<ICopilotMentionBatchQueryable> |],
                        Audience = (ServiceAudience.PublicSdk ||| ServiceAudience.Local))>]
type internal FSharpCopilotContextProvider
    [<ImportingConstructor>]
    (
        cache: FSharpNavigableItemsCache,
        [<Import(typeof<SVsFullAccessServiceBroker>)>] serviceBroker: IServiceBroker,
        [<Import(AllowDefault = true)>] workspace: VisualStudioWorkspace | null
    ) =

    // Never disposed: disposing it would dispose the broker it wraps.
    let documentContexts =
        new ServiceBrokerClient(serviceBroker, ThreadHelper.JoinableTaskFactory)

    let editorFocus () =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            use! rental = documentContexts.GetProxyAsync<ICopilotDocumentContextProvider>(CopilotDescriptors.Context.Document, ct).AsTask()

            match rental.Proxy with
            | null -> return ValueNone
            | proxy ->
                let! context = proxy.GetActiveDocumentAsync(CopilotCorrelationId.New(), ct)

                match context with
                | null -> return ValueNone
                | context ->
                    match context.TryGetValue<DocumentContext>() with
                    | true, document -> return CopilotSymbolMapping.editorFocusOf document
                    | _ -> return ValueNone
        }

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
                CopilotInputDescriptor(
                    CopilotSymbolMapping.DeclarationFileInput,
                    "File the picked declaration is in.",
                    CopilotDefaultTypes.StringName,
                    IsRequired = false
                )
                CopilotInputDescriptor(
                    CopilotSymbolMapping.DeclarationProjectInput,
                    "Project the picked declaration is compiled in.",
                    CopilotDefaultTypes.StringName,
                    IsRequired = false
                )
                CopilotInputDescriptor(
                    CopilotSymbolMapping.DeclarationLineInput,
                    "Line the picked overload or partial definition is declared on.",
                    CopilotDefaultTypes.IntegerName,
                    IsRequired = false
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
        let inputs = Dictionary<string, CopilotValue>(4, StringComparer.Ordinal)

        inputs[CopilotSymbolMapping.FullyQualifiedNameInput] <-
            CopilotValue(CopilotDefaultTypes.StringName, CopilotSymbolMapping.fullyQualifiedName item)

        match document.FilePath with
        | null -> ()
        | filePath ->
            inputs[CopilotSymbolMapping.DeclarationFileInput] <- CopilotValue(CopilotDefaultTypes.StringName, filePath)
            inputs[CopilotSymbolMapping.DeclarationProjectInput] <- CopilotValue(CopilotDefaultTypes.StringName, document.Project.Name)

        inputs[CopilotSymbolMapping.DeclarationLineInput] <- CopilotValue(CopilotDefaultTypes.IntegerName, item.Range.StartLine)

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

    let mentionsFor (searchTexts: string voption[]) =
        cancellableTask {
            match workspace, searchTexts |> Seq.chooseV id |> Seq.distinct |> Seq.toArray with
            | null, _
            | _, [||] -> return Array.create searchTexts.Length noMentions
            | workspace, distinct ->
                let! focus = editorFocus ()

                let! hits = CopilotSymbolQuery.search cache (workspace.GetOpenDocumentIds()) focus workspace.CurrentSolution distinct

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

    let fullyQualifiedNameOf inputs =
        CopilotSymbolMapping.textOf CopilotSymbolMapping.FullyQualifiedNameInput inputs

    let pickedAt inputs =
        match
            CopilotSymbolMapping.textOf CopilotSymbolMapping.DeclarationFileInput inputs,
            CopilotSymbolMapping.textOf CopilotSymbolMapping.DeclarationProjectInput inputs,
            CopilotSymbolMapping.inputOf<int> CopilotSymbolMapping.DeclarationLineInput inputs
        with
        | ValueSome file, ValueSome project, ValueSome line ->
            ValueSome
                {
                    File = file
                    ProjectName = project
                    Line = line
                }
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
                        CopilotSymbolQuery.symbolContext
                            cache
                            (workspace.GetOpenDocumentIds())
                            workspace.CurrentSolution
                            fullyQualifiedName
                            (pickedAt inputs)

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

                    let! declarations =
                        CopilotSymbolQuery.declarationsOf
                            cache
                            (workspace.GetOpenDocumentIds())
                            solution
                            fullyQualifiedName
                            (pickedAt mention.Inputs)

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

    interface ICopilotMentionBatchQueryable with
        member _.QueryMentionBatchAsync(queries, cancellationToken) : Task<IReadOnlyList<IReadOnlyCollection<CopilotQueriedMention>>> =
            mentionsFor (queries |> Seq.map CopilotSymbolMapping.searchTextOf |> Seq.toArray)
            |> CancellableTask.map (fun mentions -> mentions :> IReadOnlyList<IReadOnlyCollection<CopilotQueriedMention>>)
            |> CancellableTask.start cancellationToken
