// #Regression #Conformance #PatternMatching #Tuples 
#light

// Verify warning with redundant pattern
//<Expects id="FS0026" status="warning">This rule will never be matched</Expects>

let redPat () =
    function
    | 1, _, _ 
        -> true
    | 1, _, _
        -> true
    | _ -> false

exit 0
