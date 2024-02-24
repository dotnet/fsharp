// #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: double
#light

let isZero x =
    match x with
    | 1.f      -> true
    | 1.01f    -> false
    | 1.01e10f -> false
    | _        -> false

exit (if isZero 1.f then 0 else 1)
