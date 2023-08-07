// #Regression #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: byte[]
// This is a regression test for FSHARP1.0:2036
#light

let isZero x =
    match x with
    | ""B -> true
    | @"\"B -> false
    | "ASCII"B -> false
    | _ -> false

exit (if isZero ""B then 0 else 1)
