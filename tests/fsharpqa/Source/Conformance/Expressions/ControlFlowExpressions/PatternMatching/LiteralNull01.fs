// #Regression #Conformance #ControlFlow 
// Regression test for FSHARP1.0:2323 (Compiler ICE when matching on literal Null)
//<Expects status="success"></Expects>
#light

[<Literal>]
let Null = null

let x = 5
let mutable res = 0

match box x with
  | Null -> res <- 1
  | _ -> res <- 0 

exit res
