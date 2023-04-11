// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.Symbols
open Hints

module HintService =

    let private getHints sourceText parseResults hintKinds symbolUses (symbol: FSharpSymbol) =
        let rec getHints hintKinds acc =
            match hintKinds with
            | [] -> acc
            | hintKind :: hintKinds ->
                match (hintKind, symbol) with
                | HintKind.TypeHint, (:? FSharpMemberOrFunctionOrValue as symbol) ->
                    symbolUses 
                    |> Seq.collect (InlineTypeHints(parseResults, symbol)).getHints
                | HintKind.ReturnTypeHint, (:? FSharpMemberOrFunctionOrValue as symbol) ->
                    symbolUses 
                    |> Seq.collect (InlineReturnTypeHints(parseResults, symbol).getHints)
                | HintKind.ParameterNameHint, (:? FSharpMemberOrFunctionOrValue as symbol) ->
                    symbolUses 
                    |> Seq.collect (InlineParameterNameHints(parseResults).getHintsForMemberOrFunctionOrValue sourceText symbol)
                | HintKind.ParameterNameHint, (:? FSharpUnionCase as symbol) ->
                    symbolUses 
                    |> Seq.collect (InlineParameterNameHints(parseResults).getHintsForUnionCase symbol)
                | _ -> []
                :: acc
                |> getHints hintKinds

        getHints (hintKinds |> Set.toList) []

    let private getHintsForSymbol (sourceText: SourceText) parseResults hintKinds (symbol, symbolUses) =
        symbol
        |> getHints sourceText parseResults hintKinds symbolUses
        |> Seq.concat

    let getHintsForDocument sourceText (document: Document) hintKinds userOpName cancellationToken =
        async {
            if isSignatureFile document.FilePath then
                return []
            else
                let! parseResults, checkResults = document.GetFSharpParseAndCheckResultsAsync userOpName

                return
                    checkResults.GetAllUsesOfAllSymbolsInFile cancellationToken
                    |> Seq.groupBy (fun symbolUse -> symbolUse.Symbol)
                    |> Seq.collect (getHintsForSymbol sourceText parseResults hintKinds)
                    |> Seq.toList
        }
