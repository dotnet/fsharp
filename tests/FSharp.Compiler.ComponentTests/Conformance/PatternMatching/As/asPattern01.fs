// #Conformance #PatternMatching 
#light

let t1 = (1, 2)
let (x, y) as asPatResult = t1

if x <> 1 then exit 1
if y <> 2 then exit 1
if asPatResult <> (1, 2) then exit 1

exit 0
