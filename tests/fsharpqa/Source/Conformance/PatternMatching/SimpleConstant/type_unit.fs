// #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: unit
#light

let isZero x =
    match x with
    | () -> true
    | _ -> false

exit (if isZero () then 0 else 1)
