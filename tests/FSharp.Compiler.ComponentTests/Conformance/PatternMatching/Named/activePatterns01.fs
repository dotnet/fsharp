// #Conformance #PatternMatching #ActivePatterns 
#light

// Single case active pattern
let (|Double|) x = (x, x)

let test x = match x with Double y -> y

if test (1) <> (1,1) then exit 1
if test (1,1) <> ((1,1), (1,1)) then exit 1
if test ((1,1), (1,1)) <> (((1,1), (1,1)), ((1,1), (1,1))) then exit 1

exit 0


