// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

/// Functions to compute the edit distance between two strings
module internal Internal.Utilities.EditDistance

/// Computes the DamerauLevenstein distance
///  - read more at https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance
///  - Implementation taken from http://www.navision-blog.de/2008/11/01/damerau-levenshtein-distance-in-fsharp-part-ii/
let private calcDamerauLevenshtein (a:string, b:string) =
    let m = b.Length + 1
    let mutable lastLine = Array.init m (fun i -> i)
    let mutable lastLastLine = Array.create m 0
    let mutable actLine = Array.create m 0
      
    for i in [1..a.Length] do
        actLine.[0] <- i
        for j in [1..b.Length] do          
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