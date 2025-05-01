// #Regression #Conformance #PatternMatching #Constants  
// Pattern Matching - Simple Constants
// Type: BigInt

#light

let isZero x =
    match x with
    | 0I -> true
    | 99I -> false
    | _ -> false

exit (if isZero 0I then 0 else 1)
