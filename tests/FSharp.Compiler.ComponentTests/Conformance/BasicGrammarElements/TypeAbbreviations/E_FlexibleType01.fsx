// #Regression #Conformance #TypesAndModules 
// Incorrect right hand side: flexible type.
// See also FSHARP1.0:4957

/

exception E of int
type BadType = #EException      // error:
