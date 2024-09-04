// #Regression #Conformance #TypesAndModules #Records 
// Verify appropriate error with ambiguous record inference
// Regression test for FSHARP1.0:2780


module N

type Red  = { A : int }
type Blue = { X : int; Y : int }

let aBlue   = { X = 0; Y = 1 }
let unknown = { X = 0 }
