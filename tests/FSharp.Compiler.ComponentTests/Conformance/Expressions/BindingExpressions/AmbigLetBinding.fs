// #Regression #Conformance #Binding 
#light
// Regression test for FSHARP1.0:1917
// Verify that when given an ambiguous let binding we treat it as a function binding, rather than a pattern match.

type IdentDU = Ident of string

// Use pattern matching
let (Ident(a)) = Ident("foo")
if a <> "foo" then exit 1

let (Ident b) = Ident("bar")
if b <> "bar" then exit 1

// Use a function binding
let id x = x

// Ambiguous, but F# will resolve as function binding
// (Otherwise would be compile error that '42' is not a valid Ident.)
let Ident h = 42

// Now call that function
if (Ident ("tuple", 1)) <> 42 then 
    exit 1

exit 0
