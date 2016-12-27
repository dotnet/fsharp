// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Functions to format error message details
module internal Microsoft.FSharp.Compiler.ErrorResolutionHints

let maxSuggestions = 5

let minStringLengthForThreshold = 3

let thresholdForSuggestions = 0.7

/// Filters predictions based on edit distance to the given unknown identifier.
let FilterPredictions (unknownIdent:string) (predictionsF:ErrorLogger.Suggestions) =    
    let unknownIdent = unknownIdent.ToUpperInvariant()
    let useThreshold = unknownIdent.Length >= minStringLengthForThreshold
    predictionsF()
    |> Seq.choose (fun p -> 
        let similarity = Internal.Utilities.EditDistance.JaroWinklerDistance unknownIdent (p.ToUpperInvariant())
        if not useThreshold || similarity >= thresholdForSuggestions then
            Some(similarity,p)
        else
            None
        )
    |> Seq.sortByDescending fst
    |> Seq.mapi (fun i x -> i,x) 
    |> Seq.takeWhile (fun (i,_) -> i < maxSuggestions) 
    |> Seq.map snd 
    |> Seq.toList

let FormatPredictions errorStyle normalizeF (predictions: (float * string) list) =
    match predictions with
    | [] -> System.String.Empty
    | _ ->
        " " + FSComp.SR.undefinedNameSuggestionsIntro() + 
        match errorStyle with
        | ErrorLogger.ErrorStyle.VSErrors ->
            let predictionText =
                predictions 
                |> List.map (snd >> normalizeF)
                |> String.concat ", "

            " " + predictionText
        | _ ->
            predictions
            |> List.map (snd >> normalizeF) 
            |> List.map (sprintf "%s   %s" System.Environment.NewLine)
            |> String.concat ""
