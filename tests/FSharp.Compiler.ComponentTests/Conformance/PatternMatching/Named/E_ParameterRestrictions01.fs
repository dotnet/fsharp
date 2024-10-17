// #Regression #Conformance #PatternMatching #ActivePatterns 
#light

// Verify error cases for restrictions on active pattern parameters.



// Multi-Case
let rec (|MCAP|MCAP2|) (times : int) (ip : string) =
    match times with
    | 0 -> MCAP (ip + "!")
    | 1 -> MCAP2(ip + ".")
    | x -> (|MCAP|MCAP2|) (times - 1) (ip + "-")
    
let _ = match "" with MCAP  0 "!"   -> ()  | _ -> exit 1

exit 1
