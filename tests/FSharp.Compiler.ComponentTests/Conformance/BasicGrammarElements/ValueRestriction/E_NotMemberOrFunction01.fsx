// #Conformance #ObjectOrientedTypes #Classes #TypeInference #ValueRestriction

//<Expects status="error" span="(11,5-11,7)" id="FS0030">Value restriction\. The value 'x1' has been inferred to have generic type.    val x1: \('_a -> unit\)    .Either make the arguments to 'x1' explicit or, if you do not intend for it to be generic, add a type annotation\.$</Expects>

// We expect a value restriction here. The inferred signature is:
//     val x1: (?1 -> unit)
// Here 'x1' is not a member/function. Further, looking at the signature alone, the type inference 
// variable could have  feasibly be genrealized at 'x1' (c.f. the case above, where it
// was generalized).
let f1 (x:obj) = ()
let x1 = ((); f1)