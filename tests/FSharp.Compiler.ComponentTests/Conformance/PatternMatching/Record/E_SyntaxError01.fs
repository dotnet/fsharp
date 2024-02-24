// #Regression #Conformance #PatternMatching #Records 
// Verify syntax error
//<Expects status="error" id="FS0010">Unexpected symbol '}' in pattern\. Expected '\.', '=' or other token\.$</Expects>

type RecordType = { Value : int }

let test x = 
    match x with
    | { Value} -> true
    | _ -> false

exit 1
