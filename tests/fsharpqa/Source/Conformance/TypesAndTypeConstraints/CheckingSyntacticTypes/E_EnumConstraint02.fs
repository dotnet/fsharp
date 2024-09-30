// #Regression #Conformance #TypeConstraints 
// Verify error when enum constraint is not satisfied
//<Expects id="FS0001" status="error">The type 'string' is not a CLI enum type$</Expects>

let isEnum (x : 'a when 'a : enum<'b>) = ()

// Works
type E1 =
   | A = 0
   | B = 1

do isEnum (E1.B)

// Fails

do isEnum ("a string")

exit 1
