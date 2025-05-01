// #Conformance #PatternMatching #PatternMatchingGuards 
#light

// Verify that and patterns must match both sides

// And patterns are pretty much only useful with active patterns...
let (|MulTwo|_|)   x = if x % 2 = 0 then Some() else None
let (|MulThree|_|) x = if x % 3 = 0 then Some() else None
let (|MulFour|_|)  x = if x % 4 = 0 then Some() else None

let mulOf234 x =
    match x with
    | MulTwo & MulThree & MulFour   -> (true,  true,   true)
    | MulTwo & MulThree             -> (true,  true,  false)
    |          MulThree & MulFour   -> (false, true,   true)
    | MulTwo &            MulFour   -> (true,  false,  true)
    | MulTwo                        -> (true,  false, false)
    |          MulThree             -> (false, true,  false)
    |                     MulFour   -> (false, false,  true)
    | _ -> (false,false,false)


for i = 1 to 100 do
    let m2,m3,m4 = mulOf234 i
    if m2 <> (i % 2 = 0) then exit 1
    if m3 <> (i % 3 = 0) then exit 1
    if m4 <> (i % 4 = 0) then exit 1

exit 0
