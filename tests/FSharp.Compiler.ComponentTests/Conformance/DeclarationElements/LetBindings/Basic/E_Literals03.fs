// #Regression #Conformance #DeclarationElements #LetBindings 
// verify error when literal value specified in the signature file does not match actual literal value

module M
[<Literal>]
let test1 = "a" + "b"

[<Literal>]
let test2 = 1 ||| 64