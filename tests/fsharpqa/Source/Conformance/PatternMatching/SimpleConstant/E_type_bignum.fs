// #Regression #Conformance #PatternMatching #Constants #NETFX20Only 
// Pattern Matching - Simple Constants
// Type: BigNum
//<Expects status="error" span="(9,7-9,13)" id="FS0720">Non-primitive numeric literal constants cannot be used in pattern matches because they can be mapped to multiple different types through the use of a NumericLiteral module\. Consider using replacing with a variable, and use 'when <variable> = <constant>' at the end of the match clause\.$</Expects>
//<Expects status="error" span="(13,17-13,19)" id="FS0784">This numeric literal requires that a module 'NumericLiteralN' defining functions FromZero, FromOne, FromInt32, FromInt64 and FromString be in scope$</Expects>

let isZero x =
    match x with
    | 99999N -> false
    | 0N -> true
    | _ -> false

exit (if isZero 0N then 0 else 1)

