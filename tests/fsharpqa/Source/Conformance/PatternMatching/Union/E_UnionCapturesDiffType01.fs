// #Regression #Conformance #PatternMatching #Unions
// Verify error if  two union rules capture values with different types
//<Expects id="FS0001" status="error" span="(8,7-9,11)">This expression was expected to have type.    'int'    .but here has type.    'float'</Expects>

let testMatch x =
    match x with
    | 0, 0.0 -> true
    | x, 0.0
    | 0, x -> false
    | _, _ -> false

