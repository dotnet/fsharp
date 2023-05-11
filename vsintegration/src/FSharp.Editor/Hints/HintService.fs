// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.Symbols
open Hints
open CancellableTasks

module HintService =

    let private getHints sourceText parseResults hintKinds symbolUses (symbol: FSharpSymbol) =

        let getHintsPerKind hintKind =
            match hintKind, symbol with
            | HintKind.TypeHint, (:? FSharpMemberOrFunctionOrValue as symbol) ->
                symbolUses |> Seq.collect (InlineTypeHints(parseResults, symbol)).GetHints
            | HintKind.ReturnTypeHint, (:? FSharpMemberOrFunctionOrValue as symbol) ->
                symbolUses |> Seq.collect (InlineReturnTypeHints(parseResults, symbol).GetHints)
            | HintKind.ParameterNameHint, (:? FSharpMemberOrFunctionOrValue as symbol) ->
                symbolUses
                |> Seq.collect (InlineParameterNameHints(parseResults).GetHintsForMemberOrFunctionOrValue sourceText symbol)
            | HintKind.ParameterNameHint, (:? FSharpUnionCase as symbol) ->
                symbolUses
                |> Seq.collect (InlineParameterNameHints(parseResults).GetHintsForUnionCase symbol)
            | _ -> []

        let rec loop hintKinds acc =
            match hintKinds with
            | [] -> acc
            | hintKind :: hintKinds -> (getHintsPerKind hintKind) :: (loop hintKinds acc)

        loop (hintKinds |> Set.toList) []

    let private getHintsForSymbol (sourceText: SourceText) parseResults hintKinds (symbol, symbolUses) =
        let hints = getHints sourceText parseResults hintKinds symbolUses symbol
        Seq.concat hints

    let getHintsForDocument sourceText (document: Document) hintKinds userOpName =
        cancellableTask {
            if isSignatureFile document.FilePath then
                return []
            else
                let! cancellationToken = CancellableTask.getCurrentCancellationToken ()
                let! parseResults, checkResults = document.GetFSharpParseAndCheckResultsAsync userOpName

                return
                    checkResults.GetAllUsesOfAllSymbolsInFile cancellationToken
                    |> Seq.groupBy (fun symbolUse -> symbolUse.Symbol)
                    |> Seq.collect (getHintsForSymbol sourceText parseResults hintKinds)
                    |> Seq.toList
        }
