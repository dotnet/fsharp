// #Regression #Conformance #PatternMatching #Unions 
#light

// Verify error if not all union pattern rules capture the
// same set of values


let tupeMatch x =
    match x with
    | (x, 0) | (0, x) | (0, 0) -> true
    | _ -> false

exit 1
