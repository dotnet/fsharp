// #Regression #Conformance #PatternMatching #Records 
// Verify error if two record types in a pattern match don't match
// Verify error if type of a record field is incorrect.


type R1 = { X : int; Y : int }

let testMatch x =
    match x with
        | { X = 0; Y = 0} -> true
        | { A = 0; B = 0} -> true
        | _ -> false
