// #Regression #Conformance #PatternMatching #Constants  
// Pattern Matching - Simple Constants
// Type: BigInt
//<Expects id="FS0720" span="(9,7-9,9)" status="error">Non-primitive numeric literal constants.+</Expects>
#light

let isZero x =
    match x with
    | 0I -> true
    | 99I -> false
    | _ -> false

exit (if isZero 0I then 0 else 1)
