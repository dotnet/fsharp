// #Conformance #PatternMatching 
#light

// Verify ability to match just a single identifier

let testTrue x = match x with true -> true
let test42 x   = match x with 42 -> true
let testEven (x : string) = match x with _ when x.Length % 2 = 0 -> true
let identity x = match x with y -> y

if (testTrue true) <> true then exit 1
if (test42 42) <> true then exit 1
if (testEven "aabbcc") <> true then exit 1
if (identity ("apple", "ORANGE")) <> ("apple", "ORANGE") then exit 1

exit 0
