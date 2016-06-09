// #Regression #Conformance #DeclarationElements #LetBindings #TypeTests 
// Regression test for FSHARP1.0:1490
// Basic usage of typeof
//<Expected status="success"></Expects>

#light
let x = typeof<int>

exit (if (x.Name = "Int32") then 0 else 1)
