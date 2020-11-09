// #Regression #Conformance #PatternMatching #Unions 
#light

// Verify error if not all union pattern rules capture the
// same set of values
//<Expects id="FS0018" status="error">The two sides of this 'or' pattern bind different sets of variables</Expects>

let tupeMatch x =
    match x with
    | (x, 0) | (0, x) | (0, 0) -> true
    | _ -> false

exit 1
