// #Regression #Conformance #TypesAndModules #Records 
// Verify error when trying to clone a non-record type
//<Expects id="FS1129" status="error" span="(7,17)">This expression was expected to have type 'int \[\]' but here an incompatible record type was given.$</Expects>

type RecType = { A : int; B : string }

let shouldErr = { [| 1 .. 100 |] with B = "" }
