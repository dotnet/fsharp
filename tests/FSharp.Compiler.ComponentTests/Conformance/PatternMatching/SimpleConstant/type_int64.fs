// #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: int64
#light

let isZero x =
    match x with
    | 0L   -> false
    | 99L  -> true
    | -99L -> false
    | _  -> false

exit (if isZero 99L then 0 else 1)

