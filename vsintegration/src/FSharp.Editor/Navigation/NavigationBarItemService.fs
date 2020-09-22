// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Generic
open System.Threading.Tasks

open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Navigation
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Notification
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Navigation

open FSharp.Compiler.SourceCodeServices

type internal NavigationBarSymbolItem(text, glyph, spans, childItems) =
    inherit FSharpNavigationBarItem(text, glyph, spans, childItems)

[<Export(typeof<IFSharpNavigationBarItemService>)>]
type internal FSharpNavigationBarItemService
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    
    static let userOpName = "NavigationBarItem"
    static let emptyResult: IList<FSharpNavigationBarItem> = upcast [||]

    interface IFSharpNavigationBarItemService with
        member __.GetItemsAsync(document, cancellationToken) : Task<IList<FSharpNavigationBarItem>> = 
            asyncMaybe {
                let! parsingOptions, _options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, cancellationToken, userOpName)
                let! sourceText = document.GetTextAsync(cancellationToken)
                let! parsedInput = checkerProvider.Checker.ParseDocument(document, parsingOptions, sourceText=sourceText, userOpName=userOpName)
                let navItems = FSharpNavigation.getNavigation parsedInput
                let rangeToTextSpan range = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range)
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
                                        NavigationBarSymbolItem(decl.Name, decl.RoslynGlyph, [| textSpan |], null) :> FSharpNavigationBarItem))
                            
                            NavigationBarSymbolItem(topLevelDecl.Declaration.Name, topLevelDecl.Declaration.RoslynGlyph, [| topLevelTextSpan |], childItems)
                            :> FSharpNavigationBarItem)) :> IList<_>
            } 
            |> Async.map (Option.defaultValue emptyResult)
            |> RoslynHelpers.StartAsyncAsTask(cancellationToken)
