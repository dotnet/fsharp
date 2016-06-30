// #NoMono #CodeGen #Optimizations 
module StructUnion01

[<Struct>]
type U = U of int * int

let g1 (U(a,b)) = a + b 

let g2 u = 
    let (U(a,b)) = u
    a + b 

let g3 (x:U) = 
    match x with 
    | U(3,a) -> a 
    | U(a,b) -> a + b 

let g4 (x:U) (y: U) = 
    match x,y with 
    | U(3,a), U(5,b) -> a + b
    | U(a,b), U(c,d) -> a + b + c + d

let f1 (x:U byref) =  
    let (U(a,b)) = x 
    a + b 

let f2 (x:U byref) = 
    match x with 
    | U(a,b) -> a + b 

let f3 (x:U byref) = 
    match x with 
    | U(3,a) -> a 
    | U(a,b) -> a + b 

let f4 (x:U byref) (y: U byref) = 
    match x,y with 
    | U(3,a), U(5,b) -> a + b
    | U(a,b), U(c,d) -> a + b + c + d
