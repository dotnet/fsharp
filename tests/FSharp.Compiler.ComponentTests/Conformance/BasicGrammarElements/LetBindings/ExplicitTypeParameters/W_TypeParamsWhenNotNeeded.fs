// #Regression #Conformance #DeclarationElements #LetBindings #TypeAnnotations #TypeInference #TypeConstraints 
// Verify warning when providing explicit type parameters when
// the function is defined without them.

//<Expects id="FS0686" status="warning">The method or function 'f' should not be given explicit type argument\(s\) because it does not declare its type parameters explicitly</Expects>

let f x y = (x, y)


if f<int, string> 1 "2" <> (1, "2") then failwith "Failed: 1"
