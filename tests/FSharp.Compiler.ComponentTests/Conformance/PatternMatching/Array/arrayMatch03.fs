// #Conformance #PatternMatching #Arrays 
#light

// Verify ability to match against null arrays

let isNull x =
    match x with
    | [| |] -> false
    | null  -> true
    | _     -> false

if isNull [| |]         <> false then exit 1
if isNull [| 1 .. 10 |] <> false then exit 1
if isNull null          <> true  then exit 1

exit 0
