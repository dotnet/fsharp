// #Regression #Conformance #DeclarationElements #LetBindings #TypeTests 
// Regression test for FSHARP1.0:2320
// A is an not defined
//<Expects id="FS0929" span="(7,6-7,7)" status="error">This type requires a definition</Expects>
#light

type A

let _ = typeof<A>
