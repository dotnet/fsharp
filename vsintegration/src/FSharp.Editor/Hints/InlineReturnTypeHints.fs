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

    let getHint symbolUse range =
        getHintParts symbolUse
        |> Option.map (fun parts ->
            {
                Kind = HintKind.ReturnTypeHint
                Range = range
                Parts = parts
            })

    member _.getHints(symbolUse: FSharpSymbolUse) =
        [
            if symbol.IsFunction then
                yield!
                    parseFileResults.TryRangeOfReturnTypeHint symbolUse.Range.Start
                    |> Option.bind (getHint symbolUse)
                    |> Option.toList
        ]
