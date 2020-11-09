// #Regression #Conformance #PatternMatching #Records 
#light

// Verify error if type of a record field is incorrect.
//<Expects id="FS0039" status="error">The record label 'X' is not defined</Expects>

let testMatch x =
    match x with
        | { X = 0; Y = 0} -> true
        | _ -> false
        
exit 1
