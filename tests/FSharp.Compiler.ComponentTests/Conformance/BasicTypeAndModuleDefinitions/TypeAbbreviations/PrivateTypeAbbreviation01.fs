// #Regression #Conformance #TypesAndModules 
// Regression test for FSharp1.0:5229
// Title: Allow private type abbreviations

module M = 
    type private X = int * int
    let f (x:X) = x

let g y = M.f y

type private X = int
