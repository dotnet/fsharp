// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.IO
open System.Composition
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices
open System.Runtime.Caching
open System.Globalization

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.NavigateTo
open Microsoft.CodeAnalysis.Navigation
open Microsoft.CodeAnalysis.PatternMatching
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Navigation
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.NavigateTo

open FSharp.Compiler
open FSharp.Compiler.SourceCodeServices

type internal NavigableItem(document: Document, sourceSpan: TextSpan, glyph: Glyph, name: string, kind: string, additionalInfo: string) =
    inherit FSharpNavigableItem(glyph, ImmutableArray.Create (TaggedText(TextTags.Text, name)), document, sourceSpan)

    member __.Name = name
    member __.Kind = kind
    member __.AdditionalInfo = additionalInfo

type internal NavigateToSearchResult(item: NavigableItem, matchKind: FSharpNavigateToMatchKind) =
    inherit FSharpNavigateToSearchResult(item.AdditionalInfo, item.Kind, matchKind, item.Name, item)

module private Index =
    [<System.Diagnostics.DebuggerDisplay("{DebugString()}")>]
    type private IndexEntry(str: string, offset: int, item: NavigableItem, isOperator: bool) =
        member __.String = str
        member __.Offset = offset
        member __.Length = str.Length - offset
        member __.Item = item
        member __.IsOperator = isOperator
        member x.StartsWith (s: string) = 
            if s.Length > x.Length then false
            else CultureInfo.CurrentCulture.CompareInfo.IndexOf(str, s, offset, s.Length, CompareOptions.IgnoreCase) = offset
        member private __.DebugString() = sprintf "%s (offset %d) (%s)" (str.Substring offset) offset str

    let private indexEntryComparer =
        { new IComparer<IndexEntry> with
            member __.Compare(a, b) = 
                let res = CultureInfo.CurrentCulture.CompareInfo.Compare(a.String, a.Offset, b.String, b.Offset, CompareOptions.IgnoreCase)
                if res = 0 then a.Offset.CompareTo(b.Offset) else res }

    type IIndexedNavigableItems =
        abstract Find: searchValue: string -> FSharpNavigateToSearchResult []
        abstract AllItems: NavigableItem []

    let private navigateToSearchResultComparer =
        { new IEqualityComparer<FSharpNavigateToSearchResult> with 
            member __.Equals(x: FSharpNavigateToSearchResult, y: FSharpNavigateToSearchResult) =
                match x, y with
                | null, _ | _, null -> false
                | _ -> x.NavigableItem.Document.Id = y.NavigableItem.Document.Id &&
                       x.NavigableItem.SourceSpan = y.NavigableItem.SourceSpan
            
            member __.GetHashCode(x: FSharpNavigateToSearchResult) =
                if isNull x then 0
                else 23 * (17 * 23 + x.NavigableItem.Document.Id.GetHashCode()) + x.NavigableItem.SourceSpan.GetHashCode() }

    let build (items: NavigableItem []) =
        let entries = ResizeArray()

        for item in items do
            let isOperator, name = 
                if PrettyNaming.IsMangledOpName item.Name then 
                    true, PrettyNaming.DecompileOpName item.Name 
                else 
                    false, item.Name
            for i = 0 to name.Length - 1 do
                entries.Add(IndexEntry(name, i, item, isOperator))

        entries.Sort(indexEntryComparer)
        { new IIndexedNavigableItems with
              member __.Find (searchValue) =
                  let result = HashSet(navigateToSearchResultComparer)
                  if entries.Count > 0 then 
                     let entryToFind = IndexEntry(searchValue, 0, Unchecked.defaultof<_>, Unchecked.defaultof<_>)
                     
                     let initial = 
                         let p = entries.BinarySearch(entryToFind, indexEntryComparer)
                         if p < 0 then ~~~p else p
                     
                     let handle index =
                         let entry = entries.[index]
                         let matchKind = 
                             if entry.Offset = 0 then
                                 if entry.Length = searchValue.Length then FSharpNavigateToMatchKind.Exact
                                 else FSharpNavigateToMatchKind.Prefix
                             else FSharpNavigateToMatchKind.Substring
                         let item = entry.Item
                         result.Add (NavigateToSearchResult(item, matchKind) :> FSharpNavigateToSearchResult) |> ignore
                    
                     // in case if there are multiple matching items binary search might return not the first one.
                     // in this case we'll walk backwards searching for the applicable answers
                     let mutable pos = initial
                     while pos >= 0 && pos < entries.Count && entries.[pos].StartsWith searchValue do
                         handle pos
                         pos <- pos - 1
                    
                     // value of 'initial' position was already handled on the previous step so here we'll bump it
                     let mutable pos = initial + 1
                     while pos < entries.Count && entries.[pos].StartsWith searchValue do
                         handle pos
                         pos <- pos + 1
                  Seq.toArray result 
              member __.AllItems = items }

[<AutoOpen>]
module private Utils =

    let navigateToItemKindToRoslynKind = function
        | NavigateTo.NavigableItemKind.Module -> FSharpNavigateToItemKind.Module
        | NavigateTo.NavigableItemKind.ModuleAbbreviation -> FSharpNavigateToItemKind.Module
        | NavigateTo.NavigableItemKind.Exception -> FSharpNavigateToItemKind.Class
        | NavigateTo.NavigableItemKind.Type -> FSharpNavigateToItemKind.Class
        | NavigateTo.NavigableItemKind.ModuleValue -> FSharpNavigateToItemKind.Field
        | NavigateTo.NavigableItemKind.Field -> FSharpNavigateToItemKind.Field
        | NavigateTo.NavigableItemKind.Property -> FSharpNavigateToItemKind.Property
        | NavigateTo.NavigableItemKind.Constructor -> FSharpNavigateToItemKind.Method
        | NavigateTo.NavigableItemKind.Member -> FSharpNavigateToItemKind.Method
        | NavigateTo.NavigableItemKind.EnumCase -> FSharpNavigateToItemKind.EnumItem
        | NavigateTo.NavigableItemKind.UnionCase -> FSharpNavigateToItemKind.EnumItem

    let navigateToItemKindToGlyph = function
        | NavigateTo.NavigableItemKind.Module -> Glyph.ModulePublic
        | NavigateTo.NavigableItemKind.ModuleAbbreviation -> Glyph.ModulePublic
        | NavigateTo.NavigableItemKind.Exception -> Glyph.ClassPublic
        | NavigateTo.NavigableItemKind.Type -> Glyph.ClassPublic
        | NavigateTo.NavigableItemKind.ModuleValue -> Glyph.FieldPublic
        | NavigateTo.NavigableItemKind.Field -> Glyph.FieldPublic
        | NavigateTo.NavigableItemKind.Property -> Glyph.PropertyPublic
        | NavigateTo.NavigableItemKind.Constructor -> Glyph.MethodPublic
        | NavigateTo.NavigableItemKind.Member -> Glyph.MethodPublic
        | NavigateTo.NavigableItemKind.EnumCase -> Glyph.EnumPublic
        | NavigateTo.NavigableItemKind.UnionCase -> Glyph.EnumPublic

    let containerToString (container: NavigateTo.Container) (project: Project) =
        let typeAsString =
            match container.Type with
            | NavigateTo.ContainerType.File -> "project "
            | NavigateTo.ContainerType.Namespace -> "namespace "
            | NavigateTo.ContainerType.Module -> "module "
            | NavigateTo.ContainerType.Exception -> "exception "
            | NavigateTo.ContainerType.Type -> "type "
        let name =
            match container.Type with
            | NavigateTo.ContainerType.File ->
                (Path.GetFileNameWithoutExtension project.Name) + ", " + (Path.GetFileName container.Name)
            | _ -> container.Name
        typeAsString + name

    type PerDocumentSavedData = { Hash: int; Items: Index.IIndexedNavigableItems }

[<Export(typeof<IFSharpNavigateToSearchService>)>]
type internal FSharpNavigateToSearchService 
    [<ImportingConstructor>] 
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =

    let userOpName = "FSharpNavigateToSearchService"
    let kindsProvided = ImmutableHashSet.Create(FSharpNavigateToItemKind.Module, FSharpNavigateToItemKind.Class, FSharpNavigateToItemKind.Field, FSharpNavigateToItemKind.Property, FSharpNavigateToItemKind.Method, FSharpNavigateToItemKind.Enum, FSharpNavigateToItemKind.EnumItem) :> IImmutableSet<string>

    // Save the backing navigation data in a memory cache held in a sliding window
    let itemsByDocumentId = new MemoryCache("FSharp.Editor.FSharpNavigateToSearchService")

    let getNavigableItems(document: Document, parsingOptions: FSharpParsingOptions, kinds: IImmutableSet<string>) =
        async {
            let! cancellationToken = Async.CancellationToken
            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
            let! parseResults = checkerProvider.Checker.ParseFile(document.FilePath, sourceText.ToFSharpSourceText(), parsingOptions)

            let navItems parsedInput =
                NavigateTo.getNavigableItems parsedInput
                |> Array.filter (fun i -> kinds.Contains(navigateToItemKindToRoslynKind i.Kind))

            return 
                match parseResults.ParseTree |> Option.map navItems with
                | Some items ->
                    [| for item in items do
                         match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, item.Range) with 
                         | None -> ()
                         | Some sourceSpan ->
                             let glyph = navigateToItemKindToGlyph item.Kind
                             let kind = navigateToItemKindToRoslynKind item.Kind
                             let additionalInfo = containerToString item.Container document.Project
                             yield NavigableItem(document, sourceSpan, glyph, item.Name, kind, additionalInfo) |]
                | None -> [||]
        }

    let getCachedIndexedNavigableItems(document: Document, parsingOptions: FSharpParsingOptions, kinds: IImmutableSet<string>) =
        async {
            let! cancellationToken = Async.CancellationToken
            let! textVersion = document.GetTextVersionAsync(cancellationToken)  |> Async.AwaitTask
            let textVersionHash = hash textVersion
            let key = document.Id.ToString()
            match itemsByDocumentId.Get(key) with
            | :? PerDocumentSavedData as data when data.Hash = textVersionHash -> return data.Items
            | _ -> 
                let! items = getNavigableItems(document, parsingOptions, kinds)
                let indexedItems = Index.build items
                let data = { Hash= textVersionHash; Items = indexedItems }
                let cacheItem = CacheItem(key, data)
                let policy = CacheItemPolicy(SlidingExpiration=DefaultTuning.PerDocumentSavedDataSlidingWindow)
                itemsByDocumentId.Set(cacheItem, policy)
                return indexedItems }

    let patternMatchKindToNavigateToMatchKind = function
        | PatternMatchKind.Exact -> FSharpNavigateToMatchKind.Exact
        | PatternMatchKind.Prefix -> FSharpNavigateToMatchKind.Prefix
        | PatternMatchKind.Substring -> FSharpNavigateToMatchKind.Substring
        | PatternMatchKind.CamelCase -> FSharpNavigateToMatchKind.Regular
        | PatternMatchKind.Fuzzy -> FSharpNavigateToMatchKind.Regular
        | _ -> FSharpNavigateToMatchKind.Regular

    interface IFSharpNavigateToSearchService with
        member __.SearchProjectAsync(project, _priorityDocuments, searchPattern, kinds, cancellationToken) : Task<ImmutableArray<FSharpNavigateToSearchResult>> =
            asyncMaybe {
                let! parsingOptions, _options = projectInfoManager.TryGetOptionsByProject(project, cancellationToken)
                let! items =
                    project.Documents
                    |> Seq.map (fun document -> getCachedIndexedNavigableItems(document, parsingOptions, kinds))
                    |> Async.Parallel
                    |> liftAsync
                
                let items =
                    if searchPattern.Length = 1 then
                        items 
                        |> Array.map (fun items -> items.Find(searchPattern))
                        |> Array.concat
                        |> Array.filter (fun x -> x.Name.Length = 1 && String.Equals(x.Name, searchPattern, StringComparison.InvariantCultureIgnoreCase))
                    else
                        [| yield! items |> Array.map (fun items -> items.Find(searchPattern)) |> Array.concat
                           use patternMatcher = new PatternMatcher(searchPattern, allowFuzzyMatching = true)
                           yield! items
                                  |> Array.collect (fun item -> item.AllItems)
                                  |> Array.Parallel.collect (fun x -> 
                                      patternMatcher.GetMatches(x.Name)
                                      |> Seq.map (fun pm ->
                                          NavigateToSearchResult(x, patternMatchKindToNavigateToMatchKind pm.Kind) :> FSharpNavigateToSearchResult)
                                      |> Seq.toArray) |]

                return items |> Array.distinctBy (fun x -> x.NavigableItem.Document.Id, x.NavigableItem.SourceSpan)
            } 
            |> Async.map (Option.defaultValue [||])
            |> Async.map Seq.toImmutableArray
            |> RoslynHelpers.StartAsyncAsTask(cancellationToken)

        member __.SearchDocumentAsync(document, searchPattern, kinds, cancellationToken) : Task<ImmutableArray<FSharpNavigateToSearchResult>> =
            asyncMaybe {
                let! parsingOptions, _, _ = projectInfoManager.TryGetOptionsForDocumentOrProject(document, cancellationToken, userOpName)
                let! items = getCachedIndexedNavigableItems(document, parsingOptions, kinds) |> liftAsync
                return items.Find(searchPattern)
            }
            |> Async.map (Option.defaultValue [||])
            |> Async.map Seq.toImmutableArray
            |> RoslynHelpers.StartAsyncAsTask(cancellationToken)

        member __.KindsProvided = kindsProvided

        member __.CanFilter = true