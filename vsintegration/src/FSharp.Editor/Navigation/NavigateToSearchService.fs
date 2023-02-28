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
open Microsoft.VisualStudio.Text.PatternMatching

open FSharp.Compiler.EditorServices
open FSharp.Compiler.Syntax
open Microsoft.VisualStudio.LanguageServices

[<Export(typeof<IFSharpNavigateToSearchService>)>]
type internal FSharpNavigateToSearchService 
    [<ImportingConstructor>] 
    (
        patternMatcherFactory: IPatternMatcherFactory,
        workspace: VisualStudioWorkspace
    ) =

    let cache = ConcurrentDictionary<DocumentId, VersionStamp * NavigableItem array>()

    do workspace.WorkspaceChanged.Add <| fun e -> if e.NewSolution.Id <> e.OldSolution.Id then cache.Clear()

    let getNavigableItems (document: Document) = async {
        let! currentVersion = document.GetTextVersionAsync() |> Async.AwaitTask
        match cache.TryGetValue document.Id with
        | true, (version, items) when version = currentVersion ->
            return items
        | _ ->  
            let! parseResults = document.GetFSharpParseResultsAsync(nameof(FSharpNavigateToSearchService))
            let items = parseResults.ParseTree |> NavigateTo.GetNavigableItems
            cache[document.Id] <- currentVersion, items
            return items
    }

    let kindsProvided = ImmutableHashSet.Create(FSharpNavigateToItemKind.Module, FSharpNavigateToItemKind.Class, FSharpNavigateToItemKind.Field, FSharpNavigateToItemKind.Property, FSharpNavigateToItemKind.Method, FSharpNavigateToItemKind.Enum, FSharpNavigateToItemKind.EnumItem) :> IImmutableSet<string>

    let navigateToItemKindToRoslynKind = function
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

    let navigateToItemKindToGlyph = function
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

    let containerToString (container: NavigableContainer) (document: Document) =
        let typeAsString =
            match container.Type with
            | NavigableContainerType.File -> "project"
            | NavigableContainerType.Namespace -> "namespace"
            | NavigableContainerType.Module -> "module"
            | NavigableContainerType.Exception -> "exception"
            | NavigableContainerType.Type -> "type"
        let name =
            match container.Type with
            | NavigableContainerType.File ->
                $"{Path.GetFileNameWithoutExtension document.Project.Name}, {Path.GetFileName container.LogicalName}"
            | _ -> container.LogicalName

        if document.IsFSharpSignatureFile then $"signature for: {typeAsString} {name}" else $"{typeAsString} {name}"

    let patternMatchKindToNavigateToMatchKind = function
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
            lazy patternMatcherFactory.CreatePatternMatcher(searchPattern, PatternMatcherCreationOptions(
                cultureInfo = CultureInfo.CurrentUICulture,
                flags = (PatternMatcherCreationFlags.AllowFuzzyMatching ||| PatternMatcherCreationFlags.AllowSimpleSubstringMatching)
            ))
        // PatternMatcher will not match operators and some backtick escaped identifiers.
        // To handle them, do a simple substring match first.
        fun (name: string) ->
            match name.IndexOf(searchPattern, StringComparison.CurrentCultureIgnoreCase) with
            | i when i > 0 ->
                PatternMatch(PatternMatchKind.Substring, false, true) |> Some
            | 0 when name.Length = searchPattern.Length ->
                PatternMatch(PatternMatchKind.Exact, false, true) |> Some
            | 0 ->
                PatternMatch(PatternMatchKind.Prefix, false, true) |> Some
            | _ ->
                patternMatcher.Value.TryMatch name |> Option.ofNullable

    let processDocument (document: Document) (tryMatch: string -> PatternMatch option) (kinds: IImmutableSet<string>) =
        async {
            let! sourceText = document.GetTextAsync Async.DefaultCancellationToken |> Async.AwaitTask

            let processItem item = asyncMaybe {
                do! Option.guard (kinds.Contains(navigateToItemKindToRoslynKind item.Kind))
                let name = 
                    // This conversion back from logical names to display names should never be needed, because
                    // the FCS API should only report display names in the first place.
                    if PrettyNaming.IsLogicalOpName item.LogicalName then 
                        PrettyNaming.ConvertValLogicalNameToDisplayNameCore item.LogicalName
                    else 
                        item.LogicalName
                let! m = tryMatch name
                let! sourceSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, item.Range)
                let glyph = navigateToItemKindToGlyph item.Kind
                let kind = navigateToItemKindToRoslynKind item.Kind
                let additionalInfo = containerToString item.Container document
                return FSharpNavigateToSearchResult(additionalInfo, kind, patternMatchKindToNavigateToMatchKind m.Kind, name, 
                    FSharpNavigableItem(glyph, ImmutableArray.Create (TaggedText(TextTags.Text, name)), document, sourceSpan))
            }

            let! items = getNavigableItems document 
            let! processed = items |> Seq.map (fun item -> processItem item) |> Async.Parallel
            return processed |> Array.choose id
        }

    interface IFSharpNavigateToSearchService with
        member _.SearchProjectAsync(project, _priorityDocuments, searchPattern, kinds, cancellationToken) : Task<ImmutableArray<FSharpNavigateToSearchResult>> =
            async {
                if not project.IsFSharp then raise (ArgumentException("Project is not a F# project", nameof project))
                let tryMatch = createMatcherFor searchPattern
                let! results =
                    seq { for document in project.Documents -> processDocument document tryMatch kinds }
                    |> Async.Parallel
                return results |> Array.concat |> Array.toImmutableArray
            } |> RoslynHelpers.StartAsyncAsTask cancellationToken

        member _.SearchDocumentAsync(document: Document, searchPattern, kinds, cancellationToken) : Task<ImmutableArray<FSharpNavigateToSearchResult>> =
            async { 
                let! result = processDocument document (createMatcherFor searchPattern) kinds
                return result |> Array.toImmutableArray
            } |> RoslynHelpers.StartAsyncAsTask cancellationToken

        member _.KindsProvided = kindsProvided

        member _.CanFilter = true