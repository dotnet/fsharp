// #Regression #Conformance #TypeInference #TypeConstraints 
#light

// Verify error associated with open type variable

(*
error FS0030: Value restriction. The value 'x' has been inferred to have generic type
        val x : '_a list ref
Either define 'x' as a simple data term, make it a function with explicit arguments or, if you do not intend for it to be generic, add a type constraint.
*)
//<Expects id="FS0030" status="error">Value restriction. The value</Expects>

let x = ref []

exit 1
