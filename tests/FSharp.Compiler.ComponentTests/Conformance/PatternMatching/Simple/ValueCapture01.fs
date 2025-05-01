// #Conformance #PatternMatching 
#light

// Test some of the semantics of value capture

// Note that pattern matching doesn't execute all pats in order, rather
// it builds a tree, so when it chooses '0' over '1', it won't reconsider
// '1'.

let test1() =
    let mutable i = 0
    match i with
    | 0 when (i <- 1; false) -> 0
    | 1 -> 1
    | _ -> -i
    
if test1() <> -1 then exit 1

exit 0
