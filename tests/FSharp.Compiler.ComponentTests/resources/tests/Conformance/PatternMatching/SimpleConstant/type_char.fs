// #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: char
#light

let isZero x =
    match x with
     | 'a' -> true
     | '\\' -> false
     | _ -> false

exit (if isZero 'a' then 0 else 1)
