// #Conformance #ObjectOrientedTypes #Classes #TypeInference #ValueRestriction

//<Expects id="FS0030" span="(11,5-11,7)" status="error">Value restriction: The value 'x1' has an inferred generic function type    val x1: \('_a -> unit\)However, values cannot have generic type variables like '_a in "let f: '_a". You should define 'x1' as a function instead by doing one of the following:- Add an explicit parameter that is applied instead of using a partial application "let f param"- Add a unit parameter like "let f\(\)"- Write explicit type parameters like "let f<'a>"or if you do not intend for it to be generic, either:- Add an explicit type annotation like "let f : obj -> obj"- Apply arguments of non-generic types to the function value in later code for type inference like "do f\(\)"\.This error is because a let binding without parameters defines a value, not a function\. Values cannot be generic because reading a value is assumed to result in the same everywhere but generic type parameters may invalidate this assumption by enabling type-dependent results\.$</Expects>

// We expect a value restriction here. The inferred signature is:
//     val x1: (?1 -> unit)
// Here 'x1' is not a member/function. Further, looking at the signature alone, the type inference 
// variable could have  feasibly be genrealized at 'x1' (c.f. the case above, where it
// was generalized).
let f1 (x:obj) = ()
let x1 = ((); f1)
