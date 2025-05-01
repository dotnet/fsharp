// #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: uint16
#light

let isZero x =
    match x with
    | 0us -> true
    | 99us -> false
    | _ -> false

exit (if isZero 0us then 0 else 1)
