// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text
open Hints

module InlineTypeHints =
    
    let private getHintParts
        (symbol: FSharpMemberOrFunctionOrValue) 
        (symbolUse: FSharpSymbolUse) =
        
        match symbol.GetReturnTypeLayout symbolUse.DisplayContext with
        | Some typeInfo -> 
            let colon = TaggedText(TextTag.Text, ": ")
            colon :: (typeInfo |> Array.toList)
        
        // not sure when this can happen
        | None -> 
            []

    let private getHint symbol (symbolUse: FSharpSymbolUse) =
        {
            Kind = HintKind.TypeHint
            Range = symbolUse.Range.EndRange
            Parts = getHintParts symbol symbolUse
        }

    let isValidForHint 
        (parseFileResults: FSharpParseFileResults) 
        (symbol: FSharpMemberOrFunctionOrValue)
        (symbolUse: FSharpSymbolUse) =
        
        let isNotAnnotatedManually = 
            not (parseFileResults.IsTypeAnnotationGivenAtPosition symbolUse.Range.Start)

        let isNotAfterDot = 
            symbolUse.IsFromDefinition 
            && not symbol.IsMemberThisValue

        let isNotTypeAlias = 
            not symbol.IsConstructorThisValue
        
        symbol.IsValue // we'll be adding other stuff gradually here
        && isNotAnnotatedManually
        && isNotAfterDot
        && isNotTypeAlias

    let getHints symbol symbolUse = 
        [ getHint symbol symbolUse ]
