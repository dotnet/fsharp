// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Functions to format error message details
module internal FSharp.Compiler.ErrorResolutionHints

open Internal.Utilities
open FSharp.Compiler.AbstractIL.Internal.Library

let maxSuggestions = 5
let minThresholdForSuggestions = 0.7
let highConfidenceThreshold = 0.85
let minStringLengthForSuggestion = 3

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

/// Demangles a suggestion
let DemangleOperator (nm:string) =
    if nm.StartsWithOrdinal("( ") && nm.EndsWithOrdinal(" )") then
        nm.[2..nm.Length - 3]
    else 
        nm

type SuggestionBuffer(idText:string) = 
    let data = Array.zeroCreate<System.Collections.Generic.KeyValuePair<float,string>>(maxSuggestions)
    let uppercaseText = idText.ToUpperInvariant()
    let dotIdText = "." + idText

    let mutable elements = 0
    let mutable disableSuggestions = idText.Length < minStringLengthForSuggestion

    let isSmaller i k =
        if i >= elements then false else
        let kvpr : byref<_> = &data.[i] 
        kvpr.Key < k

    let insert (k,v) =
        let mutable pos = 0
        while pos < maxSuggestions && isSmaller pos k do
            pos <- pos + 1
        
        if pos < maxSuggestions then
            for i = maxSuggestions-1 downto (pos+1) do
                data.[i] <- data.[i-1]
            data.[pos] <- System.Collections.Generic.KeyValuePair(k,v)

        elements <- elements + 1

    member __.Values() : string [] =
        if disableSuggestions then 
            [||]
        else
            [| let hashSet = System.Collections.Generic.HashSet<string>()
               let bound = min (maxSuggestions-1) (elements-1)
               for i in 0..bound do
                    let x = data.[i].Value
                    if hashSet.Add x then yield x |]
    
    member __.Disabled with get () = disableSuggestions

    member __.Add (suggestion:string) =
        if not disableSuggestions then
            if suggestion = idText then // some other parse error happened
                disableSuggestions <- true

            // Because beginning a name with _ is used both to indicate an unused
            // value as well as to formally squelch the associated compiler
            // error/warning (FS1182), we remove such names from the suggestions,
            // both to prevent accidental usages as well as to encourage good taste
            if suggestion.Length >= minStringLengthForSuggestion && not (suggestion.StartsWithOrdinal "_") then
                let suggestion:string = DemangleOperator suggestion
                let suggestedText = suggestion.ToUpperInvariant()
                let similarity = EditDistance.JaroWinklerDistance uppercaseText suggestedText
                if similarity >= highConfidenceThreshold ||
                    suggestion.EndsWithOrdinal dotIdText ||
                    (similarity >= minThresholdForSuggestions && IsInEditDistanceProximity uppercaseText suggestedText)
                then
                    insert(similarity, suggestion)
