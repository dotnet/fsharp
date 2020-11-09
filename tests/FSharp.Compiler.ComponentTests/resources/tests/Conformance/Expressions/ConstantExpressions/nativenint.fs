// #Conformance #Constants 
// Constant expressions
// This is the example quoted from the specs
// Verification:
// - all the different types work
// - the type we get is what we meant it to be
#light 

let v1 = 99999999n      // nativeint       (System.IntPtr)

let check(x:nativeint) = true

exit (if check(v1) then 0 else 1)
