// #Regression #Conformance #TypesAndModules #Records 
// Verify error when trying to clone a non-record type
//<Expects id="FS1129" status="error" span="(7,39)">The type 'int \[\]' does not contain a field 'B'$</Expects>

type RecType = { A : int; B : string }

let shouldErr = { [| 1 .. 100 |] with B = "" }
