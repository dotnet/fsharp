// #Conformance #PatternMatching 
#light

// Test guarded patterns
let success =
    let x = 42
    match x with
    | x when x < 40 -> exit 1
    | _ when x > 42 -> exit 1
    | x when x = 42 -> true
    | _ -> exit 1

if success then exit 0
exit 1
    
