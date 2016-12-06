// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Linq
open System.Runtime.CompilerServices
open System.Windows
open System.Windows.Controls
open System.Windows.Media

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.QuickInfo
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.NavigateTo
open Microsoft.CodeAnalysis.Navigation

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Text.Formatting
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open System.Windows.Documents

type internal NavigableItem(document: Document, sourceSpan: TextSpan, glyph: Glyph) =
    interface INavigableItem with
        member __.Glyph = glyph
        /// The tagged parts to display for this item. If default, the line of text from <see cref="Document"/> is used.
        member __.DisplayTaggedParts = [].ToImmutableArray()
        /// Return true to display the file path of <see cref="Document"/> and the span of <see cref="SourceSpan"/> when displaying this item.
        member __.DisplayFileLocation = true
        /// This is intended for symbols that are ordinary symbols in the language sense, and may be used by code, but that are simply declared 
        /// implicitly rather than with explicit language syntax.  For example, a default synthesized constructor in C# when the class contains no
        /// explicit constructors.
        member __.IsImplicitlyDeclared = false
        member __.Document = document
        member __.SourceSpan = sourceSpan
        member __.ChildItems = [].ToImmutableArray()

type internal NavigateToSearchResult(name: string, item: INavigableItem, kind: string) =
    interface INavigateToSearchResult with
        member __.AdditionalInformation = ""
        member __.Kind = kind
        member __.MatchKind = NavigateToMatchKind.Exact
        member __.IsCaseSensitive = false
        member __.Name = name
        member __.SecondarySort = ""
        member __.Summary = "summary"
        member __.NavigableItem = item

module private Utils =
    let fsharpNavigableItemKindToRoslynKind = function
        | FSharpNavigableItemKind.Module -> NavigateToItemKind.Module
        | FSharpNavigableItemKind.ModuleAbbreviation -> NavigateToItemKind.Module
        | FSharpNavigableItemKind.Exception -> NavigateToItemKind.Class
        | FSharpNavigableItemKind.Type -> NavigateToItemKind.Class
        | FSharpNavigableItemKind.ModuleValue -> NavigateToItemKind.Field
        | FSharpNavigableItemKind.Field -> NavigateToItemKind.Field
        | FSharpNavigableItemKind.Property -> NavigateToItemKind.Property
        | FSharpNavigableItemKind.Constructor -> NavigateToItemKind.Method
        | FSharpNavigableItemKind.Member -> NavigateToItemKind.Method
        | FSharpNavigableItemKind.EnumCase -> NavigateToItemKind.EnumItem
        | FSharpNavigableItemKind.UnionCase -> NavigateToItemKind.EnumItem

    let fsharpNavigableItemKindToGlyph = function
        | FSharpNavigableItemKind.Module -> Glyph.ModulePublic
        | FSharpNavigableItemKind.ModuleAbbreviation -> Glyph.ModulePublic
        | FSharpNavigableItemKind.Exception -> Glyph.ClassPublic
        | FSharpNavigableItemKind.Type -> Glyph.ClassPublic
        | FSharpNavigableItemKind.ModuleValue -> Glyph.FieldPublic
        | FSharpNavigableItemKind.Field -> Glyph.FieldPublic
        | FSharpNavigableItemKind.Property -> Glyph.PropertyPublic
        | FSharpNavigableItemKind.Constructor -> Glyph.MethodPublic
        | FSharpNavigableItemKind.Member -> Glyph.MethodPublic
        | FSharpNavigableItemKind.EnumCase -> Glyph.EnumPublic
        | FSharpNavigableItemKind.UnionCase -> Glyph.EnumPublic

[<ExportLanguageService(typeof<INavigateToSearchService>, FSharpCommonConstants.FSharpLanguageName); Shared>]
type internal FSharpNavigateToSearchService 
    [<ImportingConstructor>] 
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =

    static member GetNavigateToSearchResult(filePath: string, sourceText: SourceText, checker: FSharpChecker, options: FSharpProjectOptions) =
        async {
            let! parseResults = checker.ParseFileInProject(filePath, sourceText.ToString(), options)
            match parseResults.ParseTree with
            | Some parseTree -> return NavigableItemsCollector.collect filePath parseTree
            | None -> return Seq.empty
        }

    interface INavigateToSearchService with
        member __.SearchProjectAsync(project, _searchPattern, cancellationToken) : Task<ImmutableArray<INavigateToSearchResult>> =
            async {
                match projectInfoManager.TryGetOptionsForProject(project.Id) with
                | Some options ->
                    let! items =
                        project.Documents
                        |> Seq.map (fun document -> 
                            async {
                                let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                                let! results = FSharpNavigateToSearchService.GetNavigateToSearchResult(document.FilePath, sourceText, checkerProvider.Checker, options)
                                return document, sourceText, results
                            })
                        |> Async.Parallel
                    
                    return
                        (items
                         |> Seq.collect (fun (document, sourceText, items: seq<InternalNavigableItem>) ->
                             items |> Seq.map (fun item ->
                                let navigableItem = NavigableItem(document, CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, item.Range),
                                                                  Utils.fsharpNavigableItemKindToGlyph item.Kind)
                                NavigateToSearchResult(item.Name, navigableItem, Utils.fsharpNavigableItemKindToRoslynKind item.Kind) :> INavigateToSearchResult))
                        ).ToImmutableArray()

                | None -> return [].ToImmutableArray()
            } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)


        member __.SearchDocumentAsync(_document, _searchPattern, _cancellationToken) : Task<ImmutableArray<INavigateToSearchResult>> =
            Task.FromResult([].ToImmutableArray())