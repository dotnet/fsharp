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
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.Text.PatternMatching

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax
open CancellableTasks

/// Where a parse of a file is kept: under what the parse depends on besides the text. The defines are those it was
/// parsed with, or `AnyDefines` when its tree holds no conditional directives and so reads the same under any of
/// them; the language version decides what the parser accepts and is never shared across versions. The remaining
/// parsing options cannot differ for one path: the editor never applies line directives, and whether a file is
/// interactive follows from its own extension.
[<Struct>]
type private NavigableItemsKey =
    {
        Defines: string
        LangVersion: string
        FilePath: string
    }

/// The navigable items of one parse of a file, and the text version it was taken from.
[<Struct>]
type private NavigableItemsEntry =
    {
        Version: VersionStamp
        /// Parsed without the project's defines, while the solution was still loading.
        Approximate: bool
        Items: NavigableItem array
    }

/// Parse-tree navigable items per document, cached on the document's text version, and kept in persistent storage
/// on the text's checksum so that the next session reads them back instead of parsing.
/// Shared by NavigateTo and by the Copilot chat mention provider.
[<Export; Shared>]
type internal FSharpNavigableItemsCache
    [<ImportingConstructor>]
    (
        patternMatcherFactory: IPatternMatcherFactory,
        storageService: IFSharpChecksummedPersistentStorageService,
        [<Import(AllowDefault = true)>] workspace: VisualStudioWorkspace
    ) =

    /// A multi-targeted project is one Roslyn project per target framework over the same files, so the same
    /// file is parsed once per instance. A parse whose tree holds no conditional directives does not depend
    /// on the defines: it is stored under `AnyDefines` and every instance reuses it. One that does hold them
    /// is stored per define set, because those instances genuinely parse the file differently.
    ///
    /// The duplicate results that produces are not for this service to remove: `NavigateToSearcher` pools its
    /// seen set with `NavigateToSearchResultComparer`, which collapses results by file path and span.
    let cache = ConcurrentDictionary<NavigableItemsKey, NavigableItemsEntry>()

    /// The key for a parse that does not depend on the defines. Not a define set any instance can have,
    /// since defines are identifiers — an instance with none of its own must not read this entry as its own.
    [<Literal>]
    let AnyDefines = "?"

    /// The key for a parse made before the project's options arrived, with the editing defaults in place of its
    /// defines. Not a define set any instance can have, for the same reason as `AnyDefines`.
    [<Literal>]
    let UnknownDefines = "??"

    do
        match workspace with
        | null -> ()
        | workspace ->
            workspace.WorkspaceChanged.Add(fun e ->
                if e.NewSolution.Id <> e.OldSolution.Id then
                    cache.Clear())

    let dependsOnDefines (parseTree: ParsedInput) =
        match parseTree with
        | ParsedInput.ImplFile file -> not file.Trivia.ConditionalDirectives.IsEmpty
        | ParsedInput.SigFile file -> not file.Trivia.ConditionalDirectives.IsEmpty

    let definesOf (document: Document) =
        document.GetFSharpQuickDefines() |> String.concat ";"

    let keyOf (document: Document) defines path =
        {
            Defines = defines
            LangVersion = snd (document.GetFsharpParsingOptions())
            FilePath = path
        }

    /// The entry for the file, from the parse every instance shares when it has no directives, otherwise from
    /// the one parsed with this instance's defines. `matchesVersion` is false for the caller that takes the
    /// last parse whatever version it came from.
    let tryCached (document: Document) matchesVersion =
        match document.FilePath with
        | null -> ValueNone
        | path ->
            let entry defines =
                match cache.TryGetValue(keyOf document defines path) with
                | true, entry when matchesVersion entry.Version -> ValueSome entry
                | _ -> ValueNone

            // The defines are the project's own only once it has produced its options. Before that they are the
            // editing defaults, which a project that defines nothing of its own really has, so a project without
            // options would read that project's parse as if it were its own.
            let ownParse =
                match document.TryGetFSharpParsingOptionsData() with
                | ValueSome _ -> entry (definesOf document)
                | ValueNone -> ValueNone

            match entry AnyDefines with
            | ValueSome found -> ValueSome found
            | ValueNone ->
                match ownParse with
                | ValueSome found -> ValueSome found
                // Only the search that runs while the solution loads takes this one: it is approximate, and the
                // exact search refuses an approximate entry.
                | ValueNone -> entry UnknownDefines

    let remember (document: Document) defines version approximate items =
        match document.FilePath with
        | null -> ()
        | path ->
            cache[keyOf document defines path] <-
                {
                    Version = version
                    Approximate = approximate
                    Items = items
                }

    let store (document: Document) version approximate (parseTree: ParsedInput) =
        let items = NavigateTo.GetNavigableItems parseTree

        let defines =
            if not (dependsOnDefines parseTree) then
                AnyDefines
            elif approximate then
                // A project without its options yet parses with the editing defaults, which are nobody's defines:
                // an instance whose own defines happen to equal them must not read this entry as its own parse.
                UnknownDefines
            else
                definesOf document

        remember document defines version approximate items
        items

    /// Storage keeps one entry per document, under the checksum of the text alone when its tree holds no conditional
    /// directives and of the text and the defines when it does — as the memory entry is kept under `AnyDefines` or
    /// the defines — so a read tries one checksum, then the other.
    let tryRestore (document: Document) defines version approximate checksum =
        cancellableTask {
            match! NavigableItemsIndex.tryLoad storageService document checksum with
            | ValueSome items ->
                remember document defines version approximate items
                return ValueSome items
            | ValueNone -> return ValueNone
        }

    member _.GetNavigableItems(document: Document) =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            let! currentVersion = document.GetTextVersionAsync(ct)

            match tryCached document ((=) currentVersion) with
            | ValueSome entry when not entry.Approximate -> return entry.Items
            | _ ->
                let! text = document.GetTextAsync(ct)
                let textChecksum = NavigableItemsIndex.textChecksum text

                match! tryRestore document AnyDefines currentVersion false textChecksum with
                | ValueSome items -> return items
                | ValueNone ->
                    // The defines are only the project's once it has its options.
                    let! _ = document.GetFSharpCompilationOptionsAsync(nameof (FSharpNavigableItemsCache))
                    let defines = definesOf document

                    let definesChecksum =
                        NavigableItemsIndex.textAndDefinesChecksum textChecksum defines

                    match! tryRestore document defines currentVersion false definesChecksum with
                    | ValueSome items -> return items
                    | ValueNone ->
                        let! parseResults = document.GetFSharpParseResultsAsync(nameof (FSharpNavigableItemsCache))
                        let parseTree = parseResults.ParseTree
                        let items = store document currentVersion false parseTree

                        let checksum =
                            if dependsOnDefines parseTree then
                                definesChecksum
                            else
                                textChecksum

                        do! NavigableItemsIndex.save storageService document checksum items
                        return items
        }

    /// The items of a parse that does not wait for the project's compilation options, for the search that runs while
    /// the solution is still loading. A file behind `#if` can be read under the wrong defines, so the entry it leaves
    /// behind never answers `GetNavigableItems`, and is not stored.
    member _.GetNavigableItemsWhileLoading(document: Document) =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            let! currentVersion = document.GetTextVersionAsync(ct)

            match tryCached document ((=) currentVersion) with
            | ValueSome entry -> return entry.Items
            | ValueNone ->
                let! text = document.GetTextAsync(ct)
                let textChecksum = NavigableItemsIndex.textChecksum text

                match! tryRestore document AnyDefines currentVersion false textChecksum with
                | ValueSome items -> return items
                | ValueNone ->
                    let defines = definesOf document

                    let definesChecksum =
                        NavigableItemsIndex.textAndDefinesChecksum textChecksum defines

                    match! tryRestore document defines currentVersion true definesChecksum with
                    | ValueSome items -> return items
                    | ValueNone ->
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
            let! items = getItems document

            let matches =
                [|
                    for item in items do
                        if kinds.Contains(navigateToItemKindToRoslynKind item.Kind) then
                            match tryMatch item with
                            | ValueSome m -> yield struct (item, m)
                            | ValueNone -> ()
                |]

            // The text, read from disk for a closed document, is only needed to place the matches.
            if matches.Length = 0 then
                return ImmutableArray.Empty
            else
                let! ct = CancellableTask.getCancellationToken ()
                let! sourceText = document.GetTextAsync ct

                return
                    [|
                        for struct (item, m) in matches do
                            match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, item.Range) with
                            | ValueNone -> ()
                            | ValueSome sourceSpan ->
                                yield
                                    FSharpNavigateToSearchResult(
                                        formatInfo item.Container document,
                                        navigateToItemKindToRoslynKind item.Kind,
                                        patternMatchKindToNavigateToMatchKind m.Kind,
                                        item.Name,
                                        FSharpNavigableItem(
                                            navigateToItemKindToGlyph item.Kind,
                                            ImmutableArray.Create(TaggedText(TextTags.Text, item.Name)),
                                            document,
                                            sourceSpan
                                        )
                                    )
                    |]
                    |> Array.toImmutableArray
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
                    // Throttle to avoid launching a parse per document in the project all at once.
                    |> CancellableTask.whenAllThrottled (max 1 Environment.ProcessorCount)

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
