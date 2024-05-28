module M
open System

type MyAttribute<^T>() =
    inherit Attribute()
    
[<My<int>()>]
let x = 1