// Expected: No warning - module at valid position
module Module

type A = 
    | CaseA of int
    | CaseB of string
module ValidModule = 
    let x = 1
