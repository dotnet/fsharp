// #Regression #Conformance #PatternMatching #Constants 
#light

// Verify error when trying to use range constants


let areKnownLists l =
    match l with
    | 1 .. 2 .. 10 -> "odds"
    | 2 .. 2 .. 10 -> "evens"
    | 1 .. 10      -> "all"
    | _ -> "?"


if areKnownLists [2; 4; 6; 8; 10] <> "evens" then exit 1

let odds = 1 :: 3 :: 5 :: 7 :: 9 :: []
if areKnownLists odds <> "odds" then exit 1

let all = [10 .. -1 .. 1] |> List.rev
if areKnownLists all <> "all" then exit 1

if areKnownLists [] <> "?" then exit 1

exit 1
