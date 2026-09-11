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

open FSharp.Compiler.EditorServices
open CancellableTasks

/// Where a declaration sits relative to what the user is working on. The picker merges answers from every
/// provider and ranks them by the priority each one reports, so a match has to say where it sits rather
/// than rely on its position. The tiers mirror what Copilot's own symbol provider reports for C#.
[<RequireQualifiedAccess>]
type internal DocumentFocus =
    | Focused
    | Open
    | Elsewhere

/// Solution-wide lookup of F# declarations behind the Copilot chat "#" mention picker.
/// Kept apart from the brokered service so it can be exercised without a Visual Studio workspace.
module internal CopilotSymbolQuery =

    /// Also the point at which the search stops parsing files nobody has opened, so it bounds the cold
    /// scan as much as the answer. Copilot's own provider reads an index and can afford a far larger cap.
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
    let private ColdSearchBudgetMs = 1500

    let private parallelism = max 1 (Environment.ProcessorCount - 1)

    let private fsharpDocuments (solution: Solution) =
        solution.Projects
        |> Seq.where (fun project -> project.Language = FSharpConstants.FSharpLanguageName)
        |> Seq.collect _.Documents

    let focusOf (focusedFilePath: string voption) (openIds: HashSet<DocumentId>) (document: Document) =
        match focusedFilePath with
        | ValueSome path when String.Equals(path, document.FilePath, StringComparison.OrdinalIgnoreCase) -> DocumentFocus.Focused
        | _ when openIds.Contains document.Id -> DocumentFocus.Open
        | _ -> DocumentFocus.Elsewhere

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

    let describe (item: NavigableItem) (document: Document) =
        let container =
            match item.Container.FullName with
            | "" -> Path.GetFileName document.FilePath
            | name -> name

        if document.IsFSharpSignatureFile then
            $"signature, {container} - {document.Project.Name}"
        else
            $"{container} - {document.Project.Name}"

    /// Declarations whose fully qualified name matches each search text, best match first, one entry
    /// per name. Every document is visited once for all of the texts.
    let search
        (cache: FSharpNavigableItemsCache)
        (openDocumentIds: DocumentId seq)
        (focusedFilePath: string voption)
        (solution: Solution)
        (searchTexts: string[])
        =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            let openIds = HashSet openDocumentIds
            let matchers = searchTexts |> Array.map cache.CreateMatcherFor
            let hits = Array.init searchTexts.Length (fun _ -> ResizeArray())
            let found = Array.init searchTexts.Length (fun _ -> HashSet StringComparer.Ordinal)

            let collect (document: Document) (items: NavigableItem array) =
                lock hits (fun () ->
                    for index in 0 .. matchers.Length - 1 do
                        let tryMatch = matchers[index]

                        for item in items do
                            match tryMatch item with
                            | ValueSome patternMatch ->
                                hits[index].Add(struct (patternMatch.Kind, item, document))

                                found[index].Add(CopilotSymbolMapping.fullyQualifiedName item) |> ignore
                            | ValueNone -> ())

            let enough () =
                lock hits (fun () -> found |> Array.forall (fun names -> names.Count >= MaxMentions))

            let parseAndCollect (document: Document) =
                cancellableTask {
                    let! items = cache.GetNavigableItems document
                    collect document items
                }

            let struct (opened, cached, cold) = tiers cache openIds solution

            do! opened |> CancellableTask.forEachThrottled parallelism parseAndCollect

            if not (enough ()) then
                for struct (document, items) in cached do
                    ct.ThrowIfCancellationRequested()
                    collect document items

            if not (enough ()) then
                let budget = Stopwatch.StartNew()

                // The budget stops handing out documents rather than cancelling a parse under way:
                // the first query of a session has to survive its first parse to answer at all.
                let scan (document: Document) =
                    cancellableTask {
                        if budget.ElapsedMilliseconds < ColdSearchBudgetMs && not (enough ()) then
                            do! parseAndCollect document
                    }

                do! cold |> CancellableTask.forEachThrottled parallelism scan

            return
                hits
                |> Array.map (
                    Seq.sortBy (fun (struct (kind, item: NavigableItem, document: Document)) ->
                        focusOf focusedFilePath openIds document, document.IsFSharpSignatureFile, kind, item.Name.Length)
                    >> Seq.distinctBy (fun (struct (_, item, _)) -> CopilotSymbolMapping.fullyQualifiedName item)
                    >> Seq.truncate MaxMentions
                    >> Seq.map (fun (struct (_, item, document)) -> struct (item, document, focusOf focusedFilePath openIds document))
                    >> Seq.toArray
                )
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
            let! ct = CancellableTask.getCancellationToken ()
            let hits = ResizeArray()

            let collect (document: Document) (items: NavigableItem array) =
                lock hits (fun () ->
                    for item in items do
                        if CopilotSymbolMapping.hasFullyQualifiedName fullyQualifiedName item then
                            hits.Add(struct (item, document)))

            let declaredInImplementation () =
                lock hits (fun () ->
                    hits
                    |> Seq.exists (fun (struct (_, document: Document)) -> not document.IsFSharpSignatureFile))

            let parseAndCollect (document: Document) =
                cancellableTask {
                    let! items = cache.GetNavigableItems document
                    collect document items
                }

            let struct (opened, cached, cold) = tiers cache (HashSet openDocumentIds) solution

            do! opened |> CancellableTask.forEachThrottled parallelism parseAndCollect

            if not (declaredInImplementation ()) then
                for struct (document, items) in cached do
                    ct.ThrowIfCancellationRequested()
                    collect document items

            if not (declaredInImplementation ()) then
                let scan (document: Document) =
                    cancellableTask {
                        if not (declaredInImplementation ()) then
                            do! parseAndCollect document
                    }

                do! cold |> CancellableTask.forEachThrottled parallelism scan

            let implementations =
                hits
                |> Seq.filter (fun (struct (_, document: Document)) -> not document.IsFSharpSignatureFile)

            let preferred =
                if Seq.isEmpty implementations then
                    hits :> _ seq
                else
                    implementations

            return preferred |> Seq.truncate MaxDeclarations |> Seq.toArray
        }

    /// The source of the whole declaration `item` names, together with the span it occupies.
    let snippetOf (item: NavigableItem) (document: Document) =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            let! sourceText = document.GetTextAsync ct
            let! parseResults = document.GetFSharpParseResultsAsync UserOpName

            let sourceLines = sourceText.GetLinesAsMemory()

            let scopes = Structure.getOutliningRanges sourceLines parseResults.ParseTree

            let struct (firstLine, lastLine) =
                CopilotSymbolSnippets.definitionLines sourceLines scopes item

            let firstLine = max 1 firstLine
            let lastLine = min sourceText.Lines.Count lastLine

            let span =
                TextSpan.FromBounds(sourceText.Lines[firstLine - 1].Start, sourceText.Lines[lastLine - 1].End)

            return struct (sourceText.GetSubText(span).ToString(), span)
        }

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

                for struct (item, document) in declarations do
                    let! struct (text, span) = snippetOf item document
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
        | DocumentFocus.Focused -> CopilotQueriedMentionPriority.High
        | DocumentFocus.Open -> CopilotQueriedMentionPriority.Low
        | DocumentFocus.Elsewhere -> CopilotQueriedMentionPriority.None

    let mentionFor (item: NavigableItem) (document: Document) focus =
        let inputs =
            Dictionary<string, CopilotValue>(
                dict
                    [
                        CopilotSymbolMapping.FullyQualifiedNameInput,
                        CopilotValue(CopilotDefaultTypes.StringName, CopilotSymbolMapping.fullyQualifiedName item)
                    ],
                StringComparer.Ordinal
            )

        let description = CopilotSymbolQuery.describe item document

        CopilotQueriedContextMention(
            moniker,
            descriptor,
            inputs,
            item.Name,
            Description = description,
            Tooltip = description,
            Icon = Nullable(CopilotSymbolMapping.icon item.Kind),
            IsNavigable = true,
            Priority = priorityOf focus
        )
        :> CopilotQueriedMention

    /// The user is still typing, so the trailing input is the search text. It is preceded by the member
    /// name once the mention has been committed, as in "#fsharpSymbol:Namespace.Type". The picker asks
    /// before it has resolved what kind of mention is being typed, which Copilot's own provider answers
    /// as readily as a resolved one.
    let searchTextOf (query: CopilotMentionQuery) =
        match query.Type, query.Inputs with
        | (CopilotMentionType.Context | CopilotMentionType.Unknown), null -> ValueNone
        | (CopilotMentionType.Context | CopilotMentionType.Unknown), inputs when inputs.Count > 0 ->
            match inputs[inputs.Count - 1] with
            | text when String.IsNullOrWhiteSpace text -> ValueNone
            | text when String.Equals(text, CopilotSymbolMapping.SymbolMember, StringComparison.Ordinal) -> ValueNone
            | text -> ValueSome text
        | _ -> ValueNone

    /// One pass over the solution for the whole batch: Copilot's picker asks for several texts at once
    /// and each of them would otherwise walk the same documents.
    let mentionsFor (searchTexts: string voption[]) =
        cancellableTask {
            let distinct = searchTexts |> Seq.chooseV id |> Seq.distinct |> Seq.toArray

            match workspace with
            | null -> return searchTexts |> Array.map (fun _ -> noMentions)
            | _ when Array.isEmpty distinct -> return searchTexts |> Array.map (fun _ -> noMentions)
            | workspace ->
                let! hits =
                    CopilotSymbolQuery.search
                        cache
                        (workspace.GetOpenDocumentIds())
                        activeDocument.FocusedFilePath
                        workspace.CurrentSolution
                        distinct

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
            mentionsFor [| searchTextOf query |]
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
            mentionsFor (queries |> Seq.map searchTextOf |> Seq.toArray)
            |> CancellableTask.map (fun mentions -> mentions :> IReadOnlyList<IReadOnlyCollection<CopilotQueriedMention>>)
            |> CancellableTask.start cancellationToken
