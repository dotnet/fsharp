// #Conformance #Constants 
// Constant expressions
// This is the example quoted from the specs
// Verification:
// - all the different types work
// - the type we get is what we meant it to be
#light 

let v = 18us           // uint16
let check(x:uint16) = true
 
exit (if check(v) then 0 else 1)
