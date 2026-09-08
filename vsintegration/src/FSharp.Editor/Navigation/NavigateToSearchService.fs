// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.IO
open System.Composition
open System.Collections.Immutable
open System.Collections.Concurrent
open System.Globalization
open System.Linq
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Navigation
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.NavigateTo
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.Text.PatternMatching

open FSharp.Compiler.EditorServices
open CancellableTasks

/// The navigable items of one parse of a document, and the text version it was taken from.
[<Struct>]
type private NavigableItemsEntry =
    {
        Version: VersionStamp
        /// Parsed without the project's compilation options, while the solution was still loading.
        Approximate: bool
        Items: NavigableItem array
    }

/// Parse-tree navigable items per document, cached on the document's text version.
/// Shared by NavigateTo and by the Copilot chat mention provider.
[<Export; Shared>]
type internal FSharpNavigableItemsCache
    [<ImportingConstructor>]
    (patternMatcherFactory: IPatternMatcherFactory, [<Import(AllowDefault = true)>] workspace: VisualStudioWorkspace) =

    let cache = ConcurrentDictionary<DocumentId, NavigableItemsEntry>()

    do
        match workspace with
        | null -> ()
        | workspace ->
            workspace.WorkspaceChanged.Add(fun e ->
                if e.NewSolution.Id <> e.OldSolution.Id then
                    cache.Clear())

    let store (document: Document) version approximate parseTree =
        let items = NavigateTo.GetNavigableItems parseTree

        cache[document.Id] <-
            {
                Version = version
                Approximate = approximate
                Items = items
            }

        items

    member _.GetNavigableItems(document: Document) =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            let! currentVersion = document.GetTextVersionAsync(ct)

            match cache.TryGetValue document.Id with
            | true, entry when entry.Version = currentVersion && not entry.Approximate -> return entry.Items
            | _ ->
                let! parseResults = document.GetFSharpParseResultsAsync(nameof (FSharpNavigableItemsCache))
                return store document currentVersion false parseResults.ParseTree
        }

    /// The items of a parse that does not wait for the project's compilation options, for the search that runs while
    /// the solution is still loading. A file behind `#if` can be read under the wrong defines, so the entry it leaves
    /// behind never answers `GetNavigableItems`.
    member _.GetNavigableItemsWhileLoading(document: Document) =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            let! currentVersion = document.GetTextVersionAsync(ct)

            match cache.TryGetValue document.Id with
            | true, entry when entry.Version = currentVersion -> return entry.Items
            | _ ->
                let! parseResults = document.GetFSharpQuickParseResultsAsync(nameof (FSharpNavigableItemsCache))
                return store document currentVersion true parseResults.ParseTree
        }

    member _.CreateMatcherFor(searchPattern: string) =
        let patternMatcher =
            patternMatcherFactory.CreatePatternMatcher(
                searchPattern,
                PatternMatcherCreationOptions(
                    cultureInfo = CultureInfo.CurrentUICulture,
                    flags = PatternMatcherCreationFlags.AllowFuzzyMatching,
                    containerSplitCharacters = [ '.' ]
                )
            )

        fun (item: NavigableItem) ->
            // PatternMatcher will not match operators and some backtick escaped identifiers.
            // To handle them, we fall back to simple substring match.
            let name = item.Name

            if item.NeedsBackticks then
                match name.IndexOf(searchPattern, StringComparison.CurrentCultureIgnoreCase) with
                | i when i > 0 -> ValueSome(PatternMatch(PatternMatchKind.Substring, false, false))
                | 0 when name.Length = searchPattern.Length -> ValueSome(PatternMatch(PatternMatchKind.Exact, false, false))
                | 0 -> ValueSome(PatternMatch(PatternMatchKind.Prefix, false, false))
                | _ -> ValueNone
            else
                // full name with dots allows for path matching, e.g.
                // "f.c.so.elseif" will match "Fantomas.Core.SyntaxOak.ElseIfNode"
                patternMatcher.TryMatch $"{item.Container.FullName}.{name}"
                |> ValueOption.ofNullable

[<Export(typeof<IFSharpNavigateToSearchService>); Shared>]
type internal FSharpNavigateToSearchService [<ImportingConstructor>] (itemsCache: FSharpNavigableItemsCache) =

    /// The parses of the search that runs while the solution loads take turns across all its projects, and leave a
    /// core to the load itself.
    let loadingThrottle = new SemaphoreSlim(max 1 (Environment.ProcessorCount - 1))

    let kindsProvided =
        ImmutableHashSet.Create(
            FSharpNavigateToItemKind.Module,
            FSharpNavigateToItemKind.Class,
            FSharpNavigateToItemKind.Field,
            FSharpNavigateToItemKind.Property,
            FSharpNavigateToItemKind.Method,
            FSharpNavigateToItemKind.Enum,
            FSharpNavigateToItemKind.EnumItem
        )

    let navigateToItemKindToRoslynKind =
        function
        | NavigableItemKind.Module -> FSharpNavigateToItemKind.Module
        | NavigableItemKind.ModuleAbbreviation -> FSharpNavigateToItemKind.Module
        | NavigableItemKind.Exception -> FSharpNavigateToItemKind.Class
        | NavigableItemKind.Type -> FSharpNavigateToItemKind.Class
        | NavigableItemKind.ModuleValue -> FSharpNavigateToItemKind.Field
        | NavigableItemKind.Field -> FSharpNavigateToItemKind.Field
        | NavigableItemKind.Property -> FSharpNavigateToItemKind.Property
        | NavigableItemKind.Constructor -> FSharpNavigateToItemKind.Method
        | NavigableItemKind.Member -> FSharpNavigateToItemKind.Method
        | NavigableItemKind.EnumCase -> FSharpNavigateToItemKind.EnumItem
        | NavigableItemKind.UnionCase -> FSharpNavigateToItemKind.EnumItem

    let navigateToItemKindToGlyph =
        function
        | NavigableItemKind.Module -> Glyph.ModulePublic
        | NavigableItemKind.ModuleAbbreviation -> Glyph.ModulePublic
        | NavigableItemKind.Exception -> Glyph.ClassPublic
        | NavigableItemKind.Type -> Glyph.ClassPublic
        | NavigableItemKind.ModuleValue -> Glyph.FieldPublic
        | NavigableItemKind.Field -> Glyph.FieldPublic
        | NavigableItemKind.Property -> Glyph.PropertyPublic
        | NavigableItemKind.Constructor -> Glyph.MethodPublic
        | NavigableItemKind.Member -> Glyph.MethodPublic
        | NavigableItemKind.EnumCase -> Glyph.EnumPublic
        | NavigableItemKind.UnionCase -> Glyph.EnumPublic

    let formatInfo (container: NavigableContainer) (document: Document) =
        let projectName = document.Project.Name

        let description =
            match container.Type with
            | NavigableContainerType.File -> $"{Path.GetFileName container.Name} - project {projectName}"
            | NavigableContainerType.Exception
            | NavigableContainerType.Type -> $"in {container.Name} - project {projectName}"
            | NavigableContainerType.Module -> $"module {container.Name} - project {projectName}"
            | NavigableContainerType.Namespace -> $"{container.FullName} - project {projectName}" // or maybe show only project name?

        if document.IsFSharpSignatureFile then
            $"signature, {description}"
        else
            description

    let patternMatchKindToNavigateToMatchKind =
        function
        | PatternMatchKind.Exact -> FSharpNavigateToMatchKind.Exact
        | PatternMatchKind.Prefix -> FSharpNavigateToMatchKind.Prefix
        | PatternMatchKind.Substring -> FSharpNavigateToMatchKind.Substring
        | PatternMatchKind.CamelCaseExact -> FSharpNavigateToMatchKind.CamelCaseExact
        | PatternMatchKind.CamelCasePrefix -> FSharpNavigateToMatchKind.CamelCasePrefix
        | PatternMatchKind.CamelCaseNonContiguousPrefix -> FSharpNavigateToMatchKind.CamelCaseNonContiguousPrefix
        | PatternMatchKind.CamelCaseSubstring -> FSharpNavigateToMatchKind.CamelCaseSubstring
        | PatternMatchKind.CamelCaseNonContiguousSubstring -> FSharpNavigateToMatchKind.CamelCaseNonContiguousSubstring
        | PatternMatchKind.Fuzzy -> FSharpNavigateToMatchKind.Fuzzy
        | _ -> FSharpNavigateToMatchKind.None

    let createMatcherFor (searchPattern: string) =
        itemsCache.CreateMatcherFor searchPattern

    let processDocument
        (getItems: Document -> CancellableTask<NavigableItem array>)
        (tryMatch: NavigableItem -> PatternMatch voption)
        (kinds: IImmutableSet<string>)
        (document: Document)
        =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()

            let! sourceText = document.GetTextAsync ct

            let! items = getItems document

            let processed =
                seq {
                    for item in items do
                        let contains = kinds.Contains(navigateToItemKindToRoslynKind item.Kind)
                        let patternMatch = tryMatch item

                        match contains, patternMatch with
                        | true, ValueSome m ->
                            let sourceSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, item.Range)

                            match sourceSpan with
                            | ValueNone -> ()
                            | ValueSome sourceSpan ->
                                let glyph = navigateToItemKindToGlyph item.Kind
                                let kind = navigateToItemKindToRoslynKind item.Kind
                                let additionalInfo = formatInfo item.Container document

                                yield
                                    FSharpNavigateToSearchResult(
                                        additionalInfo,
                                        kind,
                                        patternMatchKindToNavigateToMatchKind m.Kind,
                                        item.Name,
                                        FSharpNavigableItem(
                                            glyph,
                                            ImmutableArray.Create(TaggedText(TextTags.Text, item.Name)),
                                            document,
                                            sourceSpan
                                        )
                                    )
                        | _ -> ()
                }

            return processed |> Seq.toImmutableArray
        }

    /// Priority items first, each half in its original order, as NavigateTo's own service orders its work.
    let prioritize isPriority items =
        let priority, rest = items |> Seq.toArray |> Array.partition isPriority
        [| yield! priority; yield! rest |]

    let throttled (work: CancellableTask<'a>) =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            do! loadingThrottle.WaitAsync ct

            try
                return! work
            finally
                loadingThrottle.Release() |> ignore
        }

    interface IFSharpNavigateToSearchService with
        member _.SearchProjectAsync
            (project, _priorityDocuments, searchPattern, kinds, cancellationToken)
            : Task<ImmutableArray<FSharpNavigateToSearchResult>> =
            cancellableTask {
                let tryMatch = createMatcherFor searchPattern

                let! results =
                    project.Documents
                    |> Seq.map (processDocument itemsCache.GetNavigableItems tryMatch kinds)
                    |> CancellableTask.whenAll

                return results |> Seq.collect _.AsEnumerable() |> Seq.toImmutableArray
            }
            |> CancellableTask.start cancellationToken

        member _.SearchDocumentAsync(document: Document, searchPattern, kinds, cancellationToken) =
            processDocument itemsCache.GetNavigableItems (createMatcherFor searchPattern) kinds document cancellationToken

        member _.KindsProvided = kindsProvided

        member _.CanFilter = true

    interface IFSharpAdvancedNavigateToSearchService with
        member _.SearchCachedDocumentsAsync
            (
                _solution,
                projects,
                priorityDocuments,
                searchPattern,
                kinds,
                _activeDocument,
                onResultsFound,
                onProjectCompleted,
                cancellationToken
            ) : Task =
            let tryMatch = createMatcherFor searchPattern
            let priorityIds = ImmutableHashSet.CreateRange(priorityDocuments |> Seq.map _.Id)
            let isPriority (document: Document) = priorityIds.Contains document.Id

            let searchDocumentWhileLoading document =
                cancellableTask {
                    let! results = throttled (processDocument itemsCache.GetNavigableItemsWhileLoading tryMatch kinds document)

                    if results.Length > 0 then
                        do! onResultsFound.Invoke results
                }

            // Every document waits on the throttle in the order it is started, so priority documents, and the
            // projects that hold them, are parsed first.
            let searchProjectWhileLoading (project: Project) =
                cancellableTask {
                    let! _ =
                        project.Documents
                        |> prioritize isPriority
                        |> Seq.map searchDocumentWhileLoading
                        |> CancellableTask.whenAll

                    do! onProjectCompleted.Invoke()
                }

            projects
            |> prioritize (fun project -> project.Documents |> Seq.exists isPriority)
            |> Seq.map searchProjectWhileLoading
            |> CancellableTask.whenAll
            |> CancellableTask.startAsTask cancellationToken
