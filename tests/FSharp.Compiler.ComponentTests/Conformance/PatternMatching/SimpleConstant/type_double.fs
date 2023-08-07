// #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: double
#light

let isZero x =
    match x with
    | 1.      -> false
    | 1.01    -> false
    | 1.01e10 -> true
    | _       -> false

exit (if isZero 1.01e10 then 0 else 1)
