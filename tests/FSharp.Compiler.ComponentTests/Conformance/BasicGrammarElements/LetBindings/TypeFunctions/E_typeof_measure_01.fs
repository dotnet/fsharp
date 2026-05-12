// #Regression #Conformance #DeclarationElements #LetBindings #TypeTests 
// Regression test for FSHARP1.0:2320
// A is a measure


[<Measure>]
type A

let _ = typeof<A>
