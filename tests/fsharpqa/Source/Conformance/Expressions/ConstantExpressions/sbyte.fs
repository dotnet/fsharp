// #Conformance #Constants 
// Constant expressions
// This is the example quoted from the specs
// Verification:
// - all the different types work
// - the type we get is what we meant it to be
#light 

let v = 3y           // sbyte
let check(x:sbyte) = true
 
exit (if check(v) then 0 else 1)
