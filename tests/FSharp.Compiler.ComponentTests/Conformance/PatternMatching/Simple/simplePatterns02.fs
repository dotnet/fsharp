// #Conformance #PatternMatching 
#light

// Test constant pattern matches
let x = 42
let _ = 
    match x with
    | 41 -> exit 1
    | 43 -> exit 1
    | 42 -> exit 0
    | _ -> exit 1

exit 1
