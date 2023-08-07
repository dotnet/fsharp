// #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: sbyte
#light

let isZero x =
    match x with
    | -20y -> true
    | 99y -> false
    | _ -> false

exit (if isZero -20y then 0 else 1)
