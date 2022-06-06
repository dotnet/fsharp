// #Regression #Conformance #DeclarationElements #LetBindings #TypeTests 
// Regression test for FSHARP1.0:2320
// A is a class
#light

type A = class
         end

let _ = typeof<A>
