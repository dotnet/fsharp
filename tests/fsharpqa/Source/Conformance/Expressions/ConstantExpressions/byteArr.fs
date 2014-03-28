// #Conformance #Constants 
// Constant expressions
// This is the example quoted from the specs
// Verification:
// - all the different types work
// - the type we get is what we meant it to be
#light 


let v1 = "ASCII"B  // byte[]
let v2 = ""B       // byte[]
let v3 = @"\"B     // byte[]

let check(x:byte[]) = true

// exit (if (check(v1) && check(v2) && check(v3) ) then 0 else 1)
exit (if check(v1) then 0 else 1)
