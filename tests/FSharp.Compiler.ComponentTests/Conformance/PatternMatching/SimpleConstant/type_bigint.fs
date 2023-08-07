// #Conformance #PatternMatching #Constants  
// Pattern Matching - Simple Constants
// Type: BigInt
#light

let isZero x =
    match x with
    | y when y = 0I -> true    // notice: 0I -> true would yield an error
    | y when y = 99I -> false  // notice: 99I -> false would yield an error
    | _ -> false

exit (if isZero 0I then 0 else 1)
