// #Regression #Conformance #PatternMatching

// Verify that a '[]' is only valid at the end of a list in a cons pattern
// This is OK. Type inference will pick up that this x is a generic list of lists.

let test x  = 
    match x with
    | firstele :: secondele :: thirdele :: fourthele  -> 0//firstele + secondele
    | firstele :: [] :: thirdele -> 0//first + thirdele
    | _ -> 0
    
let test2 (x : int list) =
    match x with
    | fst :: snd :: tail -> fst + snd
    | lastElement :: [] :: [] -> 0
    | fst :: [] :: tail -> failwith "Shouldn't get here."

