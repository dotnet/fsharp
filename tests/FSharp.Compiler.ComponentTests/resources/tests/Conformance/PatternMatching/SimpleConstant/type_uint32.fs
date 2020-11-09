// #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: uint32
#light

let isZero x =
    match x with
    | 0u -> true
    | 99u -> false
    | _ -> false

exit (if isZero 0u then 0 else 1)
