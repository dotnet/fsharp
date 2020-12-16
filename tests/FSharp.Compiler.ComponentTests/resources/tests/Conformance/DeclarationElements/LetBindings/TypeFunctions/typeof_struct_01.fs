// #Regression #Conformance #DeclarationElements #LetBindings #TypeTests 
// Regression test for FSHARP1.0:2320
// A is a struct
#light

type A = struct
           val x : int
         end

let _ = typeof<A>
