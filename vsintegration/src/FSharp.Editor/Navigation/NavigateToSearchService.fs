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

open FSharp.Compiler.EditorServices

[<Export(typeof<IFSharpNavigateToSearchService>); Shared>]
type internal FSharpNavigateToSearchService [<ImportingConstructor>]
    (
        patternMatcherFactory: IPatternMatcherFactory,
        [<Import(AllowDefault = true)>] workspace: VisualStudioWorkspace
    ) =

    let cache = ConcurrentDictionary<DocumentId, VersionStamp * NavigableItem array>()

    do
        if workspace <> null then
            workspace.WorkspaceChanged.Add
            <| fun e ->
                if e.NewSolution.Id <> e.OldSolution.Id then
                    cache.Clear()

    let getNavigableItems (document: Document) =
        async {
            let! currentVersion = document.GetTextVersionAsync() |> Async.AwaitTask

            match cache.TryGetValue document.Id with
            | true, (version, items) when version = currentVersion -> return items
            | _ ->
                let! parseResults = document.GetFSharpParseResultsAsync(nameof (FSharpNavigateToSearchService))
                let items = parseResults.ParseTree |> NavigateTo.GetNavigableItems
                cache[document.Id] <- currentVersion, items
                return items
        }

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

    let createMatcherFor searchPattern =
        let patternMatcher =
                patternMatcherFactory.CreatePatternMatcher(
                    searchPattern,
                    PatternMatcherCreationOptions(
                        cultureInfo = CultureInfo.CurrentUICulture,
                        flags = PatternMatcherCreationFlags.AllowFuzzyMatching,
                        containerSplitCharacters = ['.']
                    )
                )
        fun (item: NavigableItem) ->  
            // PatternMatcher will not match operators and some backtick escaped identifiers.
            // To handle them, we fall back to simple substring match.
            let name = item.Name
            if item.NeedsBackticks then
                match name.IndexOf(searchPattern, StringComparison.CurrentCultureIgnoreCase) with
                | i when i > 0 -> PatternMatch(PatternMatchKind.Substring, false, false) |> Some
                | 0 when name.Length = searchPattern.Length -> PatternMatch(PatternMatchKind.Exact, false, false) |> Some
                | 0 -> PatternMatch(PatternMatchKind.Prefix, false, false) |> Some
                | _ -> None
            else
                // full name with dots allows for path matching, e.g.
                // "f.c.so.elseif" will match "Fantomas.Core.SyntaxOak.ElseIfNode"
                patternMatcher.TryMatch $"{item.Container.FullName}.{name}"
                |> Option.ofNullable   
            

    let processDocument (tryMatch: NavigableItem -> PatternMatch option) (kinds: IImmutableSet<string>) (document: Document) =
        async {
            let! sourceText = document.GetTextAsync Async.DefaultCancellationToken |> Async.AwaitTask

            let processItem item =
                asyncMaybe {
                    do! Option.guard (kinds.Contains(navigateToItemKindToRoslynKind item.Kind))

                    let! m = tryMatch item

                    let! sourceSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, item.Range)
                    let glyph = navigateToItemKindToGlyph item.Kind
                    let kind = navigateToItemKindToRoslynKind item.Kind
                    let additionalInfo = formatInfo item.Container document

                    return
                        FSharpNavigateToSearchResult(
                            additionalInfo,
                            kind,
                            patternMatchKindToNavigateToMatchKind m.Kind,
                            item.Name,
                            FSharpNavigableItem(glyph, ImmutableArray.Create(TaggedText(TextTags.Text, item.Name)), document, sourceSpan)
                        )
                }

            let! items = getNavigableItems document
            let! processed = items |> Seq.map processItem |> Async.Parallel
            return processed |> Array.choose id
        }

    interface IFSharpNavigateToSearchService with
        member _.SearchProjectAsync
            (
                project,
                _priorityDocuments,
                searchPattern,
                kinds,
                cancellationToken
            ) : Task<ImmutableArray<FSharpNavigateToSearchResult>> =
            async {
                let tryMatch = createMatcherFor searchPattern

                let! results =
                    project.Documents
                    |> Seq.map (processDocument tryMatch kinds)
                    |> Async.Parallel

                return results |> Array.concat |> Array.toImmutableArray
            }
            |> RoslynHelpers.StartAsyncAsTask cancellationToken

        member _.SearchDocumentAsync
            (
                document: Document,
                searchPattern,
                kinds,
                cancellationToken
            ) : Task<ImmutableArray<FSharpNavigateToSearchResult>> =
            async {
                let! result = processDocument (createMatcherFor searchPattern) kinds document
                return result |> Array.toImmutableArray
            }
            |> RoslynHelpers.StartAsyncAsTask cancellationToken

        member _.KindsProvided = kindsProvided

        member _.CanFilter = true
