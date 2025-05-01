// #Regression #Conformance #TypesAndModules #Exceptions 
// Regression test for FSHARP1.0:3769
// Verify that Active recognizer 'Failure' matches System.Exception only 

exception E of int * int * int
exception E' of string

let e = E(1,2,3)
let e' = E'("hello")
let e'' = System.Exception("Foo")

// Expected NOT to match
let p = match e with
        | Failure(_) -> false
        | _ -> true

// Expected NOT to match
let p' = match e' with
         | Failure(_) -> false
         | _ -> true

// Expected to match           
let p'' = match e'' with
          | Failure(_) -> true
          | _ -> false

if not (p && p' && p'') then failwith "Failed: 1"
