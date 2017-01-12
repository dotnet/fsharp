// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Functions to format error message details
module internal Microsoft.FSharp.Compiler.ErrorResolutionHints

open Internal.Utilities

let maxSuggestions = 5
let minThresholdForSuggestions = 0.7
let highConfidenceThreshold = 0.85
let minStringLengthForThreshold = 3

/// We report a candidate if its edit distance is <= the threshold.
/// The threshhold is set to about a quarter of the number of characters.
let IsInEditDistanceProximity idText suggestion =
    let editDistance = EditDistance.CalcEditDistance(idText,suggestion)
    let threshold =
        match idText.Length with
        | x when x < 5 -> 1
        | x when x < 7 -> 2
        | x -> x / 4 + 1
    
    editDistance <= threshold

/// Filters predictions based on edit distance to the given unknown identifier.
let FilterPredictions (idText:string) (suggestionF:ErrorLogger.Suggestions) =    
    let uppercaseText = idText.ToUpperInvariant()
    let allSuggestions = suggestionF()

    if allSuggestions.Contains idText then [] else // some other parsing error occurred
    allSuggestions
    |> Seq.choose (fun suggestion ->
        if PrettyNaming.IsOperatorName suggestion then None else
        let suggestion:string = PrettyNaming.DemangleBacktickedName suggestion
        let suggestedText = suggestion.ToUpperInvariant()
        let similarity = EditDistance.JaroWinklerDistance uppercaseText suggestedText
        if similarity >= highConfidenceThreshold || suggestion.EndsWith ("." + idText) then
            Some(similarity,suggestion)
        elif similarity < minThresholdForSuggestions && suggestedText.Length > minStringLengthForThreshold then
            None
        elif IsInEditDistanceProximity uppercaseText suggestedText then
            Some(similarity,suggestion)
        else
            None)
    |> Seq.sortByDescending fst
    |> Seq.mapi (fun i x -> i,x) 
    |> Seq.takeWhile (fun (i,_) -> i < maxSuggestions) 
    |> Seq.map snd 
    |> Seq.toList

/// Formats the given predictions according to the error style.
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
