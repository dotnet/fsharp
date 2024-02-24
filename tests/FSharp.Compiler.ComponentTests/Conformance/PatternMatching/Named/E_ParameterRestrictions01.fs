// #Regression #Conformance #PatternMatching #ActivePatterns 
#light

// Verify error cases for restrictions on active pattern parameters.
//<Expects id="FS0722" status="error">Only active patterns returning exactly one result may accept arguments</Expects>


// Multi-Case
let rec (|MCAP|MCAP2|) (times : int) (ip : string) =
    match times with
    | 0 -> MCAP (ip + "!")
    | 1 -> MCAP2(ip + ".")
    | x -> (|MCAP|MCAP2|) (times - 1) (ip + "-")
    
let _ = match "" with MCAP  0 "!"   -> ()  | _ -> exit 1

exit 1
