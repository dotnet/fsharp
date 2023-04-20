// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open Hints

type InlineTypeHints(parseResults: FSharpParseFileResults, symbol: FSharpMemberOrFunctionOrValue) =

    let getHintParts (symbol: FSharpMemberOrFunctionOrValue) (symbolUse: FSharpSymbolUse) =

        match symbol.GetReturnTypeLayout symbolUse.DisplayContext with
        | Some typeInfo ->
            let colon = TaggedText(TextTag.Text, ": ")
            colon :: (typeInfo |> Array.toList)

        // not sure when this can happen
        | None -> []

    let getTooltip _document _cancellationToken =
        async {
            // the hardest part
            return [ TaggedText(TextTag.Text, "42") ]
        // the hardest part
        }

    let getHint symbol (symbolUse: FSharpSymbolUse) =
        {
            Kind = HintKind.TypeHint
            Range = symbolUse.Range.EndRange
            Parts = getHintParts symbol symbolUse
            GetTooltip = getTooltip
        }

    let isSolved (symbol: FSharpMemberOrFunctionOrValue) =
        if symbol.GenericParameters.Count > 0 then
            symbol.GenericParameters |> Seq.forall (fun p -> p.IsSolveAtCompileTime)

        elif symbol.FullType.IsGenericParameter then
            symbol.FullType.GenericParameter.DisplayNameCore <> "?"

        else
            true

    let isValidForHint (symbolUse: FSharpSymbolUse) =

        let isOptionalParameter =
            symbolUse.IsFromDefinition
            && symbol.FullType.IsAbbreviation
            && symbol.FullType.TypeDefinition.DisplayName = "option"

        let adjustedRangeStart =
            if isOptionalParameter then
                // we need the position to start at the '?' symbol
                mkPos symbolUse.Range.StartLine (symbolUse.Range.StartColumn - 1)
            else
                symbolUse.Range.Start

        let isNotAnnotatedManually =
            not (parseResults.IsTypeAnnotationGivenAtPosition adjustedRangeStart)

        let isNotAfterDot = symbolUse.IsFromDefinition && not symbol.IsMemberThisValue

        let isNotTypeAlias = not symbol.IsConstructorThisValue

        symbol.IsValue // we'll be adding other stuff gradually here
        && isSolved symbol
        && isNotAnnotatedManually
        && isNotAfterDot
        && isNotTypeAlias

    member _.getHints symbolUse =
        [
            if isValidForHint symbolUse then
                getHint symbol symbolUse
        ]
