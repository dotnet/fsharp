// #Conformance #PatternMatching #Constants #RequiresPowerPack #NETFX20Only 
// Pattern Matching - Simple Constants
// Type: BigNum
#light

let isZero x =
    match x with
    | y when y = 0N -> true
    | y when y = 99999N -> false
    | _ -> false

exit (if isZero 0N then 0 else 1)
