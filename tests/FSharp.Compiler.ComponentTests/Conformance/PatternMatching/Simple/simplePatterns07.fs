// #Conformance #PatternMatching 
#light

// Contains sequence [6; 20; 82]
let rec containsSeq list =
    match list with
    // Base cases
    | []            -> false
    | _ :: []       -> false
    | _ :: _ :: []  -> false
    // Check
    | 6 :: 20 :: 82 :: tl -> true
    // Recurse
    | _ :: tail           -> containsSeq tail
    
if containsSeq [1 .. 100] = true then exit 1
if containsSeq [1; 2; 3; 6; 20; 82; 1; 1] = false then exit 1

exit 0
    
