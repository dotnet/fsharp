// #Conformance #PatternMatching #Unions 
#light

// Test a match statement with just one 'match thingy'
type Foo = A | B of string | C of int

let test x = 
    match x with 
    | A
    | B _
    | C (_)
     -> true

if (test (A)) <> true then exit 1
if (test (B(""))) <> true then exit 1
if (test (C(0))) <> true then exit 1

exit 0
