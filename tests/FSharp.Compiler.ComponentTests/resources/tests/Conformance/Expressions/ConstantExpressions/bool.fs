// #Conformance #Constants 
// Constant expressions
// This is the example quoted from the specs
// Verification:
// - all the different types work
// - the type we get is what we meant it to be
#light 

let v1 = false          // bool            (System.Boolean)
let v2 = true           // bool            (System.Boolean)

let check(x:bool) = x

exit (if ( not(check(v1)) && check(v2)) then 0 else 1)
