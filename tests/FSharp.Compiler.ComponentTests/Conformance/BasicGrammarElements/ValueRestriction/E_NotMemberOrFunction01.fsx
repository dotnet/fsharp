// #Conformance #ObjectOrientedTypes #Classes #TypeInference #ValueRestriction



// We expect a value restriction here. The inferred signature is:
//     val x1: (?1 -> unit)
// Here 'x1' is not a member/function. Further, looking at the signature alone, the type inference 
// variable could have  feasibly be genrealized at 'x1' (c.f. the case above, where it
// was generalized).
let f1 (x:obj) = ()
let x1 = ((); f1)