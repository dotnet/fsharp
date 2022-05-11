// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Functions to compute the edit distance between two strings
module internal Internal.Utilities.EditDistance

open System

/// Given an offset and a radius from that offset, does mChar exist in that part of str?
let inline existsInWin (mChar: char) (str: string) (offset: int) (rad: int) =
    let startAt = Math.Max(0, offset - rad)
    let endAt = Math.Min(offset + rad, str.Length - 1)  
    if endAt - startAt < 0 then false
    else
        let rec exists index =
            if str[index] = mChar then true
            elif index = endAt then false
            else exists (index + 1)
        exists startAt
    
let jaro (s1: string) (s2: string) =    
    // The radius is half of the lesser of the two string lengths rounded up.
    let matchRadius = 
        let minLen = Math.Min(s1.Length, s2.Length)
        minLen / 2 + minLen % 2

    let rec nextChar (s1:string) (s2:string) i c =
        if i < s1.Length then
            let c = s1[i]
            if not (existsInWin c s2 i matchRadius) then
                nextChar s1 s2 (i + 1) c
            else
                struct (i, c)
        else
            struct (i, c)

    // The sets of common characters and their lengths as floats         
    // The number of transpositions within the sets of common characters.
    let struct (transpositions, c1length, c2length) =
        let rec loop i j mismatches c1length c2length =
            if i < s1.Length && j < s2.Length then
                let struct (ti, ci) = nextChar s1 s2 i ' '
                let struct (tj, cj) = nextChar s2 s1 j ' '
                if ci <> cj then
                    loop (ti + 1) (tj + 1) (mismatches + 1) (c1length + 1) (c2length + 1)
                else
                    loop (ti + 1) (tj + 1) mismatches (c1length + 1) (c2length + 1)
            else struct (i, j, mismatches, c1length, c2length)

        let struct (i, j, mismatches, c1length, c2length) = loop 0 0 0 0 0

        let rec loop (s1:string) (s2:string) i length =
            if i < s1.Length - 1 then
                let c = s1[i]
                if existsInWin c s2 i matchRadius then 
                    loop s1 s2 (i + 1) (length + 1)
                else
                    loop s1 s2 (i + 1) length
            else
                length
        let c1length = loop s1 s2 i c1length |> float
        let c2length = loop s2 s1 j c2length |> float

        struct ((float mismatches + abs (c1length - c2length)) / 2.0, c1length, c2length)
    
    let tLength = Math.Max(c1length, c2length)
    
    // The jaro distance as given by 1/3 ( m2/|s1| + m1/|s2| + (mc-t)/mc )
    let result = (c1length / float s1.Length + c2length / float s2.Length + (tLength - transpositions) / tLength) / 3.0
    
    // This is for cases where |s1|, |s2| or m are zero 
    if Double.IsNaN result then 0.0 else result

/// Calculates the Jaro-Winkler edit distance between two strings.
/// The edit distance is a metric that allows to measure the amount of similarity between two strings.
let JaroWinklerDistance s1 s2 = 
    let jaroScore = jaro s1 s2
    // Accumulate the number of matching initial characters
    let maxLength = (min s1.Length s2.Length) - 1
    let rec calcL i acc =
        if i > maxLength || s1[i] <> s2[i] then acc
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
        actLine[0] <- i
        for j in 1 .. b.Length do
            let cost = if a[i-1] = b[j-1] then 0 else 1
            let deletion = lastLine[j] + 1
            let insertion = actLine[j-1] + 1
            let substitution = lastLine[j-1] + cost
            actLine[j] <- 
              deletion 
              |> min insertion 
              |> min substitution
  
            if i > 1 && j > 1 then
              if a[i-1] = b[j-2] && a[i-2] = b[j-1] then
                  let transposition = lastLastLine[j-2] + cost  
                  actLine[j] <- min actLine[j] transposition
      
        // swap lines
        let temp = lastLastLine
        lastLastLine <- lastLine
        lastLine <- actLine
        actLine <- temp
              
    lastLine[b.Length]

/// Calculates the edit distance between two strings.
/// The edit distance is a metric that allows to measure the amount of difference between two strings 
/// and shows how many edit operations (insert, delete, substitution) are needed to transform one string into the other.
let CalculateEditDistance(a:string, b:string) =
    if a.Length > b.Length then
        calcDamerauLevenshtein(a, b)
    else
        calcDamerauLevenshtein(b, a)