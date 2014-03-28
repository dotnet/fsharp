// #Conformance #Constants 
// Constant expressions
// This is the example quoted from the specs
// Verification:
// - all the different types work
// - the type we get is what we meant it to be
#light 


let v1 = 1.             // float/double
let v2 = 1.01           // float/double
let v3 = 1.01e10        // float/double

let check(x:double) = true

exit (if (check(v1) && check(v2) && check(v3)) then 0 else 1)
