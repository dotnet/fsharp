// #Conformance #PatternMatching #Arrays 
#light

// Verify ability to match against empty arrays
let isEmpty x =
    match x with
    | [| |] -> true
    | _     -> false

if isEmpty [| |]         <> true then exit 1
if isEmpty [| 1 .. 10 |] <> false then exit 1

exit 0
