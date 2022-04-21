// #Conformance #DeclarationElements #Import 
#light

module TestModule1 =
    
    type DiscUnion =
         | A of int
         | B of string
         | C

// DU tags should now be in scope
open TestModule1

let mutable successes = 0

let a = A 1
let _ = 
    match a with 
    | A _ -> successes <- successes + 1
             ()
    | _ -> failwith "error"

let b = B "foo"
let _ = 
    match b with
    | B _ -> successes <- successes + 1 
             ()
    | _ -> failwith "error"

let c = C
let _ =
    match c with
    | C -> successes <- successes + 1
           ()
    | _ -> failwith "error"

// Everything worked out ok
if successes = 3 then
    ()
else 
    failwith "Failed: 1"
