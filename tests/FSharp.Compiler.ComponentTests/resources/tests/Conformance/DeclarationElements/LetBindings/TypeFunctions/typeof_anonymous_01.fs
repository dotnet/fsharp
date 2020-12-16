// #Regression #Conformance #DeclarationElements #LetBindings #TypeTests 
// Regression test for FSHARP1.0:2320
// Type passed to typeof<> is _
//<Expects status="success"></Expects>
#light

let _ = typeof<_>
