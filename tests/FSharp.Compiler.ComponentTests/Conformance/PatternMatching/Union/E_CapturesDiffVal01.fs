// #Regression #Conformance #PatternMatching #Unions 
#light

// Verify error if two pattern match clauses match
// different values
//<Expects id="FS0018" status="error">The two sides of this 'or' pattern bind different sets of variables</Expects>

let testMatch x =
    match x with
    | (x, 0, 0) | (0, y, 0) | (0, 0, z) -> true
    | _ -> false

exit 1
