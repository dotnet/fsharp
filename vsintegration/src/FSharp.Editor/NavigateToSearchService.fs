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
            | Some parsedInput -> return NavigateTo.getNavigableItems parsedInput
            | None -> return [||]
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
                        [| for document, sourceText, items in items do
                             for item in items do
                                 let sourceSpan = CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, item.Range)
                                 let glyph = Utils.navigateToItemKindToGlyph item.Kind
                                 let navigableItem = NavigableItem(document, sourceSpan, glyph)
                                 let kind = Utils.navigateToItemKindToRoslynKind item.Kind
                                 yield NavigateToSearchResult(item.Name, navigableItem, kind) :> INavigateToSearchResult |]
                        .ToImmutableArray()

                | None -> return [].ToImmutableArray()
            } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)


        member __.SearchDocumentAsync(_document, _searchPattern, _cancellationToken) : Task<ImmutableArray<INavigateToSearchResult>> =
            Task.FromResult([].ToImmutableArray())