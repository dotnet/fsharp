// #Regression #Conformance #TypesAndModules #Records 
// Verify error when trying to clone a non-record type
//<Expects id="FS0001" status="error" span="(7,17)">This expression was expected to have type</Expects>

type RecType = { A : int; B : string }

let shouldErr = { [| 1 .. 100 |] with B = "" }
