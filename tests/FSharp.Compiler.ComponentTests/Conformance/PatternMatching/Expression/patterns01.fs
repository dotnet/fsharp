// #Conformance #PatternMatching 
#light

// Verify the first vertical bar in a pattern match is optional.

let test1 x = match x with "0" -> 0 | "1" -> 1 | _ -> -1

let test2 x =
    match x with
      "0" -> 0
    | "1" -> 1
    | _ -> -1
    
if (test1 "0") <> (test2 "0") then exit 1
if (test1 "1") <> (test2 "1") then exit 1
if (test1 "2") <> (test2 "2") then exit 1

exit 0
