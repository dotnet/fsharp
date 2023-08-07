// #Regression #Conformance #PatternMatching #ActivePatterns 
// Verify error if Active Patterns used with named parameters
//<Expects id="FS3210" status="error" span="(6,15)">A is an active pattern and cannot be treated as a discriminated union case with named fields.</Expects>

let (|A|B|) n = if n%2 = 0 then A n else B n
match 10 with A(hoho=n) -> n | _ -> 0

exit 1
