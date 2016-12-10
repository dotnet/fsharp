// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Functions to format error message details
module internal Microsoft.FSharp.Compiler.ErrorResolutionHints

/// Filters predictions based on edit distance to an unknown identifier.
let FilterPredictions unknownIdent (allPredictions:ErrorLogger.Predictions) =
    let rec take n predictions = 
        predictions 
        |> Seq.mapi (fun i x -> i,x) 
        |> Seq.takeWhile (fun (i,_) -> i < n) 
        |> Seq.map snd 
        |> Seq.toList

    allPredictions
    |> Seq.sortByDescending (Internal.Utilities.EditDistance.JaroWinklerDistance unknownIdent)
    |> take 5

let FormatPredictions errorStyle predictions =
    match predictions with
    | [] -> System.String.Empty
    | _ ->
        match errorStyle with
        | ErrorLogger.ErrorStyle.VSErrors ->
            let predictionText =
                predictions 
                |> String.concat ", "

            " " + FSComp.SR.undefinedNameRecordLabelDetails() + " " + predictionText
        | _ ->
            let predictionText =
                    predictions 
                    |> Seq.map (sprintf "%s   %s" System.Environment.NewLine) 
                    |> String.concat ""

            System.Environment.NewLine + FSComp.SR.undefinedNameRecordLabelDetails() + predictionText
