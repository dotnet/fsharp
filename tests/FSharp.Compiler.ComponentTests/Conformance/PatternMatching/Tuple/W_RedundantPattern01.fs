// #Regression #Conformance #PatternMatching #Tuples 
#light

// Verify warning with redundant pattern


let redPat () =
    function
    | 1, _, _ 
        -> true
    | 1, _, _
        -> true
    | _ -> false

exit 0
