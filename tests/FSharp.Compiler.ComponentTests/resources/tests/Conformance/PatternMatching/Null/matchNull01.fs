// #Conformance #PatternMatching 
#light

// Verify ability to match against null

let isNull x = 
    match x with
    | null -> true
    | _    -> false

if isNull ("abc" :> obj) <> false then exit 1
if isNull (12345 :> obj) <> false then exit 1

if isNull null <> true then exit 1

exit 0
