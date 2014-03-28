// #Conformance #PatternMatching 
#light

// Test guarded patterns
let success =
    let x = -1
    match x with
    | even when x % 2 = 0 && x > 0  -> exit 1
    | odd when x % 2= 1 && x > 0    -> exit 1
    | lessThanZero                  -> true

if success then exit 0
exit 1
    
