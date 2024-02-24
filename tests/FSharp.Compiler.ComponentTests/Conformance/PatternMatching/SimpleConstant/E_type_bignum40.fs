// #Regression #Conformance #PatternMatching #Constants  
// Pattern Matching - Simple Constants
// Type: BigNum
//<Expects id="FS0720" span="(10,7-10,13)" status="error">Non-primitive numeric literal constants.+</Expects>
// On NetFx4.0/Dev10, we don't give the deprecation error: this is ok (FSHARP1.0:4599)
#light

let isZero x =
    match x with
    | 99999N -> false
    | 0N -> true
    | _ -> false

exit (if isZero 0N then 0 else 1)
