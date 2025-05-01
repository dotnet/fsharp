// #Regression #Conformance #PatternMatching #Constants 
#light

// Test ability to match a const literal value




[<Literal>]
let intLiteral  = 42

if (match 42 with 
    | intLiteral -> true
    | _          -> false) <> true then exit 1

[<Literal>]
let strLiteral  = "foobaz"

if (match "foobaz" with 
    | strLiteral -> true
    | _          -> false) <> true then exit 1

[<Literal>]
let boolLiteral = false

if (match false with 
    | boolLiteral -> true
    | true        -> false) <> true then exit 1

exit 0
