// #Conformance #PatternMatching 
#light

let rec listLength list =
    match list with
    | []            -> 0
    | [_]           -> 1
    | [_; _]        -> 2
    | [_; _; _]     -> 3
    | hd :: tl      -> 1 + listLength tl
    
if listLength [] <> 0 then exit 1
if listLength [1] <> 1 then exit 1
if listLength [1..2] <> 2 then exit 1
if listLength [1..5] <> 5 then exit 1

exit 0
