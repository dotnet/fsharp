// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.Hints

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Symbols
open Hints

module HintService =

    type private NativeHintResolver = FSharpSymbolUse seq -> NativeHint seq

    let inline private getTypeHints parseResults symbol: NativeHintResolver =
        Seq.filter (InlineTypeHints.isValidForHint parseResults symbol) 
        >> Seq.collect (InlineTypeHints.getHints symbol)

    let inline private getHintsForMemberOrFunctionOrValue parseResults symbol: NativeHintResolver =
        Seq.filter (InlineParameterNameHints.isMemberOrFunctionOrValueValidForHint symbol)
        >> Seq.collect (InlineParameterNameHints.getHintsForMemberOrFunctionOrValue parseResults symbol)

    let inline private getHintsForUnionCase parseResults symbol: NativeHintResolver =
        Seq.filter (InlineParameterNameHints.isUnionCaseValidForHint symbol) 
        >> Seq.collect (InlineParameterNameHints.getHintsForUnionCase parseResults symbol)

    let private getHintResolvers parseResults hintKinds (symbol: FSharpSymbol): NativeHintResolver seq = 
        let rec resolve hintKinds resolvers =
            match hintKinds with
            | [] -> resolvers |> Seq.choose id
            | hintKind :: hintKinds ->
                match hintKind with
                | HintKind.TypeHint -> 
                    match symbol with
                    | :? FSharpMemberOrFunctionOrValue as symbol -> getTypeHints parseResults symbol |> Some
                    | _ -> None
                | HintKind.ParameterNameHint ->
                    match symbol with
                    | :? FSharpMemberOrFunctionOrValue as symbol -> getHintsForMemberOrFunctionOrValue parseResults symbol |> Some
                    | :? FSharpUnionCase as symbol -> getHintsForUnionCase parseResults symbol |> Some
                    | _ -> None
                // we'll be adding other stuff gradually here
                :: resolvers |> resolve hintKinds

        in resolve hintKinds []
        
    let private getHintsForSymbol parseResults hintKinds (symbol: FSharpSymbol, symbolUses: FSharpSymbolUse seq) =
        symbol 
        |> getHintResolvers parseResults hintKinds 
        |> Seq.collect (fun resolve -> resolve symbolUses)

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
                    |> Seq.groupBy (fun symbolUse -> symbolUse.Symbol)
                    |> Seq.collect (getHintsForSymbol parseResults (hintKinds |> Set.toList))
                    |> Seq.toList
        }
