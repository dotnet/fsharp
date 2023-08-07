// #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: int
#light

let isZero x =
    match x with
     | 0  -> true
     | 99 -> false
     | _  -> false

exit (if isZero 0 then 0 else 1)
