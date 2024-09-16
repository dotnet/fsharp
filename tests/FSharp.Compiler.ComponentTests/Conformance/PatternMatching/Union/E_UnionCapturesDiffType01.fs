// #Regression #Conformance #PatternMatching #Unions
// Verify error if  two union rules capture values with different types


let testMatch x =
    match x with
    | 0, 0.0 -> true
    | x, 0.0
    | 0, x -> false
    | _, _ -> false

