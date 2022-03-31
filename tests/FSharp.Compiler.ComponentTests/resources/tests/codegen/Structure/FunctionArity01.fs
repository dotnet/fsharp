// #NoMT #CodeGen #Interop 
#light

// Verify that suples are dropped when parameters to a function
// So x (y, z) / (x, y) z / (x, y, z) all produce a function taking three
// parameters. (Not one parameter and a tuple of two parts.)

module FunctionArity01

let a ()        = ()
let b x         = ()
let c x y       = ()
let d x y z     = ()
let e (x, y) z  = ()
let f x (y, z)  = ()
let g (x, y, z) = ()


let h (x, (y, z)) = ()
let i ((x, y), z) = ()
let j ((x, y, z)) = ()

