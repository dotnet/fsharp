// #Conformance #PatternMatching
#light

// Verify that [] works at the end of a list
let rec lengthOf x =
    match x with
    | [] -> 0
    | _ :: [] -> 1
    | _ :: _ :: [] -> 2
    | hd :: tail -> 1 + lengthOf  tail

if lengthOf [] <> 0 then exit 1
if lengthOf [1] <> 1 then exit 1
if lengthOf [1;2] <> 2 then exit 1
if lengthOf [1..10] <> 10 then exit 1

exit 0
