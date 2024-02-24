// #Regression #Conformance #TypesAndModules 
// Incorrect right hand side: flexibe type.
// See also FSHARP1.0:4957
//<Expects id="FS0010" status="error">Unexpected infix operator in implementation file</Expects>
/

exception E of int
type BadType = #EException      // error:
