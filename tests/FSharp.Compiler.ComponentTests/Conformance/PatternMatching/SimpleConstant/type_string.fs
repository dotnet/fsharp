// #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: string
#light

let isZero x =
    match x with
    | "3" -> false
    | "c:\\home" -> false
    | @"c:\home" -> false
    | @"@" -> true
    | _ -> false

exit (if isZero @"@" then 0 else 1)
