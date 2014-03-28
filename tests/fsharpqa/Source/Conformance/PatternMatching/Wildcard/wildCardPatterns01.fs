// #Conformance #PatternMatching 
#light

// Verify that the shape of data matched against a wildcard
// doesn't have to match.

let matchList x =
    match x with
    | fsd :: second :: tail -> false
    | last :: [] -> false
    | _ -> true

let matchTuple x =
    match x with
    | (1,2,3) | (3,4,5) -> false
    | (x,_,9) -> false
    | _ -> true
