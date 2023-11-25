// #Conformance #ObjectOrientedTypes #Classes #TypeInference #ValueRestriction

//<Expects status="success"></Expects>

// We expect no value restriction here. The inferred signature is:
//     val x0: ('T -> unit)
// Here the type inference variable is generalized at 'x0'.
let f0 (x:obj) = ()
let x0 = f0
