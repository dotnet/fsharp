// #Conformance #PatternMatching 
#light 

let f x y =
    match x, y with
    | (0), (0) -> false
    | (1), (0) -> true
    | _, _ -> false
    
let r1 = f 0 0
let r2 = f 1 0
let r3 = f 0 1

if r1 <> false then exit 1
if r2 <> true  then exit 1
if r3 <> false then exit 1

exit 0
