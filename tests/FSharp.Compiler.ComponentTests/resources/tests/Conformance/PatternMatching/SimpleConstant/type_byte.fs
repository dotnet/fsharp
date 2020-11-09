// #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: byte
#light

let isZero x =
    match x with
    | 0uy -> true
    | 99uy -> false
    | _ -> false

exit (if isZero 0uy then 0 else 1)
