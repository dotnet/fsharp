// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open Hints

module HintService =
    let private getHintsForSymbol parseResults hintKinds (symbolUse: FSharpSymbolUse) =
        match symbolUse.Symbol with
        | :? FSharpMemberOrFunctionOrValue as symbol 
          when hintKinds |> Set.contains HintKind.TypeHint 
            && InlineTypeHints.isValidForHint parseResults symbol symbolUse ->
            
            InlineTypeHints.getHints symbol symbolUse
        
        | :? FSharpMemberOrFunctionOrValue as symbol
          when hintKinds |> Set.contains HintKind.ParameterNameHint 
            && InlineParameterNameHints.isMemberOrFunctionOrValueValidForHint symbol ->

            InlineParameterNameHints.getHintsForMemberOrFunctionOrValue parseResults symbol symbolUse

        | :? FSharpUnionCase as symbol
          when hintKinds |> Set.contains HintKind.ParameterNameHint
            && InlineParameterNameHints.isUnionCaseValidForHint symbol symbolUse ->

          InlineParameterNameHints.getHintsForUnionCase parseResults symbol symbolUse

        // we'll be adding other stuff gradually here
        | _ -> 
            []

    let getHintsForDocument (document: Document) hintKinds userOpName cancellationToken = 
        async {
            if isSignatureFile document.FilePath
            then 
                return []
            else
                let! parseResults, checkResults = 
                    document.GetFSharpParseAndCheckResultsAsync userOpName 
                
                return 
                    checkResults.GetAllUsesOfAllSymbolsInFile cancellationToken
                    |> Seq.toList
                    |> List.collect (getHintsForSymbol parseResults hintKinds)
        }
