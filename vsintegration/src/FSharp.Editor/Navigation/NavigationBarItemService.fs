// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Generic
open System.Threading.Tasks

open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor

open FSharp.Compiler.EditorServices
open CancellableTasks

type internal NavigationBarSymbolItem(text, glyph, spans, childItems) =
    inherit FSharpNavigationBarItem(text, glyph, spans, childItems)

[<Export(typeof<IFSharpNavigationBarItemService>)>]
type internal FSharpNavigationBarItemService [<ImportingConstructor>] () =

    static let emptyResult: IList<FSharpNavigationBarItem> = upcast [||]

    interface IFSharpNavigationBarItemService with
        member _.GetItemsAsync(document, cancellationToken) : Task<IList<FSharpNavigationBarItem>> =
            cancellableTask {

                let! cancellationToken = CancellableTask.getCancellationToken ()

                let! parseResults = document.GetFSharpParseResultsAsync(nameof (FSharpNavigationBarItemService))

                let navItems = Navigation.getNavigation parseResults.ParseTree
                let! sourceText = document.GetTextAsync(cancellationToken)

                let rangeToTextSpan range =
                    RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range)

                if navItems.Declarations.Length = 0 then
                    return emptyResult
                else

                    return
                        navItems.Declarations
                        |> Array.chooseV (fun topLevelDecl ->
                            rangeToTextSpan (topLevelDecl.Declaration.Range)
                            |> ValueOption.map (fun topLevelTextSpan ->
                                let childItems =
                                    topLevelDecl.Nested
                                    |> Array.chooseV (fun decl ->
                                        rangeToTextSpan (decl.Range)
                                        |> ValueOption.map (fun textSpan ->
                                            NavigationBarSymbolItem(decl.LogicalName, decl.RoslynGlyph, [| textSpan |], null)
                                            :> FSharpNavigationBarItem))

                                NavigationBarSymbolItem(
                                    topLevelDecl.Declaration.LogicalName,
                                    topLevelDecl.Declaration.RoslynGlyph,
                                    [| topLevelTextSpan |],
                                    childItems
                                )
                                :> FSharpNavigationBarItem))
                        :> IList<_>
            }
            |> CancellableTask.start cancellationToken
