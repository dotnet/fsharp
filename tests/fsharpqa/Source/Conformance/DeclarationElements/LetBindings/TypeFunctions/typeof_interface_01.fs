// #Regression #Conformance #DeclarationElements #LetBindings #TypeTests 
// Regression test for FSHARP1.0:2320
// A is an interface
#light

type A = interface
         end

let _ = typeof<A>
