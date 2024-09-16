// #Regression #Conformance #TypesAndModules #Records 
// Verify error when trying to clone a non-record type


type RecType = { A : int; B : string }

let shouldErr = { [| 1 .. 100 |] with B = "" }
