// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open FSharp.Compiler.Text

module HintService =

    // Relatively convenient for testing
    type NativeHint = {
        Range: range
        Parts: TaggedText list
    }

    let private isValidForHint 
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

    let private getHintParts
        (symbol: FSharpMemberOrFunctionOrValue) 
        (symbolUse: FSharpSymbolUse) =
        
        match symbol.GetReturnTypeLayout symbolUse.DisplayContext with
        | Some typeInfo -> 
            let colon = TaggedText(TextTag.Text, ": ")
            colon :: (typeInfo |> Array.toList)
        
        // not sure when this can happen but better safe than sorry
        | None -> 
            []
        
    let private getHintsForSymbol parseResults (symbolUse: FSharpSymbolUse) =
        match symbolUse.Symbol with
        | :? FSharpMemberOrFunctionOrValue as mfvSymbol 
          when isValidForHint parseResults mfvSymbol symbolUse ->
            
            [ {
                Range = symbolUse.Range
                Parts = getHintParts mfvSymbol symbolUse
            } ]
        
        // we'll be adding other stuff gradually here
        | _ -> 
            []

    let getHintsForDocument (document: Document) userOpName cancellationToken = 
        task {
            if isSignatureFile document.FilePath
            then 
                return []
            else
                let! parseResults, checkResults = 
                    document.GetFSharpParseAndCheckResultsAsync userOpName 
                
                return 
                    checkResults.GetAllUsesOfAllSymbolsInFile cancellationToken
                    |> Seq.toList
                    |> List.collect (getHintsForSymbol parseResults)
        }
