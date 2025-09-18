// Expected: Warning for type inside type
module Module

type A = 
    | A
    type NestedType = 
        | B of int
