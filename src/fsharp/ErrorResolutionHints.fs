// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Functions to format error message details
module internal Microsoft.FSharp.Compiler.ErrorResolutionHints

open Internal.Utilities

let maxSuggestions = 5
let minThresholdForSuggestions = 0.7
let highConfidenceThreshold = 0.85
let minStringLengthForThreshold = 3

/// We report a candidate if its edit distance is <= the threshold.
/// The threshold is set to about a quarter of the number of characters.
let IsInEditDistanceProximity idText suggestion =
    let editDistance = EditDistance.CalcEditDistance(idText, suggestion)
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

    let demangle (nm:string) =
        if nm.StartsWith "( " && nm.EndsWith " )" then
            let cleanName = nm.[2..nm.Length - 3]
            cleanName
        else nm

    /// Returns `true` if given string is an operator display name, e.g. ( |>> )
    let IsOperatorName (name: string) =
        if not (name.StartsWith "( " && name.EndsWith " )") then false else
        let name =  name.[2..name.Length - 3]
        let res = name |> Seq.forall (fun c -> c <> ' ')
        res        

    if allSuggestions.Contains idText then [] else // some other parsing error occurred
    allSuggestions
    |> Seq.choose (fun suggestion ->
        // Because beginning a name with _ is used both to indicate an unused
        // value as well as to formally squelch the associated compiler
        // error/warning (FS1182), we remove such names from the suggestions,
        // both to prevent accidental usages as well as to encourage good taste
        if IsOperatorName suggestion || suggestion.StartsWith "_" then None else
        let suggestion:string = demangle suggestion
        let suggestedText = suggestion.ToUpperInvariant()
        let similarity = EditDistance.JaroWinklerDistance uppercaseText suggestedText
        if similarity >= highConfidenceThreshold || suggestion.EndsWith ("." + idText) then
            Some(similarity, suggestion)
        elif similarity < minThresholdForSuggestions && suggestedText.Length > minStringLengthForThreshold then
            None
        elif IsInEditDistanceProximity uppercaseText suggestedText then
            Some(similarity, suggestion)
        else
            None)
    |> Seq.sortByDescending fst
    |> Seq.mapi (fun i x -> i, x) 
    |> Seq.takeWhile (fun (i, _) -> i < maxSuggestions) 
    |> Seq.map snd 
    |> Seq.toList

/// Formats the given predictions according to the error style.
let FormatPredictions normalizeF (predictions: (float * string) list) =
    match predictions with
    | [] -> System.String.Empty
    | _ ->
        let suggestions =
            predictions
            |> List.map (snd >> normalizeF) 
            |> List.map (sprintf "%s   %s" System.Environment.NewLine)
            |> String.concat ""

        " " + FSComp.SR.undefinedNameSuggestionsIntro() + suggestions
