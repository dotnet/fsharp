// #Conformance #PatternMatching #Arrays 
#light

// Verify ability to match against arrays

let test x =
    match x with
    | [| 1 |] -> 1
    | [| 1; 2 |] -> 2
    | [| 1; 2; 3 |] -> 3
    | _ -> -1


if test [| 1 |]       <> 1 then exit 1
if test [| 1; 2 |]    <> 2 then exit 1
if test [| 1; 2; 3 |] <> 3 then exit 1

exit 0
