// #Regression #Conformance #PatternMatching #Unions 


// Verify error if two pattern match clauses match
// different values


let testMatch x =
    match x with
    | (x, 0, 0) | (0, y, 0) | (0, 0, z) -> true
    | _ -> false

exit 1
