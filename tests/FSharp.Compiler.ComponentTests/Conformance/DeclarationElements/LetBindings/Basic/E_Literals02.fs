// #Regression #Conformance #DeclarationElements #LetBindings 


// FSB 1981, Signature must contain the literal value

// The literal value must be given in the signature as well: 
// we must be able to type check against a signature alone, 
// and literal values are relevant to things like redundant pattern match detection (you can use literals in patterns)

module M
[<Literal>]
let test1 = "a" + "b"

[<Literal>]
let test2 = 1 ||| 64