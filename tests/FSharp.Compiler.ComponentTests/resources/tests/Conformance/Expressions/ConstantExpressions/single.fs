// #Conformance #Constants 
// Constant expressions
// This is the example quoted from the specs
// Verification:
// - all the different types work
// - the type we get is what we meant it to be
#light 

let v1 = 1.0f           // float32/single 
let v2 = 1.01f          // float32/single
let v3 = 1.01e10f       // float32/single 

let check(x:single) = true

exit (if (check(v1) && check(v2) && check(v3)) then 0 else 1)
