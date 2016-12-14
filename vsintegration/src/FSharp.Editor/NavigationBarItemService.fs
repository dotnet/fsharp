// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Generic
open System.Collections.Immutable
open System.Linq
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Host
open Microsoft.CodeAnalysis.Navigation
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Notification

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text

open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Ast

type internal NavigationBarSymbolItem(text, glyph, spans, childItems) =
    inherit NavigationBarItem(text, glyph, spans, childItems)

[<ExportLanguageService(typeof<INavigationBarItemService>, FSharpCommonConstants.FSharpLanguageName); Shared>]
type internal FSharpNavigationBarItemService
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: ProjectInfoManager
    ) =
    
    static let emptyResult: IList<NavigationBarItem> = upcast [||]

    interface INavigationBarItemService with
        member __.GetItemsAsync(document, cancellationToken) : Task<IList<NavigationBarItem>> = 
            async {
                match projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)  with 
                | Some options ->
                    let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                    let! fileParseResults = checkerProvider.Checker.ParseFileInProject(document.FilePath, sourceText.ToString(), options)
                    match fileParseResults.ParseTree with
                    | Some parsedInput ->
                        let navItems = NavigationImpl.getNavigation parsedInput
                        let rangeToTextSpan range = 
                            try Some(CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, range))
                            with _ -> None
                        return 
                            navItems.Declarations
                            |> Array.choose (fun topLevelDecl ->
                                rangeToTextSpan(topLevelDecl.Declaration.Range)
                                |> Option.map (fun topLevelTextSpan ->
                                    let childItems =
                                        topLevelDecl.Nested
                                        |> Array.choose (fun decl ->
                                            rangeToTextSpan(decl.Range)
                                            |> Option.map(fun textSpan ->
                                                NavigationBarSymbolItem(
                                                    decl.Name, 
                                                    CommonHelpers.glyphMajorToRoslynGlyph(decl.GlyphMajor), 
                                                    [| textSpan |],
                                                    null)
                                                :> NavigationBarItem))
                                    
                                    NavigationBarSymbolItem(
                                        topLevelDecl.Declaration.Name, 
                                        CommonHelpers.glyphMajorToRoslynGlyph(topLevelDecl.Declaration.GlyphMajor),
                                        [| topLevelTextSpan |],
                                        childItems)
                                    :> NavigationBarItem)) :> IList<_>
                    | None -> return emptyResult
                | None -> return emptyResult
            } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
        
        member __.ShowItemGrayedIfNear (_item) : bool = false
        
        member __.NavigateToItem(document, item, _view, _cancellationToken) =
            match item.Spans |> Seq.tryHead with
            | Some span ->
                let workspace = document.Project.Solution.Workspace
                let navigationService = workspace.Services.GetService<IDocumentNavigationService>()
                
                if navigationService.CanNavigateToPosition(workspace, document.Id, span.Start) then
                    navigationService.TryNavigateToPosition(workspace, document.Id, span.Start) |> ignore
                else
                    let notificationService = workspace.Services.GetService<INotificationService>()
                    notificationService.SendNotification(EditorFeaturesResources.The_definition_of_the_object_is_hidden, severity = NotificationSeverity.Error)
            | None -> ()