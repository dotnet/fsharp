// #Regression #Conformance #PatternMatching #Tuples 
// Verify error if tuple sizes mismatch

//<Expects id="FS0001" status="error" span="(9,7-9,15)">Type mismatch\. Expecting a.    'int \* string \* char'    .but given a.    'int \* string'    .The tuples have differing lengths of 3 and 2</Expects>

let test (x : int * string * char) =
    match x with
    |  0,  "1", '2' -> true
    | 10, "20"      -> true
    |     "-1", '0' -> true
    | 99,       '9' -> true
    | _ -> false

exit 1

