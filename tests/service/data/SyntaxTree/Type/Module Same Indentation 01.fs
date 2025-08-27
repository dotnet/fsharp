// Expected: No warning - module not nested
module Module

type A = 
    | CaseA of int
    | CaseB of string
    
module B = 
    let x = 42
    
type C = 
    { Field: int }
