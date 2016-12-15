// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.IO
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices
open System.Globalization

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.NavigateTo
open Microsoft.CodeAnalysis.Navigation

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

type internal NavigableItem(document: Document, sourceSpan: TextSpan, glyph: Glyph, name: string, kind: string, additionalInfo: string) =
    interface INavigableItem with
        member __.Glyph = glyph
        /// The tagged parts to display for this item. If default, the line of text from <see cref="Document"/> is used.
        member __.DisplayTaggedParts = [| TaggedText(TextTags.Text, name) |].ToImmutableArray()
        /// Return true to display the file path of <see cref="Document"/> and the span of <see cref="SourceSpan"/> when displaying this item.
        member __.DisplayFileLocation = true
        /// This is intended for symbols that are ordinary symbols in the language sense, and may be used by code, but that are simply declared 
        /// implicitly rather than with explicit language syntax.  For example, a default synthesized constructor in C# when the class contains no
        /// explicit constructors.
        member __.IsImplicitlyDeclared = false
        member __.Document = document
        member __.SourceSpan = sourceSpan
        member __.ChildItems = ImmutableArray<INavigableItem>.Empty
    member __.Name = name
    member __.Kind = kind
    member __.AdditionalInfo = additionalInfo

type internal NavigateToSearchResult(item: NavigableItem, matchKind: NavigateToMatchKind) =
    interface INavigateToSearchResult with
        member __.AdditionalInformation = item.AdditionalInfo
        member __.Kind = item.Kind
        member __.MatchKind = matchKind
        member __.IsCaseSensitive = false
        member __.Name = item.Name
        member __.NameMatchSpans = ImmutableArray<_>.Empty
        member __.SecondarySort = null
        member __.Summary = null
        member __.NavigableItem = upcast item

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
        abstract Find: searchValue: string -> INavigateToSearchResult []

    let build (items: seq<NavigableItem>) =
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
                  let result = ResizeArray()
                  if entries.Count > 0 then 
                     let entryToFind = IndexEntry(searchValue, 0, Unchecked.defaultof<_>, Unchecked.defaultof<_>)
                     
                     let initial = 
                         let p = entries.BinarySearch(entryToFind, indexEntryComparer)
                         if p < 0 then ~~~p else p
                     
                     let handle index =
                         let entry = entries.[index]
                         let matchKind = 
                             if entry.Offset = 0 then
                                 if entry.Length = searchValue.Length then NavigateToMatchKind.Exact
                                 else NavigateToMatchKind.Prefix
                             else NavigateToMatchKind.Substring
                         let item = entry.Item
                         result.Add (NavigateToSearchResult(item, matchKind) :> INavigateToSearchResult)
                    
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
                  result.ToArray() }

module private Utils =
    let navigateToItemKindToRoslynKind = function
        | NavigateTo.NavigableItemKind.Module -> NavigateToItemKind.Module
        | NavigateTo.NavigableItemKind.ModuleAbbreviation -> NavigateToItemKind.Module
        | NavigateTo.NavigableItemKind.Exception -> NavigateToItemKind.Class
        | NavigateTo.NavigableItemKind.Type -> NavigateToItemKind.Class
        | NavigateTo.NavigableItemKind.ModuleValue -> NavigateToItemKind.Field
        | NavigateTo.NavigableItemKind.Field -> NavigateToItemKind.Field
        | NavigateTo.NavigableItemKind.Property -> NavigateToItemKind.Property
        | NavigateTo.NavigableItemKind.Constructor -> NavigateToItemKind.Method
        | NavigateTo.NavigableItemKind.Member -> NavigateToItemKind.Method
        | NavigateTo.NavigableItemKind.EnumCase -> NavigateToItemKind.EnumItem
        | NavigateTo.NavigableItemKind.UnionCase -> NavigateToItemKind.EnumItem

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

[<ExportLanguageService(typeof<INavigateToSearchService>, FSharpCommonConstants.FSharpLanguageName); Shared>]
type internal FSharpNavigateToSearchService 
    [<ImportingConstructor>] 
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =

    let itemsByDocumentId = ConditionalWeakTable<DocumentId, (int * Index.IIndexedNavigableItems)>()

    let getNavigableItems(document: Document, options: FSharpProjectOptions, cancellationToken: CancellationToken) =
        async {
            let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
            let! parseResults = checkerProvider.Checker.ParseFileInProject(document.FilePath, sourceText.ToString(), options)
            return 
                match parseResults.ParseTree |> Option.map NavigateTo.getNavigableItems with
                | Some items ->
                    [| for item in items do
                         let sourceSpan = CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, item.Range)
                         let glyph = Utils.navigateToItemKindToGlyph item.Kind
                         let kind = Utils.navigateToItemKindToRoslynKind item.Kind
                         let additionalInfo = Utils.containerToString item.Container document.Project
                         yield NavigableItem(document, sourceSpan, glyph, item.Name, kind, additionalInfo) |]
                | None -> [||]
        }

    let getCachedIndexedNavigableItems(document: Document, options: FSharpProjectOptions, cancellationToken: CancellationToken) =
        async {
            let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask
            let textVersionHash = hash textVersion
            match itemsByDocumentId.TryGetValue document.Id with
            | true, (oldTextVersionHash, items) when oldTextVersionHash = textVersionHash ->
                return items
            | _ ->
                let! items = getNavigableItems(document, options, cancellationToken)
                let indexedItems = Index.build items
                itemsByDocumentId.Add(document.Id, (textVersionHash, indexedItems))
                return indexedItems
        }

    interface INavigateToSearchService with
        member __.SearchProjectAsync(project, searchPattern, cancellationToken) : Task<ImmutableArray<INavigateToSearchResult>> =
            async {
                match projectInfoManager.TryGetOptionsForProject(project.Id) with
                | Some options ->
                    let! items =
                        project.Documents
                        |> Seq.map (fun document -> getCachedIndexedNavigableItems(document, options, cancellationToken))
                        |> Async.Parallel

                    return (items |> Array.map (fun items -> items.Find(searchPattern)) |> Array.concat).ToImmutableArray()

                | None -> return ImmutableArray<INavigateToSearchResult>.Empty
            } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)


        member __.SearchDocumentAsync(document, searchPattern, cancellationToken) : Task<ImmutableArray<INavigateToSearchResult>> =
            async {
                let! options = projectInfoManager.TryGetOptionsForDocumentOrProject(document)
                match options with
                | Some options ->
                    let! items = getCachedIndexedNavigableItems(document, options, cancellationToken)
                    return items.Find(searchPattern).ToImmutableArray()
                | None -> return ImmutableArray<INavigateToSearchResult>.Empty
            } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)