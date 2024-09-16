// #Regression #Conformance #PatternMatching #ActivePatterns 
// Verify error if Active Patterns used with named parameters


let (|A|B|) n = if n%2 = 0 then A n else B n
match 10 with A(hoho=n) -> n | _ -> 0

exit 1
