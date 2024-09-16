// #Regression #Conformance #DeclarationElements #LetBindings #TypeAnnotations #TypeInference #TypeConstraints 
// Verify warning when providing explicit type parameters when
// the function is defined without them.



let f x y = (x, y)


if f<int, string> 1 "2" <> (1, "2") then failwith "Failed: 1"
