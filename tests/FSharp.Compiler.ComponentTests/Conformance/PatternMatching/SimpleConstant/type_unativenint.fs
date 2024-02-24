// #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: unativeint
#light

let isZero x =
    match x with
    | 0un -> true
    | 99un -> false
    | _ -> false

exit (if isZero 0un then 0 else 1)
