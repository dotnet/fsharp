// #Regression #NoMT #Import 
// Part of regression for FSHARP1.0:4652
// Compiles to a simple executable that we'll try to reference in FSI in ReferenceExe01.fsx
module M

let f x = 1

[<EntryPoint>]
let main args =
    f 1
