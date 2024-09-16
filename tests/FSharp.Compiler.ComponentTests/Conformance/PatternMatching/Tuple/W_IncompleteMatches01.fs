// #Regression #Conformance #PatternMatching #Tuples 


// Verify warnings for incomplete pattern matches


let test() = 
    function
    | 1, _
    | 2, _
    | 3, _ -> 'a'
    | _, 0 -> 'b'

exit 0
