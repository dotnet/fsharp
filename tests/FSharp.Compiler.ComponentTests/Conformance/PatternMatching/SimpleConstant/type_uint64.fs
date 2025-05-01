// #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: uint64
#light

let isZero x =
    match x with
    | 0UL           -> false
    | 9999999999999UL -> true
    | _ -> false

exit (if isZero 9999999999999UL then 0 else 1)
