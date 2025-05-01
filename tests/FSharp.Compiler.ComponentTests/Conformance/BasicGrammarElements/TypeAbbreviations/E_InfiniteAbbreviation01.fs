// #Regression #Conformance #TypesAndModules
// Verify error if creating a type abbreviation which results in an infinite type expression.



type X = option<X>

let test : X = None

