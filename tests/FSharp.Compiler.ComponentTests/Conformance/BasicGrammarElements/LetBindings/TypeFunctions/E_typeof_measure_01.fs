// #Regression #Conformance #DeclarationElements #LetBindings #TypeTests 
// Regression test for FSHARP1.0:2320
// A is a measure
//<Expects id="FS0704" span="(9,16-9,17)" status="error">Expected type, not unit-of-measure</Expects>
#light
[<Measure>]
type A

let _ = typeof<A>
