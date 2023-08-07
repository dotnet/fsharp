// #Conformance #PatternMatching #Constants 
// Pattern Matching - Simple Constants
// Type: Bool
#light

let isZero x =
    match x with
    | true -> true
    | false -> false
    | _ -> failwith "What?"

exit (if (isZero true) && (not (isZero false)) then 0 else 1)
