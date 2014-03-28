// #Regression #Conformance #TypesAndModules 
// Regression test for FSharp1.0:5229
// Title: Allow private type abbreviations

//<Expects id="FS1092" status="error" span="(15,10-15,13)">The type 'X' is not accessible from this code location</Expects>

module M = 
    type private X = int * int
    let f (x:X) = x

let g y = M.f y

type private X = int

let (a : M.X) = (1, 2)
let (b : X) = 100
