// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Functions to compute the edit distance between two strings
module internal Internal.Utilities.EditDistance

/// Given an offset and a radius from that offset, 
/// does mChar exist in that part of str?
let inline existsInWin (mChar: char) (str: string) offset rad =
    let startAt = max 0 (offset - rad)
    let endAt = min (offset + rad) (String.length str - 1)  
    if endAt - startAt < 0 then false
    else
        let rec exists index =
            if str.[index] = mChar then true
            elif index = endAt then false
            else exists (index + 1)
        exists startAt

/// The jaro distance between s1 and s2
let jaro s1 s2 =    
    // The radius is half of the lesser 
    // of the two string lengths rounded up.
    let matchRadius = 
        let minLen = 
            min (String.length s1) (String.length s2) in
              minLen / 2 + minLen % 2

    // An inner function which recursively finds the number  
    // of matched characters within the radius.
    let commonChars (chars1: string) (chars2: string) =
        let rec inner i result = 
            match i with
            | -1 -> result
            | _ -> if existsInWin chars1.[i] chars2 i matchRadius
                   then inner (i - 1) (chars1.[i] :: result)
                   else inner (i - 1) result
        inner (chars1.Length - 1) []

    // The sets of common characters and their lengths as floats 
    let c1 = commonChars s1 s2
    let c2 = commonChars s2 s1
    let c1length = float (List.length c1)
    let c2length = float (List.length c2)
    
    // The number of transpositions within 
    // the sets of common characters.
    let transpositions = 
        let rec inner cl1 cl2 result = 
            match cl1, cl2 with
            | [], _ | _, [] -> result
            | c1h :: c1t, c2h :: c2t -> 
                if c1h <> c2h
                then inner c1t c2t (result + 1.0)
                else inner c1t c2t result
        let mismatches = inner c1 c2 0.0
        // If one common string is longer than the other
        // each additional char counts as half a transposition
        (mismatches + abs (c1length - c2length)) / 2.0

    let s1length = float (String.length s1)
    let s2length = float (String.length s2)
    let tLength = max c1length c2length

    // The jaro distance as given by 
    // 1/3 ( m2/|s1| + m1/|s2| + (mc-t)/mc )
    let result = (c1length / s1length +
                  c2length / s2length + 
                  (tLength - transpositions) / tLength)
                 / 3.0

    // This is for cases where |s1|, |s2| or m are zero 
    if System.Double.IsNaN result then 0.0 else result

/// Calculates the Jaro-Winkler edit distance between two strings.
/// The edit distance is a metric that allows to measure the amount of similarity between two strings.
let JaroWinklerDistance s1 s2 = 
    let jaroScore = jaro s1 s2
    // Accumulate the number of matching initial characters
    let maxLength = (min s1.Length s2.Length) - 1
    let rec calcL i acc =
        if i > maxLength || s1.[i] <> s2.[i] then acc
        else calcL (i + 1) (acc + 1.0)
    let l = min (calcL 0 0.0) 4.0
    // Calculate the JW distance
    let p = 0.1
    jaroScore + (l * p * (1.0 - jaroScore))

/// Computes the restricted Damerau-Levenstein edit distance,
/// also known as the "optimal string alignment" distance.
///  - read more at https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance
///  - Implementation taken from http://www.navision-blog.de/2008/11/01/damerau-levenshtein-distance-in-fsharp-part-ii/
let private calcDamerauLevenshtein (a:string, b:string) =
    let m = b.Length + 1
    let mutable lastLine = Array.init m id
    let mutable lastLastLine = Array.zeroCreate m
    let mutable actLine = Array.zeroCreate m

    for i in 1 .. a.Length do
        actLine.[0] <- i
        for j in 1 .. b.Length do
            let cost = if a.[i-1] = b.[j-1] then 0 else 1
            let deletion = lastLine.[j] + 1
            let insertion = actLine.[j-1] + 1
            let substitution = lastLine.[j-1] + cost
            actLine.[j] <- 
              deletion 
              |> min insertion 
              |> min substitution
  
            if i > 1 && j > 1 then
              if a.[i-1] = b.[j-2] && a.[i-2] = b.[j-1] then
                  let transposition = lastLastLine.[j-2] + cost  
                  actLine.[j] <- min actLine.[j] transposition
      
        // swap lines
        let temp = lastLastLine
        lastLastLine <- lastLine
        lastLine <- actLine
        actLine <- temp
              
    lastLine.[b.Length]

/// Calculates the edit distance between two strings.
/// The edit distance is a metric that allows to measure the amount of difference between two strings 
/// and shows how many edit operations (insert, delete, substitution) are needed to transform one string into the other.
let CalcEditDistance(a:string, b:string) =
    if a.Length > b.Length then
        calcDamerauLevenshtein(a, b)
    else
        calcDamerauLevenshtein(b, a)