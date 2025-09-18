// Expected: No warning - module at correct indentation level
module Module

type SimpleType = 
    | A of int
    | B of string
module ValidModule = 
    let x = 42
