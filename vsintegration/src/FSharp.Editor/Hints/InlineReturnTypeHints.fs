// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open Hints

type InlineReturnTypeHints(parseFileResults: FSharpParseFileResults, symbol: FSharpMemberOrFunctionOrValue) =

    let getHintParts (symbolUse: FSharpSymbolUse) =
        symbol.GetReturnTypeLayout symbolUse.DisplayContext
        |> Option.map (fun typeInfo ->
            [
                TaggedText(TextTag.Text, ": ")
                yield! typeInfo |> Array.toList
                TaggedText(TextTag.Space, " ")
            ])

    let getTooltip _ =
        async {
            let typeAsString = symbol.ReturnParameter.Type.TypeDefinition.ToString()
            let text = $"type {typeAsString}"
            return [ TaggedText(TextTag.Text, text) ]
        }

    let getHint symbolUse range =
        getHintParts symbolUse
        |> Option.map (fun parts ->
            {
                Kind = HintKind.ReturnTypeHint
                Range = range
                Parts = parts
                GetTooltip = getTooltip
            })

    let isValidForHint (symbol: FSharpMemberOrFunctionOrValue) = symbol.IsFunction

    member _.getHints(symbolUse: FSharpSymbolUse) =
        [
            if isValidForHint symbol then
                yield!
                    parseFileResults.TryRangeOfReturnTypeHint symbolUse.Range.Start
                    |> Option.bind (getHint symbolUse)
                    |> Option.toList
        ]
