// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.IO
open System.Composition
open System.Collections.Immutable
open System.Collections.Concurrent
open System.Threading.Tasks
open System.Globalization

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Navigation
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.NavigateTo
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.Text.PatternMatching

open FSharp.Compiler
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax
open CancellableTasks

[<Struct>]
type private ParseKey =
    | AnyDefines of filePath: string * langVersion: string
    | DefinedAs of filePath: string * langVersion: string * defines: string list

[<Export; Shared>]
type internal FSharpNavigableItemsCache
    [<ImportingConstructor>]
    (patternMatcherFactory: IPatternMatcherFactory, [<Import(AllowDefault = true)>] workspace: VisualStudioWorkspace) =

    let cache = ConcurrentDictionary<ParseKey, VersionStamp * NavigableItem array>()

    do
        if workspace <> null then
            workspace.WorkspaceChanged.Add
            <| fun e ->
                if e.NewSolution.Id <> e.OldSolution.Id then
                    cache.Clear()

    // Awaiting a project's options recomputes them after an edit to a C# project it references.
    let parsingOptionsOf (document: Document) =
        let optionsManager =
            document.Project.Solution.GetFSharpWorkspaceService().FSharpProjectOptionsManager

        match optionsManager.TryGetCachedOptionsByProjectId document.Project.Id with
        | Some(_, parsingOptions, _) -> CancellableTask.singleton parsingOptions
        | None ->
            document.GetFSharpCompilationOptionsAsync(nameof (FSharpNavigableItemsCache))
            |> CancellableTask.map (fun (_, _, parsingOptions, _) -> parsingOptions)

    let storedUnder anyDefinesKey definedAsKey =
        function
        | ParsedInput.ImplFile(ParsedImplFileInput(trivia = trivia))
        | ParsedInput.SigFile(ParsedSigFileInput(trivia = trivia)) when trivia.ConditionalDirectives.IsEmpty -> anyDefinesKey
        | _ -> definedAsKey

    member _.GetNavigableItems(document: Document) =
        cancellableTask {
            let! ct = CancellableTask.getCancellationToken ()
            let! currentVersion = document.GetTextVersionAsync(ct)
            let! parsingOptions = parsingOptionsOf document
            let anyDefinesKey = AnyDefines(document.FilePath, parsingOptions.LangVersionText)

            let definedAsKey =
                DefinedAs(
                    document.FilePath,
                    parsingOptions.LangVersionText,
                    CompilerEnvironment.GetConditionalDefinesForEditing parsingOptions
                )

            let cached key =
                match cache.TryGetValue key with
                | true, (version, items) when version = currentVersion -> ValueSome items
                | _ -> ValueNone

            match cached anyDefinesKey |> ValueOption.orElseWith (fun () -> cached definedAsKey) with
            | ValueSome items -> return items
            | ValueNone ->
                let! parseResults = document.GetFSharpParseResultsAsync(nameof (FSharpNavigableItemsCache))
                let items = NavigateTo.GetNavigableItems parseResults.ParseTree
                cache[storedUnder anyDefinesKey definedAsKey parseResults.ParseTree] <- currentVersion, items
                return items
        }

    /// Uses the version the document already knows: asking for it could read a closed file from disk.
    member _.TryGetCachedNavigableItems(document: Document) =
        let optionsManager =
            document.Project.Solution.GetFSharpWorkspaceService().FSharpProjectOptionsManager

        let parsingOptions =
            match optionsManager.TryGetCachedOptionsByProjectId document.Project.Id with
            | Some(_, parsingOptions, _) -> ValueSome parsingOptions
            | None ->
                match ProjectCache.Projects.TryGetValue document.Project with
                | true, options -> ValueSome options.ParsingOptions
                | _ -> ValueNone

        match parsingOptions, document.TryGetTextVersion() with
        | ValueSome parsingOptions, (true, currentVersion) ->
            let cached key =
                match cache.TryGetValue key with
                | true, (version, items) when version = currentVersion -> ValueSome items
                | _ -> ValueNone

            cached (AnyDefines(document.FilePath, parsingOptions.LangVersionText))
            |> ValueOption.orElseWith (fun () ->
                cached (
                    DefinedAs(
                        document.FilePath,
                        parsingOptions.LangVersionText,
                        CompilerEnvironment.GetConditionalDefinesForEditing parsingOptions
                    )
                ))
        | _ -> ValueNone

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

    let getNavigableItems (document: Document) = itemsCache.GetNavigableItems document

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

    let processDocument (tryMatch: NavigableItem -> PatternMatch voption) (kinds: IImmutableSet<string>) (document: Document) =
        cancellableTask {
            let! items = getNavigableItems document

            let matches =
                [|
                    for item in items do
                        if kinds.Contains(navigateToItemKindToRoslynKind item.Kind) then
                            match tryMatch item with
                            | ValueSome m -> yield struct (item, m)
                            | ValueNone -> ()
                |]

            if matches.Length = 0 then
                return [||]
            else
                let! ct = CancellableTask.getCancellationToken ()
                let! sourceText = document.GetTextAsync ct

                return
                    [|
                        for struct (item, m) in matches do
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
                    |]
        }

    interface IFSharpNavigateToSearchService with
        member _.SearchProjectAsync
            (project, priorityDocuments, searchPattern, kinds, cancellationToken)
            : Task<ImmutableArray<FSharpNavigateToSearchResult>> =
            cancellableTask {
                let tryMatch = createMatcherFor searchPattern
                let priorityIds = ImmutableHashSet.CreateRange(priorityDocuments |> Seq.map _.Id)

                let tasks =
                    [|
                        for doc in project.Documents |> Seq.sortBy (fun doc -> not (priorityIds.Contains doc.Id)) do
                            yield processDocument tryMatch kinds doc
                    |]

                let! results = CancellableTask.whenAllThrottled (max 1 Environment.ProcessorCount) tasks

                return results |> Array.concat |> Array.toImmutableArray

            }
            |> CancellableTask.start cancellationToken

        member _.SearchDocumentAsync(document: Document, searchPattern, kinds, cancellationToken) =
            cancellableTask {
                let! result = processDocument (createMatcherFor searchPattern) kinds document
                return Array.toImmutableArray result
            }
            |> CancellableTask.start cancellationToken

        member _.KindsProvided = kindsProvided

        member _.CanFilter = true
