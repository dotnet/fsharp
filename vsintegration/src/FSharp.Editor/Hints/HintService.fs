// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open System

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.Symbols
open Hints
open CancellableTasks
open Microsoft.VisualStudio.FSharp.Editor.Telemetry

module HintService =

    let semanticClassificationCache =
        new DocumentCache<NativeHint list>("fsharp-hints-cache")

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

        hintKinds |> Set.toList |> List.map getHintsPerKind

    let private getHintsForSymbol (sourceText: SourceText) parseResults hintKinds (symbol, symbolUses) =
        let hints = getHints sourceText parseResults hintKinds symbolUses symbol
        Seq.concat hints

    let getHintsForDocument sourceText (document: Document) hintKinds userOpName =
        cancellableTask {
            if isSignatureFile document.FilePath then
                return List.empty
            else
                let hintKindsSerialized = hintKinds |> Set.map Hints.serialize |> String.concat ","

                match! semanticClassificationCache.TryGetValueAsync document with
                | ValueSome nativeHints ->
                    do
                        TelemetryReporter.ReportSingleEvent(
                            TelemetryEvents.Hints,
                            [| ("hints.kinds", hintKindsSerialized); ("cacheHit", true) |]
                        )

                    return nativeHints
                | ValueNone ->
                    do
                        TelemetryReporter.ReportSingleEvent(
                            TelemetryEvents.Hints,
                            [| ("hints.kinds", hintKindsSerialized); ("cacheHit", false) |]
                        )

                    let! cancellationToken = CancellableTask.getCurrentCancellationToken ()
                    let! parseResults, checkResults = document.GetFSharpParseAndCheckResultsAsync userOpName

                    let nativeHints =
                        checkResults.GetAllUsesOfAllSymbolsInFile cancellationToken
                        |> Seq.groupBy (fun symbolUse -> symbolUse.Symbol)
                        |> Seq.collect (getHintsForSymbol sourceText parseResults hintKinds)
                        |> Seq.toList

                    do! semanticClassificationCache.SetAsync(document, nativeHints)

                    return nativeHints
        }
