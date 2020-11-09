// #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: nativeint
#light

let isZero x =
    match x with
    | 0n        -> false
    | 99999999n -> true
    | _         -> false

exit (if isZero 99999999n then 0 else 1)
