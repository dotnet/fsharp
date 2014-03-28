// #Regression #Conformance #PatternMatching #Tuples 


// Verify warnings for incomplete pattern matches
//<Expects id="FS0025" span="(8,5)" status="warning">Incomplete pattern matches on this expression.</Expects>

let test() = 
    function
    | 1, _
    | 2, _
    | 3, _ -> 'a'
    | _, 0 -> 'b'

exit 0
