// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal Internal.Utilities.EditDistance

/// Calculates the Jaro-Winkler edit distance between two strings.
/// The edit distance is a metric that allows to measure the amount of similarity between two strings.
val JaroWinklerDistance: s1: string -> s2: string -> float

/// Calculates the edit distance between two strings.
/// The edit distance is a metric that allows to measure the amount of difference between two strings
/// and shows how many edit operations (insert, delete, substitution) are needed to transform one string into the other.
val CalculateEditDistance: a: string * b: string -> int
